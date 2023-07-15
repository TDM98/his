using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class BaoCaoChiTietThanhToan : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoChiTietThanhToan()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCaoChiTietThanhToan1.EnforceConstraints = false;
            spRpt_BaoCaoChiTietThanhToanTableAdapter.Fill(baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan,Convert.ToInt64(this.PtRegistrationID.Value), Convert.ToInt32(this.FindPatient.Value));
            decimal total = 0;
            if (baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan != null && baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows.Count > 0)
            {
                for (int i = 0; i < baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows.Count; i++)
                {
                    decimal Amount = 0;
                    decimal HealthInsuranceRebate = 0;

                    if (baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows[i]["Amount"]!=DBNull.Value)
                    {
                        decimal.TryParse(baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows[i]["Amount"].ToString(), out Amount);
                    }
                    if (baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows[i]["HealthInsuranceRebate"]!=DBNull.Value)
                    {
                        decimal.TryParse(baoCaoChiTietThanhToan1.spRpt_BaoCaoChiTietThanhToan.Rows[i]["HealthInsuranceRebate"].ToString(),out HealthInsuranceRebate);
                    }
                    
                    total += Amount - HealthInsuranceRebate;
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - total;
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }

                else
                {
                    temp = total;
                    prefix = "";
                }
                this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }
        private void BaoCaoChiTietThanhToan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
