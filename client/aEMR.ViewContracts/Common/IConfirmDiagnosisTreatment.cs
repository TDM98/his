using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IConfirmDiagnosisTreatment
    {
        void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
        DiagnosisTreatment CurrentDiagnosisTreatment { get; }
        long DeptID { get; set; }
        long V_RegistrationType { get; set; }
        bool IsPreAndDischargeView { get; set; }
        bool IsConfirmAgainDiagnosisTreatment { get; set; }
    }
    public interface IConfirmDiagnosisTreatmentForSmallProcedure
    {
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        SmallProcedure SmallProcedureObj { get; set; }
    }
}