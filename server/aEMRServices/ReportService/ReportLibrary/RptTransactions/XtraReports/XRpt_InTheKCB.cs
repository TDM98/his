using aEMR.DataAccessLayer.Providers;
using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_InTheKCB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_InTheKCB()
        {
            InitializeComponent();
        }

        private void XRpt_InTheKCB_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int TypeCard = Convert.ToInt16(parTypeCard.Value);
            switch (TypeCard)
            {
                case 1:  // Thẻ vaccine
                    ((XRSubreport)sender).ReportSource = new XRpt_InTheKCBVaccine();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parDOB.Value = parDOB.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parFullName.Value = parFullName.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parParentName.Value = parParentName.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parParentPhone.Value = parParentPhone.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parPatientCode.Value = parPatientCode.Value.ToString();
                    break;
                case 2: // thẻ KCB VietinBank
                    ((XRSubreport)sender).ReportSource = new XRpt_InTheKCBVietinBank();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parDOB.Value = parDOB.Value.ToString();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parFullName.Value = parFullName.Value.ToString();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parParentName.Value = parParentName.Value.ToString();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parParentPhone.Value = parParentPhone.Value.ToString();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parPatientCode.Value = parPatientCode.Value.ToString();
                    ((XRpt_InTheKCBVietinBank)((XRSubreport)sender).ReportSource).parImageUrl.Value = parImageUrl.Value.ToString();
                    break;
                default:
                    ((XRSubreport)sender).ReportSource = new XRpt_InTheKCBVaccine();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parDOB.Value = parDOB.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parFullName.Value = parFullName.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parParentName.Value = parParentName.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parParentPhone.Value = parParentPhone.Value.ToString();
                    ((XRpt_InTheKCBVaccine)((XRSubreport)sender).ReportSource).parPatientCode.Value = parPatientCode.Value.ToString();
                    break;
            }
        }
    }
}
