using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRpt_BaoCaoKSNK : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoKSNK()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRpt_BaoCaoKSNK_BeforePrint);
        }

        public void FillData()
        {
            dsRpt_BaoCaoKSNK1.EnforceConstraints = false;
            spRpt_BaoCaoKSNKTableAdapter.Fill(dsRpt_BaoCaoKSNK1.spRpt_BaoCaoKSNK
                                                                , Convert.ToDateTime(parFromDate.Value)
                                                                , Convert.ToDateTime(parToDate.Value)
                                                                , Convert.ToInt16(Quarter.Value)
                                                                , Convert.ToInt16(Month.Value)
                                                                , Convert.ToInt16(Year.Value)
                                                                , Convert.ToInt16(flag.Value)
                                                                , Convert.ToInt32(DeptID.Value)
                                                                , Convert.ToInt16(parFindInfectionControl.Value));
        }

        private void XRpt_BaoCaoKSNK_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
