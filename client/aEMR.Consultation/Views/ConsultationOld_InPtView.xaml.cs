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
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class ConsultationOld_InPtView : UserControl
    {
        public ConsultationOld_InPtView()
        {
            InitializeComponent();
        }

        private void AxAutoComplete_TextChanged(object sender, RoutedEventArgs e)
        {
            var a = (TextBox)sender;
            a.Text = a.Text.ToUpper();
            a.SelectionStart = a.Text.Length;
            a.SelectionLength = 0;
        }

        private void AxAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            var a = (AxAutoComplete)sender;
            a.Text = a.Text.ToUpper();
        }

        
    }
}
