using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WetLib
{
    /// <summary>
    /// Classe contenente il motore di WetNet
    /// </summary>
    public sealed class WetEngine : WetJob
    {
        #region Costanti

        /// <summary>
        /// Stringa del job principale
        /// </summary>
        const string ENGINE_NAME = "WetEngine";

        #endregion

        #region Istanze

        /// <summary>
        /// Job per la copia dei dati degli LCF
        /// </summary>
        WJ_LCFCopy wj_lcf_copy;

        /// <summary>
        /// Job per la gestione dei dati delle misure
        /// </summary>
        WJ_MeasuresData wj_measures_data;

        /// <summary>
        /// Job per la gestione del bilancio dei distretti
        /// </summary>
        WJ_DistrictsBalance wj_districts_balance;

        /// <summary>
        /// Job per il calcolo delle statistiche dei distretti
        /// </summary>
        WJ_Statistics wj_statistics;

        /// <summary>
        /// Job per la gestione degli eventi
        /// </summary>
        WJ_Events wj_events;

        /// <summary>
        /// Job per la gestione degli allarmi sulle misure
        /// </summary>
        WJ_MeasuresAlarms wj_measures_alarms;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WetEngine()
        {
            // Istanziamento dei jobs
            wj_lcf_copy = new WJ_LCFCopy();
            wj_measures_data = new WJ_MeasuresData();
            wj_districts_balance = new WJ_DistrictsBalance();
            wj_statistics = new WJ_Statistics();
            wj_events = new WJ_Events();
            wj_measures_alarms = new WJ_MeasuresAlarms();
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Inizializzazione
        /// </summary>
        protected override void Load()
        {
            // Avvio dei jobs
            wj_lcf_copy.Start();
            wj_measures_data.Start();
            wj_districts_balance.Start();
            wj_statistics.Start();
            wj_events.Start();
            wj_measures_alarms.Start();
        }

        /// <summary>
        /// Corpo del job principale
        /// </summary>
        protected override void DoJob()
        {
            // Dummy
        }

        /// <summary>
        /// Finalizzazione
        /// </summary>
        protected override void UnLoad()
        {
            // Arresto dei jobs
            wj_lcf_copy.Stop();
            wj_measures_data.Stop();
            wj_districts_balance.Stop();
            wj_statistics.Stop();
            wj_events.Stop();
            wj_measures_alarms.Stop();
        }

        #endregion
    }
}
