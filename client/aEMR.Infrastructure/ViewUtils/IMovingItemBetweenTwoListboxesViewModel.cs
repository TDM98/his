using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.Infrastructure.ViewUtils
{
    public interface IMovingItemBetweenTwoListboxesViewModel<T> where T:class 
    {
        List<T> AllItems { get; set; }
        ObservableCollection<T> UnAssignedItems { get;}
        ObservableCollection<T> AssignedItems { get; set; }
        IEqualityComparer<T> ItemComparer { get; set; }
        T AssignedListSelectedItem { get; set; }
        T UnAssignedListSelectedItem { get; set; }

        void AddAllCmd();
        void AddCmd();
        void RemoveCmd();
        void RemoveAllCmd();
    }
}
