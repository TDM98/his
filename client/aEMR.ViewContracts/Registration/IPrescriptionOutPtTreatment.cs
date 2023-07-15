/*
 * 201470803 #001 CMN: Add HI Store Service
*/
using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IPrescriptionOutPtTreatment
    {
        string TitleForm { get; set; }
        bool IsHIOutPt { get; set; }
        bool IsConfirmHIView { get; set; }
        void ApplySearchInvoiceCriteriaValues(string aOutInvID = null, long? PtRegistrationID = null);
        void SearchInvoiceOld();
        void LoadPrescriptionDetail(Prescription aPrescription);
        void PrepareDataForConfirmRegistration_New(ObservableCollection<Prescription> aPrescription, PatientRegistration cPatientRegistration);
        bool IsConfirmPrescriptionOnly { get; set; }
        bool IsServicePatient { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void SetCurPatientAndRegistration(PatientRegistration ObjRegistration);
    }
}