using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptOutPatientReceiptAuto_V4
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
            this.parame_SequenceNumberString = new DevExpress.XtraReports.Parameters.Parameter();
            this.parTotalPatientPaymentInWords = new DevExpress.XtraReports.Parameters.Parameter();
            this.parTotalPTPaymentBeforeVAT = new DevExpress.XtraReports.Parameters.Parameter();
            this.parTotalAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.parServiceString = new DevExpress.XtraReports.Parameters.Parameter();
            this.parBHYTString = new DevExpress.XtraReports.Parameters.Parameter();
            this.parTotalHIPayment = new DevExpress.XtraReports.Parameters.Parameter();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter();
            this.outPatientReceipt1 = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceipt();
            this.calculatedServiceName = new DevExpress.XtraReports.UI.CalculatedField();
            this.param_PaymentID = new DevExpress.XtraReports.Parameters.Parameter();
            this.calFullName = new DevExpress.XtraReports.UI.CalculatedField();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.parVATPercent = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.parTotalPTPaymentAfterVAT = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.parVATAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.sParagraph = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH2 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH3 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sParagraphBold = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this.outPatientReceipt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
            this.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parame_SequenceNumberString
            // 
            this.parame_SequenceNumberString.Name = "parame_SequenceNumberString";
            this.parame_SequenceNumberString.Visible = false;
            // 
            // parTotalPatientPaymentInWords
            // 
            this.parTotalPatientPaymentInWords.Name = "parTotalPatientPaymentInWords";
            this.parTotalPatientPaymentInWords.Visible = false;
            // 
            // parTotalPTPaymentBeforeVAT
            // 
            this.parTotalPTPaymentBeforeVAT.Name = "parTotalPTPaymentBeforeVAT";
            this.parTotalPTPaymentBeforeVAT.Visible = false;
            // 
            // parTotalAmount
            // 
            this.parTotalAmount.Name = "parTotalAmount";
            // 
            // parServiceString
            // 
            this.parServiceString.Name = "parServiceString";
            this.parServiceString.Visible = false;
            // 
            // parBHYTString
            // 
            this.parBHYTString.Name = "parBHYTString";
            this.parBHYTString.Visible = false;
            // 
            // parTotalHIPayment
            // 
            this.parTotalHIPayment.Name = "parTotalHIPayment";
            this.parTotalHIPayment.Visible = false;
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
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill = true;
            // 
            // outPatientReceipt1
            // 
            this.outPatientReceipt1.DataSetName = "OutPatientReceipt";
            this.outPatientReceipt1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // calculatedServiceName
            // 
            this.calculatedServiceName.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.calculatedServiceName.Expression = "[ServiceName]+\' [\'+[PatientAmount]+\']\'";
            this.calculatedServiceName.Name = "calculatedServiceName";
            // 
            // param_PaymentID
            // 
            this.param_PaymentID.Name = "param_PaymentID";
            this.param_PaymentID.Type = typeof(int);
            this.param_PaymentID.ValueInfo = "0";
            this.param_PaymentID.Visible = false;
            // 
            // calFullName
            // 
            this.calFullName.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.calFullName.Expression = "Upper([FullName]) +\' - \' +[YearOfBirth]+\'(\'+[Age]+\'t) - \' +[Gender]";
            this.calFullName.Name = "calFullName";
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel4,
            this.xrLabel15,
            this.xrLabel16,
            this.xrLabel13,
            this.xrLabel14,
            this.xrLabel17,
            this.xrLabel20,
            this.xrLabel21,
            this.xrLabel18,
            this.xrLabel19,
            this.xrLabel12,
            this.xrLabel3,
            this.xrPageInfo2,
            this.xrLabel1,
            this.xrLabel2,
            this.xrLabel7,
            this.xrLabel10,
            this.xrLabel11,
            this.xrLabel8,
            this.xrLabel9});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("PaymentID", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.HeightF = 547F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // xrLabel6
            // 
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parVATPercent]")});
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(107.1665F, 340F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(46.88F, 17F);
            this.xrLabel6.StyleName = "sParagraph";
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "xrLabel6";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // parVATPercent
            // 
            this.parVATPercent.Name = "parVATPercent";
            this.parVATPercent.Visible = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalPTPaymentAfterVAT]")});
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(571.7501F, 354F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(145.25F, 14F);
            this.xrLabel5.StyleName = "sParagraph";
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "xrLabel5";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // parTotalPTPaymentAfterVAT
            // 
            this.parTotalPTPaymentAfterVAT.Name = "parTotalPTPaymentAfterVAT";
            this.parTotalPTPaymentAfterVAT.Visible = false;
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parVATAmount]")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(571.7501F, 340F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(145.25F, 14F);
            this.xrLabel4.StyleName = "sParagraph";
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "xrLabel4";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // parVATAmount
            // 
            this.parVATAmount.Name = "parVATAmount";
            this.parVATAmount.Visible = false;
            // 
            // xrLabel15
            // 
            this.xrLabel15.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalPTPaymentBeforeVAT]")});
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 11.5F);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(571.75F, 326F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(145.25F, 14F);
            this.xrLabel15.StyleName = "sParagraph";
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "xrLabel17";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
            // 
            // xrLabel16
            // 
            this.xrLabel16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalPatientPaymentInWords]")});
            this.xrLabel16.Font = new System.Drawing.Font("Times New Roman", 11.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(133.75F, 371F);
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(552.9966F, 20F);
            this.xrLabel16.StyleName = "sH3";
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "xrLabel7";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel13
            // 
            this.xrLabel13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StaffName]")});
            this.xrLabel13.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(455.21F, 492.83F);
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(251.5384F, 20F);
            this.xrLabel13.StyleName = "sParagraph";
            this.xrLabel13.StylePriority.UseTextAlignment = false;
            this.xrLabel13.Text = "xrLabel4";
            this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel14
            // 
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalAmount]")});
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(571.7501F, 232.7901F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(145.2499F, 19.99992F);
            this.xrLabel14.StyleName = "sParagraph";
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            this.xrLabel14.Text = "xrLabel16";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLabel17
            // 
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parame_SequenceNumberString]")});
            this.xrLabel17.Font = new System.Drawing.Font("Times New Roman", 12.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(315.24F, 80.29F);
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(106.67F, 20F);
            this.xrLabel17.StyleName = "sH2";
            this.xrLabel17.StylePriority.UseTextAlignment = false;
            this.xrLabel17.Text = "xrLabel13";
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel20
            // 
            this.xrLabel20.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(391.9701F, 232.79F);
            this.xrLabel20.Multiline = true;
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(69.14996F, 20F);
            this.xrLabel20.StyleName = "sParagraph";
            this.xrLabel20.StylePriority.UseTextAlignment = false;
            this.xrLabel20.Text = "1";
            this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(303.5401F, 232.79F);
            this.xrLabel21.Multiline = true;
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(88.43F, 20F);
            this.xrLabel21.StyleName = "sParagraph";
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.Text = "Lần";
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel18
            // 
            this.xrLabel18.CanGrow = false;
            this.xrLabel18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NationalMedicalCode]")});
            this.xrLabel18.Font = new System.Drawing.Font("Times New Roman", 11.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(607.9F, 62.29F);
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(119.1F, 19.99999F);
            this.xrLabel18.StyleName = "sH3";
            this.xrLabel18.StylePriority.UseTextAlignment = false;
            this.xrLabel18.Text = "xrLabel9";
            this.xrLabel18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel18.WordWrap = false;
            // 
            // xrLabel19
            // 
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[LocationName]")});
            this.xrLabel19.Font = new System.Drawing.Font("Times New Roman", 12.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(315.24F, 60.29F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(248.51F, 20F);
            this.xrLabel19.StyleName = "sH2";
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.Text = "xrLabel12";
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel12
            // 
            this.xrLabel12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FContactCellPhone]")});
            this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(400.03F, 122.29F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(158.7201F, 20F);
            this.xrLabel12.StyleName = "sParagraph";
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "xrLabel14";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel12.TextFormatString = "ĐT:{0}";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalAmount]")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(461.12F, 232.7901F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(110.63F, 19.99992F);
            this.xrLabel3.StyleName = "sParagraph";
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "xrLabel16";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(315.24F, 40.29F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(198.42F, 20F);
            this.xrPageInfo2.StyleName = "sParagraph";
            this.xrPageInfo2.StylePriority.UseTextAlignment = false;
            this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrPageInfo2.TextFormatString = "{0:dd}                   {0:MM}                 {0:yy}";
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientCode]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 12.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(607.9F, 42.29F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(119.1F, 20F);
            this.xrLabel1.StyleName = "sH2";
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel1";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalHIPayment]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(303.54F, 272.79F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(413.46F, 20F);
            this.xrLabel2.StyleName = "sParagraphBold";
            this.xrLabel2.StylePriority.UsePadding = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "xrLabel5";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrLabel2.TextFormatString = "{0:#,#}";
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parBHYTString]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(303.54F, 292.79F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(413.46F, 20F);
            this.xrLabel7.StyleName = "sParagraph";
            this.xrLabel7.StylePriority.UsePadding = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "xrLabel6";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel10
            // 
            this.xrLabel10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientStreetAddress]")});
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(39.17F, 144.29F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(497.5768F, 20F);
            this.xrLabel10.StyleName = "sParagraph";
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "xrLabel4";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel11
            // 
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[calFullName]")});
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 13.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(133.75F, 102.29F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(415F, 20F);
            this.xrLabel11.StyleName = "sH1";
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.Text = "xrLabel3";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.CanGrow = false;
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parServiceString]")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(17.49999F, 232.79F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(286.04F, 107.21F);
            this.xrLabel8.StyleName = "sParagraph";
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "xrLabel10";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel9
            // 
            this.xrLabel9.CanGrow = false;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ReceiptNumber]")});
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 11.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(647.48F, 82.29F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(79.51666F, 20.00002F);
            this.xrLabel9.StyleName = "sH3";
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "xrLabel2";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel9.WordWrap = false;
            // 
            // sParagraph
            // 
            this.sParagraph.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.sParagraph.Name = "sParagraph";
            // 
            // sH1
            // 
            this.sH1.Font = new System.Drawing.Font("Times New Roman", 13.5F, System.Drawing.FontStyle.Bold);
            this.sH1.Name = "sH1";
            // 
            // sH2
            // 
            this.sH2.Font = new System.Drawing.Font("Times New Roman", 12.5F, System.Drawing.FontStyle.Bold);
            this.sH2.Name = "sH2";
            // 
            // sH3
            // 
            this.sH3.Font = new System.Drawing.Font("Times New Roman", 11.75F, System.Drawing.FontStyle.Bold);
            this.sH3.Name = "sH3";
            // 
            // sParagraphBold
            // 
            this.sParagraphBold.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            this.sParagraphBold.Name = "sParagraphBold";
            // 
            // XRptOutPatientReceiptAuto_V4
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.GroupHeader1});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedServiceName,
            this.calFullName});
            this.DataAdapter = this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter;
            this.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.DataSource = this.outPatientReceipt1;
            this.DrawGrid = false;
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 0, 0);
            this.PageHeight = 547;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parTotalPatientPaymentInWords,
            this.param_PaymentID,
            this.parServiceString,
            this.parTotalHIPayment,
            this.parBHYTString,
            this.parame_SequenceNumberString,
            this.parTotalPTPaymentBeforeVAT,
            this.parTotalAmount,
            this.parTotalPTPaymentAfterVAT,
            this.parVATPercent,
            this.parVATAmount});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.sParagraph,
            this.sH1,
            this.sH2,
            this.sH3,
            this.sParagraphBold});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptPatientPayment_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.outPatientReceipt1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter;
        private DataSchema.OutPatientReceipt outPatientReceipt1;
        private DevExpress.XtraReports.UI.CalculatedField calculatedServiceName;
        public DevExpress.XtraReports.Parameters.Parameter parTotalPatientPaymentInWords;
        public DevExpress.XtraReports.Parameters.Parameter param_PaymentID;
        private DevExpress.XtraReports.Parameters.Parameter parServiceString;
        private DevExpress.XtraReports.Parameters.Parameter parTotalHIPayment;
        private DevExpress.XtraReports.Parameters.Parameter parBHYTString;
        private DevExpress.XtraReports.UI.CalculatedField calFullName;
        private DevExpress.XtraReports.Parameters.Parameter parame_SequenceNumberString;
        private DevExpress.XtraReports.Parameters.Parameter parTotalPTPaymentBeforeVAT;
        private DevExpress.XtraReports.Parameters.Parameter parTotalAmount;
        private DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraph;
        private DevExpress.XtraReports.UI.XRControlStyle sH1;
        private DevExpress.XtraReports.UI.XRControlStyle sH2;
        private DevExpress.XtraReports.UI.XRControlStyle sH3;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraphBold;
        private DevExpress.XtraReports.Parameters.Parameter parTotalPTPaymentAfterVAT;
        private DevExpress.XtraReports.Parameters.Parameter parVATPercent;
        private DevExpress.XtraReports.Parameters.Parameter parVATAmount;
    }
}
