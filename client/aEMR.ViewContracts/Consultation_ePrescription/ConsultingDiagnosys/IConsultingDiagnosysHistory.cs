using DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IConsultingDiagnosysHistory
    {
        ConsultingDiagnosys gConsultingDiagnosys { get; set; }
        List<Lookup> DischargeTypeArray { get; set; }
        List<Staff> DoctorStaffs { get; set; }
        void InitDataView();
        SurgeryScheduleDetail GetSurgeryScheduleDetail();
        SurgerySchedule SelectedSurgerySchedule { get; set; }
        ObservableCollection<Staff> SurgeryDoctorCollection { get; set; }
        DateTime? ExpSurgeryDate { get; set; }
    }
}
