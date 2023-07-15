using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesTopMenuView))]
    public partial class ResourcesTopMenuView
    {
        public ResourcesTopMenuView()
        {
            InitializeComponent();
        }
    }
}
