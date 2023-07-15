using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IEnumListing
    {
        ObservableCollection<EnumDescription> EnumItemList { get; set; }
        EnumDescription SelectedItem { get; set; }
        void LoadData();
        Type EnumType { get; set; }
        bool AddSelectOneItem{get;set;}
        bool AddSelectedAllItem { get; set; }
        void SetSelectedID(string itemID);
        event EventHandler SelectionChange;
        /// <summary>
        /// Item dau tien la item duoc chon.
        /// </summary>
        bool FirstItemAsDefault { get; set; }
        ObservableCollection<string> ExclusionValues { get; set; }
    }

    public class EnumDescription
    {
        public Enum EnumItem { get; set; }
        public string EnumValue { get; set; }
        public string Description { get; set; }
        public int EnumIntValue { get; set; }
    }
}
