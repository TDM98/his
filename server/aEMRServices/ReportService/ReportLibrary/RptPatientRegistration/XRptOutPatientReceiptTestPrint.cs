using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceiptTestPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceiptTestPrint()
        {
            InitializeComponent();
        }
        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

    }
}
