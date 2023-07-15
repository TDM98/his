using DataEntities;
using System;

namespace aEMR.ViewContracts
{
    public interface IConsultationRoomStaff_V2
    {
        bool IsEditViewCase { get; set; }
        bool IsSuccessedCalled { get; set; }
        long SelectedTimeSegmentID { get; set; }
        DateTime SelectedCalendarDate { get; set; }
        DeptLocation SelectedLocation { get; set; }
    }
}