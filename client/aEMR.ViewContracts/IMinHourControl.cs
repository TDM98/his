using System;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Control gio, phut, ngay.
    /// </summary>
    public interface IMinHourControl
    {
        DateTime? DateTime { get; set; }
        DateTime? DatePart { get;}
        int? HourPart { get;}
        int? MinutePart { get;}
        bool bShowButton { get; set; }
        bool bOK { get; set; }
        bool IsEnableMinHourControl { get; set; }
    }
}
