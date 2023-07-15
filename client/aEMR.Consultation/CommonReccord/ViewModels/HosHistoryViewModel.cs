using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.Common;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IHosHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HosHistoryViewModel : ViewModelBase, IHosHistory
        , IHandle<ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>>
    {

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HosHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);
            //Khi khoi tao module thi load menu ben trai luon.
            _PtHosHistory = new BlankRowCollectionEx<HospitalizationHistory>();
            _refVAdmissionType = new ObservableCollection<Lookup>();
            _refVTreatResult = new ObservableCollection<Lookup>();
            _refVDischargeResult = new ObservableCollection<Lookup>();
            _refVHospitalType = new ObservableCollection<Lookup>();
            _RefHospital = new PagedSortableCollectionView<Hospital>();
            RefHospital.OnRefresh += new EventHandler<RefreshEventArgs>(_RefHospital_OnRefresh);

            GetLookupAdmissionType();
            GetLookupDischargeReason();
            GetLookupTreatmentResult();
            GetLookupHospitalType();
            InitPatientInfo();
            Globals.EventAggregator.Subscribe(this);

            refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            refIDC10.OnRefresh += new EventHandler<RefreshEventArgs>(refIDC10_OnRefresh);
            refIDC10.PageSize = Globals.PageSize;

        }
        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo();
        }

        void _RefHospital_OnRefresh(object sender, RefreshEventArgs e)
        {
            //GetHospitalByKey(e.Parameter, 0, _RefHospital.PageIndex, _RefHospital.PageSize);
            LoadHospitals(_RefHospital.PageIndex, _RefHospital.PageSize, true);
        }

        void refIDC10_OnRefresh(object sender, RefreshEventArgs e)
        {
            LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);
        }
        public void InitPatientInfo()
        {
            if (!mTongQuat_XemThongTin)
            {
                return;
            }
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                LoadHospitalizationHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
        //TBL: Su kien nay lam cho chi can 2 click la co the chinh sua duoc
        public void grdCommonRecord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdCommonRecord != null && grdCommonRecord.SelectedItem != null)
            {
                grdCommonRecord.BeginEdit();
            }
        }
        public void grdCommonRecordLoaded(object sender, RoutedEventArgs e)
        {
            grdCommonRecord = sender as AxDataGridNy;
            if (!mTongQuat_XemThongTin)
            {
                grdCommonRecord.IsReadOnly = true;
            }
            if (Registration_DataStorage.CurrentPatient != null)
            {
                grdCommonRecord.IsEnabled = true;
            }
            else
            {
                grdCommonRecord.IsEnabled = false;
            }
        }
        public AxDataGridNy grdCommonRecord { get; set; }
        private object _mainContent;
        public object mainContent
        {
            get
            {
                return _mainContent;
            }
            set
            {
                if (_mainContent == value)
                    return;
                _mainContent = value;
                NotifyOfPropertyChange(() => mainContent);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }


            mTongQuat_XemThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_XemThongTin, (int)ePermission.mView);
            mTongQuat_ChinhSuaThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_ChinhSuaThongTin, (int)ePermission.mView);
        }

        #region account checking

        private bool _mTongQuat_XemThongTin = true;
        private bool _mTongQuat_ChinhSuaThongTin = true && Globals.isConsultationStateEdit;

        public bool mTongQuat_XemThongTin
        {
            get
            {
                return _mTongQuat_XemThongTin;
            }
            set
            {
                if (_mTongQuat_XemThongTin == value)
                    return;
                _mTongQuat_XemThongTin = value;
            }
        }
        public bool mTongQuat_ChinhSuaThongTin
        {
            get
            {
                return _mTongQuat_ChinhSuaThongTin;
            }
            set
            {
                if (_mTongQuat_ChinhSuaThongTin == value)
                    return;
                _mTongQuat_ChinhSuaThongTin = value && Globals.isConsultationStateEdit;
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }
        public Button lnkSave { get; set; }
        public Button lnkCancel { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkSave_Loaded(object sender)
        {
            lnkSave = sender as Button;
            lnkSave.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkCancel_Loaded(object sender)
        {
            lnkCancel = sender as Button;
            lnkCancel.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }

        #endregion
        #region property

        private BlankRowCollectionEx<HospitalizationHistory> _PtHosHistory;
        public BlankRowCollectionEx<HospitalizationHistory> PtHosHistory
        {
            get
            {
                return _PtHosHistory;
            }
            set
            {
                if (_PtHosHistory == value)
                    return;
                _PtHosHistory = value;
                NotifyOfPropertyChange(() => PtHosHistory);
            }
        }
        #endregion
        #region Private member variables
        private const string ALLITEMS = "[All]";

        private ObservableCollection<Lookup> _refVAdmissionType;
        private ObservableCollection<Lookup> _refVAdmissionReason;
        private ObservableCollection<Lookup> _refVDischargeResult;
        private ObservableCollection<Lookup> _refVHospitalType;
        private ObservableCollection<Lookup> _refVReferralType;
        private ObservableCollection<Lookup> _refVTreatResult;
        private PagedSortableCollectionView<Hospital> _RefHospital;
        private HospitalizationHistory _selectedPtHosHistory;

        public ObservableCollection<Lookup> refVAdmissionType
        {
            get
            {
                return _refVAdmissionType;
            }
            set
            {
                if (_refVAdmissionType == value)
                    return;
                _refVAdmissionType = value;
                NotifyOfPropertyChange(() => refVAdmissionType);
            }
        }
        public ObservableCollection<Lookup> refVAdmissionReason
        {
            get
            {
                return _refVAdmissionReason;
            }
            set
            {
                if (_refVAdmissionReason == value)
                    return;
                _refVAdmissionReason = value;
                NotifyOfPropertyChange(() => refVAdmissionReason);
            }
        }
        public ObservableCollection<Lookup> refVDischargeResult
        {
            get
            {
                return _refVDischargeResult;
            }
            set
            {
                if (_refVDischargeResult == value)
                    return;
                _refVDischargeResult = value;
                NotifyOfPropertyChange(() => refVDischargeResult);
            }
        }
        public ObservableCollection<Lookup> refVHospitalType
        {
            get
            {
                return _refVHospitalType;
            }
            set
            {
                if (_refVHospitalType == value)
                    return;
                _refVHospitalType = value;
                NotifyOfPropertyChange(() => refVHospitalType);
            }
        }
        public ObservableCollection<Lookup> refVReferralType
        {
            get
            {
                return _refVReferralType;
            }
            set
            {
                if (_refVReferralType == value)
                    return;
                _refVReferralType = value;
                NotifyOfPropertyChange(() => refVReferralType);
            }
        }
        public ObservableCollection<Lookup> refVTreatResult
        {
            get
            {
                return _refVTreatResult;
            }
            set
            {
                if (_refVTreatResult == value)
                    return;
                _refVTreatResult = value;
                NotifyOfPropertyChange(() => refVTreatResult);
            }
        }
        public PagedSortableCollectionView<Hospital> RefHospital
        {
            get
            {
                return _RefHospital;
            }
            set
            {
                if (_RefHospital == value)
                    return;
                _RefHospital = value;
                NotifyOfPropertyChange(() => RefHospital);
            }
        }
        public HospitalizationHistory selectedPtHosHistory
        {
            get
            {
                return _selectedPtHosHistory;
            }
            set
            {
                if (_selectedPtHosHistory == value)
                    return;
                _selectedPtHosHistory = value;
                NotifyOfPropertyChange(() => selectedPtHosHistory);
            }
        }

        private PagedSortableCollectionView<DiseasesReference> _refIDC10;
        public PagedSortableCollectionView<DiseasesReference> refIDC10
        {
            get
            {
                return _refIDC10;
            }
            set
            {
                if (_refIDC10 != value)
                {
                    _refIDC10 = value;
                }
                NotifyOfPropertyChange(() => refIDC10);
            }
        }

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            private set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }
        #endregion
        private bool validateDay(string stMatch)
        {
            Regex regStr1 = new Regex(@"\d{2}/\d{2}/\d{4}");
            Regex regStr2 = new Regex(@"\d{2}/\d{4}");
            Regex regStr3 = new Regex(@"\d{4}");
            Regex regStr4 = new Regex(@"\d{2}-\d{2}-\d{4}");
            Regex regStr5 = new Regex(@"\d{2}-\d{4}");

            if (regStr1.IsMatch(stMatch))
            {
                return true;
            }
            else
                if (regStr2.IsMatch(stMatch))
            {
                return true;
            }
            else
                    if (regStr3.IsMatch(stMatch))
            {
                return true;
            }
            else
                        if (regStr4.IsMatch(stMatch))
            {
                return true;
            }
            else
                            if (regStr5.IsMatch(stMatch))
            {
                return true;
            }
            return false;
        }


        public bool CheckValid(object temp)
        {
            HospitalizationHistory u = temp as HospitalizationHistory;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void lnkSaveClick(RoutedEventArgs e)
        {
            if (!CheckValid(selectedPtHosHistory))
            {
                return;
            }
            //TBL: Luc truoc khi save thi dua vao co nhung bay gio phai kiem tra co IHID hay khong
            if (selectedPtHosHistory.HHID > 0)
            {
                if (selectedPtHosHistory.HDate != "" && validateDay(selectedPtHosHistory.HDate) == false)
                {
                    Globals.ShowMessage(eHCMSResources.Z0500_G1_NhapKieuNgayChuaDung, "");
                    ((HospitalizationHistory)grdCommonRecord.SelectedItem).isEdit = false;
                    return;
                }
                else
                {
                    UpdateHospitalization(selectedPtHosHistory, selectedPtHosHistory.HHID, 2);
                }
            }
            else
            {
                selectedPtHosHistory.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPtHosHistory.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                if (selectedPtHosHistory.LookupAdmissionType == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.T2793_G1_LoaiNhVien), "");
                    return;
                }
                if (selectedPtHosHistory.LookupTreatmentResult == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0498_G1_KetQuaChanDoan), "");
                    return;
                }
                if (selectedPtHosHistory.LookupDischargeReason == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0308_G1_LyDoXV), "");
                    return;
                }
                if (selectedPtHosHistory.FromHospital == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.K1206_G1_BV2), "");
                    return;
                }
                if (selectedPtHosHistory.DiseasesReference == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0499_G1_Benh), "");
                    return;
                }
                if (selectedPtHosHistory.GeneralDiagnoses == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.K1746_G1_CDoan), "");
                    return;
                }
                if (selectedPtHosHistory.HDate != null
                    && validateDay(selectedPtHosHistory.HDate) == false)
                {
                    Globals.ShowMessage(eHCMSResources.Z0500_G1_NhapKieuNgayChuaDung, "");

                    return;
                }
                AddNewHospitalization(selectedPtHosHistory, 2);
            }
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetHospitalizationHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPtHosHistory.CommonMedRecID == null
                || selectedPtHosHistory.CommonMedRecID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteHospitalization(selectedPtHosHistory.HHID, 2, selectedPtHosHistory.CommonMedRecID);
            }
        }

        #region method
        public void cboLookupAdmissionTypeLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refVAdmissionType;
            }
        }
        public void cboLookupTreatResultLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refVTreatResult;
            }
        }
        public void cboLookupDischargeReasonLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refVDischargeResult;
            }
        }

        AutoCompleteBox AutoHos;
        public void aucRefFromHospital_Populating(object sender, PopulatingEventArgs e)
        {
            AutoHos = (AutoCompleteBox)sender;
            SearchCriteria = e.Parameter;
            LoadHospitals(0, 10, true);
        }

        public void aucRefFromHospital_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoHos = (AutoCompleteBox)sender;
            if (AutoHos.SelectedItem != null)
            {
                selectedPtHosHistory.FromHospital = AutoHos.SelectedItem as Hospital;
            }
        }

        public void GetHospitalByKey(string SearchKey, long V_HospitalType, int pageSize, int pageIndex)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetHospitalByKey(SearchKey, V_HospitalType, pageSize, pageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetHospitalByKey(asyncResult);
                            if (items != null)
                            {
                                RefHospital = new PagedSortableCollectionView<Hospital>();
                                foreach (var tp in items)
                                {
                                    RefHospital.Add(tp);
                                }
                                NotifyOfPropertyChange(() => RefHospital);
                                //AutoHos.ItemsSource = RefHospital;
                                //AutoHos.PopulateComplete();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void LoadHospitals(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchHospitals(SearchCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            try
                            {
                                var allItems = client.EndSearchHospitals(out totalCount, asyncResult);
                                RefHospital.Clear();
                                RefHospital.TotalItemCount = totalCount;

                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        RefHospital.Add(item);
                                    }
                                }
                                NotifyOfPropertyChange(() => RefHospital);
                                AutoHos.ItemsSource = RefHospital;
                                AutoHos.PopulateComplete();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {

                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        AutoCompleteBox Auto;
        private string Name = "";
        private byte Type = 0;
        public void aucRefDiseases_Populating(object sender, PopulatingEventArgs e)
        {
            Auto = (AutoCompleteBox)sender;
            Name = e.Parameter;
            Type = 0;
            LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        }
        public void aucRefDiseases_PopulatingVN(object sender, PopulatingEventArgs e)
        {
            Auto = (AutoCompleteBox)sender;
            Name = e.Parameter;
            Type = 1;
            LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        }
        public void aucRefDiseases_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            Auto = (AutoCompleteBox)sender;
            if (Auto.SelectedItem != null)
            {
                selectedPtHosHistory.DiseasesReference = Auto.SelectedItem as DiseasesReference;
            }
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, Globals.PageSize, type
                        , Registration_DataStorage.CurrentPatient.PatientID
                        , Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime ?? Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            refIDC10.Clear();
                            refIDC10.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    refIDC10.Add(p);
                                }
                            }
                            Auto.ItemsSource = refIDC10;
                            Auto.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void LoadHospitalizationHistoryByPtID(long patientID)
        {
            GetHospitalizationHistoryByPtID(patientID);
        }
        private void GetHospitalizationHistoryByPtID(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetHospitalizationHistoryByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetHospitalizationHistoryByPtID(asyncResult);
                            if (items != null)
                            {
                                PtHosHistory = new BlankRowCollectionEx<HospitalizationHistory>();
                                foreach (var tp in items)
                                {
                                    PtHosHistory.Add(tp);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        //-------------------

        public void GetLookupAdmissionType()
        {
            //▼====== #001
            refVAdmissionType = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.ADMISSION_TYPE))
                {
                    refVAdmissionType.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading = true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupAdmissionType(Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupAdmissionType(asyncResult);
            //                if (items != null)
            //                {
            //                    refVAdmissionType.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVAdmissionType.Add(tp);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
        public void GetLookupAdmissionReason()
        {
            //▼====== #001
            refVAdmissionReason = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.ADMISSION_REASON))
                {
                    refVAdmissionReason.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading = true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupAdmissionReason( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupAdmissionReason(asyncResult);
            //                if (items != null)
            //                {
            //                    refVAdmissionReason.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVAdmissionReason.Add(tp);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
        public void GetLookupReferralType()
        {
            //▼====== #001
            refVReferralType = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.REFERRAL_TYPE))
                {
                    refVReferralType.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading=true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupReferralType(Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupReferralType(asyncResult);
            //                if (items != null)
            //                {
            //                    refVReferralType.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVReferralType.Add(tp);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
        public void GetLookupTreatmentResult()
        {
            //▼====== #001
            refVTreatResult = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.TREATMENT_RESULT))
                {
                    refVTreatResult.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading=true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupTreatmentResult( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupTreatmentResult(asyncResult);
            //                if (items != null)
            //                {
            //                    refVTreatResult.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVTreatResult.Add(tp);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
        public void GetLookupDischargeReason()
        {
            //▼====== #001
            refVDischargeResult = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.DISCHARGE_TYPE))
                {
                    refVDischargeResult.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading = true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupDischargeReason( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupDischargeReason(asyncResult);
            //                if (items != null)
            //                {
            //                    refVDischargeResult.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVDischargeResult.Add(tp);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
        public void GetLookupHospitalType()
        {
            //▼====== #001
            refVHospitalType = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.HOSPITAL_TYPE))
                {
                    refVHospitalType.Add(tmpLookup);
                }
            }
            //▲====== #001
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading=true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupHospitalType( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupHospitalType(asyncResult);
            //                if (items != null)
            //                {
            //                    refVHospitalType.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refVHospitalType.Add(tp);
            //                    }

            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //                IsLoading = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }


        public void UpdateHospitalization(HospitalizationHistory entity, long? HHID, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginUpdateHospitalization(entity, HHID, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateHospitalization(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                GetHospitalizationHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0387_G1_ChSuaBiLoi, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void DeleteHospitalization(long? HHID, long? staffID, long? commonRecordID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteHospitalization(HHID, staffID, commonRecordID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteHospitalization(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                GetHospitalizationHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.K0484_G1_XoaFail, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void AddNewHospitalization(HospitalizationHistory entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddNewHospitalization(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddNewHospitalization(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                GetHospitalizationHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A1026_G1_Msg_InfoThemFail, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
    }
}