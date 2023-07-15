using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IeInPrescriptions
    {
        //long ServiceRecID { get; set; }
        //long PtRegistrationID { get; set; }
        //string DiagnosisForDrug { get; set; }

        //bool IsChildWindow { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
        bool IsUpdateDiagConfirmInPT { get; set; }
        void SetLastDiagnosisForConfirm();
    }
}