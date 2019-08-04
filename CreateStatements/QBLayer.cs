using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Interop.QBFC13;

namespace CreateStatements
{
    public class QBLayer
    {
        private QBSessionManager mSessionManager = null;
        private bool mIsConnectionOpened = false;
        private bool mIsSessionBegined = false;
        public bool mIsErrorProcessing = false;
        public string QBApplicationName = "CreateStatements";
        private Settings objSettings = new Settings();
    
        public delegate void ProgressChangedEventHandler(String data);

        public static event ProgressChangedEventHandler Changed;

        #region Quickbooks Connections and session Management

        /// <summary>
        /// To open the QuickBooks connection
        /// </summary>
        /// <returns></returns>
        public bool OpenConnection(string pApplicationName)
        {
            mIsErrorProcessing = false;
            bool retVal = false;
            try
            {
                //Step1: Create instance for qbsession manager
                if (mSessionManager == null)
                    mSessionManager = new QBSessionManager();

                //Step2: Open QB session manager
                if (!mIsConnectionOpened)
                {
                    //LogManager.Log.WriteToApplicationLog("Open the QuickBooks Connection.");
                    mSessionManager.OpenConnection("", pApplicationName);
                    mIsConnectionOpened = true;
                    //// log.WriteToApplicationLog("QBW Application Name is:" + pApplicationName);
                }

                if (mIsConnectionOpened)
                    retVal = true;
            }
            catch (Exception ex)
            {
                //mIsErrorProcessing = true;
                //UpdateStatus(ex.Message);
                Log.WriteToErrorLog("Method:OpenConnection(), Message:" + ex.Message);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// To open the QuickBooks session
        /// </summary>
        /// <returns></returns>
        public bool OpenSession(string QBFilePath)
        {
            mIsErrorProcessing = false;
            bool retVal = false;
            try
            {
                //Step1: Create instance for qbsession manager
                if (mSessionManager == null)
                    return false;

                //Step2: check whether quickBooks opened or not
                if (!mIsConnectionOpened) return false;

                //Step2: Begin the QB session

                if (!mIsSessionBegined)
                {
                    //LogManager.Log.WriteToApplicationLog("Open the QuickBooks Session.");
                    //mSessionManager.BeginSession(string.Empty, ENOpenMode.omDontCare);
                    mSessionManager.BeginSession(QBFilePath, ENOpenMode.omDontCare);
                    mIsSessionBegined = true;
                    ////  log.WriteToApplicationLog("QBW File Path Is:" + QBFilePath);
                }

                if (mIsSessionBegined)
                    retVal = true;
            }
            catch (Exception ex)
            {
                mIsErrorProcessing = true;
                //UpdateStatus(ex.Message);
                Log.WriteToErrorLog("Method:OpenSession(), Message:" + ex.Message);
                retVal = false;
                MessageBox.Show(ex.Message, "CreateStatements", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return retVal;
        }

        /// <summary>
        /// To close the QuickBooks session
        /// </summary>
        /// <returns></returns>
        public bool CloseSession()
        {
            bool retVal = false;
            try
            {
                if (mSessionManager != null)
                {
                    //LogManager.Log.WriteToApplicationLog("Close the QuickBooks Session.");
                    mSessionManager.EndSession();
                    mIsSessionBegined = false;
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                // UpdateStatus(ex.Message);
                Log.WriteToErrorLog("Method:CloseSession(), Message:" + ex.Message);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// To close the QuickBooks connection
        /// </summary>
        /// <returns></returns>
        public bool CloseConnection()
        {
            bool retVal = false;
            try
            {
                if (mSessionManager != null)
                {
                    //Log.WriteToApplicationLog("Close the QuickBooks Connection.");
                    mSessionManager.CloseConnection();
                    mIsConnectionOpened = false;
                    retVal = true;
                }

                //Kill Process
                GC.Collect();

                //foreach (Process p in Process.GetProcesses())
                //{
                //    if (p.ProcessName.ToUpper() == "QBW32")
                //        p.Kill();
                //}
                GC.Collect();
            }
            catch (Exception ex)
            {
                // UpdateStatus(ex.Message);
                Log.WriteToErrorLog("Method:CloseConnection(),Message:" + ex.Message);
                retVal = false;
            }
            return retVal;
        }

        public bool TestQB(string QBPath)
        {
            try
            {
                string stMsg = string.Empty;
                if (!OpenConnection(QBApplicationName))
                {
                    // statusMsg = "QuickBooks connection failed.";
                    return false;
                }
                if (!OpenSession(QBPath))
                {
                    //statusMsg = "QuickBooks connection failed. \n";
                    return false;
                }
                //statusMsg = "QuickBooks successfully connected.";
                return true;
            }
            catch (Exception ex)
            {
                // statusMsg = ex.Message;
                return true;
            }
            finally
            {
                //CloseSession();
                //CloseConnection();
            }
        }

        #endregion Quickbooks Connections and session Management

        #region GetAllInvoice

        public IInvoiceRetList GetAllOpenInvoice(String CustomerName, DateTime Toval, DateTime? FromVal, bool includeListItems = true)
        {
            try
            {
                IInvoiceRetList InvoiceRet = null;

                //if (!OpenConnection(QBApplicationName))
                //{
                //    //statusMsg = "QuickBooks Connection not opened.";
                //    //return false;
                //}
                //if (!OpenSession(QBFilePath))
                //{
                //    // statusMsg = "QuickBooks Session not started.";
                //    //return false;
                //}

                IMsgSetRequest requestMsgSet = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                IInvoiceQuery InvoiceQueryRq = requestMsgSet.AppendInvoiceQueryRq();
                if (FromVal.HasValue)
                    InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.FromTxnDate.SetValue(FromVal.Value);
                InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.ToTxnDate.SetValue(Toval);

                //InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.DateMacro.SetValue(ENDateMacro.dmToday);
                //InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.ModifiedDateRangeFilter
                InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.PaidStatus.SetValue(ENPaidStatus.psNotPaidOnly);
                InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.EntityFilter.OREntityFilter.FullNameList.Add(CustomerName);

                //InvoiceQueryRq.ORInvoiceQuery.TxnIDList.Add();
                if (includeListItems)
                    InvoiceQueryRq.IncludeLineItems.SetValue(true);
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(requestMsgSet);

                if (responseMsgSet == null) return null;

                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null) return null;

                IResponse response = (IResponse)responseList.GetAt(0);

                if (response.StatusCode < 0)
                {
                    Log.WriteToErrorLog("Method:IInvoiceRetList,GetAllInvoice,StatusMessage:" + response.StatusMessage);
                    return null;
                }
                if (response.Detail != null)
                {
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtInvoiceQueryRs)
                    {
                        InvoiceRet = (IInvoiceRetList)response.Detail;
                    }
                }
                return InvoiceRet;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method():GetAllInvoice,Message: " + ex.Message);
                return null;
            }
            finally
            {
                //CloseSession();
                //CloseConnection();
            }
        }

        #endregion GetAllInvoice

        public DataTable GetAllInvoices(String CustomerName, DateTime Toval)
        {
            StringBuilder strBillAddress = new StringBuilder();
            StringBuilder strShipAddress = new StringBuilder();
            DataRow dr = null;

            DataSet dsInvoice = new DataSet();
            DataTable dtInvoiceDetails = new DataTable();

            #region dtInvoiceDetails Columns

            dtInvoiceDetails.Columns.Add("Customer");

            dtInvoiceDetails.Columns.Add("Invoice Number");

            dtInvoiceDetails.Columns.Add("Invoice Date");

            dtInvoiceDetails.Columns.Add("Due Date");

            dtInvoiceDetails.Columns.Add("Amount");

            dtInvoiceDetails.Columns.Add("AppAmount");

            #endregion dtInvoiceDetails Columns

            try
            {
                IInvoiceRetList InvoiceRet = GetAllOpenInvoice(CustomerName, Toval, null);

                if (InvoiceRet != null)
                {
                    //for (int i = 0; i < 4; i++)
                    //{
                    for (int i = 0; i < InvoiceRet.Count; i++)
                    {
                        dr = dtInvoiceDetails.NewRow();
                        if (InvoiceRet.GetAt(i).RefNumber != null)
                        {
                            //dr["Invoice Number"] = InvoiceRet.GetAt(i).RefNumber.GetValue().ToString();

                            dr["Invoice Number"] = InvoiceRet.GetAt(i).RefNumber.GetValue().ToString();
                        }

                        if (InvoiceRet.GetAt(i).TxnDate != null)
                        {
                            dr["Invoice Date"] = InvoiceRet.GetAt(i).TxnDate.GetValue();
                        }

                        if (InvoiceRet.GetAt(i).CustomerRef != null)
                        {
                            dr["Customer"] = Convert.ToString(InvoiceRet.GetAt(i).CustomerRef.FullName.GetValue().ToString());
                        }

                        //IsToBeEmailed

                        //Class Code
                        if (InvoiceRet.GetAt(i).DueDate != null)
                        {
                            dr["Due Date"] = Convert.ToDateTime(InvoiceRet.GetAt(i).DueDate.GetValue());
                        }

                        if (InvoiceRet.GetAt(i).BalanceRemaining != null)
                        {
                            dr["Amount"] = Convert.ToDouble(InvoiceRet.GetAt(i).BalanceRemaining.GetValue());
                        }
                        if (InvoiceRet.GetAt(i).Subtotal != null)
                        {
                            dr["AppAmount"] = Convert.ToDouble(InvoiceRet.GetAt(i).Subtotal.GetValue());
                        }

                        dtInvoiceDetails.Rows.Add(dr);
                        dtInvoiceDetails.AcceptChanges();
                    }
                }
                //dsInvoice.Tables.Add(dtInvoiceDetails);
                //dsInvoice.Tables.Add(dtInvoiceLineItemDetails);
                //  PaymentQuery(CustomerName);
                return dtInvoiceDetails;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method():GetAllInvoices,Message: " + ex.ToString());
                return dtInvoiceDetails;
            }
            finally
            {
            }
        }

        #region GetCustomers

        public DataTable GetCustomerRet(string QBPath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("First Name");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("Full Name");
            dt.Columns.Add("Job");
            dt.Columns.Add("FullAddress");
            dt.Columns.Add("CityState");
            dt.Columns.Add("Address");
            dt.Columns.Add("Type");
            dt.Columns.Add("Email");
            dt.Columns.Add("TypeToSend");
            DataRow drname;
            ICustomerRetList CustomerRetList = null;
            ICustomerRet CustomerRet = null;
            StringBuilder strBillAddress = new StringBuilder();
            try
            {

                IMsgSetRequest requestMsgSet = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                //Append customer customer request to messageset request
                ICustomerQuery CustomerQueryRq = requestMsgSet.AppendCustomerQueryRq();
                //CustomerQueryRq.ORCustomerListQuery.FullNameList.Add(strCustomerName);
                CustomerQueryRq.ORCustomerListQuery.CustomerListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
                CustomerQueryRq.OwnerIDList.Add("0");
                //Process the customer query request
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(requestMsgSet);

                if (responseMsgSet == null)
                {
                }//return CustomerRet;

                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null)
                {
                }//return CustomerRet;

                IResponse response = (IResponse)responseList.GetAt(0);

                //if (response.StatusCode < 0) //return CustomerRet;

                if (response.Detail != null)
                {
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtCustomerQueryRs)
                    {
                        //upcast to more specific type here, this is safe because we checked with response.Type check above
                        CustomerRetList = (ICustomerRetList)response.Detail;
                        if (CustomerRetList != null)
                        {
                            if (CustomerRetList.Count > 0)
                                for (int i = 0; i < CustomerRetList.Count; i++)
                                {
                                    if (CustomerRetList.GetAt(i).TotalBalance != null)
                                        if (CustomerRetList.GetAt(i).TotalBalance.GetValue() == 0.00)
                                            continue;

                                    CustomerRet = (ICustomerRet)CustomerRetList.GetAt(i);

                                    drname = dt.NewRow();

                                    if (CustomerRetList.GetAt(i).FirstName != null)
                                        drname["First Name"] = CustomerRetList.GetAt(i).FirstName.GetValue().ToString();
                                    else
                                        drname["First Name"] = string.Empty;
                                    if (CustomerRetList.GetAt(i).LastName != null)
                                        drname["Last Name"] = CustomerRetList.GetAt(i).LastName.GetValue().ToString();
                                    else
                                        drname["Last Name"] = string.Empty;
                                    if (CustomerRetList.GetAt(i).ParentRef == null)
                                    {
                                        if (CustomerRetList.GetAt(i).FullName != null)
                                            drname["Full Name"] = CustomerRetList.GetAt(i).FullName.GetValue().ToString();
                                        else
                                            drname["Full Name"] = string.Empty;
                                    }
                                    else
                                    {
                                        if (CustomerRetList.GetAt(i).FullName != null)
                                        {
                                            drname["Full Name"] = CustomerRetList.GetAt(i).FullName.GetValue().ToString();
                                            drname["Job"] = CustomerRetList.GetAt(i).ParentRef.FullName.GetValue().ToString();
                                        }
                                        else
                                        {
                                            drname["Job"] = string.Empty;
                                            drname["Full Name"] = string.Empty;
                                        }
                                    }

                                    if (CustomerRetList.GetAt(i).BillAddress != null)
                                    {
                                        strBillAddress = new StringBuilder();

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr2 != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr2.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr3 != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr3.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr4 != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr4.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr5 != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr5.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }

                                        drname["FullAddress"] = strBillAddress.ToString();
                                        strBillAddress = new StringBuilder();
                                        if (CustomerRetList.GetAt(i).BillAddress.City != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.City.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }
                                        if (CustomerRetList.GetAt(i).BillAddress.State != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.State.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }

                                        if (CustomerRetList.GetAt(i).BillAddress.PostalCode != null)
                                        {
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.PostalCode.GetValue().ToString());
                                            strBillAddress.Append("*");
                                        }
                                        drname["CityState"] = strBillAddress;
                                    }
                                    else
                                    {
                                        drname["FullAddress"] = string.Empty;
                                    }


                                    if (CustomerRetList.GetAt(i).BillAddress != null)
                                    {
                                        strBillAddress = new StringBuilder();

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr2 != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr2.GetValue().ToString() + ",");

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr3 != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr3.GetValue().ToString() + ",");
                                        string g = CustomerRetList.GetAt(i).Balance.GetValue().ToString();

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr4 != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr4.GetValue().ToString() + ",");

                                        if (CustomerRetList.GetAt(i).BillAddress.Addr5 != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Addr5.GetValue().ToString() + ",");
                                        if (CustomerRetList.GetAt(i).BillAddress.City != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.City.GetValue().ToString() + ",");
                                        if (CustomerRetList.GetAt(i).BillAddress.Country != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.Country.GetValue().ToString() + ",");

                                        if (CustomerRetList.GetAt(i).BillAddress.PostalCode != null)
                                            strBillAddress.Append(CustomerRetList.GetAt(i).BillAddress.PostalCode.GetValue().ToString());

                                        drname["Address"] = strBillAddress.ToString();
                                    }
                                    else
                                        drname["Address"] = string.Empty;


                                    if (CustomerRetList.GetAt(i).CustomerTypeRef != null)
                                    {
                                        drname["Type"] = CustomerRetList.GetAt(i).CustomerTypeRef.FullName.GetValue();
                                    }

                                    StringBuilder sbEmail = new StringBuilder();
                                    if (CustomerRetList.GetAt(i).Email != null)
                                    {
                                        sbEmail.Append(CustomerRetList.GetAt(i).Email.GetValue() + ",");
                                       // drname["Email"] = CustomerRetList.GetAt(i).Email.GetValue();
                                    }
                                    if (CustomerRetList.GetAt(i).Cc != null)
                                    {
                                        sbEmail.Append(CustomerRetList.GetAt(i).Cc.GetValue());
                                    }
                                    drname["Email"] = sbEmail.ToString().Trim(',');

                                    
                                    if (CustomerRetList.GetAt(i).DataExtRetList != null)
                                    {
                                        for (int k = 0; k < CustomerRetList.GetAt(i).DataExtRetList.Count; k++)
                                        {
                                            IDataExtRet DataExtRet = CustomerRetList.GetAt(i).DataExtRetList.GetAt(k);
                                            if (Convert.ToString(DataExtRet.DataExtName.GetValue()) == "AutoEmail")
                                            {
                                                if (DataExtRet.DataExtValue != null)
                                                    drname["TypeToSend"] = DataExtRet.DataExtValue.GetValue();

                                            }
                                        }
                                    }


                                    dt.Rows.Add(drname);

                                    //return CustomerRet;
                                }
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method():GetCustomerRet,Message: " + ex.ToString());
                return dt;
            }
            finally
            {
                CustomerRetList = null;
            }
        }

        #endregion GetCustomers

        public DataSet PaymentQuery(string CustomerName)
        {
            IReceivePaymentRetList PaymentRet = null;
            try
            {
                DataTable dtPayments = new DataTable();
                dtPayments.Columns.Add("PymntDate");
                dtPayments.Columns.Add("PymntType");
                dtPayments.Columns.Add("ListId");
                DataTable dtPaymentslst = new DataTable();
                dtPaymentslst.Columns.Add("ListId");
                dtPaymentslst.Columns.Add("PayedAmount");
                dtPaymentslst.Columns.Add("BalanceAmount");
                dtPaymentslst.Columns.Add("Discount");
                dtPaymentslst.Columns.Add("InvoiceNumber");

                dtPaymentslst.Columns.Add("TxnNumber");
                dtPaymentslst.Columns.Add("PymntDate");
                IMsgSetRequest requestMsgSet = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                IReceivePaymentQuery ReceivePaymentQueryRq = requestMsgSet.AppendReceivePaymentQueryRq();
                ReceivePaymentQueryRq.ORTxnQuery.TxnFilter.EntityFilter.OREntityFilter.FullNameList.Add(CustomerName);
                ReceivePaymentQueryRq.IncludeLineItems.SetValue(true);
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(requestMsgSet);
                if (responseMsgSet == null) ;

                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null) ;

                IResponse response = (IResponse)responseList.GetAt(0);
                string type = "";
                if (response.StatusCode < 0) ;

                if (response.Detail != null)
                {
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtReceivePaymentQueryRs)
                    {
                        PaymentRet = (IReceivePaymentRetList)response.Detail;

                        for (int k = 0; k < PaymentRet.Count; k++)
                        {
                            DataRow Dr = dtPayments.NewRow();

                            if (PaymentRet.GetAt(k).TxnDate != null)
                                Dr["PymntDate"] = PaymentRet.GetAt(k).TxnDate.GetValue();

                            if (PaymentRet.GetAt(k).PaymentMethodRef != null)
                                Dr["PymntType"] = PaymentRet.GetAt(k).PaymentMethodRef.FullName.GetValue().ToString(); ;

                            if (PaymentRet.GetAt(k).TxnID != null)
                                Dr["ListId"] = PaymentRet.GetAt(k).TxnID.GetValue().ToString(); ;

                            if (PaymentRet.GetAt(k).AppliedToTxnRetList != null)
                            {
                                for (int i = 0; i < PaymentRet.GetAt(k).AppliedToTxnRetList.Count; i++)
                                {
                                    DataRow drPaymentItems = dtPaymentslst.NewRow();
                                    IAppliedToTxnRet AppliedToTxnRet = PaymentRet.GetAt(k).AppliedToTxnRetList.GetAt(i);

                                    if (PaymentRet.GetAt(k).TxnDate != null)
                                        drPaymentItems["PymntDate"] = PaymentRet.GetAt(k).TxnDate.GetValue();

                                    if (PaymentRet.GetAt(k).TxnNumber != null)
                                        drPaymentItems["TxnNumber"] = PaymentRet.GetAt(k).TxnNumber.GetValue();

                                    if (AppliedToTxnRet.RefNumber != null)
                                    {
                                        drPaymentItems["InvoiceNumber"] = (string)AppliedToTxnRet.RefNumber.GetValue();
                                    }

                                    if (AppliedToTxnRet.RefNumber != null)
                                    {
                                        drPaymentItems["PayedAmount"] = (string)AppliedToTxnRet.Amount.GetValue().ToString();
                                    }

                                    if (AppliedToTxnRet.BalanceRemaining != null)
                                    {
                                        drPaymentItems["BalanceAmount"] = (string)AppliedToTxnRet.BalanceRemaining.GetValue().ToString();
                                    }
                                    if (AppliedToTxnRet.DiscountAmount != null)
                                    {
                                        drPaymentItems["Discount"] = (string)AppliedToTxnRet.DiscountAmount.GetValue().ToString();
                                    }

                                    if (AppliedToTxnRet.RefNumber != null)
                                    {
                                        drPaymentItems["InvoiceNumber"] = (string)AppliedToTxnRet.RefNumber.GetValue();
                                    }
                                    if (PaymentRet.GetAt(k).TxnID != null)
                                    {
                                        drPaymentItems["ListId"] = PaymentRet.GetAt(k).TxnID.GetValue().ToString();
                                    }
                                    dtPaymentslst.Rows.Add(drPaymentItems);
                                }
                            }

                            dtPayments.Rows.Add(Dr);
                        }
                    }
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(dtPayments);
                ds.Tables.Add(dtPaymentslst);
                return ds;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method:PaymentQuery() " + ex.Message);
                return null;
            }
            finally
            {
            }
        }

