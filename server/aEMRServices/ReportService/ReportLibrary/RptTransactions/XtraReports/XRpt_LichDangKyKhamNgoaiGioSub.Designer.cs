namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    partial class XRpt_LichDangKyKhamNgoaiGioSub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XRpt_LichDangKyKhamNgoaiGioSub));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.OvertimeWorkingWeekID = new DevExpress.XtraReports.Parameters.Parameter();
            this.WorkDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.spXRpt_LichDangKyKhamNgoaiGioTableAdapter = new eHCMS.ReportLib.RptTransactions.DataSchema.dsXRpt_LichDangKyKhamNgoaiGioTableAdapters.spXRpt_LichDangKyKhamNgoaiGioTableAdapter();
            this.dsXRpt_LichDangKyKhamNgoaiGioSub1 = new eHCMS.ReportLib.RptTransactions.DataSchema.dsXRpt_LichDangKyKhamNgoaiGioSub();
            this.spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter = new eHCMS.ReportLib.RptTransactions.DataSchema.dsXRpt_LichDangKyKhamNgoaiGioSubTableAdapters.spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_LichDangKyKhamNgoaiGioSub1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1});
            this.Detail.HeightF = 69F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.CanShrink = true;
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[LocationName]")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 46F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(156F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "xrLabel2";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel3.TextFormatString = "+ PK: {0}";
            // 
            // xrLabel2
            // 
            this.xrLabel2.CanShrink = true;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", resources.GetString("xrLabel2.ExpressionBindings"))});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(156F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.CanShrink = true;
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DoctorName]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(156F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel1";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel1.TextFormatString = "BS: {0}";
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
            // OvertimeWorkingWeekID
            // 
            this.OvertimeWorkingWeekID.Description = "Parameter1";
            this.OvertimeWorkingWeekID.Name = "OvertimeWorkingWeekID";
            this.OvertimeWorkingWeekID.Type = typeof(long);
            this.OvertimeWorkingWeekID.ValueInfo = "0";
            // 
            // WorkDate
            // 
            this.WorkDate.Description = "Parameter1";
            this.WorkDate.Name = "WorkDate";
            this.WorkDate.Type = typeof(System.DateTime);
            // 
            // spXRpt_LichDangKyKhamNgoaiGioTableAdapter
            // 
            this.spXRpt_LichDangKyKhamNgoaiGioTableAdapter.ClearBeforeFill = true;
            // 
            // dsXRpt_LichDangKyKhamNgoaiGioSub1
            // 
            this.dsXRpt_LichDangKyKhamNgoaiGioSub1.DataSetName = "dsXRpt_LichDangKyKhamNgoaiGioSub";
            this.dsXRpt_LichDangKyKhamNgoaiGioSub1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter
            // 
            this.spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter.ClearBeforeFill = true;
            // 
            // XRpt_LichDangKyKhamNgoaiGioSub
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRpt_LichDangKyKhamNgoaiGioSub1});
            this.DataAdapter = this.spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter;
            this.DataMember = "spXRpt_LichDangKyKhamNgoaiGioSub";
            this.DataSource = this.dsXRpt_LichDangKyKhamNgoaiGioSub1;
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 827;
            this.PageWidth = 156;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.OvertimeWorkingWeekID,
            this.WorkDate});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRpt_LichDangKyKhamNgoaiGioSub_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsXRpt_LichDangKyKhamNgoaiGioSub1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        public DevExpress.XtraReports.Parameters.Parameter OvertimeWorkingWeekID;
        public DevExpress.XtraReports.Parameters.Parameter WorkDate;
        private DataSchema.dsXRpt_LichDangKyKhamNgoaiGioTableAdapters.spXRpt_LichDangKyKhamNgoaiGioTableAdapter spXRpt_LichDangKyKhamNgoaiGioTableAdapter;
        private DataSchema.dsXRpt_LichDangKyKhamNgoaiGioSub dsXRpt_LichDangKyKhamNgoaiGioSub1;
        private DataSchema.dsXRpt_LichDangKyKhamNgoaiGioSubTableAdapters.spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
    }
}
