using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptResources
{
    public partial class XRptResources : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptResources()
        {
            InitializeComponent();
        }
        private void InitData()
        {
            spRpt_ResourceForEarchTypeTableAdapter.Fill((this.DataSource as DataSchema.dsResources).spRpt_ResourceForEarchType,0,0,0,false);
        }
        private void FillData()
        {
            spRpt_ResourceForEarchTypeTableAdapter.Fill((this.DataSource as DataSchema.dsResources).spRpt_ResourceForEarchType, this.GroupID.Value as Int64?, this.TypeID.Value as Int64?, this.SupplierID.Value as Int64?, this.IsDeleted.Value as Boolean?);
        }
        private void XRptResources_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
