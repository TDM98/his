using eHCMSLanguage;
/*
 * 20181030 #001 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth. Update report base on new flow (XacNhanQLBH) and template for ThanhVuHospital
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptOutPatientReceipt_V4_InNhiet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XRptOutPatientReceipt_V4_InNhiet));
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
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
            this.xrLabel35 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel33 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel32 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel41 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel50 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.parVATPercent = new DevExpress.XtraReports.Parameters.Parameter();
            this.parTotalPTPaymentAfterVAT = new DevExpress.XtraReports.Parameters.Parameter();
            this.parVATAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPaymentDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtCustName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtCustAddr = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtCustPhone = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPaymentAmount = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsGenericPayment = new DevExpress.XtraReports.Parameters.Parameter();
            this.parStaffName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtReason = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtCustDOB = new DevExpress.XtraReports.Parameters.Parameter();
            this.parGenPaymtOrgName = new DevExpress.XtraReports.Parameters.Parameter();
            this.sParagraph = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH2 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sH3 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.sParagraphBold = new DevExpress.XtraReports.UI.XRControlStyle();
            this.parPatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parFileCodeNumber = new DevExpress.XtraReports.Parameters.Parameter();
            this.pOutPtCashAdvanceID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel34 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel37 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.calculatedField1 = new DevExpress.XtraReports.UI.CalculatedField();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.calTotalMG = new DevExpress.XtraReports.UI.CalculatedField();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientPhieuChiDinhTableAdapters.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.outPatientReceipt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8});
            this.Detail.HeightF = 17.99998F;
            this.Detail.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
            this.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Detail_BeforePrint);
            // 
            // xrLabel8
            // 
            this.xrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", resources.GetString("xrLabel8.ExpressionBindings"))});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(267F, 17.99998F);
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UsePadding = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
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
            // xrLabel35
            // 
            this.xrLabel35.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientCode]")});
            this.xrLabel35.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel35.LocationFloat = new DevExpress.Utils.PointFloat(202.0301F, 66.00003F);
            this.xrLabel35.Name = "xrLabel35";
            this.xrLabel35.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel35.SizeF = new System.Drawing.SizeF(64.96996F, 17.99998F);
            this.xrLabel35.StylePriority.UseFont = false;
            this.xrLabel35.StylePriority.UseTextAlignment = false;
            this.xrLabel35.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 7F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 84.00002F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(49F, 17.99998F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Giới tính:";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Gender]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 7F, System.Drawing.FontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(49F, 84.00002F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(67.53002F, 17.99998F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 7F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(132.9816F, 84.00002F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(39.88181F, 18F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Tuổi:";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Age] + \' (NS:\'+[YearOfBirth]+\')\'")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(172.8634F, 84.00002F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(94.13324F, 18.00001F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel17
            // 
            this.xrLabel17.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Double;
            this.xrLabel17.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'STT: \'+[ServiceSeqNum]")});
            this.xrLabel17.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(202.0301F, 0F);
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(64.96988F, 29.99999F);
            this.xrLabel17.StyleName = "sH2";
            this.xrLabel17.StylePriority.UseBorderDashStyle = false;
            this.xrLabel17.StylePriority.UseBorders = false;
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.StylePriority.UseTextAlignment = false;
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel30
            // 
            this.xrLabel30.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(0F, 30F);
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(267F, 18F);
            this.xrLabel30.StylePriority.UseFont = false;
            this.xrLabel30.StylePriority.UseTextAlignment = false;
            this.xrLabel30.Text = "BIÊN LAI VIỆN PHÍ";
            this.xrLabel30.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel33
            // 
            this.xrLabel33.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel33.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel33.LocationFloat = new DevExpress.Utils.PointFloat(49F, 66F);
            this.xrLabel33.Name = "xrLabel33";
            this.xrLabel33.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel33.SizeF = new System.Drawing.SizeF(153.0301F, 18.00001F);
            this.xrLabel33.StylePriority.UseFont = false;
            this.xrLabel33.StylePriority.UseTextAlignment = false;
            this.xrLabel33.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel32
            // 
            this.xrLabel32.LocationFloat = new DevExpress.Utils.PointFloat(0F, 66F);
            this.xrLabel32.Name = "xrLabel32";
            this.xrLabel32.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel32.SizeF = new System.Drawing.SizeF(49F, 18F);
            this.xrLabel32.StylePriority.UseTextAlignment = false;
            this.xrLabel32.Text = "Họ và Tên:";
            this.xrLabel32.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel41
            // 
            this.xrLabel41.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientStreetAddress]")});
            this.xrLabel41.LocationFloat = new DevExpress.Utils.PointFloat(49F, 102F);
            this.xrLabel41.Name = "xrLabel41";
            this.xrLabel41.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel41.SizeF = new System.Drawing.SizeF(217.9984F, 18F);
            this.xrLabel41.StylePriority.UseFont = false;
            this.xrLabel41.StylePriority.UseTextAlignment = false;
            this.xrLabel41.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel38
            // 
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(0F, 102F);
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(49F, 17.99998F);
            this.xrLabel38.StylePriority.UseTextAlignment = false;
            this.xrLabel38.Text = "Địa chỉ:";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel50
            // 
            this.xrLabel50.Font = new System.Drawing.Font("Times New Roman", 7F, System.Drawing.FontStyle.Bold);
            this.xrLabel50.LocationFloat = new DevExpress.Utils.PointFloat(0F, 120F);
            this.xrLabel50.Multiline = true;
            this.xrLabel50.Name = "xrLabel50";
            this.xrLabel50.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel50.SizeF = new System.Drawing.SizeF(49F, 17.99998F);
            this.xrLabel50.StylePriority.UseFont = false;
            this.xrLabel50.StylePriority.UseTextAlignment = false;
            this.xrLabel50.Text = "Nội Dung\r\n";
            this.xrLabel50.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parBHYTString]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Italic);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 48F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(267F, 18F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel27
            // 
            this.xrLabel27.CanGrow = false;
            this.xrLabel27.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(0.6383181F, 14.99999F);
            this.xrLabel27.Name = "xrLabel27";
            this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel27.SizeF = new System.Drawing.SizeF(43.44334F, 15F);
            this.xrLabel27.StyleName = "sH3";
            this.xrLabel27.StylePriority.UseFont = false;
            this.xrLabel27.StylePriority.UseTextAlignment = false;
            this.xrLabel27.Text = "Số HĐ:";
            this.xrLabel27.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel27.WordWrap = false;
            // 
            // xrLabel25
            // 
            this.xrLabel25.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parHospitalName]")});
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 7F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(202.0301F, 15F);
            this.xrLabel25.StyleName = "sParagraph";
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.StylePriority.UseTextAlignment = false;
            this.xrLabel25.Text = "xrLabel4";
            this.xrLabel25.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel9
            // 
            this.xrLabel9.CanGrow = false;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ReceiptNumber]")});
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(44.08166F, 15F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(95.14F, 15F);
            this.xrLabel9.StyleName = "sH3";
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "xrLabel2";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel9.WordWrap = false;
            // 
            // parVATPercent
            // 
            this.parVATPercent.Name = "parVATPercent";
            this.parVATPercent.Visible = false;
            // 
            // parTotalPTPaymentAfterVAT
            // 
            this.parTotalPTPaymentAfterVAT.Name = "parTotalPTPaymentAfterVAT";
            this.parTotalPTPaymentAfterVAT.Visible = false;
            // 
            // parVATAmount
            // 
            this.parVATAmount.Name = "parVATAmount";
            this.parVATAmount.Visible = false;
            // 
            // parPaymentDate
            // 
            this.parPaymentDate.Name = "parPaymentDate";
            this.parPaymentDate.Type = typeof(System.DateTime);
            // 
            // parGenPaymtCustName
            // 
            this.parGenPaymtCustName.Name = "parGenPaymtCustName";
            // 
            // parGenPaymtCustAddr
            // 
            this.parGenPaymtCustAddr.Name = "parGenPaymtCustAddr";
            // 
            // parGenPaymtCustPhone
            // 
            this.parGenPaymtCustPhone.Name = "parGenPaymtCustPhone";
            // 
            // parGenPaymtCode
            // 
            this.parGenPaymtCode.Name = "parGenPaymtCode";
            // 
            // parPaymentAmount
            // 
            this.parPaymentAmount.Name = "parPaymentAmount";
            // 
            // parIsGenericPayment
            // 
            this.parIsGenericPayment.Name = "parIsGenericPayment";
            this.parIsGenericPayment.Type = typeof(bool);
            // 
            // parStaffName
            // 
            this.parStaffName.Name = "parStaffName";
            // 
            // parGenPaymtType
            // 
            this.parGenPaymtType.Name = "parGenPaymtType";
            // 
            // parGenPaymtReason
            // 
            this.parGenPaymtReason.Name = "parGenPaymtReason";
            // 
            // parGenPaymtCustDOB
            // 
            this.parGenPaymtCustDOB.Name = "parGenPaymtCustDOB";
            // 
            // parGenPaymtOrgName
            // 
            this.parGenPaymtOrgName.Name = "parGenPaymtOrgName";
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
            // parPatientCode
            // 
            this.parPatientCode.Name = "parPatientCode";
            this.parPatientCode.Visible = false;
            // 
            // parFileCodeNumber
            // 
            this.parFileCodeNumber.Name = "parFileCodeNumber";
            this.parFileCodeNumber.Visible = false;
            // 
            // pOutPtCashAdvanceID
            // 
            this.pOutPtCashAdvanceID.Description = "pOutPtCashAdvanceID";
            this.pOutPtCashAdvanceID.Name = "pOutPtCashAdvanceID";
            this.pOutPtCashAdvanceID.Type = typeof(long);
            this.pOutPtCashAdvanceID.ValueInfo = "0";
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // xrTable1
            // 
            this.xrTable1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(266.9966F, 15F);
            this.xrTable1.StylePriority.UseBorders = false;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell2,
            this.xrTableCell16});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell2.Font = new System.Drawing.Font("Times New Roman", 7F);
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.StylePriority.UseBorders = false;
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.StylePriority.UseTextAlignment = false;
            xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.RecordNumber;
            this.xrTableCell2.Summary = xrSummary1;
            this.xrTableCell2.Text = "Tổng Cộng";
            this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell2.Weight = 0.18125149517540679D;
            // 
            // xrTableCell16
            // 
            this.xrTableCell16.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.parTotalAmount]")});
            this.xrTableCell16.Font = new System.Drawing.Font("Times New Roman", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell16.Name = "xrTableCell16";
            this.xrTableCell16.StylePriority.UseBorders = false;
            this.xrTableCell16.StylePriority.UseFont = false;
            this.xrTableCell16.StylePriority.UseTextAlignment = false;
            this.xrTableCell16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell16.TextFormatString = "{0:#,0.#}";
            this.xrTableCell16.Weight = 0.44476401163697704D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLine1,
            this.xrLabel11,
            this.xrLabel10,
            this.xrLabel34,
            this.xrLabel22,
            this.xrLabel39,
            this.xrLabel37,
            this.xrLabel28,
            this.xrLabel31,
            this.xrLabel21,
            this.xrLabel20,
            this.xrLabel19,
            this.xrTable1});
            this.ReportFooter.HeightF = 195.7915F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 7.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 112F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(267F, 38.54166F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "( Cần kiểm tra đối chiếu khi lập, giao, nhận biên lai)\r\nLưu ý: Đây không phải là " +
    "hóa đơn GTGT, biên lai chỉ có giá trị xuất hóa đơn trong ngày.";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel1.TextTrimming = System.Drawing.StringTrimming.EllipsisWord;
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.276652F, 77.99998F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(265.72F, 4F);
            this.xrLine1.Visible = false;
            // 
            // xrLabel11
            // 
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sum([DiscountAmount])")});
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(77.30832F, 14.99999F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(189.6883F, 15F);
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel11.TextFormatString = "{0:#,0.#}";
            // 
            // xrLabel10
            // 
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(0.638326F, 15F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(76.67F, 15F);
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "Miễn Giảm:";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel34
            // 
            this.xrLabel34.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel34.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel34.Font = new System.Drawing.Font("Arial", 7F, System.Drawing.FontStyle.Italic);
            this.xrLabel34.LocationFloat = new DevExpress.Utils.PointFloat(0F, 150.5417F);
            this.xrLabel34.Name = "xrLabel34";
            this.xrLabel34.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel34.SizeF = new System.Drawing.SizeF(266.36F, 15F);
            this.xrLabel34.StylePriority.UseBorders = false;
            this.xrLabel34.StylePriority.UseFont = false;
            this.xrLabel34.StylePriority.UseTextAlignment = false;
            this.xrLabel34.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel34.TextFormatString = "{0:dd/MM/yyyy hh:mm tt}";
            // 
            // xrLabel22
            // 
            this.xrLabel22.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'{0:#,0.#}\',[Parameters].[parTotalPTPaymentAfterVAT]) + \' đồng\'")});
            this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(77.30832F, 29.99999F);
            this.xrLabel22.Multiline = true;
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(189.6883F, 15F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.StylePriority.UseTextAlignment = false;
            this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel39
            // 
            this.xrLabel39.AutoWidth = true;
            this.xrLabel39.CanShrink = true;
            this.xrLabel39.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ChungTuGoc]")});
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(77.30832F, 60.00001F);
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(189.6883F, 17.99998F);
            this.xrLabel39.StylePriority.UseTextAlignment = false;
            this.xrLabel39.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel37
            // 
            this.xrLabel37.CanShrink = true;
            this.xrLabel37.LocationFloat = new DevExpress.Utils.PointFloat(0.6383181F, 60F);
            this.xrLabel37.Name = "xrLabel37";
            this.xrLabel37.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel37.SizeF = new System.Drawing.SizeF(76.67F, 17.99998F);
            this.xrLabel37.StylePriority.UseTextAlignment = false;
            this.xrLabel37.Text = "Chứng từ gốc:";
            this.xrLabel37.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel28
            // 
            this.xrLabel28.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel28.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[calculatedField1]")});
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(6.103516E-05F, 81.99998F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(267F, 15F);
            this.xrLabel28.StylePriority.UseBorders = false;
            this.xrLabel28.StylePriority.UseTextAlignment = false;
            this.xrLabel28.Text = "xrLabel27";
            this.xrLabel28.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel28.TextTrimming = System.Drawing.StringTrimming.EllipsisWord;
            // 
            // xrLabel31
            // 
            this.xrLabel31.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel31.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StaffName]")});
            this.xrLabel31.Font = new System.Drawing.Font("Arial", 7F, System.Drawing.FontStyle.Bold);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(0.6400757F, 96.99998F);
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(266.36F, 15F);
            this.xrLabel31.StylePriority.UseBorders = false;
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.StylePriority.UseTextAlignment = false;
            this.xrLabel31.Text = "xrLabel20";
            this.xrLabel31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel21
            // 
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(0.6383101F, 45F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(76.67F, 15F);
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.Text = "Bằng chữ:";
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel20
            // 
            this.xrLabel20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parTotalPatientPaymentInWords]")});
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(77.30832F, 45.00001F);
            this.xrLabel20.Multiline = true;
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(189.6883F, 15F);
            this.xrLabel20.StylePriority.UseTextAlignment = false;
            this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel19
            // 
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(0.6383101F, 30F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(76.67F, 15F);
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.Text = "Người bệnh trả:";
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // calculatedField1
            // 
            this.calculatedField1.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.calculatedField1.Expression = "\'Ngày \' + GetDay([CreateDate]) + \' tháng \' +GetMonth([CreateDate]) + \' năm \' +Get" +
    "Year([CreateDate])";
            this.calculatedField1.Name = "calculatedField1";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLabel35,
            this.xrLabel6,
            this.xrLabel7,
            this.xrLabel3,
            this.xrLabel4,
            this.xrLabel17,
            this.xrLabel30,
            this.xrLabel9,
            this.xrLabel32,
            this.xrLabel41,
            this.xrLabel38,
            this.xrLabel50,
            this.xrLabel2,
            this.xrLabel27,
            this.xrLabel33,
            this.xrLabel25});
            this.ReportHeader.HeightF = 138F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NoiDungThu]")});
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 7F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(49F, 120F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(173.7965F, 17.99998F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // calTotalMG
            // 
            this.calTotalMG.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.calTotalMG.Expression = "[DiscountAmount]";
            this.calTotalMG.FieldType = DevExpress.XtraReports.UI.FieldType.Float;
            this.calTotalMG.Name = "calTotalMG";
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // XRptOutPatientReceipt_V4_InNhiet
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportFooter,
            this.ReportHeader});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedServiceName,
            this.calFullName,
            this.calculatedField1,
            this.calTotalMG});
            this.DataAdapter = this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter;
            this.DataMember = "sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID";
            this.DataSource = this.outPatientReceipt1;
            this.DrawGrid = false;
            this.Font = new System.Drawing.Font("Times New Roman", 7F);
            this.Margins = new System.Drawing.Printing.Margins(2, 2, 0, 0);
            this.PageHeight = 827;
            this.PageWidth = 271;
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
            this.parGenPaymtCustName,
            this.parGenPaymtCustAddr,
            this.parGenPaymtCustPhone,
            this.parGenPaymtCode,
            this.parPaymentAmount,
            this.parPaymentDate,
            this.parIsGenericPayment,
            this.parStaffName,
            this.parGenPaymtType,
            this.parGenPaymtReason,
            this.parGenPaymtCustDOB,
            this.parGenPaymtOrgName,
            this.parTotalPTPaymentAfterVAT,
            this.parVATPercent,
            this.parVATAmount,
            this.parPatientCode,
            this.parFileCodeNumber,
            this.pOutPtCashAdvanceID,
            this.parDepartmentOfHealth,
            this.parHospitalName});
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
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtCustName;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtCustAddr;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtCustPhone;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtCode;
        public DevExpress.XtraReports.Parameters.Parameter parPaymentAmount;
        public DevExpress.XtraReports.Parameters.Parameter parPaymentDate;
        public DevExpress.XtraReports.Parameters.Parameter parIsGenericPayment;
        public DevExpress.XtraReports.Parameters.Parameter parStaffName;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtType;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtReason;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtCustDOB;
        public DevExpress.XtraReports.Parameters.Parameter parGenPaymtOrgName;
        private DevExpress.XtraReports.Parameters.Parameter parTotalPTPaymentAfterVAT;
        private DevExpress.XtraReports.Parameters.Parameter parVATPercent;
        private DevExpress.XtraReports.Parameters.Parameter parVATAmount;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraph;
        private DevExpress.XtraReports.UI.XRControlStyle sH1;
        private DevExpress.XtraReports.UI.XRControlStyle sH2;
        private DevExpress.XtraReports.UI.XRControlStyle sH3;
        private DevExpress.XtraReports.UI.XRControlStyle sParagraphBold;
        private DevExpress.XtraReports.Parameters.Parameter parPatientCode;
        private DevExpress.XtraReports.Parameters.Parameter parFileCodeNumber;
        public DevExpress.XtraReports.Parameters.Parameter pOutPtCashAdvanceID;
        private DevExpress.XtraReports.UI.XRLabel xrLabel27;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel30;
        private DevExpress.XtraReports.UI.XRLabel xrLabel33;
        private DevExpress.XtraReports.UI.XRLabel xrLabel32;
        private DevExpress.XtraReports.UI.XRLabel xrLabel41;
        private DevExpress.XtraReports.UI.XRLabel xrLabel38;
        private DevExpress.XtraReports.UI.XRLabel xrLabel50;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell16;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel28;
        private DevExpress.XtraReports.UI.XRLabel xrLabel31;
        private DevExpress.XtraReports.UI.XRLabel xrLabel34;
        private DevExpress.XtraReports.UI.CalculatedField calculatedField1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel35;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.CalculatedField calTotalMG;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel39;
        private DevExpress.XtraReports.UI.XRLabel xrLabel37;
        private RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DataSchema.OutPatientPhieuChiDinhTableAdapters.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
    }
}
