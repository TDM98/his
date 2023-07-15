using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.CommonViews
{
    public partial class ScanImageCaptureView : UserControl, IScanImageCaptureView
    {
        public ScanImageCaptureView()
        {
            InitializeComponent();
        }

        public Window theScanImageViewWindow
        {
            get
            {
                foreach(Window theWin in Application.Current.Windows)
                {
                    if (theWin is IScanImageCaptureView)
                        return theWin;
                }
                return null;
            }
        }
    }
}