using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.ICDList.Views
{
    [Export(typeof(ICD_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ICD_ListFindView : UserControl
    {
        public ICD_ListFindView()
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
