namespace WetAdmin
{
    partial class frmModelling_Measures
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModelling_Measures));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgDown = new System.Windows.Forms.DataGridView();
            this.bnMain = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdCommit = new System.Windows.Forms.ToolStripButton();
            this.cmdExit = new System.Windows.Forms.ToolStripButton();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabMain_General = new System.Windows.Forms.TabPage();
            this.dtpUpdateTimestamp = new System.Windows.Forms.DateTimePicker();
            this.lblUpdateTimestamp = new System.Windows.Forms.Label();
            this.numFixedValue = new System.Windows.Forms.NumericUpDown();
            this.lblFixedValue = new System.Windows.Forms.Label();
            this.chkCritical = new System.Windows.Forms.CheckBox();
            this.chkReliable = new System.Windows.Forms.CheckBox();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.lblID = new System.Windows.Forms.Label();
            this.tabMain_Database = new System.Windows.Forms.TabPage();
            this.cboTable = new System.Windows.Forms.ComboBox();
            this.lblTable = new System.Windows.Forms.Label();
            this.cboConnection = new System.Windows.Forms.ComboBox();
            this.lblConnection = new System.Windows.Forms.Label();
            this.grpTable = new System.Windows.Forms.GroupBox();
            this.cboTable_RelationalIDValue = new System.Windows.Forms.ComboBox();
            this.cboTable_RelationalIDColumn = new System.Windows.Forms.ComboBox();
            this.cboTable_RelationalIDType = new System.Windows.Forms.ComboBox();
            this.lblTable_RelationalIDType = new System.Windows.Forms.Label();
            this.lblTable_RelationalIDValue = new System.Windows.Forms.Label();
            this.lblTable_RelationalIDColumn = new System.Windows.Forms.Label();
            this.cboTable_ValueColumn = new System.Windows.Forms.ComboBox();
            this.lblTable_ValueColumn = new System.Windows.Forms.Label();
            this.cboTable_TimestampColumn = new System.Windows.Forms.ComboBox();
            this.lblTable_TimestampColumn = new System.Windows.Forms.Label();
            this.tabMain_Analytics = new System.Windows.Forms.TabPage();
            this.grpMaxDay = new System.Windows.Forms.GroupBox();
            this.grpMaxDaySlot2 = new System.Windows.Forms.GroupBox();
            this.dtpMaxDay_Slot2_Stop = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot2_Stop = new System.Windows.Forms.Label();
            this.dtpMaxDay_Slot2_Start = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot2_Start = new System.Windows.Forms.Label();
            this.grpMaxDaySlot3 = new System.Windows.Forms.GroupBox();
            this.dtpMaxDay_Slot3_Stop = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot3_Stop = new System.Windows.Forms.Label();
            this.dtpMaxDay_Slot3_Start = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot3_Start = new System.Windows.Forms.Label();
            this.grpMaxDaySlot1 = new System.Windows.Forms.GroupBox();
            this.dtpMaxDay_Slot1_Stop = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot1_Stop = new System.Windows.Forms.Label();
            this.dtpMaxDay_Slot1_Start = new System.Windows.Forms.DateTimePicker();
            this.lblMaxDay_Slot1_Start = new System.Windows.Forms.Label();
            this.grpMinNight = new System.Windows.Forms.GroupBox();
            this.dtpMinNight_Stop = new System.Windows.Forms.DateTimePicker();
            this.lblMinNight_Stop = new System.Windows.Forms.Label();
            this.dtpMinNight_Start = new System.Windows.Forms.DateTimePicker();
            this.lblMinNight_Start = new System.Windows.Forms.Label();
            this.tabEnergy = new System.Windows.Forms.TabPage();
            this.numEnergySpecificContent = new System.Windows.Forms.NumericUpDown();
            this.lblEnergySpecificContent = new System.Windows.Forms.Label();
            this.cboEnergyCategory = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.tabStrumentation = new System.Windows.Forms.TabPage();
            this.cboSt_LinkType = new System.Windows.Forms.ComboBox();
            this.lblSt_LinkType = new System.Windows.Forms.Label();
            this.txtSt_SerialNumber = new System.Windows.Forms.TextBox();
            this.lblSt_SerialNumber = new System.Windows.Forms.Label();
            this.txtSt_Model = new System.Windows.Forms.TextBox();
            this.lblSt_Model = new System.Windows.Forms.Label();
            this.cboSt_Type = new System.Windows.Forms.ComboBox();
            this.lblSt_Type = new System.Windows.Forms.Label();
            this.tabMain_Geolocation = new System.Windows.Forms.TabPage();
            this.num_zpos = new System.Windows.Forms.NumericUpDown();
            this.lbl_zpos = new System.Windows.Forms.Label();
            this.num_ypos = new System.Windows.Forms.NumericUpDown();
            this.lbl_ypos = new System.Windows.Forms.Label();
            this.num_xpos = new System.Windows.Forms.NumericUpDown();
            this.lbl_xpos = new System.Windows.Forms.Label();
            this.tabMain_Alarms = new System.Windows.Forms.TabPage();
            this.grpAlarms_ConstantCheck = new System.Windows.Forms.GroupBox();
            this.numConstant_Time = new System.Windows.Forms.NumericUpDown();
            this.lblConstant_Time = new System.Windows.Forms.Label();
            this.numConstant_Hysteresis = new System.Windows.Forms.NumericUpDown();
            this.lblConstant_Hysteresis = new System.Windows.Forms.Label();
            this.chkConstant_Enable = new System.Windows.Forms.CheckBox();
            this.grpAlarms_Thresholds = new System.Windows.Forms.GroupBox();
            this.numThresholds_Max = new System.Windows.Forms.NumericUpDown();
            this.lblThresholds_Max = new System.Windows.Forms.Label();
            this.numThresholds_Min = new System.Windows.Forms.NumericUpDown();
            this.lblThresholds_Min = new System.Windows.Forms.Label();
            this.chkThresholds_Enable = new System.Windows.Forms.CheckBox();
            this.tabMain_EPANET = new System.Windows.Forms.TabPage();
            this.cmdEpa_Browse = new System.Windows.Forms.Button();
            this.txtEpa_PatternFile = new System.Windows.Forms.TextBox();
            this.lblEpa_PatternFile = new System.Windows.Forms.Label();
            this.txtEpa_Pattern = new System.Windows.Forms.TextBox();
            this.lblEpa_Pattern = new System.Windows.Forms.Label();
            this.txtEpa_ObjectID = new System.Windows.Forms.TextBox();
            this.lblEpa_ObjectID = new System.Windows.Forms.Label();
            this.tabMain_SAP = new System.Windows.Forms.TabPage();
            this.txtSAP_Code = new System.Windows.Forms.TextBox();
            this.lblSAP_Code = new System.Windows.Forms.Label();
            this.dsMeasures = new System.Data.DataSet();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dsConnections = new System.Data.DataSet();
            this.bsMeasures = new System.Windows.Forms.BindingSource(this.components);
            this.bsConnections = new System.Windows.Forms.BindingSource(this.components);
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnMain)).BeginInit();
            this.bnMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabMain_General.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFixedValue)).BeginInit();
            this.tabMain_Database.SuspendLayout();
            this.grpTable.SuspendLayout();
            this.tabMain_Analytics.SuspendLayout();
            this.grpMaxDay.SuspendLayout();
            this.grpMaxDaySlot2.SuspendLayout();
            this.grpMaxDaySlot3.SuspendLayout();
            this.grpMaxDaySlot1.SuspendLayout();
            this.grpMinNight.SuspendLayout();
            this.tabEnergy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEnergySpecificContent)).BeginInit();
            this.tabStrumentation.SuspendLayout();
            this.tabMain_Geolocation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_zpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ypos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_xpos)).BeginInit();
            this.tabMain_Alarms.SuspendLayout();
            this.grpAlarms_ConstantCheck.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConstant_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConstant_Hysteresis)).BeginInit();
            this.grpAlarms_Thresholds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholds_Max)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholds_Min)).BeginInit();
            this.tabMain_EPANET.SuspendLayout();
            this.tabMain_SAP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsMeasures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsMeasures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.dgDown, 0, 1);
            this.tlpMain.Controls.Add(this.bnMain, 0, 2);
            this.tlpMain.Controls.Add(this.tabMain, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 330F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.Size = new System.Drawing.Size(714, 507);
            this.tlpMain.TabIndex = 0;
            // 
            // dgDown
            // 
            this.dgDown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDown.Location = new System.Drawing.Point(3, 333);
            this.dgDown.Name = "dgDown";
            this.dgDown.ReadOnly = true;
            this.dgDown.RowTemplate.Height = 24;
            this.dgDown.Size = new System.Drawing.Size(708, 144);
            this.dgDown.TabIndex = 2;
            // 
            // bnMain
            // 
            this.bnMain.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnMain.CountItem = this.bindingNavigatorCountItem;
            this.bnMain.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.cmdCommit,
            this.cmdExit});
            this.bnMain.Location = new System.Drawing.Point(0, 480);
            this.bnMain.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnMain.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnMain.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnMain.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnMain.Name = "bnMain";
            this.bnMain.PositionItem = this.bindingNavigatorPositionItem;
            this.bnMain.ShowItemToolTips = false;
            this.bnMain.Size = new System.Drawing.Size(714, 27);
            this.bnMain.TabIndex = 1;
            this.bnMain.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.bindingNavigatorAddNewItem_Click);
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(44, 24);
            this.bindingNavigatorCountItem.Text = "di {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Numero totale di elementi";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveFirstItem.Text = "Sposta in prima posizione";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMovePreviousItem.Text = "Sposta indietro";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Posizione";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Posizione corrente";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveNextItem.Text = "Sposta avanti";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveLastItem.Text = "Sposta in ultima posizione";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // cmdCommit
            // 
            this.cmdCommit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdCommit.Enabled = false;
            this.cmdCommit.Image = global::WetAdmin.Resources_PNG_16x16.dialog_ok_apply_4;
            this.cmdCommit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdCommit.Name = "cmdCommit";
            this.cmdCommit.Size = new System.Drawing.Size(23, 24);
            this.cmdCommit.Text = "Commit changes";
            this.cmdCommit.Click += new System.EventHandler(this.cmdCommit_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdExit.Image = global::WetAdmin.Resources_PNG_16x16.application_exit_2;
            this.cmdExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(23, 24);
            this.cmdExit.Text = "Exit";
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabMain_General);
            this.tabMain.Controls.Add(this.tabMain_Database);
            this.tabMain.Controls.Add(this.tabMain_Analytics);
            this.tabMain.Controls.Add(this.tabEnergy);
            this.tabMain.Controls.Add(this.tabStrumentation);
            this.tabMain.Controls.Add(this.tabMain_Geolocation);
            this.tabMain.Controls.Add(this.tabMain_Alarms);
            this.tabMain.Controls.Add(this.tabMain_EPANET);
            this.tabMain.Controls.Add(this.tabMain_SAP);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(3, 3);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(708, 324);
            this.tabMain.TabIndex = 0;
            // 
            // tabMain_General
            // 
            this.tabMain_General.Controls.Add(this.dtpUpdateTimestamp);
            this.tabMain_General.Controls.Add(this.lblUpdateTimestamp);
            this.tabMain_General.Controls.Add(this.numFixedValue);
            this.tabMain_General.Controls.Add(this.lblFixedValue);
            this.tabMain_General.Controls.Add(this.chkCritical);
            this.tabMain_General.Controls.Add(this.chkReliable);
            this.tabMain_General.Controls.Add(this.cboType);
            this.tabMain_General.Controls.Add(this.lblType);
            this.tabMain_General.Controls.Add(this.txtDescription);
            this.tabMain_General.Controls.Add(this.lblDescription);
            this.tabMain_General.Controls.Add(this.txtName);
            this.tabMain_General.Controls.Add(this.lblName);
            this.tabMain_General.Controls.Add(this.txtID);
            this.tabMain_General.Controls.Add(this.lblID);
            this.tabMain_General.Location = new System.Drawing.Point(4, 25);
            this.tabMain_General.Name = "tabMain_General";
            this.tabMain_General.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain_General.Size = new System.Drawing.Size(700, 295);
            this.tabMain_General.TabIndex = 0;
            this.tabMain_General.Text = "General";
            this.tabMain_General.UseVisualStyleBackColor = true;
            // 
            // dtpUpdateTimestamp
            // 
            this.dtpUpdateTimestamp.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpUpdateTimestamp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpUpdateTimestamp.Location = new System.Drawing.Point(9, 219);
            this.dtpUpdateTimestamp.Name = "dtpUpdateTimestamp";
            this.dtpUpdateTimestamp.Size = new System.Drawing.Size(214, 22);
            this.dtpUpdateTimestamp.TabIndex = 13;
            // 
            // lblUpdateTimestamp
            // 
            this.lblUpdateTimestamp.AutoSize = true;
            this.lblUpdateTimestamp.Location = new System.Drawing.Point(6, 199);
            this.lblUpdateTimestamp.Name = "lblUpdateTimestamp";
            this.lblUpdateTimestamp.Size = new System.Drawing.Size(183, 17);
            this.lblUpdateTimestamp.TabIndex = 11;
            this.lblUpdateTimestamp.Text = "Measure update timestamp:";
            // 
            // numFixedValue
            // 
            this.numFixedValue.DecimalPlaces = 2;
            this.numFixedValue.Location = new System.Drawing.Point(9, 165);
            this.numFixedValue.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.numFixedValue.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.numFixedValue.Name = "numFixedValue";
            this.numFixedValue.Size = new System.Drawing.Size(106, 22);
            this.numFixedValue.TabIndex = 3;
            // 
            // lblFixedValue
            // 
            this.lblFixedValue.AutoSize = true;
            this.lblFixedValue.Location = new System.Drawing.Point(6, 145);
            this.lblFixedValue.Name = "lblFixedValue";
            this.lblFixedValue.Size = new System.Drawing.Size(150, 17);
            this.lblFixedValue.TabIndex = 10;
            this.lblFixedValue.Text = "Fixed value (required):";
            // 
            // chkCritical
            // 
            this.chkCritical.AutoSize = true;
            this.chkCritical.Location = new System.Drawing.Point(254, 166);
            this.chkCritical.Name = "chkCritical";
            this.chkCritical.Size = new System.Drawing.Size(139, 21);
            this.chkCritical.TabIndex = 5;
            this.chkCritical.Text = "Critical (required)";
            this.chkCritical.UseVisualStyleBackColor = true;
            // 
            // chkReliable
            // 
            this.chkReliable.AutoSize = true;
            this.chkReliable.Location = new System.Drawing.Point(254, 120);
            this.chkReliable.Name = "chkReliable";
            this.chkReliable.Size = new System.Drawing.Size(148, 21);
            this.chkReliable.TabIndex = 4;
            this.chkReliable.Text = "Reliable (required)";
            this.chkReliable.UseVisualStyleBackColor = true;
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "FLOW",
            "PRESSURE",
            "COUNTER"});
            this.cboType.Location = new System.Drawing.Point(9, 118);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(214, 24);
            this.cboType.TabIndex = 2;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 98);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(111, 17);
            this.lblType.TabIndex = 6;
            this.lblType.Text = "Type (required):";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(254, 24);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDescription.Size = new System.Drawing.Size(280, 71);
            this.txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(251, 3);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(83, 17);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(9, 73);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(214, 22);
            this.txtName.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 53);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(116, 17);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name (required):";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(9, 24);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(106, 22);
            this.txtID.TabIndex = 6;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(6, 3);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(25, 17);
            this.lblID.TabIndex = 0;
            this.lblID.Text = "ID:";
            // 
            // tabMain_Database
            // 
            this.tabMain_Database.Controls.Add(this.cboTable);
            this.tabMain_Database.Controls.Add(this.lblTable);
            this.tabMain_Database.Controls.Add(this.cboConnection);
            this.tabMain_Database.Controls.Add(this.lblConnection);
            this.tabMain_Database.Controls.Add(this.grpTable);
            this.tabMain_Database.Location = new System.Drawing.Point(4, 25);
            this.tabMain_Database.Name = "tabMain_Database";
            this.tabMain_Database.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain_Database.Size = new System.Drawing.Size(700, 295);
            this.tabMain_Database.TabIndex = 5;
            this.tabMain_Database.Text = "Database";
            this.tabMain_Database.UseVisualStyleBackColor = true;
            // 
            // cboTable
            // 
            this.cboTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable.FormattingEnabled = true;
            this.cboTable.Location = new System.Drawing.Point(19, 94);
            this.cboTable.Name = "cboTable";
            this.cboTable.Size = new System.Drawing.Size(214, 24);
            this.cboTable.TabIndex = 1;
            this.cboTable.SelectedIndexChanged += new System.EventHandler(this.cboTable_SelectedIndexChanged);
            // 
            // lblTable
            // 
            this.lblTable.AutoSize = true;
            this.lblTable.Location = new System.Drawing.Point(16, 74);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(115, 17);
            this.lblTable.TabIndex = 2;
            this.lblTable.Text = "Table (required):";
            // 
            // cboConnection
            // 
            this.cboConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConnection.FormattingEnabled = true;
            this.cboConnection.Location = new System.Drawing.Point(10, 27);
            this.cboConnection.Name = "cboConnection";
            this.cboConnection.Size = new System.Drawing.Size(214, 24);
            this.cboConnection.TabIndex = 0;
            this.cboConnection.SelectedIndexChanged += new System.EventHandler(this.cboConnection_SelectedIndexChanged);
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Location = new System.Drawing.Point(7, 7);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(150, 17);
            this.lblConnection.TabIndex = 0;
            this.lblConnection.Text = "Connection (required):";
            // 
            // grpTable
            // 
            this.grpTable.Controls.Add(this.cboTable_RelationalIDValue);
            this.grpTable.Controls.Add(this.cboTable_RelationalIDColumn);
            this.grpTable.Controls.Add(this.cboTable_RelationalIDType);
            this.grpTable.Controls.Add(this.lblTable_RelationalIDType);
            this.grpTable.Controls.Add(this.lblTable_RelationalIDValue);
            this.grpTable.Controls.Add(this.lblTable_RelationalIDColumn);
            this.grpTable.Controls.Add(this.cboTable_ValueColumn);
            this.grpTable.Controls.Add(this.lblTable_ValueColumn);
            this.grpTable.Controls.Add(this.cboTable_TimestampColumn);
            this.grpTable.Controls.Add(this.lblTable_TimestampColumn);
            this.grpTable.Location = new System.Drawing.Point(10, 98);
            this.grpTable.Name = "grpTable";
            this.grpTable.Size = new System.Drawing.Size(679, 141);
            this.grpTable.TabIndex = 4;
            this.grpTable.TabStop = false;
            this.grpTable.Text = "groupBox1";
            // 
            // cboTable_RelationalIDValue
            // 
            this.cboTable_RelationalIDValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable_RelationalIDValue.FormattingEnabled = true;
            this.cboTable_RelationalIDValue.Location = new System.Drawing.Point(232, 103);
            this.cboTable_RelationalIDValue.Name = "cboTable_RelationalIDValue";
            this.cboTable_RelationalIDValue.Size = new System.Drawing.Size(214, 24);
            this.cboTable_RelationalIDValue.TabIndex = 5;
            // 
            // cboTable_RelationalIDColumn
            // 
            this.cboTable_RelationalIDColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable_RelationalIDColumn.FormattingEnabled = true;
            this.cboTable_RelationalIDColumn.Location = new System.Drawing.Point(12, 103);
            this.cboTable_RelationalIDColumn.Name = "cboTable_RelationalIDColumn";
            this.cboTable_RelationalIDColumn.Size = new System.Drawing.Size(214, 24);
            this.cboTable_RelationalIDColumn.TabIndex = 4;
            this.cboTable_RelationalIDColumn.SelectedIndexChanged += new System.EventHandler(this.cboTable_RelationalIDColumn_SelectedIndexChanged);
            // 
            // cboTable_RelationalIDType
            // 
            this.cboTable_RelationalIDType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable_RelationalIDType.Enabled = false;
            this.cboTable_RelationalIDType.FormattingEnabled = true;
            this.cboTable_RelationalIDType.Location = new System.Drawing.Point(452, 103);
            this.cboTable_RelationalIDType.Name = "cboTable_RelationalIDType";
            this.cboTable_RelationalIDType.Size = new System.Drawing.Size(214, 24);
            this.cboTable_RelationalIDType.TabIndex = 6;
            // 
            // lblTable_RelationalIDType
            // 
            this.lblTable_RelationalIDType.AutoSize = true;
            this.lblTable_RelationalIDType.Location = new System.Drawing.Point(449, 85);
            this.lblTable_RelationalIDType.Name = "lblTable_RelationalIDType";
            this.lblTable_RelationalIDType.Size = new System.Drawing.Size(123, 17);
            this.lblTable_RelationalIDType.TabIndex = 12;
            this.lblTable_RelationalIDType.Text = "Relational ID type:";
            // 
            // lblTable_RelationalIDValue
            // 
            this.lblTable_RelationalIDValue.AutoSize = true;
            this.lblTable_RelationalIDValue.Location = new System.Drawing.Point(229, 85);
            this.lblTable_RelationalIDValue.Name = "lblTable_RelationalIDValue";
            this.lblTable_RelationalIDValue.Size = new System.Drawing.Size(130, 17);
            this.lblTable_RelationalIDValue.TabIndex = 10;
            this.lblTable_RelationalIDValue.Text = "Relational ID value:";
            // 
            // lblTable_RelationalIDColumn
            // 
            this.lblTable_RelationalIDColumn.AutoSize = true;
            this.lblTable_RelationalIDColumn.Location = new System.Drawing.Point(9, 85);
            this.lblTable_RelationalIDColumn.Name = "lblTable_RelationalIDColumn";
            this.lblTable_RelationalIDColumn.Size = new System.Drawing.Size(141, 17);
            this.lblTable_RelationalIDColumn.TabIndex = 8;
            this.lblTable_RelationalIDColumn.Text = "Relational ID column:";
            // 
            // cboTable_ValueColumn
            // 
            this.cboTable_ValueColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable_ValueColumn.FormattingEnabled = true;
            this.cboTable_ValueColumn.Location = new System.Drawing.Point(232, 58);
            this.cboTable_ValueColumn.Name = "cboTable_ValueColumn";
            this.cboTable_ValueColumn.Size = new System.Drawing.Size(214, 24);
            this.cboTable_ValueColumn.TabIndex = 3;
            // 
            // lblTable_ValueColumn
            // 
            this.lblTable_ValueColumn.AutoSize = true;
            this.lblTable_ValueColumn.Location = new System.Drawing.Point(229, 38);
            this.lblTable_ValueColumn.Name = "lblTable_ValueColumn";
            this.lblTable_ValueColumn.Size = new System.Drawing.Size(164, 17);
            this.lblTable_ValueColumn.TabIndex = 6;
            this.lblTable_ValueColumn.Text = "Value column (required):";
            // 
            // cboTable_TimestampColumn
            // 
            this.cboTable_TimestampColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable_TimestampColumn.FormattingEnabled = true;
            this.cboTable_TimestampColumn.Location = new System.Drawing.Point(12, 58);
            this.cboTable_TimestampColumn.Name = "cboTable_TimestampColumn";
            this.cboTable_TimestampColumn.Size = new System.Drawing.Size(214, 24);
            this.cboTable_TimestampColumn.TabIndex = 2;
            // 
            // lblTable_TimestampColumn
            // 
            this.lblTable_TimestampColumn.AutoSize = true;
            this.lblTable_TimestampColumn.Location = new System.Drawing.Point(9, 38);
            this.lblTable_TimestampColumn.Name = "lblTable_TimestampColumn";
            this.lblTable_TimestampColumn.Size = new System.Drawing.Size(197, 17);
            this.lblTable_TimestampColumn.TabIndex = 0;
            this.lblTable_TimestampColumn.Text = "Timestamp column (required):";
            // 
            // tabMain_Analytics
            // 
            this.tabMain_Analytics.Controls.Add(this.grpMaxDay);
            this.tabMain_Analytics.Controls.Add(this.grpMinNight);
            this.tabMain_Analytics.Location = new System.Drawing.Point(4, 25);
            this.tabMain_Analytics.Name = "tabMain_Analytics";
            this.tabMain_Analytics.Size = new System.Drawing.Size(700, 295);
            this.tabMain_Analytics.TabIndex = 7;
            this.tabMain_Analytics.Text = "Analytics Data";
            this.tabMain_Analytics.UseVisualStyleBackColor = true;
            // 
            // grpMaxDay
            // 
            this.grpMaxDay.Controls.Add(this.grpMaxDaySlot2);
            this.grpMaxDay.Controls.Add(this.grpMaxDaySlot3);
            this.grpMaxDay.Controls.Add(this.grpMaxDaySlot1);
            this.grpMaxDay.Location = new System.Drawing.Point(7, 88);
            this.grpMaxDay.Name = "grpMaxDay";
            this.grpMaxDay.Size = new System.Drawing.Size(643, 197);
            this.grpMaxDay.TabIndex = 1;
            this.grpMaxDay.TabStop = false;
            this.grpMaxDay.Text = "Maximum daily time slots";
            // 
            // grpMaxDaySlot2
            // 
            this.grpMaxDaySlot2.Controls.Add(this.dtpMaxDay_Slot2_Stop);
            this.grpMaxDaySlot2.Controls.Add(this.lblMaxDay_Slot2_Stop);
            this.grpMaxDaySlot2.Controls.Add(this.dtpMaxDay_Slot2_Start);
            this.grpMaxDaySlot2.Controls.Add(this.lblMaxDay_Slot2_Start);
            this.grpMaxDaySlot2.Location = new System.Drawing.Point(6, 108);
            this.grpMaxDaySlot2.Name = "grpMaxDaySlot2";
            this.grpMaxDaySlot2.Size = new System.Drawing.Size(312, 81);
            this.grpMaxDaySlot2.TabIndex = 9;
            this.grpMaxDaySlot2.TabStop = false;
            this.grpMaxDaySlot2.Text = "Slot 2";
            // 
            // dtpMaxDay_Slot2_Stop
            // 
            this.dtpMaxDay_Slot2_Stop.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot2_Stop.Location = new System.Drawing.Point(165, 47);
            this.dtpMaxDay_Slot2_Stop.Name = "dtpMaxDay_Slot2_Stop";
            this.dtpMaxDay_Slot2_Stop.ShowUpDown = true;
            this.dtpMaxDay_Slot2_Stop.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot2_Stop.TabIndex = 5;
            // 
            // lblMaxDay_Slot2_Stop
            // 
            this.lblMaxDay_Slot2_Stop.AutoSize = true;
            this.lblMaxDay_Slot2_Stop.Location = new System.Drawing.Point(162, 26);
            this.lblMaxDay_Slot2_Stop.Name = "lblMaxDay_Slot2_Stop";
            this.lblMaxDay_Slot2_Stop.Size = new System.Drawing.Size(138, 17);
            this.lblMaxDay_Slot2_Stop.TabIndex = 6;
            this.lblMaxDay_Slot2_Stop.Text = "Stop time (required):";
            // 
            // dtpMaxDay_Slot2_Start
            // 
            this.dtpMaxDay_Slot2_Start.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot2_Start.Location = new System.Drawing.Point(9, 47);
            this.dtpMaxDay_Slot2_Start.Name = "dtpMaxDay_Slot2_Start";
            this.dtpMaxDay_Slot2_Start.ShowUpDown = true;
            this.dtpMaxDay_Slot2_Start.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot2_Start.TabIndex = 4;
            // 
            // lblMaxDay_Slot2_Start
            // 
            this.lblMaxDay_Slot2_Start.AutoSize = true;
            this.lblMaxDay_Slot2_Start.Location = new System.Drawing.Point(6, 26);
            this.lblMaxDay_Slot2_Start.Name = "lblMaxDay_Slot2_Start";
            this.lblMaxDay_Slot2_Start.Size = new System.Drawing.Size(139, 17);
            this.lblMaxDay_Slot2_Start.TabIndex = 4;
            this.lblMaxDay_Slot2_Start.Text = "Start time (required):";
            // 
            // grpMaxDaySlot3
            // 
            this.grpMaxDaySlot3.Controls.Add(this.dtpMaxDay_Slot3_Stop);
            this.grpMaxDaySlot3.Controls.Add(this.lblMaxDay_Slot3_Stop);
            this.grpMaxDaySlot3.Controls.Add(this.dtpMaxDay_Slot3_Start);
            this.grpMaxDaySlot3.Controls.Add(this.lblMaxDay_Slot3_Start);
            this.grpMaxDaySlot3.Location = new System.Drawing.Point(324, 21);
            this.grpMaxDaySlot3.Name = "grpMaxDaySlot3";
            this.grpMaxDaySlot3.Size = new System.Drawing.Size(312, 81);
            this.grpMaxDaySlot3.TabIndex = 8;
            this.grpMaxDaySlot3.TabStop = false;
            this.grpMaxDaySlot3.Text = "Slot 3";
            // 
            // dtpMaxDay_Slot3_Stop
            // 
            this.dtpMaxDay_Slot3_Stop.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot3_Stop.Location = new System.Drawing.Point(165, 47);
            this.dtpMaxDay_Slot3_Stop.Name = "dtpMaxDay_Slot3_Stop";
            this.dtpMaxDay_Slot3_Stop.ShowUpDown = true;
            this.dtpMaxDay_Slot3_Stop.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot3_Stop.TabIndex = 7;
            // 
            // lblMaxDay_Slot3_Stop
            // 
            this.lblMaxDay_Slot3_Stop.AutoSize = true;
            this.lblMaxDay_Slot3_Stop.Location = new System.Drawing.Point(162, 26);
            this.lblMaxDay_Slot3_Stop.Name = "lblMaxDay_Slot3_Stop";
            this.lblMaxDay_Slot3_Stop.Size = new System.Drawing.Size(138, 17);
            this.lblMaxDay_Slot3_Stop.TabIndex = 6;
            this.lblMaxDay_Slot3_Stop.Text = "Stop time (required):";
            // 
            // dtpMaxDay_Slot3_Start
            // 
            this.dtpMaxDay_Slot3_Start.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot3_Start.Location = new System.Drawing.Point(9, 47);
            this.dtpMaxDay_Slot3_Start.Name = "dtpMaxDay_Slot3_Start";
            this.dtpMaxDay_Slot3_Start.ShowUpDown = true;
            this.dtpMaxDay_Slot3_Start.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot3_Start.TabIndex = 6;
            // 
            // lblMaxDay_Slot3_Start
            // 
            this.lblMaxDay_Slot3_Start.AutoSize = true;
            this.lblMaxDay_Slot3_Start.Location = new System.Drawing.Point(6, 26);
            this.lblMaxDay_Slot3_Start.Name = "lblMaxDay_Slot3_Start";
            this.lblMaxDay_Slot3_Start.Size = new System.Drawing.Size(139, 17);
            this.lblMaxDay_Slot3_Start.TabIndex = 4;
            this.lblMaxDay_Slot3_Start.Text = "Start time (required):";
            // 
            // grpMaxDaySlot1
            // 
            this.grpMaxDaySlot1.Controls.Add(this.dtpMaxDay_Slot1_Stop);
            this.grpMaxDaySlot1.Controls.Add(this.lblMaxDay_Slot1_Stop);
            this.grpMaxDaySlot1.Controls.Add(this.dtpMaxDay_Slot1_Start);
            this.grpMaxDaySlot1.Controls.Add(this.lblMaxDay_Slot1_Start);
            this.grpMaxDaySlot1.Location = new System.Drawing.Point(6, 21);
            this.grpMaxDaySlot1.Name = "grpMaxDaySlot1";
            this.grpMaxDaySlot1.Size = new System.Drawing.Size(312, 81);
            this.grpMaxDaySlot1.TabIndex = 0;
            this.grpMaxDaySlot1.TabStop = false;
            this.grpMaxDaySlot1.Text = "Slot 1";
            // 
            // dtpMaxDay_Slot1_Stop
            // 
            this.dtpMaxDay_Slot1_Stop.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot1_Stop.Location = new System.Drawing.Point(165, 47);
            this.dtpMaxDay_Slot1_Stop.Name = "dtpMaxDay_Slot1_Stop";
            this.dtpMaxDay_Slot1_Stop.ShowUpDown = true;
            this.dtpMaxDay_Slot1_Stop.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot1_Stop.TabIndex = 3;
            // 
            // lblMaxDay_Slot1_Stop
            // 
            this.lblMaxDay_Slot1_Stop.AutoSize = true;
            this.lblMaxDay_Slot1_Stop.Location = new System.Drawing.Point(162, 26);
            this.lblMaxDay_Slot1_Stop.Name = "lblMaxDay_Slot1_Stop";
            this.lblMaxDay_Slot1_Stop.Size = new System.Drawing.Size(138, 17);
            this.lblMaxDay_Slot1_Stop.TabIndex = 6;
            this.lblMaxDay_Slot1_Stop.Text = "Stop time (required):";
            // 
            // dtpMaxDay_Slot1_Start
            // 
            this.dtpMaxDay_Slot1_Start.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMaxDay_Slot1_Start.Location = new System.Drawing.Point(9, 47);
            this.dtpMaxDay_Slot1_Start.Name = "dtpMaxDay_Slot1_Start";
            this.dtpMaxDay_Slot1_Start.ShowUpDown = true;
            this.dtpMaxDay_Slot1_Start.Size = new System.Drawing.Size(136, 22);
            this.dtpMaxDay_Slot1_Start.TabIndex = 2;
            // 
            // lblMaxDay_Slot1_Start
            // 
            this.lblMaxDay_Slot1_Start.AutoSize = true;
            this.lblMaxDay_Slot1_Start.Location = new System.Drawing.Point(6, 26);
            this.lblMaxDay_Slot1_Start.Name = "lblMaxDay_Slot1_Start";
            this.lblMaxDay_Slot1_Start.Size = new System.Drawing.Size(139, 17);
            this.lblMaxDay_Slot1_Start.TabIndex = 4;
            this.lblMaxDay_Slot1_Start.Text = "Start time (required):";
            // 
            // grpMinNight
            // 
            this.grpMinNight.Controls.Add(this.dtpMinNight_Stop);
            this.grpMinNight.Controls.Add(this.lblMinNight_Stop);
            this.grpMinNight.Controls.Add(this.dtpMinNight_Start);
            this.grpMinNight.Controls.Add(this.lblMinNight_Start);
            this.grpMinNight.Location = new System.Drawing.Point(7, 5);
            this.grpMinNight.Name = "grpMinNight";
            this.grpMinNight.Size = new System.Drawing.Size(315, 77);
            this.grpMinNight.TabIndex = 0;
            this.grpMinNight.TabStop = false;
            this.grpMinNight.Text = "Minimum night time slot";
            // 
            // dtpMinNight_Stop
            // 
            this.dtpMinNight_Stop.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMinNight_Stop.Location = new System.Drawing.Point(166, 43);
            this.dtpMinNight_Stop.Name = "dtpMinNight_Stop";
            this.dtpMinNight_Stop.ShowUpDown = true;
            this.dtpMinNight_Stop.Size = new System.Drawing.Size(136, 22);
            this.dtpMinNight_Stop.TabIndex = 1;
            // 
            // lblMinNight_Stop
            // 
            this.lblMinNight_Stop.AutoSize = true;
            this.lblMinNight_Stop.Location = new System.Drawing.Point(163, 22);
            this.lblMinNight_Stop.Name = "lblMinNight_Stop";
            this.lblMinNight_Stop.Size = new System.Drawing.Size(138, 17);
            this.lblMinNight_Stop.TabIndex = 2;
            this.lblMinNight_Stop.Text = "Stop time (required):";
            // 
            // dtpMinNight_Start
            // 
            this.dtpMinNight_Start.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMinNight_Start.Location = new System.Drawing.Point(10, 43);
            this.dtpMinNight_Start.Name = "dtpMinNight_Start";
            this.dtpMinNight_Start.ShowUpDown = true;
            this.dtpMinNight_Start.Size = new System.Drawing.Size(136, 22);
            this.dtpMinNight_Start.TabIndex = 0;
            this.dtpMinNight_Start.Value = new System.DateTime(2014, 4, 26, 0, 0, 0, 0);
            // 
            // lblMinNight_Start
            // 
            this.lblMinNight_Start.AutoSize = true;
            this.lblMinNight_Start.Location = new System.Drawing.Point(7, 22);
            this.lblMinNight_Start.Name = "lblMinNight_Start";
            this.lblMinNight_Start.Size = new System.Drawing.Size(139, 17);
            this.lblMinNight_Start.TabIndex = 0;
            this.lblMinNight_Start.Text = "Start time (required):";
            // 
            // tabEnergy
            // 
            this.tabEnergy.Controls.Add(this.numEnergySpecificContent);
            this.tabEnergy.Controls.Add(this.lblEnergySpecificContent);
            this.tabEnergy.Controls.Add(this.cboEnergyCategory);
            this.tabEnergy.Controls.Add(this.lblCategory);
            this.tabEnergy.Location = new System.Drawing.Point(4, 25);
            this.tabEnergy.Name = "tabEnergy";
            this.tabEnergy.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnergy.Size = new System.Drawing.Size(700, 295);
            this.tabEnergy.TabIndex = 8;
            this.tabEnergy.Text = "Energy";
            this.tabEnergy.UseVisualStyleBackColor = true;
            // 
            // numEnergySpecificContent
            // 
            this.numEnergySpecificContent.DecimalPlaces = 2;
            this.numEnergySpecificContent.Location = new System.Drawing.Point(9, 70);
            this.numEnergySpecificContent.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.numEnergySpecificContent.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.numEnergySpecificContent.Name = "numEnergySpecificContent";
            this.numEnergySpecificContent.Size = new System.Drawing.Size(106, 22);
            this.numEnergySpecificContent.TabIndex = 1;
            // 
            // lblEnergySpecificContent
            // 
            this.lblEnergySpecificContent.AutoSize = true;
            this.lblEnergySpecificContent.Location = new System.Drawing.Point(6, 50);
            this.lblEnergySpecificContent.Name = "lblEnergySpecificContent";
            this.lblEnergySpecificContent.Size = new System.Drawing.Size(225, 17);
            this.lblEnergySpecificContent.TabIndex = 10;
            this.lblEnergySpecificContent.Text = "Energy specific content (KWh/mc):";
            // 
            // cboEnergyCategory
            // 
            this.cboEnergyCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEnergyCategory.FormattingEnabled = true;
            this.cboEnergyCategory.Items.AddRange(new object[] {
            "NONE",
            "POWER",
            "GRAVITY"});
            this.cboEnergyCategory.Location = new System.Drawing.Point(9, 23);
            this.cboEnergyCategory.Name = "cboEnergyCategory";
            this.cboEnergyCategory.Size = new System.Drawing.Size(214, 24);
            this.cboEnergyCategory.TabIndex = 0;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(6, 3);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(69, 17);
            this.lblCategory.TabIndex = 8;
            this.lblCategory.Text = "Category:";
            // 
            // tabStrumentation
            // 
            this.tabStrumentation.Controls.Add(this.cboSt_LinkType);
            this.tabStrumentation.Controls.Add(this.lblSt_LinkType);
            this.tabStrumentation.Controls.Add(this.txtSt_SerialNumber);
            this.tabStrumentation.Controls.Add(this.lblSt_SerialNumber);
            this.tabStrumentation.Controls.Add(this.txtSt_Model);
            this.tabStrumentation.Controls.Add(this.lblSt_Model);
            this.tabStrumentation.Controls.Add(this.cboSt_Type);
            this.tabStrumentation.Controls.Add(this.lblSt_Type);
            this.tabStrumentation.Location = new System.Drawing.Point(4, 25);
            this.tabStrumentation.Name = "tabStrumentation";
            this.tabStrumentation.Padding = new System.Windows.Forms.Padding(3);
            this.tabStrumentation.Size = new System.Drawing.Size(700, 295);
            this.tabStrumentation.TabIndex = 1;
            this.tabStrumentation.Text = "Strumentation";
            this.tabStrumentation.UseVisualStyleBackColor = true;
            // 
            // cboSt_LinkType
            // 
            this.cboSt_LinkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSt_LinkType.FormattingEnabled = true;
            this.cboSt_LinkType.Location = new System.Drawing.Point(251, 68);
            this.cboSt_LinkType.Name = "cboSt_LinkType";
            this.cboSt_LinkType.Size = new System.Drawing.Size(214, 24);
            this.cboSt_LinkType.TabIndex = 3;
            // 
            // lblSt_LinkType
            // 
            this.lblSt_LinkType.AutoSize = true;
            this.lblSt_LinkType.Location = new System.Drawing.Point(248, 50);
            this.lblSt_LinkType.Name = "lblSt_LinkType";
            this.lblSt_LinkType.Size = new System.Drawing.Size(69, 17);
            this.lblSt_LinkType.TabIndex = 6;
            this.lblSt_LinkType.Text = "Link type:";
            // 
            // txtSt_SerialNumber
            // 
            this.txtSt_SerialNumber.Location = new System.Drawing.Point(251, 25);
            this.txtSt_SerialNumber.Name = "txtSt_SerialNumber";
            this.txtSt_SerialNumber.Size = new System.Drawing.Size(428, 22);
            this.txtSt_SerialNumber.TabIndex = 2;
            // 
            // lblSt_SerialNumber
            // 
            this.lblSt_SerialNumber.AutoSize = true;
            this.lblSt_SerialNumber.Location = new System.Drawing.Point(248, 3);
            this.lblSt_SerialNumber.Name = "lblSt_SerialNumber";
            this.lblSt_SerialNumber.Size = new System.Drawing.Size(102, 17);
            this.lblSt_SerialNumber.TabIndex = 4;
            this.lblSt_SerialNumber.Text = "Serial Number:";
            // 
            // txtSt_Model
            // 
            this.txtSt_Model.Location = new System.Drawing.Point(9, 70);
            this.txtSt_Model.Name = "txtSt_Model";
            this.txtSt_Model.Size = new System.Drawing.Size(214, 22);
            this.txtSt_Model.TabIndex = 1;
            // 
            // lblSt_Model
            // 
            this.lblSt_Model.AutoSize = true;
            this.lblSt_Model.Location = new System.Drawing.Point(6, 50);
            this.lblSt_Model.Name = "lblSt_Model";
            this.lblSt_Model.Size = new System.Drawing.Size(50, 17);
            this.lblSt_Model.TabIndex = 2;
            this.lblSt_Model.Text = "Model:";
            // 
            // cboSt_Type
            // 
            this.cboSt_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSt_Type.FormattingEnabled = true;
            this.cboSt_Type.Location = new System.Drawing.Point(9, 23);
            this.cboSt_Type.Name = "cboSt_Type";
            this.cboSt_Type.Size = new System.Drawing.Size(214, 24);
            this.cboSt_Type.TabIndex = 0;
            // 
            // lblSt_Type
            // 
            this.lblSt_Type.AutoSize = true;
            this.lblSt_Type.Location = new System.Drawing.Point(6, 3);
            this.lblSt_Type.Name = "lblSt_Type";
            this.lblSt_Type.Size = new System.Drawing.Size(44, 17);
            this.lblSt_Type.TabIndex = 0;
            this.lblSt_Type.Text = "Type:";
            // 
            // tabMain_Geolocation
            // 
            this.tabMain_Geolocation.Controls.Add(this.num_zpos);
            this.tabMain_Geolocation.Controls.Add(this.lbl_zpos);
            this.tabMain_Geolocation.Controls.Add(this.num_ypos);
            this.tabMain_Geolocation.Controls.Add(this.lbl_ypos);
            this.tabMain_Geolocation.Controls.Add(this.num_xpos);
            this.tabMain_Geolocation.Controls.Add(this.lbl_xpos);
            this.tabMain_Geolocation.Location = new System.Drawing.Point(4, 25);
            this.tabMain_Geolocation.Name = "tabMain_Geolocation";
            this.tabMain_Geolocation.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain_Geolocation.Size = new System.Drawing.Size(700, 295);
            this.tabMain_Geolocation.TabIndex = 3;
            this.tabMain_Geolocation.Text = "Geolocation";
            this.tabMain_Geolocation.UseVisualStyleBackColor = true;
            // 
            // num_zpos
            // 
            this.num_zpos.DecimalPlaces = 2;
            this.num_zpos.Location = new System.Drawing.Point(9, 113);
            this.num_zpos.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.num_zpos.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.num_zpos.Name = "num_zpos";
            this.num_zpos.Size = new System.Drawing.Size(106, 22);
            this.num_zpos.TabIndex = 2;
            // 
            // lbl_zpos
            // 
            this.lbl_zpos.AutoSize = true;
            this.lbl_zpos.Location = new System.Drawing.Point(6, 93);
            this.lbl_zpos.Name = "lbl_zpos";
            this.lbl_zpos.Size = new System.Drawing.Size(74, 17);
            this.lbl_zpos.TabIndex = 15;
            this.lbl_zpos.Text = "Z position:";
            // 
            // num_ypos
            // 
            this.num_ypos.DecimalPlaces = 2;
            this.num_ypos.Location = new System.Drawing.Point(9, 68);
            this.num_ypos.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.num_ypos.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.num_ypos.Name = "num_ypos";
            this.num_ypos.Size = new System.Drawing.Size(106, 22);
            this.num_ypos.TabIndex = 1;
            // 
            // lbl_ypos
            // 
            this.lbl_ypos.AutoSize = true;
            this.lbl_ypos.Location = new System.Drawing.Point(6, 48);
            this.lbl_ypos.Name = "lbl_ypos";
            this.lbl_ypos.Size = new System.Drawing.Size(74, 17);
            this.lbl_ypos.TabIndex = 13;
            this.lbl_ypos.Text = "Y position:";
            // 
            // num_xpos
            // 
            this.num_xpos.DecimalPlaces = 2;
            this.num_xpos.Location = new System.Drawing.Point(9, 23);
            this.num_xpos.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.num_xpos.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.num_xpos.Name = "num_xpos";
            this.num_xpos.Size = new System.Drawing.Size(106, 22);
            this.num_xpos.TabIndex = 0;
            // 
            // lbl_xpos
            // 
            this.lbl_xpos.AutoSize = true;
            this.lbl_xpos.Location = new System.Drawing.Point(6, 3);
            this.lbl_xpos.Name = "lbl_xpos";
            this.lbl_xpos.Size = new System.Drawing.Size(74, 17);
            this.lbl_xpos.TabIndex = 0;
            this.lbl_xpos.Text = "X position:";
            // 
            // tabMain_Alarms
            // 
            this.tabMain_Alarms.Controls.Add(this.grpAlarms_ConstantCheck);
            this.tabMain_Alarms.Controls.Add(this.grpAlarms_Thresholds);
            this.tabMain_Alarms.Location = new System.Drawing.Point(4, 25);
            this.tabMain_Alarms.Name = "tabMain_Alarms";
            this.tabMain_Alarms.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain_Alarms.Size = new System.Drawing.Size(700, 295);
            this.tabMain_Alarms.TabIndex = 4;
            this.tabMain_Alarms.Text = "Alarms";
            this.tabMain_Alarms.UseVisualStyleBackColor = true;
            // 
            // grpAlarms_ConstantCheck
            // 
            this.grpAlarms_ConstantCheck.Controls.Add(this.numConstant_Time);
            this.grpAlarms_ConstantCheck.Controls.Add(this.lblConstant_Time);
            this.grpAlarms_ConstantCheck.Controls.Add(this.numConstant_Hysteresis);
            this.grpAlarms_ConstantCheck.Controls.Add(this.lblConstant_Hysteresis);
            this.grpAlarms_ConstantCheck.Controls.Add(this.chkConstant_Enable);
            this.grpAlarms_ConstantCheck.Location = new System.Drawing.Point(6, 116);
            this.grpAlarms_ConstantCheck.Name = "grpAlarms_ConstantCheck";
            this.grpAlarms_ConstantCheck.Size = new System.Drawing.Size(363, 104);
            this.grpAlarms_ConstantCheck.TabIndex = 1;
            this.grpAlarms_ConstantCheck.TabStop = false;
            this.grpAlarms_ConstantCheck.Text = "Constant Check";
            // 
            // numConstant_Time
            // 
            this.numConstant_Time.Location = new System.Drawing.Point(118, 69);
            this.numConstant_Time.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numConstant_Time.Name = "numConstant_Time";
            this.numConstant_Time.Size = new System.Drawing.Size(106, 22);
            this.numConstant_Time.TabIndex = 6;
            // 
            // lblConstant_Time
            // 
            this.lblConstant_Time.AutoSize = true;
            this.lblConstant_Time.Location = new System.Drawing.Point(115, 49);
            this.lblConstant_Time.Name = "lblConstant_Time";
            this.lblConstant_Time.Size = new System.Drawing.Size(131, 17);
            this.lblConstant_Time.TabIndex = 22;
            this.lblConstant_Time.Text = "Time value (hours):";
            // 
            // numConstant_Hysteresis
            // 
            this.numConstant_Hysteresis.DecimalPlaces = 2;
            this.numConstant_Hysteresis.Location = new System.Drawing.Point(6, 69);
            this.numConstant_Hysteresis.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numConstant_Hysteresis.Name = "numConstant_Hysteresis";
            this.numConstant_Hysteresis.Size = new System.Drawing.Size(106, 22);
            this.numConstant_Hysteresis.TabIndex = 5;
            // 
            // lblConstant_Hysteresis
            // 
            this.lblConstant_Hysteresis.AutoSize = true;
            this.lblConstant_Hysteresis.Location = new System.Drawing.Point(3, 49);
            this.lblConstant_Hysteresis.Name = "lblConstant_Hysteresis";
            this.lblConstant_Hysteresis.Size = new System.Drawing.Size(109, 17);
            this.lblConstant_Hysteresis.TabIndex = 20;
            this.lblConstant_Hysteresis.Text = "Hysteresis (+/-):";
            // 
            // chkConstant_Enable
            // 
            this.chkConstant_Enable.AutoSize = true;
            this.chkConstant_Enable.Location = new System.Drawing.Point(6, 22);
            this.chkConstant_Enable.Name = "chkConstant_Enable";
            this.chkConstant_Enable.Size = new System.Drawing.Size(74, 21);
            this.chkConstant_Enable.TabIndex = 3;
            this.chkConstant_Enable.Text = "Enable";
            this.chkConstant_Enable.UseVisualStyleBackColor = true;
            // 
            // grpAlarms_Thresholds
            // 
            this.grpAlarms_Thresholds.Controls.Add(this.numThresholds_Max);
            this.grpAlarms_Thresholds.Controls.Add(this.lblThresholds_Max);
            this.grpAlarms_Thresholds.Controls.Add(this.numThresholds_Min);
            this.grpAlarms_Thresholds.Controls.Add(this.lblThresholds_Min);
            this.grpAlarms_Thresholds.Controls.Add(this.chkThresholds_Enable);
            this.grpAlarms_Thresholds.Location = new System.Drawing.Point(6, 6);
            this.grpAlarms_Thresholds.Name = "grpAlarms_Thresholds";
            this.grpAlarms_Thresholds.Size = new System.Drawing.Size(237, 104);
            this.grpAlarms_Thresholds.TabIndex = 0;
            this.grpAlarms_Thresholds.TabStop = false;
            this.grpAlarms_Thresholds.Text = "Thresholds";
            // 
            // numThresholds_Max
            // 
            this.numThresholds_Max.DecimalPlaces = 2;
            this.numThresholds_Max.Location = new System.Drawing.Point(118, 70);
            this.numThresholds_Max.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.numThresholds_Max.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.numThresholds_Max.Name = "numThresholds_Max";
            this.numThresholds_Max.Size = new System.Drawing.Size(106, 22);
            this.numThresholds_Max.TabIndex = 2;
            // 
            // lblThresholds_Max
            // 
            this.lblThresholds_Max.AutoSize = true;
            this.lblThresholds_Max.Location = new System.Drawing.Point(115, 50);
            this.lblThresholds_Max.Name = "lblThresholds_Max";
            this.lblThresholds_Max.Size = new System.Drawing.Size(79, 17);
            this.lblThresholds_Max.TabIndex = 16;
            this.lblThresholds_Max.Text = "Max. value:";
            // 
            // numThresholds_Min
            // 
            this.numThresholds_Min.DecimalPlaces = 2;
            this.numThresholds_Min.Location = new System.Drawing.Point(6, 70);
            this.numThresholds_Min.Maximum = new decimal(new int[] {
            32765,
            0,
            0,
            0});
            this.numThresholds_Min.Minimum = new decimal(new int[] {
            32765,
            0,
            0,
            -2147483648});
            this.numThresholds_Min.Name = "numThresholds_Min";
            this.numThresholds_Min.Size = new System.Drawing.Size(106, 22);
            this.numThresholds_Min.TabIndex = 1;
            // 
            // lblThresholds_Min
            // 
            this.lblThresholds_Min.AutoSize = true;
            this.lblThresholds_Min.Location = new System.Drawing.Point(3, 50);
            this.lblThresholds_Min.Name = "lblThresholds_Min";
            this.lblThresholds_Min.Size = new System.Drawing.Size(76, 17);
            this.lblThresholds_Min.TabIndex = 1;
            this.lblThresholds_Min.Text = "Min. value:";
            // 
            // chkThresholds_Enable
            // 
            this.chkThresholds_Enable.AutoSize = true;
            this.chkThresholds_Enable.Location = new System.Drawing.Point(7, 22);
            this.chkThresholds_Enable.Name = "chkThresholds_Enable";
            this.chkThresholds_Enable.Size = new System.Drawing.Size(74, 21);
            this.chkThresholds_Enable.TabIndex = 0;
            this.chkThresholds_Enable.Text = "Enable";
            this.chkThresholds_Enable.UseVisualStyleBackColor = true;
            // 
            // tabMain_EPANET
            // 
            this.tabMain_EPANET.Controls.Add(this.cmdEpa_Browse);
            this.tabMain_EPANET.Controls.Add(this.txtEpa_PatternFile);
            this.tabMain_EPANET.Controls.Add(this.lblEpa_PatternFile);
            this.tabMain_EPANET.Controls.Add(this.txtEpa_Pattern);
            this.tabMain_EPANET.Controls.Add(this.lblEpa_Pattern);
            this.tabMain_EPANET.Controls.Add(this.txtEpa_ObjectID);
            this.tabMain_EPANET.Controls.Add(this.lblEpa_ObjectID);
            this.tabMain_EPANET.Location = new System.Drawing.Point(4, 25);
            this.tabMain_EPANET.Name = "tabMain_EPANET";
            this.tabMain_EPANET.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain_EPANET.Size = new System.Drawing.Size(700, 295);
            this.tabMain_EPANET.TabIndex = 2;
            this.tabMain_EPANET.Text = "EPANET";
            this.tabMain_EPANET.UseVisualStyleBackColor = true;
            // 
            // cmdEpa_Browse
            // 
            this.cmdEpa_Browse.Location = new System.Drawing.Point(229, 226);
            this.cmdEpa_Browse.Name = "cmdEpa_Browse";
            this.cmdEpa_Browse.Size = new System.Drawing.Size(75, 23);
            this.cmdEpa_Browse.TabIndex = 3;
            this.cmdEpa_Browse.Text = "Browse...";
            this.cmdEpa_Browse.UseVisualStyleBackColor = true;
            this.cmdEpa_Browse.Click += new System.EventHandler(this.cmdEpa_Browse_Click);
            // 
            // txtEpa_PatternFile
            // 
            this.txtEpa_PatternFile.Location = new System.Drawing.Point(229, 23);
            this.txtEpa_PatternFile.Multiline = true;
            this.txtEpa_PatternFile.Name = "txtEpa_PatternFile";
            this.txtEpa_PatternFile.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEpa_PatternFile.Size = new System.Drawing.Size(465, 197);
            this.txtEpa_PatternFile.TabIndex = 2;
            // 
            // lblEpa_PatternFile
            // 
            this.lblEpa_PatternFile.AutoSize = true;
            this.lblEpa_PatternFile.Location = new System.Drawing.Point(226, 3);
            this.lblEpa_PatternFile.Name = "lblEpa_PatternFile";
            this.lblEpa_PatternFile.Size = new System.Drawing.Size(80, 17);
            this.lblEpa_PatternFile.TabIndex = 4;
            this.lblEpa_PatternFile.Text = "Pattern file:";
            // 
            // txtEpa_Pattern
            // 
            this.txtEpa_Pattern.Location = new System.Drawing.Point(9, 68);
            this.txtEpa_Pattern.Name = "txtEpa_Pattern";
            this.txtEpa_Pattern.Size = new System.Drawing.Size(214, 22);
            this.txtEpa_Pattern.TabIndex = 1;
            // 
            // lblEpa_Pattern
            // 
            this.lblEpa_Pattern.AutoSize = true;
            this.lblEpa_Pattern.Location = new System.Drawing.Point(6, 48);
            this.lblEpa_Pattern.Name = "lblEpa_Pattern";
            this.lblEpa_Pattern.Size = new System.Drawing.Size(58, 17);
            this.lblEpa_Pattern.TabIndex = 2;
            this.lblEpa_Pattern.Text = "Pattern:";
            // 
            // txtEpa_ObjectID
            // 
            this.txtEpa_ObjectID.Location = new System.Drawing.Point(9, 23);
            this.txtEpa_ObjectID.Name = "txtEpa_ObjectID";
            this.txtEpa_ObjectID.Size = new System.Drawing.Size(214, 22);
            this.txtEpa_ObjectID.TabIndex = 0;
            // 
            // lblEpa_ObjectID
            // 
            this.lblEpa_ObjectID.AutoSize = true;
            this.lblEpa_ObjectID.Location = new System.Drawing.Point(6, 3);
            this.lblEpa_ObjectID.Name = "lblEpa_ObjectID";
            this.lblEpa_ObjectID.Size = new System.Drawing.Size(70, 17);
            this.lblEpa_ObjectID.TabIndex = 0;
            this.lblEpa_ObjectID.Text = "Object ID:";
            // 
            // tabMain_SAP
            // 
            this.tabMain_SAP.Controls.Add(this.txtSAP_Code);
            this.tabMain_SAP.Controls.Add(this.lblSAP_Code);
            this.tabMain_SAP.Location = new System.Drawing.Point(4, 25);
            this.tabMain_SAP.Name = "tabMain_SAP";
            this.tabMain_SAP.Size = new System.Drawing.Size(700, 295);
            this.tabMain_SAP.TabIndex = 6;
            this.tabMain_SAP.Text = "SAP";
            this.tabMain_SAP.UseVisualStyleBackColor = true;
            // 
            // txtSAP_Code
            // 
            this.txtSAP_Code.Location = new System.Drawing.Point(6, 24);
            this.txtSAP_Code.Name = "txtSAP_Code";
            this.txtSAP_Code.Size = new System.Drawing.Size(434, 22);
            this.txtSAP_Code.TabIndex = 0;
            // 
            // lblSAP_Code
            // 
            this.lblSAP_Code.AutoSize = true;
            this.lblSAP_Code.Location = new System.Drawing.Point(3, 4);
            this.lblSAP_Code.Name = "lblSAP_Code";
            this.lblSAP_Code.Size = new System.Drawing.Size(74, 17);
            this.lblSAP_Code.TabIndex = 0;
            this.lblSAP_Code.Text = "SAP code:";
            // 
            // dsMeasures
            // 
            this.dsMeasures.DataSetName = "NewDataSet";
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "*.pat";
            this.dlgOpen.Filter = "EPANET pattern file (*.pat)|*.pat|All files (*.*)|*.*";
            this.dlgOpen.Title = "Load EPANET pattern file";
            // 
            // dsConnections
            // 
            this.dsConnections.DataSetName = "NewDataSet";
            // 
            // bsMeasures
            // 
            this.bsMeasures.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.bsMeasures_AddingNew);
            // 
            // frmModelling_Measures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 507);
            this.Controls.Add(this.tlpMain);
            this.MinimizeBox = false;
            this.Name = "frmModelling_Measures";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Measures";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmModelling_Measures_FormClosing);
            this.Shown += new System.EventHandler(this.frmModelling_Measures_Shown);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnMain)).EndInit();
            this.bnMain.ResumeLayout(false);
            this.bnMain.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabMain_General.ResumeLayout(false);
            this.tabMain_General.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFixedValue)).EndInit();
            this.tabMain_Database.ResumeLayout(false);
            this.tabMain_Database.PerformLayout();
            this.grpTable.ResumeLayout(false);
            this.grpTable.PerformLayout();
            this.tabMain_Analytics.ResumeLayout(false);
            this.grpMaxDay.ResumeLayout(false);
            this.grpMaxDaySlot2.ResumeLayout(false);
            this.grpMaxDaySlot2.PerformLayout();
            this.grpMaxDaySlot3.ResumeLayout(false);
            this.grpMaxDaySlot3.PerformLayout();
            this.grpMaxDaySlot1.ResumeLayout(false);
            this.grpMaxDaySlot1.PerformLayout();
            this.grpMinNight.ResumeLayout(false);
            this.grpMinNight.PerformLayout();
            this.tabEnergy.ResumeLayout(false);
            this.tabEnergy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEnergySpecificContent)).EndInit();
            this.tabStrumentation.ResumeLayout(false);
            this.tabStrumentation.PerformLayout();
            this.tabMain_Geolocation.ResumeLayout(false);
            this.tabMain_Geolocation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_zpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ypos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_xpos)).EndInit();
            this.tabMain_Alarms.ResumeLayout(false);
            this.grpAlarms_ConstantCheck.ResumeLayout(false);
            this.grpAlarms_ConstantCheck.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConstant_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConstant_Hysteresis)).EndInit();
            this.grpAlarms_Thresholds.ResumeLayout(false);
            this.grpAlarms_Thresholds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholds_Max)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholds_Min)).EndInit();
            this.tabMain_EPANET.ResumeLayout(false);
            this.tabMain_EPANET.PerformLayout();
            this.tabMain_SAP.ResumeLayout(false);
            this.tabMain_SAP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsMeasures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsMeasures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.DataGridView dgDown;
        private System.Windows.Forms.BindingNavigator bnMain;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Data.DataSet dsMeasures;
        private System.Windows.Forms.BindingSource bsMeasures;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabMain_General;
        private System.Windows.Forms.TabPage tabStrumentation;
        private System.Windows.Forms.TabPage tabMain_Geolocation;
        private System.Windows.Forms.TabPage tabMain_EPANET;
        private System.Windows.Forms.TabPage tabMain_Alarms;
        private System.Windows.Forms.TabPage tabMain_Database;
        private System.Windows.Forms.TabPage tabMain_SAP;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.CheckBox chkCritical;
        private System.Windows.Forms.CheckBox chkReliable;
        private System.Windows.Forms.NumericUpDown numFixedValue;
        private System.Windows.Forms.Label lblFixedValue;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.ComboBox cboConnection;
        private System.Windows.Forms.Label lblTable;
        private System.Windows.Forms.ComboBox cboTable;
        private System.Windows.Forms.GroupBox grpTable;
        private System.Windows.Forms.ComboBox cboTable_ValueColumn;
        private System.Windows.Forms.Label lblTable_ValueColumn;
        private System.Windows.Forms.ComboBox cboTable_TimestampColumn;
        private System.Windows.Forms.Label lblTable_TimestampColumn;
        private System.Windows.Forms.Label lblTable_RelationalIDType;
        private System.Windows.Forms.Label lblTable_RelationalIDValue;
        private System.Windows.Forms.Label lblTable_RelationalIDColumn;
        private System.Windows.Forms.ComboBox cboSt_Type;
        private System.Windows.Forms.Label lblSt_Type;
        private System.Windows.Forms.TextBox txtSt_Model;
        private System.Windows.Forms.Label lblSt_Model;
        private System.Windows.Forms.ComboBox cboSt_LinkType;
        private System.Windows.Forms.Label lblSt_LinkType;
        private System.Windows.Forms.TextBox txtSt_SerialNumber;
        private System.Windows.Forms.Label lblSt_SerialNumber;
        private System.Windows.Forms.NumericUpDown num_zpos;
        private System.Windows.Forms.Label lbl_zpos;
        private System.Windows.Forms.NumericUpDown num_ypos;
        private System.Windows.Forms.Label lbl_ypos;
        private System.Windows.Forms.NumericUpDown num_xpos;
        private System.Windows.Forms.Label lbl_xpos;
        private System.Windows.Forms.GroupBox grpAlarms_Thresholds;
        private System.Windows.Forms.NumericUpDown numThresholds_Max;
        private System.Windows.Forms.Label lblThresholds_Max;
        private System.Windows.Forms.NumericUpDown numThresholds_Min;
        private System.Windows.Forms.Label lblThresholds_Min;
        private System.Windows.Forms.CheckBox chkThresholds_Enable;
        private System.Windows.Forms.GroupBox grpAlarms_ConstantCheck;
        private System.Windows.Forms.CheckBox chkConstant_Enable;
        private System.Windows.Forms.NumericUpDown numConstant_Hysteresis;
        private System.Windows.Forms.Label lblConstant_Hysteresis;
        private System.Windows.Forms.NumericUpDown numConstant_Time;
        private System.Windows.Forms.Label lblConstant_Time;
        private System.Windows.Forms.TextBox txtEpa_Pattern;
        private System.Windows.Forms.Label lblEpa_Pattern;
        private System.Windows.Forms.TextBox txtEpa_ObjectID;
        private System.Windows.Forms.Label lblEpa_ObjectID;
        private System.Windows.Forms.TextBox txtEpa_PatternFile;
        private System.Windows.Forms.Label lblEpa_PatternFile;
        private System.Windows.Forms.Button cmdEpa_Browse;
        private System.Windows.Forms.TextBox txtSAP_Code;
        private System.Windows.Forms.Label lblSAP_Code;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.ToolStripButton cmdCommit;
        private System.Windows.Forms.ToolStripButton cmdExit;
        private System.Data.DataSet dsConnections;
        private System.Windows.Forms.BindingSource bsConnections;
        private System.Windows.Forms.ComboBox cboTable_RelationalIDColumn;
        private System.Windows.Forms.ComboBox cboTable_RelationalIDType;
        private System.Windows.Forms.ComboBox cboTable_RelationalIDValue;
        private System.Windows.Forms.TabPage tabMain_Analytics;
        private System.Windows.Forms.GroupBox grpMinNight;
        private System.Windows.Forms.DateTimePicker dtpMinNight_Start;
        private System.Windows.Forms.Label lblMinNight_Start;
        private System.Windows.Forms.DateTimePicker dtpMinNight_Stop;
        private System.Windows.Forms.Label lblMinNight_Stop;
        private System.Windows.Forms.GroupBox grpMaxDay;
        private System.Windows.Forms.GroupBox grpMaxDaySlot2;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot2_Stop;
        private System.Windows.Forms.Label lblMaxDay_Slot2_Stop;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot2_Start;
        private System.Windows.Forms.Label lblMaxDay_Slot2_Start;
        private System.Windows.Forms.GroupBox grpMaxDaySlot3;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot3_Stop;
        private System.Windows.Forms.Label lblMaxDay_Slot3_Stop;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot3_Start;
        private System.Windows.Forms.Label lblMaxDay_Slot3_Start;
        private System.Windows.Forms.GroupBox grpMaxDaySlot1;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot1_Stop;
        private System.Windows.Forms.Label lblMaxDay_Slot1_Stop;
        private System.Windows.Forms.DateTimePicker dtpMaxDay_Slot1_Start;
        private System.Windows.Forms.Label lblMaxDay_Slot1_Start;
        private System.Windows.Forms.Label lblUpdateTimestamp;
        private System.Windows.Forms.TabPage tabEnergy;
        private System.Windows.Forms.ComboBox cboEnergyCategory;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.NumericUpDown numEnergySpecificContent;
        private System.Windows.Forms.Label lblEnergySpecificContent;
        private System.Windows.Forms.DateTimePicker dtpUpdateTimestamp;
    }
}