using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.ServiceProcess;

namespace WetAdmin
{
    /// <summary>
    /// Form principale dell'applicazione
    /// </summary>
    public partial class frmMain : Form
    {
        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public frmMain()
        {
            // Inizializzazione standard dei componenti
            InitializeComponent();
            // Inizializzazione personalizzata dei componenti
            this.Text = Assembly.GetEntryAssembly().GetName().Name + " - v" +
                Assembly.GetEntryAssembly().GetName().Version.ToString() + " (alpha) - " +
                (Environment.Is64BitProcess ? "64" : "32") + " bit";
            this.Icon = Resources_ICO_48x48.ksirtet;
            svcWetSvc.ServiceName = "WetSvc";
            svcWetSvc.MachineName = Environment.MachineName;
        }

        #endregion

        #region Funzioni comuni agli eventi

        /// <summary>
        /// Avvio del servizio WetSvc
        /// </summary>
        void Ecf_StartService()
        {
            try
            {
                svcWetSvc.Start();
            }
            catch
            {
                MessageBox.Show("Unexpected error while starting service!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Arresto del servizio WetSvc
        /// </summary>
        void Ecf_StopService()
        {
            try
            {
                svcWetSvc.Stop();
            }
            catch
            {
                MessageBox.Show("Unexpected error while stopping service!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Riavvio del servizio WetSvc
        /// </summary>
        void Ecf_RestartService()
        {
            try
            {
                Ecf_StopService();
                svcWetSvc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 1, 0));
                Ecf_StartService();
            }
            catch
            {
                MessageBox.Show("Unexpected error while restarting service!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Aggiorna lo stato dei controlli del servizio
        /// </summary>
        void UpdateServiceStatusControl()
        {
            try
            {
                switch (svcWetSvc.Status)
                {
                    case ServiceControllerStatus.Running:
                        mnuMain_Service_Start.Enabled = false;
                        mnuMain_Service_Stop.Enabled = true;
                        mnuMain_Service_Restart.Enabled = true;
                        tsMain_Service_Start.Enabled = false;
                        tsMain_Service_Stop.Enabled = true;
                        tsMain_Service_Restart.Enabled = true;
                        break;

                    case ServiceControllerStatus.Stopped:
                        mnuMain_Service_Start.Enabled = true;
                        mnuMain_Service_Stop.Enabled = false;
                        mnuMain_Service_Restart.Enabled = false;
                        tsMain_Service_Start.Enabled = true;
                        tsMain_Service_Stop.Enabled = false;
                        tsMain_Service_Restart.Enabled = false;
                        break;
                }
            }
            catch
            {
                mnuMain_Service_Start.Enabled = false;
                mnuMain_Service_Stop.Enabled = false;
                mnuMain_Service_Restart.Enabled = false;
                tsMain_Service_Start.Enabled = false;
                tsMain_Service_Stop.Enabled = false;
                tsMain_Service_Restart.Enabled = false;
            }
        }

        #endregion

        /// <summary>
        /// Carico il form di impostazione dei DSN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Settings_ODBCconnections_Click(object sender, EventArgs e)
        {
            frmSettings_ODBCconnections frm = new frmSettings_ODBCconnections();
            frm.ShowDialog();
        }

        /// <summary>
        /// Carico il form di impostazione delle misure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Modelling_Measures_Click(object sender, EventArgs e)
        {
            frmModelling_Measures frm = new frmModelling_Measures();
            frm.ShowDialog();
        }

        /// <summary>
        /// Carico il form di impostazione dei distretti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Modelling_Districts_Click(object sender, EventArgs e)
        {
            frmModelling_Districts frm = new frmModelling_Districts();
            frm.ShowDialog();
        }

        /// <summary>
        /// Avvio del servizio WetSvc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Service_Start_Click(object sender, EventArgs e)
        {
            Ecf_StartService();
        }

        /// <summary>
        /// Controllo stato servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrCheckService_Tick(object sender, EventArgs e)
        {
            UpdateServiceStatusControl();
        }

        /// <summary>
        /// Evento di richiesta chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to close the application?", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == System.Windows.Forms.DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        /// <summary>
        /// Richiede la chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMain_File_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Richiede la chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_File_Esci_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Richiesta di arresto servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Service_Stop_Click(object sender, EventArgs e)
        {
            Ecf_StopService();
        }

        /// <summary>
        /// Richiesta di riavvio servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMain_Service_Restart_Click(object sender, EventArgs e)
        {
            Ecf_RestartService();
        }

        /// <summary>
        /// Richiesta di avvio servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMain_Service_Start_Click(object sender, EventArgs e)
        {
            Ecf_StartService();
        }

        /// <summary>
        /// Richiesta di arresto servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMain_Service_Stop_Click(object sender, EventArgs e)
        {
            Ecf_StopService();
        }

        /// <summary>
        /// Richiesta di riavvio servizio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMain_Service_Restart_Click(object sender, EventArgs e)
        {
            Ecf_RestartService();
        }
    }
}
