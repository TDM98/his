using System;
using DevExpress.XtraReports.UI;
/*
 * 20181101 #001 TTM: BM 0004220:   Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_V2_SubReport_TV : XtraReport
    {
        public XRptEPrescription_V2_SubReport_TV()
        {
            InitializeComponent();
        }

        private void XRptEPrescription_V2_SubReport_TV_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = parIssueID.Value.ToString() + "2".ToString();
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] != 4 && [RefGenDrugCatID_1] != 5 && [RefGenDrugCatID_1] != 6"; // toa thuốc gây nghiện, toa thuốc khác hướng thần
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value.ToString();
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = parIssueID.Value.ToString() + "4".ToString();
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] = 4"; // toa thuốc hướng thần
            //▼====== #001:     Set biến để report kiểm tra xem có thuốc hướng thần không.
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).parIsPsychotropicDrugs = parIsPsychotropicDrugs;
            //▲====== #001
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value.ToString();
        }

        //▼====== #001
        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "5".ToString();
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] = 5 || [RefGenDrugCatID_1] = 6"; // toa thuốc chứa TPCN/MP
            //Set biến để report kiểm tra xem có thực phẩm chức năng/ mỹ phẩm không.
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).parIsFuncfoodsOrCosmetics = parIsFuncfoodsOrCosmetics;
            ((XRptEPrescription_TV)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value.ToString();
        }
        //▲====== #001
    }
}
