using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
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
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IMedicalHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalHistoryViewModel : ViewModelBase, IMedicalHistory
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
        public MedicalHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);

            authorization();
            _PtMedHis = new BlankRowCollectionEx<PastMedicalConditionHistory>();
            _refMedHis = new ObservableCollection<RefMedicalHistory>();
            _refPMHStatus = new ObservableCollection<Lookup>();
            GetRefMedHistory();
            GetLookupPMHStatus();
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
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                LoadMedCondHisByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mEdit);
        //    bAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mAdd);
        //    bDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mDelete);
        //    bView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mView);
        //    bPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mPrint);

        //    bReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
        //                                       , (int)eConsultation.mPtDashboardCommonRecs,
        //                                       (int)oConsultationEx.mTienSuBenh, (int)ePermission.mReport);

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

        private BlankRowCollectionEx<PastMedicalConditionHistory> _PtMedHis;
        public BlankRowCollectionEx<PastMedicalConditionHistory> PtMedHis
        {
            get
            {
                return _PtMedHis;
            }
            set
            {
                if (_PtMedHis == value)
                    return;
                _PtMedHis = value;
                NotifyOfPropertyChange(() => PtMedHis);
            }
        }
        #endregion
        #region Private member variables
        private const string ALLITEMS = "[All]";

        private ObservableCollection<RefMedicalHistory> _refMedHis;
        private PastMedicalConditionHistory _selectedPastMedHis;
        private RefMedicalHistory _newRefMedHis;
        private ObservableCollection<Lookup> _refPMHStatus;

        public ObservableCollection<RefMedicalHistory> refMedHis
        {
            get
            {
                return _refMedHis;
            }
            set
            {
                if (_refMedHis == value)
                    return;
                _refMedHis = value;
                NotifyOfPropertyChange(() => refMedHis);
            }
        }
        public PastMedicalConditionHistory selectedPastMedHis
        {
            get
            {
                return _selectedPastMedHis;
            }
            set
            {
                if (_selectedPastMedHis == value)
                    return;
                _selectedPastMedHis = value;
                NotifyOfPropertyChange(() => selectedPastMedHis);
            }
        }
        public RefMedicalHistory newRefMedHis
        {
            get
            {
                return _newRefMedHis;
            }
            set
            {
                if (_newRefMedHis == value)
                    return;
                _newRefMedHis = value;
                NotifyOfPropertyChange(() => newRefMedHis);
            }
        }
        public ObservableCollection<Lookup> refPMHStatus
        {
            get
            {
                return _refPMHStatus;
            }
            set
            {
                if (_refPMHStatus == value)
                    return;
                _refPMHStatus = value;
                NotifyOfPropertyChange(() => refPMHStatus);
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
            ((PastMedicalConditionHistory)grdCommonRecord.SelectedItem).isEdit = false;
            isEdit = true;
        }

        public bool CheckValid(object temp)
        {
            PastMedicalConditionHistory u = temp as PastMedicalConditionHistory;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void lnkSaveClick(RoutedEventArgs e)
        {
            grdCommonRecord.Cancel();
            ((PastMedicalConditionHistory)grdCommonRecord.SelectedItem).isEdit = true;
            if (!CheckValid(selectedPastMedHis))
            {
                return;
            }
            //save o day
            if (!isEdit)
            {
                selectedPastMedHis.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPastMedHis.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;//cai nay sua 
                //validate o day
                if (selectedPastMedHis.RefMedicalHistory == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.K0818_G1_2TienSuBenh), "");
                    return;
                }
                if (selectedPastMedHis.LookupPMHStatus == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0502_G1_TinhTrangBenh), "");
                    return;
                }
                AddNewMedicalHistory(selectedPastMedHis, 2);
            }
            else
            {
                UpdateMedicalHistory(selectedPastMedHis, 2, selectedPastMedHis.PMHID);
            }
            isEdit = false;
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetPastMedCondHisByPtID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPastMedHis.CommonMedRecID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteMedicalHistory(selectedPastMedHis.PMHID, 2, selectedPastMedHis.CommonMedRecID);
            }
            isEdit = false;
        }
        public void grdCommonRecord_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((PastMedicalConditionHistory)grdCommonRecord.SelectedItem != null)
            {
                if (grdCommonRecord.isLastRow())
                {
                    ((PastMedicalConditionHistory)grdCommonRecord.SelectedItem).isEdit = false;
                    ((PastMedicalConditionHistory)grdCommonRecord.SelectedItem).isDeleted = false;
                }
                else
                {
                    //((PastMedicalConditionHistory)((object[])(grdCommonRecord.ItemsSource))[grdCommonRecord.TotalItem()] as PastMedicalConditionHistory).isEdit = true;
                    ((ObservableCollection<PastMedicalConditionHistory>)grdCommonRecord.ItemsSource)[grdCommonRecord.TotalItem() - 1].isEdit = true;
                }
            }
        }
        #region method
        public void cboRefMedHis(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refMedHis;
            }
        }

        public void cboGridStatus(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refPMHStatus;
            }
        }

        private void LoadMedCondHisByPtID(long patientID)
        {
            GetPastMedCondHisByPtID(patientID);
        }
        private void GetPastMedCondHisByPtID(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetPastMedCondHisByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetPastMedCondHisByPtID(asyncResult);
                            if (items != null)
                            {
                                PtMedHis = new BlankRowCollectionEx<PastMedicalConditionHistory>();
                                foreach (var tp in items)
                                {
                                    PtMedHis.Add(tp);
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
        ///--------

        public void GetRefMedHistory()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetRefMedHistory(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetRefMedHistory(asyncResult);
                            if (items != null)
                            {
                                refMedHis.Clear();
                                foreach (var tp in items)
                                {
                                    refMedHis.Add(tp);
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

        public void GetLookupPMHStatus()
        {
            //▼====== #001
            //refPMHStatus.Clear();
            refPMHStatus = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.PAST_MED_HISTORY_STATUS))
                {
                    refPMHStatus.Add(tmpLookup);
                }
            }
            //▲====== #001
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    IsLoading = true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupPMHStatus( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupPMHStatus(asyncResult);
            //                if (items != null)
            //                {
            //                    refPMHStatus.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refPMHStatus.Add(tp);
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

        //dinh them

        public void AddNewMedicalHistory(PastMedicalConditionHistory entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddNewMedicalHistory(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddNewMedicalHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                GetPastMedCondHisByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
                            //Gobals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void UpdateMedicalHistory(PastMedicalConditionHistory entity, long? staffID, long? PMHID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginUpdateMedicalHistory(entity, staffID, PMHID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateMedicalHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                GetPastMedCondHisByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
        public void DeleteMedicalHistory(long? PMHID, long? staffID, long? CommonMedRecID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteMedicalHistory(PMHID, staffID, CommonMedRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteMedicalHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                GetPastMedCondHisByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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