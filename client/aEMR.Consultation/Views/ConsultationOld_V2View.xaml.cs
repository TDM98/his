﻿using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class ConsultationOld_V2View : UserControl
    {
        public ConsultationOld_V2View()
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