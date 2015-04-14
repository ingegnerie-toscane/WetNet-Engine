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
using System.Data.Odbc;
using Microsoft.Win32;
using System.Threading;

namespace WetLib
{
    public sealed class WetDBConn
    {
        #region Costanti

        /// <summary>
        /// Numero massimo di records inseribili in una singola query di insert
        /// </summary>
        const int MAX_INSERT_RECORDS = 255;

        /// <summary>
        /// Stringa di formato data/ora di MySQL
        /// </summary>
        public const string MYSQL_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Stringa di formato data di MySQL
        /// </summary>
        public const string MYSQL_DATE_FORMAT = "yyyy-MM-dd";

        /// <summary>
        /// Stringa di formato tempo di MySQL
        /// </summary>
        public const string MYSQL_TIME_FORMAT = @"hh\:mm\:ss";

        #endregion

        #region Enumerazioni

        /// <summary>
        /// Tipo di server
        /// </summary>
        public enum DBServerTypes : int
        {
            /// <summary>
            /// Sconosciuto
            /// </summary>
            UNKNOWN = 0,

            /// <summary>
            /// MySQL
            /// </summary>
            MYSQL = 1,

            /// <summary>
            /// SQL Server
            /// </summary>
            SQLSERVER = 2,

            /// <summary>
            /// Oracle
            /// </summary>
            ORACLE = 3
        }

        /// <summary>
        /// Tipi di dati per la colonna della chiave primaria
        /// </summary>
        public enum PrimaryKeyColumnTypes : int
        {
            /// <summary>
            /// Sconosciuto
            /// </summary>
            UNKNOWN = 0,

            /// <summary>
            /// Tipo testo
            /// </summary>
            TEXT = 1,

            /// <summary>
            /// Tipo intero
            /// </summary>
            INT = 2,

            /// <summary>
            /// Tipo a virgola mobile
            /// </summary>
            REAL = 3,

            /// <summary>
            /// Tipo data e ora
            /// </summary>
            DATETIME = 4,

            /// <summary>
            /// Tipo data
            /// </summary>
            DATE = 5,

            /// <summary>
            /// Tipo ora
            /// </summary>
            TIME = 6
        }

        /// <summary>
        /// Tipi di ordinamento
        /// </summary>
        public enum OrderTypes : int
        {
            /// <summary>
            /// Ordinamento ascendente
            /// </summary>
            ASC = 0,

            /// <summary>
            /// Ordinamento discendente
            /// </summary>
            DESC = 1
        }

        /// <summary>
        /// Providers di dati
        /// </summary>
        public enum ProviderType : int
        {
            /// <summary>
            /// Database generico MySQL (x es.: Movicon)
            /// </summary>
            GENERIC_MYSQL = 0,

            /// <summary>
            /// SQL Server con Archestra
            /// </summary>
            ARCHESTRA_SQL = 1,

            /// <summary>
            /// SQL Server con IFix
            /// </summary>
            IFIX_SQL = 2
        }

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database ODBC
        /// </summary>
        OdbcConnection conn;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="odbc_dsn">DSN ODBC</param>
        /// <param name="username">Nome utente</param>
        /// <param name="password">Password</param>
        /// <param name="mysql_required">Indica se il database deve essere MySQL</param>
        public WetDBConn(string odbc_dsn, string username, string password, bool mysql_required)
        {
            string connection_string = "DSN=" + odbc_dsn;
            if (username != null)
                connection_string += "; Uid=" + username;
            if (password != null)
                connection_string += "; Pwd=" + password;
            // Istanzio la connessione
            conn = new OdbcConnection(connection_string);
            conn.Open();
            // Controllo che faccia riferimento ad un database MySQL
            if (mysql_required && (GetServerType() != DBServerTypes.MYSQL))
                throw new Exception("MySQL ODBC Driver requested!");
        }

        #endregion

        #region Funzioni statiche

        /// <summary>
        /// Restituisce tutti i DSN
        /// </summary>
        /// <returns>Lista dei DSN</returns>
        public static string[] GetDSNs()
        {
            List<string> dsns = new List<string>();
            RegistryKey rkey = null;

            try
            {
                // DSN utente
                rkey = Registry.CurrentUser.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
                dsns.AddRange(rkey.GetValueNames());
            }
            catch (Exception ex) 
            {
                WetDebug.GestException(ex);
            }

            try
            {
                // DSN di sistema
                rkey = Registry.LocalMachine.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
                dsns.AddRange(rkey.GetValueNames());
            }
            catch (Exception ex) 
            {
                WetDebug.GestException(ex);
            }

            return dsns.ToArray();
        }

