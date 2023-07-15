using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPCLLabResult : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPCLLabResult()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            long? pPtPCLLabReqID = null;
            bool? pAllHistories = null;
            try
            {
                pPtPCLLabReqID = long.Parse(this.parPtPCLLabReqID.Value.ToString());
            }
            catch
            {
                pPtPCLLabReqID = null;
            }
            try
            {
                pAllHistories = bool.Parse(this.parAllHistories.Value.ToString());
            }
            catch
            {
                pAllHistories = null;
            }

            this.ptPCLReq_PatientInfo_TblAdapter.ClearBeforeFill = true;
            this.ptPCLReq_PatientInfo_TblAdapter.Fill(this.dsPtPCLLabResult1.spRpt_PatientPCLRequest_GetHeaderInfoByID, pPtPCLLabReqID);
            this.ptPCLLabResults_HeaderInfo_TblAdapter.ClearBeforeFill = true;
            this.ptPCLLabResults_HeaderInfo_TblAdapter.Fill(this.dsPtPCLLabResult1.spRpt_PatientPCLLabResults_GetHeaderInfo, pPtPCLLabReqID);
            this.ptPCLLabResults_GetDetailsInfo_TblAdapter.ClearBeforeFill = true;
            this.ptPCLLabResults_GetDetailsInfo_TblAdapter.Fill(this.dsPtPCLLabResult1.spRpt_PatientPCLLabResults_GetDetailsInfo, pPtPCLLabReqID, pAllHistories);
        }

        private void XRptPCLLabResult_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
