using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRpt_BaoCaoHuyDTDT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoHuyDTDT()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_BaoCaoHuyDTDT1.EnforceConstraints = false;
            spXRpt_BaoCaoHuyDTDTTableAdapter.Fill(dsXRpt_BaoCaoHuyDTDT1.spXRpt_BaoCaoHuyDTDT
                , Convert.ToInt32(this.Quarter.Value)
                , Convert.ToInt32(this.Month.Value)
                , Convert.ToInt32(this.Year.Value)
                , Convert.ToByte(this.Flag.Value)
                , Convert.ToDateTime(this.FromDate.Value)
                , Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRpt_BaoCaoHuyDTDT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            switch (Convert.ToByte(this.Flag.Value))
            {
                case 0:
                    xrLabel4.Text = "Quý " + Convert.ToInt32(this.Quarter.Value) + " năm " + Convert.ToInt32(this.Year.Value);
                    break;
                case 1:
                    xrLabel4.Text = "Tháng " + Convert.ToInt32(this.Month.Value) + " năm " + Convert.ToInt32(this.Year.Value);
                    break;
                case 2:
                    xrLabel4.Text = "Từ ngày " + string.Format("{0:dd/MM/yyyy}",Convert.ToDateTime(this.FromDate.Value)) 
                        + " đến ngày " + string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.ToDate.Value));
                    break;
                
            }
        }
    }
}
