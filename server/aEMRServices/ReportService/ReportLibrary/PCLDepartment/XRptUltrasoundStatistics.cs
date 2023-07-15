using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptUltrasoundStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptUltrasoundStatistics()
        {
            InitializeComponent();
        }


        public void FillData()
        {
            spRpt_UltrasoundStatisticsTableAdapter.Fill(dsUltrasoundStatistics1.spRpt_UltrasoundStatistics, Convert.ToDateTime(this.parFromDate.Value)
                                                                                                            , Convert.ToDateTime(this.parToDate.Value)
                                                                                                            , Convert.ToInt16(this.parQuarter.Value)
                                                                                                            , Convert.ToInt16(this.parMonth.Value)
                                                                                                            , Convert.ToInt16(this.parYear.Value)
                                                                                                            , Convert.ToInt16(this.parFlag.Value)
                                                                                                            , Convert.ToInt32(this.parStaffID.Value)
                                                                                                            , Convert.ToInt32(this.DeptLocationID.Value)
                                                                                                            , Convert.ToInt32(this.PCLResultParamImpID.Value));
        }
        private void XRptUltrasoundStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
