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
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using aEMR.Controls;
using System.Linq;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindRegistrationDetailForMedicalExamination)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindRegistrationDetailForMedicalExaminationViewModel : ViewModelBase, IFindRegistrationDetailForMedicalExamination
        , IHandle<ItemSelected<PatientRegistration, PatientRegistrationDetail>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public FindRegistrationDetailForMedicalExaminationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            
            ResetFilter();
            
            RegistrationDetails = new PagedSortableCollectionView<PatientRegistrationDetail>();
            RegistrationDetails.OnRefresh += RegistrationDetails_OnRefresh;
            GetHospitalClientsData();
            LoadRegStatusList();
          
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
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
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
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
        void RegistrationDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchRegistrationDetails(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true);
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
            selectedRegistrationDetail = null;
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void OkCmd()
        {
            MessageBox.Show(string.Format("{0}.", eHCMSResources.A0898_G1_Msg_InfoNutKhSuDung), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            return;
        }

        private void SelectItemAndCloseFormIfNeeded()
        {
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

            if (ModuleConsult != null)
            {
                IPaperReferalFull TransferFormVM = ModuleConsult.MainContent as IPaperReferalFull;

                if (selectedRegistrationDetail != null && TransferFormVM != null)
                {
                    Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = selectedRegistrationDetail.PatientRegistration, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
                }
                if (selectedRegistrationDetail != null && Globals.PatientFindBy_ForConsultation == (long)AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = selectedRegistrationDetail });
                }

                if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NOITRU)
                {
                    Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = selectedRegistrationDetail.PatientRegistration });
                }
            }
            else if (ModuleStorageClinicDept != null || ModuleTransaction != null)
            {
                if (SearchCriteria != null && selectedRegistrationDetail != null)
                {
                    Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = selectedRegistrationDetail.PatientRegistration });
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = selectedRegistrationDetail.PatientRegistration });
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

        private PatientRegistrationDetail _selectedRegistrationDetail;
        public PatientRegistrationDetail selectedRegistrationDetail
        {
            get { return _selectedRegistrationDetail; }
            set
            {
                _selectedRegistrationDetail = value;
                NotifyOfPropertyChange(() => selectedRegistrationDetail);
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

        private PagedSortableCollectionView<PatientRegistrationDetail> _RegistrationDetails;
        public PagedSortableCollectionView<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _RegistrationDetails; }
            private set
            {
                _RegistrationDetails = value;
                NotifyOfPropertyChange(() => RegistrationDetails);
                if (theGrid != null)
                {
                    theGrid.SelectedIndex = -1;
                }
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
        AxSearchPatientTextBox txtNameTextBox;
        public void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtNameTextBox = (AxSearchPatientTextBox)sender;
        }
        public void SearchCmd()
        {
            if (String.IsNullOrEmpty(txtNameTextBox.Text))
            {
                MessageBox.Show(eHCMSResources.Z2291_G1_CanhBaoNhapDuChiTiet);
                return;
            }
            if (SearchCriteria == null)
            {
                return;
            }
            if (!String.IsNullOrEmpty(SearchCriteria.FullName) && !IsSearchPtByNameChecked)
            {
                MessageBox.Show(eHCMSResources.Z2304_G1_KhongTheTimDKBangTen);
                return;
            }
            _criteria = _searchCriteria.DeepCopy();
            RegistrationDetails.PageIndex = 0;
            if(HospitalClientSelected != null)
            {
                _criteria.HosClientID = HospitalClientSelected.HosClientID;
            }
            SearchRegistrationDetails(0, RegistrationDetails.PageSize, true);
        }
        private void SearchRegistrationDetails(int pageIndex, int pageSize, bool bCountTotal)
        {            
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            IsProcessing = true;
            if (_criteria == null) _criteria = new SeachPtRegistrationCriteria();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsForMedicalExaminationDiag(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistrationDetail> allItems = null;
                            try
                            {
                                RegistrationDetails.Clear();
                                allItems = client.EndSearchRegistrationsForMedicalExaminationDiag(out totalCount, asyncResult);
                                if (allItems.Count > 0)
                                {
                                    allItems = allItems.Where(x => x.RefMedicalServiceItem.IsMedicalExamination).ToList();
                                }
                                if (bCountTotal)
                                {
                                    RegistrationDetails.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        RegistrationDetails.Add(item);
                                    }

                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                IsProcessing = false;
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        //private void SearchRegistrationDetails(int pageIndex, int pageSize, bool bCountTotal, bool IsSearchPhysicalExaminationOnly)
        //{            
        //    this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
        //    if (_criteria == null) _criteria = new SeachPtRegistrationCriteria();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PatientRegistrationServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginSearchRegistrationsForDiag(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    int totalCount = 0;
        //                    IList<PatientRegistrationDetail> allItems = null;
        //                    try
        //                    {
        //                        RegistrationDetails.Clear();
        //                        allItems = client.EndSearchRegistrationsForDiag(out totalCount, asyncResult);
                                
        //                        if (bCountTotal)
        //                        {
        //                            RegistrationDetails.TotalItemCount = totalCount;
        //                        }
        //                        if (allItems != null)
        //                        {
        //                            if (allItems.Count > 0)
        //                            {
        //                                allItems = allItems.Where(x => x.RefMedicalServiceItem.IsMedicalExamination).ToList();
        //                                RegistrationDetails.TotalItemCount = allItems.Count;
        //                            }
        //                            foreach (var item in allItems)
        //                            {
        //                                RegistrationDetails.Add(item);
        //                            }

        //                        }
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }
        //                    finally
        //                    {
        //                        this.DlgHideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ClientLoggerHelper.LogInfo(ex.ToString());
        //            this.DlgHideBusyIndicator();
        //        }
        //    });

        //    t.Start();
        //}

        public void ResetFilterCmd()
        {
            ResetFilter();
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            selectedRegistrationDetail = eventArgs.Value as PatientRegistrationDetail;
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

      
        public void CopyExistingPatientList(IList<PatientRegistrationDetail> items, SeachPtRegistrationCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _RegistrationDetails.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (PatientRegistrationDetail p in items)
                {
                    _RegistrationDetails.Add(p);
                }
                _RegistrationDetails.TotalItemCount = total;
            }
            NotifyOfPropertyChange(() => RegistrationDetails);
        }

        public void Handle(ItemSelected<PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                try
                {
                    TryClose();
                    Globals.EventAggregator.Unsubscribe(this);
                }
                catch { }
            }
        }

        public void PrintCmd()
        {
            Action<ICommonPreviewView> onInitDlg = (Alloc) =>
            {
                Alloc.SeachRegistrationCriteria = SearchCriteria;
                Alloc.eItem = ReportName.REGISTRATIONDETAILLIST;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void rdoChuaKham_Click(object sender, RoutedEventArgs e) 
        {
            SearchCriteria.IsHoanTat = false;            
        }
        public void rdoKhamRoi_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsHoanTat = true;            
        }
        public void rdoTatCa_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsHoanTat = null;
        }

        private ReadOnlyDataGrid theGrid = null;

        public void  gridRegistrations_Loaded(object sender)
        {
            theGrid = (ReadOnlyDataGrid)sender;            
        }

        public void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistrationDetail objRows = e.Row.DataContext as PatientRegistrationDetail;
            if (objRows != null)
            {
                switch (objRows.ExamRegStatus )
                {
                    case AllLookupValues.ExamRegStatus.HOAN_TAT:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                }
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
        public bool IsSearchPtByNameChecked_SameTwoView { get; set; } = false;
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
        private bool _IsSearchPhysicalExaminationOnly = false;
        public bool IsSearchPhysicalExaminationOnly
        {
            get { return _IsSearchPhysicalExaminationOnly; }
            set
            {
                _IsSearchPhysicalExaminationOnly = value;
                NotifyOfPropertyChange(() => IsSearchPhysicalExaminationOnly);
            }
        }
        #region Tìm kiếm theo tên công ty
        private HospitalClient _HospitalClientSelected;
        public HospitalClient HospitalClientSelected
        {
            get
            {
                return _HospitalClientSelected;
            }
            set
            {
                if (_HospitalClientSelected == value)
                {
                    return;
                }
                _HospitalClientSelected = value;
                NotifyOfPropertyChange(() => HospitalClientSelected);
            }
        }

        private ObservableCollection<HospitalClient> _HospitalClientCollection;
        public ObservableCollection<HospitalClient> HospitalClientCollection
        {
            get
            {
                return _HospitalClientCollection;
            }
            set
            {
                if (_HospitalClientCollection == value)
                {
                    return;
                }
                _HospitalClientCollection = value;
                NotifyOfPropertyChange(() => HospitalClientCollection);
            }
        }

        private void GetHospitalClientsData()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClients(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var Collection = mContract.EndGetHospitalClients(asyncResult);
                                if (Collection == null || Collection.Count == 0)
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>();
                                }
                                else
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>(Collection);
                                    HospitalClient firstItem = new HospitalClient();
                                    firstItem.HosClientID = 0;
                                    firstItem.CompanyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    HospitalClientCollection.Insert(0, firstItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
    }
}
