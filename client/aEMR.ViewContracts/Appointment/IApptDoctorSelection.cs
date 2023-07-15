using DataEntities;
using System;

namespace aEMR.ViewContracts
{
    public interface IApptDoctorSelection
    {
        DateTime CurrentDate { get; set; }
        ConsultationRoomStaffAllocations ConfirmedAllocation { get; set; }
        DeptLocation CurrentLocation { get; set; }
        long CurrentMedServiceID { get; set; }
        DateTime? ConfirmedDate { get; set; }
        ConsultationTimeSegments ConfirmedTimeSegment { get; set; }
        Patient CurrentPatient { get; set; }
        long CurrentAppointmentID { get; set; }
        DateTime? EndDate { get; set; }
    }
}