using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescriptionInpt_TV
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
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parIssueID = new DevExpress.XtraReports.Parameters.Parameter();
            this.calculatedFieldGender = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcGenericName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcSoLuongNgayDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcCachDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcLoiDanChung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcDayMonthYearCurrent = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcBsRaToa = new DevExpress.XtraReports.UI.CalculatedField();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel68 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel69 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel70 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel65 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel66 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel67 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel75 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel74 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel73 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel47 = new DevExpress.XtraReports.UI.XRLabel();
            this.calcMark = new DevExpress.XtraReports.UI.CalculatedField();
            this.calSang = new DevExpress.XtraReports.UI.CalculatedField();
            this.calTrua = new DevExpress.XtraReports.UI.CalculatedField();
            this.calChieu = new DevExpress.XtraReports.UI.CalculatedField();
            this.calToi = new DevExpress.XtraReports.UI.CalculatedField();
            this.calConsutation = new DevExpress.XtraReports.UI.CalculatedField();
            this.calBrandName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calBsRatoa = new DevExpress.XtraReports.UI.CalculatedField();
            this.calLoiDanChung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calReIssue = new DevExpress.XtraReports.UI.CalculatedField();
            this.parIsPsychotropicDrugs = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsFuncfoodsOrCosmetics = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsPrescriptionNew_InPt1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPt();
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew_InPt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1,
            this.xrLabel15,
            this.xrSubreport2});
            this.Detail.HeightF = 626F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel15.ForeColor = System.Drawing.Color.DarkRed;
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(200F, 3.178914E-05F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(584.9999F, 35.49999F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UseForeColor = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "TOA THUỐC XUẤT VIỆN";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.White;
            this.Title.BorderColor = System.Drawing.SystemColors.ControlText;
            this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Title.BorderWidth = 1F;
            this.Title.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold);
            this.Title.ForeColor = System.Drawing.Color.Maroon;
            this.Title.Name = "Title";
            // 
            // FieldCaption
            // 
            this.FieldCaption.BackColor = System.Drawing.Color.White;
            this.FieldCaption.BorderColor = System.Drawing.SystemColors.ControlText;
            this.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.FieldCaption.BorderWidth = 1F;
            this.FieldCaption.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FieldCaption.ForeColor = System.Drawing.Color.Maroon;
            this.FieldCaption.Name = "FieldCaption";
            // 
            // PageInfo
            // 
            this.PageInfo.BackColor = System.Drawing.Color.White;
            this.PageInfo.BorderColor = System.Drawing.SystemColors.ControlText;
            this.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.PageInfo.BorderWidth = 1F;
            this.PageInfo.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.PageInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PageInfo.Name = "PageInfo";
            // 
            // DataField
            // 
            this.DataField.BackColor = System.Drawing.Color.White;
            this.DataField.BorderColor = System.Drawing.SystemColors.ControlText;
            this.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.DataField.BorderWidth = 1F;
            this.DataField.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.DataField.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DataField.Name = "DataField";
            this.DataField.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Arial", 9.75F);
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(663.5668F, 2.166621F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(123.4332F, 17.99997F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrPageInfo1.TextFormatString = "Trang {0}/{1}";
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 21F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 27F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // parIssueID
            // 
            this.parIssueID.Name = "parIssueID";
            this.parIssueID.Type = typeof(int);
            this.parIssueID.ValueInfo = "0";
            this.parIssueID.Visible = false;
            // 
            // calculatedFieldGender
            // 
            this.calculatedFieldGender.Name = "calculatedFieldGender";
            // 
            // calcGenericName
            // 
            this.calcGenericName.Name = "calcGenericName";
            // 
            // calcSoLuongNgayDung
            // 
            this.calcSoLuongNgayDung.Name = "calcSoLuongNgayDung";
            // 
            // calcCachDung
            // 
            this.calcCachDung.Name = "calcCachDung";
            // 
            // calcLoiDanChung
            // 
            this.calcLoiDanChung.Name = "calcLoiDanChung";
            // 
            // calcDayMonthYearCurrent
            // 
            this.calcDayMonthYearCurrent.Name = "calcDayMonthYearCurrent";
            // 
            // calcBsRaToa
            // 
            this.calcBsRaToa.Name = "calcBsRaToa";
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel9,
            this.xrLabel6,
            this.xrLabel8,
            this.xrLabel1,
            this.xrLabel68,
            this.xrLabel69,
            this.xrLabel70,
            this.xrLabel65,
            this.xrLabel66,
            this.xrLabel67,
            this.xrLabel38,
            this.xrLabel75,
            this.xrLabel74,
            this.xrLabel73});
            this.ReportFooter.HeightF = 248.2702F;
            this.ReportFooter.KeepTogether = true;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel9
            // 
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(21F, 144.665F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(218.4333F, 39.66669F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "(ký, ghi rõ họ tên \r\nvà đã nhận đủ thuốc)";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(21F, 121.665F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(218.4333F, 23.00001F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "NGƯỜI NHẬN THUỐC";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.FullName]")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(21F, 225.2702F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(218.4333F, 22.99998F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "xrLabel40";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(568.5667F, 144.665F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(218.4333F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "(ký, ghi rõ họ tên)";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel68
            // 
            this.xrLabel68.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.ApptDate]")});
            this.xrLabel68.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel68.LocationFloat = new DevExpress.Utils.PointFloat(125.4183F, 46.66498F);
            this.xrLabel68.Name = "xrLabel68";
            this.xrLabel68.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel68.SizeF = new System.Drawing.SizeF(625.9987F, 25F);
            this.xrLabel68.StylePriority.UseFont = false;
            this.xrLabel68.StylePriority.UseTextAlignment = false;
            this.xrLabel68.Text = "xrLabel68";
            this.xrLabel68.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel68.TextFormatString = "{0:\'Ngày\' dd \'tháng\' MM \'năm\' yyyy}";
            // 
            // xrLabel69
            // 
            this.xrLabel69.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.ApptLocationName]")});
            this.xrLabel69.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel69.LocationFloat = new DevExpress.Utils.PointFloat(40.0001F, 71.66499F);
            this.xrLabel69.Name = "xrLabel69";
            this.xrLabel69.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel69.SizeF = new System.Drawing.SizeF(711.4169F, 24.99999F);
            this.xrLabel69.StylePriority.UseFont = false;
            this.xrLabel69.StylePriority.UsePadding = false;
            this.xrLabel69.StylePriority.UseTextAlignment = false;
            this.xrLabel69.Text = "xrLabel69";
            this.xrLabel69.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel70
            // 
            this.xrLabel70.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel70.LocationFloat = new DevExpress.Utils.PointFloat(10.0001F, 71.66498F);
            this.xrLabel70.Name = "xrLabel70";
            this.xrLabel70.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel70.SizeF = new System.Drawing.SizeF(30F, 25F);
            this.xrLabel70.StylePriority.UseFont = false;
            this.xrLabel70.StylePriority.UseTextAlignment = false;
            this.xrLabel70.Text = "Tại:";
            this.xrLabel70.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel65
            // 
            this.xrLabel65.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel65.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 0F);
            this.xrLabel65.Name = "xrLabel65";
            this.xrLabel65.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel65.SizeF = new System.Drawing.SizeF(55F, 23F);
            this.xrLabel65.StylePriority.UseFont = false;
            this.xrLabel65.StylePriority.UseTextAlignment = false;
            this.xrLabel65.Text = "Lời dặn:";
            this.xrLabel65.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel66
            // 
            this.xrLabel66.CanShrink = true;
            this.xrLabel66.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.DoctorAdvice]")});
            this.xrLabel66.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel66.LocationFloat = new DevExpress.Utils.PointFloat(10.0001F, 23F);
            this.xrLabel66.Multiline = true;
            this.xrLabel66.Name = "xrLabel66";
            this.xrLabel66.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel66.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel66.SizeF = new System.Drawing.SizeF(777F, 23.66498F);
            this.xrLabel66.StylePriority.UseFont = false;
            this.xrLabel66.StylePriority.UseTextAlignment = false;
            this.xrLabel66.Text = "xrLabel66";
            this.xrLabel66.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel67
            // 
            this.xrLabel67.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel67.LocationFloat = new DevExpress.Utils.PointFloat(10.0001F, 46.66498F);
            this.xrLabel67.Name = "xrLabel67";
            this.xrLabel67.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel67.SizeF = new System.Drawing.SizeF(115.21F, 25F);
            this.xrLabel67.StylePriority.UseFont = false;
            this.xrLabel67.StylePriority.UseTextAlignment = false;
            this.xrLabel67.Text = "Hẹn khám lại vào:";
            this.xrLabel67.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel38
            // 
            this.xrLabel38.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.ReasonToAllowPaperReferral]")});
            this.xrLabel38.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(10.0001F, 96.66498F);
            this.xrLabel38.Multiline = true;
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(541.4233F, 25.00003F);
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.StylePriority.UseTextAlignment = false;
            this.xrLabel38.Text = "xrLabel29";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel75
            // 
            this.xrLabel75.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.IssuedDateTime]")});
            this.xrLabel75.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel75.LocationFloat = new DevExpress.Utils.PointFloat(568.5668F, 98.66501F);
            this.xrLabel75.Name = "xrLabel75";
            this.xrLabel75.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel75.SizeF = new System.Drawing.SizeF(218.4333F, 23F);
            this.xrLabel75.StylePriority.UseFont = false;
            this.xrLabel75.StylePriority.UseTextAlignment = false;
            this.xrLabel75.Text = "xrLabel28";
            this.xrLabel75.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel75.TextFormatString = "{0:\'Ngày\' dd \'tháng\' MM \'năm\' yyyy}";
            // 
            // xrLabel74
            // 
            this.xrLabel74.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel74.LocationFloat = new DevExpress.Utils.PointFloat(568.5668F, 121.665F);
            this.xrLabel74.Name = "xrLabel74";
            this.xrLabel74.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel74.SizeF = new System.Drawing.SizeF(218.4333F, 23.00001F);
            this.xrLabel74.StylePriority.UseFont = false;
            this.xrLabel74.StylePriority.UseTextAlignment = false;
            this.xrLabel74.Text = "BÁC SĨ KHÁM BỆNH";
            this.xrLabel74.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel73
            // 
            this.xrLabel73.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.IssuerStaffIDName]")});
            this.xrLabel73.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel73.LocationFloat = new DevExpress.Utils.PointFloat(568.5668F, 225.2702F);
            this.xrLabel73.Name = "xrLabel73";
            this.xrLabel73.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel73.SizeF = new System.Drawing.SizeF(218.4333F, 22.99998F);
            this.xrLabel73.StylePriority.UseFont = false;
            this.xrLabel73.StylePriority.UseTextAlignment = false;
            this.xrLabel73.Text = "xrLabel40";
            this.xrLabel73.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine2,
            this.xrLabel47,
            this.xrPageInfo1});
            this.PageFooter.HeightF = 20.16675F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLine2
            // 
            this.xrLine2.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Dot;
            this.xrLine2.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(787F, 2.166684F);
            this.xrLine2.StylePriority.UseBorderDashStyle = false;
            // 
            // xrLabel47
            // 
            this.xrLabel47.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top;
            this.xrLabel47.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel47.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic);
            this.xrLabel47.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 2.166621F);
            this.xrLabel47.Name = "xrLabel47";
            this.xrLabel47.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel47.SizeF = new System.Drawing.SizeF(166.1002F, 18F);
            this.xrLabel47.StylePriority.UseFont = false;
            this.xrLabel47.StylePriority.UseTextAlignment = false;
            this.xrLabel47.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel47.TextFormatString = "{0:dd/MM/yyyy hh:mm tt}";
            // 
            // calcMark
            // 
            this.calcMark.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcMark.Expression = "Iif([IsDrugNotInCat]==0,\'-\'  ,\'*\' )";
            this.calcMark.Name = "calcMark";
            // 
            // calSang
            // 
            this.calSang.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calSang.Expression = "Iif(IsNullOrEmpty([PrescriptDetailScheduleID]), Iif(IsNullOrEmpty([MDoseStr]) Or " +
    "[MDoseStr] == \'\' Or [MDoseStr] == \'0\', \'\' , \'* Sáng: \' + [MDoseStr] + \' \' + Trim" +
    "([UnitUse])+ \'. \'),null)";
            this.calSang.Name = "calSang";
            // 
            // calTrua
            // 
            this.calTrua.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calTrua.Expression = "Iif(IsNullOrEmpty([PrescriptDetailScheduleID]), Iif(IsNullOrEmpty([NDoseStr]) Or " +
    "[NDoseStr] == \'\' Or [NDoseStr] == \'0\', \'\' ,\' *  Trưa: \' + [NDoseStr] + \' \' + Tri" +
    "m([UnitUse])+ \'. \'),null)";
            this.calTrua.Name = "calTrua";
            // 
            // calChieu
            // 
            this.calChieu.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calChieu.Expression = "Iif(IsNullOrEmpty([PrescriptDetailScheduleID]), Iif(IsNullOrEmpty([ADoseStr]) Or " +
    "[ADoseStr] == \'\' Or [ADoseStr] == \'0\', \'\' , \' * Chiều: \' + [ADoseStr] + \' \' + Tr" +
    "im([UnitUse])+ \'. \'),null)";
            this.calChieu.Name = "calChieu";
            // 
            // calToi
            // 
            this.calToi.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calToi.Expression = "Iif(IsNullOrEmpty([PrescriptDetailScheduleID]), Iif(IsNullOrEmpty([EDoseStr]) Or " +
    "[EDoseStr] == \'\' Or [EDoseStr] == \'0\', \'\' , \' *  Tối:  \' + [EDoseStr] + \' \' + Tr" +
    "im([UnitUse])+ \'. \'),null)";
            this.calToi.Name = "calToi";
            // 
            // calConsutation
            // 
            this.calConsutation.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calConsutation.Expression = "Iif(Len([ConsultantName]) > 0, \'BS Hội Chẩn:\' ,NULL )";
            this.calConsutation.Name = "calConsutation";
            // 
            // calBrandName
            // 
            this.calBrandName.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calBrandName.Expression = "Iif([IsDrugNotInCat]=true,\'(\' + \' * \' +  [BrandName] + \')\',\'(\' +[BrandName] + \')\'" +
    ")";
            this.calBrandName.Name = "calBrandName";
            // 
            // calBsRatoa
            // 
            this.calBsRatoa.DataMember = "spPrescriptions_RptHeaderByIssueID";
            this.calBsRatoa.Expression = "Upper(Trim([IssuerStaffIDName]))";
            this.calBsRatoa.Name = "calBsRatoa";
            // 
            // calLoiDanChung
            // 
            this.calLoiDanChung.DataMember = "spPrescriptions_RptHeaderByIssueID";
            this.calLoiDanChung.DisplayName = "calLoiDanChung";
            this.calLoiDanChung.Expression = "Trim([DoctorAdvice])";
            this.calLoiDanChung.Name = "calLoiDanChung";
            // 
            // calReIssue
            // 
            this.calReIssue.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calReIssue.Expression = "Iif([V_PrescriptionIssuedCase] == \'2305\', \'Lưu ý: Toa thuốc này được cập nhật từ " +
    "1 toa đã xuất bán rồi.\' , NULL)";
            this.calReIssue.Name = "calReIssue";
            // 
            // parIsPsychotropicDrugs
            // 
            this.parIsPsychotropicDrugs.Description = "parIsPsychotropicDrugs";
            this.parIsPsychotropicDrugs.Name = "parIsPsychotropicDrugs";
            this.parIsPsychotropicDrugs.Type = typeof(bool);
            this.parIsPsychotropicDrugs.ValueInfo = "False";
            this.parIsPsychotropicDrugs.Visible = false;
            // 
            // parIsFuncfoodsOrCosmetics
            // 
            this.parIsFuncfoodsOrCosmetics.Description = "parIsFuncfoodsOrCosmetics";
            this.parIsFuncfoodsOrCosmetics.Name = "parIsFuncfoodsOrCosmetics";
            this.parIsFuncfoodsOrCosmetics.Type = typeof(bool);
            this.parIsFuncfoodsOrCosmetics.ValueInfo = "False";
            this.parIsFuncfoodsOrCosmetics.Visible = false;
            // 
            // dsPrescriptionNew_InPt1
            // 
            this.dsPrescriptionNew_InPt1.DataSetName = "dsPrescriptionNew_InPt";
            this.dsPrescriptionNew_InPt1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spPrescriptions_RptHeaderByIssueID_InPtTableAdapter
            // 
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrBarCode1,
            this.xrLabel61,
            this.xrLabel2});
            this.PageHeader.HeightF = 150F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.PrescriptionIssueCode]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(21F, 107F);
            this.xrBarCode1.Module = 1F;
            this.xrBarCode1.Name = "xrBarCode1";
            this.xrBarCode1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrBarCode1.SizeF = new System.Drawing.SizeF(118.34F, 43F);
            this.xrBarCode1.StylePriority.UseFont = false;
            this.xrBarCode1.StylePriority.UsePadding = false;
            this.xrBarCode1.StylePriority.UseTextAlignment = false;
            this.xrBarCode1.Symbology = code128Generator1;
            this.xrBarCode1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel61
            // 
            this.xrLabel61.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'ID: \' + [spPrescriptions_RptHeaderByIssueID_InPt.PatientCode]")});
            this.xrLabel61.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(139.34F, 107F);
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(155.4167F, 20F);
            this.xrLabel61.StylePriority.UseFont = false;
            this.xrLabel61.StylePriority.UseTextAlignment = false;
            this.xrLabel61.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'BA: \' + [spPrescriptions_RptHeaderByIssueID_InPt.FileCodeNumber]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(139.34F, 130F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(155.4167F, 20F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parHospitalCode
            // 
            this.parHospitalCode.Description = "parHospitalCode";
            this.parHospitalCode.Name = "parHospitalCode";
            this.parHospitalCode.Visible = false;
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_SubLeft_TV1();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(200F, 626F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrSubreport1_BeforePrint);
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(200F, 35.50002F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionNew_InPt_V2_TV();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(585F, 590.5F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrSubreport2_BeforePrint);
            // 
            // XRptEPrescriptionInpt_TV
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.topMarginBand1,
            this.bottomMarginBand1,
            this.ReportFooter,
            this.PageFooter,
            this.PageHeader});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedFieldGender,
            this.calcGenericName,
            this.calcSoLuongNgayDung,
            this.calcCachDung,
            this.calcLoiDanChung,
            this.calcDayMonthYearCurrent,
            this.calcBsRaToa,
            this.calcMark,
            this.calSang,
            this.calTrua,
            this.calChieu,
            this.calToi,
            this.calConsutation,
            this.calBrandName,
            this.calBsRatoa,
            this.calLoiDanChung,
            this.calReIssue});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsPrescriptionNew_InPt1});
            this.DataAdapter = this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
            this.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.DataSource = this.dsPrescriptionNew_InPt1;
            this.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 21, 27);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parIssueID,
            this.parIsPsychotropicDrugs,
            this.parIsFuncfoodsOrCosmetics,
            this.parHospitalCode});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptEPrescriptionInpt_TV_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew_InPt1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.XRControlStyle FieldCaption;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.UI.XRControlStyle DataField;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.Parameters.Parameter parIssueID;
        private DevExpress.XtraReports.UI.CalculatedField calculatedFieldGender;
        private DevExpress.XtraReports.UI.CalculatedField calcGenericName;
        private DevExpress.XtraReports.UI.CalculatedField calcSoLuongNgayDung;
        private DevExpress.XtraReports.UI.CalculatedField calcCachDung;
        private DevExpress.XtraReports.UI.CalculatedField calcLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calcDayMonthYearCurrent;
        private DevExpress.XtraReports.UI.CalculatedField calcBsRaToa;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.CalculatedField calcMark;
        private DevExpress.XtraReports.UI.CalculatedField calSang;
        private DevExpress.XtraReports.UI.CalculatedField calTrua;
        private DevExpress.XtraReports.UI.CalculatedField calChieu;
        private DevExpress.XtraReports.UI.CalculatedField calToi;
        private DevExpress.XtraReports.UI.XRLabel xrLabel47;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.CalculatedField calConsutation;
        private DevExpress.XtraReports.UI.CalculatedField calBrandName;
        private DevExpress.XtraReports.UI.CalculatedField calBsRatoa;
        private DevExpress.XtraReports.UI.CalculatedField calLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calReIssue;
        public DevExpress.XtraReports.Parameters.Parameter parIsPsychotropicDrugs;
        public DevExpress.XtraReports.Parameters.Parameter parIsFuncfoodsOrCosmetics;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel75;
        private DevExpress.XtraReports.UI.XRLabel xrLabel74;
        private DevExpress.XtraReports.UI.XRLabel xrLabel73;
        private DataSchema.dsPrescriptionNew_InPt dsPrescriptionNew_InPt1;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel68;
        private DevExpress.XtraReports.UI.XRLabel xrLabel69;
        private DevExpress.XtraReports.UI.XRLabel xrLabel70;
        private DevExpress.XtraReports.UI.XRLabel xrLabel65;
        private DevExpress.XtraReports.UI.XRLabel xrLabel66;
        private DevExpress.XtraReports.UI.XRLabel xrLabel67;
        private DevExpress.XtraReports.UI.XRLabel xrLabel38;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
    }
}
