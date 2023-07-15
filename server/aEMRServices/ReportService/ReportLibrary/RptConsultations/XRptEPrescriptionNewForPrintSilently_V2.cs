using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNewForPrintSilently_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNewForPrintSilently_V2()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(this.parIssueID.Value), true, false);
        }

        private void XRptEPrescriptionNewForPrintSilently_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID != null && dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.Rows.Count > 0)
            {
                if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"] == null || (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"] == null)
                {
                    lbFContactName.Visible = false;
                    lbMOB.Visible = false;
                    lbFContactNameValue.Visible = false;
                    lbMOBValue.Visible = false;
                }
                DateTime DOB = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"];
                DateTime IssueDate = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"];
                int mMOB = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
                if (mMOB <= 72)
                {
                    lbMOBValue.Text = mMOB.ToString() + " tháng";
                    lbFContactName.Visible = true;
                    lbMOB.Visible = true;
                    lbFContactNameValue.Visible = true;
                }
                else
                {
                    lbMOBValue.Text = null;
                    lbFContactName.Visible = false;
                    lbMOB.Visible = false;
                    lbFContactNameValue.Visible = false;
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
