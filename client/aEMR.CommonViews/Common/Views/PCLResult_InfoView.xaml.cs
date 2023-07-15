using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class PCLResult_InfoView : UserControl, IPCLResult_InfoView
    {
        public PCLResult_InfoView()
        {
            InitializeComponent();
        }

        void IPCLResult_InfoView.SetObjectSource(Stream stream)
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


        void IPCLResult_InfoView.btPlay()
        {
            mediaPreview.Play();
        }

        void IPCLResult_InfoView.btPause()
        {
            mediaPreview.Pause();
        }

        void IPCLResult_InfoView.btStop()
        {
            mediaPreview.Stop();
        }

        void IPCLResult_InfoView.btMute()
        {
            mediaPreview.IsMuted = !mediaPreview.IsMuted;
        }

        void IPCLResult_InfoView.btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPreview.Volume = (sender as Slider).Value;
        }
    }
}
