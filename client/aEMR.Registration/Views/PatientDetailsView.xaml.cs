﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Registration.Views
{
    public partial class PatientDetailsView : AxUserControl, IPatientDetailsView
    {
        bool _firstFocus = true;
        public PatientDetailsView()
        {
            InitializeComponent();

            this.GotFocus += new RoutedEventHandler(PatientDetailsView_GotFocus);
            
        }

        void PatientDetailsView_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_firstFocus)
            {
                FocusOnFirstItem();
                this.GotFocus -= new RoutedEventHandler(PatientDetailsView_GotFocus);
            }
        }
        public void FocusOnFirstItem()
        {
            if (tabPatientInfo.SelectedIndex == (int)PatientInfoTabs.GENERAL_INFO)
            {
                txtFullName.Focus();
            }
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

        /// <summary>
        /// Lay nhung gia tri tren form gan lai voi datasource
        /// </summary>
        public void UpdateGeneralInfoToSource()
        {
            txtFullName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtAge.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtDateOfBirth.GetBindingExpression(AxDateTextBox.DateProperty).UpdateSource();
            cboGender.GetBindingExpression(AxComboBox.SelectedItemExProperty).UpdateSource();
            cboMaritalStatus.GetBindingExpression(AxComboBox.SelectedValueExProperty).UpdateSource();
            txtOccupation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtEmployee.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cboEthnic.GetBindingExpression(AxComboBox.SelectedValueExProperty).UpdateSource();
            txtStreetAddress.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtSurburb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cboCity.GetBindingExpression(AxComboBox.SelectedValueExProperty).UpdateSource();
            cboCountry.GetBindingExpression(AxComboBox.SelectedValueExProperty).UpdateSource();
            txtPhoneNo.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtCellPhone.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtEmailAddress.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            cboContactRelationship.GetBindingExpression(AxComboBox.SelectedValueExProperty).UpdateSource();
            txtContactFullName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactAddress.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactHomePhone.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactBizPhone.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactCellPhone.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactAlternateContact.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtContactAlternatePhone.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtPatientNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
        public void SetActiveTab(PatientInfoTabs activeTab)
        {
            tabPatientInfo.SelectedIndex = (int)activeTab;
        }
    }
}
