using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescription : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescription()
        {
            InitializeComponent();
            FillData();
        }

        public bool isPrinted = false;

        public void FillData()
        {
            ePrescript_PtInfoTableAdapter.ClearBeforeFill = true;
            this.ePrescript_PrescriptHeaderTableAdapter.ClearBeforeFill = true;
            this.ePrescript_PrescriptDetailsTableAdapter.ClearBeforeFill = true;
            this.ePrescript_PtInfoTableAdapter.Fill(this.dsPrescription1.EPrescript_PtInfo, this.parPatientID.Value as long?);
            this.ePrescript_PrescriptHeaderTableAdapter.Fill(this.dsPrescription1.EPrescript_PrescriptHeader, this.parPrescriptID.Value as long?);
            this.ePrescript_PrescriptDetailsTableAdapter.Fill(this.dsPrescription1.EPrescript_PrescriptDetails, this.parPrescriptID.Value as long?);

        }

        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
