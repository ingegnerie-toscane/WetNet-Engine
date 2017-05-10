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
using System.Threading;

namespace WetLib
{
    /// <summary>
    /// Job per il calcolo delle statistiche dei distretti
    /// </summary>
    class WJ_Statistics : WetJob
    {
        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_Statistics";

        /// <summary>
        /// Ora in cui eseguire il controllo dei valori statistici del giorno precedente
        /// </summary>
        public const int CHECK_HOUR = 3;

        /// <summary>
        /// Giorni di correlazione
        /// </summary>
        /// <remarks>1 mese</remarks>
        public const int CORRELATION_TIME_DAYS = 30;

        /// <summary>
        /// Intervallo in ore fra le correlazioni
        /// </summary>
        public const int CORRELATION_CHECK_HOURS = 4;

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database wetnet
        /// </summary>
        WetDBConn wet_db;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Configurazione del job
        /// </summary>
        WetConfig.WJ_Statistics_Config_Struct config;

        /// <summary>
        /// Data e ora ultima esecuzione delle correlazioni
        /// </summary>
        DateTime last_correlation = DateTime.MinValue;

        /// <summary>
        /// Token di cancellazione per la correlazione
        /// </summary>
        CancellationTokenSource ct_correlation = new CancellationTokenSource();

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Statistics()
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
            config = cfg.GetWJ_Statistics_Config();
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            // Controllo che sia passata l'ora di verifica
            if (DateTime.Now.Hour < CHECK_HOUR)
                return;
            // Controllo cold_start_counter
            if (WetEngine.cold_start_counter < 3)
                return;
            // Processo la statistica sulle misure
            MeasuresStatistic();
            // Processo la statistica sui distretti
            DistrictsStatistic();
            // Controllo correlazioni
            if ((DateTime.Now - last_correlation).TotalHours > CORRELATION_CHECK_HOURS)
            {
                Task.Run(() =>
                {
                    // Correlo le misure
                    MeasuresCorrelations();
                }, ct_correlation.Token);
            }
            // Aggiorno cold_start_counter
            if (WetEngine.cold_start_counter == 3)
                WetEngine.cold_start_counter++;
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Effettua la statistica sulle misure
        /// </summary>
        void MeasuresStatistic()
        {
            try
            {
                // Acquisisco tutte le misure configurate
                DataTable measures = wet_db.ExecCustomQuery("SELECT * FROM measures");                
                foreach (DataRow measure in measures.Rows)
                {
                    try
                    {
                        // Acquisisco l'ID della misura
                        int id_measure = Convert.ToInt32(measure["id_measures"]);
                        int id_odbc_dsn = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                        // Leggo l'ultimo giorno scritto sulle statistiche giornaliere
                        DateTime first_day = DateTime.MinValue;
                        DataTable first_day_table = wet_db.ExecCustomQuery("SELECT * FROM measures_day_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " ORDER BY `day` DESC LIMIT 1");
                        if (first_day_table.Rows.Count == 1)
                            first_day = Convert.ToDateTime(first_day_table.Rows[0]["day"]);
                        // Calcolo statistiche mensili e annuali
                        if (WetDBConn.wetdb_model_version != WetDBConn.WetDBModelVersion.V1_0)
                        {
                            // Controllo di avere nelle statistiche giornaliere almeno un record per il mese corrente
                            DataTable last_day_statistic = wet_db.ExecCustomQuery("SELECT `day` FROM measures_day_statistic WHERE measures_id_measures = " + id_measure.ToString() + " ORDER BY `day` DESC LIMIT 1");
                            if (last_day_statistic.Rows.Count == 1)
                            {
                                DateTime last_day_record = Convert.ToDateTime(last_day_statistic.Rows[0]["day"]).Date;
                                if (DateTime.Now.Date == last_day_record.AddDays(1.0d))
                                {
                                    // Leggo l'ultimo giorno scritto sulle statistiche mensili e annuali
                                    DateTime update_timestamp = Convert.ToDateTime(measure["update_timestamp"]);
                                    DateTime first_month = update_timestamp.Date;
                                    DateTime first_year = update_timestamp.Date;
                                    DataTable first_month_table = wet_db.ExecCustomQuery("SELECT * FROM measures_month_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " ORDER BY `month` DESC LIMIT 1");
                                    if (first_month_table.Rows.Count == 1)
                                        first_month = Convert.ToDateTime(first_month_table.Rows[0]["month"]).Date.AddMonths(1);
                                    DataTable first_year_table = wet_db.ExecCustomQuery("SELECT * FROM measures_year_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " ORDER BY `year` DESC LIMIT 1");
                                    if (first_year_table.Rows.Count == 1)
                                        first_year = Convert.ToDateTime(first_year_table.Rows[0]["year"]).Date.AddYears(1);
                                    // Calcolo mese e anno precedenti
                                    DateTime prec_month = DateTime.Now.Date.AddMonths(-1);
                                    DateTime prec_year = DateTime.Now.Date.AddYears(-1);

                                    #region Calcolo mensile

                                    // Ciclo per il calcolo dei mesi

                                    DateTime check_limit = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                    while (first_month < check_limit)
                                    {
                                        // Acquisisco gli storici giornalieri
                                        DataTable month_statistic_records = wet_db.ExecCustomQuery("SELECT `day`, `min_night`, `min_day`, `max_day`, `avg_day` FROM measures_day_statistic WHERE `day` >= '" +
                                            first_month.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND `day` < '" +
                                            first_month.AddMonths(1).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                        // Calcolo su base mensile
                                        List<double> min_night_list = new List<double>();
                                        List<double> min_day_list = new List<double>();
                                        List<double> max_day_list = new List<double>();
                                        List<double> avg_day_list = new List<double>();
                                        foreach (DataRow dr in month_statistic_records.Rows)
                                        {
                                            if (dr["min_night"] != DBNull.Value)
                                                min_night_list.Add(Convert.ToDouble(dr["min_night"]));
                                            if (dr["min_day"] != DBNull.Value)
                                                min_day_list.Add(Convert.ToDouble(dr["min_day"]));
                                            if (dr["max_day"] != DBNull.Value)
                                                max_day_list.Add(Convert.ToDouble(dr["max_day"]));
                                            if (dr["avg_day"] != DBNull.Value)
                                                avg_day_list.Add(Convert.ToDouble(dr["avg_day"]));
                                        }
                                        double min_night = double.NaN;
                                        if (min_night_list.Count > 0)
                                            min_night = WetStatistics.GetMean(min_night_list.ToArray());
                                        double min_month = double.NaN;
                                        if (min_day_list.Count > 0)
                                            min_month = WetStatistics.GetMin(min_day_list.ToArray());
                                        double max_month = double.NaN;
                                        if (max_day_list.Count > 0)
                                            max_month = WetStatistics.GetMax(max_day_list.ToArray());
                                        double avg_month = double.NaN;
                                        if (avg_day_list.Count > 0)
                                            avg_month = WetStatistics.GetMean(avg_day_list.ToArray());
                                        // Aggiungo il record mensile
                                        DateTime record_date = new DateTime(first_month.Year, first_month.Month, DateTime.DaysInMonth(first_month.Year, first_month.Month));
                                        wet_db.ExecCustomCommand("INSERT INTO measures_month_statistic VALUES ('" +
                                            record_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'," +
                                            (double.IsNaN(min_night) ? "NULL" : min_night.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(min_month) ? "NULL" : min_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(max_month) ? "NULL" : max_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(avg_month) ? "NULL" : avg_month.ToString().Replace(",", ".")) + "," +
                                            id_measure.ToString() + ")");
                                        // Passo al mese successivo
                                        first_month = first_month.AddMonths(1);
                                    }

                                    #endregion

                                    #region Calcolo annuale

                                    // Ciclo per il calcolo degli anni
                                    check_limit = new DateTime(DateTime.Now.Year, 1, 1);
                                    while (first_year < check_limit)
                                    {
                                        // Acquisisco gli storici mensili
                                        DataTable year_statistic_records = wet_db.ExecCustomQuery("SELECT * FROM measures_month_statistic WHERE `month` >= '" +
                                            first_year.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND `month` < '" +
                                            first_year.AddYears(1).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                        // Calcolo su base annuale
                                        List<double> min_night_list = new List<double>();
                                        List<double> min_month_list = new List<double>();
                                        List<double> max_month_list = new List<double>();
                                        List<double> avg_month_list = new List<double>();
                                        foreach (DataRow dr in year_statistic_records.Rows)
                                        {
                                            if (dr["min_night"] != DBNull.Value)
                                                min_night_list.Add(Convert.ToDouble(dr["min_night"]));
                                            if (dr["min_month"] != DBNull.Value)
                                                min_month_list.Add(Convert.ToDouble(dr["min_month"]));
                                            if (dr["max_month"] != DBNull.Value)
                                                max_month_list.Add(Convert.ToDouble(dr["max_month"]));
                                            if (dr["avg_month"] != DBNull.Value)
                                                avg_month_list.Add(Convert.ToDouble(dr["avg_month"]));
                                        }
                                        double min_night = double.NaN;
                                        if (min_night_list.Count > 0)
                                            min_night = WetStatistics.GetMean(min_night_list.ToArray());
                                        double min_month = double.NaN;
                                        if (min_month_list.Count > 0)
                                            min_month = WetStatistics.GetMin(min_month_list.ToArray());
                                        double max_month = double.NaN;
                                        if (max_month_list.Count > 0)
                                            max_month = WetStatistics.GetMax(max_month_list.ToArray());
                                        double avg_month = double.NaN;
                                        if (avg_month_list.Count > 0)
                                            avg_month = WetStatistics.GetMean(avg_month_list.ToArray());
                                        // Aggiungo il record mensile
                                        DateTime record_date = new DateTime(first_year.Year, 12, 31);
                                        wet_db.ExecCustomCommand("INSERT INTO measures_year_statistic VALUES ('" +
                                            record_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'," +
                                            (double.IsNaN(min_night) ? "NULL" : min_night.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(min_month) ? "NULL" : min_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(max_month) ? "NULL" : max_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(avg_month) ? "NULL" : avg_month.ToString().Replace(",", ".")) + "," +
                                            id_measure.ToString() + ")");
                                        // Passo al mese successivo
                                        first_year = first_year.AddYears(1);
                                    }

                                    #endregion
                                }
                            }
                        }
                        // Imposto il giorno di analisi (giorno precedente)
                        DateTime yesterday = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                        if (first_day == yesterday)
                            continue;
                        // Controllo se ho almeno un campione per il giorno corrente, altrimenti esco
                        DataTable last_samples = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString() + " AND `timestamp` >= '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' ORDER BY `timestamp` LIMIT 1");
                        if (last_samples.Rows.Count == 0)
                            continue;
                        // Controllo il numero di giorni da campionare
                        first_day = first_day.AddDays(1.0d);
                        DataTable days_table = wet_db.ExecCustomQuery("SELECT DISTINCT DATE(`timestamp`) AS `date` FROM data_measures WHERE `timestamp` > '" + first_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` < '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND measures_id_measures = " + id_measure + " ORDER BY `date` ASC");
                        for (int ii = 0; ii < days_table.Rows.Count; ii++)
                        {
                            // Giorno da analizzare
                            DateTime current_day = Convert.ToDateTime(days_table.Rows[ii]["date"]);
                            // Controllo se ho un record del giorno corrente nelle statistiche, altrimenti lo aggiungo
                            DataTable current_statistics = wet_db.ExecCustomQuery("SELECT * FROM measures_day_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            if (current_statistics.Rows.Count == 0)
                            {
                                // Creo il record
                                int count = 0;
                                if (WetDBConn.wetdb_model_version == WetDBConn.WetDBModelVersion.V1_0)
                                    count = wet_db.ExecCustomCommand("INSERT INTO measures_day_statistic VALUES ('" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "', " + (WetUtility.IsHolyday(current_day) ? ((int)DayTypes.holyday).ToString() : ((int)DayTypes.workday).ToString()) + ", NULL, NULL, NULL, NULL, NULL, NULL, " + id_measure + ", " + id_odbc_dsn + ")");
                                else
                                    count = wet_db.ExecCustomCommand("INSERT INTO measures_day_statistic VALUES ('" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "', " + (WetUtility.IsHolyday(current_day) ? ((int)DayTypes.holyday).ToString() : ((int)DayTypes.workday).ToString()) + ", NULL, NULL, NULL, NULL, NULL, NULL, " + id_measure + ")");
                                if (count != 1)
                                    throw new Exception("Unattempted error while adding new measure statistic record!");
                                current_statistics = wet_db.ExecCustomQuery("SELECT * FROM measures_day_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            }
                            else
                                continue; // L'analisi statistica è già stata fatta
                            // Acquisisco la finestra per il calcolo della minima notturna
                            TimeSpan ts_min_night_start_time = (TimeSpan)measure["min_night_start_time"];
                            TimeSpan ts_min_night_stop_time = (TimeSpan)measure["min_night_stop_time"];
                            DateTime dt_min_night_start_time = new DateTime(current_day.Year, current_day.Month, current_day.Day,
                                ts_min_night_start_time.Hours, ts_min_night_start_time.Minutes, ts_min_night_start_time.Seconds);
                            DateTime dt_min_night_stop_time = new DateTime(current_day.Year, current_day.Month, current_day.Day,
                                ts_min_night_stop_time.Hours, ts_min_night_stop_time.Minutes, ts_min_night_stop_time.Seconds);
                            // Acquisisco le tre finestre per il calcolo della massima giornaliera
                            DateTime dt_max_day_start_time = new DateTime(current_day.Year, current_day.Month, current_day.Day, 0, 0, 0);
                            DateTime dt_max_day_stop_time = new DateTime(current_day.Year, current_day.Month, current_day.Day, 23, 59, 59);
                            // Calcolo la minima notturna e variabili collegate                   
                            double min_night = double.NaN;
                            DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString() + " AND (`timestamp` >= '" + dt_min_night_start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + dt_min_night_stop_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                min_night = WetStatistics.GetMean(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(min_night))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET `min_night` = " + min_night.ToString().Replace(',', '.') +
                                    " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Calcolo le massime giornaliere
                            double max_day = double.NaN;
                            dt = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString() +
                                " AND (`timestamp` >= '" + dt_max_day_start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + dt_max_day_stop_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                max_day = WetStatistics.GetMax(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(max_day))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET max_day = " + max_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Calcolo la minima giornaliera
                            double min_day = double.NaN;
                            dt = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString() + " AND (`timestamp` >= '" + current_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + current_day.Add(new TimeSpan(23, 59, 59)).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                min_day = WetStatistics.GetMin(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(min_day))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET min_day = " + min_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Calcolo la media giornaliera
                            double avg_day = double.NaN;
                            if (dt.Rows.Count > 0)
                                avg_day = WetStatistics.GetMean(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(avg_day))
                            {
                                int cnt = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET `avg_day` = " + avg_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (cnt != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Calcolo il range
                            if ((!double.IsNaN(max_day)) && (!double.IsNaN(min_day)))
                            {
                                double range = max_day - min_day;
                                int count = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET `range` = " + range.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Calcolo da deviazione standard
                            double standard_deviation = double.NaN;
                            if (dt.Rows.Count > 0)
                                standard_deviation = WetStatistics.StandardDeviation(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(standard_deviation))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE measures_day_statistic SET standard_deviation = " + standard_deviation.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND measures_id_measures = " + id_measure.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating measure statistic record!");
                            }
                            // Passo il controllo al S.O. per l'attesa
                            if (cancellation_token_source.IsCancellationRequested)
                                return;
                            Sleep();
                        }
                        // Passo il controllo al S.O. per l'attesa
                        if (cancellation_token_source.IsCancellationRequested)
                            return;
                        Sleep();
                    }
                    catch (Exception ex0)
                    {
                        WetDebug.GestException(ex0);
                    }
                }
            }
            catch (Exception ex1)
            {
                WetDebug.GestException(ex1);
            }
        }

        /// <summary>
        /// Effettua la statistica sui distretti
        /// </summary>
        void DistrictsStatistic()
        {
            try
            {                
                // Acquisisco tutti i distretti configurati
                DataTable districts = wet_db.ExecCustomQuery("SELECT * FROM districts");
                // Ciclo per tutti i distretti
                foreach (DataRow district in districts.Rows)
                {
                    try
                    {
                        // Controllo se è in corso il reset di un distretto
                        if (wet_db.IsLocked("districts"))
                            return;
                        // Acquisisco l'ID del distretto
                        int id_district = Convert.ToInt32(district["id_districts"]);
                        double alpha = Convert.ToDouble(district["alpha_emitter_exponent"]);
                        double household_night_use = Convert.ToDouble(district["household_night_use"]);
                        double not_household_night_use = Convert.ToDouble(district["not_household_night_use"]);
                        // Controllo se il distretto è critico
                        bool critical = false;
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
                        // Controllo il campo di reset
                        int reset_all_data = Convert.ToInt32(district["reset_all_data"]);
                        if ((reset_all_data >= id_district) && (reset_all_data < (id_district + 2)))
                            continue;
                        // Leggo l'ultimo giorno scritto sulle statistiche
                        DateTime first_day = DateTime.MinValue;
                        DataTable first_day_table = wet_db.ExecCustomQuery("SELECT * FROM districts_day_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " ORDER BY `day` DESC LIMIT 1");
                        if (first_day_table.Rows.Count == 1)
                        {
                            first_day = Convert.ToDateTime(first_day_table.Rows[0]["day"]);
                            if (critical)
                                first_day = first_day.Subtract(new TimeSpan(1, 0, 0, 0));
                        }
                        // Calcolo statistiche mensili e annuali
                        if (WetDBConn.wetdb_model_version != WetDBConn.WetDBModelVersion.V1_0)
                        {
                            // Controllo di avere nelle statistiche giornaliere almeno un record per il mese corrente
                            DataTable last_day_statistic = wet_db.ExecCustomQuery("SELECT `day` FROM districts_day_statistic WHERE districts_id_districts = " + id_district.ToString() + " ORDER BY `day` DESC LIMIT 1");
                            if (last_day_statistic.Rows.Count == 1)
                            {
                                DateTime last_day_record = Convert.ToDateTime(last_day_statistic.Rows[0]["day"]).Date;
                                if (DateTime.Now.Date == last_day_record.AddDays(1.0d))
                                {
                                    // Leggo l'ultimo giorno scritto sulle statistiche mensili e annuali
                                    DateTime update_timestamp = Convert.ToDateTime(district["update_timestamp"]);
                                    DateTime first_month = update_timestamp.Date;
                                    DateTime first_year = update_timestamp.Date;
                                    DataTable first_month_table = wet_db.ExecCustomQuery("SELECT * FROM districts_month_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " ORDER BY `month` DESC LIMIT 1");
                                    if (first_month_table.Rows.Count == 1)
                                        first_month = Convert.ToDateTime(first_month_table.Rows[0]["month"]).Date.AddMonths(1);
                                    DataTable first_year_table = wet_db.ExecCustomQuery("SELECT * FROM districts_year_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " ORDER BY `year` DESC LIMIT 1");
                                    if (first_year_table.Rows.Count == 1)
                                        first_year = Convert.ToDateTime(first_year_table.Rows[0]["year"]).Date.AddYears(1);
                                    // Calcolo mese e anno precedenti
                                    DateTime prec_month = DateTime.Now.Date.AddMonths(-1);
                                    DateTime prec_year = DateTime.Now.Date.AddYears(-1);

                                    #region Calcolo mensile

                                    // Ciclo per il calcolo dei mesi

                                    DateTime check_limit = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                    while (first_month < check_limit)
                                    {
                                        // Acquisisco gli storici giornalieri
                                        DataTable month_statistic_records = wet_db.ExecCustomQuery("SELECT `day`, `min_night`, `min_day`, `max_day`, `avg_day` FROM districts_day_statistic WHERE `day` >= '" +
                                            first_month.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND `day` < '" +
                                            first_month.AddMonths(1).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                        // Calcolo su base mensile
                                        List<double> min_night_list = new List<double>();
                                        List<double> min_day_list = new List<double>();
                                        List<double> max_day_list = new List<double>();
                                        List<double> avg_day_list = new List<double>();
                                        foreach (DataRow dr in month_statistic_records.Rows)
                                        {
                                            if (dr["min_night"] != DBNull.Value)
                                                min_night_list.Add(Convert.ToDouble(dr["min_night"]));
                                            if (dr["min_day"] != DBNull.Value)
                                                min_day_list.Add(Convert.ToDouble(dr["min_day"]));
                                            if (dr["max_day"] != DBNull.Value)
                                                max_day_list.Add(Convert.ToDouble(dr["max_day"]));
                                            if (dr["avg_day"] != DBNull.Value)
                                                avg_day_list.Add(Convert.ToDouble(dr["avg_day"]));
                                        }
                                        double min_night = double.NaN;
                                        if (min_night_list.Count > 0)
                                            min_night = WetStatistics.GetMean(min_night_list.ToArray());
                                        double min_month = double.NaN;
                                        if (min_day_list.Count > 0)
                                            min_month = WetStatistics.GetMin(min_day_list.ToArray());
                                        double max_month = double.NaN;
                                        if (max_day_list.Count > 0)
                                            max_month = WetStatistics.GetMax(max_day_list.ToArray());
                                        double avg_month = double.NaN;
                                        if (avg_day_list.Count > 0)
                                            avg_month = WetStatistics.GetMean(avg_day_list.ToArray());
                                        // Aggiungo il record mensile
                                        DateTime record_date = new DateTime(first_month.Year, first_month.Month, DateTime.DaysInMonth(first_month.Year, first_month.Month));
                                        wet_db.ExecCustomCommand("INSERT INTO districts_month_statistic VALUES ('" +
                                            record_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'," +
                                            (double.IsNaN(min_night) ? "NULL" : min_night.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(min_month) ? "NULL" : min_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(max_month) ? "NULL" : max_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(avg_month) ? "NULL" : avg_month.ToString().Replace(",", ".")) + "," +
                                            id_district.ToString() + ")");
                                        // Passo al mese successivo
                                        first_month = first_month.AddMonths(1);
                                    }

                                    #endregion

                                    #region Calcolo annuale

                                    // Ciclo per il calcolo degli anni
                                    check_limit = new DateTime(DateTime.Now.Year, 1, 1);
                                    while (first_year < check_limit)
                                    {
                                        // Acquisisco gli storici mensili
                                        DataTable year_statistic_records = wet_db.ExecCustomQuery("SELECT * FROM districts_month_statistic WHERE `month` >= '" +
                                            first_year.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND `month` < '" +
                                            first_year.AddYears(1).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                        // Calcolo su base annuale
                                        List<double> min_night_list = new List<double>();
                                        List<double> min_month_list = new List<double>();
                                        List<double> max_month_list = new List<double>();
                                        List<double> avg_month_list = new List<double>();
                                        foreach (DataRow dr in year_statistic_records.Rows)
                                        {
                                            if (dr["min_night"] != DBNull.Value)
                                                min_night_list.Add(Convert.ToDouble(dr["min_night"]));
                                            if (dr["min_month"] != DBNull.Value)
                                                min_month_list.Add(Convert.ToDouble(dr["min_month"]));
                                            if (dr["max_month"] != DBNull.Value)
                                                max_month_list.Add(Convert.ToDouble(dr["max_month"]));
                                            if (dr["avg_month"] != DBNull.Value)
                                                avg_month_list.Add(Convert.ToDouble(dr["avg_month"]));
                                        }
                                        double min_night = double.NaN;
                                        if (min_night_list.Count > 0)
                                            min_night = WetStatistics.GetMean(min_night_list.ToArray());
                                        double min_month = double.NaN;
                                        if (min_month_list.Count > 0)
                                            min_month = WetStatistics.GetMin(min_month_list.ToArray());
                                        double max_month = double.NaN;
                                        if (max_month_list.Count > 0)
                                            max_month = WetStatistics.GetMax(max_month_list.ToArray());
                                        double avg_month = double.NaN;
                                        if (avg_month_list.Count > 0)
                                            avg_month = WetStatistics.GetMean(avg_month_list.ToArray());
                                        // Aggiungo il record mensile
                                        DateTime record_date = new DateTime(first_year.Year, 12, 31);
                                        wet_db.ExecCustomCommand("INSERT INTO districts_year_statistic VALUES ('" +
                                            record_date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'," +
                                            (double.IsNaN(min_night) ? "NULL" : min_night.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(min_month) ? "NULL" : min_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(max_month) ? "NULL" : max_month.ToString().Replace(",", ".")) + "," +
                                            (double.IsNaN(avg_month) ? "NULL" : avg_month.ToString().Replace(",", ".")) + "," +
                                            id_district.ToString() + ")");
                                        // Passo al mese successivo
                                        first_year = first_year.AddYears(1);
                                    }

                                    #endregion
                                }
                            }
                        }
                        // Imposto il giorno di analisi (giorno precedente)
                        DateTime yesterday = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                        if (first_day == yesterday)
                            continue;
                        // Controllo se ho almeno un campione per il giorno corrente, altrimenti esco
                        DataTable last_samples = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() + " AND `timestamp` >= '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' ORDER BY `timestamp` LIMIT 1");
                        if (last_samples.Rows.Count == 0)
                            continue;
                        // Controllo se ci sono delle pressioni che siano aggiornate
                        DataTable pressure = wet_db.ExecCustomQuery("SELECT `measures_id_measures`, `type`, `districts_id_districts`, `sign` FROM measures_has_districts INNER JOIN measures ON measures_has_districts.measures_id_measures = measures.id_measures WHERE `districts_id_districts` = " + id_district.ToString() + " AND measures.type = 1");
                        bool pressure_statistics_presence = true;
                        foreach (DataRow pm in pressure.Rows)
                        {
                            // Acquisisco l'ID della misura di pressione
                            int id_measure = Convert.ToInt32(pm["measures_id_measures"]);
                            // Controllo che sia scritta la minima notturna
                            DataTable mnp_t = wet_db.ExecCustomQuery("SELECT min_night FROM measures_day_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " AND `day` = '" + DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            if (mnp_t.Rows.Count == 0)
                            {
                                pressure_statistics_presence = false;
                                break;
                            }
                            // Passo il controllo al S.O. per l'attesa
                            if (cancellation_token_source.IsCancellationRequested)
                                return;
                            Sleep();
                        }
                        // Controllo il numero di giorni da campionare
                        first_day = first_day.AddDays(1.0d);
                        DataTable days_table = wet_db.ExecCustomQuery("SELECT DISTINCT DATE(`timestamp`) AS `date` FROM data_districts WHERE `timestamp` > '" + first_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` < '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND districts_id_districts = " + id_district + " ORDER BY `date` ASC");
                        for (int ii = 0; ii < days_table.Rows.Count; ii++)
                        {
                            // Giorno da analizzare
                            DateTime current_day = Convert.ToDateTime(days_table.Rows[ii]["date"]);
                            // Controllo se ho un record del giorno corrente nelle statistiche, altrimenti lo aggiungo
                            DataTable current_statistics = wet_db.ExecCustomQuery("SELECT * FROM districts_day_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            if (current_statistics.Rows.Count == 0)
                            {
                                // Creo il record
                                int count = wet_db.ExecCustomCommand("INSERT INTO districts_day_statistic VALUES ('" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "', " + (WetUtility.IsHolyday(current_day) ? ((int)DayTypes.holyday).ToString() : ((int)DayTypes.workday).ToString()) + ", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, " + id_district + ")");
                                if (count != 1)
                                    throw new Exception("Unattempted error while adding new district statistic record!");
                                current_statistics = wet_db.ExecCustomQuery("SELECT * FROM districts_day_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            }
                            else
                                continue; // L'analisi statistica è già stata fatta
                            // Acquisisco la finestra per il calcolo della minima notturna
                            TimeSpan ts_min_night_start_time = (TimeSpan)district["min_night_start_time"];
                            TimeSpan ts_min_night_stop_time = (TimeSpan)district["min_night_stop_time"];
                            DateTime dt_min_night_start_time = new DateTime(current_day.Year, current_day.Month, current_day.Day,
                                ts_min_night_start_time.Hours, ts_min_night_start_time.Minutes, ts_min_night_start_time.Seconds);
                            DateTime dt_min_night_stop_time = new DateTime(current_day.Year, current_day.Month, current_day.Day,
                                ts_min_night_stop_time.Hours, ts_min_night_stop_time.Minutes, ts_min_night_stop_time.Seconds);
                            // Acquisisco le tre finestre per il calcolo della massima giornaliera
                            DateTime dt_max_day_start_time = new DateTime(current_day.Year, current_day.Month, current_day.Day, 0, 0, 0);
                            DateTime dt_max_day_stop_time = new DateTime(current_day.Year, current_day.Month, current_day.Day, 23, 59, 59);
                            // Calcolo la minima notturna e variabili collegate                   
                            double min_night = double.NaN;
                            double real_leakage = double.NaN;
                            double nfcu = Convert.ToDouble(district["household_night_use"]) + Convert.ToDouble(district["not_household_night_use"]);
                            DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() + " AND (`timestamp` >= '" + dt_min_night_start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + dt_min_night_stop_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                min_night = WetStatistics.GetMean(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(min_night))
                            {
                                real_leakage = min_night - nfcu;
                                double volume_real_losses = real_leakage * 3.60d * 24.0d;
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET `min_night` = " + min_night.ToString().Replace(',', '.') +
                                    ", `real_leakage` = " + real_leakage.ToString().Replace(',', '.') +
                                    ", `volume_real_losses` = " + volume_real_losses.ToString().Replace(',', '.') +
                                    " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo le massime giornaliere
                            double max_day = double.NaN;
                            dt = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() +
                                " AND (`timestamp` >= '" + dt_max_day_start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + dt_max_day_stop_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                max_day = WetStatistics.GetMax(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(max_day))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET max_day = " + max_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo la minima giornaliera
                            double min_day = double.NaN;
                            dt = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() + " AND (`timestamp` >= '" + current_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + current_day.Add(new TimeSpan(23, 59, 59)).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            if (dt.Rows.Count > 0)
                                min_day = WetStatistics.GetMin(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(min_day))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET min_day = " + min_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo la media giornaliera
                            double avg_day = double.NaN;
                            if (dt.Rows.Count > 0)
                                avg_day = WetStatistics.GetMean(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(avg_day))
                            {
                                int cnt = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET `avg_day` = " + avg_day.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (cnt != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo il range
                            if ((!double.IsNaN(max_day)) && (!double.IsNaN(min_day)))
                            {
                                double range = max_day - min_day;
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET `range` = " + range.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo da deviazione standard
                            double standard_deviation = double.NaN;
                            if (dt.Rows.Count > 0)
                                standard_deviation = WetStatistics.StandardDeviation(WetUtility.GetDoubleValuesFromColumn(dt, "value"));
                            if (!double.IsNaN(standard_deviation))
                            {
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET standard_deviation = " + standard_deviation.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo il consumo notturno ideale
                            double ideal_night_use = household_night_use + not_household_night_use;
                            int cc = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET ideal_night_use = " + ideal_night_use.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                            if (cc != 1)
                                throw new Exception("Unattempted error while updating district statistic record!");

                            // Popolo il profilo giornaliero delle perdite
                            // Tabella con il profilo delle pressioni
                            DataTable dt3 = new DataTable();
                            // Media delle medie delle pressioni minime notturne
                            double p_min_night = 0.0d;
                            // Vettore con le medie delle pressioni minime notturne
                            List<double> ps_min_night = new List<double>();
                            // Ciclo per tutte le pressioni, se presenti
                            if ((pressure.Rows.Count > 0) && pressure_statistics_presence)
                            {
                                foreach (DataRow dr in pressure.Rows)
                                {
                                    // Acquisisco l'ID della misura di pressione
                                    int id_measure = Convert.ToInt32(dr["measures_id_measures"]);
                                    // Acquisisco il profilo giornaliero della misura
                                    DataTable dt1 = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " + id_measure.ToString() + " AND (`timestamp` >= '" + current_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + current_day.Add(new TimeSpan(23, 59, 59)).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                                    if (dt3.Rows.Count == 0)
                                        dt3 = dt1;
                                    else
                                    {
                                        // Sommo i valori di tutte le pressioni per ogni campione
                                        for (int jj = 0; jj < dt1.Rows.Count; jj++)
                                        {
                                            // Aggiungo la pressione
                                            dt3.Rows[ii]["value"] = Convert.ToDouble(dt3.Rows[ii]["value"]) + Convert.ToDouble(dt1.Rows[ii]["value"]);
                                        }
                                        // Calcolo la media
                                        foreach (DataRow dr3 in dt3.Rows)
                                        {
                                            // Divido per il numero delle pressioni
                                            dr3["value"] = Convert.ToDouble(dr3["value"]) / (double)dt1.Rows.Count;
                                        }
                                    }
                                    // Acquisisco la media della pressione minima notturna
                                    DataTable dt2 = wet_db.ExecCustomQuery("SELECT min_night FROM measures_day_statistic WHERE `measures_id_measures` = " + id_measure.ToString() + " AND `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                                    if (dt2.Rows.Count > 0)
                                    {
                                        if (dt2.Rows[0][0] != DBNull.Value)
                                            ps_min_night.Add(Convert.ToDouble(dt2.Rows[0][0]));
                                    }
                                }
                                // Calcolo la media delle medie
                                p_min_night = WetStatistics.GetMean(ps_min_night.ToArray());
                            }
                            else
                            {
                                DateTime tmp_datetime = current_day;
                                // Imposto "dt3" fittizia
                                dt3.Columns.Add("timestamp");
                                dt3.Columns.Add("value");
                                DateTime sf = current_day;
                                // Acquisisco l'"average_zone_night_pressure"                            
                                double aznp = Convert.ToDouble(district["average_zone_night_pressure"]) / 10.0d;
                                // Riempio un profilo giornaliero "fittizio"                            
                                for (int jj = 0; jj < (60 / config.interpolation_time * 24); jj++)
                                {
                                    dt3.Rows.Add(tmp_datetime, aznp < 1.0d ? 1.0d : aznp);
                                    tmp_datetime = tmp_datetime.AddMinutes(config.interpolation_time);
                                }
                                // Imposto la media delle medie
                                p_min_night = aznp;
                            }
                            if ((p_min_night == 0.0d) || double.IsNaN(p_min_night))
                                p_min_night = 1.0d;
                            // Calcolo l'mnf della pressione
                            if (!double.IsNaN(min_night))
                            {
                                double mnf_pressure = min_night / (Math.Pow(10.0d * p_min_night, alpha));
                                int count = wet_db.ExecCustomCommand("UPDATE districts_day_statistic SET mnf_pressure = " + mnf_pressure.ToString().Replace(',', '.') + ", min_night_pressure = " + p_min_night.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (count != 1)
                                    throw new Exception("Unattempted error while updating district statistic record!");
                            }
                            // Calcolo il profilo
                            Dictionary<DateTime, double> loss_profile = new Dictionary<DateTime, double>();
                            Dictionary<DateTime, double> theoretical_trend = new Dictionary<DateTime, double>();
                            foreach (DataRow dr in dt3.Rows)
                            {
                                DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                                double loss = ((double.IsNaN(real_leakage) ? 0.0d : real_leakage) / Math.Pow(p_min_night, alpha)) * (Math.Pow(Convert.ToDouble(dr["value"]), alpha));
                                double val = 0.0d;
                                dt.PrimaryKey = new DataColumn[] { dt.Columns["timestamp"] };
                                DataRow drs = dt.Rows.Find(ts);
                                if (drs != null)
                                    val = Convert.ToDouble(drs["value"]);
                                double theoretical = val - loss;
                                loss_profile.Add(ts, loss);
                                theoretical_trend.Add(ts, theoretical);
                            }
                            // Compongo la stringa di inserimento
                            if (loss_profile.Count > 0)
                            {
                                string ins_str = "INSERT IGNORE INTO districts_statistic_profiles VALUES ";
                                for (int jj = 0; jj < loss_profile.Count; jj++)
                                {
                                    ins_str += "('" + loss_profile.ElementAt(jj).Key.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', " +
                                        loss_profile.ElementAt(jj).Value.ToString().Replace(',', '.') + ", " +
                                        theoretical_trend.ElementAt(jj).Value.ToString().Replace(',', '.') + ", " + id_district.ToString() + "),";
                                }
                                ins_str = ins_str.Remove(ins_str.Length - 1, 1);
                                wet_db.ExecCustomCommand(ins_str);
                            }

                            /********************************/
                            /*** Statistiche sull'energia ***/
                            /********************************/

                            // Controllo se ho un record del giorno corrente nelle statistiche, altrimenti lo aggiungo
                            current_statistics = wet_db.ExecCustomQuery("SELECT * FROM districts_energy_day_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            if (current_statistics.Rows.Count == 0)
                            {
                                // Creo il record
                                int count = wet_db.ExecCustomCommand("INSERT INTO districts_energy_day_statistic VALUES ('" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "', " + (WetUtility.IsHolyday(current_day) ? ((int)DayTypes.holyday).ToString() : ((int)DayTypes.workday).ToString()) + ", NULL, NULL, NULL, " + id_district + ")");
                                if (count != 1)
                                    throw new Exception("Unattempted error while adding new district statistic record!");
                                current_statistics = wet_db.ExecCustomQuery("SELECT * FROM districts_energy_day_statistic WHERE `districts_id_districts` = " + id_district.ToString() + " AND `day` = '" + current_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                            }
                            else
                                continue; // L'analisi statistica è già stata fatta
                            // Acquisisco il profilo energetico
                            dt = wet_db.ExecCustomQuery("SELECT * FROM districts_energy_profile WHERE `districts_id_districts` = " + id_district.ToString() + " AND (`timestamp` >= '" + current_day.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` <= '" + current_day.Add(new TimeSpan(23, 59, 59)).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "') ORDER BY `timestamp` ASC");
                            // Calcolo i valori
                            double energy_sum = 0.0d;
                            foreach (DataRow dr in dt.Rows)
                                energy_sum += Convert.ToDouble(dr["value"]);
                            double epd = energy_sum / 10.0d;
                            int upd_cnt = wet_db.ExecCustomCommand("UPDATE districts_energy_day_statistic SET epd = " + epd.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                            if (upd_cnt != 1)
                                throw new Exception("Unattempted error while updating district energy statistic record!");
                            double ied = double.NaN;
                            if (!double.IsNaN(avg_day) && (avg_day != 0.0d))
                            {
                                ied = epd / (avg_day * 3.6d * 24.0d);
                                double iela = ied * real_leakage * 3.6d * 24.0d;
                                upd_cnt = wet_db.ExecCustomCommand("UPDATE districts_energy_day_statistic SET ied = " + ied.ToString().Replace(',', '.') + ", iela = " + iela.ToString().Replace(',', '.') + " WHERE `day` = '" + current_day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' AND districts_id_districts = " + id_district.ToString());
                                if (upd_cnt != 1)
                                    throw new Exception("Unattempted error while updating district energy statistic record!");
                            }
                            // Passo il controllo al S.O. per l'attesa
                            if (cancellation_token_source.IsCancellationRequested)
                                return;
                            Sleep();
                        }
                        // Controllo se devo aggiornare il campo di reset
                        if (reset_all_data == (id_district + 2))
                        {
                            // Aggiorno il campo di reset
                            wet_db.ExecCustomCommand("UPDATE districts SET `reset_all_data` = 0 WHERE id_districts = " + id_district.ToString());
                        }
                        // Passo il controllo al S.O. per l'attesa
                        if (cancellation_token_source.IsCancellationRequested)
                            return;
                        Sleep();
                    }
                    catch (Exception ex0)
                    {
                        WetDebug.GestException(ex0);
                    }
                }
            }
            catch (Exception ex1)
            {
                WetDebug.GestException(ex1);
            }
        }

        /// <summary>
        /// Esegue le correlazioni fra le misure
        /// </summary>        
        void MeasuresCorrelations()
        {            
            WetDBConn db = null;

            try
            {
                // Carico la configurazione del DSN
                WetConfig cfg = new WetConfig();
                // Istanzio la connessione al database
                db = new WetDBConn(cfg.GetWetDBDSN(), null, null, true);
                // Controllo se la tabella esiste, se non esiste, esco
                string[] tables = db.GetTables();
                if (!tables.Any(x => x == "measures_correlations"))
                    return;
                // Imposto il timestamp di correlazione
                last_correlation = DateTime.Now;
                // Creazione della lista delle tuple
                List<Tuple<int, int>> measures_tuples = new List<Tuple<int, int>>();
                // Calcolo il primo giorno di analisi
                DateTime first_day = DateTime.Now.Subtract(new TimeSpan(CORRELATION_TIME_DAYS, 0, 0, 0));
                // Acquisisco tutte le misure configurate
                //DataTable measures = db.ExecCustomQuery("SELECT * FROM measures");
                DataTable measures = db.ExecCustomQuery("SELECT * FROM measures WHERE `description` LIKE '%FINGERPRINT%'");
                // Ciclo per tutte le misure configurate
                foreach (DataRow measure in measures.Rows)
                {
                    // Acquisisco l'id della misura
                    int id_measure = Convert.ToInt32(measure["id_measures"]);
                    try
                    {
                        // Ciclo per tutte le misure eccetto la presente
                        foreach (DataRow other_measure in measures.Rows)
                        {
                            // In caso di richiesta di cancellazione del task la processo immediatamente
                            if (ct_correlation.IsCancellationRequested)
                                return;
                            // Acquisisco l'id della misura
                            int id_other_measure = Convert.ToInt32(other_measure["id_measures"]);
                            // Se la misura è la stessa la salto
                            if (id_measure == id_other_measure)
                                continue;
                            try
                            {
                                // Controllo che la tupla non esista
                                if (measures_tuples.Any(x => x == new Tuple<int, int>(id_measure, id_other_measure) || x == new Tuple<int, int>(id_other_measure, id_measure)))
                                    continue;
                                // Calcolo la correlazione con l'indice di Bravais-Pearson notturna e giornaliera                               
                                double correlation_night = WetStatistics.GetPearsonCorrelation(id_measure, id_other_measure, DateTime.Now.Date.Subtract(new TimeSpan(2, 0, 0, 0)), DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)), 20, 7);
                                double correlation_day = WetStatistics.GetPearsonCorrelation(id_measure, id_other_measure, DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)), DateTime.Now.Date, 7, 20);
                                // Controllo l'ordine della tupla
                                int tp_type = 0;
                                DataTable tp0 = db.ExecCustomQuery("SELECT * FROM measures_correlations " +
                                    "WHERE `id_first_measure` = " + id_measure.ToString() + " AND `id_second_measure` = " + id_other_measure.ToString());
                                DataTable tp1 = db.ExecCustomQuery("SELECT * FROM measures_correlations " +
                                    "WHERE `id_first_measure` = " + id_other_measure.ToString() + " AND `id_second_measure` = " + id_measure.ToString());
                                if (tp0.Rows.Count > 1)
                                {
                                    db.ExecCustomCommand("DELETE FROM measures_correlations " +
                                        "WHERE `id_first_measure` = " + id_measure.ToString() + " AND `id_second_measure` = " + id_other_measure.ToString());
                                }
                                if (tp1.Rows.Count > 1)
                                {
                                    db.ExecCustomCommand("DELETE FROM measures_correlations " +
                                        "WHERE `id_first_measure` = " + id_other_measure.ToString() + " AND `id_second_measure` = " + id_measure.ToString());
                                }
                                if (tp1.Rows.Count == 1)
                                    tp_type = 1;
                                // Inserisco la tupla
                                string qs;
                                switch (tp_type)
                                {
                                    default:
                                    case 0:
                                        qs = "INSERT INTO measures_correlations VALUES ("
                                            + id_measure.ToString() + ", " + id_other_measure.ToString() + ", " + correlation_night.ToString().Replace(',', '.') + ", " + correlation_day.ToString().Replace(',', '.') + ") " +
                                            "ON DUPLICATE KEY UPDATE `pearson_correlation_night` = " + correlation_night.ToString().Replace(',', '.') + ", `pearson_correlation_day` = " + correlation_day.ToString().Replace(',', '.');
                                        break;

                                    case 1:
                                        qs = "INSERT INTO measures_correlations VALUES ("
                                            + id_other_measure.ToString() + ", " + id_measure.ToString() + ", " + correlation_night.ToString().Replace(',', '.') + ", " + correlation_day.ToString().Replace(',', '.') + ") " +
                                            "ON DUPLICATE KEY UPDATE `pearson_correlation_night` = " + correlation_night.ToString().Replace(',', '.') + ", `pearson_correlation_day` = " + correlation_day.ToString().Replace(',', '.');
                                        break;
                                }
                                db.ExecCustomCommand(qs);
                            }
                            catch (Exception ex0)
                            {
                                WetDebug.GestException(ex0);
                            }
                            // Aggiungo la tupla
                            measures_tuples.Add(new Tuple<int, int>(id_measure, id_other_measure));
                            // Passo il controllo al S.O.
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex1)
                    {
                        WetDebug.GestException(ex1);
                    }
                }
            }
            catch (Exception ex2)
            {
                WetDebug.GestException(ex2);
            }
        }

        #endregion
    }
}
