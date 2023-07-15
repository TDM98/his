namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptOutPatientPhieuChiDinhXML
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
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.outPatientPhieuChiDinh = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientPhieuChiDinh();
            this.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientPhieuChiDinhTableAdapters.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter();
            this.param_ListID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_ItemID = new DevExpress.XtraReports.Parameters.Parameter();
            this.param_PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parLogoUrl = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.outPatientPhieuChiDinh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.xrSubreport1});
            this.Detail.HeightF = 106.25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 101.5417F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientPhieuChiDinh();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(543F, 100F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 20.83333F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 10F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // outPatientPhieuChiDinh
            // 
            this.outPatientPhieuChiDinh.DataSetName = "OutPatientPhieuChiDinh";
            this.outPatientPhieuChiDinh.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // param_ListID
            // 
            this.param_ListID.Name = "param_ListID";
            this.param_ListID.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            this.parHospitalName.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Visible = false;
            // 
            // param_ItemID
            // 
            this.param_ItemID.Description = "param_ItemID";
            this.param_ItemID.Name = "param_ItemID";
            this.param_ItemID.Type = typeof(long);
            this.param_ItemID.ValueInfo = "0";
            this.param_ItemID.Visible = false;
            // 
            // param_PtRegistrationID
            // 
            this.param_PtRegistrationID.Description = "param_PtRegistrationID";
            this.param_PtRegistrationID.Name = "param_PtRegistrationID";
            this.param_PtRegistrationID.Type = typeof(long);
            this.param_PtRegistrationID.ValueInfo = "0";
            // 
            // parLogoUrl
            // 
            this.parLogoUrl.Description = "parLogoUrl";
            this.parLogoUrl.Name = "parLogoUrl";
            // 
            // XRptOutPatientPhieuChiDinhXML
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.DataAdapter = this.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter;
            this.DataMember = "sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXml";
            this.DataSource = this.outPatientPhieuChiDinh;
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 21, 10);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.param_ListID,
            this.parHospitalName,
            this.parDepartmentOfHealth,
            this.param_ItemID,
            this.param_PtRegistrationID,
            this.parLogoUrl});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XtraReport1_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.outPatientPhieuChiDinh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DataSchema.OutPatientPhieuChiDinh outPatientPhieuChiDinh;
        private DataSchema.OutPatientPhieuChiDinhTableAdapters.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter param_ListID;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.Parameters.Parameter param_ItemID;
        private DevExpress.XtraReports.Parameters.Parameter param_PtRegistrationID;
        private DevExpress.XtraReports.Parameters.Parameter parLogoUrl;
    }
}
