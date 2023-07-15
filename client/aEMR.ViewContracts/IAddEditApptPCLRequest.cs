using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAddEditApptPCLRequest
    {
        IEditApptPclRequestDetailList PCLRequestDetailsContent { get; set; }
        void SetCurrentAppointment(PatientAppointment appt);
        long? RegistrationID { get; set; }
        void StartEditing(long PatientID, PatientApptPCLRequests apptPCLRequest);
        bool CanEdit { get; }
        bool mPCL_TaoPhieuMoi_Them { get; set; }
        bool mPCL_TaoPhieuMoi_XemIn { get; set; }
        bool mPCL_TaoPhieuMoi_In { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}