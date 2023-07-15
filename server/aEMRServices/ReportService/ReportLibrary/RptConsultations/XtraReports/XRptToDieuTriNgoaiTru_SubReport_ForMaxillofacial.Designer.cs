using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptToDieuTriNgoaiTru_SubReport_ForMaxillofacial
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
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.PtRegDetailID = new DevExpress.XtraReports.Parameters.Parameter();
            this.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacialTableAdapters.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter();
            this.dsToDieuTriNgoaiTru_ForMaxillofacial1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacial();
            this.spRptMaxillofacialOutPtMedicalFileTableAdapter1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsMaxillofacialOutPtMedicalFileFrontCoverTableAdapters.spRptMaxillofacialOutPtMedicalFileTableAdapter();
            this.spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacialTableAdapters.spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsToDieuTriNgoaiTru_ForMaxillofacial1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.Detail.HeightF = 25.8222F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable1
            // 
            this.xrTable1.BackColor = System.Drawing.Color.Transparent;
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable1.SizeF = new System.Drawing.SizeF(753.29F, 25.8222F);
            this.xrTable1.StylePriority.UseFont = false;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell5,
            this.xrTableCell6,
            this.xrTableCell7});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 0.28943354119970416D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Ngay]")});
            this.xrTableCell5.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTableCell5.Multiline = true;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrTableCell5.StylePriority.UseBorders = false;
            this.xrTableCell5.StylePriority.UseFont = false;
            this.xrTableCell5.StylePriority.UsePadding = false;
            this.xrTableCell5.StylePriority.UseTextAlignment = false;
            this.xrTableCell5.Text = "NGÀY";
            this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell5.Weight = 1.8513152066965908D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)));
            this.xrTableCell6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DienTien]")});
            this.xrTableCell6.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTableCell6.Multiline = true;
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrTableCell6.StylePriority.UseBorders = false;
            this.xrTableCell6.StylePriority.UseFont = false;
            this.xrTableCell6.StylePriority.UsePadding = false;
            this.xrTableCell6.StylePriority.UseTextAlignment = false;
            this.xrTableCell6.Text = "DIỄN TIẾN BỆNH";
            this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell6.Weight = 7.355225457998988D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[YLenh]")});
            this.xrTableCell7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTableCell7.Multiline = true;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrTableCell7.StylePriority.UseBorders = false;
            this.xrTableCell7.StylePriority.UseFont = false;
            this.xrTableCell7.StylePriority.UsePadding = false;
            this.xrTableCell7.StylePriority.UseTextAlignment = false;
            this.xrTableCell7.Text = "Y LỆNH";
            this.xrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell7.TextTrimming = System.Drawing.StringTrimming.None;
            this.xrTableCell7.Weight = 9.6390984285610255D;
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
            // GroupFooter1
            // 
            this.GroupFooter1.HeightF = 0F;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 0F;
            this.PageHeader.Name = "PageHeader";
            // 
            // PtRegDetailID
            // 
            this.PtRegDetailID.Name = "PtRegDetailID";
            this.PtRegDetailID.Type = typeof(long);
            this.PtRegDetailID.ValueInfo = "0";
            // 
            // spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter
            // 
            this.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter.ClearBeforeFill = true;
            // 
            // dsToDieuTriNgoaiTru_ForMaxillofacial1
            // 
            this.dsToDieuTriNgoaiTru_ForMaxillofacial1.DataSetName = "dsToDieuTriNgoaiTru_ForMaxillofacial";
            this.dsToDieuTriNgoaiTru_ForMaxillofacial1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spRptMaxillofacialOutPtMedicalFileTableAdapter1
            // 
            this.spRptMaxillofacialOutPtMedicalFileTableAdapter1.ClearBeforeFill = true;
            // 
            // spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter
            // 
            this.spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter.ClearBeforeFill = true;
            // 
            // XRptToDieuTriNgoaiTru_SubReport_ForMaxillofacial
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.GroupFooter1,
            this.PageHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsToDieuTriNgoaiTru_ForMaxillofacial1});
            this.DataAdapter = this.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter;
            this.DataMember = "spRptToDieuTriNgoaiTru_Sub_ForMaxillofacial";
            this.DataSource = this.dsToDieuTriNgoaiTru_ForMaxillofacial1;
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 754;
            this.PageWidth = 755;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegDetailID});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Report_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsToDieuTriNgoaiTru_ForMaxillofacial1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter spRptToDieuTriNgoaiTruTableAdapter;
        private DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter spRptGeneralOutPtMedicalFileTableAdapter;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTru_SubTableAdapter spRptToDieuTriNgoaiTru_SubTableAdapter;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DataSchema.dsMaxillofacialOutPtMedicalFileFrontCoverTableAdapters.spRptMaxillofacialOutPtMedicalFileTableAdapter spRptMaxillofacialOutPtMedicalFileTableAdapter;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
        private DevExpress.XtraReports.Parameters.Parameter PtRegDetailID;
        private DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacialTableAdapters.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter;
        private DataSchema.dsTransferFormTableAdapters.spGetTransferFormTableAdapter spGetTransferFormTableAdapter;
        private DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacial dsToDieuTriNgoaiTru_ForMaxillofacial1;
        private DataSchema.dsMaxillofacialOutPtMedicalFileFrontCoverTableAdapters.spRptMaxillofacialOutPtMedicalFileTableAdapter spRptMaxillofacialOutPtMedicalFileTableAdapter1;
        private DataSchema.dsToDieuTriNgoaiTru_ForMaxillofacialTableAdapters.spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter;
    }
}