using System.Collections.ObjectModel;
using System.Windows.Controls;
using DataEntities;
using aEMR.Controls;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho 1 view chi chua 1 AutoCompleteBox
    /// </summary>
    public interface IAutoCompleteView
    {
        void PopulateComplete();
        AxAutoComplete AutoCompleteBox { get; }
    }
}
