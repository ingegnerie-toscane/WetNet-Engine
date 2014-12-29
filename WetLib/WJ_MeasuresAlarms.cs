using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WetLib
{
    /// <summary>
    /// Job per la gestione degli allarmi sulle misure
    /// </summary>
    sealed class WJ_MeasuresAlarms : WetJob
    {
        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_MeasuresAlarms";

        /// <summary>
        /// Tempo di attesa fra una esecuzione e la successiva = 6 minuti
        /// </summary>
        const int JOB_SLEEP_TIME_MS = 360000;

        #endregion

        #region Enumerazioni

        /// <summary>
        /// Tipo di allarmi
        /// </summary>
        public enum AlarmTypes : int
        {
            /// <summary>
            /// Allarme non valido, evento inatteso
            /// </summary>
            INVALID = 0,

            /// <summary>
            /// Allarme superamento soglia massima
            /// </summary>
            EXCEEDED_MAX_THRESHOLD = 1,

            /// <summary>
            /// Allarme superamento soglia minima
            /// </summary>
            EXCEEDED_MIN_THRESHOLD = 2,

            /// <summary>
            /// Allarme valore intermedio costante
            /// </summary>
            CONSTANT_INTERMEDIATE_VALUE = 3,

            /// <summary>
            /// mancanza di dati
            /// </summary>
            NO_DATA_AVAILABLE = 4
        }

        /// <summary>
        /// Tipo di evento
        /// </summary>
        public enum EventTypes : int
        {
            /// <summary>
            /// Sconosciuto, inatteso
            /// </summary>
            UNKNOWN = 0,

            /// <summary>
            /// Attivazione allarme
            /// </summary>
            ALARM_ON = 1,

            /// <summary>
            /// Disattivazione allarme
            /// </summary>
            ALARM_OFF = 2
        }

        #endregion

        #region Strutture

        /// <summary>
        /// Struttura dati di un allarme
        /// </summary>
        public struct AlarmStruct
        {
            /// <summary>
            /// Timestamp dell'allarme
            /// </summary>
            public DateTime timestamp;

            /// <summary>
            /// Tipo di allarme occorso
            /// </summary>
            public AlarmTypes alarm_type;

            /// <summary>
            /// Tipo di evento di allarme
            /// </summary>
            public EventTypes event_type;

            /// <summary>
            /// Valore della misura al momento dell'allarme
            /// </summary>
            public double alarm_value;

            /// <summary>
            /// Valore di riferimento (solo per eventi a soglie, altrimenti NaN)
            /// </summary>
            public double reference_value;

            /// <summary>
            /// Lunghezza dell'evento fra condizione ON e OFF (valido per OFF)
            /// </summary>
            public TimeSpan duration;

            /// <summary>
            /// ID della misura
            /// </summary>
            public int id_measure;

            /// <summary>
            /// ID della connessione
            /// </summary>
            public int id_odbcdsn;
        }

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
        public WJ_MeasuresAlarms()
            : base(JOB_NAME, JOB_SLEEP_TIME_MS)
        {
        }

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Caricamento del job
        /// </summary>
        protected override void Load()
        {
            WetConfig cfg = new WetConfig();
            wet_db = new WetDBConn(cfg.GetWetDBDSN(), true);
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            try
            {
                // Acquisizione di tutte le misure configurate
                DataTable measures = wet_db.ExecCustomQuery("SELECT * FROM measures");
                // Ciclo per tutte le misure
                foreach (DataRow measure in measures.Rows)
                {
                    // Acquisisco l'id della misura
                    int id_measure = Convert.ToInt32(measure["id_measures"]);
                    int id_odbcdsn = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                    // Acquisisco le variabili di allarme
                    bool alarm_ths_enable = Convert.ToBoolean(Convert.ToInt32(measure["alarm_thresholds_enable"]));
                    double min_th = Convert.ToDouble(measure["alarm_min_threshold"]);
                    double max_th = Convert.ToDouble(measure["alarm_max_threshold"]);
                    bool alarm_cc_enable = Convert.ToBoolean(Convert.ToInt32(measure["alarm_constant_check_enable"]));
                    double hyst = Convert.ToDouble(measure["alarm_constant_hysteresis"]);
                    int check_time = Convert.ToInt32(measure["alarm_constant_check_time"]);
                    // Creo una struttura allarme fittizia
                    AlarmStruct alarm;
                    alarm.timestamp = DateTime.Now;
                    alarm.alarm_type = AlarmTypes.INVALID;
                    alarm.event_type = EventTypes.UNKNOWN;
                    alarm.alarm_value = 0.0d;
                    alarm.reference_value = 0.0d;
                    alarm.duration = new TimeSpan();
                    alarm.id_measure = id_measure;
                    alarm.id_odbcdsn = id_odbcdsn;
                    // Leggo l'ultimo allarme presente
                    AlarmStruct last_alarm = ReadLastAlarmDay(wet_db, id_measure, id_odbcdsn, DateTime.Now);
                    if (check_time > 0)
                    {                        
                        // Prendo le ultime ore di funzionamento
                        DateTime first = DateTime.Now.Subtract(new TimeSpan(check_time, 0, 0));
                        DateTime last = DateTime.Now;
                        // Effettuo la query sui dati
                        DataTable measure_data = wet_db.ExecCustomQuery("SELECT * FROM data_measures WHERE `measures_id_measures` = " +
                            id_measure.ToString() + " AND `timestamp` >= '" + first.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                            "' AND `timestamp` <= '" + last.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                        // Vettorizzo i dati
                        List<double> vals = new List<double>();
                        foreach (DataRow dr in measure_data.Rows)
                            vals.Add(Convert.ToDouble(dr["value"]));
                        // Controllo che ci sia almeno un valore, altrimenti imposto un allarme per dati mancanti
                        if (vals.Count == 0)
                        {
                            if ((last_alarm.alarm_type != AlarmTypes.NO_DATA_AVAILABLE) || (last_alarm.event_type != EventTypes.ALARM_ON))
                            {
                                // Controllo se c'è un altro allarme pendente e lo disattivo
                                if (last_alarm.event_type == EventTypes.ALARM_ON)
                                {
                                    last_alarm.event_type = EventTypes.ALARM_OFF;
                                    last_alarm.duration = DateTime.Now - last_alarm.timestamp;
                                    last_alarm.timestamp = DateTime.Now;
                                    // Inserisco la disattivazione
                                    InsertAlarm(last_alarm);
                                }
                                // Compongo l'allarme
                                alarm.alarm_type = AlarmTypes.NO_DATA_AVAILABLE;
                                alarm.event_type = EventTypes.ALARM_ON;
                            }
                            else
                                alarm.event_type = EventTypes.ALARM_ON;
                        }
                        else
                        {
                            if (alarm_ths_enable || alarm_cc_enable)
                            {
                                // Controllo sui massimi e minimi costanti
                                if ((alarm_ths_enable) && (max_th > min_th))
                                {
                                    if ((vals.Min() < min_th) || (vals.Max() > max_th))
                                    {
                                        if (vals.Max() > max_th)
                                        {
                                            if ((last_alarm.alarm_type != AlarmTypes.EXCEEDED_MAX_THRESHOLD) || (last_alarm.event_type != EventTypes.ALARM_ON))
                                            {
                                                // Controllo se c'è un altro allarme pendente e lo disattivo
                                                if (last_alarm.event_type == EventTypes.ALARM_ON)
                                                {
                                                    last_alarm.event_type = EventTypes.ALARM_OFF;
                                                    last_alarm.duration = DateTime.Now - last_alarm.timestamp;
                                                    last_alarm.timestamp = DateTime.Now;
                                                    // Inserisco la disattivazione
                                                    InsertAlarm(last_alarm);
                                                }
                                                // Compongo l'allarme
                                                alarm.alarm_type = AlarmTypes.EXCEEDED_MAX_THRESHOLD;
                                                alarm.event_type = EventTypes.ALARM_ON;
                                                alarm.alarm_value = vals.Max();
                                                alarm.reference_value = max_th;
                                            }
                                            else
                                                alarm.event_type = EventTypes.ALARM_ON;
                                        }
                                        if (vals.Min() < min_th)
                                        {
                                            if ((last_alarm.alarm_type != AlarmTypes.EXCEEDED_MIN_THRESHOLD) || (last_alarm.event_type != EventTypes.ALARM_ON))
                                            {
                                                // Controllo se c'è un altro allarme pendente e lo disattivo
                                                if (last_alarm.event_type == EventTypes.ALARM_ON)
                                                {
                                                    last_alarm.event_type = EventTypes.ALARM_OFF;
                                                    last_alarm.duration = DateTime.Now - last_alarm.timestamp;
                                                    last_alarm.timestamp = DateTime.Now;
                                                    // Inserisco la disattivazione
                                                    InsertAlarm(last_alarm);
                                                }
                                                // Compongo l'allarme
                                                alarm.alarm_type = AlarmTypes.EXCEEDED_MIN_THRESHOLD;
                                                alarm.event_type = EventTypes.ALARM_ON;
                                                alarm.alarm_value = vals.Min();
                                                alarm.reference_value = min_th;
                                            }
                                            else
                                                alarm.event_type = EventTypes.ALARM_ON;
                                        }
                                    }
                                }
                                else if ((alarm_cc_enable) && (hyst != 0))
                                {
                                    // Valore assoluto
                                    double hyst_val = Math.Abs(hyst);
                                    // Calcolo l'isteresi sui dati
                                    double abs_diff = Math.Abs(vals.Max() - vals.Min());
                                    if (abs_diff <= hyst_val)
                                    {
                                        if ((last_alarm.alarm_type != AlarmTypes.CONSTANT_INTERMEDIATE_VALUE) || (last_alarm.event_type != EventTypes.ALARM_ON))
                                        {
                                            // Controllo se c'è un altro allarme pendente e lo disattivo
                                            if (last_alarm.event_type == EventTypes.ALARM_ON)
                                            {
                                                last_alarm.event_type = EventTypes.ALARM_OFF;
                                                last_alarm.duration = DateTime.Now - last_alarm.timestamp;
                                                last_alarm.timestamp = DateTime.Now;
                                                // Inserisco la disattivazione
                                                InsertAlarm(last_alarm);
                                            }
                                            // Compongo l'allarme
                                            alarm.alarm_type = AlarmTypes.CONSTANT_INTERMEDIATE_VALUE;
                                            alarm.event_type = EventTypes.ALARM_ON;
                                            alarm.alarm_value = abs_diff;
                                            alarm.reference_value = hyst_val;
                                        }
                                        else
                                            alarm.event_type = EventTypes.ALARM_ON;
                                    }
                                }
                            }
                        }
                    }
                    // Inserisco o disabilito allarmi
                    if ((alarm.alarm_type == AlarmTypes.INVALID) && (alarm.event_type == EventTypes.UNKNOWN))
                    {
                        // Non ci sono allarmi, controllo se l'ultimo allarme è attivo e lo disattivo
                        if (last_alarm.event_type == EventTypes.ALARM_ON)
                        {
                            alarm = last_alarm;
                            alarm.timestamp = DateTime.Now;
                            alarm.event_type = EventTypes.ALARM_OFF;
                            alarm.duration = alarm.timestamp - last_alarm.timestamp;
                        }
                    }
                    if (alarm.alarm_type != AlarmTypes.INVALID)
                    {
                        // C'è un allarme valido, lo inserisco
                        InsertAlarm(alarm);
                    }
                    // Metto in pausa il job
                    Sleep(100);
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Inserisce una allarme in tabella
        /// </summary>
        /// <param name="alarm">Allarme</param>
        void InsertAlarm(AlarmStruct alarm)
        {
            // Inserisco l'allarme
            wet_db.ExecCustomCommand("INSERT INTO measures_alarms VALUES ('" +
                alarm.timestamp.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', " +
                ((int)alarm.alarm_type).ToString() + ", " + ((int)alarm.event_type).ToString() + ", " +
                alarm.alarm_value.ToString().Replace(',', '.') + ", " +
                alarm.reference_value.ToString().Replace(',', '.') + ", '" +
                alarm.duration.ToString(WetDBConn.MYSQL_TIME_FORMAT) + "', " +
                alarm.id_measure.ToString() + ", " +
                alarm.id_odbcdsn.ToString() + ")");
            // Imposto il bit di qualità sulla misura
            wet_db.ExecCustomCommand("UPDATE measures SET `reliable` = " + (alarm.event_type == EventTypes.ALARM_ON ? 0 : 1).ToString() +
                " WHERE id_measures = " + alarm.id_measure);
        }

        /// <summary>
        /// Restituisce l'ultimo allarme presente per la misura in questione
        /// </summary>
        /// <param name="wet_db">Connessione al database WetNet</param>
        /// <param name="id_measure">ID della misura</param>
        /// <param name="id_odbcdsn">ID della connessione</param>
        /// <param name="date">Data da analizzare</param>
        /// <returns>Ultimo allarme del giorno specificato</returns>
        public static AlarmStruct ReadLastAlarmDay(WetDBConn wet_db, int id_measure, int id_odbcdsn, DateTime date)
        {
            // Leggo l'ultimo allarme presente
            DataTable last_alarm_data = wet_db.ExecCustomQuery("SELECT * FROM measures_alarms WHERE `measures_id_measures` = " + id_measure.ToString() +
                " AND `timestamp` < '" + date.Date.AddDays(1.0d).ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' ORDER BY `timestamp` DESC LIMIT 1");
            AlarmStruct last_alarm;
            if (last_alarm_data.Rows.Count > 0)
            {
                last_alarm.timestamp = Convert.ToDateTime(last_alarm_data.Rows[0]["timestamp"]);
                last_alarm.alarm_type = (AlarmTypes)Convert.ToInt32(last_alarm_data.Rows[0]["alarm_type"]);
                last_alarm.event_type = (EventTypes)Convert.ToInt32(last_alarm_data.Rows[0]["event_type"]);
                last_alarm.alarm_value = Convert.ToDouble(last_alarm_data.Rows[0]["alarm_value"]);
                last_alarm.reference_value = Convert.ToDouble(last_alarm_data.Rows[0]["reference_value"]);
                string[] strs = Convert.ToString(last_alarm_data.Rows[0]["duration"]).Split(new char[] { ':' });
                last_alarm.duration = new TimeSpan(Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]), Convert.ToInt32(strs[2]));
                last_alarm.id_measure = Convert.ToInt32(last_alarm_data.Rows[0]["measures_id_measures"]);
                last_alarm.id_odbcdsn = Convert.ToInt32(last_alarm_data.Rows[0]["measures_connections_id_odbcdsn"]);
            }
            else
            {
                last_alarm.timestamp = DateTime.Now;
                last_alarm.alarm_type = AlarmTypes.INVALID;
                last_alarm.event_type = EventTypes.UNKNOWN;
                last_alarm.alarm_value = 0.0d;
                last_alarm.reference_value = 0.0d;
                last_alarm.duration = new TimeSpan();
                last_alarm.id_measure = id_measure;
                last_alarm.id_odbcdsn = id_odbcdsn;
            }

            return last_alarm;
        }

        #endregion
    }
}
