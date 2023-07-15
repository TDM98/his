using System.Collections.ObjectModel;
using System.Windows.Controls;
using DataEntities;
using aEMR.Controls;

namespace aEMR.ViewContracts
{
    public interface IPclExamTypeListingView
    {
        void PopulateComplete();
        AxAutoComplete PclExamTypes { get; }
    }
}
