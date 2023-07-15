namespace eHCMS.ReportLib.PCLDepartment
{
    partial class XRpt_PCLImagingResult_New_3_Hinh_SubReport_Image
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
            this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.spXRpt_PCLImagingResult_New_SubTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubTableAdapters.spXRpt_PCLImagingResult_New_SubTableAdapter();
            this.PCLImgResultID = new DevExpress.XtraReports.Parameters.Parameter();
            this.V_PCLRequestType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1 = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubReport_Image();
            this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_SubReport_Image1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox1});
            this.Detail.HeightF = 171.0417F;
            this.Detail.MultiColumn.ColumnCount = 3;
            this.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
            this.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPictureBox1
            // 
            this.xrPictureBox1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'/\'+[PCLResultFileName]")});
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(20F, 0F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(223F, 171F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
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
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
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
            this.ReportHeader.HeightF = 0.08333524F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // spXRpt_PCLImagingResult_NewTableAdapter
            // 
            this.spXRpt_PCLImagingResult_NewTableAdapter.ClearBeforeFill = true;
            // 
            // dsXRpt_PCLImagingResult_New_SubReport_Image1
            // 
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1.DataSetName = "dsXRpt_PCLImagingResult_New_SubReport_Image";
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter.ClearBeforeFill = true;
            // 
            // XRpt_PCLImagingResult_New_3_Hinh_SubReport_Image
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1});
            this.DataAdapter = this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter;
            this.DataMember = "spXRpt_PCLImagingResult_New_Sub_Image";
            this.DataSource = this.dsXRpt_PCLImagingResult_New_SubReport_Image1;
            this.Margins = new System.Drawing.Printing.Margins(0, 40, 0, 30);
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
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_SubReport_Image1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsXRpt_PCLImagingResult_New_SubTableAdapters.spXRpt_PCLImagingResult_New_SubTableAdapter spXRpt_PCLImagingResult_New_SubTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter PCLImgResultID;
        public DevExpress.XtraReports.Parameters.Parameter V_PCLRequestType;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
        private DataSchema.dsXRpt_PCLImagingResult_New_SubReport_Image dsXRpt_PCLImagingResult_New_SubReport_Image1;
        private DataSchema.dsXRpt_PCLImagingResult_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox1;
    }
}
