using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    public partial class PatientRegistrationView : UserControl,IPatientRegistrationView
    {
        public PatientRegistrationView()
        {
            InitializeComponent();
        }

        //private void gridPCLRequests_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        //}

        //private void gridServices_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        //}

        public void ResetView()
        {
            //tabRegistrationInfo.SelectedIndex = 0;
        }
    }
}
