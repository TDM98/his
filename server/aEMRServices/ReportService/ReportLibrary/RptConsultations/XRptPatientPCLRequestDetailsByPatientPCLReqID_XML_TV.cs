using System;
using System.Linq;
/*
 * 20200417 #001 TNHX: Báo cáo này để hiển thị khi: 
 * 1. Lưu trong chỉ định + xem in tổng hợp với cấu hình in không tách phiếu yêu cầu (in gộp) 
 */
namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader != null && dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader.Rows.Count > 0)
            {
                DOB = (DateTime)dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader.FirstOrDefault()["DOB"];
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
            dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.EnforceConstraints = false;
            //this.spRptPatientPCLRequestDetailsByPatientPCLReqIDXmlTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml, this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
            //this.spRptPatientPCLRequestsHeaderTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader, Convert.ToInt32(this.parPtRegistrationID.Value), this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
            spRptPatientPCLRequestDetailsByPatientPCLReqIDXmlTableAdapter.Fill(dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml, parPCLReqID_XML.Value.ToString(), Convert.ToInt32(paramV_RegistrationType.Value));
            spRptPatientPCLRequestsHeaderTableAdapter.Fill(dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader, Convert.ToInt32(parPtRegistrationID.Value), parPCLReqID_XML.Value.ToString(), Convert.ToInt32(paramV_RegistrationType.Value));

            if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader.Rows.Count > 0)
            {
                if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[0]["V_PCLMainCategory"].ToString() == "28201")
                {
                    string ListAgencyID = "";
                    string ListAgencyInfo = "";
                    int RowCount = dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows.Count;
                    for (int i = 0; i <= RowCount - 1; i++)
                    {
                        if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[i]["PCLSectionID"].ToString() == "3")
                        {
                            xrCheckBox1.Visible = true;
                            xrCheckBox2.Visible = true;
                        }
                        string tempAgencyID = dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[i]["AgencyID"].ToString();
                        if (!ListAgencyID.Contains(tempAgencyID))
                        {
                            ListAgencyID += ";" + tempAgencyID;
                            ListAgencyInfo += dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[i]["AgencyInfo"].ToString() + "\n";
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
                if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[0]["AllowToPayAfter"].ToString() == "True")
                {
                    xrPageInfo3.Visible = true;
                    xrPageInfo4.Visible = true;
                    xrLabel25.Visible = true;
                    xrPanel2.Visible = true;
                }
                if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[0]["HospitalCode"].ToString() == "95078")
                {
                    xrRichText1.Visible = false;
                    xrRichText2.Visible = true;
                    xrRichText4.Visible = false;
                }
                else if (dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml.Rows[0]["HospitalCode"].ToString() == "95076")
                {
                    xrRichText1.Visible = false;
                    xrRichText2.Visible = false;
                    xrRichText4.Visible = true;
                }
                else
                {
                    xrRichText1.Visible = true;
                    xrRichText2.Visible = false;
                    xrRichText4.Visible = false;
                }
            }
        }
    }
}
