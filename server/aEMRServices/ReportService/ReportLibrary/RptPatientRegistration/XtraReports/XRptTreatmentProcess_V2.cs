using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace eHCMS.ReportLib.RptPatientRegistration.XtraReports
{
    public partial class XRptTreatmentProcess_V2 : XtraReport
    {
        public XRptTreatmentProcess_V2()
        {
            InitializeComponent();
        }

        private void XRptTreatmentProcessV2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            //if (dsRptTreatmentProcessV21.spGetTreatmentProcessByID_V2 != null && dsRptTreatmentProcessV21.spGetTreatmentProcessByID_V2.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dsRptTreatmentProcessV21.spGetTreatmentProcessByID_V2.Rows)
            //    {
            //        //if (row["Gender"].ToString() == "M")
            //        //{
            //        //    cbM.Checked = true;
            //        //    cbF.Checked = false;
            //        //}
            //        //else
            //        //{
            //        //    cbF.Checked = true;
            //        //    cbM.Checked = false;
            //        //}
            //    }
            //}
        }
        private void FillData()
        {
            dsRptTreatmentProcessV21.EnforceConstraints = false;
            spGetTreatmentProcessByID_V2TableAdapter.Fill(dsRptTreatmentProcessV21.spGetTreatmentProcessByID_V2, Convert.ToInt64(parTreatmentProcessID.Value));
        }
    }
}
