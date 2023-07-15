using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPMR : DevExpress.XtraReports.UI.XtraReport
    {

        public XRptPMR()
        {
            InitializeComponent();
            FillData();  
        }

        public void FillData()
        {                        
            this.spRpt_PMR_VitalSignTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_MedicalCondTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_PastMedicalCondTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_ImmunizationsTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_FamilyHistoryTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_DiagTrtmtTblAdapter.ClearBeforeFill = true;
            this.spRpt_PMR_HeaderInfoTblAdapter.ClearBeforeFill = true;

            this.spRpt_PMR_HeaderInfoTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_HeaderInfo, this.parPatientID.Value as long?);
            this.spRpt_PMR_VitalSignTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_VitalSign, this.parPatientID.Value as long?);
            this.spRpt_PMR_MedicalCondTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_MedicalCond, this.parPatientID.Value as long?);
            this.spRpt_PMR_PastMedicalCondTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_PastMedicalCond, this.parPatientID.Value as long?);
            this.spRpt_PMR_ImmunizationsTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_Immunizations, this.parPatientID.Value as long?);
            this.spRpt_PMR_FamilyHistoryTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_FamilyHistory, this.parPatientID.Value as long?);
            this.spRpt_PMR_DiagTrtmtTblAdapter.Fill(this.dsPMRDataSet.spRpt_PMR_DiagTrtmt, this.parPatientID.Value as long?);

        }

        private void XRptPMR_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();  
        }
        
    }
}
