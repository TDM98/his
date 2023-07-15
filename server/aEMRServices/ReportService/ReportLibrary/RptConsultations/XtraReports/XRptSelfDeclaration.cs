using DataEntities;
using System;
using System.Drawing;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptSelfDeclaration : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSelfDeclaration()
        {
            InitializeComponent();
        }

        private void XRptSelfDeclaration_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            spXRptSelfDeclaration1.EnforceConstraints = false;
            spXRptSelfDeclarationTableAdapter.Fill(spXRptSelfDeclaration1._spXRptSelfDeclaration
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt64(V_RegistrationType.Value));

            if (Convert.ToInt64(V_RegistrationType.Value) != Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU))
            {
                xrLabel41.Visible = false;
                xrLabel46.Visible = false;
                xrLabel42.LocationF = new PointF(10f, 696.26f);
            }
        }
    }
}
