namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptSoKhamSucKhoe
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
            DevExpress.XtraPrinting.Shape.ShapeRectangle shapeRectangle1 = new DevExpress.XtraPrinting.Shape.ShapeRectangle();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPageBreak6 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrPageBreak5 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrPageBreak4 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.MedicalExaminationExamAllResults = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageBreak3 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrPageBreak2 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.parPtRegDetailID = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.parPtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.IsShowExamAllResultDetails = new DevExpress.XtraReports.Parameters.Parameter();
            this.ExaminationResultVersion = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrShape2 = new DevExpress.XtraReports.UI.XRShape();
            this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel29 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrSubreport4 = new DevExpress.XtraReports.UI.XRSubreport();
            this.XRptPatientFinal = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak6,
            this.xrSubreport4,
            this.xrPageBreak5,
            this.xrPageBreak4,
            this.XRptPatientFinal,
            this.MedicalExaminationExamAllResults,
            this.xrPageBreak3,
            this.xrPageBreak2,
            this.xrPageBreak1,
            this.xrSubreport3,
            this.xrSubreport2,
            this.xrSubreport1});
            this.Detail.HeightF = 267.9861F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StylePriority.UsePadding = false;
            this.Detail.StylePriority.UseTextAlignment = false;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrPageBreak6
            // 
            this.xrPageBreak6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 178F);
            this.xrPageBreak6.Name = "xrPageBreak6";
            // 
            // xrPageBreak5
            // 
            this.xrPageBreak5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 178F);
            this.xrPageBreak5.Name = "xrPageBreak5";
            // 
            // xrPageBreak4
            // 
            this.xrPageBreak4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 211.3333F);
            this.xrPageBreak4.Name = "xrPageBreak4";
            // 
            // MedicalExaminationExamAllResults
            // 
            this.MedicalExaminationExamAllResults.LocationFloat = new DevExpress.Utils.PointFloat(25F, 180F);
            this.MedicalExaminationExamAllResults.Name = "MedicalExaminationExamAllResults";
            this.MedicalExaminationExamAllResults.SizeF = new System.Drawing.SizeF(727.0001F, 31.33331F);
            this.MedicalExaminationExamAllResults.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.MedicalExaminationExamAllResults_BeforePrint);
            // 
            // xrPageBreak3
            // 
            this.xrPageBreak3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 134.25F);
            this.xrPageBreak3.Name = "xrPageBreak3";
            // 
            // xrPageBreak2
            // 
            this.xrPageBreak2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 89.49998F);
            this.xrPageBreak2.Name = "xrPageBreak2";
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 43.75F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 30F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parPtRegDetailID
            // 
            this.parPtRegDetailID.Name = "parPtRegDetailID";
            this.parPtRegDetailID.Type = typeof(long);
            this.parPtRegDetailID.ValueInfo = "0";
            this.parPtRegDetailID.Visible = false;
            // 
            // spPrescriptions_RptHeaderByIssueID_InPtTableAdapter
            // 
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // parPtRegistrationID
            // 
            this.parPtRegistrationID.Description = "parPtRegistrationID";
            this.parPtRegistrationID.Name = "parPtRegistrationID";
            this.parPtRegistrationID.Type = typeof(long);
            this.parPtRegistrationID.ValueInfo = "0";
            // 
            // IsShowExamAllResultDetails
            // 
            this.IsShowExamAllResultDetails.Description = "IsShowExamAllResultDetails";
            this.IsShowExamAllResultDetails.Name = "IsShowExamAllResultDetails";
            this.IsShowExamAllResultDetails.Type = typeof(bool);
            this.IsShowExamAllResultDetails.ValueInfo = "False";
            // 
            // ExaminationResultVersion
            // 
            this.ExaminationResultVersion.Description = "ExaminationResultVersion";
            this.ExaminationResultVersion.Name = "ExaminationResultVersion";
            this.ExaminationResultVersion.Type = typeof(int);
            this.ExaminationResultVersion.ValueInfo = "0";
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrShape2,
            this.xrLabel26,
            this.xrLabel29,
            this.xrLabel2});
            this.PageHeader.HeightF = 140F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrShape2
            // 
            this.xrShape2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrShape2.BorderWidth = 2F;
            this.xrShape2.LineWidth = 2;
            this.xrShape2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrShape2.Name = "xrShape2";
            shapeRectangle1.Fillet = 20;
            this.xrShape2.Shape = shapeRectangle1;
            this.xrShape2.SizeF = new System.Drawing.SizeF(523.208F, 119.0833F);
            this.xrShape2.StylePriority.UseBorders = false;
            this.xrShape2.StylePriority.UseBorderWidth = false;
            // 
            // xrLabel26
            // 
            this.xrLabel26.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel26.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parDepartmentOfHealth]")});
            this.xrLabel26.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.xrLabel26.Multiline = true;
            this.xrLabel26.Name = "xrLabel26";
            this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel26.SizeF = new System.Drawing.SizeF(523.208F, 32F);
            this.xrLabel26.StylePriority.UseBorders = false;
            this.xrLabel26.StylePriority.UseFont = false;
            this.xrLabel26.StylePriority.UseTextAlignment = false;
            this.xrLabel26.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel29
            // 
            this.xrLabel29.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel29.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel29.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel29.LocationFloat = new DevExpress.Utils.PointFloat(0F, 41.99999F);
            this.xrLabel29.Multiline = true;
            this.xrLabel29.Name = "xrLabel29";
            this.xrLabel29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel29.SizeF = new System.Drawing.SizeF(523.208F, 32F);
            this.xrLabel29.StylePriority.UseBorders = false;
            this.xrLabel29.StylePriority.UseFont = false;
            this.xrLabel29.StylePriority.UseTextAlignment = false;
            this.xrLabel29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalAddress]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 74.00001F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(523.208F, 32F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "Parameter1";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // parHospitalAddress
            // 
            this.parHospitalAddress.Description = "Parameter1";
            this.parHospitalAddress.Name = "parHospitalAddress";
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "Parameter1";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // xrSubreport4
            // 
            this.xrSubreport4.LocationFloat = new DevExpress.Utils.PointFloat(25F, 136.25F);
            this.xrSubreport4.Name = "xrSubreport4";
            this.xrSubreport4.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptMedicalExaminationResults3_New();
            this.xrSubreport4.SizeF = new System.Drawing.SizeF(727.0001F, 43.75F);
            this.xrSubreport4.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport4_BeforePrint);
            // 
            // XRptPatientFinal
            // 
            this.XRptPatientFinal.LocationFloat = new DevExpress.Utils.PointFloat(25F, 215.3334F);
            this.XRptPatientFinal.Name = "XRptPatientFinal";
            this.XRptPatientFinal.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptPatientFinal();
            this.XRptPatientFinal.SizeF = new System.Drawing.SizeF(727.0002F, 29.16663F);
            this.XRptPatientFinal.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptPatientFinal_BeforePrint);
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(25F, 90.49998F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptMedicalExaminationResults2_New();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(727.0001F, 43.75F);
            this.xrSubreport3.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport3_BeforePrint);
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(25F, 45.74998F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptMedicalExaminationResults1_New();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(727F, 43.75F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport2_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(25F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptPatientDetail_New();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(727F, 43.75F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // XRptSoKhamSucKhoe
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader});
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(25, 27, 30, 30);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parPtRegDetailID,
            this.parPtRegistrationID,
            this.IsShowExamAllResultDetails,
            this.ExaminationResultVersion,
            this.parDepartmentOfHealth,
            this.parHospitalAddress,
            this.parHospitalName});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptSoKhamSucKhoe_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter parPtRegDetailID;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter spPrescriptions_RptHeaderByIssueID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak2;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.Parameters.Parameter parPtRegistrationID;
        private DevExpress.XtraReports.UI.XRSubreport MedicalExaminationExamAllResults;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak3;
        private DevExpress.XtraReports.Parameters.Parameter IsShowExamAllResultDetails;
        private DevExpress.XtraReports.UI.XRSubreport XRptPatientFinal;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak4;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport3;
        private DevExpress.XtraReports.Parameters.Parameter ExaminationResultVersion;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak6;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport4;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak5;
        private DevExpress.XtraReports.UI.XRShape xrShape2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel26;
        private DevExpress.XtraReports.UI.XRLabel xrLabel29;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalAddress;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
    }
}
