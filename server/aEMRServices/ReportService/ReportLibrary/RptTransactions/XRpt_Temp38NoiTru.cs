using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp38NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp38NoiTru()
        {
            InitializeComponent();
          //  FillDataInit();
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
            e.Result = Math.Round(sumTotalAmount, MidpointRounding.AwayFromZero) - Math.Round(sumHIRebate, MidpointRounding.AwayFromZero);
            e.Handled = true;
        }

        //public void FillDataInit()
        //{
        //    spRpt_CreateTemp38NoiTruTableAdapter.Fill(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru,0,null,null,null);
        //    spHealthInsuranceHistory_InPtTableAdapter.Fill(dsTemp38NoiTruNew1.spHealthInsuranceHistory_InPt, 0, 0,1);
        //}
        public void FillData()
        {
            spRpt_CreateTemp38NoiTruTableAdapter.Fill(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru, Convert.ToInt64(this.PtRegistrationID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.DeptID.Value), Convert.ToBoolean(this.ViewByDate.Value));
            spHealthInsuranceHistory_InPtTableAdapter.Fill(dsTemp38NoiTruNew1.spHealthInsuranceHistory_InPt, Convert.ToInt64(this.PtRegistrationID.Value), Convert.ToDateTime(this.DischargeDate.Value));

            decimal totalCP = 0;
            decimal totalBNTra = 0;
            decimal totalBHTra = 0;
            if (dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru != null && dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows.Count > 0)
            {
                //for (int i = 0; i < dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows.Count; i++)
                //{
                //    totalCP += Convert.ToDecimal(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows[i]["TotalAmount"]);
                //    totalBNTra += Convert.ToDecimal(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows[i]["Amount"]);
                //    totalBHTra += Convert.ToDecimal(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows[i]["HealthInsuranceRebate"]);
                //}

                for (int i = 0; i < dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows.Count; i++)
                {
                    totalCP += Convert.ToDecimal(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows[i]["TotalAmount"]);
                    totalBHTra += Convert.ToDecimal(dsTemp38NoiTruNew1.spRpt_CreateTemp38NoiTru.Rows[i]["HealthInsuranceRebate"]);
                }

                totalCP = Math.Round(totalCP, MidpointRounding.AwayFromZero);
                totalBHTra = Math.Round(totalBHTra, MidpointRounding.AwayFromZero);
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
                string prefix2 = "";
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

        private void XRpt_Temp38NoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}