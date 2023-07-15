namespace eHCMS.ReportLib.Enterprise.DrHuan
{
    partial class XRptSub_DrHuanLogo
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
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.parOrganizationNotes = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.parOrganizationAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.parOrganizationName = new DevExpress.XtraReports.Parameters.Parameter();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.styleParagraph = new DevExpress.XtraReports.UI.XRControlStyle();
            this.styleH4 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.styleH2 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.styleLine = new DevExpress.XtraReports.UI.XRControlStyle();
            this.styleH3 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.styleH1 = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel4,
            this.xrLabel3});
            this.Detail.HeightF = 46F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel1.CanShrink = true;
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parOrganizationNotes, "Text", "")});
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(519F, 0F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(243F, 32.38F);
            this.xrLabel1.StyleName = "styleParagraph";
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel1";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // parOrganizationNotes
            // 
            this.parOrganizationNotes.Name = "parOrganizationNotes";
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel4.CanShrink = true;
            this.xrLabel4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parOrganizationAddress, "Text", "")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(255.64F, 23F);
            this.xrLabel4.StyleName = "styleParagraph";
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "xrLabel4";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // parOrganizationAddress
            // 
            this.parOrganizationAddress.Name = "parOrganizationAddress";
            this.parOrganizationAddress.Visible = false;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel3.CanShrink = true;
            this.xrLabel3.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.parOrganizationName, "Text", "")});
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(255.64F, 23F);
            this.xrLabel3.StyleName = "styleH4";
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "xrLabel3";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // parOrganizationName
            // 
            this.parOrganizationName.Name = "parOrganizationName";
            this.parOrganizationName.Visible = false;
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
            // styleParagraph
            // 
            this.styleParagraph.Font = new System.Drawing.Font("Arial", 10F);
            this.styleParagraph.Name = "styleParagraph";
            // 
            // styleH4
            // 
            this.styleH4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.styleH4.Name = "styleH4";
            // 
            // styleH2
            // 
            this.styleH2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.styleH2.ForeColor = System.Drawing.Color.DarkRed;
            this.styleH2.Name = "styleH2";
            // 
            // styleLine
            // 
            this.styleLine.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
            this.styleLine.BorderWidth = 1F;
            this.styleLine.ForeColor = System.Drawing.Color.DarkRed;
            this.styleLine.Name = "styleLine";
            // 
            // styleH3
            // 
            this.styleH3.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.styleH3.Name = "styleH3";
            // 
            // styleH1
            // 
            this.styleH1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.styleH1.ForeColor = System.Drawing.Color.DarkRed;
            this.styleH1.Name = "styleH1";
            // 
            // XRptSub_DrHuanLogo
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 1169;
            this.PageWidth = 762;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.parOrganizationName,
            this.parOrganizationAddress,
            this.parOrganizationNotes});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.styleParagraph,
            this.styleH4,
            this.styleH2,
            this.styleLine,
            this.styleH3,
            this.styleH1});
            this.Version = "17.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRControlStyle styleParagraph;
        private DevExpress.XtraReports.UI.XRControlStyle styleH4;
        private DevExpress.XtraReports.UI.XRControlStyle styleH2;
        private DevExpress.XtraReports.UI.XRControlStyle styleLine;
        private DevExpress.XtraReports.UI.XRControlStyle styleH3;
        private DevExpress.XtraReports.UI.XRControlStyle styleH1;
        public DevExpress.XtraReports.Parameters.Parameter parOrganizationName;
        public DevExpress.XtraReports.Parameters.Parameter parOrganizationAddress;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.Parameters.Parameter parOrganizationNotes;
    }
}
