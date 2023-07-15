using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCUserGroupFormEx
    {
        // 20180921 TNHX: Add method for set allUserAccount, allGroup, apply BusyIndicator
        ObservableCollection<UserAccount> allUserAccount { get; set; }

        UserAccount SelectedUserAccount { get; set; }

        ObservableCollection<Group> allGroup { get; set; }

        Group SelectedGroup { get; set; }
    }
}
