using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayBaoTu : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayBaoTu()
        {
            InitializeComponent();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt64(PtRegistrationID.Value);
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalNameShort.Value = parHospitalNameShort.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt64(PtRegistrationID.Value);
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalNameShort.Value = parHospitalNameShort.Value;
            ((XRpt_GiayBaoTuSubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
        }
    }
}
