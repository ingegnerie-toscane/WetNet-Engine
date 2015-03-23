using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Data;
using System.Globalization;

namespace WetLib
{
    /// <summary>
    /// Classe per la gestione delle connessioni FTP
    /// </summary>
    sealed class WetFTP
    {
        #region Costanti

        /// <summary>
        /// profondità massima sottocartelle
        /// </summary>
        const int MAX_FOLDER_DEEP = 16;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Nome host o indirizzo IP del server FTP
        /// </summary>
        readonly string server;

        /// <summary>
        /// Porta TCP del server FTP
        /// </summary>
        readonly int port;

        /// <summary>
        /// Indica se utilizzare SLL per la connessione
        /// </summary>
        readonly bool use_ssl;

        /// <summary>
        /// Indica se utilizzare la modalità passiva
        /// </summary>
        readonly bool passive;

        /// <summary>
        /// Nome utente
        /// </summary>
        readonly string username;

        /// <summary>
        /// Password
        /// </summary>
        readonly string password;

        /// <summary>
        /// Cartella radice
        /// </summary>
        readonly string folder;

        /// <summary>
        /// URI corrente
        /// </summary>
        string current_uri;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="server">Nome host o indirizzo IP del server FTP</param>
        /// <param name="port">Porta TCP del server FTP</param>
        /// <param name="use_ssl">Indica se utilizzare SLL per la connessione</param>
        /// <param name="passive">Indica se utilizzare la modalità passiva</param>
        /// <param name="username">Nome utente</param>
        /// <param name="password">Password</param>
        /// <param name="folder">Cartella radice</param>
        public WetFTP(string server, int port, bool use_ssl, bool passive, string username, string password, string folder)
        {
            // Assegnazione dei campi
            this.server = server;
            this.port = port;
            this.use_ssl = use_ssl;
            this.passive = passive;
            this.username = username;
            this.password = password;
            this.folder = folder;
            // Inizializzazione
            FTPAPI_Initialize();
        }

        #endregion

        #region API

        /// <summary>
        /// Inizializzazione FTP
        /// </summary>
        FtpWebRequest FTPAPI_Initialize(string new_folder = "")
        {
            FtpWebRequest ftp;

            // Carico la cartella di destinazione
            string fld = new_folder == string.Empty ? folder : new_folder;
            // Controllo di congruenza con le directory
            if (fld == null)
                fld = "/";
            if (fld == string.Empty)
                fld = "/";
            else if (fld.Length == 1)
            {
                if (fld[0] != '/')
                    fld = "/";
            }
            else
            {
                if (fld[0] != '/')
                    fld = fld.Insert(0, "/");
            }
            // Creo l'istanza
            ftp = (FtpWebRequest)WebRequest.Create("ftp://" + server + ":" + port + fld);
            ftp.Credentials = new NetworkCredential(username, password);
            ftp.EnableSsl = use_ssl;
            ftp.UsePassive = passive;
            ftp.UseBinary = true;
            // Cambio l'uri corrente, non inizializzo per non creare una race-condition
            current_uri = ftp.RequestUri.AbsoluteUri;

            return ftp;
        }

        /// <summary>
        /// Effettua una query che ritorna una stringa
        /// </summary>
        /// <param name="method">Metodo della richiesta</param>
        /// <returns>Stringa restituita</returns>
        string FTPAPI_GetStringQuery(string method, string base_folder = "")
        {
            string ret;

            FtpWebRequest ftp = FTPAPI_Initialize(base_folder);
            ftp.Method = method;
            using (FtpWebResponse resp = (FtpWebResponse)ftp.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.ASCII))
                {
                    ret = sr.ReadToEnd();
                }
            }

            return ret;
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce la directory di lavoro corrente
        /// </summary>        
        /// <returns>Directory corrente</returns>
        public string GetCurrentFolder()
        {
            return FTPAPI_GetStringQuery(WebRequestMethods.Ftp.PrintWorkingDirectory);
        }

