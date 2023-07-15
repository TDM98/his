using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IMedicalConditions)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalConditionsViewModel : ViewModelBase, IMedicalConditions
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
        public MedicalConditionsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);

            //Khi khoi tao module thi load menu ben trai luon.
            _PtMedCond = new BlankRowCollectionEx<MedicalConditionRecord>();
            _refMedCondByType = new ObservableCollection<RefMedContraIndicationICD>();
            GetRefMedConditionsByType(0);
            InitPatientInfo();
            Globals.EventAggregator.Subscribe(this);
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
            _PtMedCond = new BlankRowCollectionEx<MedicalConditionRecord>();
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                LoadMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }


        public void grdCommonRecordLoaded(object sender, RoutedEventArgs e)
        {
            grdCommonRecord = sender as AxDataGridEx;
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
        public AxDataGridEx grdCommonRecord { get; set; }
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
        //public void authorization()
        //{
        //    if (!Globals.isAccountCheck)
        //    {
        //        return;
        //    }
        //    bEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mEdit);
        //    bAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mAdd);
        //    bDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mDelete);
        //    bView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mView);
        //    bPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mPrint);

        //    bReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mDieuKienSucKhoe, (int)ePermission.mReport);

        //}
        //#region checking account

        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;

        //public bool bEdit
        //{
        //    get
        //    {
        //        return _bEdit;
        //    }
        //    set
        //    {
        //        if (_bEdit == value)
        //            return;
        //        _bEdit = value;
        //    }
        //}
        //public bool bAdd
        //{
        //    get
        //    {
        //        return _bAdd;
        //    }
        //    set
        //    {
        //        if (_bAdd == value)
        //            return;
        //        _bAdd = value;
        //    }
        //}
        //public bool bDelete
        //{
        //    get
        //    {
        //        return _bDelete;
        //    }
        //    set
        //    {
        //        if (_bDelete == value)
        //            return;
        //        _bDelete = value;
        //    }
        //}
        //public bool bView
        //{
        //    get
        //    {
        //        return _bView;
        //    }
        //    set
        //    {
        //        if (_bView == value)
        //            return;
        //        _bView = value;
        //    }
        //}
        //public bool bPrint
        //{
        //    get
        //    {
        //        return _bPrint;
        //    }
        //    set
        //    {
        //        if (_bPrint == value)
        //            return;
        //        _bPrint = value;
        //    }
        //}

        //public bool bReport
        //{
        //    get
        //    {
        //        return _bReport;
        //    }
        //    set
        //    {
        //        if (_bReport == value)
        //            return;
        //        _bReport = value;
        //    }
        //}
        //#endregion
        //#region binding visibilty

        //public HyperlinkButton lnkDelete { get; set; }
        //public HyperlinkButton lnkEdit { get; set; }
        //public HyperlinkButton lnkSave { get; set; }
        //public HyperlinkButton lnkCancel { get; set; }
        //public void lnkDelete_Loaded(object sender)
        //{
        //    lnkDelete = sender as HyperlinkButton;
        //    lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        //}
        //public void lnkEdit_Loaded(object sender)
        //{
        //    lnkEdit = sender as HyperlinkButton;
        //    lnkEdit.Visibility = Globals.convertVisibility(bEdit);
        //}
        //public void lnkSave_Loaded(object sender)
        //{
        //    lnkSave = sender as HyperlinkButton;
        //    lnkSave.Visibility = Globals.convertVisibility(bAdd || bEdit);
        //}
        //public void lnkCancel_Loaded(object sender)
        //{
        //    lnkCancel = sender as HyperlinkButton;
        //    lnkCancel.Visibility = Globals.convertVisibility(bAdd || bEdit);
        //}
        //#endregion

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

        private BlankRowCollectionEx<MedicalConditionRecord> _PtMedCond;
        public BlankRowCollectionEx<MedicalConditionRecord> PtMedCond
        {
            get
            {
                return _PtMedCond;
            }
            set
            {
                if (_PtMedCond == value)
                    return;
                _PtMedCond = value;
                NotifyOfPropertyChange(() => PtMedCond);
            }
        }
        #endregion
        #region Private member variables
        private const string ALLITEMS = "[All]";

        private ObservableCollection<RefMedContraIndicationICD> _refMedCond;
        private ObservableCollection<RefMedContraIndicationICD> _refMedCondByType;
        private ObservableCollection<RefMedContraIndicationTypes> _refMedCondType;
        private string _medCondTypeFilterString;
        private bool _beginEditForm;
        private bool _canFilterByMedCond;
        private MedicalConditionRecord _selectedPtMedCond;
        private RefMedContraIndicationICD _newRefMedCond;

        public ObservableCollection<RefMedContraIndicationICD> refMedCond
        {
            get
            {
                return _refMedCond;
            }
            set
            {
                if (_refMedCond == value)
                    return;
                _refMedCond = value;
                NotifyOfPropertyChange(() => refMedCond);
            }
        }
        public ObservableCollection<RefMedContraIndicationICD> refMedCondByType
        {
            get
            {
                return _refMedCondByType;
            }
            set
            {
                if (_refMedCondByType == value)
                    return;
                _refMedCondByType = value;
                NotifyOfPropertyChange(() => refMedCondByType);
            }
        }
        public ObservableCollection<RefMedContraIndicationTypes> refMedCondType
        {
            get
            {
                return _refMedCondType;
            }
            set
            {
                if (_refMedCondType == value)
                    return;
                _refMedCondType = value;
                NotifyOfPropertyChange(() => refMedCondType);
            }
        }
        public string medCondTypeFilterString
        {
            get
            {
                return _medCondTypeFilterString;
            }
            set
            {
                if (_medCondTypeFilterString == value)
                    return;
                _medCondTypeFilterString = value;
                NotifyOfPropertyChange(() => medCondTypeFilterString);
            }
        }
        public bool beginEditForm
        {
            get
            {
                return _beginEditForm;
            }
            set
            {
                if (_beginEditForm == value)
                    return;
                _beginEditForm = value;
                NotifyOfPropertyChange(() => beginEditForm);
            }
        }
        public bool canFilterByMedCond
        {
            get
            {
                return _canFilterByMedCond;
            }
            set
            {
                if (_canFilterByMedCond == value)
                    return;
                _canFilterByMedCond = value;
                NotifyOfPropertyChange(() => canFilterByMedCond);
            }
        }
        public MedicalConditionRecord selectedPtMedCond
        {
            get
            {
                return _selectedPtMedCond;
            }
            set
            {
                if (_selectedPtMedCond == value)
                    return;
                _selectedPtMedCond = value;
                NotifyOfPropertyChange(() => selectedPtMedCond);
            }
        }
        public RefMedContraIndicationICD newRefMedCond
        {
            get
            {
                return _newRefMedCond;
            }
            set
            {
                if (_newRefMedCond == value)
                    return;
                _newRefMedCond = value;
                NotifyOfPropertyChange(() => newRefMedCond);
            }
        }
        #endregion
        private bool isEdit = false;
        public void lnkEditClick(RoutedEventArgs e)
        {
            grdCommonRecord.IsReadOnly = false;
            grdCommonRecord.EditRecord();
            grdCommonRecord.BeginEdit();
            DataTemplate row = this.grdCommonRecord.RowDetailsTemplate;
            ((MedicalConditionRecord)grdCommonRecord.SelectedItem).isEdit = false;
            isEdit = true;
        }
        public bool CheckValid(object temp)
        {
            MedicalConditionRecord u = temp as MedicalConditionRecord;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void lnkSaveClick(RoutedEventArgs e)
        {
            grdCommonRecord.Cancel();
            ((MedicalConditionRecord)grdCommonRecord.SelectedItem).isEdit = true;
            if (!CheckValid(selectedPtMedCond))
            {
                return;
            }
            //save o day
            if (!isEdit)
            {
                selectedPtMedCond.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPtMedCond.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;//cai nay sua 
                //validate o day
                if (selectedPtMedCond.RefMedicalCondition == null)
                {
                    MessageBox.Show(eHCMSResources.Z0383_G1_ChuaChonDKienSK);
                    return;
                }
                AddNewMedReCond(selectedPtMedCond, 2);
            }
            else
            {
                UpdateMedReCond(selectedPtMedCond, selectedPtMedCond.MCRecID, 2);
            }
            isEdit = false;
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPtMedCond.CommonMedRecID == 0)
            {
                MessageBox.Show(eHCMSResources.Z0384_G1_DongTrong);
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteMedReCond(selectedPtMedCond.MCRecID, 2, selectedPtMedCond.CommonMedRecID);
            }
            isEdit = false;
        }
        public void grdCommonRecord_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((MedicalConditionRecord)grdCommonRecord.SelectedItem != null)
            {
                if (grdCommonRecord.isLastRow())
                {
                    ((MedicalConditionRecord)grdCommonRecord.SelectedItem).isEdit = false;
                    ((MedicalConditionRecord)grdCommonRecord.SelectedItem).isDeleted = false;
                }
                else
                {
                    //((MedicalConditionRecord)((object[])(grdCommonRecord.ItemsSource))[grdCommonRecord.TotalItem()] as MedicalConditionRecord).isEdit = true;
                    ((ObservableCollection<MedicalConditionRecord>)grdCommonRecord.ItemsSource)[grdCommonRecord.TotalItem() - 1].isEdit = true;
                }
            }
        }
        #region method

        public void cborefMedCod(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refMedCondByType;
            }
        }
        private void LoadMedConditionByPtID(long patientID, int mcTypeID)
        {
            GetMedConditionByPtID(patientID, mcTypeID);
        }
        private void GetMedConditionByPtID(long patientID, int mcTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetMedConditionByPtID(patientID, mcTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetMedConditionByPtID(asyncResult);
                            if (items != null)
                            {
                                PtMedCond = new BlankRowCollectionEx<MedicalConditionRecord>();
                                foreach (var tp in items)
                                {
                                    PtMedCond.Add(tp);
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
        //---

        public void GetRefMedConditionsByType(int medCondTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetRefMedConditionsByType(medCondTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetRefMedConditionsByType(asyncResult);
                            if (items != null)
                            {
                                refMedCondByType.Clear();
                                foreach (var tp in items)
                                {
                                    refMedCondByType.Add(tp);
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


        public void GetRefMedCondType()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetRefMedCondType(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetRefMedCondType(asyncResult);
                            if (items != null)
                            {
                                refMedCondType.Clear();
                                foreach (var tp in items)
                                {
                                    refMedCondType.Add(tp);
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

        public void GetRefMedConditions()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetRefMedConditions(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetRefMedConditions(asyncResult);
                            if (items != null)
                            {
                                refMedCond.Clear();
                                foreach (var tp in items)
                                {
                                    refMedCond.Add(tp);
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

        //Dinh implement
        public void AddNewMedReCond(MedicalConditionRecord entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddNewMedReCond(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddNewMedReCond(asyncResult);
                            if (items == true)
                            {
                                MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK);
                                GetMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0386_G1_ThemBiLoi);
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
        public void UpdateMedReCond(MedicalConditionRecord entity, long? MCRecID, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginUpdateMedReCond(entity, MCRecID, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateMedReCond(asyncResult);
                            if (items == true)
                            {
                                MessageBox.Show(eHCMSResources.A0296_G1_Msg_InfoSuaOK);
                                GetMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0387_G1_ChSuaBiLoi);
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
        public void DeleteMedReCond(long? MCRecID, long? staffID, long? CommonMedRecID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteMedReCond(MCRecID, staffID, CommonMedRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteMedReCond(asyncResult);
                            if (items == true)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                GetMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0484_G1_XoaFail);
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