namespace eHCMS.ReportLib.RptPatientRegistration
{
    partial class XRptPatientInfo_TV
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
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PatientName = new DevExpress.XtraReports.Parameters.Parameter();
            this.FileCodeNumber = new DevExpress.XtraReports.Parameters.Parameter();
            this.DOB = new DevExpress.XtraReports.Parameters.Parameter();
            this.Gender = new DevExpress.XtraReports.Parameters.Parameter();
            this.AdmissionDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.Age = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.PatientFullAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.parIsChild = new DevExpress.XtraReports.Parameters.Parameter();
            this.parWeight = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport2,
            this.xrSubreport1});
            this.Detail.HeightF = 90F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(202.425F, 0F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(202.425F, 90F);
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport2_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(202.425F, 90F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
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
            // DOB
            // 
            this.DOB.Name = "DOB";
            this.DOB.Visible = false;
            // 
            // Gender
            // 
            this.Gender.Name = "Gender";
            this.Gender.Visible = false;
            // 
            // AdmissionDate
            // 
            this.AdmissionDate.Name = "AdmissionDate";
            this.AdmissionDate.Type = typeof(System.DateTime);
            this.AdmissionDate.Visible = false;
            // 
            // Age
            // 
            this.Age.Name = "Age";
            this.Age.Visible = false;
            // 
            // PatientCode
            // 
            this.PatientCode.Name = "PatientCode";
            this.PatientCode.Visible = false;
            // 
            // PatientFullAddress
            // 
            this.PatientFullAddress.Description = "PatientFullAddress";
            this.PatientFullAddress.Name = "PatientFullAddress";
            this.PatientFullAddress.Visible = false;
            // 
            // parIsChild
            // 
            this.parIsChild.Description = "Trẻ sơ sinh";
            this.parIsChild.Name = "parIsChild";
            this.parIsChild.Type = typeof(bool);
            this.parIsChild.ValueInfo = "False";
            // 
            // parWeight
            // 
            this.parWeight.Description = "Cân nặng:";
            this.parWeight.Name = "parWeight";
            this.parWeight.ValueInfo = "0";
            // 
            // XRptPatientInfo_TV
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Margins = new System.Drawing.Printing.Margins(17, 41, 0, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.PatientName,
            this.FileCodeNumber,
            this.DOB,
            this.Gender,
            this.AdmissionDate,
            this.Age,
            this.PatientCode,
            this.PatientFullAddress,
            this.parIsChild,
            this.parWeight});
            this.RequestParameters = false;
            this.RollPaper = true;
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.Parameters.Parameter PatientName;
        private DevExpress.XtraReports.Parameters.Parameter FileCodeNumber;
        private DevExpress.XtraReports.Parameters.Parameter DOB;
        private DevExpress.XtraReports.Parameters.Parameter Gender;
        private DevExpress.XtraReports.Parameters.Parameter AdmissionDate;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter Age;
        private DevExpress.XtraReports.Parameters.Parameter PatientCode;
        private DevExpress.XtraReports.Parameters.Parameter PatientFullAddress;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.Parameters.Parameter parIsChild;
        private DevExpress.XtraReports.Parameters.Parameter parWeight;
    }
}
