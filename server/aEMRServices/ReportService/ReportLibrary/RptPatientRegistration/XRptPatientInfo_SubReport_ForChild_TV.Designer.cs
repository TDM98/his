namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptPatientInfo_SubReport_ForChild_TV
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
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.FileAndPatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.DOB_Gender = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.AdmissionDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientName = new DevExpress.XtraReports.Parameters.Parameter();
            this.FileCodeNumber = new DevExpress.XtraReports.Parameters.Parameter();
            this.Gender = new DevExpress.XtraReports.Parameters.Parameter();
            this.DOB = new DevExpress.XtraReports.Parameters.Parameter();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Age = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientFullAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.DOB_Age = new DevExpress.XtraReports.Parameters.Parameter();
            this.parWeight = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel10,
            this.xrLabel8,
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel2,
            this.xrLabel1});
            this.Detail.HeightF = 82.00002F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[parWeight]")});
            this.xrLabel6.Font = new System.Drawing.Font("Calibri", 8.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(125.2083F, 50.00002F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(69.7917F, 17.00001F);
            this.xrLabel6.StylePriority.UseBackColor = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "xrLabel4";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel6.WordWrap = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'(\' + [Parameters].[Gender] + \')\'")});
            this.xrLabel5.Font = new System.Drawing.Font("Calibri", 8.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(135F, 18F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(60F, 17F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "xrLabel3";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel5.WordWrap = false;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new System.Drawing.Font("Calibri", 9F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(99.99998F, 50.00002F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(25.20831F, 17F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "CN:";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Calibri", 9F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50.00002F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(22F, 17F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "ID:";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
            // 
            // xrLabel4
            // 
            this.xrLabel4.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[PatientCode]")});
            this.xrLabel4.Font = new System.Drawing.Font("Calibri", 8.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(21.99999F, 50.00002F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(78F, 17.00001F);
            this.xrLabel4.StylePriority.UseBackColor = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "xrLabel4";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrLabel4.WordWrap = false;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.DOB_Age]")});
            this.xrLabel3.Font = new System.Drawing.Font("Calibri", 8.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 18F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(135F, 17F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "xrLabel3";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel3.WordWrap = false;
            // 
            // xrLabel2
            // 
            this.xrLabel2.CanGrow = false;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[PatientFullAddress]")});
            this.xrLabel2.Font = new System.Drawing.Font("Calibri", 8.5F);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 35.00001F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(195F, 15F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "xrLabel2";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrLabel2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrLabel2_BeforePrint);
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.PatientName]")});
            this.xrLabel1.Font = new System.Drawing.Font("Calibri", 9.5F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(195F, 18F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel1";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // FileAndPatientCode
            // 
            this.FileAndPatientCode.Name = "FileAndPatientCode";
            this.FileAndPatientCode.Visible = false;
            // 
            // DOB_Gender
            // 
            this.DOB_Gender.Name = "DOB_Gender";
            this.DOB_Gender.Visible = false;
            // 
            // PatientCode
            // 
            this.PatientCode.Name = "PatientCode";
            this.PatientCode.Visible = false;
            // 
            // AdmissionDate
            // 
            this.AdmissionDate.Name = "AdmissionDate";
            this.AdmissionDate.Visible = false;
            // 
            // PatientName
            // 
            this.PatientName.Name = "PatientName";
            this.PatientName.Visible = false;
            // 
            // FileCodeNumber
            // 
            this.FileCodeNumber.Name = "FileCodeNumber";
            this.FileCodeNumber.Visible = false;
            // 
            // Gender
            // 
            this.Gender.Name = "Gender";
            this.Gender.Visible = false;
            // 
            // DOB
            // 
            this.DOB.Name = "DOB";
            this.DOB.Visible = false;
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
            this.BottomMargin.HeightF = 16.73936F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // Age
            // 
            this.Age.Name = "Age";
            this.Age.Visible = false;
            // 
            // PatientFullAddress
            // 
            this.PatientFullAddress.Description = "PatientFullAddress";
            this.PatientFullAddress.Name = "PatientFullAddress";
            this.PatientFullAddress.Visible = false;
            // 
            // DOB_Age
            // 
            this.DOB_Age.Description = "DOB_Age";
            this.DOB_Age.Name = "DOB_Age";
            this.DOB_Age.Visible = false;
            // 
            // parWeight
            // 
            this.parWeight.Description = "parWeight";
            this.parWeight.Name = "parWeight";
            this.parWeight.ValueInfo = "0";
            // 
            // XRptPatientInfo_SubReport_ForChild_TV
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Margins = new System.Drawing.Printing.Margins(5, 0, 0, 17);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PatientName,
            this.FileCodeNumber,
            this.DOB,
            this.Gender,
            this.AdmissionDate,
            this.DOB_Gender,
            this.Age,
            this.PatientCode,
            this.FileAndPatientCode,
            this.PatientFullAddress,
            this.DOB_Age,
            this.parWeight});
            this.Version = "17.2";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.XRptPatientInfo_SubReport_ForChild_TV_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        public DevExpress.XtraReports.Parameters.Parameter PatientName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.Parameters.Parameter Gender;
        public DevExpress.XtraReports.Parameters.Parameter DOB;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.Parameters.Parameter AdmissionDate;
        public DevExpress.XtraReports.Parameters.Parameter FileCodeNumber;
        private DevExpress.XtraReports.Parameters.Parameter DOB_Gender;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        public DevExpress.XtraReports.Parameters.Parameter Age;
        public DevExpress.XtraReports.Parameters.Parameter PatientCode;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.Parameters.Parameter FileAndPatientCode;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        public DevExpress.XtraReports.Parameters.Parameter PatientFullAddress;
        private DevExpress.XtraReports.Parameters.Parameter DOB_Age;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        public DevExpress.XtraReports.Parameters.Parameter parWeight;
    }
}
