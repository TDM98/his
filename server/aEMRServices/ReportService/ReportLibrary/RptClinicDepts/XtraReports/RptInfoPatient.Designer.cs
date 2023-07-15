namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    partial class RptInfoPatient
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
            this.xrSubreport6 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport5 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport4 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.DOB = new DevExpress.XtraReports.Parameters.Parameter();
            this.FileCodeNumber = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientName = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport6,
            this.xrSubreport5,
            this.xrSubreport4,
            this.xrSubreport3,
            this.xrSubreport2,
            this.xrSubreport1});
            this.Detail.HeightF = 211.4583F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport6
            // 
            this.xrSubreport6.LocationFloat = new DevExpress.Utils.PointFloat(24.58334F, 119.2083F);
            this.xrSubreport6.Name = "xrSubreport6";
            this.xrSubreport6.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport6.SizeF = new System.Drawing.SizeF(258.4915F, 47.91666F);
            this.xrSubreport6.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport6_BeforePrint);
            // 
            // xrSubreport5
            // 
            this.xrSubreport5.LocationFloat = new DevExpress.Utils.PointFloat(24.58334F, 71.29167F);
            this.xrSubreport5.Name = "xrSubreport5";
            this.xrSubreport5.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport5.SizeF = new System.Drawing.SizeF(258.4915F, 47.91666F);
            this.xrSubreport5.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport5_BeforePrint);
            // 
            // xrSubreport4
            // 
            this.xrSubreport4.LocationFloat = new DevExpress.Utils.PointFloat(411.1999F, 119.2083F);
            this.xrSubreport4.Name = "xrSubreport4";
            this.xrSubreport4.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport4.SizeF = new System.Drawing.SizeF(253.2834F, 47.91666F);
            this.xrSubreport4.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport4_BeforePrint);
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(411.1999F, 71.29167F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(253.2833F, 47.91666F);
            this.xrSubreport3.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport3_BeforePrint);
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(411.1997F, 23.37499F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(253.2834F, 47.91668F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport2_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(24.58334F, 23.37499F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient_Sub();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(258.4915F, 47.91669F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint_1);
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
            this.BottomMargin.HeightF = 6.25F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // PatientCode
            // 
            this.PatientCode.Name = "PatientCode";
            this.PatientCode.Visible = false;
            // 
            // DOB
            // 
            this.DOB.Name = "DOB";
            this.DOB.Visible = false;
            // 
            // FileCodeNumber
            // 
            this.FileCodeNumber.Name = "FileCodeNumber";
            this.FileCodeNumber.Visible = false;
            // 
            // PatientName
            // 
            this.PatientName.Name = "PatientName";
            this.PatientName.Visible = false;
            // 
            // RptInfoPatient
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 6);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PatientCode,
            this.DOB,
            this.FileCodeNumber,
            this.PatientName});
            this.Version = "14.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport6;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport5;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport4;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport3;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.Parameters.Parameter PatientCode;
        private DevExpress.XtraReports.Parameters.Parameter DOB;
        private DevExpress.XtraReports.Parameters.Parameter FileCodeNumber;
        private DevExpress.XtraReports.Parameters.Parameter PatientName;
    }
}
