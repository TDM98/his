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

namespace aEMR.Common.Views
{
    public partial class PatientSummaryInfoV2View : UserControl, IPatientSummaryInfoV2View
    {
        public PatientSummaryInfoV2View()
        {
            InitializeComponent();
        }

        public void Switch(bool isExpanded)
        {
            if (isExpanded)
            {
                border1.Visibility = Visibility.Visible;
                border2.Visibility = Visibility.Collapsed;
            }
            else
            {
                border1.Visibility = Visibility.Collapsed;
                border2.Visibility = Visibility.Visible;
            }
        }
    }
}
