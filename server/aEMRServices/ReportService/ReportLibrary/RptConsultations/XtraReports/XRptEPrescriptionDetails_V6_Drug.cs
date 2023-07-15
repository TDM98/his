using System;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionDetails_V6_Drug : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionDetails_V6_Drug()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);/*TMA 23/11/2017 */ //mục định : xác định toa có hướng thần hay ko ? nếu không thì ko in lần 2
            //spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(Str1));
            spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(Str1), false, false);
        }

        private void XRptEPrescription_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            //if (Convert.ToString(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["HasDrugOrNot"]) != "")
            //{
            //    xrLabel4.Visible = false;
            //}
            ///*TMA 07/11/2017 : Mr Công yêu cầu thêm label tuổi = ngày ra toa - ngày sinh*/
            //DateTime DOB = DateTime.Now;
            //DateTime IssueDate = DateTime.Now;
            //if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID != null && dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.Rows.Count > 0)
            //{
            //    DOB = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["DOB"];
            //    IssueDate = (DateTime)dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["IssuedDateTime"];
            //}

            //int age = IssueDate.Year - DOB.Year;
            //-------------- DPT 10/11/2017 : Tính số tháng cho bệnh nhân dưới 6 tuổi
            //int monthnew = (IssueDate.Month + IssueDate.Year * 12) - (DOB.Month + DOB.Year * 12);
            //if (monthnew <= 72)
            //{
            //    xrLabel40.Text = "" + monthnew.ToString() + " Tháng (NS:" + DOB.Year + ")";               
            //}
            //else
            //{
            //    xrLabel40.Text = age.ToString() + " Tuổi (NS:" + DOB.Year + ")";
            //}       


            //var dt = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
            //int dem = 0;
            //if (dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i <= dt.Rows.Count - 1; i++)
            //    {
            //        var dataRow = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
            //        if (dataRow["BeOfHIMedicineList"] != null && dataRow["BeOfHIMedicineList"].ToString().Length > 0)
            //        {
            //            if (dataRow["BeOfHIMedicineList"].ToString() == "True")
            //            {
            //                dem = dem + 1;
            //            }
            //        }
            //    }
            //}
            //if (dem == 0)
            //{
            //    xrLabel4.Text = null;
            //    xrLine3.Visible = false;
            //}
        }

        private void GroupHeaderBand_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (GetCurrentColumnValue("BeOfHIMedicineList") != null && GetCurrentColumnValue("BeOfHIMedicineList") != DBNull.Value && GetCurrentColumnValue("BeOfHIMedicineList").ToString() == "False")
            {
                xrLabel4.Visible = false;
                xrLabel4.HeightF = 0;
                xrLabel1.Visible = false;
                xrLabel1.HeightF = 0;
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
                        xrLine3.Visible = Convert.ToBoolean(HasShowThuocBHYT.Value) ? true : false;  
                        HasShowLine.Value = true;
                        xrLabel8.Visible = true;
                        xrLabel3.Visible = Convert.ToBoolean(HasShowThuocBHYT.Value) ? false : true;
                    }
                    else
                    {
                        xrLine3.Visible = false;
                        xrLabel8.Visible = false;
                        xrLabel8.HeightF = 0;
                        xrLabel3.Visible = false;
                        xrLabel3.HeightF = 0;
                    }
                }
            }
            else
            {
                xrLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
                xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F);
                // Kiểm tra có thuốc bảo hiểm thì hiển thì II.
                if (!Convert.ToBoolean(HasShowThuocBHYT.Value))
                {
                    HasShowThuocBHYT.Value = true;
                    xrLabel4.Visible = true;
                    xrLabel1.Visible = true;
                }
                else
                {
                    xrLabel4.Visible = false;
                    xrLabel4.HeightF = 0;
                    xrLabel1.Visible = false;
                    xrLabel1.HeightF = 0;
                }
            }
        }
    }
}
