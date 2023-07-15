using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoGiaoBan : XtraReport
    {
        public XRptBaoCaoGiaoBan()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoGiaoBan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoGiaoBan1.EnforceConstraints = false;
            spDSCaKhamTheoPhongTableAdapter.Fill(dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
            spDSCaKhamTheoPhongSummaryTableAdapter.Fill(dsBaoCaoGiaoBan1.spDSCaKhamTheoPhongSummary, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));

            float totalPhongKham = 0;
            if (dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong != null && dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong.Rows.Count > 0)
            {
                for(int i=0; i< dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong.Rows.Count; i++)
                {
                    totalPhongKham += Convert.ToInt64(dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong.Rows[i]["SoCa"]);
                }
                xrLabel9.Text = totalPhongKham.ToString();
            }
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptBaoCaoGiaoBan_Bsy)((XRSubreport)sender).ReportSource).parFromDate.Value = Convert.ToDateTime(parFromDate.Value);
            ((XRptBaoCaoGiaoBan_Bsy)((XRSubreport)sender).ReportSource).parToDate.Value = Convert.ToDateTime(parToDate.Value);
        }

        private void XrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptBaoCaoGiaoBan_Phong)((XRSubreport)sender).ReportSource).parFromDate.Value = Convert.ToDateTime(parFromDate.Value);
            ((XRptBaoCaoGiaoBan_Phong)((XRSubreport)sender).ReportSource).parToDate.Value = Convert.ToDateTime(parToDate.Value);
        }
    }
}
