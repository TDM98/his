using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.OutPatientTreatmentType.Views
{
    [Export(typeof(OutPatientTreatmentTypeListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OutPatientTreatmentTypeListFindView : UserControl
    {
        public OutPatientTreatmentTypeListFindView()
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
