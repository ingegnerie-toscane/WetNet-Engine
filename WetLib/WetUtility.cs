﻿/****************************************************************************
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
using System.Diagnostics;
using System.Reflection;
using System.Net;

namespace WetLib
{
    #region Strutture

    /// <summary>
    /// Struttura di un campione del trend
    /// </summary>
    struct DayTrendSample
    {
        /// <summary>
        /// Timestamp del campione
        /// </summary>
        public DateTime timestamp;

        /// <summary>
        /// Valore massimo
        /// </summary>
        public double hi_value;

        /// <summary>
        /// Valore medio
        /// </summary>
        public double avg_value;

        /// <summary>
        /// Valore minimo
        /// </summary>
        public double lo_value;
    }

    /// <summary>
    /// Struttura di un record mensile M1
    /// </summary>
    struct PI_M1_Month_Struct
    {
        /// <summary>
        /// Mese di analisi
        /// </summary>
        public DateTime month;

        /// <summary>
        /// Numero di giorni nel mese
        /// </summary>
        public int days_in_month;

        /// <summary>
        /// Giorni nell'anno trascorsi
        /// </summary>
        public int days_in_year;

        /// <summary>
        /// Media del mese corrente
        /// </summary>
        public double avg_month;

        /// <summary>
        /// Media dei giorni trascorsi da inizio anno
        /// </summary>
        public double avg_days_in_year;

        /// <summary>
        /// Volume mese corrente
        /// </summary>
        public double vol_month;

        /// <summary>
        /// Volume totale cumulato da inizio anno
        /// </summary>
        public double vol_tot;

        /// <summary>
        /// M1a calcolato da inizio anno
        /// </summary>
        public double m1a_tot;

        /// <summary>
        /// M1b calcolato da inizio anno
        /// </summary>
        public double m1b_tot;

        /// <summary>
        /// M1a mese corrente
        /// </summary>
        public double m1a_month;

        /// <summary>
        /// M1b mese corrente
        /// </summary>
        public double m1b_month;
    }

    #endregion

    /// <summary>
    /// Classe statica che incorpora una libreria di funzioni utili
    /// </summary>
    static class WetUtility
    {
        #region Funzioni del modulo

        /// <summary>
        /// Restituisce true se mi trovo in un intervallo di tempo valido per l'interpolazione
        /// </summary>
        /// <param name="dt">Timestamp da analizzare</param>
        /// <returns><c>true</c>Posso interpolare <c>false</c>Non posso interpolare</returns>
        public static bool CanIterpolate(DateTime dt)
        {
            DateTime dl_start = TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year).Start;
            DateTime dl_start_end = dl_start.AddHours(1.0);

            if ((dt >= dl_start) && (dt < dl_start_end))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Restituisce una oggetto di tipo "DateTime" con data corrente e ora estratta dalla stringa specificata
        /// </summary>
        /// <param name="time_str">Stringa con l'ora</param>
        /// <returns>Oggetto "DateTime" risultante</returns>
        /// <remarks>
        /// la stringa "time_str" deve essere nel formato HH:mm:ss
        /// </remarks>
        public static DateTime GetDateTimeFromTime(string time_str)
        {
            DateTime ret;

            string[] ss = time_str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 0)
                throw new Exception("Wrong time string format!");
            else
            {
                if (ss.Length == 1)
                    throw new Exception("Minutes in time string must be specified!");
                else
                {
                    int hours, mins, secs;
                    
                    // Imposto i valori delle ore e minuti
                    hours = Convert.ToInt32(ss[0]);
                    if ((hours < 0) && (hours > 23))
                        throw new Exception("Hours value must be between 0 and 23!");
                    mins = Convert.ToInt32(ss[1]);
                    if ((mins < 0) && (mins > 59))
                        throw new Exception("Minutes value must be between 0 and 59!");
                    if (ss.Length == 3)
                    {
                        secs = Convert.ToInt32(ss[2]);
                        if ((secs < 0) && (secs > 59))
                            throw new Exception("Seconds value must be between 0 and 59!");
                    }
                    else if (ss.Length < 3)
                        secs = 0;
                    else
                        throw new Exception("It is allowed max. 3 parameters in this string format -> \"HH:mm:ss\"");

                    // Converto nel valore da ritornare
                    ret = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, mins, secs);
                }
            }

            return ret;
        }

        /// <summary>
        /// Indica se il giorno corrente è festivo
        /// </summary>
        /// <param name="date">Data</param>
        /// <returns>Stato di festivistà</returns>
        public static bool IsHolyday(DateTime date)
        {
            bool holyday = false;

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
                holyday = true;

            return holyday;
        }        

        /// <summary>
        /// Restituisce un vettore di 'doubles' da una colonna di una tabella
        /// </summary>
        /// <param name="dt">Tabella</param>
        /// <param name="column">Nome della colonna</param>
        /// <returns>Vettore restituito</returns>
        public static double[] GetDoubleValuesFromColumn(DataTable dt, string column)
        {
            List<double> values = new List<double>();

            foreach (DataRow dr in dt.Rows)
                values.Add(Convert.ToDouble(dr[column]));

            return values.ToArray();
        }

        /// <summary>
        /// Restituisce il tipo di ingresso in base all'oggetto
        /// </summary>
        /// <param name="mtype">Tipo di oggetto</param>
        /// <returns>Tipo di ingresso</returns>
        public static InputMeterTypes GetInputTypeFromMeterType(MeterTypes mtype)
        {
            InputMeterTypes imt;

            switch (mtype)
            {
                default:
                case MeterTypes.UNKNOWN:
                    imt = InputMeterTypes.UNKNOWN;
                    break;

                case MeterTypes.MAGNETIC_FLOW_METER:
                case MeterTypes.ULTRASONIC_FLOW_METER:
                case MeterTypes.LCF_FLOW_METER:
                case MeterTypes.PRESSURE_METER:
                case MeterTypes.TANK:
                case MeterTypes.WELL:
                case MeterTypes.VALVE_REGULATION:
                case MeterTypes.MOTOR_FREQUENCY:
                    imt = InputMeterTypes.ANALOG_INPUT;
                    break;

                case MeterTypes.VOLUMETRIC_COUNTER:
                    imt = InputMeterTypes.PULSE_INPUT;
                    break;

                case MeterTypes.PUMP:
                case MeterTypes.VALVE_NO_REGULATION:
                    imt = InputMeterTypes.DIGITAL_STATE;
                    break;
            }

            return imt;
        }

        /// <summary>
        /// Restituisce il grado massimo di parallelismo
        /// </summary>
        /// <returns>Grado massimo di parallelismo</returns>
        public static int GetMaxDegreeOfParallelism()
        {
            return Environment.ProcessorCount;
        }

        /// <summary>
        /// Effettuo una stampa nel log eventi
        /// </summary>
        /// <param name="message">Messaggio da scrivere</param>
        /// <param name="type">Tipo di messaggio</param>
        public static void WriteEventLog(string message, EventLogEntryType type)
        {
            // Stampo nel log eventi
            string log_name = Assembly.GetEntryAssembly().GetName().Name;
            if (!EventLog.SourceExists(log_name))
                EventLog.CreateEventSource(log_name, log_name);            
            EventLog.WriteEntry(log_name, message, type);
        }

        /// <summary>
        /// Restituisce il profilo di un giorno specifico di un distretto
        /// </summary>
        /// <param name="id_district">ID univoco di un distretto</param>
        /// <param name="day">Giorno da estrarre</param>
        /// <param name="max_samples">Numero massimo di campioni restituiti (0 default = illimitati)</param>
        /// <returns>Dizionario con i valori</returns>
        public static Dictionary<DateTime, double> GetDistrictProfileOfDay(int id_district, DateTime day, int max_samples = -1)
        {
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            Dictionary<DateTime, double> profile = new Dictionary<DateTime, double>();

            try
            {
                // Istanzio la connessione al database wetnet
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                // Eseguo la query
                DataTable dt = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_districts WHERE `timestamp` >= '" + day.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                    "' AND `timestamp` < '" + day.AddDays(1.0).Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + 
                    "' AND `districts_id_districts` = " + id_district.ToString() + " ORDER BY `timestamp` ASC");
                // Popolo il dizionario
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                    double value = Convert.ToDouble(dr["value"]);
                    if ((max_samples <= 0) || ((max_samples > 0) && (profile.Count < max_samples)))
                        profile.Add(ts, value);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return profile;
        }

        /// <summary>
        /// Restituisce l'evento per un giorno selezionato
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="day">Giorno da estrarre</param>
        /// <returns>Evento</returns>
        public static WJ_Events.Event GetEventOfDay(int id_district, DateTime day)
        {
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            WJ_Events.Event ev = new WJ_Events.Event();

            ev.day = DateTime.MinValue;
            try
            {
                // Istanzio la connessione al database wetnet
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                // Eseguo la query
                DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM districts_events WHERE districts_id_districts = " + id_district.ToString() +
                    " AND `day` = '" + day.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                if (dt.Rows.Count == 1)
                {
                    ev.day = Convert.ToDateTime(dt.Rows[0]["day"]);
                    ev.type = (WJ_Events.EventTypes)Convert.ToInt32(dt.Rows[0]["type"]);
                    ev.measure_type = (DistrictStatisticMeasureTypes)Convert.ToInt32(dt.Rows[0]["measure_type"]);
                    ev.duration = Convert.ToInt32(dt.Rows[0]["duration"]);
                    ev.value = Convert.ToDouble(dt.Rows[0]["value"]);
                    ev.delta = Convert.ToDouble(dt.Rows[0]["delta_value"]);
                    ev.ranking = Convert.ToDouble(dt.Rows[0]["ranking"]);
                    ev.description = Convert.ToString(dt.Rows[0]["description"]);
                    ev.id_district = Convert.ToInt32(dt.Rows[0]["districts_id_districts"]);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ev;
        }

        /// <summary>
        /// Restituisce il profilo previsionale di un giorno specificato
        /// </summary>
        /// <param name="id_district">ID univoco di un distretto</param>
        /// <param name="day">Giorno previsionale</param>
        /// <param name="retro_weeks">Numero settimane precedenti su cui effettuare il calcolo</param>
        /// <returns>Dizionario con i valori</returns>
        public static DayTrendSample[] GetDayTrend(int id_district, DateTime day, int samples_in_day, int retro_weeks)
        {
            DayTrendSample[] profile = new DayTrendSample[samples_in_day];

            try
            {
                // Compongo i giorni da analizzare
                DateTime[] days = new DateTime[retro_weeks];
                for (int ii = 0; ii < retro_weeks; ii++)
                    days[ii] = day.Subtract(new TimeSpan((ii + 1) * 7, 0, 0, 0));
                // Compongo vettore bidimensionale
                double[,] vector2d = new double[retro_weeks, samples_in_day];
                for (int ii = 0; ii < retro_weeks; ii++)
                {
                    Dictionary<DateTime, double> vect = GetDistrictProfileOfDay(id_district, days[ii], samples_in_day);
                    for (int jj = 0; jj < vect.Count; jj++)
                        vector2d[ii, jj] = vect.Values.ElementAt(jj);
                }
                // Compongo i vettori di media e deviazione standard
                double[] avg_vect = new double[samples_in_day];
                double[] stddev_vect = new double[samples_in_day];
                for (int ii = 0; ii < samples_in_day; ii++)
                {
                    // Compongo il vettore verticale dei campioni giornalieri
                    double[] samples = new double[retro_weeks];
                    for (int jj = 0; jj < retro_weeks; jj++)
                        samples[jj] = vector2d[jj, ii];
                    // Calcolo media e deviazione standard
                    avg_vect[ii] = WetStatistics.GetMean(samples);
                    stddev_vect[ii] = WetStatistics.StandardDeviation(samples);
                }
                // Compongo il vettore finale
                for (int ii = 0; ii < samples_in_day; ii++)
                {
                    profile[ii].timestamp = day.Date.AddMinutes(ii * 24 * 60 / samples_in_day);
                    profile[ii].hi_value = avg_vect[ii] + (2 * stddev_vect[ii]);
                    profile[ii].avg_value = avg_vect[ii];
                    profile[ii].lo_value = avg_vect[ii] - (2 * stddev_vect[ii]);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return profile;
        }

        /// <summary>
        /// Restituisce il profilo previsionale di un giorno specificato con correlazione avarie
        /// </summary>
        /// <param name="id_district">ID univoco di un distretto</param>
        /// <param name="day">Giorno previsionale</param>
        /// <param name="retro_weeks">Numero settimane precedenti su cui effettuare il calcolo</param>
        /// <returns>Dizionario con i valori</returns>
        public static DayTrendSample[] GetDayTrendEx(int id_district, DateTime day, int samples_in_day, int retro_weeks)
        {
            DayTrendSample[] profile = new DayTrendSample[samples_in_day];
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            bool extended_mode = false;

            try
            {
                // Istanzio la connessione al database wetnet
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                // Controllo se sul distretto sono attivati gli eventi
                DataTable district = wet_db.ExecCustomQuery("SELECT `ev_enable` FROM districts WHERE id_districts = " + id_district.ToString());
                if (district.Rows.Count == 1)
                    extended_mode = Convert.ToBoolean(district.Rows[0]["ev_enable"]);
                // Compongo i giorni da analizzare
                DateTime[] days = new DateTime[retro_weeks];
                if (extended_mode)
                {
                    int rw = retro_weeks;
                    int kk = 0, ww = 0;
                    do
                    {
                        // Acquisisco il giorno
                        days[kk] = day.Subtract(new TimeSpan((ww + 1) * 7, 0, 0, 0));
                        // Controllo l'evento del giorno
                        WJ_Events.Event ev = GetEventOfDay(id_district, days[kk]);
                        if (ev.day != DateTime.MinValue)
                        {
                            if (ev.type != WJ_Events.EventTypes.OUT_OF_CONTROL)
                            {
                                rw--;
                                kk++;
                            }
                        }
                        ww++;   // Contatore totale dei cicli
                    } while ((rw > 0) && (ww <= (retro_weeks * 12)));    // Massimo limite 12 volte il numero settimane
                }
                else
                {
                    for (int ii = 0; ii < retro_weeks; ii++)
                        days[ii] = day.Subtract(new TimeSpan((ii + 1) * 7, 0, 0, 0));
                }
                // Compongo vettore bidimensionale
                double[,] vector2d = new double[retro_weeks, samples_in_day];
                for (int ii = 0; ii < retro_weeks; ii++)
                {
                    Dictionary<DateTime, double> vect = GetDistrictProfileOfDay(id_district, days[ii], samples_in_day);
                    for (int jj = 0; jj < vect.Count; jj++)
                        vector2d[ii, jj] = vect.Values.ElementAt(jj);
                }
                // Compongo i vettori di media e deviazione standard
                double[] avg_vect = new double[samples_in_day];
                double[] stddev_vect = new double[samples_in_day];
                for (int ii = 0; ii < samples_in_day; ii++)
                {
                    // Compongo il vettore verticale dei campioni giornalieri
                    double[] samples = new double[retro_weeks];
                    for (int jj = 0; jj < retro_weeks; jj++)
                        samples[jj] = vector2d[jj, ii];
                    // Calcolo media e deviazione standard
                    avg_vect[ii] = WetStatistics.GetMean(samples);
                    stddev_vect[ii] = WetStatistics.StandardDeviation(samples);
                }
                // Compongo il vettore finale
                for (int ii = 0; ii < samples_in_day; ii++)
                {
                    profile[ii].timestamp = day.Date.AddMinutes(ii * 24 * 60 / samples_in_day);
                    profile[ii].hi_value = avg_vect[ii] + (2 * stddev_vect[ii]);
                    profile[ii].avg_value = avg_vect[ii];
                    profile[ii].lo_value = avg_vect[ii] - (2 * stddev_vect[ii]);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return profile;
        }

        /// <summary>
        /// Restituisce lo stato di allarme per una misura in un intervallo di tempo specificato
        /// </summary>
        /// <param name="id_measure">ID univoco della misura</param>
        /// <param name="start_time">Inzio del periodo</param>
        /// <param name="stop_time">Fine del periodo</param>
        /// <returns>Stato di allarme</returns>
        public static bool IsMeasureInAlarm(int id_measure, DateTime start_time, DateTime stop_time)
        {
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            int event_type;
            bool is_in_alarm = false;

            try
            {
                // Istanzio la connessione al database wetnet
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                // Acquisisco lo stato degli allarmi per il periodo selezionato
                DataTable alarms = wet_db.ExecCustomQuery("SELECT * FROM measures_alarms WHERE measures_id_measures = " + id_measure.ToString() +
                    " AND `timestamp` >= '" + start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                    "' AND `timestamp` < '" + stop_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + 
                    "' ORDER BY `timestamp` DESC LIMIT 1");
                if (alarms.Rows.Count == 0)
                {
                    // Non ci sono allarmi nel periodo selezionato, reimposto la query sul periodo precedente
                    alarms = wet_db.ExecCustomQuery("SELECT * FROM measures_alarms WHERE measures_id_measures = " + id_measure.ToString() +
                    " AND `timestamp` < '" + start_time.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                    "' ORDER BY `timestamp` DESC LIMIT 1");
                    if (alarms.Rows.Count == 1)
                    {
                        event_type = Convert.ToInt32(alarms.Rows[0]["event_type"]);
                        if (event_type == (int)WJ_MeasuresAlarms.EventTypes.ALARM_ON)
                            is_in_alarm = true;
                    }
                }
                else
                    is_in_alarm = true;
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return is_in_alarm;
        }

        /// <summary>
        /// Restituisce un vettore di ID misure associate ad un distretto
        /// </summary>
        /// <param name="id_district">ID del distretto da analizzare</param>
        /// <returns>Vettore di ID misure</returns>
        public static int[] GetMeasuresInDistrict(int id_district)
        {
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            List<int> ids = new List<int>();

            try
            {
                // Istanzio la connessione al database wetnet
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                // Effetto la join fra tabelle
                DataTable measures = wet_db.ExecCustomQuery("SELECT m.id_measures FROM districts d " +
                    "INNER JOIN measures_has_districts mhd ON d.id_districts = mhd.districts_id_districts " +
                    "INNER JOIN measures m ON mhd.measures_id_measures = m.id_measures " +
                    "WHERE mhd.districts_id_districts = " + id_district.ToString() +
                    " ORDER BY m.id_measures ASC");
                foreach (DataRow dr in measures.Rows)
                    ids.Add(Convert.ToInt32(dr["id_measures"]));
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Restituisce lo stato di allarme per un distretto in un intervallo di tempo specificato
        /// </summary>
        /// <param name="id_district">ID univoco del distretto</param>
        /// <param name="start_time">Inizio del periodo</param>
        /// <param name="stop_time">Fine del periodo</param>
        /// <returns>Stato di allarme</returns>
        public static bool IsDistrictInAlarm(int id_district, DateTime start_time, DateTime stop_time)
        {
            bool is_in_alarm = false;

            try
            {
                int[] measures = GetMeasuresInDistrict(id_district);
                if (measures != null)
                {
                    foreach (int id_measure in measures)
                    {
                        if (IsMeasureInAlarm(id_measure, start_time, stop_time))
                        {
                            is_in_alarm = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return is_in_alarm;
        }

        /// <summary>
        /// Restituisce il vettore dell'indice M1a da inizio anno alla data specificata
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="date">Data di calcolo</param>
        /// <returns>Lista con le strutture dati</returns>
        public static List<PI_M1_Month_Struct> GetPI_M1(int id_district, DateTime date)
        {
            WetConfig wcfg = null;
            WetDBConn wet_db = null;
            List<PI_M1_Month_Struct> m1_list = new List<PI_M1_Month_Struct>();

            try
            {
                DateTime start_dt = new DateTime(date.Year, 1, 1);
                DateTime end_dt = new DateTime(date.Year, date.Month, date.Day).AddDays(1);
                // Leggo il vettore dei dati dal database
                wcfg = new WetConfig();
                wet_db = new WetDBConn(wcfg.GetWetDBDSN(), null, null, true);
                DataTable dt = wet_db.ExecCustomQuery("SELECT `timestamp`, `value` FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() +
                    " AND `timestamp` >= '" + start_dt.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "' AND `timestamp` < '" + end_dt.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                // Leggo i campi di lunghezza delle condotte e acqua contabilizzata
                DataTable district = wet_db.ExecCustomQuery("SELECT `length_main`, `rewarded_water` FROM districts WHERE `id_districts` = " + id_district);
                double length_main = 0.0d;
                double rewarded_water = 0.0d;
                if (district.Rows.Count == 0)
                    throw new Exception("District #" + id_district + " not found!");
                else
                {
                    length_main = WetMath.ValidateDouble(Convert.ToDouble(district.Rows[0]["length_main"]));
                    rewarded_water = WetMath.ValidateDouble(Convert.ToDouble(district.Rows[0]["rewarded_water"]));
                }
                if (length_main == 0.0)
                    throw new Exception("'length_main' must be > 0!!");
                // Sposto i valori letti in un dizionario
                Dictionary<DateTime, double> values = new Dictionary<DateTime, double>();
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime dt_row = Convert.ToDateTime(dr["timestamp"]);
                    double val_row = Convert.ToDouble(dr["value"]);
                    values.Add(dt_row, val_row);
                }
                // Numero di mesi da analizzare
                int months = date.Month;
                // Creo la lista dei giorni trascorsi da inizio anno
                List<int> days_from_start_year = new List<int>();
                // Creo la lista delle medie mensili fu finestra crescente
                List<double> avg_months = new List<double>();
                // Creo la lista dei volumi mensili
                List<double> vol_month = new List<double>();
                // Creo la lista dei volumi cumulati mensili
                List<double> vol_cum = new List<double>();
                // Creo la lista degli indici M1a cumulato
                List<double> m1a_cum = new List<double>();
                // Creo la lista degli indici M1b cumulato
                List<double> m1b_cum = new List<double>();
                // Creo lista degli indici M1a mensile
                List<double> m1a_month = new List<double>();
                // Creo lista degli indici M1b mensile
                List<double> m1b_month = new List<double>();
                // Ciclo per tutti i mesi da analizzare
                for (int month = 1; month <= months; month++)
                {
                    if (month == months)
                    {
                        // L'ultimo mese devo considerare il giorno richiesto
                        days_from_start_year.Add((date - start_dt).Days + 1);
                    }
                    else
                    {
                        // Se non sono all'ultimo mese devo considerare il mese intero
                        DateTime tmp_dt = new DateTime(date.Year, month, DateTime.DaysInMonth(date.Year, month));
                        days_from_start_year.Add((tmp_dt - start_dt).Days + 1);
                    }
                    // Calcolo la media mensile
                    avg_months.Add(values.Where(x => x.Key.Month < month + 1).Average(y => y.Value));
                    // Calcolo il volume mensile
                    vol_month.Add(values.Where(x => x.Key.Month == month).Average(y => y.Value) * 3.60d * 24.0d * DateTime.DaysInMonth(date.Year, month));
                    // Calcolo il volume mensile cumulato, se sono a Gennaio prendo solo il volume del mese, altrimenti aggiungo a quello del mese il cumulato precedente
                    if (month == 1)
                        vol_cum.Add(vol_month[month - 1]);
                    else
                        vol_cum.Add(vol_month[month - 1] + vol_cum[month - 2]);
                    // Calcolo l'indice M1a cumulato
                    m1a_cum.Add((vol_cum[month - 1] - (rewarded_water * 3.60d * 24.0d * days_from_start_year[month - 1])) / ((length_main / 1000.0d) * days_from_start_year[month - 1]));
                    // Calcolo l'indice M1b cumulato
                    m1b_cum.Add((vol_cum[month - 1] - (rewarded_water * 3.60d * 24.0d * days_from_start_year[month - 1])) / vol_cum[month - 1]);
                    // Calcolo l'indice M1a mensile
                    m1a_month.Add((vol_month[month - 1] - (rewarded_water * 3.60d * 24.0d * DateTime.DaysInMonth(date.Year, month))) / ((length_main / 1000.0d) * DateTime.DaysInMonth(date.Year, month)));
                    // Calcolo l'indice M1b mensile
                    m1b_month.Add((vol_month[month - 1] - (rewarded_water * 3.60d * 24.0d * DateTime.DaysInMonth(date.Year, month))) / vol_month[month - 1]);

                    // Popolo la struttura
                    PI_M1_Month_Struct m1;
                    m1.month = new DateTime(date.Year, month, DateTime.DaysInMonth(date.Year, month));  // Uso sempre l'ultimo giorno del mese
                    m1.days_in_month = DateTime.DaysInMonth(date.Year, month);
                    m1.days_in_year = days_from_start_year[month - 1];
                    m1.avg_month = values.Where(x => x.Key.Month == month).Average(y => y.Value);
                    m1.avg_days_in_year = avg_months[month - 1];
                    m1.vol_month = vol_month[month - 1];
                    m1.vol_tot = vol_cum[month - 1];
                    m1.m1a_month = m1a_month[month - 1];
                    m1.m1b_month = m1b_month[month - 1];
                    m1.m1a_tot = m1a_cum[month - 1];
                    m1.m1b_tot = m1b_cum[month - 1];
                    m1_list.Add(m1);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return m1_list;
        }

        #endregion
    }
}
