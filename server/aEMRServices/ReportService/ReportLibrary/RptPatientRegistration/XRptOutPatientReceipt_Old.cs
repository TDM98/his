using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceipt_Old : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceipt_Old()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt_Old HAM KHOI TAO");  

            InitializeComponent();

            BeforePrint += XRptPatientPayment_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }
        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            AxLogger.Instance.LogInfo("RptPatientRegistration FillData Starting");  

            try
            {
                this.eHCMS_DB_DEVDataSet1.EnforceConstraints = false;

                sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill = true;
                var paymentID = Parameters["param_PaymentID"].Value as int?;
                if (!paymentID.HasValue || paymentID.Value <= 0)
                {
                    AxLogger.Instance.LogInfo("[ERROR PaymentID]");  
                    return;
                }
                //Test
                var printingMode = Parameters["param_ReceiptForEachLocationPrintingMode"].Value as int?;
                var bPrintReceiptForEachLocation = printingMode.HasValue && printingMode.Value == 1;
                if (bPrintReceiptForEachLocation)
                {
                    var serviceItemIDList = GetIDListFromString(Parameters["param_ServiceItemIDString"].Value.ToString());
                    var serviceItemIDString = GetIDListString(serviceItemIDList);

                    sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Adapter.SelectCommand.CommandText = "sp_Rpt_spReportOutPatientCashReceipt_ForEachLocation";

                    var param_ServiceItems = new SqlParameter("@ServiceItemIDString", SqlDbType.Xml);
                    if (string.IsNullOrWhiteSpace(serviceItemIDString))
                    {
                        param_ServiceItems.Value = DBNull.Value;
                    }
                    else
                    {
                        param_ServiceItems.Value = serviceItemIDString;
                    }
                    sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Adapter.SelectCommand.Parameters.Add(param_ServiceItems);

                    var pclItemIDList = GetIDListFromString(Parameters["param_PclItemIDString"].Value.ToString());
                    var pclItemIDString = GetIDListString(pclItemIDList);

                    var param_PclItems = new SqlParameter("@PclItemIDString", SqlDbType.Xml);
                    if (string.IsNullOrWhiteSpace(pclItemIDString))
                    {
                        param_PclItems.Value = DBNull.Value;
                    }
                    else
                    {
                        param_PclItems.Value = pclItemIDString;
                    }

                    sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Adapter.SelectCommand.Parameters.Add(param_PclItems);
                }

                //

                sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Fill(eHCMS_DB_DEVDataSet1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID, paymentID);
                if (eHCMS_DB_DEVDataSet1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID == null || eHCMS_DB_DEVDataSet1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows.Count <= 0)
                    return;

                var firstRow = eHCMS_DB_DEVDataSet1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0];
                Parameters["param_PatientCode"].Value = firstRow["PatientCode"];
                Parameters["param_ReceiptNo"].Value = firstRow["ReceiptNumber"];
                Parameters["param_CashierName"].Value = firstRow["StaffName"];

                var patientName = firstRow["FullName"].ToString();
                Parameters["param_PatientNameOnly"].Value = patientName;
                var yearOfBirth = firstRow["YearOfBirth"].ToString();
                var age = firstRow["Age"].ToString();
                var gender = firstRow["Gender"].ToString();

                Parameters["param_PatientName"].Value = string.Format("{0} - {1} ({2} t) - {3}", patientName, yearOfBirth, age, gender);

                string patientAddress = firstRow["PatientStreetAddress"].ToString();
                string patientSurburb = firstRow["PatientSurburb"].ToString();
                if (!string.IsNullOrWhiteSpace(patientSurburb))
                {
                    patientAddress += ", " + patientSurburb;
                }
                Parameters["param_PatientAddress"].Value = patientAddress;


                //var sTemp = string.Empty;
                var sKCB = string.Empty;
                var sCLS = string.Empty;
                decimal amount = 0;
                foreach (DataRow row in eHCMS_DB_DEVDataSet1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows)
                {
                    var type = (long)row["V_ServiceItemType"];
                    if (type == (long)AllLookupValues.ServiceItemType.CHI_TIET_KCB)
                    {
                        if (sKCB.Length > 0)
                        {
                            sKCB += ",";
                        }
                        //sKCB += string.Format("{0} [{1}]", row["ServiceName"], ((decimal)row["Amount"]).ToString("N0"));
                        sKCB += string.Format("{0} [{1}]", row["ServiceName"], ((decimal)row["PatientAmount"]).ToString("N0"));
                    }
                    else if (type == (long)AllLookupValues.ServiceItemType.CHI_TIET_CLS)
                    {
                        if (sCLS.Length > 0)
                        {
                            sCLS += ",";
                        }
                        //sCLS += string.Format("{0} [{1}]", row["ServiceName"], ((decimal)row["Amount"]).ToString("N0"));
                        sCLS += string.Format("{0} [{1}]", row["ServiceName"], ((decimal)row["PatientAmount"]).ToString("N0"));
                    }

                    if (bPrintReceiptForEachLocation)
                    {
                        //amount += (decimal)row["Amount"];
                        amount += (decimal)row["PatientAmount"];
                    }
                }

                var strServices = string.Empty;
                if (sKCB.Length > 0)
                {
                    sKCB = string.Format("KCB({0})", sKCB);
                }
                if (sCLS.Length > 0)
                {
                    sCLS = string.Format("CLS({0})", sCLS);
                }

                strServices = string.Join(",", new string[] { sKCB, sCLS });

                const int maxLength = 256;
                if (!string.IsNullOrWhiteSpace(strServices) && strServices.Length > maxLength)
                {
                    strServices = strServices.Substring(0, maxLength - 4) + " ...";
                }

                if (!bPrintReceiptForEachLocation)
                {
                    amount = (decimal)firstRow["PayAmount"];
                }
                Parameters["param_Services"].Value = strServices;
                Parameters["param_Amount"].Value = amount;

                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                var converter = new NumberToLetterConverter();
                Parameters["param_AmountInWords"].Value = converter.Convert(amount.ToString("0.00"), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND);
                AxLogger.Instance.LogInfo("RptPatientRegistration FillData SUCCESS");  
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("RptPatientRegistration FillData ERROR" + ex.ToString());  
            }
        }

        private List<long> GetIDListFromString(string sInput)
        {
            //sInput co dang: a,b,c,d
            if(string.IsNullOrWhiteSpace(sInput))
            {
                return null;
            }
            var arr = sInput.Split(new[]{','});
            return arr.Select(long.Parse).ToList();
        }

        private string GetIDListString(IEnumerable<long> items)
        {
            if(items == null)
            {
                return  null;
            }
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                  new XElement("Root",
                                  from item in items
                                  select new XElement("IDList",
                                  new XElement("ID", item))));

            return xmlDocument.ToString();
        }
    }
}
