using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
//using DevExpress.XtraPrinting;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNew_InPt_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNew_InPt_V2()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew_InPt1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(this.parIssueID.Value), false, false);
        }
        private void XRptEPrescriptionNew_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            /*TMA 07/11/2017 : Mr Công yêu cầu thêm label tuổi = ngày ra toa - ngày sinh*/
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"];
            }
            
            int age = IssueDate.Year - DOB.Year;

            //-------------- DPT 10/11/2017 : Tính số tháng cho bệnh nhân dưới 6 tuổi
            int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            if (monthnew <= 72)
            {
                xrLabel71.Text = "";
                xrLabel40.Text = "" + monthnew.ToString() + " tháng";

            }
            else
            {
                xrLabel71.Text = "(" + age.ToString() + "T)";
            }
        }

        private void xrLabel4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //KMx: Anh Tuấn kiu ẩn hiệp hội Alain Carpentier đi (08/08/2016 15:18).
            /*TMA 07/11/2017 : Mr Công yêu cầu thêm label tuổi = ngày ra toa - ngày sinh - nên đã thay đổi điều kiện if của code ban đầu*/
            e.Cancel = true;
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["IssuedDateTime"];
            }

            //-------------- DPT 10/11/2017 : Tính số tháng cho bệnh nhân dưới 6 tuổi
            int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            if (monthnew <= 72)
            {
                GroupHeader2.Visible = true;
            }
            else
            {
                GroupHeader2.Visible = false;
            }
        }

    }
}
