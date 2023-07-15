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
    public sealed partial class XRptGiayMienTamUngNoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptGiayMienTamUngNoiTru()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptGiayMienTamUngNoiTru_BeforePrint);
        }


        void XRptGiayMienTamUngNoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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
