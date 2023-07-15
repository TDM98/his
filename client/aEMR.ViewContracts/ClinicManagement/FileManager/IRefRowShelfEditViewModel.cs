using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRefRowShelfEditViewModel
    {
        long StoreID { get; set; }
        long RefRowID { get; set; }
        long RefShelfID { get; set; }
        long RefShelfDetailID { get; set; }
        bool IsUpdate { get; set; }
        bool IsRefRowManager { get; set; }
        bool IsRefShelfManager { get; set; }
        bool IsRefShelfDetailManager { get; set; }
        int MaxWidth { get; set; }
    }
}
