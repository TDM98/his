using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPhieuTheoDoiDichTruyen : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuTheoDoiDichTruyen()
        {
            InitializeComponent();
        }

        private void XRpt_AdmissionExamination_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRptPhieuTheoDoiDichTruyen1.EnforceConstraints = false;
            DateTime? mFromDate = null;
            DateTime? mToDate = null;
            if (FromDate.Value != null && Convert.ToDateTime(FromDate.Value) > DateTime.MinValue)
            {
                mFromDate = FromDate.Value as DateTime?;
            }
            if (ToDate.Value != null && Convert.ToDateTime(ToDate.Value) > DateTime.MinValue)
            {
                mToDate = ToDate.Value as DateTime?;
            }
            spXRptPhieuTheoDoiDichTruyenTableAdapter.Fill(dsXRptPhieuTheoDoiDichTruyen1.spXRptPhieuTheoDoiDichTruyen, 
                Convert.ToInt64(PtRegistrationID.Value), 
                Convert.ToInt64(IntPtDiagDrInstructionID.Value),
                mFromDate,
                mToDate
                );
        }
    }
}
