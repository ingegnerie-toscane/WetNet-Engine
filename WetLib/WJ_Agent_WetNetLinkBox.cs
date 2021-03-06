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

#region OLD_BUT_GOOD

namespace WetLib
{
    /// <summary>
    /// Job per il trattamento dei dati degli WetNet Link Box (WLB)
    /// </summary>
    sealed class WJ_Agent_WetNetLinkBox : WetJob
    {
        #region Istanze

        /// <summary>
        /// Configurazione
        /// </summary>
        WetConfig cfg;

        /// <summary>
        /// Connessione al database WetNet
        /// </summary>
        WetDBConn wet_db;

        /// <summary>
        /// Connessione FTP
        /// </summary>
        WetFTP wet_ftp;

        #endregion

        #region Variabili globali

        /// <summary>
        /// DSN ODBC del database WetNet
        /// </summary>
        string wetnet_dsn;

        /// <summary>
        /// Configurazione del job
        /// </summary>
        WetConfig.WJ_Agent_WLB_Config_Struct config;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Agent_WetNetLinkBox()
        {
            // Millisecondi di attesa fra le esecuzioni
            job_sleep_time = WetConfig.GetInterpolationTimeMinutes() * 60 * 1000;
            // Istanzio la configurazione
            cfg = new WetConfig();
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce data e ora dal nome del file
        /// </summary>
        /// <param name="filename">Nome del file</param>
        /// <returns>Data e ora</returns>
        DateTime GetDTFromFilename(string filename)
        {
            DateTime dt = DateTime.MaxValue;

            try
            {
                int sep_pos = filename.IndexOf('-');
                int point_pos = filename.IndexOf('.');
                string val_str = filename.Substring(sep_pos + 1, filename.Length - (filename.Length - point_pos) - sep_pos - 1);
                int year = Convert.ToInt32(val_str.Substring(0, 4));
                int month = Convert.ToInt32(val_str.Substring(4, 2));
                int day = Convert.ToInt32(val_str.Substring(6, 2));
                int hour = Convert.ToInt32(val_str.Substring(8, 2));
                int minute = Convert.ToInt32(val_str.Substring(10, 2));

                dt = new DateTime(year, month, day, hour, minute, 0);
            }
            catch { }

            return dt;
        }

        #endregion

        #region Funzioni del Job

        /// <summary>
        /// Funzione di caricamento
        /// </summary>
        protected override void Load()
        {
            // Carico la configurazione
            config = cfg.GetWJ_Agent_WetNetLinkBox_Config();
            // Carico i parametri della configurazione
            job_sleep_time = config.execution_interval_minutes * 60 * 1000;
            wetnet_dsn = cfg.GetWetDBDSN();
            wet_ftp = new WetFTP(config.ftp_server_name, config.ftp_server_port,
                config.use_ssl, config.is_passive_connection,
                config.username, config.password, config.folder);
            // Istanzio le connessioni
            wet_db = new WetDBConn(wetnet_dsn, null, null, true);
        }

        /// <summary>
        /// Corpo del Job
        /// </summary>
        protected override void DoJob()
        {
            // Se il Job non è abilitato esco
            if (!config.enabled)
                return;

            try
            {
                string last_device = "";
                DataTable dt_dev = new DataTable();

                // Acquisisco solo i wlb configurati nell'ambiente
                List<string> wlbs = new List<string>();
                DataTable wlbs_dt = wet_db.ExecCustomQuery("SELECT DISTINCT `table_relational_id_value` FROM measures WHERE `table_relational_id_value` LIKE 'wlb%' ORDER BY `table_relational_id_value` ASC");
                foreach (DataRow dr in wlbs_dt.Rows)
                    wlbs.Add(Convert.ToString(dr["table_relational_id_value"]).ToLower());
                // Acquisisco tutte le path della base FTP
                string[] folders = wet_ftp.GetFolderTree(wet_ftp.GetCurrentFolder());
                // Ciclo per tutte le cartelle alla ricerca di files validi
                foreach (string folder in folders)
                {
                    try
                    {
                        // Elenco i files della cartella
                        List<string> files = wet_ftp.ListFiles(folder).ToList();
                        // Ordino i files per nome in ordine crescente
                        files.Sort();
                        // Ciclo per tutti i files scremando quelli non necessari
                        Dictionary<string, string> device_lastfile = new Dictionary<string, string>();
                        foreach (string file in files)
                        {
                            // Controllo che il nome abbia estensione CSV
                            if (file.Length < 5)
                                continue;
                            if (file.Substring(file.Length - 4, 4).ToLower() != ".csv")
                                continue;
                            // Se il file non contiene separatori è segno che non è valido
                            if (!file.Any(x => (x == '-') || (x == '_')))
                                continue;
                            // Determino il tipo di separatore per il file
                            char fsep = '-';
                            if (file.Contains('_'))
                                fsep = '_';
                            // Determino il tipo di separatore per il formato CSV
                            char separator = fsep == '-' ? ';' : '\t';
                            // Determino il nome del misuratore
                            string device_name = file.Substring(0, file.IndexOf(fsep)).ToLower();
                            // Se il device non è fondamentale, lo scarto
                            if (!wlbs.Any(x => x == device_name))
                                continue;
                            if (device_name != last_device)
                            {
                                // Controllo la coerenza dei dati
                                dt_dev = wet_db.ExecCustomQuery("SELECT * FROM wlb_data WHERE `wlb_identities_wlb_name` = '" + device_name + "' ORDER BY `timestamp` DESC LIMIT 1");
                                last_device = device_name;
                            }
                            // Se il device è già stato processato continuo
                            if (!device_lastfile.ContainsKey(device_name))
                            {
                                // Controllo se lo strumento esiste nel database
                                DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM wlb_identities WHERE `wlb_name` = '" + device_name + "'");
                                if (dt.Rows.Count == 0)
                                {
                                    // Se il device non esiste, lo creo
                                    wet_db.ExecCustomCommand("INSERT INTO wlb_identities VALUES ('" + device_name + "', '', '')");
                                    // Aggiorno il dizionario
                                    device_lastfile.Add(device_name, string.Empty);
                                }
                                else
                                {
                                    // Il device esiste già nel db, acquisisco l'ultimo file processato
                                    device_lastfile.Add(device_name, Convert.ToString(dt.Rows[0]["last_processed_filename"]));
                                }
                            }
                            // Controllo se il file è da analizzare, l'indice nella lista ordinata deve essere maggiore
                            if (device_lastfile[device_name] != string.Empty)
                            {                               
                                if (dt_dev.Rows.Count == 1)
                                {
                                    DateTime ts = Convert.ToDateTime(dt_dev.Rows[0]["timestamp"]);
                                    DateTime processed = GetDTFromFilename(file);
                                    if (ts > processed)
                                        continue;   // Ci sono già dati più recenti del file
                                    if (files.IndexOf(file) <= files.IndexOf(device_lastfile[device_name]))
                                        continue;
                                }
                            }
                            // Il file è valido, eseguo il download
                            DataTable file_dt = wet_ftp.DownloadCSVFileToTable(folder + "/" + file, separator, 1, 7, "MM/dd/yy HH:mm:ss", false);
                            // Aggiungo il campo di riferimento
                            file_dt.Columns.Add(new DataColumn("wlb_identities_wlb_name", typeof(string), "'" + device_name + "'"));
                            // Inserisco i dati in tabella
                            wet_db.TableInsert(file_dt, "wlb_data");
                            // Aggiorno il campo dell'ultimo file processato
                            wet_db.ExecCustomCommand("UPDATE wlb_identities SET `last_processed_filename` = '" + file + "' WHERE `wlb_name` = '" + device_name + "'");
                        }
                    }
                    catch (Exception ex1)
                    {
                        WetDebug.GestException(ex1);
                    }
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

        }

        #endregion
    }
}

#endregion

#region NEW_BUT_BAD

/*
namespace WetLib
{
    /// <summary>
    /// Job per il trattamento dei dati degli WetNet Link Box (WLB)
    /// </summary>
    sealed class WJ_Agent_WetNetLinkBox : WetJob
    {
        #region Costanti

        /// <summary>
        /// Numero di tentativi
        /// </summary>
        const int RETRIES = 5;

        #endregion

        #region Istanze

        /// <summary>
        /// Configurazione
        /// </summary>
        WetConfig cfg;

        /// <summary>
        /// Connessione al database WetNet
        /// </summary>
        WetDBConn wet_db;

        /// <summary>
        /// Connessione FTP
        /// </summary>
        WetFTP wet_ftp;

        #endregion

        #region Variabili globali

        /// <summary>
        /// DSN ODBC del database WetNet
        /// </summary>
        string wetnet_dsn;

        /// <summary>
        /// Configurazione del job
        /// </summary>
        WetConfig.WJ_Agent_WLB_Config_Struct config;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Agent_WetNetLinkBox()
        {
            // Millisecondi di attesa fra le esecuzioni
            job_sleep_time = WetConfig.GetInterpolationTimeMinutes() * 60 * 1000;
            // Istanzio la configurazione
            cfg = new WetConfig();
        }

        #endregion

        #region Funzioni del Job

        /// <summary>
        /// Funzione di caricamento
        /// </summary>
        protected override void Load()
        {
            // Carico la configurazione
            config = cfg.GetWJ_Agent_WetNetLinkBox_Config();
            // Carico i parametri della configurazione
            job_sleep_time = config.execution_interval_minutes * 60 * 1000;
            wetnet_dsn = cfg.GetWetDBDSN();
            wet_ftp = new WetFTP(config.ftp_server_name, config.ftp_server_port,
                config.use_ssl, config.is_passive_connection,
                config.username, config.password, config.folder);
            // Istanzio le connessioni
            wet_db = new WetDBConn(wetnet_dsn, null, null, true);
        }

        /// <summary>
        /// Corpo del Job
        /// </summary>
        protected override void DoJob()
        {
            bool tree_ok = false;
            int retries_folder;
            int retries_subfolder;
            int retries_file;

            // Se il Job non è abilitato esco
            if (!config.enabled)
                return;
            
            try
            {
                List<string> folders = new List<string>();
                retries_folder = RETRIES;
                do
                {
                    try
                    {
                        // Acquisisco tutte le path della base FTP
                        folders = wet_ftp.GetFolderTree(wet_ftp.GetCurrentFolder()).ToList();
                        tree_ok = true;
                    }
                    catch (Exception ex3)
                    {
                        WetDebug.GestException(ex3, "GetFolderTree");
                        retries_folder--;
                        Sleep(10000);
                    }
                } while ((!tree_ok) && (retries_folder > 0));
                // Ciclo per tutte le cartelle alla ricerca di files validi
                retries_subfolder = RETRIES;
                for (int jj = 0; jj < folders.Count; jj++)
                {
                    string folder = folders[jj];
                    try
                    {
                        // Elenco i files della cartella
                        List<string> files = wet_ftp.ListFiles(folder).ToList();
                        // Ordino i files per nome in ordine crescente
                        files.Sort();
                        // Ciclo per tutti i files scremando quelli non necessari
                        Dictionary<string, string> device_lastfile = new Dictionary<string, string>();
                        retries_file = RETRIES;
                        for (int ii = 0; ii < files.Count; ii++)
                        {
                            string file = files[ii];
                            try
                            {
                                // Controllo che il nome abbia estensione CSV
                                if (file.Length < 5)
                                    continue;
                                if (file.Substring(file.Length - 4, 4).ToLower() != ".csv")
                                    continue;
                                // Se il file non contiene separatori è segno che non è valido
                                if (!file.Any(x => (x == '-') || (x == '_')))
                                    continue;
                                // Determino il tipo di separatore per il file
                                char fsep = '-';
                                if (file.Contains('_'))
                                    fsep = '_';
                                // Determino il tipo di separatore per il formato CSV
                                char separator = fsep == '-' ? ';' : '\t';
                                // Determino il nome del misuratore
                                string device_name = file.Substring(0, file.IndexOf(fsep)).ToLower();
                                // Se il device è già stato processato continuo
                                if (!device_lastfile.ContainsKey(device_name))
                                {
                                    // Controllo se lo strumento esiste nel database
                                    DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM wlb_identities WHERE `wlb_name` = '" + device_name + "'");
                                    if (dt.Rows.Count == 0)
                                    {
                                        // Se il device non esiste, lo creo
                                        wet_db.ExecCustomCommand("INSERT INTO wlb_identities VALUES ('" + device_name + "', '', '')");
                                        // Aggiorno il dizionario
                                        device_lastfile.Add(device_name, string.Empty);
                                    }
                                    else
                                    {
                                        // Il device esiste già nel db, acquisisco l'ultimo file processato
                                        device_lastfile.Add(device_name, Convert.ToString(dt.Rows[0]["last_processed_filename"]));
                                    }
                                }
                                // Controllo se il file è da analizzare, l'indice nella lista ordinata deve essere maggiore
                                if (device_lastfile[device_name] != string.Empty)
                                {
                                    if (files.IndexOf(file) <= files.IndexOf(device_lastfile[device_name]))
                                        continue;
                                }
                                // Il file è valido, eseguo il download
                                DataTable file_dt = wet_ftp.DownloadCSVFileToTable(folder + "/" + file, separator, 1, 7, "MM/dd/yy HH:mm:ss", false);
                                // Aggiungo il campo di riferimento
                                file_dt.Columns.Add(new DataColumn("wlb_identities_wlb_name", typeof(string), "'" + device_name + "'"));
                                // Inserisco i dati in tabella
                                wet_db.TableInsert(file_dt, "wlb_data");
                                // Aggiorno il campo dell'ultimo file processato
                                wet_db.ExecCustomCommand("UPDATE wlb_identities SET `last_processed_filename` = '" + file + "' WHERE `wlb_name` = '" + device_name + "'");
                                // Aggiorno il contatore
                                retries_file = RETRIES;
                                // Attendo...
                                Sleep(1000);
                            }
                            catch (Exception ex2)
                            {
                                WetDebug.GestException(ex2, file);
                                if (retries_file > 0)
                                {
                                    retries_file--;
                                    ii--;
                                    Sleep(10000);
                                }
                                else
                                    retries_file = RETRIES;
                            }
                        }
                        // Aggiorno il contatore
                        retries_subfolder = RETRIES;
                    }
                    catch (Exception ex1)
                    {
                        WetDebug.GestException(ex1, folder);
                        if (retries_subfolder > 0)
                        {
                            retries_subfolder--;
                            jj--;
                            Sleep(10000);
                        }
                        else
                            retries_subfolder = RETRIES;
                    }
                }
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }
            
        }

        #endregion
    }
}
*/

#endregion