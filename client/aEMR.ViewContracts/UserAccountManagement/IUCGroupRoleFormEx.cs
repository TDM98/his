using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCGroupRoleFormEx
    {
        // 20180921 TNHX: Add method for set AllGroup, allRole, apply BusyIndicator
        ObservableCollection<Group> allGroup { get; set; }
        Group SelectedGroup { get; set; }
        ObservableCollection<Role> allRole { get; set; }
        Role SelectedRole { get; set; }
    }
}
