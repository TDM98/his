using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace aEMR.PCLDepartment.Views
{
    public partial class AbdominalUltrasoundResultView : UserControl
    {
        public Storyboard sbImageAnimation { get; set; }
        public AbdominalUltrasoundResultView()
        {
            InitializeComponent();
            sbImageAnimation = (Storyboard)FindResource("sbImageAnimation");
        }
    }
}