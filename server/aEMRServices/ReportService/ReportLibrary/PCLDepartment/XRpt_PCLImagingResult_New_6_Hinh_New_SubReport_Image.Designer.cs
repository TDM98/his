namespace eHCMS.ReportLib.PCLDepartment
{
    partial class XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_Image
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
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            DevExpress.XtraPrinting.Shape.ShapeRectangle shapeRectangle1 = new DevExpress.XtraPrinting.Shape.ShapeRectangle();
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
            this.xrPictureBox6 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel53 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel55 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel54 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrShape1 = new DevExpress.XtraReports.UI.XRShape();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1 = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubReport_Image();
            this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_SubReport_Image1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox1});
            this.Detail.HeightF = 290.4167F;
            this.Detail.MultiColumn.ColumnCount = 2;
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
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(8.957581F, 0F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(370F, 280F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 20F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 21F;
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
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox6,
            this.xrLabel24,
            this.xrBarCode1,
            this.xrLabel61,
            this.xrLabel19,
            this.xrLabel53,
            this.xrLabel55,
            this.xrLabel54,
            this.xrShape1});
            this.ReportHeader.HeightF = 207.8751F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrPictureBox6
            // 
            this.xrPictureBox6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[LogoUrl]")});
            this.xrPictureBox6.LocationFloat = new DevExpress.Utils.PointFloat(570.6917F, 10.00001F);
            this.xrPictureBox6.Name = "xrPictureBox6";
            this.xrPictureBox6.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox6.SizeF = new System.Drawing.SizeF(180F, 60F);
            this.xrPictureBox6.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrLabel24
            // 
            this.xrLabel24.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel24.ForeColor = System.Drawing.Color.Blue;
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(8.957545F, 160.1668F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(778.0425F, 29.99998F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.StylePriority.UseForeColor = false;
            this.xrLabel24.StylePriority.UseTextAlignment = false;
            this.xrLabel24.Text = "DANH SÁCH HÌNH ẢNH CHẨN ĐOÁN";
            this.xrLabel24.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PCLRequestNumID]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(558.1917F, 91.99991F);
            this.xrBarCode1.Module = 1F;
            this.xrBarCode1.Name = "xrBarCode1";
            this.xrBarCode1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrBarCode1.ShowText = false;
            this.xrBarCode1.SizeF = new System.Drawing.SizeF(110.1939F, 34F);
            this.xrBarCode1.StylePriority.UseFont = false;
            this.xrBarCode1.StylePriority.UsePadding = false;
            this.xrBarCode1.StylePriority.UseTextAlignment = false;
            this.xrBarCode1.Symbology = code128Generator1;
            this.xrBarCode1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel61
            // 
            this.xrLabel61.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'ID: \' + [PatientCode]")});
            this.xrLabel61.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(668.3856F, 91.99991F);
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(118.6144F, 17F);
            this.xrLabel61.StylePriority.UseFont = false;
            this.xrLabel61.StylePriority.UsePadding = false;
            this.xrLabel61.StylePriority.UseTextAlignment = false;
            this.xrLabel61.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel19
            // 
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'BA: \'")});
            this.xrLabel19.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(668.3856F, 108.9999F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(118.6144F, 17.00001F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UsePadding = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel53
            // 
            this.xrLabel53.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel53.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parDepartmentOfHealth]")});
            this.xrLabel53.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel53.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(134)))), ((int)(((byte)(18)))));
            this.xrLabel53.LocationFloat = new DevExpress.Utils.PointFloat(11.0376F, 0F);
            this.xrLabel53.Name = "xrLabel53";
            this.xrLabel53.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel53.SizeF = new System.Drawing.SizeF(508.6123F, 40F);
            this.xrLabel53.StylePriority.UseBorders = false;
            this.xrLabel53.StylePriority.UseFont = false;
            this.xrLabel53.StylePriority.UseForeColor = false;
            this.xrLabel53.StylePriority.UseTextAlignment = false;
            this.xrLabel53.Text = "xrLabel4";
            this.xrLabel53.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrLabel55
            // 
            this.xrLabel55.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel55.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalAddress]")});
            this.xrLabel55.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel55.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(134)))), ((int)(((byte)(18)))));
            this.xrLabel55.LocationFloat = new DevExpress.Utils.PointFloat(11.03758F, 83.00004F);
            this.xrLabel55.Name = "xrLabel55";
            this.xrLabel55.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel55.SizeF = new System.Drawing.SizeF(508.6123F, 42.99996F);
            this.xrLabel55.StylePriority.UseBorders = false;
            this.xrLabel55.StylePriority.UseFont = false;
            this.xrLabel55.StylePriority.UseForeColor = false;
            this.xrLabel55.StylePriority.UseTextAlignment = false;
            this.xrLabel55.Text = "xrLabel5";
            this.xrLabel55.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel54
            // 
            this.xrLabel54.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel54.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel54.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.xrLabel54.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
            this.xrLabel54.LocationFloat = new DevExpress.Utils.PointFloat(11.0376F, 40F);
            this.xrLabel54.Name = "xrLabel54";
            this.xrLabel54.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel54.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.No;
            this.xrLabel54.SizeF = new System.Drawing.SizeF(508.6123F, 42.99998F);
            this.xrLabel54.StylePriority.UseBorders = false;
            this.xrLabel54.StylePriority.UseFont = false;
            this.xrLabel54.StylePriority.UseForeColor = false;
            this.xrLabel54.StylePriority.UseTextAlignment = false;
            this.xrLabel54.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrShape1
            // 
            this.xrShape1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrShape1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(134)))), ((int)(((byte)(18)))));
            this.xrShape1.LocationFloat = new DevExpress.Utils.PointFloat(11.0376F, 0F);
            this.xrShape1.Name = "xrShape1";
            shapeRectangle1.Fillet = 10;
            this.xrShape1.Shape = shapeRectangle1;
            this.xrShape1.SizeF = new System.Drawing.SizeF(508.6124F, 126F);
            this.xrShape1.SnapLineMargin = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrShape1.StylePriority.UseBackColor = false;
            this.xrShape1.StylePriority.UseBorderColor = false;
            this.xrShape1.StylePriority.UseBorders = false;
            this.xrShape1.StylePriority.UseForeColor = false;
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
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo2,
            this.xrLine2,
            this.xrPageInfo1,
            this.xrLabel8,
            this.xrLabel7});
            this.PageFooter.HeightF = 60F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(8.957581F, 20F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(201.42F, 20F);
            this.xrPageInfo2.StylePriority.UseFont = false;
            this.xrPageInfo2.StylePriority.UseTextAlignment = false;
            this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo2.TextFormatString = "Ngày {0:dd}  tháng {0:MM} năm {0:yyyy}";
            // 
            // xrLine2
            // 
            this.xrLine2.LineWidth = 3;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(11.03758F, 0F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(766.9933F, 20F);
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(308.2298F, 20F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(201.42F, 20F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo1.TextFormatString = "Số trang {0}/{1}";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(547.4525F, 20F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "Bác sĩ thực hiện";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PerformStaff]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(547.4525F, 40F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "Số phiếu:";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_Image
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRpt_PCLImagingResult_New_SubReport_Image1});
            this.DataAdapter = this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter;
            this.DataMember = "spXRpt_PCLImagingResult_New_Sub_Image";
            this.DataSource = this.dsXRpt_PCLImagingResult_New_SubReport_Image1;
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 21);
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
        private DevExpress.XtraReports.UI.XRLabel xrLabel24;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel53;
        private DevExpress.XtraReports.UI.XRLabel xrLabel55;
        private DevExpress.XtraReports.UI.XRLabel xrLabel54;
        private DevExpress.XtraReports.UI.XRShape xrShape1;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox6;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo2;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
    }
}
