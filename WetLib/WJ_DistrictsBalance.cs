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
    /// Job per l'esecuzione del bilancio di distretto
    /// </summary>
    class WJ_DistrictsBalance : WetJob
    {
        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_DistrictsBalance";

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database wetnet
        /// </summary>
        WetDBConn wet_db;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_DistrictsBalance()
        {
            // Millisecondi di attesa fra le esecuzioni
            job_sleep_time = WetConfig.GetInterpolationTimeMinutes() * 60 * 1000;
        }

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Varicamento del job
        /// </summary>
        protected override void Load()
        {
            // Istanzio la connessione al database wetnet
            WetConfig cfg = new WetConfig();
            wet_db = new WetDBConn(cfg.GetWetDBDSN(), null, null, true);
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            // Controllo cold_start_conter
            if (WetEngine.cold_start_counter < 2)
                return;
            // Acquisisco tutti i distretti configurati
            DataTable districts = wet_db.ExecCustomQuery("SELECT * FROM districts");
            // Ciclo per tutti i distretti
            foreach (DataRow district in districts.Rows)
            {
                try
                {
                    // Acquisisco l'ID univoco del distretto
                    int id_district = Convert.ToInt32(district["id_districts"]);
                    // Imposto il flag di criticità del distretto
                    bool critical = false;
                    // Acquisisco la data di creazione del distretto
                    DateTime timestamp = Convert.ToDateTime(district["update_timestamp"]);
                    // Controllo la necessità di ricreare i dati del distretto
                    int reset_all_data = Convert.ToInt32(district["reset_all_data"]);
                    if (reset_all_data == id_district)
                        ResetAllData(id_district);
                    // Creo una tabella di bilancio di portata
                    DataTable balance = new DataTable();
                    balance.Columns.Add("timestamp", typeof(DateTime));
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        balance.Columns.Add("reliable", typeof(bool));
                    balance.Columns.Add("value", typeof(double));
                    balance.Columns.Add("districts_id_districts", typeof(int));
                    // Creo la tabella di bilancio energetico
                    DataTable districts_energy_profile = new DataTable();
                    districts_energy_profile.Columns.Add("timestamp", typeof(DateTime));
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        districts_energy_profile.Columns.Add("reliable", typeof(bool));
                    districts_energy_profile.Columns.Add("value", typeof(double));
                    districts_energy_profile.Columns.Add("districts_id_districts", typeof(int));
                    districts_energy_profile.PrimaryKey = new DataColumn[] { districts_energy_profile.Columns["timestamp"] };
                    // Creo una tabella di interscambio per la portata
                    DataTable xchange = new DataTable();
                    xchange.Columns.Add("timestamp", typeof(DateTime));
                    if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                        xchange.Columns.Add("reliable", typeof(bool));
                    xchange.Columns.Add("plus", typeof(double));
                    xchange.Columns.Add("minus", typeof(double));
                    xchange.PrimaryKey = new DataColumn[] { xchange.Columns["timestamp"] };
                    // Acquisisco tutte le misure configurate per il distretto, eccetto le pressioni
                    DataTable measures = wet_db.ExecCustomQuery("SELECT `measures_id_measures`, `type`, `districts_id_districts`, `sign` FROM measures_has_districts INNER JOIN measures ON measures_has_districts.measures_id_measures = measures.id_measures WHERE `districts_id_districts` = " + id_district.ToString() + " AND ((measures.type = 0) OR (measures.type = 2))");
                    measures.PrimaryKey = new DataColumn[] { measures.Columns["measures_id_measures"] };
                    // Controllo se il distretto è critico
                    DataTable measures_crit = wet_db.ExecCustomQuery("SELECT `measures_id_measures`, `districts_id_districts`, `critical` FROM measures_has_districts INNER JOIN measures ON measures_has_districts.measures_id_measures = measures.id_measures WHERE `districts_id_districts` = " + id_district.ToString());
                    foreach (DataRow measure in measures_crit.Rows)
                    {
                        int id_measure = Convert.ToInt32(measure["measures_id_measures"]);
                        bool m_critical = Convert.ToBoolean(measure["critical"]);
                        if (m_critical)
                            critical = true;                        
                        // Passo il controllo al S.O. per l'attesa
                        if (cancellation_token_source.IsCancellationRequested)
                            return;
                        Sleep();
                    }
                    // Acquisisco il timestamp dell'ultimo giorno campionato
                    DataTable tmp = wet_db.ExecCustomQuery("SELECT `timestamp` FROM data_districts WHERE `districts_id_districts` = " + id_district + " ORDER BY `timestamp` DESC LIMIT 1");
                    DateTime last;
                    if (tmp.Rows.Count == 1)
                        last = Convert.ToDateTime(tmp.Rows[0][0]);
                    else
                        last = DateTime.MinValue;
                    DateTime start;
                    if (!critical)
                    {
                        if (last > timestamp)
                            start = last;
                        else
                            start = timestamp;
                    }
                    else
                    {
                        if (last > timestamp)
                            start = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        else
                            start = timestamp;
                    }
                    // Cerco l'ultimo compione comune a tutte le misure
                    DateTime last_of_measures = start;
                    List<DateTime> lasts = new List<DateTime>();
                    foreach (DataRow measure in measures.Rows)
                    {
                        int id_measure = Convert.ToInt32(measure["measures_id_measures"]);
                        tmp = wet_db.ExecCustomQuery("SELECT `timestamp` FROM data_measures WHERE `measures_id_measures` = " + id_measure +
                            " AND `timestamp` > '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' ORDER BY `timestamp` DESC LIMIT 1");
                        if (tmp.Rows.Count == 1)
                            lasts.Add(Convert.ToDateTime(tmp.Rows[0][0]));
                        // Passo il controllo al S.O. per l'attesa
                        if (cancellation_token_source.IsCancellationRequested)
                            return;
                        Sleep();
                    }
                    if (lasts.Count == measures.Rows.Count)
                    {
                        lasts.Sort();
                        last_of_measures = lasts[0];
                    }

                    /***************************/
                    /*** Bilancio di portata ***/
                    /***************************/

                    // Acquisisco tutti i campioni delle misure maggiori di "start"
                    string query = "SELECT * FROM data_measures WHERE (";
                    for (int ii = 0; ii < measures.Rows.Count; ii++)
                    {
                        int id_measure = Convert.ToInt32(measures.Rows[ii]["measures_id_measures"]);
                        query += ("(measures_id_measures = " + id_measure + ")");
                        if (ii < (measures.Rows.Count - 1))
                            query += " OR ";
                    }
                    query += ") AND `timestamp` > '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                        "' AND `timestamp` <= '" + last_of_measures.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' ORDER BY `timestamp` ASC";
                    tmp = wet_db.ExecCustomQuery(query);
                    // Creo una tabella unita per timestamp
                    foreach (DataRow dr in tmp.Rows)
                    {
                        // Definisco la misura letta nella riga
                        int id_measure = Convert.ToInt32(dr["measures_id_measures"]);
                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                        bool reliable = true;
                        if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            reliable = Convert.ToBoolean(dr["reliable"]);
                        double value = Convert.ToDouble(dr["value"]);
                        DistrictsMeasuresSigns sign = (DistrictsMeasuresSigns)Convert.ToInt32(measures.Rows.Find(id_measure)["sign"]);
                        // Se non esiste il record di bilancio, lo creo, altrimenti prendo l'esistente                        
                        DataRow record = xchange.Rows.Find(ts);
                        if (record == null)
                        {
                            record = xchange.NewRow();
                            record["timestamp"] = ts;
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                record["reliable"] = true;
                            record["plus"] = 0.0d;
                            record["minus"] = 0.0d;
                            xchange.Rows.Add(record);
                        }
                        record = xchange.Rows.Find(ts);
                        int record_index = xchange.Rows.IndexOf(record);
                        // Popolo il record corrente
                        if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            xchange.Rows[record_index]["reliable"] = (Convert.ToBoolean(xchange.Rows[record_index]["reliable"]) ? reliable : false);
                        switch (sign)
                        {
                            case DistrictsMeasuresSigns.PLUS:
                                xchange.Rows[record_index]["plus"] = Convert.ToDouble(xchange.Rows[record_index]["plus"]) + value;
                                break;

                            case DistrictsMeasuresSigns.MINUS:
                                xchange.Rows[record_index]["minus"] = Convert.ToDouble(xchange.Rows[record_index]["minus"]) + value;
                                break;
                        }
                    }
                    // Dalla tabella di interscambio creo il bilancio di distretto
                    foreach (DataRow dr in xchange.Rows)
                    {
                        bool reliable = true;
                        // Calcolo il bilancio
                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                        if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            reliable = Convert.ToBoolean(dr["reliable"]);
                        double plus = Convert.ToDouble(dr["plus"]);
                        double minus = Convert.ToDouble(dr["minus"]);
                        double result = (plus - minus);
                        // Aggiungo la riga al profilo del distretto
                        if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            balance.Rows.Add(ts, reliable, result, id_district);
                        else
                            balance.Rows.Add(ts, result, id_district);
                    }
                    wet_db.TableInsert(balance, "data_districts");

                    /***************************/
                    /*** Bilancio energetico ***/
                    /***************************/

                    // Acquisisco tutti i campioni delle misure maggiori di start
                    query = "SELECT * FROM measures_energy_profile WHERE (";
                    for (int ii = 0; ii < measures.Rows.Count; ii++)
                    {
                        int id_measure = Convert.ToInt32(measures.Rows[ii]["measures_id_measures"]);
                        query += ("(measures_id_measures = " + id_measure + ")");
                        if (ii < (measures.Rows.Count - 1))
                            query += " OR ";
                    }
                    query += ") AND `timestamp` > '" + start.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                        "' AND `timestamp` <= '" + last_of_measures.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' ORDER BY `timestamp` ASC";
                    tmp = wet_db.ExecCustomQuery(query);
                    // Creo una tabella unita per timestamp
                    foreach (DataRow dr in tmp.Rows)
                    {
                        // Definisco la misura letta nella riga
                        int id_measure = Convert.ToInt32(dr["measures_id_measures"]);
                        DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                        bool reliable = true;
                        if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                            reliable = Convert.ToBoolean(dr["reliable"]);
                        double value = Convert.ToDouble(dr["value"]);
                        DistrictsMeasuresSigns sign = (DistrictsMeasuresSigns)Convert.ToInt32(measures.Rows.Find(id_measure)["sign"]);
                        // Se non esiste il record di bilancio, lo creo, altrimenti prendo l'esistente
                        DataRow record = districts_energy_profile.Rows.Find(ts);
                        if (record == null)
                        {
                            record = districts_energy_profile.NewRow();
                            record["timestamp"] = ts;
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                record["reliable"] = true;
                            record["value"] = 0.0d;
                            record["districts_id_districts"] = id_district;
                            districts_energy_profile.Rows.Add(record);
                        }
                        record = districts_energy_profile.Rows.Find(ts);
                        int record_index = districts_energy_profile.Rows.IndexOf(record);
                        // Popolo il record corrente e sommo i valori
                        if (sign == DistrictsMeasuresSigns.PLUS)
                        {
                            if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                districts_energy_profile.Rows[record_index]["reliable"] = (Convert.ToBoolean(districts_energy_profile.Rows[record_index]["reliable"]) ? reliable : false);
                            districts_energy_profile.Rows[record_index]["value"] = Convert.ToDouble(districts_energy_profile.Rows[record_index]["value"]) + value;
                        }
                    }
                    // Inserisco il bilancio
                    wet_db.TableInsert(districts_energy_profile, "districts_energy_profile");
                    // Aggiorno il contatore
                    if (reset_all_data == (id_district + 1))
                    {
                        // Aggiorno il campo di reset
                        wet_db.ExecCustomCommand("UPDATE districts SET `reset_all_data` = " + (id_district + 2).ToString() + " WHERE id_districts = " + id_district.ToString());
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
            if (WetEngine.cold_start_counter == 2)
                WetEngine.cold_start_counter++;                           
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Resetta tutti i valori storici del distretto
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="start_date">Data di inzio</param>
        void ResetAllData(int id_district)
        {
            try
            {
                // Inserisco il blocco su tutte le tabelle da modificare
                wet_db.ExecCustomCommand("LOCK TABLES districts WRITE, data_districts WRITE, districts_bands_history WRITE, " +
                    "districts_energy_profile WRITE, districts_energy_day_statistic WRITE, districts_statistic_profiles WRITE, " +
                    "districts_events WRITE, districts_day_statistic WRITE");
                // Elimino i profili di consumo
                wet_db.ExecCustomCommand("DELETE FROM data_districts WHERE districts_id_districts = " + id_district.ToString());
                // Elimino lo storico delle bande
                wet_db.ExecCustomCommand("DELETE FROM districts_bands_history WHERE districts_id_districts = " + id_district.ToString());
                // Elimino i profili energetici
                wet_db.ExecCustomCommand("DELETE FROM districts_energy_profile WHERE districts_id_districts = " + id_district.ToString());
                // Elimino le statistiche energetiche
                wet_db.ExecCustomCommand("DELETE FROM districts_energy_day_statistic WHERE districts_id_districts = " + id_district.ToString());
                // Elimino i profili statistici
                wet_db.ExecCustomCommand("DELETE FROM districts_statistic_profiles WHERE districts_id_districts = " + id_district.ToString());
                // Elimino gli eventi
                wet_db.ExecCustomCommand("DELETE FROM districts_events WHERE districts_id_districts = " + id_district.ToString());
                // Elimino le statistiche
                wet_db.ExecCustomCommand("DELETE FROM districts_day_statistic WHERE districts_id_districts = " + id_district.ToString());
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }
            finally
            {
                // Tolgo il blocco
                wet_db.ExecCustomCommand("UNLOCK TABLES");
                // Aggiorno il campo di reset
                wet_db.ExecCustomCommand("UPDATE districts SET `reset_all_data` = " + (id_district + 1).ToString() + " WHERE id_districts = " + id_district.ToString());
            }
        }

        #endregion
    }
}
