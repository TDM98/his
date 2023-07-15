using System;
using System.Linq;
//using DevExpress.XtraPrinting;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionNew_InPt_V2_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNew_InPt_V2_TV()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew_InPt1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(Str1));
            spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(Str1), false, false);
        }

        private void XRptEPrescriptionNew_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
             // Toa thuoc noi tru khong can quan tam loai thuoc
            FillData();
            if (Convert.ToString(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["HasDrugOrNot"]) != "")
            {
                xrLabel4.Visible = false;
            }
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"];
            }            
            int age = IssueDate.Year - DOB.Year;
            int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            if (monthnew <= 72)
            {
                xrLabel71.Text = xrLabel71.Text + " - " + monthnew.ToString() + " tháng";
                //xrLabel40.Text = "" + monthnew.ToString() + " tháng";
                xrLabel14.Visible = true;
            }
            else
            {
                //xrLabel71.Text = "(" + age.ToString() + "T)";
                xrLabel14.Visible = false;
            }
        }
    }
}
