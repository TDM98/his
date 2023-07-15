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

namespace aEMR.Configuration.DeptLocMedServices.Views
{
    [Export(typeof(DeptLocMedServicesView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DeptLocMedServicesView : UserControl
    {
        public DeptLocMedServicesView()
        {
            InitializeComponent();
        }
    }
}
