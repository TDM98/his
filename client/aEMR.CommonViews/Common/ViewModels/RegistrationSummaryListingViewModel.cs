using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.Common;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRegistrationSummaryListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationSummaryListingViewModel : Conductor<object>, IRegistrationSummaryListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RegistrationSummaryListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            aucHoldConsultDoctor=Globals.GetViewModel<IAucHoldConsultDoctor>();

            ResetSearchCriteria();

            regisStatus = Globals.GetViewModel<IEnumListing>();
            regisStatus.EnumType = typeof(AllLookupValues.RegistrationStatus);
            regisStatus.AddSelectedAllItem = true;
            regisStatus.AddSelectOneItem = false;
            regisStatus.LoadData();
            regisStatus.SelectionChange += new EventHandler(regisStatus_SelectionChange);


            regisType = Globals.GetViewModel<IEnumListing>();
            regisType.EnumType = typeof(AllLookupValues.RegistrationType);
            regisType.AddSelectedAllItem = true;
            regisType.AddSelectOneItem = false;
            regisType.LoadData();
            regisType.SelectionChange += new EventHandler(regisType_SelectionChange);

            regisPaymentStatus = Globals.GetViewModel<IEnumListing>();
            regisPaymentStatus.EnumType = typeof(AllLookupValues.RegistrationPaymentStatus);
            regisPaymentStatus.AddSelectedAllItem = true;
            regisPaymentStatus.AddSelectOneItem = false;
            regisPaymentStatus.LoadData();
            regisPaymentStatus.SelectionChange += new EventHandler(regisPaymentStatus_SelectionChange);

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoadStaffHaveRegistrationList());

                //LoadRegStatusList();
                //LoadRegTypeList();
                

                LoadPaymentStatusList();
            }



            RegistrationSummaryList = new PagedSortableCollectionView<RegistrationSummaryInfo>();
            RegistrationSummaryList.OnRefresh += new WeakEventHandler<aEMR.Common.Collections.RefreshEventArgs>(RegistrationSummaryList_OnRefresh).Handler;

        }

        void regisPaymentStatus_SelectionChange(object sender, EventArgs e)
        {
            SearchCriteria.PaymentStatus = regisPaymentStatus.SelectedItem.EnumItem == null ? null :
                (long)(AllLookupValues.RegistrationPaymentStatus)regisPaymentStatus.SelectedItem.EnumItem as long?;
        }

        void regisType_SelectionChange(object sender, EventArgs e)
        {
            SearchCriteria.RegType = regisType.SelectedItem.EnumItem==null? null:
                (long)(AllLookupValues.RegistrationType)regisType.SelectedItem.EnumItem as long?;
        }

        void regisStatus_SelectionChange(object sender, EventArgs e)
        {
            SearchCriteria.RegStatus = regisType.SelectedItem.EnumItem ==null? null:
                (long)(AllLookupValues.RegistrationStatus)regisStatus.SelectedItem.EnumItem as long?;
        }
        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private SeachPtRegistrationCriteria _criteria;
        public void RegistrationSummaryList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            Search(RegistrationSummaryList.PageIndex, RegistrationSummaryList.PageSize, false);
        }
        public void StartSearching()
        {
            RegistrationSummaryList.PageIndex = 0;
            _criteria = _searchCriteria.DeepCopy();
            _criteria.StaffID = aucHoldConsultDoctor.StaffID;

            if (_criteria.StaffID < 1)
            {
                aucHoldConsultDoctor.setDefault();
            }
            Search(0, RegistrationSummaryList.PageSize, true);
        }

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IEnumListing _regisType;
        public IEnumListing regisType
        {
            get
            {
                return _regisType;
            }
            set
            {
                if (_regisType != value)
                {
                    _regisType = value;
                    NotifyOfPropertyChange(() => regisType);
                }
            }
        }

        private IEnumListing _regisStatus;
        public IEnumListing regisStatus
        {
            get
            {
                return _regisStatus;
            }
            set
            {
                if (_regisStatus != value)
                {
                    _regisStatus = value;
                    NotifyOfPropertyChange(() => regisStatus);
                }
            }
        }

        private IEnumListing _regisPaymentStatus;
        public IEnumListing regisPaymentStatus
        {
            get
            {
                return _regisPaymentStatus;
            }
            set
            {
                if (_regisPaymentStatus != value)
                {
                    _regisPaymentStatus = value;
                    NotifyOfPropertyChange(() => regisPaymentStatus);
                }
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedItem = eventArgs.Value as RegistrationSummaryInfo;

            //var detailsViewVm = Globals.GetViewModel<IRegistrationFullDetailsView>();
            //detailsViewVm.LoadRegistrationById(SelectedItem);

            //Globals.ShowDialog(detailsViewVm as Conductor<object>);

            Action<IRegistrationFullDetailsView> onInitDlg = (detailsViewVm) =>
            {
                detailsViewVm.LoadRegistrationById(SelectedItem);
            };

            GlobalsNAV.ShowDialog<IRegistrationFullDetailsView>(onInitDlg);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(()=>IsLoading);
                NotifyOfPropertyChange(() => CanSearchCmd);
            }
        }

        private RegistrationsTotalSummary _totalSummary;
        public RegistrationsTotalSummary TotalSummary
        {
            get { return _totalSummary; }
            set { _totalSummary = value;
            NotifyOfPropertyChange(()=>TotalSummary);}
        }

        private RegistrationSummaryInfo _selectedItem;

        public RegistrationSummaryInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        private SeachPtRegistrationCriteria _searchCriteria;

        public SeachPtRegistrationCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(()=>SearchCriteria);
            }
        }

        private PagedSortableCollectionView<RegistrationSummaryInfo> _registrationSummaryList;

        public PagedSortableCollectionView<RegistrationSummaryInfo> RegistrationSummaryList
        {
            get { return _registrationSummaryList; }
            private set
            {
                _registrationSummaryList = value;
                NotifyOfPropertyChange(() => RegistrationSummaryList);
                NotifyOfPropertyChange(() => SummaryTitle);
                NotifyOfPropertyChange(() => ShowSummaryPanel);
            }
        }

        public string SummaryTitle
        {
            get
            {
                if(RegistrationSummaryList != null)
                {
                    return string.Format("Tổng kết: ({0} đăng ký)", RegistrationSummaryList.TotalItemCount);
                }
                return string.Format("{0}:", eHCMSResources.G1523_G1_TKet);
            }
        }

        //private ObservableCollection<Lookup> _appointmentStatusList;
        //public ObservableCollection<Lookup> AppointmentStatusList
        //{
        //    get { return _appointmentStatusList; }
        //    set
        //    {
        //        _appointmentStatusList = value;
        //        NotifyOfPropertyChange(() => AppointmentStatusList);
        //    }
        //}
        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return _staffs; }
            set
            {
                _staffs = value;
                NotifyOfPropertyChange(() => Staffs);
            }
        }
        private ObservableCollection<Lookup> _regStatusList;
        public ObservableCollection<Lookup> RegStatusList
        {
            get { return _regStatusList; }
            set
            {
                _regStatusList = value;
                NotifyOfPropertyChange(() => RegStatusList);
            }
        }

        private ObservableCollection<Lookup> _regTypeList;
        public ObservableCollection<Lookup> RegTypeList
        {
            get { return _regTypeList; }
            set
            {
                _regTypeList = value;
                NotifyOfPropertyChange(() => RegTypeList);
            }
        }

        private ObservableCollection<Lookup> _paymentStatusList;
        public ObservableCollection<Lookup> PaymentStatusList
        {
            get { return _paymentStatusList; }
            set
            {
                _paymentStatusList = value;
                NotifyOfPropertyChange(() => PaymentStatusList);
            }
        }


        private IEnumerator<IResult> LoadStaffHaveRegistrationList()
        {
            var paymentTypeTask = new LoadStaffHaveRegistrationListTask(false, true);
            yield return paymentTypeTask;
            Staffs = paymentTypeTask.StaffList;
            yield break;
        }


        public void LoadRegStatusList()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0124_G1_DangLayDSTThaiDK)
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.REGISTRATION_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                                RegStatusList = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                RegStatusList.Insert(0, firstItem);

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void LoadRegTypeList()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Đang lấy danh sách loại đăng ký..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RegistrationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                                RegTypeList = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                RegTypeList.Insert(0, firstItem);

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void LoadPaymentStatusList()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Đang lấy danh sách trạng thái thanh toán..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.REGISTRATION_PAYMENT_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                                PaymentStatusList = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                PaymentStatusList.Insert(0, firstItem);

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        private void Search(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2882_G1_DangTaiDLieu) });
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationSummaryList(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            List<RegistrationSummaryInfo> allItems = null;
                            RegistrationsTotalSummary totalSummary = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationSummaryList(out totalCount, out totalSummary, asyncResult);
                                TotalSummary = totalSummary;
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                bOK = false;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                bOK = false;
                            }
                            
                            RegistrationSummaryList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    RegistrationSummaryList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        RegistrationSummaryList.Add(item);
                                    }

                                }
                            }
                            NotifyOfPropertyChange(() => SummaryTitle);
                            NotifyOfPropertyChange(() => ShowSummaryPanel);
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    RegistrationSummaryList.Clear();
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public bool CanSearchCmd
        {
            get { return !_isLoading; }
        }
        public void SearchCmd()
        {
            StartSearching();
        }
        public bool CanResetFilterCmd
        {
            get { return true; }
        }
        public void ResetFilterCmd()
        {
            ResetSearchCriteria();
        }
        private void ResetSearchCriteria()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.FromDate = Globals.ServerDate.Value;
            SearchCriteria.ToDate = Globals.ServerDate.Value;
            SearchCriteria.StaffID = -1;
            SearchCriteria.RegStatus = -1;
            SearchCriteria.RegType = -1;
            SearchCriteria.PaymentStatus = -1;
        }

        public bool ShowSummaryPanel
        {
            get
            {
                return RegistrationSummaryList != null && RegistrationSummaryList.Count > 0;
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mTongKet_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegSummary,
                                               (int)oRegistrionEx.mTongKet_Xem, (int)ePermission.mView);
        }
        #region checking account

        private bool _mTongKet_Xem = true;

        public bool mTongKet_Xem
        {
            get
            {
                return _mTongKet_Xem;
            }
            set
            {
                if (_mTongKet_Xem == value)
                    return;
                _mTongKet_Xem = value;
                NotifyOfPropertyChange(() => mTongKet_Xem);
            }
        }


        #endregion
    }
}
