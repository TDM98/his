using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescriptionInPtDetails_V5_Info
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XRptEPrescriptionInPtDetails_V5_Info));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parIssueID = new DevExpress.XtraReports.Parameters.Parameter();
            this.calculatedFieldGender = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcGenericName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcSoLuongNgayDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcCachDung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcLoiDanChung = new DevExpress.XtraReports.UI.CalculatedField();
            this.calcDayMonthYearCurrent = new DevExpress.XtraReports.UI.CalculatedField();
            this.groupHeaderBand1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.calcBsRaToa = new DevExpress.XtraReports.UI.CalculatedField();
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
            this.dsPrescriptionNew_InPt1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPt();
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.xrLabel79 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel63 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel62 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel61 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel58 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel72 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel40 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel33 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCheckBox1 = new DevExpress.XtraReports.UI.XRCheckBox();
            this.xrCheckBox2 = new DevExpress.XtraReports.UI.XRCheckBox();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel36 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dsPrescriptionNew_InPt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
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
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 10F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 15F;
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
            this.calcGenericName.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcGenericName.Expression = "Trim([GenericName])";
            this.calcGenericName.Name = "calcGenericName";
            // 
            // calcSoLuongNgayDung
            // 
            this.calcSoLuongNgayDung.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcSoLuongNgayDung.Expression = "\'SL: \' + Round([QtyRpts],2) + \' \' + Trim([UnitName])";
            this.calcSoLuongNgayDung.Name = "calcSoLuongNgayDung";
            // 
            // calcCachDung
            // 
            this.calcCachDung.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcCachDung.Expression = "Trim([Administration])";
            this.calcCachDung.Name = "calcCachDung";
            // 
            // calcLoiDanChung
            // 
            this.calcLoiDanChung.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcLoiDanChung.Expression = "Trim([DoctorAdvice])\r\n\r\n";
            this.calcLoiDanChung.Name = "calcLoiDanChung";
            // 
            // calcDayMonthYearCurrent
            // 
            this.calcDayMonthYearCurrent.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcDayMonthYearCurrent.Expression = "\'Ngày \' + GetDay(Now()) + \' tháng \' + GetMonth(Now()) + \' năm \' + GetYear(Now())";
            this.calcDayMonthYearCurrent.Name = "calcDayMonthYearCurrent";
            // 
            // groupHeaderBand1
            // 
            this.groupHeaderBand1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Row", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.groupHeaderBand1.HeightF = 0F;
            this.groupHeaderBand1.KeepTogether = true;
            this.groupHeaderBand1.Name = "groupHeaderBand1";
            this.groupHeaderBand1.StyleName = "DataField";
            // 
            // calcBsRaToa
            // 
            this.calcBsRaToa.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcBsRaToa.Expression = "Upper(Trim([IssuerStaffIDName]))";
            this.calcBsRaToa.Name = "calcBsRaToa";
            // 
            // calcMark
            // 
            this.calcMark.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calcMark.Expression = "Iif([IsDrugNotInCat]==0,\'-\'  ,\'*\' )";
            this.calcMark.Name = "calcMark";
            // 
            // calSang
            // 
            this.calSang.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calSang.Expression = resources.GetString("calSang.Expression");
            this.calSang.Name = "calSang";
            // 
            // calTrua
            // 
            this.calTrua.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calTrua.Expression = resources.GetString("calTrua.Expression");
            this.calTrua.Name = "calTrua";
            // 
            // calChieu
            // 
            this.calChieu.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calChieu.Expression = resources.GetString("calChieu.Expression");
            this.calChieu.Name = "calChieu";
            // 
            // calToi
            // 
            this.calToi.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calToi.Expression = resources.GetString("calToi.Expression");
            this.calToi.Name = "calToi";
            // 
            // calConsutation
            // 
            this.calConsutation.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calConsutation.Expression = "Iif(Len([ConsultantName]) > 0, \'BS Hội Chẩn:\' ,NULL )";
            this.calConsutation.Name = "calConsutation";
            // 
            // calBrandName
            // 
            this.calBrandName.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calBrandName.Expression = "Iif([IsDrugNotInCat]=true,\'(\' + \' * \' +  [BrandName] + \')\',\'(\' +[BrandName] + \')\'" +
    ")";
            this.calBrandName.Name = "calBrandName";
            // 
            // calBsRatoa
            // 
            this.calBsRatoa.DataMember = "spPrescriptions_RptHeaderByIssueID_InPt";
            this.calBsRatoa.Expression = "Upper(Trim([IssuerStaffIDName]))";
            this.calBsRatoa.Name = "calBsRatoa";
            // 
            // calLoiDanChung
            // 
            this.calLoiDanChung.DataMember = "spPrescriptions_RptHeaderByIssueID_InPt";
            this.calLoiDanChung.DisplayName = "calLoiDanChung";
            this.calLoiDanChung.Expression = "Trim([DoctorAdvice])";
            this.calLoiDanChung.Name = "calLoiDanChung";
            // 
            // calReIssue
            // 
            this.calReIssue.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.calReIssue.Expression = "Iif([V_PrescriptionIssuedCase] == \'2305\', \'Lưu ý: Toa thuốc này được cập nhật từ " +
    "1 toa đã xuất bán rồi.\' , NULL)";
            this.calReIssue.Name = "calReIssue";
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
            // xrLabel79
            // 
            this.xrLabel79.LocationFloat = new DevExpress.Utils.PointFloat(30.00005F, 97.17326F);
            this.xrLabel79.Name = "xrLabel79";
            this.xrLabel79.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel79.SizeF = new System.Drawing.SizeF(64.45831F, 23.16899F);
            this.xrLabel79.StylePriority.UseFont = false;
            this.xrLabel79.StylePriority.UseTextAlignment = false;
            this.xrLabel79.Text = "Địa chỉ:";
            this.xrLabel79.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel63
            // 
            this.xrLabel63.LocationFloat = new DevExpress.Utils.PointFloat(272.8365F, 28.25533F);
            this.xrLabel63.Name = "xrLabel63";
            this.xrLabel63.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel63.SizeF = new System.Drawing.SizeF(60.29181F, 22.74467F);
            this.xrLabel63.StylePriority.UseFont = false;
            this.xrLabel63.StylePriority.UseTextAlignment = false;
            this.xrLabel63.Text = "Họ Tên:";
            this.xrLabel63.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel62
            // 
            this.xrLabel62.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.FullNameUPPER]")});
            this.xrLabel62.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel62.LocationFloat = new DevExpress.Utils.PointFloat(333.1284F, 28.25533F);
            this.xrLabel62.Name = "xrLabel62";
            this.xrLabel62.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel62.SizeF = new System.Drawing.SizeF(259.8549F, 23.169F);
            this.xrLabel62.StylePriority.UseFont = false;
            this.xrLabel62.StylePriority.UseTextAlignment = false;
            this.xrLabel62.Text = "xrLabel3";
            this.xrLabel62.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel61
            // 
            this.xrLabel61.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel61.LocationFloat = new DevExpress.Utils.PointFloat(15F, 3.318155F);
            this.xrLabel61.Multiline = true;
            this.xrLabel61.Name = "xrLabel61";
            this.xrLabel61.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel61.SizeF = new System.Drawing.SizeF(303.1251F, 23F);
            this.xrLabel61.StylePriority.UseFont = false;
            this.xrLabel61.StylePriority.UseTextAlignment = false;
            this.xrLabel61.Text = "I. PHẦN HÀNH CHÍNH:\r\n";
            this.xrLabel61.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel58
            // 
            this.xrLabel58.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.PatientStreetAddress]")});
            this.xrLabel58.LocationFloat = new DevExpress.Utils.PointFloat(94.45826F, 97.17325F);
            this.xrLabel58.Name = "xrLabel58";
            this.xrLabel58.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel58.SizeF = new System.Drawing.SizeF(674.2783F, 23.169F);
            this.xrLabel58.StylePriority.UseTextAlignment = false;
            this.xrLabel58.Text = "xrLabel13";
            this.xrLabel58.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.HICardNo]")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(110.9385F, 0.002988179F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(190.7283F, 23.1685F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "xrLabel14";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0.002981186F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(110.9385F, 23.17F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Số thẻ BHYT:";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLine3
            // 
            this.xrLine3.LineWidth = 2;
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(1.458486F, 166.3423F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(783.54F, 2.08F);
            // 
            // xrLabel7
            // 
            this.xrLabel7.CanShrink = true;
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID_InPt.Treatment]")});
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(30.00005F, 143.3423F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(738.7365F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "xrLabel43";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2,
            this.xrLine1,
            this.xrLabel1,
            this.xrLabel72,
            this.xrLabel20,
            this.xrLabel40,
            this.xrLabel33,
            this.xrCheckBox1,
            this.xrCheckBox2,
            this.xrLabel4,
            this.xrLabel36,
            this.xrLabel21,
            this.xrPanel1,
            this.xrLabel7,
            this.xrLine3,
            this.xrLabel58,
            this.xrLabel61,
            this.xrLabel62,
            this.xrLabel63,
            this.xrLabel79});
            this.GroupHeader1.HeightF = 168.4223F;
            this.GroupHeader1.Level = 1;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 2;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.458486F, 1.238155F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(783.54F, 2.08F);
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Chiều cao: \'+[spPrescriptions_RptHeaderByIssueID_InPt.Height]\n+\' cm\'")});
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(437.2571F, 51.42433F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(143.2261F, 22.57575F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Cân nặng:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel72
            // 
            this.xrLabel72.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Cân nặng: \'+[spPrescriptions_RptHeaderByIssueID_InPt.Weight]\n+\' kg\'")});
            this.xrLabel72.LocationFloat = new DevExpress.Utils.PointFloat(272.8365F, 51.42433F);
            this.xrLabel72.Name = "xrLabel72";
            this.xrLabel72.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel72.SizeF = new System.Drawing.SizeF(139.8193F, 22.57597F);
            this.xrLabel72.StylePriority.UseFont = false;
            this.xrLabel72.StylePriority.UseTextAlignment = false;
            this.xrLabel72.Text = "Cân nặng:";
            this.xrLabel72.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel20
            // 
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(30F, 51F);
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(75.41656F, 23.00007F);
            this.xrLabel20.StylePriority.UseFont = false;
            this.xrLabel20.StylePriority.UseTextAlignment = false;
            this.xrLabel20.Text = "Ngày sinh:";
            this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel40
            // 
            this.xrLabel40.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel40.LocationFloat = new DevExpress.Utils.PointFloat(105.431F, 51.00029F);
            this.xrLabel40.Name = "xrLabel40";
            this.xrLabel40.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel40.SizeF = new System.Drawing.SizeF(167.4055F, 23F);
            this.xrLabel40.StylePriority.UseFont = false;
            this.xrLabel40.StylePriority.UseTextAlignment = false;
            this.xrLabel40.Text = "xrLabel40";
            this.xrLabel40.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel33
            // 
            this.xrLabel33.LocationFloat = new DevExpress.Utils.PointFloat(592.9832F, 28.25533F);
            this.xrLabel33.Name = "xrLabel33";
            this.xrLabel33.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel33.SizeF = new System.Drawing.SizeF(70F, 22.99994F);
            this.xrLabel33.StylePriority.UseFont = false;
            this.xrLabel33.StylePriority.UseTextAlignment = false;
            this.xrLabel33.Text = "Giới Tính:";
            this.xrLabel33.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrCheckBox1
            // 
            this.xrCheckBox1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "CheckState", "Iif([spPrescriptions_RptHeaderByIssueID.GioiTinh] = \'Nam\', true, false)")});
            this.xrCheckBox1.LocationFloat = new DevExpress.Utils.PointFloat(662.9832F, 28.25527F);
            this.xrCheckBox1.Name = "xrCheckBox1";
            this.xrCheckBox1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrCheckBox1.SizeF = new System.Drawing.SizeF(57.29166F, 23F);
            this.xrCheckBox1.Text = "Nam";
            // 
            // xrCheckBox2
            // 
            this.xrCheckBox2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "CheckState", "Iif([spPrescriptions_RptHeaderByIssueID.GioiTinh] = \'Nam\', false, true)")});
            this.xrCheckBox2.LocationFloat = new DevExpress.Utils.PointFloat(720.2748F, 28.25527F);
            this.xrCheckBox2.Name = "xrCheckBox2";
            this.xrCheckBox2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrCheckBox2.SizeF = new System.Drawing.SizeF(48.46167F, 23F);
            this.xrCheckBox2.Text = "Nữ";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Chẩn đoán: \'+ [spPrescriptions_RptHeaderByIssueID.PrescriptionsDiagnosis]")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(30.00005F, 120.3423F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(738.7365F, 23.00002F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel36
            // 
            this.xrLabel36.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[spPrescriptions_RptHeaderByIssueID.PatientCode]")});
            this.xrLabel36.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel36.LocationFloat = new DevExpress.Utils.PointFloat(89F, 28.25533F);
            this.xrLabel36.Name = "xrLabel36";
            this.xrLabel36.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel36.SizeF = new System.Drawing.SizeF(137.9199F, 22.74467F);
            this.xrLabel36.StylePriority.UseFont = false;
            this.xrLabel36.StylePriority.UseTextAlignment = false;
            this.xrLabel36.Text = "xrLabel36";
            this.xrLabel36.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel21
            // 
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(30F, 28F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(59F, 23F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.Text = "Mã BN:";
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrPanel1
            // 
            this.xrPanel1.CanShrink = true;
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8,
            this.xrLabel6});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(30.00005F, 74.00028F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(738.7365F, 23.17298F);
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'BMI: \'+[spPrescriptions_RptHeaderByIssueID_InPt.BMI]")});
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(592.9833F, 51.42433F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(175.7532F, 22.57575F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // XRptEPrescriptionInPtDetails_V5_Info
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.groupHeaderBand1,
            this.topMarginBand1,
            this.bottomMarginBand1,
            this.GroupHeader1});
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
            this.DataAdapter = this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
            this.DataMember = "spPrescriptions_RptViewByPrescriptID_InPt";
            this.DataSource = this.dsPrescriptionNew_InPt1;
            this.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 10, 15);
            this.PageWidth = 785;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parIssueID});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptEPrescriptionNew_InPt_BeforePrint);
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
        private DevExpress.XtraReports.Parameters.Parameter parIssueID;
        private DevExpress.XtraReports.UI.CalculatedField calculatedFieldGender;
        private DevExpress.XtraReports.UI.CalculatedField calcGenericName;
        private DevExpress.XtraReports.UI.CalculatedField calcSoLuongNgayDung;
        private DevExpress.XtraReports.UI.CalculatedField calcCachDung;
        private DevExpress.XtraReports.UI.CalculatedField calcLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calcDayMonthYearCurrent;
        private DevExpress.XtraReports.UI.GroupHeaderBand groupHeaderBand1;
        private DevExpress.XtraReports.UI.CalculatedField calcBsRaToa;
        private DevExpress.XtraReports.UI.CalculatedField calcMark;
        private DevExpress.XtraReports.UI.CalculatedField calSang;
        private DevExpress.XtraReports.UI.CalculatedField calTrua;
        private DevExpress.XtraReports.UI.CalculatedField calChieu;
        private DevExpress.XtraReports.UI.CalculatedField calToi;
        private DevExpress.XtraReports.UI.CalculatedField calConsutation;
        private DevExpress.XtraReports.UI.CalculatedField calBrandName;
        private DevExpress.XtraReports.UI.CalculatedField calBsRatoa;
        private DevExpress.XtraReports.UI.CalculatedField calLoiDanChung;
        private DevExpress.XtraReports.UI.CalculatedField calReIssue;
        private DataSchema.dsPrescriptionNew_InPt dsPrescriptionNew_InPt1;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel79;
        private DevExpress.XtraReports.UI.XRLabel xrLabel63;
        private DevExpress.XtraReports.UI.XRLabel xrLabel62;
        private DevExpress.XtraReports.UI.XRLabel xrLabel61;
        private DevExpress.XtraReports.UI.XRLabel xrLabel58;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel36;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel33;
        private DevExpress.XtraReports.UI.XRCheckBox xrCheckBox1;
        private DevExpress.XtraReports.UI.XRCheckBox xrCheckBox2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel40;
        private DevExpress.XtraReports.UI.XRLabel xrLabel72;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
    }
}
