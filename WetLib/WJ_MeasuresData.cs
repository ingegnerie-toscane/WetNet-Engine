/****************************************************************************
 * 
 * WetLib - Common library for WetNet applications.
 * Copyright 2013-2015 Ingegnerie Toscane S.r.l.
 * 
 * This file is part of WetNet application.
 * 
 * Licensed under the EUPL, Version 1.1 or – as soon they
 * will be approved by the European Commission - subsequent
 * versions of the EUPL (the "Licence");
 * 
 * You may not use this work except in compliance with the licence.
 * You may obtain a copy of the Licence at:
 * http://ec.europa.eu/idabc/eupl
 * 
 * Unless required by applicable law or agreed to in
 * writing, software distributed under the Licence is
 * distributed on an "AS IS" basis,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied.
 * 
 * See the Licence for the specific language governing
 * permissions and limitations under the Licence.
 * 
 ***************************************************************************/

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
            /// Nome utente
            /// </summary>
            public string username;

            /// <summary>
            /// Password
            /// </summary>
            public string password;

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
        {
            // Millisecondi di attesa fra le esecuzioni
            job_sleep_time = WetConfig.GetInterpolationTimeMinutes() * 60 * 1000;
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
            wet_db = new WetDBConn(config.wetdb_dsn, null, null, true);            
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            // Acquisisco le connessioni presenti nel database
            DataTable connections = wet_db.ExecCustomQuery("SELECT * FROM connections");
            connections.PrimaryKey = new DataColumn[] { connections.Columns["id_odbcdsn"] };
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
                    MeasureTypes mtype = (MeasureTypes)Convert.ToInt32(measure["type"]);
                    bool reliable = Convert.ToBoolean(measure["reliable"]);
                    DateTime start_date = Convert.ToDateTime(measure["update_timestamp"]);
                    double energy_specific_content = Convert.ToDouble(measure["energy_specific_content"]) * 3.6d;   // KWh/mc -> KW/(l/s)
                    // Popolo le coordinate database per la misura
                    MeasureDBCoord_Struct measure_coord;
                    int dsn_id = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                    measure_coord.odbc_connection = Convert.ToString(connections.Rows.Find(dsn_id)["odbc_dsn"]);
                    measure_coord.username = (connections.Rows.Find(dsn_id)["username"] == DBNull.Value ? null : Convert.ToString(connections.Rows.Find(dsn_id)["username"]));
                    measure_coord.password = (connections.Rows.Find(dsn_id)["password"] == DBNull.Value ? null : Convert.ToString(connections.Rows.Find(dsn_id)["password"]));                    
                    measure_coord.table_name = Convert.ToString(measure["table_name"]);
                    measure_coord.timestamp_column = Convert.ToString(measure["table_timestamp_column"]);
                    measure_coord.value_column = Convert.ToString(measure["table_value_column"]);
                    measure_coord.relational_id_column = Convert.ToString(measure["table_relational_id_column"]);
                    measure_coord.relational_id_value = Convert.ToString(measure["table_relational_id_value"]);
                    measure_coord.relational_id_type = (WetDBConn.PrimaryKeyColumnTypes)Convert.ToInt32(measure["table_relational_id_type"]);
                    // Istanzio la connessione al database sorgente
                    WetDBConn source_db = new WetDBConn(measure_coord.odbc_connection, measure_coord.username, measure_coord.password, false);
                    // Estraggo il timestamp dell'ultimo valore scritto nel database sorgente
                    DateTime last_source = GetLastSourceSample(source_db, start_date, measure_coord);
                    // Estraggo il timestamp dell'ultimo valore scritto nel database WetNet
                    DateTime last_dest = GetLastDestSample(id_measure);
                    if (last_dest == DateTime.MinValue)
                        last_dest = start_date;
                    // Controllo se ci sono campioni da acquisire
                    if (last_dest < last_source)
                    {
                        // Acquisisco tutti i campioni da scrivere                        
                        DataTable samples = source_db.ExecCustomQuery(GetBaseQueryStr(source_db, measure_coord, last_dest, DateTime.Now, WetDBConn.OrderTypes.ASC, MAX_RECORDS_IN_QUERY));
                        // Gestione dei contatori volumetrici
                        if (mtype == MeasureTypes.COUNTER)
                        {                            
                            // Acquisisco il campione precedente al primo della tabella samples
                            DataTable cnt_tbl = source_db.ExecCustomQuery(GetBaseQueryStr(source_db, measure_coord, WetDBConn.START_DATE, last_dest.Subtract(new TimeSpan(0, 0, 0, 1)), WetDBConn.OrderTypes.DESC, 1));
                            // Lo inserisco nella tabella samples
                            if (cnt_tbl.Rows.Count > 0)
                            {
                                samples.ImportRow(cnt_tbl.Rows[0]);
                                DataView dv = samples.DefaultView;
                                dv.Sort = "[" + cnt_tbl.Columns[0].ColumnName + "] ASC";
                                samples = dv.ToTable();
                            }                                                                                   
                            // Creo una tabella di appoggio temporanea
                            DataTable cnt_tbl_q = samples.Clone();
                            // Ciclo per tutti i campioni di samples
                            DateTime now_dt, prec_dt;
                            double now_v, prec_v, liters, flow, seconds;
                            for (int ii = 0; ii < samples.Rows.Count; ii++)
                            {
                                if (ii == 0)
                                    continue;   // Salta il primo record

                                // Acquisisco i valori attuali e precedenti
                                prec_dt = Convert.ToDateTime(samples.Rows[ii - 1][0]);
                                now_dt = Convert.ToDateTime(samples.Rows[ii][0]);
                                prec_v = Convert.ToDouble(samples.Rows[ii - 1][1]);
                                now_v = Convert.ToDouble(samples.Rows[ii][1]);
                                // Calcolo la differenza in litri
                                seconds = (now_dt - prec_dt).TotalSeconds;
                                liters = (now_v * 1000) - (prec_v * 1000);
                                // Calcolo la portata
                                flow = liters / seconds;
                                // Popolo la tabella
                                cnt_tbl_q.Rows.Add(now_dt, flow);
                            }
                            // Assegno la tabella temporanea a 'samples'
                            samples.Clear();
                            samples = cnt_tbl_q.Copy();
                        }

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

                        // Interpolazione lineare
                        Dictionary<DateTime, double> interpolated = WetMath.LinearInterpolation(interpolation_time, 
                            WetMath.DataTable2Dictionary(samples, measure_coord.timestamp_column, measure_coord.value_column));

                        // Riconversione in tabella dati
                        for (int ii = 0; ii < interpolated.Count; ii++)
                            dest.Rows.Add(interpolated.ElementAt(ii).Key, 1, interpolated.ElementAt(ii).Value, id_measure, id_odbc_dsn);                        

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
                // Passo il controllo al S.O. per l'attesa
                if (cancellation_token_source.IsCancellationRequested)
                    return;
                Sleep();
            }
            // Aggiorno cold_start_counter
            if (WetEngine.cold_start_counter == 0)
                WetEngine.cold_start_counter++;
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce la stringa di query base per la misura
        /// </summary>
        /// <param name="connection">Connessione</param>
        /// <param name="measure_coord">Coordinata del database per la misura</param>
        /// <param name="start_date">Data di inizio nella clausola WHERE</param>
        /// <param name="stop_date">Data di fine nella clausola WHERE</param>    
        /// <param name="order">Tipo di ordinamento</param>
        /// <param name="num_records">Numero di records (0 = massimo concesso)</param>
        /// <returns>Stringa di query</returns>
        /// <remarks>
        /// Per stringa base si intende una query compilata nelle specifiche SELECT, FROM e WHERE (solo per tabelle relazionali),
        /// con la possibilità di aggiungere parametri.
        /// </remarks>
        string GetBaseQueryStr(WetDBConn connection, MeasureDBCoord_Struct measure_coord, DateTime start_date, DateTime stop_date, WetDBConn.OrderTypes order, ulong num_records)
        {
            string query;

            switch (connection.GetProvider())
            {
                default:
                    query = string.Empty;
                    break;

                case WetDBConn.ProviderType.ARCHESTRA_SQL:
                    query = "SELECT ";
                    if (num_records > 0)
                        query += "TOP " + num_records.ToString() + " ";
                    query += "Format(Datetime,'yyyy-MM-dd HH:mm:ss') AS " + measure_coord.timestamp_column + ", Format(Value, '#########0.00') AS '" + measure_coord.value_column + "' FROM " + measure_coord.table_name +
                        " WHERE History.TagName = '" + measure_coord.value_column + "'" +
                        " AND vValue IS NOT NULL " +
                        "AND (Quality = 0 OR Quality = 1) " +
                        "AND (QualityDetail = 192 OR QualityDetail = 202 OR QualityDetail = 64) " +
                        //"AND wwResolution = " + ((int)(config.interpolation_time * 60 * 1000)).ToString() + " " +
                        //"AND wwRetrievalMode = 'Cyclic' " +
                        "AND wwRetrievalMode = 'Full' " +
                        "AND DateTime > CONVERT(datetime, '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', 120) " +
                        "AND DateTime <= CONVERT(datetime, '" + stop_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', 120) ORDER BY " +
                        measure_coord.timestamp_column + (order == WetDBConn.OrderTypes.ASC ? " ASC" : " DESC");
                    break;

                case WetDBConn.ProviderType.IFIX_SQL:
                    query = "SELECT ";
                    if (num_records > 0)
                        query += "TOP " + num_records.ToString() + " ";
                    query += "* FROM OPENQUERY(IHIST,'SELECT timestamp AS " + measure_coord.timestamp_column +
                        ", value AS " + measure_coord.value_column + " FROM " + measure_coord.table_name +
                        " WHERE Tagname = " + measure_coord.value_column +
                        " AND quality = 100 AND timestamp > ''" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                        "'' AND timestamp <= ''" + stop_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'' AND samplingmode = LAB ORDER BY " +
                        measure_coord.timestamp_column + (order == WetDBConn.OrderTypes.ASC ? " ASC" : " DESC") + "')";
                    break;

                case WetDBConn.ProviderType.EXCEL:
                    query = "SELECT ";
                    if (num_records > 0)
                        query += "TOP " + num_records.ToString() + " ";
                    query += measure_coord.timestamp_column + ", " + measure_coord.value_column + " FROM " + measure_coord.table_name +
                        " WHERE (" + measure_coord.timestamp_column + " > #" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                "# AND " + measure_coord.timestamp_column + " <= #" + stop_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "#)" +
                        " ORDER BY " + measure_coord.timestamp_column + " " + (order == WetDBConn.OrderTypes.ASC ? "ASC" : "DESC");
                    break;

                case WetDBConn.ProviderType.GENERIC_MYSQL:
                    query = "SELECT `" + measure_coord.timestamp_column + "`, `" + measure_coord.value_column + "` FROM " + measure_coord.table_name +
                        " WHERE ";
                    if (measure_coord.relational_id_column != string.Empty)
                    {
                        query += "`" + measure_coord.relational_id_column + "` = ";
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
                        query += " AND ";
                    }
                    query += "(`" + measure_coord.timestamp_column + "` > '" + start_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                "' AND `" + measure_coord.timestamp_column + "` <= '" + stop_date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "')";
                    query += " ORDER BY `" + measure_coord.timestamp_column + "` " + (order == WetDBConn.OrderTypes.ASC ? "ASC" : "DESC");
                    if (num_records > 0)
                        query += " LIMIT " + num_records.ToString();
                    break;
            }

            return query;
        }

        /// <summary>
        /// Restituisce l'ultimo timestamp scritto nel database sorgente
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="start_date">Data di inizio</param>
        /// <param name="measure_coord"></param>
        /// <returns></returns>
        DateTime GetLastSourceSample(WetDBConn connection, DateTime start_date, MeasureDBCoord_Struct measure_coord)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                DataTable dt = connection.ExecCustomQuery(GetBaseQueryStr(connection, measure_coord, start_date, DateTime.Now, WetDBConn.OrderTypes.DESC, 1));
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
                {
                    if(dt.Rows[0][0] != DBNull.Value)
                        ret = Convert.ToDateTime(dt.Rows[0][0]);
                }
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
