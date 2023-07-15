using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptAppointmentReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptAppointmentReport()
        {
            InitializeComponent();
        }

        private void XRptAppointmentReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DateTime? DateFrom = null;
            DateTime? DateTo = null;

            this.ShowNgayThang.Value = "";

            if (Convert.ToInt16(this.DateFromFlag.Value) != 0)
            {
                DateFrom = null;
            }
            else
            {
                DateFrom = this.para_DateFrom.Value as DateTime?;
                this.ShowNgayThang.Value = string.Format("{0} : ",  eHCMSResources.G1933_G1_TuNg) + DateFrom.GetValueOrDefault().ToString("dd/MM/yyyy");
            }
            if (Convert.ToInt16(this.DateToFlag.Value) != 0)
            {
                DateTo = null;
            }
            else
            {
                DateTo = this.para_DateTo.Value as DateTime?;
                if (!string.IsNullOrEmpty(this.ShowNgayThang.Value.ToString()))
                {
                    this.ShowNgayThang.Value += " - ";
                }
                this.ShowNgayThang.Value = string.Format("{0} : ",  eHCMSResources.K3192_G1_DenNg) + DateTo.GetValueOrDefault().ToString("dd/MM/yyyy");
            }

            if (string.IsNullOrEmpty(this.ShowNgayThang.Value.ToString()))
            {
                this.ShowNgayThang.Value = null;
            }
            spGetAppointments_RptTableAdapter.Fill(dsGetAppointments1.spGetAppointments_Rpt
                , Convert.ToInt64(this.para_PatientID.Value)
                , DateFrom, DateTo
                , Convert.ToInt64(this.para_V_ApptStatus.Value)
                , Convert.ToInt16(this.para_LoaiDV.Value)
                , Convert.ToInt64(this.para_DeptLocationID.Value)
                , Convert.ToBoolean(this.para_IsConsultation.Value)
                ,Convert.ToInt64(this.para_StaffID.Value)
                , Convert.ToInt64(this.para_DoctorStaffID.Value)
                , Convert.ToInt16(this.para_ConsultationTimeSegmentID.Value)
                , this.para_StartTime.Value as DateTime?
                , this.para_EndTime.Value as DateTime?
                );
        }

    }
}
