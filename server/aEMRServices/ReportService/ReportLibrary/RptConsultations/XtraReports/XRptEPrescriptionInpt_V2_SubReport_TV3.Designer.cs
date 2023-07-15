namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescriptionInpt_V2_SubReport_TV3
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
            this.parIssueID = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPrescriptionMainRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPrescriptionSubRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parKBYTLink = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport1});
            this.Detail.HeightF = 43.75F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_TV3();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(787F, 43.75F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint_1);
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
            this.BottomMargin.HeightF = 20F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parIssueID
            // 
            this.parIssueID.Name = "parIssueID";
            this.parIssueID.Type = typeof(int);
            this.parIssueID.ValueInfo = "0";
            this.parIssueID.Visible = false;
            // 
            // spPrescriptions_RptHeaderByIssueID_InPtTableAdapter
            // 
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // parHospitalCode
            // 
            this.parHospitalCode.Description = "parHospitalCode";
            this.parHospitalCode.Name = "parHospitalCode";
            this.parHospitalCode.Visible = false;
            // 
            // parPrescriptionMainRightHeader
            // 
            this.parPrescriptionMainRightHeader.Description = "parPrescriptionMainRightHeader";
            this.parPrescriptionMainRightHeader.Name = "parPrescriptionMainRightHeader";
            // 
            // parPrescriptionSubRightHeader
            // 
            this.parPrescriptionSubRightHeader.Description = "parPrescriptionSubRightHeader";
            this.parPrescriptionSubRightHeader.Name = "parPrescriptionSubRightHeader";
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // parHospitalAddress
            // 
            this.parHospitalAddress.Description = "parHospitalAddress";
            this.parHospitalAddress.Name = "parHospitalAddress";
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parKBYTLink
            // 
            this.parKBYTLink.Name = "parKBYTLink";
            // 
            // XRptEPrescriptionInpt_V2_SubReport_TV3
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(20, 0, 10, 20);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parIssueID,
            this.parHospitalCode,
            this.parPrescriptionMainRightHeader,
            this.parPrescriptionSubRightHeader,
            this.parDepartmentOfHealth,
            this.parHospitalAddress,
            this.parHospitalName,
            this.parKBYTLink});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptEPrescriptionInpt_V2_SubReport_TV_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter parIssueID;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        private DevExpress.XtraReports.Parameters.Parameter parPrescriptionMainRightHeader;
        private DevExpress.XtraReports.Parameters.Parameter parPrescriptionSubRightHeader;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parKBYTLink;
    }
}
