using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.RefApplicationConfig.Views
{
    [Export(typeof(RefApplicationConfig_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RefApplicationConfig_ListFindView : UserControl
    {
        public RefApplicationConfig_ListFindView()
        {
            InitializeComponent();
        }

        public event EventHandler DblClick;

        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null)
            {
                DblClick(this, null);
            }
        }

     
    }
}
