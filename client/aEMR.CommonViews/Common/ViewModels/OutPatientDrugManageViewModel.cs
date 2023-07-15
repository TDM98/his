/*
 * 20140803 #001 CMN: Add HI Store Service
 * 20181114 #002 TTM: BM 0005259: Chuyển CellEditEnded -> CellEditEnding.
*/
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.ViewContracts.GuiContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Linq;

namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Quản lý danh sách CLS của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    [Export(typeof(IOutPatientDrugManage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientDrugManageViewModel : Conductor<object>, IOutPatientDrugManage
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OutPatientDrugManageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        IOutPatientDrugManageView _currentView;
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _currentView = view as IOutPatientDrugManageView;
            if (_currentView != null)
            {
                _currentView.SetVisibilityBindingForHiColumns();
                //==== #001
                _currentView.SetVisibilityBindingForHIOutPtColumns();
                //==== #001
            }
        }
        #region PROPERTIES
        private aEMR.Common.PagedCollectionView.PagedCollectionView _drugItems;
        public aEMR.Common.PagedCollectionView.PagedCollectionView DrugItems
        {
            get { return _drugItems; }
            set
            {
                if (_drugItems != value)
                {
                    _drugItems = value;
                    NotifyOfPropertyChange(() => DrugItems);
                }
            }
        }

        private bool _hiServiceBeingUsed;
        public bool HiServiceBeingUsed
        {
            get { return _hiServiceBeingUsed; }
            set
            {
                if (_hiServiceBeingUsed != value)
                {
                    _hiServiceBeingUsed = value;
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);
                    if (_currentView != null)
                    {
                        _currentView.SetVisibilityBindingForHiColumns();
                    }
                }
            }
        }
        //==== #001
        private bool _IsHIOutPt = false;
        public bool IsHIOutPt
        {
            get { return _IsHIOutPt; }
            set
            {
                if (_IsHIOutPt != value)
                {
                    _IsHIOutPt = value;
                    NotifyOfPropertyChange(() => IsHIOutPt);
                }
            }
        }
        DataGrid gridDrugs;
        public void gridDrugs_Loaded(object sender, RoutedEventArgs e)
        {
            gridDrugs = (sender as DataGrid);
            if (!IsHIOutPt)
            {
                gridDrugs.Columns.Where(x => x is DataGridCheckBoxColumn).FirstOrDefault().Visibility = Visibility.Collapsed;
                gridDrugs.Columns.Where(x => x is DataGridTemplateColumn).FirstOrDefault().Visibility = Visibility.Collapsed;
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IsHIOutPt)
            {
                Globals.EventAggregator.Publish(new DeletedOutwardDrug { DeleteOutwardDrug = DrugItems.CurrentItem as OutwardDrug });
            }
        }
        //▼====== #002
        //public void gridDrugs_CellEditEnded(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        //{
        //    if (IsHIOutPt)
        //    {
        //        Globals.EventAggregator.Publish(new ChangedQtyOutwardDrug { DeleteOutwardDrug = DrugItems.CurrentItem as OutwardDrug });
        //    }
        //}
        public void gridDrugs_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if (IsHIOutPt)
            {
                Globals.EventAggregator.Publish(new ChangedQtyOutwardDrug { DeleteOutwardDrug = DrugItems.CurrentItem as OutwardDrug });
            }
        }
        //▲====== #002
        //==== #001
        #endregion
    }
}
