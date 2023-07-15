using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRefRow
    {
        int MaxWidth { get; set; }
        bool IsDisableEdit { get; }
    }
}
