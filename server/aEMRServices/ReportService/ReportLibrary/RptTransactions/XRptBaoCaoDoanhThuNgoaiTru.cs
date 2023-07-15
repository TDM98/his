using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoDoanhThuNgoaiTru : XtraReport
    {
        public XRptBaoCaoDoanhThuNgoaiTru()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoDoanhThuNgoaiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsKT_BaoCaoDoanhThuNgoai_NoiTru1.EnforceConstraints = false;
            spRpt_KT_BaoCaoDoanhThuNgoai_NoiTruTableAdapter.Fill(dsKT_BaoCaoDoanhThuNgoai_NoiTru1.spRpt_KT_BaoCaoDoanhThuNgoai_NoiTru, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt16(parFindPatient.Value), Convert.ToInt64(PatientClassID.Value));

            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "BÁO CÁO DOANH THU NỘI TRÚ";
                xrLabel29.Text = "BÁO CÁO DOANH THU NỘI TRÚ";
            } else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "BÁO CÁO DOANH THU NGOẠI TRÚ";
                xrLabel29.Text = "BÁO CÁO DOANH THU NGOẠI TRÚ";
            }
            else
            {
                xrLabel15.Text = "BÁO CÁO DOANH THU NỘI-NGOẠI TRÚ";
                xrLabel29.Text = "BÁO CÁO DOANH THU NỘI-NGOẠI TRÚ";
            }
        }
    }
}
