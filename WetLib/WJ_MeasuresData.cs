using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WetLib
{
    /// <summary>
    /// Questo job si occupa dell'importazione dei dati delle misure e della conseguente interpolazione
    /// </summary>
    sealed class WJ_MeasuresData : WetJob
    {
        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_MeasuresData";

        /// <summary>
        /// Numero massimo di record in una query
        /// </summary>
        const uint MAX_RECORDS_IN_QUERY = 65536;

        /// <summary>
        /// Tempo di attesa fra una esecuzione e la successiva = 6 minuti
        /// </summary>
        const int JOB_SLEEP_TIME_MS = 360000; 

        #endregion

        #region Strutture

        /// <summary>
        /// Struttura con le coordinate al database della misura
        /// </summary>
        struct MeasureDBCoord_Struct
        {
            /// <summary>
            /// Nome della connessione ODBC
            /// </summary>
            public string odbc_connection;

            /// <summary>
            /// Nome della tabella
            /// </summary>
            public string table_name;

            /// <summary>
            /// Nome della colonna con il timestamp
            /// </summary>
            public string timestamp_column;

            /// <summary>
            /// Nome della colonna con il valore
            /// </summary>
            public string value_column;

            /// <summary>
            /// Nome della colonna con l'indice univoco
            /// </summary>
            public string relational_id_column;

            /// <summary>
            /// Valore dell'indice relazionale
            /// </summary>
            public string relational_id_value;

            /// <summary>
            /// Tupo di valore dell'indice relazionale
            /// </summary>
            public WetDBConn.PrimaryKeyColumnTypes relational_id_type;
        }

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database wetnet
        /// </summary>
        WetDBConn wet_db;

        /// <summary>
        /// Tempo di interpolazione
        /// </summary>
        TimeSpan interpolation_time;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Configurazione del job
        /// </summary>
        readonly WetConfig.WJ_MeasuresData_Config_Struct config;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_MeasuresData()
            : base(JOB_NAME, JOB_SLEEP_TIME_MS)
        {
            // Carico la configurazione
            WetConfig cfg = new WetConfig();
            config = cfg.GetWJ_MeasuresData_Config();
            // carico il tempo di interpolazione
            interpolation_time = new TimeSpan(0, config.interpolation_time, 0);
        }

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Funzione di caricamento del job
        /// </summary>
        protected override void Load()
        {
            // Istanzio la connessione al database wetnet
            wet_db = new WetDBConn(config.wetdb_dsn, true);            
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            // Acquisisco le connessioni presenti nel database
            DataTable connections = wet_db.ExecCustomQuery("SELECT * FROM connections");
            // Acquisisco le misure presenti nel database
            DataTable measures = wet_db.ExecCustomQuery("SELECT * FROM measures");
            // Ciclo per tutte le misure
            foreach (DataRow measure in measures.Rows)
            {
                try
                {
                    // Acquisisco l'ID univoco della misura
                    int id_measure = Convert.ToInt32(measure["id_measures"]);
                    int id_odbc_dsn = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                    bool reliable = Convert.ToBoolean(measure["reliable"]);
                    double energy_specific_content = Convert.ToDouble(measure["energy_specific_content"]) * 3.6d;   // KWh/mc -> KW/(l/s)
                    // Popolo le coordinate database per la misura
                    MeasureDBCoord_Struct measure_coord;
                    int dsn_id = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                    measure_coord.odbc_connection = Convert.ToString(connections.Select("id_odbcdsn = " + dsn_id.ToString()).First()["odbc_dsn"]);
                    measure_coord.table_name = Convert.ToString(measure["table_name"]);
                    measure_coord.timestamp_column = Convert.ToString(measure["table_timestamp_column"]);
                    measure_coord.value_column = Convert.ToString(measure["table_value_column"]);
                    measure_coord.relational_id_column = Convert.ToString(measure["table_relational_id_column"]);
                    measure_coord.relational_id_value = Convert.ToString(measure["table_relational_id_value"]);
                    measure_coord.relational_id_type = (WetDBConn.PrimaryKeyColumnTypes)Convert.ToInt32(measure["table_relational_id_type"]);
                    // Istanzio la connessione al database sorgente
                    WetDBConn source_db = new WetDBConn(measure_coord.odbc_connection, false);
                    // Estraggo il timestamp dell'ultimo valore scritto nel database sorgente
                    DateTime last_source = GetLastSourceSample(source_db, measure_coord);
                    // Estraggo il timestamp dell'ultimo valore scritto nel database WetNet
                    DateTime last_dest = GetLastDestSample(id_measure);
                    // Controllo se ci sono campioni da acquisire
                    if (last_dest < last_source)
                    {
                        // Acquisisco tutti i campioni da scrivere
                        DataTable samples = source_db.ExecCustomQuery(GetBaseQueryStr(measure_coord, " `" + measure_coord.timestamp_column + "` > '" + last_dest.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'", " ORDER BY `" + measure_coord.timestamp_column + "` ASC LIMIT " + MAX_RECORDS_IN_QUERY.ToString()));
                        DataTable dest = new DataTable();
                        dest.Columns.Add("timestamp", typeof(DateTime));
                        dest.Columns.Add("reliable", typeof(bool));
                        dest.Columns.Add("value", typeof(double));
                        dest.Columns.Add("measures_id_measures", typeof(int));
                        dest.Columns.Add("measures_connections_id_odbcdsn", typeof(int));

                        /************************************************************/
                        /*** INIZIO PROCEDURA DI INTERPOLAZIONE LINEARE DEI PUNTI ***/
                        /************************************************************/

                        // Calcolo il timestamp del valore precedente
                        DateTime first = Convert.ToDateTime(samples.Rows[0][0]);
                        DateTime prec = new DateTime(first.Ticks % interpolation_time.Ticks == 0 ? first.Ticks - interpolation_time.Ticks : (first.Ticks / interpolation_time.Ticks) * interpolation_time.Ticks);

                        // Acquisisco, se presente, ultimo campione precedente a quelli acquisiti, se non esiste, il valore lo considero a zero                                                
                        DataTable tmp = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_measures WHERE `timestamp` <= '" +
                            prec.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `measures_id_measures` = " +
                            id_measure.ToString() + " ORDER BY `timestamp` DESC LIMIT 1");
                        DataRow new_row = samples.NewRow();
                        if (tmp.Rows.Count == 1)
                        {
                            new_row[0] = Convert.ToDateTime(tmp.Rows[0][0]);
                            new_row[1] = Convert.ToDouble(tmp.Rows[0][1]);
                        }
                        else
                        {
                            new_row[0] = prec;
                            new_row[1] = 0.0d;
                        }
                        samples.Rows.InsertAt(new_row, 0);

                        DateTime start = Convert.ToDateTime(samples.Rows[0][0]);
                        DateTime stop = start + interpolation_time;
                        for (int ii = 0, jj = 0; ii < samples.Rows.Count - 1; )
                        {
                            if (Convert.ToDateTime(samples.Rows[ii][0]) >= stop)
                            {
                                bool is_reliable;

                                // Interpolo
                                double y0 = samples.Rows[jj][1] == DBNull.Value ? 0.0d : Convert.ToDouble(samples.Rows[jj][1]);
                                double y1 = samples.Rows[ii][1] == DBNull.Value ? 0.0d : Convert.ToDouble(samples.Rows[ii][1]);
                                double x0 = Convert.ToDouble(Convert.ToDateTime(samples.Rows[jj][0]).Ticks);
                                double x1 = Convert.ToDouble(Convert.ToDateTime(samples.Rows[ii][0]).Ticks);
                                double x = stop.Ticks;
                                double y = (((y1 - y0) * (x - x0)) / (x1 - x0)) + y0;
                                if (double.IsNaN(y))
                                    y = 0;
                                // Calcolo l'affidabilità
                                if (stop.Date == DateTime.Now.Date)
                                    is_reliable = reliable;
                                else
                                    is_reliable = true;
                                // Aggiungo la riga
                                dest.Rows.Add(stop, is_reliable, y, id_measure, id_odbc_dsn);
                                // Aggiorno i contatori
                                if ((Convert.ToDateTime(samples.Rows[ii + 1][0]) - stop) <= interpolation_time)
                                    jj = ii++;
                                start = stop;
                                stop = start + interpolation_time;
                            }
                            else
                                ii++;
                        }

                        /**********************************************************/
                        /*** FINE PROCEDURA DI INTERPOLAZIONE LINEARE DEI PUNTI ***/
                        /**********************************************************/

                        // Inserisco i valori ottenuti nella tabella dati
                        wet_db.TableInsert(dest, "data_measures");

                        /**********************************/
                        /*** Calcolo profilo energetico ***/
                        /**********************************/

                        // Creo la tabella di appoggio
                        DataTable measures_energy_profile = new DataTable();
                        measures_energy_profile.Columns.Add("timestamp", typeof(DateTime));
                        measures_energy_profile.Columns.Add("reliable", typeof(bool));
                        measures_energy_profile.Columns.Add("value", typeof(double));
                        measures_energy_profile.Columns.Add("measures_id_measures", typeof(int));
                        measures_energy_profile.Columns.Add("measures_connections_id_odbcdsn", typeof(int));
                        // Ciclo per tutti i campioni di portata
                        foreach (DataRow dr in dest.Rows)
                        {
                            // Creo un nuovo record vuoto
                            DataRow mep_r = measures_energy_profile.NewRow();

                            // Lo popolo calcolando la potenza associata
                            mep_r["timestamp"] = dr["timestamp"];
                            mep_r["reliable"] = dr["reliable"];
                            mep_r["value"] = Convert.ToDouble(dr["value"]) * energy_specific_content;
                            mep_r["measures_id_measures"] = dr["measures_id_measures"];
                            mep_r["measures_connections_id_odbcdsn"] = dr["measures_connections_id_odbcdsn"];

                            // Lo inserisco nella tabella temporanea
                            measures_energy_profile.Rows.Add(mep_r);
                        }
                        // Inserisco i dati sul DB
                        wet_db.TableInsert(measures_energy_profile, "measures_energy_profile");
                    }
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
                Sleep(100);
            }
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce la stringa di query base per la misura
        /// </summary>
        /// <param name="measure_coord">Coordinata del database per la misura</param>
        /// <param name="where_clausole">Clausola WHERE</param>       
        /// <param name="other_clausoles">Altre clausole</param>
        /// <returns>Stringa di query</returns>
        /// <remarks>
        /// Per stringa base si intende una query compilata nelle specifiche SELECT, FROM e WHERE (solo per tabelle relazionali),
        /// con la possibilità di aggiungere parametri.
        /// </remarks>
        string GetBaseQueryStr(MeasureDBCoord_Struct measure_coord, string where_clausole, string other_clausoles)
        {
            string query;

            query = "SELECT `" + measure_coord.timestamp_column + "`, `" + measure_coord.value_column + "` FROM " + measure_coord.table_name;
            if (measure_coord.relational_id_column != string.Empty)
            {
                query += " WHERE `" + measure_coord.relational_id_column + "` = ";
                switch (measure_coord.relational_id_type)
                {
                    case WetDBConn.PrimaryKeyColumnTypes.REAL:
                        query += measure_coord.relational_id_value.Replace(',', '.');
                        break;

                    case WetDBConn.PrimaryKeyColumnTypes.DATETIME:
                        query += "'" + Convert.ToDateTime(measure_coord.relational_id_value).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'";
                        break;

                    case WetDBConn.PrimaryKeyColumnTypes.TEXT:
                        query += "'" + measure_coord.relational_id_value + "'";
                        break;

                    default:
                        query += measure_coord.relational_id_value;
                        break;
                }
                if ((where_clausole != string.Empty) && (where_clausole != null))
                    query += " AND (" + where_clausole + ")";
            }
            else
            {
                if ((where_clausole != string.Empty) && (where_clausole != null))
                    query += " WHERE " + where_clausole;
            }
            if ((other_clausoles != string.Empty) && (other_clausoles != null))
                query += " " + other_clausoles;

            return query;
        }

        /// <summary>
        /// Restituisce l'ultimo timestamp scritto nel database sorgente
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="measure_coord"></param>
        /// <returns></returns>
        DateTime GetLastSourceSample(WetDBConn connection, MeasureDBCoord_Struct measure_coord)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                DataTable dt = connection.ExecCustomQuery(GetBaseQueryStr(measure_coord, null, " ORDER BY `" + measure_coord.timestamp_column + "` DESC LIMIT 1"));
                if (dt.Rows.Count == 1)
                    ret = Convert.ToDateTime(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ret;
        }

        /// <summary>
        /// Restituisce l'ultimo timestamp della misura scritto
        /// </summary>
        /// <param name="id_measure">ID univoco della misura</param>
        /// <returns></returns>
        DateTime GetLastDestSample(int id_measure)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                DataTable dt = wet_db.ExecCustomQuery("SELECT MAX(`timestamp`) FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString());
                if (dt.Rows.Count == 1)
                    ret = Convert.ToDateTime(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ret;
        }

        #endregion
    }
}
