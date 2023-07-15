using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IcwdGroupUpdate
    {
        Group SelectedGroup { get; set; }
    }
}
