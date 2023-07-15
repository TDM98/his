using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNewForPrintSilently_InPt_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNewForPrintSilently_InPt_V2()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew_InPt1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(this.parIssueID.Value), true, false);
        }

        private void XRptEPrescriptionNewForPrintSilently_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0)
            {
                if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"] == null || (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"] == null)
                {
                    lbFContactName.Visible = false;
                    lbFContactNameValue.Visible = false;
                    lbMOB.Visible = false;
                    lbMOBValue.Visible = false;
                }
                DateTime DOB = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"];
                DateTime IssueDate = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"];
                int mMOB = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
                if (mMOB <= 72)
                {
                    lbFContactName.Visible = true;
                    lbFContactNameValue.Visible = true;
                    lbMOB.Visible = true;
                    lbMOBValue.Text = mMOB.ToString() + " tháng";
                }
                else
                {
                    lbFContactName.Visible = false;
                    lbFContactNameValue.Visible = false;
                    lbMOB.Visible = false;
                    lbMOBValue.Text = null;
                }
            }
        }

        private void xrLabel4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //KMx: Anh Tuấn kiu ẩn hiệp hội Alain Carpentier đi (08/08/2016 15:18).
            e.Cancel = true;
        }

    }
}
