using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace CreateStatements
{
    public partial class Configuration : Form
    {
        private QBLayer objQblayer = new QBLayer();
        public static string gg = "ddd";
        private Settings objSettings = new Settings();
        private string CmpStngsFile = AppDomain.CurrentDomain.BaseDirectory + "CmpnySettings.xml";

        private DataTable dtGetConfiguration = new DataTable();
        private int SettingsID = 0;

        public Configuration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectCompany();
        }

        private void selectCompany()
        {
            try
            {
                OpenFileDialog dlgOpenFile = new OpenFileDialog();
                dlgOpenFile.Filter = "QuickBooks File(*.QBW)|*.QBW";
                switch (dlgOpenFile.ShowDialog())
                {
                    case DialogResult.Cancel:
                        break;

                    case DialogResult.OK:
                        objQblayer.CloseSession();
                        objQblayer.CloseConnection();
                        try
                        {
                            Process qbpro = new Process();
                            qbpro.StartInfo.FileName = dlgOpenFile.FileName;
                            qbpro.Start();
                        }
                        catch (Exception ex)
                        {
                            Log.WriteToErrorLog("Method:select company file button(),Message: " + ex.Message);
                        }
                        txtQBCompanyPath.Text = dlgOpenFile.FileName;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:select company file button(),Message: " + ex.Message);
            }
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {
            if (txtQBCompanyPath.Text == string.Empty)
            {
                MessageBox.Show("Please select the company file path", "CreateStatements", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveConfiguration();
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

        private void SaveConfiguration()
        {
            try
            {
                if (txtQBCompanyPath.Text == string.Empty)
                {
                    //MessageBox.Show(objDeclaration.strCompanyMessage, objDeclaration.Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Hashtable htAttributes = new Hashtable();
                htAttributes.Add("QBFilePath", txtQBCompanyPath.Text.Trim());

                if (SettingsID == 0)
                {
                    objSettings.CreateXML(htAttributes, "Root", "QBSettings",  "QBSettings.xml");
                    MessageBox.Show("All Information saved successfully ", "CreateStatements");
                }
                else
                {
                    objSettings.ModifyXML(htAttributes, Convert.ToString(SettingsID), "QBSettings", "QBSettings.xml");
                    MessageBox.Show("All Information saved successfully ", "CreateStatements");
                }
                //objQblayer.CloseSession();
                //objQblayer.CloseConnection();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:save QBfile Button(),Message:" + ex.Message);
            }
            finally
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //objQblayer.CloseSession();
            //objQblayer.CloseConnection();
            if (txtQBCompanyPath.Text == string.Empty)
            {
                MessageBox.Show("Please select the company file path", "CreateStatements", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                objQblayer.OpenConnection("CreateStatements");
                bool session = objQblayer.OpenSession(txtQBCompanyPath.Text.Trim());
                if (session)
                {
                    MessageBox.Show("QuickBooks certificate verified,please click the save button", "CreateStatements", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:Verify QBFile button" + ex.Message);
            }
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            verifyCompanyFile();
        }

        private void verifyCompanyFile()
        {
            dtGetConfiguration = new DataTable();
            try
            {
                if (File.Exists(Application.StartupPath + "\\QBSettings.xml"))
                {
                    dtGetConfiguration = objSettings.GetXMLToDataTable("QBSettings.xml");
                    SettingsID = Convert.ToInt32(dtGetConfiguration.Rows[0]["Id"].ToString());
                    txtQBCompanyPath.Text = dtGetConfiguration.Rows[0]["QBFilePath"].ToString();
                }
                if (File.Exists(CmpStngsFile))
                {
                    DataSet Ds = new DataSet();
                    Ds.ReadXml(CmpStngsFile);
                    if (Ds.Tables.Count > 0)
                    {
                        txtcmpname.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["CompanyName"]).Trim();
                        txtadrs.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["Address"]).Trim();
                        txtcity.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["City"]).Trim();
                        txtstate.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["State"]).Trim();
                        txtzip.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["Zip"]).Trim();
                        txtphone.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["Phone"]).Trim();
                        txtfax.Text = Convert.ToString(Ds.Tables["Settings"].Rows[0]["Fax"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:verifyCompanyFile -" + ex.Message);
            }
        }

        private void Configuration_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        public void CreateEmailAndLogoXML()
        {
            try
            {
                DataSet DS = new DataSet();
                DS.Tables.Add("Settings");
                DS.Tables["Settings"].Columns.Add("CompanyName");
                DS.Tables["Settings"].Columns.Add("Address");
                DS.Tables["Settings"].Columns.Add("City");
                DS.Tables["Settings"].Columns.Add("State");
                DS.Tables["Settings"].Columns.Add("Zip");
                DS.Tables["Settings"].Columns.Add("Phone");
                DS.Tables["Settings"].Columns.Add("Fax");
                DataRow dr = DS.Tables["Settings"].NewRow(); ;
                dr["CompanyName"] = " ";
                dr["Address"] = " ";
                dr["City"] = " ";
                dr["State"] = " ";
                dr["Zip"] = " ";
                dr["Phone"] = " ";
                dr["Fax"] = " ";
                DS.Tables["Settings"].Rows.Add(dr);
                DS.WriteXml(CmpStngsFile);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:CreateEmailAndLogoXML-" + ex.Message);
            }
        }

        private void btnFooterSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(CmpStngsFile))
                {
                    CreateEmailAndLogoXML();
                }

                DataSet Ds = new DataSet();
                Ds.ReadXml(CmpStngsFile);
                Ds.Tables["Settings"].Rows[0]["CompanyName"] = txtcmpname.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["Address"] = txtadrs.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["City"] = txtcity.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["State"] = txtstate.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["Zip"] = txtzip.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["Phone"] = txtphone.Text.Trim();
                Ds.Tables["Settings"].Rows[0]["Fax"] = txtfax.Text.Trim();

                Ds.WriteXml(CmpStngsFile);
                MessageBox.Show("Saved Successfully", "CreateStatements", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:FooterSave-" + ex.Message);
            }
        }

        private void txtIntrnlMail_Leave(object sender, EventArgs e)
        {
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

    }
}