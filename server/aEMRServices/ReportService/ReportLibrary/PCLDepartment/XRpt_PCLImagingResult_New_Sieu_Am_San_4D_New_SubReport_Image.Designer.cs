namespace eHCMS.ReportLib.PCLDepartment
{
    partial class XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image
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
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPictureBox5 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrPictureBox4 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrPictureBox3 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrPictureBox2 = new DevExpress.XtraReports.UI.XRPictureBox();
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
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPictureBox6 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter();
            this.spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter();
            this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1 = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image();
            this.spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo3 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrPageInfo4 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBox5,
            this.xrPictureBox4,
            this.xrPictureBox3,
            this.xrPictureBox2,
            this.xrPictureBox1});
            this.Detail.HeightF = 660F;
            this.Detail.MultiColumn.ColumnCount = 3;
            this.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPictureBox5
            // 
            this.xrPictureBox5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'\\\'+[Hinh5]")});
            this.xrPictureBox5.LocationFloat = new DevExpress.Utils.PointFloat(760F, 400F);
            this.xrPictureBox5.Name = "xrPictureBox5";
            this.xrPictureBox5.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox5.SizeF = new System.Drawing.SizeF(360F, 260F);
            this.xrPictureBox5.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrPictureBox4
            // 
            this.xrPictureBox4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'\\\'+[Hinh4]")});
            this.xrPictureBox4.LocationFloat = new DevExpress.Utils.PointFloat(385F, 400F);
            this.xrPictureBox4.Name = "xrPictureBox4";
            this.xrPictureBox4.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox4.SizeF = new System.Drawing.SizeF(360F, 260F);
            this.xrPictureBox4.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrPictureBox3
            // 
            this.xrPictureBox3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'\\\'+[Hinh2]")});
            this.xrPictureBox3.LocationFloat = new DevExpress.Utils.PointFloat(570F, 0F);
            this.xrPictureBox3.Name = "xrPictureBox3";
            this.xrPictureBox3.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox3.SizeF = new System.Drawing.SizeF(550F, 390F);
            this.xrPictureBox3.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrPictureBox2
            // 
            this.xrPictureBox2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'\\\'+[Hinh3]")});
            this.xrPictureBox2.LocationFloat = new DevExpress.Utils.PointFloat(10F, 400F);
            this.xrPictureBox2.Name = "xrPictureBox2";
            this.xrPictureBox2.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox2.SizeF = new System.Drawing.SizeF(360F, 260F);
            this.xrPictureBox2.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrPictureBox1
            // 
            this.xrPictureBox1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[PCLResultLocation]+\'\\\'+[Hinh1]")});
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(10F, 0F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(550F, 390F);
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
            this.BottomMargin.HeightF = 20F;
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
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PCLRequestNumID]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(891.1917F, 10.00001F);
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
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(1001.386F, 10.00001F);
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
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(1001.386F, 27.00001F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(118.6144F, 17.00001F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UsePadding = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPictureBox6
            // 
            this.xrPictureBox6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageUrl", "[LogoUrl]")});
            this.xrPictureBox6.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 0F);
            this.xrPictureBox6.Name = "xrPictureBox6";
            this.xrPictureBox6.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrPictureBox6.SizeF = new System.Drawing.SizeF(175F, 50F);
            this.xrPictureBox6.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // xrLabel24
            // 
            this.xrLabel24.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel24.ForeColor = System.Drawing.Color.Blue;
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(329.1666F, 0F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(517.5417F, 48F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.StylePriority.UseForeColor = false;
            this.xrLabel24.StylePriority.UseTextAlignment = false;
            this.xrLabel24.Text = "DANH SÁCH HÌNH ẢNH CHẨN ĐOÁN";
            this.xrLabel24.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // spXRpt_PCLImagingResult_NewTableAdapter
            // 
            this.spXRpt_PCLImagingResult_NewTableAdapter.ClearBeforeFill = true;
            // 
            // spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter.ClearBeforeFill = true;
            // 
            // spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter.ClearBeforeFill = true;
            // 
            // dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1
            // 
            this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1.DataSetName = "dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image";
            this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter.ClearBeforeFill = true;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel1,
            this.xrLabel2,
            this.xrPageInfo3,
            this.xrLine1,
            this.xrPageInfo4});
            this.PageFooter.HeightF = 52F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(10F, 32F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientCode]")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(10F, 12F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PerformStaff]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(888.4101F, 32F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Số phiếu:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(888.4101F, 12F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(230.59F, 20F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Bác sĩ thực hiện";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrPageInfo3
            // 
            this.xrPageInfo3.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrPageInfo3.LocationFloat = new DevExpress.Utils.PointFloat(479.2299F, 12F);
            this.xrPageInfo3.Name = "xrPageInfo3";
            this.xrPageInfo3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo3.SizeF = new System.Drawing.SizeF(201.42F, 20F);
            this.xrPageInfo3.StylePriority.UseFont = false;
            this.xrPageInfo3.StylePriority.UseTextAlignment = false;
            this.xrPageInfo3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo3.TextFormatString = "Số trang {0}/{1}";
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 3;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(6.103516E-05F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(1129F, 12F);
            // 
            // xrPageInfo4
            // 
            this.xrPageInfo4.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrPageInfo4.LocationFloat = new DevExpress.Utils.PointFloat(479.2299F, 32F);
            this.xrPageInfo4.Name = "xrPageInfo4";
            this.xrPageInfo4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo4.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo4.SizeF = new System.Drawing.SizeF(201.42F, 20F);
            this.xrPageInfo4.StylePriority.UseFont = false;
            this.xrPageInfo4.StylePriority.UseTextAlignment = false;
            this.xrPageInfo4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo4.TextFormatString = "Ngày {0:dd}  tháng {0:MM} năm {0:yyyy}";
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel61,
            this.xrLabel19,
            this.xrBarCode1,
            this.xrLabel24,
            this.xrPictureBox6});
            this.PageHeader.HeightF = 62F;
            this.PageHeader.Name = "PageHeader";
            // 
            // XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageFooter,
            this.PageHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1});
            this.DataAdapter = this.spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter;
            this.DataMember = "spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image";
            this.DataSource = this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1;
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
            this.PageHeight = 827;
            this.PageWidth = 1169;
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
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1)).EndInit();
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
        private DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
        private DataSchema.dsXRpt_PCLImagingResult_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox1;
        private DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter spXRpt_PCLImagingResult_New_Sieu_Am_San_4DTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel24;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox5;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox4;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox3;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox2;
        private DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_Image1;
        private DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter spXRpt_PCLImagingResult_New_Sieu_Am_San_4D_New_SubReport_ImageTableAdapter;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox6;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo3;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
    }
}
