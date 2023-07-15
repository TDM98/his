using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IGroupRoleTab
    {
        // 20180921 TNHX: Add method for allGroup
        ObservableCollection<Group> allGroup { get; set; }
    }
}
