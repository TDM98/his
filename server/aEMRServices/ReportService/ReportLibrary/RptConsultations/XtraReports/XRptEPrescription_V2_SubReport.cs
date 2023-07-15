using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
/*
 * 20181101 #001 TTM: BM 0004220:   Thêm điều kiện chỉ FillData lúc cần thiết (khi bệnh nhân có toa hướng thần hoặc toa chứ TPCN/MP) và thêm toa TPCN/MP
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescription_V2_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescription_V2_SubReport()
        {
            InitializeComponent();
        }

        private void XRptEPrescription_V2_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "2".ToString();
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] != 4 && [RefGenDrugCatID_1] != 5 && [RefGenDrugCatID_1] != 6"; // toa thuốc gây nghiện, toa thuốc khác hướng thần
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "4".ToString();
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] = 4"; // toa thuốc hướng thần
            //▼====== #001:     Set biến để report kiểm tra xem có thuốc hướng thần không.
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).parIsPsychotropicDrugs = parIsPsychotropicDrugs; 
            //▲====== #001
        }

        //▼====== #001
        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "5".ToString();
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] = 5 || [RefGenDrugCatID_1] = 6"; // toa thuốc chứa TPCN/MP
            //Set biến để report kiểm tra xem có thực phẩm chức năng/ mỹ phẩm không.
            ((XRptEPrescriptionNew_V2)((XRSubreport)sender).ReportSource).parIsFuncfoodsOrCosmetics = parIsFuncfoodsOrCosmetics;   
        }
        //▲====== #001
    }
}
