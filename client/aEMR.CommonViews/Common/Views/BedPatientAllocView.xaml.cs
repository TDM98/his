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
using aEMR.ViewContracts.Configuration;

namespace aEMR.Common.Views
{
    public partial class BedPatientAllocView : UserControl, IBedPatientAllocView
    {
        public BedPatientAllocView()
        {
            InitializeComponent();
        }

        public void LoadGrid(Grid GridControl)
        {
            GridBedPatientAlloc.Children.Clear();
            GridBedPatientAlloc.Children.Add(GridControl);
        }
    }
}
