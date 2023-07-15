using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
//using DevExpress.XtraPrinting;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
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
            string Str1 = this.parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(Str1));
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(Str1), false, false);
        }
        private void XRptEPrescriptionNew_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            /*TMA 23/11/2017 */
            //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            string str = this.parIssueID.Value.ToString().Remove(0, parIssueID.Value.ToString().Length - 1);
            this.parIssueID.Value = Convert.ToInt32(this.parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            if (String.Compare(str, "2".ToString(), true) == 0) // hướng thần
            {
                var dt = this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt;
                int dem = 0;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        var dataRow = this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt.Rows[i];
                        if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length>0)
                        {
                            if(Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) == 2)
                                dem = dem + 1;
                        }
                    }
                }
                if (dem == 0)
                {
                    e.Cancel = true;
                }
                else
                {
                    xrLabel8.Text = xrLabel8.Text + " 'H'";
                    xrLabel15.Text = xrLabel15.Text + " 'H'";
                    e.Cancel = false;
                }
            }
            else
            {
                var dt = this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt;
                int dem = 0;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        var dataRow = this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt.Rows[i];
                        if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                        {
                            if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 2)
                                dem = dem + 1;
                        }
                        else
                            dem = dem + 1;
                    }
                }
                // 20190918 TNHX: Add condition to show "TOA KHONG THUOC"
                if (dem == 0 && dt.Rows.Count > 0)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
            /*TMA 23/11/2017 */
            //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
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
