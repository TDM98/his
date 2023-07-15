using System;

namespace aEMR.ViewContracts
{
    public interface IPCLBookingSchedule
    {
        int CurrentMonth { get; set; }
        long PCLExamTypeID { get; set; }
        bool IsHasSelected { get; set; }
        DateTime CurrentDate { get; set; }
        DateTime OriginalDate { get; set; }
    }
}