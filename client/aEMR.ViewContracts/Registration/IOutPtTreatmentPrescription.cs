/*
 * 201470803 #001 CMN: Add HI Store Service
*/
using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IOutPtTreatmentPrescription
    {
        string TitleForm { get; set; }
        //==== #001
        bool IsHIOutPt { get; set; }
        //==== #001
        bool IsConfirmHIView { get; set; }
        void ApplySearchInvoiceCriteriaValues(string aOutInvID = null, long? PtRegistrationID = null);
        void SearchInvoiceOld();
        void LoadPrescriptionDetail(Prescription aPrescription);
        bool IsConfirmPrescriptionOnly { get; set; }
        bool IsServicePatient { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void PrepareDataForConfirmRegistration_New(ObservableCollection<Prescription> aPrescription, PatientRegistration cPatientRegistration);
        void SetCurPatientAndRegistration(PatientRegistration ObjRegistration);
    }
}