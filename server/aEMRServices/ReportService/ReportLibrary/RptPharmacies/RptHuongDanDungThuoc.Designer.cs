using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    partial class RptHuongDanDungThuoc
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
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PrescriptID = new DevExpress.XtraReports.Parameters.Parameter();
            this.EvenStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.OddStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.spRpt_PhieuNhanThuoc_DetailsTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuocTableAdapters.spRpt_PhieuNhanThuoc_DetailsTableAdapter();
            this.spRpt_PhieuNhanThuoc_InfoTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuocTableAdapters.spRpt_PhieuNhanThuoc_InfoTableAdapter();
            this.calculatedStoreName = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedDOB = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedICD10 = new DevExpress.XtraReports.UI.CalculatedField();
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter();
            this.dsHuongDanDungThuoc1 = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsHuongDanDungThuoc();
            this.spGetHuongDanDungThuocTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsHuongDanDungThuocTableAdapters.spGetHuongDanDungThuocTableAdapter();
            this.BeOfHIMedicineList = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsHuongDanDungThuoc1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLine1,
            this.xrLabel5,
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1});
            this.Detail.Font = new System.Drawing.Font("Arial", 10F);
            this.Detail.HeightF = 142F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 3, 3, 3, 100F);
            this.Detail.StylePriority.UseFont = false;
            this.Detail.StylePriority.UsePadding = false;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DoseEN]")});
            this.xrLabel6.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(9.781274E-05F, 73.03397F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 5, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(207.9983F, 42F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UsePadding = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "xrLabel1";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLine1
            // 
            this.xrLine1.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(207.9984F, 31.03398F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLine1.SizeF = new System.Drawing.SizeF(2.144608F, 83.99999F);
            this.xrLine1.StylePriority.UsePadding = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'SL: {0} {1}\', [Qty], [UnitName])")});
            this.xrLabel5.Font = new System.Drawing.Font("Arial", 9F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(210.143F, 31.03398F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(88.8569F, 22.85881F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UsePadding = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Administration]")});
            this.xrLabel4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(210.1431F, 53.89279F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(88.8569F, 61.14118F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UsePadding = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'{0}, {1}\', [FullName], [AgeOnly])")});
            this.xrLabel3.Font = new System.Drawing.Font("Arial", 10F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 115.034F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(298.9999F, 26.96601F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DoseMA]")});
            this.xrLabel2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(9.781274E-05F, 31.03398F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 5, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(207.9983F, 42F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UsePadding = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "xrLabel1";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BrandName]")});
            this.xrLabel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(299F, 31.03398F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UsePadding = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
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
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 0F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 0F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // PrescriptID
            // 
            this.PrescriptID.Name = "PrescriptID";
            this.PrescriptID.Type = typeof(long);
            this.PrescriptID.ValueInfo = "0";
            this.PrescriptID.Visible = false;
            // 
            // EvenStyle
            // 
            this.EvenStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
            this.EvenStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.EvenStyle.Name = "EvenStyle";
            // 
            // OddStyle
            // 
            this.OddStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(232)))), ((int)(((byte)(220)))));
            this.OddStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.OddStyle.Name = "OddStyle";
            // 
            // spRpt_PhieuNhanThuoc_DetailsTableAdapter
            // 
            this.spRpt_PhieuNhanThuoc_DetailsTableAdapter.ClearBeforeFill = true;
            // 
            // spRpt_PhieuNhanThuoc_InfoTableAdapter
            // 
            this.spRpt_PhieuNhanThuoc_InfoTableAdapter.ClearBeforeFill = true;
            // 
            // calculatedStoreName
            // 
            this.calculatedStoreName.DataMember = "spRpt_PhieuNhanThuoc_Info";
            this.calculatedStoreName.Expression = "Upper([swhlName])";
            this.calculatedStoreName.Name = "calculatedStoreName";
            // 
            // calculatedDOB
            // 
            this.calculatedDOB.DataMember = "spRpt_PhieuNhanThuoc_Info";
            this.calculatedDOB.Expression = "Iif(Len([DOB]) > 0,GetYear([DOB]),[DOBString])";
            this.calculatedDOB.Name = "calculatedDOB";
            // 
            // calculatedICD10
            // 
            this.calculatedICD10.DataMember = "spRpt_PhieuNhanThuoc_Info";
            this.calculatedICD10.Expression = "Iif(Len([ICD10])>0,[ICD10],Iif(Len([DiagnosisFinal])>0, [DiagnosisFinal] ,[Diagno" +
    "sis] ) )";
            this.calculatedICD10.Name = "calculatedICD10";
            // 
            // sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter
            // 
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // dsHuongDanDungThuoc1
            // 
            this.dsHuongDanDungThuoc1.DataSetName = "dsHuongDanDungThuoc";
            this.dsHuongDanDungThuoc1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spGetHuongDanDungThuocTableAdapter
            // 
            this.spGetHuongDanDungThuocTableAdapter.ClearBeforeFill = true;
            // 
            // BeOfHIMedicineList
            // 
            this.BeOfHIMedicineList.Name = "BeOfHIMedicineList";
            this.BeOfHIMedicineList.Type = typeof(bool);
            this.BeOfHIMedicineList.ValueInfo = "True";
            // 
            // RptHuongDanDungThuoc
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.topMarginBand1,
            this.bottomMarginBand1});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedStoreName,
            this.calculatedDOB,
            this.calculatedICD10});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsHuongDanDungThuoc1});
            this.DataAdapter = this.spGetHuongDanDungThuocTableAdapter;
            this.DataMember = "spGetHuongDanDungThuoc";
            this.DataSource = this.dsHuongDanDungThuoc1;
            this.Font = new System.Drawing.Font("Arial", 8.5F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 142;
            this.PageWidth = 299;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PrescriptID,
            this.BeOfHIMedicineList});
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.EvenStyle,
            this.OddStyle});
            this.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.RptHuongDanDungThuoc_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsHuongDanDungThuoc1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.XRControlStyle FieldCaption;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRControlStyle EvenStyle;
        private DevExpress.XtraReports.UI.XRControlStyle OddStyle;
        private DataSchema.dsPhieuNhanThuocTableAdapters.spRpt_PhieuNhanThuoc_InfoTableAdapter spRpt_PhieuNhanThuoc_InfoTableAdapter;
        private DevExpress.XtraReports.UI.CalculatedField calculatedStoreName;
        private DevExpress.XtraReports.UI.CalculatedField calculatedDOB;
        private DevExpress.XtraReports.UI.CalculatedField calculatedICD10;
        private DataSchema.dsPhieuNhanThuocTableAdapters.spRpt_PhieuNhanThuoc_DetailsTableAdapter spRpt_PhieuNhanThuoc_DetailsTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter PrescriptID;
        private DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter;
        private DataSchema.dsHuongDanDungThuoc dsHuongDanDungThuoc1;
        private DataSchema.dsHuongDanDungThuocTableAdapters.spGetHuongDanDungThuocTableAdapter spGetHuongDanDungThuocTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.Parameters.Parameter BeOfHIMedicineList;
    }
}
