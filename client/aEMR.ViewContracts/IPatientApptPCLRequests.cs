using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientApptPCLRequests
    {
        IPCLRequestDetails PCLRequestDetailsContent { get; set; }

        void SetCurrentAppointment(PatientAppointment appt);

        bool mPCL_TaoPhieuMoi_Them { get; set; }
        bool mPCL_TaoPhieuMoi_XemIn { get; set; }
        bool mPCL_TaoPhieuMoi_In { get; set; }
    }
}
