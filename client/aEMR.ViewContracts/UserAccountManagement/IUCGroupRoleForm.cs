using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCGroupRoleForm
    {
        // 20180921 TNHX: Add method for set ModulesTree, AllGroup, allUserAccount, allRole, apply BusyIndicator
        ObservableCollection<Group> allGroup { get; set; }
        Group SelectedGroup { get; set; }
        ObservableCollection<Role> allRole { get; set; }
        Role SelectedRole { get; set; }
    }
}
