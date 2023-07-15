using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using static aEMR.Infrastructure.Events.TransferFormEvent;
/*
* 20161224 #001 CMN: Check department for dept change only
* 20180123 #002 CMN: Added remove button
* 20180830 #003 TTM: Thêm hàm OnDeActive để xe rác hốt đi.
* 20180913 #004 TTM: Thêm cờ để xác định khi nào là popup sẽ đóng khi xóa và bỏ event TransferFormEvent.
* 20230311 #005 QTD: Đổ lại dữ liệu quốc tịch
* 20230516 #006 DatTB: 
* + Lưu trường tình trạng BN thành chuỗi
* + Chỉnh sửa màn hình thêm/sửa/xóa giấy chuyển tuyến
* + Chỉnh sửa service xóa giấy chuyển tuyến
* 20230518 #007 DatTB: Chỉnh service lưu thêm người thêm,sửa giấy chuyển tuyến
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPaperReferalFull)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PaperReferalFullViewModel : Conductor<object>, IPaperReferalFull
        , IHandle<RegistrationSelectedToTransfer>
    //, IHandle<TransferFormEvent>/*TMA*/ 
    {
        //▼====== #004 Không xài Event này nữa, vì không cần thiết. Đã gọi thông qua hàm SetCurrentInformation
        /*TMA*/
        //public void Handle(TransferFormEvent aEvent)
        //{
        //    if (aEvent != null && aEvent.Item != null)
        //    {
        //        if (aEvent.Item.TransferFormID > 0)
        //        {
        //            SearchTransferForm(FindBy != null ? FindBy.LookupID : 0, aEvent.Item.PatientFindBy);
        //        }
        //        else
        //        {
        //            SetCurrentTransferForm(aEvent.Item);
        //        }
        //    }
        //}
        /*TMA*/

        public void SetCurrentInformation(TransferFormEvent aEvent)
        {
            if (aEvent != null && aEvent.Item != null)
            {
                //20201204 TVN không hiểu có ID rồi thì mở cái cái pop up tìm kiếm để làm gì?
                //if (aEvent.Item.TransferFormID > 0)
                //{
                //    SearchTransferForm(FindBy != null ? FindBy.LookupID : 0, aEvent.Item.PatientFindBy);
                //}
                //else
                {
                    SetCurrentTransferForm(aEvent.Item);
                }
            }
        }
        private PhysicalExamination _pPhyExamItem;
        public PhysicalExamination PtPhyExamItem
        {
            get
            {
                return _pPhyExamItem;
            }
            set
            {
                if (_pPhyExamItem != value)
                {
                    _pPhyExamItem = value;
                    NotifyOfPropertyChange(() => PtPhyExamItem);
                }
            }
        }

        private TransferForm _CurrentTransferForm;
        public TransferForm CurrentTransferForm
        {
            get
            {
                return _CurrentTransferForm;
            }
            set
            {
                _CurrentTransferForm = value;
                NotifyOfPropertyChange(() => CurrentTransferForm);
                NotifyOfPropertyChange(() => IsTransferTo);
                NotifyOfPropertyChange(() => IsTransferTo_copy);//TMA
            }
        }

        /*TMA*/
        private HealthInsurance _CurrentHiItem;
        public HealthInsurance CurrentHiItem
        {
            get
            {
                return _CurrentHiItem;
            }
            set
            {
                _CurrentHiItem = value;
                NotifyOfPropertyChange(() => _CurrentHiItem);
                NotifyOfPropertyChange(() => CurrentHiItem);
            }
        }

        private Patient _CurrentPatient;
        public Patient CurrentPatient
        {
            get
            {
                return _CurrentPatient;
            }
            set
            {
                _CurrentPatient = value;
                NotifyOfPropertyChange(() => _CurrentPatient);
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }
        private PaperReferal _ConfirmedPaperReferal;
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _ConfirmedPaperReferal;
            }
            set
            {
                _ConfirmedPaperReferal = value;
                NotifyOfPropertyChange(() => _ConfirmedPaperReferal);
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }
        //
        private PatientRegistration _PtRegistration;
        public PatientRegistration PtRegistration
        {
            get
            {
                return _PtRegistration;
            }
            set
            {
                _PtRegistration = value;
                NotifyOfPropertyChange(() => _PtRegistration);
                NotifyOfPropertyChange(() => PtRegistration);
            }
        }
        /*TMA*/

        private IHospitalAutoCompleteListing _FromHospitalAutoCnt;
        public IHospitalAutoCompleteListing FromHospitalAutoCnt
        {
            get { return _FromHospitalAutoCnt; }
            set
            {
                _FromHospitalAutoCnt = value;
                NotifyOfPropertyChange(() => FromHospitalAutoCnt);
            }
        }

        private IHospitalAutoCompleteListing _ToHospitalAutoCnt;
        public IHospitalAutoCompleteListing ToHospitalAutoCnt
        {
            get { return _ToHospitalAutoCnt; }
            set
            {
                _ToHospitalAutoCnt = value;
                NotifyOfPropertyChange(() => ToHospitalAutoCnt);
            }
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;
        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        public void ActiveContentCmd()
        {
            // HospitalAutoCompleteConten
            var hospitalAutoCompleteVm1 = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm1.IsPaperReferal = true;
            hospitalAutoCompleteVm1.Parent = this;
            FromHospitalAutoCnt = hospitalAutoCompleteVm1;
            FromHospitalAutoCnt.InitBlankBindingValue();
            _eventArg.Subscribe(this);

            var hospitalAutoCompleteVm2 = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm2.IsPaperReferal = true;
            hospitalAutoCompleteVm1.Parent = this;
            ToHospitalAutoCnt = hospitalAutoCompleteVm2;
            ToHospitalAutoCnt.InitBlankBindingValue();
            _eventArg.Subscribe(this);

            // DepartmentContent
            _eventArg.Subscribe(this);
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.LoadData();

            // SearchRegistrationContent
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            searchPatientAndRegVm.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            searchPatientAndRegVm.CloseRegistrationFormWhenCompleteSelection = false;

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            searchPatientAndRegVm.IsSearchGoToKhamBenh = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);
        }

        private string _TitleForm = eHCMSResources.T1205_G1_GCVien;
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
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PaperReferalFullViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            TitleForm = Globals.TitleForm;
            ActiveContentCmd();
            LoadCriterialTypes();
            LoadCountries();
            LoadEthnicsList();
            //▼====: #005
            Coroutine.BeginExecute(DoNationalityListList());
            //▲====: #005
            GetAllLookupValuesForTransferForm();
            CurrentTransferForm = new TransferForm
            {
                FromDate = Globals.GetCurServerDateTime().Date,
                ToDate = Globals.GetCurServerDateTime().Date,
                TransferDate = Globals.GetCurServerDateTime(),
            };

            SetTxfrDefValues();
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                NotifyOfPropertyChange(() => Countries);
            }
        }

        private ObservableCollection<Lookup> _ethnicsList;
        public ObservableCollection<Lookup> EthnicsList
        {
            get { return _ethnicsList; }
            set
            {
                _ethnicsList = value;
                NotifyOfPropertyChange(() => EthnicsList);
            }
        }

        private ObservableCollection<eHCMS.Services.Core.Gender> _genders;
        public ObservableCollection<eHCMS.Services.Core.Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
            }
        }

        private ObservableCollection<Lookup> _TreatmentResultList;
        public ObservableCollection<Lookup> TreatmentResultList
        {
            get { return _TreatmentResultList; }
            set
            {
                _TreatmentResultList = value;
                NotifyOfPropertyChange(() => TreatmentResultList);
            }
        }

        private ObservableCollection<Lookup> _TransferReasonList;
        public ObservableCollection<Lookup> TransferReasonList
        {
            get { return _TransferReasonList; }
            set
            {
                _TransferReasonList = value;
                NotifyOfPropertyChange(() => TransferReasonList);
            }
        }

        private ObservableCollection<Lookup> _TransferTypeList;
        public ObservableCollection<Lookup> TransferTypeList
        {
            get { return _TransferTypeList; }
            set
            {
                _TransferTypeList = value;
                NotifyOfPropertyChange(() => TransferTypeList);
            }
        }

        private ObservableCollection<Lookup> _CMKTList;
        public ObservableCollection<Lookup> CMKTList
        {
            get { return _CMKTList; }
            set
            {
                _CMKTList = value;
                NotifyOfPropertyChange(() => CMKTList);
            }
        }

        private ObservableCollection<Lookup> _PatientStatusList;
        public ObservableCollection<Lookup> PatientStatusList
        {
            get { return _PatientStatusList; }
            set
            {
                _PatientStatusList = value;
                NotifyOfPropertyChange(() => PatientStatusList);
            }
        }
        public void LoadEthnicsList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.ETHNIC,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    EthnicsList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
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
            t.Start();
        }

        void SetTxfrDefValues()
        {
            if (V_TransferFormType == (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN)
            {
                CurrentTransferForm.V_TransferTypeID = 62601;
                CurrentTransferForm.V_PatientStatusID = 63001;
                CurrentTransferForm.V_TransferReasonID = 62902;
                CurrentTransferForm.V_TreatmentResultID = 62701;
            }
            else
            {

            }
        }

        public void GetAllLookupValuesForTransferForm()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesForTransferForm(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesForTransferForm(asyncResult);

                                    TreatmentResultList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_TreatmentResult)) : null;
                                    TransferTypeList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransferType)) : null;
                                    TransferReasonList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransferReason)) : null;
                                    CMKTList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_CMKT)) : null;
                                    PatientStatusList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_PatientStatus)) : null;
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
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
            t.Start();
        }
        public void LoadCountries()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllCountries(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllCountries(asyncResult);

                                Countries = allItems != null ? new ObservableCollection<RefCountry>(allItems) : null;
                            }
                            catch (Exception ex1)
                            {
                                ClientLoggerHelper.LogInfo(ex1.ToString());
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
            t.Start();
        }
        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }


        public void InitValueBeforeSave()
        {
            if (CurrentTransferForm == null || CurrentTransferForm.CurPatientRegistration == null || CurrentTransferForm.CurPatientRegistration.Patient == null
                || CurrentTransferForm.CurPatientRegistration.Patient.PatientID <= 0)
            {
                return;
            }
            if (CurrentTransferForm.V_TreatmentResultID > 0 && TreatmentResultList.Any(x => x.LookupID == CurrentTransferForm.V_TreatmentResultID))
            {
                CurrentTransferForm.TreatmentResult = TreatmentResultList.FirstOrDefault(x => x.LookupID == CurrentTransferForm.V_TreatmentResultID).ObjectValue;
            }
            if (CurrentTransferForm.V_TransferReasonID > 0 && TransferReasonList.Any(x => x.LookupID == CurrentTransferForm.V_TransferReasonID))
            {
                CurrentTransferForm.TransferReason = TransferReasonList.FirstOrDefault(x => x.LookupID == CurrentTransferForm.V_TransferReasonID).ObjectValue;
            }
            if (CurrentTransferForm.V_TransferTypeID > 0 && TransferTypeList.Any(x => x.LookupID == CurrentTransferForm.V_TransferTypeID))
            {
                CurrentTransferForm.TransferType = TransferTypeList.FirstOrDefault(x => x.LookupID == CurrentTransferForm.V_TransferTypeID).ObjectValue;
            }
            //▼==== #006
            //if (CurrentTransferForm.V_PatientStatusID > 0 && PatientStatusList.Any(x => x.LookupID == CurrentTransferForm.V_PatientStatusID))
            //{
            //    CurrentTransferForm.PatientStatus = PatientStatusList.FirstOrDefault(x => x.LookupID == CurrentTransferForm.V_PatientStatusID).ObjectValue;
            //}
            //▲==== #006
            if (DepartmentContent.SelectedItem != null)
            {
                CurrentTransferForm.TransferFromDept = DepartmentContent.SelectedItem;
            }
            else
            {
                CurrentTransferForm.TransferFromDept = null;
            }
            if (FromHospitalAutoCnt.SelectedHospital != null)
            {
                CurrentTransferForm.TransferFromHos = FromHospitalAutoCnt.SelectedHospital;
            }
            else
            {
                CurrentTransferForm.TransferFromHos = null;
            }
            if (ToHospitalAutoCnt.SelectedHospital != null)
            {
                CurrentTransferForm.TransferToHos = ToHospitalAutoCnt.SelectedHospital;
            }
            else
            {
                CurrentTransferForm.TransferToHos = null;
            }
            CurrentTransferForm.V_TransferFormType = V_TransferFormType;
        }

        public bool CheckValidBeforeSave()
        {
            //▼==== #006
            if (IsHiReportOrDischarge)
            {
                MessageBox.Show(eHCMSResources.Z3320_G1_ChTuyenHiReportOrDischarge, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▲==== #006
            if (V_TransferFormType == (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN && (FromHospitalAutoCnt.SelectedHospital == null || FromHospitalAutoCnt.SelectedHospital.HosID <= 0))
            {
                MessageBox.Show("Vui lòng chọn bệnh viện tuyến trước!");
                return false;
            }
            return true;
        }

        public void btnSave_Click()
        {
            if (CheckValidBeforeSave() == false)
            {
                return;
            }
            InitValueBeforeSave();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveTransferForm(CurrentTransferForm, Globals.LoggedUserAccount.Staff.StaffID //==== #007
                    , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            TransferForm result = contract.EndSaveTransferForm(asyncResult);
                            if (result != null)
                            {
                                CurrentTransferForm = result.DeepCopy();
                                if (CurrentTransferForm.TransferFromDept != null && CurrentTransferForm.TransferFromDept.DeptID > 0)
                                {
                                    DepartmentContent.SetSelectedDeptItem(CurrentTransferForm.TransferFromDept.DeptID);
                                }
                                FromHospitalAutoCnt.SelectedHospital = new Hospital
                                {
                                    HosID = CurrentTransferForm.TransferFromHos.HosID,
                                    HosName = CurrentTransferForm.TransferFromHos.HosName,
                                    HICode = CurrentTransferForm.TransferFromHos.HICode,
                                    HospitalCode = CurrentTransferForm.TransferFromHos.HospitalCode
                                };
                                ToHospitalAutoCnt.SelectedHospital = new Hospital
                                {
                                    HosID = CurrentTransferForm.TransferToHos.HosID,
                                    HosName = CurrentTransferForm.TransferToHos.HosName,
                                    HICode = CurrentTransferForm.TransferToHos.HICode,
                                    HospitalCode = CurrentTransferForm.TransferToHos.HospitalCode
                                };
                                V_TransferFormType = CurrentTransferForm.V_TransferFormType;

                                /*TMA*/
                                if (V_TransferFormType == (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN)
                                {
                                    ConfirmedPaperReferal.TransferFormID = (long)CurrentTransferForm.TransferFormID;
                                    ConfirmedPaperReferal.TransferNum = CurrentTransferForm.TransferNum;
                                }
                                /*TMA*/

                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                Globals.EventAggregator.Publish(new OnChangedPaperReferal() { TransferForm = CurrentTransferForm });
                                Globals.EventAggregator.Publish(new HaveTransferForm() { IsHaveTransferForm = CurrentTransferForm.TransferFormID > 0 });
                                //▼==== #006
                                TransferFormSaveTitle = eHCMSResources.T1484_G1_HChinh;
                                //▲==== #006
                            }
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }

        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return _refIDC10List;
            }
            set
            {
                if (_refIDC10List != value)
                {
                    _refIDC10List = value;
                }
                NotifyOfPropertyChange(() => refIDC10List);
            }
        }

        private DiagnosisIcd10Items _refIDC10Item;
        public DiagnosisIcd10Items refIDC10Item
        {
            get
            {
                return _refIDC10Item;
            }
            set
            {
                if (_refIDC10Item != value)
                {
                    _refIDC10Item = value;
                }
                NotifyOfPropertyChange(() => refIDC10Item);
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

        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , (long)CurrentTransferForm.CurPatientRegistration.PatientID
                        , CurrentTransferForm.CurPatientRegistration.ExamDate
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
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
                            if (type == 0)
                            {
                                Auto.ItemsSource = refIDC10;
                                Auto.PopulateComplete();
                            }
                            else
                            {
                                AutoName.ItemsSource = refIDC10;
                                AutoName.PopulateComplete();
                            }

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

        AutoCompleteBox Auto;
        AutoCompleteBox DiseasesName;
        private string Name = "";
        private byte Type = 0;
        public void DiseaseName_Loaded(object sender, RoutedEventArgs e)
        {
            DiseasesName = (AutoCompleteBox)sender;
        }
        private string _typedText;
        public string TypedText
        {
            get { return _typedText; }
            set
            {
                _typedText = value.ToUpper();
                NotifyOfPropertyChange(() => TypedText);
            }
        }
        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (IsCode)
            {
                e.Cancel = true;
                Auto = (AutoCompleteBox)sender;
                Name = e.Parameter;
                Type = 0;
                refIDC10.PageIndex = 0;
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    CurrentTransferForm.ICD10Final = null;
                    CurrentTransferForm.DiagnosisTreatment_Final = null;
                    return;
                }
                LoadRefDiseases(e.Parameter, 0, 0, refIDC10.PageSize);
            }
        }

        AutoCompleteBox AutoName;
        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode && ColumnIndex == 1)
            {
                e.Cancel = true;
                AutoName = (AutoCompleteBox)sender;
                Name = e.Parameter;
                Type = 1;
                refIDC10.PageIndex = 0;
                if (string.IsNullOrEmpty(e.Parameter))
                {
                    CurrentTransferForm.ICD10Final = null;
                    CurrentTransferForm.DiagnosisTreatment_Final = null;
                    return;
                }
                LoadRefDiseases(e.Parameter, 1, 0, refIDC10.PageSize);
            }
        }

        public void AutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsCode)
            {
                Auto = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = Auto.SelectedItem as DiseasesReference;
                }
            }
        }

        //private bool isDropDown = false;
        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //isDropDown = true;
        }

        //private bool isDiseaseDropDown = false;
        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //isDiseaseDropDown = true;
        }

        public void AutoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsCode)
            {
                AutoName = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = AutoName.SelectedItem as DiseasesReference;
                }
            }
        }
        public void AutoName_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            if (!IsCode)
            {
                AutoName = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = AutoName.SelectedItem as DiseasesReference;
                }
            }
        }
        bool IsCode = true;
        int ColumnIndex = 0;


        private bool Equal(DiagnosisIcd10Items a, DiagnosisIcd10Items b)
        {
            return a.DiagIcd10ItemID == b.DiagIcd10ItemID
                && a.DiagnosisIcd10ListID == b.DiagnosisIcd10ListID
                && a.ICD10Code == b.ICD10Code
                && a.IsMain == b.IsMain
                && a.IsCongenital == b.IsCongenital
                && (a.LookupStatus != null && b.LookupStatus != null
                    && a.LookupStatus.LookupID == b.LookupStatus.LookupID);
        }

        public void Handle(RegistrationSelectedToTransfer message)
        {
            if (message == null || message.PtRegistration == null || message.PtRegistration.PtRegistrationID <= 0)
            {
                return;
            }
            Coroutine.BeginExecute(CreatNewBlankFormForRegistration(message.PtRegistration.PtRegistrationID, (int)message.PatientFindBy, V_TransferFormType));
        }

        private int _V_TransferFormType;
        public int V_TransferFormType
        {
            get
            {
                return _V_TransferFormType;
            }
            set
            {
                _V_TransferFormType = value;
                NotifyOfPropertyChange(() => V_TransferFormType);
                NotifyOfPropertyChange(() => IsTransferTo);
                NotifyOfPropertyChange(() => IsTransferTo_copy);//TMA
            }
        }
        /*TMA*/
        private bool _IsReadOnlySCT = true;

        public bool IsReadOnlySCT
        {
            get
            {
                if (V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_Di || V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS)
                    _IsReadOnlySCT = true;
                else
                    _IsReadOnlySCT = false;
                return _IsReadOnlySCT;
            }
            set
            {
                if (_IsReadOnlySCT == value)
                    return;
                _IsReadOnlySCT = value;
                NotifyOfPropertyChange(() => IsReadOnlySCT);
            }
        }
        /*TMA*/
        public bool IsTransferTo
        {
            get { return (V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_Di || V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS); }
        }
        /*TMA*/
        public bool IsTransferTo_copy
        {
            get { return (V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_DEN); }
        }
        /*TMA*/
        private bool _CanEditRefDepartment = true;
        public bool CanEditRefDepartment
        {
            get
            {
                if (V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_Di || V_TransferFormType == (long)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS)
                {
                    if (_CurrentTransferForm.PatientFindBy == 1)
                        _CanEditRefDepartment = true;
                    else
                        if (_CurrentTransferForm.PatientFindBy == 0)
                        _CanEditRefDepartment = false;
                }
                return _CanEditRefDepartment;
            }
            set
            {
                if (_CanEditRefDepartment == value)
                    return;
                _CanEditRefDepartment = value;
                NotifyOfPropertyChange(() => CanEditRefDepartment);
            }
        }

        /*TMA*/

        private IEnumerator<IResult> CreatNewBlankFormForRegistration(long PtRegistrationID, int PatientFindBy, long V_TransferFormType)
        {
            yield return GenericCoRoutineTask.StartTask(CreateTransferFormAction, PtRegistrationID, PatientFindBy, V_TransferFormType);
        }

        private void CreateTransferFormAction(GenericCoRoutineTask genTask, object PtRegistrationID, object PatientFindBy, object V_TransferFormType)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCreateBlankTransferFormByRegID((long)PtRegistrationID, (int)PatientFindBy, (long)V_TransferFormType,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                TransferForm NewTransferForm = contract.EndCreateBlankTransferFormByRegID(asyncResult);
                                if (NewTransferForm != null)
                                {
                                    CurrentTransferForm = NewTransferForm.DeepCopy();
                                    CurrentTransferForm.FromDate = Globals.GetCurServerDateTime().Date;
                                    CurrentTransferForm.ToDate = Globals.GetCurServerDateTime().Date;
                                    CurrentTransferForm.TransferDate = Globals.GetCurServerDateTime();

                                    if ((int)PatientFindBy == (int)AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        CurrentTransferForm.FromDate = CurrentTransferForm.CurPatientRegistration.ExamDate;
                                        CurrentTransferForm.TransferFromDept = new RefDepartment();
                                        CurrentTransferForm.TransferFromDept.DeptID = CurrentTransferForm.CurPatientRegistration != null && CurrentTransferForm.CurPatientRegistration.DeptID.HasValue
                                            ? CurrentTransferForm.CurPatientRegistration.DeptID.GetValueOrDefault() : 0;
                                    }
                                    else
                                    {
                                        CurrentTransferForm.FromDate = CurrentTransferForm.CurPatientRegistration.AdmissionDate.GetValueOrDefault();
                                        CurrentTransferForm.TransferFromDept = new RefDepartment();
                                        CurrentTransferForm.TransferFromDept.DeptID = CurrentTransferForm.CurPatientRegistration != null && CurrentTransferForm.CurPatientRegistration.AdmDeptID > 0
                                            ? CurrentTransferForm.CurPatientRegistration.AdmDeptID : 0;
                                    }
                                    CurrentTransferForm.ICD10 = "";
                                    CurrentTransferForm.DiagnosisTreatment = "";
                                    CurrentTransferForm.DiagnosisTreatment_Final = "";
                                    CurrentTransferForm.ICD10Final = "";
                                    if (CurrentTransferForm.LastDiagnosisTreatment != null)
                                    {
                                        CurrentTransferForm.DiagnosisTreatment_Final = CurrentTransferForm.LastDiagnosisTreatment.DiagnosisFinal;
                                        CurrentTransferForm.ClinicalSign = CurrentTransferForm.LastDiagnosisTreatment.Diagnosis;
                                    }
                                    if (CurrentTransferForm.ICD10Main != null)
                                    {
                                        CurrentTransferForm.ICD10Final = CurrentTransferForm.ICD10Main.ICD10Code;
                                    }
                                    FromHospitalAutoCnt.InitBlankBindingValue();
                                    ToHospitalAutoCnt.InitBlankBindingValue();
                                    DepartmentContent.SelectedItem = null;
                                    CurrentTransferForm.DiagnosisTreatment = null;
                                    CurrentTransferForm.PatientFindBy = (int)PatientFindBy;
                                    StrHIStatus = CurrentTransferForm.CurPatientRegistration.HisID.HasValue && CurrentTransferForm.CurPatientRegistration.HisID.Value > 0 ? "Hưởng BHYT" : "Không hưởng BHYT";
                                }
                            }
                            catch (Exception ex)
                            {
                                //ClientLoggerHelper.LogError(ex.Message);
                                MessageBox.Show(ex.Message);/*TMA*/
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();/*TMA*/
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private string _TextCriterial;
        public string TextCriterial
        {
            get
            {
                return _TextCriterial;
            }
            set
            {
                _TextCriterial = value;
                NotifyOfPropertyChange(() => TextCriterial);
            }
        }

        public void btnSearch_Click()
        {
            if (V_TransferFormType == 0 || (FindBy != null && (string.IsNullOrEmpty(TextCriterial) || string.IsNullOrWhiteSpace(TextCriterial))))
            {
                MessageBox.Show("Vui lòng thiết lập đầy đủ điều kiện tìm kiếm!");
                return;
            }
            int PatientFindBy = IsOutPatient ? (int)AllLookupValues.PatientFindBy.NGOAITRU : (int)AllLookupValues.PatientFindBy.NOITRU;
            SearchTransferForm(FindBy != null ? FindBy.LookupID : 0, PatientFindBy);
        }

        /*TMA 26/10/2017 - TÌM KIẾM LỊCH SỬ GIẤY CHUYỂN TUYẾN*/
        //private void SearchTransferFormOld(object FindBy, object PatientFindBy)
        //{
        //    this.ShowBusyIndicator();

        //    var t = new Thread(() =>
        //    {

        //        try
        //        {
        //            using (var serviceFactory = new ePMRsServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                /*TMA*/
        //                //if
        //                contract.BeginGetTransferForm(string.IsNullOrEmpty(TextCriterial) ? "" : TextCriterial.ToString(),
        //                (long)FindBy, (int)V_TransferFormType, (int)PatientFindBy, ConfirmedPaperReferal == null ? null : (long?)this.ConfirmedPaperReferal.TransferFormID,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {

        //                        try
        //                        {
        //                            IList<TransferForm> TransferFormList = contract.EndGetTransferForm(asyncResult);
        //                            if (TransferFormList == null || TransferFormList.Count <= 0)
        //                            {
        //                                if (this.ConfirmedPaperReferal == null || this.ConfirmedPaperReferal.TransferFormID <= 0)/*TMA*/
        //                                    MessageBox.Show("Không tìm thấy giấy chuyển tuyến theo điều kiện đã cho!");

        //                            }
        //                            else
        //                            {
        //                                if (TransferFormList.Count() == 1)
        //                                {
        //                                    SetCurrentTransferForm(TransferFormList[0]);
        //                                }
        //                                else
        //                                {
        //                                    var vm = Globals.GetViewModel<ITransferFormList>();
        //                                    vm.TransferFormList = TransferFormList.ToObservableCollection();
        //                                    vm.V_TransferFormType = V_TransferFormType;
        //                                    vm.Parent = this;
        //                                    Globals.ShowDialog(vm as Conductor<object>);
        //                                }
        //                            }
        //                            TextCriterial = "";
        //                            this.HideBusyIndicator();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ClientLoggerHelper.LogError(ex.Message);
        //                            this.HideBusyIndicator();
        //                        }
        //                    }), null);

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //            ClientLoggerHelper.LogError(ex.Message);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}
        /*TMA 26/10/2017 - TÌM KIẾM LỊCH SỬ GIẤY CHUYỂN TUYẾN*/
        public void SetCurrentTransferForm(TransferForm item)
        {
            CurrentTransferForm = item;
            /*TMA - 28/9/2017 : mặc định đối tượng chuyển đến có bảo hiển y tế - được hưởng BHYT*/
            if (CurrentTransferForm.CurPatientRegistration != null)
            {
                /*TMA - 06/10/2017 - XÉT ĐỐI TƯỢNG BHYT*/
                if (V_TransferFormType == 2) //CHUYỂN ĐẾN
                {
                    StrHIStatus = "Hưởng BHYT";
                }
                else //CHUYỂN ĐI - ĐI CLS
                {
                    StrHIStatus = CurrentTransferForm.CurPatientRegistration.HisID.HasValue && CurrentTransferForm.CurPatientRegistration.HisID.Value > 0 ? "Hưởng BHYT" : "Không hưởng BHYT";
                }
                /*TMA - 28/9/2017 : dòng này chị Huyền làm - cảm thấy không đúng nên bỏ */
                // StrHIStatus = CurrentTransferForm.CurPatientRegistration.HisID.HasValue && CurrentTransferForm.CurPatientRegistration.HisID.Value > 0 ? "Hưởng BHYT" : "Không hưởng BHYT";
            }
            if (CurrentTransferForm.TransferFromDept != null && CurrentTransferForm.TransferFromDept.DeptID > 0)
            {
                DepartmentContent.SetSelectedDeptItem(CurrentTransferForm.TransferFromDept.DeptID);
            }
            FromHospitalAutoCnt.SelectedHospital = new Hospital
            {
                HosID = CurrentTransferForm.TransferFromHos.HosID,
                HosName = CurrentTransferForm.TransferFromHos.HosName,
                HICode = CurrentTransferForm.TransferFromHos.HICode,
                HospitalCode = CurrentTransferForm.TransferFromHos.HospitalCode
            };
            ToHospitalAutoCnt.SelectedHospital = new Hospital
            {
                HosID = CurrentTransferForm.TransferToHos.HosID,
                HosName = CurrentTransferForm.TransferToHos.HosName,
                HICode = CurrentTransferForm.TransferToHos.HICode,
                HospitalCode = CurrentTransferForm.TransferToHos.HospitalCode
            };
            V_TransferFormType = CurrentTransferForm.V_TransferFormType;
            if (CurrentTransferForm.TransferFormID == 0 && CurrentTransferForm.TransferFromHos.HosID >= 0 && CurrentTransferForm.TransferFromHos.HosName == null)
            {
                LoadHospital(CurrentTransferForm.TransferFromHos.HICode);
            }
            if (IsThisViewDialog && CurrentTransferForm.TransferFormID >0 )
            {
                IsNew = false;
                TransferFormSaveTitle = eHCMSResources.T1484_G1_HChinh; //==== #006
            }
            else
            {
                IsNew = true;
                TransferFormSaveTitle = eHCMSResources.T2937_G1_Luu; //==== #006
            }
        }

        private void SearchTransferForm(object FindBy, object PatientFindBy)
        {
            if (string.IsNullOrEmpty(TextCriterial) && ConfirmedPaperReferal == null)
            {
                Action<ITransferFormList> onInitDlg = delegate (ITransferFormList vm)
                {
                    vm.TransferFormList = new ObservableCollection<TransferForm>();
                    vm.V_TransferFormType = V_TransferFormType;
                    vm.ParentReferralPaper = this;
                };
                GlobalsNAV.ShowDialog<ITransferFormList>(onInitDlg);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {

                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        /*TMA*/
                        //if
                        contract.BeginGetTransferForm(string.IsNullOrEmpty(TextCriterial) ? "" : TextCriterial.ToString(),
                        (long)FindBy, (int)V_TransferFormType, (int)PatientFindBy, ConfirmedPaperReferal == null ? null : (long?)this.ConfirmedPaperReferal.TransferFormID,
                            Globals.DispatchCallback((asyncResult) =>
                            {

                                try
                                {
                                    IList<TransferForm> TransferFormList = contract.EndGetTransferForm(asyncResult);
                                    if (TransferFormList == null || TransferFormList.Count <= 0)
                                    {
                                        if (this.ConfirmedPaperReferal == null || this.ConfirmedPaperReferal.TransferFormID <= 0)/*TMA*/
                                            MessageBox.Show("Không tìm thấy giấy chuyển tuyến theo điều kiện đã cho!");

                                    }
                                    else
                                    {
                                        if (TransferFormList.Count() == 1)
                                        {
                                            SetCurrentTransferForm(TransferFormList[0]);
                                        }
                                        else
                                        {
                                            Action<ITransferFormList> onInitDlg = delegate (ITransferFormList vm)
                                            {
                                                vm.TransferFormList = TransferFormList.ToObservableCollection();
                                                vm.V_TransferFormType = V_TransferFormType;
                                                vm.ParentReferralPaper = this;
                                            };
                                            GlobalsNAV.ShowDialog<ITransferFormList>(onInitDlg);
                                        }
                                    }
                                    TextCriterial = "";
                                    this.HideBusyIndicator();
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.Message);
                                    this.HideBusyIndicator();
                                }
                            }), null);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private string _StrHIStatus;
        public string StrHIStatus
        {
            get
            {
                return _StrHIStatus;
            }
            set
            {
                _StrHIStatus = value;
                NotifyOfPropertyChange(() => StrHIStatus);
            }
        }

        public void btnPrint_Click()
        {
            if (CurrentTransferForm.TransferFormID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TransferFormID = (long)CurrentTransferForm.TransferFormID;
                    proAlloc.RegistrationID = (long)CurrentTransferForm.CurPatientRegistration.PtRegistrationID;
                    proAlloc.TransferFormType = (int)CurrentTransferForm.V_TransferFormType;
                    proAlloc.FindBy = (int)CurrentTransferForm.PatientFindBy;
                    proAlloc.eItem = ReportName.TRANSFERFORM;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2415_G1_ErrorGCV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void PrevICD10_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            Auto = (AutoCompleteBox)sender;
            Name = e.Parameter;
            Type = 0;
            if (refIDC10 == null)
                refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            refIDC10.PageIndex = 0;
            if (string.IsNullOrEmpty(e.Parameter))
            {
                CurrentTransferForm.ICD10 = null;
                CurrentTransferForm.DiagnosisTreatment = null;
                return;
            }
            LoadRefDiseases(e.Parameter, 0, 0, refIDC10.PageSize);
        }
        public void PrevICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            AutoName = (AutoCompleteBox)sender;
            Name = e.Parameter;
            Type = 1;
            if (refIDC10 == null)
                refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            refIDC10.PageIndex = 0;
            if (string.IsNullOrEmpty(e.Parameter))
            {
                CurrentTransferForm.ICD10 = null;
                CurrentTransferForm.DiagnosisTreatment = null;
                return;
            }
            LoadRefDiseases(e.Parameter, 1, 0, refIDC10.PageSize);
        }
        public void PrevICD10_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var item = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
            if (item != null)
            {
                CurrentTransferForm.ICD10 = item.ICD10Code;
                CurrentTransferForm.DiagnosisTreatment = item.DiseaseNameVN;
            }
            else
            {
                CurrentTransferForm.ICD10 = null;
                CurrentTransferForm.DiagnosisTreatment = null;
            }
        }
        public void PrevICD10Final_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var item = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
            if (item != null)
            {
                CurrentTransferForm.ICD10Final = item.ICD10Code;
                CurrentTransferForm.DiagnosisTreatment_Final = item.DiseaseNameVN;
            }
            else
            {
                CurrentTransferForm.ICD10Final = null;
                CurrentTransferForm.DiagnosisTreatment_Final = null;
            }
        }

        private bool _IsOutPatient = true;
        public bool IsOutPatient
        {
            get
            {
                return _IsOutPatient;
            }
            set
            {
                _IsOutPatient = value;
                NotifyOfPropertyChange(() => IsOutPatient);
            }
        }

        private bool _IsInPatient;
        public bool IsInPatient
        {
            get
            {
                return _IsInPatient;
            }
            set
            {
                _IsInPatient = value;
                NotifyOfPropertyChange(() => IsInPatient);
            }
        }

        private ObservableCollection<Lookup> _FindByList;
        public ObservableCollection<Lookup> FindByList
        {
            get
            {
                return _FindByList;
            }
            set
            {
                _FindByList = value;
                NotifyOfPropertyChange(() => FindByList);
            }
        }

        private Lookup _FindBy;
        public Lookup FindBy
        {
            get
            {
                return _FindBy;
            }
            set
            {
                _FindBy = value;
                NotifyOfPropertyChange(() => FindBy);
            }
        }

        public void LoadCriterialTypes()
        {
            FindByList = new ObservableCollection<Lookup>
            {
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.MA_BN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.MA_BN)
                } ,
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.TEN_BN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.TEN_BN)
                } ,
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.MA_CHUYEN_TUYEN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.MA_CHUYEN_TUYEN)
                }
            };
        }
        public string GetDescription(Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public void txtRegistrationCode_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            if (CurrentTransferForm == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(sender.Text))
            {
                FromHospitalAutoCnt.InitBlankBindingValue();
                return;
            }

            // TxD 28/03/2014: Commented out the following block of code to enable 
            //                  RELOADING OF Hospital Info based on Code regardless of HiCode being the same as before

            if (FromHospitalAutoCnt.SelectedHospital != null && FromHospitalAutoCnt.HosID > 0 && FromHospitalAutoCnt.SelectedHospital.HICode == sender.Text)
            {
                return;
            }
            if (CurrentTransferForm.TransferFromHos == null)
            {
                CurrentTransferForm.TransferFromHos = new Hospital();
            }
            CurrentTransferForm.TransferFromHos.HosName = string.Empty;
            CurrentTransferForm.TransferFromHos.HICode = sender.Text;
            LoadHospital(CurrentTransferForm.TransferFromHos.HICode);
        }
        private void LoadHospital(string HICode)
        {
            if (string.IsNullOrEmpty(HICode) || string.IsNullOrWhiteSpace(HICode))
            {
                return;
            }

            // TxD 26/03/2014 : PLEASE NOTE THAT: For some reasons if we have busyindicator here then focus does not goes to 
            //                  the next field the Autocomplete hospital name lookup
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginSearchHospitalByHICode(HICode,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var hospital = contract.EndSearchHospitalByHICode(asyncResult);

                                    if (hospital != null)
                                    {
                                        //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                                        //EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICode(hospital.HICode);
                                        CurrentTransferForm.TransferFromHos.HICode = hospital.HICode;
                                        FromHospitalAutoCnt.SetSelHospital(hospital);
                                    }
                                    else
                                    {
                                        FromHospitalAutoCnt.InitBlankBindingValue();
                                    }
                                }
                                catch (Exception innerEx)
                                {
                                    ClientLoggerHelper.LogInfo(innerEx.ToString());
                                    FromHospitalAutoCnt.InitBlankBindingValue();
                                }
                                finally
                                {
                                    //this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }

            });
            t.Start();
        }

        private bool _AddCriterialDetail;
        public bool AddCriterialDetail
        {
            get
            {
                return _AddCriterialDetail;
            }
            set
            {
                _AddCriterialDetail = value;
                if (_AddCriterialDetail == false)
                {
                    FindBy = null;
                }
                NotifyOfPropertyChange(() => AddCriterialDetail);
            }
        }

        /*TMA*/
        private bool _IsAllowSearching = true;

        public bool IsAllowSearching
        {
            get
            {
                return _IsAllowSearching;
            }
            set
            {
                if (_IsAllowSearching == value)
                    return;
                _IsAllowSearching = value;
                NotifyOfPropertyChange(() => IsAllowSearching);
            }
        }

        private bool _IsAllowFinding = true;

        public bool IsAllowFinding
        {
            get
            {
                return _IsAllowFinding;
            }
            set
            {
                if (_IsAllowFinding == value)
                    return;
                _IsAllowFinding = value;
                NotifyOfPropertyChange(() => IsAllowFinding);
            }
        }

        private bool _V_GetPaperReferalFullFromOtherView = false;

        public bool V_GetPaperReferalFullFromOtherView
        {
            get
            {
                return _V_GetPaperReferalFullFromOtherView;
            }
            set
            {
                if (_V_GetPaperReferalFullFromOtherView == value)
                    return;
                _V_GetPaperReferalFullFromOtherView = value;
                NotifyOfPropertyChange(() => V_GetPaperReferalFullFromOtherView);
            }
        }
        /*TMA*/
        protected override void OnActivate()
        {
            base.OnActivate();
            if (V_GetPaperReferalFullFromOtherView == true)
            {
                IsAllowFinding = false;
                IsAllowSearching = false;
                _eventArg.Subscribe(this);
                CurrentTransferForm.V_TransferFormType = V_TransferFormType;

            }
            else
            {
                IsAllowFinding = false;
                IsAllowSearching = true;
            }
            if (V_TransferFormType == (int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS)
                TitleForm = eHCMSResources.Z2238_G1_GiayChuyenDiLamCanLamSang;
            else
                TitleForm = eHCMSResources.T1205_G1_GCVien;
        }
        //▼====== #003
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }
        //▲====== #003
        /*TMA*/
        private bool _gIsAdmin = Globals.isAccountCheck;
        public bool gIsAdmin
        {
            get
            {
                return _gIsAdmin;
            }
            set
            {
                if (_gIsAdmin == value) return;
                _gIsAdmin = value;
                NotifyOfPropertyChange(() => gIsAdmin);
            }
        }
        private bool _IsNew = true;
        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                if (_IsNew == value) return;
                _IsNew = value;
                NotifyOfPropertyChange(() => IsNew);
            }
        }
        public void btnDelete_Click()
        {
            if (CurrentTransferForm == null || CurrentTransferForm.TransferFormID == 0) return;
            //▼==== #006
            if (IsHiReportOrDischarge)
            {
                MessageBox.Show(eHCMSResources.Z3320_G1_ChTuyenHiReportOrDischarge, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var Reason = "";

            IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
            MessBox.FireOncloseEvent = true;
            MessBox.IsShowReason = true;
            MessBox.SetMessage("Nhập lý do", eHCMSResources.Z0627_G1_TiepTucLuu);
            GlobalsNAV.ShowDialog_V3(MessBox);
            if (!MessBox.IsAccept)
            {
                return;
            }
            Reason = MessBox.Reason;
            //▲==== #006
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteTransferForm(CurrentTransferForm.TransferFormID, Globals.LoggedUserAccount.Staff.StaffID, Reason, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeleteTransferForm(asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //▼====== #004
                                    if (IsThisViewDialog)
                                    {
                                        TryClose();
                                    }
                                    else
                                    {
                                        //Sau khi xóa thì màn hình trắng tinh, người dùng sẽ tìm cái giấy chuyển khác làm việc
                                        //Vì sau khi xóa rồi thì không cần phải chừa thông tin nào lại cả.
                                        CurrentTransferForm = new TransferForm();
                                    }
                                    //▲====== #004
                                    Globals.EventAggregator.Publish(new OnChangedPaperReferal() { TransferForm = new TransferForm() });
                                }
                                else
                                    MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private bool _IsThisViewDialog = false;
        public bool IsThisViewDialog
        {
            get
            {
                return _IsThisViewDialog;
            }
            set
            {
                _IsThisViewDialog = value;
                NotifyOfPropertyChange(() => IsThisViewDialog);
            }
        }

        //▼====: #005
        private ObservableCollection<RefNationality> _Nationalities;
        public ObservableCollection<RefNationality> Nationalities
        {
            get { return _Nationalities; }
            set
            {
                _Nationalities = value;
                NotifyOfPropertyChange(() => Nationalities);
            }
        }

        private IEnumerator<IResult> DoNationalityListList()
        {
            if (Globals.allNationalities != null)
            {
                Nationalities = Globals.allNationalities.ToObservableCollection();
                yield break;
            }
            var paymentTask = new LoadNationalityListTask(false, false);
            yield return paymentTask;
            Nationalities = paymentTask.RefNationalityList;
            yield break;
        }
        //▲====: #005

        //▼==== #006
        private bool _CanEditTransferForm = true;
        public bool CanEditTransferForm
        {
            get
            {
                return _CanEditTransferForm;
            }
            set
            {
                _CanEditTransferForm = value;
                NotifyOfPropertyChange(() => CanEditTransferForm);
            }
        }

        private bool _IsHiReportOrDischarge = false;
        public bool IsHiReportOrDischarge
        {
            get
            {
                return _IsHiReportOrDischarge;
            }
            set
            {
                _IsHiReportOrDischarge = value;
                NotifyOfPropertyChange(() => IsHiReportOrDischarge);
            }
        }

        private string _TransferFormSaveTitle = eHCMSResources.T2937_G1_Luu;
        public string TransferFormSaveTitle
        {
            get
            {
                return _TransferFormSaveTitle;
            }
            set
            {
                _TransferFormSaveTitle = value;
                NotifyOfPropertyChange(() => TransferFormSaveTitle);
            }
        }
        //▲==== #006
    }
}