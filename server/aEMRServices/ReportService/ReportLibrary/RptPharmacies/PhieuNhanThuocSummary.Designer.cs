using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    partial class PhieuNhanThuocSummary
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
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.OutiID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter();
            this.phieuNhanThuoc = new DevExpress.XtraReports.UI.XRSubreport();
            this.phieuNhanThuocBH = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 20F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.phieuNhanThuoc,
            this.phieuNhanThuocBH});
            this.detailBand1.HeightF = 142.0833F;
            this.detailBand1.Name = "detailBand1";
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77.49998F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 10F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // OutiID
            // 
            this.OutiID.Description = "OutiID";
            this.OutiID.Name = "OutiID";
            this.OutiID.Type = typeof(long);
            this.OutiID.ValueInfo = "0";
            this.OutiID.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            this.parHospitalName.Visible = false;
            // 
            // sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter
            // 
            this.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // phieuNhanThuoc
            // 
            this.phieuNhanThuoc.LocationFloat = new DevExpress.Utils.PointFloat(0F, 80.95831F);
            this.phieuNhanThuoc.Name = "phieuNhanThuoc";
            this.phieuNhanThuoc.ReportSource = new eHCMS.ReportLib.RptPharmacies.PhieuNhanThuoc();
            this.phieuNhanThuoc.SizeF = new System.Drawing.SizeF(492.1667F, 61.12498F);
            this.phieuNhanThuoc.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.phieuNhanThuoc_BeforePrint);
            // 
            // phieuNhanThuocBH
            // 
            this.phieuNhanThuocBH.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.phieuNhanThuocBH.Name = "phieuNhanThuocBH";
            this.phieuNhanThuocBH.ReportSource = new eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocBH();
            this.phieuNhanThuocBH.SizeF = new System.Drawing.SizeF(492.1667F, 77.49998F);
            this.phieuNhanThuocBH.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.phieuNhanThuocBH_BeforePrint);
            // 
            // PhieuNhanThuocSummary
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 20, 10);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.OutiID,
            this.parHospitalName});
            this.RequestParameters = false;
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.UI.XRSubreport phieuNhanThuoc;
        private DevExpress.XtraReports.UI.XRSubreport phieuNhanThuocBH;
        public DevExpress.XtraReports.Parameters.Parameter OutiID;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DataSchema.dsPhieuNhanThuocTableAdapters.sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter;
    }
}
