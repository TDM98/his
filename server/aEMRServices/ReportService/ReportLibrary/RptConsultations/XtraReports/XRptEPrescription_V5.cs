using System;
using DevExpress.XtraReports.UI;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 * 20220823 #003 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_V5 : XtraReport
    {
        public XRptEPrescription_V5()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);
            spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(Str1));
            spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(Str1), false, false);
        }

        private void XRptEPrescription_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string TypePrescription = parIssueID.Value.ToString().Remove(0, parIssueID.Value.ToString().Length - 1);
            if (!Convert.ToBoolean(parIsPsychotropicDrugs.Value) && TypePrescription == "4")
            {
                e.Cancel = true;
            }
            else if (!Convert.ToBoolean(parIsFuncfoodsOrCosmetics.Value) && TypePrescription == "5")
            {
                e.Cancel = true;
            }
            else
            {
                FillData();
                if (!string.IsNullOrEmpty(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["BHYT"].ToString()))
                {
                    xrLabel15.Text = "TOA THUỐC ĐIỀU TRỊ";
                    xrLabel17.Visible = true; //==== #003
                }
                else
                {
                    xrLabel15.Text = "TOA THUỐC ĐIỀU TRỊ";
                }
                if (TypePrescription == "4") // hướng thần
                {
                    var dt = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                    int dem = 0;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            var dataRow = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                            if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                            {
                                if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) == 4)
                                {
                                    dem = dem + 1;
                                }
                            }
                        }
                    }
                    if (dem == 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        xrLabel8.Text = "'Toa hướng thần'";
                        e.Cancel = false;
                    }
                }
                //▼====== #002: Bổ sung thêm toa thực phẩm chức năng, mỹ phẩm
                else if (TypePrescription == "5")
                {
                    xrLabel15.Text = "PHIẾU TƯ VẤN";
                    var dt = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                    int dem = 0;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            var dataRow = this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                            if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                            {
                                if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) == 5 || Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) == 6)
                                {
                                    dem = dem + 1;
                                }
                            }
                        }
                    }
                    if (dem == 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        xrLabel8.Text = "'TPCN/MP'";
                        e.Cancel = false;
                    }
                }
                //▲====== #002
                else
                {
                    var dt = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                    int dem = 0;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            var dataRow = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                            if (dataRow["RefGenDrugCatID_1"] != null && dataRow["RefGenDrugCatID_1"].ToString().Length > 0)
                            {
                                //▼====== #002: 
                                //if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 4)
                                if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 4 && Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 5 && Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 6)
                                //▲====== #002
                                {
                                    dem = dem + 1;
                                }
                            }
                            else
                            {
                                dem = dem + 1;
                            }
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
                //switch (parHospitalCode.Value.ToString())
                //{
                //    case "95076":
                //        xrLabel2.Visible = true;
                //        xrLabel2.Text = "Thông báo: Từ 1/8/2021, BVĐK Thanh Vũ Medic sẽ ngưng tiếp nhận khám chữa bệnh tại khoa khám bệnh ngoại trú vào chiều Chủ nhật hàng tuần (11h-17h). Các hoạt động tiếp nhận bệnh cấp cứu vẫn hoạt động 24/24h tất cả các ngày trong tuần. Chi tiết vui lòng liên hệ hotline 1800969698.";
                //        break;
                //    case "95078":
                //        xrLabel2.Visible = true;
                //        xrLabel2.Text = "Thông báo: Từ 1/8/2021, BVĐK Thanh Vũ Medic Bạc Liêu sẽ ngưng tiếp nhận khám chữa bệnh tại khoa khám bệnh ngoại trú vào chiều Chủ nhật hàng tuần (11h-17h). Các hoạt động tiếp nhận bệnh cấp cứu vẫn hoạt động 24/24h tất cả các ngày trong tuần. Chi tiết vui lòng liên hệ hotline 1800969698.";
                //        break;
                //    default:
                //        break;
                //}
                if (parHospitalCode.Value.ToString() != "95078" || !Convert.ToBoolean(parIsYHCTPrescript.Value))
                {
                    xrLabel3.Text = null;
                    xrLabel13.Text = null;
                    xrLabel11.Text = null;
                    xrLabel14.Text = null;
                    xrLabel12.Text = null;
                    xrLabel10.Text = null;
                }
                else
                {
                    xrLabel73.Text = null;
                    xrLabel74.Text = null;
                    xrLabel75.Text = null;
                    xrLabel75.LocationF = new DevExpress.Utils.PointFloat(552.78F, 108.27F); 
                }
            }
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToInt32(parDetailBeforePrintCount.Value) != 0)
            {
                e.Cancel = true;
            }
            else
            {
                parDetailBeforePrintCount.Value = Convert.ToInt32(parDetailBeforePrintCount.Value) + 1;
            }
        }

        private void XrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptEPrescriptionDetails_V5_Drug)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueID.Value.ToString());
            ((XRptEPrescriptionDetails_V5_Drug)((XRSubreport)sender).ReportSource).FilterString = FilterString;
            ((XRptEPrescriptionDetails_V5_Drug)((XRSubreport)sender).ReportSource).parIsPsychotropicDrugs = parIsPsychotropicDrugs;
            ((XRptEPrescriptionDetails_V5_Drug)((XRSubreport)sender).ReportSource).parIsFuncfoodsOrCosmetics = parIsFuncfoodsOrCosmetics;
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //switch(parHospitalCode.Value.ToString())
            //{
            //    case "95076":
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV1();
            //        xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            //        xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
            //        break;
            //    case "95078":
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV2();
            //        xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            //        xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
            //        break;
            //    default:
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV3();
            //        xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            //        xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
            //        break;
            //}
            xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_V5();
            xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
        }

        private void XrLabel80_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            bool IsOutCatConfirmed = false;
            bool IsHIPatient = false;
            if (!string.IsNullOrEmpty(dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault()["BHYT"].ToString()))
            {
                IsHIPatient = true;               
            }
            if (dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID.FirstOrDefault().IsOutCatConfirmed)
            {
                IsOutCatConfirmed = true;
            }
            if (IsOutCatConfirmed && IsHIPatient)
            {
                xrLabel80.Visible = true;                
            }
            else
            {
                xrLabel80.Visible = false;
            }
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueID.Value.ToString());
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).FilterString = FilterString;
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).parIsPsychotropicDrugs = parIsPsychotropicDrugs;
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).parIsFuncfoodsOrCosmetics = parIsFuncfoodsOrCosmetics;
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode;
            ((XRptEPrescriptionDetails_V5_Info)((XRSubreport)sender).ReportSource).parIsYHCTPrescript.Value = parIsYHCTPrescript;
        }
    }
}
