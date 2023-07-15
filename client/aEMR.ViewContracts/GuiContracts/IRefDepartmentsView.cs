using System.Collections.ObjectModel;
using aEMR.ViewContracts.Configuration;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRefDepartmentsView
    {
        IRefDepartments ViewModel { get; set; }
    }
}
