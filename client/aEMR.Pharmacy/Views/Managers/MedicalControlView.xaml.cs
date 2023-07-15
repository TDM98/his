using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Pharmacy.Views
{
    public partial class MedicalControlView : UserControl
    {
        public MedicalControlView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MedicalControlView_Loaded);
            this.Unloaded += new RoutedEventHandler(MedicalControlView_Unloaded);
        }

        public void MedicalControlView_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        public void MedicalControlView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, null);
            }
        }
        public event EventHandler DoubleClick;

    }
}
