using System;

namespace aEMR.Infrastructure
{
    [Flags]
    public enum MessageBoxOptions
    {
        Ok = 2,
        Cancel = 4,
        Yes = 8,
        No = 16,

        OkCancel = Ok | Cancel,
        YesNo = Yes | No,
        YesNoCancel = Yes | No | Cancel

    }
    public enum AxMessageBoxResult
    {
        Unknown = 0,
        Ok = 2,
        Cancel = 4,
        Yes = 8,
        No = 16
    }

    public enum ConfirmHiBenefitEnum
    {
        NoChangeConfirmHiBenefit = 0,
        ConfirmHiBenefit = 1,
        RemoveConfirmedHiCard = 2,
    }
}
