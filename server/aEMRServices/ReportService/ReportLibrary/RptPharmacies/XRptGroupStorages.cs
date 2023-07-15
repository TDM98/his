using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptGroupStorages : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptGroupStorages()
        {
            InitializeComponent();
            FillDataInit();
        }
        private void FillDataInit()
        {
            groupStorageTableAdapter.Fill((this.DataSource as DataSchema.dsSumRemainByStorages).GroupStorage, 0, null, Convert.ToDateTime("01/01/2010"), Convert.ToDateTime("01/01/2010"));
        }
        private void FillData()
        {
            groupStorageTableAdapter.Fill((this.DataSource as DataSchema.dsSumRemainByStorages).GroupStorage, Convert.ToInt64(this.DrugID.Value), null, Convert.ToDateTime(this.FromDate.Value.ToString()), Convert.ToDateTime(this.ToDate.Value.ToString()));
        }
        //public string Convertdmytomdy(string value)
        //{
        //    string[] val = value.Split('/');
        //    if (val != null)
        //    {
        //        return val[1] + "/" + val[0] + '/' + val[2];
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}
        private void XRptGroupStorages_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
