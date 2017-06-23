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
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace WetLib
{
    #region Enumerazioni

    /// <summary>
    /// Tipo di giorno
    /// </summary>
    enum DayTypes : int
    {
        /// <summary>
        /// Giorno feriale
        /// </summary>
        workday = 0,

        /// <summary>
        /// Giorno festivo
        /// </summary>
        holyday = 1
    }

    /// <summary>
    /// Tipi di misura supportati
    /// </summary>
    public enum MeasureTypes : int
    {
        /// <summary>
        /// Misura di portata
        /// </summary>
        FLOW = 0,

        /// <summary>
        /// Misura di pressione
        /// </summary>
        PRESSURE = 1,

        /// <summary>
        /// Contatore progressivo
        /// </summary>
        COUNTER = 2,

        /// <summary>
        /// Stato digitale
        /// </summary>
        /// <remarks>
        /// Viene archiviato come un <c>double</c> con solo due valori
        /// possibili: 0.0d e 1.0d
        /// </remarks>
        DIGITAL_STATE = 3,
        
        /// <summary>
        /// Other analog measures
        /// </summary>
        OTHERS = 4
    }

    /// <summary>
    /// categoria energetica delle misure
    /// </summary>
    enum EnergyCategories : int
    {
        /// <summary>
        /// Nessuna
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Energizzata
        /// </summary>
        POWER = 1,

        /// <summary>
        /// Gravità (non energizzata)
        /// </summary>
        GRAVITY = 2
    }

    /// <summary>
    /// Tipo di misuratori
    /// </summary>
    enum MeterTypes : int
    {
        /// <summary>
        /// Sconosciuto
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Misuratore di portata magnetico
        /// </summary>
        MAGNETIC_FLOW_METER = 1,

        /// <summary>
        /// Misuratore di portata a ultrasuoni
        /// </summary>
        ULTRASONIC_FLOW_METER = 2,

        /// <summary>
        /// Misuratore di portata LCF (celle di carico)
        /// </summary>
        LCF_FLOW_METER = 3,

        /// <summary>
        /// Misuratore di pressione
        /// </summary>
        PRESSURE_METER = 4,

        /// <summary>
        /// Contatore volumetrico
        /// </summary>
        VOLUMETRIC_COUNTER = 5,

        /// <summary>
        /// Pompa
        /// </summary>
        PUMP = 6,

        /// <summary>
        /// Valvola motorizzata senza posizionatore
        /// </summary>
        VALVE_NO_REGULATION = 7,

        /// <summary>
        /// Valvola motorizzata con posizionatore
        /// </summary>
        VALVE_REGULATION = 8,

        /// <summary>
        /// Serbatoio
        /// </summary>
        TANK = 9,

        /// <summary>
        /// Pozzo
        /// </summary>
        WELL = 10,

        /// <summary>
        /// Frequenza del motore
        /// </summary>
        MOTOR_FREQUENCY = 11,

        /// <summary>
        /// Solidi sospesi
        /// </summary>
        SST = 12,

        /// <summary>
        /// Ossigeno disciolto
        /// </summary>
        OX = 13,

        /// <summary>
        /// Ammonio
        /// </summary>
        NH4 = 14,

        /// <summary>
        /// Misuratore elettrico
        /// </summary>
        POWER_METER = 15,

        /// <summary>
        /// Misuratore di torbidità
        /// </summary>
        TURBIDITY_METER = 16
    }

    /// <summary>
    /// Tipo di collegamento dei misuratori
    /// </summary>
    enum MeterLinkTypes : int
    {
        /// <summary>
        /// Sconosciuto
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Radio UHF/VHF
        /// </summary>
        RADIO = 1,

        /// <summary>
        /// GSM SMS
        /// </summary>
        SMS = 2,

        /// <summary>
        /// GSM dati
        /// </summary>
        GSM = 3,

        /// <summary>
        /// GPRS
        /// </summary>
        GPRS = 4
    }

    /// <summary>
    /// Tipi di classi dei distretti
    /// </summary>
    enum DistrictClasses : int
    {
        /// <summary>
        /// Classe A
        /// </summary>
        A = 0,

        /// <summary>
        /// Classe B
        /// </summary>
        B = 1,

        /// <summary>
        /// Classe C
        /// </summary>
        C = 2
    }

    /// <summary>
    /// Districts measures signs
    /// </summary>
    enum DistrictsMeasuresSigns : int
    {
        /// <summary>
        /// +
        /// </summary>
        PLUS = 0,

        /// <summary>
        /// -
        /// </summary>
        MINUS = 1,

        /// <summary>
        /// /
        /// </summary>
        DIVISION = 2
    }

    /// <summary>
    /// Tipo di misura statistica per la generazione degli eventi
    /// </summary>
    enum DistrictStatisticMeasureTypes : int
    {
        /// <summary>
        /// Minima notturna
        /// </summary>
        MIN_NIGHT = 0,

        /// <summary>
        /// Minima giornaliera
        /// </summary>
        MIN_DAY = 1,

        /// <summary>
        /// Massima giornaliera
        /// </summary>
        MAX_DAY = 2,

        /// <summary>
        /// Media giornaliera
        /// </summary>
        AVG_DAY = 3,

        /// <summary>
        /// Profilo statistico
        /// </summary>
        STATISTICAL_PROFILE = 4
    }

    /// <summary>
    /// Tipo di misure acquisite
    /// </summary>
    /// <remarks>
    /// Valido solo su modello DB >= 2.0
    /// </remarks>
    public enum MeasuresSourcesTypes : int
    {
        /// <summary>
        /// Misure reali acquisite via ODBC
        /// </summary>
        REAL = 0,

        /// <summary>
        /// Misure con valore costante
        /// </summary>
        FIXED = 1,

        /// <summary>
        /// Misure calcolate
        /// </summary>
        CALCULATED = 2,

        /// <summary>
        /// Misure inserite manualmente dal campo (volumi)
        /// </summary>
        MANUALLY_INSERTED = 3
    }

    /// <summary>
    /// Tipo di ingresso del misuratore
    /// </summary>
    enum InputMeterTypes : int
    {
        /// <summary>
        /// Sconosciuto
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Ingresso analogico
        /// </summary>
        ANALOG_INPUT = 1,

        /// <summary>
        /// Ingresso impulsivo
        /// </summary>
        PULSE_INPUT = 2,

        /// <summary>
        /// Stato digitale
        /// </summary>
        DIGITAL_STATE = 3
    }

    #endregion

    /// <summary>
    /// Classe per la gestione della configurazione
    /// </summary>
    sealed class WetConfig
    {
        #region Costanti

        /// <summary>
        /// Nome del file di configurazione
        /// </summary>
        const string CONFIG_FILE_NAME = "WetEngine.config.xml";

        #endregion

        #region Strutture

        /// <summary>
        /// Valori di default per l'analisi
        /// </summary>
        public struct AnalysisSettings
        {
            /// <summary>
            /// Orario di inizio fascia oraria per analisi delle minime notturne
            /// </summary>
            public DateTime min_night_start_time;

            /// <summary>
            /// Orario di fine fascia oraria per analisi delle minime notturne
            /// </summary>
            public DateTime min_night_stop_time;            
        }

        /// <summary>
        /// Struttura di configurazione per job di copia degli LCF
        /// </summary>
        public struct WJ_Agent_LCF_Config_Struct
        {
            /// <summary>
            /// Stato di abilitazione dell'agente
            /// </summary>
            public bool enabled;

            /// <summary>
            /// Nome del DSN con il database di origine degli LCF
            /// </summary>
            public string odbc_dsn;
        }

        /// <summary>
        /// Struttura di configurazione per job di gestione dei WLB
        /// </summary>
        public struct WJ_Agent_WLB_Config_Struct
        {
            /// <summary>
            /// Stato di abilitazione dell'agente
            /// </summary>
            public bool enabled;

            /// <summary>
            /// Minuti di intervallo fra esecuzioni consecutive del job
            /// </summary>
            public int execution_interval_minutes;

            /// <summary>
            /// Nome host o indirizzo IP del server FTP
            /// </summary>
            public string ftp_server_name;

            /// <summary>
            /// Porta TCP del server FTP
            /// </summary>
            /// <remarks>Tipicamente la 21</remarks>
            public int ftp_server_port;

            /// <summary>
            /// Indica se il server supporta SSL
            /// </summary>
            public bool use_ssl;

            /// <summary>
            /// Indica se FTP deve usare la modalità passiva
            /// </summary>
            public bool is_passive_connection;

            /// <summary>
            /// Nome utente per la connessione
            /// </summary>
            public string username;

            /// <summary>
            /// Password per la connessione
            /// </summary>
            public string password;

            /// <summary>
            /// Cartella principale di archiviazione dei files
            /// </summary>
            public string folder;
        }

        /// <summary>
        /// Struttura di configurazione per job di gestione dei WLB
        /// </summary>
        public struct WJ_Agent_Primayer_Config_Struct
        {
            /// <summary>
            /// Stato di abilitazione dell'agente
            /// </summary>
            public bool enabled;

            /// <summary>
            /// Minuti di intervallo fra esecuzioni consecutive del job
            /// </summary>
            public int execution_interval_minutes;

            /// <summary>
            /// Nome host o indirizzo IP del server FTP
            /// </summary>
            public string ftp_server_name;

            /// <summary>
            /// Porta TCP del server FTP
            /// </summary>
            /// <remarks>Tipicamente la 21</remarks>
            public int ftp_server_port;

            /// <summary>
            /// Indica se il server supporta SSL
            /// </summary>
            public bool use_ssl;

            /// <summary>
            /// Indica se FTP deve usare la modalità passiva
            /// </summary>
            public bool is_passive_connection;

            /// <summary>
            /// Nome utente per la connessione
            /// </summary>
            public string username;

            /// <summary>
            /// Password per la connessione
            /// </summary>
            public string password;

            /// <summary>
            /// Cartella principale di archiviazione dei files
            /// </summary>
            public string folder;
        }

        /// <summary>
        /// Struttura di configurazione per job di gestione dati delle misure
        /// </summary>
        public struct WJ_MeasuresData_Config_Struct
        {
            /// <summary>
            /// DSN del database wetnet
            /// </summary>
            public string wetdb_dsn;

            /// <summary>
            /// Tempo di interpolazione espresso in minuti
            /// </summary>
            public int interpolation_time;
        }

        /// <summary>
        /// Struttura di configurazione per job degli eventi
        /// </summary>
        public struct WJ_Events_Config_Struct
        {
            /// <summary>
            /// Tempo di interpolazione espresso in minuti
            /// </summary>
            public int interpolation_time;

            /// <summary>
            /// Server SMTP di inoltro mail
            /// </summary>
            public string smtp_server;

            /// <summary>
            /// Porta SMTP del server di inoltro mail
            /// </summary>
            public int smtp_server_port;

            /// <summary>
            /// Uso di SSL su server SMTP di inoltro mail
            /// </summary>
            public bool smtp_use_ssl;

            /// <summary>
            /// Nome utente accesso al server SMTP
            /// </summary>
            public string smtp_username;

            /// <summary>
            /// Password accesso al server SMTP
            /// </summary>
            public string smtp_password;
        }

        /// <summary>
        /// Struttura di configurazione per job statistico
        /// </summary>
        public struct WJ_Statistics_Config_Struct
        {
            /// <summary>
            /// Tempo di interpolazione espresso in minuti
            /// </summary>
            public int interpolation_time;
        }

        /// <summary>
        /// Struttura di configurazione per job servizio web
        /// </summary>
        public struct WJ_WebService_Config_Struct
        {
            /// <summary>
            /// Porta di ascolto
            /// </summary>
            public int port;

            /// <summary>
            /// Tempo di interpolazione espresso in minuti
            /// </summary>
            public int interpolation_time;
        }

        #endregion

        #region Istanze

        /// <summary>
        /// Radice del documento
        /// </summary>
        XElement root;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WetConfig()
        {
            root = XElement.Load(Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName + @"\" + CONFIG_FILE_NAME);
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce l'ODBC DSN del database WetNet
        /// </summary>
        /// <returns>ODBC DSN del database WetNet</returns>
        public string GetWetDBDSN()
        {
            return root.Element("WetDBDSN").Value;
        }

        /// <summary>
        /// Acquisisce i dati di analisi di default
        /// </summary>
        /// <returns>Dati di analisi</returns>
        public AnalysisSettings GetDefaultAnalysisSettings()
        {
            AnalysisSettings settings;

            settings.min_night_start_time = WetUtility.GetDateTimeFromTime(root.Element("MinNightDefaultStartTime").Value);
            settings.min_night_stop_time = WetUtility.GetDateTimeFromTime(root.Element("MinNightDefaultStopTime").Value);

            return settings;
        }

        /// <summary>
        /// Restituisce la variabile con i tempi di interpolazione espressi in minuti
        /// </summary>
        /// <returns>Tempo di interpolazione espresso in minuti</returns>
        public static int GetInterpolationTimeMinutes()
        {
            XElement root = XElement.Load(Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName + @"\" + CONFIG_FILE_NAME);

            return Convert.ToInt32(root.Element("InterpolationTimeMinutes").Value);
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job di copia degli LCF
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_Agent_LCF_Config_Struct GetWJ_Agent_LCF_Config()
        {
            WJ_Agent_LCF_Config_Struct cfg;

            cfg.enabled = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_LCF").Attribute("enabled").Value);
            cfg.odbc_dsn = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_LCF").Value;

            return cfg;
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job di gestione dei WLB
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_Agent_WLB_Config_Struct GetWJ_Agent_WetNetLinkBox_Config()
        {
            WJ_Agent_WLB_Config_Struct cfg;

            cfg.enabled = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Attribute("enabled").Value);
            cfg.execution_interval_minutes = Convert.ToInt32(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Attribute("execution_interval_minutes").Value);
            cfg.ftp_server_name = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPServerName").Value;
            cfg.ftp_server_port = Convert.ToInt32(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPServerPort").Value);
            cfg.use_ssl = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPUseSSL").Value);
            cfg.is_passive_connection = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPPassiveMode").Value);
            cfg.username = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPUserName").Value;
            cfg.password = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPPassword").Value;
            cfg.folder = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_WetNetLinkBox").Element("FTPFolder").Value;

            return cfg;
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job di gestione dei device Primayer
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_Agent_Primayer_Config_Struct GetWJ_Agent_Primayer_Config()
        {
            WJ_Agent_Primayer_Config_Struct cfg;

            cfg.enabled = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Attribute("enabled").Value);
            cfg.execution_interval_minutes = Convert.ToInt32(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Attribute("execution_interval_minutes").Value);
            cfg.ftp_server_name = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPServerName").Value;
            cfg.ftp_server_port = Convert.ToInt32(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPServerPort").Value);
            cfg.use_ssl = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPUseSSL").Value);
            cfg.is_passive_connection = Convert.ToBoolean(root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPPassiveMode").Value);
            cfg.username = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPUserName").Value;
            cfg.password = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPPassword").Value;
            cfg.folder = root.Element("Jobs").Elements().Single(x => x.Attribute("name").Value == "Agent_Primayer").Element("FTPFolder").Value;

            return cfg;
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job di gestione dei dati delle misure
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_MeasuresData_Config_Struct GetWJ_MeasuresData_Config()
        {
            WJ_MeasuresData_Config_Struct cfg;

            cfg.wetdb_dsn = root.Element("WetDBDSN").Value;
            cfg.interpolation_time = Convert.ToInt32(root.Element("InterpolationTimeMinutes").Value);

            return cfg;
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job di gestione degli eventi
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_Events_Config_Struct GetWJ_Events_Config()
        {
            WJ_Events_Config_Struct cfg;

            cfg.interpolation_time = Convert.ToInt32(root.Element("InterpolationTimeMinutes").Value);
            cfg.smtp_server = root.Element("SMTPServer").Value;
            cfg.smtp_server_port = Convert.ToInt32(root.Element("SMTPServerPort").Value);
            cfg.smtp_use_ssl = Convert.ToBoolean(Convert.ToInt32(root.Element("SMTPUseSSL").Value));
            cfg.smtp_username = root.Element("SMTPServerUsername").Value;
            if (cfg.smtp_username == null)
                cfg.smtp_username = string.Empty;
            cfg.smtp_password = root.Element("SMTPServerPassword").Value;
            if (cfg.smtp_password == null)
                cfg.smtp_password = string.Empty;

            return cfg;
        }

        // <summary>
        /// Restituisce la struttura con la configurazione per il job statistico
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_Statistics_Config_Struct GetWJ_Statistics_Config()
        {
            WJ_Statistics_Config_Struct cfg;

            cfg.interpolation_time = Convert.ToInt32(root.Element("InterpolationTimeMinutes").Value);

            return cfg;
        }

        /// <summary>
        /// Restituisce la struttura con la configurazione per il job del servizio web
        /// </summary>
        /// <returns>Struttura con la configurazione</returns>
        public WJ_WebService_Config_Struct GetWJ_Webservice_Config()
        {
            WJ_WebService_Config_Struct cfg;

            cfg.port = Convert.ToInt32(root.Element("WebServicePort").Value);
            cfg.interpolation_time = Convert.ToInt32(root.Element("InterpolationTimeMinutes").Value);

            return cfg;
        }

        #endregion
    }
}
