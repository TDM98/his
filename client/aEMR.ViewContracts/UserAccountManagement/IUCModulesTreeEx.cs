using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCModulesTreeEx
    {
        // 20180921 TNHX: Add method for ModulesTree
        ObservableCollection<ModulesTree> allModulesTree { get; set; }
    }
}
