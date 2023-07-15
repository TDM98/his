using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptStoreDept
{
    public partial class XRptStaffDeptPresence : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptStaffDeptPresence()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            try
            {
                dsStaffDeptPresence.EnforceConstraints = false;
                spGetStaffDeptPresenceTableAdapter.Fill(this.dsStaffDeptPresence.spGetStaffDeptPresence, (DateTime)this.StaffCountDate.Value, (int)this.DeptID.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                var mDetail = sender as DetailBand;
                var DeptID = (long)mDetail.Report.GetCurrentColumnValue("DeptID");
                if (DeptID == 95)
                {
                    xrTable2.Visible = false;
                    xrTable3.Visible = true;
                }
                else
                {
                    xrTable2.Visible = true;
                    xrTable3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void XRptStaffDeptPresence_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
