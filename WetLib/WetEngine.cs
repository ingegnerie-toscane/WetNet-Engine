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
        WJ_Agent_LCF wj_agent_lcf;

        /// <summary>
        /// Job per la gestione dei WLB
        /// </summary>
        WJ_Agent_WetNetLinkBox wj_agent_wetnetlinkbox;

        /// <summary>
        /// Job per la gestione dei device Primayer
        /// </summary>
        WJ_Agent_Primayer wj_agent_primayer;

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

        #region Variabili globali

        /// <summary>
        /// Contatore di attività eseguite all'avvio
        /// </summary>
        public static int cold_start_counter = 0;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WetEngine()
        {
            // Istanziamento dei jobs
            wj_agent_lcf = new WJ_Agent_LCF();
            wj_agent_wetnetlinkbox = new WJ_Agent_WetNetLinkBox();
            wj_agent_primayer = new WJ_Agent_Primayer();
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
            wj_agent_lcf.Start();
            wj_agent_wetnetlinkbox.Start();
            wj_agent_primayer.Start();
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
            wj_agent_lcf.Stop();
            wj_agent_wetnetlinkbox.Stop();
            wj_agent_primayer.Stop();
            wj_measures_data.Stop();
            wj_districts_balance.Stop();
            wj_statistics.Stop();
            wj_events.Stop();
            wj_measures_alarms.Stop();
        }

        #endregion
    }
}