        #endregion

        #region Funzioni del modulo

        #region Primitive

        /// <summary>
        /// Restituisce il dataset per una tabella
        /// </summary>
        /// <param name="table_name">Nome della tabella</param>
        /// <returns>Dataset restituito</returns>
        public DataSet GetDataset(string table_name)
        {           
            OdbcDataAdapter da = new OdbcDataAdapter("SELECT * FROM " + table_name, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        /// <summary>
        /// Esegue una query personalizzata e ritorna una tabella di valori
        /// </summary>
        /// <param name="query">Query da eseguire</param>
        /// <returns>Tabella restituita</returns>
        public DataTable ExecCustomQuery(string query)
        {
            DataTable dt = new DataTable();

            using (OdbcCommand cmd = new OdbcCommand(query, conn))
            {
                cmd.CommandTimeout = 360;   // 6 minuti
                using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                {                    
                    da.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// Eseguo un comando personalizzato
        /// </summary>
        /// <param name="command">Comando</param>
        /// <returns>Numero di records interessati</returns>
        public int ExecCustomCommand(string command)
        {
            OdbcCommand cmd = new OdbcCommand(command, conn);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Restituisce la lista delle tabelle nel database corrente
        /// </summary>
        /// <returns>Lista delle tabelle</returns>
        public string[] GetTables()
        {
            List<string> tables = new List<string>();

            DataTable dt = ExecCustomQuery("SHOW TABLES");
            if (dt.Rows.Count > 0)
                foreach (DataRow dr in dt.Rows)
                    tables.Add(Convert.ToString(dr[0]));

            return tables.ToArray();
        }

        /// <summary>
        /// Inserisce i valori di una tabella in un'altra tabella
        /// </summary>
        /// <param name="source_table">Tabella da inserire</param>
        /// <param name="dest_table">Nome della tabella di destinazione</param>
        /// <returns>Numero di record inseriti</returns>
        public int TableInsert(DataTable source_table, string dest_table)
        {
            int insert_count = 0;

            if (source_table.Rows.Count > 0)
            {
                int n_query;

                // Calcolo il numero delle query da eseguire
                n_query = (source_table.Rows.Count / MAX_INSERT_RECORDS) +
                    ((source_table.Rows.Count % MAX_INSERT_RECORDS) == 0 ? 0 : 1);

                // Ciclo per tutte le query da inserire
                for (int qq = 0; qq < n_query; qq++)
                {
                    // Colcolo la riga di partenza e il numero delle righe
                    int base_row = qq * MAX_INSERT_RECORDS;
                    int n_row = (source_table.Rows.Count - base_row) > MAX_INSERT_RECORDS ? MAX_INSERT_RECORDS : (source_table.Rows.Count - base_row);

                    // Compilo la query
                    string cmd = "INSERT INTO " + dest_table + " (";
                    for (int ii = 0; ii < source_table.Columns.Count; ii++)
                    {
                        cmd += source_table.Columns[ii].ColumnName;
                        if (ii < (source_table.Columns.Count - 1))
                            cmd += ", ";
                    }
                    cmd += ") VALUES ";
                    for (int ii = base_row; ii < (base_row + n_row); ii++)
                    {
                        cmd += "(";
                        for (int jj = 0; jj < source_table.Columns.Count; jj++)
                        {
                            string val_str;

                            // Acquisisco il valore
                            if (source_table.Columns[jj].DataType == typeof(DateTime))
                                val_str = Convert.ToDateTime(source_table.Rows[ii][jj]).ToString(WetDBConn.MYSQL_DATETIME_FORMAT);
                            else
                                val_str = Convert.ToString(source_table.Rows[ii][jj]);

                            // Aggiungo gli apici se necessario
                            if (((source_table.Columns[jj].DataType == typeof(string)) ||
                                (source_table.Columns[jj].DataType == typeof(TimeSpan)) ||
                                (source_table.Columns[jj].DataType == typeof(DateTime))
                                ) && (val_str != null))
                            {
                                val_str = val_str.Insert(0, "'");
                                val_str += "'";
                            }
                            else if ((source_table.Columns[jj].DataType == typeof(float)) ||
                                (source_table.Columns[jj].DataType == typeof(double)) ||
                                (source_table.Columns[jj].DataType == typeof(decimal)))
                            {
                                val_str = val_str.Replace(',', '.');
                            }

                            // Inserisco valori di default
                            if (val_str == string.Empty)
                            {
                                switch (source_table.Columns[jj].DataType.Name)
                                {
                                    case "Byte":
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        val_str = "0";
                                        break;

                                    case "TimeSpan":
                                        val_str = DateTime.Now.ToString(WetDBConn.MYSQL_DATETIME_FORMAT);
                                        break;

                                    case "Decimal":
                                    case "Single":
                                    case "Double":
                                        val_str = "0.0";
                                        break;
                                }
                            }

                            // Aggiungo il valore al comando
                            if (val_str == null)
                                cmd += "NULL";
                            else
                                cmd += val_str;

                            // Aggiungo la virgola se non sono a fine lista
                            if (jj < (source_table.Columns.Count - 1))
                                cmd += ",";
                        }
                        cmd += ")";
                        if (ii < (base_row + n_row - 1))
                            cmd += ", ";
                    }

                    cmd += " ON DUPLICATE KEY UPDATE ";

                    for (int ii = 0; ii < source_table.Columns.Count; ii++)
                    {
                        cmd += source_table.Columns[ii].ColumnName + "=VALUES(" + source_table.Columns[ii].ColumnName + ")";
                        if (ii < source_table.Columns.Count - 1)
                            cmd += ",";
                    }

                    // Eseguo il comando
                    OdbcCommand sql_cmd = new OdbcCommand(cmd, conn);
                    insert_count += sql_cmd.ExecuteNonQuery();
                    Thread.Sleep(100);
                }            
            }

            return insert_count;
        }

        #endregion

        #region Informative

        /// <summary>
        /// Indica se la tabella specificata è bloccata
        /// </summary>
        /// <param name="table_name">Nome della tabella</param>
        /// <returns>Stato del blocco</returns>
        public bool IsLocked(string table_name)
        {
            DataTable dt = ExecCustomQuery("SHOW OPEN TABLES WHERE `Table` = '" + table_name + "' AND `In_use` > 0");
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Restituisce una coppia di valori (nome tabella - chiave primaria) da una foreign key della tabella collegata
        /// </summary>
        /// <param name="table_name">Nome della tabella</param>
        /// <param name="foreign_key">Foreign key</param>
        /// <returns>Coppia di valori (nome tabella - chiave primaria)</returns>
        /// <remarks>
        /// L'operazione è valida per relazioni 1:1 e 1:N
        /// </remarks>
        public KeyValuePair<string, string> GetReferencedTableAndPKFromFK(string table_name, string foreign_key)
        {
            string source_table_name = string.Empty;
            string source_table_pk = string.Empty;

            switch (GetServerType())
            {
                case DBServerTypes.MYSQL:
                    DataTable dt = ExecCustomQuery("SELECT TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '" + conn.Database + "' AND TABLE_NAME = '" + table_name + "' AND COLUMN_NAME = '" + foreign_key + "' AND REFERENCED_COLUMN_NAME IS NOT NULL");
                    if (dt.Rows.Count > 0)
                    {
                        source_table_name = Convert.ToString(dt.Rows[0][3]);
                        source_table_pk = Convert.ToString(dt.Rows[0][4]);
                    }
                    break;
            }

            return new KeyValuePair<string, string>(source_table_name, source_table_pk);
        }

        /// <summary>
        /// Restituisce il tipo di dato della chiave primaria
        /// </summary>
        /// <param name="table_name">Nome della tabella da analizzare</param>
        /// <param name="column_name">Nome della colonna</param>
        /// <returns>Tipo di dato della chiave primaria</returns>
        public PrimaryKeyColumnTypes GetPrimaryKeyColumnType(string table_name, string column_name)
        {
            PrimaryKeyColumnTypes type = PrimaryKeyColumnTypes.UNKNOWN;

            switch (GetServerType())
            {
                case DBServerTypes.MYSQL:
                    DataTable dt = ExecCustomQuery("SHOW COLUMNS IN " + table_name + " WHERE `Field` = '" + column_name + "' AND `Key` = 'PRI'");
                    if (dt.Rows.Count > 0)
                    {
                        string type_str = Convert.ToString(dt.Rows[0][1]).ToLower();
                        if (type_str.Contains("int"))
                            type = PrimaryKeyColumnTypes.INT;
                        else if (type_str.Contains("char"))
                            type = PrimaryKeyColumnTypes.TEXT;
                        else if (type_str == "double")
                            type = PrimaryKeyColumnTypes.REAL;
                        else if (type_str == "datetime")
                            type = PrimaryKeyColumnTypes.DATETIME;
                        else if (type_str == "date")
                            type = PrimaryKeyColumnTypes.DATE;
                        else if (type_str == "time")
                            type = PrimaryKeyColumnTypes.TIME;
                    }
                    break;
            }

            return type;
        }        

        /// <summary>
        /// Restituisce il tipo di server utilizzato
        /// </summary>
        /// <returns>Tipo di server</returns>
        public DBServerTypes GetServerType()
        {
            DBServerTypes type;

            if (conn.Driver.ToLower().Contains("myodbc"))
            {
                // MySQL
                type = DBServerTypes.MYSQL;
            }
            else
                type = DBServerTypes.UNKNOWN;

            return type;
        }

        /// <summary>
        /// Restituisce il DSN del database WetNet
        /// </summary>
        /// <returns>ODBC DSN del database WetNet</returns>
        public string GetWetDBDSN()
        {
            WetConfig cfg = new WetConfig();

            return cfg.GetWetDBDSN();
        }

        /// <summary>
        /// Restituisce il nome della chiave primaria di una tabella
        /// </summary>
        /// <param name="table_name">Nome della tabella</param>
        /// <returns>Nome della chiave primaria oppure 'string.Empty' in caso di fallimento</returns>
        public string GetPrimaryKeyName(string table_name)
        {
            string key_name = string.Empty;

            DataTable dt = ExecCustomQuery("SHOW INDEX FROM " + table_name + " WHERE Key_name = 'PRIMARY'");
            if (dt.Rows.Count > 0)
                key_name = Convert.ToString(dt.Rows[0]["Column_name"]);

            return key_name;
        }

        /// <summary>
        /// Ritorna il tipo di provider
        /// </summary>
        /// <returns></returns>
        public ProviderType GetProvider()
        {
            ProviderType provider;

            if (conn.ConnectionString.ToLower().Contains("archestra"))
                provider = ProviderType.ARCHESTRA_SQL;
            else if (conn.ConnectionString.ToLower().Contains("ifix"))
                provider = ProviderType.IFIX_SQL;
            else
                provider = ProviderType.GENERIC_MYSQL;

            return provider;
        }

        #endregion

        #region Operative

        /// <summary>
        /// Esegue la sincronizzazione fra due tabelle equivalenti
        /// </summary>
        /// <param name="source_table">Tabella sorgente</param>
        /// <param name="dest_table">Tabella di destinazione</param>
        /// <returns>Numero di records affetti</returns>
        public int TableSync(DataTable source_table, string dest_table)
        {
            int affected_records = 0;
            string primary_key;

            // Acquisisco il valore di chiave primaria
            primary_key = GetPrimaryKeyName(dest_table);
            if (primary_key != string.Empty)
            {                
                // Creo una tabella temporanea con la stessa struttura della tabella di destinazione
                string tmp_dest_table = "tmp_" + dest_table;
                ExecCustomCommand("CREATE TABLE IF NOT EXISTS " + tmp_dest_table + " LIKE " + dest_table);
                // Copio la tabella sorgente in quella temporanea di destinazione
                TableInsert(source_table, tmp_dest_table);
                // Cancello i records della tabella di destinazione non presenti nella tabella temporanea
                affected_records = ExecCustomCommand("DELETE FROM " + dest_table + " WHERE " + primary_key + " NOT IN (SELECT "  + primary_key + " FROM " + tmp_dest_table + ")");
                // Cancello la tabella temporanea
                ExecCustomCommand("DROP TABLE IF EXISTS " + tmp_dest_table);
                // Copio i dati mancanti nella tabella di destinazione
                affected_records += TableInsert(source_table, dest_table);
            }

            return affected_records;
        }

        #endregion

        #endregion
    }
}
