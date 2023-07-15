using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace aEMR.ViewContracts
{
    public interface IImageDisgnosticExplorer
    {
        BitmapImage ObjBitmapImage { get; set; }
        //==== 20161008 CMN Begin: Change Image View to WriteableBitmap
        WriteableBitmap ObjWBitmapImage { get; set; }
        int TypeOfBitmapImage { get; set; }
        //==== 20161008 CMN End.
    }
}
