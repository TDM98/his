using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoDoChucNangHoHapNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoDoChucNangHoHapNew()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoDoChucNangHoHap1.EnforceConstraints = false;
            spRptSoDoChucNangHoHapTableAdapter.Fill(dsSoDoChucNangHoHap1.spRptSoDoChucNangHoHap
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );

            if (Convert.ToInt64(V_HealthRecordsType.Value) == 891001)
            {
                xrTable1.Visible = true;
                xrTable2.Visible = true;
                xrTable4.Visible = false;
                xrTable6.Visible = false;
            }
            else
            {
                xrTable1.Visible = false;
                xrTable2.Visible = false;
                xrTable4.Visible = true;
                xrTable6.Visible = true;
            }
        }

        private void XRptSoDoChucNangHoHapNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
