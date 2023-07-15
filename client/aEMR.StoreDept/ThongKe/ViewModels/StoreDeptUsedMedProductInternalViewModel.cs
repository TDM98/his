using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.BaseModel;

namespace aEMR.StoreDept.ThongKe.ViewModels
{
    [Export(typeof(IStoreDeptUsedMedProductInternal)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreDeptUsedMedProductInternalViewModel : ViewModelBase, IStoreDeptUsedMedProductInternal
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StoreDeptUsedMedProductInternalViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            OutwardDrugClinicDepts = new PagedSortableCollectionView<OutwardDrugClinicDept>();
            OutwardDrugClinicDepts.OnRefresh += new EventHandler<RefreshEventArgs>(OutwardDrugClinicDepts_OnRefresh);
            OutwardDrugClinicDepts.PageSize = Globals.PageSize;

            //Coroutine.BeginExecute(DoGetStore_MedDept());
           

            SearchCriteria = new SearchOutwardInfo();
        }

        void OutwardDrugClinicDepts_OnRefresh(object sender, RefreshEventArgs e)
        {
            OutwardDrugClinicDeptInvoice_Search(OutwardDrugClinicDepts.PageIndex, OutwardDrugClinicDepts.PageSize);
        }
        protected override void OnActivate()
        {
            //khi nao share thi dung cai nay
            StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, true);
            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
        }
        protected override void OnDeactivate(bool close)
        {
            OutwardDrugClinicDepts = null;
          
            Globals.EventAggregator.Unsubscribe(this);

        }
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private decimal _TotalOutAmount = 0;
        public decimal TotalOutAmount
        {
            get { return _TotalOutAmount; }
            set
            {
                if(_TotalOutAmount != value)
                {
                    _TotalOutAmount = value;
                    NotifyOfPropertyChange(() => TotalOutAmount);
                }
            }
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account
        private bool _mThongKe_xem = true;
        private bool _mThongKe_PhieuMoi = true;
        private bool _mThongKe_XemIn = true;
        private bool _mThongKe_In = true;

        public bool mThongKe_xem
        {
            get
            {
                return _mThongKe_xem;
            }
            set
            {
                if (_mThongKe_xem == value)
                    return;
                _mThongKe_xem = value;
                NotifyOfPropertyChange(() => mThongKe_xem);
            }
        }

        public bool mThongKe_PhieuMoi
        {
            get
            {
                return _mThongKe_PhieuMoi;
            }
            set
            {
                if (_mThongKe_PhieuMoi == value)
                    return;
                _mThongKe_PhieuMoi = value;
                NotifyOfPropertyChange(() => mThongKe_PhieuMoi);
            }
        }

        public bool mThongKe_XemIn
        {
            get
            {
                return _mThongKe_XemIn;
            }
            set
            {
                if (_mThongKe_XemIn == value)
                    return;
                _mThongKe_XemIn = value;
                NotifyOfPropertyChange(() => mThongKe_XemIn);
            }
        }

        public bool mThongKe_In
        {
            get
            {
                return _mThongKe_In;
            }
            set
            {
                if (_mThongKe_In == value)
                    return;
                _mThongKe_In = value;
                NotifyOfPropertyChange(() => mThongKe_In);
            }
        }
        
        #endregion

        #region Propeties Member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private PagedSortableCollectionView<OutwardDrugClinicDept> _OutwardDrugClinicDepts;
        public PagedSortableCollectionView<OutwardDrugClinicDept> OutwardDrugClinicDepts
        {
            get
            {
                return _OutwardDrugClinicDepts;
            }
            set
            {
                if (_OutwardDrugClinicDepts != value)
                {
                    _OutwardDrugClinicDepts = value;
                    NotifyOfPropertyChange(() => OutwardDrugClinicDepts);
                }
            }
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null,false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void OutwardDrugClinicDeptInvoice_Search(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugClinicDeptInvoices_SearchTKPaging(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                         {

                             try
                             {
                                 int TotalCount = 0;
                                 TotalOutAmount = 0;
                                 var results = contract.EndOutwardDrugClinicDeptInvoices_SearchTKPaging(out TotalCount, asyncResult);
                                 OutwardDrugClinicDepts.Clear();
                                 OutwardDrugClinicDepts.TotalItemCount = TotalCount;
                                 if (results != null)
                                 {
                                     foreach (OutwardDrugClinicDept p in results)
                                     {
                                         OutwardDrugClinicDepts.Add(p);
                                         TotalOutAmount += p.OutAmount.Value;
                                     }

                                 }
                             }
                             catch (Exception ex)
                             {
                                 Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 _logger.Error(ex.Message);
                             }
                             finally
                             {
                                 IsLoading = false;
                                 //Globals.IsBusy = false;
                                 this.DlgHideBusyIndicator();
                             }

                         }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        #endregion

        public void btnSearch()
        {
            OutwardDrugClinicDepts.PageIndex = 0;
            OutwardDrugClinicDeptInvoice_Search(OutwardDrugClinicDepts.PageIndex, OutwardDrugClinicDepts.PageSize);
        }
    }
}
