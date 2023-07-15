using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptGiayHoanTamUngNoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptGiayHoanTamUngNoiTru()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptGiayHoanTamUngNoiTru_BeforePrint);
        }


        void XRptGiayHoanTamUngNoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            spGiayHoanMienTamUngNoiTruTableAdapter.Fill(dsGiayHoanMienTamUngNoiTru1.spGiayHoanMienTamUngNoiTru
                                                                , Convert.ToInt32(this.InPatientAdmDisDetailID.Value));
        }
    }
}
