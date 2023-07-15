using aEMR.Controls;
using aEMR.ViewContracts;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Common.Views
{
    public partial class PatientDetailsAndHIView : AxUserControl, IPatientDetailsAndHIView
    {
        bool _firstFocus = true;
        public PatientDetailsAndHIView()
        {
            InitializeComponent();

            this.GotFocus += new RoutedEventHandler(PatientDetailsAndHIView_GotFocus);
        }

        void PatientDetailsAndHIView_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_firstFocus)
            {
                FocusOnFirstItem();
                this.GotFocus -= new RoutedEventHandler(PatientDetailsAndHIView_GotFocus);
            }
        }

        public void FocusOnFirstItem()
        {
            txtFullName.Focus();
            _firstFocus = false;
        }

        public DateTime? DateBecamePatient
        {
            get
            {
                CultureInfo culture = new CultureInfo("vi-VN");
                DateTime? date = null;
                try
                {
                    date = DateTime.Parse(txtDateBecamePatient.Text, culture);
                }
                catch
                {
                }
                return date;
            }
        }

        private void txtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtRegistrationCode_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}