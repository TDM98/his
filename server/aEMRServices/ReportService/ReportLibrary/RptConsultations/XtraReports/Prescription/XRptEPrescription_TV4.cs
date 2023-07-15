using System;
using DevExpress.XtraReports.UI;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_TV4 : XtraReport
    {
        public XRptEPrescription_TV4()
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
            if (!Convert.ToBoolean(parHasPharmacyDrug.Value) && TypePrescription == "7")
            {
                e.Cancel = true;
            }
            else if(!Convert.ToBoolean(parIsPsychotropicDrugs.Value) && TypePrescription == "4")
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
                }
                else
                {
                    xrLabel15.Text = "TOA THUỐC ĐIỀU TRỊ";
                }
                if (TypePrescription == "7")
                {
                    var dt = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID;
                    int dem = 0;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            var dataRow = dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID.Rows[i];
                            if (dataRow["BeOfHIMedicineList"] != null && dataRow["BeOfHIMedicineList"].ToString().Length > 0)
                            {
                                if (!Convert.ToBoolean(dataRow["BeOfHIMedicineList"]))
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
                        //xrLabel8.Text = "'Thuốc mua ngoài'";
                        e.Cancel = false;
                    }
                }
                else if (TypePrescription == "4") // hướng thần
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
                                if (Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 4 && Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 5 && Convert.ToInt32(dataRow["RefGenDrugCatID_1"]) != 6 && Convert.ToBoolean(dataRow["BeOfHIMedicineList"]))
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
            ((XRptEPrescriptionDetails_TV)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueID.Value.ToString());
            ((XRptEPrescriptionDetails_TV)((XRSubreport)sender).ReportSource).FilterString = FilterString;
            ((XRptEPrescriptionDetails_TV)((XRSubreport)sender).ReportSource).parIsPsychotropicDrugs = parIsPsychotropicDrugs;
            ((XRptEPrescriptionDetails_TV)((XRSubreport)sender).ReportSource).parIsFuncfoodsOrCosmetics = parIsFuncfoodsOrCosmetics;
            ((XRptEPrescriptionDetails_TV)((XRSubreport)sender).ReportSource).parHasPharmacyDrug = parHasPharmacyDrug;
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            switch(parHospitalCode.Value.ToString())
            {
                case "95076":
                    xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV1();
                    xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
                    xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
                    break;
                case "95078":
                    xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV2();
                    xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
                    xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
                    break;
                default:
                    xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV3();
                    xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
                    xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24001;
                    break;
            }
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
    }
}
