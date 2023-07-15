using System;
using System.Linq;
//using DevExpress.XtraPrinting;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionInPtDetails_V5_Drug : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionInPtDetails_V5_Drug()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew_InPt1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            //spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(Str1));
            spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(Str1), false, false);
        }

        private void XRptEPrescriptionNew_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            // Toa thuoc noi tru khong can quan tam loai thuoc
            FillData();
            //if (Convert.ToString(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["HasDrugOrNot"]) != "")
            //{
            //    xrLabel4.Visible = false;
            //}
            //DateTime DOB = DateTime.Now;
            //DateTime IssueDate = DateTime.Now;
            //if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0)
            //{
            //    DOB = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"];
            //    IssueDate = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"];
            //}            
            //int age = IssueDate.Year - DOB.Year;
            //int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            //if (monthnew <= 72)
            //{
            //    xrLabel71.Text = xrLabel71.Text + " - " + monthnew.ToString() + " tháng";
            //    //xrLabel40.Text = "" + monthnew.ToString() + " tháng";
            //    xrLabel14.Visible = true;
            //}
            //else
            //{
            //    //xrLabel71.Text = "(" + age.ToString() + "T)";
            //    xrLabel14.Visible = false;
            //}
        }

        private void groupHeaderBand1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (!Convert.ToBoolean(parIsBHYT.Value))
            {
                xrLabel3.Visible = false;
                xrLabel3.HeightF = 0;
                xrLine3.Visible = false;
                xrLabel8.Visible = false;
                xrLabel8.HeightF = 0;
            }
            else
            {
                if (GetCurrentColumnValue("BeOfHIMedicineList") != null && GetCurrentColumnValue("BeOfHIMedicineList") != DBNull.Value && GetCurrentColumnValue("BeOfHIMedicineList").ToString() == "False")
                {
                    xrLabel3.Visible = false;
                    xrLabel3.HeightF = 0;

                    if (!Convert.ToBoolean(HasShowLine.Value))
                    {
                        xrLine3.Visible = Convert.ToBoolean(HasShowThuocBHYT.Value) ? true : false;
                        HasShowLine.Value = true;
                        xrLabel8.Visible = true;
                    }
                    else
                    {
                        xrLine3.Visible = false;
                        xrLabel8.Visible = false;
                        xrLabel8.HeightF = 0;
                    }
                }
                else
                {
                    //xrLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
                    //xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F);
                    // Kiểm tra có thuốc bảo hiểm thì hiển thì II.
                    if (!Convert.ToBoolean(HasShowThuocBHYT.Value))
                    {
                        HasShowThuocBHYT.Value = true;
                        xrLabel3.Visible = true;
                    }
                    else
                    {
                        xrLabel3.Visible = false;
                        xrLabel3.HeightF = 0;
                    }
                }
            }
        }
    }
}