        #region GetCreditMemo

        public DataTable CreditMemoQuery(string Customer, DateTime Toval)
        {
            ICreditMemoRetList CreditMemoRet = null;
            DataTable dtCreditMemo = new DataTable();
            dtCreditMemo.Columns.Add("CrdtDate");
            dtCreditMemo.Columns.Add("CrdtNumber");
            dtCreditMemo.Columns.Add("CrdtAmount");
            dtCreditMemo.Columns.Add("CrdtType");
            dtCreditMemo.Columns.Add("Bal");
            dtCreditMemo.Columns.Add("Customer");
            try
            {
                IMsgSetRequest requestMsgSet = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                ICreditMemoQuery CreditMemoQueryRq = requestMsgSet.AppendCreditMemoQueryRq();

                CreditMemoQueryRq.IncludeLinkedTxns.SetValue(true);
                //CreditMemoQueryRq.IncludeLinkedTxns.SetValue(true);
                CreditMemoQueryRq.ORTxnQuery.TxnFilter.EntityFilter.OREntityFilter.FullNameList.Add(Customer);
                //  CreditMemoQueryRq.ORTxnQuery.TxnFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.ToTxnDate.SetValue(Toval);
                //Qb Execute query
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(requestMsgSet);
                if (responseMsgSet == null) ;

                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null) ;

                IResponse response = (IResponse)responseList.GetAt(0);

                if (response.StatusCode < 0) ;

                if (response.Detail != null)
                {
                    string f;
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtCreditMemoQueryRs)
                    {
                        CreditMemoRet = (ICreditMemoRetList)response.Detail;
                        for (int i = 0; i < CreditMemoRet.Count; i++)
                        {
                            DataRow Dr = dtCreditMemo.NewRow();
                            if (CreditMemoRet.GetAt(i).TotalAmount != null)
                                Dr["CrdtAmount"] = CreditMemoRet.GetAt(i).TotalAmount.GetValue().ToString();
                            if (CreditMemoRet.GetAt(i).TxnNumber != null)
                                Dr["CrdtNumber"] = CreditMemoRet.GetAt(i).TxnNumber.GetValue().ToString();
                            if (CreditMemoRet.GetAt(i).DueDate != null)
                                Dr["CrdtDate"] = CreditMemoRet.GetAt(i).TxnDate.GetValue();
                            if (CreditMemoRet.GetAt(i).DueDate != null)
                                Dr["CrdtType"] = "";
                            if (CreditMemoRet.GetAt(i).IsPending != null)
                                Dr["Bal"] = CreditMemoRet.GetAt(i).CreditRemaining.GetValue().ToString();
                            if (CreditMemoRet.GetAt(i).CustomerRef != null)
                                Dr["Customer"] = CreditMemoRet.GetAt(i).CustomerRef.FullName.GetValue().ToString();
                            dtCreditMemo.Rows.Add(Dr);

                            if (CreditMemoRet.GetAt(i).LinkedTxnList != null)
                            {
                                string ggf;
                                ILinkedTxn LinkedTxn = CreditMemoRet.GetAt(i).LinkedTxnList.GetAt(0);

                                ENTxnType TxnType968 = (ENTxnType)LinkedTxn.TxnType.GetValue();
                                if (LinkedTxn.RefNumber != null)
                                    ggf = LinkedTxn.RefNumber.GetValue();
                                {
                                    Dr["CrdtType"] = TxnType968.ToString();
                                }
                            }
                        }
                    }
                }
                return dtCreditMemo;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method():CreditMemoQuery,Message: " + ex.ToString());
                return null;
            }
            finally
            {
            }
        }

        #endregion GetCreditMemo



        #region NewReportQuery

        private DataTable WalkReportRet(IReportRet ReportRet, string strtype)
        {
            DataTable dtOpenInv = new DataTable();
            try
            {
                Hashtable htxmlClass = new Hashtable();

                dtOpenInv.Columns.Add("Customer");

                DataTable dtDetails = new DataTable();

                if (ReportRet == null) return dtOpenInv;
                string ReportTitle7 = (string)ReportRet.ReportTitle.GetValue();
                string ReportSubtitle8 = (string)ReportRet.ReportSubtitle.GetValue();
                if (ReportRet.ReportBasis != null)
                {
                    ENReportBasis ReportBasis9 = (ENReportBasis)ReportRet.ReportBasis.GetValue();
                }
                int NumRows10 = (int)ReportRet.NumRows.GetValue();
                int NumColumns11 = (int)ReportRet.NumColumns.GetValue();
                int NumColTitleRows12 = (int)ReportRet.NumColTitleRows.GetValue();
                if (ReportRet.ColDescList != null)
                {
                    List<string> lstcolid = new List<string>();
                    List<string> lstcolidName = new List<string>();
                    for (int i13 = 0; i13 < ReportRet.ColDescList.Count; i13++)
                    {
                        string count = "";
                        int s = 0;
                        string StrColName = string.Empty;
                        IColDesc ColDesc = ReportRet.ColDescList.GetAt(i13);
                        if (ColDesc.ColTitleList != null)
                        {
                            for (int i14 = 0; i14 < ColDesc.ColTitleList.Count; i14++)
                            {
                                IColTitle ColTitle = ColDesc.ColTitleList.GetAt(i14);
                                if (ColTitle.value != null)
                                {
                                    if (string.IsNullOrEmpty(StrColName))
                                    {
                                        StrColName = StrColName + ColTitle.value.GetValue();
                                        count = ColDesc.colID.GetValue().ToString();
                                    }
                                }
                            }
                        }
                        ENColType ColType15 = (ENColType)ColDesc.ColType.GetValue();
                        if (i13 != (ReportRet.ColDescList.Count) && !StrColName.Contains("Total "))
                        {
                            if (!string.IsNullOrEmpty(StrColName))
                            {
                                if (dtOpenInv.Columns.Contains(StrColName))
                                {
                                    StrColName = StrColName + "Checkforduplicate" + s;
                                    dtOpenInv.Columns.Add(StrColName);
                                    s++;
                                }
                                else
                                {
                                    dtOpenInv.Columns.Add(StrColName);
                                }
                                lstcolid.Add(count);
                                lstcolidName.Add(StrColName);
                                htxmlClass.Add(count, StrColName);
                            }
                        }
                    }
                }
                if (ReportRet.ReportData != null)
                {
                    if (ReportRet.ReportData.ORReportDataList != null)
                    {
                        for (int i16 = 0; i16 < ReportRet.ReportData.ORReportDataList.Count; i16++)
                        {
                            DataRow dr1 = dtDetails.NewRow();
                            IORReportData ORReportData = ReportRet.ReportData.ORReportDataList.GetAt(i16);
                            if (ORReportData.DataRow != null)
                            {
                                if (ORReportData.DataRow != null)
                                {
                                    if (ORReportData.DataRow.RowData != null)
                                    {
                                    }
                                    if (ORReportData.DataRow.ColDataList != null)
                                    {
                                        DataRow dr;
                                        dr = dtOpenInv.NewRow();
                                        dr["Customer"] = ORReportData.DataRow.RowData.value.GetValue();

                                        for (int i17 = 0; i17 < ORReportData.DataRow.ColDataList.Count; i17++)
                                        {
                                            IColData ColData = ORReportData.DataRow.ColDataList.GetAt(i17);
                                            if (ColData.value != null)
                                            {

                                                if (!string.IsNullOrEmpty(Convert.ToString(htxmlClass[Convert.ToString(ColData.colID.GetValue())])))
                                                {
                                                    if (strtype == "PL")
                                                    {
                                                        if (ColData.value.GetValue().ToString().Contains('-'))
                                                        {
                                                            dr[Convert.ToString(htxmlClass[Convert.ToString(ColData.colID.GetValue())])] = ColData.value.GetValue().ToString().Replace("-", "");
                                                        }
                                                        else
                                                        {
                                                            dr[Convert.ToString(htxmlClass[Convert.ToString(ColData.colID.GetValue())])] = "-" + ColData.value.GetValue().ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dr[Convert.ToString(htxmlClass[Convert.ToString(ColData.colID.GetValue())])] = ColData.value.GetValue().ToString();
                                                    }
                                                }

                                            }
                                        }
                                        dtOpenInv.Rows.Add(dr);
                                    }
                                }
                            }
                            if (ORReportData.TextRow != null)
                            {
                                if (ORReportData.TextRow != null)
                                {
                                }
                            }
                            if (ORReportData.SubtotalRow != null)
                            {
                                short strType = -1;
                                if (ORReportData.SubtotalRow != null)
                                {
                                    if (ORReportData.SubtotalRow.RowData != null)
                                    {
                                        if (ORReportData.SubtotalRow.Type != null)
                                        {
                                            strType = ORReportData.SubtotalRow.ColDataList.Type.GetValue();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return dtOpenInv;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLog("Method : WalkReportRet() " + ex.ToString());
                return dtOpenInv;
            }
        }

        public DataTable OpenInvoiceRept(DateTime toDate, string customerName)
        {
            string strQbPath = string.Empty;
            string osStatus = string.Empty;
            try
            {
                IMsgSetRequest msgSetRequest = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                msgSetRequest.Attributes.OnError = ENRqOnError.roeContinue;

                IGeneralDetailReportQuery CustomeSummaryReportQueryRq = msgSetRequest.AppendGeneralDetailReportQueryRq();
                CustomeSummaryReportQueryRq.GeneralDetailReportType.SetValue(ENGeneralDetailReportType.gdrtOpenInvoices);
                CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
                CustomeSummaryReportQueryRq.ReportEntityFilter.ORReportEntityFilter.FullNameList.Add(customerName);
                //   CustomeSummaryReportQueryRq.ReportEntityFilter.ORReportEntityFilter.
                //selva
                //CustomeSummaryReportQueryRq.DisplayReport.SetValue(true);
                //selva
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(msgSetRequest);

                if (responseMsgSet == null) return null;
                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null) return null;
                for (int i = 0; i < responseList.Count; i++)
                {
                    IResponse response = responseList.GetAt(i);

                    if (response.StatusCode >= 0)
                    {
                        if (response.Detail != null)
                        {
                            ENResponseType responseType = (ENResponseType)response.Type.GetValue();

                            IReportRet ReportRet = (IReportRet)response.Detail;

                            //string strxml = responseMsgSet.ToXMLString();
                            //File.WriteAllText("TestQB.txt", strxml);
                            return WalkReportRet(ReportRet, "");
                        }
                    }
                }

                return null;
            }
            catch (Exception Ex)
            {
                Log.WriteToErrorLog("Method:OpenInvoiceRept() " + Ex.Message);
                return null;
            }
            finally
            {
            }
        }

        #endregion NewReportQuery

        #region aging

        //public DataTable AgingData(DateTime toDate,string customername)
        //{
        //  //  statusMsg = string.Empty;
        //    string strQbPath = string.Empty;
        //    string osStatus = string.Empty;
        //    //fromDate = Convert.ToDateTime("01/01/2013");
        //    //toDate = Convert.ToDateTime("12/31/2013");
        //    try
        //    {
        //        //    if (Settings.QBConnect == "1")
        //        //        strQbPath = Settings.QBFilePath;
        //        //    if (!OpenConnection(QBApplicationName))
        //        //    {
        //        //        //statusMsg = "QuickBooks Connection not opened.";
        //        //        return false;
        //        //    }
        //        //    if (!OpenSession(strQbPath, out osStatus))
        //        //    {
        //        //        //statusMsg = "QuickBooks connection failed.";
        //        //        return false;
        //        //    }

        //        IMsgSetRequest msgSetRequest = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
        //        msgSetRequest.Attributes.OnError = ENRqOnError.roeContinue;

        //        IAgingReportQuery CustomeSummaryReportQueryRq = msgSetRequest.AppendAgingReportQueryRq();
        //        CustomeSummaryReportQueryRq.AgingReportType.SetValue(ENAgingReportType.artARAgingSummary);
        //        CustomeSummaryReportQueryRq.ReportEntityFilter.ORReportEntityFilter.FullNameList.Add(customername);
        //        //selvatoday
        //        //   CustomeSummaryReportQueryRq.SummarizeRowsBy.SetValue(ENSummarizeRowsBy.srbAccount);
        //        //Raja <start>
        //        //CustomeSummaryReportQueryRq.ReportCalendar.SetValue(ENReportCalendar.rcFiscalYear);
        //        //CustomeSummaryReportQueryRq.ReturnColumns.SetValue(ENReturnColumns.rcAll);
        //        //Raja <End>
        //        //CustomeSummaryReportQueryRq.ReportBasis.SetValue(ENReportBasis.rbCash);

        //        //if (strAction == "PL")
        //        //{
        //        if (true)
        //        {
        //            //CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(fromDate);
        //            CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
        //            // CustomeSummaryReportQueryRq.ReportEntityFilter.ORReportEntityFilter.FullNameWithChildren.SetValue("Cook, Brian");
        //            //  CustomeSummaryReportQueryRq.IncludeColumnList.Add(ENIncludeColumn. .SetValue(true);
        //        }
        //        //else
        //        //{
        //        //    if (isStartFY && !isASOF)
        //        //    {
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
        //        //        //CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(fromDate);
        //        //        //CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
        //        //        if (!isExportClass)
        //        //            CustomeSummaryReportQueryRq.IncludeSubcolumns.SetValue(true);
        //        //    }
        //        //    else
        //        //    {
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(fromDate);
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
        //        //        CustomeSummaryReportQueryRq.IncludeSubcolumns.SetValue(true);
        //        //    }
        //        //}
        //        //CustomeSummaryReportQueryRq.ReportAccountFilter.ORReportAccountFilter.AccountTypeFilter.SetValue(ENAccountTypeFilter.atfIncomeAndExpense);
        //        //CustomeSummaryReportQueryRq.ReportAccountFilter.ORReportAccountFilter.AccountTypeFilter.SetValue(ENAccountTypeFilter.atfIncomeAndOtherIncome);
        //        //}
        //        //else
        //        //{
        //        //    if (isASOF)
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(fromDate);
        //        //    else
        //        //    {
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(fromDate);
        //        //        CustomeSummaryReportQueryRq.ORReportPeriod.ReportPeriod.ToReportDate.SetValue(toDate);
        //        //    }
        //        //    CustomeSummaryReportQueryRq.ReportAccountFilter.ORReportAccountFilter.AccountTypeFilter.SetValue(ENAccountTypeFilter.atfBalanceSheet);
        //        //    CustomeSummaryReportQueryRq.IncludeSubcolumns.SetValue(true);
        //        //}
        //        //CustomeSummaryReportQueryRq.ReturnRows.SetValue(ENReturnRows.rrNonZero);
        //        //if (!isNonZeroAccount && !isAllAcc)
        //        //{
        //        //    CustomeSummaryReportQueryRq.ReturnRows.SetValue(ENReturnRows.rrActiveOnly);
        //        //}
        //        //else if (isAllAcc && !isNonZeroAccount)
        //        //{
        //        //    CustomeSummaryReportQueryRq.ReturnRows.SetValue(ENReturnRows.rrAll);
        //        //}
        //        ////else
        //        ////    CustomeSummaryReportQueryRq.ReturnRows.SetValue(ENReturnRows.rrAll);

        //        //if (isExportClass)
        //        //    CustomeSummaryReportQueryRq.SummarizeColumnsBy.SetValue(ENSummarizeColumnsBy.scbClass);
        //        //else
        //        //    CustomeSummaryReportQueryRq.SummarizeColumnsBy.SetValue(ENSummarizeColumnsBy.scbTotalOnly);

        //        //CustomeSummaryReportQueryRq.SummarizeColumnsBy.SetValue(ENSummarizeColumnsBy.scbMonth);
        //        //selva
        //        //    CustomeSummaryReportQueryRq.DisplayReport.SetValue(true);
        //        //selva
        //        IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(msgSetRequest);

        //        if (responseMsgSet == null) return null;
        //        IResponseList responseList = responseMsgSet.ResponseList;
        //        if (responseList == null) return null;
        //        for (int i = 0; i < responseList.Count; i++)
        //        {
        //            IResponse response = responseList.GetAt(i);

        //            if (response.StatusCode >= 0)
        //            {
        //                if (response.Detail != null)
        //                {
        //                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
        //                    //if (responseType == ENResponseType.rtGeneralSummaryReportQueryRs)
        //                    //{
        //                    IReportRet ReportRet = (IReportRet)response.Detail;

        //                    string strxml = responseMsgSet.ToXMLString();
        //                    File.WriteAllText("TestQB.txt", strxml);
        //                  return  WalkReportRet(ReportRet, "");
        //                    //}
        //                    //else
        //                    //{
        //                    //    dt = WalkReportRet1(ReportRet);
        //                    //}
        //                    //}
        //                }
        //            }
        //        }

        //        //IResponseList responseList = responseMsgSet.ResponseList;
        //        //IResponse response = (IResponse)responseList.GetAt(0);
        //        //string strxml = responseMsgSet.ToXMLString();
        //        //WalkReportRet(IReportRet ReportRet, string strtype)
        //        string strMyDocument = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\TBX";
        //        System.IO.StreamWriter writer = null;
        //        //if (strAction == "BS")
        //        //    writer = new System.IO.StreamWriter(Application.StartupPath + "\\TbBS.xml", false);
        //        //if (strAction == "PL")
        //        //    writer = new System.IO.StreamWriter(Application.StartupPath + "\\TbPL.xml", false);
        //        //if (strAction == "BS")
        //        //    writer = new System.IO.StreamWriter(strMyDocument + "\\TbBS.xml", false);
        //        //if (strAction == "PL")
        //        //    writer = new System.IO.StreamWriter(strMyDocument + "\\TbPL.xml", false);
        //        //writer.Write(strxml);
        //        writer.Flush();
        //        writer.Close();
        //        writer.Dispose();
        //        writer = null;

        //        return null;
        //    }
        //    catch (Exception Ex)
        //    {
        //        //statusMsg = Ex.Message;
        //        Log.WriteToErrorLog("Method:BalanceSheetXML() " + Ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        //CloseSession();
        //        //CloseConnection();
        //    }
        //}

        #endregion aging




        public InvDetails OrderItems(string invNum)
        {
            string strInvStatus = string.Empty;
            InvDetails oInvDetails = new InvDetails();

            try
            {
                IMsgSetRequest requestMsgSet = mSessionManager.CreateMsgSetRequest(Settings.QBCountry, Settings.QBMajorVer, Settings.QBMinorVer);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                IInvoiceQuery InvQueryRq = requestMsgSet.AppendInvoiceQueryRq();
                InvQueryRq.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                InvQueryRq.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(invNum);
                InvQueryRq.IncludeLineItems.SetValue(true);
                //InvQueryRq.IncludeLinkedTxns.SetValue(true);
                InvQueryRq.OwnerIDList.Add("0");
                //Qb Execute query
                IMsgSetResponse responseMsgSet = mSessionManager.DoRequests(requestMsgSet);
                if (responseMsgSet == null)
                {
                    return null;
                }

                IResponseList responseList = responseMsgSet.ResponseList;
                if (responseList == null) { }//return null;

                IResponse response = (IResponse)responseList.GetAt(0);

                if (response.StatusCode < 0) { }

                if (response.Detail != null)
                {
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtInvoiceQueryRs)
                    {
                        IInvoiceRetList InvoiceRet = (IInvoiceRetList)response.Detail;

                        oInvDetails.oLstOrderLineItem = new List<OrderLineItem>();
                        for (int i = 0; i < InvoiceRet.Count; i++)
                        {
                            IInvoiceRet oInvRet = InvoiceRet.GetAt(i);
                            if (oInvRet!=null)
                            {
                                if (oInvRet.RefNumber.GetValue() != invNum)
                                    continue;
                                oInvDetails.InvNum = oInvRet.RefNumber.GetValue();
                                oInvDetails.CustomerName = oInvRet.CustomerRef.FullName.GetValue();
                                oInvDetails.InvDate = oInvRet.TxnDate.GetValue();
                                oInvDetails.InvNum = oInvRet.RefNumber.GetValue();
                                oInvDetails.Tax = oInvRet.ItemSalesTaxRef != null ? oInvRet.ItemSalesTaxRef.FullName.GetValue() : "";
                                oInvDetails.InvAmount = oInvRet.Subtotal.GetValue();
                                oInvDetails.taxAmount = oInvRet.SalesTaxTotal != null ? oInvRet.SalesTaxTotal.GetValue() : 0;
                                oInvDetails.Terms = oInvRet.TermsRef != null ? oInvRet.TermsRef.FullName.GetValue() : "";
                                if (oInvRet.ShipDate != null)
                                    oInvDetails.ShipDate = oInvRet.ShipDate.GetValue();
                                if (oInvRet.DueDate != null)
                                    oInvDetails.DueDate = oInvRet.DueDate.GetValue();
                                if (oInvRet.PONumber != null)
                                    oInvDetails.poNum = oInvRet.PONumber.GetValue();
                                oInvDetails.Rep = oInvRet.SalesRepRef != null ? oInvRet.SalesRepRef.FullName.GetValue() : "";
                                oInvDetails.ShippingMethod = oInvRet.ShipMethodRef != null ? oInvRet.ShipMethodRef.FullName.GetValue() : "";
                                oInvDetails.FOB = oInvRet.FOB != null ? oInvRet.FOB.GetValue() : "";
                                oInvDetails.Other = oInvRet.Other != null ? oInvRet.Other.GetValue() : "";

                                #region CustomerAddress

                                oInvDetails.oCustAddress = new List<CustAddress>();
                                CustAddress oCustAddress;
                                if (oInvRet.BillAddress != null)
                                {
                                    oCustAddress = new CustAddress();
                                    oCustAddress.Type = "BillTo";
                                    oCustAddress.Addr1 = oInvRet.BillAddress.Addr1 != null ? oInvRet.BillAddress.Addr1.GetValue() : "";
                                    oCustAddress.Addr2 = oInvRet.BillAddress.Addr2 != null ? oInvRet.BillAddress.Addr2.GetValue() : "";
                                    oCustAddress.Addr3 = oInvRet.BillAddress.Addr3 != null ? oInvRet.BillAddress.Addr3.GetValue() : "";
                                    oCustAddress.Addr4 = oInvRet.BillAddress.Addr4 != null ? oInvRet.BillAddress.Addr4.GetValue() : "";
                                    oCustAddress.Addr5 = oInvRet.BillAddress.Addr5 != null ? oInvRet.BillAddress.Addr5.GetValue() : "";
                                    oCustAddress.City = oInvRet.BillAddress.City != null ? oInvRet.BillAddress.City.GetValue() : "";
                                    oCustAddress.State = oInvRet.BillAddress.State != null ? oInvRet.BillAddress.State.GetValue() : "";
                                    oCustAddress.Country = oInvRet.BillAddress.Country != null ? oInvRet.BillAddress.Country.GetValue() : "";
                                    oCustAddress.PostalCode = oInvRet.BillAddress.PostalCode != null ? oInvRet.BillAddress.PostalCode.GetValue() : "";
                                    oInvDetails.oCustAddress.Add(oCustAddress);
                                }

                                //if (oInvRet.ShipAddress != null)
                                //{
                                //    oCustAddress = new CustAddress();
                                //    oCustAddress.Type = "ShipToParty";
                                //    oCustAddress.Addr1 = oInvRet.ShipAddress.Addr1 != null ? oInvRet.ShipAddress.Addr1.GetValue() : "";
                                //    oCustAddress.Addr2 = oInvRet.ShipAddress.Addr2 != null ? oInvRet.ShipAddress.Addr2.GetValue() : "";
                                //    oCustAddress.Addr3 = oInvRet.ShipAddress.Addr3 != null ? oInvRet.ShipAddress.Addr3.GetValue() : "";
                                //    oCustAddress.Addr4 = oInvRet.ShipAddress.Addr4 != null ? oInvRet.ShipAddress.Addr4.GetValue() : "";
                                //    oCustAddress.Addr5 = oInvRet.ShipAddress.Addr5 != null ? oInvRet.ShipAddress.Addr5.GetValue() : "";
                                //    oCustAddress.City = oInvRet.ShipAddress.City != null ? oInvRet.ShipAddress.City.GetValue() : "";
                                //    oCustAddress.State = oInvRet.ShipAddress.State != null ? oInvRet.ShipAddress.State.GetValue() : "";
                                //    oCustAddress.Country = oInvRet.ShipAddress.Country != null ? oInvRet.ShipAddress.Country.GetValue() : "";
                                //    oCustAddress.PostalCode = oInvRet.ShipAddress.PostalCode != null ? oInvRet.ShipAddress.PostalCode.GetValue() : "";
                                //    oInvDetails.oCustAddress.Add(oCustAddress);
                                //}

                                #endregion CustomerAddress

                                for (int j = 0; j < oInvRet.ORInvoiceLineRetList.Count; j++)
                                {
                                    OrderLineItem oOrderLineItem = new OrderLineItem();
                                    IORInvoiceLineRet oSalesOrderLineRet = oInvRet.ORInvoiceLineRetList.GetAt(j);
                                    if (oSalesOrderLineRet.InvoiceLineRet != null && oSalesOrderLineRet.InvoiceLineRet.ItemRef != null && !string.IsNullOrEmpty(oSalesOrderLineRet.InvoiceLineRet.ItemRef.FullName.GetValue()) && oSalesOrderLineRet.InvoiceLineRet.ORRate != null && oSalesOrderLineRet.InvoiceLineRet.ORRate.Rate != null)
                                    {
                                        if (oSalesOrderLineRet.InvoiceLineRet.DataExtRetList != null)
                                        {


                                            for (int k = 0; k < oSalesOrderLineRet.InvoiceLineRet.DataExtRetList.Count; k++)
                                            {
                                                IDataExtRet DataExtRet = oSalesOrderLineRet.InvoiceLineRet.DataExtRetList.GetAt(k);
                                                if (Convert.ToString(DataExtRet.DataExtName.GetValue()) == "Boxes")
                                                {
                                                    if (DataExtRet.DataExtValue != null)
                                                        oOrderLineItem.Box = DataExtRet.DataExtValue.GetValue();

                                                }
                                            }
                                        }


                                        oOrderLineItem.ItemSKU = oSalesOrderLineRet.InvoiceLineRet.ItemRef.FullName.GetValue();
                                        oOrderLineItem.ItemDesc = oSalesOrderLineRet.InvoiceLineRet.Desc != null ? oSalesOrderLineRet.InvoiceLineRet.Desc.GetValue() : "";

                                        oOrderLineItem.Quanity = Convert.ToString(oSalesOrderLineRet.InvoiceLineRet.Quantity != null ? oSalesOrderLineRet.InvoiceLineRet.Quantity.GetValue() : 1);
                                        oOrderLineItem.TaxItem = Convert.ToString(oSalesOrderLineRet.InvoiceLineRet.SalesTaxCodeRef != null ? oSalesOrderLineRet.InvoiceLineRet.SalesTaxCodeRef.FullName.GetValue() : "");
                                        oOrderLineItem.TaxAmount = 0;// QB doesnot give tax for each item . It is applied as whole
                                        oOrderLineItem.UOM = oSalesOrderLineRet.InvoiceLineRet.UnitOfMeasure != null ? oSalesOrderLineRet.InvoiceLineRet.UnitOfMeasure.GetValue() : "";
                                        oOrderLineItem.Price = Convert.ToString(oSalesOrderLineRet.InvoiceLineRet.ORRate.Rate.GetValue());
                                        if (oSalesOrderLineRet.InvoiceLineRet.ServiceDate != null)
                                            oOrderLineItem.ServiceDate = oSalesOrderLineRet.InvoiceLineRet.ServiceDate.GetValue();
                                        oOrderLineItem.Other1 = oSalesOrderLineRet.InvoiceLineRet.Other1 != null ? oSalesOrderLineRet.InvoiceLineRet.Other1.GetValue() : "";
                                        oOrderLineItem.Other2 = oSalesOrderLineRet.InvoiceLineRet.Other2 != null ? oSalesOrderLineRet.InvoiceLineRet.Other2.GetValue() : "";
                                        oInvDetails.oLstOrderLineItem.Add(oOrderLineItem);
                                    }
                                }
                            }
                        }
                    }
                }
                return oInvDetails;
            }
            catch (Exception ex)
            {
                //  Log.WriteToErrorLog(ex.StackTrace.ToString() + Environment.NewLine + ex.Message, "QBLayer", "OrderItems");
                return null;
            }
        }
    }
}