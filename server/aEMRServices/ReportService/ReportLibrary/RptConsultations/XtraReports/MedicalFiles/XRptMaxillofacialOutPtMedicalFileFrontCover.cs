using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMaxillofacialOutPtMedicalFileFrontCover : XtraReport
    {
        public XRptMaxillofacialOutPtMedicalFileFrontCover()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptMaxillofacialOutPtMedicalFileTableAdapter.Fill(dsMaxillofacialOutPtMedicalFileFrontCover1.spRptMaxillofacialOutPtMedicalFile, PtRegDetailID.Value as long?, null, null, OutPtTreatmentProgramID.Value as long?);
            //spRptGeneralOutPtMedicalFileFilmsRecvTableAdapter.Fill(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv, PtRegDetailID.Value as long?, OutPtTreatmentProgramID.Value as long?);

        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
              ((XRptToDieuTriNgoaiTru_Summary)((XRSubreport)sender).ReportSource).Parameters["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID.Value as long?;
              ((XRptToDieuTriNgoaiTru_Summary)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
              ((XRptToDieuTriNgoaiTru_Summary)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }
    }
}
