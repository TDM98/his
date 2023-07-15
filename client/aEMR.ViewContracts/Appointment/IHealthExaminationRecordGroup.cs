using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IHealthExaminationRecordGroup
    {
        long HosClientContractID { get; set; }
        HosClientContractPatientGroup CurrentHosClientContractPatientGroup { get; set; }
        bool IsCompleted { get; set; }
        ObservableCollection<HosClientContractPatientGroup> PatientGroupCollection { set; }
    }
}