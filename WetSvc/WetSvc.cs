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

#if DEBUG

        /// <summary>
        /// Funzione di avvio per debug del servizio
        /// </summary>
        /// <param name="args">Argomenti di avvio</param>
        public void StartDebug(string[] args)
        {
            OnStart(args);
        }

        /// <summary>
        /// Funzione di arresto per debug del servizio
        /// </summary>
        public void StopDebug()
        {
            OnStop();
        }

#endif

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
