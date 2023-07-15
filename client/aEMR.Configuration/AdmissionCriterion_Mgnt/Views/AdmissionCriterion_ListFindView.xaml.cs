using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.Views
{
    [Export(typeof(AdmissionCriterion_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriterion_ListFindView : UserControl
    {
        public AdmissionCriterion_ListFindView()
        {
            InitializeComponent();
        }

        //public event EventHandler DblClick;

        //private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (DblClick != null)
        //    {
        //        DblClick(this, null);
        //    }
        //}

     
    }
}
