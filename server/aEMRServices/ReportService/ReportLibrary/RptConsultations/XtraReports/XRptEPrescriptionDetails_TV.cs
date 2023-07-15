using System;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionDetails_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionDetails_TV()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(Str1));
            spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(Str1), false, false);
        }

        private void XRptEPrescription_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (Convert.ToString(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["HasDrugOrNot"]) != "")
            {
                xrLabel4.Visible = false;
            }
            /*TMA 07/11/2017 : Mr Công yêu cầu thêm label tuổi = ngày ra toa - ngày sinh*/
            DateTime DOB = DateTime.Now;
            DateTime IssueDate = DateTime.Now;
            if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID != null && dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.Rows.Count > 0)
            {
                DOB = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"];
                IssueDate = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"];
            }

            int age = IssueDate.Year - DOB.Year;
            //-------------- DPT 10/11/2017 : Tính số tháng cho bệnh nhân dưới 6 tuổi
            int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            if (monthnew <= 72)
            {
                xrLabel40.Text = "" + monthnew.ToString() + " Tháng (NS:" + DOB.Year + ")";               
            }
            else
            {
                xrLabel40.Text = age.ToString() + " Tuổi (NS:" + DOB.Year + ")";
            }       
        }

        private void GroupHeaderBand_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (GetCurrentColumnValue("BeOfHIMedicineList") != null && GetCurrentColumnValue("BeOfHIMedicineList") != DBNull.Value && GetCurrentColumnValue("BeOfHIMedicineList").ToString() == "False")
            {
                if (GetCurrentColumnValue("IsDrugNotInCat") != null && GetCurrentColumnValue("IsDrugNotInCat") != DBNull.Value && GetCurrentColumnValue("IsDrugNotInCat").ToString() != "0")
                {
                    xrLabel2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
                    xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F);
                }
                else
                {
                    xrLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
                    xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
                    if (!Convert.ToBoolean(HasShowLine.Value))
                    {
                        xrLine3.Visible = true;
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
            }
            else
            {
                xrLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
                xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F);
            }
        }
    }
}
