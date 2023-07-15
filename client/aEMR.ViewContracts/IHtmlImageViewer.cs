using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IHtmlImageViewer
    {
        ObservableCollection<string> FileCollection { get; set; }
        string SelectedFilePath { get; set; }
    }
}