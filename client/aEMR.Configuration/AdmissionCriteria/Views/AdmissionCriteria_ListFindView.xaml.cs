using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.AdmissionCriteria.Views
{
    [Export(typeof(AdmissionCriteria_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriteria_ListFindView : UserControl
    {
        public AdmissionCriteria_ListFindView()
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
