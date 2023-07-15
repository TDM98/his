using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace aEMR.ViewContracts
{
    public interface IFileExplorerView
    {
        void SetObjectSource(Stream stream);

        void btPlay();
        void btPause();
        void btStop();
        void btMute();
        void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e);
    }
}
