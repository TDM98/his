using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.Locations.Views
{
    [Export(typeof(Locations_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Locations_ListFindView : UserControl
    {
        public Locations_ListFindView()
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
