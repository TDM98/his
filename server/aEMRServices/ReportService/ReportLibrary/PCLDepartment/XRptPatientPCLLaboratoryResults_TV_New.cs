using System;
using System.Linq;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptPatientPCLLaboratoryResults_TV_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLLaboratoryResults_TV_New()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPatientPCLLaboratoryResults_TV1.EnforceConstraints = false;
            spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_InfoTableAdapter.Fill(dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info, 
                Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt32(parPatientFindBy.Value));

            spPatientPCLLaboratoryResults_ByPatientPCLReqID_TVTableAdapter.Fill(dsPatientPCLLaboratoryResults_TV1.spPatientPCLLaboratoryResults_ByPatientPCLReqID_TV,
                Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt32(parV_PCLRequestType.Value));

            if (parV_PCLRequestType.Value.ToString() == "25001")
            {
                xrLabel2.Visible = false;
                xrLabel12.Visible = false;
            }
            if (dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info != null && dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.Rows.Count > 0)
            {
                if ((int)dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["HasExplanation"] == 1)
                {
                    xrPanel1.Visible = true;
                    if (dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault() != null && !string.IsNullOrEmpty(dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["DeNghi"].ToString()))
                    {
                        xrLabel14.Visible = true;
                        xrLabel18.Visible = true;
                    }
                }
                else
                {
                    if (dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault() != null && !string.IsNullOrEmpty(dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["DeNghi"].ToString()))
                    {
                        xrLabel14.Visible = true;
                        xrLabel18.Visible = true;
                        xrLabel14.LocationF = new System.Drawing.Point(10, 10);
                        xrLabel18.LocationF = new System.Drawing.Point(87, 10);
                    }
                }
                if ((int)dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["IsSeparate"] == 0)
                {
                    GroupHeader2.Visible = false;
                    GroupFooter1.Visible = false;
                    xrPageBreak1.Visible = false;
                }

                DateTime CurDate = DateTime.Now;
                int DOB = (int)dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["NamSinh"];
                if ((CurDate.Year - DOB) < 7)
                {
                    xrLabel46.Text = dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["Tuoi"].ToString() + " Tháng";
                }
                else
                {
                    xrLabel46.Text = dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["Tuoi"].ToString() 
                        + " (" + dsPatientPCLLaboratoryResults_TV1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info.FirstOrDefault()["NamSinh"].ToString() + ")";
                }
            }
        }

        private void XRptPatientPCLLaboratoryResults_TV_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
