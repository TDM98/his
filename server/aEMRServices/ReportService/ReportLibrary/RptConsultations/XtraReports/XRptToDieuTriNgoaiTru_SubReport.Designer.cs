using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptToDieuTriNgoaiTru_SubReport
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel59 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel46 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel45 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrPictureBox2 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.spRptToDieuTriNgoaiTru_SubTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTru_SubTableAdapter();
            this.OutPtTreatmentProgramID = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsRptToDieuTriNgoaiTru1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTru();
            this.PtRegDetailID = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.spRptGeneralOutPtMedicalFileTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter();
            this.spRptToDieuTriNgoaiTruTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter();
            this.spRptMedicalInstructionTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsRptToDieuTriNgoaiTru1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel59,
            this.xrLabel11,
            this.xrLabel12,
            this.xrLabel46,
            this.xrLabel45});
            this.Detail.HeightF = 46F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel59
            // 
            this.xrLabel59.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Row] + \'. \' + [BrandName]")});
            this.xrLabel59.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.xrLabel59.LocationFloat = new DevExpress.Utils.PointFloat(21.45834F, 0F);
            this.xrLabel59.Name = "xrLabel59";
            this.xrLabel59.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel59.SizeF = new System.Drawing.SizeF(329.9167F, 23F);
            this.xrLabel59.StylePriority.UseFont = false;
            this.xrLabel59.StylePriority.UseTextAlignment = false;
            xrSummary1.FormatString = "{0:#.}";
            xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.RecordNumber;
            this.xrLabel59.Summary = xrSummary1;
            this.xrLabel59.Text = "xrLabel59";
            this.xrLabel59.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel11
            // 
            this.xrLabel11.AutoWidth = true;
            this.xrLabel11.CanShrink = true;
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MDose]")});
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(41.25001F, 22.99999F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.Text = "xrLabel11";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel11.WordWrap = false;
            // 
            // xrLabel12
            // 
            this.xrLabel12.AutoWidth = true;
            this.xrLabel12.CanShrink = true;
            this.xrLabel12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ADose]")});
            this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(116.25F, 22.99999F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "xrLabel12";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel12.WordWrap = false;
            // 
            // xrLabel46
            // 
            this.xrLabel46.AutoWidth = true;
            this.xrLabel46.CanShrink = true;
            this.xrLabel46.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NDose]")});
            this.xrLabel46.LocationFloat = new DevExpress.Utils.PointFloat(276.375F, 23F);
            this.xrLabel46.Name = "xrLabel46";
            this.xrLabel46.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel46.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrLabel46.StylePriority.UseFont = false;
            this.xrLabel46.StylePriority.UseTextAlignment = false;
            this.xrLabel46.Text = "xrLabel46";
            this.xrLabel46.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel46.WordWrap = false;
            // 
            // xrLabel45
            // 
            this.xrLabel45.AutoWidth = true;
            this.xrLabel45.CanShrink = true;
            this.xrLabel45.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EDose]")});
            this.xrLabel45.LocationFloat = new DevExpress.Utils.PointFloat(201.375F, 23F);
            this.xrLabel45.Name = "xrLabel45";
            this.xrLabel45.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel45.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrLabel45.StylePriority.UseFont = false;
            this.xrLabel45.StylePriority.UseTextAlignment = false;
            this.xrLabel45.Text = "xrLabel45";
            this.xrLabel45.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel45.WordWrap = false;
            // 
            // xrLabel24
            // 
            this.xrLabel24.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DayRpts]")});
            this.xrLabel24.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(266.2782F, 27.60958F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.StylePriority.UseTextAlignment = false;
            this.xrLabel24.Text = "xrLabel24";
            this.xrLabel24.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel24.Visible = false;
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
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox2,
            this.xrLabel23,
            this.xrLabel1});
            this.ReportFooter.HeightF = 103.224F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrPictureBox2
            // 
            this.xrPictureBox2.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
            this.xrPictureBox2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrPictureBox2.BorderWidth = 1F;
            this.xrPictureBox2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[SignImageURL]")});
            this.xrPictureBox2.LocationFloat = new DevExpress.Utils.PointFloat(63.45851F, 25.08332F);
            this.xrPictureBox2.Name = "xrPictureBox2";
            this.xrPictureBox2.SizeF = new System.Drawing.SizeF(287.9166F, 50F);
            this.xrPictureBox2.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Squeeze;
            this.xrPictureBox2.StylePriority.UseBorderDashStyle = false;
            this.xrPictureBox2.StylePriority.UseBorders = false;
            this.xrPictureBox2.StylePriority.UseBorderWidth = false;
            // 
            // xrLabel23
            // 
            this.xrLabel23.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(63.4584F, 0F);
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(287.9166F, 25.08332F);
            this.xrLabel23.StylePriority.UseFont = false;
            this.xrLabel23.StylePriority.UsePadding = false;
            this.xrLabel23.StylePriority.UseTextAlignment = false;
            this.xrLabel23.Text = "Bác sĩ y lệnh";
            this.xrLabel23.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DoctorFullName]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(63.45851F, 75.08332F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(287.9165F, 25.08331F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UsePadding = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // spRptToDieuTriNgoaiTru_SubTableAdapter
            // 
            this.spRptToDieuTriNgoaiTru_SubTableAdapter.ClearBeforeFill = true;
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
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3,
            this.xrLabel24,
            this.xrLabel31,
            this.xrLabel2});
            this.ReportHeader.HeightF = 53.31791F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 4.609581F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(84.375F, 23F);
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Y lệnh thuốc:";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel31
            // 
            this.xrLabel31.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DayRpts]")});
            this.xrLabel31.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(94.37501F, 27.60958F);
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(39.00204F, 23F);
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.StylePriority.UsePadding = false;
            this.xrLabel31.StylePriority.UseTextAlignment = false;
            this.xrLabel31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 27.60958F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(84.375F, 23F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Ngày dùng:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // spRptGeneralOutPtMedicalFileTableAdapter
            // 
            this.spRptGeneralOutPtMedicalFileTableAdapter.ClearBeforeFill = true;
            // 
            // spRptToDieuTriNgoaiTruTableAdapter
            // 
            this.spRptToDieuTriNgoaiTruTableAdapter.ClearBeforeFill = true;
            // 
            // spRptMedicalInstructionTableAdapter
            // 
            this.spRptMedicalInstructionTableAdapter.ClearBeforeFill = true;
            // 
            // XRptToDieuTriNgoaiTru_SubReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportFooter,
            this.ReportHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsRptToDieuTriNgoaiTru1});
            this.DataAdapter = this.spRptToDieuTriNgoaiTru_SubTableAdapter;
            this.DataMember = "spRptToDieuTriNgoaiTru_Sub";
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
        private RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTru_SubTableAdapter spRptToDieuTriNgoaiTru_SubTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter OutPtTreatmentProgramID;
        private DataSchema.dsRptToDieuTriNgoaiTru dsRptToDieuTriNgoaiTru1;
        private DevExpress.XtraReports.Parameters.Parameter PtRegDetailID;
        private DevExpress.XtraReports.UI.XRLabel xrLabel59;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel46;
        private DevExpress.XtraReports.UI.XRLabel xrLabel45;
        private DevExpress.XtraReports.UI.XRLabel xrLabel24;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel31;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter spRptGeneralOutPtMedicalFileTableAdapter;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter spRptToDieuTriNgoaiTruTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel23;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox2;
    }
}