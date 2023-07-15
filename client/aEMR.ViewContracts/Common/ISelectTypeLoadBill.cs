using System;

namespace aEMR.ViewContracts
{
    public interface ISelectTypeLoadBill
    {
        DateTime FromDate { get; set; }
        DateTime ToDate { get; set; }
        bool IsCompleted { get; set; }
        int LoadBillType { get; set; }
        DateTime? DischargeDate { get; set; }
    }
}
