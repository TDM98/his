using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;

namespace aEMR.Pharmacy.Views
{
    [Export(typeof(PharmacyModuleView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PharmacyModuleView : UserControl
    {
        public PharmacyModuleView()
        {
            InitializeComponent();
        }
    }
}
