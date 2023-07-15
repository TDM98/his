using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.ViewModels;

namespace aEMR.Common.Views
{
    public partial class ListPCLRequest_CommonView : UserControl, IListPCLRequest_CommonView
    {
        public ListPCLRequest_CommonView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dtgListDetail.Columns[1].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateThucHien_Visible;
            dtgListDetail.Columns[2].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateNgung_Visible;
            //dtgListDetail.Columns[3].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateKetQua_Visible;
            dtgListDetail.Columns[3].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateInputKetQua_Visible;
        }


        public void ShowHide_dtgDetailColumns()
        {
            dtgListDetail.Columns[1].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateThucHien_Visible;
            dtgListDetail.Columns[2].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateNgung_Visible;
            //dtgListDetail.Columns[3].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateKetQua_Visible;
            dtgListDetail.Columns[3].Visibility = (this.DataContext as ListPCLRequest_CommonViewModel).dtgCellTemplateInputKetQua_Visible;
        }

        //private void dtgListDetail_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    PatientPCLRequestDetail objRows = e.Row.DataContext as PatientPCLRequestDetail;
        //    if (objRows != null)
        //    {
        //        Button CtrhplKetQua = dtgListDetail.Columns[3].GetCellContent(e.Row) as Button;
        //        //ChangedWPF-CMN: Added condition check null on CtrhplKetQua
        //        if (CtrhplKetQua != null)
        //        {
        //            CtrhplKetQua.IsEnabled = objRows.HasResult == null ? false : objRows.HasResult.Value;
        //            if (CtrhplKetQua.IsEnabled == false)
        //            {
        //                CtrhplKetQua.Foreground = new SolidColorBrush(Colors.Gray);
        //            }

        //            Button CtrhplThucHien = dtgListDetail.Columns[1].GetCellContent(e.Row) as Button;
        //            CtrhplThucHien.IsEnabled = !CtrhplKetQua.IsEnabled;

        //            Button CtrhplNgung = dtgListDetail.Columns[1].GetCellContent(e.Row) as Button;
        //            CtrhplNgung.IsEnabled = !CtrhplKetQua.IsEnabled;
        //        }
        //    }
        //}
    }
}
