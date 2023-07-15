using DataEntities;
using System;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IOvertimeWorkingScheduleDetail
    {
        DayOfWeek CurDayOfWeek { get; set; }
        DateTime CurrentDate { get; set; }
        OvertimeWorkingSchedule CurrentOvertimeWorkingSchedule { get; set; }
        OvertimeWorkingWeek CurrentOvertimeWorkingWeek { get; set; }
        ObservableCollection<DeptLocation> LocationCollection { get; set; }
        ObservableCollection<Staff> DoctorStaffs { get; set; }
        void InitOvertimeWorkingSchedule();
    }
}