using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptRegisteredService : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRegisteredService()
        {
            InitializeComponent();

            this.PrintingSystem.ShowPrintStatusDialog = false;
        }
    }
}
