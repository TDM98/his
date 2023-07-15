using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.Exemptions.Views
{
    [Export(typeof(Exemptions_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Exemptions_ListFindView : UserControl
    {
        public Exemptions_ListFindView()
        {
            InitializeComponent();
        }

     
    }
}
