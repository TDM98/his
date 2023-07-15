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
using System.Windows.Navigation;


namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class PtBedAllocationsView : UserControl
    {
        public PtBedAllocationsView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PtBedAllocationsView_Loaded);
            this.Unloaded += new RoutedEventHandler(PtBedAllocationsView_Unloaded);
            
        }

        void PtBedAllocationsView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void PtBedAllocationsView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
    }
}
