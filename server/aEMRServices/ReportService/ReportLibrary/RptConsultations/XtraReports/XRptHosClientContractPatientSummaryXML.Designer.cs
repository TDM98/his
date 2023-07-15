using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptHosClientContractPatientSummaryXML
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.spRptMedicalInstructionTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.RptHosClientContractPatientSummary = new DevExpress.XtraReports.UI.XRSubreport();
            this.HosContractPtIDCollection = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.RptHosClientContractPatientSummary});
            this.Detail.HeightF = 16.99999F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 40F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 40F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // spRptMedicalInstructionTableAdapter
            // 
            this.spRptMedicalInstructionTableAdapter.ClearBeforeFill = true;
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 14.99999F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // RptHosClientContractPatientSummary
            // 
            this.RptHosClientContractPatientSummary.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.RptHosClientContractPatientSummary.Name = "RptHosClientContractPatientSummary";
            this.RptHosClientContractPatientSummary.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptHosClientContractPatientSummary();
            this.RptHosClientContractPatientSummary.SizeF = new System.Drawing.SizeF(543F, 15F);
            this.RptHosClientContractPatientSummary.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.RptHosClientContractPatientSummary_BeforePrint);
            // 
            // HosContractPtIDCollection
            // 
            this.HosContractPtIDCollection.Description = "HosContractPtIDCollection";
            this.HosContractPtIDCollection.Name = "HosContractPtIDCollection";
            // 
            // XRptHosClientContractPatientSummaryXML
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 40, 40);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.HosContractPtIDCollection});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Report_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.UI.XRSubreport RptHosClientContractPatientSummary;
        private DevExpress.XtraReports.Parameters.Parameter HosContractPtIDCollection;
    }
}