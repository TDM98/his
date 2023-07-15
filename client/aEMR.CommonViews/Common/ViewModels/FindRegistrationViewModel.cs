using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Controls;
/*
 * 20180904 #001 TTM:   Ngăn không cho tìm kiếm rỗng
 * 20181003 #002 TTM:   BM 0000121: Hạn chế tìm kiếm bằng tên, tránh trường hợp nhầm lẫn khi tìm bằng tên (nhiều bệnh nhân cùng tên, đăng ký khám trong một ngày)
 *                      Điều khiển bằng cấu hình tên IsAllowSearchingPtByName, giá trị hiện tại đang là False (hạn chế tìm bằng tên) 03/10/2018.
 * 20181102 #003 TTM:   BM 0005213: Cho phép tìm kiếm tất cả đăng ký trong ngày bằng cách nhập kí tự "*"
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindRegistration)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindRegistrationViewModel : ViewModelBase, IFindRegistration
        , IHandle<ItemSelected<PatientRegistration, PatientRegistrationDetail>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public FindRegistrationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            Globals.EventAggregator.Subscribe(this);
            ResetFilter();
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                LoadStaffs();
                LoadRegStatusList();
                //LoadDeptLocation(Globals.DeptLocation.DeptID);
                Coroutine.BeginExecute(DoLoadLocations(Globals.DeptLocation.DeptID));
            }
            
            Registrations = new PagedSortableCollectionView<PatientRegistration>();
            Registrations.OnRefresh += Registrations_OnRefresh;
        }

        protected override void OnActivate()
        {
            //▼====== #002: IsSearchPtByNameChecked_SameTwoView được truyền từ SearchPatientAndRegistation khi người dùng mở popup từ bên đó.
            //              Và ở màn hình đó cũng có 1 nút tìm bệnh nhân cho nên nếu ở ngoài có tick để tìm kiếm thì giá trị đó cũng phải đc truyền vào trong.
            //20181024 TNHX: [BM0002186] disable this. parent will set isSearchPtByNameChecked
            //if (IsSearchPtByNameChecked_SameTwoView)
            //{
            //    IsSearchPtByNameChecked = true;
            //}
            //▲====== #002
            if (_searchCriteria.PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU
                && _searchCriteria.KhamBenh)
            {
                isNgoaiTru = true;
                _searchCriteria.DeptID = Globals.DeptLocation.DeptID;                
                _searchCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
            }
            else 
            {
                isNgoaiTru = false;
            }

            base.OnActivate();
            //SearchCmd();
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
        //protected override void OnDeactivate(bool close)
        //{
        //    base.OnDeactivate(close);
        //    Globals.EventAggregator.Unsubscribe(this);
        //}
        private bool _IsProcessing;
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                if (_IsProcessing == value)
                {
                    return;
                }
                _IsProcessing = value;
                NotifyOfPropertyChange(() => IsProcessing);
            }
        }

        private bool _IsSearchGoToKhamBenh;
        public bool IsSearchGoToKhamBenh
        {
            get { return _IsSearchGoToKhamBenh; }
            set
            {
                _IsSearchGoToKhamBenh = value;
                NotifyOfPropertyChange(() => IsSearchGoToKhamBenh);
            }
        }

        private string _pageTitle;
        public string pageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                NotifyOfPropertyChange(() => pageTitle);
            }
        }
        void Registrations_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchRegistrations(Registrations.PageIndex, Registrations.PageSize, true);
        }
        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.RegStatus = -1;
            SearchCriteria.FromDate = Globals.ServerDate.Value;
            SearchCriteria.ToDate = Globals.ServerDate.Value;
            if (IsEnableCbx)
            {
                SearchCriteria.StaffID = -1;
            }
            else
            {
                SearchCriteria.StaffID = Globals.LoggedUserAccount.StaffID;
            }
        }
        private bool _IsEnableCbx=true;
        public bool IsEnableCbx
        {
            get { return _IsEnableCbx; }
            set
            {
                _IsEnableCbx = value;
                NotifyOfPropertyChange(()=>IsEnableCbx);
            }
        }

        public void CancelCmd()
        {
            SelectedRegistration = null;
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void OkCmd()
        {
            SelectItemAndCloseFormIfNeeded();
        }
        private void SelectItemAndCloseFormIfNeeded()
        {
            //Get Module dang lam viec ra
            if (!IsPopup)
            {
                Globals.ShowMessage("FindRegistrationViewModel. IsPopup =false", "");
            }

            var home = Globals.GetViewModel<IHome>();

            var activeItem = home.ActiveContent;

            IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;

            IConsultationModule ModuleConsult = activeItem as IConsultationModule;

            IStoreDeptHome ModuleStorageClinicDept = activeItem as IStoreDeptHome;

            ITransactionModule ModuleTransaction = activeItem as ITransactionModule;

            if (ModuleRegis != null)/*Dang dung ben Module Dang Ky*/
            {
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
            }


            if (ModuleConsult != null)
            {
                IPaperReferalFull TransferForm = ModuleConsult.MainContent as IPaperReferalFull;
                if (TransferForm != null)
                {
                    if (SelectedRegistration != null && Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NOITRU)
                    {
                        Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = SelectedRegistration, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
                    }
                }
                if (SelectedRegistration != null && Globals.PatientFindBy_ForConsultation == (long)AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
                }

                if (SelectedRegistration != null && Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NOITRU)
                {
                    Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = SelectedRegistration });
                }
            }
            else if (ModuleStorageClinicDept != null || ModuleTransaction != null)
            {
                if (SearchCriteria != null && SelectedRegistration != null)
                {
                    Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = SelectedRegistration });
                }
            }
            else
            {
                if (SelectedRegistration != null && Globals.PatientFindBy_ForConsultation == (long)AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
                }
            }
            if (CloseAfterSelection)
            {
                TryClose();
                Globals.EventAggregator.Unsubscribe(this);
            }
        }

        private bool _closeAfterSelection = true;
        public bool CloseAfterSelection
        {
            get { return _closeAfterSelection; }
            set
            {
                _closeAfterSelection = value;
                NotifyOfPropertyChange(() => CloseAfterSelection);
            }
        }

        private PatientRegistration _selectedRegistration;
        public PatientRegistration SelectedRegistration
        {
            get { return _selectedRegistration; }
            set
            {
                _selectedRegistration = value;
                NotifyOfPropertyChange(() => SelectedRegistration);
                //SelectedRegistration.
            }
        }

        private SeachPtRegistrationCriteria _searchCriteria;
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

                //get ViewModel ra coi đang đứng đâu.

                var home = Globals.GetViewModel<IHome>();

                var activeItem = home.ActiveContent;

                IConsultationModule consultModule = activeItem as IConsultationModule;
                if (consultModule != null)
                {
                    if (SearchCriteria.PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                    {
                        pageTitle = eHCMSResources.G1172_G1_TimDKNoiTruKb;
                    }
                    if (SearchCriteria.PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                    {
                        pageTitle = eHCMSResources.G1167_G1_TimDKNgTruKb;
                    }
                }
                else
                {
                    if (SearchCriteria.PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                    {
                        pageTitle = eHCMSResources.G1170_G1_TimDKNoiTruKbCLS;
                    }
                    if (SearchCriteria.PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                    {
                        pageTitle = eHCMSResources.G1165_G1_TimDKNgTruKbCLS;
                    }
                }

            }
        }

        private PagedSortableCollectionView<PatientRegistration> _registrations;
        public PagedSortableCollectionView<PatientRegistration> Registrations
        {
            get { return _registrations; }
            private set
            {
                _registrations = value;
                NotifyOfPropertyChange(() => Registrations);
            }
        }

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

        private ObservableCollection<DeptLocation> _DeptLocations;
        public ObservableCollection<DeptLocation> DeptLocations
        {
            get { return _DeptLocations; }
            set
            {
                _DeptLocations = value;
                NotifyOfPropertyChange(() => DeptLocations);
            }
        }

        private DeptLocation _SelectedLocation;
        public DeptLocation SelectedLocation
        {
            get { return _SelectedLocation; }
            set
            {
                _SelectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
                if (isNgoaiTru && 
                    _SelectedLocation != null)
                {
                    _searchCriteria.DeptID = _SelectedLocation.DeptID;
                    _searchCriteria.DeptLocationID = _SelectedLocation.DeptLocationID;                    
                }
                else 
                {
                    _searchCriteria.DeptID = null;
                    _searchCriteria.DeptID = null;
                }
            }
        }
        /// <summary>
        /// Bien nay dung de tim kiem dang ky.
        /// Khi nguoi dung thay doi thong tin tim kiem tren form thi khong set lai bien nay.
        /// Khi nao nguoi dung bat dau search thi moi lay gia tri cua SearchCriteria gan cho no.
        /// </summary>
        private SeachPtRegistrationCriteria _criteria;

        public void CreatePatientCmd()
        {

        }
        public void CheckHoanTat_Click(object sender) 
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                SearchCriteria.IsHoanTat = null;
            }
            else 
            {
                SearchCriteria.IsHoanTat = false;
            }
        }


        //▼====== #001
        AxSearchPatientTextBox txtNameTextBox;
        public void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtNameTextBox = (AxSearchPatientTextBox)sender;
        }
        public void SearchCmd()
        {
            //if(IsSearchGoToKhamBenh)
            //{
            //    if (Globals.DeptLocation != null)
            //    {
            //        _searchCriteria.DeptID = Globals.DeptLocation.DeptID;
            //        _searchCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
            //    }
            //}
            if (String.IsNullOrEmpty(txtNameTextBox.Text))
            {
                MessageBox.Show(eHCMSResources.Z2291_G1_CanhBaoNhapDuChiTiet);
                return;
            }
            if (SearchCriteria == null)
            {
                return;
            }
            //▼====== #002: Kiểm tra xem người dùng có tìm kiếm bằng tên không nếu có thì kiểm tra xem người dùng có tick vào cho phép tìm tên không.
            if (!String.IsNullOrEmpty(SearchCriteria.FullName) && !IsSearchPtByNameChecked)
            {
                MessageBox.Show(eHCMSResources.Z2304_G1_KhongTheTimDKBangTen);
                return;
            }
            //▲====== #002

            //▼====== #003
            string StringToFindAll = "*";
            if (string.Compare(SearchCriteria.FullName, StringToFindAll) == 0)
            {
                SearchCriteria.FullName = "";
                _criteria = _searchCriteria.DeepCopy();
                Registrations.PageIndex = 0;
                SearchRegistrations(0, Registrations.PageSize, true);
                return;
            }
            //▲====== #003

            _criteria = _searchCriteria.DeepCopy();
            Registrations.PageIndex = 0;
            SearchRegistrations(0, Registrations.PageSize, true);
            //▲====== #001
        }
        private void SearchRegistrations(int pageIndex, int pageSize, bool bCountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK) });
            //IsProcessing = true;
            //20180716 TBL: Comment _criteria.StaffID de khong xay ra loi 
            //_criteria.StaffID = aucHoldConsultDoctor.StaffID;
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrations(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrations(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                //IsProcessing = false;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            Registrations.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    Registrations.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        Registrations.Add(item);
                                    }

                                }
                            }
                            //IsProcessing = false;
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void ResetFilterCmd()
        {
            ResetFilter();
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedRegistration = eventArgs.Value as PatientRegistration;
            SelectItemAndCloseFormIfNeeded();
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private bool _isNgoaiTru;
        public bool isNgoaiTru
        {
            get { return _isNgoaiTru; }
            set
            {
                _isNgoaiTru = value;
                NotifyOfPropertyChange(() => isNgoaiTru);
            }
        }



        private bool _IsPopup=true;
        public bool IsPopup
        {
            get { return _IsPopup; }
            set {
                _IsPopup = value;
                NotifyOfPropertyChange(()=>IsPopup);
            }
        }

        public void LoadStaffs()
        {
            //_patientCatalog.GetStaffsHaveRegistrations();
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0603_G1_DangLayDSNVien)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetStaffsHaveRegistrations((byte)StaffRegistrationType.NORMAL,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Staff> allItems = new ObservableCollection<Staff>();
                                try
                                {
                                    allItems = contract.EndGetStaffsHaveRegistrations(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                Staffs = new ObservableCollection<Staff>(allItems);
                                Staff firstItem = new Staff();
                                firstItem.StaffID = -1;
                                firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                Staffs.Insert(0, firstItem);
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

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                DeptLocations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                DeptLocations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            DeptLocations.Insert(0, itemDefault);
            SelectedLocation = Globals.DeptLocation;
            yield break;
        }

        public void LoadDeptLocation(long? deptId)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}.", eHCMSResources.Z0115_G1_LayDSPgBan) });

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                DeptLocations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                DeptLocations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1098_G1_TatCaPhg);
                            DeptLocations.Insert(0, itemDefault);

                            SelectedLocation = Globals.DeptLocation;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void CopyExistingPatientList(IList<PatientRegistration> items, SeachPtRegistrationCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _registrations.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (PatientRegistration p in items)
                {
                    _registrations.Add(p);
                }
                _registrations.TotalItemCount = total;
            }
            NotifyOfPropertyChange(() => Registrations);
        }

        public void Handle(ItemSelected<PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                TryClose();
                Globals.EventAggregator.Unsubscribe(this);
            }
        }

        public void PrintCmd()
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.SeachRegistrationCriteria = SearchCriteria;
            //proAlloc.eItem = ReportName.REGISTRATIOBLIST;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            Action<ICommonPreviewView> onInitDlg = (Alloc) =>
            {
                Alloc.SeachRegistrationCriteria = SearchCriteria;
                Alloc.eItem = ReportName.REGISTRATIOBLIST;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void rdoAll_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.TypeSearch = 0;
        }

        public void rdoNoHI_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.TypeSearch = 1;
        }

        public void rdoHasHI_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.TypeSearch = 2;
        }

        public void rdoCross_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.TypeSearch = 3;
        }

        public void rdoNoCross_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.TypeSearch = 4;
        }

        public void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistration objRows = e.Row.DataContext as PatientRegistration;
            if(SearchCriteria.IsCancel)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            if (objRows != null)
            {
                switch (objRows.RegisType)
                {
                    case 0:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                    case 1:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    case 2:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                }
            }

        }

        //20181018 TNHX: [BM0002186] set default _IsSearchPtByNameChecked = true & IsAllowSearchingPtByName_Visible = false;
        //▼====== #002
        private bool _IsAllowSearchingPtByName_Visible = false;
        public bool IsAllowSearchingPtByName_Visible
        {
            get { return _IsAllowSearchingPtByName_Visible; }
            set
            {
                _IsAllowSearchingPtByName_Visible = value;
                NotifyOfPropertyChange(() => IsAllowSearchingPtByName_Visible);
            }
        }

        private bool _IsSearchPtByNameChecked = true;
        public bool IsSearchPtByNameChecked
        {
            get { return _IsSearchPtByNameChecked; }
            set
            {
                _IsSearchPtByNameChecked = value;
            }
        }
        //▲====== #002
    }
}
