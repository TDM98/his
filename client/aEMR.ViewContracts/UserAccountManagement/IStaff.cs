using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IStaff
    {
        bool IsAddNew { get; set; }
        Staff curStaff { get; set; }
        bool ModifiedPasswordDT { get; set; }
    }
}
