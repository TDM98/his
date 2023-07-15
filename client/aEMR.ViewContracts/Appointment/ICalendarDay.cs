using DataEntities;
using System;

namespace aEMR.ViewContracts
{
    public delegate void OnDateChanged(DateTime? StartDate, DateTime? EndDate);
    public interface ICalendarDay
    {
        DateTime CurrentDate { get; set; }
        Patient CurrentPatient { get; set; }
        OnDateChanged OnDateChangedCallback { get; set; }
        DeptLocation CurrentLocation { get; set; }
        long CurrentAppointmentID { get; set; }
        long? CurrentConsultationRoomStaffAllocID { get; set; }
        long CurrentMedServiceID { get; set; }
        byte CurrentViewCase { get; set; }
        bool IsConfirmed { get; set; }
        DateTime[] CurrentValidDateTime { get; set; }
    }
}