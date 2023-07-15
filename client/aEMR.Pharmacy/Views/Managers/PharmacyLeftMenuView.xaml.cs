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
    [Export(typeof(PharmacyLeftMenuView))]
    public partial class PharmacyLeftMenuView : UserControl
    {
        public PharmacyLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }

        private void AccordionItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
