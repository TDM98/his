using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;
using eHCMSLanguage;
using aEMR.Controls;
using aEMR.Common.BaseModel;
using System.Windows.Input;

namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IImmunizations)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImmunizationsViewModel : ViewModelBase, IImmunizations
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
        public ImmunizationsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            //Khi khoi tao module thi load menu ben trai luon.
            _PtImmuHis = new BlankRowCollectionEx<ImmunizationHistory>();
            _refImmu = new ObservableCollection<RefImmunization>();
            //20181124 TBL: Luc truoc khi vao la load het cac loai Vaccine, bay gio phai load theo MedServiceID
            //GetRefImmunization();
        }
        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo();
        }
        public void InitPatientInfo()
        {
            if (!mTongQuat_XemThongTin)
            {
                return;
            }
            _PtImmuHis = new BlankRowCollectionEx<ImmunizationHistory>();
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                GetImmunizationByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            InitPatientInfo();
            Globals.EventAggregator.Subscribe(this);
        }
        //TBL: Su kien nay lam cho chi can 2 click la co the chon chinh sua
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
            if (Registration_DataStorage.CurrentPatient != null
                && Registration_DataStorage.CurrentPatient.PatientID > 0)
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
        public bool CheckValid(object temp)
        {
            ImmunizationHistory u = temp as ImmunizationHistory;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void Save()
        {

        }
        public void lnkSaveClick(RoutedEventArgs e)
        {
            if (!CheckValid(selectedPtImmuHis))
            {
                return;
            }
            //TBL: Luc truoc khi save thi dua vao co nhung bay gio phai kiem tra co IHID hay khong
            if (selectedPtImmuHis.IHID > 0)
            {
                UpdateImmunization(selectedPtImmuHis, selectedPtImmuHis.IHID, 2);
            }
            else
            {
                selectedPtImmuHis.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPtImmuHis.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                if (selectedPtImmuHis.RefImmunization == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0501_G1_LoaiVaccine), "");
                    return;
                }
                AddNewImmunization(selectedPtImmuHis, 2);
            }
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetImmunizationByPtID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPtImmuHis.CommonMedRecID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteImmunization(selectedPtImmuHis.IHID, 2, selectedPtImmuHis.CommonMedRecID);
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

        private BlankRowCollectionEx<ImmunizationHistory> _PtImmuHis;
        public BlankRowCollectionEx<ImmunizationHistory> PtImmuHis
        {
            get
            {
                return _PtImmuHis;
            }
            set
            {
                if (_PtImmuHis == value)
                    return;
                _PtImmuHis = value;
                NotifyOfPropertyChange(() => PtImmuHis);
            }
        }
        #endregion

        #region Private member variables
        private const string ALLITEMS = "[All]";

        private ObservableCollection<RefImmunization> _refImmu;
        private ImmunizationHistory _selectedPtImmuHis;
        private RefImmunization _newRefImmu;

        public ObservableCollection<RefImmunization> refImmu
        {
            get
            {
                return _refImmu;
            }
            set
            {
                if (_refImmu == value)
                    return;
                _refImmu = value;
                NotifyOfPropertyChange(() => refImmu);
            }
        }
        public ImmunizationHistory selectedPtImmuHis
        {
            get
            {
                return _selectedPtImmuHis;
            }
            set
            {
                if (_selectedPtImmuHis == value)
                    return;
                _selectedPtImmuHis = value;
                NotifyOfPropertyChange(() => selectedPtImmuHis);
            }
        }
        public RefImmunization newRefImmu
        {
            get
            {
                return _newRefImmu;
            }
            set
            {
                if (_newRefImmu == value)
                    return;
                _newRefImmu = value;
                NotifyOfPropertyChange(() => newRefImmu);
            }
        }

        #endregion
        #region method

        public void cboRefImmunization(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refImmu;
            }
        }

        private void LoadImmunizationByPtID(long patientID)
        {
            GetImmunizationByPtID(patientID);
        }
        private void GetImmunizationByPtID(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetImmunizationByPtID(patientID, Globals.DispatchCallback((Result) =>
                    {
                        try
                        {
                            var items = contract.EndGetImmunizationByPtID(Result);
                            if (items != null)
                            {
                                PtImmuHis = new BlankRowCollectionEx<ImmunizationHistory>();
                                foreach (var tp in items)
                                {
                                    PtImmuHis.Add(tp);
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
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        ////

        public void GetRefImmunization(long MedServiceID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefImmunization(MedServiceID, Globals.DispatchCallback((Result) =>
                    {
                        try
                        {
                            var items = contract.EndGetRefImmunization(Result);
                            if (items != null)
                            {
                                refImmu.Clear();
                                foreach (var tp in items)
                                {
                                    refImmu.Add(tp);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        public void AddNewImmunization(ImmunizationHistory entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddNewImmunization(entity, staffID, Globals.DispatchCallback((Result) =>
                    {
                        try
                        {

                            var items = contract.EndAddNewImmunization(Result);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                GetImmunizationByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void UpdateImmunization(ImmunizationHistory entity, long? IHID, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateImmunization(entity, IHID, staffID, Globals.DispatchCallback((Result) =>
                    {
                        try
                        {
                            var items = contract.EndUpdateImmunization(Result);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                GetImmunizationByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void DeleteImmunization(long? IHID, long? staffID, long? CommonMedRecID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteImmunization(IHID, staffID, CommonMedRecID, Globals.DispatchCallback((Result) =>
                    {
                        try
                        {
                            var items = contract.EndDeleteImmunization(Result);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                GetImmunizationByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
                            this.DlgHideBusyIndicator();
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