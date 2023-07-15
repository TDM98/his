using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common;
using System.ComponentModel;
using System.Windows.Data;
using aEMR.Controls;
/*
* 20190908 #001 TTM:   BM 0013139 [Khám sức khoẻ] Import Bệnh nhân vào chương trình từ file csv.
* 20191219 #002 TBL:   BM 0020728: Tạo mới hợp đồng giữ lại danh sách khách hàng và nhóm khách hàng
*/
namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHealthExaminationRecord)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HealthExaminationRecordViewModel : ViewModelBase, IHealthExaminationRecord
        , IHandle<ItemSelected<Patient>>
    {
        #region Properties
        private string _TitleForm = Globals.TitleForm;
        private PatientRegistration _CurrentRegistration = new PatientRegistration();
        private HospitalClientContract _CurrentClientContract;
        private int gCurrentPatientRowIndex = -1;
        private DataTable _SummaryContractTable = new DataTable();
        private decimal _TotalContractAmountTemp;
        public decimal TotalContractAmountTemp
        {
            get
            {
                return _TotalContractAmountTemp;
            }
            set
            {
                if (_TotalContractAmountTemp != value)
                {
                    _TotalContractAmountTemp = value;
                    NotifyOfPropertyChange(() => TotalContractAmountTemp);
                }
            }
        }
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                _CurrentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }
        public HospitalClientContract CurrentClientContract
        {
            get
            {
                return _CurrentClientContract;
            }
            set
            {
                if (_CurrentClientContract == value)
                {
                    return;
                }
                _CurrentClientContract = value;
                NotifyOfPropertyChange(() => CurrentClientContract);
                if (HosClientComboBox != null)
                {
                    HosClientComboBox.Text = CurrentClientContract.HosClient == null ? string.Empty : CurrentClientContract.HosClient.ClientName;
                }
                NotifyOfPropertyChange(() => TotalContractPaidAmount);
                NotifyOfPropertyChange(() => TotalFinalizedAmount);
                InitPatientCollectionView();
                InitServiceCollectionView();
                if (CurrentClientContract != null && CurrentClientContract.HosClientContractID > 0)
                {
                    GetPatientGroupCollection(CurrentClientContract.HosClientContractID);
                }
                else
                {
                    PatientGroupCollection = new ObservableCollection<HosClientContractPatientGroup> { new HosClientContractPatientGroup { HosClientContractPatientGroupID = 0, HosClientContractPatientGroupName = string.Empty } };
                }
            }
        }
        public DataTable SummaryContractTable
        {
            get
            {
                return _SummaryContractTable;
            }
            set
            {
                _SummaryContractTable = value;
                NotifyOfPropertyChange(() => SummaryContractTable);
            }
        }
        public List<HospitalClient> HospitalClientCollection { get; set; }
        public decimal TotalContractPaidAmount
        {
            get
            {
                return CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null ? 0 : CurrentClientContract.ContractPatientCollection.Sum(x => x.TotalContractPaidAmount);
            }
        }
        public decimal TotalFinalizedAmount
        {
            get
            {
                return CurrentClientContract == null || CurrentClientContract.ClientContractFinalizationCollection == null ? 0 : CurrentClientContract.ClientContractFinalizationCollection.Sum(x => x.Amount);
            }
        }
        private ObservableCollection<eHCMS.Services.Core.Gender> _GenderCollection;
        public ObservableCollection<eHCMS.Services.Core.Gender> GenderCollection
        {
            get
            {
                return _GenderCollection;
            }
            set
            {
                if (_GenderCollection == value)
                {
                    return;
                }
                _GenderCollection = value;
                NotifyOfPropertyChange(() => GenderCollection);
            }
        }
        private string _CurrentFilerGender;
        public string CurrentFilerGender
        {
            get
            {
                return _CurrentFilerGender;
            }
            set
            {
                if (_CurrentFilerGender == value)
                {
                    return;
                }
                _CurrentFilerGender = value;
                NotifyOfPropertyChange(() => CurrentFilerGender);
                PatientCollectionView.Refresh();
            }
        }
        private ICollectionView _PatientCollectionView;
        public ICollectionView PatientCollectionView
        {
            get
            {
                return _PatientCollectionView;
            }
            set
            {
                if (_PatientCollectionView == value)
                {
                    return;
                }
                _PatientCollectionView = value;
                NotifyOfPropertyChange(() => PatientCollectionView);
            }
        }
        private ObservableCollection<HosClientContractPatientGroup> _PatientGroupCollection;
        public ObservableCollection<HosClientContractPatientGroup> PatientGroupCollection
        {
            get
            {
                return _PatientGroupCollection;
            }
            set
            {
                if (_PatientGroupCollection == value)
                {
                    return;
                }
                _PatientGroupCollection = value;
                NotifyOfPropertyChange(() => PatientGroupCollection);
            }
        }
        private ObservableCollection<HosClientContractPatientGroup> _PatientGroup;
        public ObservableCollection<HosClientContractPatientGroup> PatientGroup
        {
            get
            {
                return _PatientGroup;
            }
            set
            {
                if (_PatientGroup == value)
                {
                    return;
                }
                _PatientGroup = value;
                NotifyOfPropertyChange(() => PatientGroup);
            }
        }

        private long _CurrentFilerPatientGroupID = 0;
        public long CurrentFilerPatientGroupID
        {
            get
            {
                return _CurrentFilerPatientGroupID;
            }
            set
            {
                if (_CurrentFilerPatientGroupID == value)
                {
                    NotifyOfPropertyChange(() => CurrentFilerPatientGroupID);
                    return;
                }
                _CurrentFilerPatientGroupID = value;
                NotifyOfPropertyChange(() => CurrentFilerPatientGroupID);
                PatientCollectionView.Refresh();
            }
        }
        private int? _FilterAgeFrom;
        public int? FilterAgeFrom
        {
            get
            {
                return _FilterAgeFrom;
            }
            set
            {
                if (_FilterAgeFrom == value)
                {
                    return;
                }
                _FilterAgeFrom = value;
                NotifyOfPropertyChange(() => FilterAgeFrom);
                PatientCollectionView.Refresh();
            }
        }
        private int? _FilterAgeTo;
        public int? FilterAgeTo
        {
            get
            {
                return _FilterAgeTo;
            }
            set
            {
                if (_FilterAgeTo == value)
                {
                    return;
                }
                _FilterAgeTo = value;
                NotifyOfPropertyChange(() => FilterAgeTo);
                PatientCollectionView.Refresh();
            }
        }
        private string _FullNameFilterString;
        public string FullNameFilterString
        {
            get
            {
                return _FullNameFilterString;
            }
            set
            {
                if (_FullNameFilterString == value)
                {
                    return;
                }
                _FullNameFilterString = value;
                NotifyOfPropertyChange(() => FullNameFilterString);
                PatientCollectionView.Refresh();
            }
        }
        private string _ServiceNameFilterString;
        public string ServiceNameFilterString
        {
            get
            {
                return _ServiceNameFilterString;
            }
            set
            {
                if (_ServiceNameFilterString == value)
                {
                    return;
                }
                _ServiceNameFilterString = value;
                NotifyOfPropertyChange(() => ServiceNameFilterString);
                ServiceCollectionView.Refresh();
            }
        }
        private ICollectionView _ServiceCollectionView;
        public ICollectionView ServiceCollectionView
        {
            get
            {
                return _ServiceCollectionView;
            }
            set
            {
                if (_ServiceCollectionView == value)
                {
                    return;
                }
                _ServiceCollectionView = value;
                NotifyOfPropertyChange(() => ServiceCollectionView);
            }
        }
        private AxAutoComplete HosClientComboBox { get; set; }
        private bool _IsPayment;
        public bool IsPayment
        {
            get { return _IsPayment; }
            set
            {
                if(_IsPayment != value)
                {
                    _IsPayment = value;
                    NotifyOfPropertyChange(() => IsPayment);
                }
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public HealthExaminationRecordViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgr, ISalePosCaching aCaching)
        {
            LoadGenderCollection();
            DateTime mNow = Globals.GetCurServerDateTime();
            CurrentClientContract = new HospitalClientContract { ContractDate = mNow, ValidDateFrom = mNow, ValidDateTo = mNow, ModifiedStaff = new Staff { StaffID = (Globals.LoggedUserAccount == null ? 0 : Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0)) } };
            GetHospitalClientsData();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        public void btnAddPatient()
        {
            GlobalsNAV.ShowDialog<IFindPatient>((aView) =>
            {
                aView.Patients.Clear();
                aView.SearchCriteria = new PatientSearchCriteria();
                aView.IsAddingMultiples = true;
            });
            InitSummaryTabItem();
        }
        public void btnAddService()
        {
            IHealthExaminationRecordServiceEdit aView = Globals.GetViewModel<IHealthExaminationRecordServiceEdit>();
            aView.InitCurrentRegistration(CurrentRegistration);
            GlobalsNAV.ShowDialog_V3(aView, null, null, false, true, new Size(1000, 600));
            CurrentRegistration = aView.CurrentRegistration;
            ReInitRegistration(CurrentRegistration);
        }
        public void btnApplyService()
        {
            if (CurrentClientContract == null
                || CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0
                || CurrentClientContract.ContractServiceItemCollection == null || CurrentClientContract.ContractServiceItemCollection.Count == 0)
            {
                return;
            }
            if (CurrentClientContract.ServiceItemPatientLinkCollection == null)
            {
                CurrentClientContract.ServiceItemPatientLinkCollection = new List<ClientContractServiceItemPatientLink>();
            }
            var PatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.IsSelected).ToList();
            var KeepCurrentValueItemLinkCollection = !CurrentClientContract.ContractServiceItemCollection.Any(x => x.IsChecked == null) ? null : CurrentClientContract.ContractServiceItemCollection.Where(x => x.IsChecked == null).ToList();
            var MedServiceItemCollection = CurrentClientContract.ContractServiceItemCollection.Where(x => x.IsChecked.GetValueOrDefault(false) == true).ToList();
            CallSelectServiceOnPatientCollection(PatientCollection, MedServiceItemCollection, KeepCurrentValueItemLinkCollection);
        }
        public void CurrentPatient_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0
                || CurrentClientContract.ServiceItemPatientLinkCollection == null)
            {
                return;
            }
            if (!CurrentClientContract.ContractPatientCollection.Any(x => x.IsSelected))
            {
                foreach (var Item in CurrentClientContract.ContractServiceItemCollection)
                {
                    Item.IsChecked = false;
                }
                return;
            }
            var CurrentPatientCollecion = CurrentClientContract.ContractPatientCollection.Where(i => i.IsSelected);
            var CurrentRegItemCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => CurrentPatientCollecion.Any(i => i == x.ContractPatient)).ToList();
            if (CurrentRegItemCollection == null)
            {
                return;
            }
            List<MedRegItemBase> DistictItem = CurrentRegItemCollection.GroupBy(x => x.ContractMedRegItem.MedRegItem).Where(x => x.Count() == CurrentPatientCollecion.Count()).Select(x => x.Key).ToList();
            foreach (var Item in CurrentClientContract.ContractServiceItemCollection)
            {
                if (!CurrentRegItemCollection.Any(x => x.ContractMedRegItem.MedRegItem == Item.MedRegItem))
                {
                    Item.IsChecked = false;
                }
                else if (DistictItem.Any(x => x == Item.MedRegItem))
                {
                    Item.IsChecked = true;
                }
                else
                {
                    Item.IsChecked = null;
                }
            }
        }
        public void gvPatients_CurrentCellChanged(object sender, EventArgs e)
        {
            if (CurrentClientContract.ContractPatientCollection.Any(x => x.IsSelected))
            {
                return;
            }
            if (CurrentClientContract == null || CurrentClientContract.ContractServiceItemCollection == null || CurrentClientContract.ContractServiceItemCollection.Count == 0)
            {
                return;
            }
            if (CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Any(x => x.IsSelected))
            {
                return;
            }
            if ((sender as DataGrid).CurrentCell == null || (sender as DataGrid).CurrentCell.Item == null || !((sender as DataGrid).CurrentCell.Item is HosClientContractPatient))
            {
                return;
            }
            if (gCurrentPatientRowIndex != CurrentClientContract.ContractPatientCollection.IndexOf((sender as DataGrid).CurrentCell.Item as HosClientContractPatient))
            {
                foreach (var aItem in CurrentClientContract.ContractServiceItemCollection)
                {
                    aItem.IsChecked = false;
                }
                if (CurrentClientContract.ServiceItemPatientLinkCollection != null
                    && CurrentClientContract.ServiceItemPatientLinkCollection.Any(x => x.ContractPatient == (sender as DataGrid).CurrentCell.Item))
                {
                    foreach (var aItem in CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => x.ContractPatient == (sender as DataGrid).CurrentCell.Item))
                    {
                        if (!CurrentClientContract.ContractServiceItemCollection.Any(x => x.MedRegItem == aItem.ContractMedRegItem.MedRegItem))
                        {
                            continue;
                        }
                        CurrentClientContract.ContractServiceItemCollection.Where(x => x.MedRegItem == aItem.ContractMedRegItem.MedRegItem).First().IsChecked = true;
                    }
                }
            }
            gCurrentPatientRowIndex = CurrentClientContract.ContractPatientCollection.IndexOf((sender as DataGrid).CurrentCell.Item as HosClientContractPatient);
        }
        public void btnAddClient()
        {
            IHospitalClientEdit mView = Globals.GetViewModel<IHospitalClientEdit>();
            GlobalsNAV.ShowDialog_V3(mView);
            if (mView.IsCompleted)
            {
                if (HospitalClientCollection == null)
                {
                    HospitalClientCollection = new List<HospitalClient>();
                }
                if (HospitalClientCollection.Any(x => x.HosClientID == mView.CurrentHospitalClient.HosClientID))
                {
                    var CurrentItem = HospitalClientCollection.First(x => x.HosClientID == mView.CurrentHospitalClient.HosClientID);
                    CurrentItem = mView.CurrentHospitalClient.EntityDeepCopy();
                }
                else
                {
                    HospitalClientCollection.Add(mView.CurrentHospitalClient.EntityDeepCopy());
                }
                CurrentClientContract.HosClient = HospitalClientCollection.First(x => x.HosClientID == mView.CurrentHospitalClient.HosClientID);
            }
        }
        public void HosClient_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (HospitalClientCollection != null && HospitalClientCollection.Count > 0
                && HospitalClientCollection.Any(x => !string.IsNullOrEmpty(x.ClientName)))
            {
                cboContext.ItemsSource = new ObservableCollection<HospitalClient>(HospitalClientCollection.Where(x => string.IsNullOrEmpty(cboContext.SearchText) || (!string.IsNullOrEmpty(x.ClientName) && Globals.RemoveVietnameseString(x.ClientName.ToLower()).Contains(Globals.RemoveVietnameseString(cboContext.SearchText.ToLower())))));
            }
            else
            {
                cboContext.ItemsSource = new ObservableCollection<HospitalClient>();
            }
            cboContext.PopulateComplete();
        }
        public void HosClient_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            CurrentClientContract.HosClient = ((AutoCompleteBox)sender).SelectedItem as HospitalClient;
        }
        public void CheckBoxAllPatient_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0
                || !(sender is CheckBox))
            {
                return;
            }
            bool IsCurrentCheckedValue = (sender as CheckBox).IsChecked.GetValueOrDefault(false);
            foreach (var Item in CurrentClientContract.ContractPatientCollection)
            {
                if (Item.IsScheduled || Item.IsProcessed)
                {
                    continue;
                }
                if (FilterPatient(Item))
                {
                    Item.IsSelected = IsCurrentCheckedValue;
                }
                else
                {
                    Item.IsSelected = false;
                }
            }
        }
        public void CheckBoxAllServiceItem_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractServiceItemCollection == null || CurrentClientContract.ContractServiceItemCollection.Count == 0
                || !(sender is CheckBox))
            {
                return;
            }
            bool IsCurrentCheckedValue = (sender as CheckBox).IsChecked.GetValueOrDefault(false);
            foreach (var item in CurrentClientContract.ContractServiceItemCollection)
            {
                item.IsChecked = IsCurrentCheckedValue;
            }
        }
        public void btnSave()
        {
            if (!IsValidClientContract())
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginEditHospitalClientContract(CurrentClientContract, PatientGroup != null ? PatientGroup.ToList() : null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool mExCode;
                                var mUpdatedClientContract = mContract.EndEditHospitalClientContract(out mExCode, asyncResult);
                                if (mExCode)
                                {
                                    CurrentClientContract = mUpdatedClientContract;
                                    ReloadCurrentClientContract(CurrentClientContract);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0432_G1_Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        public void RemovePatientItemCmd(HosClientContractPatient aContractPatient)
        {
            if (aContractPatient == null)
            {
                return;
            }
            if (CurrentClientContract.ServiceItemPatientLinkCollection != null && CurrentClientContract.ServiceItemPatientLinkCollection.Count > 0)
            {
                CurrentClientContract.ServiceItemPatientLinkCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => x.ContractPatient != aContractPatient).ToList();
            }
            CurrentClientContract.ContractPatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x != aContractPatient).ToObservableCollection();
            InitPatientCollectionView();
        }
        public void btnRenew()
        {
            DateTime mNow = Globals.GetCurServerDateTime();
            CurrentClientContract = new HospitalClientContract { ContractDate = mNow, ValidDateFrom = mNow, ValidDateTo = mNow, ModifiedStaff = new Staff { StaffID = (Globals.LoggedUserAccount == null ? 0 : Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0)) } };
            CurrentRegistration = new PatientRegistration();
            TotalContractAmountTemp = 0;
            SummaryContractTable = new DataTable();
        }
        //▼===== #002
        public void btnCreateNewToOld()
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClient == null || CurrentClientContract.ContractPatientCollection == null)
            {
                return;
            }
            PatientGroup = new ObservableCollection<HosClientContractPatientGroup>(PatientGroupCollection);
            HospitalClient hospitalClient = CurrentClientContract.HosClient;
            IList<HosClientContractPatient> hosClientContractPatients = CurrentClientContract.ContractPatientCollection;
            long HosClientContractIDOld = CurrentClientContract.HosClientContractID;
            DateTime mNow = Globals.GetCurServerDateTime();
            CurrentClientContract = new HospitalClientContract { ContractDate = mNow, ValidDateFrom = mNow,
                ValidDateTo = mNow, ModifiedStaff = new Staff { StaffID = (Globals.LoggedUserAccount == null ? 0 : Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0)) },
                HosClient = hospitalClient, ContractPatientCollection = hosClientContractPatients, IsCreateNewToOld = true };
            CurrentRegistration = new PatientRegistration();
            TotalContractAmountTemp = 0;
            SummaryContractTable = new DataTable();
        }
        //▲===== #002
        public void btnFindOld()
        {
            IHealthExaminationRecordSearch aView = Globals.GetViewModel<IHealthExaminationRecordSearch>();
            aView.HospitalClientCollection = HospitalClientCollection;
            GlobalsNAV.ShowDialog_V3(aView, null, null, false, true, new Size(1000, 600));
            if (aView.IsCompleted)
            {
                CurrentClientContract = aView.SelectedHospitalClientContract;
                InitCurrentClientContractViewDetails(CurrentClientContract);
                gCurrentPatientRowIndex = -1;
            }
        }
        public void btnConfirmCompleted()
        {
            if (Globals.LoggedUserAccount == null)
            {
                return;
            }
            if (CurrentClientContract.ContractPatientCollection.Any(x => x.HosContractPtID == 0)
                || CurrentClientContract.ContractServiceItemCollection.Any(x => x.ClientContractSvcID == 0)
                || CurrentClientContract.ServiceItemPatientLinkCollection.Any(x => x.ClientContractSvcPtID == 0))
            {
                if (MessageBox.Show(eHCMSResources.Z2798_G1_TiepTucThayDoiChuaLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginActiveHospitalClientContract(CurrentClientContract.HosClientContractID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (mContract.EndActiveHospitalClientContract(asyncResult))
                                {
                                    CurrentClientContract.ActivationDate = Globals.GetCurServerDateTime();
                                    ReloadCurrentClientContract(CurrentClientContract);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0432_G1_Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        public void btnGetPatientPaidAmount()
        {
            GetPatientPaidAmount();
        }
        public void btnCompleteContract()
        {
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginCompleteHospitalClientContract(CurrentClientContract.HosClientContractID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                mContract.EndCompleteHospitalClientContract(asyncResult);
                                CurrentClientContract.CompletedDate = Globals.GetCurServerDateTime();
                                ReloadCurrentClientContract(CurrentClientContract);
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        public void btnFinalizeContract()
        {
            FinalizeContract();
        }
        public void btnConfirmFinalizeContract()
        {
            FinalizeContract(true);
        }
        public void btnExportExcel()
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            SaveFileDialog mFileDialog = new SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
            if (mFileDialog.ShowDialog() != true)
            {
                return;
            }
            this.ShowBusyIndicator();
            ExportToExcelGeneric.ExportHospitalClientContractExcel(CurrentClientContract.HosClientContractID, mFileDialog, this);
            this.HideBusyIndicator();
        }
        public void btnExportResultExcel()
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            SaveFileDialog mFileDialog = new SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
            if (mFileDialog.ShowDialog() != true)
            {
                return;
            }
            this.ShowBusyIndicator();
            ExportToExcelGeneric.ExportHospitalClientContract_ReulstExcel(CurrentClientContract.HosClientContractID, mFileDialog, this);
            this.HideBusyIndicator();
        }
        public void btnPrintPreview()
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                aView.ID = CurrentClientContract.HosClientContractID;
                aView.eItem = ReportName.CT_HD_KhamSucKhoe;
            });
        }
        public void GroupPatientButton()
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null ||
                !CurrentClientContract.ContractPatientCollection.Any(x => x.IsSelected) ||
                CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            var DialogView = Globals.GetViewModel<IHealthExaminationRecordGroup>();
            DialogView.PatientGroupCollection = PatientGroupCollection == null || !PatientGroupCollection.Any(x => x.HosClientContractPatientGroupID > 0) ? null : PatientGroupCollection.Where(x => x.HosClientContractPatientGroupID > 0).ToObservableCollection();
            DialogView.HosClientContractID = CurrentClientContract.HosClientContractID;
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.IsCompleted)
            {
                if (PatientGroupCollection == null)
                {
                    PatientGroupCollection = new ObservableCollection<HosClientContractPatientGroup> { new HosClientContractPatientGroup { HosClientContractPatientGroupID = 0, HosClientContractPatientGroupName = string.Empty } };
                }
                HosClientContractPatient PatientOfGroup = null;
                List<MedRegItemBase> ItemCollectionOfGroup = null;
                if (PatientGroupCollection.Any(x => x.HosClientContractPatientGroupID == DialogView.CurrentHosClientContractPatientGroup.HosClientContractPatientGroupID))
                {
                    DialogView.CurrentHosClientContractPatientGroup = PatientGroupCollection.First(x => x.HosClientContractPatientGroupID == DialogView.CurrentHosClientContractPatientGroup.HosClientContractPatientGroupID);
                    PatientOfGroup = CurrentClientContract.ContractPatientCollection.FirstOrDefault(x => !x.IsSelected && x.PatientObj != null && x.PatientGroupCollection != null && x.PatientGroupCollection.Any(i => i.HosClientContractPatientGroupID == DialogView.CurrentHosClientContractPatientGroup.HosClientContractPatientGroupID));
                    ItemCollectionOfGroup = PatientOfGroup == null || !CurrentClientContract.ServiceItemPatientLinkCollection.Any(x => PatientOfGroup != null && x.ContractPatient != null && x.ContractPatient.PatientObj != null && x.ContractPatient.PatientObj.PatientID == PatientOfGroup.PatientObj.PatientID && x.ContractMedRegItem != null && x.ContractMedRegItem.MedRegItem != null) ? null :
                        CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => PatientOfGroup != null && x.ContractPatient != null && x.ContractPatient.PatientObj != null && x.ContractPatient.PatientObj.PatientID == PatientOfGroup.PatientObj.PatientID && x.ContractMedRegItem != null && x.ContractMedRegItem.MedRegItem != null).Select(x => x.ContractMedRegItem.MedRegItem).ToList();
                }
                else
                {
                    PatientGroupCollection.Add(DialogView.CurrentHosClientContractPatientGroup);
                }
                List<HosClientContractPatient> CurrentSelectedPatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.IsSelected).ToList();
                CurrentClientContract.ServiceItemPatientLinkCollection = PatientOfGroup == null || ItemCollectionOfGroup == null || ItemCollectionOfGroup.Count == 0 ? CurrentClientContract.ServiceItemPatientLinkCollection : CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => !CurrentSelectedPatientCollection.Select(i => i.PatientObj.PatientID).Contains(x.ContractPatient.PatientObj.PatientID)).ToObservableCollection();
                foreach (var Item in CurrentSelectedPatientCollection)
                {
                    Item.PatientGroupCollection = new List<HosClientContractPatientGroup> { DialogView.CurrentHosClientContractPatientGroup };
                    if (ItemCollectionOfGroup == null || ItemCollectionOfGroup.Count == 0)
                    {
                        continue;
                    }
                    foreach (var ServiceItem in ItemCollectionOfGroup)
                    {
                        var aServiceItem = CurrentClientContract.ContractServiceItemCollection.FirstOrDefault(x => x.MedRegItem.SameAsMedItemPrimary(ServiceItem));
                        if (aServiceItem == null)
                        {
                            break;
                        }
                        CurrentClientContract.ServiceItemPatientLinkCollection.Add(new ClientContractServiceItemPatientLink { ContractPatient = Item, ContractMedRegItem = aServiceItem });
                    }
                    //if (Item.PatientGroupCollection == null)
                    //{
                    //    Item.PatientGroupCollection = new List<HosClientContractPatientGroup> { DialogView.CurrentHosClientContractPatientGroup };
                    //}
                    //else
                    //{
                    //    Item.PatientGroupCollection.Add(DialogView.CurrentHosClientContractPatientGroup);
                    //    Item.NotifyChangedPatientGroupString();
                    //}
                }
                if (ItemCollectionOfGroup != null && ItemCollectionOfGroup.Count > 0)
                {
                    CurrentPatient_CheckChanged(null, null);
                }
            }
        }
        public void GroupPatientGroupServiceButton()
        {
            if (CurrentClientContract == null ||
                CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0)
            {
                return;
            }
            if (PatientGroupCollection == null || !PatientGroupCollection.Any(x => x.HosClientContractPatientGroupID > 0))
            {
                return;
            }
            IHealthExaminationRecordServiceEdit aView = Globals.GetViewModel<IHealthExaminationRecordServiceEdit>();
            aView.PatientGroupCollection = PatientGroupCollection.Where(x => x.HosClientContractPatientGroupID > 0 && CurrentClientContract.ContractPatientCollection.Any(i => i.PatientGroupCollection != null && i.PatientGroupCollection.Any(z => z.HosClientContractPatientGroupID == x.HosClientContractPatientGroupID))).ToObservableCollection();
            aView.CurrentClientContract = CurrentClientContract;
            aView.InitCurrentRegistration(CurrentRegistration);
            GlobalsNAV.ShowDialog_V3(aView, null, null, false, true, new Size(1000, 600));
            CurrentRegistration = aView.CurrentRegistration;
            if (!aView.IsConfirmed)
            {
                return;
            }
            var AddedServices = CurrentRegistration.AllSavedInvoiceItem.Where(x => x.DisplayID == 1 || x.DisplayID == 2).DeepCopy();
            ReInitRegistration(CurrentRegistration);
            //Thêm dịch vụ lên các bệnh nhân trong nhóm đã chọn
            if (CurrentClientContract.ServiceItemPatientLinkCollection == null)
            {
                CurrentClientContract.ServiceItemPatientLinkCollection = new List<ClientContractServiceItemPatientLink>();
            }
            var PatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.PatientGroupCollection != null && !x.IsProcessed && x.PatientGroupCollection.Any(i => i.HosClientContractPatientGroupID == aView.CurrentPatientGroupID)).ToList();
            if (PatientCollection == null || PatientCollection.Count == 0)
            {
                return;
            }
            var MedServiceItemCollection = CurrentClientContract.ContractServiceItemCollection.Where(x => AddedServices.Any(i => i.SameAsMedItemPrimary(x.MedRegItem))).Distinct().ToList();
            CallSelectServiceOnPatientCollection(PatientCollection, MedServiceItemCollection, null);
        }
        public void AddPatientToGroupButton()
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            IHospitalClientImportPatientCollection DialogView = Globals.GetViewModel<IHospitalClientImportPatientCollection>();
            DialogView.PatientGroupCollection = PatientGroupCollection == null || !PatientGroupCollection.Any(x => x.HosClientContractPatientGroupID > 0) ? null : PatientGroupCollection.Where(x => x.HosClientContractPatientGroupID > 0).ToObservableCollection();
            DialogView.HosClientContractID = CurrentClientContract.HosClientContractID;
            DialogView.CurrentClientContract = CurrentClientContract;
            DialogView.CurrentRegistration = CurrentRegistration;
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.IsConfirmed && !string.IsNullOrEmpty(DialogView.CurrentFileName))
            {
                if (DialogView.AddedNewHosClientContractPatientGroup != null)
                {
                    if (PatientGroupCollection == null)
                    {
                        PatientGroupCollection = new ObservableCollection<HosClientContractPatientGroup> { DialogView.AddedNewHosClientContractPatientGroup };
                    }
                    else
                    {
                        PatientGroupCollection.Add(DialogView.AddedNewHosClientContractPatientGroup);
                    }
                }
                CallAddPatientCollection(DialogView.CurrentFileName, DialogView.CurrentPatientGroupID, DialogView.MedRegItemBaseCollection);
            }
        }
        public void ClearFilterButton()
        {
            _CurrentFilerGender = GenderCollection.First().ID;
            _CurrentFilerPatientGroupID = PatientGroupCollection.First().HosClientContractPatientGroupID;
            _FilterAgeFrom = null;
            _FilterAgeTo = null;
            NotifyOfPropertyChange(() => CurrentFilerGender);
            NotifyOfPropertyChange(() => CurrentFilerPatientGroupID);
            NotifyOfPropertyChange(() => FilterAgeFrom);
            NotifyOfPropertyChange(() => FilterAgeTo);
            PatientCollectionView.Refresh();
        }
        public void DiscountServicePriceButton()
        {
            if (CurrentRegistration == null || CurrentRegistration.AllSavedInvoiceItem == null || CurrentRegistration.AllSavedInvoiceItem.Count == 0)
            {
                return;
            }
            IConfirmDecimal DialogView = Globals.GetViewModel<IConfirmDecimal>();
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.IsConfirmed)
            {
                if (DialogView.ConfirmValue < 0 || DialogView.ConfirmValue > 100)
                {
                    Globals.ShowMessage(eHCMSResources.Z0671_G1_GTriKgHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                ObservableCollection<ClientContractServiceItem> CurrentItemCollection = CurrentClientContract.ContractServiceItemCollection.ToObservableCollection();
                foreach(var Item in CurrentRegistration.AllSavedInvoiceItem)
                {
                    Item.InvoicePrice = Math.Ceiling(Item.ChargeableItem.NormalPrice - Item.ChargeableItem.NormalPrice * DialogView.ConfirmValue / 100);
                }
                ReInitRegistration(CurrentRegistration);
            }
        }
        public void HosClientComboBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            HosClientComboBox = sender as AxAutoComplete;
        }
        public void PrintPatientItems(HosClientContractPatient CurrentPatient)
        {
            if (CurrentPatient == null || CurrentPatient.HosContractPtID == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) =>
            {
                aView.eItem = ReportName.HosClientContractPatientSummary;
                aView.RegistrationDetailIDList = new List<long> { CurrentPatient.HosContractPtID };
            });
        }
        public void PrintPatientResults(HosClientContractPatient CurrentPatient)
        {
            if (CurrentPatient == null || CurrentPatient.PtRegistrationID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                //aView.RegistrationDetailID = CurrentMedicalExaminationResult.PtRegDetailID;
                aView.ID = CurrentPatient.PtRegistrationID.Value;
                aView.eItem = ReportName.Kham_Suc_Khoe;
                aView.IsDetails = true;
            });
        }
        public void PrintPatientItemCollection()
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null ||
                !CurrentClientContract.ContractPatientCollection.Any(x => x.HosContractPtID > 0))
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) =>
            {
                aView.eItem = ReportName.HosClientContractPatientSummary;
                aView.RegistrationDetailIDList = CurrentClientContract.ContractPatientCollection.Where(x => x.HosContractPtID > 0).Select(x => x.HosContractPtID).ToList();
            });
        }
        #endregion
        #region Handles
        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null && GetView() != null && message.Item != null)
            {
                if (CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0)
                {
                    CurrentClientContract.ContractPatientCollection = new ObservableCollection<HosClientContractPatient>();
                }
                if (CurrentClientContract.ContractPatientCollection.Any(x => x.PatientObj == message.Item))
                {
                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2439_G1_BenhNhanDaTonTai);
                    return;
                }
                var mContractPatientCollection = new ObservableCollection<HosClientContractPatient>(CurrentClientContract.ContractPatientCollection)
                {
                    new HosClientContractPatient { PatientObj = message.Item }
                };
                CurrentClientContract.ContractPatientCollection = mContractPatientCollection;
                InitPatientCollectionView();
            }
        }
        #endregion
        #region Methods
        private void CallSelectServiceOnPatientCollection(IList<HosClientContractPatient> PatientCollection
            , IList<ClientContractServiceItem> MedServiceItemCollection
            , IList<ClientContractServiceItem> KeepCurrentValueItemLinkCollection = null)
        {
            CurrentClientContract.ServiceItemPatientLinkCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => !PatientCollection.Any(i => i.PatientObj == x.ContractPatient.PatientObj) || (KeepCurrentValueItemLinkCollection != null && KeepCurrentValueItemLinkCollection.Any(i => i.MedRegItem == x.ContractMedRegItem.MedRegItem))).ToList();
            foreach (var CurrentPatient in PatientCollection)
            {
                foreach (var aServiceItem in MedServiceItemCollection)
                {
                    CurrentClientContract.ServiceItemPatientLinkCollection.Add(new ClientContractServiceItemPatientLink { ContractPatient = CurrentPatient, ContractMedRegItem = aServiceItem });
                }
                CurrentClientContract.InitContractPatientItemPrice(CurrentPatient);
            }
            foreach (var CurrentPatient in PatientCollection)
            {
                CurrentPatient.IsSelected = false;
            }
            InitSummaryTabItem();
        }
        private void ReInitRegistration(PatientRegistration CurrentRegistration)
        {
            if (CurrentRegistration != null)
            {
                foreach (var Item in CurrentRegistration.AllSavedInvoiceItem)
                {
                    Item.DisplayID = 0;
                }
                if (CurrentClientContract.ContractServiceItemCollection == null)
                {
                    CurrentClientContract.ContractServiceItemCollection = new ObservableCollection<ClientContractServiceItem>();
                }
                if (CurrentClientContract.ServiceItemPatientLinkCollection == null)
                {
                    CurrentClientContract.ServiceItemPatientLinkCollection = new List<ClientContractServiceItemPatientLink>();
                }
                List<ClientContractServiceItemPatientLink> mCurrentLinkCollection = new List<ClientContractServiceItemPatientLink>();
                if (CurrentClientContract.ServiceItemPatientLinkCollection != null && CurrentClientContract.ServiceItemPatientLinkCollection.Count > 0)
                {
                    mCurrentLinkCollection = CurrentClientContract.ServiceItemPatientLinkCollection.EntityDeepCopy().ToList();
                }
                CurrentClientContract.ContractServiceItemCollection = CurrentRegistration.AllSavedInvoiceItem.Select(x => new ClientContractServiceItem { MedRegItem = x }).ToObservableCollection();
                InitServiceCollectionView();
                CurrentClientContract.ServiceItemPatientLinkCollection = new List<ClientContractServiceItemPatientLink>();
                foreach (var aItem in mCurrentLinkCollection)
                {
                    ClientContractServiceItem mServiceItem = CurrentClientContract.ContractServiceItemCollection.FirstOrDefault(x => x.MedRegItem is PatientRegistrationDetail && aItem.ContractMedRegItem.MedRegItem is PatientRegistrationDetail && (x.MedRegItem as PatientRegistrationDetail).MedServiceID == (aItem.ContractMedRegItem.MedRegItem as PatientRegistrationDetail).MedServiceID);
                    if (mServiceItem == null)
                    {
                        mServiceItem = CurrentClientContract.ContractServiceItemCollection.FirstOrDefault(x => x.MedRegItem is PatientPCLRequestDetail && aItem.ContractMedRegItem.MedRegItem is PatientPCLRequestDetail && (x.MedRegItem as PatientPCLRequestDetail).PCLExamTypeID == (aItem.ContractMedRegItem.MedRegItem as PatientPCLRequestDetail).PCLExamTypeID);
                    }
                    if (mServiceItem == null)
                    {
                        continue;
                    }
                    CurrentClientContract.ServiceItemPatientLinkCollection.Add(new ClientContractServiceItemPatientLink { ContractPatient = CurrentClientContract.ContractPatientCollection.First(x => x.PatientObj.PatientID == aItem.ContractPatient.PatientObj.PatientID), ContractMedRegItem = mServiceItem });
                    CurrentClientContract.InitContractPatientItemPrice(CurrentClientContract.ContractPatientCollection.First(x => x.PatientObj.PatientID == aItem.ContractPatient.PatientObj.PatientID));
                }
            }
            InitSummaryTabItem();
            if (CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0 || CurrentClientContract.ServiceItemPatientLinkCollection == null || CurrentClientContract.ServiceItemPatientLinkCollection.Count == 0)
            {
                return;
            }
            foreach (var item in CurrentClientContract.ContractPatientCollection)
            {
                if (CurrentClientContract.ServiceItemPatientLinkCollection.Any(x => x.ContractPatient == item))
                {
                    CurrentClientContract.InitContractPatientItemPrice(item);
                }
            }
        }
        private void InitSummaryTabItem()
        {
            if (CurrentClientContract == null || CurrentClientContract.ContractServiceItemCollection == null || CurrentClientContract.ContractServiceItemCollection.Count == 0)
            {
                return;
            }
            if (CurrentClientContract.ContractPatientCollection == null || CurrentClientContract.ContractPatientCollection.Count == 0)
            {
                return;
            }
            DataTable mSummaryTable = new DataTable();
            mSummaryTable.Columns.AddRange(new DataColumn[] {
                new DataColumn(eHCMSResources.T3274_G1_MaBN),
                new DataColumn(eHCMSResources.T1584_G1_HoTenBN2),
                new DataColumn(eHCMSResources.K2274_G1_CVu),
                new DataColumn(eHCMSResources.T2261_G1_KhoaPh)
            });
            int mDupeColumnCount = 0;
            foreach (var item in CurrentClientContract.ContractServiceItemCollection)
            {
                string mColumnName = item.MedRegItem.ChargeableItemName;
                if (mColumnName == null)
                {
                    mColumnName = "";
                }
                mColumnName = mColumnName.Replace("(", "'").Replace(")", "'").Replace(",", "").Replace("/", " ");
                mColumnName = mColumnName.Length <= 30 || string.IsNullOrEmpty(mColumnName) ? mColumnName : mColumnName.Substring(0, 30);
                mColumnName = mColumnName.Trim();
                if (mColumnName.Count(x => x == '[') != mColumnName.Count(x => x == ']'))
                {
                    mColumnName = mColumnName.Replace("[", "").Replace("]", "");
                }
                else
                {
                    mColumnName = mColumnName.Replace("[", "'").Replace("]", "'");
                }
                if (mSummaryTable.Columns.Cast<DataColumn>().Any(x => x.ColumnName.Equals(mColumnName)))
                {
                    mDupeColumnCount++;
                    mColumnName = string.Format("{0}_{1}", mColumnName, mDupeColumnCount.ToString());
                }
                mSummaryTable.Columns.Add(mColumnName);
            }
            TotalContractAmountTemp = 0;
            foreach (var item in CurrentClientContract.ContractPatientCollection)
            {
                var mNewRow = mSummaryTable.NewRow();
                mNewRow[eHCMSResources.T3274_G1_MaBN] = item.PatientObj.PatientCode;
                mNewRow[eHCMSResources.T1584_G1_HoTenBN2] = item.PatientObj.FullName;
                mNewRow[eHCMSResources.K2274_G1_CVu] = item.ClientClassification;
                mNewRow[eHCMSResources.T2261_G1_KhoaPh] = item.ClientGroup;
                if (CurrentClientContract.ServiceItemPatientLinkCollection != null && CurrentClientContract.ServiceItemPatientLinkCollection.Count > 0)
                {
                    for (int i = 0; i < CurrentClientContract.ContractServiceItemCollection.Count; i++)
                    {
                        if (CurrentClientContract.ServiceItemPatientLinkCollection.Any(x => x.ContractPatient == item && x.ContractMedRegItem.MedRegItem == CurrentClientContract.ContractServiceItemCollection[i].MedRegItem))
                        {
                            mNewRow[i + 4] = "x";
                        }
                    }
                }
                mSummaryTable.Rows.Add(mNewRow);
                TotalContractAmountTemp += item.TotalInvoicePrice;
            }
            SummaryContractTable = new DataTable();
            SummaryContractTable = mSummaryTable;
        }
        private void GetHospitalClientsData()
        {
            this.ShowBusyIndicator();
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
                                HospitalClientCollection = mContract.EndGetHospitalClients(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        private bool IsValidClientContract()
        {
            if (string.IsNullOrEmpty(CurrentClientContract.ContractName))
            {
                MessageBox.Show(eHCMSResources.Z2670_G1_ThieuThongTinTenHopDong, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentClientContract.HosClient == null || CurrentClientContract.HosClient.HosClientID == 0)
            {
                MessageBox.Show(eHCMSResources.Z2670_G1_ThieuThongTinCongTy, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                MessageBox.Show(eHCMSResources.Z2670_G1_TaiKhoanKhongHopLe, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentClientContract.ValidDateFrom.Date >= CurrentClientContract.ValidDateTo.Date)
            {
                MessageBox.Show(eHCMSResources.Z2216_G1_TuNgayPhaiNhoHonDenNgay, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return true;
        }
        private void GetPatientPaidAmount(bool IsGetOnly = false)
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetContractPaidAmount(CurrentClientContract.HosClientContractID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var PaidCollection = mContract.EndGetContractPaidAmount(asyncResult);
                                if (PaidCollection != null)
                                {
                                    foreach (var aPaidItem in PaidCollection)
                                    {
                                        if (CurrentClientContract.ContractPatientCollection.Any(x => x.PatientObj.PatientID == aPaidItem.PatientObj.PatientID))
                                        {
                                            CurrentClientContract.ContractPatientCollection.First(x => x.PatientObj.PatientID == aPaidItem.PatientObj.PatientID).TotalContractPaidAmount = aPaidItem.TotalContractPaidAmount;
                                        }
                                    }
                                    if (!IsGetOnly)
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2715_G1_ThanhCong);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.G0140_G1_ThatBai, eHCMSResources.T0432_G1_Error);
                                }
                                NotifyOfPropertyChange(() => TotalContractPaidAmount);
                                NotifyOfPropertyChange(() => TotalFinalizedAmount);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        private void FinalizeContract(bool IsConfirmFinalized = false)
        {
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            if (!IsConfirmFinalized && CurrentClientContract.TotalContractAmount == 0)
            {
                MessageBox.Show(eHCMSResources.Z2789_G1_GiaTriTTKhongHL, eHCMSResources.T0074_G1_I);
                return;
            }
            IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
            MessBox.isCheckBox = true;
            MessBox.SetMessage(eHCMSResources.Z2789_G1_HopDongChotSauTT, eHCMSResources.K3847_G1_DongY);
            MessBox.FireOncloseEvent = true;
            GlobalsNAV.ShowDialog_V3(MessBox);
            if (MessBox.IsAccept)
            {
                this.ShowBusyIndicator();
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var mFactory = new AppointmentServiceClient())
                        {
                            var mContract = mFactory.ServiceInstance;
                            mContract.BeginFinalizeHospitalClientContract(CurrentClientContract.HosClientContractID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0), CurrentClientContract.TotalContractAmount
                                , IsConfirmFinalized
                                , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    mContract.EndFinalizeHospitalClientContract(asyncResult);
                                    if (IsConfirmFinalized || !CurrentClientContract.IsHealthyOrganization)
                                    {
                                        CurrentClientContract.FinalizedDate = Globals.GetCurServerDateTime();
                                    }
                                    ReloadCurrentClientContract(CurrentClientContract);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.HideBusyIndicator();
                        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    }
                });

                t.Start();
            }
        }
        private void ReloadCurrentClientContract(HospitalClientContract aCurrentClientContract)
        {
            if (aCurrentClientContract == null || aCurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClientContractDetails(aCurrentClientContract, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentClientContract = mContract.EndGetHospitalClientContractDetails(asyncResult);
                                InitCurrentClientContractViewDetails(CurrentClientContract);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        private void InitCurrentClientContractViewDetails(HospitalClientContract aCurrentClientContract)
        {
            CurrentRegistration = new PatientRegistration();
            SummaryContractTable = new DataTable();
            TotalContractAmountTemp = 0;
            if (aCurrentClientContract.ContractServiceItemCollection == null || aCurrentClientContract.ContractServiceItemCollection.Count == 0)
            {
                return;
            }
            foreach (var aItem in aCurrentClientContract.ContractServiceItemCollection.Select(x => x.MedRegItem))
            {
                if (aItem is PatientRegistrationDetail)
                {
                    if (CurrentRegistration.PatientRegistrationDetails == null)
                    {
                        CurrentRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                    }
                    CurrentRegistration.PatientRegistrationDetails.Add(aItem as PatientRegistrationDetail);
                }
                if (aItem is PatientPCLRequestDetail)
                {
                    if (CurrentRegistration.PCLRequests == null || CurrentRegistration.PCLRequests.Count != 1)
                    {
                        CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>
                        {
                            new PatientPCLRequest
                            {
                                PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
                                Diagnosis = eHCMSResources.Z1116_G1_ChuaXacDinh,
                                StaffID = Globals.LoggedUserAccount.StaffID,
                                V_PCLRequestType = AllLookupValues.V_PCLRequestType.NGOAI_TRU,
                                V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN,
                                RecordState = RecordState.DETACHED,
                                EntityState = Service.Core.Common.EntityState.DETACHED,
                                ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID
                            }
                        };
                    }
                    CurrentRegistration.PCLRequests[0].PatientPCLRequestIndicators.Add(aItem as PatientPCLRequestDetail);
                }
            }
            InitSummaryTabItem();
            if (aCurrentClientContract.IsFinalized || (aCurrentClientContract.ClientContractFinalizationCollection != null && aCurrentClientContract.ClientContractFinalizationCollection.Count > 0))
            {
                GetPatientPaidAmount(true);
            }
            foreach (var Item in CurrentRegistration.AllSavedInvoiceItem)
            {
                if (CurrentClientContract.ContractServiceItemCollection.Any(x => x.MedRegItem.SameAsMedItemPrimary(Item) && x.IsProcessed))
                {
                    Item.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Fixed_PriceType);
                }
                else
                {
                    Item.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Updatable_PriceType);
                }
            }
        }
        public void LoadGenderCollection()
        {
            if (Globals.allGenders != null && Globals.allGenders.Any(x => !string.IsNullOrEmpty(x.ID)))
            {
                var ItemCollection = Globals.allGenders.ToObservableCollection();
                if (ItemCollection == null)
                {
                    ItemCollection = new ObservableCollection<eHCMS.Services.Core.Gender>();
                }
                ItemCollection.Insert(0, new eHCMS.Services.Core.Gender("", ""));
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new CommonService_V2Client())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetAllGenders(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var ItemCollection = CurrentContract.EndGetAllGenders(asyncResult).ToObservableCollection();
                                if (ItemCollection == null)
                                {
                                    ItemCollection = new ObservableCollection<eHCMS.Services.Core.Gender>();
                                }
                                ItemCollection.Insert(0, new eHCMS.Services.Core.Gender("", ""));
                                GenderCollection = ItemCollection;
                                CurrentFilerGender = GenderCollection.FirstOrDefault().ID;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0692_G1_Msg_InfoKhTheLayDSGioiTinh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void InitPatientCollectionView()
        {
            PatientCollectionView = CollectionViewSource.GetDefaultView(CurrentClientContract == null || CurrentClientContract.ContractPatientCollection == null ? new ObservableCollection<HosClientContractPatient>() : CurrentClientContract.ContractPatientCollection);
            PatientCollectionView.Filter = x =>
            {
                return FilterPatient(x);
            };
        }
        private void InitServiceCollectionView()
        {
            ServiceCollectionView = CollectionViewSource.GetDefaultView(CurrentClientContract == null || CurrentClientContract.ContractServiceItemCollection == null ? new ObservableCollection<ClientContractServiceItem>() : CurrentClientContract.ContractServiceItemCollection);
            ServiceCollectionView.Filter = x =>
            {
                return FilterService(x);
            };
        }
        private void GetPatientGroupCollection(long HosClientContractID)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        var Currentontract = CurrentFactory.ServiceInstance;
                        Currentontract.BeginGetHosClientContractPatientGroups(HosClientContractID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var ItemCollection = Currentontract.EndGetHosClientContractPatientGroups(asyncResult).ToObservableCollection();
                                if (ItemCollection == null)
                                {
                                    ItemCollection = new ObservableCollection<HosClientContractPatientGroup> { new HosClientContractPatientGroup { HosClientContractPatientGroupID = 0, HosClientContractPatientGroupName = string.Empty } };
                                }
                                else
                                {
                                    ItemCollection.Insert(0, new HosClientContractPatientGroup { HosClientContractPatientGroupID = 0, HosClientContractPatientGroupName = string.Empty });
                                }
                                PatientGroupCollection = ItemCollection;
                                CurrentFilerPatientGroupID = PatientGroupCollection.FirstOrDefault().HosClientContractPatientGroupID;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private bool FilterPatient(object CurrentPatient)
        {
            var CurrentContractPatient = (HosClientContractPatient)CurrentPatient;
            return (string.IsNullOrEmpty(CurrentFilerGender) || (CurrentContractPatient.PatientObj != null && CurrentContractPatient.PatientObj.Gender.Equals(CurrentFilerGender))) &&
                (CurrentFilerPatientGroupID == 0 || (CurrentContractPatient.PatientGroupCollection != null && CurrentContractPatient.PatientGroupCollection.Any(x => x.HosClientContractPatientGroupID == CurrentFilerPatientGroupID))) &&
                (FilterAgeFrom.GetValueOrDefault(0) == 0 || (CurrentContractPatient.PatientObj != null && CurrentContractPatient.PatientObj.Age >= FilterAgeFrom.Value)) &&
                (FilterAgeTo.GetValueOrDefault(0) == 0 || (CurrentContractPatient.PatientObj != null && CurrentContractPatient.PatientObj.Age <= FilterAgeTo.Value)) &&
                (string.IsNullOrEmpty(FullNameFilterString) || (CurrentContractPatient.PatientObj != null && !string.IsNullOrEmpty(CurrentContractPatient.PatientObj.FullName) && Globals.RemoveVietnameseString(CurrentContractPatient.PatientObj.FullName).ToLower().Contains(Globals.RemoveVietnameseString(FullNameFilterString).ToLower())));
        }
        private bool FilterService(object CurrentService)
        {
            var CurrentServiceItem = (ClientContractServiceItem)CurrentService;
            return (string.IsNullOrEmpty(ServiceNameFilterString) || (CurrentServiceItem.MedRegItem != null && !string.IsNullOrEmpty(CurrentServiceItem.MedRegItem.ChargeableItemName) && Globals.RemoveVietnameseString(CurrentServiceItem.MedRegItem.ChargeableItemName).ToLower().Contains(Globals.RemoveVietnameseString(ServiceNameFilterString).ToLower())));
        }
        private void CallAddPatientCollection(string ImportFileName = null, long? HosClientContractPatientGroupID = null, ObservableCollection<MedRegItemBase> MedRegItemBaseCollection = null)
        {
            try
            {
                if (CurrentClientContract == null)
                {
                    MessageBox.Show(eHCMSResources.Z2815_G1_ThongTinHDKhongHopLe);
                    return;
                }
                if (string.IsNullOrEmpty(CurrentClientContract.ContractNo))
                {
                    MessageBox.Show(eHCMSResources.Z2814_G1_ThieuMaHD);
                    return;
                }
                if (string.IsNullOrEmpty(ImportFileName))
                {
                    OpenFileDialog objSFD = new OpenFileDialog()
                    {
                        DefaultExt = ".csv",
                        Filter = "CSV|*.csv",
                        FilterIndex = 1
                    };
                    if (objSFD.ShowDialog() != true)
                    {
                        return;
                    }
                    ImportFileName = objSFD.FileName;
                }
                List<APIPatient> ListPatient = new List<APIPatient>();
                APIPatient tmpPatient = new APIPatient();
                StreamReader sr = new StreamReader(ImportFileName);
                // 20191110 TNHX: Add regex for address has ""
                string Pattern = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
                string line = sr.ReadLine();
                string[] valueColumn = System.Text.RegularExpressions.Regex.Split(line.Substring(0, line.Length - 2), Pattern);
                valueColumn = valueColumn.Where(x => x.Length > 0 && x.ToString() != ",").ToArray();
                DataTable dt = new DataTable();
                foreach (string dc in valueColumn)
                {
                    dt.Columns.Add(new DataColumn(dc.Trim('"')));
                }
                int count = 1;
                List<Error> ErrorArr = new List<Error>();
                while (!sr.EndOfStream)
                {
                    Error temp = new Error
                    {
                        Position = count
                    };
                    List<string> ErrMess = new List<string>();
                    string itemLine = sr.ReadLine();
                    //▼====: 2011219 CMN
                    //Không hiểu lý do cắt chuỗi bỏ đi 2 ký tự cuối làm thiếu sót dữ liệu truyền vào.
                    //string[] value = System.Text.RegularExpressions.Regex.Split(itemLine.Substring(0, itemLine.Length - 2), Pattern);
                    string[] value = System.Text.RegularExpressions.Regex.Split(itemLine, Pattern);
                    //▲====:
                    value = value.Where(x => x.Length > 0 && x.ToString() != ",").ToArray();
                    if (value.Length == dt.Columns.Count && dt.Columns.Count == 5)
                    {
                        if (string.IsNullOrEmpty(value[0]))
                        {
                            ErrMess.Add("Thiếu Họ tên");
                        }
                        if (string.IsNullOrEmpty(value[1]))
                        {
                            ErrMess.Add("Thiếu Giới tính");
                        }
                        tmpPatient.FullName = value[0];
                        tmpPatient.GenderString = value[1];
                        tmpPatient.DOB = value[2];
                        tmpPatient.PatientFullStreetAddress = value[3].Replace("\"", string.Empty); //20191111 TBL: Do khi import vào trường địa chỉ có dấu " ở đầu và cuối nên cần bỏ ra
                        tmpPatient.PatientCellPhoneNumber = value[4];
                        tmpPatient.ContractPatientGroupID = HosClientContractPatientGroupID;
                        ListPatient.Add(tmpPatient);
                        tmpPatient = new APIPatient();
                        temp.ErrMess = ErrMess;
                        ErrorArr.Add(temp);
                        count++;
                    }
                    else
                    {
                        MessageBox.Show("Định dạng file sai!");
                        sr.Close();
                        return;
                    }
                }
                sr.Close();
                if (ErrorArr.Count() > 0)
                {
                    string message = "";
                    foreach (var item in ErrorArr)
                    {
                        if (item.ErrMess.Count() > 0)
                        {
                            message += "Dòng {" + item.Position + "}: ";
                            for (int i = 0; i < item.ErrMess.Count(); i++)
                            {
                                string temp = item.ErrMess[i];
                                if (i == item.ErrMess.Count() - 1)
                                {
                                    message += temp.ToString() + Environment.NewLine;
                                }
                                else
                                {
                                    message += temp.ToString() + ", ";
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                        MessBox.isCheckBox = false;
                        MessBox.SetMessage("Không thể thêm DS BN" + Environment.NewLine + message, "");
                        MessBox.ErrorTitle = eHCMSResources.G0449_G1_TBaoLoi;
                        GlobalsNAV.ShowDialog_V3(MessBox);
                    }
                }
                AddListPatient(ListPatient, true, MedRegItemBaseCollection, HosClientContractPatientGroupID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
            }
        }
        private void ChangeItemCollectioForGroup(long? PatientGroupID, ObservableCollection<MedRegItemBase> ItemCollectionForGroup)
        {
            ObservableCollection<ClientContractServiceItem> InsertedItemCollection = new ObservableCollection<ClientContractServiceItem>();
            if (ItemCollectionForGroup != null)
            {
                ObservableCollection<ClientContractServiceItem> CurrentItemCollection = CurrentClientContract.ContractServiceItemCollection.ToObservableCollection();
                foreach (var Item in ItemCollectionForGroup)
                {
                    if (CurrentItemCollection.Any(x => x.MedRegItem.SameAsMedItemPrimary(Item)))
                    {
                        InsertedItemCollection.Add(CurrentItemCollection.First(x => x.MedRegItem.SameAsMedItemPrimary(Item)));
                        continue;
                    }
                    CurrentItemCollection.Add(new ClientContractServiceItem { MedRegItem = Item });
                    InsertedItemCollection.Add(CurrentItemCollection.First(x => x.MedRegItem.SameAsMedItemPrimary(Item)));
                }
                CurrentClientContract.ContractServiceItemCollection = CurrentItemCollection;
                InitServiceCollectionView();
            }
            if (InsertedItemCollection != null && InsertedItemCollection.Count > 0 && PatientGroupID > 0)
            {
                var PatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.PatientGroupCollection != null && x.PatientGroupCollection.Any(i => i.HosClientContractPatientGroupID == PatientGroupID)).ToList();
                ObservableCollection<ClientContractServiceItemPatientLink> CurrentItemLinkCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => !PatientCollection.Any(i => i.PatientObj == x.ContractPatient.PatientObj)).ToObservableCollection();
                foreach (var PatientItem in PatientCollection)
                {
                    foreach (var Item in ItemCollectionForGroup)
                    {
                        var ContractMedRegItem = CurrentClientContract.ContractServiceItemCollection.FirstOrDefault(x => x.MedRegItem.SameAsMedItemPrimary(Item));
                        CurrentItemLinkCollection.Add(new ClientContractServiceItemPatientLink
                        {
                            ContractMedRegItem = ContractMedRegItem,
                            ContractPatient = PatientItem
                        });
                    }
                }
                CurrentClientContract.ServiceItemPatientLinkCollection = CurrentItemLinkCollection;
            }
        }
        private void ReloadCurrentContract(long? PatientGroupID = null, ObservableCollection<MedRegItemBase> ItemCollectionForGroup = null)
        {
            if (CurrentClientContract == null || CurrentClientContract.HosClientContractID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        CurrentFactory.ServiceInstance.BeginGetHospitalClientContractDetails(CurrentClientContract, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentClientContract = CurrentFactory.ServiceInstance.EndGetHospitalClientContractDetails(asyncResult);
                                if (PatientGroupID.GetValueOrDefault(0) > 0)
                                {
                                    ChangeItemCollectioForGroup(PatientGroupID, ItemCollectionForGroup);
                                }
                                InitCurrentClientContractViewDetails(CurrentClientContract);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
        //▼===== #001
        #region Import DS BN
        public void btnAddListPatient()
        {
            CallAddPatientCollection();
        }
        private void AddListPatient(List<APIPatient> ListPatient, bool IsReloadContract = false
            , ObservableCollection<MedRegItemBase> ItemCollectionForGroup = null
            , long? PatientGroupID = null)
        {
            //20191212 CMN: Sửa lại quy cách Import bệnh nhân không Import trực tiếp vào hợp đồng nữa
            bool IsImportOnlyIntoScreen = true;
            this.ShowBusyIndicator(eHCMSResources.Z1102_G1_DangLuuTTinBN);
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewListPatient(ListPatient, IsImportOnlyIntoScreen ? null : CurrentClientContract.ContractNo, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var NewListPatient = contract.EndAddNewListPatient(asyncResult);
                                if (IsImportOnlyIntoScreen)
                                {
                                    var CurrentPatientGroup = PatientGroupCollection.FirstOrDefault(x => x.HosClientContractPatientGroupID == PatientGroupID);
                                    if (NewListPatient != null && NewListPatient.Count > 0)
                                    {
                                        var PatientCollection = CurrentClientContract.ContractPatientCollection.ToObservableCollection();
                                        if (PatientCollection == null)
                                        {
                                            PatientCollection = new ObservableCollection<HosClientContractPatient>();
                                        }
                                        foreach (var Item in NewListPatient)
                                        {
                                            PatientCollection.Add(new HosClientContractPatient { PatientObj = Item, PatientGroupCollection = PatientGroupID.GetValueOrDefault(0) > 0 ? new List<HosClientContractPatientGroup> { CurrentPatientGroup } : null });
                                        }
                                        CurrentClientContract.ContractPatientCollection = PatientCollection;
                                    }
                                    if (PatientGroupID.GetValueOrDefault(0) > 0)
                                    {
                                        ChangeItemCollectioForGroup(PatientGroupID, ItemCollectionForGroup);
                                    }
                                    InitPatientCollectionView();
                                    InitCurrentClientContractViewDetails(CurrentClientContract);
                                    ReInitRegistration(CurrentRegistration);
                                }
                                else if (IsReloadContract)
                                {
                                    ReloadCurrentContract(PatientGroupID, ItemCollectionForGroup);
                                }
                                else
                                {
                                //20191109 TBL: Clear danh sách bệnh nhân cũ để không bị trùng khi hiển thị
                                //var mContractPatientCollection = new ObservableCollection<HosClientContractPatient>(CurrentClientContract.ContractPatientCollection);
                                var mContractPatientCollection = new ObservableCollection<HosClientContractPatient>();
                                    if (NewListPatient != null && NewListPatient.Count > 0 && CurrentClientContract != null)
                                    {
                                        foreach (Patient tmpPatient in NewListPatient)
                                        {
                                            mContractPatientCollection.Add(new HosClientContractPatient { PatientObj = tmpPatient });
                                        }
                                        CurrentClientContract.ContractPatientCollection = mContractPatientCollection;
                                        InitPatientCollectionView();
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                this.HideBusyIndicator();
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
        //▲===== #001
        public void UpdatePatientItemCmd(HosClientContractPatient aContractPatient, object eventArgs)
        {
            if (aContractPatient == null)
            {
                return;
            }
            Coroutine.BeginExecute(OpenPatientDetailDialog(aContractPatient.PatientObj));
        }
        private IEnumerator<IResult> OpenPatientDetailDialog(Patient SelectedPatient)
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.IsChildWindow = true;

            yield return new GenericCoRoutineTask(patientDetailsVm.InitLoadControlData_FromExt);
            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, SelectedPatient, false);
            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            GlobalsNAV.ShowDialog_V3(patientDetailsVm);

            yield break;
        }
        public void gvPatients_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void gvService_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void btnHealthExaminationRecordInfo(HosClientContractPatient CurrentPatient)
        {
            if (CurrentPatient.PtRegistrationID != null && CurrentPatient.PtRegistrationID != 0)
            {
                GlobalsNAV.ShowDialog<IHealthExaminationRecordInfo>((aView) =>
                {
                    aView.HosPatient = CurrentPatient;
                });
                InitSummaryTabItem();
            }
            else
                MessageBox.Show("Không có thông tin KSK");
        }
    }

    public class Error
    {
        public int Position { get; set; }
        public List<string> ErrMess { get; set; }
    }
}
