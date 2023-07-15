using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IcwdUserAccountUpdate
    {
        UserAccount SelectedUserAccount { get; set; }
        ObservableCollection<UserAccount> allUserName { get; set; }
    }
}
