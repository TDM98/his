using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.InsuranceBenefitCategories.Views
{
    [Export(typeof(InsuranceBenefitCategories_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InsuranceBenefitCategories_ListFindView : UserControl
    {
        public InsuranceBenefitCategories_ListFindView()
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
