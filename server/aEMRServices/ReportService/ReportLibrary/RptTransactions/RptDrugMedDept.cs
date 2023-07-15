using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptDrugMedDept : DevExpress.XtraReports.UI.XtraReport
    {
        public RptDrugMedDept()
        {
            InitializeComponent();
        }

        private void RptDrugMedDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spSoKiemNhapThuocTableAdapter.Fill(dsRptDrugMedDept1.spSoKiemNhapThuoc
                                                    ,Convert.ToDateTime(this.FromDate.Value)
                                                    , Convert.ToDateTime(this.ToDate.Value)
                                                    , Convert.ToInt32(this.Quarter.Value)
                                                    , Convert.ToInt32(this.Month.Value)
                                                    , Convert.ToInt32(this.Year.Value)
                                                    , Convert.ToByte(this.Flag.Value)
                                                    , Convert.ToInt32(this.V_MedProductType.Value)
                                                    ,Convert.ToInt32(this.DeptID.Value));
        }

    }
}
