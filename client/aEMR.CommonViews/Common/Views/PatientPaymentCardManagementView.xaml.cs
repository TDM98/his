using aEMR.ViewContracts;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace aEMR.Common.Views
{
    public partial class PatientPaymentCardManagementView : UserControl, IPatientPaymentCardManagementView
    {
        public PatientPaymentCardManagementView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PatientPaymentCardManagementView_Loaded);
        }

        void PatientPaymentCardManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding editModeBinding = new Binding();
            editModeBinding.Mode = BindingMode.OneWay;
            editModeBinding.Path = new PropertyPath("IsEditMode");
        }
    }
}
