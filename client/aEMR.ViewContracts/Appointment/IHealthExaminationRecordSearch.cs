using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IHealthExaminationRecordSearch
    {
        List<HospitalClient> HospitalClientCollection { get; set; }
        HospitalClientContract SelectedHospitalClientContract { get; set; }
        bool IsCompleted { get; set; }
    }
}