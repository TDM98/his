using System;

namespace eHCMS.ReportLib.RptConfiguration.XtraReports
{
    public partial class XRpt_BangGiaCLS : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BangGiaCLS()
        {
            InitializeComponent();
        }

        private void XRpt_BangGiaCLS_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsPCLExamTypePriceList_Detail1.EnforceConstraints = false;
            spRpt_PCLExamTypePriceList_DetailTableAdapter.Fill(dsPCLExamTypePriceList_Detail1.spRpt_PCLExamTypePriceList_Detail
                , Convert.ToInt64(parPCLExamTypePriceListID.Value)
            );
        }
    }
}
