using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptToDieuTriNgoaiTru_CLS_SubReport
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
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.spRptPCLRequestForDieuTriNgoaiTruTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptPCLRequestForDieuTriNgoaiTruTableAdapter();
            this.OutPtTreatmentProgramID = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsRptToDieuTriNgoaiTru1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTru();
            this.PtRegDetailID = new DevExpress.XtraReports.Parameters.Parameter();
            this.spRptGeneralOutPtMedicalFileTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter();
            this.spRptToDieuTriNgoaiTruTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.spRptMedicalInstructionTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsRptToDieuTriNgoaiTru1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7});
            this.Detail.HeightF = 25.08333F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel7
            // 
            this.xrLabel7.CanShrink = true;
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PCLExamTypeName]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(21.45831F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(338.5417F, 25.08333F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UsePadding = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ReportFooter
            // 
            this.ReportFooter.HeightF = 0F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // spRptPCLRequestForDieuTriNgoaiTruTableAdapter
            // 
            this.spRptPCLRequestForDieuTriNgoaiTruTableAdapter.ClearBeforeFill = true;
            // 
            // OutPtTreatmentProgramID
            // 
            this.OutPtTreatmentProgramID.Description = "Parameter1";
            this.OutPtTreatmentProgramID.Name = "OutPtTreatmentProgramID";
            this.OutPtTreatmentProgramID.Type = typeof(long);
            this.OutPtTreatmentProgramID.ValueInfo = "0";
            // 
            // dsRptToDieuTriNgoaiTru1
            // 
            this.dsRptToDieuTriNgoaiTru1.DataSetName = "dsRptToDieuTriNgoaiTru";
            this.dsRptToDieuTriNgoaiTru1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // PtRegDetailID
            // 
            this.PtRegDetailID.Description = "Parameter1";
            this.PtRegDetailID.Name = "PtRegDetailID";
            this.PtRegDetailID.Type = typeof(long);
            this.PtRegDetailID.ValueInfo = "0";
            // 
            // spRptGeneralOutPtMedicalFileTableAdapter
            // 
            this.spRptGeneralOutPtMedicalFileTableAdapter.ClearBeforeFill = true;
            // 
            // spRptToDieuTriNgoaiTruTableAdapter
            // 
            this.spRptToDieuTriNgoaiTruTableAdapter.ClearBeforeFill = true;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2});
            this.ReportHeader.HeightF = 37.5F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.CanShrink = true;
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(9.999974F, 10.00001F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(122.9167F, 23F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Cận lâm sàng:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // spRptMedicalInstructionTableAdapter
            // 
            this.spRptMedicalInstructionTableAdapter.ClearBeforeFill = true;
            // 
            // XRptToDieuTriNgoaiTru_CLS_SubReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportFooter,
            this.ReportHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsRptToDieuTriNgoaiTru1});
            this.DataAdapter = this.spRptPCLRequestForDieuTriNgoaiTruTableAdapter;
            this.DataMember = "spRptPCLRequestForDieuTriNgoaiTru";
            this.DataSource = this.dsRptToDieuTriNgoaiTru1;
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.OutPtTreatmentProgramID,
            this.PtRegDetailID});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Report_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsRptToDieuTriNgoaiTru1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptPCLRequestForDieuTriNgoaiTruTableAdapter spRptPCLRequestForDieuTriNgoaiTruTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter OutPtTreatmentProgramID;
        private DataSchema.dsRptToDieuTriNgoaiTru dsRptToDieuTriNgoaiTru1;
        private DevExpress.XtraReports.Parameters.Parameter PtRegDetailID;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter spRptGeneralOutPtMedicalFileTableAdapter;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter spRptToDieuTriNgoaiTruTableAdapter;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
    }
}