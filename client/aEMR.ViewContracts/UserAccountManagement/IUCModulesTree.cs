using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCModulesTree
    {
        // 20180921 TNHX: Add method for ModulesTree
        ObservableCollection<ModulesTree> allModulesTree { get; set; }
    }
}
