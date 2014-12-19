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
    /// Form per la definizione dei distretti
    /// </summary>
    public partial class frmModelling_Districts : Form
    {
        #region Costanti

        /// <summary>
        /// Nome tabella anagrafica distretti
        /// </summary>
        const string DISTRICTS_TABLE_NAME = "districts";

        /// <summary>
        /// Nome tabella misure
        /// </summary>
        const string MEASURES_TABLE_NAME = "measures";

        /// <summary>
        /// Tabella di collegamento n:n misure-distretti
        /// </summary>
        const string MEASURES_HAS_DISTRICTS_TABLE_NAME = "measures_has_districts";

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
        public frmModelling_Districts()
        {
            // Inizializzazione automatica dei componenti
            InitializeComponent();
            // Istanzio la configurazione
            cfg = new WetLib.WetConfig();
            // Istanzio la connessione al database
            db_conn = new WetLib.WetDBConn(cfg.GetWetDBDSN(), true);
            // Eseguo il binding dei dati
            ExecBindings();           
            // Associo i controlli
            bnDistricts.BindingSource = bsDistricts;
            dgDistricts.DataSource = bsDistricts;
            dgDistricts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Scheda "Generals"
            txtID.DataBindings.Add(new Binding("Text", bsDistricts, "id_districts"));
            txtName.DataBindings.Add(new Binding("Text", bsDistricts, "name"));
            txtZone.DataBindings.Add(new Binding("Text", bsDistricts, "zone"));
            Dictionary<int, string> classes = new Dictionary<int, string>()
            {
                { (int)WetLib.DistrictClasses.A, WetLib.DistrictClasses.A.ToString() },
                { (int)WetLib.DistrictClasses.B, WetLib.DistrictClasses.B.ToString() },
                { (int)WetLib.DistrictClasses.C, WetLib.DistrictClasses.C.ToString() },
            };
            cboClass.DataSource = new BindingSource(classes, null);
            cboClass.DisplayMember = "Value";
            cboClass.ValueMember = "Key";
            cboClass.DataBindings.Add(new Binding("SelectedValue", bsDistricts, "class"));
            txtMunicipality.DataBindings.Add(new Binding("Text", bsDistricts, "municipality"));
            numInhabitants.Maximum = long.MaxValue;
            numInhabitants.DataBindings.Add(new Binding("Value", bsDistricts, "inhabitants", true, DataSourceUpdateMode.OnValidation));
            dtpUpdateTimestamp.DataBindings.Add(new Binding("Text", bsDistricts, "update_timestamp", true, DataSourceUpdateMode.OnValidation));
            numUnitaryPhisiologicalNightDemand.DataBindings.Add(new Binding("Value", bsDistricts, "unitary_phisiological_nigth_demand", true, DataSourceUpdateMode.OnValidation));
            numProperties.DataBindings.Add(new Binding("Value", bsDistricts, "properties", true, DataSourceUpdateMode.OnValidation));
            numNotRewardedWater.DataBindings.Add(new Binding("Value", bsDistricts, "rewarded_water", true, DataSourceUpdateMode.OnValidation));            
            numBilling.DataBindings.Add(new Binding("Value", bsDistricts, "billing", true, DataSourceUpdateMode.OnValidation));
            numNotHouseholdNightUse.DataBindings.Add(new Binding("Value", bsDistricts, "not_household_night_use", true, DataSourceUpdateMode.OnValidation));
            numLengthMain.DataBindings.Add(new Binding("Value", bsDistricts, "length_main", true, DataSourceUpdateMode.OnValidation));
            numAverageZoneNightPressure.DataBindings.Add(new Binding("Value", bsDistricts, "average_zone_night_pressure", true, DataSourceUpdateMode.OnValidation));
            numHouseholdNightUse.DataBindings.Add(new Binding("Value", bsDistricts, "household_night_use", true, DataSourceUpdateMode.OnValidation));
            numAlphaEmitterExponent.DataBindings.Add(new Binding("Value", bsDistricts, "alpha_emitter_exponent", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Analytics Data"                                  
            dtpMinNight_Start.DataBindings.Add(new Binding("Text", bsDistricts, "min_night_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpMinNight_Stop.DataBindings.Add(new Binding("Text", bsDistricts, "min_night_stop_time", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot1_Start.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_start_time_1", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot1_Stop.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_stop_time_1", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot2_Start.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_start_time_2", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot2_Stop.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_stop_time_2", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot3_Start.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_start_time_3", true, DataSourceUpdateMode.OnValidation));
            dtpMaxDay_Slot3_Stop.DataBindings.Add(new Binding("Text", bsDistricts, "max_day_stop_time_3", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Events"
            numEvLowBand.DataBindings.Add(new Binding("Value", bsDistricts, "ev_low_band", true, DataSourceUpdateMode.OnValidation));
            numEvHighBand.DataBindings.Add(new Binding("Value", bsDistricts, "ev_high_band", true, DataSourceUpdateMode.OnValidation));
            txtEvRecommendedLowBand.DataBindings.Add(new Binding("Text", bsDistricts, "ev_statistic_low_band"));
            txtEvRecommendedHighBand.DataBindings.Add(new Binding("Text", bsDistricts, "ev_statistic_high_band"));
            Dictionary<int, string> ev_measure_types = new Dictionary<int, string>()
            {
                { (int)WetLib.DistrictStatisticMeasureType.MIN_NIGHT, WetLib.DistrictStatisticMeasureType.MIN_NIGHT.ToString() },
                { (int)WetLib.DistrictStatisticMeasureType.MIN_DAY, WetLib.DistrictStatisticMeasureType.MIN_DAY.ToString() },
                { (int)WetLib.DistrictStatisticMeasureType.MAX_DAY, WetLib.DistrictStatisticMeasureType.MAX_DAY.ToString() },
                { (int)WetLib.DistrictStatisticMeasureType.AVG_DAY, WetLib.DistrictStatisticMeasureType.AVG_DAY.ToString() },
                { (int)WetLib.DistrictStatisticMeasureType.STATISTICAL_PROFILE, WetLib.DistrictStatisticMeasureType.STATISTICAL_PROFILE.ToString() }
            };
            cboEvVariableType.DataSource = new BindingSource(ev_measure_types, null);
            cboEvVariableType.DisplayMember = "Value";
            cboEvVariableType.ValueMember = "Key";
            cboEvVariableType.DataBindings.Add(new Binding("SelectedValue", bsDistricts, "ev_variable_type"));
            dtpEvLastGoodSampleDay.DataBindings.Add(new Binding("Text", bsDistricts, "ev_last_good_sample_day", true, DataSourceUpdateMode.OnValidation));
            numEvLastGoodSamples.DataBindings.Add(new Binding("Value", bsDistricts, "ev_last_good_samples", true, DataSourceUpdateMode.OnValidation));
            numEvAlpha.DataBindings.Add(new Binding("Value", bsDistricts, "ev_alpha", true, DataSourceUpdateMode.OnValidation));
            numEvSamplesTrigger.DataBindings.Add(new Binding("Value", bsDistricts, "ev_samples_trigger", true, DataSourceUpdateMode.OnValidation));
            // Scheda "Alarms"
            chkAlarm1_Enable.DataBindings.Add(new Binding("Checked", bsDistricts, "alarm1_thresholds_enable", true));
            dtpAlarm1_StartTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm1_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpAlarm1_StopTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm1_stop_time", true, DataSourceUpdateMode.OnValidation));
            numAlarm1_Min_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm1_min_threshold", true, DataSourceUpdateMode.OnValidation));
            numAlarm1_Max_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm1_max_threshold", true, DataSourceUpdateMode.OnValidation));
            chkAlarm2_Enable.DataBindings.Add(new Binding("Checked", bsDistricts, "alarm2_thresholds_enable", true));
            dtpAlarm2_StartTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm2_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpAlarm2_StopTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm2_stop_time", true, DataSourceUpdateMode.OnValidation));
            numAlarm2_Min_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm2_min_threshold", true, DataSourceUpdateMode.OnValidation));
            numAlarm2_Max_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm2_max_threshold", true, DataSourceUpdateMode.OnValidation));
            chkAlarm3_Enable.DataBindings.Add(new Binding("Checked", bsDistricts, "alarm3_thresholds_enable", true));
            dtpAlarm3_StartTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm3_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpAlarm3_StopTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm3_stop_time", true, DataSourceUpdateMode.OnValidation));
            numAlarm3_Min_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm3_min_threshold", true, DataSourceUpdateMode.OnValidation));
            numAlarm3_Max_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm3_max_threshold", true, DataSourceUpdateMode.OnValidation));
            chkAlarm4_Enable.DataBindings.Add(new Binding("Checked", bsDistricts, "alarm4_thresholds_enable", true));
            dtpAlarm4_StartTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm4_start_time", true, DataSourceUpdateMode.OnValidation));
            dtpAlarm4_StopTime.DataBindings.Add(new Binding("Text", bsDistricts, "alarm4_stop_time", true, DataSourceUpdateMode.OnValidation));
            numAlarm4_Min_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm4_min_threshold", true, DataSourceUpdateMode.OnValidation));
            numAlarm4_Max_Threshold.DataBindings.Add(new Binding("Value", bsDistricts, "alarm4_max_threshold", true, DataSourceUpdateMode.OnValidation));
            // Scheda "SAP"
            txtSAP_Code.DataBindings.Add(new Binding("Text", bsDistricts, "sap_code"));
            // Scheda "Measures"
            dgAllMeasures.DataSource = bsMeasures;
            dgAllMeasures.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgMeasuresOfDistrict.DataSource = bsDistrictMeasures;
            dgMeasuresOfDistrict.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;         
            Dictionary<int, string> measures_signs = new Dictionary<int, string>()
            {
                { (int)WetLib.DistrictsMeasuresSigns.PLUS, WetLib.DistrictsMeasuresSigns.PLUS.ToString() },
                { (int)WetLib.DistrictsMeasuresSigns.MINUS, WetLib.DistrictsMeasuresSigns.MINUS.ToString() },
                { (int)WetLib.DistrictsMeasuresSigns.DIVISION, WetLib.DistrictsMeasuresSigns.DIVISION.ToString() }
            };
            cboSign.DataSource = new BindingSource(measures_signs, null);
            cboSign.DisplayMember = "Value";
            cboSign.ValueMember = "Key";         
            cmdAdd.Image = Resources_PNG_16x16.arrow_right_2;
            cmdRemove.Image = Resources_PNG_16x16.arrow_left_2;
            // Scheda "Note"
            txtNotes.DataBindings.Add(new Binding("Text", bsDistricts, "notes"));
        }

        #endregion

        #region Funzioni del modulo

        /// <summary>
        /// Esegue la query per visualizzare le misure associate al distretto
        /// </summary>
        void ExecDistrictMeasuresQuery()
        {
            dsDistrictMeasures.Tables.Clear();
            dsDistrictMeasures.Tables.Add(db_conn.ExecCustomQuery("SELECT id_measures, name, description, sign FROM measures INNER JOIN measures_has_districts ON measures.id_measures=measures_has_districts.measures_id_measures WHERE measures_has_districts.districts_id_districts=" + (txtID.Text == string.Empty ? "0" : txtID.Text)));
            bsDistrictMeasures.DataSource = dsDistrictMeasures.Tables[0];
        }

        /// <summary>
        /// Esegue il binding dei dati
        /// </summary>
        void ExecBindings()
        {
            try
            {
                dsDistricts = db_conn.GetDataset(DISTRICTS_TABLE_NAME);
                bsDistricts.DataSource = dsDistricts.Tables[0];
                dsMeasures = db_conn.GetDataset(MEASURES_TABLE_NAME);
                bsMeasures.DataSource = dsMeasures.Tables[0];
                dsMeasuresHasDistricts = db_conn.GetDataset(MEASURES_HAS_DISTRICTS_TABLE_NAME);
                bsMeasuresHasDistricts.DataSource = dsMeasuresHasDistricts.Tables[0];
                ExecDistrictMeasuresQuery();
            }
            catch { }
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
                bsDistricts.EndEdit();
                bsMeasuresHasDistricts.EndEdit();
                dsDistricts.AcceptChanges();
                dsMeasuresHasDistricts.AcceptChanges();
                // Controllo i campi "DATE" critici                
                foreach (DataRow dr in dsDistricts.Tables[0].Rows)
                {
                    if (dr["ev_last_good_sample_day"] == DBNull.Value)
                        dr["ev_last_good_sample_day"] = DateTime.Now.Date;                    
                }
                // Effettuo la sincronizzazione delle tabelle
                db_conn.TableSync(dsDistricts.Tables[0], DISTRICTS_TABLE_NAME);
                try
                {
                    db_conn.TableSync(dsMeasuresHasDistricts.Tables[0], MEASURES_HAS_DISTRICTS_TABLE_NAME);
                }
                catch { }
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
        /// Evento sulla richiesta di chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmModelling_Districts_FormClosing(object sender, FormClosingEventArgs e)
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
        /// Evento di aggiunta di un nuovo record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsDistricts_AddingNew(object sender, AddingNewEventArgs e)
        {
            DataView dtv = (DataView)bsDistricts.List;
            DataRowView drv = dtv.AddNew();

            WetLib.WetConfig.AnalysisSettings analysis_default_settings = cfg.GetDefaultAnalysisSettings();

            // Impostazione valori di default
            drv[0] = DBNull.Value;
            drv[1] = string.Empty;
            drv[3] = 0;
            drv[6] = DateTime.Now;
            drv[7] = new TimeSpan(analysis_default_settings.min_night_start_time.Hour, analysis_default_settings.min_night_start_time.Minute, analysis_default_settings.min_night_start_time.Second);
            drv[8] = new TimeSpan(analysis_default_settings.min_night_stop_time.Hour, analysis_default_settings.min_night_stop_time.Minute, analysis_default_settings.min_night_stop_time.Second);
            drv[9] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_1.Hour, analysis_default_settings.max_day_start_time_slot_1.Minute, analysis_default_settings.max_day_start_time_slot_1.Second);
            drv[10] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_1.Hour, analysis_default_settings.max_day_stop_time_slot_1.Minute, analysis_default_settings.max_day_stop_time_slot_1.Second);
            drv[11] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_2.Hour, analysis_default_settings.max_day_start_time_slot_2.Minute, analysis_default_settings.max_day_start_time_slot_2.Second);
            drv[12] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_2.Hour, analysis_default_settings.max_day_stop_time_slot_2.Minute, analysis_default_settings.max_day_stop_time_slot_2.Second);
            drv[13] = new TimeSpan(analysis_default_settings.max_day_start_time_slot_3.Hour, analysis_default_settings.max_day_start_time_slot_3.Minute, analysis_default_settings.max_day_start_time_slot_3.Second);
            drv[14] = new TimeSpan(analysis_default_settings.max_day_stop_time_slot_3.Hour, analysis_default_settings.max_day_stop_time_slot_3.Minute, analysis_default_settings.max_day_stop_time_slot_3.Second);
            drv["unitary_phisiological_nigth_demand"] = 2.0d;
            drv["billing"] = new decimal(1.8d);
            drv["alarm1_thresholds_enable"] = 0;
            drv["alarm1_start_time"] = new TimeSpan();
            drv["alarm1_stop_time"] = new TimeSpan();
            drv["alarm1_min_threshold"] = 0.0d;
            drv["alarm1_max_threshold"] = 0.0d;
            drv["alarm2_thresholds_enable"] = 0;
            drv["alarm2_start_time"] = new TimeSpan();
            drv["alarm2_stop_time"] = new TimeSpan();
            drv["alarm2_min_threshold"] = 0.0d;
            drv["alarm2_max_threshold"] = 0.0d;
            drv["alarm3_thresholds_enable"] = 0;
            drv["alarm3_start_time"] = new TimeSpan();
            drv["alarm3_stop_time"] = new TimeSpan();
            drv["alarm3_min_threshold"] = 0.0d;
            drv["alarm3_max_threshold"] = 0.0d;
            drv["alarm4_thresholds_enable"] = 0;
            drv["alarm4_start_time"] = new TimeSpan();
            drv["alarm4_stop_time"] = new TimeSpan();
            drv["alarm4_min_threshold"] = 0.0d;
            drv["alarm4_max_threshold"] = 0.0d;
            drv["ev_high_band"] = 0.0d;
            drv["ev_low_band"] = 0.0d;
            drv["ev_statistic_high_band"] = 0.0d;
            drv["ev_statistic_low_band"] = 0.0d;
            drv["ev_variable_type"] = (int)WetLib.DistrictStatisticMeasureType.MIN_NIGHT;
            drv["ev_last_good_sample_day"] = DateTime.Now.Date;
            drv["ev_last_good_samples"] = 10;
            drv["ev_alpha"] = 2;
            drv["ev_samples_trigger"] = 3;
            
            e.NewObject = drv;

            bsDistricts.MoveLast();            
        }

        /// <summary>
        /// Aggiunta di una nuova misura al distretto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {                
                if (((DataRowView)bsDistricts.Current)[0] == DBNull.Value)
                    throw new Exception("You must save the district before adding measures!");
                // Acquisisco il segno della misura
                int sign = (int)cboSign.SelectedValue;
                // Acquisisco la misura selezionata
                DataRow measure = ((DataRowView)bsMeasures.Current).Row;
                // Acquisco il distretto selezionato
                DataRow district = ((DataRowView)bsDistricts.Current).Row;
                // Controllo la coerenza di segno, la divisione vale solo per le pressioni
                if ((((WetLib.MeasureTypes)measure["type"]) != WetLib.MeasureTypes.PRESSURE) &&
                    ((WetLib.DistrictsMeasuresSigns)sign == WetLib.DistrictsMeasuresSigns.DIVISION))
                    throw new Exception("Sign DIVISION is valid only on pressure measure!");
                // Aggiungo il record nella tabella di interscambio                
                DataRow dr = dsMeasuresHasDistricts.Tables[0].NewRow();
                dr["measures_id_measures"] = Convert.ToInt32(measure["id_measures"]);
                dr["measures_connections_id_odbcdsn"] = Convert.ToInt32(measure["connections_id_odbcdsn"]);
                dr["districts_id_districts"] = Convert.ToInt32(district["id_districts"]);
                dr["sign"] = sign;
                dsMeasuresHasDistricts.Tables[0].Rows.Add(dr);
                ExecCommit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Esegue il commit dei dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCommit_Click(object sender, EventArgs e)
        {
            ExecCommit();
        }

        /// <summary>
        /// Richiede la chiusura del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Evento di aggiunta di righe alla tabella misure del distretto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMeasuresOfDistrict_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                for (int ii = 0; ii < e.RowCount; ii++)
                {
                    int val = Convert.ToInt32(dgMeasuresOfDistrict.Rows[e.RowIndex + ii].Cells["sign"].Value);
                    string text = ((WetLib.DistrictsMeasuresSigns)val).ToString();
                    DataGridViewCellStyle cs = new DataGridViewCellStyle();
                    cs.Format = text;
                    dgMeasuresOfDistrict.Rows[e.RowIndex + ii].Cells["sign"].Style = cs;
                }
            }
            catch { }
        }

        /// <summary>
        /// Pulsante per la rimozione di una misura dal distretto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdRemove_Click(object sender, EventArgs e)
        {
            int measure_id = Convert.ToInt32(dgMeasuresOfDistrict.CurrentRow.Cells[0].Value);
            db_conn.ExecCustomCommand("DELETE FROM measures_has_districts WHERE measures_id_measures=" + Convert.ToString(measure_id));
            ExecBindings();
        }

        /// <summary>
        /// Evento di cambio posizione nel record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsDistricts_PositionChanged(object sender, EventArgs e)
        {
            ExecDistrictMeasuresQuery();
        }

        /// <summary>
        /// Evento di visualizzazione del form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmModelling_Districts_Shown(object sender, EventArgs e)
        {
            ExecDistrictMeasuresQuery();
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

        /// <summary>
        /// Computazione NFCU
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdNFCUCompute_Click(object sender, EventArgs e)
        {
            double hnu, inh, upnd;

            inh = Convert.ToDouble(numInhabitants.Value);
            upnd = Convert.ToDouble(numUnitaryPhisiologicalNightDemand.Value);
            hnu = inh * upnd / 3600.0d;

            numHouseholdNightUse.Value = Convert.ToDecimal(hnu);
        }

        /// <summary>
        /// Evento per il trasferimento dei valori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdEvTransfer_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure to set up this values?", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                numEvLowBand.Value = Convert.ToDecimal(txtEvRecommendedLowBand.Text);
                numEvHighBand.Value = Convert.ToDecimal(txtEvRecommendedHighBand.Text);
            }
        }
    }
}
