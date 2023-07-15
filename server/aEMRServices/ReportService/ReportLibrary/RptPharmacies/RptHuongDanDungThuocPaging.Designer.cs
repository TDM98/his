using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    partial class RptHuongDanDungThuocPaging
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
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsPhieuSoanThuocPaging1 = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuSoanThuocPaging();
            this.spPhieuSoanThuocPagingTableAdapter = new eHCMS.ReportLib.RptPharmacies.DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter();
            this.BeOfHIMedicineList = new DevExpress.XtraReports.Parameters.Parameter();
            this.RptHuongDanDungThuoc = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuSoanThuocPaging1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.HeightF = 0F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.RptHuongDanDungThuoc});
            this.detailBand1.HeightF = 118F;
            this.detailBand1.Name = "detailBand1";
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 116F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 0F;
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
            // dsPhieuSoanThuocPaging1
            // 
            this.dsPhieuSoanThuocPaging1.DataSetName = "dsPhieuSoanThuocPaging";
            this.dsPhieuSoanThuocPaging1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spPhieuSoanThuocPagingTableAdapter
            // 
            this.spPhieuSoanThuocPagingTableAdapter.ClearBeforeFill = true;
            // 
            // BeOfHIMedicineList
            // 
            this.BeOfHIMedicineList.Name = "BeOfHIMedicineList";
            this.BeOfHIMedicineList.Type = typeof(bool);
            this.BeOfHIMedicineList.ValueInfo = "False";
            // 
            // RptHuongDanDungThuoc
            // 
            this.RptHuongDanDungThuoc.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.RptHuongDanDungThuoc.Name = "RptHuongDanDungThuoc";
            this.RptHuongDanDungThuoc.ReportSource = new eHCMS.ReportLib.RptPharmacies.RptHuongDanDungThuoc3x6();
            this.RptHuongDanDungThuoc.SizeF = new System.Drawing.SizeF(236F, 118F);
            this.RptHuongDanDungThuoc.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.RptHuongDanDungThuoc_BeforePrint);
            // 
            // RptHuongDanDungThuocPaging
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
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 118;
            this.PageWidth = 236;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegistrationID,
            this.BeOfHIMedicineList});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.RptHuongDanDungThuocPaging_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPhieuSoanThuocPaging1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRSubreport RptHuongDanDungThuoc;
        public DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        private DataSchema.dsPhieuSoanThuocPaging dsPhieuSoanThuocPaging1;
        private DataSchema.dsPhieuSoanThuocPagingTableAdapters.spPhieuSoanThuocPagingTableAdapter spPhieuSoanThuocPagingTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        public DevExpress.XtraReports.Parameters.Parameter BeOfHIMedicineList;
    }
}
