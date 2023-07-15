using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.LookupList.Views
{
    [Export(typeof(Lookup_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Lookup_ListFindView : UserControl
    {
        public Lookup_ListFindView()
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
