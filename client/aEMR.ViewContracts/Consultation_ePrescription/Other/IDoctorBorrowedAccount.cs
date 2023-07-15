using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDoctorBorrowedAccount
    {
        bool IsPopupView { get; set; }
        int PatientFindBy { get; set; }
    }
}
