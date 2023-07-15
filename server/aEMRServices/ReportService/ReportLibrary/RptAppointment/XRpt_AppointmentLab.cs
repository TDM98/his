using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRpt_AppointmentLab : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_AppointmentLab()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRpt_AppointmentLab_BeforePrint);
        }

        public void FillData()
        {
            dsAppointmentLab1.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(dsAppointmentLab1.spXRptAppointmentsLab, spXRptAppointmentsLabTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt16(Quarter.Value)
                , Convert.ToInt16(Month.Value)
                , Convert.ToInt16(Year.Value)
                , Convert.ToByte(Flag.Value)
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt64(V_SendSMSStatus.Value)
            }, int.MaxValue);
        }

        private void XRpt_AppointmentLab_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
