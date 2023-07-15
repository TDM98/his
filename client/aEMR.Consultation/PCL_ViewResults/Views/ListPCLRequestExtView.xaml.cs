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
using DataEntities;
using aEMR.ConsultantEPrescription.PCL_ViewResults.ViewModels;


namespace aEMR.ConsultantEPrescription.PCL_ViewResults.Views
{
    public partial class ListPCLRequestExtView : UserControl, IListPCLRequest_CommonView
    {
        public ListPCLRequestExtView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }


        public void ShowHide_dtgDetailColumns()
        {
           
        }

        private void dtgListDetail_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
        }

    }
}
