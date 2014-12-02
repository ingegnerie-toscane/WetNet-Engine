using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace WetLib
{
    /// <summary>
    /// Job per la gestione degli eventi
    /// </summary>
    class WJ_Events : WetJob
    {
        #region Enumerazioni

        /// <summary>
        /// Enumerazione dei tipi di eventi
        /// </summary>
        enum EventTypes : int
        {
            /// <summary>
            /// Evento non valido
            /// </summary>
            NO_EVENT = 0,

            /// <summary>
            /// Incremento anomalo
            /// </summary>
            ANOMAL_INCREASE = 1,

            /// <summary>
            /// Possibile perdita
            /// </summary>
            POSSIBLE_LOSS = 2,

            /// <summary>
            /// Decremento anomalo
            /// </summary>
            ANOMAL_DECREASE = 3,

            /// <summary>
            /// Possibile efficientamento
            /// </summary>
            POSSIBLE_GAIN = 4,

            /// <summary>
            /// Distretto fuori controllo
            /// </summary>
            OUT_OF_CONTROL = 5
        }

        #endregion

        #region Strutture

        /// <summary>
        /// Struttura per la definizione di un evento
        /// </summary>
        struct Event
        {
            /// <summary>
            /// Giorno in cui si è verificato l'evento
            /// </summary>
            public DateTime day;

            /// <summary>
            /// Tipo di evnto
            /// </summary>
            public EventTypes type;

            /// <summary>
            /// Tipo di misura di riferimento
            /// </summary>
            public DistrictStatisticMeasureType measure_type;

            /// <summary>
            /// Tempo di perdurazione dell'evento espresso in giorni
            /// </summary>
            public int duration;

            /// <summary>
            /// Valore della misura di riferimento
            /// </summary>
            public double value;

            /// <summary>
            /// Scostamento
            /// </summary>
            public double delta;

            /// <summary>
            /// Ranking
            /// </summary>
            public double ranking;

            /// <summary>
            /// Descrizione
            /// </summary>
            public string description;

            /// <summary>
            /// Distretto di appartenenza
            /// </summary>
            public int id_district;
        }

        #endregion

        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_Statistics";

        /// <summary>
        /// Tempo di attesa fra una esecuzione e la successiva = 6 minuti
        /// </summary>
        const int JOB_SLEEP_TIME_MS = 360000;

        /// <summary>
        /// Ora minima per l'analisi degli eventi
        /// </summary>
        const int CHECK_HOUR = WJ_Statistics.CHECK_HOUR + 1;

        /// <summary>
        /// Ranking per l'evento OUT_OF_CONTROL
        /// </summary>
        const double OUT_OF_CONTROL_RANKING = 1.0d;

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database wetnet
        /// </summary>
        WetDBConn wet_db;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Struttura con la configurazione
        /// </summary>
        WetConfig.WJ_Events_Config_Struct cfg;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Events()
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
            // Istanzio la connessione al database wetnet
            WetConfig wcfg = new WetConfig();
            wet_db = new WetDBConn(wcfg.GetWetDBDSN(), true);
            cfg = wcfg.GetWJ_Events_Config();
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {            
            try
            {
                // Controllo che sia passata l'ora di verifica
                if (DateTime.Now.Hour < CHECK_HOUR)
                    return;
                // Acquisisco tutti i distretti configurati
                DataTable districts = wet_db.ExecCustomQuery("SELECT * FROM districts");
                // Ciclo per tutti i distretti
                foreach (DataRow district in districts.Rows)
                {                    
                    // Acquisisco l'ID del distretto
                    int id_district = Convert.ToInt32(district["id_districts"]);

                    #region Gestione distretto fuori controllo

                    // Creo un vettore delle misure in allarme
                    List<WJ_MeasuresAlarms.AlarmStruct> alarms = new List<WJ_MeasuresAlarms.AlarmStruct>();
                    // Controllo se ci sono allarmi sulle misure
                    DataTable measures_of_district_table = wet_db.ExecCustomQuery("SELECT `measures_id_measures`, `measures_connections_id_odbcdsn` FROM measures_has_districts INNER JOIN measures ON measures_has_districts.measures_id_measures = measures.id_measures WHERE `districts_id_districts` = " + id_district.ToString());
                    foreach (DataRow measure in measures_of_district_table.Rows)
                    {
                        // Acquisisco l'ID della misura
                        int id_measure = Convert.ToInt32(measure["measures_id_measures"]);
                        int id_odbcdsn = Convert.ToInt32(measure["measures_connections_id_odbcdsn"]);
                        // Leggo l'ultimo allarme della misura
                        DataTable alarms_table = wet_db.ExecCustomQuery("SELECT * FROM measures_alarms WHERE measures_id_measures = " + id_measure + " ORDER BY `timestamp` DESC LIMIT 1");
                        if (alarms_table.Rows.Count > 0)
                        {
                            WJ_MeasuresAlarms.AlarmStruct alarm = WJ_MeasuresAlarms.ReadLastAlarm(wet_db, id_measure, id_odbcdsn);
                            if (alarm.event_type == WJ_MeasuresAlarms.EventTypes.ALARM_ON)
                                alarms.Add(alarm);
                        }
                        // Passo il controllo al S.O. per l'attesa
                        Sleep(100);
                    }

                    // Se c'è almeno un allarme lo gestisco e creo l'evento
                    if (alarms.Count > 0)
                    {
                        // Inizializzo la struttura di un evento
                        Event ev;
                        ev.day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                        ev.type = EventTypes.OUT_OF_CONTROL;
                        ev.measure_type = DistrictStatisticMeasureType.STATISTICAL_PROFILE;
                        ev.duration = 0;
                        ev.description = "District out of control - Allarm(s) on measure(s): ";
                        ev.id_district = id_district;
                        ev.value = 0.0d;
                        ev.delta = 0.0d;
                        ev.ranking = OUT_OF_CONTROL_RANKING;
                        // Controllo se ci sono già altri eventi uguali pregressi
                        Event[] lasts;
                        ReadLastPastEvents(id_district, 1, out lasts);
                        if (lasts.Length > 0)
                            ev.ranking = lasts[0].duration++;
                        // Ciclo per tutti gli allarmi
                        for (int ii = 0; ii < alarms.Count; ii++)
                        {
                            ev.description += alarms[ii].id_measure;
                            if (ii < (alarms.Count - 1))
                                ev.description += ", ";
                        }
                        // Controllo che non sia già presente un evento uguale per il giorno corrente
                        Event[] actual_day_events;
                        ReadActualEvent(id_district, out actual_day_events);
                        bool can_write = !actual_day_events.Any(x => x.type == ev.type);
                        // Scrivo l'evento
                        if (can_write)
                        {
                            AppendEvent(ev);
                            ReportEvent(ev);
                        }
                        // Non processo ulteriori eventi, esco
                        continue;
                    }

                    #endregion

                    // Acquisisco l'abilitazione agli eventi
                    bool ev_enable = Convert.ToBoolean(Convert.ToInt32(district["ev_enable"]));
                    // Acquisisco la possibilità di autoupdate delle bande
                    bool bands_autoupdate = Convert.ToBoolean(Convert.ToInt32(district["ev_bands_autoupdate"]));
                    // Acquisisco la banda superiore attiva
                    double high_band = Convert.ToDouble(district["ev_high_band"]);
                    // Acquisisco la banda inferiore attiva
                    double low_band = Convert.ToDouble(district["ev_low_band"]);
                    // Acquisisco la banda superiore statistica
                    double statistic_high_band = Convert.ToDouble(district["ev_statistic_high_band"]);
                    // Acquisisco la banda inferiore statistica
                    double statistic_low_band = Convert.ToDouble(district["ev_statistic_low_band"]);
                    // Acquisisco tipo di variabile statistica da utilizzare
                    DistrictStatisticMeasureType measure_type = (DistrictStatisticMeasureType)(Convert.ToInt32(district["ev_variable_type"]));
                    // Acquisisco l'ultimo giorno valido
                    DateTime last_good_day = Convert.ToDateTime(district["ev_last_good_sample_day"]);
                    // Acquisisco il numero di campioni precedenti l'ultimo giorno valido
                    int last_good_samples = Convert.ToInt32(district["ev_last_good_samples"]);
                    // Acquisisco l'alpha
                    int alpha = Convert.ToInt32(district["ev_alpha"]);
                    // Acquisisco il numero di giorni per il trigger
                    int samples_trigger = Convert.ToInt32(district["ev_samples_trigger"]);
                    // Controllo che la data 'last_good_day' sia valida
                    if (last_good_day >= DateTime.Now.Date)
                        continue;
                    // Eseguo l'analisi in base al tipo di variabile
                    if (measure_type == DistrictStatisticMeasureType.STATISTICAL_PROFILE)
                    {
                        #region Profili statistici

                        double[] avg_vect;

                        // Controllo il tempo di interpolazione
                        if (cfg.interpolation_time <= 0)
                            throw new Exception("Interpolation time must be > 0!");

                        // Controllo se ho almeno un record statistico per il giorno precedente
                        DataTable tmp_dt = wet_db.ExecCustomQuery("SELECT `day` FROM districts_day_statistic WHERE `districts_id_districts` = " + id_district + " AND `day` = '" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                        if (tmp_dt.Rows.Count == 0)
                            continue;

                        /**********************************/
                        /*** Controllo per nuovi eventi ***/
                        /**********************************/

                        if ((ev_enable) && (high_band > low_band))
                        {
                            // Acquisisco il profilo del giorno precedente
                            tmp_dt = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE districts_id_districts = " +
                                id_district.ToString() + " AND `timestamp` >= '" + DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                                "' AND `timestamp` < '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");

                            // Controllo tutti i campioni del profilo
                            foreach (DataRow dr in tmp_dt.Rows)
                            {
                                DateTime ts = Convert.ToDateTime(dr["timestamp"]);
                                double val = Convert.ToDouble(dr["value"]);
                                // Acquisisco la media giornaliera statistica
                                avg_vect = GetDayAvgVector(id_district, DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)), last_good_samples);
                                double avg_sample = avg_vect[tmp_dt.Rows.IndexOf(dr)];
                                // Acquisisco gli ultimi eventi in base al trigger
                                Event[] lasts;
                                ReadLastPastEvents(id_district, samples_trigger, out lasts);
                                bool check = true;
                                if (lasts.Length > 0)
                                {
                                    if (lasts.Last().day == DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)))
                                        check = false;
                                }
                                // Se esiste già un record per il giorno precedente passo al distretto successivo
                                if (check)
                                {
                                    // Creo un nuovo evento
                                    Event ev;
                                    ev.day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                                    ev.type = EventTypes.NO_EVENT;
                                    ev.measure_type = measure_type;
                                    ev.duration = 0;
                                    ev.description = string.Empty;
                                    ev.id_district = id_district;
                                    ev.value = 0.0d;
                                    ev.delta = 0.0d;
                                    ev.ranking = 0.0d;
                                    // Superamento soglia superiore
                                    if (val > high_band)
                                    {
                                        int trigger = 0;

                                        // Controllo se sono già in perdita
                                        if (lasts.Length > 1)
                                        {
                                            if (lasts[lasts.Length - 1].type == EventTypes.POSSIBLE_LOSS)
                                            {
                                                ev.type = EventTypes.POSSIBLE_LOSS;
                                                ev.duration = lasts[lasts.Length - 1].duration;
                                            }
                                        }
                                        // Se non lo sono controllo se potrei esserci
                                        if (ev.type != EventTypes.POSSIBLE_LOSS)
                                        {
                                            // Imposto il valore di default per il tipo di evento
                                            ev.type = EventTypes.ANOMAL_INCREASE;
                                            // Scorro per il numero di trigger
                                            for (int ii = 0; ii < lasts.Length; ii++)
                                            {
                                                if (lasts[ii].type == EventTypes.ANOMAL_INCREASE)
                                                {
                                                    if (ii > 0)
                                                    {
                                                        if (lasts[ii - 1].type == EventTypes.ANOMAL_INCREASE)
                                                            trigger++;  // Gli eventi devono essere consecutivi...
                                                        else
                                                            break;  // ...altrimenti esco!
                                                    }
                                                    else
                                                        trigger++;  // Sono al primo evento ed incremento il trigger
                                                }
                                            }
                                            // Se il trigger viene raggiunto imposto un evento perdita
                                            if (trigger == samples_trigger)
                                                ev.type = EventTypes.POSSIBLE_LOSS;
                                            ev.duration = trigger;
                                        }
                                        // Calcolo la durata
                                        ev.duration++;
                                        // Calcolo il delta
                                        ev.value = val;
                                        ev.delta = val - high_band;
                                        // Scrivo la descrizione
                                        switch (ev.type)
                                        {
                                            case EventTypes.ANOMAL_INCREASE:
                                                ev.description = "Anomal increase found! - Timestamp '" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'";
                                                break;

                                            case EventTypes.POSSIBLE_LOSS:
                                                ev.description = "Possible water loss found! - Timestamp '" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'";
                                                break;

                                            default:
                                                ev.description = "Unhandled error in event engine. Please contact support!";
                                                break;
                                        }
                                    }
                                    // Superamento soglia inferiore
                                    if (val < low_band)
                                    {
                                        int trigger = 0;

                                        // Controllo se sono già in perdita
                                        if (lasts.Length > 1)
                                        {
                                            if (lasts[lasts.Length - 1].type == EventTypes.POSSIBLE_GAIN)
                                            {
                                                ev.type = EventTypes.POSSIBLE_GAIN;
                                                ev.duration = lasts[lasts.Length - 1].duration;
                                            }
                                        }
                                        // Se non lo sono controllo se potrei esserci
                                        if (ev.type != EventTypes.POSSIBLE_GAIN)
                                        {
                                            // Imposto il valore di default per il tipo di evento
                                            ev.type = EventTypes.ANOMAL_DECREASE;
                                            // Scorro per il numero di trigger
                                            for (int ii = 0; ii < lasts.Length; ii++)
                                            {
                                                if (lasts[ii].type == EventTypes.ANOMAL_DECREASE)
                                                {
                                                    if (ii > 0)
                                                    {
                                                        if (lasts[ii - 1].type == EventTypes.ANOMAL_DECREASE)
                                                            trigger++;  // Gli eventi devono essere consecutivi...
                                                        else
                                                            break;  // ...altrimenti esco!
                                                    }
                                                    else
                                                        trigger++;  // Sono al primo evento ed incremento il trigger
                                                }
                                            }
                                            // Se il trigger viene raggiunto imposto un evento perdita
                                            if (trigger == (samples_trigger - 1))
                                                ev.type = EventTypes.POSSIBLE_GAIN;
                                            ev.duration = trigger;
                                        }
                                        // Calcolo la durata
                                        ev.duration++;
                                        // Calcolo il delta
                                        ev.value = val;
                                        ev.delta = val - low_band;
                                        // Calcolo il ranking
                                        ev.ranking = ev.delta / avg_sample;
                                        // Scrivo la descrizione
                                        switch (ev.type)
                                        {
                                            case EventTypes.ANOMAL_DECREASE:
                                                ev.description = "Anomal decrease found! - Timestamp '" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'";
                                                break;

                                            case EventTypes.POSSIBLE_GAIN:
                                                ev.description = "Possible water gain found! - Timestamp '" + ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'";
                                                break;

                                            default:
                                                ev.description = "Unhandled error in event engine. Please contact support!";
                                                break;
                                        }
                                    }                                        
                                    // Controllo che non sia già presente un evento uguale per il giorno corrente
                                    Event[] actual_day_events;
                                    ReadActualEvent(id_district, out actual_day_events);
                                    bool can_write = !actual_day_events.Any(x => x.type == ev.type);
                                    // Scrivo l'evento
                                    if (can_write)
                                    {
                                        AppendEvent(ev);
                                        ReportEvent(ev);
                                    }
                                }
                                Sleep(100);
                            }
                        }

                        /*****************************************/
                        /*** Calcolo valori di efficientamento ***/
                        /*****************************************/
                        // Acquisisco l'ultimo evento del distretto
                        DateTime last_day;
                        Event ev_last = ReadLastEvent(id_district);
                        if (bands_autoupdate)
                        {
                            if (ev_last.type != EventTypes.NO_EVENT)
                            {
                                // L'evento è valido, controllo la data
                                if ((DateTime.Now.Date - ev_last.day).Days < last_good_samples)
                                    last_day = last_good_day;   // Non sono passati abbastanza giorni per il ricalcolo dei parametri, li calcolo basandomi sui valori impostati
                                else
                                    last_day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                            }
                            else
                                last_day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                        }
                        else
                            last_day = last_good_day;
                        // Trovo il vettore delle medie
                        avg_vect = GetDayAvgVector(id_district, last_day, last_good_samples);                        
                        // Calcolo media e deviazione standard
                        double avg = WetStatistics.GetMean(avg_vect);
                        double standard_deviation = WetStatistics.StandardDeviation(avg_vect);
                        // Calcolo i valori proposti
                        double phb = avg + (standard_deviation * alpha);
                        double plb = avg - (standard_deviation * alpha);
                        // Effettuo l'update sul DB
                        wet_db.ExecCustomCommand("UPDATE districts SET ev_statistic_high_band = " + phb.ToString().Replace(',', '.') +
                            ", ev_statistic_low_band = " + plb.ToString().Replace(',', '.') + " WHERE id_districts = " + id_district.ToString());
                        // Acquisisco la data dell'ultimo evento scritto
                        tmp_dt = wet_db.ExecCustomQuery("SELECT * FROM districts_bands_history WHERE `districts_id_districts` = " + id_district.ToString() + " ORDER BY `timestamp` DESC LIMIT 1");
                        DateTime last_change;
                        if (tmp_dt.Rows.Count == 0)
                            last_change = DateTime.MinValue;
                        else
                            last_change = Convert.ToDateTime(tmp_dt.Rows[0]["timestamp"]);
                        // Effettuo l'autoupdate delle bande se abilitato
                        if ((bands_autoupdate) && (ev_last.type == EventTypes.POSSIBLE_GAIN) &&
                            (ev_last.duration >= last_good_samples) &&
                            (last_change < ev_last.day) &&
                            (statistic_low_band != plb) && (statistic_high_band != phb))
                        {
                            // Aggiorno la tabella distretti
                            wet_db.ExecCustomCommand("UPDATE districts SET ev_high_band = " + phb.ToString().Replace(',', '.') +
                                ", ev_low_band = " + plb.ToString().Replace(',', '.') + " WHERE id_districts = " + id_district.ToString());
                            // Inserisco i nuovi valori nella tabella dei cambiamenti
                            wet_db.ExecCustomCommand("INSERT INTO districts_bands_history VALUES ('" +
                                DateTime.Now.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', " +
                                phb.ToString().Replace(',', '.') + ", " +
                                plb.ToString().Replace(',', '.') + ", " +
                                id_district.ToString() + ") ");                                
                        }

                        #endregion
                    }
                    else
                    {
                        #region Variabili statistiche

                        // Imposto il nome della variabile da acquisire
                        string measure_name;
                        switch (measure_type)
                        {
                            default:
                            case DistrictStatisticMeasureType.MIN_NIGHT:
                                measure_name = "min_night";
                                break;

                            case DistrictStatisticMeasureType.MIN_DAY:
                                measure_name = "min_day";
                                break;

                            case DistrictStatisticMeasureType.MAX_DAY:
                                measure_name = "max_day";
                                break;

                            case DistrictStatisticMeasureType.AVG_DAY:
                                measure_name = "avg_day";
                                break;
                        }
                        // Controllo che siano stati scritti i valori statistici per il giorno precedente, altrimenti proseguo
                        DataTable tmp_dt = wet_db.ExecCustomQuery("SELECT * FROM districts_day_statistic WHERE `districts_id_districts` = " + id_district + " AND `day` = '" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "'");
                        if (tmp_dt.Rows.Count == 0)
                            continue;   // Il record non è ancora stato scritto
                        if ((tmp_dt.Rows[0][measure_name] == DBNull.Value) || (tmp_dt.Rows[0]["avg_day"] == DBNull.Value))
                            continue;   // Il campo non è ancora stato scritto

                        /**********************************/
                        /*** Controllo per nuovi eventi ***/
                        /**********************************/

                        if ((ev_enable) && (high_band > low_band))
                        {
                            // Acquisisco il valore attuale della misura
                            double val = Convert.ToDouble(tmp_dt.Rows[0][measure_name]);
                            // Acquisisco la media giornaliera statistica
                            double avg_day = Convert.ToDouble(tmp_dt.Rows[0]["avg_day"]);
                            // Acquisisco gli ultimi eventi in base al trigger
                            Event[] lasts;
                            ReadLastPastEvents(id_district, samples_trigger, out lasts);
                            bool check = true;
                            if (lasts.Length > 0)
                            {
                                if (lasts.Last().day == DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0)))
                                    check = false;
                            }
                            // Se esiste già un record per il giorno precedente passo al distretto successivo
                            if (check)
                            {
                                // Creo un nuovo evento
                                Event ev;
                                ev.day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                                ev.type = EventTypes.NO_EVENT;
                                ev.measure_type = measure_type;
                                ev.duration = 0;
                                ev.description = string.Empty;
                                ev.id_district = id_district;
                                ev.value = 0.0d;
                                ev.delta = 0.0d;
                                ev.ranking = 0.0d;
                                // Superamento soglia superiore
                                if (val > high_band)
                                {
                                    int trigger = 0;

                                    // Controllo se sono già in perdita
                                    if (lasts.Length > 1)
                                    {
                                        if (lasts[lasts.Length - 1].type == EventTypes.POSSIBLE_LOSS)
                                        {
                                            ev.type = EventTypes.POSSIBLE_LOSS;
                                            ev.duration = lasts[lasts.Length - 1].duration;
                                        }
                                    }
                                    // Se non lo sono controllo se potrei esserci
                                    if (ev.type != EventTypes.POSSIBLE_LOSS)
                                    {
                                        // Imposto il valore di default per il tipo di evento
                                        ev.type = EventTypes.ANOMAL_INCREASE;
                                        // Scorro per il numero di trigger
                                        for (int ii = 0; ii < lasts.Length; ii++)
                                        {
                                            if (lasts[ii].type == EventTypes.ANOMAL_INCREASE)
                                            {
                                                if (ii > 0)
                                                {
                                                    if (lasts[ii - 1].type == EventTypes.ANOMAL_INCREASE)
                                                        trigger++;  // Gli eventi devono essere consecutivi...
                                                    else
                                                        break;  // ...altrimenti esco!
                                                }
                                                else
                                                    trigger++;  // Sono al primo evento ed incremento il trigger
                                            }
                                        }
                                        // Se il trigger viene raggiunto imposto un evento perdita
                                        if (trigger == (samples_trigger - 1))
                                            ev.type = EventTypes.POSSIBLE_LOSS;
                                        ev.duration = trigger;
                                    }
                                    // Calcolo la durata
                                    ev.duration++;
                                    // Calcolo il delta                                
                                    ev.value = val;
                                    ev.delta = val - high_band;
                                    // Calcolo il ranking
                                    ev.ranking = ev.delta / avg_day;
                                    // Scrivo la descrizione
                                    switch (ev.type)
                                    {
                                        case EventTypes.ANOMAL_INCREASE:
                                            ev.description = "Anomal increase found!";
                                            break;

                                        case EventTypes.POSSIBLE_LOSS:
                                            ev.description = "Possible water loss found!";
                                            break;

                                        default:
                                            ev.description = "Unhandled error in event engine. Please contact support!";
                                            break;
                                    }
                                }
                                // Superamento soglia inferiore
                                if (val < low_band)
                                {
                                    int trigger = 0;

                                    // Controllo se sono già in perdita
                                    if (lasts.Length > 1)
                                    {
                                        if (lasts[lasts.Length - 1].type == EventTypes.POSSIBLE_GAIN)
                                        {
                                            ev.type = EventTypes.POSSIBLE_GAIN;
                                            ev.duration = lasts[lasts.Length - 1].duration;
                                        }
                                    }
                                    // Se non lo sono controllo se potrei esserci
                                    if (ev.type != EventTypes.POSSIBLE_GAIN)
                                    {
                                        // Imposto il valore di default per il tipo di evento
                                        ev.type = EventTypes.ANOMAL_DECREASE;
                                        // Scorro per il numero di trigger
                                        for (int ii = 0; ii < lasts.Length; ii++)
                                        {
                                            if (lasts[ii].type == EventTypes.ANOMAL_DECREASE)
                                            {
                                                if (ii > 0)
                                                {
                                                    if (lasts[ii - 1].type == EventTypes.ANOMAL_DECREASE)
                                                        trigger++;  // Gli eventi devono essere consecutivi...
                                                    else
                                                        break;  // ...altrimenti esco!
                                                }
                                                else
                                                    trigger++;  // Sono al primo evento ed incremento il trigger
                                            }
                                        }
                                        // Se il trigger viene raggiunto imposto un evento perdita
                                        if (trigger == (samples_trigger - 1))
                                            ev.type = EventTypes.POSSIBLE_GAIN;
                                        ev.duration = trigger;
                                    }
                                    // Calcolo la durata
                                    ev.duration++;
                                    // Calcolo il delta
                                    ev.value = val;
                                    ev.delta = val - low_band;
                                    // Calcolo il ranking
                                    ev.ranking = ev.delta / avg_day;
                                    // Scrivo la descrizione
                                    switch (ev.type)
                                    {
                                        case EventTypes.ANOMAL_DECREASE:
                                            ev.description = "Anomal decrease found!";
                                            break;

                                        case EventTypes.POSSIBLE_GAIN:
                                            ev.description = "Possible water gain found!";
                                            break;

                                        default:
                                            ev.description = "Unhandled error in event engine. Please contact support!";
                                            break;
                                    }
                                }                                    
                                // Controllo che non sia già presente un evento uguale per il giorno corrente
                                Event[] actual_day_events;
                                ReadActualEvent(id_district, out actual_day_events);
                                bool can_write = !actual_day_events.Any(x => x.type == ev.type);
                                // Scrivo l'evento
                                if (can_write)
                                {
                                    AppendEvent(ev);
                                    ReportEvent(ev);
                                }
                            }
                        }

                        /*****************************************/
                        /*** Calcolo valori di efficientamento ***/
                        /*****************************************/
                        // Acquisisco l'ultimo evento del distretto
                        DateTime last_day;
                        Event ev_last = ReadLastEvent(id_district);
                        if (bands_autoupdate)
                        {
                            if (ev_last.type != EventTypes.NO_EVENT)
                            {
                                // L'evento è valido, controllo la data
                                if ((DateTime.Now.Date - ev_last.day).Days < last_good_samples)
                                    last_day = last_good_day;   // Non sono passati abbastanza giorni per il ricalcolo dei parametri, li calcolo basandomi sui valori impostati
                                else
                                    last_day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                            }
                            else
                                last_day = DateTime.Now.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                        }
                        else
                            last_day = last_good_day;
                        // OK, posso ricalcolare i valori, acquisisco gli ultimi 'last_good_samples' valori
                        DataTable last_good_samples_table = wet_db.ExecCustomQuery("SELECT `day`, `" + measure_name + "` FROM districts_day_statistic WHERE districts_id_districts = " +
                            id_district.ToString() + " AND `day` <= '" + last_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' ORDER BY `day` DESC LIMIT " + last_good_samples.ToString());
                        // Vettorizzo
                        List<double> lgsv = new List<double>();
                        foreach (DataRow dr in last_good_samples_table.Rows)
                            lgsv.Add(Convert.ToDouble(dr[measure_name] == DBNull.Value ? 0.0d : dr[measure_name]));
                        // Calcolo la media
                        double avg = 0.0d;
                        if (lgsv.Count > 0)
                            avg = WetStatistics.GetMean(lgsv.ToArray());
                        // Calcolo la deviazione standard
                        double standard_deviation = 0.0d;
                        if (lgsv.Count > 1)
                            standard_deviation = WetStatistics.StandardDeviation(lgsv.ToArray());
                        // Calcolo i valori proposti
                        double phb = avg + (standard_deviation * alpha);
                        double plb = avg - (standard_deviation * alpha);
                        // Effettuo l'update sul DB
                        wet_db.ExecCustomCommand("UPDATE districts SET ev_statistic_high_band = " + phb.ToString().Replace(',', '.') +
                            ", ev_statistic_low_band = " + plb.ToString().Replace(',', '.') + " WHERE id_districts = " + id_district.ToString());
                        // Acquisisco la data dell'ultimo evento scritto
                        tmp_dt = wet_db.ExecCustomQuery("SELECT * FROM districts_bands_history WHERE `districts_id_districts` = " + id_district.ToString() + " ORDER BY `timestamp` DESC LIMIT 1");
                        DateTime last_change;
                        if (tmp_dt.Rows.Count == 0)
                            last_change = DateTime.MinValue;
                        else
                            last_change = Convert.ToDateTime(tmp_dt.Rows[0]["timestamp"]);
                        // Effettuo l'autoupdate delle bande se abilitato
                        if ((bands_autoupdate) && (ev_last.type == EventTypes.POSSIBLE_GAIN) &&
                            (ev_last.duration >= last_good_samples) &&
                            (last_change < ev_last.day) &&
                            (statistic_low_band != plb) && (statistic_high_band != phb))
                        {
                            // Aggiorno la tabella distretti
                            wet_db.ExecCustomCommand("UPDATE districts SET ev_high_band = " + phb.ToString().Replace(',', '.') +
                                ", ev_low_band = " + plb.ToString().Replace(',', '.') + " WHERE id_districts = " + id_district.ToString());
                            // Inserisco i nuovi valori nella tabella dei cambiamenti
                            wet_db.ExecCustomCommand("INSERT INTO districts_bands_history VALUES ('" +
                                DateTime.Now.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "', " +
                                phb.ToString().Replace(',', '.') + ", " +
                                plb.ToString().Replace(',', '.') + ", " +
                                id_district.ToString() + ") ");
                        }

                        #endregion
                    }
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
        /// Restituisce un vettore giornaliero con le medie di ciascun campione nehli ultimi 'days' giorni.
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="last_day">Ultimo giorno di analisi</param>
        /// <param name="days">Giorni precedenti a <paramref name="last_day"/> giorno</param>
        /// <returns>Vettore delle medie</returns>
        double[] GetDayAvgVector(int id_district, DateTime last_day, int days)
        {
            // OK, posso ricalcolare i valori, acquisisco gli ultimi 'last_good_samples' valori
            int samples_in_day = (60 / cfg.interpolation_time) * 24;
            DataTable tmp_dt = wet_db.ExecCustomQuery("SELECT * FROM data_districts WHERE districts_id_districts = " +
                id_district.ToString() + " AND `timestamp` < '" + last_day.AddDays(1.0d).ToString(WetDBConn.MYSQL_DATETIME_FORMAT) +
                "' ORDER BY `timestamp` DESC LIMIT " + (days * samples_in_day).ToString());
            // Creo una matrice multidimensionale con i valori dei giorni e la popolo
            double[,] matrix = new double[days, samples_in_day];
            for (int ii = 0; ii < days; ii++)
            {
                for (int jj = 0; jj < samples_in_day; jj++)
                    matrix[ii, jj] = Convert.ToDouble(tmp_dt.Rows[(ii * samples_in_day) + jj]["value"]);
            }
            // Creo un vettore con le medie
            double[] avg_vect = new double[samples_in_day];
            for (int ii = 0; ii < samples_in_day; ii++)
            {
                double sum = 0.0d;
                for (int jj = 0; jj < days; jj++)
                    sum += matrix[jj, ii];
                avg_vect[ii] = sum / samples_in_day;
            }

            return avg_vect;
        }

        /// <summary>
        /// Legge gli ultimi eventi eccetto quelli del giorno attuale
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="number">Numero di eventi da leggere</param>
        /// <param name="events">Vettore degli eventi</param>
        void ReadLastPastEvents(int id_district, int number, out Event[] events)
        {
            // Calcolo giorno di inizio e giorno di fine
            DateTime first_day = DateTime.Now.Date.Subtract(new TimeSpan(number, 0, 0, 0));
            // Acquisisco la lista degli ultimi eventi
            DataTable events_table = wet_db.ExecCustomQuery("SELECT * FROM districts_events WHERE districts_id_districts = " + id_district.ToString() +
                " AND `day` >= '" + first_day.ToString(WetDBConn.MYSQL_DATE_FORMAT) +
                "' AND `day` < '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' ORDER BY `day` ASC");
            List<Event> evs = new List<Event>();
            for (int ii = 0; ii < events_table.Rows.Count; ii++)
            {
                Event ev;

                ev.day = Convert.ToDateTime(events_table.Rows[ii]["day"]);
                // Se fra questo evento ed il precedente è trascorso più di un giorno, azzero gli eventi precedenti
                if (evs.Count > 0)
                {
                    if ((ev.day - evs.Last().day) > new TimeSpan(1, 0, 0, 0))
                        evs.Clear();
                }
                ev.type = (EventTypes)Convert.ToInt32(events_table.Rows[ii]["type"]);
                ev.measure_type = (DistrictStatisticMeasureType)Convert.ToInt32(events_table.Rows[ii]["measure_type"]);
                ev.duration = Convert.ToInt32(events_table.Rows[ii]["duration"]);
                ev.value = Convert.ToDouble(events_table.Rows[ii]["value"]);
                ev.delta = Convert.ToDouble(events_table.Rows[ii]["delta_value"]);
                ev.ranking = Convert.ToDouble(events_table.Rows[ii]["ranking"]);
                ev.description = Convert.ToString(events_table.Rows[ii]["description"]);
                ev.id_district = Convert.ToInt32(events_table.Rows[ii]["districts_id_districts"]);

                evs.Add(ev);
            }
            events = evs.ToArray();
        }

        /// <summary>
        /// Legge l'ultimo evento per il distretto
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <returns>Evento</returns>
        Event ReadLastEvent(int id_district)
        {
            Event ev = new Event();

            DataTable last_ev = wet_db.ExecCustomQuery("SELECT * FROM districts_events WHERE districts_id_districts = " + id_district.ToString() +
                " ORDER BY `day` DESC LIMIT 1");
            if (last_ev.Rows.Count == 1)
            {
                ev.day = Convert.ToDateTime(last_ev.Rows[0]["day"]);
                ev.type = (EventTypes)Convert.ToInt32(last_ev.Rows[0]["type"]);
                ev.measure_type = (DistrictStatisticMeasureType)Convert.ToInt32(last_ev.Rows[0]["measure_type"]);
                ev.duration = Convert.ToInt32(last_ev.Rows[0]["duration"]);
                ev.value = Convert.ToDouble(last_ev.Rows[0]["value"]);
                ev.delta = Convert.ToDouble(last_ev.Rows[0]["delta_value"]);
                ev.ranking = Convert.ToDouble(last_ev.Rows[0]["ranking"]);
                ev.description = Convert.ToString(last_ev.Rows[0]["description"]);
                ev.id_district = Convert.ToInt32(last_ev.Rows[0]["districts_id_districts"]);
            }

            return ev;
        }

        /// <summary>
        /// Acquisisce la lista degli eventi per il giorno corrente
        /// </summary>
        /// <param name="id_district">ID del distretto</param>
        /// <param name="events">Vettore degli eventi</param>
        void ReadActualEvent(int id_district, out Event[] events)
        {
            // Acquisisco gòli eventi per il giorno corrente
            DataTable events_table = wet_db.ExecCustomQuery("SELECT * FROM districts_events WHERE districts_id_districts = " + id_district.ToString() +
                " AND `day` = '" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Date.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "' ORDER BY `day` ASC");
            events = new Event[events_table.Rows.Count];
            for (int ii = 0; ii < events_table.Rows.Count; ii++)
            {
                events[ii].day = Convert.ToDateTime(events_table.Rows[ii]["day"]);
                events[ii].type = (EventTypes)Convert.ToInt32(events_table.Rows[ii]["type"]);
                events[ii].measure_type = (DistrictStatisticMeasureType)Convert.ToInt32(events_table.Rows[ii]["measure_type"]);
                events[ii].duration = Convert.ToInt32(events_table.Rows[ii]["duration"]);
                events[ii].value = Convert.ToDouble(events_table.Rows[ii]["value"]);
                events[ii].delta = Convert.ToDouble(events_table.Rows[ii]["delta_value"]);
                events[ii].ranking = Convert.ToDouble(events_table.Rows[ii]["ranking"]);
                events[ii].description = Convert.ToString(events_table.Rows[ii]["description"]);
                events[ii].id_district = Convert.ToInt32(events_table.Rows[ii]["districts_id_districts"]);
            }
        }

        /// <summary>
        /// Inserisce un nuovo evento
        /// </summary>
        /// <param name="ev">Evento</param>
        /// <returns>Stato di successo</returns>
        bool AppendEvent(Event ev)
        {
            // Inserisco il record
            int ret = wet_db.ExecCustomCommand("INSERT INTO districts_events VALUES ('" + ev.day.ToString(WetDBConn.MYSQL_DATE_FORMAT) + "', " +
                ((int)ev.type).ToString() + ", " + ((int)ev.measure_type).ToString() + ", " + ev.duration.ToString() + ", " +
                ev.value.ToString().Replace(',', '.') + ", " + ev.delta.ToString().Replace(',', '.') + ", " +
                ev.ranking.ToString().Replace(',', '.') + ", '" + ev.description + "', " + ev.id_district.ToString() + ")");
            if (ret == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Effettua il report di un allarme a tutti gli utenti abilitati
        /// </summary>
        /// <param name="ev">Evento</param>
        void ReportEvent(Event ev)
        {
            if (ev.type != EventTypes.NO_EVENT)
            {
                // Acquisisco il nome per il distretto
                DataTable dt = wet_db.ExecCustomQuery("SELECT `name` FROM districts WHERE id_districts = " + ev.id_district);
                string name = string.Empty;
                if (dt.Rows.Count == 1)
                    name += Convert.ToString(dt.Rows[0]["name"]);
                // Compongo la stringa dell'evento
                string sms_msg =
                    "EventType:";
                string mail_msg =
                    "Event type   : ";
                switch (ev.type)
                {
                    case EventTypes.ANOMAL_INCREASE:
                        mail_msg += "Anomal flow rate increase detected!";
                        sms_msg += "flow+";
                        break;

                    case EventTypes.ANOMAL_DECREASE:
                        mail_msg += "Anomal flow rate decrease detected!";
                        sms_msg += "flow-";
                        break;

                    case EventTypes.POSSIBLE_LOSS:
                        mail_msg += "Possible water loss detected!";
                        sms_msg += "loss";
                        break;

                    case EventTypes.POSSIBLE_GAIN:
                        mail_msg += "Possible water gain detected!";
                        sms_msg += "gain";
                        break;

                    default:
                        mail_msg += "Invalid!";
                        sms_msg += "invalid";
                        break;
                }
                mail_msg += Environment.NewLine +
                    "Delta        : " + ev.delta.ToString("F", CultureInfo.InvariantCulture) + " l/s" + Environment.NewLine +
                    "District     : " + name + " (#" + ev.id_district + ")" + Environment.NewLine +
                    "Date         : " + ev.day.ToString("yyyy-MM-dd") + Environment.NewLine +
                    "Duration     : " + ev.duration + " days" + Environment.NewLine +
                    "Variable     : ";
                sms_msg += " Delta:" + ev.delta.ToString("F", CultureInfo.InvariantCulture) +
                    " District:" + ev.id_district +
                    " Date:" + ev.day.ToString("yyyy-MM-dd") +
                    " Duration:" + ev.duration +
                    " Variable:";
                switch (ev.measure_type)
                {
                    case DistrictStatisticMeasureType.MIN_NIGHT:
                        mail_msg += "Min. Night";
                        sms_msg += "MinNight";
                        break;

                    case DistrictStatisticMeasureType.MIN_DAY:
                        mail_msg += "Min. Day";
                        sms_msg += "MinDay";
                        break;

                    case DistrictStatisticMeasureType.AVG_DAY:
                        mail_msg += "Avg. Day";
                        sms_msg += "AvgDay";
                        break;

                    case DistrictStatisticMeasureType.MAX_DAY:
                        mail_msg += "Max. Day";
                        sms_msg += "MaxDay";
                        break;

                    case DistrictStatisticMeasureType.STATISTICAL_PROFILE:
                        mail_msg += "Statistical profile";
                        sms_msg += "StatProfile";
                        break;

                    default:
                        mail_msg += "Unknown";
                        sms_msg += "Unknown";
                        break;
                }
                mail_msg += Environment.NewLine +
                    "Actual value : " + ev.value.ToString("F", CultureInfo.InvariantCulture) + " l/s" + Environment.NewLine + Environment.NewLine +
                    "###### Automatic message, please don't reply! ######";
                sms_msg += " ActualValue:" + ev.value.ToString("F", CultureInfo.InvariantCulture);
                // Acquisisco la lista di tutti gli utenti abilitati alla notifica sul distretto
                dt = wet_db.ExecCustomQuery("SELECT * FROM users_has_districts INNER JOIN users ON users_has_districts.users_idusers = users.idusers WHERE districts_id_districts = " + ev.id_district + " AND events_notification = 1");
                foreach (DataRow dr in dt.Rows)
                {
                    // Controllo che l'utente sia abilitato all'invio delle e-mail
                    if (Convert.ToBoolean(dr["email_enabled"]))
                    {
                        // Controllo che il campo e-mail non sia vuoto
                        string mail_address = Convert.ToString(dr["email"]);
                        if (mail_address != null)
                        {
                            if (mail_address != string.Empty)
                            {
                                // Compongo la mail
                                SmtpClient smtp_client = new SmtpClient();
                                smtp_client.Host = cfg.smtp_server;
                                smtp_client.Port = cfg.smtp_server_port;
                                smtp_client.EnableSsl = cfg.smtp_use_ssl;
                                smtp_client.Credentials = new NetworkCredential(cfg.smtp_username, cfg.smtp_password);
                                MailMessage msg = new MailMessage();
                                msg.From = new MailAddress(cfg.smtp_username);
                                msg.To.Add(mail_address);
                                msg.Subject = "WetNet event report";
                                msg.Body = mail_msg;
                                smtp_client.Send(msg);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
