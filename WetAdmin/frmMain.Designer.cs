namespace WetAdmin
{
    partial class frmMain
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuMain_File = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_File_Esci = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_File_Modelling = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Modelling_Measures = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Modelling_Districts = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Service = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Service_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Service_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Service_Restart = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_File_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain_Settings_ODBCconnections = new System.Windows.Forms.ToolStripMenuItem();
            this.svcWetSvc = new System.ServiceProcess.ServiceController();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsMain_Service_Start = new System.Windows.Forms.ToolStripButton();
            this.tsMain_Service_Stop = new System.Windows.Forms.ToolStripButton();
            this.tsMain_Service_Restart = new System.Windows.Forms.ToolStripButton();
            this.tsMain_Sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsMain_File_Exit = new System.Windows.Forms.ToolStripButton();
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.tmrCheckService = new System.Windows.Forms.Timer(this.components);
            this.stbMain = new System.Windows.Forms.StatusStrip();
            this.mnuMain.SuspendLayout();
            this.tsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain_File,
            this.mnuMain_File_Modelling,
            this.mnuMain_Service,
            this.mnuMain_File_Settings});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(682, 28);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuMain_File
            // 
            this.mnuMain_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain_File_Esci});
            this.mnuMain_File.Name = "mnuMain_File";
            this.mnuMain_File.Size = new System.Drawing.Size(44, 24);
            this.mnuMain_File.Text = "&File";
            // 
            // mnuMain_File_Esci
            // 
            this.mnuMain_File_Esci.Image = global::WetAdmin.Resources_PNG_16x16.application_exit_2;
            this.mnuMain_File_Esci.Name = "mnuMain_File_Esci";
            this.mnuMain_File_Esci.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuMain_File_Esci.Size = new System.Drawing.Size(162, 24);
            this.mnuMain_File_Esci.Text = "&Esci";
            this.mnuMain_File_Esci.Click += new System.EventHandler(this.mnuMain_File_Esci_Click);
            // 
            // mnuMain_File_Modelling
            // 
            this.mnuMain_File_Modelling.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain_Modelling_Measures,
            this.mnuMain_Modelling_Districts});
            this.mnuMain_File_Modelling.Name = "mnuMain_File_Modelling";
            this.mnuMain_File_Modelling.Size = new System.Drawing.Size(89, 24);
            this.mnuMain_File_Modelling.Text = "&Modelling";
            // 
            // mnuMain_Modelling_Measures
            // 
            this.mnuMain_Modelling_Measures.Image = global::WetAdmin.Resources_PNG_16x16.utilities_system_monitor;
            this.mnuMain_Modelling_Measures.Name = "mnuMain_Modelling_Measures";
            this.mnuMain_Modelling_Measures.Size = new System.Drawing.Size(149, 24);
            this.mnuMain_Modelling_Measures.Text = "&Measures...";
            this.mnuMain_Modelling_Measures.Click += new System.EventHandler(this.mnuMain_Modelling_Measures_Click);
            // 
            // mnuMain_Modelling_Districts
            // 
            this.mnuMain_Modelling_Districts.Image = global::WetAdmin.Resources_PNG_16x16.bkchem;
            this.mnuMain_Modelling_Districts.Name = "mnuMain_Modelling_Districts";
            this.mnuMain_Modelling_Districts.Size = new System.Drawing.Size(149, 24);
            this.mnuMain_Modelling_Districts.Text = "&Districts...";
            this.mnuMain_Modelling_Districts.Click += new System.EventHandler(this.mnuMain_Modelling_Districts_Click);
            // 
            // mnuMain_Service
            // 
            this.mnuMain_Service.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain_Service_Start,
            this.mnuMain_Service_Stop,
            this.mnuMain_Service_Restart});
            this.mnuMain_Service.Name = "mnuMain_Service";
            this.mnuMain_Service.Size = new System.Drawing.Size(68, 24);
            this.mnuMain_Service.Text = "Ser&vice";
            // 
            // mnuMain_Service_Start
            // 
            this.mnuMain_Service_Start.Image = global::WetAdmin.Resources_PNG_16x16.arrow_right_3;
            this.mnuMain_Service_Start.Name = "mnuMain_Service_Start";
            this.mnuMain_Service_Start.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.mnuMain_Service_Start.Size = new System.Drawing.Size(191, 24);
            this.mnuMain_Service_Start.Text = "S&tart";
            this.mnuMain_Service_Start.Click += new System.EventHandler(this.mnuMain_Service_Start_Click);
            // 
            // mnuMain_Service_Stop
            // 
            this.mnuMain_Service_Stop.Image = global::WetAdmin.Resources_PNG_16x16.media_playback_stop_7;
            this.mnuMain_Service_Stop.Name = "mnuMain_Service_Stop";
            this.mnuMain_Service_Stop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.mnuMain_Service_Stop.Size = new System.Drawing.Size(191, 24);
            this.mnuMain_Service_Stop.Text = "Sto&p";
            this.mnuMain_Service_Stop.Click += new System.EventHandler(this.mnuMain_Service_Stop_Click);
            // 
            // mnuMain_Service_Restart
            // 
            this.mnuMain_Service_Restart.Image = global::WetAdmin.Resources_PNG_16x16.view_refresh_7;
            this.mnuMain_Service_Restart.Name = "mnuMain_Service_Restart";
            this.mnuMain_Service_Restart.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
            this.mnuMain_Service_Restart.Size = new System.Drawing.Size(191, 24);
            this.mnuMain_Service_Restart.Text = "&Restart";
            this.mnuMain_Service_Restart.Click += new System.EventHandler(this.mnuMain_Service_Restart_Click);
            // 
            // mnuMain_File_Settings
            // 
            this.mnuMain_File_Settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMain_Settings_ODBCconnections});
            this.mnuMain_File_Settings.Name = "mnuMain_File_Settings";
            this.mnuMain_File_Settings.Size = new System.Drawing.Size(74, 24);
            this.mnuMain_File_Settings.Text = "&Settings";
            // 
            // mnuMain_Settings_ODBCconnections
            // 
            this.mnuMain_Settings_ODBCconnections.Image = global::WetAdmin.Resources_PNG_16x16.db;
            this.mnuMain_Settings_ODBCconnections.Name = "mnuMain_Settings_ODBCconnections";
            this.mnuMain_Settings_ODBCconnections.Size = new System.Drawing.Size(210, 24);
            this.mnuMain_Settings_ODBCconnections.Text = "&ODBC connections...";
            this.mnuMain_Settings_ODBCconnections.Click += new System.EventHandler(this.mnuMain_Settings_ODBCconnections_Click);
            // 
            // tsMain
            // 
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMain_Service_Start,
            this.tsMain_Service_Stop,
            this.tsMain_Service_Restart,
            this.tsMain_Sep1,
            this.tsMain_File_Exit});
            this.tsMain.Location = new System.Drawing.Point(0, 28);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(682, 25);
            this.tsMain.TabIndex = 3;
            this.tsMain.Text = "toolStrip1";
            // 
            // tsMain_Service_Start
            // 
            this.tsMain_Service_Start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsMain_Service_Start.Image = global::WetAdmin.Resources_PNG_16x16.arrow_right_3;
            this.tsMain_Service_Start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsMain_Service_Start.Name = "tsMain_Service_Start";
            this.tsMain_Service_Start.Size = new System.Drawing.Size(23, 22);
            this.tsMain_Service_Start.Text = "toolStripButton1";
            this.tsMain_Service_Start.ToolTipText = "Start service";
            this.tsMain_Service_Start.Click += new System.EventHandler(this.tsMain_Service_Start_Click);
            // 
            // tsMain_Service_Stop
            // 
            this.tsMain_Service_Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsMain_Service_Stop.Image = global::WetAdmin.Resources_PNG_16x16.media_playback_stop_7;
            this.tsMain_Service_Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsMain_Service_Stop.Name = "tsMain_Service_Stop";
            this.tsMain_Service_Stop.Size = new System.Drawing.Size(23, 22);
            this.tsMain_Service_Stop.Text = "toolStripButton2";
            this.tsMain_Service_Stop.ToolTipText = "Stop service";
            this.tsMain_Service_Stop.Click += new System.EventHandler(this.tsMain_Service_Stop_Click);
            // 
            // tsMain_Service_Restart
            // 
            this.tsMain_Service_Restart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsMain_Service_Restart.Image = global::WetAdmin.Resources_PNG_16x16.view_refresh_7;
            this.tsMain_Service_Restart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsMain_Service_Restart.Name = "tsMain_Service_Restart";
            this.tsMain_Service_Restart.Size = new System.Drawing.Size(23, 22);
            this.tsMain_Service_Restart.Text = "toolStripButton3";
            this.tsMain_Service_Restart.ToolTipText = "Restart service";
            this.tsMain_Service_Restart.Click += new System.EventHandler(this.tsMain_Service_Restart_Click);
            // 
            // tsMain_Sep1
            // 
            this.tsMain_Sep1.Name = "tsMain_Sep1";
            this.tsMain_Sep1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsMain_File_Exit
            // 
            this.tsMain_File_Exit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsMain_File_Exit.Image = global::WetAdmin.Resources_PNG_16x16.application_exit_2;
            this.tsMain_File_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsMain_File_Exit.Name = "tsMain_File_Exit";
            this.tsMain_File_Exit.Size = new System.Drawing.Size(23, 22);
            this.tsMain_File_Exit.Text = "toolStripButton1";
            this.tsMain_File_Exit.ToolTipText = "Exit application";
            this.tsMain_File_Exit.Click += new System.EventHandler(this.tsMain_File_Exit_Click);
            // 
            // pbMain
            // 
            this.pbMain.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbMain.Image = global::WetAdmin.Properties.Resources.wetnet;
            this.pbMain.Location = new System.Drawing.Point(0, 28);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(682, 444);
            this.pbMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMain.TabIndex = 2;
            this.pbMain.TabStop = false;
            // 
            // tmrCheckService
            // 
            this.tmrCheckService.Enabled = true;
            this.tmrCheckService.Interval = 1000;
            this.tmrCheckService.Tick += new System.EventHandler(this.tmrCheckService_Tick);
            // 
            // stbMain
            // 
            this.stbMain.Location = new System.Drawing.Point(0, 450);
            this.stbMain.Name = "stbMain";
            this.stbMain.Size = new System.Drawing.Size(682, 22);
            this.stbMain.TabIndex = 5;
            this.stbMain.Text = "statusStrip1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 472);
            this.Controls.Add(this.stbMain);
            this.Controls.Add(this.tsMain);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_File;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_File_Esci;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_File_Modelling;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_File_Settings;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Settings_ODBCconnections;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Modelling_Measures;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Modelling_Districts;
        private System.Windows.Forms.PictureBox pbMain;
        private System.ServiceProcess.ServiceController svcWetSvc;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Service;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Service_Start;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Service_Stop;
        private System.Windows.Forms.ToolStripMenuItem mnuMain_Service_Restart;
        private System.Windows.Forms.ToolStripButton tsMain_Service_Start;
        private System.Windows.Forms.ToolStripButton tsMain_Service_Stop;
        private System.Windows.Forms.ToolStripButton tsMain_Service_Restart;
        private System.Windows.Forms.ToolStripSeparator tsMain_Sep1;
        private System.Windows.Forms.ToolStripButton tsMain_File_Exit;
        private System.Windows.Forms.Timer tmrCheckService;
        private System.Windows.Forms.StatusStrip stbMain;
    }
}

