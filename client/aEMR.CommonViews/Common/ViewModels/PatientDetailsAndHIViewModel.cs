using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using DataEntities;
using eHCMS.Services.Core;
using System.Linq;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Controls;
using aEMR.ServiceClient;
using aEMR.DataContracts;
using aEMR.CommonTasks;
using aEMR.Common.DataValidation;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.Common.BaseModel;
/*
* 20170113 #001 CMN: Add QRCode
* 20170517 #002 CMN: Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
* 20230311 #003 QTD: Đổ lại dữ liệu quốc tịch
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientDetailsAndHI)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientDetailsAndHIViewModel : ViewModelBase, IPatientDetailsAndHI
        , IHandle<PaperReferalDeleteEvent>
    {
        private IPatientRegistrationNew _thePtRegNewView = null;
        public IPatientRegistrationNew ThePtRegNewView
        {
            get { return _thePtRegNewView;  }
            set
            {
                _thePtRegNewView = value;
            }
        }

        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PatientDetailsAndHIViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            authorization();
            Operation = FormOperation.None;
            
            var hospitalAutoCompleteVm = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm.IsPaperReferal = true;
            HospitalAutoCompleteContent = hospitalAutoCompleteVm;
            HospitalAutoCompleteContent.DisplayHiCode = false;
            HospitalAutoCompleteContent.InitBlankBindingValue();

            var paperReferralHospitalAutoCompleteVm = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            paperReferralHospitalAutoCompleteVm.IsPaperReferal = true;
            PaperReferralHospitalAutoCompleteContent = paperReferralHospitalAutoCompleteVm;
            PaperReferralHospitalAutoCompleteContent.DisplayHiCode = false;
            PaperReferralHospitalAutoCompleteContent.InitBlankBindingValue();

            EditingHiItem = new HealthInsurance();
            EditingPaperReferal = new PaperReferal();

            LoadView();
        }

        private void LoadView()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadProvinces();
                LoadCountries();
                LoadEthnicsList();
                LoadGenders();
                LoadFamilyRelationshipList();
                LoadMaritalStatusList();
                ExamDate = Globals.GetCurServerDateTime();
                //Coroutine.BeginExecute(DoGetDateServer());

                // 12082018 TNHX: MrTuan said: set HiCardType = 2015
               // LoadHiCardTypes();
                LoadKVCode();
                //▼====: #003
                Coroutine.BeginExecute(DoNationalityListList());
                //▲====: #003
                GetAllLookupValuesForTransferForm();
            }
            if (Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion || Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
            {
                LoadCrossRegionHospitals();
            }
            SelectedProvince = new CitiesProvince();
        }

        /*TMA*/
        private int _PatientFindBy;
        public int PatientFindBy
        {
            get { return _PatientFindBy; }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                }
                NotifyOfPropertyChange(() => PatientFindBy);
                //HealthInsuranceContent.PatientFindBy = PatientFindBy;
            }
        }
        /*TMA*/

        //==== #001
        private HIQRCode _QRCode;
        public HIQRCode QRCode
        {
            get { return _QRCode; }
            set
            {
                _QRCode = value;
                try
                {
                    if (QRCode != null)
                    {
                        CurrentPatient.FullName = QRCode.FullName;
                        CurrentPatient.AgeOnly = true;
                        CurrentPatient.YOB = QRCode.DOB.Year;
                        CurrentPatient.Gender = QRCode.Gender.ID;
                        CurrentPatient.GenderObj = QRCode.Gender;
                        CurrentPatient.PatientStreetAddress = QRCode.Address;
                        if (!string.IsNullOrEmpty(QRCode.HICardNo))
                        {
                            var DistrictName = Globals.GetDistrictNameFromHICardNo(QRCode.HICardNo);
                            if (!string.IsNullOrEmpty(DistrictName))
                            {
                                ObservableCollection<CitiesProvince> FoundProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName).IndexOf(ConvertString(DistrictName), StringComparison.InvariantCultureIgnoreCase) >= 0));
                                if (FoundProvince != null && FoundProvince.Count == 1)
                                {
                                    QRCode.CitiesProvince = FoundProvince.First();
                                    SelectedProvince = FoundProvince.First();
                                    PagingLinq(SelectedProvince.CityProvinceID);
                                    //CurrentPatient.CitiesProvince = FoundProvince.First();
                                    CurrentPatient.CityProvinceID = SelectedProvince.DeepCopy().CityProvinceID;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                NotifyOfPropertyChange(() => QRCode);
            }
        }

        public void CheckToModConfirmBtn_And_AllowEditHI()
        {
            if (!IsChildWindow)
                return;
            // TxD 10/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
            if (Enable_ReConfirmHI_InPatientOnly)
            {
                CanHIEdit = true;
            }
            else if (!Globals.ServerConfigSection.Hospitals.IsConfirmHI)
            {
                CanHIEdit = false;
            }
        }

        public void PatientHiManagement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsInstalling":
                case "InfoHasChanged":
                    NotifyOfPropertyChange(() => CanSaveChangesCmd);
                    NotifyOfPropertyChange(() => CanCancelChangesCmd);
                    break;
            }
        }

        // TxD 10/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        private bool _Enable_ReConfirmHI_InPatientOnly = false;
        public bool Enable_ReConfirmHI_InPatientOnly
        {
            get
            {
                return _Enable_ReConfirmHI_InPatientOnly;
            }
            set
            {
                _Enable_ReConfirmHI_InPatientOnly = value;
            }
        }

        private IPatientDetailsAndHIView currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            currentView = view as IPatientDetailsAndHIView;
            authorization();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        public void InitPatient(Patient _currentPatient)
        {
            CurrentPatient = _currentPatient;
            if (CurrentPatient != null)
            {

            }
        }

        private bool _ExpanderVisible = true;
        public bool ExpanderVisible
        {
            get
            {
                return _ExpanderVisible;
            }
            set
            {
                _ExpanderVisible = value;
            }
        }

        /// <summary>
        /// Loại đăng ký (Nhận bệnh cho đăng ký nội trú hay ngoại trú)
        /// </summary>
        private AllLookupValues.RegistrationType _registrationType = AllLookupValues.RegistrationType.Unknown;
        public AllLookupValues.RegistrationType RegistrationType
        {
            get
            {
                return _registrationType;
            }
            set
            {
                _registrationType = value;
                /*if (HealthInsuranceContent != null)
                {
                    HealthInsuranceContent.RegistrationType = value;
                }*/
            }
        }

        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                //allProvince = new ObservableCollection<CitiesProvince>();
                //allSuburbNames = new ObservableCollection<DataEntities.SuburbNames>();
                
                // TxD 28/09/2017 : Commented out the following because the SAME is done in StartEditing
                /*TMA - 28/09/2017 : mở lại vì ko update ptregistration vào paperreferral */
                //if (_currentPatient != null)
                //{
                //    _currentPatient.PropertyChanged -= CurrentPatient_PropertyChanged;
                //}
                /*TMA - 28/09/2017 : mở lại vì ko update ptregistration vào paperreferral */

                _currentPatient = value;
               // HealthInsuranceContent.CurrentPatient = _currentPatient; /*TMA*/
                allProvince = new ObservableCollection<CitiesProvince>();
                allSuburbNames = new ObservableCollection<DataEntities.SuburbNames>();

                // TxD 08/11/2014: Why logging the following lines?????? commented them out
                //if (_currentPatient != null)
                //{
                //    ClientLoggerHelper.LogError("PatientDetailsAndHIViewModel CurrentPatient Property: Name = [" + _currentPatient.FullName + "] - Code = [" + _currentPatient.PatientCode + "]");
                //}
                //else
                //{
                //    ClientLoggerHelper.LogError("PatientDetailsAndHIViewModel CurrentPatient Property: NULL");
                //}
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CurrentPatient.CitiesProvince.CityProvinceName);
            }
        }

        private bool _IsProcessing;
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                _IsProcessing = value;
                NotifyOfPropertyChange(() => IsProcessing);
            }
        }

        private string _ConfirmCmdContent = eHCMSResources.G2465_G1_XemQLoiBHYT;
        public string ConfirmCmdContent
        {
            get { return _ConfirmCmdContent; }
            set
            {
                _ConfirmCmdContent = value;
                NotifyOfPropertyChange(() => ConfirmCmdContent);
            }
        }

        private bool _showCloseFormButton = true;
        public bool ShowCloseFormButton
        {
            get { return _showCloseFormButton; }
            set
            {
                _showCloseFormButton = value && mNhanBenh_ThongTin_Sua;
                NotifyOfPropertyChange(() => ShowCloseFormButton);
            }
        }


        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
            }
        }

        private ObservableCollection<Lookup> _maritalStatusList;
        public ObservableCollection<Lookup> MaritalStatusList
        {
            get { return _maritalStatusList; }
            set
            {
                _maritalStatusList = value;
                NotifyOfPropertyChange(() => MaritalStatusList);
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

        private ObservableCollection<Lookup> _familyRelationshipList;
        public ObservableCollection<Lookup> FamilyRelationshipList
        {
            get { return _familyRelationshipList; }
            set
            {
                _familyRelationshipList = value;
                NotifyOfPropertyChange(() => FamilyRelationshipList);
            }
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

        private ObservableCollection<CitiesProvince> _provinces;
        public ObservableCollection<CitiesProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                NotifyOfPropertyChange(() => Provinces);
            }
        }

        private ObservableCollection<CitiesProvince> _allProvince;
        public ObservableCollection<CitiesProvince> allProvince
        {
            get { return _allProvince; }
            set
            {
                _allProvince = value;
                NotifyOfPropertyChange(() => allProvince);
            }
        }

        private ObservableCollection<CitiesProvince> _HIAllProvince;
        public ObservableCollection<CitiesProvince> HIAllProvince
        {
            get { return _HIAllProvince; }
            set
            {
                _HIAllProvince = value;
                NotifyOfPropertyChange(() => HIAllProvince);
            }
        }

        // TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
        private CitiesProvince _selectedProvince;
        public CitiesProvince SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                NotifyOfPropertyChange(() => SelectedProvince);
            }
        }
        
        private ObservableCollection<SuburbNames> _SuburbNames;
        public ObservableCollection<SuburbNames> SuburbNames
        {
            get { return _SuburbNames; }
            set
            {
                _SuburbNames = value;
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }

        private ObservableCollection<SuburbNames> _SelectedSuburbName;
        public ObservableCollection<SuburbNames> SelectedSuburbName
        {
            get { return _SelectedSuburbName; }
            set
            {
                _SelectedSuburbName = value;
                NotifyOfPropertyChange(() => SelectedSuburbName);
            }
        }

        private ObservableCollection<SuburbNames> _allSuburbNames;
        public ObservableCollection<SuburbNames> allSuburbNames
        {
            get { return _allSuburbNames; }
            set
            {
                _allSuburbNames = value;
                NotifyOfPropertyChange(() => allSuburbNames);
            }
        }

        private long? _selectedSuburbName;
        public long? selectedSuburbName
        {
            get { return _selectedSuburbName; }
            set
            {
                _selectedSuburbName = value;
                NotifyOfPropertyChange(() => selectedSuburbName);
            }
        }

        private ObservableCollection<SuburbNames> _HISuburbNames;
        public ObservableCollection<SuburbNames> HISuburbNames
        {
            get { return _HISuburbNames; }
            set
            {
                _HISuburbNames = value;
                NotifyOfPropertyChange(() => HISuburbNames);
            }
        }

        private ObservableCollection<SuburbNames> _HISelectedSuburbName;
        public ObservableCollection<SuburbNames> HISelectedSuburbName
        {
            get { return _HISelectedSuburbName; }
            set
            {
                _HISelectedSuburbName = value;
                NotifyOfPropertyChange(() => HISelectedSuburbName);
            }
        }

        private long? _HIselectedSuburbName;
        public long? HIselectedSuburbName
        {
            get { return _HIselectedSuburbName; }
            set
            {
                _HIselectedSuburbName = value;
                NotifyOfPropertyChange(() => HIselectedSuburbName);
            }
        }

        private bool _isSaving = false;
        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                NotifyOfPropertyChange(() => IsSaving);
                NotifyOfPropertyChange(() => CanSaveChangesCmd);
            }
        }

        private bool _IsChildWindow = false;
        public bool IsChildWindow
        {
            get
            {
                return _IsChildWindow;
            }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                }

                NotifyOfPropertyChange(() => IsChildWindow);
            }
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

        private bool _CanHIEdit = true;
        public bool CanHIEdit
        {
            get { return _CanHIEdit; }
            set
            {
                _CanHIEdit = value;
                NotifyOfPropertyChange(() => CanHIEdit);
            }
        }

        private bool _CanInfoEdit = true;
        public bool CanInfoEdit
        {
            get { return _CanInfoEdit; }
            set
            {
                _CanInfoEdit = value;
                NotifyOfPropertyChange(() => CanInfoEdit);
            }
        }

        private bool _generalInfoChanged;
        public bool GeneralInfoChanged
        {
            get { return _generalInfoChanged; }
            set
            {
                _generalInfoChanged = value;
                NotifyOfPropertyChange(() => GeneralInfoChanged);

                NotifyOfPropertyChange(() => CanSaveChangesCmd);
                NotifyOfPropertyChange(() => CanCancelChangesCmd);
            }
        }

        private HealthInsurance _confirmedItem;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public HealthInsurance ConfirmedItem
        {
            get
            {
                return _confirmedItem;
            }
            set
            {
                _confirmedItem = value;
                NotifyOfPropertyChange(() => ConfirmedItem);
            }
        }
        private PaperReferal _confirmedPaperReferal;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _confirmedPaperReferal;
            }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }

        private FormState _formState;
        /// <summary>
        /// Biến điều khiển trạng thái của form.
        /// </summary>
        public FormState FormState
        {
            get
            {
                return _formState;
            }
            set
            {
                _formState = value;
                NotifyOfPropertyChange(() => FormState);
            }
        }

        private string _currentAction = "";
        public string CurrentAction
        {
            get
            {
                return _currentAction;
            }
            set
            {
                if (_currentAction != value)
                {
                    _currentAction = value;
                    NotifyOfPropertyChange(() => CurrentAction);
                }
            }
        }

        DateTime ExamDate = DateTime.Now;

        public string ConvertString(string stringInput)
        {
            stringInput = stringInput.ToUpper();
            string convert = "ĂÂÀẰẦÁẮẤẢẲẨÃẴẪẠẶẬỄẼỂẺÉÊÈỀẾẸỆÔÒỒƠỜÓỐỚỎỔỞÕỖỠỌỘỢƯÚÙỨỪỦỬŨỮỤỰÌÍỈĨỊỲÝỶỸỴĐăâàằầáắấảẳẩãẵẫạặậễẽểẻéêèềếẹệôòồơờóốớỏổởõỗỡọộợưúùứừủửũữụựìíỉĩịỳýỷỹỵđ";
            string To = "AAAAAAAAAAAAAAAAAEEEEEEEEEEEOOOOOOOOOOOOOOOOOUUUUUUUUUUUIIIIIYYYYYDaaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyyd";
            for (int i = 0; i < To.Length; i++)
            {
                stringInput = stringInput.Replace(convert[i], To[i]);
            }
            return stringInput;
        }﻿

        public void CreateNewPatientAndHI(bool bCreateNewPatientDetailsOnly = false)
        {
            SuburbNames = new ObservableCollection<SuburbNames>();
            CurrentPatient = new Patient
            {
                CountryID = 229,
                //CityProvinceID = 42,
                CurrentClassification = new PatientClassification { PatientClassID = 1 },
                V_Ethnic = 425,
                V_MaritalStatus = 300,
                CurrentHealthInsurance = new HealthInsurance()
            };
            CurrentPatient.DateBecamePatient = Globals.GetCurServerDateTime();

            if (bCreateNewPatientDetailsOnly)
            {
                ActivationMode = ActivationMode.CREATE_NEW_PATIENT_DETAILS_ONLY;
            }
            else
            {
                ActivationMode = ActivationMode.NEW_PATIENT_FOR_NEW_REGISTRATION;
            }

            Lookup Mau2015 = new Lookup();
            //Mau2015.HasErrors = false;
            Mau2015.LookupID = 5907;
            //Mau2015.MyID = 10695;
            Mau2015.ObjectName = "V_HICardType";
            Mau2015.ObjectNotes = "Mau moi ap dung cho cac the phat hanh va su dung tu 01/01/2015";
            Mau2015.ObjectTypeID = 60;
            Mau2015.ObjectValue = "Mau 2015";

            EditingHiItem.HICardType = Mau2015;

            HospitalAutoCompleteContent.InitBlankBindingValue();

            Operation = FormOperation.AddNew;
            CanEditAddressForHICard = true;
           

            ChangeFormState(FormState.NEW);
            GeneralInfoChanged = false;
        }

        private void EditingHIAndPaperReferal()
        {
            if (CurrentPatient.CurrentHealthInsurance != null)
            {
                EditingHiItem = ObjectCopier.DeepCopy(CurrentPatient.CurrentHealthInsurance);
                SelectedProvince = Provinces.Where(x => x.CityProvinceID == EditingHiItem.CityProvinceID_Address).FirstOrDefault();
                HIPagingLinq(SelectedProvince.CityProvinceID);
                if (CurrentPatient.ActivePaperReferal != null)
                {
                    EditingPaperReferal = ObjectCopier.DeepCopy(CurrentPatient.ActivePaperReferal);
                }
            }
        }

        public void SavePatientCmd()
        {
            if (cboSuburb.SelectedIndex > 0)
            {
                CurrentPatient.SuburbNameID = SuburbNames[cboSuburb.SelectedIndex].SuburbNameID;
            }

            if (currentView != null && CurrentPatient != null && CurrentPatient.PatientID > 0)
            {
                CurrentPatient.DateBecamePatient = currentView.DateBecamePatient;
            }
            else
                CurrentPatient.DateBecamePatient = Globals.GetCurServerDateTime();

            //PMRSave();
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            bool valid = ValidateGeneralInfo(CurrentPatient, out validationResults);

            if (!valid)
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent { ValidationResults = validationResults });
                IsSaving = false;
                this.HideBusyIndicator();
                return;
            }
            CurrentPatient.StaffID = Globals.LoggedUserAccount.StaffID.Value;

            // Validate HI
            // just set default for add new HI 

            EditingHiItem.IsActive = true;
            EditingHiItem.RegistrationLocation = HospitalAutoCompleteContent.SelectedHospital.HosName;
            EditingHiItem.RegistrationCode = HospitalAutoCompleteContent.SelectedHospital.HICode;
            //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
            //EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICode(HospitalAutoCompleteContent.SelectedHospital.HICode);
            EditingHiItem.HosID = HospitalAutoCompleteContent.SelectedHospital.HosID;
            //EditingHiItem.PatientStreetAddress = CurrentPatient.PatientStreetAddress;
            //KMx: Kiểm tra các thuộc tính trong thẻ BH có bị null hay không ([Required] của property) (24/02/2014 09:54).
            if (!ValidateHealthInsurance(_editingHiItem, out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                this.HideBusyIndicator();
                return;
            }

            // TxD 28/12/2015 added the following for new BHYT Card Rules applied 01/01/2015
            bool bCardValid = true;
            /*

            if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101)
            {
                bCardValid = _editingHiItem.ValidateAllFields_New_2015(out validationResults);
            }
            else
            {
                bCardValid = _editingHiItem.ValidateAllFields(out validationResults);
            }
            */

            //KMx: Kiểm tra Mã thẻ, From Date, To Date có hợp lệ hay không (khi 3 thuộc tính đó != null) (24/02/2014 09:54).
            if (!bCardValid)
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }

            if (HospitalAutoCompleteContent.SelectedHospital == null || HospitalAutoCompleteContent.SelectedHospital.HosID <= 0 ||
                string.IsNullOrEmpty(HospitalAutoCompleteContent.SelectedHospital.HosName) || string.IsNullOrEmpty(HospitalAutoCompleteContent.SelectedHospital.HICode))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0345_G1_Msg_InfoChonKCBBDKhHopLe));
                return;
            }

            

            //KMx: Nếu nơi KCB-BĐ không có tỉnh thành thì không cho lưu.
            if (string.IsNullOrEmpty(_editingHiItem.CityProvinceName))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A1015_G1_Msg_InfoTheBHKhCoT_TP));
                return;
            }
            /*

            if (Globals.ServerConfigSection.HealthInsurances.CheckAddressInHealthInsurance && (EditingHiItem.PatientStreetAddress == null || EditingHiItem.PatientStreetAddress.Trim().Length <= 0))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.K0429_G1_NhapDChiThTruBN));
                this.HideBusyIndicator();
                return;
            }
            if (SelectedProvince == null || SelectedProvince.CityProvinceID <= 0)
            {
                if (Globals.ServerConfigSection.HealthInsurances.CheckAddressInHealthInsurance)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.K0417_G1_ChonTinhThanhThTruBN));
                    this.HideBusyIndicator();
                    return;
                }
                else
                {
                    EditingHiItem.CityProvinceID_Address = 0;
                    EditingHiItem.SuburbNameID = 0;
                }
            }
            else
            {
                EditingHiItem.CityProvinceID_Address = SelectedProvince.CityProvinceID;
                if (EditingHiItem.SuburbNameID <= 0 && Globals.ServerConfigSection.HealthInsurances.CheckAddressInHealthInsurance)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.K0387_G1_ChonQuanThTruBN));
                    this.HideBusyIndicator();
                    return;
                }
            }
            */
            GetKVCode();

            // Validate EditingPaperReferral
            EditingPaperReferal.Hospital = PaperReferralHospitalAutoCompleteContent.SelectedHospital;
            if (EditingPaperReferal.Hospital == null
                || string.IsNullOrEmpty(EditingPaperReferal.Hospital.HosName)
                || EditingPaperReferal.Hospital.HosID < 1)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z1274_G1_NoiCVKgHopLe));
                return;
            }         

            else if (Globals.ServerConfigSection.HealthInsurances.IsCheckHICodeInPaperReferal && (EditingPaperReferal.Hospital.HICode == null || EditingPaperReferal.Hospital.HICode.Trim().Length < 5))
            {
                MessageBox.Show(eHCMSResources.Z1275_G1_MaBVKgHopLe);
                return;
            }
            else
            {
                //gan cac gia tri tu hospital cho editingpaper
                EditingPaperReferal.HospitalToValue();
                // EditingPaperReferal.HospitalID == 0 after set. 
                if (EditingPaperReferal.HospitalID == 0) EditingPaperReferal.HospitalID = PaperReferralHospitalAutoCompleteContent.HosID;
            }

            //EditingPaperReferal.HospitalToValue();
            // RUn save Patient
            if (CurrentPatient.PatientID > 0)
            {

                //18082018 TTM:
                //Lối cũ: Lưu tuần tự bệnh nhân => thẻ => giấy CV nên không có vấn đề gì
                //Lối mới: Lưu tất cả cùng lúc, nên 1 số trường bắt buộc thẻ BHYT không có (PatientID) nên gán từ thông tin chi tiết bệnh nhân sang để thực hiện việc lưu 1 lúc cả bệnh nhân, thẻ, giấy CV
                EditingHiItem.PatientID = CurrentPatient.PatientID;

                //HIPCode và IBID bị null nên gán code cứng để lấy HIPCode và IBID từ trên client dựa trên thông tin thẻ nhập vào.
                EditingHiItem.HIPCode = EditingHiItem.HICardNo.Substring(0, 2);
                var num = EditingHiItem.HICardNo.Substring(2, 1);
                EditingHiItem.IBID = Convert.ToInt32(num) + 10;
                CurrentPatient.CurrentHealthInsurance = EditingHiItem;

                //Chỗ này cần xem lại 
                CurrentPatient.ActivePaperReferal = EditingPaperReferal;

                UpdatePatientDetailsAndHI();
            }
            else
            {
                CurrentPatient.CurrentHealthInsurance = EditingHiItem;
                CurrentPatient.ActivePaperReferal = EditingPaperReferal;
                AddPatientDetailsAndHI();
            }
        }

        //18082018 TTM: Thêm điều kiện để nhận biết khi nào update khi nào add mới.       
        const short cUpdatePatientDetails = 0x0001;
        const short cAddNewHICard = 0x0010;
        const short cUpdateHICard = 0x0020;
        const short cAddPaperReferral = 0x0100;
        const short cUpdatePaperReferral = 0x0200;
        public void SetUpdateMode()
        {
            //Luon luon
            CurrentPatient.NumberOfUpdate = cUpdatePatientDetails;

            if (CurrentPatient.CurrentHealthInsurance != null && CurrentPatient.CurrentHealthInsurance.HIID > 0)
            {
                CurrentPatient.NumberOfUpdate += cUpdateHICard;
            }
            else
            {
                CurrentPatient.NumberOfUpdate += cAddNewHICard;
            }

            if (CurrentPatient.ActivePaperReferal != null && CurrentPatient.ActivePaperReferal.RefID > 0)
            {
                CurrentPatient.NumberOfUpdate += cUpdatePaperReferral;
            }
            else
            {
                CurrentPatient.NumberOfUpdate += cAddPaperReferral;
            }
        }

        private void AddPatientDetailsAndHI()
        {
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1102_G1_DangLuuTTinBN));
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddConfirmNewPatientAndHIDetails(CurrentPatient, false, 0,
                                                             false, false, false,
                                                             false, false, Globals.DispatchCallback(asyncResult =>
                                                             {
                                                                 try
                                                                 {
                                                                     var bOK = contract.EndAddConfirmNewPatientAndHIDetails(out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                                                      out double RebatePercentage, out double CalculatedHiBenefit, asyncResult);

                                                                     if (bOK)
                                                                     {
                                                                         GeneralInfoChanged = false;
                                                                         CurrentPatient.PatientID = PatientID;
                                                                         CurrentPatient.PatientCode = PatientCode;

                                                                         EditingHiItem.HIID = HIID;
                                                                         CurrentPatient.CurrentHealthInsurance = EditingHiItem;
                                                                         CurrentPatient.ActivePaperReferal = EditingPaperReferal;

                                                                         if (QRCode != null)
                                                                             CurrentPatient.QRCode = QRCode;

                                                                         Globals.EventAggregator.Publish(new SavePatientDetailsAndHI_OKEvent { theSavedPatient = CurrentPatient, CalculatedHiBenefit = CalculatedHiBenefit });
                                                                         TryClose();
                                                                         GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                     }
                                                                     else
                                                                     {
                                                                         Countries = null;
                                                                     }

                                                                 }
                                                                 catch (Exception innerEx)
                                                                 {
                                                                     error = new AxErrorEventArgs(innerEx);
                                                                 }
                                                                 finally
                                                                 {
                                                                     this.DlgHideBusyIndicator();
                                                                 }
                                                             }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                    this.DlgHideBusyIndicator();
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.DlgHideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        private void UpdatePatientDetailsAndHI()
        {
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1102_G1_DangLuuTTinBN));
            SetUpdateMode();
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateConfirmPatientAndHIDetails(CurrentPatient, false, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), true, false, EditingHiItem.HICardType.LookupID, false,
                                                                        false, false, false, false, Globals.DispatchCallback(asyncResult =>
                                                                        {
                                                                            try
                                                                            {
                                                                                var bOK = contract.EndUpdateConfirmPatientAndHIDetails(out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                                                                 out double RebatePercentage, out double CalculatedHiBenefit, out string Result, asyncResult);

                                                                                if (bOK)
                                                                                {
                                                                                    GeneralInfoChanged = false;
                                                                                    CurrentPatient.PatientID = PatientID;
                                                                                    CurrentPatient.PatientCode = PatientCode;
                                                                                    CurrentPatient.CurrentHealthInsurance.HIID = HIID;
                                                                                    CurrentPatient.CurrentHealthInsurance.HIPCode = HIPCode;
                                                                                    CurrentPatient.CurrentHealthInsurance.IBID = IBID;
                                                                                    CurrentPatient.CurrentHealthInsurance.InsuranceBenefit.RebatePercentage = RebatePercentage;

                                                                                    if (QRCode != null)
                                                                                        CurrentPatient.QRCode = QRCode;

                                                                                    Globals.EventAggregator.Publish(new SavePatientDetailsAndHI_OKEvent { theSavedPatient = CurrentPatient });
                                                                                    TryClose();
                                                                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                                }
                                                                                else
                                                                                {
                                                                                    Countries = null;
                                                                                }
                                                                            }
                                                                            catch (Exception innerEx)
                                                                            {
                                                                                error = new AxErrorEventArgs(innerEx);
                                                                            }
                                                                            finally
                                                                            {
                                                                                this.DlgHideBusyIndicator();
                                                                            }
                                                                        }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                    this.DlgHideBusyIndicator();
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.DlgHideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        public void PagingLinq(long CityProvinceID)
        {
            SuburbNames = new ObservableCollection<SuburbNames>();

            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CityProvinceID)
                {
                    SuburbNames.Add(item);
                }
            }
        }

        public void HIPagingLinq(long CityProvinceID)
        {
            HISuburbNames = new ObservableCollection<SuburbNames>();

            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CityProvinceID)
                {
                    HISuburbNames.Add(item);
                }
            }
        }

        AxComboBox cboSuburb { get; set; }
        public void cboSuburb_Loaded(object sender, RoutedEventArgs e)
        {
            cboSuburb = sender as AxComboBox;
        }

        public void cboSuburb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneralInfoChanged = true;
            CurrentPatient.SuburbNameID = (long)cboSuburb.SelectedValueEx;
        }

        public void cboSuburb_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        AxAutoComplete AcbCity { get; set; }
        public void AcbCity_Loaded(object sender, RoutedEventArgs e)
        {
            AcbCity = sender as AxAutoComplete;
        }

        public void AcbCity_Populating(object sender, PopulatingEventArgs e)
        {
          if (sender == null || Provinces == null)
            {
                return;
            }

            string SearchText = ((AutoCompleteBox)sender).SearchText;
            // TxD 09/11/2014: For some unknown reason if Search text is set to empty string ie. Length = 0
            //                  the autocompletebox will fail to populate the next time ie. this function is not called  
            //                  So the following block of code is to prevent that from happening
            if (SearchText.Length == 0)
            {
                ((AutoCompleteBox)sender).PopulateComplete();
                PagingLinq(0); // Clear the District Combobox
                return;
            }

            allProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName)
                    .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
            ((AutoCompleteBox)sender).ItemsSource = allProvince;
            ((AutoCompleteBox)sender).PopulateComplete();
            
        }

        public void AcbCity_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            IsProcessing = true;
            if (sender == null)
            {
                return;
            }
            if (((AutoCompleteBox)sender).SelectedItem == null)
            {
                return;
            }
            long CityProvinceID = ((CitiesProvince)(((AutoCompleteBox)sender).SelectedItem)).CityProvinceID;
            if (CityProvinceID > 0)
            {
                CurrentPatient.CityProvinceID = CityProvinceID;
                CurrentPatient.SuburbNameID = -1;
                PagingLinq((long)CurrentPatient.CityProvinceID);
            }
            else
            {
                SuburbNames = new ObservableCollection<SuburbNames>();
                NotifyOfPropertyChange(() => SuburbNames);
                IsProcessing = false;
            }
        }

        public void LoadAddressFromPatientDetail()
        {
            bSelectProvinceInit = false;
            if (EditingHiItem == null)
            {
                return;
            }
            if (CurrentPatient.PatientStreetAddress != null && CurrentPatient.PatientStreetAddress.Trim() != "")
            {
                EditingHiItem.PatientStreetAddress = CurrentPatient.PatientStreetAddress;
            }
            else
            {
                EditingHiItem.PatientStreetAddress = "";
            }

            if (CurrentPatient.CitiesProvince == null || CurrentPatient.CitiesProvince.CityProvinceID <= 0)
            {
                HIPagingLinq(0);
                return;
            }
            SelectedProvince = Provinces.Where(x => x.CityProvinceID == CurrentPatient.CitiesProvince.CityProvinceID).FirstOrDefault();
            HIPagingLinq(SelectedProvince.CityProvinceID);
            if (CurrentPatient.SuburbNameID > 0)
            {
                EditingHiItem.SuburbNameID = CurrentPatient.SuburbNameID;
            }
            else
            {
                EditingHiItem.SuburbNameID = 0;
            }
        }

        Button ConfirmBtn { get; set; }
        public void ConfirmHIBenefitCmd_Loaded(object sender)
        {
            ConfirmBtn = sender as Button;

            // TxD 12/07/2014 : Commented the call to the bogus checkForDisplay method 
            //                  and added direct checking Enable_ReConfirmHI_InPatientOnly || Globals.ServerConfigSection.Hospitals.IsConfirmHI
            //                  to change the button's Label or Content
            //checkForDisplay();

            if (Enable_ReConfirmHI_InPatientOnly || Globals.ServerConfigSection.Hospitals.IsConfirmHI)
            {
                ConfirmBtn.Content = eHCMSResources.G2363_G1_XNhan;
                ConfirmBtn.Width = 80;
            }

        }

        private bool _IsConfirmedEmergencyPatient = false;
        public bool IsConfirmedEmergencyPatient
        {
            get { return _IsConfirmedEmergencyPatient; }
            set
            {
                _IsConfirmedEmergencyPatient = value;
                NotifyOfPropertyChange(() => IsConfirmedEmergencyPatient);
            }
        }

        private bool _ShowConfirmedEmergencyPatient = true;
        public bool ShowConfirmedEmergencyPatient
        {
            get { return _ShowConfirmedEmergencyPatient; }
            set
            {
                _ShowConfirmedEmergencyPatient = value;
                NotifyOfPropertyChange(() => ShowConfirmedEmergencyPatient);
            }
        }

        private bool _IsConfirmedForeignerPatient = false;
        public bool IsConfirmedForeignerPatient
        {
            get { return _IsConfirmedForeignerPatient; }
            set
            {
                _IsConfirmedForeignerPatient = value;
                NotifyOfPropertyChange(() => IsConfirmedForeignerPatient);
            }
        }

        private bool _ShowConfirmedForeignerPatient = false;
        public bool ShowConfirmedForeignerPatient
        {
            get { return _ShowConfirmedForeignerPatient; }
            set
            {
                _ShowConfirmedForeignerPatient = value;
                NotifyOfPropertyChange(() => ShowConfirmedForeignerPatient);
            }
        }

        private bool _EmergInPtReExamination = false;
        public bool EmergInPtReExamination
        {
            get { return _EmergInPtReExamination; }
            set
            {
                _EmergInPtReExamination = value;
                NotifyOfPropertyChange(() => EmergInPtReExamination);
            }
        }

        private bool _ShowEmergInPtReExamination = true;
        public bool ShowEmergInPtReExamination
        {
            get { return _ShowEmergInPtReExamination; }
            set
            {
                _ShowEmergInPtReExamination = value;
                NotifyOfPropertyChange(() => ShowEmergInPtReExamination);
            }
        }

        // Hpt 04/12/2015: Biến xác nhận hương quyền lợi trẻ em dưới 6 tuổi có thẻ BHYT (truyền từ ReceivePatientVM vào)
        private bool _IsChildUnder6YearsOld = false;
        public bool IsChildUnder6YearsOld
        {
            get { return _IsChildUnder6YearsOld; }
            set
            {
                _IsChildUnder6YearsOld = value;
                NotifyOfPropertyChange(() => IsChildUnder6YearsOld);
            }
        }

        private bool _IsHICard_FiveYearsCont = false;
        public bool IsHICard_FiveYearsCont
        {
            get { return _IsHICard_FiveYearsCont; }
            set
            {
                _IsHICard_FiveYearsCont = value;
                NotifyOfPropertyChange(() => IsHICard_FiveYearsCont);
            }
        }
        

        void PatientMedicalRecordCurrent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GeneralInfoChanged = true;
        }

        void CurrentPatient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DateBecamePatient":
                case "FullName":
                case "Age":
                case "DOB":
                case "GenderObj":
                case "V_MaritalStatus":
                case "PatientEmployer":
                case "PatientOccupation":
                case "V_Ethnic":
                case "IDNumber":
                case "PatientStreetAddress":
                case "PatientSurburb":
                case "CityProvinceID":
                case "CityProvinceName":
                case "CountryID":
                case "PatientPhoneNumber":
                case "PatientCellPhoneNumber":
                case "PatientEmailAddress":
                case "V_FamilyRelationship":
                case "FContactFullName":
                case "FContactAddress":
                case "FContactHomePhone":
                case "FContactBusinessPhone":
                case "FContactCellPhone":
                case "FAlternateContact":
                case "FAlternatePhone":
                case "PatientNotes":
                case "SuburbName":
                case "SuburbNameID":
                    GeneralInfoChanged = true;
                    NotifyOfPropertyChange(() => GeneralInfoChanged);
                    break;
            }
        }

        public void ChangeFormState(FormState newState)
        {
            FormState = newState;

            switch (_formState)
            {
                case FormState.NONE:
                    CurrentAction = "";
                    break;
                case FormState.NEW:
                    //CurrentAction = "Add New Patient";
                    CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                    break;
                case FormState.EDIT:
                    //CurrentAction = eHCMSResources.T0015_G1_EditPatient;
                    CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;
                    break;
                case FormState.READONLY:
                    //CurrentAction = "Patient Information";
                    CurrentAction = eHCMSResources.G0525_G1_TTinBN;
                    break;
            }
        }

        public bool ValidateGeneralInfo(object p, out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> results)
        {
            var patient = p as Patient;
            return patient.ValidatePatient(patient, out results);
        }

        public void LoadGenders()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllGenders(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<Gender> allItems = null;
                                try
                                {
                                    allItems = contract.EndGetAllGenders(asyncResult);
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

                                Genders = allItems != null ? new ObservableCollection<Gender>(allItems) : null;
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

        public void LoadMaritalStatusList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.MARITAL_STATUS,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    MaritalStatusList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
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

        public void LoadFamilyRelationshipList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        try
                        {
                            contract.BeginGetAllLookupValuesByType(LookupValues.FAMILY_RELATIONSHIP,
                                    Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                            FamilyRelationshipList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
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
                        catch (Exception exc)
                        {
                            Globals.ShowMessage(exc.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
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

        public void LoadProvinces()
        {
            if (Globals.allCitiesProvince != null && Globals.allCitiesProvince.Count > 0)
            {
                Provinces = Globals.allCitiesProvince.ToObservableCollection();
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                if (Globals.allCitiesProvince == null)
                                {
                                    Globals.allCitiesProvince = new List<CitiesProvince>(allItems);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
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

        public void LoadHiCardTypes()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Đang lấy danh sách các loại thẻ bảo hiểm..."
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.HI_CARD_TYPE,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<Lookup> allItems = null;
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                                HiCardTypes = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                            }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        public void Handle(CloseHiManagementView message)
        {
            if (message != null)
            {
                Close();
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            ResetView(view as IPatientDetailsAndHIView);
        }

        private void ResetView(IPatientDetailsAndHIView view)
        {
            view.FocusOnFirstItem();

            //PaperReferralContent.ConfirmPaperReferalSelected = false;
        }

        public void CancelChangesOnGeneralInfo()
        {
            if (_formState == FormState.EDIT || _formState == FormState.NEW)
            {
                if (CurrentPatient != null)
                {
                    CurrentPatient.CancelEdit();
                    GeneralInfoChanged = false;
                }
            }
        }
        
        private bool _closeWhenFinish;
        public bool CloseWhenFinish
        {
            get { return _closeWhenFinish; }
            set
            {
                _closeWhenFinish = value;
                NotifyOfPropertyChange(() => CloseWhenFinish);
            }
        }

        public void LoadPatientDetailsAndHI_GenAction(object objGenTask, object patient, object ToRegisOutPt)
        {
            GenericCoRoutineTask theGenTask = null;
            if (objGenTask != null)
            {
                theGenTask = (GenericCoRoutineTask)objGenTask;
            }

            if (patient == null)
            {
                if (theGenTask != null)
                {
                    theGenTask.ActionComplete(true);
                }
                return;
            }
            
            this.ShowBusyIndicator();

            //HealthInsuranceContent.IsLoading = true;
            var t = new Thread(() =>
            {                
                try
                {                    
                    //KMx: Phải set = null trước khi load, tránh trường hợp load bị lỗi => Thông tin của người A, mà thẻ BH người B. Dẫn đến đăng ký bị lỗi (29/10/2014 10:46).
                    CurrentPatient = new Patient();                    
                    //HealthInsuranceContent.CurrentPatient = new Patient();
                    // TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
                    SelectedProvince = new CitiesProvince();
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByIDFullInfo(((Patient)patient).PatientID, (bool)ToRegisOutPt,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    if (AcbCity != null)
                                    {
                                        AcbCity.SelectedItem = null;
                                    }
                                    CurrentPatient = contract.EndGetPatientByIDFullInfo(asyncResult);

                                    if (CurrentPatient != null)
                                    {
                                        // TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
                                        //SelectedProvince = CurrentPatient.CitiesProvince;

                                        NotifyOfPropertyChange(() => CurrentPatient.CitiesProvince.CityProvinceName);
                                        NotifyOfPropertyChange(() => CurrentPatient.SuburbName.SuburbName);
                                        
                                        PermissionManager.ApplyPermissionToHealthInsuranceList(CurrentPatient.HealthInsurances);
                                        PermissionManager.ApplyPermissionToPaperReferalList(CurrentPatient.PaperReferals);
                                        //them o day moi dung ne
                                        SuburbNames = new ObservableCollection<SuburbNames>();
                                        if (CurrentPatient.CityProvinceID != null && CurrentPatient.CityProvinceID.Value > 0)
                                        {
                                            PagingLinq(CurrentPatient.CityProvinceID.Value);
                                        }

                                        GetPMRsByPtIDCurrent(_currentPatient.PatientID);
                                    }
                                    else
                                    {
                                        ClientLoggerHelper.LogInfo("StartEditingPatientLazyLoad EndGetPatientByIDFullInfo Exception Error: CurrentPatient NULL");
                                    }
                                    ChangeFormState(FormState.EDIT);
                                    GeneralInfoChanged = false;

                                    EditingHIAndPaperReferal();

                                    //HealthInsuranceContent.CurrentPatient = CurrentPatient;
                                    //==== #001
                                    //if (((Patient)patient).QRCode != null)
                                    //    HealthInsuranceContent.QRCode = ((Patient)patient).QRCode;
                                    //else
                                    //    HealthInsuranceContent.QRCode = QRCode;
                                    //==== #001
                                    //PaperReferralContent.CurrentPatient = CurrentPatient;
                                }
                                catch (Exception ex)
                                {
                                    theGenTask.Error = ex;
                                    Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                                    ClientLoggerHelper.LogError("StartEditingPatientLazyLoad EndGetPatientByIDFullInfo Exception Error: " + ex.Message);
                                }
                                finally
                                {                                    
                                    this.HideBusyIndicator();
                                    //HealthInsuranceContent.IsLoading = false;
                                    if (theGenTask != null)
                                    {
                                        theGenTask.ActionComplete(true);
                                    }
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    theGenTask.Error = ex;
                    Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError("StartEditingPatientLazyLoad Exception Error: " + ex.Message);
                    //HealthInsuranceContent.IsLoading = false;
                    this.HideBusyIndicator();
                    if (theGenTask != null)
                    {
                        theGenTask.ActionComplete(true);
                    }
                }

            });
            t.Start();
        }
    
        private ActivationMode _activationMode;
        public ActivationMode ActivationMode
        {
            get { return _activationMode; }
            set
            {
                _activationMode = value;
                NotifyOfPropertyChange(() => ActivationMode);

                // TxD 14/07/2014 : Commented out the call to the bogus method checkForDisplay
                //if (checkForDisplay())
                //{
                //    return;
                //}

                if (_activationMode == ActivationMode.PATIENT_GENERAL_HI_VIEW || _activationMode == ActivationMode.CREATE_NEW_PATIENT_DETAILS_ONLY)
                {
                    CanHIEdit = false;
                }
                else
                {
                    CanHIEdit = true;
                }
            }
        }

        public void Close()
        {
            Globals.EventAggregator.Publish(new ViewModelClosing<IPatientDetailsAndHI> { ViewModel = this });
            TryClose();
        }
        
        public void Handle(PaperReferalDeleteEvent message)
        {
            if (IsChildWindow)
            {
                ConfirmHIBenefitCmd_Click();
            }
        }

        /// <summary>
        /// Xac nhan the bao hiem + giay chuyen vien cung 1 luc
        /// </summary>
        /// Xem or Xac Nhan Quyen Loi BHYT Button Clicked
        ///  20180815 TNHX: Remove PatientHiManagementViewModel -> Disable this function
        public void ConfirmHIBenefitCmd_Click()
        {
            //ConfirmHIBenefit(true);
            //HealthInsuranceContent.ConfirmedItem.HisID=
        }

        // The following method is Called by the ReceivePatientViewModel when the Dang Ky button is pressed
        // 20180815 TNHX: Remove PatientHiManagementViewModel -> Disable this function
       public bool ConfirmHIBeforeRegister()
       {
            //goi tu receivePatient 
            //ko can tra ve event
            //return ConfirmHIBenefit(false);
            return true;
       }

       public HiCardConfirmedEvent hiCardConfirmedEvent { get; set; }

       public IEnumerator<IResult> WarningResult(string message, string checkBoxContent)
       {
           var dialog = new MessageWarningShowDialogTask(message, checkBoxContent);
           yield return dialog;
           flag = dialog.IsAccept;
           yield break;
       }

        bool flag = true;

        /* 20180815 TNHX: Remove PatientHiManagementViewModel -> Disable this function
           public IEnumerator<IResult> ConfirmHICoroutine(bool returnEvent)
           {
               if (CurrentPatient == null)
               {
                   ConfirmedItem = null;
                   Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.A0378_G1_Msg_InfoChuaChonBN) });
                   flag = false;
                   yield break;
               }

               if (HealthInsuranceContent.InfoHasChanged)
               {
                   Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1103_G1_LuuTrcKhiXNhanTheBH) });
                   flag = false;
                   yield break;
               }

               if (HealthInsuranceContent.HealthInsurances == null || HealthInsuranceContent.HealthInsurances.Count == 0)
               {
                   ConfirmedItem = null;
                   Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1104_G1_ChuaCoTheBHXNhan) });
                   flag = false;
                   yield break;
               }

               bool confirmHiOK = HealthInsuranceContent.CheckValidationAndGetConfirmedItem();
               if (!confirmHiOK)
               {
                   flag = false;
                   yield break;
               }

               if (!EmergInPtReExamination && !IsChildUnder6YearsOld && !Parent.IsAllowCrossRegion && (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse == null || HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked == false))
               {
                   if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101 && !Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
                   {
                       MessageBox.Show(string.Format("{0}.", eHCMSResources.Z1032_G1_BNNgoaiTruKgTheDKTraiTuyen));
                       yield break;
                   }
                   //if (Globals.ServerConfigSection.InRegisElements.ApplyHINewRule20160101 && Parent.IsAllowCrossRegion && Globals.CrossRegionHospital != null
                   //    && !Globals.CrossRegionHospital.Any(x=>x.HosID == HealthInsuranceContent.ConfirmedItem.HosID))
                   //{
                   //    var dialog = new MessageWarningShowDialogTask("Nơi đăng ký KCBBD của bệnh nhân không thông tuyến với bệnh viện! ", "Tiếp tục đăng ký thông tuyến");
                   //    yield return dialog;
                   //    if (!dialog.IsAccept)
                   //    {
                   //        yield break;
                   //    }
                   //}
                   //if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101 && (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse == null || HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked == false)
                   //               && !Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
                   //{
                   //    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z1032_G1_BNNgoaiTruKgTheDKTraiTuyen));
                   //    yield break;
                   //}
               }

               if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU)
               {
                   DateTime ExamDateTemp = new DateTime(ExamDate.Year, ExamDate.Month, ExamDate.Day);
                   DateTime MaxExamDateHITemp = new DateTime(CurrentPatient.MaxExamDateHI.GetValueOrDefault().Year, CurrentPatient.MaxExamDateHI.GetValueOrDefault().Month, CurrentPatient.MaxExamDateHI.GetValueOrDefault().Day);
                   System.TimeSpan diff1 = ExamDateTemp.Subtract(MaxExamDateHITemp);

                   //int DifferenceDayPrecriptHI = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.DifferenceDayPrecriptHI]);
                   //int DifferenceDayRegistrationHI = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.DifferenceDayRegistrationHI]);

                   // Txd 25/05/2014 Replaced ConfigList
                   int DifferenceDayPrecriptHI = Globals.ServerConfigSection.HealthInsurances.DifferenceDayPrecriptHI;
                   int DifferenceDayRegistrationHI = Globals.ServerConfigSection.HealthInsurances.DifferenceDayRegistrationHI;

                   if (CurrentPatient.MaxExamDateHI != null && CurrentPatient.MaxDayRptsHI != null)
                   {
                       //lay ngay hien tai - ngay dang ky BH gan nhat
                       if (diff1.TotalDays < Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()))
                       {
                           if ((Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()) - diff1.TotalDays) > DifferenceDayPrecriptHI)
                           {
                               MessageBox.Show(string.Format("{0}!", eHCMSResources.A0250_G1_Msg_InfoKhTheDKBHYT_BNConNhieuThuocBH));
                               flag = false;
                               yield break;
                           }

                           if ((Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()) - diff1.TotalDays) <= DifferenceDayPrecriptHI)
                           {
                               var dialog = new MessageWarningShowDialogTask(string.Format(eHCMSResources.Z1261_G1_BNConThuocBHYT, DifferenceDayPrecriptHI.ToString()), eHCMSResources.Z1259_G1_TiepTucDK);
                               yield return dialog;
                               flag = dialog.IsAccept;
                               if (!flag)
                               {
                                   flag = false;
                                   yield break;
                               }
                           }
                       }
                   }
               }

               var evt = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
               hiCardConfirmedEvent = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
               if (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse != null && HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked)
               {
                   evt.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
                   hiCardConfirmedEvent.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
               }
               if (returnEvent)
               {
                   Globals.EventAggregator.Publish(evt);
               }
               flag = true;
               yield break;
           }
        */
        /* 20180815 TNHX: Remove PatientHiManagementViewModel -> Disable this function
        private bool ConfirmHIBenefit(bool returnEvent)
        {
            if (CurrentPatient == null)
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.A0378_G1_Msg_InfoChuaChonBN) });
                return false;
            }

            // TxD: Nhan Benh Noi Tru & Cap Cuu Khong Co BHYT ===> By Pass the following validations
            if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU || RegistrationType == AllLookupValues.RegistrationType.CAP_CUU)
            {
                return false;
            }

            if (HealthInsuranceContent.InfoHasChanged)
            {
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1103_G1_LuuTrcKhiXNhanTheBH) });
                return false;
            }
            if (HealthInsuranceContent.HealthInsurances == null || HealthInsuranceContent.HealthInsurances.Count == 0)
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1104_G1_ChuaCoTheBHXNhan) });
                return false;
            }

            if (HealthInsuranceContent.IsMarkAsDeleted)
            {
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}..", eHCMSResources.Z1105_G1_KgTheSDTheBHDaXoa) });
                return false;
            }

            bool confirmHiOK = HealthInsuranceContent.CheckValidationAndGetConfirmedItem();
            if (!confirmHiOK)
            {
                return false;
            }
            Parent.IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(HealthInsuranceContent.ConfirmedItem, RegistrationType); 
            if (IsHICard_FiveYearsCont && IsChildUnder6YearsOld)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0288_G1_Msg_Info1in2QL));
                return false;
            }
            //HPT
            if (!EmergInPtReExamination && !IsChildUnder6YearsOld && !Parent.IsAllowCrossRegion && RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101 && (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse == null || HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked == false)
                               && !Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z1032_G1_BNNgoaiTruKgTheDKTraiTuyen));
                    return false;
                }
            }

            // Hpt 07/12/2015: Không cùng lúc check vào cả hai checkbox Xác nhận bệnh nhân có BHYT 5 năm liên tiếp và trẻ em dưới 6 tuổi (lưu ý: quyền lợi cho hai hình thức xác nhận này là khác nhau)
            if (IsHICard_FiveYearsCont && IsChildUnder6YearsOld)
            {
                MessageBox.Show(eHCMSResources.A0288_G1_Msg_Info1in2QL);
                return false;
            }


            if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                DateTime ExamDateTemp = new DateTime(ExamDate.Year, ExamDate.Month, ExamDate.Day);
                DateTime MaxExamDateHITemp = new DateTime(CurrentPatient.MaxExamDateHI.GetValueOrDefault().Year, CurrentPatient.MaxExamDateHI.GetValueOrDefault().Month, CurrentPatient.MaxExamDateHI.GetValueOrDefault().Day);
                System.TimeSpan diff1 = ExamDateTemp.Subtract(MaxExamDateHITemp);

                //int DifferenceDayPrecriptHI = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.DifferenceDayPrecriptHI]);
                //int DifferenceDayRegistrationHI = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.DifferenceDayRegistrationHI]);

                // Txd 25/05/2014 Replaced ConfigList
                int DifferenceDayPrecriptHI = Globals.ServerConfigSection.HealthInsurances.DifferenceDayPrecriptHI;
                int DifferenceDayRegistrationHI = Globals.ServerConfigSection.HealthInsurances.DifferenceDayRegistrationHI;

                if (diff1.TotalDays < DifferenceDayRegistrationHI)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1034_G1_BNMoiDKKhamBHYT, MaxExamDateHITemp.ToString("dd/MM/yyyy")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return false;
                    }
                }
            }

            var evt = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
            hiCardConfirmedEvent = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
            if (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse != null && HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked)
            {
                evt.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
                hiCardConfirmedEvent.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
            }
            if (returnEvent)
            {
                Globals.EventAggregator.Publish(evt);
            }

            return true;
        }
        */
        private bool CanClose(bool infoChanged)
        {
            if (infoChanged)
            {
                MessageBoxResult result = MessageBox.Show(eHCMSResources.Z1035_G1_TTinDaThayDoiCoMuonThoat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);

                return result == MessageBoxResult.OK;
            }
            return true;
        }

        public void CloseFormCmd()
        {
            bool bCanClose = CanClose(GeneralInfoChanged);

            if (bCanClose)
            {
                Close();
            }
        }
        
        Expander HIExpander { get; set; }
        public void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            HIExpander = sender as Expander;
        }

        /// <summary>
        /// Luu thong tin general info, hoac the bao hiem, hoac giay chuyen vien tuy theo TabItem nao dang duoc kich hoat
        /// </summary>
        public void SaveChangesCmd()
        {
            //IsSaving = true;
            SavePatientCmd();
        }

        public bool CanSaveChangesCmd
        {
            get
            {
                return GeneralInfoChanged && !IsSaving;
            }
        }

        public bool ShowCancelChangesCmd
        {
            get
            {
                //return _activeTab == PatientInfoTabs.GENERAL_INFO && mNhanBenh_ThongTin_Sua;
                return true;
            }
        }

        public void CancelChangesCmd()
        {
            //switch (_activeTab)
            //{
            //    case PatientInfoTabs.GENERAL_INFO:
            //        CancelChangesOnGeneralInfo();
            //        break;
            //}
            CancelChangesOnGeneralInfo();
        }

        public bool CanCancelChangesCmd
        {
            get
            {
                //switch (_activeTab)
                //{
                //    case PatientInfoTabs.GENERAL_INFO:
                //        return GeneralInfoChanged;
                //    default:
                //        return false;
                //}
                return GeneralInfoChanged;
            }
        }

        private int CalcAge(int age)
        {
            if (age > 1900)
            {
                return Globals.ServerDate.Value.Year - age;
            }
            return age;
        }

        public void txtYOB_LostFocus(TextBox sender, RoutedEventArgs eventArgs)
        {
            var str = sender.Text;
            var vm = sender.DataContext as PatientDetailsAndHIViewModel;
            var p = vm.CurrentPatient;

            if (p != null)
            {
                if (p.AgeOnly.HasValue && p.AgeOnly.Value)
                {
                    int year;
                    if (int.TryParse(str, out year))
                    {
                        if (year > Globals.ServerDate.Value.Year)
                        {
                            year = Globals.ServerDate.Value.Year;
                        }
                        p.YOB = year;
                    }
                    else
                    {
                        p.Age = null;
                        p.YOB = null;
                    }
                    //------- DPT 08/11/2017 < 6 tuổi
                    if (p.Age <= 6)
                    {
                        if (!p.DOBForBaby.HasValue)
                        {
                            p.Age = null;
                            p.YOB = null;
                            MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ ngày tháng năm sinh");
                        }
                    }

                    //int monthnew;
                    //DateTime today = DateTime.Today;
                    //DateTime day = Convert.ToDateTime(p.DOBForBaby);
                    //monthnew = (today.Month + today.Year * 12) - (day.Month + day.Year * 12);
                    //if (monthnew < 73)
                    //{
                    //    p.YOB = null;
                    //    MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ ngày tháng năm sinh");
                    //}

                    //-------------------------------
           
                }
            }
        }

        public void txtYOB_KeyUp(TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                string str = sender.Text;
                var vm = sender.DataContext as PatientDetailsAndHIViewModel;
                var p = vm.CurrentPatient;
                if (p != null)
                {
                    p.AgeOnly = true;
                    p.DOBForBaby = null;
                }
                GeneralInfoChanged = true;
            }
        }

        public void txtAge_LostFocus(TextBox sender, RoutedEventArgs eventArgs)
        {
            string str = sender.Text;
            var vm = sender.DataContext as PatientDetailsAndHIViewModel;
            var p = vm.CurrentPatient;

            if (p != null)
            {
                if (p.AgeOnly.HasValue && p.AgeOnly.Value)
                {
                    p.YOB = Globals.ServerDate.Value.Year - p.Age;
                    NotifyOfPropertyChange(() => p.YOB);
                }
            }
        }

        public void txtAge_TextChanged(TextBox sender, TextChangedEventArgs eventArgs)
        {

        }

        public void txtAge_KeyUp(TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                string str = sender.Text;
                var vm = sender.DataContext as PatientDetailsAndHIViewModel;
                var p = vm.CurrentPatient;
                if (p != null)
                {
                    p.AgeOnly = true;
                    p.DOBForBaby = null;
                    //p.CalDOB();
                }
                GeneralInfoChanged = true;
            }
        }

        public void txtDateOfBirth_DateChanged(TextBox sender, DateTimeSelectedEventArgs eventArgs)
        {
            var str = sender.Text;
            var vm = sender.DataContext as PatientDetailsAndHIViewModel;
            var p = vm.CurrentPatient;
            if (p != null && str != null && str != "")
            {
                //TODO:
                if (p.DOBForBaby != eventArgs.NewValue)
                {
                    GeneralInfoChanged = true;
                }

                p.DOBForBaby = eventArgs.NewValue;
                //p.AgeOnly = false;
                p.AgeOnly = eventArgs.YearOnly;
                p.DOB = p.DOBForBaby;
                if (p.DOB.HasValue)
                {
                    int yearsOld;
                    int monthsOld;
                    AxHelper.ConvertAge(p.DOB.Value, out yearsOld, out monthsOld);
                    p.Age = yearsOld;
                    //p.YOB = Globals.ServerDate.Value.Year - p.Age;
                    p.YOB = p.DOB.Value.Year;
                    p.MonthsOld = monthsOld;
                    
                }
            }
        }

        public void txtDateBecamePatient_TextChanged(TextBox sender, TextChangedEventArgs eventArgs)
        {
            var str = sender.Text;
            var p = sender.DataContext as Patient;
            if (p != null)
            {
                //TODO:
            }
            //GeneralInfoChanged = true;
        }
        
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mNhanBenh_ThongTin_Sua = true;
        private bool _mNhanBenh_ThongTin_XacNhan = true;
        private bool _mNhanBenh_TheBH_ThemMoi = true;
        private bool _mNhanBenh_TheBH_XacNhan = true;
        private bool _mNhanBenh_DangKy = true;
        private bool _mNhanBenh_TheBH_Sua = true;


        public bool mNhanBenh_ThongTin_Sua
        {
            get
            {
                return _mNhanBenh_ThongTin_Sua;
            }
            set
            {
                if (_mNhanBenh_ThongTin_Sua == value)
                    return;
                _mNhanBenh_ThongTin_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_ThongTin_Sua);
            }
        }

        public bool mNhanBenh_ThongTin_XacNhan
        {
            get
            {
                return _mNhanBenh_ThongTin_XacNhan;
            }
            set
            {
                if (_mNhanBenh_ThongTin_XacNhan == value)
                    return;
                _mNhanBenh_ThongTin_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenh_ThongTin_XacNhan);
            }
        }

        public bool mNhanBenh_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenh_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenh_TheBH_ThemMoi == value)
                    return;
                _mNhanBenh_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_ThemMoi);
            }
        }

        public bool mNhanBenh_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenh_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenh_TheBH_XacNhan == value)
                    return;
                _mNhanBenh_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_XacNhan);
            }
        }

        public bool mNhanBenh_DangKy
        {
            get
            {
                return _mNhanBenh_DangKy;
            }
            set
            {
                if (_mNhanBenh_DangKy == value)
                    return;
                _mNhanBenh_DangKy = value;
                NotifyOfPropertyChange(() => mNhanBenh_DangKy);
            }
        }

        public bool mNhanBenh_TheBH_Sua
        {
            get
            {
                return _mNhanBenh_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenh_TheBH_Sua == value)
                    return;
                _mNhanBenh_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_Sua);
            }
        }

        private bool bSelectProvinceInit = false;
        #endregion

        #region binding visibilty

        //public HyperlinkButton lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as HyperlinkButton;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        #region PMR
        public void hplPMR()
        {

            //var typeInfo = Globals.GetViewModel<IPatientMedicalRecords>();
            //typeInfo.ObjPatient = CurrentPatient;
            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //                                 {
            //                                     //làm gì đó
            //                                 });
            // hoi truoc la PatientMedicalRecord
            //bay gio doi la PatientMedicalFile
            Action<IPatientMedicalFiles> onInitDlg = delegate (IPatientMedicalFiles typeInfo)
            {
                typeInfo.ObjPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IPatientMedicalFiles>(onInitDlg);
        }

        private PatientMedicalRecord _PatientMedicalRecordCurrent;

        public PatientMedicalRecord PatientMedicalRecordCurrent
        {
            get { return _PatientMedicalRecordCurrent; }
            set
            {
                _PatientMedicalRecordCurrent = value;
                NotifyOfPropertyChange(() => PatientMedicalRecordCurrent);
            }
        }

        public void PMRSave()
        {
            if (!string.IsNullOrEmpty(PatientMedicalRecordCurrent.NationalMedicalCode))
            {
                PatientMedicalRecords_Save(PatientMedicalRecordCurrent.PatientRecID, CurrentPatient.PatientID
                    , PatientMedicalRecordCurrent.NationalMedicalCode);
            }
        }

        private void GetPMRsByPtIDCurrent(long? patientID)
        {
            IsLoading = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPMRsByPtID(patientID, 1, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPMRsByPtID(asyncResult);
                            PatientMedicalRecordCurrent = new PatientMedicalRecord();
                            if (items != null)
                            {
                                if (items.Count > 0)
                                {
                                    PatientMedicalRecordCurrent = items[0];
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
                        }
                    }), null);
                }


            });
            t.Start();
        }
        private void PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode)
        {
            IsLoading = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalRecords_Save(PatientRecID, PatientID, NationalMedicalCode, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string msg = "";
                            var b = contract.EndPatientMedicalRecords_Save(out msg, asyncResult);
                            MessageBox.Show(msg);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        public void hplHospitalEdit()
        {
            if (ActivationMode == ActivationMode.CREATE_NEW_PATIENT_DETAILS_ONLY)
                return;

            Action<IHospitalAutoCompleteEdit> onInitDlg = delegate (IHospitalAutoCompleteEdit obj)
            {
                obj.IsUpdate = true;
                obj.Title = eHCMSResources.K1679_G1_CNhatTTinBV.ToUpper();
            };
            GlobalsNAV.ShowDialog<IHospitalAutoCompleteEdit>(onInitDlg);
        }

        public void hplHospitalAddNew()
        {
            if (ActivationMode == ActivationMode.CREATE_NEW_PATIENT_DETAILS_ONLY)
                return;

            Action<IHospitalAutoCompleteEdit> onInitDlg = delegate (IHospitalAutoCompleteEdit obj)
            {
                obj.IsUpdate = false;
                obj.Title = eHCMSResources.G0292_G1_ThemMoiBV.ToUpper();
            };
            GlobalsNAV.ShowDialog<IHospitalAutoCompleteEdit>(onInitDlg);
        }

        // Hpt 08/12/2015: Dời hàm kiểm tra xác nhận quyền lợi trẻ em tp dưới 6 tuổi vào đây. Anh Kiên nói kiểm tra qua 2 - 3 lớp không ổn
        public bool CheckResult_ChildUnder6YearsOld(out string Error)
        {
            Error = "";

            if (CurrentPatient == null)
            {
                return false;
            }

            if (CurrentPatient.YOB == null || CurrentPatient.YOB.Value <= 0)
            {
                return false;
            }

            if ((Globals.GetCurServerDateTime().Year - CurrentPatient.YOB.GetValueOrDefault()) > 6)
            {
                Error += "\n - " + eHCMSResources.Z0230_G1_LaTreEmDuoi6Tuoi;
            }

            if (ConfirmedItem == null)
            {
                return false;
            }

            if (!(ConfirmedItem.HICardNo.StartsWith("TE179")))
            {
                Error += "\n - " + eHCMSResources.Z0231_G1_OTPho;
            }

            if (Error != "")
            {
                return false;
            }

            return true;
        }
       
        private ObservableCollection<Lookup> _hiCardTypes;
        public ObservableCollection<Lookup> HiCardTypes
        {
            get { return _hiCardTypes; }
            set
            {
                _hiCardTypes = value;
                NotifyOfPropertyChange(() => HiCardTypes);
            }
        }

        private bool _CanEditAddressForHICard = true;
        public bool CanEditAddressForHICard
        {
            get
            {
                return _CanEditAddressForHICard;
            }
            set
            {
                _CanEditAddressForHICard = value;
                NotifyOfPropertyChange(() => CanEditAddressForHICard);
            }
        }

        private int _TextLength = 15;
        public int TextLength
        {
            get
            {
                return _TextLength;
            }
            set
            {
                if (_TextLength != value)
                {
                    _TextLength = value;
                    NotifyOfPropertyChange(() => TextLength);
                }
            }
        }

        private ObservableCollection<string> _AllKVCode;
        public ObservableCollection<string> AllKVCode
        {
            get
            {
                return _AllKVCode;
            }
            set
            {
                _AllKVCode = value;
                NotifyOfPropertyChange(() => AllKVCode);
            }
        }

        private string _SelectedKV;
        public string SelectedKV
        {
            get
            {
                return _SelectedKV;
            }
            set
            {
                _SelectedKV = value;
                NotifyOfPropertyChange(() => SelectedKV);
            }
        }

        public void SetKVCode()
        {
            if (EditingHiItem == null)
            {
                SelectedKV = "";
                return;
            }
            switch (EditingHiItem.KVCode)
            {
                case (long)AllLookupValues.KVCode.KV1:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV1")];
                        break;
                    }
                case (long)AllLookupValues.KVCode.KV2:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV2")];
                        break;
                    }
                case (long)AllLookupValues.KVCode.KV3:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV3")];
                        break;
                    }
                default:
                    {
                        SelectedKV = "";
                        break;
                    }
            }
        }

        public void GetKVCode()
        {
            switch (SelectedKV)
            {
                case "KV1":
                    {
                        EditingHiItem.KVCode = (long)AllLookupValues.KVCode.KV1;
                        break;
                    }
                case "KV2":
                    {
                        EditingHiItem.KVCode = (long)AllLookupValues.KVCode.KV2;
                        break;
                    }
                case "KV3":
                    {
                        EditingHiItem.KVCode = (long)AllLookupValues.KVCode.KV3;
                        break;
                    }
                default:
                    {
                        EditingHiItem.KVCode = 0;
                        break;
                    }
            }

        }

        public void LoadKVCode()
        {
            AllKVCode = new ObservableCollection<string> { "---", "KV1", "KV2", "KV3" };
        }

        private IHospitalAutoCompleteListing _hospitalAutoCompleteContent;
        public IHospitalAutoCompleteListing HospitalAutoCompleteContent
        {
            get { return _hospitalAutoCompleteContent; }
            set
            {
                _hospitalAutoCompleteContent = value;
                NotifyOfPropertyChange(() => HospitalAutoCompleteContent);
            }
        }

        private IHospitalAutoCompleteListing _paperReferralHospitalAutoCompleteContent;
        public IHospitalAutoCompleteListing PaperReferralHospitalAutoCompleteContent
        {
            get { return _paperReferralHospitalAutoCompleteContent; }
            set
            {
                _paperReferralHospitalAutoCompleteContent = value;
                NotifyOfPropertyChange(() => PaperReferralHospitalAutoCompleteContent);
            }
        }

        private HealthInsurance _editingHiItem;
        /// <summary>
        /// Thẻ bảo hiểm đang edit (thêm mới hoặc cập nhật)
        /// </summary>
        public HealthInsurance EditingHiItem
        {
            get
                {
                return _editingHiItem;
            }
            set
            {
                _editingHiItem = value;
                if (HospitalAutoCompleteContent != null)
                {
                    HospitalAutoCompleteContent.CurSelHealthInsuranceCard = _editingHiItem;
                }
                SetKVCode();
                NotifyOfPropertyChange(() => EditingHiItem);
                NotifyOfPropertyChange(() => CanSaveHiInfoCmd);
                NotifyOfPropertyChange(() => CanCancelEditCmd);
                NotifyOfPropertyChange(() => CanEditHiInfo);
            }
        }

        private PaperReferal _editingPaperReferal;
        /// <summary>
        /// Giấy chuyển viện đang edit (thêm mới hoặc cập nhật)
        /// </summary>
        public PaperReferal EditingPaperReferal
        {
            get
            {
                return _editingPaperReferal;
            }
            set
            {
                if (_editingPaperReferal == value)
                {
                    return;
                }
                _editingPaperReferal = value;
                NotifyOfPropertyChange(() => EditingPaperReferal);
                //NotifyOfPropertyChange(() => AddEditPaperReferalTitle);
                //NotifyOfPropertyChange(() => CanSavePaperReferalCmd);
                //NotifyOfPropertyChange(() => CanUsePaperReferalCmd);
                if (_editingPaperReferal != null)
                {
                    if (PaperReferralHospitalAutoCompleteContent != null)
                    {
                        //HospitalAutoCompleteContent.setDisplayText(_editingPaperReferal.IssuerLocation, _editingPaperReferal.Hospital, true);
                        PaperReferralHospitalAutoCompleteContent.SetSelHospital(_editingPaperReferal.Hospital);
                    }
                }
                else
                {
                    if (PaperReferralHospitalAutoCompleteContent != null)
                    {
                        //HospitalAutoCompleteContent.setDisplayText("", null);
                        PaperReferralHospitalAutoCompleteContent.InitBlankBindingValue();
                    }
                }
            }
        }

        public bool CanCancelEditCmd
        {
            get
            {
                return true;
            }
        }

        public bool CanSaveHiInfoCmd
        {
            get
            {
                return !IsSaving && EditingHiItem != null;
            }
        }

        public bool CanEditHiInfo
        {
            get
            {
                //return (_operation == FormOperation.Edit || _operation == FormOperation.AddNew)
                //    && _editingHiItem != null && !_editingHiItem.Used
                //    && !PaperReferalContent.InfoHasChanged;
                //KMx: Nếu thẻ BH đã được sử dụng, thì không được chỉnh sửa. Nhưng nếu chọn nút "Sửa sau khi đăng ký" thì được quyền sửa. Modified Date: 20/02/2014 15:14.
                /** return (_operation == FormOperation.Edit || _operation == FormOperation.AddNew)
                    && _editingHiItem != null && (!_editingHiItem.Used || CanEditAfterRegistration)
                    && !PaperReferalContent.InfoHasChanged;
                */
                return true;
            }
        }

        private bool ValidateHealthInsurance(HealthInsurance hiItem, out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            bool bValid = ValidationExtensions.ValidateObject(hiItem, out validationResults);

            return bValid;
        }

        private bool _canEditAfterRegistration;
        public bool CanEditAfterRegistration
        {
            get { return _canEditAfterRegistration; }
            set { _canEditAfterRegistration = value; }
        }

        private FormOperation _operation;
        public FormOperation Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
                if (_operation == FormOperation.Edit || _operation == FormOperation.AddNew)
                {
                    InfoHasChanged = true;
                }
                else
                {
                    InfoHasChanged = false;
                }
                NotifyOfPropertyChange(() => Operation);
                //NotifyOfPropertyChange(() => IsEditMode);
            }
        }

        private bool _infoHasChanged;
        /// <summary>
        /// Danh sách thẻ bảo hiểm đã thay đổi hay chưa (kể từ lần load từ database)
        /// (Bao gồm chỉnh sửa 1 thẻ trong list, thêm mới hay đánh dấu xóa)
        /// </summary>
        public bool InfoHasChanged
        {
            get
            {
                return _infoHasChanged;
            }
            set
            {
                if (_infoHasChanged != value)
                {
                    _infoHasChanged = value;
                    NotifyOfPropertyChange(() => InfoHasChanged);
                }
            }
        }
        
        AxAutoComplete AcbHICity { get; set; }
        public void AcbHICity_Loaded(object sender, RoutedEventArgs e)
        {
            AcbHICity = sender as AxAutoComplete;
        }

        public void AcbHICity_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender == null)
            {
                return;
            }
            if (((AutoCompleteBox)sender).SelectedItem == null)
            {
                return;
            }
            long CityProvinceID = ((CitiesProvince)(((AutoCompleteBox)sender).SelectedItem)).CityProvinceID;
            if (CityProvinceID > 0)
            {
                EditingHiItem.CityProvinceID_Address = Convert.ToInt16(CityProvinceID);
                HIPagingLinq((long)EditingHiItem.CityProvinceID_Address);
            }
            else
            {
                HISuburbNames = new ObservableCollection<SuburbNames>();
                NotifyOfPropertyChange(() => HISuburbNames);
            }
        }

        public void AcbHICity_Populating(object sender, PopulatingEventArgs e)
        {
            bSelectProvinceInit = false;
            if (sender == null || Provinces == null)
            {
                return;
            }
            string SearchText = ((AutoCompleteBox)sender).SearchText;
            if (SearchText == null || SearchText == "")
            {
                return;
            }
            // TxD 09/11/2014: For some unknown reason if Search text is set to empty string ie. Length = 0
            //                  the autocompletebox will fail to populate the next time ie. this function is not called  
            //                  So the following block of code is to prevent that from happening
            //if (SearchText == null || SearchText.Length == 0)
            //{
            //    ((AutoCompleteBox)sender).PopulateComplete();
            //    HIPagingLinq(0); // Clear the District Combobox
            //    return;
            //}
            HIAllProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName)
                    .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
            ((AutoCompleteBox)sender).ItemsSource = HIAllProvince;
            ((AutoCompleteBox)sender).PopulateComplete();
        }

        AxComboBox cboHISuburb { get; set; }
        public void cboHISuburb_Loaded(object sender, RoutedEventArgs e)
        {
            cboHISuburb = sender as AxComboBox;
        }

        public void cboHISuburb_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private bool _CanPaperReferalEdit = true;
        public bool CanPaperReferalEdit
        {
            get { return _CanPaperReferalEdit; }
            set
            {
                _CanPaperReferalEdit = value;
                NotifyOfPropertyChange(() => CanPaperReferalEdit);
            }
        }

        public void txtHICardNo_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            //if (EditingHiItem == null || IsEditMode == false)
            if (EditingHiItem == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(sender.Text))
            {
                return;
            }

            //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH (11/11/2014 11:33).
            EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICardNo(sender.Text);
        }

        public void txtRegistrationCode_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            //if (EditingHiItem == null || IsEditMode == false)
            if (EditingHiItem == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(sender.Text))
            {
                return;
            }

            // TxD 28/03/2014: Commented out the following block of code to enable 
            //                  RELOADING OF Hospital Info based on Code regardless of HiCode being the same as before

            if (Operation == FormOperation.AddNew && EditingHiItem.RegistrationCode == sender.Text
                            && HospitalAutoCompleteContent.SelectedHospital != null && HospitalAutoCompleteContent.HosID > 0 && HospitalAutoCompleteContent.SelectedHospital.HICode == sender.Text)
            {
                return;
            }

            EditingHiItem.RegistrationLocation = string.Empty;
            EditingHiItem.RegistrationCode = sender.Text;
            LoadHospital(EditingHiItem);
        }

        public void LoadHospitalByID(string IDCode)
        {
            if (!string.IsNullOrEmpty(IDCode))
            {
                EditingHiItem.RegistrationLocation = string.Empty;

                EditingHiItem.RegistrationCode = IDCode;
                LoadHospital(EditingHiItem);
            }
        }

        /// <summary>
        /// Lay thong tin noi KCB ban dau tu Ma KCB ban dau cua the bao hiem
        /// </summary>
        /// <param name="hiItem"></param>
        private void LoadHospital(HealthInsurance hiItem)
        {
            if (hiItem == null)
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

                        contract.BeginSearchHospitalByHICode(hiItem.RegistrationCode,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var hospital = contract.EndSearchHospitalByHICode(asyncResult);

                                    if (hospital != null)
                                    {
                                        //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                                        //EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICode(hospital.HICode);
                                        EditingHiItem.RegistrationLocation = hospital.HosName;
                                        EditingHiItem.HosID = hospital.HosID;
                                        HospitalAutoCompleteContent.SetSelHospital(hospital);
                                    }
                                    else
                                    {
                                        HospitalAutoCompleteContent.InitBlankBindingValue();
                                    }
                                }
                                catch (Exception innerEx)
                                {
                                    ClientLoggerHelper.LogInfo(innerEx.ToString());
                                    HospitalAutoCompleteContent.InitBlankBindingValue();
                                }
                                finally
                                {
                                    //this.HideBusyIndicator();
                                }

                            }), hiItem);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    ClientLoggerHelper.LogInfo(fault.ToString());
                    //this.HideBusyIndicator();
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    //this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        private void LoadCrossRegionHospitals()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginLoadCrossRegionHospitals(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    Globals.CrossRegionHospital = contract.EndLoadCrossRegionHospitals(asyncResult).ToObservableCollection();
                                }
                                catch (Exception innerEx)
                                {
                                    ClientLoggerHelper.LogInfo(innerEx.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
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

        public void GetAllLookupValuesForTransferForm()
        {
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

                                    TransferTypeList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransferType)) : null;
                                    TransferReasonList = allItems != null ? new ObservableCollection<Lookup>(allItems.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransferReason)) : null;
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        List<ScanImageFileStorageDetail> PtDocFileScanForStore = new List<ScanImageFileStorageDetail>();
        public void AttachHICmd()
        {
            Action<IScanImageCapture> onInitDlg = delegate (IScanImageCapture vm)
            {
                //vm.FileForStore = PtDocFileScanForStore;
            };
            GlobalsNAV.ShowDialog<IScanImageCapture>(onInitDlg);
        }

        public void Test88Cmd()
        {
            foreach(var theItem in PtDocFileScanForStore)
            {
                theItem.ScanImageFileName = Guid.NewGuid().ToString();
            }
        }

        //▼====: #003
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
        //▲====: #003
    }

}