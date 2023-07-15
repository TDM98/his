using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.JobList.Views
{
    [Export(typeof(Job_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Job_AddEditView : UserControl
    {
        public Job_AddEditView()
        {
            InitializeComponent();
        }
    }
}
