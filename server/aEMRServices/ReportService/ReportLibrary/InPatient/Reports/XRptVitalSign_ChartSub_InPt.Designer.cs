namespace eHCMS.ReportLib.InPatient.Reports
{
    partial class XRptVitalSign_ChartSub_InPt
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
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.WorkTimeRule workTimeRule1 = new DevExpress.XtraCharts.WorkTimeRule();
            DevExpress.XtraCharts.TimeInterval timeInterval1 = new DevExpress.XtraCharts.TimeInterval();
            DevExpress.XtraCharts.SecondaryAxisY secondaryAxisY1 = new DevExpress.XtraCharts.SecondaryAxisY();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.LineSeriesView lineSeriesView1 = new DevExpress.XtraCharts.LineSeriesView();
            DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.LineSeriesView lineSeriesView2 = new DevExpress.XtraCharts.LineSeriesView();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrChart1 = new DevExpress.XtraReports.UI.XRChart();
            this.spXRptVitalSign_SubChart_InPtTableAdapter = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPtTableAdapters.spXRptVitalSign_SubChart_InPtTableAdapter();
            this.dsXRptVitalSign_InPt1 = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPt();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.numericChartRangeControlClient1 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter = new eHCMS.ReportLib.PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter();
            this.numericChartRangeControlClient2 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.numericChartRangeControlClient3 = new DevExpress.XtraEditors.NumericChartRangeControlClient();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.PageNumber = new DevExpress.XtraReports.Parameters.Parameter();
            this.spXRptVitalSign_InPtTableAdapter = new eHCMS.ReportLib.InPatient.DataSchema.dsXRptVitalSign_InPtTableAdapters.spXRptVitalSign_InPtTableAdapter();
            this.V_RegistrationType = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(secondaryAxisY1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRptVitalSign_InPt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrChart1});
            this.Detail.HeightF = 436.4584F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrChart1
            // 
            this.xrChart1.BorderColor = System.Drawing.Color.Black;
            this.xrChart1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrChart1.DataAdapter = this.spXRptVitalSign_SubChart_InPtTableAdapter;
            this.xrChart1.DataMember = "spXRptVitalSign_SubChart_InPt";
            this.xrChart1.DataSource = this.dsXRptVitalSign_InPt1;
            xyDiagram1.AxisX.Alignment = DevExpress.XtraCharts.AxisAlignment.Far;
            xyDiagram1.AxisX.AutoScaleBreaks.Enabled = true;
            xyDiagram1.AxisX.DateTimeScaleOptions.AggregateFunction = DevExpress.XtraCharts.AggregateFunction.Minimum;
            xyDiagram1.AxisX.DateTimeScaleOptions.AutoGrid = false;
            xyDiagram1.AxisX.DateTimeScaleOptions.GridSpacing = 12D;
            xyDiagram1.AxisX.DateTimeScaleOptions.ScaleMode = DevExpress.XtraCharts.ScaleMode.Automatic;
            workTimeRule1.WorkIntervals.AddRange(new DevExpress.XtraCharts.TimeInterval[] {
            timeInterval1});
            xyDiagram1.AxisX.DateTimeScaleOptions.WorkTimeRules.AddRange(new DevExpress.XtraCharts.WorkTimeRule[] {
            workTimeRule1});
            xyDiagram1.AxisX.GridLines.MinorVisible = true;
            xyDiagram1.AxisX.GridLines.Visible = true;
            xyDiagram1.AxisX.MinorCount = 1;
            xyDiagram1.AxisX.StickToEnd = true;
            xyDiagram1.AxisX.Tickmarks.MinorVisible = false;
            xyDiagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
            xyDiagram1.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.Alignment = DevExpress.XtraCharts.AxisAlignment.Far;
            xyDiagram1.AxisY.GridLines.MinorVisible = true;
            xyDiagram1.AxisY.Label.Visible = false;
            xyDiagram1.AxisY.MinorCount = 10;
            xyDiagram1.AxisY.NumericScaleOptions.AutoGrid = false;
            xyDiagram1.AxisY.Tickmarks.MinorLength = 1;
            xyDiagram1.AxisY.Tickmarks.MinorVisible = false;
            xyDiagram1.AxisY.Tickmarks.Visible = false;
            xyDiagram1.AxisY.Visibility = DevExpress.Utils.DefaultBoolean.True;
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisY.VisualRange.Auto = false;
            xyDiagram1.AxisY.VisualRange.AutoSideMargins = false;
            xyDiagram1.AxisY.VisualRange.MaxValueSerializable = "41.5";
            xyDiagram1.AxisY.VisualRange.MinValueSerializable = "35";
            xyDiagram1.AxisY.VisualRange.SideMarginsValue = 0D;
            xyDiagram1.AxisY.WholeRange.Auto = false;
            xyDiagram1.AxisY.WholeRange.AutoSideMargins = false;
            xyDiagram1.AxisY.WholeRange.MaxValueSerializable = "41.5";
            xyDiagram1.AxisY.WholeRange.MinValueSerializable = "35";
            xyDiagram1.AxisY.WholeRange.SideMarginsValue = 0D;
            xyDiagram1.DefaultPane.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            xyDiagram1.DefaultPane.EnableAxisXScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisXZooming = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYScrolling = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.DefaultPane.EnableAxisYZooming = DevExpress.Utils.DefaultBoolean.False;
            xyDiagram1.Margins.Bottom = 0;
            xyDiagram1.Margins.Left = 0;
            xyDiagram1.Margins.Right = 0;
            xyDiagram1.Margins.Top = 0;
            secondaryAxisY1.Alignment = DevExpress.XtraCharts.AxisAlignment.Near;
            secondaryAxisY1.AxisID = 0;
            secondaryAxisY1.GridLines.Color = System.Drawing.Color.Black;
            secondaryAxisY1.GridLines.MinorVisible = true;
            secondaryAxisY1.GridLines.Visible = true;
            secondaryAxisY1.Label.Visible = false;
            secondaryAxisY1.MinorCount = 10;
            secondaryAxisY1.Name = "Secondary AxisY 1";
            secondaryAxisY1.NumericScaleOptions.AutoGrid = false;
            secondaryAxisY1.NumericScaleOptions.CustomGridAlignment = 20D;
            secondaryAxisY1.NumericScaleOptions.GridAlignment = DevExpress.XtraCharts.NumericGridAlignment.Custom;
            secondaryAxisY1.Tickmarks.MinorLength = 1;
            secondaryAxisY1.Tickmarks.MinorVisible = false;
            secondaryAxisY1.Tickmarks.Visible = false;
            secondaryAxisY1.Visibility = DevExpress.Utils.DefaultBoolean.True;
            secondaryAxisY1.VisibleInPanesSerializable = "-1";
            secondaryAxisY1.VisualRange.Auto = false;
            secondaryAxisY1.VisualRange.MaxValueSerializable = "170";
            secondaryAxisY1.VisualRange.MinValueSerializable = "40";
            secondaryAxisY1.WholeRange.Auto = false;
            secondaryAxisY1.WholeRange.AutoSideMargins = false;
            secondaryAxisY1.WholeRange.MaxValueSerializable = "170";
            secondaryAxisY1.WholeRange.MinValueSerializable = "40";
            secondaryAxisY1.WholeRange.SideMarginsValue = 0D;
            xyDiagram1.SecondaryAxesY.AddRange(new DevExpress.XtraCharts.SecondaryAxisY[] {
            secondaryAxisY1});
            this.xrChart1.Diagram = xyDiagram1;
            this.xrChart1.Legend.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;
            this.xrChart1.Legend.Name = "Default Legend";
            this.xrChart1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            this.xrChart1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrChart1.Name = "xrChart1";
            this.xrChart1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 96F);
            series1.ArgumentDataMember = "Day";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            series1.Name = "Series 1";
            series1.ValueDataMembersSerializable = "Temperature";
            series1.View = lineSeriesView1;
            series2.ArgumentDataMember = "Day";
            series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            series2.Name = "Series 2";
            series2.ValueDataMembersSerializable = "Pulse";
            lineSeriesView2.AxisYName = "Secondary AxisY 1";
            series2.View = lineSeriesView2;
            this.xrChart1.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
            this.xrChart1.SizeF = new System.Drawing.SizeF(654.8588F, 436.4584F);
            this.xrChart1.StylePriority.UsePadding = false;
            // 
            // spXRptVitalSign_SubChart_InPtTableAdapter
            // 
            this.spXRptVitalSign_SubChart_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // dsXRptVitalSign_InPt1
            // 
            this.dsXRptVitalSign_InPt1.DataSetName = "dsXRptVitalSign_InPt";
            this.dsXRptVitalSign_InPt1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
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
            // PtRegistrationID
            // 
            this.PtRegistrationID.Name = "PtRegistrationID";
            this.PtRegistrationID.Type = typeof(long);
            this.PtRegistrationID.ValueInfo = "0";
            this.PtRegistrationID.Visible = false;
            // 
            // spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter
            // 
            this.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter.ClearBeforeFill = true;
            // 
            // ReportFooter
            // 
            this.ReportFooter.HeightF = 0F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // PageNumber
            // 
            this.PageNumber.Description = "PageNumber";
            this.PageNumber.Name = "PageNumber";
            this.PageNumber.Type = typeof(int);
            this.PageNumber.ValueInfo = "0";
            this.PageNumber.Visible = false;
            // 
            // spXRptVitalSign_InPtTableAdapter
            // 
            this.spXRptVitalSign_InPtTableAdapter.ClearBeforeFill = true;
            // 
            // V_RegistrationType
            // 
            this.V_RegistrationType.Description = "Parameter1";
            this.V_RegistrationType.Name = "V_RegistrationType";
            this.V_RegistrationType.Type = typeof(long);
            this.V_RegistrationType.ValueInfo = "0";
            // 
            // XRptVitalSign_ChartSub_InPt
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsXRptVitalSign_InPt1});
            this.DataMember = "spXRptVitalSign_ChartSub_InPt";
            this.DataSource = this.dsXRptVitalSign_InPt1;
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 450;
            this.PageWidth = 660;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PtRegistrationID,
            this.PageNumber,
            this.V_RegistrationType});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptVitalSign_ChartSub_InPt_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(secondaryAxisY1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(lineSeriesView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsXRptVitalSign_InPt1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChartRangeControlClient3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DataSchema.dsXRptVitalSign_InPt dsXRptVitalSign_InPt1;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient1;
        private PCLDepartment.DataSchema.dsXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapters.spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient2;
        private DevExpress.XtraEditors.NumericChartRangeControlClient numericChartRangeControlClient3;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRChart xrChart1;
        private DataSchema.dsXRptVitalSign_InPtTableAdapters.spXRptVitalSign_SubChart_InPtTableAdapter spXRptVitalSign_SubChart_InPtTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter PageNumber;
        public DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        private DataSchema.dsXRptVitalSign_InPtTableAdapters.spXRptVitalSign_InPtTableAdapter spXRptVitalSign_InPtTableAdapter;
        public DevExpress.XtraReports.Parameters.Parameter V_RegistrationType;
    }
}
