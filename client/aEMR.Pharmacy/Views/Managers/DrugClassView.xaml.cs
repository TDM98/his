using System;
using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.Pharmacy.Views
{
    [Export(typeof(DrugClassView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DrugClassView : UserControl
    {
        public DrugClassView()
        {
            InitializeComponent();
        }

      
     
    }
}
