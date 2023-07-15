using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Printing;

namespace aEMR.ViewContracts
{
    public interface IPrinterSettings
    {
        ObservableCollection<PrinterTypePrinterAssignment> AllPrinterTypePrinterAssignments { get; set; }
        IPrinterListing AvailablePrintersContent { get; set; }
        void GetAllPrinterTypeAssignments();
    }
}
