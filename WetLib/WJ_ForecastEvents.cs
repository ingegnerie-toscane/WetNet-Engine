using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Globalization;

namespace WetLib
{
    /// <summary>
    /// Classe che definisce la struttura di controllo per un distretto
    /// </summary>
    sealed class CheckClass
    {
        /// <summary>
        /// ID del distretto
        /// </summary>
        public int id_district = -1;

        /// <summary>
        /// Data e ora di aggiornamento o creazione della classe
        /// </summary>
        public DateTime update = DateTime.MinValue;

        /// <summary>
        /// Data e ora ultima mail di allarme inviata
        /// </summary>
        public DateTime last_mail_sent = DateTime.MinValue;

        /// <summary>
        /// Trend previsionale per il distretto
        /// </summary>
        public List<DayTrendSample> trend = new List<DayTrendSample>();
    }

    /// <summary>
    /// Job per il calcolo runtime delle previsioni
    /// </summary>
    class WJ_ForecastEvents : WetJob
    {
        #region Costanti

        /// <summary>
        /// Nome del job
        /// </summary>
        const string JOB_NAME = "WJ_ForecastEvents";

        /// <summary>
        /// Tempo in minuti per aggiornamento della check class
        /// </summary>
        const int UPDATE_CHECK_TIME_MINUTES = 360;

        /// <summary>
        /// Numero di settimane retroattive di calcolo previsionale
        /// </summary>
        const int RETRO_WEEKS = 8;

        #endregion

        #region Istanze

        /// <summary>
        /// Connessione al database wetnet
        /// </summary>
        WetDBConn wet_db;

        #endregion

        #region Variabili globali

        /// <summary>
        /// Struttura con la configurazione degli eventi
        /// </summary>
        WetConfig.WJ_Events_Config_Struct ecfg;

        /// <summary>
        /// Tempo di interpolazione in minuti
        /// </summary>
        int interpolation_time_minutes;

        /// <summary>
        /// Numero di campioni giornalieri
        /// </summary>
        int samples_in_day = 0;

        /// <summary>
        /// Check list per i distretti
        /// </summary>
        List<CheckClass> checks = new List<CheckClass>();

        #endregion

        #region Funzioni del job

        /// <summary>
        /// Caricamento del job
        /// </summary>
        protected override void Load()
        {
            // Istanzio la connessione al database wetnet
            WetConfig cfg = new WetConfig();
            interpolation_time_minutes = WetConfig.GetInterpolationTimeMinutes();
            samples_in_day = (int)(24 * 60 / interpolation_time_minutes);
            wet_db = new WetDBConn(cfg.GetWetDBDSN(), null, null, true);
            ecfg = cfg.GetWJ_Events_Config();
        }

