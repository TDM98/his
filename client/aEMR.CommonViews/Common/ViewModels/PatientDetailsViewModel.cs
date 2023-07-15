using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using aEMR.Common;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.Common.BaseModel;
using System.Text.RegularExpressions;
/*
* 20170113 #001 CMN:   Add QRCode
* 20170517 #002 CMN:   Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
* 20180926 #003 TTM:   Chuyển dời việc kiểm tra thẻ online từ trong hàm ConfirmHIBenefit() ra ngoài nút luôn, vì không nhất thiết phải kiểm tra hết, mới đi kiểm tra thẻ BHYT Online
* 20181015 #004 TTM:   BM00021781: Cho phép kiểm tra online thẻ chưa lưu.
* 20181020 #005 TTM:   BM0003204 Set giá trị mặc định cho tỉnh thành/quận huyện khi người dùng thêm mới bệnh nhân.
* 20181113 #006 TTM:   BM 0005228: Thêm mới trường phường/ xã để nhập liệu.
* 20190523 #007 TNHX:  Sửa lỗi không lấy quận vào PatientFullStreetAddress đối với Index = 0
* 20191113 #008 TTM:   BM 0019566: Lỗi khi mở PatientDetails và PatientHiManagement như 1 popup => Không close được busy do không set đúng giá trị nhận biết popup hay view.
* 20191114 #009 CMN:   Chuyển phần clear dữ liệu bệnh nhân vào các trường hợp bị lỗi tránh trường hợp load dữ liệu nhiều lần sẽ bị mất giá trị được chọn
* 20191205 #010 TBL:   BM 0019684: Fix lỗi lấy mã đăng ký lên sai bên nhận bệnh nội trú
* 20200825 #011 TTM:   BM 0041467: Lỗi trẻ em dưới 6 tuổi không có đầy đủ ngày tháng năm sinh vẫn cho lưu
* 20200924 #012 TNHX:   BM: Thêm nút in thẻ KCB (VietinBank + Tiêm ngừa)
* 20201128 #013 TNHX:   BM: Chỉnh lại cách lấy địa chỉ của thẻ BHYT + Bỏ chặn SDT. nếu không nhập chỉ cảnh báo.
* 20210704 #014 TNHX:   385 Lấy giờ tạo STT từ QMS làm giờ vào viện nếu có
* 20210730 #015 TNHX:   Bắt buộc nhập ngày cấp, nơi cấp khi nhập CMND
* 20221221 #016 QTD:    Ràng buộc độ dài min max trường SĐT
* 20230311 #017 QTD:    Dữ liệu 130
* 20230530 #018 DatTB:
* + Thêm service tìm kiếm bệnh nhân bằng QRCode CCCD
* + Thêm bệnh nhân bằng thông tin QRCode CCCD
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientDetails)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientDetailsViewModel : ViewModelBase, IPatientDetails
        , IHandle<CloseHiManagementView>
        , IHandle<ViewModelClosing<IPaperReferral>>
        , IHandle<ViewModelClosing<IPatientHiManagement>>
        , IHandle<PaperReferalDeleteEvent>
        , IHandle<DoubleClickEvent>
        , IHandle<SaveHIAndConfirm>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PatientDetailsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;

            _eventArg = eventArg;
            authorization();
            var hiManagementVm = Globals.GetViewModel<IPatientHiManagement>();
            hiManagementVm.IsChildWindow = IsChildWindow;
            HealthInsuranceContent = hiManagementVm;
            ActivateItem(hiManagementVm);
            (hiManagementVm as PropertyChangedBase).PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(PatientHiManagement_PropertyChanged).Handler;

            //▼====: #012
            var PaymentCardManagementVm = Globals.GetViewModel<IPatientPaymentCardManagement>();
            PaymentCardContent = PaymentCardManagementVm;
            ActivateItem(PaymentCardManagementVm);
            (PaymentCardManagementVm as PropertyChangedBase).PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(PatientHiManagement_PropertyChanged).Handler;
            //▲====: #012
            //▼====: #017
            Coroutine.BeginExecute(DoNationalityList());
            //▲====: #017
            
            //▼==== #018
            GetAllProvinces(null);
            //▲==== #018
        }
        public void LoginHIAPI()
        {
            if ((ActiveTab == PatientInfoTabs.HEALTH_INSURANCE_INFO || RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT) &&
                 (GlobalsNAV.gLoggedHIAPIUser == null
                 || GlobalsNAV.gLoggedHIAPIUser.APIKey == null
                 || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token)))
            {
                GlobalsNAV.LoginHIAPI();
            }
        }

        private bool AllControlDataLoaded = false;
        private const int NumOfLoadingDataFunctions = 10;
        //▼====: #013
        private string tempSuburbName = "";
        private string tempWardName = "";
        //▲====: #013

        private void InitDataLoading(List<EventWaitHandle> lstWaitEvents)
        {
            if (lstWaitEvents != null && lstWaitEvents.Count == NumOfLoadingDataFunctions)
            {
                LoadCountries(lstWaitEvents[0]);
                LoadEthnicsList(lstWaitEvents[1]);
                LoadGenders(lstWaitEvents[2]);
                LoadFamilyRelationshipList(lstWaitEvents[3]);
                LoadMaritalStatusList(lstWaitEvents[4]);
                GetAllProvinces(lstWaitEvents[5]);
                GetAllSuburbNameAction(lstWaitEvents[6]);
                GetAllWardNameAction(lstWaitEvents[7]);
                LoadJobList(lstWaitEvents[8]);
                LoadJob130List(lstWaitEvents[9]);
            }
            else
            {
                LoadCountries(null);
                LoadEthnicsList(null);
                LoadGenders(null);
                LoadFamilyRelationshipList(null);
                LoadMaritalStatusList(null);
                GetAllProvinces(null);
                GetAllSuburbNameAction(null);
                GetAllWardNameAction(null);
                LoadJobList(null);
                LoadJob130List(null);
            }
            AllControlDataLoaded = true;
            ExamDate = Globals.GetCurServerDateTime();
        }

        public void InitLoadControlData_FromExt(object objGenTask)
        {
            GenericCoRoutineTask theGenTask = null;
            if (objGenTask != null)
            {
                theGenTask = (GenericCoRoutineTask)objGenTask;
            }
            var LoadDataAndWaitTask = new GenericTask_WithWaitEvent(InitDataLoading, NumOfLoadingDataFunctions);
            LoadDataAndWaitTask.ExecuteGenericTask(theGenTask);

            AllControlDataLoaded = true;
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
                HealthInsuranceContent.PatientFindBy = PatientFindBy;
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
                        CurrentPatient.AgeOnly = false;
                        CurrentPatient.YOB = QRCode.DOB.Year;
                        CurrentPatient.DOB = QRCode.DOB;
                        CurrentPatient.DOBForBaby = QRCode.DOB;
                        if (CurrentPatient.DOB != null && CurrentPatient.DOB > DateTime.MinValue)
                        {
                            int mYearOld;
                            int mMonthOld;
                            AxHelper.ConvertAge(CurrentPatient.DOB.Value, out mYearOld, out mMonthOld);
                            CurrentPatient.Age = mYearOld;
                            CurrentPatient.MonthsOld = mMonthOld;
                        }
                        CurrentPatient.Gender = QRCode.Gender.ID;
                        CurrentPatient.GenderObj = QRCode.Gender;
                        CurrentPatient.SocialInsuranceNumber = QRCode.HICardNo.Substring(QRCode.HICardNo.Length-10, 10);
                        //▼====: #013
                        //19-11-2020 Chia nhỏ địa chỉ ra thành mảng 
                        string[] addressArray = QRCode.Address.Split(',');
                        int lengthAddressArray = addressArray.Length;
                        if (lengthAddressArray == 1)
                        {
                            CurrentPatient.PatientStreetAddress = addressArray[0];
                        }
                        else if (lengthAddressArray > 3)
                        {
                            for (int i = 0; i < lengthAddressArray - 3; i++)
                            {
                                CurrentPatient.PatientStreetAddress += addressArray[i];
                            }
                            CurrentPatient.CitiesProvince.CityProvinceName = addressArray[lengthAddressArray - 1].Replace("Tỉnh", "").Trim();
                            tempSuburbName = addressArray[lengthAddressArray - 2].Trim();
                            tempWardName = addressArray[lengthAddressArray - 3].Trim();

                        }
                        //▲====: #013

                        //▼====== #005: Bỏ set mặc định khi sử dụng tìm kiếm bằng QRCode vì đã có giá trị mặc định dựa vào HospitalCode rồi
                        //if (!string.IsNullOrEmpty(QRCode.HICardNo))
                        //{
                        //    var DistrictName = Globals.GetDistrictNameFromHICardNo(QRCode.HICardNo);
                        //    if (!string.IsNullOrEmpty(DistrictName))
                        //    {
                        //        ObservableCollection<CitiesProvince> FoundProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName).IndexOf(ConvertString(DistrictName), StringComparison.InvariantCultureIgnoreCase) >= 0));
                        //        if (FoundProvince != null && FoundProvince.Count == 1)
                        //        {
                        //            QRCode.CitiesProvince = FoundProvince.First();
                        //            SelectedProvince = FoundProvince.First();
                        //            PagingLinq(SelectedProvince.CityProvinceID);
                        //            //CurrentPatient.CitiesProvince = FoundProvince.First();
                        //            CurrentPatient.CityProvinceID = SelectedProvince.DeepCopy().CityProvinceID;
                        //        }
                        //    }
                        //}
                        //▲======= #005
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                NotifyOfPropertyChange(() => QRCode);
            }
        }
        //==== #001
        //▼==== #018
        private IDCardQRCode _IDCardQRCode;
        public IDCardQRCode IDCardQRCode
        {
            get { return _IDCardQRCode; }
            set
            {
                _IDCardQRCode = value;
                try
                {
                    if (IDCardQRCode != null)
                    {
                        CurrentPatient.FullName = IDCardQRCode.FullName;
                        CurrentPatient.AgeOnly = false;
                        CurrentPatient.YOB = IDCardQRCode.DOB.Year;
                        CurrentPatient.DOB = IDCardQRCode.DOB;
                        CurrentPatient.DOBForBaby = IDCardQRCode.DOB;
                        if (CurrentPatient.DOB != null && CurrentPatient.DOB > DateTime.MinValue)
                        {
                            int mYearOld;
                            int mMonthOld;
                            AxHelper.ConvertAge(CurrentPatient.DOB.Value, out mYearOld, out mMonthOld);
                            CurrentPatient.Age = mYearOld;
                            CurrentPatient.MonthsOld = mMonthOld;
                        }
                        CurrentPatient.Gender = IDCardQRCode.Gender.ID;
                        CurrentPatient.GenderObj = IDCardQRCode.Gender;
                        CurrentPatient.IDNumber = IDCardQRCode.IDNumber;
                        CurrentPatient.IDCreatedDate = IDCardQRCode.IDCreatedDate;
                        CurrentPatient.IDCreatedFrom = "Cục Cảnh sát QLHC về trật tự xã hội";
                        
                        string[] addressArray = IDCardQRCode.Address.Split(',');
                        int lengthAddressArray = addressArray.Length;
                        if (lengthAddressArray == 1)
                        {
                            CurrentPatient.PatientStreetAddress = addressArray[0];
                        }
                        else if (lengthAddressArray > 3)
                        {
                            for (int i = 0; i < lengthAddressArray - 3; i++)
                            {
                                CurrentPatient.PatientStreetAddress += addressArray[i];
                            }

                            CurrentPatient.CitiesProvince.CityProvinceName = addressArray[lengthAddressArray - 1].Replace("Tỉnh", "").Trim();
                            tempSuburbName = addressArray[lengthAddressArray - 2].Trim();
                            tempWardName = addressArray[lengthAddressArray - 3].Trim();
                            setaddressArrayToForm();
                        }
                        else if (lengthAddressArray > 2)
                        {
                            CurrentPatient.CitiesProvince.CityProvinceName = addressArray[lengthAddressArray - 1].Replace("Tỉnh", "").Trim();
                            tempSuburbName = addressArray[lengthAddressArray - 2].Trim();
                            tempWardName = addressArray[lengthAddressArray - 3].Trim();
                            setaddressArrayToForm();
                        }
                        else if (lengthAddressArray > 1)
                        {
                            CurrentPatient.CitiesProvince.CityProvinceName = addressArray[lengthAddressArray - 1].Replace("Tỉnh", "").Trim();
                            tempSuburbName = addressArray[lengthAddressArray - 2].Trim();
                            setaddressArrayToForm();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                NotifyOfPropertyChange(() => IDCardQRCode);
            }
        }

        public void setaddressArrayToForm()
        {
            if (Globals.allCitiesProvince != null)
            {
                IDCardQRCode.CitiesProvince = Globals.allCitiesProvince.Where(x => x.CityProvinceName == CurrentPatient.CitiesProvince.CityProvinceName).FirstOrDefault();
                AutoCompleteBox sender = new AutoCompleteBox();
                sender.SelectedItem = IDCardQRCode.CitiesProvince;
                try
                {
                    AcbCity_DropDownClosed(sender, new RoutedPropertyChangedEventArgs<bool>(true, true));
                }
                catch { }
                finally
                {
                    if (SuburbNames != null)
                    {
                        IDCardQRCode.SuburbNames = SuburbNames.Where(x => x.SuburbName.EndsWith(tempSuburbName)).FirstOrDefault();
                        if (IDCardQRCode.SuburbNames != null)
                        {
                            CurrentPatient.SuburbNameID = IDCardQRCode.SuburbNames.SuburbNameID;
                            try
                            {
                                PagingLinqForWardName(CurrentPatient.SuburbNameID);
                            }
                            catch { }
                            finally
                            {
                                if (ListWardNames != null)
                                {
                                    IDCardQRCode.WardNames = ListWardNames.Where(x => x.WardName == tempWardName).FirstOrDefault();
                                    if (IDCardQRCode.WardNames != null)
                                    {
                                        CurrentPatient.WardNameID = IDCardQRCode.WardNames.WardNameID;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //▲==== #018

        private IReceivePatient _Parent;
        public IReceivePatient Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
                NotifyOfPropertyChange(() => Parent);
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
                HealthInsuranceContent.CanEdit = true;
            }
            else if (!Globals.ServerConfigSection.Hospitals.IsConfirmHI)
            {
                CanHIEdit = false;
                HealthInsuranceContent.CanEdit = false;
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

        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(SavePatientDetailsAndClose)
            {
                GestureModifier = ModifierKeys.Control,
                GestureKey = Key.S
            };
        }

        public void SavePatientDetailsAndClose()
        {
            bCloseAfterSave = true;
            SaveChangesCmd();
        }

        private IPatientDetailsView currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            currentView = view as IPatientDetailsView;
            //▼===== #008:  Hiện tại do PatientDetails và PatientHiManagement là 1 cặp màn hình cha con. Cho nên nếu cha là Diaglog thì con cũng phỉa set diaglog lên do IsDialogView không được set tự động tất cả các 
            //              contentcontrol con của 1 view. Nên phải set tay.
            if (this.IsDialogView)
            {
                (HealthInsuranceContent as ViewModelBase).IsDialogView = true;
            }
            //▲=====
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
            HealthInsuranceContent.ShowSaveHIAndConfirmCmd = ShowSaveHIAndConfirmCmd;
            if (!AllControlDataLoaded)
            {
                var LoadDataAndWaitTask = new GenericTask_WithWaitEvent(InitDataLoading, NumOfLoadingDataFunctions);
                LoadDataAndWaitTask.ExecuteGenericTask(null);
            }
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

        private bool _HITabVisible = true;
        public bool HITabVisible
        {
            get
            {
                return _HITabVisible;
            }
            set
            {
                _HITabVisible = value;
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
                if (HealthInsuranceContent != null)
                {
                    HealthInsuranceContent.RegistrationType = value;
                }
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

                allProvince = new ObservableCollection<CitiesProvince>();
                allSuburbNames = new ObservableCollection<DataEntities.SuburbNames>();

                _currentPatient = value;
                // HealthInsuranceContent.CurrentPatient = _currentPatient; /*TMA*/

                // TxD 08/11/2014: Why logging the following lines?????? commented them out
                //if (_currentPatient != null)
                //{
                //    ClientLoggerHelper.LogError("PatientDetailsViewModel CurrentPatient Property: Name = [" + _currentPatient.FullName + "] - Code = [" + _currentPatient.PatientCode + "]");
                //}
                //else
                //{
                //    ClientLoggerHelper.LogError("PatientDetailsViewModel CurrentPatient Property: NULL");
                //}
                // 20191118 TNHX: Do AxComboBox Không nhận giá trị null => chọn không được => Set Default
                if (_currentPatient.V_FamilyRelationship == null)
                {
                    _currentPatient.V_FamilyRelationship = 0;
                }

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

        private IPatientHiManagement _healthInsuranceContent;
        public IPatientHiManagement HealthInsuranceContent
        {
            get { return _healthInsuranceContent; }
            set
            {
                _healthInsuranceContent = value;
                NotifyOfPropertyChange(() => HealthInsuranceContent);
            }
        }

        //▼====: #012
        private IPatientPaymentCardManagement _PaymentCardContent;
        public IPatientPaymentCardManagement PaymentCardContent
        {
            get { return _PaymentCardContent; }
            set
            {
                _PaymentCardContent = value;
                NotifyOfPropertyChange(() => PaymentCardContent);
            }
        }
        //▲====: #012

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
                if (_SuburbNames == value)
                {
                    return;
                }
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
                    HealthInsuranceContent.IsChildWindow = IsChildWindow;
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

        private PatientInfoTabs _activeTab;
        public PatientInfoTabs ActiveTab
        {
            get { return _activeTab; }
            set
            {
                _activeTab = value;
                NotifyOfPropertyChange(() => ActiveTab);

                NotifyOfPropertyChange(() => CanSaveChangesCmd);
                NotifyOfPropertyChange(() => CanCancelChangesCmd);
                NotifyOfPropertyChange(() => ConfirmButtonVisibility);
                NotifyOfPropertyChange(() => ShowSaveChangesCmd);
                NotifyOfPropertyChange(() => ShowCancelChangesCmd);
                NotifyOfPropertyChange(() => IsEnableEdit);
            }
        }

        DateTime ExamDate = DateTime.Now;

        //public IEnumerator<IResult> DoGetDateServer()
        //{
        //    var loadCurrentDate = new LoadCurrentDateTask();
        //    yield return loadCurrentDate;
        //    ExamDate = loadCurrentDate.CurrentDate;
        //    yield break;
        //}

        public Visibility ConfirmButtonVisibility
        {
            get
            {
                if ((_activeTab == PatientInfoTabs.HEALTH_INSURANCE_INFO || _activeTab == PatientInfoTabs.PAPER_REFERRAL_INFO)
                    && mNhanBenh_TheBH_XacNhan && CanHIEdit)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        private bool _IsReceivePatient;
        public bool IsReceivePatient
        {
            get { return _IsReceivePatient; }
            set
            {
                if (_IsReceivePatient != value)
                {
                    _IsReceivePatient = value;
                    NotifyOfPropertyChange(() => IsReceivePatient);
                }
            }
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

        public void AcbSuburb_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {

        }


        public void AcbSuburb_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null && SuburbNames != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText == null ? "" : ((AutoCompleteBox)sender).SearchText;
                allSuburbNames = new ObservableCollection<SuburbNames>(SuburbNames.Where(item => ConvertString(item.SuburbName)
                     .IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = allSuburbNames;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }

        public void CreateNewPatient(bool bCreateNewPatientDetailsOnly = false)
        {
            SuburbNames = new ObservableCollection<SuburbNames>();
            CurrentPatient = new Patient
            {
                CountryID = 229,
                //CityProvinceID = 42,
                CurrentClassification = new PatientClassification { PatientClassID = 1 },
                V_Ethnic = 425,
                V_MaritalStatus = 300,
                NationalityID = 190,
                CurrentHealthInsurance = new HealthInsurance()
            };

            GetDefaultProvAndSuburb();

            CurrentPatient.DateBecamePatient = Globals.GetCurServerDateTime();
            // TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
            //Set some default values here.
            //Quoc gia VIET NAM
            //Tinh thanh THANH PHO HO CHI MINH
            //Loai benh nhan BENH NHAN THONG THUONG
            //Dan toc KINH
            // Tinh trang hon nhan CHUA XAC DINH

            if (bCreateNewPatientDetailsOnly)
            {
                ActivationMode = ActivationMode.CREATE_NEW_PATIENT_DETAILS_ONLY;
            }
            else
            {
                ActivationMode = ActivationMode.NEW_PATIENT_FOR_NEW_REGISTRATION;
            }

            //ActivationMode = ActivationMode.NEW

            ActiveTab = PatientInfoTabs.GENERAL_INFO;
            if (CurrentTabIndex != (int)ActiveTab && currentView != null)
            {
                ResetView(currentView);
            }
            StartEditing();
        }

        public void SavePatientCmd()
        {
            this.ShowBusyIndicator();
            try
            {
                //▼====== #006
                if (cboWardName.SelectedIndex >= 0)
                {
                    WardNames SelectedWardName = new WardNames();
                    SelectedWardName.WardNameID = ListWardNames[cboWardName.SelectedIndex].WardNameID;
                    SelectedWardName.WardName = ListWardNames[cboWardName.SelectedIndex].WardName;
                    CurrentPatient.WardName = SelectedWardName;
                }
                else
                {
                    WardNames SelectedWardName = new WardNames();
                    if (!Globals.ServerConfigSection.CommonItems.ShowAddressPKBSHuan)
                    {
                        SelectedWardName.WardName = eHCMSResources.Z2338_G1_KhongXacDinh;
                        SelectedWardName.WardNameID = -1;
                    }
                    else
                    {
                        SelectedWardName.WardName = "";
                        SelectedWardName.WardNameID = -2;
                    }
                    CurrentPatient.WardName = SelectedWardName;
                }
                //▲====== #006
                //▼====== #007
                //if (cboSuburb.SelectedIndex > 0)
                //{
                //    CurrentPatient.SuburbNameID = SuburbNames[cboSuburb.SelectedIndex].SuburbNameID;
                //}
                //▲====== #007

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
                //▼====: #016
                //▼====: #013
                //if (!string.IsNullOrWhiteSpace(CurrentPatient.PatientCellPhoneNumber))
                //{
                //    if (MessageBox.Show("Chưa nhập số ĐT. Bạn có muốn bỏ qua?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                //    {
                //        IsSaving = false;
                //        this.HideBusyIndicator();
                //        return;
                //    }
                //}
                //▲====: #013
                string SDDTRegex = "\\D";
                if (string.IsNullOrWhiteSpace(CurrentPatient.PatientCellPhoneNumber) || CurrentPatient.PatientCellPhoneNumber.Length < 10 || CurrentPatient.PatientCellPhoneNumber.Length > 12 
                    || Regex.IsMatch(CurrentPatient.PatientCellPhoneNumber, SDDTRegex))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại người bệnh là kiểu số, giới hạn tối thiểu 10 ký tự, tối đa 12 ký tự tại trường [ĐTDD] Tab Liên hệ");
                    IsSaving = false;
                    this.HideBusyIndicator();
                    return;
                }
                if (string.IsNullOrWhiteSpace(CurrentPatient.FContactCellPhone) || CurrentPatient.FContactCellPhone != null && CurrentPatient.FContactCellPhone.Length < 10 || CurrentPatient.FContactCellPhone.Length > 12
                    || Regex.IsMatch(CurrentPatient.FContactCellPhone, SDDTRegex))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại người bệnh là kiểu số, giới hạn tối thiểu 10 ký tự, tối đa 12 ký tự tại trường [ĐThoại Di Động] Tab Liên hệ phụ");
                    IsSaving = false;
                    this.HideBusyIndicator();
                    return;
                }
                //▲====: #016
                //▼====: #015
                if (!string.IsNullOrWhiteSpace(CurrentPatient.IDNumber))
                {
                    string CMNDRegex = "\\D";
                    if ((CurrentPatient.IDNumber.Length != 9 && CurrentPatient.IDNumber.Length != 10 && CurrentPatient.IDNumber.Length != 12)
                        || Regex.IsMatch(CurrentPatient.IDNumber, CMNDRegex))
                    {
                        MessageBox.Show("Sai định dạng CMND");
                        IsSaving = false;
                        this.HideBusyIndicator();
                        return;
                    }
                    if (!CurrentPatient.IDCreatedDate.HasValue)
                    {
                        MessageBox.Show("Vui lòng nhập Ngày cấp");
                        IsSaving = false;
                        this.HideBusyIndicator();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(CurrentPatient.IDCreatedFrom))
                    {
                        MessageBox.Show("Vui lòng nhập Nơi cấp");
                        IsSaving = false;
                        this.HideBusyIndicator();
                        return;
                    }
                }
                //▲====: #015

                CurrentPatient.StaffID = Globals.LoggedUserAccount.StaffID.Value;
                //▼====== #007
                CurrentPatient.PatientFullStreetAddress = CurrentPatient.PatientStreetAddress
                    + (CurrentPatient.WardName != null && CurrentPatient.WardName.WardNameID > 0 && !string.IsNullOrEmpty(CurrentPatient.WardName.WardName) ? ", " + CurrentPatient.WardName.WardName : "")
                    + (cboSuburb != null && cboSuburb.SelectedIndex >= 0 ? ", " + SuburbNames[cboSuburb.SelectedIndex].SuburbName : "")
                    + (AcbCity != null && AcbCity.SelectedItem != null && (AcbCity.SelectedItem is CitiesProvince) && (AcbCity.SelectedItem as CitiesProvince).CityProvinceID > 0 ? ", " + (AcbCity.SelectedItem as CitiesProvince).CityProvinceName : "");
                //▲====== #007
                if (CurrentPatient.PatientID > 0)
                {
                    SavePatient(CurrentPatient);
                }
                else
                {
                    AddPatient(CurrentPatient);
                }

                this.HideBusyIndicator();
            }
            catch
            {
                this.HideBusyIndicator();
                MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail);
            }
        }

        public void PagingLinq(long CityProvinceID)
        {
            if (Globals.allSuburbNames == null)
            {
                return;
            }
            selectedSuburbName = null;
            if (!Globals.allSuburbNames.Any(x => x.CityProvinceID == CityProvinceID))
            {
                SuburbNames = new ObservableCollection<SuburbNames>();
            }
            else
            {
                SuburbNames = Globals.allSuburbNames.Where(x => x.CityProvinceID == CityProvinceID).ToObservableCollection();
            }
            IsProcessing = false;
        }

        AxComboBox cboSuburb { get; set; }
        public void cboSuburb_Loaded(object sender, RoutedEventArgs e)
        {
            cboSuburb = sender as AxComboBox;
            //IsProcessing = true;
            //if (CurrentPatient != null &&
            //    CurrentPatient.CityProvinceID > 0)
            //{
            //    PagingLinq((long)CurrentPatient.CityProvinceID);
            //}
            //else
            //{
            //    SuburbNames = new ObservableCollection<SuburbNames>();
            //    NotifyOfPropertyChange(() => SuburbNames);
            //    IsProcessing = false;
            //}
        }

        AxAutoComplete AcbCity { get; set; }
        public void AcbCity_Loaded(object sender, RoutedEventArgs e)
        {
            AcbCity = sender as AxAutoComplete;
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
                if (_IsConfirmedEmergencyPatient != value )
                {
                    _IsConfirmedEmergencyPatient = value;
                    NotifyOfPropertyChange(() => IsConfirmedEmergencyPatient);
                }
            }
        }

        private bool _ShowConfirmedEmergencyPatient = false;
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

        private bool _ShowEmergInPtReExamination = false;
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
                if (!IsHICard_FiveYearsCont)
                {
                    IsHICard_FiveYearsCont_NoPaid = false;
                }
            }
        }

        private bool _IsHICard_FiveYearsCont_NoPaid = false;
        public bool IsHICard_FiveYearsCont_NoPaid
        {
            get { return _IsHICard_FiveYearsCont_NoPaid; }
            set
            {
                _IsHICard_FiveYearsCont_NoPaid = value;
                NotifyOfPropertyChange(() => IsHICard_FiveYearsCont_NoPaid);
                if (IsHICard_FiveYearsCont_NoPaid)
                {
                    IsHICard_FiveYearsCont = true;
                    NotifyOfPropertyChange(() => IsHICard_FiveYearsCont);
                }
            }
        }

        private bool _IsAllowCrossRegion;
        public bool IsAllowCrossRegion
        {
            get
            {
                return _IsAllowCrossRegion;
            }
            set
            {
                _IsAllowCrossRegion = value;
                NotifyOfPropertyChange(() => IsAllowCrossRegion);
            }
        }

        private bool _ShowSaveHIAndConfirmCmd;
        public bool ShowSaveHIAndConfirmCmd
        {
            get { return _ShowSaveHIAndConfirmCmd; }
            set
            {
                if (_ShowSaveHIAndConfirmCmd != value)
                {
                    _ShowSaveHIAndConfirmCmd = value;
                    NotifyOfPropertyChange(() => ShowSaveHIAndConfirmCmd);
                }
            }
        }
        public bool CurrentlyUsed_ToConfirm_HI_Benefit { get; set; }


        public void cboSuburb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //▼====== #006: Lấy dữ liệu về phường xã khi quận thay đổi.
            PagingLinqForWardName(CurrentPatient.SuburbNameID);
            //▲====== #006
        }
        public void cboSuburb_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Cập nhật thông tin bệnh nhân
        /// </summary>
        /// <param name="p">Thông tin bệnh nhân (bệnh nhân đã có trong database rồi).</param>
        private void SavePatient(Patient p)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsSaving = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1102_G1_DangLuuTTinBN)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        /*==== #002 ====*/
                        //contract.BeginUpdatePatient(p, Globals.DispatchCallback(asyncResult =>
                        contract.BeginUpdatePatientAdmin(p, !Globals.isAccountCheck, Globals.DispatchCallback(asyncResult =>
                        /*==== #002 ====*/
                        {
                            try
                            {
                                Patient updatedPatient;
                                /*==== #002 ====*/
                                //var bOK = contract.EndUpdatePatient(out updatedPatient, asyncResult);
                                var bOK = contract.EndUpdatePatientAdmin(out updatedPatient, asyncResult);
                                /*==== #002 ====*/
                                if (bOK)
                                {
                                    IsSaving = false;
                                    GeneralInfoChanged = false;
                                    CurrentPatient = updatedPatient;
                                    //Thông báo cập nhật bệnh nhân OK.
                                    bool bUpdateCurrentPatient_Only = CurrentlyUsed_ToConfirm_HI_Benefit;
                                    Globals.EventAggregator.Publish(new UpdateCompleted<Patient> { Item = updatedPatient, bUpdate_CurrentPatient_Info_Only = bUpdateCurrentPatient_Only });
                                    StartEditing();

                                    // VuTTM - QMS Service
                                    if (GlobalsNAV.IsQMSEnable() 
                                        && 0 != CurrentPatient.OrderNumber)
                                    {
                                        GlobalsNAV.UpdateOrder(CurrentPatient, OrderDTO.CALLING_STATUS);
                                    }

                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);

                                    if (bCloseAfterSave)
                                    {
                                        TryClose();
                                    }
                                }
                                else
                                {
                                    Countries = null;
                                    Nationalities = null;
                                }
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show(innerEx.Message.ToString());
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsSaving = false;
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        /// <summary>
        /// Thêm mới bệnh nhân
        /// </summary>
        /// <param name="p">Thông tin bệnh nhân mới.</param>
        private void AddPatient(Patient p)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsSaving = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1102_G1_DangLuuTTinBN)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddNewPatient(p, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                Patient addedPatient;
                                var bOK = contract.EndAddNewPatient(out addedPatient, asyncResult);

                                if (bOK)
                                {
                                    // VuTTM - Add the order number for a new patient
                                    addedPatient.OrderNumber = p.OrderNumber;
                                    addedPatient.ServiceStartedAt = p.ServiceStartedAt;

                                    IsSaving = false;
                                    GeneralInfoChanged = false;
                                    CurrentPatient = addedPatient;

                                    StartEditing();
                                    //var hiManagementVm = Globals.GetViewModel<IPatientHiManagement>();
                                    HealthInsuranceContent.CurrentPatient = addedPatient;
                                    //PaperReferralContent.CurrentPatient = AddedPatient;
                                    //==== #001
                                    if (QRCode != null)
                                        addedPatient.QRCode = QRCode;
                                    //==== #001
                                    // VuTTM - QMS Service
                                    if (GlobalsNAV.IsQMSEnable()
                                        && 0 != CurrentPatient.OrderNumber)
                                    {
                                        //▼====: #014
                                        OrderDTO temp = GlobalsNAV.UpdateOrder(CurrentPatient, OrderDTO.CALLING_STATUS);
                                        if (temp != null && temp.createdAt != "")
                                        {
                                            CurrentPatient.DateCreatedQMSTicket = DateTime.Parse(temp.createdAt);
                                        }
                                        //▲====: #014
                                    }
                                    Globals.EventAggregator.Publish(new AddCompleted<Patient> { Item = addedPatient });

                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);

                                    if (bCloseAfterSave)
                                    {
                                        TryClose();
                                    }
                                }
                                else
                                {
                                    Countries = null;
                                    Nationalities = null;
                                }
                            }
                            catch (Exception innerEx)
                            {
                                error = new AxErrorEventArgs(innerEx);
                            }
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
                    IsSaving = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        private void StartEditing()
        {
            //Detect form changed (General Info)
            CurrentPatient.PropertyChanged -= CurrentPatient_PropertyChanged;
            CurrentPatient.PropertyChanged += CurrentPatient_PropertyChanged;
            //PatientMedicalRecordCurrent.PropertyChanged -= PatientMedicalRecordCurrent_PropertyChanged;
            //PatientMedicalRecordCurrent.PropertyChanged += PatientMedicalRecordCurrent_PropertyChanged;

            // TxD 12/07/2014 : Commented the following call to the bogus method checkForDisplay() 
            //                  and replaced with the call to new method CheckToModConfirmBtn_And_AllowEditHI
            // checkForDisplay();
            CheckToModConfirmBtn_And_AllowEditHI();

            ChangeFormState(CurrentPatient.PatientID > 0 ? FormState.EDIT : FormState.NEW);
            GeneralInfoChanged = false;
            CurrentPatient.BeginEdit();
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
                case "V_Job":
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
                case "IDCreatedDate":
                case "OccupationDate":
                case "IDCreatedFrom":
                case "SocialInsuranceNumber":
                case "WardNameID":
                case "Passport":
                case "Nationality":
                case "JobID130":
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
            //results = new ObservableCollection<ValidationResult>();
            var patient = p as Patient;
            //if (patient == null)
            //{
            //    return false;
            //}
            //var vc = new ValidationContext(patient, null, null);

            //bool isValid = Validator.TryValidateObject(patient, vc, results, true);

            //if (patient.Age.HasValue)
            //{
            //    if (patient.Age.Value < 0 || patient.Age.Value > 120)
            //    {
            //        var vr = new ValidationResult("Tuổi không hợp lệ.", new[] { "Age" });
            //        results.Add(vr);
            //        isValid = false;
            //    }
            //}

            //if (string.IsNullOrWhiteSpace(patient.PatientCellPhoneNumber) &&
            //    string.IsNullOrWhiteSpace(patient.PatientPhoneNumber))
            //{
            //    var vr = new ValidationResult("Hãy nhập số ĐT di động hoặc ĐT bàn", new[] { "PatientPhoneNumber" });
            //    results.Add(vr);
            //    isValid = false;
            //}

            //return isValid;
            return patient.ValidatePatient(patient, out results);
        }
        public void LoadGenders(EventWaitHandle waitEvent)
        {
            if (Globals.allGenders != null)
            {
                Genders = Globals.allGenders.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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
                                    if (waitEvent != null)
                                    {
                                        waitEvent.Set();
                                    }
                                    this.HideBusyIndicator();
                                }
                                if (allItems != null)
                                {
                                    Globals.allGenders = allItems.ToList();
                                    Genders = Globals.allGenders.ToObservableCollection();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void LoadMaritalStatusList(EventWaitHandle waitEvent)
        {
            if (Globals.allMaritalStatuses != null)
            {
                MaritalStatusList = Globals.allMaritalStatuses.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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

                        contract.BeginGetAllLookupValuesByType(LookupValues.MARITAL_STATUS,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        Globals.allMaritalStatuses = allItems.ToList();
                                        MaritalStatusList = Globals.allMaritalStatuses.ToObservableCollection();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {

                                    if (waitEvent != null)
                                    {
                                        waitEvent.Set();
                                    }
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void LoadEthnicsList(EventWaitHandle waitEvent)
        {
            if (Globals.allEthnics != null)
            {
                EthnicsList = Globals.allEthnics.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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

                        contract.BeginGetAllLookupValuesByType(LookupValues.ETHNIC,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        Globals.allEthnics = allItems.ToList();
                                        EthnicsList = Globals.allEthnics.ToObservableCollection();
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
                                }
                                finally
                                {
                                    if (waitEvent != null)
                                    {
                                        waitEvent.Set();
                                    }
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        public void LoadFamilyRelationshipList(EventWaitHandle waitEvent)
        {
            if (Globals.allFamilyRelationShips != null)
            {
                FamilyRelationshipList = Globals.allFamilyRelationShips.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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

                        try
                        {
                            contract.BeginGetAllLookupValuesByType(LookupValues.FAMILY_RELATIONSHIP,
                                    Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                            if (allItems != null)
                                            {
                                                Globals.allFamilyRelationShips = allItems.ToList();
                                                FamilyRelationshipList = Globals.allFamilyRelationShips.ToObservableCollection();
                                            }
                                        }
                                        catch (Exception ex1)
                                        {
                                            ClientLoggerHelper.LogInfo(ex1.ToString());
                                        }
                                        finally
                                        {
                                            if (waitEvent != null)
                                            {
                                                waitEvent.Set();
                                            }

                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                        }
                        catch (Exception exc)
                        {
                            if (waitEvent != null)
                            {
                                waitEvent.Set();
                            }
                            Globals.ShowMessage(exc.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }
                }
                catch (Exception ex)
                {
                    waitEvent.Set();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void LoadCountries(EventWaitHandle waitEvent)
        {
            if (Globals.allCountries != null)
            {
                Countries = Globals.allCountries.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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

                        contract.BeginGetAllCountries(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllCountries(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allCountries = allItems.ToList();
                                    Countries = Globals.allCountries.ToObservableCollection();
                                }
                            }
                            catch (Exception ex1)
                            {
                                ClientLoggerHelper.LogInfo(ex1.ToString());
                            }
                            finally
                            {
                                if (waitEvent != null)
                                {
                                    waitEvent.Set();
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }



        private bool bCloseAfterSave = false;

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

            ResetView(view as IPatientDetailsView);
        }
        private void ResetView(IPatientDetailsView view)
        {
            view.FocusOnFirstItem();
            view.SetActiveTab(ActiveTab);
            CurrentTabIndex = (int)ActiveTab;

            HealthInsuranceContent.ConfirmHealthInsuranceSelected = false;
            //PaperReferralContent.ConfirmPaperReferalSelected = false;
        }

        private int _currentTabIndex;
        public int CurrentTabIndex
        {
            get { return _currentTabIndex; }
            set
            {
                _currentTabIndex = value;
                NotifyOfPropertyChange(() => CurrentTabIndex);
            }
        }

        public void PatientTabsChanged(object source, object eventArgs)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => Coroutine.BeginExecute(DoChangeTab(source as TabControl)));
        }
        MessageBoxTask _msgTask;
        public IEnumerator<IResult> DoChangeTab(TabControl source)
        {
            TabControl tabPatientInfo = source;
            var destTabIndex = (int)PatientInfoTabs.GENERAL_INFO;//Tab index no dang chuyen toi.
            destTabIndex = tabPatientInfo.SelectedIndex;

            if (tabPatientInfo.SelectedIndex != _currentTabIndex)
            {
                bool isDirty = false;
                if (_currentTabIndex == (int)PatientInfoTabs.GENERAL_INFO)
                {
                    if (GeneralInfoChanged == false)
                    {
                        if (currentView != null)
                        {
                            currentView.UpdateGeneralInfoToSource();
                        }
                    }
                    isDirty = GeneralInfoChanged;
                }
                else if (_currentTabIndex == (int)PatientInfoTabs.HEALTH_INSURANCE_INFO)
                {
                    isDirty = HealthInsuranceContent.InfoHasChanged || HealthInsuranceContent.PaperReferalContent.InfoHasChanged;

                }
                if (isDirty)
                {
                    _msgTask = new MessageBoxTask(eHCMSResources.Z1180_G1_TTinKgDcLuu, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    if (_msgTask.Result == AxMessageBoxResult.Ok)
                    {
                        //Cancel edit.
                        switch (_currentTabIndex)
                        {
                            case (int)PatientInfoTabs.GENERAL_INFO:
                                CancelChangesOnGeneralInfo();
                                break;
                            case (int)PatientInfoTabs.HEALTH_INSURANCE_INFO:
                                HealthInsuranceContent.CancelEditing();
                                break;
                        }

                        //Cho no chuyen tab luon
                        _currentTabIndex = destTabIndex;
                    }
                    else
                    {
                        tabPatientInfo.SelectedIndex = _currentTabIndex;
                    }
                }
                else
                {
                    //Cho no chuyen tab luon
                    _currentTabIndex = destTabIndex;
                }
            }
            ActiveTab = (PatientInfoTabs)tabPatientInfo.SelectedIndex;

            yield break;
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

        public void LoadPatientDetailsAndHI_V2(object patient, object ToRegisOutPt)
        {
            if (patient != null && (((Patient)patient).QRCode != null || IsChildWindow) && currentView != null)
            {
                Enable_ReConfirmHI_InPatientOnly = true;
                ActivationMode = ActivationMode.NEW_PATIENT_FOR_NEW_REGISTRATION;
                CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;
                FormState = FormState.EDIT;
                CloseWhenFinish = true;
            }
            if (IsChildWindow)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            }
            else
            {
                this.ShowBusyIndicator();
            }
            HealthInsuranceContent.IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    //▼====: #009
                    ////KMx: Phải set = null trước khi load, tránh trường hợp load bị lỗi => Thông tin của người A, mà thẻ BH người B. Dẫn đến đăng ký bị lỗi (29/10/2014 10:46).
                    //CurrentPatient = new Patient();
                    //HealthInsuranceContent.CurrentPatient = new Patient();
                    //// TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
                    //SelectedProvince = new CitiesProvince();
                    //▲====: #009
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientByIDFullInfo(((Patient)patient).PatientID, (bool)ToRegisOutPt, Globals.DispatchCallback(asyncResult =>
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
                                    SelectedProvince = CurrentPatient.CitiesProvince;

                                    NotifyOfPropertyChange(() => CurrentPatient.CitiesProvince.CityProvinceName);
                                    NotifyOfPropertyChange(() => CurrentPatient.SuburbName.SuburbName);


                                    PermissionManager.ApplyPermissionToHealthInsuranceList(CurrentPatient.HealthInsurances);
                                    PermissionManager.ApplyPermissionToPaperReferalList(CurrentPatient.PaperReferals);
                                    //▼====: #009
                                    ////them o day moi dung ne
                                    //SuburbNames = new ObservableCollection<SuburbNames>();
                                    //▲====: #009
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

                                HealthInsuranceContent.CurrentPatient = CurrentPatient;
                                HealthInsuranceContent.V_ReceiveMethod = V_ReceiveMethod;
                                //▼====: #012
                                PaymentCardContent.CurrentPatient = CurrentPatient;
                                //▲====: #012
                                //HealthInsuranceContent.ConfirmedItem.HisID=
                                //==== #001
                                if (((Patient)patient).QRCode != null)
                                {
                                    if (IsChildWindow && currentView != null)
                                    {
                                        ActiveTab = PatientInfoTabs.HEALTH_INSURANCE_INFO;
                                        if (CurrentTabIndex != (int)ActiveTab && currentView != null)
                                        {
                                            ResetView(currentView);
                                        }
                                        RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
                                        CurrentlyUsed_ToConfirm_HI_Benefit = true;
                                        ShowEmergInPtReExamination = true;
                                        ShowExtraConfirmHI_Fields = true;
                                    }
                                    HealthInsuranceContent.QRCode = ((Patient)patient).QRCode;
                                    NotifyOfPropertyChange(() => ConfirmButtonVisibility);
                                }
                                else
                                    HealthInsuranceContent.QRCode = QRCode;
                                //==== #001
                                //PaperReferralContent.CurrentPatient = CurrentPatient;

                                StartEditing();
                            }
                            catch (Exception ex)
                            {
                                ClearCurrentPatientContent();
                                Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError("StartEditingPatientLazyLoad EndGetPatientByIDFullInfo Exception Error: " + ex.Message);
                            }
                            finally
                            {
                                if (IsChildWindow)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                                HealthInsuranceContent.IsLoading = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClearCurrentPatientContent();
                    Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError("StartEditingPatientLazyLoad Exception Error: " + ex.Message);
                    HealthInsuranceContent.IsLoading = false;
                    if (IsChildWindow)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }

            });
            t.Start();
        }
        //▼====: #009
        private void ClearCurrentPatientContent()
        {
            CurrentPatient = new Patient();
            HealthInsuranceContent.CurrentPatient = new Patient();
            SelectedProvince = new CitiesProvince();
        }
        //▲====: #009
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

            HealthInsuranceContent.IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    //▼====: #009
                    ////KMx: Phải set = null trước khi load, tránh trường hợp load bị lỗi => Thông tin của người A, mà thẻ BH người B. Dẫn đến đăng ký bị lỗi (29/10/2014 10:46).
                    //CurrentPatient = new Patient();
                    //HealthInsuranceContent.CurrentPatient = new Patient();
                    //// TxD 09/11/2014: Added for SelectedItem of the City Province autocomplete box
                    //SelectedProvince = new CitiesProvince();
                    //▲====: #009



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
                                            SelectedProvince = CurrentPatient.CitiesProvince;

                                        NotifyOfPropertyChange(() => CurrentPatient.CitiesProvince.CityProvinceName);
                                        NotifyOfPropertyChange(() => CurrentPatient.SuburbName.SuburbName);

                                        PermissionManager.ApplyPermissionToHealthInsuranceList(CurrentPatient.HealthInsurances);
                                        PermissionManager.ApplyPermissionToPaperReferalList(CurrentPatient.PaperReferals);
                                        if (CurrentPatient.CityProvinceID != null && CurrentPatient.CityProvinceID.Value > 0)
                                        {
                                            PagingLinq(CurrentPatient.CityProvinceID.Value);
                                        }

                                        GetPMRsByPtIDCurrent(_currentPatient.PatientID);
                                            //▼====: #010
                                            //TBL: Khi đăng ký xong thì bắn PtRegistrationCode để hiển thị đúng
                                            if (CurrentPatient.LatestRegistration_InPt != null && !string.IsNullOrEmpty(CurrentPatient.LatestRegistration_InPt.PtRegistrationCode))
                                        {
                                            Globals.EventAggregator.Publish(new ReloadPtRegistrationCode { PtRegistrationCode = CurrentPatient.LatestRegistration_InPt.PtRegistrationCode });
                                        }
                                        //▲====: #010
                                        //▼====: #017
                                        SetDefaultJob(Globals.allJob130);
                                        //▲====: #017
                                    }
                                    else
                                    {
                                        ClientLoggerHelper.LogInfo("StartEditingPatientLazyLoad EndGetPatientByIDFullInfo Exception Error: CurrentPatient NULL");
                                    }
                                    StartEditing();
                                    HealthInsuranceContent.CurrentPatient = CurrentPatient;
                                        //▼====: #012
                                        PaymentCardContent.CurrentPatient = CurrentPatient;
                                        //▲====: #012
                                        //HealthInsuranceContent.ConfirmedItem.HisID=
                                        //==== #001
                                        if (((Patient)patient).QRCode != null)
                                        HealthInsuranceContent.QRCode = ((Patient)patient).QRCode;
                                    else
                                        HealthInsuranceContent.QRCode = QRCode;
                                        //==== #001
                                        //PaperReferralContent.CurrentPatient = CurrentPatient;
                                    }
                                catch (Exception ex)
                                {
                                        //▼====: #009
                                        ClearCurrentPatientContent();
                                        //▲====: #009
                                        if (theGenTask != null)
                                    {
                                        theGenTask.Error = ex;
                                    }
                                    Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                                    ClientLoggerHelper.LogError("StartEditingPatientLazyLoad EndGetPatientByIDFullInfo Exception Error: " + ex.Message);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    HealthInsuranceContent.IsLoading = false;
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
                    //▼====: #009
                    ClearCurrentPatientContent();
                    //▲====: #009
                    theGenTask.Error = ex;
                    Globals.ShowMessage(eHCMSResources.Z1252_G1_LoadBNXayRaLoi, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError("StartEditingPatientLazyLoad Exception Error: " + ex.Message);
                    HealthInsuranceContent.IsLoading = false;
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
                    HealthInsuranceContent.CanEdit = false;
                }
                else
                {
                    CanHIEdit = true;
                    HealthInsuranceContent.CanEdit = true;
                }
            }
        }
        public void Close()
        {
            Globals.EventAggregator.Publish(new ViewModelClosing<IPatientDetails> { ViewModel = this });
            TryClose();
        }

        public void Handle(ViewModelClosing<IPaperReferral> message)
        {
            if (message != null)
            {
                Close();
            }
        }

        public void Handle(ViewModelClosing<IPatientHiManagement> message)
        {
            if (message != null)
            {
                Close();
            }
        }

        public void Handle(PaperReferalDeleteEvent message)
        {
            if (IsChildWindow)
            {
                ConfirmHIBenefitCmd_Click();
            }
        }

        ///////////////////////////////////////////////////////////

        /// <summary>
        /// Xac nhan the bao hiem + giay chuyen vien cung 1 luc
        /// </summary>
        /// Xem or Xac Nhan Quyen Loi BHYT Button Clicked
        public void ConfirmHIBenefitCmd_Click()
        {
            ConfirmHIBenefit(true);
            //HealthInsuranceContent.ConfirmedItem.HisID=
        }

        // The following method is Called by the ReceivePatientViewModel when the Dang Ky button is pressed
        public bool ConfirmHIBeforeRegister()
        {
            //goi tu receivePatient 
            //ko can tra ve event
            return ConfirmHIBenefit(false);
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

        private bool ConfirmHIBenefit(bool returnEvent, bool IsCheckValid = false)
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
            if (HealthInsuranceContent.V_ReceiveMethod == 0 && PatientFindBy == 0)
            {
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}..", "Vui lòng chọn hình thức tiếp nhận KCB!") });
                return false;
            }


            if (!HealthInsuranceContent.CheckValidationAndGetConfirmedItem(IsConfirmedEmergencyPatient))
            {
                return false;
            }

            // Hpt 07/12/2015: Không cùng lúc check vào cả hai checkbox Xác nhận bệnh nhân có BHYT 5 năm liên tiếp và trẻ em dưới 6 tuổi (lưu ý: quyền lợi cho hai hình thức xác nhận này là khác nhau)
            if (IsHICard_FiveYearsCont && IsChildUnder6YearsOld)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0288_G1_Msg_Info1in2QL));
                return false;
            }
            if (IsHICard_FiveYearsCont && !IsHICard_FiveYearsCont_NoPaid && (FiveYearsAppliedDate == null || FiveYearsAppliedDate == DateTime.MinValue))
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z2397_G1_VLNhapNgayDuocMCCT) });
                return false;
            }
            if (IsHICard_FiveYearsCont && !IsHICard_FiveYearsCont_NoPaid && (FiveYearsARowDate == null || FiveYearsARowDate == DateTime.MinValue))
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", "Vui lòng nhập ngày đủ 5 năm liên tục") });
                return false;
            }

            IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(HealthInsuranceContent.ConfirmedItem, RegistrationType);
            string strHospitalCodeConfirm = "";
            if (HealthInsuranceContent.ConfirmedItem != null)
            {
                strHospitalCodeConfirm = HealthInsuranceContent.ConfirmedItem.RegistrationCode;
            }
            bool TheBHYT_KhongDoBV_CapPhat = Globals.ServerConfigSection.Hospitals.HospitalCode != strHospitalCodeConfirm;

            bool Conditions_Not_Allow_OutPt_TraiTuyen = (!EmergInPtReExamination && !IsChildUnder6YearsOld && !IsAllowCrossRegion && RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU);
            bool PaperRefferal_NotValid = (HealthInsuranceContent.PaperReferalContent == null || HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked == false);
            if (Conditions_Not_Allow_OutPt_TraiTuyen && PaperRefferal_NotValid && TheBHYT_KhongDoBV_CapPhat)
            {
                MessageBox.Show(eHCMSResources.A0251_G1_Msg_InfoBNNgTruKhDcTraiTuyen);
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

                //KMx: Bỏ kiểm tra "Nếu ngày đăng ký BH tiếp theo < Số ngày thuốc BH thì không cho ĐK BH".
                //Trường hợp: Bác sĩ ra toa BH 28 ngày, bệnh nhân uống được 5 ngày sau đó tái khám. VẪN CHO DK BH, CHỈ HIỆN CẢNH BÁO, KHÔNG ĐƯỢC CHẶN.
                //if (CurrentPatient.MaxExamDateHI != null && CurrentPatient.MaxDayRptsHI != null)
                //{
                //    //lay ngay hien tai - ngay dang ky BH gan nhat
                //    if (diff1.TotalDays < Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()))
                //    {
                //        if ((Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()) - diff1.TotalDays) > DifferenceDayPrecriptHI)
                //        {
                //            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0250_G1_Msg_InfoKhTheDKBHYT_BNConNhieuThuocBH));
                //            return false;
                //        }
                //        if ((Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()) - diff1.TotalDays) <= DifferenceDayPrecriptHI)
                //        {
                //            if (MessageBox.Show("Bệnh nhân này còn một ít thuốc BHYT.Bạn có muốn ĐK BHYT cho bệnh nhân này không?(<=" + DifferenceDayPrecriptHI.ToString() + " )", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                //            {
                //                return false;
                //            }
                //        }
                //        //KMx: Cái if bên dưới không phải Kiên bỏ ra, someone đã bỏ ra từ trước rồi.
                //        //if ((Convert.ToDouble(CurrentPatient.MaxDayRptsHI.GetValueOrDefault()) - diff1.TotalDays) <= DifferenceDayPrecriptHI)
                //        //{
                //        //    Coroutine.BeginExecute(WarningResult("Bệnh nhân này còn một ít thuốc BHYT.Bạn có muốn ĐK BHYT cho bệnh nhân này không?(<=" + DifferenceDayPrecriptHI.ToString() + " )"
                //        //        , eHCMSResources.Z1259_G1_TiepTucDK));
                //        //    if(!flag)
                //        //    {
                //        //        return false;
                //        //    }
                //        //}
                //    }
                //}
                //------------------------------------------------------2014/02/20 11:00------------------------------------------------------

                //KMx: Kiểm tra nếu lần đăng ký trước cách lần đăng ký này chưa đến 28 ngày(cấu hình) thì thông báo.
                if (diff1.TotalDays < DifferenceDayRegistrationHI)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1034_G1_BNMoiDKKhamBHYT, MaxExamDateHITemp.ToString("dd/MM/yyyy")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return false;
                    }
                }

                //------------------------------------------------------2014/02/20 11:14------------------------------------------------------

                //else if (CurrentPatient.MaxExamDateHI != null)
                //{
                //    if (diff1.TotalDays < DifferenceDayRegistrationHI)
                //    {
                //        MessageBox.Show("Bệnh nhân này mới ĐK Khám BHYT ngày " + MaxExamDateHITemp.ToString("dd/MM/yyyy") + ".Nên không thể tiếp tục ĐK Khám BHYT!");
                //        return false;
                //    }

                //    else if (diff1.TotalDays >= DifferenceDayRegistrationHI && diff1.TotalDays <= DifferenceDayRegistrationHI)
                //    {
                //        if (MessageBox.Show("Bệnh nhân này mới ĐK Khám BHYT ngày " + MaxExamDateHITemp.ToString("dd/MM/yyyy") + ".Bạn có muốn ĐK BHYT cho bệnh nhân này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                //        {
                //            return false;
                //        }
                //    }
                //}
            }
            if (Globals.ServerConfigSection.CommonItems.CheckHIWhenConfirm)
            {
                if (!CheckHiCard(out string WarningMessage))
                {
                    if (String.IsNullOrWhiteSpace(WarningMessage))
                    {
                        return false;
                    }
                    else
                    {
                        IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                        MessBox.FireOncloseEvent = true;
                        MessBox.IsShowReason = true;
                        MessBox.SetMessage(WarningMessage, eHCMSResources.Z0627_G1_TiepTucLuu);
                        GlobalsNAV.ShowDialog_V3(MessBox);
                        if (!MessBox.IsAccept)
                        {
                            return false;
                        }
                    }
                }
            }
            //if (!HealthInsuranceContent.ConfirmHealthInsuranceSelected)
            //{
            //    Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z0157_G1_ChuaKTraTheBH) });
            //    return false;
            //}
            var evt = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
            hiCardConfirmedEvent = new HiCardConfirmedEvent { Source = this, HiProfile = HealthInsuranceContent.ConfirmedItem };
            if (HealthInsuranceContent.PaperReferalContent.PaperReferalInUse != null && HealthInsuranceContent.PaperReferalContent.PaperReferalInUse.IsChecked)
            {
                evt.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
                hiCardConfirmedEvent.PaperReferal = HealthInsuranceContent.PaperReferalContent.PaperReferalInUse;
            }
            //▼====== #003
            //if (IsCheckValid)
            //{
            //    GlobalsNAV.ShowDialog<ICheckedValidHICard>((ICheckedValidHICard aView) =>
            //    {
            //        HealthInsuranceContent.ConfirmedItem.Patient = CurrentPatient;
            //        aView.gHealthInsurance = HealthInsuranceContent.ConfirmedItem;
            //    });
            //}
            //else 
            //▲====== #003
            
            if (returnEvent)
            {
                evt.IsEmergency = false;
                evt.IsEmergInPtReExamination = EmergInPtReExamination;
                evt.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
                evt.IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(HealthInsuranceContent.ConfirmedItem, AllLookupValues.RegistrationType.NGOAI_TRU);
                evt.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
                evt.IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid;
                evt.FiveYearsAppliedDate = FiveYearsAppliedDate;
                evt.FiveYearsARowDate = FiveYearsARowDate;
                evt.V_ReceiveMethod = HealthInsuranceContent.V_ReceiveMethod;
                Globals.EventAggregator.Publish(evt);
            }
            return true;
        }
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
            bool bCanClose = false;

            switch (_activeTab)
            {
                case PatientInfoTabs.GENERAL_INFO:
                    bCanClose = CanClose(GeneralInfoChanged);
                    break;

                case PatientInfoTabs.HEALTH_INSURANCE_INFO:
                    bCanClose = CanClose(HealthInsuranceContent.InfoHasChanged || HealthInsuranceContent.PaperReferalContent.InfoHasChanged);
                    break;
            }

            if (bCanClose)
            {
                Close();
            }
        }

        public void OldPtNamesLogCmd()
        {
            if (CurrentPatient != null && CurrentPatient.OldPtNamesLog != null && CurrentPatient.OldPtNamesLog.Length > 1)
            {
                MessageBox.Show(CurrentPatient.OldPtNamesLog);
            }
            else
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0988_G1_BNKgCoLSuCNhatTen));
            }
        }

        public bool ShowSaveChangesCmd
        {
            get
            {
                return _activeTab == PatientInfoTabs.GENERAL_INFO && mNhanBenh_ThongTin_Sua;
            }
        }
        /// <summary>
        /// Luu thong tin general info, hoac the bao hiem, hoac giay chuyen vien tuy theo TabItem nao dang duoc kich hoat
        /// </summary>
        public void SaveChangesCmd()
        {
            switch (_activeTab)
            {
                case PatientInfoTabs.GENERAL_INFO:
                    IsSaving = true;
                    SavePatientCmd();
                    break;
            }
        }
        public bool CanSaveChangesCmd
        {
            get
            {
                switch (_activeTab)
                {
                    case PatientInfoTabs.GENERAL_INFO:
                        return GeneralInfoChanged && !IsSaving;

                    default:
                        return false;
                }
            }
        }

        public bool ShowCancelChangesCmd
        {
            get
            {
                return _activeTab == PatientInfoTabs.GENERAL_INFO && mNhanBenh_ThongTin_Sua;
            }
        }
        public bool IsEnableEdit
        {
            get
            {
                if (CurrentPatient == null)
                {
                    return false;
                }
                else if (CurrentPatient.PatientID == 0)
                {
                    return true;
                }
                else
                {
                    return _activeTab == PatientInfoTabs.GENERAL_INFO && mThongTinChungBN_Edit;
                }
            }
        }
        public void CancelChangesCmd()
        {
            switch (_activeTab)
            {
                case PatientInfoTabs.GENERAL_INFO:
                    CancelChangesOnGeneralInfo();
                    break;
            }
        }
        public bool CanCancelChangesCmd
        {
            get
            {
                switch (_activeTab)
                {
                    case PatientInfoTabs.GENERAL_INFO:
                        return GeneralInfoChanged;
                    default:
                        return false;
                }
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
            var vm = sender.DataContext as PatientDetailsViewModel;
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
                        //▼===== #011
                        //p.Age = null;
                        p.Age = 0;
                        //▲===== #011
                        p.YOB = null;
                    }
                    //------- DPT 08/11/2017 < 6 tuổi
                    if (p.Age <= 6)
                    {
                        if (!p.DOBForBaby.HasValue)
                        {
                            //▼===== #011
                            //p.Age = null;
                            p.Age = 0;
                            //▲===== #011
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
                var vm = sender.DataContext as PatientDetailsViewModel;
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
            var vm = sender.DataContext as PatientDetailsViewModel;
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
                var vm = sender.DataContext as PatientDetailsViewModel;
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
            var vm = sender.DataContext as PatientDetailsViewModel;
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

                    //------- DPT 08/11/2017 < 6 tuổi
                    //int monthnew;
                    //DateTime today = DateTime.Today;
                    //DateTime day = Convert.ToDateTime(p.DOBForBaby);
                    //monthnew = (today.Month + today.Year * 12) - (day.Month + day.Year * 12);
                    //if (monthnew <= 72)
                    //{
                    //    p.MonthsOld = monthnew;
                    ////    if (!(p.V_FamilyRelationshi;p.HasValue) || string.IsNullOrWhiteSpace(p.FContactFullName))
                    ////    {
                    ////        MessageBox.Show("Trẻ em dưới 6 tuổi bạn nên nhập đầy đủ thông tin người thân");
                    ////    }

                    //}

                    //--------------------------------

                }
                //else
                //{
                //    p.Age = null;
                //    p.YOB = null;
                //}
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

        #region Extra Fields Added to allow this View to do the Job of Confirm HI like ReceivePatient VM

        private bool _showExtraConfirmHI_Fields = false;
        public bool ShowExtraConfirmHI_Fields
        {
            get
            {
                return _showExtraConfirmHI_Fields;
            }
            set
            {
                _showExtraConfirmHI_Fields = value;
                NotifyOfPropertyChange(() => ShowExtraConfirmHI_Fields);
            }
        }

        #endregion
        private bool _mThongTinChungBN_Edit = true;
        public bool mThongTinChungBN_Edit
        {
            get
            {
                return _mThongTinChungBN_Edit;
            }
            set
            {
                if (_mThongTinChungBN_Edit == value)
                {
                    return;
                }
                _mThongTinChungBN_Edit = value;
                NotifyOfPropertyChange(() => mThongTinChungBN_Edit);
            }
        }
        public void authorization()
        {

            if (!Globals.isAccountCheck)
            {
                return;
            }
            mThongTinChungBN_Edit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen,
                                       (int)eModuleGeneral.mThongTinChungBN, (int)oModuleGeneralEX.mThongTinChungBN_Edit, (int)ePermission.mView);
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

        #endregion

        #region binding visibilty

        //public HyperlinkButton lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as HyperlinkButton;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        private bool _VisiDeletePatient = !Globals.isAccountCheck;
        public bool VisiDeletePatient
        {
            get { return _VisiDeletePatient; }
            set
            {
                if (_VisiDeletePatient != value)
                {
                    _VisiDeletePatient = value;
                    NotifyOfPropertyChange(() => VisiDeletePatient);
                }
            }
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
        public void CheckValidHICardCmd()
        {
            //▼====== #004
            if (HealthInsuranceContent.EditingHiItem != null)
            {
                if (!string.IsNullOrEmpty(HealthInsuranceContent.EditingHiItem.HICardNo))
                {
                    try
                    {
                        GlobalsNAV.ShowDialog<ICheckedValidHICard>((ICheckedValidHICard aView) =>
                        {
                            HealthInsuranceContent.EditingHiItem.Patient = CurrentPatient;
                            aView.gHealthInsurance = HealthInsuranceContent.EditingHiItem;
                        });
                    }
                    catch
                    {
                        MessageBox.Show(eHCMSResources.Z1072_G1_TBaoQLyChTrKhiThayLoiNay);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2316_G1_KhongTheKiemTraTheKhongMa);
                    return;
                }
            }
            //▲====== #004
            //▼====== #003
            else
            {
                bool cOK = false;
                HealthInsuranceContent.IsCheckOnline = true;
                cOK = HealthInsuranceContent.CheckValidationAndGetConfirmedItem();
                if (cOK)
                {
                    try
                    {
                        GlobalsNAV.ShowDialog<ICheckedValidHICard>((ICheckedValidHICard aView) =>
                        {
                            HealthInsuranceContent.ConfirmedItem.Patient = CurrentPatient;
                            aView.gHealthInsurance = HealthInsuranceContent.ConfirmedItem;
                        });
                    }
                    catch
                    {
                        //Lỗi này có thể xảy ra khi người dùng rớt mạng trước khi click kiểm tra online thẻ BHYT
                        MessageBox.Show(eHCMSResources.Z1072_G1_TBaoQLyChTrKhiThayLoiNay);
                        return;
                    }
                }
                else
                {
                    //Không làm gì cả.
                }
            }
            //▲====== #003
            //ConfirmHIBenefit(true, true);
        }

        //▼====== #006
        #region Khu vực load Tỉnh thành, Quận huyện, Phường Xã.
        public void GetDefaultProvAndSuburb()
        {
            if (CurrentPatient != null && CurrentPatient.PatientID > 0)
            {
                return;
            }
            CurrentPatient.CitiesProvince = new CitiesProvince();
            long CityProvinceID_ViewModel = 0;
            ObservableCollection<SuburbNames> SuburbNames_ViewModel = new ObservableCollection<SuburbNames>();
            string CityProvinceName_ViewModel = "";
            Globals.GetDefaultForProvinceAndSuburb(out CityProvinceID_ViewModel, out SuburbNames_ViewModel, out CityProvinceName_ViewModel);
            CurrentPatient.CitiesProvince.CityProvinceID = CityProvinceID_ViewModel;
            CurrentPatient.CityProvinceID = CityProvinceID_ViewModel;
            CurrentPatient.CitiesProvince.CityProvinceName = CityProvinceName_ViewModel;
            SuburbNames = SuburbNames_ViewModel;
        }
        private ObservableCollection<WardNames> _ListWardNames;
        public ObservableCollection<WardNames> ListWardNames
        {
            get { return _ListWardNames; }
            set
            {
                _ListWardNames = value;
                NotifyOfPropertyChange(() => ListWardNames);
            }
        }

        AxComboBox cboWardName { get; set; }
        public void cboWardName_Loaded(object sender, RoutedEventArgs e)
        {
            cboWardName = sender as AxComboBox;
        }
        public void PagingLinqForWardName(long SuburbNameID)
        {
            if (Globals.allWardNames == null)
            {
                return;
            }
            var mListWardNames = new ObservableCollection<WardNames>();
            WardNames defaultWardNames = new WardNames();
            if (!Globals.ServerConfigSection.CommonItems.ShowAddressPKBSHuan)
            {
                defaultWardNames.WardName = eHCMSResources.Z2338_G1_KhongXacDinh;
                defaultWardNames.WardNameID = -1;
            }
            else
            {
                defaultWardNames.WardName = "";
                defaultWardNames.WardNameID = -2;
            }
            defaultWardNames.SuburbNameID = SuburbNameID;
            mListWardNames.Add(defaultWardNames);
            foreach (var item in Globals.allWardNames)
            {
                if (item.SuburbNameID == SuburbNameID)
                {
                    mListWardNames.Add(item);
                }
            }
            ListWardNames = new ObservableCollection<WardNames>(mListWardNames);
        }

        private void GetAllSuburbNameAction(EventWaitHandle waitEvent)
        {
            if (Globals.allSuburbNames != null)
            {
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSuburbNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllSuburbNames(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allSuburbNames = allItems.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                if (waitEvent != null)
                                {
                                    waitEvent.Set();
                                }
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetAllWardNameAction(EventWaitHandle waitEvent)
        {
            if (Globals.allWardNames != null)
            {
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllWardNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllWardNames(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allWardNames = allItems.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                if (waitEvent != null)
                                {
                                    waitEvent.Set();
                                }
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void GetAllProvinces(EventWaitHandle waitEvent)
        {
            if (Globals.allCitiesProvince != null)
            {
                Provinces = Globals.allCitiesProvince.ToObservableCollection();
                //▼====: #013
                if (QRCode != null) // 06/02/2021 Chỉ chạy hàm khi check mã bảo hiểm
                {
                    SetAddressByHISAddress(); //19-11-2020 Gọi hàm lấy địa chỉ
                }
                //▲====: #013
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allCitiesProvince = allItems.ToList();
                                    Provinces = Globals.allCitiesProvince.ToObservableCollection();
                                    //▼====: #013
                                    if (QRCode != null || IDCardQRCode != null) // 06/02/2021 Chỉ chạy hàm khi check mã bảo hiểm
                                    {
                                        SetAddressByHISAddress(); //19-11-2020 Gọi hàm lấy địa chỉ
                                    }
                                    //▲====: #013
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                                bContinue = false;
                            }
                            finally
                            {
                                if (waitEvent != null)
                                {
                                    waitEvent.Set();
                                }
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        //▲====== #006

        //public DateTime? FiveYearsAppliedDate;
        //public DateTime? FiveYearsARowDate;
        private DateTime? _FiveYearsAppliedDate;
        public DateTime? FiveYearsAppliedDate
        {
            get { return _FiveYearsAppliedDate; }
            set
            {
                _FiveYearsAppliedDate = value;
                NotifyOfPropertyChange(() => FiveYearsAppliedDate);
            }
        }
        private DateTime? _FiveYearsARowDate;
        public DateTime? FiveYearsARowDate
        {
            get { return _FiveYearsARowDate; }
            set
            {
                _FiveYearsARowDate = value;
                NotifyOfPropertyChange(() => FiveYearsARowDate);
            }
        }


        public void FiveYearsAppliedDate_DateChanged(object sender, DateTimeSelectedEventArgs e)
        {
            FiveYearsAppliedDate = e.NewValue;
        }
        public void FiveYearsARowDate_DateChanged(object sender, DateTimeSelectedEventArgs e)
        {
            FiveYearsARowDate = e.NewValue;
        }
        public void Handle(DoubleClickEvent message)
        {
            if (message != null && ConfirmButtonVisibility == Visibility.Visible
                && (Enable_ReConfirmHI_InPatientOnly || Globals.ServerConfigSection.Hospitals.IsConfirmHI))
            {
                ConfirmHIBenefitCmd_Click();
            }
        }
        public void Handle(SaveHIAndConfirm message)
        {
            if (message != null)
            {
                if (IsReceivePatient)
                {
                    Globals.EventAggregator.Publish(new SaveHIAndInPtConfirmHICmd());
                }
                else
                {
                    ConfirmHIBenefitCmd_Click();
                }
            }
        }

        public void DeletedPatient()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            if ((MessageBox.Show(eHCMSResources.Z0354_G1_BanCoMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel)) != MessageBoxResult.OK)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePatientByID(CurrentPatient.PatientID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                bool result = contract.EndDeletePatientByID(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.Z2772_G1_XoaBNThanhCong, eHCMSResources.G0442_G1_TBao);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z2773_G1_XoaBNThatBai, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //▼====== #012
        public void PrintPatientCard()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.CurPatient = CurrentPatient;
                proAlloc.eItem = ReportName.InTheKCB;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //▲====== #012
        //▼====== #013
        #region Hàm lấy thông tin địa chỉ đưa vào control
        //Lấy Tỉnh đưa vào autocompletebox
        public void setProvinceByPatientAddress()
        {
            if (CurrentPatient != null && CurrentPatient.CitiesProvince != null && Provinces != null)
            {
                long tempCityID = Provinces.Where(x => x.CityProvinceName.Contains(CurrentPatient.CitiesProvince.CityProvinceName)).FirstOrDefault().CityProvinceID;
                CurrentPatient.CityProvinceID = tempCityID;

                CurrentPatient.SuburbNameID = -1;
                PagingLinq(tempCityID);
                setSuburbByPatientAddress();
            }
        }
        //Lấy quận huyện đưa vào combobox
        public void setSuburbByPatientAddress()
        {
            if (CurrentPatient != null && SuburbNames != null)
            {
                long SuburbNameID = SuburbNames.Where(x => x.SuburbName.Contains(tempSuburbName)).FirstOrDefault().SuburbNameID;
                CurrentPatient.SuburbNameID = SuburbNameID;

                CurrentPatient.WardNameID = -1;
                PagingLinqForWardName(SuburbNameID);
                setWardByPatientAddress();
            }
        }

        //Lấy phường đưa vào combobox
        public void setWardByPatientAddress()
        {
            if (CurrentPatient != null && ListWardNames != null)
            {
                long WardNameID = ListWardNames.Where(x => x.WardName.Contains(tempWardName)).FirstOrDefault().WardNameID;
                CurrentPatient.WardNameID = WardNameID;
            }
        }
        //Chạy hàm khi lấy đủ dữ liệu
        public void SetAddressByHISAddress()
        {
            var t = new Thread(() =>
            {
                try
                {
                    if (AllControlDataLoaded)
                    {
                        setProvinceByPatientAddress();
                        this.HideBusyIndicator();
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);

                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        //▲====== #013
        private ObservableCollection<Lookup> _ObjV_Job;
        public ObservableCollection<Lookup> ObjV_Job
        {
            get { return _ObjV_Job; }
            set
            {
                _ObjV_Job = value;
                NotifyOfPropertyChange(() => ObjV_Job);
            }
        }
        public void LoadJobList(EventWaitHandle waitEvent)
        {
            if (Globals.allJobs != null)
            {
                ObjV_Job = Globals.allJobs.ToObservableCollection();
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
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

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_Job,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        Globals.allJobs = allItems.ToList();
                                        ObjV_Job = Globals.allJobs.ToObservableCollection();
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
                                }
                                finally
                                {
                                    if (waitEvent != null)
                                    {
                                        waitEvent.Set();
                                    }
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (waitEvent != null)
                    {
                        waitEvent.Set();
                    }
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }
        public void cboNationality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CurrentPatient.NationalityID != null && (string.IsNullOrEmpty(CurrentPatient.Nationality) || Nationalities.Where(x=>x.NationalityName == CurrentPatient.Nationality).Count() > 0))
            {
                CurrentPatient.Nationality = Nationalities .Where(x => x.NationalityID == CurrentPatient.NationalityID).FirstOrDefault().NationalityName;
            }
        }
        //▼====: #016
        
        string txt = "";
        public void txtCellPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                CurrentPatient.FContactCellPhone = txt;
                //if (string.IsNullOrEmpty(CurrentPatient.FContactCellPhone))
                //{
                //    CurrentPatient.FContactCellPhone = txt;
                //}
            }
        }

        public void txtContactCellPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                CurrentPatient.PatientCellPhoneNumber = txt;
            }
        }
        //▲====: #016

        //▼====: #017
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

        private IEnumerator<IResult> DoNationalityList()
        {
            if(Globals.allNationalities != null)
            {
                Nationalities = Globals.allNationalities.ToObservableCollection();
                yield break;
            }
            var paymentTask = new LoadNationalityListTask(false, false);
            yield return paymentTask;
            Nationalities = paymentTask.RefNationalityList;
            yield break;
        }

        private ObservableCollection<RefJob> _ListJobParent;
        public ObservableCollection<RefJob> ListJobParent
        {
            get { return _ListJobParent; }
            set
            {
                _ListJobParent = value;
                NotifyOfPropertyChange(() => ListJobParent);
            }
        }

        private ObservableCollection<RefJob> _ListJobParentAcb;
        public ObservableCollection<RefJob> ListJobParentAcb
        {
            get { return _ListJobParentAcb; }
            set
            {
                _ListJobParentAcb = value;
                NotifyOfPropertyChange(() => ListJobParentAcb);
            }
        }

        private RefJob _SelectedJobParent;
        public RefJob SelectedJobParent
        {
            get { return _SelectedJobParent; }
            set
            {
                _SelectedJobParent = value;
                NotifyOfPropertyChange(() => SelectedJobParent);
            }
        }

        private ObservableCollection<RefJob> _ListJobChild;
        public ObservableCollection<RefJob> ListJobChild
        {
            get { return _ListJobChild; }
            set
            {
                _ListJobChild = value;
                NotifyOfPropertyChange(() => ListJobChild);
            }
        }

        private RefJob _SelectedJobChild;
        public RefJob SelectedJobChild
        {
            get { return _SelectedJobChild; }
            set
            {
                _SelectedJobChild = value;
                NotifyOfPropertyChange(() => SelectedJobChild);
            }
        }

        private IEnumerator<IResult> DoJobList()
        {
            var paymentTask = new LoadJobListTask();
            yield return paymentTask;
            ListJobParent = paymentTask.RefJobList.Where(j => j.JobParentID == 0).ToObservableCollection();
            yield break;
        }

        public void cboJobChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedJobChild = (sender as ComboBox).SelectedItem as RefJob;
            if(CurrentPatient != null && SelectedJobChild != null && CurrentPatient.JobID130 != SelectedJobChild.JobID)
            {
                CurrentPatient.JobID130 = SelectedJobChild.JobID;
            }
        }

        public bool ApplyReport130
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.ApplyReport130;
            }
        }

        private void SetDefaultJob(List<RefJob> JobList)
        {
            if(CurrentPatient.JobID130 != 0 && JobList != null && JobList.Count > 0)
            {
                SelectedJobChild = JobList.Where(j => j.JobID == CurrentPatient.JobID130 && j.JobParentID != 0).FirstOrDefault();
                if(SelectedJobChild != null)
                {
                    ListJobChild = JobList.Where(j => j.JobParentID == SelectedJobChild.JobParentID && j.JobParentID != 0).ToObservableCollection();
                    if(ListJobParent == null)
                    {
                        SelectedJobParent = JobList.Where(p => p.JobID == SelectedJobChild.JobParentID && p.JobParentID == 0).FirstOrDefault();
                        return;
                    }
                    SelectedJobParent = ListJobParent.Where(p => p.JobID == SelectedJobChild.JobParentID).FirstOrDefault();
                }
            }
        }

        public void LoadJob130List(EventWaitHandle waitEvent)
        {
            if (Globals.allJob130 != null)
            {
                ListJobParent = Globals.allJob130.Where(j => j.JobParentID == 0).ToObservableCollection();
                if(CurrentPatient != null)
                {
                    SetDefaultJob(Globals.allJob130);
                }
                if (waitEvent != null)
                {
                    waitEvent.Set();
                }
                return;
            }
            Coroutine.BeginExecute(DoJobList());
            if (CurrentPatient != null)
            {
                SetDefaultJob(Globals.allJob130);
            }
            if (waitEvent != null)
            {
                waitEvent.Set();
            }
        }

        AxAutoComplete AcbJob { get; set; }
        public void AcbJob_Loaded(object sender, RoutedEventArgs e)
        {
            AcbJob = sender as AxAutoComplete;
        }

        public void AcbJob_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender == null || ListJobParent == null)
            {
                return;
            }

            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            string SearchText = cboContext.SearchText;
            if (SearchText.Length == 0 && SelectedJobParent == null)
            {
                cboContext.PopulateComplete();
                ListJobChild = new ObservableCollection<RefJob>();
                return;
            }

            cboContext.ItemsSource = new ObservableCollection<RefJob>(ListJobParent.Where(item => ConvertString(item.JobName)
                    .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
            cboContext.PopulateComplete();
        }

        public void AcbJob_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
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
            long JobID = ((sender as AutoCompleteBox).SelectedItem as RefJob).JobID;
            SelectedJobParent = ListJobParent.Where(x => x.JobID == JobID).FirstOrDefault();
            if (SelectedJobParent != null)
            {
                if (Globals.allJob130 != null && SelectedJobParent != null)
                {
                    ListJobChild = Globals.allJob130.Where(j => j.JobParentID != 0 && j.JobParentID == SelectedJobParent.JobID).ToObservableCollection();
                }
            }
            else
            {
                ListJobChild = new ObservableCollection<RefJob>();
                NotifyOfPropertyChange(() => ListJobChild);
                IsProcessing = false;
            }
        }
        //▲====: #017
        private long _V_ReceiveMethod;

        public long V_ReceiveMethod
        {
            get { return _V_ReceiveMethod; }
            set
            {
                _V_ReceiveMethod = value;
                NotifyOfPropertyChange(() => V_ReceiveMethod);
            }
        }
        private string HIAPICheckHICardAddress = "http://egw.baohiemxahoi.gov.vn/api/egw/KQNhanLichSuKCB2019?token={0}&id_token={1}&username={2}&password={3}";
        private HIAPICheckedHICard _HIAPICheckedHICard;
        public HIAPICheckedHICard HIAPICheckedHICard
        {
            get => _HIAPICheckedHICard; set
            {
                _HIAPICheckedHICard = value;
                NotifyOfPropertyChange(() => HIAPICheckedHICard);
            }
        }
        //private HealthInsurance _gHealthInsurance;
        //public HealthInsurance gHealthInsurance
        //{
        //    get => _gHealthInsurance; set
        //    {
        //        _gHealthInsurance = value;
        //        NotifyOfPropertyChange(() => gHealthInsurance);
        //    }
        //}
        private bool CheckHiCard(out string WarningMessage)
        {
            WarningMessage = "";
            if (GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.maKetQua != 200)
            {
                MessageBox.Show("Không có phản hồi từ cổng BH. Vui lòng kiểm tra thông tin thẻ chính xác khi xác nhận!");
                return true;
            }
            this.ShowBusyIndicator();
            GetHICardInfo();
            if (HIAPICheckedHICard.maKetQua == 401)
            {
                GlobalsNAV.LoginHIAPI();
                GetHICardInfo();
            }
            if(HIAPICheckedHICard.maThe != HealthInsuranceContent.ConfirmedItem.HICardNo)
            {
                MessageBox.Show("Mã thẻ BHYT sai so với mã thẻ người bệnh trên cổng giám định. Vui lòng nhập lại!");
                return false;
            }
            if(HIAPICheckedHICard.gtTheTu != HealthInsuranceContent.ConfirmedItem.ValidDateFrom.Value.ToString("dd/MM/yyyy")
                || HIAPICheckedHICard.gtTheDen != HealthInsuranceContent.ConfirmedItem.ValidDateTo.Value.ToString("dd/MM/yyyy"))
            {
                WarningMessage = "Thời hạn thẻ BHYT sai so với thời hạn thẻ người bệnh trên cổng giám định. Vui lòng nhập lại!";
                //MessageBox.Show("Thời hạn thẻ BHYT sai so với thời hạn thẻ người bệnh trên cổng giám định. Vui lòng nhập lại!");
                return false;
            }
            if (HIAPICheckedHICard.gioiTinh != CurrentPatient.GenderString)
            {
                MessageBox.Show("Giới tính người bệnh sai so với thông tin cổng giám định. Vui lòng nhập lại!");
                return false;
            }
            if (HIAPICheckedHICard.ngaySinh != CurrentPatient.DOBText)
            {
                MessageBox.Show("Ngày tháng năm sinh của người bệnh sai so với thông tin cổng giám định. Vui lòng nhập lại!");
                return false;
            }
            return true;
        }
 
        private void GetHICardInfo()
        {
            string mHIAPICheckHICardAddress = string.Format(HIAPICheckHICardAddress, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password);
            string mHIData = string.Format("{{\"maThe\":\"{0}\",\"hoTen\":\"{1}\",\"ngaySinh\":\"{2}\"}}", HealthInsuranceContent.ConfirmedItem.HICardNo.ToUpper()
                , CurrentPatient.FullName.ToUpper()
                , CurrentPatient.DOB.HasValue 
                && CurrentPatient.DOB != null 
                && CurrentPatient.DOB.Value.Day == 1 
                && CurrentPatient.DOB.Value.Month == 1 
                ? CurrentPatient.DOB.Value.Year.ToString() : CurrentPatient.DOBText);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(mHIAPICheckHICardAddress, mHIData);
            HIAPICheckedHICard = GlobalsNAV.ConvertJsonToObject<HIAPICheckedHICard>(mRestJson);
            this.HideBusyIndicator();
        }
    }
}
