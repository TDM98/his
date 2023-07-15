using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    public partial class XRptOutwardToPatient : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutwardToPatient()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptOutwardToPatient_BeforePrint);
        }

        public void FillData()
        {
            spRpt_OutwardToPatient_SummaryTableAdapter.Fill(dsOutwardToPatient1.spRpt_OutwardToPatient_Summary
                                                            , Convert.ToDateTime(this.FromDate.Value)
                                                            , Convert.ToDateTime(this.ToDate.Value)
                                                            , Convert.ToInt16(this.Quarter.Value)
                                                            , Convert.ToInt16(this.Month.Value)
                                                            , Convert.ToInt16(this.Year.Value)
                                                            , Convert.ToInt16(this.flag.Value)
                                                            , Convert.ToInt16(this.StoreID.Value)
                                                            , Convert.ToInt32(this.V_MedProductType.Value));
        }

        private void XRptOutwardToPatient_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
