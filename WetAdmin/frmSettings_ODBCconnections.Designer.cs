namespace WetAdmin
{
    partial class frmSettings_ODBCconnections
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings_ODBCconnections));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlUp = new System.Windows.Forms.Panel();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.cboDSN = new System.Windows.Forms.ComboBox();
            this.lblDSN = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
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
            this.dsConnections = new System.Data.DataSet();
            this.bsConnections = new System.Windows.Forms.BindingSource(this.components);
            this.tlpMain.SuspendLayout();
            this.pnlUp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnMain)).BeginInit();
            this.bnMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pnlUp, 0, 0);
            this.tlpMain.Controls.Add(this.dgDown, 0, 1);
            this.tlpMain.Controls.Add(this.bnMain, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.Size = new System.Drawing.Size(596, 545);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlUp
            // 
            this.pnlUp.Controls.Add(this.txtDescription);
            this.pnlUp.Controls.Add(this.lblDescription);
            this.pnlUp.Controls.Add(this.cboDSN);
            this.pnlUp.Controls.Add(this.lblDSN);
            this.pnlUp.Controls.Add(this.txtId);
            this.pnlUp.Controls.Add(this.lblId);
            this.pnlUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlUp.Location = new System.Drawing.Point(3, 3);
            this.pnlUp.Name = "pnlUp";
            this.pnlUp.Size = new System.Drawing.Size(590, 164);
            this.pnlUp.TabIndex = 0;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(101, 68);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDescription.Size = new System.Drawing.Size(475, 84);
            this.txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(11, 72);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(79, 17);
            this.lblDescription.TabIndex = 10;
            this.lblDescription.Text = "Description";
            // 
            // cboDSN
            // 
            this.cboDSN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDSN.FormattingEnabled = true;
            this.cboDSN.Location = new System.Drawing.Point(101, 37);
            this.cboDSN.Name = "cboDSN";
            this.cboDSN.Size = new System.Drawing.Size(274, 24);
            this.cboDSN.TabIndex = 0;
            // 
            // lblDSN
            // 
            this.lblDSN.AutoSize = true;
            this.lblDSN.Location = new System.Drawing.Point(11, 40);
            this.lblDSN.Name = "lblDSN";
            this.lblDSN.Size = new System.Drawing.Size(84, 17);
            this.lblDSN.TabIndex = 8;
            this.lblDSN.Text = "ODBC DSN:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(101, 9);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(121, 22);
            this.txtId.TabIndex = 7;
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(11, 12);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(25, 17);
            this.lblId.TabIndex = 6;
            this.lblId.Text = "ID:";
            // 
            // dgDown
            // 
            this.dgDown.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDown.Location = new System.Drawing.Point(3, 173);
            this.dgDown.Name = "dgDown";
            this.dgDown.ReadOnly = true;
            this.dgDown.RowTemplate.Height = 24;
            this.dgDown.Size = new System.Drawing.Size(590, 342);
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
            this.bnMain.Location = new System.Drawing.Point(0, 518);
            this.bnMain.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnMain.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnMain.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnMain.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnMain.Name = "bnMain";
            this.bnMain.PositionItem = this.bindingNavigatorPositionItem;
            this.bnMain.Size = new System.Drawing.Size(596, 27);
            this.bnMain.TabIndex = 3;
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
            this.bindingNavigatorDeleteItem.Text = "Elimina";
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
            this.cmdCommit.Click += new System.EventHandler(this.cmdCommit_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdExit.Image = global::WetAdmin.Resources_PNG_16x16.application_exit_2;
            this.cmdExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(23, 24);
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // dsConnections
            // 
            this.dsConnections.DataSetName = "Connections";
            // 
            // bsConnections
            // 
            this.bsConnections.AllowNew = true;
            // 
            // frmSettings_ODBCconnections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 545);
            this.Controls.Add(this.tlpMain);
            this.MinimizeBox = false;
            this.Name = "frmSettings_ODBCconnections";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ODBC connections";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_ODBCconnections_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.pnlUp.ResumeLayout(false);
            this.pnlUp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnMain)).EndInit();
            this.bnMain.ResumeLayout(false);
            this.bnMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlUp;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ComboBox cboDSN;
        private System.Windows.Forms.Label lblDSN;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.DataGridView dgDown;
        private System.Data.DataSet dsConnections;
        private System.Windows.Forms.BindingSource bsConnections;
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
        private System.Windows.Forms.ToolStripButton cmdCommit;
        private System.Windows.Forms.ToolStripButton cmdExit;
    }
}