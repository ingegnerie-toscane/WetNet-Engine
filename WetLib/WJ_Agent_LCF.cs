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

namespace WetLib
{
    /// <summary>
    /// Job per la copia dei dati degli LCF
    /// </summary>
    sealed class WJ_Agent_LCF : WetJob
    {
        #region Istanze

        /// <summary>
        /// Configurazione
        /// </summary>
        WetConfig cfg;

        /// <summary>
        /// Connessione al database degli LCF
        /// </summary>
        WetDBConn lcf_db;

        /// <summary>
        /// Connessione al database WetNet
        /// </summary>
        WetDBConn wet_db;

        #endregion

        #region Variabili globali

        /// <summary>
        /// DSN ODBC del database sorgente degli LCF
        /// </summary>
        string lcf_dsn;

        /// <summary>
        /// DSN ODBC del database WetNet
        /// </summary>
        string wetnet_dsn;

        /// <summary>
        /// Configurazione
        /// </summary>
        WetConfig.WJ_Agent_LCF_Config_Struct config;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WJ_Agent_LCF()
        {
            // Millisecondi di attesa fra le esecuzioni
            job_sleep_time = WetConfig.GetInterpolationTimeMinutes() * 60 * 1000;
            // Istanzio la configurazione
            cfg = new WetConfig();            
        }

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Funzione di caricamento
        /// </summary>
        protected override void Load()
        {
            // carico la configurazione
            config = cfg.GetWJ_Agent_LCF_Config();
            // Carico i parametri della configurazione
            lcf_dsn = config.odbc_dsn;
            wetnet_dsn = cfg.GetWetDBDSN();
            // Istanzio le connessioni ai database
            lcf_db = new WetDBConn(lcf_dsn, null, null, false);
            wet_db = new WetDBConn(wetnet_dsn, null, null, true);
        }

