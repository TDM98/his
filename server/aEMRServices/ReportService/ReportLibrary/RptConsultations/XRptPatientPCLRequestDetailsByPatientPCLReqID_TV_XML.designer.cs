namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML
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
            this.parPatientPCLReqID = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramV_RegistrationType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parLogoUrl = new DevExpress.XtraReports.Parameters.Parameter();
            this.dsPatientPCLRequestDetails1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPatientPCLRequestDetails();
            this.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPatientPCLRequestDetailsTableAdapters.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
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
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID_TV();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(540F, 75.49996F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.HeightF = 9.375F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // parPatientPCLReqID
            // 
            this.parPatientPCLReqID.Name = "parPatientPCLReqID";
            this.parPatientPCLReqID.ValueInfo = "0";
            this.parPatientPCLReqID.Visible = false;
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
            // 
            // parLogoUrl
            // 
            this.parLogoUrl.Name = "parLogoUrl";
            // 
            // dsPatientPCLRequestDetails1
            // 
            this.dsPatientPCLRequestDetails1.DataSetName = "dsPatientPCLRequestDetails";
            this.dsPatientPCLRequestDetails1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter
            // 
            this.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsPatientPCLRequestDetails1});
            this.DataAdapter = this.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter;
            this.DataMember = "sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXml";
            this.DataSource = this.dsPatientPCLRequestDetails1;
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 0, 9);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parPatientPCLReqID,
            this.paramV_RegistrationType,
            this.parDepartmentOfHealth,
            this.parLogoUrl});
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
        public DevExpress.XtraReports.Parameters.Parameter parPatientPCLReqID;
        public DevExpress.XtraReports.Parameters.Parameter paramV_RegistrationType;
        private RptConsultations.DataSchema.dsPatientPCLRequestDetails dsPatientPCLRequestDetails1;
        private RptConsultations.DataSchema.dsPatientPCLRequestDetailsTableAdapters.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter;
        private RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        public DevExpress.XtraReports.Parameters.Parameter parLogoUrl;
    }
}
