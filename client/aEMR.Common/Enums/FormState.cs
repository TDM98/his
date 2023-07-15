using System;

namespace aEMR.Common.Enums
{
    public enum FormState
    {
        ReadOnly = 0,
        New = 1,
        Edit = 2
    }

    [Flags]
    public enum MsgBoxOptions
    {
        None = 0,
        Ok = 2,
        Cancel = 4,
        Yes = 8,
        No = 16,
        Close = 32,

        OkCancel = Ok | Cancel,
        YesNo = Yes | No,
        YesNoCancel = Yes | No | Cancel

    }
}