        /// <summary>
        /// Elenca le sotto directories
        /// </summary>
        /// <returns>Lista delle sotto directories</returns>
        public string[] ListSubFolders(string base_folder = "")
        {
            List<string> dirs = new List<string>();
            string query;

            if(base_folder != string.Empty)            
                query = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.ListDirectoryDetails, base_folder);
            else
                query = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.ListDirectoryDetails);
            string[] complete_dirs = query.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dir in complete_dirs)
            {
                if (dir[0] == 'd')
                    dirs.Add(dir.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(8));
            }

            return dirs.ToArray();
        }

        /// <summary>
        /// Elenca i files in una directory
        /// </summary>
        /// <param name="base_folder">Directory base</param>
        /// <returns>Lista dei files</returns>
        public string[] ListFiles(string base_folder = "")
        {
            List<string> files = new List<string>();
            string query;

            if (base_folder != string.Empty)
                query = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.ListDirectoryDetails, base_folder);
            else
                query = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.ListDirectoryDetails);
            string[] complete_dirs = query.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dir in complete_dirs)
            {
                if (dir[0] == '-')
                    files.Add(dir.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(8));
            }

            return files.ToArray();
        }

        /// <summary>
        /// Cambia la directory corrente
        /// </summary>
        /// <param name="new_folder">Nuova directory</param>
        /// <returns>Directory corrente</returns>
        public string ChangeFolder(string new_folder)
        {
            FTPAPI_Initialize(new_folder);

            return current_uri;
        }

        /// <summary>
        /// Acquisisco l'albero delle directory
        /// </summary>
        /// <returns></returns>
        public string[] GetFolderTree(string base_node)
        {
            List<string> nodes = new List<string>();

            int depth = base_node.Count(x => x == '/');
            if (depth < MAX_FOLDER_DEEP)
            {
                string[] sub_nodes = ListSubFolders(base_node);                
                Parallel.For(0, sub_nodes.Length, ii =>
                {
                    if ((sub_nodes[ii] != ".") && (sub_nodes[ii] != ".."))
                    {
                        sub_nodes[ii] = base_node + "/" + sub_nodes[ii];
                        nodes.Add(sub_nodes[ii]);
                        nodes.AddRange(GetFolderTree(sub_nodes[ii]));
                    }
                });
            }

            return nodes.ToArray();
        }

        /// <summary>
        /// Scarica un file CSV e lo archivia in una tabella di valori
        /// </summary>
        /// <param name="file_name">Nome del file con percorso completo</param>
        /// <param name="separator">Separatore del CSV</param>
        /// <param name="number_of_headerlines_to_exclude">Numero di righe di intestazione da saltare</param>
        /// <param name="max_cols">Numero massimo di colonne della tabella</param>
        /// <param name="datetime_format_str">Formato della data e ora</param>
        /// <returns>Tabella dati</returns>
        public DataTable DownloadCSVFileToTable(string file_name, char separator, int number_of_headerlines_to_exclude, int max_cols, 
            string datetime_format_str)
        {
            DataTable dt = new DataTable();

            // Creo i campi della tabella
            dt.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            for (int ii = 0; ii < max_cols - 1; ii++)
                dt.Columns.Add(new DataColumn("value" + ii.ToString(), typeof(double)));

            FtpWebRequest ftp = FTPAPI_Initialize(file_name);
            ftp.Method = WebRequestMethods.Ftp.DownloadFile;

            using (StreamReader sr = new StreamReader(ftp.GetResponse().GetResponseStream()))
            {
                List<string> rows = new List<string>();
                int line_cnt = 0;
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines)
                {
                    // Controllo se devo saltarla
                    if (line_cnt < number_of_headerlines_to_exclude)
                    {
                        line_cnt++;
                        continue;
                    }
                    // Separo le colonne
                    string[] vals = line.Split(new char[] {separator}, max_cols, StringSplitOptions.RemoveEmptyEntries);                    
                    // Tento l'interpretazione dei dati
                    try
                    {
                        DataRow dr = dt.NewRow();
                        for (int ii = 0; ii < max_cols; ii++)
                        {
                            if (ii < vals.Length)
                            {
                                // Tolgo doppi apici se presenti
                                vals[ii] = vals[ii].Trim(new char[] { '"' });
                            }
                            if (ii == 0)
                            {
                                DateTime timestamp;
                                bool res = DateTime.TryParseExact(vals[ii], datetime_format_str, CultureInfo.InvariantCulture, DateTimeStyles.None, out timestamp);
                                if (res == false)
                                    throw new Exception("Invalid CSV record!");
                                dr["timestamp"] = timestamp;
                            }
                            else
                            {
                                double tmp;
                                if (ii < vals.Length)
                                {
                                    vals[ii] = vals[ii].Replace(",", ".");
                                    bool res = double.TryParse(vals[ii], NumberStyles.Any, CultureInfo.InvariantCulture, out tmp);
                                    if (res == false)
                                        continue;
                                    if (double.IsInfinity(tmp) || double.IsNaN(tmp) || double.IsNegativeInfinity(tmp) || double.IsPositiveInfinity(tmp))
                                        tmp = 0.0d;
                                }
                                else
                                    tmp = 0.0d;
                                dr["value" + (ii - 1).ToString()] = tmp;
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    catch { }
                }
            }

            return dt;
        }

        #endregion
    }
}
