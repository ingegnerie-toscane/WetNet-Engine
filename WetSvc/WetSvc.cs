/****************************************************************************
 * 
 * WetSvc - WetNet Engine Service.
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WetLib;

namespace WetSvc
{
    /// <summary>
    /// Classe del servizio
    /// </summary>
    public partial class WetSvc : ServiceBase
    {
        #region Istanze

        /// <summary>
        /// Motore wetnet
        /// </summary>
        WetEngine wet_engine;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public WetSvc()
        {
            // Inizializzazione standard dei componenti
            InitializeComponent();
            // Inizializzazione personalizzata dei componenti
            wet_engine = new WetEngine();            
        }

        #endregion

        #region Funzioni si start e stop del servizio

        /// <summary>
        /// Funzione di avvio per debug del servizio
        /// </summary>
        /// <param name="args">Argomenti di avvio</param>
        internal void StartDebug(string[] args)
        {
            OnStart(args);
        }

        /// <summary>
        /// Funzione di arresto per debug del servizio
        /// </summary>
        internal void StopDebug()
        {
            OnStop();
        }

        /// <summary>
        /// Evento di avvio del servizio
        /// </summary>
        /// <param name="args">Argomenti di avvio</param>
        protected override void OnStart(string[] args)
        {
            wet_engine.Start();
        }

        /// <summary>
        /// Evento di arresto del servizio
        /// </summary>
        protected override void OnStop()
        {
            wet_engine.Stop();
        }

        #endregion
    }
}
