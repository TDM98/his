using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.InPatient.Reports;
using eHCMS.ReportLib.RptConsultations.XtraReports;
using eHCMS.ReportLib.RptConsultations.XtraReports.MedicalFiles;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptChronicOutPtMedicalFile : XtraReport
    {
        public XRptChronicOutPtMedicalFile()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptChronicOutPtMedicalFileTableAdapter.Fill(dsChronicOutPtMedicalFile1.spRptChronicOutPtMedicalFile, PtRegDetailID.Value as long?, null, null, OutPtTreatmentProgramID.Value as long?);
            spRptGeneralOutPtMedicalFileFilmsRecvTableAdapter.Fill(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv, PtRegDetailID.Value as long?, OutPtTreatmentProgramID.Value as long?);
            if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv != null && dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 0)
            {
                int TotalFilmsRecv = 0;
                if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 0)
                {
                    txtFilmsRecvName1.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["PCLSubCategoryDescription"].ToString();
                    if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"] != null &&
                        dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"] != DBNull.Value &&
                        !string.IsNullOrEmpty(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"].ToString()))
                    {
                        txtFilmsRecvValue1.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"].ToString();
                        TotalFilmsRecv += Convert.ToInt32(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[0]["DefaultNumFilmsRecv"]);
                    }
                }
                if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 1)
                {
                    txtFilmsRecvName2.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["PCLSubCategoryDescription"].ToString();
                    if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"] != null &&
                        dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"] != DBNull.Value &&
                        !string.IsNullOrEmpty(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"].ToString()))
                    {
                        txtFilmsRecvValue2.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"].ToString();
                        TotalFilmsRecv += Convert.ToInt32(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[1]["DefaultNumFilmsRecv"]);
                    }
                }
                if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 2)
                {
                    txtFilmsRecvName3.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["PCLSubCategoryDescription"].ToString();
                    if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"] != null &&
                        dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"] != DBNull.Value &&
                        !string.IsNullOrEmpty(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"].ToString()))
                    {
                        txtFilmsRecvValue3.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"].ToString();
                        TotalFilmsRecv += Convert.ToInt32(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[2]["DefaultNumFilmsRecv"]);
                    }
                }
                if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows.Count > 3)
                {
                    txtFilmsRecvName4.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["PCLSubCategoryDescription"].ToString();
                    if (dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"] != null &&
                        dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"] != DBNull.Value &&
                        !string.IsNullOrEmpty(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"].ToString()))
                    {
                        txtFilmsRecvValue4.Text = dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"].ToString();
                        TotalFilmsRecv += Convert.ToInt32(dsGeneralOutPtMedicalFile1.spRptGeneralOutPtMedicalFileFilmsRecv.Rows[3]["DefaultNumFilmsRecv"]);
                    }
                }
                txtFilmsRecvTotalValue.Text = TotalFilmsRecv.ToString();
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID.Value as long?;
            ((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["PtRegDetailID"].Value = PtRegDetailID.Value as long?;
            //((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = null;
            //((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["V_RegistrationType"].Value = null;
            ((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
            ((XRptChronicOutPtMedicalFileFrontCover)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptSelfDeclaration)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = (long)PtRegistrationID.Value;
            ((XRptSelfDeclaration)((XRSubreport)sender).ReportSource).Parameters["V_RegistrationType"].Value = (long)V_RegistrationType.Value;
            ((XRptSelfDeclaration)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
            ((XRptSelfDeclaration)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_AdmissionExaminationForChronic)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = (long)PtRegistrationID.Value;
            ((XRpt_AdmissionExaminationForChronic)((XRSubreport)sender).ReportSource).Parameters["V_RegistrationType"].Value = (long)V_RegistrationType.Value;
            ((XRpt_AdmissionExaminationForChronic)((XRSubreport)sender).ReportSource).Parameters["parHospitalCode"].Value = parHospitalCode.Value.ToString();
            ((XRpt_AdmissionExaminationForChronic)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
            ((XRpt_AdmissionExaminationForChronic)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }

        private void xrSubreport6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PhieuChamSocForChronic)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = OutPtTreatmentProgramID.Value as long?;
            ((XRpt_PhieuChamSocForChronic)((XRSubreport)sender).ReportSource).Parameters["V_RegistrationType"].Value = (long)V_RegistrationType.Value;
            ((XRpt_PhieuChamSocForChronic)((XRSubreport)sender).ReportSource).Parameters["IsChronic"].Value = true;
            ((XRpt_PhieuChamSocForChronic)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
            ((XRpt_PhieuChamSocForChronic)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }

        private void xrSubreport7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptToDieuTriNgoaiTru_Summary_ForChronic)((XRSubreport)sender).ReportSource).Parameters["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID.Value as long?;
            ((XRptToDieuTriNgoaiTru_Summary_ForChronic)((XRSubreport)sender).ReportSource).Parameters["parHospitalName"].Value = parHospitalName.Value.ToString();
            ((XRptToDieuTriNgoaiTru_Summary_ForChronic)((XRSubreport)sender).ReportSource).Parameters["parDepartmentOfHealth"].Value = parDepartmentOfHealth.Value.ToString();
        }
    }
}
