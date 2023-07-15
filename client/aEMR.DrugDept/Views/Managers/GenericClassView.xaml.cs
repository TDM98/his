using System;
using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.DrugDept.Views
{
    [Export(typeof(GenericClassView))]
    public partial class GenericClassView : UserControl
    {
        public GenericClassView()
        {
            InitializeComponent();
        }
    }
}
