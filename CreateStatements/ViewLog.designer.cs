namespace CreateStatements
{
    partial class ViewLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewLog));
            this.label1 = new System.Windows.Forms.Label();
            this.txtapp = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txterror = new System.Windows.Forms.TextBox();
            this.lnkClearLog = new System.Windows.Forms.LinkLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Calibri", 16F);
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Location = new System.Drawing.Point(274, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "View Log";
            // 
            // txtapp
            // 
            this.txtapp.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtapp.Font = new System.Drawing.Font("Calibri", 9F);
            this.txtapp.Location = new System.Drawing.Point(-4, 0);
            this.txtapp.Multiline = true;
            this.txtapp.Name = "txtapp";
            this.txtapp.ReadOnly = true;
            this.txtapp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtapp.Size = new System.Drawing.Size(654, 339);
            this.txtapp.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Calibri", 9F);
            this.tabControl1.Location = new System.Drawing.Point(0, 51);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(661, 362);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtapp);
            this.tabPage1.Font = new System.Drawing.Font("Calibri", 9F);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(653, 335);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Application Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txterror);
            this.tabPage2.Font = new System.Drawing.Font("Calibri", 9F);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(653, 335);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Error Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txterror
            // 
            this.txterror.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txterror.Font = new System.Drawing.Font("Calibri", 9F);
            this.txterror.Location = new System.Drawing.Point(0, 0);
            this.txterror.Multiline = true;
            this.txterror.Name = "txterror";
            this.txterror.ReadOnly = true;
            this.txterror.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txterror.Size = new System.Drawing.Size(650, 339);
            this.txterror.TabIndex = 0;
            // 
            // lnkClearLog
            // 
            this.lnkClearLog.AutoSize = true;
            this.lnkClearLog.BackColor = System.Drawing.Color.Transparent;
            this.lnkClearLog.Font = new System.Drawing.Font("Calibri", 9F);
            this.lnkClearLog.LinkColor = System.Drawing.Color.Maroon;
            this.lnkClearLog.Location = new System.Drawing.Point(547, 416);
            this.lnkClearLog.Name = "lnkClearLog";
            this.lnkClearLog.Size = new System.Drawing.Size(56, 14);
            this.lnkClearLog.TabIndex = 0;
            this.lnkClearLog.TabStop = true;
            this.lnkClearLog.Text = "Clear Log";
            this.lnkClearLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearLog_LinkClicked);
            // 
            // ViewLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CadetBlue;
            this.ClientSize = new System.Drawing.Size(661, 439);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lnkClearLog);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Calibri", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Statements";
            this.Load += new System.EventHandler(this.ViewLog_Load);
            this.Resize += new System.EventHandler(this.ViewLog_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtapp;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txterror;
        private System.Windows.Forms.LinkLabel lnkClearLog;
        private System.Windows.Forms.Label label1;
    }
}