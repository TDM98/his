using DataEntities;
using System;

namespace aEMR.ViewContracts
{
    public interface IServicesBookingSchedule
    {
        int CurrentMonth { get; set; }
        bool IsHasSelected { get; set; }
        DateTime CurrentDate { get; set; }
        DateTime OriginalDate { get; set; }
        DeptLocation CurrentLocation { get; set; }
        Patient CurrentPatient { get; set; }
        DateTime? EndDate { get; set; }
        long CurrentAppointmentID { get; set; }
    }
}