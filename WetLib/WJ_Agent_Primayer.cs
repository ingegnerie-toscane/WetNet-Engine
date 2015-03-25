using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WetLib
{
    /// <summary>
    /// Classe per la gestione dei device Primayer via FTP
    /// </summary>
    class WJ_Agent_Primayer : WetJob
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
        WetConfig.WJ_Agent_Primayer_Config_Struct config;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Agent_Primayer()
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
            config = cfg.GetWJ_Agent_Primayer_Config();
            // Carico i parametri della configurazione
            wetnet_dsn = cfg.GetWetDBDSN();
            wet_ftp = new WetFTP(config.ftp_server_name, config.ftp_server_port,
                config.use_ssl, config.is_passive_connection,
                config.username, config.password, config.folder);
            // Istanzio le connessioni
            wet_db = new WetDBConn(wetnet_dsn, true);
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
                // Acquisisco tutte le path della base FTP
                string[] folders = wet_ftp.GetFolderTree(wet_ftp.GetCurrentFolder());
                // Ciclo per tutte le cartelle alla ricerca di files validi
                foreach (string folder in folders)
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
                        if (!file.Any(x => x == '_'))
                            continue;
                        // Determino il nome del misuratore
                        string device_name = file.Substring(0, file.IndexOf('_')).ToLower();
                        // Se il device è già stato processato continuo
                        if (!device_lastfile.ContainsKey(device_name))
                        {
                            // Controllo se lo strumento esiste nel database
                            DataTable dt = wet_db.ExecCustomQuery("SELECT * FROM primayer_identities WHERE `primayer_name` = '" + device_name + "'");
                            if (dt.Rows.Count == 0)
                            {
                                // Se il device non esiste, lo creo
                                wet_db.ExecCustomCommand("INSERT INTO primayer_identities VALUES ('" + device_name + "', '', '')");
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
                        DataTable file_dt = wet_ftp.DownloadCSVFileToTable(folder + "/" + file, ';', 2, 7, "dd/MM/yyyy HH:mm:ss", true);
                        // Aggiungo il campo di riferimento
                        file_dt.Columns.Add(new DataColumn("primayer_identities_primayer_name", typeof(string), "'" + device_name + "'"));
                        // Inserisco i dati in tabella
                        wet_db.TableInsert(file_dt, "primayer_data");
                        // Aggiorno il campo dell'ultimo file processato
                        wet_db.ExecCustomCommand("UPDATE primayer_identities SET `last_processed_filename` = '" + file + "' WHERE `primayer_name` = '" + device_name + "'");
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
