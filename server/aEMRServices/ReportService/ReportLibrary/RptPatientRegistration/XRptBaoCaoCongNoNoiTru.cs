using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoCongNoNoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoCongNoNoiTru()
        {
            InitializeComponent();
            this.BeforePrint+=new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoCongNoNoiTru_BeforePrint);
        }

        public void FillData() 
        {
            dsBaoCaoCongNoNoiTru1.EnforceConstraints = false;
            spBCCongNoNoiTruTableAdapter.Fill(dsBaoCaoCongNoNoiTru1.spBCCongNoNoiTru
                , Convert.ToDateTime(ToDate.Value)
                , Convert.ToDateTime(FromDate.Value),
                Convert.ToInt32(DeptID.Value));
        }

        private void XRptBaoCaoCongNoNoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
