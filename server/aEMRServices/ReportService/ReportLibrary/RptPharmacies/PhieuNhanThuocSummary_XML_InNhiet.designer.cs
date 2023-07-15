namespace eHCMS.ReportLib.RptPharmacies
{
    partial class PhieuNhanThuocSummary_XML_InNhiet
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
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.dsPhieuNhanThuoc1 = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuoc();
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.OutiID = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuNhanThuoc1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1});
            this.Detail.HeightF = 42.70833F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocSummary_InNhiet();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(271F, 42.70833F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
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
            // dsPhieuNhanThuoc1
            // 
            this.dsPhieuNhanThuoc1.DataSetName = "dsPhieuNhanThuoc";
            this.dsPhieuNhanThuoc1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter
            // 
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // OutiID
            // 
            this.OutiID.Description = "OutiID";
            this.OutiID.Name = "OutiID";
            // 
            // PhieuNhanThuocSummary_XML_InNhiet
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.DataAdapter = this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter;
            this.DataMember = "sp_PhieuNhanThuocSummary_ByOutiIDXml";
            this.DataSource = this.dsPhieuNhanThuoc1;
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 20, 20);
            this.PageHeight = 827;
            this.PageWidth = 271;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parHospitalName,
            this.OutiID});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.PhieuNhanThuocSummary_XML_InNhiet_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuNhanThuoc1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DataSchema.dsPhieuNhanThuoc dsPhieuNhanThuoc1;
        private DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter OutiID;
    }
}
