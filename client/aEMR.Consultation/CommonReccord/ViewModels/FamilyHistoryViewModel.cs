using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using eHCMSLanguage;
using System.Windows.Input;
using aEMR.Common.BaseModel;
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IFamilyHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FamilyHistoryViewModel : ViewModelBase, IFamilyHistory
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
        public FamilyHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;



            authorization();
            //Khi khoi tao module thi load menu ben trai luon.
            _PtFamilyHis = new BlankRowCollectionEx<FamilyHistory>();
            _refFamilyRel = new ObservableCollection<Lookup>();

            GetLookupFamilyRelationship();
            InitPatientInfo();

            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);

            refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            refIDC10.OnRefresh += new EventHandler<RefreshEventArgs>(refIDC10_OnRefresh);
            refIDC10.PageSize = Globals.PageSize;

        }
        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo();
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
                GetFamilyHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        //TBL: Su kien nay lam cho chi can 2 click la co the chinh sua
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

        private BlankRowCollectionEx<FamilyHistory> _PtFamilyHis;
        public BlankRowCollectionEx<FamilyHistory> PtFamilyHis
        {
            get
            {
                return _PtFamilyHis;
            }
            set
            {
                if (_PtFamilyHis == value)
                    return;
                _PtFamilyHis = value;
                NotifyOfPropertyChange(() => PtFamilyHis);
                NotifyOfPropertyChange(() => grdCommonRecord);
            }
        }

        private FamilyHistory _SelectedPtFamilyHis;
        public FamilyHistory SelectedPtFamilyHis
        {
            get
            {
                return _SelectedPtFamilyHis;
            }
            set
            {
                if (_SelectedPtFamilyHis == value)
                    return;
                _SelectedPtFamilyHis = value;
                NotifyOfPropertyChange(() => SelectedPtFamilyHis);
            }
        }
        #endregion

        #region Private member variables
        private const string ALLITEMS = "[All]";
        private ObservableCollection<Lookup> _refFamilyRel;
        private ObservableCollection<DiseasesReference> _refDiseases;
        private DiseasesReference _selectedRefDisease;
        private FamilyHistory _selectedPtFamilyHis;

        public ObservableCollection<Lookup> refFamilyRel
        {
            get
            {
                return _refFamilyRel;
            }
            set
            {
                if (_refFamilyRel == value)
                    return;
                _refFamilyRel = value;
                NotifyOfPropertyChange(() => refFamilyRel);
            }
        }
        public ObservableCollection<DiseasesReference> refDiseases
        {
            get
            {
                return _refDiseases;
            }
            set
            {
                if (_refDiseases == value)
                    return;
                _refDiseases = value;
                NotifyOfPropertyChange(() => refDiseases);
            }
        }
        public DiseasesReference selectedRefDisease
        {
            get
            {
                return _selectedRefDisease;
            }
            set
            {
                if (_selectedRefDisease == value)
                    return;
                _selectedRefDisease = value;
                NotifyOfPropertyChange(() => selectedRefDisease);
            }
        }
        public FamilyHistory selectedPtFamilyHis
        {
            get
            {
                return _selectedPtFamilyHis;
            }
            set
            {
                if (_selectedPtFamilyHis == value)
                    return;
                _selectedPtFamilyHis = value;
                NotifyOfPropertyChange(() => selectedPtFamilyHis);
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
        #endregion
        public bool CheckValid(object temp)
        {
            FamilyHistory u = temp as FamilyHistory;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetFamilyHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        public void lnkSaveClick(RoutedEventArgs e)
        {
            ((FamilyHistory)grdCommonRecord.SelectedItem).isEdit = true;

            if (!CheckValid(selectedPtFamilyHis))
            {
                return;
            }
            if (selectedPtFamilyHis.FHCode > 0)
            {
                UpdateFamilyHistory(selectedPtFamilyHis, Registration_DataStorage.CurrentPatient.PatientID, selectedPtFamilyHis.FHCode);
            }
            else
            {
                selectedPtFamilyHis.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPtFamilyHis.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                if (selectedPtFamilyHis.FHFullName == "")
                {
                    Globals.ShowMessage(eHCMSResources.Z0494_G1_ChuaChonTen, "");
                    return;
                }
                if (selectedPtFamilyHis.LookupFamilyRelationship == null)
                {
                    Globals.ShowMessage(eHCMSResources.Z0496_G1_ChuaChonQuanHe, "");
                    return;
                }
                //20180812 TBL: Không cần kiểm tra đã điền tên bệnh chưa
                //if (selectedPtFamilyHis.DiseasesReference == null)
                //{
                //    Globals.ShowMessage(eHCMSResources.Z0495_G1_ChuaChonBenh, "");
                //    return;
                //}
                AddNewFamilyHistory(selectedPtFamilyHis, 2);
            }
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPtFamilyHis.CommonMedRecID == null
                || selectedPtFamilyHis.CommonMedRecID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
                return;
            }

            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteFamilyHistory(2, selectedPtFamilyHis.FHCode, selectedPtFamilyHis.CommonMedRecID);
            }
        }

        #region method
        public void cboLookupFamilyRelLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refFamilyRel;
            }
        }

        AutoCompleteBox Auto;
        private string Name = "";
        private byte Type = 0;
        public void aucRefDiseases_Populating(object sender, PopulatingEventArgs e)
        {
            Auto = (AutoCompleteBox)sender;
            Name = e.Parameter;
            Type = 0;
            LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);

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
                selectedPtFamilyHis.DiseasesReference = Auto.SelectedItem as DiseasesReference;
                //20180812 TBL: Lấy tên bệnh nếu người dùng chọn từ autocomplete
                Name = selectedPtFamilyHis.DiseasesReference.DiseaseNameVN;
            }
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
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
        private void LoadPhyExamByPtID(long patientID)
        {
            GetFamilyHistoryByPtID(patientID);
        }
        private void GetFamilyHistoryByPtID(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetFamilyHistoryByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetFamilyHistoryByPtID(asyncResult);
                            if (items != null)
                            {
                                PtFamilyHis = new BlankRowCollectionEx<FamilyHistory>();
                                foreach (var tp in items)
                                {
                                    PtFamilyHis.Add(tp);
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

        public void GetLookupFamilyRelationship()
        {
            //▼====== #001
            refFamilyRel = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.FAMILY_RELATIONSHIP))
                {
                    refFamilyRel.Add(tmpLookup);
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



            //        contract.BeginGetLookupFamilyRelationship(Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupFamilyRelationship(asyncResult);
            //                if (items != null)
            //                {
            //                    refFamilyRel.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refFamilyRel.Add(tp);
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
        public void GetDiseasessReferences()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetDiseasessReferences(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetDiseasessReferences(asyncResult);
                            if (items != null)
                            {
                                refDiseases = new ObservableCollection<DiseasesReference>();
                                foreach (var tp in items)
                                {
                                    refDiseases.Add(tp);
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
        public void GetDiseasessRefByICD10Code(string icd10Code)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetDiseasessRefByICD10Code(icd10Code, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetDiseasessRefByICD10Code(asyncResult);
                            if (items != null)
                            {
                                refDiseases.Clear();
                                foreach (var tp in items)
                                {
                                    refDiseases.Add(tp);
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
        public void UpdateFamilyHistory(FamilyHistory entity, long? staffID, long? FHCode)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.ShowBusyIndicator();
            entity.DiseaseNameVN = Name;
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateFamilyHistory(entity, staffID, FHCode, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateFamilyHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                GetFamilyHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                                //20180812 TBL: Khi lưu thành công thì set Name 
                                Name = "";
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
                            this.HideBusyIndicator();
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void DeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteFamilyHistory(StaffID, FHCode, CommonMedRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteFamilyHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                GetFamilyHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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
                            this.HideBusyIndicator();
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void AddNewFamilyHistory(FamilyHistory entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.ShowBusyIndicator();
            entity.DiseaseNameVN = Name;
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddNewFamilyHistory(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddNewFamilyHistory(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                GetFamilyHistoryByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                                //20180812 TBL: Khi lưu thành công thì set Name 
                                Name = "";
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
                            this.HideBusyIndicator();
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