﻿using System;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using D = System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace CreateStatements
{
    public partial class CreateStatements : Form
    {
        private QBLayer objQBLayer = new QBLayer();
        private string EmailStngsFile = AppDomain.CurrentDomain.BaseDirectory + "CmpnySettings.xml";
        private Settings objSettings = new Settings();
        private DataTable dtCmpnyStng;
        private DataTable Customers;

        public decimal Total = 0m;
        private string customeraddress, Emailid, citystate, subject;
        private string CustmrNme;
        private string CustomerName;
        private string transType;
        private DataSet TempPay = null;
        private DataTable OpenInvoice = null;
        private DataTable TempCrdt = null;
        private DataTable TempInv = null;
        private DataTable AgingInfo;
        private string LastPaymentAmt;
        private DateTime PaymentOn;
        private string LastDiscountAmt;
        private DateTime DiscountOn;
        CheckBox ckBox;
        List<FileName> FileNames;
        public bool IsBatchProcessing;
        public CreateStatements()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, D.Color.Gray, D.Color.Black, 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
        }

        private string EscapeLikeValue(string value)
        {
            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case ']':
                    case '[':
                    case '%':
                    case '*':
                        sb.Append("[").Append(c).Append("]");
                        break;

                    case '\'':
                        sb.Append("''");
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        private void btnFetchCustomers_Click(object sender, EventArgs e)
        {
            lblstatus.Text = "Fetching Customers";
            DataTable CustomersndJobs;
            DateTime? invoiceFromDate = null;
            if (File.Exists(Application.StartupPath + "\\QBSettings.xml"))
            {
                DataTable dtCompanyName = objSettings.GetXMLToDataTable("QBSettings.xml");

                if (dtCompanyName.Rows[0]["QBFilePath"].ToString() == "")
                {
                    MessageBox.Show("Please Fill the QB Settings", "Create Statements", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                bool test = objQBLayer.TestQB(dtCompanyName.Rows[0]["QBFilePath"].ToString()); ;
               this.Cursor = Cursors.Arrow;
               if (test)
                {
                    Customers = new DataTable();
                    if (invFrom.Checked)
                        invoiceFromDate = invFrom.Value;
                    CustomersndJobs = objQBLayer.GetCustomerRet(dtCompanyName.Rows[0]["QBFilePath"].ToString());
                    Customers = CustomersndJobs.Copy();

                    Customers.Columns.Add("OpenInvCount");
                    Customers.Columns.Add("OpenInvNumbers");
                    foreach (DataRow Cust in Customers.Rows)
                    {
                        if (invoiceFromDate == null)
                        {
                            Cust["OpenInvCount"] =0;
                            Cust["OpenInvNumbers"] ="";
                        }
                        else
                        {
                            Tuple<string, Int32> invInfo = GetOpenInvoiceCount(Convert.ToString(Cust["Full Name"]), dateTo.Value, invoiceFromDate,false);
                            Cust["OpenInvCount"] = invInfo.Item2;
                            Cust["OpenInvNumbers"] = invInfo.Item1;
                        }
                    }

                    if (IsBatchProcessing)
                    {
                        var rows = Customers.Select("TypeToSend is null");
                        foreach (var row in rows)
                            row.Delete();
                    }
                    //Customers.Columns.Remove("Job");

                    BindGrid(Customers);

                    lblstatus.Text = "Customers fetched";

                    DataView Dv = new DataView(Customers);
                    Dv.Sort = "Type ASC";
                    DataTable Type = Dv.ToTable(true, "Type");

                    for (int i = Type.Rows.Count - 1; i >= 0; i--)
                    {
                        if (Type.Rows[i][0] == DBNull.Value || Convert.ToString(Type.Rows[i][0]) == "")
                            Type.Rows[i].Delete();
                    }
                    Type.AcceptChanges();
                    DataRow dr = Type.NewRow();
                    dr["Type"] = "All";
                    Type.Rows.InsertAt(dr, 0);
                    ddlType.DataSource = Type;
                    ddlType.DisplayMember = "Type";
                    ddlType.ValueMember = "Type";
                    if (IsBatchProcessing)
                    {
                        ckBox.CheckState = CheckState.Checked;
                        //ckBox.Checked = true;
                    }
                }
                else
                {
                    lblstatus.Text = "Unable to fetch customers";
                }
            }
            else
                MessageBox.Show("Please Fill the QB Settings", "Create Statements", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// To display the open invoice count for the customer in grid
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="invTo"></param>
        /// <param name="invFrom"></param>
        /// <returns></returns>
        private Tuple<string, Int32> GetOpenInvoiceCount(string customerName, DateTime invTo, DateTime? invFrom,bool IncludeLineItems=true)
        {
            var inv = objQBLayer.GetAllOpenInvoice(customerName, invTo, invFrom,IncludeLineItems);

            StringBuilder sbInvNumber = new StringBuilder();
            if (inv != null)
            {
                //for (int i = 0; i < 4; i++)
                //{
                for (int i = 0; i < inv.Count; i++)
                {
                    if (null != inv.GetAt(i).RefNumber)
                    {
                        sbInvNumber.Append(inv.GetAt(i).RefNumber.GetValue().ToString());
                        sbInvNumber.Append("*");
                    }
                }
            }

            return new Tuple<string, Int32>(sbInvNumber.ToString(), inv != null ? inv.Count : 0);
        }

        private void BindGrid(DataTable cus)
        {
            dgvCustomers.Columns.Clear();
            dgvCustomers.DataSource = null;
            ckBox = new CheckBox();
            ckBox.Name = "ckBox";
            ckBox.BackColor = D.Color.Transparent;
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.ColumnCount = 9;
            DataGridViewCheckBoxColumn Chk = new DataGridViewCheckBoxColumn();

            Chk.Width = 25;
            dgvCustomers.Columns.Insert(0, Chk);
            dgvCustomers.Columns[1].Width = 15;
            D.Rectangle rect = dgvCustomers.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + (rect.Width / 4);
            ckBox.Size = new D.Size(20, 20);
            ckBox.Location = rect.Location;
            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);
            dgvCustomers.Controls.Add(ckBox);
            dgvCustomers.Columns[1].Name = "Full Name";
            dgvCustomers.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[1].Width = 210;

            dgvCustomers.Columns[1].DataPropertyName = "Full Name";

            dgvCustomers.Columns[2].Name = "FullAddress";

            dgvCustomers.Columns[2].Visible = false;
            dgvCustomers.Columns[2].DataPropertyName = "FullAddress";

            dgvCustomers.Columns[3].Name = "CityState";
            dgvCustomers.Columns[3].Visible = false;
            dgvCustomers.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[3].Width = 275;
            dgvCustomers.Columns[3].DataPropertyName = "CityState";

            dgvCustomers.Columns[4].Name = "Address";
            dgvCustomers.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[4].Width = 275;
            dgvCustomers.Columns[4].DataPropertyName = "Address";
            dgvCustomers.Columns[5].Name = "Type";
            dgvCustomers.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[5].Width = 73;
            dgvCustomers.Columns[5].DataPropertyName = "Type";



            dgvCustomers.Columns[6].Name = "Open Invoice Count";
            dgvCustomers.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[6].Width = 111;
            dgvCustomers.Columns[6].DataPropertyName = "OpenInvCount";

            dgvCustomers.Columns[7].Name = "OpenInvNumbers";
            dgvCustomers.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.Columns[7].Width = 110;
            dgvCustomers.Columns[7].DataPropertyName = "OpenInvNumbers";
            dgvCustomers.Columns[7].Visible = false;

            dgvCustomers.Columns[8].Name = "TypeToSend";
            dgvCustomers.Columns[8].DataPropertyName = "TypeToSend";
            dgvCustomers.Columns[8].Visible = false;


            dgvCustomers.Columns[9].Name = "Email";
            dgvCustomers.Columns[9].DataPropertyName = "Email";
            dgvCustomers.Columns[9].Visible = false;

            DataGridViewTextBoxColumn textboxColumn = new DataGridViewTextBoxColumn();
            textboxColumn.MaxInputLength = 50;
            //Bind DataGridView to Datasource
            dgvCustomers.DataSource = cus;
            //Add TextBoxColumn dynamically to DataGridView
            dgvCustomers.Columns.Add(textboxColumn);
            dgvCustomers.Columns[10].Width = 295;


        
        }

        private void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool State = ((CheckBox)sender).Checked;

            dgvCustomers.ClearSelection();
            dgvCustomers.CurrentCell = null;
            for (int i = 0; i < dgvCustomers.RowCount; i++)
            {
                dgvCustomers.Rows[i].Cells[0].Value = false;

                dgvCustomers.Rows[i].Cells[0].Value = State;
  
            }
        }

        private void btnCrtRprt_Click(object sender, EventArgs e)
        {
            if (chkWeekly.Checked)
                IsBatchProcessing = true;
            if (!IsBatchProcessing &&(chkInvoice.Checked == false && chkStatement.Checked == false))
                MessageBox.Show("Select any option");
            else
            {
             
                foreach (string filePath in  Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "PDF"))
                    File.Delete(filePath);
                FetchData();
                if (FileNames!=null && FileNames.Count > 0)
                {
                    //SendMail oSendMail = new SendMail();
                    string fileName = "";
                    string trasType = "";
                    if ((chkInvoice.Checked && chkStatement.Checked) || chkEmail.Checked)
                    { 
                        var distinctCustomers = FileNames.Select(f => f.CustomerName).Distinct();
                        foreach (string customer in distinctCustomers)
                        {
                            fileName = AppDomain.CurrentDomain.BaseDirectory + "PDF\\" + CleanFileName(customer + ".pdf");

                            CombineMultiplePDFs(FileNames.Where(f => f.CustomerName == customer).Select(s => s.FName).ToArray(), AppDomain.CurrentDomain.BaseDirectory + "PDF\\" + CleanFileName(customer + ".pdf"));
                            if (chkEmail.Checked)
                            {
                                trasType = IsBatchProcessing ? "Weekly Invoice/Statement" : (chkInvoice.Checked && chkStatement.Checked ? "Invoice & Statement" : (chkStatement.Checked ? "Statement" : chkInvoice.Checked ? "Invoice" : ""));
                      
                                Emailid= FileNames.Where(f => f.CustomerName==customer).Select(c=>c.Email).FirstOrDefault();
                                string subject = FileNames.Where(f => f.CustomerName == customer).Select(c => c.subject).FirstOrDefault();
                                //if (!string.IsNullOrEmpty(Emailid.Trim(',')))
                                //    oSendMail.SendEmail(trasType, CustomerName, new List<string>() { fileName }, Emailid, subject);
                                //else
                                    Log.WriteToApplicationLog( string.Format("Email not found - {0}",customer));
                            }
                        }
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + "PDF");

                    }
                    else
                    {
                        fileName = AppDomain.CurrentDomain.BaseDirectory + "PDF\\" + (chkInvoice.Checked ? "FinalInvoice.pdf" : "FinalStatement.pdf");
                        CombineMultiplePDFs(FileNames.Select(s => s.FName).ToArray(),  fileName);
                        Process.Start(fileName);
                    }
                    
                    string[] filePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Temp\\");
                    foreach (string filePath in filePaths)
                        File.Delete(filePath);

                }

                this.Cursor = Cursors.Arrow;
                lblstatus.Text = "Process completed";
                IsBatchProcessing = false;
            }
        }

       

        private void FetchData()
        {
            int Count = 0;
            CustmrNme = "";

            foreach (DataGridViewRow row in dgvCustomers.Rows)
            {
                bool checkd = Convert.ToBoolean(row.Cells[0].Value);
                if (checkd)
                {
                    Count = Count + 1;
                    if (Count > 2)
                    {
                        break;
                    }
                }
            }

            if (Count == 0)
            {
                lblstatus.Text = "No customer selected";
            }
            else
                if (FetchCmpnyPdfSettings())
                    ComputeAndCreatePDF();
                else
                    lblstatus.Text = "No company information found";
        }

        private void ComputeAndCreatePDF()
        {


            try
            {
                this.Cursor = Cursors.WaitCursor;
                DataTable dtCompanyName = objSettings.GetXMLToDataTable("QBSettings.xml");
                FileNames = new List<FileName>();
                if (dtCmpnyStng.Rows.Count == 0)
                {
                    lblstatus.Text = "No company information found";
                }
                bool test = objQBLayer.TestQB(dtCompanyName.Rows[0]["QBFilePath"].ToString()); ;
                if (test)
                {
                    foreach (DataGridViewRow row in dgvCustomers.Rows)
                    {
                        bool checkd = Convert.ToBoolean(row.Cells[0].Value); // Assuming the first column contains the Checkbox
                        if (checkd)
                        {
                            CustmrNme = "";
                         
                            CustmrNme = Convert.ToString(row.Cells[1].Value);
                            CustomerName = Convert.ToString(row.Cells[1].Value);
                            citystate = Convert.ToString(row.Cells[3].Value);

                            customeraddress = Convert.ToString(row.Cells[2].Value);
                            Emailid = Convert.ToString(row.Cells[9].Value); ;
                            subject = Convert.ToString(row.Cells[10].Value); ;

                            CustmrNme = Convert.ToString(CustmrNme);
                            lblstatus.Text = "Fetching open invoices for " + CustmrNme;
                            //  Application.DoEvents();
                            this.Refresh(); Total = 0m;
                            if (chkStatement.Checked ||ShouldCreate("s",Convert.ToString(row.Cells[8].Value) ))
                                CreateCustomerStatement();
                            if (chkInvoice.Checked || ShouldCreate("i",Convert.ToString(row.Cells[8].Value)))
                            {
                                string invNumbers = Convert.ToString(row.Cells[7].Value);
                                foreach (string invNumber in invNumbers.Split('*'))
                                {
                                    if (!string.IsNullOrEmpty(invNumber))
                                    {
                                        lblstatus.Text = "Fetching invoices for " + CustmrNme;
                                        this.Refresh();
                                        CreateInvoicePdf(objQBLayer.OrderItems(invNumber));
                                    }
                                }
                            }
                            //}


                        }
                    }

                }
                else
                {
                    lblstatus.Text = "Unable to connect to quickbooks";
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                Log.WriteToErrorLog("Method:ComputeAndCreatePDF-" + ex.Message);
            }
            finally
            {
                objQBLayer.CloseConnection();
                objQBLayer.CloseSession();
            }
        }

        private void CreateCustomerStatement()
        {
            OpenInvoice = objQBLayer.OpenInvoiceRept(dateTo.Value, CustmrNme);
            if (OpenInvoice != null)
            {
                if (OpenInvoice.Rows.Count > 0)
                {
                    lblstatus.Text = "Fetching payments for " + CustmrNme;
                    this.Refresh();
                    TempPay = objQBLayer.PaymentQuery(CustmrNme);
                    computefooter(OpenInvoice);
                    lblstatus.Text = "Fetching credits for " + CustmrNme;
                    this.Refresh();
                    TempCrdt = objQBLayer.CreditMemoQuery(CustmrNme, dateTo.Value);
                    TempInv = objQBLayer.GetAllInvoices(CustmrNme, dateTo.Value);
                    lblstatus.Text = "Creating statement report for " + CustmrNme;
                    this.Refresh();
                    if (TempPay.Tables[1].Rows.Count > 0)
                    {

                        int maxPymntid = TempPay.Tables[1].AsEnumerable().Where(tr => string.IsNullOrEmpty(Convert.ToString(tr["Discount"]))).Max(tr => Convert.ToInt32(tr["TxnNumber"]));

                        PaymentOn = TempPay.Tables[1].AsEnumerable().Where(tr => Convert.ToString(tr["TxnNumber"]) == maxPymntid.ToString()).Max(tr => Convert.ToDateTime(tr["PymntDate"]));
                        LastPaymentAmt = TempPay.Tables[1].AsEnumerable().Where(tr => Convert.ToString(tr["TxnNumber"]) == maxPymntid.ToString() && string.IsNullOrEmpty(Convert.ToString(tr["Discount"]))).Sum(tr => Convert.ToDecimal(tr["PayedAmount"])).ToString();
                    }
                    else
                    {
                        PaymentOn = Convert.ToDateTime("01/01/0001");
                        LastPaymentAmt = null;

                    }
                    if (TempCrdt.Rows.Count > 0)
                    {
                        var maxCrdtid = TempCrdt.AsEnumerable().Max(tr => Convert.ToInt32(tr["CrdtNumber"]));
                        if (maxCrdtid != 0)
                        {
                            DiscountOn =
                               TempCrdt.AsEnumerable().Where(tr => Convert.ToString(tr["CrdtNumber"]) == maxCrdtid.ToString()).Max(tr => Convert.ToDateTime(tr["CrdtDate"]));
                            LastDiscountAmt = TempCrdt.AsEnumerable().Where(tr => Convert.ToString(tr["CrdtNumber"]) == maxCrdtid.ToString()).Max(tr => Convert.ToString(tr["CrdtAmount"]));
                        }
                        else
                        {
                            DiscountOn = Convert.ToDateTime("01/01/0001");
                            LastDiscountAmt = null;

                        }

                        if (DiscountOn < PaymentOn)
                        {
                            DiscountOn = Convert.ToDateTime("01/01/0001");
                            LastDiscountAmt = null;

                        }

                    }
                    else
                    {
                        DiscountOn = Convert.ToDateTime("01/01/0001");
                        LastDiscountAmt = null;

                    }


                    OpenInvoice.Columns.Add("OrginalAmount");
                    foreach (DataRow dr in OpenInvoice.Rows)
                    {
                        DataRow[] drOrgAmount = TempInv.Select("[Invoice Number]='" + Convert.ToString(dr["Num"]) + "'");
                        if (drOrgAmount.Length > 0)
                            dr["OrginalAmount"] = drOrgAmount[0]["AppAmount"];
                        else
                        {
                            if (Convert.ToString(dr["Type"]) == "Invoice")
                            {
                                DataRow[] drOrgAmountt = TempInv.Select("[Amount]='" + Convert.ToString(dr["Open Balance"]) + "'");
                                if (drOrgAmountt.Length > 0)
                                    dr["OrginalAmount"] = drOrgAmountt[0]["AppAmount"];
                                else
                                    dr["OrginalAmount"] = dr["Amount"];

                            }
                        }
                    }

                    CreatePdfNw(OpenInvoice, customeraddress, citystate, CustomerName, DiscountOn, PaymentOn, LastPaymentAmt, LastDiscountAmt);
                }
                else
                    Log.WriteToApplicationLog("No data found for " + CustmrNme);
            }
            else
                Log.WriteToApplicationLog("No data found for " + CustmrNme);
        }

        public void computefooter(DataTable OpnInv)
        {
            try
            {
                DataTable dtCloned = OpnInv.Clone();
                dtCloned.Columns["Aging"].DataType = typeof(Int32);
                foreach (DataRow row in OpnInv.Rows)
                {
                    dtCloned.ImportRow(row);
                }

                DataRow[] val = dtCloned.Select("[Aging] is null");
                if (val.Length > 0)
                    AgingDetail.Current = GetString(val.CopyToDataTable());
                else
                    AgingDetail.Current = "0.00";

                val = dtCloned.Select("Aging < 31");
                if (val.Length > 0)
                    AgingDetail.tilThirty = GetString(val.CopyToDataTable());
                else
                    AgingDetail.tilThirty = "0.00";

                val = dtCloned.Select("Aging > 30 and Aging < 61");
                if (val.Length > 0)
                    AgingDetail.tillSixty = GetString(val.CopyToDataTable());
                else
                    AgingDetail.tillSixty = "0.00";
                val = dtCloned.Select("Aging > 60 and Aging < 91");
                if (val.Length > 0)
                    AgingDetail.tillNinty = GetString(val.CopyToDataTable());
                else
                    AgingDetail.tillNinty = "0.00";
                val = dtCloned.Select("Aging> 90");
                if (val.Length > 0)
                    AgingDetail.aboveNinty = GetString(val.CopyToDataTable());
                else
                    AgingDetail.aboveNinty = "0.00";

                AgingDetail.TotalBal = string.Format("{0:N2}", Math.Truncate(Total * 100) / 100); ;

                if (OpenInvoice.Rows.Count > 33)
                    AgingDetail.PageNumber = true;
                else
                    AgingDetail.PageNumber = false;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:ComputeFooter-" + ex.Message); throw;
            }
        }

        public string GetString(DataTable Comput)
        {
            decimal Totl = 0M;
            foreach (DataRow opn in Comput.Rows)
            {
                if (!Convert.ToString(opn["Open Balance"]).Contains('-'))
                {
                    Totl += Convert.ToDecimal(opn["Open Balance"]);
                    Total += Convert.ToDecimal(opn["Open Balance"]);
                }
                else
                {
                    Totl -= Convert.ToDecimal(Convert.ToString(opn["Open Balance"]).Replace('-', ' ').Trim());
                    Total -= Convert.ToDecimal(Convert.ToString(opn["Open Balance"]).Replace('-', ' ').Trim());
                }
            }

            if (Totl > 0 || Totl < 0)
                return string.Format("{0:N2}", Math.Truncate(Totl * 100) / 100);
            else
                return "0.00";
        }

        public Boolean FetchCmpnyPdfSettings()
        {
            try
            {
                if (!File.Exists(EmailStngsFile))
                {
                    MessageBox.Show("Company information is empty");
                    //return false;
                }
                DataSet dsFetchCmpnyPdfSettings = new DataSet();
                dsFetchCmpnyPdfSettings.ReadXml(EmailStngsFile);
                dtCmpnyStng = new DataTable();
                if (dsFetchCmpnyPdfSettings.Tables.Count > 0)
                {
                    dtCmpnyStng = dsFetchCmpnyPdfSettings.Tables[0];
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {

                Log.WriteToErrorLog("Method:FetchPDFSettings-" + ex.Message);
                return false;
            }
        }

        private string CleanFileName(string fileName)
        {
            return System.IO.Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private void CreateStatements_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void lnkSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Configuration ObjConfiguration = new Configuration();
            ObjConfiguration.ShowDialog();
        }

        private void lnkLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ViewLog ObjViewLog = new ViewLog();
            ObjViewLog.ShowDialog();
        }

        protected Font footer
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                Font Verdana = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL);
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = Verdana;
                return font;
            }
        }

        public void CreatePdfNw(DataTable Dt, string address, string citystate, string customername, DateTime LastDiscount, DateTime LastPayment, string LastPay, string Lastdisc)
        {
            try
            {
                Document doc = new Document(iTextSharp.text.PageSize.A4);

                string Path = AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + CleanFileName(customername + "_Statement.pdf");
                PdfWriter pw = PdfWriter.GetInstance(doc, new FileStream(Path, FileMode.Create));
                doc.Open();
                FontFactory.RegisterDirectories();
                //create an instance of your PDFpage class. This is the class we generated above.
                pdfPage page = new pdfPage();
                int cnt = 1;
                int currentcnt = 1;
                pw.PageEvent = page;
                PdfPTable FrstTable = new PdfPTable(2);
                FrstTable.DefaultCell.Border = Rectangle.TOP_BORDER;
                FrstTable.WidthPercentage = 105f;
                FrstTable.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                Font Verdana = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL);
                Font VerdanaBld = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);

                FrstTable.DefaultCell.BorderWidthBottom = 0;
                FrstTable.DefaultCell.BorderWidthTop = 0;
                FrstTable.HorizontalAlignment = iTextSharp.text.Image.ALIGN_TOP;
                string[] Addrs;
                PdfPCell NwCell;
                Phrase HeaderPhrase;

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["CompanyName"]), FontFactory.GetFont("Verdana", 17, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                FrstTable.AddCell(NwCell);
                HeaderPhrase = new Phrase("Statement", FontFactory.GetFont("Verdana", 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.PaddingRight = 15f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                FrstTable.AddCell(NwCell);
                PdfPTable cmpAdrs = new PdfPTable(1);

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["Address"]), FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                cmpAdrs.AddCell(NwCell);

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["City"]) + ", " + Convert.ToString(dtCmpnyStng.Rows[0]["State"]) + " " + Convert.ToString(dtCmpnyStng.Rows[0]["Zip"]), FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                cmpAdrs.AddCell(NwCell);

                HeaderPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 2f;
                cmpAdrs.AddCell(NwCell);

                Chunk Telph = new Chunk("           Fax#: " + Convert.ToString(dtCmpnyStng.Rows[0]["Fax"]), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                HeaderPhrase = new Phrase("Telephone: " + Convert.ToString(dtCmpnyStng.Rows[0]["Phone"]), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                HeaderPhrase.Add(Telph);
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                cmpAdrs.AddCell(NwCell);
                FrstTable.AddCell(cmpAdrs);

                PdfPTable HdrDate = new PdfPTable(3);
                int[] widths = new int[] { 50, 40, 10 };
                HdrDate.SetWidths(widths);
                HdrDate.DefaultCell.Border = Rectangle.NO_BORDER;

                HdrDate.TotalWidth = 20f;

                HeaderPhrase = new Phrase("  ", FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED));
                NwCell = new PdfPCell(HeaderPhrase);

                NwCell.Rowspan = 2;
                NwCell.Border = Rectangle.NO_BORDER;
                HdrDate.AddCell(NwCell);
                HeaderPhrase = new Phrase("Date", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                //  NwCell.Border = Rectangle.NO_BORDER;
                HdrDate.AddCell(NwCell);

                HeaderPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Rowspan = 2;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 3f;
                HdrDate.AddCell(NwCell);

                HeaderPhrase = new Phrase(dateTo.Value.ToString("MM/dd/yyyy"), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                // NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 15f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                HdrDate.AddCell(NwCell);

                NwCell = new PdfPCell(HdrDate);
                //   NwCell.PaddingLeft = 70f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.PaddingLeft = 93;
                NwCell.PaddingTop = 20;
                FrstTable.AddCell(NwCell);

                PdfPTable tblCustomerAddress = new PdfPTable(2);
                tblCustomerAddress.WidthPercentage = 105f;
                widths = new int[] { 40, 60 };
                tblCustomerAddress.DefaultCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.SetWidths(widths);
                PdfPTable InnerCustTable = new PdfPTable(1);
                InnerCustTable.DefaultCell.Border = Rectangle.NO_BORDER;
                Phrase CustAddress;

                NwCell = new PdfPCell(new Phrase(""));
                NwCell.FixedHeight = 30f;
                NwCell.Colspan = 2;
                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);

                CustAddress = new Phrase(customername, FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(CustAddress);
                NwCell.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                InnerCustTable.AddCell(NwCell);

                Addrs = new string[3];
                if (!string.IsNullOrEmpty(address))
                {
                    var AdrsLine = address.Split('*');

                    foreach (string Line in AdrsLine)
                    {
                        Addrs[0] += Line + " ";
                    }
                    Addrs[1] = string.IsNullOrEmpty(citystate) ? " " : citystate.Replace('*', ' '); ;
                }
                else
                {
                    Addrs[0] = string.IsNullOrEmpty(citystate) ? " " : citystate.Replace('*', ' '); ;
                    Addrs[1] = " ";

                }

                Addrs[2] = " ";


                for (int j = 0; j < Addrs.Length; j++)
                {
                    if (j == Addrs.Length - 1)
                    {
                        CustAddress = new Phrase(Addrs[j], FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                        NwCell = new PdfPCell(CustAddress);
                        NwCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                        InnerCustTable.AddCell(NwCell);
                    }
                    else
                    {
                        CustAddress = new Phrase(Addrs[j], FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(CustAddress);
                        NwCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        InnerCustTable.AddCell(NwCell);
                    }
                }

                NwCell = new PdfPCell(InnerCustTable);
                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);

                NwCell = new PdfPCell(new Phrase(""));
                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);

                PdfPTable SecondHeader = new PdfPTable(4);
                SecondHeader.WidthPercentage = 105f;
                //  SecondHeader.DefaultCell.Border = Rectangle.NO_BORDER;
                widths = new int[] { 30, 40, 17, 17 };
                SecondHeader.SetWidths(widths);
                NwCell = new PdfPCell(new Phrase("  "));
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Colspan = 4;
                SecondHeader.AddCell(NwCell);

                Phrase dscntPhrase;
                PdfPTable tblDiscount = new PdfPTable(3);
                tblDiscount.DefaultCell.Border = Rectangle.NO_BORDER;
                NwCell = new PdfPCell(new Phrase(" "));
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Colspan = 2;
                tblDiscount.AddCell(NwCell);
                NwCell = new PdfPCell(new Phrase(" "));
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Rowspan = 2;
                tblDiscount.AddCell(NwCell);

                PdfPTable InnerDiscnt = new PdfPTable(4);

                //widths = new int[] { 20, 15, 15,10 };
                //InnerDiscnt.SetWidths(widths);

                InnerDiscnt.DefaultCell.Border = Rectangle.NO_BORDER;

                dscntPhrase = new Phrase("Date Recv`d", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                InnerDiscnt.AddCell(NwCell);
                if (PaymentOn.ToString("MM/dd/yyyy") != "01/01/0001")
                    dscntPhrase = new Phrase(PaymentOn.ToString("MM/dd/yyyy"), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                else
                    dscntPhrase = new Phrase("", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.TOP_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                InnerDiscnt.AddCell(NwCell);

                dscntPhrase = new Phrase("Date Recv`d", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.TOP_BORDER;
                //      NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                InnerDiscnt.AddCell(NwCell);
                if (DiscountOn.ToString("MM/dd/yyyy") != "01/01/0001")
                    dscntPhrase = new Phrase(DiscountOn.ToString("MM/dd/yyyy"), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                else
                    dscntPhrase = new Phrase("", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                InnerDiscnt.AddCell(NwCell);

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Colspan = 4;
                NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                InnerDiscnt.AddCell(NwCell);

                dscntPhrase = new Phrase("Payment $", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER;

                InnerDiscnt.AddCell(NwCell);
                if (LastPaymentAmt != null)
                    NwCell = new PdfPCell(new Phrase(string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(LastPaymentAmt) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                else
                    NwCell = new PdfPCell(new Phrase(""));
                NwCell.Border = Rectangle.BOTTOM_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                InnerDiscnt.AddCell(NwCell);
                dscntPhrase = new Phrase("Disc. $ ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.BOTTOM_BORDER;
                InnerDiscnt.AddCell(NwCell);
                if (LastDiscountAmt != null)

                    dscntPhrase = new Phrase(string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(LastDiscountAmt) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                else
                    dscntPhrase = new Phrase("", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                InnerDiscnt.AddCell(NwCell);

                NwCell = new PdfPCell(InnerDiscnt); //change here
                //    NwCell.FixedHeight = 30f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                //   NwCell.PaddingLeft = 10f;
                //   NwCell.PaddingRight = 40f;
                NwCell.PaddingTop = 40f;
                NwCell.Colspan = 2;
                NwCell.Rowspan = 2;
                tblDiscount.AddCell(NwCell);

                NwCell = new PdfPCell(tblDiscount);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Colspan = 2;
                //  NwCell.FixedHeight =55f;
                SecondHeader.AddCell(NwCell);

                PdfPTable manualTable = new PdfPTable(2);
                manualTable.DefaultCell.Border = Rectangle.NO_BORDER;
                NwCell = new PdfPCell(new Phrase(" "));
                //NwCell.FixedHeight = 3f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Rowspan = 2;
                manualTable.AddCell(NwCell);                

                dscntPhrase = new Phrase("Discount #1", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                //    NwCell.FixedHeight = 30f;
                manualTable.AddCell(NwCell);

                dscntPhrase = new Phrase("Discount #2", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                //    NwCell.FixedHeight = 30f;
                manualTable.AddCell(NwCell);

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 25f;
                manualTable.AddCell(NwCell);                

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 25f;
                manualTable.AddCell(NwCell);

                dscntPhrase = new Phrase("Amount Due", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                manualTable.AddCell(NwCell);
                //    NwCell.FixedHeight = 30f;

                dscntPhrase = new Phrase("Amount Enc.", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                manualTable.AddCell(NwCell);

                dscntPhrase = new Phrase("$" + string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(AgingDetail.TotalBal) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                NwCell.FixedHeight = 25f;

                manualTable.AddCell(NwCell);

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 25f;
                manualTable.AddCell(NwCell);                

                NwCell = new PdfPCell(manualTable);
                NwCell.Colspan = 2;
                NwCell.Border = Rectangle.NO_BORDER;
                SecondHeader.AddCell(NwCell);

                PdfPTable MainHdr = new PdfPTable(4);
                MainHdr.WidthPercentage = 105f;
                MainHdr.DefaultCell.Border = Rectangle.NO_BORDER;
                widths = new int[] { 12, 53, 17, 17 };
                MainHdr.SetWidths(widths);

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Colspan = 4;
                NwCell.FixedHeight = 3f;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Date", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Transaction", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Amount", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Balance", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);
                decimal TotalAmount = 0;
                doc.Add(FrstTable);
                doc.Add(tblCustomerAddress);

                PdfPTable MainTable = new PdfPTable(4);
                MainTable.WidthPercentage = 105f;
                MainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                widths = new int[] { 12, 53, 17, 17 };
                MainTable.SetWidths(widths);

                foreach (DataRow drDetails in Dt.Rows)
                {
                    TotalAmount += Convert.ToDecimal(drDetails["Open Balance"]);

                    dscntPhrase = new Phrase(Convert.ToDateTime(drDetails["Date"]).ToString("MM/dd/yyyy"), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    if (Convert.ToString(drDetails["Type"]).Contains("Inv"))
                    {
                        dscntPhrase = new Phrase("INV #" + Convert.ToString(drDetails["Num"]) + ".   Orig.Amount $" + string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(drDetails["OrginalAmount"]) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    }
                    else
                        dscntPhrase = new Phrase("CREDMEM #" + Convert.ToString(drDetails["Num"]) + ".", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    dscntPhrase = new Phrase(string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(drDetails["Open Balance"]) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    dscntPhrase = new Phrase(string.Format("{0:N2}", Math.Truncate(Convert.ToDecimal(TotalAmount) * 100) / 100), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    if (cnt == Dt.Rows.Count)
                    {
                        int remainder = 0;
                        //if (Dt.Rows.Count > 36)
                        //    remainder = Dt.Rows.Count % 36;
                        //else
                        remainder = 33 - currentcnt;

                        while (remainder > 0)
                        {
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            remainder--;
                        }
                        dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(dscntPhrase);
                        NwCell.Border = Rectangle.TOP_BORDER;
                        NwCell.Colspan = 4;
                        MainTable.AddCell(NwCell);

                        doc.Add(SecondHeader);
                        doc.Add(MainHdr);
                        doc.Add(MainTable);
                    }
                    else if (cnt % 33 == 0 && cnt > 0)
                    {
                        dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(dscntPhrase);
                        NwCell.Border = Rectangle.TOP_BORDER;

                        NwCell.Colspan = 4;

                        MainTable.AddCell(NwCell);

                        doc.Add(SecondHeader);
                        doc.Add(MainHdr);
                        doc.Add(MainTable);

                        doc.NewPage();
                        doc.Add(FrstTable);
                        doc.Add(tblCustomerAddress);

                        MainTable = new PdfPTable(4);
                        MainTable.WidthPercentage = 105f;
                        MainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                        widths = new int[] { 12, 53, 17, 17 };
                        MainTable.SetWidths(widths);
                        currentcnt = 0;
                    }
                    cnt += 1;
                    currentcnt += 1;
                }
                doc.Close();
                Log.WriteToApplicationLog("Statement created for customer " + CustmrNme);
                // Process.Start(AppDomain.CurrentDomain.BaseDirectory + CleanFileName(customername + "_Statement.pdf"));
                //    SendToPrinter(AppDomain.CurrentDomain.BaseDirectory + CleanFileName(customername + "_Statement.pdf"));
                FileName ofileName = new FileName() { CustomerName = CustmrNme, FileType = FileName.Statement, FName = Path, Email = Emailid, subject =subject };
                FileNames.Add(ofileName);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:CreatePDF-" + ex.Message);
            }
        }




        private void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlType.Text))
                {
                    if (ddlType.Text == "All")
                    {
                        BindGrid(Customers);
                        if (ckBox != null)
                            ckBox.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        DataRow[] drcust = Customers.Select("Type='" + ddlType.Text + "'");
                        if (drcust.Length > 0)
                        {
                            BindGrid(drcust.CopyToDataTable());

                            if (ckBox != null)
                            {
                                ckBox.CheckState = CheckState.Unchecked;
                                ckBox.Refresh();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                Log.WriteToErrorLog("Method:TypeChange-" + ex.Message);
            }
        }



        public void CombineMultiplePDFs(string[] fileNames, string outFile)
        {
            try
            {
                int pageOffset = 0;
                ArrayList master = new ArrayList();
                int f = 0;
                Document document = null;
                PdfCopy writer = null;
                while (f < fileNames.Length)
                {
                    // we create a reader for a certain document
                    PdfReader reader = new PdfReader(fileNames[f]);
                    reader.ConsolidateNamedDestinations();
                    // we retrieve the total number of pages
                    int n = reader.NumberOfPages;
                    pageOffset += n;
                    if (f == 0)
                    {
                        // step 1: creation of a document-object
                        document = new Document(reader.GetPageSizeWithRotation(1));
                        // step 2: we create a writer that listens to the document
                        writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
                        // step 3: we open the document
                        document.Open();
                    }
                    // step 4: we add content
                    for (int i = 0; i < n; )
                    {
                        ++i;
                        if (writer != null)
                        {
                            PdfImportedPage page = writer.GetImportedPage(reader, i);
                            writer.AddPage(page);
                        }
                    }
                    PRAcroForm form = reader.AcroForm;
                    if (form != null && writer != null)
                    {
                        writer.CopyAcroForm(reader);
                    }
                    f++;
                    if (reader != null)
                        reader.Close();
                }
                // step 5: we close the document
                if (writer != null)
                {
                    writer.Close();
                }
                if (document != null)
                {
                    document.Close();
                }


                
                // SendEmail(outFile);
            }

            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:CombinePDF-" + ex.Message);
            }
        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCustomers.DataSource != null)
                    ((DataTable)dgvCustomers.DataSource).DefaultView.RowFilter = string.Format("[Full Name] like '{0}%'", txtsearch.Text.Trim().Replace("'", "''"));

            }
            catch (Exception ex)
            {


            }
        }




        public void CreateInvoicePdf(InvDetails oInvDetails)
        {
            try
            {
                Document doc = new Document(iTextSharp.text.PageSize.A4);
                AgingDetail.TotalBal = String.Format("{0:N2}", oInvDetails.InvAmount);
                string Path = AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + CleanFileName(oInvDetails.InvNum + "_Statement.pdf");
                PdfWriter pw = PdfWriter.GetInstance(doc, new FileStream(Path, FileMode.Create));
                doc.Open();
                FontFactory.RegisterDirectories();
                //create an instance of your PDFpage class. This is the class we generated above.
                pdfPageInv page = new pdfPageInv();
                int cnt = 1;
                int currentcnt = 1;
                pw.PageEvent = page;
                PdfPTable FrstTable = new PdfPTable(2);
                FrstTable.DefaultCell.Border = Rectangle.TOP_BORDER;
                //  FrstTable.DefaultCell.Border = 0;
                FrstTable.WidthPercentage = 105f;

                FrstTable.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                Font Verdana = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL);
                Font VerdanaBld = FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD);

                FrstTable.DefaultCell.BorderWidthBottom = 0;
                FrstTable.DefaultCell.BorderWidthTop = 0;
                FrstTable.HorizontalAlignment = iTextSharp.text.Image.ALIGN_TOP;
                string[] Addrs;

                PdfPCell NwCell;
                Phrase HeaderPhrase;

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["CompanyName"]), FontFactory.GetFont("Verdana", 17, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                FrstTable.AddCell(NwCell);
                HeaderPhrase = new Phrase("Invoice", FontFactory.GetFont("Verdana", 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.PaddingRight = 15f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                FrstTable.AddCell(NwCell);
                PdfPTable cmpAdrs = new PdfPTable(1);

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["Address"]), FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                cmpAdrs.AddCell(NwCell);

                HeaderPhrase = new Phrase(Convert.ToString(dtCmpnyStng.Rows[0]["City"]) + ", " + Convert.ToString(dtCmpnyStng.Rows[0]["State"]) + " " + Convert.ToString(dtCmpnyStng.Rows[0]["Zip"]), FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;

                cmpAdrs.AddCell(NwCell);

                HeaderPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 2f;
                cmpAdrs.AddCell(NwCell);

                Chunk Telph = new Chunk("           Fax#: " + Convert.ToString(dtCmpnyStng.Rows[0]["Fax"]), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                HeaderPhrase = new Phrase("Telephone: " + Convert.ToString(dtCmpnyStng.Rows[0]["Phone"]), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                HeaderPhrase.Add(Telph);
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                cmpAdrs.AddCell(NwCell);
                FrstTable.AddCell(cmpAdrs);

                PdfPTable HdrDate = new PdfPTable(3);
                int[] widths = new int[] { 10, 40, 40 };
                HdrDate.SetWidths(widths);
                //HdrDate.DefaultCell.Border = Rectangle.NO_BORDER;

                HdrDate.TotalWidth = 120f;

                HeaderPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED));
                NwCell = new PdfPCell(HeaderPhrase);

                NwCell.Rowspan = 1;
                NwCell.Border = Rectangle.NO_BORDER;
                HdrDate.AddCell(NwCell);
                HeaderPhrase = new Phrase("Date", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                //  NwCell.Border = Rectangle.NO_BORDER;
                HdrDate.AddCell(NwCell);

                HeaderPhrase = new Phrase("Invoice #", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                //  NwCell.Border = Rectangle.NO_BORDER;
                HdrDate.AddCell(NwCell);

                HeaderPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Rowspan = 1;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 3f;
                HdrDate.AddCell(NwCell);

                HeaderPhrase = new Phrase (  oInvDetails.InvDate.ToString("MM/dd/yyyy") , FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                // NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 15f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                HdrDate.AddCell(NwCell);


                HeaderPhrase = new Phrase(oInvDetails.InvNum, FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(HeaderPhrase);
                // NwCell.Border = Rectangle.NO_BORDER;
                NwCell.FixedHeight = 15f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                HdrDate.AddCell(NwCell);

                NwCell = new PdfPCell(HdrDate);
                //   NwCell.PaddingLeft = 70f;
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.PaddingLeft = 93;
                NwCell.PaddingTop = 20;
                FrstTable.AddCell(NwCell);

                PdfPTable tblCustomerAddress = new PdfPTable(2);
                tblCustomerAddress.WidthPercentage = 105f;
                widths = new int[] { 40, 60 };
                tblCustomerAddress.DefaultCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.SetWidths(widths);
                PdfPTable InnerCustTable = new PdfPTable(1);
                InnerCustTable.DefaultCell.Border = Rectangle.NO_BORDER;
                Phrase CustAddress;
                var adrs = oInvDetails.oCustAddress.First();
                NwCell = new PdfPCell(new Phrase(" "));
                NwCell.FixedHeight = 20f;
                NwCell.Colspan = 2;
                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);
                NwCell = new PdfPCell(new Phrase("Bill To"));
                //   NwCell.FixedHeight = 30f;
                // NwCell.Colspan = 2;
                //  NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);

                NwCell = new PdfPCell(new Phrase(" "));
                //  NwCell.FixedHeight = 30f;

                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);


                Addrs = new string[3];


                Addrs[0] = adrs.Addr1;
                Addrs[1] = adrs.Addr2;
                Addrs[2] = adrs.City + " " + adrs.PostalCode;


                for (int j = 0; j < Addrs.Length; j++)
                {
                    if (j == Addrs.Length - 1)
                    {
                        CustAddress = new Phrase(Addrs[j], FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                        NwCell = new PdfPCell(CustAddress);
                        NwCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                        InnerCustTable.AddCell(NwCell);
                    }
                    else
                    {
                        CustAddress = new Phrase(Addrs[j], FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(CustAddress);
                        NwCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        InnerCustTable.AddCell(NwCell);
                    }
                }

                NwCell = new PdfPCell(InnerCustTable);
                NwCell.Border = Rectangle.NO_BORDER;
                tblCustomerAddress.AddCell(NwCell);

                //terms

                PdfPTable tblTerms = new PdfPTable(2);
                tblTerms.WidthPercentage = 100f;
                widths = new int[] { 70, 30 };
                tblTerms.DefaultCell.Border = Rectangle.NO_BORDER;
                tblTerms.SetWidths(widths);
                NwCell = new PdfPCell(new Phrase(" "));
                NwCell.Border = Rectangle.NO_BORDER;
                tblTerms.AddCell(NwCell);
                

                //inner terms table start

                PdfPTable tblInnerTerms = new PdfPTable(1);
                tblInnerTerms.WidthPercentage = 100f;
                tblInnerTerms.DefaultCell.Border = Rectangle.NO_BORDER;

                NwCell = new PdfPCell(new Phrase("Terms"));
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                tblInnerTerms.AddCell(NwCell);

                NwCell = new PdfPCell(new Phrase(" "));
                //NwCell.Border = Rectangle.NO_BORDER;
                tblInnerTerms.AddCell(NwCell);
                //inner terms table end

                NwCell = new PdfPCell(tblInnerTerms);
                //NwCell.Border = Rectangle.NO_BORDER;
                tblTerms.AddCell(NwCell);

                tblCustomerAddress.AddCell(tblTerms);

                                
                Phrase dscntPhrase;


                PdfPTable MainHdr = new PdfPTable(6);
                MainHdr.WidthPercentage = 105f;
                MainHdr.DefaultCell.Border = Rectangle.NO_BORDER;
                widths = new int[] { 30, 30, 7, 10, 12, 12 };
                MainHdr.SetWidths(widths);

                dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.Border = Rectangle.NO_BORDER;
                NwCell.Colspan =6;
                NwCell.FixedHeight = 3f;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Item Code", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Description", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);


                dscntPhrase = new Phrase("Boxes", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Quantity", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Price Each", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                dscntPhrase = new Phrase("Amount", FontFactory.GetFont("Verdana", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                NwCell = new PdfPCell(dscntPhrase);
                NwCell.FixedHeight = 28f;
                NwCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                MainHdr.AddCell(NwCell);

                decimal TotalAmount = 0;
                doc.Add(FrstTable);
                doc.Add(tblCustomerAddress);
                // doc.Add(MainHdr);
                PdfPTable MainTable = new PdfPTable(6);
                MainTable.WidthPercentage = 105f;
                MainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                widths = new int[] { 30, 30, 7, 10, 12, 12};
                MainTable.SetWidths(widths);

                foreach (OrderLineItem drDetails in oInvDetails.oLstOrderLineItem)
                {
                    TotalAmount += 0;// Convert.ToDecimal(drDetails["Open Balance"]);

                    dscntPhrase = new Phrase(drDetails.ItemSKU, FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);


                    dscntPhrase = new Phrase(drDetails.ItemDesc, FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    dscntPhrase = new Phrase(drDetails.Box??"", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));

                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    dscntPhrase = new Phrase(drDetails.Quanity, FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    dscntPhrase = new Phrase(String.Format("{0:N2}", Convert.ToDouble(drDetails.Price)), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);


                    dscntPhrase = new Phrase(String.Format("{0:N2}", (Convert.ToDouble(drDetails.Price) * Convert.ToDouble(drDetails.Quanity))), FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                    NwCell = new PdfPCell(dscntPhrase);
                    NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                    NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                    MainTable.AddCell(NwCell);

                    if (cnt == oInvDetails.oLstOrderLineItem.Count)
                    {
                        int remainder = 0;
                        //if (Dt.Rows.Count > 36)
                        //    remainder = Dt.Rows.Count % 36;
                        //else
                        remainder = 40 - currentcnt;

                        while (remainder > 0)
                        {
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                            NwCell = new PdfPCell(dscntPhrase);
                            NwCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                            NwCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                            NwCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                            MainTable.AddCell(NwCell);
                            remainder--;
                        }
                        dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(dscntPhrase);
                        NwCell.Border = Rectangle.TOP_BORDER;
                        //|Rectangle.BOTTOM_BORDER|Rectangle.RIGHT_BORDER;
                        NwCell.Colspan = 6;
                        //   NwCell.FixedHeight = 3f;
                        MainTable.AddCell(NwCell);

                        //  doc.Add(SecondHeader);
                        doc.Add(MainHdr);
                        doc.Add(MainTable);
                    }
                    else if (cnt % 40 == 0 && cnt > 0)
                    {
                        dscntPhrase = new Phrase(" ", FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK));
                        NwCell = new PdfPCell(dscntPhrase);
                        NwCell.Border = Rectangle.TOP_BORDER;
                        //|Rectangle.BOTTOM_BORDER|Rectangle.RIGHT_BORDER;
                        NwCell.Colspan = 6;
                        //   NwCell.FixedHeight = 3f;
                        MainTable.AddCell(NwCell);

                        //   doc.Add(SecondHeader);
                        doc.Add(MainHdr);
                        doc.Add(MainTable);

                        doc.NewPage();
                        doc.Add(FrstTable);
                        doc.Add(tblCustomerAddress);

                        MainTable = new PdfPTable(6);
                        MainTable.WidthPercentage = 105f;
                        MainTable.DefaultCell.Border = Rectangle.NO_BORDER;
                        widths = new int[] { 30, 30, 7, 10, 12, 12 }; 
                        MainTable.SetWidths(widths);
                        currentcnt = 0;
                    }
                    cnt += 1;
                    currentcnt += 1;
                }
                doc.Close();
                Log.WriteToApplicationLog(string.Format("invoice created for customer {0} - #{1}", CustmrNme,oInvDetails.InvNum));
                // Process.Start(AppDomain.CurrentDomain.BaseDirectory + CleanFileName(customername + "_Statement.pdf"));
                //    SendToPrinter(AppDomain.CurrentDomain.BaseDirectory + CleanFileName(customername + "_Statement.pdf"));
                FileName ofileName = new FileName() { CustomerName = CustmrNme, FileType = FileName.Invoice, FName = Path, Email = Emailid, subject =subject };
                FileNames.Add(ofileName);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:CreatePDF-" + ex.Message);
            }
        }

        private void chkWeekly_CheckedChanged(object sender, EventArgs e)
        {
            IsBatchProcessing = chkWeekly.Checked;
            if (chkWeekly.Checked)
            {
              
                chkInvoice.Checked = false;
                chkStatement.Checked = false;
                chkInvoice.Enabled = false;
                chkStatement.Enabled = false;
            }
            else
            {
                   chkInvoice.Enabled = true;
                   chkStatement.Enabled = true;

                  
            }
            dgvCustomers.DataSource = null;
            dgvCustomers.Refresh();
        }

        private bool ShouldCreate(string type, string userSetting)
        {
            if (IsBatchProcessing)
            {
                if (userSetting.ToLower()=="b")
                {
                    return true;
                }
                else if (type == "i")
                {
                    return type == userSetting;
                }
                else 
                {
                    return type == userSetting;
                }


            }
            return false;
        }

    }




    public class FileName
    {
        public string CustomerName { get; set; }
        public string FName { get; set; }
        public string FileType { get; set; }
        public string Email { get; set; }
        public static string Invoice = "Inv", Statement = "Statement";
        public string subject { get; set; }
    }
}