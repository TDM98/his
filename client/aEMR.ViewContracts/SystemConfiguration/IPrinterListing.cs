using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Printing;

namespace aEMR.ViewContracts
{
    public interface IPrinterListing
    {
        bool ShowButtonPanel { get; set; }
        PrinterInfo SelectedItem { get; set; }
        ObservableCollection<PrinterInfo> AllPrinters { get; set; }
        PrinterInfo DefaultPrinter { get;}
        IFileDownload PrinterServerNotAvailableContent { get; set; }
        void GetAllPrinters();
    }
}
