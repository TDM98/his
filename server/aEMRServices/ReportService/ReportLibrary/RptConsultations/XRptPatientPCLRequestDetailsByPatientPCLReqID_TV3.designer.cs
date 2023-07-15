namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3
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
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parPatientPCLReqXML = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramV_RegistrationType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parLogoUrl = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsPatientPCLRequestDetails1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPatientPCLRequestDetails();
            this.sp_RptPatientPCLRequestXMLTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPatientPCLRequestDetailsTableAdapters.sp_RptPatientPCLRequestXMLTableAdapter();
            this.parPtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientPCLRequestDetails1)).BeginInit();
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
            this.xrSubreport1});
            this.detailBand1.HeightF = 77.49998F;
            this.detailBand1.Name = "detailBand1";
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 75.49998F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.589457E-05F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new DevExpress.XtraReports.UI.XtraReport();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(540F, 75.49996F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 9.375F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // parPatientPCLReqXML
            // 
            this.parPatientPCLReqXML.Description = "parPatientPCLReqXML";
            this.parPatientPCLReqXML.Name = "parPatientPCLReqXML";
            this.parPatientPCLReqXML.ValueInfo = "0";
            this.parPatientPCLReqXML.Visible = false;
            // 
            // paramV_RegistrationType
            // 
            this.paramV_RegistrationType.Name = "paramV_RegistrationType";
            this.paramV_RegistrationType.Type = typeof(int);
            this.paramV_RegistrationType.ValueInfo = "0";
            this.paramV_RegistrationType.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Visible = false;
            // 
            // parLogoUrl
            // 
            this.parLogoUrl.Name = "parLogoUrl";
            this.parLogoUrl.Visible = false;
            // 
            // dsPatientPCLRequestDetails1
            // 
            this.dsPatientPCLRequestDetails1.DataSetName = "dsPatientPCLRequestDetails";
            this.dsPatientPCLRequestDetails1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_RptPatientPCLRequestXMLTableAdapter
            // 
            this.sp_RptPatientPCLRequestXMLTableAdapter.ClearBeforeFill = true;
            // 
            // parPtRegistrationID
            // 
            this.parPtRegistrationID.Description = "parPtRegistrationID";
            this.parPtRegistrationID.Name = "parPtRegistrationID";
            this.parPtRegistrationID.Type = typeof(long);
            this.parPtRegistrationID.ValueInfo = "0";
            this.parPtRegistrationID.Visible = false;
            // 
            // XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsPatientPCLRequestDetails1});
            this.DataAdapter = this.sp_RptPatientPCLRequestXMLTableAdapter;
            this.DataMember = "sp_RptPatientPCLRequestXML";
            this.DataSource = this.dsPatientPCLRequestDetails1;
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 0, 9);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parPatientPCLReqXML,
            this.paramV_RegistrationType,
            this.parDepartmentOfHealth,
            this.parLogoUrl,
            this.parPtRegistrationID});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XtraReport1_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsPatientPCLRequestDetails1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        public DevExpress.XtraReports.Parameters.Parameter parPatientPCLReqXML;
        public DevExpress.XtraReports.Parameters.Parameter paramV_RegistrationType;
        private RptConsultations.DataSchema.dsPatientPCLRequestDetails dsPatientPCLRequestDetails1;
        private RptConsultations.DataSchema.dsPatientPCLRequestDetailsTableAdapters.sp_RptPatientPCLRequestXMLTableAdapter sp_RptPatientPCLRequestXMLTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parLogoUrl;
        private DevExpress.XtraReports.Parameters.Parameter parPtRegistrationID;
    }
}
