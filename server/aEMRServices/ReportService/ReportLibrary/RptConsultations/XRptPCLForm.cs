using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPCLForm : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPCLForm()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            long? pPatientID = null;
            long? pPCLForm = null;
            try
            {
                pPatientID = long.Parse(this.parPatientID.Value.ToString());
            }
            catch
            {
                pPatientID = null;
            }
            try
            {
                pPCLForm = long.Parse(this.parPCLFormID.Value.ToString());
            }
            catch
            {
                pPCLForm = null;
            }
            this.getHeaderInfoByPtIDTableAdapter.ClearBeforeFill = true;
            this.getHeaderInfoByPtIDTableAdapter.Fill(this.dsPtPCLForm.spRpt_GetHeaderInfoByPtID, pPatientID, pPCLForm);
            this.getBlankFormByIDTableAdapter.ClearBeforeFill = true;
            this.getBlankFormByIDTableAdapter.Fill(this.dsPtPCLForm.spRpt_GetBlankFormByID, pPCLForm);
        }

        private void XRptPCLForm_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
