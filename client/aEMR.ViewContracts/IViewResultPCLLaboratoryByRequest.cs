using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IViewResultPCLLaboratoryByRequest
    {
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
        PatientPCLRequestSearchCriteria SearchCriteria { get; set; }
        void InitData();
        void InitPatientInfo(Patient patientInfo);
        void setDefaultWhenRenewPatient();
        bool IsDialogView { get; set; }
        void LoadPCLRequestResult(PatientPCLRequest aPCLRequest);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        ObservableCollection<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail { get; }
        PatientPCLRequest ObjPatientPCLRequest_SearchPaging_Selected { get; }
        bool IsShowCheckTestItem { get; set; }
    }
}