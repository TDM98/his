namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptOutPatientReceipt_Old
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
            this.param_CashierName = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_AmountInWords = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_Amount = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_Services = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_PatientAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_PatientName = new DevExpress.XtraReports.Parameters.Parameter();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.param_ReceiptNo = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.param_PatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.eHCMS_DB_DEVDataSet1 = new eHCMS.ReportLib.eHCMS_DB_DEVDataSet();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter = new eHCMS.ReportLib.eHCMS_DB_DEVDataSetTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter();
            this.param_PaymentID = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_PatientNameOnly = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.param_ServiceItemIDString = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_PclItemIDString = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_ReceiptForEachLocationPrintingMode = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.eHCMS_DB_DEVDataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // param_CashierName
            // 
            this.param_CashierName.Name = "param_CashierName";
            this.param_CashierName.Visible = false;
            // 
            // param_AmountInWords
            // 
            this.param_AmountInWords.Name = "param_AmountInWords";
            this.param_AmountInWords.Visible = false;
            // 
            // param_Amount
            // 
            this.param_Amount.Name = "param_Amount";
            this.param_Amount.Type = typeof(decimal);
            this.param_Amount.Value = 0;
            this.param_Amount.Visible = false;
            // 
            // param_Services
            // 
            this.param_Services.Name = "param_Services";
            this.param_Services.Visible = false;
            // 
            // param_PatientAddress
            // 
            this.param_PatientAddress.Name = "param_PatientAddress";
            this.param_PatientAddress.Visible = false;
            // 
            // param_PatientName
            // 
            this.param_PatientName.Name = "param_PatientName";
            this.param_PatientName.Visible = false;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2,
            this.xrLabel1});
            this.TopMargin.HeightF = 67.08332F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_ReceiptNo, "Text", "")});
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(517F, 44.08332F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel2.Text = "xrLabel2";
            // 
            // param_ReceiptNo
            // 
            this.param_ReceiptNo.Name = "param_ReceiptNo";
            this.param_ReceiptNo.Visible = false;
            // 
            // xrLabel1
            // 
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_PatientCode, "Text", "")});
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(517F, 10.00001F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel1.Text = "xrLabel1";
            // 
            // param_PatientCode
            // 
            this.param_PatientCode.Name = "param_PatientCode";
            this.param_PatientCode.Visible = false;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // eHCMS_DB_DEVDataSet1
            // 
            this.eHCMS_DB_DEVDataSet1.DataSetName = "eHCMS_DB_DEVDataSet";
            this.eHCMS_DB_DEVDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill = true;
            // 
            // param_PaymentID
            // 
            this.param_PaymentID.Name = "param_PaymentID";
            this.param_PaymentID.Type = typeof(int);
            this.param_PaymentID.Value = 0;
            this.param_PaymentID.Visible = false;
            // 
            // param_PatientNameOnly
            // 
            this.param_PatientNameOnly.Name = "param_PatientNameOnly";
            this.param_PatientNameOnly.Visible = false;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7,
            this.xrLabel8,
            this.xrLabel9,
            this.xrLabel6,
            this.xrLabel3,
            this.xrLabel4,
            this.xrLabel5});
            this.ReportHeader.HeightF = 281.3333F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel7
            // 
            this.xrLabel7.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_AmountInWords, "Text", "")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(0F, 146.7917F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(616.9999F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "xrLabel7";
            // 
            // xrLabel8
            // 
            this.xrLabel8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_CashierName, "Text", "")});
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(428.0418F, 216.0417F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(188.9582F, 23F);
            this.xrLabel8.Text = "xrLabel8";
            // 
            // xrLabel9
            // 
            this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_PatientNameOnly, "Text", "")});
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(0F, 216.0417F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(178.125F, 23F);
            this.xrLabel9.Text = "xrLabel9";
            // 
            // xrLabel6
            // 
            this.xrLabel6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_Amount, "Text", "{0:#,#}")});
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 111.7083F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(616.9999F, 23.00002F);
            this.xrLabel6.Text = "xrLabel6";
            // 
            // xrLabel3
            // 
            this.xrLabel3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_PatientName, "Text", "")});
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 4.166667F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(616.9999F, 23F);
            this.xrLabel3.Text = "xrLabel3";
            // 
            // xrLabel4
            // 
            this.xrLabel4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_PatientAddress, "Text", "")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 41.74998F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(616.9999F, 23F);
            this.xrLabel4.Text = "xrLabel4";
            // 
            // xrLabel5
            // 
            this.xrLabel5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.param_Services, "Text", "")});
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77.24997F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(617F, 23F);
            this.xrLabel5.Text = "xrLabel5";
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.HeightF = 0F;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // param_ServiceItemIDString
            // 
            this.param_ServiceItemIDString.Name = "param_ServiceItemIDString";
            // 
            // param_PclItemIDString
            // 
            this.param_PclItemIDString.Name = "param_PclItemIDString";
            // 
            // param_ReceiptForEachLocationPrintingMode
            // 
            this.param_ReceiptForEachLocationPrintingMode.Name = "param_ReceiptForEachLocationPrintingMode";
            this.param_ReceiptForEachLocationPrintingMode.Type = typeof(int);
            // 
            // XRptOutPatientReceipt_Old
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.GroupFooter1});
            this.DataAdapter = this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter;
            this.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.DataSource = this.eHCMS_DB_DEVDataSet1;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 67, 0);
            this.PageHeight = 583;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5Rotated;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.param_PatientCode,
            this.param_ReceiptNo,
            this.param_PatientName,
            this.param_PatientAddress,
            this.param_Services,
            this.param_Amount,
            this.param_AmountInWords,
            this.param_CashierName,
            this.param_PaymentID,
            this.param_PatientNameOnly,
            this.param_ServiceItemIDString,
            this.param_PclItemIDString,
            this.param_ReceiptForEachLocationPrintingMode});
            this.RequestParameters = false;
            this.Version = "11.2";
            ((System.ComponentModel.ISupportInitialize)(this.eHCMS_DB_DEVDataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.Parameters.Parameter param_PatientCode;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.Parameters.Parameter param_ReceiptNo;
        private DevExpress.XtraReports.Parameters.Parameter param_PatientName;
        private DevExpress.XtraReports.Parameters.Parameter param_PatientAddress;
        private DevExpress.XtraReports.Parameters.Parameter param_Services;
        private DevExpress.XtraReports.Parameters.Parameter param_Amount;
        private DevExpress.XtraReports.Parameters.Parameter param_AmountInWords;
        private DevExpress.XtraReports.Parameters.Parameter param_CashierName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private eHCMS_DB_DEVDataSet eHCMS_DB_DEVDataSet1;
        private eHCMS_DB_DEVDataSetTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter param_PaymentID;
        private DevExpress.XtraReports.Parameters.Parameter param_PatientNameOnly;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooter1;
        private DevExpress.XtraReports.Parameters.Parameter param_ServiceItemIDString;
        private DevExpress.XtraReports.Parameters.Parameter param_PclItemIDString;
        private DevExpress.XtraReports.Parameters.Parameter param_ReceiptForEachLocationPrintingMode;
    }
}
