namespace eHCMS.ReportLib.RptAppointment
{
    partial class XRptPatientApptPCLRequestsXMLNew_TV
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
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.dsPatientApptPCLRequests1 = new eHCMS.ReportLib.RptAppointment.DataSchema.dsPatientApptPCLRequests();
            this.spRptPatientApptPCLRequestsXmlTableAdapter = new eHCMS.ReportLib.RptAppointment.DataSchema.dsPatientApptPCLRequestsTableAdapters.spRptPatientApptPCLRequestsXmlTableAdapter();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.parRequestXML = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parLogoUrl = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptPCLRequests1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 10F;
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
            // dsPatientApptPCLRequests1
            // 
            this.dsPatientApptPCLRequests1.DataSetName = "dsPatientApptPCLRequests";
            this.dsPatientApptPCLRequests1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spRptPatientApptPCLRequestsXmlTableAdapter
            // 
            this.spRptPatientApptPCLRequestsXmlTableAdapter.ClearBeforeFill = true;
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1});
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand;
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1});
            this.Detail1.HeightF = 62.5F;
            this.Detail1.Name = "Detail1";
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.LockedInUserDesigner = true;
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequests();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(563F, 62.5F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // parRequestXML
            // 
            this.parRequestXML.Name = "parRequestXML";
            this.parRequestXML.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // parLogoUrl
            // 
            this.parLogoUrl.Description = "parLogoUrl";
            this.parLogoUrl.Name = "parLogoUrl";
            // 
            // XRptPatientApptPCLRequestsXMLNew_TV
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.DetailReport});
            this.DataAdapter = this.spRptPatientApptPCLRequestsXmlTableAdapter;
            this.DataMember = "spRptPatientApptPCLRequestsXml";
            this.DataSource = this.dsPatientApptPCLRequests1;
            this.Margins = new System.Drawing.Printing.Margins(10, 10, 10, 10);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parRequestXML,
            this.parDepartmentOfHealth,
            this.parLogoUrl});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptPatientApptPCLRequestsXMLNew_TV_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientApptPCLRequests1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DataSchema.dsPatientApptPCLRequests dsPatientApptPCLRequests1;
        private DataSchema.dsPatientApptPCLRequestsTableAdapters.spRptPatientApptPCLRequestsXmlTableAdapter spRptPatientApptPCLRequestsXmlTableAdapter;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.Parameters.Parameter parRequestXML;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.Parameters.Parameter parLogoUrl;
    }
}
