using System;
using eHCMS.Services.Core;
using DevExpress.XtraReports.UI;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp38NgoaiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp38NgoaiTru()
        {
            InitializeComponent();

            //FillData();
            //this.ParametersRequestSubmit+=new EventHandler<ParametersRequestEventArgs>(XRpt_Temp38NgoaiTru_ParametersRequestSubmit);
            //this.ParametersRequestBeforeShow+=new EventHandler<ParametersRequestEventArgs>(XRpt_Temp38NgoaiTru_ParametersRequestBeforeShow);
        }

        decimal sumTotalAmount = 0;
        decimal sumHIRebate = 0;

        //KMx: Mỗi khi chuyển group (dịch vụ, thuốc, pcl) thì gọi hàm này.
        private void CellPtPayment_SummaryReset(object sender, EventArgs e)
        {
            // Reset the result each time a group is printed. 
            sumTotalAmount = 0;
            sumHIRebate = 0;
        }

        //KMx: Khi load mỗi dòng trong group thì gọi hàm này.
        private void CellPtPayment_SummaryRowChanged(object sender, EventArgs e)
        {
            // Calculate a summary. 
            sumTotalAmount += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("TotalAmount"));
            sumHIRebate += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
        }

        //KMx: Khi load hết các dòng trong 1 group thì gọi hàm này.
        private void CellPtPayment_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        { 
            e.Result = Math.Round(sumTotalAmount) - Math.Round(sumHIRebate);
            e.Handled = true;
        }

        public void FillData()
        {
            spRpt_Temp38TableAdapter.Fill((this.DataSource as DataSchema.dsTemp38New).spRpt_Temp38, Convert.ToInt64(this.TransactionID.Value), Convert.ToInt64(this.PtRegistrationID.Value), Convert.ToInt32(this.parTypeOfForm.Value));
            spHealthInsuranceHistory_ByTranIDTableAdapter.Fill((this.DataSource as DataSchema.dsTemp38New).spHealthInsuranceHistory_ByTranID, Convert.ToInt64(this.TransactionID.Value), Convert.ToInt64(this.PtRegistrationID.Value), 0);
            spRpt_CreateTemp38CheckTableAdapter.Fill(dsTemp38New1.spRpt_CreateTemp38Check, Convert.ToInt64(this.TransactionID.Value), Convert.ToInt64(this.PtRegistrationID.Value));
            decimal totalCP = 0;
            decimal totalBNTra = 0;
            decimal totalBHTra = 0;
            if (dsTemp38New1.spRpt_Temp38 != null && dsTemp38New1.spRpt_Temp38.Rows.Count > 0)
            {
                //for (int i = 0; i < dsTemp38New1.spRpt_Temp38.Rows.Count; i++)
                //{
                //    totalCP += Convert.ToDecimal(dsTemp38New1.spRpt_Temp38.Rows[i]["TotalAmount"]);
                //    totalBNTra += Convert.ToDecimal(dsTemp38New1.spRpt_Temp38.Rows[i]["Amount"]);
                //    totalBHTra += Convert.ToDecimal(dsTemp38New1.spRpt_Temp38.Rows[i]["HealthInsuranceRebate"]);
                //}

                for (int i = 0; i < dsTemp38New1.spRpt_Temp38.Rows.Count; i++)
                {
                    totalCP += Convert.ToDecimal(dsTemp38New1.spRpt_Temp38.Rows[i]["TotalAmount"]);
                    totalBHTra += Convert.ToDecimal(dsTemp38New1.spRpt_Temp38.Rows[i]["HealthInsuranceRebate"]);
                }

                totalCP = Math.Round(totalCP);
                totalBHTra = Math.Round(totalBHTra);
                totalBNTra = totalCP - totalBHTra;

                this.Parameters["TotalAmount"].Value = totalCP;
                this.Parameters["TotalHIPayment"].Value = totalBHTra;
                this.Parameters["TotalPatientPayment"].Value = totalBNTra;

                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (totalCP < 0)
                {
                    temp1 = 0 - totalCP;
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp1 = totalCP;
                    prefix1 = "";
                }
                this.Parameters["ReadMoneyTongCP"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
                decimal temp2 = 0;
                string prefix2= "";
                if (totalBNTra < 0)
                {
                    temp2 = 0 - totalBNTra;
                    prefix2 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp2 = totalBNTra;
                    prefix2 = "";
                }
                this.Parameters["ReadMoneyBNTra"].Value = prefix2 + converter.Convert(temp2.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
                decimal temp3 = 0;
                string prefix3 = "";
                if (totalBNTra < 0)
                {
                    temp3 = 0 - totalBHTra;
                    prefix3 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp3 = totalBHTra;
                    prefix3 = "";
                }
                this.Parameters["ReadMoneyBHTra"].Value = prefix3 + converter.Convert(temp3.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRpt_Temp38NgoaiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        //private void XRpt_Temp38NgoaiTru_ParametersRequestSubmit(object sender, DevExpress.XtraReports.Parameters.ParametersRequestEventArgs e)
        //{
        //    FillData();
        //}

        //private void XRpt_Temp38NgoaiTru_ParametersRequestBeforeShow(object sender, DevExpress.XtraReports.Parameters.ParametersRequestEventArgs e)
        //{
        //    //foreach (ParameterInfo info in e.ParametersInformation)
        //    //{
        //    //    DataTable table = new DataTable();
        //    //    table.Columns.Add("Dosage", typeof(int));
        //    //    table.Columns.Add("Drug", typeof(string));
        //    //    table.Columns.Add("Patient", typeof(string));
        //    //    table.Columns.Add("Date", typeof(DateTime));

        //    //    //
        //    //    // Here we add five DataRows.
        //    //    //
        //    //    table.Rows.Add(25, "Indocin", "David", DateTime.Now);
        //    //    table.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
        //    //    table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
        //    //    table.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
        //    //    table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);

        //    //    if (info.Parameter.Name == "TransactionID")
        //    //    {
        //    //        LookUpEdit lookUpEdit = new LookUpEdit();
        //    //        lookUpEdit.Properties.DataSource = table;
        //    //        lookUpEdit.Properties.DisplayMember = "Drug";
        //    //        lookUpEdit.Properties.ValueMember = "Dosage";
        //    //        lookUpEdit.Properties.Columns.Add(new LookUpColumnInfo("Drug", 0, "Category Name"));
        //    //        info.Editor = lookUpEdit;

        //    //    }
        //    //}

        //}

        //private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        //{
        //    ((SubTemp38NgoaiTruCheck)((XRSubreport)sender).ReportSource).parTransactionID.Value = this.TransactionID.Value;
        //    ((SubTemp38NgoaiTruCheck)((XRSubreport)sender).ReportSource).parPtRegistrationID.Value = this.PtRegistrationID.Value;
        //}
    }
}
