using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IHtmlImageViewer)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlImageViewerViewModel : ViewModelBase, IHtmlImageViewer
    {
        public string SelectedFilePath { get; set; }
        private ObservableCollection<string> _FileCollection;
        public ObservableCollection<string> FileCollection
        {
            get
            {
                return _FileCollection;
            }
            set
            {
                if (_FileCollection == value)
                {
                    return;
                }
                _FileCollection = value;
                NotifyOfPropertyChange(() => FileCollection);
            }
        }
        public void Image_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button CurrentButton = sender as Button;
            SelectedFilePath = CurrentButton.DataContext.ToString();
            IImageEditor aView = Globals.GetViewModel<IImageEditor>();
            aView.ImageSourcePath = SelectedFilePath;
            GlobalsNAV.ShowDialog_V3(aView);
            SelectedFilePath = aView.ImageSourcePath;
            TryClose();
        }
    }
}