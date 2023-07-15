namespace eHCMS.ReportLib.RptTransactions
{
    partial class XRpt_Temp12_KhoaPhong
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
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrSubreport1 = new DevExpress.XtraReports.UI.XRSubreport();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.FromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.TransactionID = new DevExpress.XtraReports.Parameters.Parameter();
            this.PtRegistrationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.ViewByDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.StaffName = new DevExpress.XtraReports.Parameters.Parameter();
            this.DeptID = new DevExpress.XtraReports.Parameters.Parameter();
            this.DeptName = new DevExpress.XtraReports.Parameters.Parameter();
            this.RegistrationType = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalName = new DevExpress.XtraReports.Parameters.Parameter();
            this.parHospitalAdress = new DevExpress.XtraReports.Parameters.Parameter();
            this.parDepartmentOfHealth = new DevExpress.XtraReports.Parameters.Parameter();
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter = new eHCMS.ReportLib.RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter();
            this.IsKHTHView = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1,
            this.xrSubreport2,
            this.xrSubreport1});
            this.Detail.HeightF = 52.50004F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 25F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 29.50004F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new eHCMS.ReportLib.RptTransactions.XRpt_Temp12();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(423.5833F, 23F);
            this.xrSubreport2.Visible = false;
            this.xrSubreport2.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport2_BeforePrint);
            // 
            // xrSubreport1
            // 
            this.xrSubreport1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport1.Name = "xrSubreport1";
            this.xrSubreport1.ReportSource = new eHCMS.ReportLib.RptTransactions.XRpt_Temp12_6556_KhoaPhong();
            this.xrSubreport1.SizeF = new System.Drawing.SizeF(423.5833F, 23F);
            this.xrSubreport1.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrSubreport1_BeforePrint);
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 16F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 25F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ToDate
            // 
            this.ToDate.Name = "ToDate";
            this.ToDate.Type = typeof(System.DateTime);
            this.ToDate.Visible = false;
            // 
            // FromDate
            // 
            this.FromDate.Name = "FromDate";
            this.FromDate.Type = typeof(System.DateTime);
            this.FromDate.Visible = false;
            // 
            // TransactionID
            // 
            this.TransactionID.Description = "TransactionID";
            this.TransactionID.Name = "TransactionID";
            this.TransactionID.Type = typeof(int);
            this.TransactionID.ValueInfo = "0";
            this.TransactionID.Visible = false;
            // 
            // PtRegistrationID
            // 
            this.PtRegistrationID.Description = "PtRegistrationID";
            this.PtRegistrationID.Name = "PtRegistrationID";
            this.PtRegistrationID.Type = typeof(int);
            this.PtRegistrationID.ValueInfo = "0";
            this.PtRegistrationID.Visible = false;
            // 
            // ViewByDate
            // 
            this.ViewByDate.Description = "ViewByDate";
            this.ViewByDate.Name = "ViewByDate";
            this.ViewByDate.Type = typeof(bool);
            this.ViewByDate.ValueInfo = "False";
            this.ViewByDate.Visible = false;
            // 
            // StaffName
            // 
            this.StaffName.Description = "StaffName";
            this.StaffName.Name = "StaffName";
            this.StaffName.Visible = false;
            // 
            // DeptID
            // 
            this.DeptID.Description = "DeptID";
            this.DeptID.Name = "DeptID";
            this.DeptID.Type = typeof(int);
            this.DeptID.ValueInfo = "0";
            this.DeptID.Visible = false;
            // 
            // DeptName
            // 
            this.DeptName.Description = "DeptName";
            this.DeptName.Name = "DeptName";
            this.DeptName.Visible = false;
            // 
            // RegistrationType
            // 
            this.RegistrationType.Description = "RegistrationType";
            this.RegistrationType.Name = "RegistrationType";
            this.RegistrationType.Type = typeof(long);
            this.RegistrationType.ValueInfo = "0";
            this.RegistrationType.Visible = false;
            // 
            // parHospitalName
            // 
            this.parHospitalName.Description = "parHospitalName";
            this.parHospitalName.Name = "parHospitalName";
            this.parHospitalName.Visible = false;
            // 
            // parHospitalAdress
            // 
            this.parHospitalAdress.Description = "parHospitalAdress";
            this.parHospitalAdress.Name = "parHospitalAdress";
            this.parHospitalAdress.Visible = false;
            // 
            // parDepartmentOfHealth
            // 
            this.parDepartmentOfHealth.Description = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Name = "parDepartmentOfHealth";
            this.parDepartmentOfHealth.Visible = false;
            // 
            // spPrescriptions_RptViewByPrescriptIDTableAdapter
            // 
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.ClearBeforeFill = true;
            // 
            // IsKHTHView
            // 
            this.IsKHTHView.Description = "IsKHTHView";
            this.IsKHTHView.Name = "IsKHTHView";
            this.IsKHTHView.Type = typeof(bool);
            this.IsKHTHView.ValueInfo = "False";
            this.IsKHTHView.Visible = false;
            // 
            // XRpt_Temp12_KhoaPhong
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(30, 0, 16, 25);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ToDate,
            this.FromDate,
            this.TransactionID,
            this.PtRegistrationID,
            this.ViewByDate,
            this.StaffName,
            this.DeptID,
            this.DeptName,
            this.RegistrationType,
            this.parHospitalName,
            this.parHospitalAdress,
            this.parDepartmentOfHealth,
            this.IsKHTHView});
            this.RequestParameters = false;
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.Parameters.Parameter ToDate;
        private DevExpress.XtraReports.Parameters.Parameter FromDate;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport2;
        private DevExpress.XtraReports.UI.XRSubreport xrSubreport1;
        private DevExpress.XtraReports.Parameters.Parameter TransactionID;
        private DevExpress.XtraReports.Parameters.Parameter PtRegistrationID;
        private DevExpress.XtraReports.Parameters.Parameter ViewByDate;
        private DevExpress.XtraReports.Parameters.Parameter StaffName;
        private DevExpress.XtraReports.Parameters.Parameter DeptID;
        private DevExpress.XtraReports.Parameters.Parameter DeptName;
        private DevExpress.XtraReports.Parameters.Parameter RegistrationType;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalName;
        private DevExpress.XtraReports.Parameters.Parameter parHospitalAdress;
        private DevExpress.XtraReports.Parameters.Parameter parDepartmentOfHealth;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private RptConsultations.DataSchema.dsPrescriptionNewTableAdapters.spPrescriptions_RptViewByPrescriptIDTableAdapter spPrescriptions_RptViewByPrescriptIDTableAdapter;
        private DevExpress.XtraReports.Parameters.Parameter IsKHTHView;
    }
}
