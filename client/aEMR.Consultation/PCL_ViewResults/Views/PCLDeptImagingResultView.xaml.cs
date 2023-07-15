using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using System.IO;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class PCLDeptImagingResultView : UserControl, IPCLDeptImagingResultView_Consultation
    {
        public PCLDeptImagingResultView()
        {
            InitializeComponent();
        }
        void IPCLDeptImagingResultView_Consultation.SetObjectSource(Stream stream)
        {
            if (stream != null)
            {
                //mediaPreview.SetSource(stream);
            }
            else
            {
                mediaPreview.Source = null;
            }
        }


        void IPCLDeptImagingResultView_Consultation.btPlay()
        {
            mediaPreview.Play();
        }

        void IPCLDeptImagingResultView_Consultation.btPause()
        {
            mediaPreview.Pause();
        }

        void IPCLDeptImagingResultView_Consultation.btStop()
        {
            mediaPreview.Stop();
        }

        void IPCLDeptImagingResultView_Consultation.btMute()
        {
            mediaPreview.IsMuted = !mediaPreview.IsMuted;
        }

        void IPCLDeptImagingResultView_Consultation.btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPreview.Volume = (sender as Slider).Value;
        }
    }
      
     
}
