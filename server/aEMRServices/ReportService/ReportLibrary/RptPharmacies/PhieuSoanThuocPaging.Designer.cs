using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    partial class PhieuSoanThuocPaging
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
            this.PhieuSoanThuocBH = new DevExpress.XtraReports.UI.XRSubreport();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsPhieuSoanThuocPaging1 = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuSoanThuocPaging();
            this.spPhieuSoanThuocPagingTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter();
            this.StaffName = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuSoanThuocPaging1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 7.291667F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.PhieuSoanThuocBH});
            this.detailBand1.HeightF = 108.25F;
            this.detailBand1.Name = "detailBand1";
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 102F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // PhieuSoanThuocBH
            // 
            this.PhieuSoanThuocBH.LocationFloat = new DevExpress.Utils.PointFloat(15.20833F, 0F);
            this.PhieuSoanThuocBH.Name = "PhieuSoanThuocBH";
            this.PhieuSoanThuocBH.ReportSource = new eHCMS.ReportLib.RptPharmacies.PhieuSoanThuocBH();
            this.PhieuSoanThuocBH.SizeF = new System.Drawing.SizeF(557.7916F, 102F);
            this.PhieuSoanThuocBH.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.PhieuSoanThuocBH_BeforePrint);
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 2.333387F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // PtRegistrationID
            // 
            this.PtRegistrationID.Description = "PtRegistrationID";
            this.PtRegistrationID.Name = "PtRegistrationID";
            this.PtRegistrationID.Type = typeof(long);
            this.PtRegistrationID.ValueInfo = "0";
            this.PtRegistrationID.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            this.parHospitalName.Visible = false;
            // 
            // dsPhieuSoanThuocPaging1
            // 
            this.dsPhieuSoanThuocPaging1.DataSetName = "dsPhieuSoanThuocPaging";
            this.dsPhieuSoanThuocPaging1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spPhieuSoanThuocPagingTableAdapter
            // 
            this.spPhieuSoanThuocPagingTableAdapter.ClearBeforeFill = true;
            // 
            // StaffName
            // 
            this.StaffName.Name = "StaffName";
            this.StaffName.Visible = false;
            // 
            // PhieuSoanThuocPaging
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsPhieuSoanThuocPaging1});
            this.DataAdapter = this.spPhieuSoanThuocPagingTableAdapter;
            this.DataMember = "spPhieuSoanThuocPaging";
            this.DataSource = this.dsPhieuSoanThuocPaging1;
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 7, 2);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegistrationID,
            this.parHospitalName,
            this.StaffName});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.PhieuSoanThuocPaging_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuSoanThuocPaging1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRSubreport PhieuSoanThuocBH;
        public DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DataSchema.dsPhieuSoanThuocPaging dsPhieuSoanThuocPaging1;
        private DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter spPhieuSoanThuocPagingTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        public DevExpress.XtraReports.Parameters.Parameter StaffName;
    }
}
