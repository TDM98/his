using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
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
            string Str1 = this.parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(Str1));
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(Str1), false);
        }

        private void XRptEPrescriptionNew_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            /*TMA 23/11/2017 */
            //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            string str = this.parIssueID.Value.ToString().Remove(0, parIssueID.Value.ToString().Length - 1);
            this.parIssueID.Value = Convert.ToInt32(this.parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            if (String.Compare(str, "4".ToString(), true) == 0) // hướng thần
            {
                var dt = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                int dem = 0;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        var dataRow = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                        if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                        {
                            if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) == 4)
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
                var dt = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                int dem = 0;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        var dataRow = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                        if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                        {
                            if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 4)
                                dem = dem + 1;
                        }
                        else
                            dem = dem + 1;
                    }
                }
                if (dem == 0)
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
