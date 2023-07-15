using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptSoDienTimNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoDienTimNew()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoDienTim1.EnforceConstraints = false;
            spRptSoDienTimTableAdapter.Fill(dsSoDienTim1.spRptSoDienTim    
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


        private void XRptSoDienTimNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
