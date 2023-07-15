using System;

namespace aEMR.ViewContracts
{
    public interface IAppHeader
    {
        string UserName { get; set; }
        bool ShowProfileInfo { get; set; }

        string DoctorAuthoName { get; set; }
        bool isDoctorAutho { get; set; }
    }
}
