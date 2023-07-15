using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptInOutStocks : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStocks()
        {
            InitializeComponent();           
        }

        public void FillData()
        {

            //dsInOutStocks1.Clear();
            //dsInOutStocks1.EnforceConstraints = false;
            //stocksTableAdapter.Fill(dsInOutStocks1.spRpt_InOutStocks, Convert.ToInt64(StoreID.Value),Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()));
            //if (dsInOutStocks1.Stocks.Count == 0)
            //{
            //    dsInOutStocks1.Stocks.Rows.Add(new object[10]{0,0,"","",0,0,0,0,0,""});
            //}
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocks1.spRpt_InOutStocks, spRpt_InOutStocksTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value),Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }

        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
