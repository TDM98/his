using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.CitiesProvinces.Views
{
    [Export(typeof(CitiesProvinces_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CitiesProvinces_ListFindView : UserControl
    {
        public CitiesProvinces_ListFindView()
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
