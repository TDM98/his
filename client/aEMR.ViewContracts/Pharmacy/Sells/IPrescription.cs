/*
 * 201470803 #001 CMN: Add HI Store Service
*/
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPrescription
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
    }
}