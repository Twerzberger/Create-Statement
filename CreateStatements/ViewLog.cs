using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CreateStatements
{
    public partial class ViewLog : Form
    {
        public ViewLog()
        {
            InitializeComponent();;
        }

        private void lnkClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch (MessageBox.Show("Are you sure, you want to clear Log?", "Clear", MessageBoxButtons.YesNo))
            {
                case DialogResult.Yes:
                    if (tabControl1.SelectedIndex == 0)
                    {
                        Log.ClearLog(Log.LogFileType.Application);;
                        txtapp.Text = Log.GetLog(Log.LogFileType.Application);
                    }
                    else if (tabControl1.SelectedIndex == 1)
                    {
                        Log.ClearLog(Log.LogFileType.Error);
                        txterror.Text = Log.GetLog(Log.LogFileType.Error);
                    }

                    break;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.Gray, Color.Black, 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
        }

        private void ViewLog_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            txtapp.Text = Log.GetLog(Log.LogFileType.Application);
            txtapp.SelectionStart = txtapp.Text.Trim().Length + 1;
            txtapp.ScrollToCaret();

            txterror.Text = Log.GetLog(Log.LogFileType.Error);
            txterror.SelectionStart = txterror.Text.Trim().Length + 1;
            txterror.ScrollToCaret();

            tabControl1.SelectedIndex = 0;
        }

        private void ViewLog_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

    }
}
