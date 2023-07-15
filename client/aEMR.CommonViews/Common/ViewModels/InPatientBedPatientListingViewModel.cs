using eHCMSLanguage;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Linq;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts.Configuration;
using System;
/*
* 20210921 #001 TNHX: Thêm cấu hình giường tự động
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientBedPatientAllocListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientBedPatientListingViewModel : Conductor<object>, IInPatientBedPatientAllocListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientBedPatientListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            if (Globals.ServerConfigSection.CommonItems.AutoAddBedService)
            {
                AutoAddBedService = Visibility.Collapsed;
            }
            else
            {
                AutoAddBedService = Visibility.Visible;
            }
        }

        //▼====: #001
        private Visibility _AutoAddBedService = Visibility.Collapsed;
        public Visibility AutoAddBedService
        {
            get
            {
                return _AutoAddBedService;
            }
            set
            {
                _AutoAddBedService = value;
                NotifyOfPropertyChange(() => AutoAddBedService);
            }
        }
        //▲====: #001

        private bool _showDeleteColumn;
        public bool ShowDeleteColumn
        {
            get
            {
                return _showDeleteColumn;
            }
            set
            {
                _showDeleteColumn = value;
                NotifyOfPropertyChange(() => ShowDeleteColumn);
            }
        }

        private bool _showReturnBedColumn;
        public bool ShowReturnBedColumn
        {
            get
            {
                return _showReturnBedColumn;
            }
            set
            {
                _showReturnBedColumn = value;
                NotifyOfPropertyChange(() => ShowReturnBedColumn);
            }
        }


        private ObservableCollection<BedPatientAllocs> _allItems;

        public ObservableCollection<BedPatientAllocs> AllItems
        {
            get { return _allItems; }
            set { _allItems = value;
            NotifyOfPropertyChange(() => AllItems);
            }
        }

        private PatientRegistration _registration;

        public PatientRegistration Registration
        {
            get { return _registration; }
            set 
            {
                _registration = value;
                NotifyOfPropertyChange(() => Registration);
                AllItems = _registration == null ? null : _registration.BedAllocations;
            }
        }

        public void LoadData()
        {
            
        }

        private bool _isEdit = true;
        public bool isEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                NotifyOfPropertyChange(() => isEdit);
            }
        }

        public bool CheckResponsibility(long refDepartmentID)
        {
            if (!Globals.isAccountCheck)
            {
                return true;
            }

            if (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0 || refDepartmentID <= 0)
            {
                return false;
            }

            var result = Globals.LoggedUserAccount.DeptIDResponsibilityList.Where(x => x == refDepartmentID).ToList();

            if (result == null || result.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public void RemoveBedAllocItem(BedPatientAllocs datacontext, object eventArgs)
        {
            if (datacontext == null || datacontext.ResponsibleDeptID <= 0)
            {
                return;
            }

            if (!CheckResponsibility(datacontext.ResponsibleDeptID))
            {
                MessageBox.Show(eHCMSResources.A0106_G1_Msg_InfoKhTheXoaGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                Globals.EventAggregator.Publish(new RemoveItem<BedPatientAllocs, object> { Item = datacontext, Source = this });
            }
        }

        public void ReturnBedAllocItem(BedPatientAllocs datacontext, object eventArgs)
        {
            if (datacontext == null || datacontext.ResponsibleDeptID <= 0)
            {
                return;
            }

            if (!CheckResponsibility(datacontext.ResponsibleDeptID))
            {
                MessageBox.Show(eHCMSResources.A0105_G1_Msg_InfoKhTheTraGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                Globals.EventAggregator.Publish(new ReturnItem<BedPatientAllocs, object> { Item = datacontext, Source = this });
            }
        }

        public void ReturnBedLoaded(object sender)
        {
            ((Button)sender).Visibility = Globals.convertVisibility(isEdit);
        }

        DataGrid grid = null;
        public void grid_Loaded(object sender, RoutedEventArgs e)
        {
            grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            var colDelete = grid.GetColumnByName("colDelete");

            var colReturnBed = grid.GetColumnByName("colReturnBed");

            if (colDelete == null || colReturnBed == null)
            {
                return;
            }

            if (ShowDeleteColumn)
            {
                colDelete.Visibility = Visibility.Visible;
            }
            else
            {
                colDelete.Visibility = Visibility.Collapsed;
            }

            if (ShowReturnBedColumn && AutoAddBedService == Visibility.Visible)
            {
                colReturnBed.Visibility = Visibility.Visible;
            }
            else
            {
                colReturnBed.Visibility = Visibility.Collapsed;
            }
        }

        private InPatientDeptDetail _InPtDeptDetail;
        public InPatientDeptDetail InPtDeptDetail
        {
            get { return _InPtDeptDetail; }
            set
            {
                if (_InPtDeptDetail != value)
                {
                    _InPtDeptDetail = value;
                    NotifyOfPropertyChange(() => InPtDeptDetail);
                }
            }
        }

        public void EditBedAllocItem(BedPatientAllocs datacontext, object eventArgs)
        {
            if (datacontext == null || datacontext.ResponsibleDeptID <= 0)
            {
                return;
            }

            if (!CheckResponsibility(datacontext.ResponsibleDeptID))
            {
                MessageBox.Show(eHCMSResources.A0105_G1_Msg_InfoKhTheTraGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatientCommon>();

                cwdBedPatientVM.InPtDeptDetail = InPtDeptDetail;

                cwdBedPatientVM.selectedBedPatientAllocs = datacontext;
                cwdBedPatientVM.selectedBedPatientAllocs.VPtRegistration = Registration;
                cwdBedPatientVM.selectedTempBedPatientAllocs = datacontext;
                cwdBedPatientVM.selectedTempBedPatientAllocs.VPtRegistration = Registration;
                cwdBedPatientVM.IsEdit = true;
                DeptMedServiceItemsSearchCriteria SearchCriteria = new DeptMedServiceItemsSearchCriteria
                {
                    DeptID = datacontext.ResponsibleDeptID
                };
                cwdBedPatientVM.GetBedAllocationAll_ByDeptID(SearchCriteria);
                ActivateItem(cwdBedPatientVM);

                if (cwdBedPatientVM != null)
                {
                    cwdBedPatientVM.eFireBookingBedEventTo = eFireBookingBedEvent.NONE;
                    GlobalsNAV.ShowDialog<IcwdBedPatientCommon>();
                }
            }
        }        
    }
}
