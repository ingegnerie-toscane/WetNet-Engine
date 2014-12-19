using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace WetAdmin
{
    /// <summary>
    /// Form per l'impostazione delle misure
    /// </summary>
    public partial class frmModelling_Measures : Form
    {
        #region Costanti

        /// <summary>
        /// Nome della tabella per le misure
        /// </summary>
        const string MEASURES_TABLE_NAME = "measures";

        /// <summary>
        /// Nome della tabella per le connessioni
        /// </summary>
        const string CONNECTIONS_TABLE_NAME = "connections";

        #endregion

        #region Istanze

        /// <summary>
        /// Configurazione
        /// </summary>
        WetLib.WetConfig cfg;

        /// <summary>
        /// Connessione al database di WetNet
        /// </summary>
        WetLib.WetDBConn db_conn;

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        public frmModelling_Measures()
        {
            // Inizializzazione standard dei componenti
            InitializeComponent();
            // Istanzio la configurazione
            cfg = new WetLib.WetConfig();
            // Istanzio la connessione al database
            db_conn = new WetLib.WetDBConn(cfg.GetWetDBDSN(), true);
            // Eseguo il binging dei dati
            ExecBindings();
            // Associo i controlli
            bnMain.BindingSource = bsMeasures;
            dgDown.DataSource = bsMeasures;
            dgDown.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Scheda "General"            
            txtID.DataBindings.Add(new Binding("Text", bsMeasures, "id_measures"));
            txtName.DataBindings.Add(new Binding("Text", bsMeasures, "name"));
            txtDescription.DataBindings.Add(new Binding("Text", bsMeasures, "description"));
            Dictionary<int, string> measure_types = new Dictionary<int, string>()
            {
                { (int)WetLib.MeasureTypes.FLOW, WetLib.MeasureTypes.FLOW.ToString() },
                { (int)WetLib.MeasureTypes.PRESSURE, WetLib.MeasureTypes.PRESSURE.ToString() },
                { (int)WetLib.MeasureTypes.COUNTER, WetLib.MeasureTypes.COUNTER.ToString() }
            };
            cboType.DataSource = new BindingSource(measure_types, null);
            cboType.DisplayMember = "Value";
            cboType.ValueMember = "Key";
            cboType.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "type"));
            numFixedValue.DataBindings.Add(new Binding("Value", bsMeasures, "fixed_value", true, DataSourceUpdateMode.OnValidation));
            chkReliable.DataBindings.Add(new Binding("Checked", bsMeasures, "reliable", true));
            chkCritical.DataBindings.Add(new Binding("Checked", bsMeasures, "critical", true));
            dtpUpdateTimestamp.DataBindings.Add(new Binding("Text", bsMeasures, "update_timestamp", true, DataSourceUpdateMode.OnValidation));            
            // Scheda "Database"
            cboConnection.DataSource = bsConnections;
            cboConnection.ValueMember = "id_odbcdsn";
            cboConnection.DisplayMember = "odbc_dsn";            
            cboConnection.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "connections_id_odbcdsn"));
            cboTable_TimestampColumn.DataBindings.Add(new Binding("Text", bsMeasures, "table_timestamp_column"));
            cboTable_ValueColumn.DataBindings.Add(new Binding("Text", bsMeasures, "table_value_column"));
            cboTable_RelationalIDColumn.DataBindings.Add(new Binding("Text", bsMeasures, "table_relational_id_column"));            
            Dictionary<int, string> primary_key_type = new Dictionary<int, string>()
            {
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.UNKNOWN, WetLib.WetDBConn.PrimaryKeyColumnTypes.UNKNOWN.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.TEXT, WetLib.WetDBConn.PrimaryKeyColumnTypes.TEXT.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.INT, WetLib.WetDBConn.PrimaryKeyColumnTypes.INT.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.REAL, WetLib.WetDBConn.PrimaryKeyColumnTypes.REAL.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.DATETIME, WetLib.WetDBConn.PrimaryKeyColumnTypes.DATETIME.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.DATE, WetLib.WetDBConn.PrimaryKeyColumnTypes.DATE.ToString() },
                { (int)WetLib.WetDBConn.PrimaryKeyColumnTypes.TIME, WetLib.WetDBConn.PrimaryKeyColumnTypes.TIME.ToString() },
            };
            cboTable_RelationalIDType.DataSource = new BindingSource(primary_key_type, null);
            cboTable_RelationalIDType.DisplayMember = "Value";
            cboTable_RelationalIDType.ValueMember = "Key";
            cboTable_RelationalIDType.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "table_relational_id_type"));
            // Scheda "Analytics Data"
            dtpMinNight_Start.DataBindings.Add(new Binding("Text", bsMeasures, "min_night_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpMinNight_Stop.DataBindings.Add(new Binding("Text", bsMeasures, "min_night_stop_time", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot1_Start.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_start_time_1", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot1_Stop.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_stop_time_1", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot2_Start.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_start_time_2", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot2_Stop.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_stop_time_2", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot3_Start.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_start_time_3", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot3_Stop.DataBindings.Add(new Binding("Text", bsMeasures, "max_day_stop_time_3", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Energy"
            Dictionary<int, string> energy_category = new Dictionary<int, string>()
            {
                { (int)WetLib.EnergyCategories.NONE, WetLib.EnergyCategories.NONE.ToString() },
                { (int)WetLib.EnergyCategories.POWER, WetLib.EnergyCategories.POWER.ToString() },
                { (int)WetLib.EnergyCategories.GRAVITY, WetLib.EnergyCategories.GRAVITY.ToString() }
            };
            cboEnergyCategory.DataSource = new BindingSource(energy_category, null);
            cboEnergyCategory.DisplayMember = "Value";
            cboEnergyCategory.ValueMember = "Key";
            cboEnergyCategory.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "energy_category"));
            numEnergySpecificContent.DataBindings.Add(new Binding("Value", bsMeasures, "energy_specific_content", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Strumentation"
            Dictionary<int, string> strumentation_type = new Dictionary<int, string>()
            {
                { (int)WetLib.MeterTypes.UNKNOWN, WetLib.MeterTypes.UNKNOWN.ToString() },
                { (int)WetLib.MeterTypes.MAGNETIC_FLOW_METER, WetLib.MeterTypes.MAGNETIC_FLOW_METER.ToString() },
                { (int)WetLib.MeterTypes.ULTRASONIC_FLOW_METER, WetLib.MeterTypes.ULTRASONIC_FLOW_METER.ToString() },
                { (int)WetLib.MeterTypes.LCF_FLOW_METER, WetLib.MeterTypes.LCF_FLOW_METER.ToString() },
                { (int)WetLib.MeterTypes.PRESSURE_METER, WetLib.MeterTypes.PRESSURE_METER.ToString() },
                { (int)WetLib.MeterTypes.VOLUMETRIC_COUNTER, WetLib.MeterTypes.VOLUMETRIC_COUNTER.ToString() }
            };
            cboSt_Type.DataSource = new BindingSource(strumentation_type, null);
            cboSt_Type.DisplayMember = "Value";
            cboSt_Type.ValueMember = "Key";
            cboSt_Type.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "strumentation_type"));
            txtSt_Model.DataBindings.Add(new Binding("Text", bsMeasures, "strumentation_model"));
            txtSt_SerialNumber.DataBindings.Add(new Binding("Text", bsMeasures, "strumentation_serial_number"));
            Dictionary<int, string> strumentation_link_type = new Dictionary<int, string>()
            {
                { (int)WetLib.MeterLinkTypes.UNKNOWN, WetLib.MeterLinkTypes.UNKNOWN.ToString() },
                { (int)WetLib.MeterLinkTypes.RADIO, WetLib.MeterLinkTypes.RADIO.ToString() },
                { (int)WetLib.MeterLinkTypes.SMS, WetLib.MeterLinkTypes.SMS.ToString() },
                { (int)WetLib.MeterLinkTypes.GSM, WetLib.MeterLinkTypes.GSM.ToString() },
                { (int)WetLib.MeterLinkTypes.GPRS, WetLib.MeterLinkTypes.GPRS.ToString() }
            };
            cboSt_LinkType.DataSource = new BindingSource(strumentation_link_type, null);
            cboSt_LinkType.DisplayMember = "Value";
            cboSt_LinkType.ValueMember = "Key";
            cboSt_LinkType.DataBindings.Add(new Binding("SelectedValue", bsMeasures, "strumentation_link_type"));
            // Scheda "Geolocation"
            num_xpos.DataBindings.Add(new Binding("Value", bsMeasures, "x_position", true, DataSourceUpdateMode.OnValidation));
            num_ypos.DataBindings.Add(new Binding("Value", bsMeasures, "y_position", true, DataSourceUpdateMode.OnValidation));
            num_zpos.DataBindings.Add(new Binding("Value", bsMeasures, "z_position", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Alarms"
            chkThresholds_Enable.DataBindings.Add(new Binding("Checked", bsMeasures, "alarm_thresholds_enable", true));
            numThresholds_Min.DataBindings.Add(new Binding("Value", bsMeasures, "alarm_min_threshold", true, DataSourceUpdateMode.OnValidation));
            numThresholds_Max.DataBindings.Add(new Binding("Value", bsMeasures, "alarm_max_threshold", true, DataSourceUpdateMode.OnValidation));
            chkConstant_Enable.DataBindings.Add(new Binding("Checked", bsMeasures, "alarm_constant_check_enable", true));            
            numConstant_Hysteresis.DataBindings.Add(new Binding("Value", bsMeasures, "alarm_constant_hysteresis", true, DataSourceUpdateMode.OnValidation));
            numConstant_Time.DataBindings.Add(new Binding("Value", bsMeasures, "alarm_constant_check_time", true, DataSourceUpdateMode.OnValidation));
            // Scheda "EPANET"
            txtEpa_ObjectID.DataBindings.Add(new Binding("Text", bsMeasures, "epanet_object_id"));
            txtEpa_Pattern.DataBindings.Add(new Binding("Text", bsMeasures, "epanet_pattern"));
            txtEpa_PatternFile.DataBindings.Add(new Binding("Text", bsMeasures, "epanet_pattern_file"));
            // Scheda "SAP"
            txtSAP_Code.DataBindings.Add(new Binding("Text", bsMeasures, "sap_code"));
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Esegue il binding dei dati
        /// </summary>
        void ExecBindings()
        {
            dsConnections = db_conn.GetDataset(CONNECTIONS_TABLE_NAME);
            bsConnections.DataSource = dsConnections.Tables[0];
            dsMeasures = db_conn.GetDataset(MEASURES_TABLE_NAME);
            bsMeasures.DataSource = dsMeasures.Tables[0];
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
                bsMeasures.EndEdit();
                dsMeasures.AcceptChanges();
                db_conn.TableSync(dsMeasures.Tables[0], MEASURES_TABLE_NAME);
                // Ricreo le associazioni dati
                ExecBindings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Ricerca di un file di pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdEpa_Browse_Click(object sender, EventArgs e)
        {
            DialogResult dr = dlgOpen.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Eseguo la compressione in formato GZip
                MemoryStream ms = new MemoryStream();
                FileInfo fi = new FileInfo(dlgOpen.FileName);
                using (GZipStream gz = new GZipStream(ms, CompressionLevel.Optimal))
                {
                    fi.OpenRead().CopyTo(gz);
                }
                byte[] buffer = ms.ToArray();
                txtEpa_PatternFile.Text = Convert.ToBase64String(buffer);
            }
        }

        /// <summary>
        /// Richiedo la chiusura della finestra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Esegue il commit dei dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCommit_Click(object sender, EventArgs e)
        {
            if (cboConnection.Text == string.Empty)
            {
                // Non è stata selezionata una connessione di riferimento, lo segnalo
                MessageBox.Show("A reference connection must be selected!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                ExecCommit();
        }

        /// <summary>
        /// Evento di chiusura in corso del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmModelling_Measures_FormClosing(object sender, FormClosingEventArgs e)
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
        /// Cambiamento del DSN di riferimento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query;

            try
            {
                // Compongo la query per la visualizzazione delle tabelle in base al database
                WetLib.WetDBConn conn = new WetLib.WetDBConn(cboConnection.Text, false);
                switch (conn.GetServerType())
                {
                    case WetLib.WetDBConn.DBServerTypes.MYSQL:
                        query = "SHOW TABLES";
                        break;

                    default:
                        return; // Valore inatteso
                }

                // Acquisisco la lista delle tabelle
                DataTable dt = conn.ExecCustomQuery(query);
                if (dt.Rows.Count > 0)
                {
                    cboTable.Items.Clear();
                    // Aggiungo gli item alla combo box
                    foreach (DataRow dr in dt.Rows)
                    {
                        cboTable.Items.Add(Convert.ToString(dr[0]));
                    }
                    cboTable.DataBindings.Clear();
                    cboTable.DataBindings.Add(new Binding("Text", bsMeasures, "table_name"));
                }
            }
            catch { }
        }

        /// <summary>
        /// Cambiamento della tabella di riferimento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WetLib.WetDBConn conn = new WetLib.WetDBConn(cboConnection.Text, false);
                switch (conn.GetServerType())
                {
                    case WetLib.WetDBConn.DBServerTypes.MYSQL:
                        DataTable dt = conn.ExecCustomQuery("SHOW COLUMNS IN " + cboTable.Text);
                        if (dt.Rows.Count > 0)
                        {
                            cboTable_TimestampColumn.Items.Clear();
                            cboTable_ValueColumn.Items.Clear();
                            cboTable_RelationalIDColumn.Items.Clear();
                            cboTable_RelationalIDValue.Items.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                cboTable_TimestampColumn.Items.Add(Convert.ToString(dr[0]));
                                cboTable_ValueColumn.Items.Add(Convert.ToString(dr[0]));
                                if (Convert.ToString(dr[3]).ToUpper() == "PRI")
                                {
                                    // Sono su una chiave primaria
                                    cboTable_RelationalIDColumn.Items.Add(Convert.ToString(dr[0]));                                    
                                }
                            }
                        }
                        break;

                    default:
                        return;     // Valore inatteso
                }
            }
            catch { }
        }

        /// <summary>
        /// Cambiamento della chiave primaria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboTable_RelationalIDColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WetLib.WetDBConn conn = new WetLib.WetDBConn(cboConnection.Text, false);
                switch (conn.GetServerType())
                {
                    case WetLib.WetDBConn.DBServerTypes.MYSQL:
                        // Imposto il tipo di valore della chiave primaria selezionata
                        cboTable_RelationalIDType.SelectedValue = (int)conn.GetPrimaryKeyColumnType(cboTable.Text, cboTable_RelationalIDColumn.Text);
                        // Popolo la combo per la scelta della chiave primaria collegata
                        KeyValuePair<string, string> source = conn.GetReferencedTableAndPKFromFK(cboTable.Text, cboTable_RelationalIDColumn.Text);
                        DataTable dt = conn.ExecCustomQuery("SELECT `" + source.Value + "` FROM `" + source.Key + "`");
                        foreach (DataRow dr in dt.Rows)
                            cboTable_RelationalIDValue.Items.Add(Convert.ToString(dr[0]));
                        cboTable_RelationalIDValue.DataBindings.Add(new Binding("Text", bsMeasures, "table_relational_id_value"));
                        break;

                    default:
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// Evento di aggiunta di un nuovo record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsMeasures_AddingNew(object sender, AddingNewEventArgs e)
        {
            DataView dtv = (DataView)bsMeasures.List;
            DataRowView drv = dtv.AddNew();

            WetLib.WetConfig.AnalysisSettings analysis_default_settings = cfg.GetDefaultAnalysisSettings();

            // Impostazione valori di default
            drv[0] = 0;
            drv[1] = string.Empty;
            drv[3] = 0;
            drv[4] = 1;
            drv[5] = DateTime.Now;
            drv[6] = new TimeSpan(analysis_default_settings.min_night_start_time.Hour, analysis_default_settings.min_night_start_time.Minute, analysis_default_settings.min_night_start_time.Second);
            drv[7] = new TimeSpan(analysis_default_settings.min_night_stop_time.Hour, analysis_default_settings.min_night_stop_time.Minute, analysis_default_settings.min_night_stop_time.Second);
            drv[8] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_1.Hour, analysis_default_settings.max_day_start_time_slot_1.Minute, analysis_default_settings.max_day_start_time_slot_1.Second);
            drv[9] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_1.Hour, analysis_default_settings.max_day_stop_time_slot_1.Minute, analysis_default_settings.max_day_stop_time_slot_1.Second);
            drv[10] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_2.Hour, analysis_default_settings.max_day_start_time_slot_2.Minute, analysis_default_settings.max_day_start_time_slot_2.Second);
            drv[11] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_2.Hour, analysis_default_settings.max_day_stop_time_slot_2.Minute, analysis_default_settings.max_day_stop_time_slot_2.Second);
            drv[12] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_3.Hour, analysis_default_settings.max_day_start_time_slot_3.Minute, analysis_default_settings.max_day_start_time_slot_3.Second);
            drv[13] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_3.Hour, analysis_default_settings.max_day_stop_time_slot_3.Minute, analysis_default_settings.max_day_stop_time_slot_3.Second);
            drv["fixed_value"] = 0.0d;
            drv["critical"] = 0;
            drv["alarm_thresholds_enable"] = 0;
            drv["alarm_min_threshold"] = 0.0d;
            drv["alarm_max_threshold"] = 0.0d;
            drv["alarm_constant_check_enable"] = 0;
            drv["alarm_constant_hysteresis"] = 0.0d;
            drv["alarm_constant_check_time"] = 0;
            drv["table_name"] = string.Empty;
            drv["table_timestamp_column"] = string.Empty;
            drv["table_value_column"] = string.Empty;

            e.NewObject = drv;

            bsMeasures.MoveLast(); 
        }

        /// <summary>
        /// Evento di visualizzazione del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmModelling_Measures_Shown(object sender, EventArgs e)
        {
            bsMeasures.ResetBindings(false);
        }

        /// <summary>
        /// Richiesta di aggiunta di un record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            tabMain.SelectTab(0);
            cmdCommit.Enabled = true;
        }
    }
}
