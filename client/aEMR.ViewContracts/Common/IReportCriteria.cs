using System;

namespace aEMR.ViewContracts
{
    public interface IReportCriteria
    {
        DateTime FromDate { get; set; }
        DateTime ToDate { get; set; }
        bool IsCompleted { get; set; }
    }
}