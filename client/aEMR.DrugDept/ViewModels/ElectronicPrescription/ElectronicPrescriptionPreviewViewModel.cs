using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.DrugDept.ViewModels.ElectronicPrescription
{
    [Export(typeof(IElectronicPrescriptionPreview)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ElectronicPrescriptionPreviewViewModel : ViewModelBase, IElectronicPrescriptionPreview
    {
        [ImportingConstructor]
        public ElectronicPrescriptionPreviewViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        //private DataSet _PreviewHIReportSet;
        //private DataTable _DataTable1;
        //private DataTable _DataTable2;
        //private DataTable _DataTable3;
        //private DataTable _DataTable4;
        //private DataTable _DataTable5;
        //private string _ErrText;
        //public DataSet PreviewHIReportSet
        //{
        //    get => _PreviewHIReportSet; set
        //    {
        //        _PreviewHIReportSet = value;
        //        NotifyOfPropertyChange(() => PreviewHIReportSet);
        //    }
        //}
        //public DataTable DataTable1
        //{
        //    get => _DataTable1; set
        //    {
        //        _DataTable1 = value;
        //        NotifyOfPropertyChange(() => DataTable1);
        //    }
        //}
        //public DataTable DataTable2
        //{
        //    get => _DataTable2; set
        //    {
        //        _DataTable2 = value;
        //        NotifyOfPropertyChange(() => DataTable2);
        //    }
        //}
        //public DataTable DataTable3
        //{
        //    get => _DataTable3; set
        //    {
        //        _DataTable3 = value;
        //        NotifyOfPropertyChange(() => DataTable3);
        //    }
        //}
        //public DataTable DataTable4
        //{
        //    get => _DataTable4; set
        //    {
        //        _DataTable4 = value;
        //        NotifyOfPropertyChange(() => DataTable4);
        //    }
        //}
        //public DataTable DataTable5
        //{
        //    get => _DataTable5; set
        //    {
        //        _DataTable5 = value;
        //        NotifyOfPropertyChange(() => DataTable5);
        //    }
        //}

        //public string ErrText
        //{
        //    get => _ErrText; set
        //    {
        //        _ErrText = value;
        //        NotifyOfPropertyChange(() => ErrText);
        //    }
        //}

        //public void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText)
        //{
        //    if (aPreviewHIReportSet == null || aPreviewHIReportSet.Tables == null || aPreviewHIReportSet.Tables.Count != 3)
        //    {
        //        if (DataTable1 != null)
        //        {
        //            DataTable1.Rows.Clear();
        //            DataTable2.Rows.Clear();
        //            //DataTable3.Rows.Clear();
        //            //DataTable4.Rows.Clear();
        //            //DataTable5.Rows.Clear();
        //        }
        //    }
        //    DataTable1 = aPreviewHIReportSet.Tables[0];
        //    DataTable2 = aPreviewHIReportSet.Tables[1];
        //    //DataTable3 = aPreviewHIReportSet.Tables[2];
        //    //DataTable4 = aPreviewHIReportSet.Tables[3];
        //    //DataTable5 = aPreviewHIReportSet.Tables[4];
        //    ErrText = aErrText;
        //}
        private List<DTDT_don_thuoc> _ListDTDT_don_thuoc;
        public List<DTDT_don_thuoc> ListDTDT_don_thuoc
        {
            get => _ListDTDT_don_thuoc;
            set
            {
                _ListDTDT_don_thuoc = value;
                NotifyOfPropertyChange(() => ListDTDT_don_thuoc);
            }
        }
        public void grd_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void Expander_Process(object sender)
        {
            if (sender == null)
            {
                return;
            }
            var row = DataGridRow.GetRowContainingElement((FrameworkElement)sender);
            if (row != null)
            {
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}