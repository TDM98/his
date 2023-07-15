using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNew_V2_Old : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNew_V2_Old()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(this.parIssueID.Value), false);
        }

        private void XRptEPrescriptionNew_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            
            /*TMA 07/11/2017 : Mr Công yêu cầu thêm label tuổi = ngày ra toa - ngày sinh*/
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID != null && dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"];
            }
            
            int age = IssueDate.Year - DOB.Year;
            // ----- DPT 10/11/2017 lấy theo tháng
            //if (age <= 6)
            //{
            //    xrLabel71.Text = "";
                
            //}
            //else
            //{
            //    xrLabel71.Text = "(" + age.ToString() + "T)";
            //}

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
            if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID != null && dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"];
            }

            // ----- DPT 10/11/2017 lấy theo tháng
            //if (IssueDate.Year - DOB.Year <= 6)
            //{
            //    GroupHeader2.Visible = true;
            //}
            //else
            //{
            //    GroupHeader2.Visible = false;
            //}

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
