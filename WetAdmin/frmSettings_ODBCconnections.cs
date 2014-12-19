using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WetAdmin
{
    /// <summary>
    /// Form per l'impostazione delle connessioni ODBC
    /// </summary>
    public partial class frmSettings_ODBCconnections : Form
    {
        #region Costanti

        /// <summary>
        /// Nome della tabella
        /// </summary>
        const string TABLE_NAME = "connections";

        #endregion

        #region Istanze

        /// <summary>
        /// Configurazione
        /// </summary>
        WetLib.WetConfig cfg;

        /// <summary>
        /// Connessione al database WetNet
        /// </summary>
        WetLib.WetDBConn db_conn;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public frmSettings_ODBCconnections()
        {
            // Inizializzazione standard dei componenti
            InitializeComponent();
            // Istanzio la configurazione
            cfg = new WetLib.WetConfig();
            // Istanzio la connesione al database
            db_conn = new WetLib.WetDBConn(cfg.GetWetDBDSN(), true);
            // Creo le associazioni dati
            ExecBindings();
            // Associo i controlli
            bnMain.BindingSource = bsConnections;
            dgDown.DataSource = bsConnections;
            dgDown.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            txtId.DataBindings.Add(new Binding("Text", bsConnections, "id_odbcdsn", true));
            cboDSN.DataBindings.Add(new Binding("Text", bsConnections, "odbc_dsn", true));
            txtDescription.DataBindings.Add(new Binding("Text", bsConnections, "description", true));
            // Acquisisco la lista dei DSN
            cboDSN.Items.AddRange(WetLib.WetDBConn.GetDSNs());            
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Esegue il binding dei dati
        /// </summary>
        void ExecBindings()
        {
            // Creo le associazioni dati
            dsConnections = db_conn.GetDataset(TABLE_NAME);
            bsConnections.DataSource = dsConnections.Tables[0];            
        }

        /// <summary>
        /// Esegue il commit dei dati sul database
        /// </summary>
        void ExecCommit()
        {
            try
            {
                // Pongo il form in stand-by
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;
                // Eseguo l'aggiornamento           
                bsConnections.EndEdit();
                dsConnections.AcceptChanges();
                db_conn.TableSync(dsConnections.Tables[0], TABLE_NAME);
                // Ricreo le associazioni dati
                ExecBindings();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Enabled = true;
                this.Cursor = Cursors.Default;
                cmdCommit.Enabled = false;
            }
        }

        #endregion

        /// <summary>
        /// Esegue il commit dei dati sul database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCommit_Click(object sender, EventArgs e)
        {
            ExecCommit();
        }

        /// <summary>
        /// Esce dall'applicazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Evento di richiesta chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmSettings_ODBCconnections_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Exit saving changes?", Application.ProductName,
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                // Effettuo i cambiamenti ed esco
                ExecCommit();
            }
            else if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                // Annullo la chiusura del form
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Evento di richiesta aggiunta di un  record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            cmdCommit.Enabled = true;
        }
    }
}
