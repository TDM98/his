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

namespace aEMR.Configuration.PCLExamTypeMedServiceDefItems.Views
{
    [Export(typeof(PCLExamTypeMedServiceDefItemsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PCLExamTypeMedServiceDefItemsView : UserControl
    {
        public PCLExamTypeMedServiceDefItemsView()
        {
            InitializeComponent();
        }
    }
}
