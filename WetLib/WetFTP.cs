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
            if (folder == string.Empty)
                this.folder = "/";
            else
                this.folder = folder.Last() == '/' ? folder : folder + '/';
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
            string path = FTPAPI_Initialize().RequestUri.AbsolutePath;

            if (path.Last() == '/')
                path = path.Remove(path.Length - 1);

            return path;
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
                    dirs.Add(dir.Split(new char[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries).Last());
                if (dir.Contains("<DIR>"))
                    dirs.Add(dir.Remove(0, dir.IndexOf("<DIR>") + 5).TrimStart(new char[] { ' ' }));
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
                if ((dir[0] != 'd') && (dir[0] != 'l'))
                    files.Add(dir.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3));
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

            string basenode = "/";
            if (base_node.Length > 0)
            {
                if (base_node.Last() == '/')
                    basenode = base_node;
                else
                    basenode = base_node + '/';
            }
            nodes.Add(basenode);

            int depth = basenode.Count(x => x == '/');
            if (depth < MAX_FOLDER_DEEP)
            {
                string[] sub_nodes = ListSubFolders(basenode);
                for (int ii = 0; ii < sub_nodes.Length; ii++)
                {
                    if ((sub_nodes[ii] != ".") && (sub_nodes[ii] != ".."))
                    {
                        sub_nodes[ii] = base_node + "/" + sub_nodes[ii];
                        nodes.Add(sub_nodes[ii]);
                        nodes.AddRange(GetFolderTree(sub_nodes[ii]));
                    }
                }
            }

            return nodes.ToArray();

            /*** OLD 29/03/2017 --- Modifica fatta x Fiora ***
            List<string> nodes = new List<string>();

            string basenode = "/";
            if (base_node.Length > 0)
            {
                if (basenode.Last() == '/')
                    basenode = base_node;
                else
                    basenode = base_node + '/';
            }
            nodes.Add(basenode);

            int depth = base_node.Count(x => x == '/');
            if (depth < MAX_FOLDER_DEEP)
            {
                string[] sub_nodes = ListSubFolders(base_node);
                for (int ii = 0; ii < sub_nodes.Length; ii++)
                {
                    if ((sub_nodes[ii] != ".") && (sub_nodes[ii] != ".."))
                    {
                        sub_nodes[ii] = base_node + "/" + sub_nodes[ii];
                        nodes.Add(sub_nodes[ii]);
                        nodes.AddRange(GetFolderTree(sub_nodes[ii]));
                    }
                }
            }

            return nodes.ToArray();
            *** END OLD ***/
        }

        /// <summary>
        /// Scarica un file CSV e lo archivia in una tabella di valori
        /// </summary>
        /// <param name="file_name">Nome del file con percorso completo</param>
        /// <param name="separator">Separatore del CSV</param>
        /// <param name="number_of_headerlines_to_exclude">Numero di righe di intestazione da saltare</param>
        /// <param name="max_cols">Numero massimo di colonne della tabella</param>
        /// <param name="datetime_format_str">Formato della data e ora</param>
        /// <param name="has_separated_date_time">Indica se la data e ora sono separate</param>
        /// <returns>Tabella dati</returns>
        public DataTable DownloadCSVFileToTable(string file_name, char separator, int number_of_headerlines_to_exclude, int max_cols, 
            string datetime_format_str, bool has_separated_date_time)
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
                string[] lines = sr.ReadToEnd().Replace("\0", "").Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines)
                {
                    // Controllo se devo saltarla
                    if (line_cnt < number_of_headerlines_to_exclude)
                    {
                        line_cnt++;
                        continue;
                    }
                    // Unifico data e ora se necessario rimuovendo il carattere di separazione
                    string tmp_line = line;
                    if (has_separated_date_time)
                    {
                        int idx = line.IndexOf(separator, 0);
                        tmp_line = line.Remove(idx, 1).Insert(idx, " ");
                    }
                    // Separo le colonne
                    string[] vals = tmp_line.Split(new char[] {separator}, max_cols, StringSplitOptions.RemoveEmptyEntries);                    
                    // Tento l'interpretazione dei dati
                    try
                    {
                        DataRow dr = dt.NewRow();
                        for (int ii = 0; ii < max_cols; ii++)
                        {
                            if (ii < vals.Length)
                            {
                                // Tolgo doppi apici se presenti
                                vals[ii] = vals[ii].Replace("\"", "");
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
                                    if (vals[ii] == string.Empty)
                                        tmp = 0.0d;
                                    else
                                    {
                                        double.TryParse(vals[ii], NumberStyles.Any, CultureInfo.InvariantCulture, out tmp);
                                        if (double.IsInfinity(tmp) || double.IsNaN(tmp) || double.IsNegativeInfinity(tmp) || double.IsPositiveInfinity(tmp))
                                            tmp = double.NaN;
                                    }
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

        /// <summary>
        /// Crea la cartella "folder" a partire da "base_folder"
        /// </summary>
        /// <param name="base_folder">Directory di base</param>
        /// <param name="folder">Nuova sottodirectory</param>
        /// <returns>xxx</returns>
        public string CreateFolder(string base_folder, string folder)
        {          
            // Provo a creare la cartella
            if (base_folder.Last() != '/')
                base_folder += '/';
            string resp = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.MakeDirectory, base_folder + folder);

            return resp;
        }

        /// <summary>
        /// Sposta un file
        /// </summary>
        /// <param name="source_file_name"></param>
        /// <param name="source_path"></param>
        /// <param name="dest_file_name"></param>
        /// <param name="dest_path"></param>
        /// <returns></returns>
        public string MoveFile(string source_file_name, string source_path, string dest_file_name, string dest_path)
        {
            // Controllo le stringhe dei percorsi
            if (source_path.Last() != '/')
                source_path += '/';
            if (dest_path.Last() != '/')
                dest_path += '/';
            // Aggiungo il nome del file ai percorsi
            source_path += source_file_name;
            dest_path += dest_file_name;
            // Eseguo il download del file
            FtpWebRequest ftp = FTPAPI_Initialize(source_file_name);
            ftp.Method = WebRequestMethods.Ftp.DownloadFile;
            byte[] file_buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                ftp.GetResponse().GetResponseStream().CopyTo(ms);
                file_buffer = ms.ToArray();
            }
            // Eseguo l'UpLoad del file nella nuova destinazione
            ftp = FTPAPI_Initialize(dest_file_name);
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            using (MemoryStream ms = new MemoryStream(file_buffer))
            {
                ms.CopyTo(ftp.GetRequestStream());
            }
            // Elimino il file sorgente
            string resp = FTPAPI_GetStringQuery(WebRequestMethods.Ftp.DeleteFile, source_file_name);

            return resp;
        }

        #endregion
    }
}
