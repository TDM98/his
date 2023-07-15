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
using aEMR.ViewContracts;

namespace aEMR.Registration.Views
{
    public partial class InPatientBedPatientListingView : UserControl, IInPatientBedPatientListingView
    {
        public InPatientBedPatientListingView()
        {
            InitializeComponent();
        }

        public void ShowDeleteColumn(bool bShow)
        {
            if (bShow)
            {
                grid.Columns[0].Visibility = Visibility.Visible;
            }
            else
            {
                grid.Columns[0].Visibility = Visibility.Collapsed;
            }
        }
    }
}
