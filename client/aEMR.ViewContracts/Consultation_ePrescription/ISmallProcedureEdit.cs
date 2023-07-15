using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ISmallProcedureEdit
    {
        SmallProcedure SmallProcedureObj { get; set; }
        SmallProcedure UpdatedSmallProcedure { get; }
        SmallProcedure SmallProcedure_InPt { get; }
        void ApplySmallProcedure(SmallProcedure aSmallProcedureObj);
        void GetSmallProcedure(long PtRegDetailID, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU);
        void GetLatesSmallProcedure(long PatientID, long MedServiceID, long V_RegistrationType);
        bool IsVisibility { get; set; }
        bool IsVisibilitySkip { get; set; }
        bool bConfirmVisi { get; set; }
        bool FormEditorIsEnabled { get; set; }
        void CallNotifyOfPropertyChange(string aBeforeDiagTreatment);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        bool IsFromOutOrInDiag { get; set; }
        long Compare2ICD9List();
        ObservableCollection<DiagnosisICD9Items> refICD9List { get; set; }
        ObservableCollection<Resources> SelectedResourceList { get; set; }
        bool CompareResource();
        void GetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID);
    }
}