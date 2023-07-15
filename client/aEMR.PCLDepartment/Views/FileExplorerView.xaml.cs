using System.IO;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.PCLDepartment.Views
{
    public partial class FileExplorerView : UserControl, IFileExplorerView
    {
        public FileExplorerView()
        {
            InitializeComponent();
        }

        #region IFileExplorerView Members

        void IFileExplorerView.SetObjectSource(Stream stream)
        {
            //28062018 TTM: Comment SetSource do khong biet cach fix.
            //mediaPreview.SetSource(stream);
        }


        void IFileExplorerView.btPlay()
        {
            mediaPreview.Play();
        }

        void IFileExplorerView.btPause()
        {
            mediaPreview.Pause();
        }

        void IFileExplorerView.btStop()
        {
            mediaPreview.Stop();
        }

        void IFileExplorerView.btMute()
        {
            mediaPreview.IsMuted = !mediaPreview.IsMuted;
        }

        void IFileExplorerView.btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPreview.Volume = (sender as Slider).Value;
        }

        #endregion
    }
}