using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.JobList.Views
{
    [Export(typeof(Job_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Job_ListFindView : UserControl
    {
        public Job_ListFindView()
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