        /// <summary>
        /// Corpo del job
        /// </summary>
        protected override void DoJob()
        {
            try
            {                
                // Acquisisco tutti i distretti configurati
                DataTable districts = wet_db.ExecCustomQuery("SELECT * FROM districts");
                // Ciclo per tutti i distretti                
                foreach (DataRow district in districts.Rows)
                {
                    Sleep();

                    try
                    {
                        // Estraggo l'ID del distretto
                        int id_district = Convert.ToInt32(district["id_districts"]);

                        #region Estrazione dei parametri

                        // Acquisisco la stringa dei parametri
                        string sap_code = Convert.ToString(district["sap_code"]);
                        // Se la stringa è nulla o vuota passo al distretto successivo
                        if ((sap_code == null) || (sap_code == string.Empty))
                            continue;
                        // Se la stringa non è nulla la spacchetto
                        string[] codes = sap_code.Split(new char[] { '#' }, StringSplitOptions.None);
                        // Il numero di parametri deve essere di almeno 3 stringhe, altrimenti c'è una anomalia e continuo col distretto successivo
                        if (codes.Length < 3)
                            continue;
                        // Estraggo l'abilitazione, la prima stringa deve essere un numero = 1, altrimenti passo al distretto successivo
                        string enabled = Convert.ToString(codes[0]);
                        if (enabled != "1")
                            continue;
                        // Provo a estrarre il trigger (rappresentato in secondi), se fallisco passo al distretto successivo
                        int trigger;
                        bool success = int.TryParse(codes[1], out trigger);
                        if (!success)
                            continue;
                        if ((trigger >= 1140) || (trigger < interpolation_time_minutes))
                            continue;
                        // Provo a estrarre il coefficente
                        double coefficent;
                        success = double.TryParse(codes[2].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out coefficent);
                        if (!success)
                            continue;
                        if (!(coefficent > 0.0))
                            continue;

                        #endregion

                        #region Estrazione, creazione o aggiornamento della check class

                        // Controllo se esiste un record di check per il distretto, altrimenti lo creo
                        if (!checks.Any(x => x.id_district == id_district))
                        {
                            CheckClass cc = new CheckClass();
                            cc.id_district = id_district;                            
                            cc.trend = WetUtility.GetDayTrendEx(id_district, DateTime.Now.Date, samples_in_day, RETRO_WEEKS).ToList();
                            cc.update = DateTime.Now;
                            checks.Add(cc);
                        }
                        // Estraggo la check class
                        int idx = checks.FindIndex(x => x.id_district == id_district);
                        CheckClass check_class = checks[idx];
                        // Se non è aggiornata, la aggiorno
                        if (((DateTime.Now - check_class.update).Minutes > UPDATE_CHECK_TIME_MINUTES) || (check_class.update.Date != DateTime.Now.Date))
                        {
                            checks[idx].trend = WetUtility.GetDayTrendEx(id_district, DateTime.Now.Date, samples_in_day, RETRO_WEEKS).ToList();
                            checks[idx].update = DateTime.Now;
                        }

                        #endregion

                        // Estraggo il profilo high
                        List<double> high_profile = new List<double>();
                        foreach (DayTrendSample dts in check_class.trend)
                            high_profile.Add(dts.hi_value * coefficent);

                        // Estraggo il profilo giornaliero del distretto fino a qui calcolato
                        DataTable dt = wet_db.ExecCustomQuery("SELECT `value` FROM data_districts WHERE `districts_id_districts` = " + id_district.ToString() + " AND `timestamp` >= '" + DateTime.Now.Date.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + "'");
                        List<double> real_profile = new List<double>();
                        foreach (DataRow dr in dt.Rows)
                            real_profile.Add(Convert.ToDouble(dr["value"]));

                        // Imposto il trigger
                        int num_consecutive_samples = Convert.ToInt32(Math.Ceiling((double)trigger / (double)interpolation_time_minutes));

                        // Inizio il confronto, azzero il contatore del trigger
                        int trigger_cnt = 0;
                        DateTime start_dt = DateTime.MinValue;
                        for (int ii = 0; ii < real_profile.Count; ii++)
                        {                            
                            if (real_profile[ii] > high_profile[ii])
                                trigger_cnt++;
                            else
                                trigger_cnt = 0;

                            if (trigger_cnt == num_consecutive_samples)
                            {
                                start_dt = DateTime.Now.Date.AddMinutes(((ii + 1) * interpolation_time_minutes) - (num_consecutive_samples * interpolation_time_minutes));
                                break;
                            }
                        }

                        // A fine esecuzione controllo il trigger, se = al numero dei campioni di trigger consecutivi, lancio l'allarme
                        if (trigger_cnt == num_consecutive_samples)
                        {
                            // Mi assicuro di non aver già inviato una mail giornaliera
                            if ((check_class.last_mail_sent.Date < DateTime.Now.Date) && (check_class.update.Date == DateTime.Now.Date))
                            {
                                SmtpClient smtp_client = new SmtpClient();
                                smtp_client.Host = ecfg.smtp_server;
                                smtp_client.Port = ecfg.smtp_server_port;
                                smtp_client.EnableSsl = ecfg.smtp_use_ssl;
                                smtp_client.Credentials = new NetworkCredential(ecfg.smtp_username, ecfg.smtp_password);
                                string from_str = ecfg.smtp_username;
                                if (!from_str.Contains("@"))
                                    from_str += "@wetnet.net";
                                // Sono in condizione di allarme, leggo la lista dei contatti mail
                                for (int ii = 0; ii < (codes.Length - 3); ii++)
                                {
                                    try
                                    {
                                        MailMessage msg = new MailMessage();
                                        msg.From = new MailAddress(from_str);
                                        msg.To.Add(codes[ii + 3]);
                                        msg.Subject = "WetNet prevision event report";
                                        msg.Body = "District: #" + id_district + " - " + Convert.ToString(district["name"]) + Environment.NewLine + Environment.NewLine +
                                                "PROFILE PREDICTION WARNING!" + Environment.NewLine + Environment.NewLine +
                                                "Event start at: " + start_dt.ToString(WetDBConn.MYSQL_DATETIME_FORMAT) + " - Duration: " + (num_consecutive_samples * interpolation_time_minutes).ToString() + " minutes." + Environment.NewLine;
                                        smtp_client.Send(msg);
                                    }
                                    catch (Exception ex2)
                                    {
                                        WetDebug.GestException(ex2);
                                    }
                                }
                                checks[idx].last_mail_sent = DateTime.Now;
                            }
                        }

                        // Passo il controllo al S.O. per l'attesa
                        if (cancellation_token_source.IsCancellationRequested)
                            return;
                        Sleep();
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
