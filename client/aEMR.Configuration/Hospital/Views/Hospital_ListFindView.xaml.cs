using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.Hospital.Views
{
    [Export(typeof(Hospital_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Hospital_ListFindView : UserControl
    {
        public Hospital_ListFindView()
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
