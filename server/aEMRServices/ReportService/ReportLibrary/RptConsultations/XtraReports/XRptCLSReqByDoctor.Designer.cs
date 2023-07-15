namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    partial class XRptCLSReqByDoctor
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
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary3 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.calSignDate = new DevExpress.XtraReports.UI.CalculatedField();
            this.HIValidDateFrom = new DevExpress.XtraReports.UI.CalculatedField();
            this.HIValidDateTo = new DevExpress.XtraReports.UI.CalculatedField();
            this.calDOB = new DevExpress.XtraReports.UI.CalculatedField();
            this.calFromDate = new DevExpress.XtraReports.UI.CalculatedField();
            this.calToDate = new DevExpress.XtraReports.UI.CalculatedField();
            this.calTuNgay = new DevExpress.XtraReports.UI.CalculatedField();
            this.calDenNgay = new DevExpress.XtraReports.UI.CalculatedField();
            this.calDiagnosisTreatment_Final = new DevExpress.XtraReports.UI.CalculatedField();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.dsCLSReqByDoctor1 = new eHCMS.ReportLib.RptConsultations.DataSchema.dsCLSReqByDoctor();
            this.spRpt_CLSReqByDoctorTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsCLSReqByDoctorTableAdapters.spRpt_CLSReqByDoctorTableAdapter();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrTable3 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.FromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.ToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.Show = new DevExpress.XtraReports.Parameters.Parameter();
            this.V_PatientFindBy = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCLSReqByDoctor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable1
            // 
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(767.0001F, 25F);
            this.xrTable1.StylePriority.UseBorders = false;
            this.xrTable1.StylePriority.UseTextAlignment = false;
            this.xrTable1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell3,
            this.xrTableCell9,
            this.xrTableCell10});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumRecordNumber([MaDichVu])")});
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.xrTableCell1.Summary = xrSummary1;
            this.xrTableCell1.Text = "STT";
            this.xrTableCell1.Weight = 0.22588010188653557D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MaDichVu] + \' - \' + [TenDichVu]")});
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell3.StylePriority.UseTextAlignment = false;
            this.xrTableCell3.Text = "Cận lâm sàng";
            this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell3.Weight = 3.1050627508250486D;
            // 
            // xrTableCell9
            // 
            this.xrTableCell9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalDV]")});
            this.xrTableCell9.Name = "xrTableCell9";
            this.xrTableCell9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell9.StylePriority.UsePadding = false;
            this.xrTableCell9.Text = "Số lượng DV";
            this.xrTableCell9.TextFormatString = "{0:#,#}";
            this.xrTableCell9.Weight = 0.42959642472447451D;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalBHYT]")});
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell10.StylePriority.UsePadding = false;
            this.xrTableCell10.Text = "Số lượng BHYT";
            this.xrTableCell10.TextFormatString = "{0:#,#}";
            this.xrTableCell10.Weight = 0.39564293335438394D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 20F;
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
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1});
            this.ReportHeader.HeightF = 126.2917F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.StylePriority.UseFont = false;
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[Show]")});
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(181.875F, 92.54169F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(516.5833F, 28.20835F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "Từ………..đến……";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(181.875F, 56.00001F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(516.5833F, 36.54168F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "BÁO CÁO CẬN LÂM SÀNG THEO TÊN BÁC SĨ";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parDepartmentOfHealth]")});
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 10.00001F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(401.0417F, 23F);
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 33.00001F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(401.0417F, 23F);
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // calSignDate
            // 
            this.calSignDate.DataMember = "spGetTransferForm";
            this.calSignDate.Expression = "\'Ngày \'  +  GetDay([RecCreatedDate]) + \' tháng \' + GetMonth([RecCreatedDate]) + \'" +
    " năm \' + GetYear([RecCreatedDate])";
            this.calSignDate.Name = "calSignDate";
            // 
            // HIValidDateFrom
            // 
            this.HIValidDateFrom.DataMember = "spGetTransferForm";
            this.HIValidDateFrom.Expression = "Iif(IsNull([ValidDateFrom]), \'...\' , GetDay([ValidDateFrom])  +  \'/\' + GetMonth([" +
    "ValidDateFrom])  +  \'/\' +  GetYear([ValidDateFrom]))";
            this.HIValidDateFrom.Name = "HIValidDateFrom";
            // 
            // HIValidDateTo
            // 
            this.HIValidDateTo.DataMember = "spGetTransferForm";
            this.HIValidDateTo.Expression = "Iif(IsNull([ValidDateTo]), \'...\' , GetDay([ValidDateTo]) +  \'/\' + GetMonth([Valid" +
    "DateTo]) + \'/\' + GetYear([ValidDateTo]))";
            this.HIValidDateTo.Name = "HIValidDateTo";
            // 
            // calDOB
            // 
            this.calDOB.DataMember = "spGetTransferForm";
            this.calDOB.Expression = "Iif(IsNull([DOB]), \'\' , GetYear([DOB]))";
            this.calDOB.Name = "calDOB";
            // 
            // calFromDate
            // 
            this.calFromDate.DataMember = "spGetTransferForm";
            this.calFromDate.Expression = "Iif(IsNull([FromDate]), \'...\' , GetDay([FromDate]) +  \'/\' + GetMonth([FromDate]) " +
    "+ \'/\' + GetYear([FromDate]))";
            this.calFromDate.Name = "calFromDate";
            // 
            // calToDate
            // 
            this.calToDate.DataMember = "spGetTransferForm";
            this.calToDate.Expression = "Iif(IsNull([ToDate]), \'...\' , GetDay([ToDate]) +  \'/\' + GetMonth([ToDate]) + \'/\' " +
    "+ GetYear([ToDate]))";
            this.calToDate.Name = "calToDate";
            // 
            // calTuNgay
            // 
            this.calTuNgay.DataMember = "spGetTransferForm";
            this.calTuNgay.Expression = "Iif(IsNull([TuNgay]), \'...\' , GetDay([TuNgay]) +  \'/\' + GetMonth([TuNgay]) + \'/\' " +
    "+ GetYear([TuNgay]))";
            this.calTuNgay.Name = "calTuNgay";
            // 
            // calDenNgay
            // 
            this.calDenNgay.DataMember = "spGetTransferForm";
            this.calDenNgay.Expression = "Iif(IsNull([DenNgay]), \'...\' , GetDay([DenNgay]) +  \'/\' + GetMonth([DenNgay]) + \'" +
    "/\' + GetYear([DenNgay]))";
            this.calDenNgay.Name = "calDenNgay";
            // 
            // calDiagnosisTreatment_Final
            // 
            this.calDiagnosisTreatment_Final.DataMember = "spGetTransferForm";
            this.calDiagnosisTreatment_Final.Expression = "[DiagnosisTreatment_Final]  +  Iif(IsNullOrEmpty([ICD10Final]), \'\' , \' (\' + Trim(" +
    "[ICD10Final]) + \')\')";
            this.calDiagnosisTreatment_Final.Name = "calDiagnosisTreatment_Final";
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
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.PageHeader.HeightF = 52.08333F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrTable2
            // 
            this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable2.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(767F, 52.08333F);
            this.xrTable2.StylePriority.UseBorders = false;
            this.xrTable2.StylePriority.UseFont = false;
            this.xrTable2.StylePriority.UseTextAlignment = false;
            this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell4,
            this.xrTableCell6,
            this.xrTableCell7,
            this.xrTableCell8});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell4.Text = "STT";
            this.xrTableCell4.Weight = 0.22588010188653557D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell6.Text = "Cận lâm sàng";
            this.xrTableCell6.Weight = 3.1050627508250486D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell7.StylePriority.UsePadding = false;
            this.xrTableCell7.Text = "Số lượng DV";
            this.xrTableCell7.Weight = 0.42959642472447451D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell8.StylePriority.UsePadding = false;
            this.xrTableCell8.Text = "Số lượng BHYT";
            this.xrTableCell8.Weight = 0.39564293335438394D;
            // 
            // dsCLSReqByDoctor1
            // 
            this.dsCLSReqByDoctor1.DataSetName = "dsCLSReqByDoctor";
            this.dsCLSReqByDoctor1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // spRpt_CLSReqByDoctorTableAdapter
            // 
            this.spRpt_CLSReqByDoctorTableAdapter.ClearBeforeFill = true;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable3});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("BSChiDinhID", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.HeightF = 25F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // xrTable3
            // 
            this.xrTable3.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable3.LocationFloat = new DevExpress.Utils.PointFloat(9.99999F, 0F);
            this.xrTable3.Name = "xrTable3";
            this.xrTable3.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow3});
            this.xrTable3.SizeF = new System.Drawing.SizeF(767F, 25F);
            this.xrTable3.StylePriority.UseBorders = false;
            this.xrTable3.StylePriority.UseTextAlignment = false;
            this.xrTable3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell12,
            this.xrTableCell14,
            this.xrTableCell15});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell12
            // 
            this.xrTableCell12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BSChiDinh]")});
            this.xrTableCell12.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.xrTableCell12.Name = "xrTableCell12";
            this.xrTableCell12.Padding = new DevExpress.XtraPrinting.PaddingInfo(15, 2, 0, 0, 100F);
            this.xrTableCell12.StylePriority.UseFont = false;
            this.xrTableCell12.StylePriority.UsePadding = false;
            this.xrTableCell12.StylePriority.UseTextAlignment = false;
            this.xrTableCell12.Text = "Tên bác sĩ";
            this.xrTableCell12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCell12.Weight = 1.5186838287758393D;
            // 
            // xrTableCell14
            // 
            this.xrTableCell14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([TotalDV])")});
            this.xrTableCell14.ForeColor = System.Drawing.Color.DarkRed;
            this.xrTableCell14.Name = "xrTableCell14";
            this.xrTableCell14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell14.StylePriority.UseForeColor = false;
            this.xrTableCell14.StylePriority.UsePadding = false;
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.xrTableCell14.Summary = xrSummary2;
            this.xrTableCell14.Text = "Số lượng DV";
            this.xrTableCell14.TextFormatString = "{0:#,#}";
            this.xrTableCell14.Weight = 0.19586680848252885D;
            // 
            // xrTableCell15
            // 
            this.xrTableCell15.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([TotalBHYT])")});
            this.xrTableCell15.ForeColor = System.Drawing.Color.DarkRed;
            this.xrTableCell15.Name = "xrTableCell15";
            this.xrTableCell15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTableCell15.StylePriority.UseForeColor = false;
            this.xrTableCell15.StylePriority.UsePadding = false;
            xrSummary3.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.xrTableCell15.Summary = xrSummary3;
            this.xrTableCell15.Text = "Số lượng BHYT";
            this.xrTableCell15.TextFormatString = "{0:#,#}";
            this.xrTableCell15.Weight = 0.18038629097845615D;
            // 
            // FromDate
            // 
            this.FromDate.Description = "FromDate";
            this.FromDate.Name = "FromDate";
            this.FromDate.Type = typeof(System.DateTime);
            // 
            // ToDate
            // 
            this.ToDate.Description = "ToDate";
            this.ToDate.Name = "ToDate";
            this.ToDate.Type = typeof(System.DateTime);
            // 
            // Show
            // 
            this.Show.Description = "Show";
            this.Show.Name = "Show";
            // 
            // V_PatientFindBy
            // 
            this.V_PatientFindBy.Description = "V_PatientFindBy";
            this.V_PatientFindBy.Name = "V_PatientFindBy";
            this.V_PatientFindBy.Type = typeof(long);
            this.V_PatientFindBy.ValueInfo = "0";
            // 
            // XRptCLSReqByDoctor
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.ReportHeader,
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.GroupHeader1});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calSignDate,
            this.HIValidDateFrom,
            this.HIValidDateTo,
            this.calDOB,
            this.calFromDate,
            this.calToDate,
            this.calTuNgay,
            this.calDenNgay,
            this.calDiagnosisTreatment_Final});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsCLSReqByDoctor1});
            this.DataAdapter = this.spRpt_CLSReqByDoctorTableAdapter;
            this.DataMember = "spRpt_CLSReqByDoctor";
            this.DataSource = this.dsCLSReqByDoctor1;
            this.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parHospitalName,
            this.parDepartmentOfHealth,
            this.FromDate,
            this.ToDate,
            this.Show,
            this.V_PatientFindBy});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptCLSReqByDoctor_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCLSReqByDoctor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.CalculatedField calSignDate;
        private DevExpress.XtraReports.UI.CalculatedField HIValidDateFrom;
        private DevExpress.XtraReports.UI.CalculatedField HIValidDateTo;
        private DevExpress.XtraReports.UI.CalculatedField calDOB;
        private DevExpress.XtraReports.UI.CalculatedField calFromDate;
        private DevExpress.XtraReports.UI.CalculatedField calToDate;
        private DevExpress.XtraReports.UI.CalculatedField calTuNgay;
        private DevExpress.XtraReports.UI.CalculatedField calDenNgay;
        private DevExpress.XtraReports.UI.CalculatedField calDiagnosisTreatment_Final;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable xrTable2;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
        private DataSchema.dsCLSReqByDoctor dsCLSReqByDoctor1;
        private DataSchema.dsCLSReqByDoctorTableAdapters.spRpt_CLSReqByDoctorTableAdapter spRpt_CLSReqByDoctorTableAdapter;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable xrTable3;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow3;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell14;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell15;
        private DevExpress.XtraReports.Parameters.Parameter FromDate;
        private DevExpress.XtraReports.Parameters.Parameter ToDate;
        private DevExpress.XtraReports.Parameters.Parameter Show;
        private DevExpress.XtraReports.Parameters.Parameter V_PatientFindBy;
    }
}
