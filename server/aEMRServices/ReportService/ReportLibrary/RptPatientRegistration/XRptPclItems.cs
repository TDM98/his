using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPclItems : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPclItems()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptPclItems_BeforePrint);
            this.PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptPclItems_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            adaptGetPclItems.ClearBeforeFill = true;
            string pclReqItemIDList = (string)this.Parameters["param_PCLReqItemIDList"].Value;
            long pclRequestId = long.Parse(this.Parameters["param_PCLRequestID"].Value.ToString());

            if(!string.IsNullOrWhiteSpace(pclReqItemIDList))
            {
                adaptGetPclItems.Fill(dsPclExamTypes.tblPclExamTypes, pclRequestId, pclReqItemIDList);

                if(dsPclExamTypes.tblPclExamTypes.Rows.Count > 0)
                {
                    foreach (DataRow row in dsPclExamTypes.tblPclExamTypes.Rows)
                    {
                        var deptLocID = (long)row["DeptLocID"];
                        var locationName = row["LocationName"].ToString();
                        var sequenceNumber = AxHelper.GetSequenceNumber((byte)row["ServiceSeqNumType"],(int)row["ServiceSeqNum"]);

                        if (!dsPclExamTypes.tblDeptLocations.Rows.Contains(deptLocID))
                        {
                            dsPclExamTypes.tblDeptLocations.AddtblDeptLocationsRow(deptLocID, locationName, sequenceNumber);    
                        }
                    }
                }
            }
        }
    }
}
