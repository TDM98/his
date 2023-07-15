using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptCardStorage : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCardStorage()
        {
            InitializeComponent();
            //FillData();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XRptCardStorageDetail)((XRSubreport)sender).ReportSource).DrugID.Value = GetCurrentColumnValue("DrugID");
            //((XRptCardStorageDetail)((XRSubreport)sender).ReportSource).inviID.Value = GetCurrentColumnValue("inviID");
            //((XRptCardStorageDetail)((XRSubreport)sender).ReportSource).outiID.Value = GetCurrentColumnValue("OutiID");
        }
        private void FillData()
        {
            spRpt_CardStorageTableAdapter.Fill(dsCardStorage1.spRpt_CardStorage,Convert.ToInt64(this.StoreID.Value), Convert.ToInt64(this.DrugID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsCardStorage1.spRpt_CardStorage != null && dsCardStorage1.spRpt_CardStorage.Rows.Count > 0)
            {
                this.Parameters["StockFinal"].Value = Convert.ToDecimal(dsCardStorage1.spRpt_CardStorage.Rows[dsCardStorage1.spRpt_CardStorage.Rows.Count - 1]["QtyStocks"]);
            }
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XSRptShowDescriptionCardStorage)((XRSubreport)sender).ReportSource).Ten.Value = GetCurrentColumnValue("FullName");
            //((XSRptShowDescriptionCardStorage)((XRSubreport)sender).ReportSource).Address.Value = GetCurrentColumnValue("DC");
        }

    }
}
