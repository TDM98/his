using System;
using DevExpress.XtraReports.UI;
using System.Linq;
/*
 * 20181029 #001 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20181101 #002 TTM: BM 0004220: Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionInpt_V5_HT : XtraReport
    {
        public XRptEPrescriptionInpt_V5_HT()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPrescriptionNew_InPt1.EnforceConstraints = false;
            string Str1 = parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1);
            spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(Str1));
            //spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(Str1), false);
        }

        private void XRptEPrescriptionInpt_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string TypePrescription = parIssueID.Value.ToString().Remove(0, parIssueID.Value.ToString().Length - 1);
            if (!Convert.ToBoolean(parIsPsychotropicDrugs.Value) && TypePrescription == "4")
            {
                e.Cancel = true;
            }
            else if (!Convert.ToBoolean(parIsFuncfoodsOrCosmetics.Value) && TypePrescription == "5")
            {
                e.Cancel = true;
            } else
            {
                FillData();
                xrLabel15.Text = "ĐƠN THUỐC \"H\"";
            }
        }

        private void XrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptEPrescriptionInPtDetails_V5_GN_HT_Drug)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueID.Value.ToString());
            ((XRptEPrescriptionInPtDetails_V5_GN_HT_Drug)((XRSubreport)sender).ReportSource).FilterString = FilterString;
            ((XRptEPrescriptionInPtDetails_V5_GN_HT_Drug)((XRSubreport)sender).ReportSource).Parameters["parIsPsychotropicDrugs"].Value = parIsPsychotropicDrugs;
            if(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt != null
                && dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.Rows.Count > 0
                && Convert.ToString(dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt.FirstOrDefault()["BHYT"]) == "BHYT")
            {
                ((XRptEPrescriptionInPtDetails_V5_GN_HT_Drug)((XRSubreport)sender).ReportSource).Parameters["parIsBHYT"].Value = true;
            }
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //switch (parHospitalCode.Value.ToString())
            //{
            //    case "95076":
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV1();
            //        break;
            //    case "95078":
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_V5();
            //        xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            //        xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24003;
            //        break;
            //    default:
            //        xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_TV3();
            //        break;
            //}
            //xrSubreport1.ReportSource = new XRptEPrescription_SubLeft_V5();
            //xrSubreport1.ReportSource.Parameters["parPtRegDetailID"].Value = Convert.ToInt64(parIssueID.Value.ToString().Substring(0, parIssueID.Value.ToString().Length - 1));
            //xrSubreport1.ReportSource.Parameters["parV_RegistrationType"].Value = 24003;
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptEPrescriptionInPtDetails_V5_GN_HT_Info)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueID.Value.ToString());
            ((XRptEPrescriptionInPtDetails_V5_GN_HT_Info)((XRSubreport)sender).ReportSource).FilterString = FilterString;
        }
    }
}
