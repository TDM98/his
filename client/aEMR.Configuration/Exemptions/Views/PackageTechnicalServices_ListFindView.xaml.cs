using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.Exemptions.Views
{
    [Export(typeof(PackageTechnicalServices_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PackageTechnicalServices_ListFindView : UserControl
    {
        public PackageTechnicalServices_ListFindView()
        {
            InitializeComponent();
        }
    }
}
