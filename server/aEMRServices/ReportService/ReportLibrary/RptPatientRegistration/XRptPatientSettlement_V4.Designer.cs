using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptPatientSettlement_V4
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
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.parVATPercent = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalPTPaymentAfterVAT = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.parVATAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.parSupportStr = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalPTPaymentBeforeVAT = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel32 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalHIPayment = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalPatientPaymentInWords = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.parBHYTString = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.sp_Rpt_PatientSettlementTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.dsPatientSettlementTableAdapters.sp_Rpt_PatientSettlementTableAdapter();
            this.dsPatientSettlement1 = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.dsPatientSettlement();
            this.calculatedServiceName = new DevExpress.XtraReports.UI.CalculatedField();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.param_ID = new DevExpress.XtraReports.Parameters.Parameter();
            this.calFullName = new DevExpress.XtraReports.UI.CalculatedField();
            this.param_flag = new DevExpress.XtraReports.Parameters.Parameter();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.sParagraph = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH2 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH3 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sParagraphBold = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH4 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.parReceiptType = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientSettlement1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel15,
            this.xrLabel13,
            this.xrLabel12,
            this.xrLabel11,
            this.xrLabel14,
            this.xrLabel21,
            this.xrLabel9,
            this.xrLabel10,
            this.xrLabel31,
            this.xrLabel32,
            this.xrLabel25,
            this.xrLabel30,
            this.xrLabel8,
            this.xrLabel3,
            this.xrPageInfo1,
            this.xrLabel1,
            this.xrLabel2,
            this.xrLabel6,
            this.xrLabel7,
            this.xrLabel4,
            this.xrLabel5});
            this.Detail.HeightF = 547F;
            this.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
            this.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel15
            // 
            this.xrLabel15.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parVATPercent, "Text", "")});
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(105.84F, 330.29F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(39.58F, 14F);
            this.xrLabel15.StyleName = "sParagraph";
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "xrLabel15";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // parVATPercent
            // 
            this.parVATPercent.Name = "parVATPercent";
            this.parVATPercent.Visible = false;
            // 
            // xrLabel13
            // 
            this.xrLabel13.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalPTPaymentAfterVAT, "Text", "")});
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(567.7501F, 344.29F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(128.9999F, 14F);
            this.xrLabel13.StyleName = "sParagraph";
            this.xrLabel13.StylePriority.UseTextAlignment = false;
            this.xrLabel13.Text = "xrLabel13";
            this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // parTotalPTPaymentAfterVAT
            // 
            this.parTotalPTPaymentAfterVAT.Name = "parTotalPTPaymentAfterVAT";
            this.parTotalPTPaymentAfterVAT.Visible = false;
            // 
            // xrLabel12
            // 
            this.xrLabel12.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parVATAmount, "Text", "")});
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(567.7501F, 330.29F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(128.9999F, 14F);
            this.xrLabel12.StyleName = "sParagraph";
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "xrLabel12";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // parVATAmount
            // 
            this.parVATAmount.Name = "parVATAmount";
            this.parVATAmount.Visible = false;
            // 
            // xrLabel11
            // 
            this.xrLabel11.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parSupportStr, "Text", "")});
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(313.7499F, 294.29F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(382.9967F, 20F);
            this.xrLabel11.StyleName = "sParagraphBold";
            this.xrLabel11.StylePriority.UsePadding = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.Text = "xrLabel11";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parSupportStr
            // 
            this.parSupportStr.Name = "parSupportStr";
            // 
            // xrLabel14
            // 
            this.xrLabel14.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalAmount, "Text", "")});
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(567.7501F, 229.2901F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(129.002F, 19.99992F);
            this.xrLabel14.StyleName = "sParagraph";
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            this.xrLabel14.Text = "xrLabel26";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // parTotalAmount
            // 
            this.parTotalAmount.Name = "parTotalAmount";
            this.parTotalAmount.Visible = false;
            // 
            // xrLabel21
            // 
            this.xrLabel21.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalPTPaymentBeforeVAT, "Text", "")});
            this.xrLabel21.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(567.7501F, 316.29F);
            this.xrLabel21.Multiline = true;
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(128.9999F, 14F);
            this.xrLabel21.StyleName = "sParagraph";
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.Text = "xrLabel29";
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // parTotalPTPaymentBeforeVAT
            // 
            this.parTotalPTPaymentBeforeVAT.Name = "parTotalPTPaymentBeforeVAT";
            this.parTotalPTPaymentBeforeVAT.Visible = false;
            // 
            // xrLabel9
            // 
            this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.FContactCellPhone", "ĐT:{0}")});
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(400.03F, 134.04F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(158.7201F, 20F);
            this.xrLabel9.StyleName = "sParagraph";
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "xrLabel19";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel10
            // 
            this.xrLabel10.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.StaffName")});
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(436.88F, 476.37F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(259.8717F, 20F);
            this.xrLabel10.StyleName = "sParagraph";
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "xrLabel27";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel31
            // 
            this.xrLabel31.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(382.5999F, 229.29F);
            this.xrLabel31.Multiline = true;
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(83.73F, 20F);
            this.xrLabel31.StyleName = "sParagraph";
            this.xrLabel31.StylePriority.UseTextAlignment = false;
            this.xrLabel31.Text = "1";
            this.xrLabel31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel32
            // 
            this.xrLabel32.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel32.LocationFloat = new DevExpress.Utils.PointFloat(313.7499F, 229.29F);
            this.xrLabel32.Multiline = true;
            this.xrLabel32.Name = "xrLabel32";
            this.xrLabel32.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel32.SizeF = new System.Drawing.SizeF(68.85F, 20F);
            this.xrLabel32.StyleName = "sParagraph";
            this.xrLabel32.StylePriority.UseTextAlignment = false;
            this.xrLabel32.Text = "Lần";
            this.xrLabel32.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel25
            // 
            this.xrLabel25.CanGrow = false;
            this.xrLabel25.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.FinalizedReceiptNum")});
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 10.25F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(623.525F, 94.04001F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(103.475F, 20.00002F);
            this.xrLabel25.StyleName = "sH4";
            this.xrLabel25.StylePriority.UseTextAlignment = false;
            this.xrLabel25.Text = "xrLabel28";
            this.xrLabel25.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel25.WordWrap = false;
            // 
            // xrLabel30
            // 
            this.xrLabel30.CanGrow = false;
            this.xrLabel30.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.NationalMedicalCode")});
            this.xrLabel30.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(595.4F, 74.04F);
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(131.5999F, 19.99999F);
            this.xrLabel30.StyleName = "sH3";
            this.xrLabel30.StylePriority.UseTextAlignment = false;
            this.xrLabel30.Text = "xrLabel22";
            this.xrLabel30.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel30.WordWrap = false;
            // 
            // xrLabel8
            // 
            this.xrLabel8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.calFullName")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(143.75F, 114.04F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(415F, 20F);
            this.xrLabel8.StyleName = "sH1";
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "xrLabel12";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalAmount, "Text", "")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(466.33F, 229.29F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(101.42F, 20F);
            this.xrLabel3.StyleName = "sParagraph";
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "xrLabel18";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrPageInfo1.Format = "{0:dd}                   {0:MM}                 {0:yy}";
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(315.24F, 52.04F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(198.42F, 20F);
            this.xrPageInfo1.StyleName = "sParagraph";
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.PatientCode")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(595.4F, 54.04002F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(131.6F, 20F);
            this.xrLabel1.StyleName = "sH2";
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel17";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalHIPayment, "Text", "")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(313.7499F, 252.29F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(382.9967F, 20F);
            this.xrLabel2.StyleName = "sParagraphBold";
            this.xrLabel2.StylePriority.UsePadding = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "xrLabel16";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parTotalHIPayment
            // 
            this.parTotalHIPayment.Name = "parTotalHIPayment";
            this.parTotalHIPayment.Visible = false;
            // 
            // xrLabel6
            // 
            this.xrLabel6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parTotalPatientPaymentInWords, "Text", "")});
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(143.75F, 358.5F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(552.9966F, 20F);
            this.xrLabel6.StyleName = "sH3";
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "xrLabel20";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
            // 
            // parTotalPatientPaymentInWords
            // 
            this.parTotalPatientPaymentInWords.Name = "parTotalPatientPaymentInWords";
            this.parTotalPatientPaymentInWords.Visible = false;
            // 
            // xrLabel7
            // 
            this.xrLabel7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.PatientStreetAddress")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(59.17002F, 154.04F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(497.5768F, 20F);
            this.xrLabel7.StyleName = "sParagraph";
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "xrLabel13";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel4
            // 
            this.xrLabel4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parBHYTString, "Text", "")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(313.7499F, 272.29F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(382.9967F, 20F);
            this.xrLabel4.StyleName = "sParagraph";
            this.xrLabel4.StylePriority.UsePadding = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "xrLabel15";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parBHYTString
            // 
            this.parBHYTString.Name = "parBHYTString";
            this.parBHYTString.Visible = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.CanGrow = false;
            this.xrLabel5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "sp_Rpt_PatientSettlement.StrBillingInvNum")});
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(33.95996F, 229.29F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(279.79F, 101F);
            this.xrLabel5.StyleName = "sParagraph";
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "xrLabel11";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
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
            // sp_Rpt_PatientSettlementTableAdapter
            // 
            this.sp_Rpt_PatientSettlementTableAdapter.ClearBeforeFill = true;
            // 
            // dsPatientSettlement1
            // 
            this.dsPatientSettlement1.DataSetName = "dsPatientSettlement";
            this.dsPatientSettlement1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // calculatedServiceName
            // 
            this.calculatedServiceName.DataMember = "sp_Rpt_PatientSettlement";
            this.calculatedServiceName.Expression = "[ServiceName]+\' [\'+[PatientAmount]+\']\'";
            this.calculatedServiceName.Name = "calculatedServiceName";
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // param_ID
            // 
            this.param_ID.Name = "param_ID";
            this.param_ID.Type = typeof(int);
            this.param_ID.ValueInfo = "0";
            this.param_ID.Visible = false;
            // 
            // calFullName
            // 
            this.calFullName.DataMember = "sp_Rpt_PatientSettlement";
            this.calFullName.Expression = "Iif([Parameters.param_flag] == 0,  Upper([FullName]) + \' - \' + [YearOfBirth] + \'(" +
    "\' + [Age] + \'t) - \' + [Gender], Upper([FullName]) )";
            this.calFullName.Name = "calFullName";
            // 
            // param_flag
            // 
            this.param_flag.Name = "param_flag";
            this.param_flag.Type = typeof(int);
            this.param_flag.ValueInfo = "0";
            this.param_flag.Visible = false;
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // sParagraph
            // 
            this.sParagraph.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.sParagraph.Name = "sParagraph";
            // 
            // sH1
            // 
            this.sH1.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.sH1.Name = "sH1";
            // 
            // sH2
            // 
            this.sH2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sH2.Name = "sH2";
            // 
            // sH3
            // 
            this.sH3.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sH3.Name = "sH3";
            // 
            // sParagraphBold
            // 
            this.sParagraphBold.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold);
            this.sParagraphBold.Name = "sParagraphBold";
            // 
            // sH4
            // 
            this.sH4.Font = new System.Drawing.Font("Times New Roman", 10.25F, System.Drawing.FontStyle.Bold);
            this.sH4.Name = "sH4";
            // 
            // parReceiptType
            // 
            this.parReceiptType.Name = "parReceiptType";
            this.parReceiptType.Type = typeof(int);
            this.parReceiptType.ValueInfo = "0";
            this.parReceiptType.Visible = false;
            // 
            // XRptPatientSettlement_V4
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedServiceName,
            this.calFullName});
            this.DataAdapter = this.sp_Rpt_PatientSettlementTableAdapter;
            this.DataMember = "sp_Rpt_PatientSettlement";
            this.DataSource = this.dsPatientSettlement1;
            this.DrawGrid = false;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 0, 0);
            this.PageHeight = 547;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parTotalPatientPaymentInWords,
            this.param_ID,
            this.parTotalHIPayment,
            this.parBHYTString,
            this.param_flag,
            this.parTotalPTPaymentBeforeVAT,
            this.parTotalAmount,
            this.parSupportStr,
            this.parTotalPTPaymentAfterVAT,
            this.parVATPercent,
            this.parVATAmount,
            this.parReceiptType});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.sParagraph,
            this.sH1,
            this.sH2,
            this.sH3,
            this.sParagraphBold,
            this.sH4});
            this.Version = "14.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptPatientPayment_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientSettlement1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsPatientSettlementTableAdapters.sp_Rpt_PatientSettlementTableAdapter sp_Rpt_PatientSettlementTableAdapter;
        private DataSchema.dsPatientSettlement dsPatientSettlement1;
        private DevExpress.XtraReports.UI.CalculatedField calculatedServiceName;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        public DevExpress.XtraReports.Parameters.Parameter parTotalPatientPaymentInWords;
        public DevExpress.XtraReports.Parameters.Parameter param_ID;
        private DevExpress.XtraReports.Parameters.Parameter parTotalHIPayment;
        private DevExpress.XtraReports.Parameters.Parameter parBHYTString;
        private DevExpress.XtraReports.UI.CalculatedField calFullName;
        private DevExpress.XtraReports.Parameters.Parameter param_flag;
        private DevExpress.XtraReports.Parameters.Parameter parTotalPTPaymentBeforeVAT;
        private DevExpress.XtraReports.Parameters.Parameter parTotalAmount;
        private DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel31;
        private DevExpress.XtraReports.UI.XRLabel xrLabel32;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLabel xrLabel30;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.Parameters.Parameter parSupportStr;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.Parameters.Parameter parVATPercent;
        private DevExpress.XtraReports.Parameters.Parameter parTotalPTPaymentAfterVAT;
        private DevExpress.XtraReports.Parameters.Parameter parVATAmount;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraph;
        private DevExpress.XtraReports.UI.XRControlStyle sH1;
        private DevExpress.XtraReports.UI.XRControlStyle sH2;
        private DevExpress.XtraReports.UI.XRControlStyle sH3;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraphBold;
        private DevExpress.XtraReports.UI.XRControlStyle sH4;
        private DevExpress.XtraReports.Parameters.Parameter parReceiptType;
    }
}