        /// <summary>
        /// Funzione del job
        /// </summary>
        protected override void DoJob()
        {
            // Se il Job non è abilitato esco
            if (!config.enabled)
                return;
            // Acquisisco le tabelle degli LCF
            string[] lcf_tables = GetLCFTables();
            // Ciclo per tutte le tabelle
            foreach (string lcf_table in lcf_tables)
            {
                try
                {
                    
                    // Acquisisco data e ora dell'ultimo campione scritto nella tabella di destinazione
                    DateTime last_dest = GetLastDestSample(lcf_table);
                    // Acquisisco data e ora dell'ultimo campione letto dalla tabella di origine
                    DateTime last_source = GetLastSourceSample(lcf_table);
                    // Se l'ultimo campione sorgente è maggiore, allora avvio la procedura di aggiornamento
                    if (last_source > last_dest)
                    {
                        // Controllo che l'LCF esista nella tabella di destinazione, altrimenti lo creo
                        if (last_dest == DateTime.MinValue)
                        {
                            DataTable dt = lcf_db.ExecCustomQuery("SELECT * FROM t_lcf WHERE table_mis = '" + lcf_table + "'");
                            if (dt.Rows.Count > 0)
                                wet_db.ExecCustomCommand("INSERT IGNORE INTO lcf_identities (`table_name`, `description`, `municipality`) VALUES ('" + Convert.ToString(dt.Rows[0]["table_mis"]) + "', '" + Convert.ToString(dt.Rows[0]["descr_mis"]) + "', '" + Convert.ToString(dt.Rows[0]["comune_mis"]) + "')");
                        }
                        // Aggiungo i records mancanti
                        DataTable src = lcf_db.ExecCustomQuery("SELECT * FROM " + lcf_table + " WHERE TIMESTAMP(`Data`, `Ora`) > '" + last_dest.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                        DataTable tmp = new DataTable();
                        tmp.Columns.Add("timestamp", typeof(DateTime));
                        tmp.Columns.Add("ft1", typeof(double));
                        tmp.Columns.Add("pt1", typeof(double));
                        tmp.Columns.Add("pt2", typeof(double));
                        tmp.Columns.Add("counter", typeof(double));
                        tmp.Columns.Add("lcf_identities_table_name", typeof(string));
                        foreach (DataRow dr in src.Rows)
                        {
                            // Popolo i dati da inserire
                            DateTime ts = Convert.ToDateTime(dr["Data"]);
                            string[] tm = Convert.ToString(dr["Ora"]).Split(new char[] { ':' });
                            ts = ts.AddHours(Convert.ToDouble(tm[0])).AddMinutes(Convert.ToDouble(tm[1])).AddSeconds(Convert.ToDouble(tm[2]));
                            double ft1 = dr["Q"] == DBNull.Value ? 0.0d : Convert.ToDouble(dr["Q"]);
                            double pt1 = double.NaN;
                            if (src.Columns.Contains("Pressione1"))
                                pt1 = dr["Pressione1"] == DBNull.Value ? 0.0d : Convert.ToDouble(dr["Pressione1"]);
                            double pt2 = double.NaN;
                            if (src.Columns.Contains("Pressione2"))
                                pt2 = dr["Pressione2"] == DBNull.Value ? 0.0d : Convert.ToDouble(dr["Pressione2"]);
                            double counter = double.NaN;
                            if (src.Columns.Contains("ContatoreUp"))
                                counter = dr["ContatoreUp"] == DBNull.Value ? 0.0d : Convert.ToDouble(dr["ContatoreUp"]);
                            // Compongo la query di inserimento
                            wet_db.ExecCustomCommand("INSERT IGNORE INTO lcf_data (`timestamp`, `ft1`, `pt1`, `pt2`, `counter`, `lcf_identities_table_name`) VALUES ('" +
                                ts.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'," +
                                (double.IsNaN(ft1) ? "NULL" : ft1.ToString().Replace(',', '.')) + "," +
                                (double.IsNaN(pt1) ? "NULL" : pt1.ToString().Replace(',', '.')) + "," +
                                (double.IsNaN(pt2) ? "NULL" : pt2.ToString().Replace(',', '.')) + "," +
                                (double.IsNaN(counter) ? "NULL" : counter.ToString().Replace(',', '.')) + ",'" +
                                lcf_table + "')");
                        }
                    }                                        
                }
                catch (Exception ex)
                {
                    WetDebug.GestException(ex);
                }
                // Passo il controllo al S.O. per l'attesa
                if (cancellation_token_source.IsCancellationRequested)
                    return;
                Sleep();
            }
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Restituisce l'elenco degli LCF presenti nel database degli LCF
        /// </summary>
        /// <returns></returns>
        string[] GetLCFTables()
        {
            List<string> names = new List<string>();

            try
            {
                DataTable dt = lcf_db.ExecCustomQuery("SELECT table_mis FROM t_lcf ORDER BY id ASC");
                foreach (DataRow dr in dt.Rows)
                    names.Add(Convert.ToString(dr[0]));
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return names.ToArray();
        }

        /// <summary>
        /// Restituisce data e ora dell'ultimo campione scritto nella tabella di destinazione
        /// </summary>
        /// <param name="table_name">Nome della tabella LCF</param>
        /// <returns>Data e ora dell'ultimo campione scritto</returns>
        DateTime GetLastDestSample(string table_name)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                DataTable dt = wet_db.ExecCustomQuery("SELECT timestamp FROM lcf_data WHERE lcf_identities_table_name='" + table_name + "' ORDER BY timestamp DESC LIMIT 1");
                if (dt.Rows.Count == 1)
                    ret = Convert.ToDateTime(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ret;
        }

        /// <summary>
        /// Restituisce data e ora dell'ultimo campione letto nella tabella sorgente
        /// </summary>
        /// <param name="table_name">Nome della tabella LCF</param>
        /// <returns>Data e ora dell'ultimo campione letto</returns>
        DateTime GetLastSourceSample(string table_name)
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                DataTable dt = lcf_db.ExecCustomQuery("SELECT TIMESTAMP(`Data`, `Ora`) AS ts FROM " + table_name + " ORDER BY ts DESC LIMIT 1");
                if (dt.Rows.Count == 1)
                    ret = Convert.ToDateTime(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                WetDebug.GestException(ex);
            }

            return ret;
        }

        #endregion
    }
}
