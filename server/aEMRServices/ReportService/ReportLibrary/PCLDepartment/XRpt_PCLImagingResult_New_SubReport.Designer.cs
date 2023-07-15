namespace eHCMS.ReportLib.PCLDepartment
{
    partial class XRpt_PCLImagingResult_New_SubReport
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
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.dsXRpt_PCLImagingResult_New_Sub1 = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sub();
            this.spXRpt_PCLImagingResult_New_SubTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubTableAdapters.spXRpt_PCLImagingResult_New_SubTableAdapter();
            this.PCLImgResultID = new DevExpress.XtraReports.Parameters.Parameter();
            this.V_PCLRequestType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrTable11 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow11 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell35 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell37 = new DevExpress.XtraReports.UI.XRTableCell();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sub1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.Detail.HeightF = 23F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable1
            // 
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTable1.ForeColor = System.Drawing.Color.Maroon;
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(5F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(408F, 23F);
            this.xrTable1.StylePriority.UseBorders = false;
            this.xrTable1.StylePriority.UseFont = false;
            this.xrTable1.StylePriority.UseForeColor = false;
            this.xrTable1.StylePriority.UseTextAlignment = false;
            this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(214)))), ((int)(((byte)(211)))));
            this.xrTableRow1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell3});
            this.xrTableRow1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrTableRow1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.StylePriority.UseBackColor = false;
            this.xrTableRow1.StylePriority.UseBorderColor = false;
            this.xrTableRow1.StylePriority.UseFont = false;
            this.xrTableRow1.StylePriority.UseForeColor = false;
            this.xrTableRow1.Weight = 1.3916644287109374D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell1.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PCLExamTestItemName]")});
            this.xrTableCell1.Font = new System.Drawing.Font("Times New Roman", 11.25F);
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseBackColor = false;
            this.xrTableCell1.StylePriority.UseBorders = false;
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.Text = "YÊU CẦU XÉT NGHIỆM";
            this.xrTableCell1.Weight = 0.27530135168582925D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell3.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Value]")});
            this.xrTableCell3.Font = new System.Drawing.Font("Times New Roman", 11.25F);
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.StylePriority.UseBackColor = false;
            this.xrTableCell3.StylePriority.UseBorders = false;
            this.xrTableCell3.StylePriority.UseFont = false;
            this.xrTableCell3.Text = "Kết quả";
            this.xrTableCell3.Weight = 0.360194902509692D;
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
            this.BottomMargin.HeightF = 7F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // dsXRpt_PCLImagingResult_New_Sub1
            // 
            this.dsXRpt_PCLImagingResult_New_Sub1.DataSetName = "dsXRpt_PCLImagingResult_New_Sub";
            this.dsXRpt_PCLImagingResult_New_Sub1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spXRpt_PCLImagingResult_New_SubTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_SubTableAdapter.ClearBeforeFill = true;
            // 
            // PCLImgResultID
            // 
            this.PCLImgResultID.Name = "PCLImgResultID";
            this.PCLImgResultID.Type = typeof(long);
            this.PCLImgResultID.ValueInfo = "0";
            // 
            // V_PCLRequestType
            // 
            this.V_PCLRequestType.Name = "V_PCLRequestType";
            this.V_PCLRequestType.Type = typeof(long);
            this.V_PCLRequestType.ValueInfo = "0";
            // 
            // parHospitalName
            // 
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parHospitalCode
            // 
            this.parHospitalCode.Name = "parHospitalCode";
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // parHospitalAddress
            // 
            this.parHospitalAddress.Name = "parHospitalAddress";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable11});
            this.ReportHeader.HeightF = 23F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrTable11
            // 
            this.xrTable11.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable11.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTable11.ForeColor = System.Drawing.Color.Maroon;
            this.xrTable11.LocationFloat = new DevExpress.Utils.PointFloat(5F, 0F);
            this.xrTable11.Name = "xrTable11";
            this.xrTable11.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow11});
            this.xrTable11.SizeF = new System.Drawing.SizeF(407.9999F, 23F);
            this.xrTable11.StylePriority.UseBorders = false;
            this.xrTable11.StylePriority.UseFont = false;
            this.xrTable11.StylePriority.UseForeColor = false;
            this.xrTable11.StylePriority.UseTextAlignment = false;
            this.xrTable11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow11
            // 
            this.xrTableRow11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(214)))), ((int)(((byte)(211)))));
            this.xrTableRow11.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.xrTableRow11.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell35,
            this.xrTableCell37});
            this.xrTableRow11.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrTableRow11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.xrTableRow11.Name = "xrTableRow11";
            this.xrTableRow11.StylePriority.UseBackColor = false;
            this.xrTableRow11.StylePriority.UseBorderColor = false;
            this.xrTableRow11.StylePriority.UseFont = false;
            this.xrTableRow11.StylePriority.UseForeColor = false;
            this.xrTableRow11.Weight = 1.3916644287109374D;
            // 
            // xrTableCell35
            // 
            this.xrTableCell35.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell35.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell35.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell35.Name = "xrTableCell35";
            this.xrTableCell35.StylePriority.UseBackColor = false;
            this.xrTableCell35.StylePriority.UseBorders = false;
            this.xrTableCell35.StylePriority.UseFont = false;
            this.xrTableCell35.Text = "YÊU CẦU";
            this.xrTableCell35.Weight = 0.27530128670605325D;
            // 
            // xrTableCell37
            // 
            this.xrTableCell37.BackColor = System.Drawing.Color.Transparent;
            this.xrTableCell37.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell37.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell37.Multiline = true;
            this.xrTableCell37.Name = "xrTableCell37";
            this.xrTableCell37.StylePriority.UseBackColor = false;
            this.xrTableCell37.StylePriority.UseBorders = false;
            this.xrTableCell37.StylePriority.UseFont = false;
            this.xrTableCell37.Text = "Kết quả";
            this.xrTableCell37.Weight = 0.36019487034784914D;
            // 
            // spXRpt_PCLImagingResult_NewTableAdapter
            // 
            this.spXRpt_PCLImagingResult_NewTableAdapter.ClearBeforeFill = true;
            // 
            // XRpt_PCLImagingResult_New_SubReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRpt_PCLImagingResult_New_Sub1});
            this.DataAdapter = this.spXRpt_PCLImagingResult_New_SubTableAdapter;
            this.DataMember = "spXRpt_PCLImagingResult_New_Sub";
            this.DataSource = this.dsXRpt_PCLImagingResult_New_Sub1;
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 7);
            this.PageHeight = 583;
            this.PageWidth = 413;
            this.PaperKind = System.Drawing.Printing.PaperKind.A6;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PCLImgResultID,
            this.V_PCLRequestType,
            this.parHospitalName,
            this.parHospitalCode,
            this.parDepartmentOfHealth,
            this.parHospitalAddress});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRpt_PCLImagingResult_New_SubReport_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sub1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsXRpt_PCLImagingResult_New_Sub dsXRpt_PCLImagingResult_New_Sub1;
        private DataSchema.dsXRpt_PCLImagingResult_New_SubTableAdapters.spXRpt_PCLImagingResult_New_SubTableAdapter spXRpt_PCLImagingResult_New_SubTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter PCLImgResultID;
        public DevExpress.XtraReports.Parameters.Parameter V_PCLRequestType;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTable xrTable11;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow11;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell35;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell37;
        private DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
    }
}
