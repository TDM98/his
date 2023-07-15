using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TinhHinhHoatDongDV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TinhHinhHoatDongDV()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            //dsTinhHinhHoatDongDV1.EnforceConstraints = false;
            //sp_Rpt_KT_TinhHinhHoatDongDVTableAdapter.Fill(dsTinhHinhHoatDongDV1.sp_Rpt_KT_TinhHinhHoatDongDV, 
            //    Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTinhHinhHoatDongDV1.sp_Rpt_KT_TinhHinhHoatDongDV, sp_Rpt_KT_TinhHinhHoatDongDVTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);

            xrTable2.Visible = (bool)parIsDetail.Value;
        }

        private void XRpt_TinhHinhHoatDongDV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
