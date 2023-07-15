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
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using DataEntities;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IVitalSigns)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class VitalSignsViewModel : ViewModelBase, IVitalSigns
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
        public VitalSignsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            //Globals.EventAggregator.Subscribe(this);
            //thay bằng
            Globals.EventAggregator.Subscribe(this);


            authorization();
            //Khi khoi tao module thi load menu ben trai luon.
            _PtVitalSigns = new BlankRowCollectionEx<PatientVitalSign>();
            _lookupVSContextList = new ObservableCollection<Lookup>();
            _vitalSignsList = new ObservableCollection<VitalSign>();
            GetLookupVitalSignContext();
            GetAllVitalSigns();
            initdata();
            Globals.EventAggregator.Subscribe(this);
        }
        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            initdata();
        }
        public void initdata()
        {
            if (!mTongQuat_XemThongTin)
            {
                return;
            }
            _PtVitalSigns = new BlankRowCollectionEx<PatientVitalSign>();
            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatient != null
                && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                GetVitalSignsByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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

        private BlankRowCollectionEx<PatientVitalSign> _PtVitalSigns;
        public BlankRowCollectionEx<PatientVitalSign> PtVitalSigns
        {
            get
            {
                return _PtVitalSigns;
            }
            set
            {
                if (_PtVitalSigns == value)
                    return;
                _PtVitalSigns = value;
                NotifyOfPropertyChange(() => PtVitalSigns);
            }
        }

        private PatientVitalSign _selectedPtVitalSigns;
        public PatientVitalSign selectedPtVitalSigns
        {
            get
            {
                return _selectedPtVitalSigns;
            }
            set
            {
                if (_selectedPtVitalSigns == value)
                    return;
                _selectedPtVitalSigns = value;
                NotifyOfPropertyChange(() => selectedPtVitalSigns);
            }
        }

        private ObservableCollection<Lookup> _lookupVSDataTypeList;
        private ObservableCollection<Lookup> _lookupVSContextList;
        private ObservableCollection<VitalSign> _vitalSignsList;


        private VitalSign _newVitalSign;
        private PatientVitalSign _newPtVitalSign;

        public ObservableCollection<Lookup> lookupVSDataTypeList
        {
            get
            {
                return _lookupVSDataTypeList;
            }
            set
            {
                if (_lookupVSDataTypeList == value)
                    return;
                _lookupVSDataTypeList = value;
                NotifyOfPropertyChange(() => lookupVSDataTypeList);
            }
        }
        public ObservableCollection<Lookup> lookupVSContextList
        {
            get
            {
                return _lookupVSContextList;
            }
            set
            {
                if (_lookupVSContextList == value)
                    return;
                _lookupVSContextList = value;
                NotifyOfPropertyChange(() => lookupVSContextList);
            }
        }
        public ObservableCollection<VitalSign> vitalSignsList
        {
            get
            {
                return _vitalSignsList;
            }
            set
            {
                if (_vitalSignsList == value)
                    return;
                _vitalSignsList = value;
                NotifyOfPropertyChange(() => vitalSignsList);
            }
        }


        public VitalSign newVitalSign
        {
            get
            {
                return _newVitalSign;
            }
            set
            {
                if (_newVitalSign == value)
                    return;
                _newVitalSign = value;
                NotifyOfPropertyChange(() => newVitalSign);
            }
        }
        public PatientVitalSign newPtVitalSign
        {
            get
            {
                return _newPtVitalSign;
            }
            set
            {
                if (_newPtVitalSign == value)
                    return;
                _newPtVitalSign = value;
                NotifyOfPropertyChange(() => newPtVitalSign);
            }
        }
        #endregion
        #region account checking
        public bool CanlnkSaveClick
        {
            get
            {
                if (Globals.isAccountCheck)
                {
                    return mTongQuat_ChinhSuaThongTin;
                }
                else
                {
                    return true;
                }
                //return stateSave;
            }
        }
        public bool CanlnkEditClick
        {
            get
            {
                if (Globals.isAccountCheck)
                {
                    return mTongQuat_ChinhSuaThongTin;


                }
                else
                {
                    return true;
                }
                //return stateEdit;
            }
        }
        public bool CanDeleteClick
        {
            get
            {
                if (Globals.isAccountCheck)
                {
                    return mTongQuat_ChinhSuaThongTin;
                }
                else
                {
                    return true;
                }
                //return stateEdit;
            }
        }
        #endregion

        public void grdCommonRecord_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((PatientVitalSign)grdCommonRecord.SelectedItem != null)
            {
                if (grdCommonRecord.isLastRow())
                {
                    ((PatientVitalSign)grdCommonRecord.SelectedItem).isEdit = false;
                    ((PatientVitalSign)grdCommonRecord.SelectedItem).isDeleted = false;
                }
                else
                {
                    ((ObservableCollection<PatientVitalSign>)grdCommonRecord.ItemsSource)[grdCommonRecord.TotalItem() - 1].isEdit = true;
                }
            }
        }
        private bool isEdit = false;
        public void lnkEditClick(RoutedEventArgs e)
        {
            grdCommonRecord.IsReadOnly = false;
            grdCommonRecord.EditRecord();
            grdCommonRecord.BeginEdit();
            DataTemplate row = this.grdCommonRecord.RowDetailsTemplate;
            ((PatientVitalSign)grdCommonRecord.SelectedItem).isEdit = false;
            isEdit = true;
        }

        public static bool CheckValid(object temp)
        {
            PatientVitalSign u = temp as PatientVitalSign;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void lnkSaveClick(RoutedEventArgs e)
        {
            grdCommonRecord.Cancel();
            PatientVitalSign pvs = (PatientVitalSign)grdCommonRecord.SelectedItem;
            ((PatientVitalSign)grdCommonRecord.SelectedItem).isEdit = true;
            if (!CheckValid(selectedPtVitalSigns))
            {
                return;
            }
            //save o day
            if (!isEdit)
            {
                selectedPtVitalSigns.CommonMedicalRecord = new CommonMedicalRecord();
                selectedPtVitalSigns.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;//cai nay sua 
                //validate o day
                if (selectedPtVitalSigns.LookupVSignContext == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.Z0504_G1_TinhHuong), "");
                    return;
                }
                if (selectedPtVitalSigns.VitalSign == null)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0497_G1_ChuaChon, eHCMSResources.K3126_G1_DauHieuSinhTon), "");
                    return;
                }
                AddItemPtVitalSigns(selectedPtVitalSigns, 2);
            }
            else
            {
                UpdateItemPtVitalSigns(selectedPtVitalSigns, selectedPtVitalSigns.PtVSignID, 2);
            }
            isEdit = false;
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
            GetVitalSignsByPtID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        public void lnkDeleteClick(RoutedEventArgs e)
        {
            if (selectedPtVitalSigns.CommonMedRecID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteItemPtVitalSigns(selectedPtVitalSigns, 2);
            }
            isEdit = false;
        }

        #region method
        public void cboVSignContext(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = lookupVSContextList;
            }
        }
        public void cboVSignName(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = vitalSignsList;
            }
        }

        private void LoadPtVitalSigns(long patientID)
        {
            GetVitalSignsByPtID(patientID);
        }
        private void GetVitalSignsByPtID(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVitalSignsByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetVitalSignsByPtID(asyncResult);
                            if (items != null)
                            {
                                PtVitalSigns = new BlankRowCollectionEx<PatientVitalSign>();
                                foreach (var tp in items)
                                {
                                    PtVitalSigns.Add(tp);
                                }
                            }
                            NotifyOfPropertyChange(() => PtVitalSigns);
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
        //-----

        public void GetLookupVitalSignDataType()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            //▼====== #001
            //lookupVSDataTypeList.Clear();
            lookupVSDataTypeList = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.VSIGN_DATA_TYPE))
                {
                    lookupVSDataTypeList.Add(tmpLookup);
                }
            }
            //▲====== #001
            //var t = new Thread(() =>
            //{
            //    IsLoading = true;

            //    using (var serviceFactory = new ComRecordsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;



            //        contract.BeginGetLookupVitalSignDataType( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupVitalSignDataType(asyncResult);
            //                if (items != null)
            //                {
            //                    lookupVSDataTypeList.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        lookupVSDataTypeList.Add(tp);
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

        public void GetLookupVitalSignContext()
        {
            //♥====== #001
            //lookupVSContextList.Clear();
            lookupVSContextList = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.VITAL_SIGN_CONTEXT))
                {
                    lookupVSContextList.Add(tmpLookup);
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



            //        contract.BeginGetLookupVitalSignContext( Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {

            //                var items = contract.EndGetLookupVitalSignContext(asyncResult);
            //                if (items != null)
            //                {
            //                    lookupVSContextList.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        lookupVSContextList.Add(tp);
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

        public void GetAllVitalSigns()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginGetAllVitalSigns(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetAllVitalSigns(asyncResult);
                            if (items != null)
                            {
                                vitalSignsList.Clear();
                                foreach (var tp in items)
                                {
                                    vitalSignsList.Add(tp);
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

        public void AddVitalSigns(VitalSign newVitalSign)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddVitalSigns(newVitalSign, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddVitalSigns(asyncResult);

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

        public void UpdateVitalSigns(VitalSign vitalSign)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginUpdateVitalSigns(vitalSign, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateVitalSigns(asyncResult);

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

        public void DeleteVitalSigns(byte vitalSignID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteVitalSigns(vitalSignID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteVitalSigns(asyncResult);

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


        public void DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginDeleteItemPtVitalSigns(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndDeleteItemPtVitalSigns(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                GetVitalSignsByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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

        public void AddItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginAddItemPtVitalSigns(entity, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndAddItemPtVitalSigns(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                GetVitalSignsByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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

        public void UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;



                    contract.BeginUpdateItemPtVitalSigns(entity, oldPtVSignID, staffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndUpdateItemPtVitalSigns(asyncResult);
                            if (items == true)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                GetVitalSignsByPtID(Registration_DataStorage.CurrentPatient.PatientID);
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