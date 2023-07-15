using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPCLDeptImagingResult_Consultation
    {
        void LoadData(PatientPCLRequest message);
        void LoadDataFromPK(long PatientPCLReqExtID);
        void Reset();
        bool IsEdit { get; set; }
        void LoadDataCoroutineEx(PatientPCLRequest message);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
