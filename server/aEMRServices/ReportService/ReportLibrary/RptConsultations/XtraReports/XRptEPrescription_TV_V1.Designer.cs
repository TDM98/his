using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescription_TV_V1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XRptEPrescription_TV));
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPanel3 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parIssueID = new DevExpress.XtraReports.Parameters.Parameter();
            this.calculatedFieldGender = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcGenericName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcSoLuongNgayDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcCachDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcLoiDanChung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcDayMonthYearCurrent = new DevExpress.XtraReports.UI.CalculatedField();
            this.dsPrescriptionNew1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptHeaderByIssueIDTableAdapter();
            this.calcBsRaToa = new DevExpress.XtraReports.UI.CalculatedField();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel75 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel74 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel73 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel80 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel68 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel69 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel70 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel65 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel66 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel67 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel47 = new DevExpress.XtraReports.UI.XRLabel();
            this.calcMark = new DevExpress.XtraReports.UI.CalculatedField();
            this.calSang = new DevExpress.XtraReports.UI.CalculatedField();
            this.calTrua = new DevExpress.XtraReports.UI.CalculatedField();
            this.calChieu = new DevExpress.XtraReports.UI.CalculatedField();
            this.calToi = new DevExpress.XtraReports.UI.CalculatedField();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.calConsutation = new DevExpress.XtraReports.UI.CalculatedField();
            this.calBrandName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calBsRatoa = new DevExpress.XtraReports.UI.CalculatedField();
            this.calLoiDanChung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calReIssue = new DevExpress.XtraReports.UI.CalculatedField();
            this.parIsPsychotropicDrugs = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsFuncfoodsOrCosmetics = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDetailBeforePrintCount = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel3,
            this.xrLabel8,
            this.xrLabel15,
            this.xrSubreport2});
            this.Detail.HeightF = 682.5416F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Detail_BeforePrint);
            // 
            // xrPanel3
            // 
            this.xrPanel3.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPanel3.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2,
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel5});
            this.xrPanel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel3.Name = "xrPanel3";
            this.xrPanel3.SizeF = new System.Drawing.SizeF(189.6419F, 682.5416F);
            this.xrPanel3.StylePriority.UseBorders = false;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(9.999895F, 33.41668F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(179.642F, 400.4165F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = resources.GetString("xrLabel2.Text");
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 466.4165F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(179.6419F, 216.1251F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = resources.GetString("xrLabel4.Text");
            // 
            // xrLabel3
            // 
            this.xrLabel3.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel3.BorderColor = System.Drawing.Color.Transparent;
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 443.8332F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(166.1002F, 22.58331F);
            this.xrLabel3.StylePriority.UseBackColor = false;
            this.xrLabel3.StylePriority.UseBorderColor = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "BÁC SĨ CỘNG TÁC ";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel5
            // 
            this.xrLabel5.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel5.BorderColor = System.Drawing.Color.Transparent;
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 0F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(166.1002F, 23.00002F);
            this.xrLabel5.StylePriority.UseBackColor = false;
            this.xrLabel5.StylePriority.UseBorderColor = false;
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "DANH SÁCH BÁC SĨ ";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(189.6419F, 33.41668F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(597.358F, 22.99998F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel15.ForeColor = System.Drawing.Color.DarkRed;
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(189.6419F, 0F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(597.358F, 33.41668F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UseForeColor = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
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
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(698.9F, 2.166621F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(79.72485F, 17.99997F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrPageInfo1.TextFormatString = "Trang {0}/{1}";
            // 
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.PrescriptionIssueCode]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(21.30191F, 107.6667F);
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
            this.calculatedFieldGender.DataMember = "spRpt_Patients_GetSumaryInfoByID";
            this.calculatedFieldGender.Expression = "Iif([Gender] == \'M\',\'Nam\'  ,\'Nữ\' )";
            this.calculatedFieldGender.Name = "calculatedFieldGender";
            // 
            // calcGenericName
            // 
            this.calcGenericName.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcGenericName.Expression = "Trim([GenericName])";
            this.calcGenericName.Name = "calcGenericName";
            // 
            // calcSoLuongNgayDung
            // 
            this.calcSoLuongNgayDung.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcSoLuongNgayDung.Expression = "Round([QtyRpts],2) + \' \' + Trim([UnitName])+\'/(\' + [DayRpts] + \'N\' + \')\'\r\n";
            this.calcSoLuongNgayDung.Name = "calcSoLuongNgayDung";
            // 
            // calcCachDung
            // 
            this.calcCachDung.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcCachDung.Expression = "\'(\' + Trim([Administration]) + \')\'";
            this.calcCachDung.Name = "calcCachDung";
            // 
            // calcLoiDanChung
            // 
            this.calcLoiDanChung.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcLoiDanChung.Expression = "Trim([DoctorAdvice])\r\n\r\n";
            this.calcLoiDanChung.Name = "calcLoiDanChung";
            // 
            // calcDayMonthYearCurrent
            // 
            this.calcDayMonthYearCurrent.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcDayMonthYearCurrent.Expression = "\'Ngày \' + GetDay(Now()) + \' tháng \' + GetMonth(Now()) + \' năm \' + GetYear(Now())";
            this.calcDayMonthYearCurrent.Name = "calcDayMonthYearCurrent";
            // 
            // dsPrescriptionNew1
            // 
            this.dsPrescriptionNew1.DataSetName = "dsPrescriptionNew";
            this.dsPrescriptionNew1.EnforceConstraints = false;
            this.dsPrescriptionNew1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptHeaderByIssueIDTableAdapter
            // 
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.ClearBeforeFill = true;
            // 
            // calcBsRaToa
            // 
            this.calcBsRaToa.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.calcBsRaToa.Expression = "Upper(Trim([IssuerStaffIDName]))";
            this.calcBsRaToa.Name = "calcBsRaToa";
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1,
            this.xrLabel75,
            this.xrLabel74,
            this.xrLabel73,
            this.xrLabel80,
            this.xrPanel1});
            this.ReportFooter.HeightF = 257.9402F;
            this.ReportFooter.KeepTogether = true;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel75
            // 
            this.xrLabel75.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.IssuedDateTime]")});
            this.xrLabel75.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel75.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 31.31332F);
            this.xrLabel75.Name = "xrLabel75";
            this.xrLabel75.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel75.SizeF = new System.Drawing.SizeF(225.8498F, 23F);
            this.xrLabel75.StylePriority.UseFont = false;
            this.xrLabel75.StylePriority.UseTextAlignment = false;
            this.xrLabel75.Text = "xrLabel28";
            this.xrLabel75.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel75.TextFormatString = "{0:\'Ngày\' dd \'tháng\' MM \'năm\' yyyy}";
            // 
            // xrLabel74
            // 
            this.xrLabel74.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel74.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 54.31328F);
            this.xrLabel74.Name = "xrLabel74";
            this.xrLabel74.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel74.SizeF = new System.Drawing.SizeF(225.8498F, 23.00001F);
            this.xrLabel74.StylePriority.UseFont = false;
            this.xrLabel74.StylePriority.UseTextAlignment = false;
            this.xrLabel74.Text = "Bác sĩ điều trị";
            this.xrLabel74.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel73
            // 
            this.xrLabel73.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.IssuerStaffIDName]")});
            this.xrLabel73.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel73.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 153.6885F);
            this.xrLabel73.Name = "xrLabel73";
            this.xrLabel73.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel73.SizeF = new System.Drawing.SizeF(225.8498F, 22.99998F);
            this.xrLabel73.StylePriority.UseFont = false;
            this.xrLabel73.StylePriority.UseTextAlignment = false;
            this.xrLabel73.Text = "xrLabel40";
            this.xrLabel73.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel80
            // 
            this.xrLabel80.BorderColor = System.Drawing.Color.Black;
            this.xrLabel80.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
            | DevExpress.XtraPrinting.BorderSide.Right)
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel80.BorderWidth = 1F;
            this.xrLabel80.CanShrink = true;
            this.xrLabel80.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel80.ForeColor = System.Drawing.Color.Black;
            this.xrLabel80.LocationFloat = new DevExpress.Utils.PointFloat(8.375175F, 226.6885F);
            this.xrLabel80.Name = "xrLabel80";
            this.xrLabel80.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel80.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel80.SizeF = new System.Drawing.SizeF(768.6248F, 31.25166F);
            this.xrLabel80.StylePriority.UseBorderColor = false;
            this.xrLabel80.StylePriority.UseBorders = false;
            this.xrLabel80.StylePriority.UseBorderWidth = false;
            this.xrLabel80.StylePriority.UseFont = false;
            this.xrLabel80.StylePriority.UseForeColor = false;
            this.xrLabel80.StylePriority.UsePadding = false;
            this.xrLabel80.StylePriority.UseTextAlignment = false;
            this.xrLabel80.Text = "BỆNH NHÂN ĐỒNG Ý CHI TRẢ THUỐC NGOÀI DANH MỤC BHYT.";
            this.xrLabel80.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel80.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrLabel80_BeforePrint);
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7,
            this.xrLabel6,
            this.xrLabel68,
            this.xrLabel69,
            this.xrLabel70,
            this.xrLabel65,
            this.xrLabel66,
            this.xrLabel67});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 9.999974F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(552.7751F, 166.6885F);
            // 
            // xrLabel7
            // 
            this.xrLabel7.CanShrink = true;
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.DoctorComments]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(10.00003F, 80.31342F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel7.SizeF = new System.Drawing.SizeF(537.6732F, 36.37511F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "xrLabel27";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 57.31341F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(118.3445F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Ghi chú của bác sĩ:";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel68
            // 
            this.xrLabel68.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.ApptDate]")});
            this.xrLabel68.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
            this.xrLabel68.LocationFloat = new DevExpress.Utils.PointFloat(128.3445F, 116.6885F);
            this.xrLabel68.Name = "xrLabel68";
            this.xrLabel68.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel68.SizeF = new System.Drawing.SizeF(419.3288F, 25F);
            this.xrLabel68.StylePriority.UseFont = false;
            this.xrLabel68.StylePriority.UseTextAlignment = false;
            this.xrLabel68.Text = "xrLabel72";
            this.xrLabel68.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel68.TextFormatString = "{0:\'Ngày\' dd \'tháng\' MM \'năm\' yyyy}";
            // 
            // xrLabel69
            // 
            this.xrLabel69.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.ApptLocationName]")});
            this.xrLabel69.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
            this.xrLabel69.LocationFloat = new DevExpress.Utils.PointFloat(43.13492F, 141.6885F);
            this.xrLabel69.Name = "xrLabel69";
            this.xrLabel69.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel69.SizeF = new System.Drawing.SizeF(504.5384F, 25F);
            this.xrLabel69.StylePriority.UseFont = false;
            this.xrLabel69.StylePriority.UsePadding = false;
            this.xrLabel69.StylePriority.UseTextAlignment = false;
            this.xrLabel69.Text = "xrLabel69";
            this.xrLabel69.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel70
            // 
            this.xrLabel70.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel70.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 141.6885F);
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
            this.xrLabel65.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrLabel65.Name = "xrLabel65";
            this.xrLabel65.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel65.SizeF = new System.Drawing.SizeF(118.3445F, 23F);
            this.xrLabel65.StylePriority.UseFont = false;
            this.xrLabel65.StylePriority.UseTextAlignment = false;
            this.xrLabel65.Text = "Lời dặn của bác sĩ:";
            this.xrLabel65.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel66
            // 
            this.xrLabel66.CanShrink = true;
            this.xrLabel66.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.DoctorAdvice]")});
            this.xrLabel66.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel66.LocationFloat = new DevExpress.Utils.PointFloat(15.10178F, 23.00002F);
            this.xrLabel66.Multiline = true;
            this.xrLabel66.Name = "xrLabel66";
            this.xrLabel66.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel66.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel66.SizeF = new System.Drawing.SizeF(537.6733F, 34.3134F);
            this.xrLabel66.StylePriority.UseFont = false;
            this.xrLabel66.StylePriority.UseTextAlignment = false;
            this.xrLabel66.Text = "xrLabel27";
            this.xrLabel66.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel67
            // 
            this.xrLabel67.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel67.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 116.6885F);
            this.xrLabel67.Name = "xrLabel67";
            this.xrLabel67.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel67.SizeF = new System.Drawing.SizeF(118.3445F, 25F);
            this.xrLabel67.StylePriority.UseFont = false;
            this.xrLabel67.StylePriority.UseTextAlignment = false;
            this.xrLabel67.Text = "Hẹn khám lại vào:";
            this.xrLabel67.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
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
            this.xrLine2.SizeF = new System.Drawing.SizeF(778.6248F, 2.166684F);
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
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel61,
            this.xrBarCode1});
            this.PageHeader.HeightF = 160.6667F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'BA: \' + [spPrescriptions_RptHeaderByIssueID.FileCodeNumber]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(145F, 130.6667F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(155.4167F, 20F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel61
            // 
            this.xrLabel61.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'ID: \' + [spPrescriptions_RptHeaderByIssueID.PatientCode]")});
            this.xrLabel61.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(145F, 107.6667F);
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(155.4167F, 20F);
            this.xrLabel61.StylePriority.UseFont = false;
            this.xrLabel61.StylePriority.UseTextAlignment = false;
            this.xrLabel61.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
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
            // parDetailBeforePrintCount
            // 
            this.parDetailBeforePrintCount.Description = "parDetailBeforePrintCount";
            this.parDetailBeforePrintCount.Name = "parDetailBeforePrintCount";
            this.parDetailBeforePrintCount.Type = typeof(int);
            this.parDetailBeforePrintCount.ValueInfo = "0";
            // 
            // xrTable1
            // 
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(10.00004F, 176.6885F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1,
            this.xrTableRow2});
            this.xrTable1.SizeF = new System.Drawing.SizeF(766.9999F, 50F);
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell3});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.StylePriority.UseTextAlignment = false;
            this.xrTableRow1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.CanShrink = true;
            this.xrTableCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.PCLBooked]")});
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell3.Weight = 1D;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.CanShrink = true;
            this.xrTableCell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.ReasonToAllowPaperReferral]")});
            this.xrTableCell1.Multiline = true;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell1.StylePriority.UsePadding = false;
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell1.Weight = 1D;
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(189.6419F, 56.41692F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionDetails_TV();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(597.3581F, 626.1247F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrSubreport2_BeforePrint);
            // 
            // XRptEPrescription_TV
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.PageHeader,
            this.Detail,
            this.topMarginBand1,
            this.bottomMarginBand1,
            this.ReportFooter,
            this.PageFooter});
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
            this.DataMember = "spPrescriptions_RptViewByPrescriptID";
            this.DataSource = this.dsPrescriptionNew1;
            this.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 21, 27);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parIssueID,
            this.parIsPsychotropicDrugs,
            this.parIsFuncfoodsOrCosmetics,
            this.parDetailBeforePrintCount});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptEPrescription_TV_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
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
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.Parameters.Parameter parIssueID;
        private DevExpress.XtraReports.UI.CalculatedField calculatedFieldGender;
        private DevExpress.XtraReports.UI.CalculatedField calcGenericName;
        private DevExpress.XtraReports.UI.CalculatedField calcSoLuongNgayDung;
        private DevExpress.XtraReports.UI.CalculatedField calcCachDung;
        private DevExpress.XtraReports.UI.CalculatedField calcLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calcDayMonthYearCurrent;
        private DataSchema.dsPrescriptionNew dsPrescriptionNew1;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptHeaderByIssueIDTableAdapter spPrescriptions_RptHeaderByIssueIDTableAdapter;
        private DevExpress.XtraReports.UI.CalculatedField calcBsRaToa;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode1;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.CalculatedField calcMark;
        private DevExpress.XtraReports.UI.CalculatedField calSang;
        private DevExpress.XtraReports.UI.CalculatedField calTrua;
        private DevExpress.XtraReports.UI.CalculatedField calChieu;
        private DevExpress.XtraReports.UI.CalculatedField calToi;
        private DevExpress.XtraReports.UI.XRLabel xrLabel47;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.CalculatedField calConsutation;
        private DevExpress.XtraReports.UI.CalculatedField calBrandName;
        private DevExpress.XtraReports.UI.CalculatedField calBsRatoa;
        private DevExpress.XtraReports.UI.CalculatedField calLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calReIssue;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel68;
        private DevExpress.XtraReports.UI.XRLabel xrLabel69;
        private DevExpress.XtraReports.UI.XRLabel xrLabel70;
        private DevExpress.XtraReports.UI.XRLabel xrLabel65;
        private DevExpress.XtraReports.UI.XRLabel xrLabel66;
        private DevExpress.XtraReports.UI.XRLabel xrLabel67;
        private DevExpress.XtraReports.UI.XRLabel xrLabel80;
        public DevExpress.XtraReports.Parameters.Parameter parIsPsychotropicDrugs;
        public DevExpress.XtraReports.Parameters.Parameter parIsFuncfoodsOrCosmetics;
        private DevExpress.XtraReports.UI.XRLabel xrLabel75;
        private DevExpress.XtraReports.UI.XRLabel xrLabel74;
        private DevExpress.XtraReports.UI.XRLabel xrLabel73;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRPanel xrPanel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.Parameters.Parameter parDetailBeforePrintCount;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
    }
}
