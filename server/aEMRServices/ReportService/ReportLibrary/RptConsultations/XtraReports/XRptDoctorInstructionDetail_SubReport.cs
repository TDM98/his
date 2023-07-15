using System;
using System.Data;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstructionDetail_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstructionDetail_SubReport()
        {
            InitializeComponent();
        }

        private void VisibleContent(bool[] VisibleArray)
        {
            DetailReport.Visible = VisibleArray.Length > 0 && VisibleArray[0];
            DetailReport1.Visible = VisibleArray.Length > 1 && VisibleArray[1];
            DetailReport2.Visible = VisibleArray.Length > 2 && VisibleArray[2];
            DetailReport4.Visible = VisibleArray.Length > 3 && VisibleArray[3];
            DetailReport5.Visible = VisibleArray.Length > 4 && VisibleArray[4];
            DetailReport6.Visible = VisibleArray.Length > 5 && VisibleArray[5];
            DetailReport3.Visible = VisibleArray.Length > 6 && VisibleArray[6];
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptMedicalInstruction1.EnforceConstraints = false;
            spRptMedicalInstructionTableAdapter.Fill(dsRptMedicalInstruction1.spRptMedicalInstruction, IntPtDiagDrInstructionID.Value as long?, 0
                , PrescriptID.Value as long?, null, null);
            spRptGetIntravenousPlan_InPtTableAdapter.Fill(dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt, IntPtDiagDrInstructionID.Value as long?
                , PrescriptID.Value as long? , 11001);
            sp_GetAllPCLItemsByInstructionIDTableAdapter.Fill(dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID, IntPtDiagDrInstructionID.Value as long?);
            sp_GetAllRegistrationItemsByInstructionIDTableAdapter.Fill(dsRptMedicalInstruction1.sp_GetAllRegistrationItemsByInstructionID, IntPtDiagDrInstructionID.Value as long?);
            bool[] VisibleArray = new bool[] { true, true, true, true, true, true, true };
            if (dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt != null && dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt.Rows.Count > 0)
            {
                if (!dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt.Rows.Cast<DataRow>().Any(x => x["V_InfusionProcessType"] != null
                    && x["V_InfusionProcessType"] != DBNull.Value
                    && Convert.ToInt64(x["V_InfusionProcessType"]) == 62201))
                {
                    VisibleArray[1] = false;
                }
                if (!dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt.Rows.Cast<DataRow>().Any(x => x["V_InfusionProcessType"] != null
                    && x["V_InfusionProcessType"] != DBNull.Value
                    && Convert.ToInt64(x["V_InfusionProcessType"]) == 62202))
                {
                    VisibleArray[2] = false;
                }
                if (!dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt.Rows.Cast<DataRow>().Any(x => x["V_InfusionProcessType"] == null
                    || x["V_InfusionProcessType"] == DBNull.Value))
                {
                    VisibleArray[3] = false;
                }
            }
            else
            {
                VisibleArray[1] = false;
                VisibleArray[2] = false;
                VisibleArray[3] = false;
            }
            if (dsRptMedicalInstruction1.sp_GetAllRegistrationItemsByInstructionID == null || dsRptMedicalInstruction1.sp_GetAllRegistrationItemsByInstructionID.Rows.Count == 0)
            {
                VisibleArray[0] = false;
            }
            if (dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID != null && dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID.Rows.Count > 0)
            {
                if (!dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID.Rows.Cast<DataRow>().Any(x => x["V_PCLMainCategory"] != null
                   && x["V_PCLMainCategory"] != DBNull.Value
                   && Convert.ToInt64(x["V_PCLMainCategory"]) == 28201))
                {
                    VisibleArray[4] = false;
                }
                if (!dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID.Rows.Cast<DataRow>().Any(x => x["V_PCLMainCategory"] != null
                   && x["V_PCLMainCategory"] != DBNull.Value
                   && Convert.ToInt64(x["V_PCLMainCategory"]) == 28200))
                {
                    VisibleArray[5] = false;
                }
            }
            else
            {
                VisibleArray[4] = false;
                VisibleArray[5] = false;
            }
            if(dsRptMedicalInstruction1.spRptMedicalInstruction != null && dsRptMedicalInstruction1.spRptMedicalInstruction.Rows.Count > 0)
            {
                if(dsRptMedicalInstruction1.spRptMedicalInstruction.Rows[0]["InstructionOther"] == null 
                    || Convert.ToString(dsRptMedicalInstruction1.spRptMedicalInstruction.Rows[0]["InstructionOther"]) == "")
                {
                    VisibleArray[6] = false;
                }
            }
            VisibleContent(VisibleArray);
        }
    }
}
