using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPhieuCungCapMau : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuCungCapMau()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPhieuCungCapMau1.EnforceConstraints = false;
            spRptPhieuCungCapMauTableAdapter.Fill(dsPhieuCungCapMau1.spRptPhieuCungCapMau, parPatientCode.Value.ToString(), Convert.ToInt64(parIntPtDiagDrInstructionID.Value));
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void XtraReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
