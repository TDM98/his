namespace eHCMS.ReportLib.PCLDepartment
{
    partial class XRpt_PCLImagingResult_New_0_Hinh_XN_SubReport
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
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
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
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sub1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLabel2});
            this.Detail.HeightF = 40F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.CanShrink = true;
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "iif([IsTechnique],null,Iif(IsNullOrEmpty([Value]),null,[Value]))")});
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrLabel5.ForeColor = System.Drawing.Color.Black;
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(10.00006F, 20F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel5.SizeF = new System.Drawing.SizeF(767F, 20F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseForeColor = false;
            // 
            // xrLabel2
            // 
            this.xrLabel2.CanShrink = true;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "iif([IsTechnique],null,Iif([IsBold],[PCLExamTestItemName],null))")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.ForeColor = System.Drawing.Color.Black;
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel2.SizeF = new System.Drawing.SizeF(767F, 20F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseForeColor = false;
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
            this.BottomMargin.HeightF = 9.083366F;
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
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel1});
            this.ReportHeader.HeightF = 60F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([IsTechnique],\'Kỹ thuật\',null)")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.xrLabel4.ForeColor = System.Drawing.Color.Blue;
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel4.SizeF = new System.Drawing.SizeF(100F, 20F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseForeColor = false;
            this.xrLabel4.Text = "MÔ TẢ";
            // 
            // xrLabel3
            // 
            this.xrLabel3.CanShrink = true;
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([IsTechnique],[Value],null)")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrLabel3.ForeColor = System.Drawing.Color.Black;
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 20F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel3.SizeF = new System.Drawing.SizeF(767F, 20F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseForeColor = false;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "iif([DataSource.RowCount]= 1,\'\',\'Mô tả\')")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 13F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.xrLabel1.ForeColor = System.Drawing.Color.Blue;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 40F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(100F, 20F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseForeColor = false;
            this.xrLabel1.Text = "Mô tả";
            // 
            // spXRpt_PCLImagingResult_NewTableAdapter
            // 
            this.spXRpt_PCLImagingResult_NewTableAdapter.ClearBeforeFill = true;
            // 
            // XRpt_PCLImagingResult_New_0_Hinh_XN_SubReport
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
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 0, 9);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
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
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sub1)).EndInit();
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
        private DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
    }
}
