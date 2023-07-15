using System.Windows.Controls;
using aEMR.ViewContracts;
using System.IO;
using System.Windows;

namespace aEMR.PCLDepartment.Views
{
    public partial class PatientPCLDeptImagingResultView : UserControl, IPCLDeptImagingResultView
    {
        public PatientPCLDeptImagingResultView()
        {
            InitializeComponent();
        }
        void IPCLDeptImagingResultView.SetObjectSource(Stream stream)
        {
            if (stream != null)
            {
                //ChangedWPF-CMN
                //mediaPreview.SetSource(stream);
            }
            else
            {
                mediaPreview.Source = null;
            }
        }


        void IPCLDeptImagingResultView.btPlay()
        {
            mediaPreview.Play();
        }

        void IPCLDeptImagingResultView.btPause()
        {
            mediaPreview.Pause();
        }

        void IPCLDeptImagingResultView.btStop()
        {
            mediaPreview.Stop();
        }

        void IPCLDeptImagingResultView.btMute()
        {
            mediaPreview.IsMuted = !mediaPreview.IsMuted;
        }

        void IPCLDeptImagingResultView.btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPreview.Volume = (sender as Slider).Value;
        }

      
     
    }
}