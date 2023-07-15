using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRouteOfAdministrationContactDrug
    {
        EntitiesEdit<Lookup> V_RouteOfAdministration_Edit { get; set; }
        ObservableCollection<string> allContrainName { get; set; }
        RefGenMedProductDetails NewDrug { get; set; }
    }
}
