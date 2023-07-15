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

using DataEntities;
using System.ComponentModel;
using aEMR.Controls;
using aEMR.ResourceMaintenance.ViewModels;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropertyAllocationsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PropertyAllocationsView : AxUserControl
    {
        
        public PropertyAllocationsView()
        {
            InitializeComponent();
        }
        void PropertyAllocationsView_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }
        void PropertyAllocationsView_Loaded(object sender, RoutedEventArgs e)
        {
                  
        }
        //public VMResourcePropertyPage ResourcePropertyPageVM;
        
    }
}

