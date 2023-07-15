using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptDoctorInstruction_SubReport
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
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel37 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel36 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel35 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.dsRptMedicalInstruction1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstruction();
            this.H1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.H3 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.H2 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.spRptMedicalInstructionTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter();
            this.IntPtDiagDrInstructionID = new DevExpress.XtraReports.Parameters.Parameter();
            this.spRptGetIntravenousPlan_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptGetIntravenousPlan_InPtTableAdapter();
            this.cADose = new DevExpress.XtraReports.UI.CalculatedField();
            this.cMDose = new DevExpress.XtraReports.UI.CalculatedField();
            this.cEDose = new DevExpress.XtraReports.UI.CalculatedField();
            this.cNDose = new DevExpress.XtraReports.UI.CalculatedField();
            this.cQty = new DevExpress.XtraReports.UI.CalculatedField();
            this.sp_GetAllPCLItemsByInstructionIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.sp_GetAllPCLItemsByInstructionIDTableAdapter();
            this.cUseQty = new DevExpress.XtraReports.UI.CalculatedField();
            this.sp_GetAllRegistrationItemsByInstructionIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.sp_GetAllRegistrationItemsByInstructionIDTableAdapter();
            this.spRptGeneralOutPtMedicalFileTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter();
            this.spRptToDieuTriNgoaiTruTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter();
            this.PrescriptID = new DevExpress.XtraReports.Parameters.Parameter();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsRptMedicalInstruction1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel1});
            this.Detail.HeightF = 164.1394F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Detail_BeforePrint);
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel38,
            this.xrLabel37,
            this.xrLabel36,
            this.xrLabel35,
            this.xrLabel11,
            this.xrLabel8,
            this.xrLabel7,
            this.xrLabel3,
            this.xrLabel2});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(5F, 0F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(295F, 161F);
            // 
            // xrLabel38
            // 
            this.xrLabel38.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel38.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(0.0005086263F, 115F);
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(290F, 23F);
            this.xrLabel38.StylePriority.UseBorders = false;
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.StylePriority.UseTextAlignment = false;
            this.xrLabel38.Text = "Chẩn đoán:";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel37
            // 
            this.xrLabel37.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel37.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DiagnosisFinal]")});
            this.xrLabel37.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel37.LocationFloat = new DevExpress.Utils.PointFloat(0F, 138F);
            this.xrLabel37.Name = "xrLabel37";
            this.xrLabel37.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel37.SizeF = new System.Drawing.SizeF(285F, 23F);
            this.xrLabel37.StylePriority.UseBorders = false;
            this.xrLabel37.StylePriority.UseFont = false;
            this.xrLabel37.StylePriority.UseTextAlignment = false;
            this.xrLabel37.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel36
            // 
            this.xrLabel36.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel36.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel36.LocationFloat = new DevExpress.Utils.PointFloat(0.0005086263F, 68.99999F);
            this.xrLabel36.Name = "xrLabel36";
            this.xrLabel36.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel36.SizeF = new System.Drawing.SizeF(290F, 23F);
            this.xrLabel36.StylePriority.UseBorders = false;
            this.xrLabel36.StylePriority.UseFont = false;
            this.xrLabel36.StylePriority.UseTextAlignment = false;
            this.xrLabel36.Text = "Diễn biến bệnh:";
            this.xrLabel36.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel35
            // 
            this.xrLabel35.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel35.CanShrink = true;
            this.xrLabel35.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrientedTreatment]")});
            this.xrLabel35.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel35.LocationFloat = new DevExpress.Utils.PointFloat(0.0005086263F, 92.00001F);
            this.xrLabel35.Name = "xrLabel35";
            this.xrLabel35.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel35.SizeF = new System.Drawing.SizeF(284.9995F, 23F);
            this.xrLabel35.StylePriority.UseBorders = false;
            this.xrLabel35.StylePriority.UseFont = false;
            this.xrLabel35.StylePriority.UseTextAlignment = false;
            this.xrLabel35.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel11
            // 
            this.xrLabel11.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Huyết áp: {0}/{1}\', [BloodPressure], [LowerBloodPressure])  + \' mmH" +
                    "g\'")});
            this.xrLabel11.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(129.9998F, 23F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(175F, 23F);
            this.xrLabel11.StylePriority.UseBorders = false;
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Nhiệt độ: \' + [Temperature] + \' °C\'")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0.0005086263F, 46F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(129.9995F, 23F);
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel7
            // 
            this.xrLabel7.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Mạch: \' + [Pulse] + \' lần/phút\'")});
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(0.0005086263F, 23F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(133F, 23F);
            this.xrLabel7.StylePriority.UseBorders = false;
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'SpO2: \' + [SpO2] + \' %\'")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(129.9998F, 46F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(141.6666F, 23F);
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel2.StyleName = "H3";
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Sinh hiệu:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
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
            // dsRptMedicalInstruction1
            // 
            this.dsRptMedicalInstruction1.DataSetName = "dsRptMedicalInstruction";
            this.dsRptMedicalInstruction1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // H1
            // 
            this.H1.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Bold);
            this.H1.Name = "H1";
            // 
            // H3
            // 
            this.H3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.H3.Name = "H3";
            // 
            // H2
            // 
            this.H2.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
            this.H2.Name = "H2";
            // 
            // spRptMedicalInstructionTableAdapter
            // 
            this.spRptMedicalInstructionTableAdapter.ClearBeforeFill = true;
            // 
            // IntPtDiagDrInstructionID
            // 
            this.IntPtDiagDrInstructionID.Description = "IntPtDiagDrInstructionID";
            this.IntPtDiagDrInstructionID.Name = "IntPtDiagDrInstructionID";
            this.IntPtDiagDrInstructionID.Type = typeof(long);
            this.IntPtDiagDrInstructionID.ValueInfo = "0";
            this.IntPtDiagDrInstructionID.Visible = false;
            // 
            // spRptGetIntravenousPlan_InPtTableAdapter
            // 
            this.spRptGetIntravenousPlan_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // cADose
            // 
            this.cADose.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cADose.Expression = "Iif([ADose]  == 0,\'\',\'*Trưa \' + [ADoseStr] + \' \' + [UnitName])";
            this.cADose.Name = "cADose";
            // 
            // cMDose
            // 
            this.cMDose.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cMDose.Expression = "Iif([MDose]  == 0,\'\',\'*Sáng \' + [MDoseStr] + \' \' + [UnitName])";
            this.cMDose.Name = "cMDose";
            // 
            // cEDose
            // 
            this.cEDose.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cEDose.Expression = "Iif([EDose]  == 0,\'\',\'*Chiều \' + [EDoseStr] + \' \' + [UnitName])";
            this.cEDose.Name = "cEDose";
            // 
            // cNDose
            // 
            this.cNDose.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cNDose.Expression = "Iif([NDose]  == 0,\'\',\'*Tối \' + [NDoseStr] + \' \' + [UnitName])";
            this.cNDose.Name = "cNDose";
            // 
            // cQty
            // 
            this.cQty.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cQty.Expression = "Iif([PrescribedQty] == Round([PrescribedQty]), Round([PrescribedQty]) , Round([Pr" +
    "escribedQty])) + \' \' + [UnitName]";
            this.cQty.Name = "cQty";
            // 
            // sp_GetAllPCLItemsByInstructionIDTableAdapter
            // 
            this.sp_GetAllPCLItemsByInstructionIDTableAdapter.ClearBeforeFill = true;
            // 
            // cUseQty
            // 
            this.cUseQty.DataMember = "spRptGetIntravenousPlan_InPt";
            this.cUseQty.Expression = "Iif([PrescribedQty] == Round([PrescribedQty]), Round([PrescribedQty]) , Round([Pr" +
    "escribedQty], 3)) + \' \' + [UnitName]";
            this.cUseQty.Name = "cUseQty";
            // 
            // sp_GetAllRegistrationItemsByInstructionIDTableAdapter
            // 
            this.sp_GetAllRegistrationItemsByInstructionIDTableAdapter.ClearBeforeFill = true;
            // 
            // spRptGeneralOutPtMedicalFileTableAdapter
            // 
            this.spRptGeneralOutPtMedicalFileTableAdapter.ClearBeforeFill = true;
            // 
            // spRptToDieuTriNgoaiTruTableAdapter
            // 
            this.spRptToDieuTriNgoaiTruTableAdapter.ClearBeforeFill = true;
            // 
            // PrescriptID
            // 
            this.PrescriptID.Name = "PrescriptID";
            this.PrescriptID.Type = typeof(long);
            this.PrescriptID.ValueInfo = "0";
            // 
            // sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter
            // 
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.ClearBeforeFill = true;
            // 
            // spPrescriptions_RptViewByPrescriptID_InPtTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // spXRpt_PCLImagingResult_NewTableAdapter
            // 
            this.spXRpt_PCLImagingResult_NewTableAdapter.ClearBeforeFill = true;
            // 
            // XRptDoctorInstruction_SubReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.cMDose,
            this.cADose,
            this.cEDose,
            this.cNDose,
            this.cQty,
            this.cUseQty});
            this.DataAdapter = this.spRptMedicalInstructionTableAdapter;
            this.DataMember = "spRptMedicalInstruction";
            this.DataSource = this.dsRptMedicalInstruction1;
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.IntPtDiagDrInstructionID,
            this.PrescriptID});
            this.ReportPrintOptions.DetailCount = 1;
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.H1,
            this.H3,
            this.H2});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptDoctorInstruction_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsRptMedicalInstruction1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRControlStyle H1;
        private DevExpress.XtraReports.UI.XRControlStyle H3;
        private DevExpress.XtraReports.UI.XRControlStyle H2;
        private DataSchema.dsRptMedicalInstruction dsRptMedicalInstruction1;
        private DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter IntPtDiagDrInstructionID;
        private DataSchema.dsRptMedicalInstructionTableAdapters.spRptGetIntravenousPlan_InPtTableAdapter spRptGetIntravenousPlan_InPtTableAdapter;
        private DevExpress.XtraReports.UI.CalculatedField cADose;
        private DevExpress.XtraReports.UI.CalculatedField cMDose;
        private DevExpress.XtraReports.UI.CalculatedField cEDose;
        private DevExpress.XtraReports.UI.CalculatedField cNDose;
        private DevExpress.XtraReports.UI.CalculatedField cQty;
        private DataSchema.dsRptMedicalInstructionTableAdapters.sp_GetAllPCLItemsByInstructionIDTableAdapter sp_GetAllPCLItemsByInstructionIDTableAdapter;
        private DevExpress.XtraReports.UI.CalculatedField cUseQty;
        private DataSchema.dsRptMedicalInstructionTableAdapters.sp_GetAllRegistrationItemsByInstructionIDTableAdapter sp_GetAllRegistrationItemsByInstructionIDTableAdapter;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel38;
        private DevExpress.XtraReports.UI.XRLabel xrLabel37;
        private DevExpress.XtraReports.UI.XRLabel xrLabel36;
        private DevExpress.XtraReports.UI.XRLabel xrLabel35;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter spRptGeneralOutPtMedicalFileTableAdapter;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter spRptToDieuTriNgoaiTruTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter PrescriptID;
        private RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
    }
}
