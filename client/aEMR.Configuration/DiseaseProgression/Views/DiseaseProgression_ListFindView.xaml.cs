using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.DiseaseProgression.Views
{
    [Export(typeof(DiseaseProgression_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiseaseProgression_ListFindView : UserControl
    {
        public DiseaseProgression_ListFindView()
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
