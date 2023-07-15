using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoThuThuatNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoThuThuatNew()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoThuThuat1.EnforceConstraints = false;
            spRptSoThuThuatTableAdapter.Fill(dsSoThuThuat1.spRptSoThuThuat    
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

        private void XRptSoThuThuatNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
