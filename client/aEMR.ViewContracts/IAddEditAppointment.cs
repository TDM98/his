using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAddEditAppointment
    {
        void SetCurrentAppointment(PatientAppointment appt, long? aIssueID = null);
        void SetCurrentPatient(Patient patient);
        void SetCurrentPatient(Patient patient, long serviceRecID);
        void CreateNewAppointment(long V_AppointmentType = (long) AllLookupValues.AppointmentType.HEN_TAI_KHAM, long? DoctorStaffID = null, DateTime? ExtApptDate = null, long? aServiceRecID = 0);
        long RegistrationID { get; set; }
        ///////// <summary>
        ///////// Tạo một yêu cầu hẹn bệnh (Trạng thái là waiting)
        ///////// </summary>
        //////void CreateNewAppointmentRequest();
        ObservableCollection<Lookup> AppointmentStatusList { get; set; }
        bool IsCreateApptFromConsultation { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        bool IsPCLBookingView { get; set; }
        long CurrentPtRegDetailAppointmentID { get; set; }
        bool IsCreateApptFromNurseModule { get; set; }
        bool IsInTreatmentProgram { get; set; }
        bool TreatmentAppointmentSameReg { get; set; }
    }
}