using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Windows.Media;
using aEMR.ViewContracts;

namespace aEMR.Registration.Views
{
    public partial class InPatientRegistrationSummaryView : UserControl
    {
        public InPatientRegistrationSummaryView()
        {
            InitializeComponent();
        }
        public void SetActiveTab(InPatientRegistrationViewTab activeTab)
        {
            //tabBillingInvoiceInfo.SelectedIndex = (int)activeTab;
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
                //▼====: #001
                try
                {
                    if (objRows is PatientPCLRequestDetail)
                    {
                        var PCLRequestDetail = (objRows as PatientPCLRequestDetail);
                        if (PCLRequestDetail != null && PCLRequestDetail.DoctorStaff != null && PCLRequestDetail.DoctorStaff.StaffCatgID != 1)
                            e.Row.Foreground = new SolidColorBrush(Colors.Red);
                        else
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
                catch { }
                //▲====: #001
            }
        }
    }
}
