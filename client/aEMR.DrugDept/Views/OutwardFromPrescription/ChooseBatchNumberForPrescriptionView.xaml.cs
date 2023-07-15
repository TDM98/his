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

namespace eHCMS.DrugDept.Views
{
    public partial class ChooseBatchNumberForPrescriptionView : UserControl
    {
        public ChooseBatchNumberForPrescriptionView()
        {
            InitializeComponent();
        }

        void grdRequestDetails_UnLoaded(object sender, RoutedEventArgs e)
        {
            grdRequestDetails.SetValue(DataGrid.ItemsSourceProperty,null);
        }
    }
}
