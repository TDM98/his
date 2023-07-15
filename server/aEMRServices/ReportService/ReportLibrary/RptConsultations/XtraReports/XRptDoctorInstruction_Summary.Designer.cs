using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations
{
    partial class XRptDoctorInstruction_Summary
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.dsRptMedicalInstruction1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstruction();
            this.spRptMedicalInstructionTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter();
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.IntPtDiagDrInstructionID = new DevExpress.XtraReports.Parameters.Parameter();
            this.FromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.ToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTable3 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.spRptToDieuTriNgoaiTruTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrCrossBandLine2 = new DevExpress.XtraReports.UI.XRCrossBandLine();
            this.spRptGeneralOutPtMedicalFileTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter();
            this.xrCrossBandLine1 = new DevExpress.XtraReports.UI.XRCrossBandLine();
            this.xrCrossBandLine3 = new DevExpress.XtraReports.UI.XRCrossBandLine();
            this.xrCrossBandLine4 = new DevExpress.XtraReports.UI.XRCrossBandLine();
            this.PrescriptID = new DevExpress.XtraReports.Parameters.Parameter();
            this.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter = new eHCMS.ReportLib.RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter();
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter();
            this.spXRpt_PCLImagingResult_NewTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter();
            this.DetailDrugSubReport = new DevExpress.XtraReports.UI.XRSubreport();
            this.DetailSubReport = new DevExpress.XtraReports.UI.XRSubreport();
            ((System.ComponentModel.ISupportInitialize)(this.dsRptMedicalInstruction1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.DetailDrugSubReport,
            this.DetailSubReport});
            this.Detail.HeightF = 32F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[InstructionDate]")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(3.041585F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(72F, 32F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel1.TextFormatString = "{0:dd/MM/yyyy HH:mm}";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 79F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 40F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // dsRptMedicalInstruction1
            // 
            this.dsRptMedicalInstruction1.DataSetName = "dsRptMedicalInstruction";
            this.dsRptMedicalInstruction1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spRptMedicalInstructionTableAdapter
            // 
            this.spRptMedicalInstructionTableAdapter.ClearBeforeFill = true;
            // 
            // PtRegistrationID
            // 
            this.PtRegistrationID.Description = "PtRegistrationID";
            this.PtRegistrationID.Name = "PtRegistrationID";
            this.PtRegistrationID.Type = typeof(long);
            this.PtRegistrationID.ValueInfo = "0";
            this.PtRegistrationID.Visible = false;
            // 
            // IntPtDiagDrInstructionID
            // 
            this.IntPtDiagDrInstructionID.Description = "IntPtDiagDrInstructionID";
            this.IntPtDiagDrInstructionID.Name = "IntPtDiagDrInstructionID";
            this.IntPtDiagDrInstructionID.Type = typeof(long);
            this.IntPtDiagDrInstructionID.ValueInfo = "0";
            this.IntPtDiagDrInstructionID.Visible = false;
            // 
            // FromDate
            // 
            this.FromDate.Description = "FromDate";
            this.FromDate.Name = "FromDate";
            this.FromDate.Type = typeof(System.DateTime);
            this.FromDate.Visible = false;
            // 
            // ToDate
            // 
            this.ToDate.Description = "ToDate";
            this.ToDate.Name = "ToDate";
            this.ToDate.Type = typeof(System.DateTime);
            this.ToDate.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel9,
            this.xrLabel14,
            this.xrTable3,
            this.xrLabel19,
            this.xrLabel18,
            this.xrLabel17,
            this.xrLabel8,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel10,
            this.xrLabel7,
            this.xrLabel6,
            this.xrLabel15,
            this.xrLabel25,
            this.xrLabel5,
            this.xrPageInfo1});
            this.PageHeader.HeightF = 184.3943F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PtRegistrationCode]")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(633.0699F, 35.29162F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(91.60004F, 20F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel9
            // 
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(559.07F, 35.29161F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(74F, 20F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "Số vào viện: ";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel14
            // 
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Chẩn đoán: \' + [DiagnosisMain]")});
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(449.4842F, 110.0111F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(271.9179F, 25.08334F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UsePadding = false;
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Page;
            this.xrLabel14.Summary = xrSummary1;
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTable3
            // 
            this.xrTable3.BackColor = System.Drawing.Color.Transparent;
            this.xrTable3.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable3.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrTable3.LocationFloat = new DevExpress.Utils.PointFloat(1.999919F, 148.5721F);
            this.xrTable3.Name = "xrTable3";
            this.xrTable3.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable3.SizeF = new System.Drawing.SizeF(720.44F, 35.8222F);
            this.xrTable3.StylePriority.UseFont = false;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell13,
            this.xrTableCell4,
            this.xrTableCell1});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 0.28943354119970416D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell13.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.StylePriority.UseBorders = false;
            this.xrTableCell13.StylePriority.UseFont = false;
            this.xrTableCell13.StylePriority.UseTextAlignment = false;
            this.xrTableCell13.Text = "NGÀY GIỜ";
            this.xrTableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell13.Weight = 1.468377725918804D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell4.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.StylePriority.UseBorders = false;
            this.xrTableCell4.StylePriority.UseFont = false;
            this.xrTableCell4.StylePriority.UseTextAlignment = false;
            this.xrTableCell4.Text = "DIỄN BIẾN BỆNH";
            this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell4.Weight = 5.7851879542214384D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCell1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseBorders = false;
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.Text = "Y LỆNH";
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell1.Weight = 7.2296438734902386D;
            // 
            // xrLabel19
            // 
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Giường: \' + [BedNumber]")});
            this.xrLabel19.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(361.7732F, 110.0111F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(87.71103F, 25.08334F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UsePadding = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel18
            // 
            this.xrLabel18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Phòng: \' + [LocationName]")});
            this.xrLabel18.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(183.4691F, 110.0111F);
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(178.3041F, 25.08332F);
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.StylePriority.UsePadding = false;
            this.xrLabel18.StylePriority.UseTextAlignment = false;
            this.xrLabel18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel17
            // 
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Khoa: \' + [DeptName]")});
            this.xrLabel17.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(8.95872F, 110.0111F);
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(174.5104F, 25.08334F);
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.StylePriority.UsePadding = false;
            this.xrLabel17.StylePriority.UseTextAlignment = false;
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Giới tính: \' + [Gender]")});
            this.xrLabel8.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(449.4842F, 84.92778F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(135.3713F, 25.08331F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UsePadding = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Age]")});
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(395.7732F, 84.92775F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(47.94107F, 25.08333F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PatientName]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(121.9587F, 84.92776F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(215F, 25.08F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UsePadding = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(361.7732F, 84.92775F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(34F, 25.08F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.StylePriority.UsePadding = false;
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "Tuổi:";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(8.958721F, 84.92775F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(113F, 25.08F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UsePadding = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "Họ tên người bệnh:";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(559.0729F, 2.291616F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(158.93F, 33F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "MS: P36/ A4/KHTH\r\nMS: 40/BV - 01";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(326.2141F, 1.422199F);
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(160.3078F, 29.91668F);
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "TỜ ĐIỀU TRỊ";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel25
            // 
            this.xrLabel25.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Upper([Parameters].[parDepartmentOfHealth])")});
            this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 1.422199F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(262.5F, 23.95834F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.StylePriority.UseTextAlignment = false;
            this.xrLabel25.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(10.0004F, 25.38052F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(262.4996F, 23.95833F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(326.2141F, 31.33888F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(160.3078F, 17.99997F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo1.TextFormatString = "Số: {0}";
            // 
            // spRptToDieuTriNgoaiTruTableAdapter
            // 
            this.spRptToDieuTriNgoaiTruTableAdapter.ClearBeforeFill = true;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine1});
            this.PageFooter.HeightF = 23F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.999919F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(720.44F, 2F);
            // 
            // xrCrossBandLine2
            // 
            this.xrCrossBandLine2.EndBand = this.PageFooter;
            this.xrCrossBandLine2.EndPointFloat = new DevExpress.Utils.PointFloat(1.999919F, 1.54171F);
            this.xrCrossBandLine2.LocationFloat = new DevExpress.Utils.PointFloat(1.999919F, 0F);
            this.xrCrossBandLine2.Name = "xrCrossBandLine2";
            this.xrCrossBandLine2.StartBand = this.Detail;
            this.xrCrossBandLine2.StartPointFloat = new DevExpress.Utils.PointFloat(1.999919F, 0F);
            this.xrCrossBandLine2.WidthF = 1.041667F;
            // 
            // spRptGeneralOutPtMedicalFileTableAdapter
            // 
            this.spRptGeneralOutPtMedicalFileTableAdapter.ClearBeforeFill = true;
            // 
            // xrCrossBandLine1
            // 
            this.xrCrossBandLine1.EndBand = this.PageFooter;
            this.xrCrossBandLine1.EndPointFloat = new DevExpress.Utils.PointFloat(361.7732F, 1.54171F);
            this.xrCrossBandLine1.LocationFloat = new DevExpress.Utils.PointFloat(361.7732F, 0F);
            this.xrCrossBandLine1.Name = "xrCrossBandLine1";
            this.xrCrossBandLine1.StartBand = this.Detail;
            this.xrCrossBandLine1.StartPointFloat = new DevExpress.Utils.PointFloat(361.7732F, 0F);
            this.xrCrossBandLine1.WidthF = 1.041667F;
            // 
            // xrCrossBandLine3
            // 
            this.xrCrossBandLine3.EndBand = this.PageFooter;
            this.xrCrossBandLine3.EndPointFloat = new DevExpress.Utils.PointFloat(75.18129F, 1.54171F);
            this.xrCrossBandLine3.LocationFloat = new DevExpress.Utils.PointFloat(75.18129F, 0F);
            this.xrCrossBandLine3.Name = "xrCrossBandLine3";
            this.xrCrossBandLine3.StartBand = this.Detail;
            this.xrCrossBandLine3.StartPointFloat = new DevExpress.Utils.PointFloat(75.18129F, 0F);
            this.xrCrossBandLine3.WidthF = 1.041667F;
            // 
            // xrCrossBandLine4
            // 
            this.xrCrossBandLine4.EndBand = this.PageFooter;
            this.xrCrossBandLine4.EndPointFloat = new DevExpress.Utils.PointFloat(721.3982F, 0F);
            this.xrCrossBandLine4.LocationFloat = new DevExpress.Utils.PointFloat(721.3982F, 0F);
            this.xrCrossBandLine4.Name = "xrCrossBandLine4";
            this.xrCrossBandLine4.StartBand = this.Detail;
            this.xrCrossBandLine4.StartPointFloat = new DevExpress.Utils.PointFloat(721.3982F, 0F);
            this.xrCrossBandLine4.WidthF = 1.041748F;
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
            // DetailDrugSubReport
            // 
            this.DetailDrugSubReport.LocationFloat = new DevExpress.Utils.PointFloat(362.8146F, 0F);
            this.DetailDrugSubReport.Name = "DetailDrugSubReport";
            this.DetailDrugSubReport.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptDoctorInstructionDetail_SubReport();
            this.DetailDrugSubReport.SizeF = new System.Drawing.SizeF(358.5835F, 32F);
            this.DetailDrugSubReport.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.DetailDrugSubReport_BeforePrint);
            // 
            // DetailSubReport
            // 
            this.DetailSubReport.LocationFloat = new DevExpress.Utils.PointFloat(76.22299F, 0F);
            this.DetailSubReport.Name = "DetailSubReport";
            this.DetailSubReport.ReportSource = new eHCMS.ReportLib.RptConsultations.XRptDoctorInstruction_SubReport();
            this.DetailSubReport.SizeF = new System.Drawing.SizeF(285.5502F, 32F);
            this.DetailSubReport.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.DetailSubReport_BeforePrint);
            // 
            // XRptDoctorInstruction_Summary
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsRptMedicalInstruction1});
            this.CrossBandControls.AddRange(new DevExpress.XtraReports.UI.XRCrossBandControl[] {
            this.xrCrossBandLine2,
            this.xrCrossBandLine1,
            this.xrCrossBandLine3,
            this.xrCrossBandLine4});
            this.DataAdapter = this.spRptMedicalInstructionTableAdapter;
            this.DataMember = "spRptMedicalInstruction";
            this.DataSource = this.dsRptMedicalInstruction1;
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(79, 20, 79, 40);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegistrationID,
            this.IntPtDiagDrInstructionID,
            this.FromDate,
            this.ToDate,
            this.parHospitalName,
            this.parDepartmentOfHealth,
            this.PrescriptID});
            this.RequestParameters = false;
            this.SnapGridSize = 9.84252F;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Report_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsRptMedicalInstruction1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private RptConsultations.DataSchema.dsRptMedicalInstruction dsRptMedicalInstruction1;
        private RptConsultations.DataSchema.dsRptMedicalInstructionTableAdapters.spRptMedicalInstructionTableAdapter spRptMedicalInstructionTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        private DevExpress.XtraReports.Parameters.Parameter IntPtDiagDrInstructionID;
        private DevExpress.XtraReports.Parameters.Parameter FromDate;
        private DevExpress.XtraReports.Parameters.Parameter ToDate;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable xrTable3;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DataSchema.dsRptToDieuTriNgoaiTruTableAdapters.spRptToDieuTriNgoaiTruTableAdapter spRptToDieuTriNgoaiTruTableAdapter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRCrossBandLine xrCrossBandLine2;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRSubreport DetailDrugSubReport;
        private DevExpress.XtraReports.UI.XRSubreport DetailSubReport;
        private DataSchema.dsGeneralOutPtMedicalFileTableAdapters.spRptGeneralOutPtMedicalFileTableAdapter spRptGeneralOutPtMedicalFileTableAdapter;
        private DevExpress.XtraReports.UI.XRCrossBandLine xrCrossBandLine1;
        private DevExpress.XtraReports.UI.XRCrossBandLine xrCrossBandLine3;
        private DevExpress.XtraReports.UI.XRCrossBandLine xrCrossBandLine4;
        private DevExpress.XtraReports.Parameters.Parameter PrescriptID;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private RptPatientRegistration.DataSchema.OutPatientReceiptTableAdapters.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter;
        private DataSchema.dsPrescriptionNew_InPtTableAdapters.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter spPrescriptions_RptViewByPrescriptID_InPtTableAdapter;
        private PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_NewTableAdapters.spXRpt_PCLImagingResult_NewTableAdapter spXRpt_PCLImagingResult_NewTableAdapter;
    }
}