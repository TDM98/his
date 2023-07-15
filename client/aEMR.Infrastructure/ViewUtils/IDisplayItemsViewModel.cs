using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.Infrastructure.ViewUtils
{
    public interface IDisplayItemsViewModel<T> where T : class 
    {
        ObservableCollection<T> AllItems { get; set; }
    }
}
