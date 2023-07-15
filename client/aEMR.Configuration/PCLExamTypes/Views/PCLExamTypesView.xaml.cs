using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.Configuration.PCLExamTypes.Views
{
    [Export(typeof(PCLExamTypesView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PCLExamTypesView : UserControl
    {
        public PCLExamTypesView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dtgList.Columns[0].Visibility = ((aEMR.Configuration.PCLExamTypes.ViewModels.PCLExamTypesViewModel)(this.DataContext)).
                    dgCellTemplate0_Visible;
        }
         
    }
}
