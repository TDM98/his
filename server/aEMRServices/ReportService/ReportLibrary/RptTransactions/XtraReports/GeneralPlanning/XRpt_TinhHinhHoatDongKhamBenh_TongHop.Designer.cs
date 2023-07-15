using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    partial class XRpt_TinhHinhHoatDongKhamBenh_TongHop
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
            this.parToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.parStaffName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.parFromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel34 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel29 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.dsTKTinhHinhKCB1 = new eHCMS.ReportLib.RptTransactions.DataSchema.GeneralPlanning.dsTKTinhHinhKCB();
            this.sp_KHTH_TKTinhHinhKCBTableAdapter = new eHCMS.ReportLib.RptTransactions.DataSchema.GeneralPlanning.dsTKTinhHinhKCBTableAdapters.sp_KHTH_TKTinhHinhKCBTableAdapter();
            this.dsTKTinhHinhKCB2 = new eHCMS.ReportLib.RptTransactions.DataSchema.GeneralPlanning.dsTKTinhHinhKCB();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            ((System.ComponentModel.ISupportInitialize)(this.dsTKTinhHinhKCB1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTKTinhHinhKCB2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.xrSubreport1,
            this.xrSubreport2});
            this.Detail.HeightF = 56.74996F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // parToDate
            // 
            this.parToDate.Description = "parToDate";
            this.parToDate.Name = "parToDate";
            this.parToDate.Type = typeof(System.DateTime);
            this.parToDate.Visible = false;
            // 
            // parStaffName
            // 
            this.parStaffName.Description = "parStaffName";
            this.parStaffName.Name = "parStaffName";
            this.parStaffName.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            this.parHospitalName.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Visible = false;
            // 
            // parFromDate
            // 
            this.parFromDate.Description = "parFromDate";
            this.parFromDate.Name = "parFromDate";
            this.parFromDate.Type = typeof(System.DateTime);
            this.parFromDate.Visible = false;
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
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel34});
            this.PageFooter.HeightF = 23F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLabel34
            // 
            this.xrLabel34.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel34.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel34.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Italic);
            this.xrLabel34.LocationFloat = new DevExpress.Utils.PointFloat(908.4167F, 0F);
            this.xrLabel34.Name = "xrLabel34";
            this.xrLabel34.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel34.SizeF = new System.Drawing.SizeF(200.5832F, 23F);
            this.xrLabel34.StylePriority.UseBorders = false;
            this.xrLabel34.StylePriority.UseFont = false;
            this.xrLabel34.StylePriority.UseTextAlignment = false;
            this.xrLabel34.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrLabel34.TextFormatString = "{0:dd/MM/yyyy hh:mm tt}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel12,
            this.xrLabel2,
            this.xrLabel15});
            this.ReportHeader.HeightF = 160F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Thời gian từ: {0:dd/MM/yyyy} - {1:dd/MM/yyyy}\', [Parameters].[parFr" +
                    "omDate], [Parameters].[parToDate])")});
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Italic);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(10.00036F, 118.3333F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(1128.999F, 32.29166F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Bệnh nhân:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel12
            // 
            this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(10.0004F, 33.95834F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(393.75F, 23.95834F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "PHÒNG KẾ HOẠCH TỔNG HỢP";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parHospitalName]")});
            this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.0004F, 10F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(393.7496F, 23.95834F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(9.999943F, 81.87504F);
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(1128.999F, 36.45833F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "THỐNG KÊ TÌNH HÌNH HOẠT ĐỘNG KHÁM BỆNH";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 0F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel31
            // 
            this.xrLabel31.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel31.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parStaffName]")});
            this.xrLabel31.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(20.58327F, 91.95849F);
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(200.5833F, 22.99998F);
            this.xrLabel31.StylePriority.UseBorders = false;
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.StylePriority.UseTextAlignment = false;
            this.xrLabel31.Text = "xrLabel20";
            this.xrLabel31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel29
            // 
            this.xrLabel29.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel29.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.xrLabel29.LocationFloat = new DevExpress.Utils.PointFloat(20.58308F, 22.99995F);
            this.xrLabel29.Name = "xrLabel29";
            this.xrLabel29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel29.SizeF = new System.Drawing.SizeF(200.5834F, 23F);
            this.xrLabel29.StylePriority.UseBorders = false;
            this.xrLabel29.StylePriority.UseFont = false;
            this.xrLabel29.StylePriority.UseTextAlignment = false;
            this.xrLabel29.Text = "Người báo cáo";
            this.xrLabel29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel28
            // 
            this.xrLabel28.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel28.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(20.5829F, 0F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(200.5832F, 23F);
            this.xrLabel28.StylePriority.UseBorders = false;
            this.xrLabel28.StylePriority.UseTextAlignment = false;
            this.xrLabel28.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel28.TextFormatString = "Ngày {0:dd}  tháng {0:MM} năm {0:yyyy}";
            this.xrLabel28.TextTrimming = System.Drawing.StringTrimming.EllipsisWord;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel1});
            this.ReportFooter.HeightF = 129.7085F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel31,
            this.xrLabel29,
            this.xrLabel28});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(877.593F, 10F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(261.4065F, 114.9584F);
            // 
            // dsTKTinhHinhKCB1
            // 
            this.dsTKTinhHinhKCB1.DataSetName = "dsTKTinhHinhKCB";
            this.dsTKTinhHinhKCB1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // sp_KHTH_TKTinhHinhKCBTableAdapter
            // 
            this.sp_KHTH_TKTinhHinhKCBTableAdapter.ClearBeforeFill = true;
            // 
            // dsTKTinhHinhKCB2
            // 
            this.dsTKTinhHinhKCB2.DataSetName = "dsTKTinhHinhKCB";
            this.dsTKTinhHinhKCB2.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(9.999943F, 33.74996F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parToDate", this.parToDate));
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parStaffName", this.parStaffName));
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parHospitalName", this.parHospitalName));
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parDepartmentOfHealth", this.parDepartmentOfHealth));
            this.xrSubreport1.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parFromDate", this.parFromDate));
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptTransactions.XRpt_TinhHinhHoatDongKhamBenh_NoiTru();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(1128.999F, 23F);
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(10.00039F, 0F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parStaffName", this.parStaffName));
            this.xrSubreport2.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parHospitalName", this.parHospitalName));
            this.xrSubreport2.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parDepartmentOfHealth", this.parDepartmentOfHealth));
            this.xrSubreport2.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parFromDate", this.parFromDate));
            this.xrSubreport2.ParameterBindings.Add(new DevExpress.XtraReports.UI.ParameterBinding("parToDate", this.parToDate));
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptTransactions.XRpt_TinhHinhHoatDongKhamBenh();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(1128.999F, 23F);
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 21F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // XRpt_TinhHinhHoatDongKhamBenh_TongHop
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageFooter,
            this.ReportHeader,
            this.PageHeader,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.dsTKTinhHinhKCB1,
            this.dsTKTinhHinhKCB2});
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(10, 10, 20, 20);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parStaffName,
            this.parHospitalName,
            this.parDepartmentOfHealth,
            this.parFromDate,
            this.parToDate});
            this.RequestParameters = false;
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRpt_TinhHinhHoatDongKhamBenh_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.dsTKTinhHinhKCB1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTKTinhHinhKCB2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.Parameters.Parameter parStaffName;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel31;
        private DevExpress.XtraReports.UI.XRLabel xrLabel29;
        private DevExpress.XtraReports.UI.XRLabel xrLabel28;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel34;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.Parameters.Parameter parFromDate;
        private DevExpress.XtraReports.Parameters.Parameter parToDate;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DataSchema.GeneralPlanning.dsTKTinhHinhKCB dsTKTinhHinhKCB1;
        private DataSchema.GeneralPlanning.dsTKTinhHinhKCBTableAdapters.sp_KHTH_TKTinhHinhKCBTableAdapter sp_KHTH_TKTinhHinhKCBTableAdapter;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DataSchema.GeneralPlanning.dsTKTinhHinhKCB dsTKTinhHinhKCB2;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
    }
}
