using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace eHCMS.ReportLib.RptPatientRegistration.XtraReports
{
    public partial class XRptTreatmentProcess : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTreatmentProcess()
        {
            InitializeComponent();
        }

        private void XRptTreatmentProcess_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsRptTreatmentProcess1.spGetTreatmentProcessByID != null && dsRptTreatmentProcess1.spGetTreatmentProcessByID.Rows.Count > 0)
            {
                foreach (DataRow row in dsRptTreatmentProcess1.spGetTreatmentProcessByID.Rows)
                {
                    if (row["Gender"].ToString() == "M")
                    {
                        cbM.Checked = true;
                        cbF.Checked = false;
                    }
                    else
                    {
                        cbF.Checked = true;
                        cbM.Checked = false;
                    }
                }
            }
        }
        private void FillData()
        {
            dsRptTreatmentProcess1.EnforceConstraints = false;
            spGetTreatmentProcessByIDTableAdapter.Fill(dsRptTreatmentProcess1.spGetTreatmentProcessByID, Convert.ToInt64(parTreatmentProcessID.Value));
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }
    }
}
