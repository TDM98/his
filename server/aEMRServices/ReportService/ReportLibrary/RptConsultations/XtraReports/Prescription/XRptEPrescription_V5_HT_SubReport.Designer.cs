namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptEPrescription_V5_HT_SubReport
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
            this.xrPageBreak3 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrSubreport4 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageBreak2 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parIssueID = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsPsychotropicDrugs = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsFuncfoodsOrCosmetics = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.parHospitalCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPrescriptionMainRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parPrescriptionSubRightHeader = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsAddictive = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parKBYTLink = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak3,
            this.xrSubreport4,
            this.xrPageBreak2,
            this.xrSubreport3,
            this.xrPageBreak1,
            this.xrSubreport2,
            this.xrSubreport1});
            this.Detail.HeightF = 94F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageBreak3
            // 
            this.xrPageBreak3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 64F);
            this.xrPageBreak3.Name = "xrPageBreak3";
            // 
            // xrSubreport4
            // 
            this.xrSubreport4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 66F);
            this.xrSubreport4.Name = "xrSubreport4";
            this.xrSubreport4.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_HT();
            this.xrSubreport4.SizeF = new System.Drawing.SizeF(787F, 20F);
            this.xrSubreport4.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport4_BeforePrint);
            // 
            // xrPageBreak2
            // 
            this.xrPageBreak2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 42F);
            this.xrPageBreak2.Name = "xrPageBreak2";
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 44F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_HT();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(787F, 20F);
            this.xrSubreport3.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport3_BeforePrint);
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 20F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            this.xrPageBreak1.Visible = false;
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_HT();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(787F, 20F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport2_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_HT();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(786.9999F, 20F);
            this.xrSubreport1.Visible = false;
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
            this.BottomMargin.HeightF = 0F;
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
            // parIsPsychotropicDrugs
            // 
            this.parIsPsychotropicDrugs.Description = "parIsPsychotropicDrugs";
            this.parIsPsychotropicDrugs.Name = "parIsPsychotropicDrugs";
            this.parIsPsychotropicDrugs.Type = typeof(bool);
            this.parIsPsychotropicDrugs.ValueInfo = "False";
            this.parIsPsychotropicDrugs.Visible = false;
            // 
            // parIsFuncfoodsOrCosmetics
            // 
            this.parIsFuncfoodsOrCosmetics.Description = "parIsFuncfoodsOrCosmetics";
            this.parIsFuncfoodsOrCosmetics.Name = "parIsFuncfoodsOrCosmetics";
            this.parIsFuncfoodsOrCosmetics.Type = typeof(bool);
            this.parIsFuncfoodsOrCosmetics.ValueInfo = "False";
            this.parIsFuncfoodsOrCosmetics.Visible = false;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
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
            this.parPrescriptionMainRightHeader.Visible = false;
            // 
            // parPrescriptionSubRightHeader
            // 
            this.parPrescriptionSubRightHeader.Description = "parPrescriptionSubRightHeader";
            this.parPrescriptionSubRightHeader.Name = "parPrescriptionSubRightHeader";
            this.parPrescriptionSubRightHeader.Visible = false;
            // 
            // parIsAddictive
            // 
            this.parIsAddictive.Description = "parIsAddictive";
            this.parIsAddictive.Name = "parIsAddictive";
            this.parIsAddictive.Type = typeof(bool);
            this.parIsAddictive.ValueInfo = "False";
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
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parKBYTLink
            // 
            this.parKBYTLink.Name = "parKBYTLink";
            // 
            // XRptEPrescription_HT_SubReport_TV3
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 10, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parIssueID,
            this.parIsPsychotropicDrugs,
            this.parIsFuncfoodsOrCosmetics,
            this.parHospitalCode,
            this.parPrescriptionMainRightHeader,
            this.parPrescriptionSubRightHeader,
            this.parIsAddictive,
            this.parDepartmentOfHealth,
            this.parHospitalAddress,
            this.parHospitalName,
            this.parKBYTLink});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptEPrescription_V2_SubReport_TV_SubReport_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.Parameters.Parameter parIssueID;
        private DevExpress.XtraReports.Parameters.Parameter parIsPsychotropicDrugs;
        private DevExpress.XtraReports.Parameters.Parameter parIsFuncfoodsOrCosmetics;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalCode;
        private DevExpress.XtraReports.Parameters.Parameter parPrescriptionMainRightHeader;
        private DevExpress.XtraReports.Parameters.Parameter parPrescriptionSubRightHeader;
        private DevExpress.XtraReports.Parameters.Parameter parIsAddictive;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak2;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport3;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak3;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport4;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parKBYTLink;
    }
}
