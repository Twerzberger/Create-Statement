namespace CreateStatements
{
    partial class CreateStatements
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateStatements));
            this.btnFetchCustomers = new System.Windows.Forms.Button();
            this.dateTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCrtRprt = new System.Windows.Forms.Button();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.lblstatus = new System.Windows.Forms.Label();
            this.lnkSettings = new System.Windows.Forms.LinkLabel();
            this.lnkLog = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlType = new System.Windows.Forms.ComboBox();
            this.txtsearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.invFrom = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkEmail = new System.Windows.Forms.CheckBox();
            this.chkStatement = new System.Windows.Forms.CheckBox();
            this.chkInvoice = new System.Windows.Forms.CheckBox();
            this.chkWeekly = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFetchCustomers
            // 
            this.btnFetchCustomers.Font = new System.Drawing.Font("Calibri", 9F);
            this.btnFetchCustomers.Location = new System.Drawing.Point(804, 405);
            this.btnFetchCustomers.Name = "btnFetchCustomers";
            this.btnFetchCustomers.Size = new System.Drawing.Size(129, 45);
            this.btnFetchCustomers.TabIndex = 1;
            this.btnFetchCustomers.Text = "Customer List";
            this.btnFetchCustomers.UseVisualStyleBackColor = true;
            this.btnFetchCustomers.Click += new System.EventHandler(this.btnFetchCustomers_Click);
            // 
            // dateTo
            // 
            this.dateTo.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTo.Location = new System.Drawing.Point(333, 413);
            this.dateTo.Name = "dateTo";
            this.dateTo.Size = new System.Drawing.Size(105, 22);
            this.dateTo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(287, 418);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "As of :";
            // 
            // btnCrtRprt
            // 
            this.btnCrtRprt.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrtRprt.Location = new System.Drawing.Point(179, 2);
            this.btnCrtRprt.Name = "btnCrtRprt";
            this.btnCrtRprt.Size = new System.Drawing.Size(114, 45);
            this.btnCrtRprt.TabIndex = 6;
            this.btnCrtRprt.Text = "Create";
            this.btnCrtRprt.UseVisualStyleBackColor = true;
            this.btnCrtRprt.Click += new System.EventHandler(this.btnCrtRprt_Click);
            // 
            // dgvCustomers
            // 
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvCustomers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomers.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.MenuBar;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Location = new System.Drawing.Point(21, 66);
            this.dgvCustomers.Name = "dgvCustomers";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Calibri", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.NullValue = "test";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomers.RowHeadersVisible = false;
            this.dgvCustomers.ShowCellErrors = false;
            this.dgvCustomers.ShowCellToolTips = false;
            this.dgvCustomers.ShowEditingIcon = false;
            this.dgvCustomers.ShowRowErrors = false;
            this.dgvCustomers.Size = new System.Drawing.Size(1010, 317);
            this.dgvCustomers.TabIndex = 7;
            // 
            // lblstatus
            // 
            this.lblstatus.AutoSize = true;
            this.lblstatus.BackColor = System.Drawing.Color.Transparent;
            this.lblstatus.ForeColor = System.Drawing.Color.White;
            this.lblstatus.Location = new System.Drawing.Point(18, 493);
            this.lblstatus.Name = "lblstatus";
            this.lblstatus.Size = new System.Drawing.Size(0, 14);
            this.lblstatus.TabIndex = 8;
            // 
            // lnkSettings
            // 
            this.lnkSettings.AutoSize = true;
            this.lnkSettings.BackColor = System.Drawing.Color.Transparent;
            this.lnkSettings.ForeColor = System.Drawing.Color.Maroon;
            this.lnkSettings.LinkColor = System.Drawing.Color.Maroon;
            this.lnkSettings.Location = new System.Drawing.Point(833, 492);
            this.lnkSettings.Name = "lnkSettings";
            this.lnkSettings.Size = new System.Drawing.Size(49, 14);
            this.lnkSettings.TabIndex = 9;
            this.lnkSettings.TabStop = true;
            this.lnkSettings.Text = "Settings";
            this.lnkSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSettings_LinkClicked);
            // 
            // lnkLog
            // 
            this.lnkLog.AutoSize = true;
            this.lnkLog.BackColor = System.Drawing.Color.Transparent;
            this.lnkLog.ForeColor = System.Drawing.Color.Maroon;
            this.lnkLog.LinkColor = System.Drawing.Color.Maroon;
            this.lnkLog.Location = new System.Drawing.Point(897, 492);
            this.lnkLog.Name = "lnkLog";
            this.lnkLog.Size = new System.Drawing.Size(25, 14);
            this.lnkLog.TabIndex = 10;
            this.lnkLog.TabStop = true;
            this.lnkLog.Text = "Log";
            this.lnkLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLog_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(430, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(195, 29);
            this.label4.TabIndex = 11;
            this.label4.Text = "Create Statements";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(448, 418);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 14);
            this.label1.TabIndex = 13;
            this.label1.Text = "Type :";
            // 
            // ddlType
            // 
            this.ddlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlType.FormattingEnabled = true;
            this.ddlType.Location = new System.Drawing.Point(491, 415);
            this.ddlType.Name = "ddlType";
            this.ddlType.Size = new System.Drawing.Size(105, 22);
            this.ddlType.TabIndex = 14;
            this.ddlType.SelectedIndexChanged += new System.EventHandler(this.ddlType_SelectedIndexChanged);
            // 
            // txtsearch
            // 
            this.txtsearch.Location = new System.Drawing.Point(803, 456);
            this.txtsearch.Name = "txtsearch";
            this.txtsearch.Size = new System.Drawing.Size(129, 22);
            this.txtsearch.TabIndex = 18;
            this.txtsearch.TextChanged += new System.EventHandler(this.txtsearch_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(751, 461);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 14);
            this.label3.TabIndex = 17;
            this.label3.Text = "Search:";
            // 
            // invFrom
            // 
            this.invFrom.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.invFrom.Location = new System.Drawing.Point(176, 412);
            this.invFrom.Name = "invFrom";
            this.invFrom.ShowCheckBox = true;
            this.invFrom.Size = new System.Drawing.Size(105, 22);
            this.invFrom.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(108, 418);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 14);
            this.label5.TabIndex = 21;
            this.label5.Text = "Inv. From:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.chkEmail);
            this.panel1.Controls.Add(this.chkStatement);
            this.panel1.Controls.Add(this.chkInvoice);
            this.panel1.Controls.Add(this.btnCrtRprt);
            this.panel1.Location = new System.Drawing.Point(368, 459);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(299, 49);
            this.panel1.TabIndex = 22;
            // 
            // chkEmail
            // 
            this.chkEmail.AutoSize = true;
            this.chkEmail.ForeColor = System.Drawing.Color.White;
            this.chkEmail.Location = new System.Drawing.Point(96, 14);
            this.chkEmail.Name = "chkEmail";
            this.chkEmail.Size = new System.Drawing.Size(57, 18);
            this.chkEmail.TabIndex = 9;
            this.chkEmail.Text = "Email";
            this.chkEmail.UseVisualStyleBackColor = true;
            // 
            // chkStatement
            // 
            this.chkStatement.AutoSize = true;
            this.chkStatement.ForeColor = System.Drawing.Color.White;
            this.chkStatement.Location = new System.Drawing.Point(9, 26);
            this.chkStatement.Name = "chkStatement";
            this.chkStatement.Size = new System.Drawing.Size(82, 18);
            this.chkStatement.TabIndex = 8;
            this.chkStatement.Text = "Statement";
            this.chkStatement.UseVisualStyleBackColor = true;
            // 
            // chkInvoice
            // 
            this.chkInvoice.AutoSize = true;
            this.chkInvoice.ForeColor = System.Drawing.Color.White;
            this.chkInvoice.Location = new System.Drawing.Point(9, 5);
            this.chkInvoice.Name = "chkInvoice";
            this.chkInvoice.Size = new System.Drawing.Size(65, 18);
            this.chkInvoice.TabIndex = 7;
            this.chkInvoice.Text = "Invoice";
            this.chkInvoice.UseVisualStyleBackColor = true;
            // 
            // chkWeekly
            // 
            this.chkWeekly.AutoSize = true;
            this.chkWeekly.BackColor = System.Drawing.Color.Transparent;
            this.chkWeekly.ForeColor = System.Drawing.Color.White;
            this.chkWeekly.Location = new System.Drawing.Point(622, 419);
            this.chkWeekly.Name = "chkWeekly";
            this.chkWeekly.Size = new System.Drawing.Size(153, 18);
            this.chkWeekly.TabIndex = 10;
            this.chkWeekly.Text = "Only Weekly Customers";
            this.chkWeekly.UseVisualStyleBackColor = false;
            this.chkWeekly.CheckedChanged += new System.EventHandler(this.chkWeekly_CheckedChanged);
            // 
            // CreateStatements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(1055, 519);
            this.Controls.Add(this.chkWeekly);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.invFrom);
            this.Controls.Add(this.txtsearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ddlType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lnkLog);
            this.Controls.Add(this.lnkSettings);
            this.Controls.Add(this.lblstatus);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTo);
            this.Controls.Add(this.btnFetchCustomers);
            this.Font = new System.Drawing.Font("Calibri", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1071, 558);
            this.MinimumSize = new System.Drawing.Size(871, 558);
            this.Name = "CreateStatements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Statements";
            this.Resize += new System.EventHandler(this.CreateStatements_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFetchCustomers;
        private System.Windows.Forms.DateTimePicker dateTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCrtRprt;
        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.Label lblstatus;
        private System.Windows.Forms.LinkLabel lnkSettings;
        private System.Windows.Forms.LinkLabel lnkLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlType;
        private System.Windows.Forms.TextBox txtsearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker invFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkStatement;
        private System.Windows.Forms.CheckBox chkInvoice;
        private System.Windows.Forms.CheckBox chkEmail;
        private System.Windows.Forms.CheckBox chkWeekly;
    }
}