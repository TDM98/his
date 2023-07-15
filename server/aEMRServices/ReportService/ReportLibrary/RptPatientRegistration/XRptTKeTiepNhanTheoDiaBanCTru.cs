using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptTKeTiepNhanTheoDiaBanCTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTKeTiepNhanTheoDiaBanCTru()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptTKeTiepNhanTheoDiaBanCTru_BeforePrint);
        }

        public void FillData()
        {
            spRptAdmissionStatisticsByWardNameTableAdapter.Fill(dsTKeTNhanTheoDiaBanCuTru.spRptAdmissionStatisticsByWardName
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRptTKeTiepNhanTheoDiaBanCTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
