using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsRpt_ChungNhanThuongTich_NgoaiTru_NoiTru1.EnforceConstraints = false;
            spGetInjuryCertificates_ByIDTableAdapter.Fill(dsRpt_ChungNhanThuongTich_NgoaiTru_NoiTru1.spGetInjuryCertificates_ByID,
                Convert.ToInt64(InjuryCertificateID.Value),
                Convert.ToInt64(V_RegistrationType.Value));
        }

        private void XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
