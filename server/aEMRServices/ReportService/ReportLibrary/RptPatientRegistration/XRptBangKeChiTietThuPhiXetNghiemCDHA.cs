using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeChiTietThuPhiXetNghiemCDHA : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeChiTietThuPhiXetNghiemCDHA()
        {
            InitializeComponent();
            this.BeforePrint+=new System.Drawing.Printing.PrintEventHandler(XRptBangKeChiTietThuPhiXetNghiemCDHA_BeforePrint);
        }

        public void FillData() 
        {
            dsBangKeChiTietThuPhiKham_CDHA1.EnforceConstraints = false;
            spRpt_BangKeChiTietThuPhi_CDHATableAdapter.Fill(dsBangKeChiTietThuPhiKham_CDHA1.spRpt_BangKeChiTietThuPhi_CDHA
                ,Convert.ToDateTime(this.FromDate.Value)
                ,Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt16(this.Quarter.Value),
                Convert.ToInt16(this.Month.Value),
                Convert.ToInt16(this.Year.Value),
                Convert.ToInt16(this.flag.Value),
                Convert.ToInt16(this.StaffID.Value));
        }

        private void XRptBangKeChiTietThuPhiXetNghiemCDHA_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
