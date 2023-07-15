using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.CommonViews.Views
{
    [Export(typeof(ICD_ListFindForConsultationView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ICD_ListFindForConsultationView : UserControl
    {
        public ICD_ListFindForConsultationView()
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
