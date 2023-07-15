using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.Pharmacy.Views
{
    public partial class InwardDrugOtherView : UserControl
    {
        public InwardDrugOtherView()
        {
            InitializeComponent();
        }

        private void GridInwardDrug_Unloaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug.SetValue(DataGrid.ItemsSourceProperty,null);
        }

      
    }
}
