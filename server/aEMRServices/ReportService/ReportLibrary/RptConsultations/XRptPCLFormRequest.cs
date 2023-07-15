using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPCLFormRequest : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPCLFormRequest()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            long? pPtPCLAppID = null;
            long? pPCLFormID = null;
            try
            {
                pPtPCLAppID = long.Parse(this.parPtPCLAppID.Value.ToString());
            }
            catch
            {
                pPtPCLAppID = null;
            }
            try
            {
                pPCLFormID = long.Parse(this.parPCLFormID.Value.ToString());
            }
            catch
            {
                pPCLFormID = null;
            }

            this.spRpt_GetHeaderInfoByIDTblAdapter.ClearBeforeFill = true;
            this.spRpt_GetHeaderInfoByIDTblAdapter.Fill(this.dsPtPCLAppointment.spRpt_GetHeaderInfoByID, pPtPCLAppID);
            this.spRpt_GetFullInfoByIDTblAdapter.ClearBeforeFill = true;
            this.spRpt_GetFullInfoByIDTblAdapter.Fill(this.dsPtPCLAppointment.spRpt_GetFullInfoByID, pPtPCLAppID, pPCLFormID);
            
        }

        private void XRptPCLFormRequest_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
