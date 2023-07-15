using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCUserGroupForm
    {
        // 20180921 TNHX: Add method for set ModulesTree, AllGroup, allUserAccount apply BusyIndicator
        ObservableCollection<UserAccount> allUserAccount { get; set; }

        UserAccount SelectedUserAccount { get; set; }

        ObservableCollection<Group> allGroup { get; set; }

        Group SelectedGroup { get; set; }
    }
}
