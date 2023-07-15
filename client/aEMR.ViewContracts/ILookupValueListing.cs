using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ILookupValueListing
    {
        void LoadData();
        ObservableCollection<Lookup> LookupList { get; set; }
        Lookup SelectedItem { get; set; }

        LookupValues Type{get;set;}
        bool AddSelectOneItem{get;set;}
        bool AddSelectedAllItem { get; set; }
        void SetSelectedID(long itemID);
    }
}
