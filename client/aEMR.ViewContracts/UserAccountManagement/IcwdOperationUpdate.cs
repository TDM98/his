using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IcwdOperationUpdate
    {
        Operation SelectedOperation { get; set; }
    }
}
