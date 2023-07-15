using System;
using System.Linq;
/*
 * 20200417 #001 TNHX: Báo cáo này để hiển thị khi 
 * 1. Lưu trong chỉ định + xem in tổng hợp với cấu hình in tách phiếu yêu cầu HA
 * 2. Xem in lại phiếu chỉ định hiện tại
 */
namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID != null && dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows.Count > 0)
            {
                DOB = (DateTime)dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.FirstOrDefault()["DOB"];
            }
            int age = IssueDate.Year - DOB.Year;
            int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            if (monthnew <= 72)
            {
                xrLabel31.Text = monthnew.ToString() + " Tháng (" + DOB.Year + ")";
            }
            else
            {
                xrLabel31.Text = age.ToString() + " (" + DOB.Year + ")";
            }
            if (paramV_RegistrationType.Value.ToString() == "24003")
            {
                xrLabel24.Visible = false;
            }
        }

        private void FillData()
        {
            dsPatientPCLRequestDetailsByPatientPCLReqID1.EnforceConstraints = false;
            spRptPatientPCLRequestDetailsByPatientPCLReqIDTableAdapter.Fill(dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID, Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt32(paramV_RegistrationType.Value));
            string ListAgencyID = "";
            string ListAgencyInfo = "";
            if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows.Count > 0)
            {
                if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[0]["V_PCLMainCategory"].ToString() == "28201")
                {
                    int RowCount = dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows.Count;
                    for (int i=0; i <= RowCount - 1; i++)
                    {
                        if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[i]["PCLSectionID"].ToString() == "3")
                        {
                            xrCheckBox1.Visible = true;
                            xrCheckBox2.Visible = true;
                        }
                        string tempAgencyID = dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[i]["AgencyID"].ToString();
                        if (!ListAgencyID.Contains(tempAgencyID))
                        {
                            ListAgencyID += ";" + tempAgencyID;
                            ListAgencyInfo += dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[i]["AgencyInfo"].ToString() + "\n";
                        }
                    }
                    xrLabel2.Text = ListAgencyInfo;
                    xrRichText3.Visible = true;
                    xrLabel28.Visible = true;
                    xrLabel21.Visible = true;
                    xrPageInfo6.Visible = true;
                    xrPageInfo7.Visible = true;
                    xrPageInfo8.Visible = true;
                    xrPageInfo9.Visible = true;
                }
                if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[0]["AllowToPayAfter"].ToString() == "True")
                {
                    xrPageInfo3.Visible = true;
                    xrPageInfo4.Visible = true;
                    xrLabel25.Visible = true;
                    xrPanel2.Visible = true;
                }

                if (dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[0]["HospitalCode"].ToString() == "96160" 
                    && dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID.Rows[0]["V_PCLMainCategory"].ToString() == "28200")
                {
                    xrTable1.Visible = false;
                    xrTable2.Visible = false;
                    xrTable3.Visible = true;
                    xrTable4.Visible = true;
                    xrLabel10.Visible = true;
                    xrLabel12.Visible = true;
                }
                else
                {
                    xrTable1.Visible = true;
                    xrTable2.Visible = true;
                    xrTable3.Visible = false;
                    xrTable4.Visible = false;
                    xrLabel10.Visible = false;
                    xrLabel12.Visible = false;
                }
            }
        }
    }
}
