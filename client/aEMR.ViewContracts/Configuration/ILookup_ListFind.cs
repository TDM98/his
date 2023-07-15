using System;
using System.Windows;


namespace aEMR.ViewContracts.Configuration
{
    public interface ILookup_ListFind
    {
        void Lookup_MarkDeleted(long LKID);

        Visibility hplAddNewVisible { get; set; }
        DataEntities.Lookup Lookup_Current { get; set; }
    }
}
