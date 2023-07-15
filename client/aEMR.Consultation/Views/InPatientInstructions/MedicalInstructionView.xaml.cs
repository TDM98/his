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

namespace eHCMS.Registration.Views
{
    public partial class MedicalInstructionView : UserControl, IMedicalInstructionView
    {
        public MedicalInstructionView()
        {
            InitializeComponent();
        }
        public void SetActiveTab(InPatientRegistrationViewTab activeTab)
        {
            tabBillingInvoiceInfo.SelectedIndex = (int)activeTab;
        }

        public void ShowDeleteColumn(bool bShow)
        {
            if (bShow)
            {
                grid.Columns[0].Visibility = Visibility.Visible;
            }
            else
            {
                grid.Columns[0].Visibility = Visibility.Collapsed;
            }
        }

        //KMx: Lúc nào cũng cho hiện cột Tính BH
        public void ShowHiAppliedColumn(bool bShow)
        {
            //    if (bShow)
            //    {
            //        grid.Columns[1].Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        grid.Columns[1].Visibility = Visibility.Collapsed;
            //    }
        }

        private void grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            MedRegItemBase objRows = e.Row.DataContext as MedRegItemBase;

            if (objRows is OutwardDrugClinicDept)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.White);
            }
        }

    }
}
