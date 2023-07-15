namespace eHCMS.ReportLib.InPatient.Reports
{
    partial class XRptVitalSign_InPt_Paging
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
            this.dsXRptVitalSign_InPt_V21 = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPt_V2();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.spXRptVitalSign_InPt_PagingTableAdapter = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_InPt_PagingTableAdapter();
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.numericChartRangeControlClient1 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter();
            this.numericChartRangeControlClient2 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.numericChartRangeControlClient3 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.spXRptVitalSign_SubChart_InPt_V2TableAdapter = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_SubChart_InPt_V2TableAdapter();
            this.V_RegistrationType = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.spRptMaxillofacialOutPtMedicalFileTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsMaxillofacialOutPtMedicalFileFrontCoverTableAdapters.spRptMaxillofacialOutPtMedicalFileTableAdapter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.spXRptVitalSign_InPt_V2TableAdapter = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_InPt_V2TableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRptVitalSign_InPt_V21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1});
            this.Detail.HeightF = 1.7917F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0.0001271566F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.InPatient.Reports.XRptVitalSign_InPt_V2();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(786.9999F, 0F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // dsXRptVitalSign_InPt_V21
            // 
            this.dsXRptVitalSign_InPt_V21.DataSetName = "dsXRptVitalSign_InPt_V2";
            this.dsXRptVitalSign_InPt_V21.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 12.79163F;
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
            // spXRptVitalSign_InPt_PagingTableAdapter
            // 
            this.spXRptVitalSign_InPt_PagingTableAdapter.ClearBeforeFill = true;
            // 
            // PtRegistrationID
            // 
            this.PtRegistrationID.Name = "PtRegistrationID";
            this.PtRegistrationID.Type = typeof(long);
            this.PtRegistrationID.ValueInfo = "0";
            // 
            // spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter.ClearBeforeFill = true;
            // 
            // spXRptVitalSign_SubChart_InPt_V2TableAdapter
            // 
            this.spXRptVitalSign_SubChart_InPt_V2TableAdapter.ClearBeforeFill = true;
            // 
            // V_RegistrationType
            // 
            this.V_RegistrationType.Description = "Parameter1";
            this.V_RegistrationType.Name = "V_RegistrationType";
            this.V_RegistrationType.Type = typeof(long);
            this.V_RegistrationType.ValueInfo = "0";
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // spRptMaxillofacialOutPtMedicalFileTableAdapter
            // 
            this.spRptMaxillofacialOutPtMedicalFileTableAdapter.ClearBeforeFill = true;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "Parameter1";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "Parameter1";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // spXRptVitalSign_InPt_V2TableAdapter
            // 
            this.spXRptVitalSign_InPt_V2TableAdapter.ClearBeforeFill = true;
            // 
            // XRptVitalSign_InPt_Paging
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRptVitalSign_InPt_V21});
            this.DataAdapter = this.spXRptVitalSign_InPt_PagingTableAdapter;
            this.DataMember = "spXRptVitalSign_InPt_Paging";
            this.DataSource = this.dsXRptVitalSign_InPt_V21;
            this.Margins = new System.Drawing.Printing.Margins(10, 30, 13, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.ParameterPanelLayoutItems.AddRange(new DevExpress.XtraReports.Parameters.ParameterPanelLayoutItem[] {
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.PtRegistrationID, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.V_RegistrationType, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.parHospitalName, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.parDepartmentOfHealth, DevExpress.XtraReports.Parameters.Orientation.Horizontal)});
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegistrationID,
            this.V_RegistrationType,
            this.parHospitalName,
            this.parDepartmentOfHealth});
            this.RequestParameters = false;
            this.Version = "22.1";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptVitalSign_InPt_Paging_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsXRptVitalSign_InPt_V21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsXRptVitalSign_InPt_V2 dsXRptVitalSign_InPt_V21;
        private DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_InPt_PagingTableAdapter spXRptVitalSign_InPt_PagingTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient1;
        private PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient2;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient3;
        private DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_SubChart_InPt_V2TableAdapter spXRptVitalSign_SubChart_InPt_V2TableAdapter;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter V_RegistrationType;
        private RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private RptConsultations.DataSchema.dsMaxillofacialOutPtMedicalFileFrontCoverTableAdapters.spRptMaxillofacialOutPtMedicalFileTableAdapter spRptMaxillofacialOutPtMedicalFileTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DataSchema.dsXRptVitalSign_InPt_V2TableAdapters.spXRptVitalSign_InPt_V2TableAdapter spXRptVitalSign_InPt_V2TableAdapter;
    }
}
