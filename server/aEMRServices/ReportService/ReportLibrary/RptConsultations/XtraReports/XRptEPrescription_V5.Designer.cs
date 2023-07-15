using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescription_V5
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
            DevExpress.XtraPrinting.BarCode.QRCodeGenerator qrCodeGenerator1 = new DevExpress.XtraPrinting.BarCode.QRCodeGenerator();
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            DevExpress.XtraPrinting.Shape.ShapeRectangle shapeRectangle1 = new DevExpress.XtraPrinting.Shape.ShapeRectangle();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrBarCode2 = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
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
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel75 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel74 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel73 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel65 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel66 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel80 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel47 = new DevExpress.XtraReports.UI.XRLabel();
            this.calcMark = new DevExpress.XtraReports.UI.CalculatedField();
            this.calSang = new DevExpress.XtraReports.UI.CalculatedField();
            this.calTrua = new DevExpress.XtraReports.UI.CalculatedField();
            this.calChieu = new DevExpress.XtraReports.UI.CalculatedField();
            this.calToi = new DevExpress.XtraReports.UI.CalculatedField();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrShape1 = new DevExpress.XtraReports.UI.XRShape();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
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
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.parPrescriptionSubRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPrescriptionMainRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parKBYTLink = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsYHCTPrescript = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.spPhieuSoanThuocPagingTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel17,
            this.xrLabel2,
            this.xrBarCode2,
            this.xrSubreport3,
            this.xrSubreport1,
            this.xrLabel8,
            this.xrLabel15,
            this.xrSubreport2});
            this.Detail.HeightF = 585.4435F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Detail_BeforePrint);
            // 
            // xrLabel2
            // 
            this.xrLabel2.CanShrink = true;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "iif(IsNullOrEmpty([Parameters].[parKBYTLink]),null,\'SỰ GÓP Ý CỦA QUÝ KHÁCH HÀNG\nS" +
                    "Ẽ GIÚP BỆNH VIỆN NGÀY CÀNG HOÀN THIỆN HƠN!\')")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 5F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.ForeColor = System.Drawing.Color.Black;
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 567.4435F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel2.SizeF = new System.Drawing.SizeF(200F, 18F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseForeColor = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrBarCode2
            // 
            this.xrBarCode2.AutoModule = true;
            this.xrBarCode2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "iif(IsNullOrEmpty([Parameters].[parKBYTLink]),null,[Parameters].[parKBYTLink])")});
            this.xrBarCode2.LocationFloat = new DevExpress.Utils.PointFloat(59.75952F, 507.4435F);
            this.xrBarCode2.Name = "xrBarCode2";
            this.xrBarCode2.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.xrBarCode2.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrBarCode2.ShowText = false;
            this.xrBarCode2.SizeF = new System.Drawing.SizeF(79.98112F, 60F);
            this.xrBarCode2.StylePriority.UseTextAlignment = false;
            qrCodeGenerator1.CompactionMode = DevExpress.XtraPrinting.BarCode.QRCodeCompactionMode.Byte;
            this.xrBarCode2.Symbology = qrCodeGenerator1;
            this.xrBarCode2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 56.41683F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionDetails_V5_Info();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(785.0583F, 70.83344F);
            this.xrSubreport3.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport3_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 127.2503F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(200F, 380.1932F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrSubreport1_BeforePrint);
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 33.41687F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel8.SizeF = new System.Drawing.SizeF(787F, 22.99998F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel15.ForeColor = System.Drawing.Color.DarkRed;
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(787F, 33.41668F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UseForeColor = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(200.0585F, 127.2503F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionDetails_V5_Drug();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(585.0001F, 458.1931F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrSubreport2_BeforePrint);
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
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(698.9F, 33.41834F);
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
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.PatientCode]")});
            this.xrBarCode1.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 106F);
            this.xrBarCode1.Module = 1F;
            this.xrBarCode1.Name = "xrBarCode1";
            this.xrBarCode1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrBarCode1.ShowText = false;
            this.xrBarCode1.SizeF = new System.Drawing.SizeF(106.7245F, 34F);
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
            this.xrLabel16,
            this.xrLabel14,
            this.xrLabel10,
            this.xrLabel13,
            this.xrLabel12,
            this.xrLabel11,
            this.xrLabel3,
            this.xrTable1,
            this.xrLabel75,
            this.xrLabel74,
            this.xrLabel73,
            this.xrPanel1});
            this.ReportFooter.HeightF = 209.6469F;
            this.ReportFooter.KeepTogether = true;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel16
            // 
            this.xrLabel16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.YHCTIssuerStaffIDName]")});
            this.xrLabel16.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 186.6469F);
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel16.SizeF = new System.Drawing.SizeF(225.8498F, 22.99998F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "xrLabel40";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel14
            // 
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(33.73893F, 132.3969F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel14.SizeF = new System.Drawing.SizeF(182.9277F, 23F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            this.xrLabel14.Text = "(ký và ghi rõ họ tên)";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(297.3391F, 132.3969F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel10.SizeF = new System.Drawing.SizeF(182.9277F, 23F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "(ký và ghi rõ họ tên)";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel13
            // 
            this.xrLabel13.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(297.3391F, 109.3969F);
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel13.SizeF = new System.Drawing.SizeF(182.9277F, 23.00002F);
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.StylePriority.UseTextAlignment = false;
            this.xrLabel13.Text = "Người cấp phát";
            this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel12
            // 
            this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 132.3969F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel12.SizeF = new System.Drawing.SizeF(224.2249F, 23.00002F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "(ký và ghi rõ họ tên)";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel11
            // 
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 109.3969F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel11.SizeF = new System.Drawing.SizeF(224.2249F, 22.99998F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.Text = "Thầy thuốc kê đơn";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(33.73893F, 109.3969F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel3.SizeF = new System.Drawing.SizeF(182.9277F, 23.00002F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Người bệnh";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTable1
            // 
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 59.39687F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1,
            this.xrTableRow2});
            this.xrTable1.SizeF = new System.Drawing.SizeF(552.7751F, 50F);
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
            this.xrTableCell3.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold);
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell3.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrTableCell3.StylePriority.UseFont = false;
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
            this.xrTableCell1.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTableCell1.Multiline = true;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell1.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.StylePriority.UsePadding = false;
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell1.Weight = 1D;
            // 
            // xrLabel75
            // 
            this.xrLabel75.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.IssuedDateTime]")});
            this.xrLabel75.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel75.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 0F);
            this.xrLabel75.Name = "xrLabel75";
            this.xrLabel75.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel75.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
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
            this.xrLabel74.LocationFloat = new DevExpress.Utils.PointFloat(552.7751F, 23.00002F);
            this.xrLabel74.Name = "xrLabel74";
            this.xrLabel74.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel74.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
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
            this.xrLabel73.LocationFloat = new DevExpress.Utils.PointFloat(552.775F, 109.3969F);
            this.xrLabel73.Name = "xrLabel73";
            this.xrLabel73.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel73.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel73.SizeF = new System.Drawing.SizeF(225.8498F, 22.99998F);
            this.xrLabel73.StylePriority.UseFont = false;
            this.xrLabel73.StylePriority.UseTextAlignment = false;
            this.xrLabel73.Text = "xrLabel40";
            this.xrLabel73.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7,
            this.xrLabel6,
            this.xrLabel65,
            this.xrLabel66});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(552.7751F, 59.39687F);
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.DoctorComments]")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(118.3444F, 23.00002F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel7.SizeF = new System.Drawing.SizeF(428.5164F, 23.00002F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "xrLabel27";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23.00003F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(118.3445F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Ghi chú của bác sĩ:";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel65
            // 
            this.xrLabel65.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.xrLabel65.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
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
            this.xrLabel66.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.DoctorAdvice]")});
            this.xrLabel66.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel66.LocationFloat = new DevExpress.Utils.PointFloat(118.3445F, 0F);
            this.xrLabel66.Multiline = true;
            this.xrLabel66.Name = "xrLabel66";
            this.xrLabel66.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel66.ProcessNullValues = DevExpress.XtraReports.UI.ValueSuppressType.SuppressAndShrink;
            this.xrLabel66.SizeF = new System.Drawing.SizeF(428.5164F, 23.00002F);
            this.xrLabel66.StylePriority.UseFont = false;
            this.xrLabel66.StylePriority.UseTextAlignment = false;
            this.xrLabel66.Text = "xrLabel27";
            this.xrLabel66.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel80,
            this.xrLine2,
            this.xrLabel47,
            this.xrPageInfo1});
            this.PageFooter.HeightF = 51.41834F;
            this.PageFooter.Name = "PageFooter";
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
            this.xrLabel80.LocationFloat = new DevExpress.Utils.PointFloat(9.187561F, 0F);
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
            this.xrLabel80.Visible = false;
            this.xrLabel80.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XrLabel80_BeforePrint);
            // 
            // xrLine2
            // 
            this.xrLine2.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Dot;
            this.xrLine2.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 31.25166F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(778.6248F, 2.166684F);
            this.xrLine2.StylePriority.UseBorderDashStyle = false;
            // 
            // xrLabel47
            // 
            this.xrLabel47.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel47.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic);
            this.xrLabel47.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 33.41834F);
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
            this.xrShape1,
            this.xrLabel5,
            this.xrLabel9,
            this.xrLabel4,
            this.xrLabel1,
            this.xrLabel61,
            this.xrBarCode1});
            this.PageHeader.HeightF = 168.7222F;
            this.PageHeader.Name = "PageHeader";
            this.PageHeader.SnapLinePadding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            // 
            // xrShape1
            // 
            this.xrShape1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrShape1.LocationFloat = new DevExpress.Utils.PointFloat(15.10169F, 17F);
            this.xrShape1.Name = "xrShape1";
            shapeRectangle1.Fillet = 10;
            this.xrShape1.Shape = shapeRectangle1;
            this.xrShape1.SizeF = new System.Drawing.SizeF(508.6124F, 126F);
            this.xrShape1.SnapLineMargin = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrShape1.StylePriority.UseBorders = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalAddress]")});
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(15.10181F, 99.99998F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(508.6122F, 42.99997F);
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "xrLabel5";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel9
            // 
            this.xrLabel9.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 13F, System.Drawing.FontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(15.10171F, 57F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.No;
            this.xrLabel9.SizeF = new System.Drawing.SizeF(508.6123F, 42.99998F);
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parDepartmentOfHealth]")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(15.10177F, 17F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(508.6123F, 40F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "xrLabel4";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'BA: \' + [spPrescriptions_RptHeaderByIssueID.FileCodeNumber]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(659.4996F, 123F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(125.5588F, 17F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel61
            // 
            this.xrLabel61.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'ID: \' + [spPrescriptions_RptHeaderByIssueID.PatientCode]")});
            this.xrLabel61.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(659.4996F, 106F);
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(125.5588F, 16.99999F);
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
            // parHospitalCode
            // 
            this.parHospitalCode.Description = "parHospitalCode";
            this.parHospitalCode.Name = "parHospitalCode";
            this.parHospitalCode.Visible = false;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // parPrescriptionSubRightHeader
            // 
            this.parPrescriptionSubRightHeader.Description = "parPrescriptionSubRightHeader";
            this.parPrescriptionSubRightHeader.Name = "parPrescriptionSubRightHeader";
            this.parPrescriptionSubRightHeader.Visible = false;
            // 
            // parPrescriptionMainRightHeader
            // 
            this.parPrescriptionMainRightHeader.Description = "parPrescriptionMainRightHeader";
            this.parPrescriptionMainRightHeader.Name = "parPrescriptionMainRightHeader";
            this.parPrescriptionMainRightHeader.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Visible = false;
            // 
            // parHospitalAddress
            // 
            this.parHospitalAddress.Description = "parHospitalAddress";
            this.parHospitalAddress.Name = "parHospitalAddress";
            this.parHospitalAddress.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parKBYTLink
            // 
            this.parKBYTLink.Name = "parKBYTLink";
            // 
            // parIsYHCTPrescript
            // 
            this.parIsYHCTPrescript.Name = "parIsYHCTPrescript";
            this.parIsYHCTPrescript.Type = typeof(bool);
            this.parIsYHCTPrescript.ValueInfo = "False";
            // 
            // xrLabel17
            // 
            this.xrLabel17.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[StoreServiceSeqNum]")});
            this.xrLabel17.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel17.ForeColor = System.Drawing.Color.Black;
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(15.10181F, 0F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(124.6388F, 33.41668F);
            this.xrLabel17.StylePriority.UseBorders = false;
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.StylePriority.UseForeColor = false;
            this.xrLabel17.StylePriority.UseTextAlignment = false;
            this.xrLabel17.Text = "STT 468";
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel17.TextFormatString = "STT {0}";
            this.xrLabel17.Visible = false;
            // 
            // spPhieuSoanThuocPagingTableAdapter
            // 
            this.spPhieuSoanThuocPagingTableAdapter.ClearBeforeFill = true;
            // 
            // XRptEPrescription_V5
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
            this.parDetailBeforePrintCount,
            this.parHospitalCode,
            this.parPrescriptionSubRightHeader,
            this.parPrescriptionMainRightHeader,
            this.parDepartmentOfHealth,
            this.parHospitalAddress,
            this.parHospitalName,
            this.parKBYTLink,
            this.parIsYHCTPrescript});
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
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.CalculatedField calConsutation;
        private DevExpress.XtraReports.UI.CalculatedField calBrandName;
        private DevExpress.XtraReports.UI.CalculatedField calBsRatoa;
        private DevExpress.XtraReports.UI.CalculatedField calLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calReIssue;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel65;
        private DevExpress.XtraReports.UI.XRLabel xrLabel66;
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
        private DevExpress.XtraReports.Parameters.Parameter parDetailBeforePrintCount;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter parPrescriptionSubRightHeader;
        public DevExpress.XtraReports.Parameters.Parameter parPrescriptionMainRightHeader;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRShape xrShape1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel80;
        public DevExpress.XtraReports.Parameters.Parameter parKBYTLink;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport3;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.Parameters.Parameter parIsYHCTPrescript;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private RptPharmacies.DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter spPhieuSoanThuocPagingTableAdapter;
    }
}
