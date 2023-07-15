using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using DataEntities;
using Service.Core.Common;
using System.Linq;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.DataValidation;
using aEMR.Common.Collections;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.CommonTasks;
using aEMR.Controls;
using System.Windows.Data;
using aEMR.Common.BaseModel;
/*
* 20170113 #001 CMN: Add QRCode
* 20180905 #002 TMM: Add ValidDateTo tu 00:00:00 -> 23:59:59.
* 20182105 #003 TMM: Thêm điều kiện cho phép người dùng được thêm mới hoặc sửa đổi một thẻ BHYT chưa sử dụng tùy ý, nhưng chỉ có thẻ thỏa điều kiện ngày nhập viện không được bé hơn ngày đến thì mới xác nhận đc.
* 20180926 #004 TTM: Thêm điều kiện, nếu như là kiểm tra BHYT online thì không cần kiểm tra giấy chuyển viện.
* 20181103 #005 TTM: BM 0005215: Mặc định giá trị cho ngày đến bằng ngày từ + 1 năm.
* 20181113 #006 TTM: BM 0005228: Thêm trường phường xã để nhập liệu.
* 20191113 #007 TTM: BM 0019566: Lỗi khi mở PatientDetails và PatientHiManagement như 1 popup => Không close được busy do không set đúng giá trị nhận biết popup hay view
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientHiManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientHiManagementViewModel : ViewModelBase, IPatientHiManagement
        , IHandle<ItemSelected<Hospital>>
        , IHandle<ItemSelected<HIAPICheckedHICard>>
    {
        [ImportingConstructor]
        public PatientHiManagementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            authorization();
            eventArg.Subscribe(this);

            var hospitalAutoCompleteVm = Globals.GetViewModel<IHospitalAutoCompleteListing>();

            hospitalAutoCompleteVm.IsPaperReferal = false;
            HospitalAutoCompleteContent = hospitalAutoCompleteVm;
            HospitalAutoCompleteContent.DisplayHiCode = false;

            var paperReferalContentVm = Globals.GetViewModel<IPaperReferral>();
            paperReferalContentVm.IsChildWindow = IsChildWindow;

            PaperReferalContent = paperReferalContentVm;
            ((PropertyChangedBase)PaperReferalContent).PropertyChanged += PatientHiManagementViewModel_PropertyChanged;
            Operation = FormOperation.None;


            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadHiCardTypes();
                LoadProvinces();
                LoadKVCode();
            }
            //▼===== #007:  Dời xuống OnViewLoaded để xảy ra sau sự kiện ở hàm khởi tạo. 
            //              Lý do nếu để ở OnViewLoad thì sẽ control đc biến IsDiagLogView của ViewModelBase để biết xem view có phải là popup hay không và quan trọng là nó đang hoạt động tốt cho đến hiện tại.
            //if (Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion || Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
            //{
            //    LoadCrossRegionHospitals();
            //}
            //▲=====
            SelectedProvince = new CitiesProvince();
            ReceiveMethod = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ReceiveMethod).ToObservableCollection();
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
                PaperReferalContent.PatientFindBy = this.PatientFindBy;
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
                if (_QRCode != value)
                {
                    _QRCode = value;
                    if (QRCode != null && _currentView != null)
                    {
                        LoadHIFromQRCode();
                    }
                }
            }
        }
        private void LoadHIFromQRCode()
        {
            try
            {
                CreateNewHiCmd();
                EditingHiItem.HICardNo = QRCode.HICardNo;
                EditingHiItem.RegistrationCode = QRCode.RegistrationCode;
                EditingHiItem.ValidDateFrom = QRCode.ValidDateFrom;
                EditingHiItem.ValidDateTo = QRCode.ValidDateTo;
                EditingHiItem.PatientStreetAddress = QRCode.Address;
                EditingHiItem.KVCode = Convert.ToInt64(QRCode.ProvinceHICode) - 4;
                LoadHospital(EditingHiItem);
                if (QRCode.CitiesProvince != null)
                {
                    EditingHiItem.CityProvinceID_Address = QRCode.CitiesProvince.CityProvinceID;
                    EditingHiItem.CityProvinceName = QRCode.CitiesProvince.CityProvinceName;
                    SelectedProvince = QRCode.CitiesProvince;
                    PagingLinq(QRCode.CitiesProvince.CityProvinceID);
                }
                else
                {
                    EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICardNo(QRCode.HICardNo);
                    if (!string.IsNullOrEmpty(EditingHiItem.CityProvinceName))
                    {
                        ObservableCollection<CitiesProvince> FoundProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName).IndexOf(ConvertString(EditingHiItem.CityProvinceName), StringComparison.InvariantCultureIgnoreCase) >= 0));
                        if (FoundProvince != null && FoundProvince.Count == 1)
                        {
                            SelectedProvince = FoundProvince.First();
                            EditingHiItem.CityProvinceID_Address = SelectedProvince.CityProvinceID;
                            PagingLinq(SelectedProvince.CityProvinceID);
                        }
                    }

                }
                EditingHiItem.SuburbNameID = -1;
                NotifyOfPropertyChange(() => QRCode);
                NotifyOfPropertyChange(() => SelectedProvince);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //==== #001
        private bool _IsChildWindow;
        public bool IsChildWindow
        {
            get { return _IsChildWindow; }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                    PaperReferalContent.IsChildWindow = IsChildWindow;
                    NotifyOfPropertyChange(() => IsChildWindow);
                }
            }
        }

        void PatientHiManagementViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InfoHasChanged")
            {
                NotifyOfPropertyChange(() => CanCreateNewHiCmd);
                NotifyOfPropertyChange(() => CanSelectHiItem);
                NotifyOfPropertyChange(() => CanEditHiInfo);
                NotifyOfPropertyChange(() => CanBeginEditCmd);
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
            }
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

        private IPaperReferral _paperReferalContent;
        public IPaperReferral PaperReferalContent
        {
            get { return _paperReferalContent; }
            set
            {
                _paperReferalContent = value;
                NotifyOfPropertyChange(() => PaperReferalContent);
            }
        }

        private IPatientHiManagementView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // TxD 12/07/2014: Commented out the following because it's already called in Constructor
            // authorization();
            
            _currentView = view as IPatientHiManagementView;
            //==== #001
            if (QRCode != null)
            {
                LoadHIFromQRCode();
            }
            //==== #001
            //▼===== #007: Dời từ hàm khởi tạo xuống đây.
            if (Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion || Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
            {
                LoadCrossRegionHospitals();
            }
            //▲===== #007
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //Reset lại những giá trị nếu trong trường hợp PartCreation là share.
            Reset();
        }
        /// <summary>
        /// Reset lại những giá trị của form quản lý thẻ bảo hiểm.
        /// </summary>
        private void Reset()
        {
            InfoHasChanged = false;
        }
        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CanCreateNewHiCmd);
                PaperReferalContent.CurrentPatient = _currentPatient; /*tma*/
                if (_currentPatient != null)
                {
                    HealthInsurances = _currentPatient.HealthInsurances;
                    _currentPatient.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(_curPatient_PropertyChanged).Handler;
                }
                else
                {
                    HealthInsurances = null;
                }
                ResetForm();
            }
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

                    PaperReferalContent.FormLocked = _infoHasChanged;
                }
            }
        }
        private bool _isSaving;
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
                NotifyOfPropertyChange(() => CanSaveHiInfoCmd);
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => CanCreateNewHiCmd);
            }
        }

        private bool _CanEdit = true;
        public bool CanEdit
        {
            get
            {
                return _CanEdit;
            }
            set
            {
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
                PaperReferalContent.CanEdit = _CanEdit;
            }
        }

        public bool ShowPaperReferalContent
        {
            get
            {
                return _selectedHiItem != null;
            }
        }
        private HealthInsurance _selectedHiItem;
        public HealthInsurance SelectedHiItem
        {
            get
            {
                return _selectedHiItem;
            }
            set
            {
                _selectedHiItem = value;
                NotifyOfPropertyChange(() => SelectedHiItem);
                NotifyOfPropertyChange(() => ShowPaperReferalContent);
                PaperReferalContent.CurrentHiItem = ObjectCopier.DeepCopy(_selectedHiItem);

                if (HealthInsurances != null)
                {
                    foreach (var item in HealthInsurances)
                    {
                        if (item.IsChecked)
                        {
                            item.IsChecked = false;
                        }
                    }
                }
                if (_selectedHiItem != null)
                {
                    _selectedHiItem.IsChecked = true;
                    PaperReferalContent.CurrentHiItem.Patient = CurrentPatient;
                }

                if (Operation == FormOperation.Edit && EditingHiItem != null)
                {
                    EditingHiItem.CancelEdit();
                }
                EditingHiItem = ObjectCopier.DeepCopy(SelectedHiItem);
                Operation = _selectedHiItem != null ? FormOperation.ReadOnly : FormOperation.None;               
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
        private bool _includeDeletedItems;
        public bool IncludeDeletedItems
        {
            get
            {
                return _includeDeletedItems;
            }
            set
            {
                if (_includeDeletedItems != value)
                {
                    _includeDeletedItems = value;
                    GetAllHealthInsurance(_includeDeletedItems);
                    NotifyOfPropertyChange(() => IncludeDeletedItems);
                }
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

        public bool IsMarkAsDeleted
        { 
            get
            {
                if (EditingHiItem != null)
                {
                    return EditingHiItem.MarkAsDeleted;
                }
                return false;
            }
        }
        //▼====== #004
        private bool _IsCheckOnline;
        public bool IsCheckOnline
        {
            get
            {
                return _IsCheckOnline;
            }
            set
            {
                _IsCheckOnline = value;
                NotifyOfPropertyChange(() => IsCheckOnline);
            }
        }
        //▲====== #004
        public bool CheckValidationAndGetConfirmedItem(bool IsEmergency)
        {
            if (HealthInsurances == null || HealthInsurances.Count == 0)
            {
                //▼====== #004: Bổ sung thêm câu thông báo khi không có bất cứ thẻ nào mà người dùng click vào kiểm tra online :"Không có thẻ nào để kiểm tra online."
                if (!IsCheckOnline)
                {
                    ConfirmedItem = null;
                    Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1104_G1_ChuaCoTheBHXNhan) });
                    return false;
                }
                else
                {
                    IsCheckOnline = false;
                    ConfirmedItem = null;
                    Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}", eHCMSResources.Z2317_G1_KhongCoTheDeKiemTraOnline) });
                    return false;
                }
                //▲====== #004
            }

            if (CurrentPatient == null)
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.A0378_G1_Msg_InfoChuaChonBN) });
                return false;
            }

            ConfirmedItem = null;

            if (InfoHasChanged)
            {
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1106_G1_FormTheBHDaThayDoi) });
                return false;
            }

            if (PaperReferalContent.InfoHasChanged)
            {
                Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z0984_G1_TTinFormGCVThayDoi) });
                return false;
            }

            if (SelectedHiItem == null)
            {
                //▼====== #004: Nếu không chọn thẻ nào mà lại đang kiểm tra thẻ BHYT online thì câu báo lỗi liên quan đến kiểm tra online
                if (!IsCheckOnline)
                {
                    ConfirmedItem = null;
                    Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z1164_G1_ChonTheBHDeXNhan) });
                    return false;
                }
                else
                {
                    IsCheckOnline = false;
                    ConfirmedItem = null;
                    Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}.", eHCMSResources.Z2302_G1_ChonTheKiemTraOnline) });
                    return false;
                }
                //▲====== #004
            }

            //▼====: #003
            // Hàm CheckValidationAndGetConfirmedItem sử dụng chung cho cả nội và ngoại trú là bước chốt chặn cuối cùng trước khi đăng ký ngoai tru hoac xác nhận BHYT cho bệnh nhân noi tru.
            // Hiện tại đã thay đổi chỉ khi nào bệnh nhân đăng ký ngoại trú mới đi kiểm tra ngày đến thẻ có < ngày hiện tại không. Có thì báo lỗi và return, không thì cho đăng ký.            
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {                
                if (!(eHCMS.Services.Core.AxHelper.CompareDate(SelectedHiItem.ValidDateTo.Value.Date, Globals.GetCurServerDateTime().Date) <= 1))
                {
                    //HPT_20160610: Vv trẻ em dưới 6 tuổi : TxD 26/05/2018: Cho nay can duoc sua lai vi dieu kien ben duoi khong thay kiem tra 
                    //                                      lien quan den 30/09
                    if (Globals.ServerConfigSection.InRegisElements.AllowChildUnder6YearsOldUseHIOverDate && Globals.CanRegHIChildUnder6YearsOld(CurrentPatient.Age.GetValueOrDefault()))
                    {
                        //HPT: Nếu là trẻ em dưới 6 tuổi, được cấu hình cho phép và thời điểm hiện tại là trước ngày 30/9 thì không cần kiểm tra thẻ hết hạn
                        //Chỗ này không làm gì, sau này nếu cần thiết thêm code cũng không làm thay đổi logic (Anh Tuấn đã review: 10/06/2016)
                    }

                    else
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A1013_G1_Msg_InfoTheBHHetHan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }                    
                }
            }
            //▲====: #003

            if (!SelectedHiItem.IsActive)
            {
                ConfirmedItem = null;
                Globals.EventAggregator.Publish(new ErrorNotification { Message = eHCMSResources.Z1107_G1_TheBHChuaKichHoat });
                return false;
            }
            //KMx: Kiểm tra các thuộc tính trong thẻ BH có bị null hay không ([Required] của property) (24/02/2014 09:54).
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;

            bool isValid = ValidateHealthInsurance(SelectedHiItem, out validationResults);
            if (!isValid)
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);

                return false;
            }

            //KMx: Kiểm tra Mã thẻ, From Date, To Date có hợp lệ hay không (khi 3 thuộc tính đó != null) (24/02/2014 09:54).
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validResults;
            if (!SelectedHiItem.ValidateAllFields(out validResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return false;
            }
            //HPT_20160611: Đã kiểm tra ngày tháng ở trên rồi. Comment lại đoạn này, nếu chạy ổn định thì có thể bỏ đi. 
            //DateTime today = Globals.ServerDate.Value.Date;
            //if (!SelectedHiItem.ValidDateFrom.HasValue
            //    || !SelectedHiItem.ValidDateTo.HasValue
            //    || SelectedHiItem.ValidDateFrom.Value.Date > today
            //    || today > SelectedHiItem.ValidDateTo.Value.Date)
            //{
            //    isValid = false;
            //}

            //Kiem tra giay chuyen vien:

            //▼====== #004: Trùm việc kiểm tra Giấy chuyển viện lại 
            //              => Nếu là đang kiểm tra online thì không kiểm tra GCV có hay không.
            //                 Ngược lại thì mọi chuyện vẫn như cũ                                                                            
            if (!IsCheckOnline)
            //▲====== #004
            {
                if (PaperReferalContent.PaperReferalInUse != null && PaperReferalContent.PaperReferalInUse.IsChecked && !IsEmergency) //Co chon xac nhan giay chuyen vien
                {
                    if (!PaperReferalContent.PaperReferalInUse.IsActive)
                    {
                        //▼====== #004
                        IsCheckOnline = false;
                        //▲====== #004
                        Globals.EventAggregator.Publish(new ErrorNotification { Message = eHCMSResources.Z0985_G1_GCVChuaDuocKichHoat });
                        return false;
                    }

                    var refCreatedDate = PaperReferalContent.PaperReferalInUse.RefCreatedDate.Value;
                    var numDays = Globals.ServerConfigSection.HealthInsurances.PaperReferalMaxDays;

                    if (refCreatedDate.Year < Globals.ServerDate.Value.Year)
                    {
                        var msg = string.Format(eHCMSResources.Z0986_G1_GCVDuocKyNg0DaHetHieuLuc, refCreatedDate.ToString("dd/MM/yyyy"));
                        var result = MessageBox.Show(msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.Cancel)
                        {
                            //▼====== #004
                            IsCheckOnline = false;
                            //▲====== #004
                            return false;
                        }
                    }

                    if (!PaperReferalContent.PaperReferalInUse.IsChronicDisease
                        && refCreatedDate.AddDays(numDays).Date < Globals.ServerDate.Value.Date)
                    {
                        var msg = string.Format(eHCMSResources.Z0987_G1_GCVDaQuaThoiHanKeTuNgKy, refCreatedDate.ToString("dd/MM/yyyy"), numDays);
                        var result = MessageBox.Show(msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.Cancel)
                        {
                            //▼====== #004
                            IsCheckOnline = false;
                            //▲====== #004
                            return false;
                        }
                    }
                }
            }
            //▼====== #004
            IsCheckOnline = false;
            //▲====== #004
            ConfirmedItem = SelectedHiItem;
            return true;
        }

        public void _curPatient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HealthInsurances")
            {
                HealthInsurances = new BlankRowCollection<HealthInsurance>(_currentPatient.HealthInsurances);
            }
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
                                    if (HiCardTypes == null)
                                    {
                                        HiCardTypes = new ObservableCollection<Lookup>();
                                    }
                                    //20200428 TBL: Anh Tuấn nói chỉ sử dụng mẫu 2015
                                    HiCardTypes.Add(allItems[3]);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                                //HiCardTypes = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
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

        //public void ComboboxHiTypesLoaded(object sender)
        //{
        //    var cbo = sender as AxComboBox;
        //    if (cbo != null)
        //    {
        //        //Cai nay chua chinh xac.
        //        cbo.ItemsSource = HiCardTypes;
        //    }
        //}

        private ObservableCollection<HealthInsurance> _healthInsurances;
        public ObservableCollection<HealthInsurance> HealthInsurances
        {
            get
            {
                return _healthInsurances;
            }
            set
            {
                _healthInsurances = value;
                NotifyOfPropertyChange(() => HealthInsurances);               
            }
        }
        public void GetAllHealthInsurance(bool IncludeDeletedItems = false, bool initEditingItem = true, bool GetFirst = false)
        {
            if (_currentPatient == null)
            {
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {                
                AxErrorEventArgs error = null;
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllHealthInsurances(_currentPatient.PatientID, IncludeDeletedItems,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllHealthInsurances(asyncResult);
                                    HealthInsurances = new ObservableCollection<HealthInsurance>(allItems);
                                    NotifyOfPropertyChange(() => HealthInsurances);
                                    PermissionManager.ApplyPermissionToHealthInsuranceList(HealthInsurances);
                                    InfoHasChanged = false;
                                    if (GetFirst)
                                    {
                                        SelectedHiItem = HealthInsurances.FirstOrDefault();
                                        Globals.EventAggregator.Publish(new SaveHIAndConfirm());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    IsLoading = false;
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                    this.HideBusyIndicator();
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                finally
                {
                    IsLoading = false;
                    if (initEditingItem)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            //HospitalAutoCompleteContent.setDisplayText("");
                            HospitalAutoCompleteContent.InitBlankBindingValue();
                        });
                    }
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        public void MarkHIItemAsDeletedCmd(object sender, object eventArgs)
        {
            var elem = sender as FrameworkElement;
            if (elem == null)
            {
                return;
            }

            var hiItem = elem.DataContext as HealthInsurance;

            if (hiItem != null)
            {
                hiItem.EntityState = EntityState.DELETED_MODIFIED;
                hiItem.MarkAsDeleted = true;
                InfoHasChanged = true;
            }

        }
        public void MarkHIItemAsUnDeletedCmd(object sender, object eventArgs)
        {
            var elem = sender as FrameworkElement;
            if (elem == null)
            {
                return;
            }

            var hiItem = elem.DataContext as HealthInsurance;

            if (hiItem != null)
            {
                hiItem.EntityState = EntityState.MODIFIED;
                hiItem.MarkAsDeleted = false;
                InfoHasChanged = true;
            }
        }
        public void ActivateHiItemCmd(object sender, object eventArgs)
        {
            var elem = sender as RadioButton;
            if (elem == null)
            {
                return;
            }

            var hiItem = elem.DataContext as HealthInsurance;

            if (hiItem != null)
            {
                EntityState state = hiItem.EntityState;
                foreach (var item in HealthInsurances)
                {
                    item.IsActive = false;
                }
                if (state != EntityState.NEW)
                {
                    hiItem.IsActive = elem.IsChecked.GetValueOrDefault(false);
                    hiItem.EntityState = EntityState.MODIFIED;
                    InfoHasChanged = true;
                }
                else
                {
                    hiItem.SetIsActive(elem.IsChecked.GetValueOrDefault(false));
                    InfoHasChanged = true;
                }
            }
            //if (hiItem != null)
            //{
            //    hiItem.EntityState = EntityState.MODIFIED;
            //    hiItem.MarkAsDeleted = false;
            //    InfoHasChanged = true;
            //}
        }

        public void CellEditEnded(object source, EventArgs eventArgs)
        {
            InfoHasChanged = true;
        }

        private bool _confirmHealthInsuranceSelected = true;
        public bool ConfirmHealthInsuranceSelected
        {
            get { return _confirmHealthInsuranceSelected; }
            set
            {
                _confirmHealthInsuranceSelected = value;
                NotifyOfPropertyChange(() => ConfirmHealthInsuranceSelected);
            }
        }

        ////////////////////////////////////////

        public bool ShowCreateNewHiCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.None:
                        return mNhanBenhBH_TheBH_ThemMoi;
                    case FormOperation.AddNew:
                        return false;
                    case FormOperation.Edit:
                        return false;
                    case FormOperation.ReadOnly:
                        return mNhanBenhBH_TheBH_ThemMoi;
                }
                return false;
            }
        }
        public bool CanCreateNewHiCmd
        {
            get
            {
                return !IsLoading && CurrentPatient != null && !PaperReferalContent.InfoHasChanged;
            }
        }
        /// <summary>
        /// Tạo mới một thẻ bảo hiểm chuẩn bị đưa vô database.
        /// </summary>
        public void CreateNewHiCmd()
        {
            if (_currentPatient != null)
            {
                var newItem = new HealthInsurance { PatientID = _currentPatient.PatientID, IsActive = true };

                if (HiCardTypes != null && HiCardTypes.Count > 0)
                {
                    if (newItem.HICardType == null || newItem.HICardType.LookupID <= 0)
                    {
                        if (newItem.HICardType == null)
                            newItem.HICardType = new Lookup();

                        // TxD 03/01/2015: Set default CradType to the newly created 2015 to apply the new 2015 rules
                        //if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101)
                        //{
                        //    newItem.HICardType = HiCardTypes[3];
                        //}
                        //else
                        //{
                        //    newItem.HICardType = HiCardTypes[1];
                        //}
                        //20200428 TBL: Anh Tuấn nói chỉ sử dụng mẫu 2015, nên khi tạo mới thẻ tự động set cho combobox mẫu 2015
                        newItem.HICardType = HiCardTypes[0];
                    }
                }
                
                EditingHiItem = ObjectCopier.DeepCopy(newItem);

                //HospitalAutoCompleteContent.setDisplayText("");
                HospitalAutoCompleteContent.InitBlankBindingValue();
                
                Operation = FormOperation.AddNew;
                CanEditAddressForHICard = true;
            }
            else
            {
                ShowMessageBox(string.Format("{0}. ", eHCMSResources.Z1276_G1_KgTheThemMoiTheBH));
            }
        }

        public bool ShowBeginEditCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.None:
                        return false;
                    case FormOperation.AddNew:
                        return false;
                    case FormOperation.Edit:
                        return false;
                    case FormOperation.ReadOnly:
                        return mNhanBenhBH_TheBH_Sua;
                }
                return false;
            }
        }

        public bool ShowBeginEditAfterRegistrationCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.None:
                        return false;
                    case FormOperation.AddNew:
                        return false;
                    case FormOperation.Edit:
                        return false;
                    case FormOperation.ReadOnly:
                        return mNhanBenhBH_TheBH_SuaSauKhiDangKy;
                }
                return false;
            }
        }

        public bool CanBeginEditCmd
        {
            get
            {
                return !PaperReferalContent.InfoHasChanged;
            }
        }

        private bool _CanEditAddressForHICard;
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

        public void BeginEditCmd()
        {
            if (_editingHiItem == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0362_G1_Msg_InfoChonTheBHYT));
                return;
            }

            if (EditingHiItem.MarkAsDeleted)
            {
                MessageBox.Show(eHCMSResources.A0706_G1_Msg_InfoKhTheSuaTheBHDaXoa);
                return;
            }
            //▼====: #003
            //Kiểm tra xem thẻ đang sử dụng ngày đến có < ngày hiện tại không.
            //Trước đây: Nếu thẻ đang sử dụng thì sẽ kiểm tra xem ngày đến thẻ có < ngày hiện tại không, nếu < thì không cho sửa và hiện thông báo thẻ hết hạn trước khi hiện thông báo thẻ đang sử dụng chỉ đc sửa các trường cho phép.
            //Hiện tại: Đã có kiểm tra khi xác nhận (nội trú) và đăng ký (ngoại trú) điều kiện này dư thừa nên comment lại (anh Tuấn cho ý kiến để chỉnh sửa).
            //Hàm này: Được gọi khi người sử dụng click vào nút sửa.
            //
            if (_editingHiItem.Used)
            {
                //{
                //    if (Globals.ServerDate.HasValue && _editingHiItem.ValidDateTo.HasValue)
                //    {
                //        //Comment lại xem thử còn chặn không cho xác nhận thẻ quá khứ không
                //        if (_editingHiItem.ValidDateTo.Value.Date < Globals.ServerDate.Value.Date)
                //        {
                //            MessageBox.Show(string.Format("{0}.", eHCMSResources.A1013_G1_Msg_InfoTheBHHetHan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //            return;
                //        }
                //    }
                //}
             //▲====: #003
                  MessageBox.Show(eHCMSResources.Z1036_G1_TheBHYTDaSD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            } 

            CanEditAddressForHICard = true;

            Operation = FormOperation.Edit;
            if (_editingHiItem.HIID > 0)
            {
                bSelectProvinceInit = true;
            }
            _editingHiItem.BeginEdit();
            //20200428 TBL: Anh Tuấn nói chỉ sử dụng mẫu 2015, nên hiệu chỉnh những thẻ BHYT có mẫu khác thì tự động set về mẫu 2015
            _editingHiItem.HICardType = HiCardTypes[0];
            // TxD 16/03/2014: COMMENTED the following to prevent a Populating
            // AND replaced it with calling the new method.
            //HospitalAutoCompleteContent.setDisplayText(_editingHiItem.RegistrationLocation, true);
            Hospital hosInfo = new Hospital();
            hosInfo.HosName = EditingHiItem.RegistrationLocation;
            hosInfo.HICode = EditingHiItem.RegistrationCode;
            hosInfo.HosID = EditingHiItem.HosID.HasValue ? EditingHiItem.HosID.Value : 0;

            // TxD 16/03/2014: The following is a Work Around to actually prevent a Populating.
            HospitalAutoCompleteContent.InitBlankBindingValue();
            
            HospitalAutoCompleteContent.SetSelHospital(hosInfo);
                
            //HospitalAutoCompleteContent.setDisplayText(_editingHiItem.RegistrationLocation, _editingHiItem.hosHospital, true);
            //HospitalAutoCompleteContent.ResetPrevent();
            
        }

        //KMx: Chỉnh sửa tất cả thông tin trên thẻ bảo hiểm. Mặc dù thẻ đã sử dụng rồi.
        public void BeginEditAfterRegistrationCmd()
        {
            if (_editingHiItem == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0362_G1_Msg_InfoChonTheBHYT));
                return;
            }

            if (_editingHiItem.Used)
            {
                if (Globals.ServerDate.HasValue && _editingHiItem.ValidDateTo.HasValue)
                {
                    if (_editingHiItem.ValidDateTo.Value.Date < Globals.ServerDate.Value.Date)
                    {
                        if (MessageBox.Show(eHCMSResources.Z1037_G1_TheSDDaHetHanCoMuonSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            return;
                    }
                }
                if (MessageBox.Show(eHCMSResources.Z0981_G1_TheDangSDCoMuonSuaKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    return;
            }
            CanEditAddressForHICard = true;
            IsEditAfterRegistration = true;
            //KMx: Khi chọn nút "Sửa sau khi ĐK" thì set CanEditAfterRegistration = true để Enable các textbox cho người dùng chỉnh sửa. Sau đó set lại bằng False.
            CanEditAfterRegistration = true;
            Operation = FormOperation.Edit;
            CanEditAfterRegistration = false;

            _editingHiItem.BeginEdit();
            //20200428 TBL: Anh Tuấn nói chỉ sử dụng mẫu 2015, nên hiệu chỉnh những thẻ BHYT có mẫu khác thì tự động set về mẫu 2015
            _editingHiItem.HICardType = HiCardTypes[0];
            Hospital hosInfo = new Hospital();
            hosInfo.HosName = EditingHiItem.RegistrationLocation;
            hosInfo.HICode = EditingHiItem.RegistrationCode;
            hosInfo.HosID = EditingHiItem.HosID.HasValue ? EditingHiItem.HosID.Value : 0;

            //HospitalAutoCompleteContent.setDisplayText(_editingHiItem.RegistrationLocation, true);
            
            HospitalAutoCompleteContent.InitBlankBindingValue();
            HospitalAutoCompleteContent.SetSelHospital(hosInfo);

            //HospitalAutoCompleteContent.setDisplayText(_editingHiItem.RegistrationLocation, _editingHiItem.hosHospital, true);
            //HospitalAutoCompleteContent.ResetPrevent();
            
        }

        public bool CanSelectHiItem
        {
            get
            {
                return (Operation == FormOperation.None || Operation == FormOperation.ReadOnly) && !PaperReferalContent.InfoHasChanged;
            }
        }

        public bool ShowCancelEditCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.None:
                        return false;
                    case FormOperation.AddNew:
                        return true;
                    case FormOperation.Edit:
                        return true;
                    case FormOperation.ReadOnly:
                        return false;
                }
                return false;
            }
        }
        public bool CanCancelEditCmd
        {
            get
            {
                return true;
            }
        }
        public void CancelEditCmd()  
        {
            CanEditAddressForHICard = false;
            if (_editingHiItem != null)
            {
                bSelectProvinceInit = false;
                _editingHiItem.CancelEdit();
                if (SelectedHiItem != null && SelectedHiItem.CityProvinceID_Address > 0)
                {
                    SelectedProvince = Provinces.Where(x => x.CityProvinceID == SelectedHiItem.CityProvinceID_Address).FirstOrDefault();
                    PagingLinq((long)SelectedHiItem.CityProvinceID_Address);
                }
                else
                {
                    SelectedProvince = new CitiesProvince();
                    PagingLinq(0);
                }
                SetKVCode();
                if (EditingHiItem.HosID.HasValue && EditingHiItem.HosID > 0 && EditingHiItem.RegistrationLocation.Length > 1)
                {
                    Hospital hosInfo = new Hospital();
                    hosInfo.HosName = EditingHiItem.RegistrationLocation;
                    hosInfo.HICode = EditingHiItem.RegistrationCode;
                    hosInfo.HosID = EditingHiItem.HosID.Value;
                    HospitalAutoCompleteContent.SetSelHospital(hosInfo);
                }

                if (_editingHiItem.HIID > 0)
                {
                    Operation = FormOperation.ReadOnly;
                }
                else
                {
                    if (SelectedHiItem != null)
                    {
                        EditingHiItem = SelectedHiItem;
                        Operation = FormOperation.ReadOnly;
                    }
                    else
                    {
                        EditingHiItem = null;
                        Operation = FormOperation.None;
                    }
                }
                NotifyOfPropertyChange(() => EditingHiItem);
            }
            else
            {
                Operation = FormOperation.None;
            }

            if (IsEditAfterRegistration)
            {
                IsEditAfterRegistration = false;
            }

        }
        public bool ShowSaveHiInfoCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.None:
                        return false;
                    case FormOperation.AddNew:
                        return mNhanBenhBH_TheBH_Luu;
                    case FormOperation.Edit:
                        return mNhanBenhBH_TheBH_Luu;
                    case FormOperation.ReadOnly:
                        return false;
                }
                return false;
            }
        }
        private bool _ShowSaveHIAndConfirmCmd;
        public bool ShowSaveHIAndConfirmCmd
        {
            get { return _ShowSaveHIAndConfirmCmd && ShowSaveHiInfoCmd; }
            set
            {
                if (_ShowSaveHIAndConfirmCmd != value)
                {
                    _ShowSaveHIAndConfirmCmd = value;
                    NotifyOfPropertyChange(() => ShowSaveHIAndConfirmCmd);
                }
            }
        }
        public bool CanSaveHiInfoCmd
        {
            get
            {
                return !IsSaving && EditingHiItem != null;
            }
        }
        public void SaveHIAndConfirmCmd()
        {
            SaveHiInfoCmd(true);
        }
        public void SaveHiInfoCmd(bool IsSaveHIAndConfirm = false)
        {
            if (IsSaving)
            {
                return;
            }

            if (_editingHiItem == null)
            {
                MessageBox.Show(eHCMSResources.A0699_G1_Msg_InfoKhTheLuuTheBH);
                return;
            }

            if (EditingHiItem.MarkAsDeleted)
            {
                MessageBox.Show(eHCMSResources.A0700_G1_Msg_InfoKhTheLuuTheBHDaXoa);
                return;
            }

            this.ShowBusyIndicator();
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            
            //KMx: Kiểm tra các thuộc tính trong thẻ BH có bị null hay không ([Required] của property) (24/02/2014 09:54).
            if(!ValidateHealthInsurance(_editingHiItem, out validationResults))
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
            bool bCardValid = false;
            if (Globals.ServerConfigSection.HealthInsurances.ApplyHINewRule20150101)
            {
                bCardValid = _editingHiItem.ValidateAllFields_New_2015(out validationResults
                    , Globals.ServerConfigSection.CommonItems.ValidHIPattern
                    , Globals.ServerConfigSection.CommonItems.InsuranceBenefitCategories);
            }
            else
            {
                bCardValid = _editingHiItem.ValidateAllFields(out validationResults);
            }

            //KMx: Kiểm tra Mã thẻ, From Date, To Date có hợp lệ hay không (khi 3 thuộc tính đó != null) (24/02/2014 09:54).
            if (!bCardValid)
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                this.HideBusyIndicator();
                return;
            }
            

            if (HospitalAutoCompleteContent.SelectedHospital == null || HospitalAutoCompleteContent.SelectedHospital.HosID <= 0 ||
                string.IsNullOrEmpty(HospitalAutoCompleteContent.SelectedHospital.HosName) || string.IsNullOrEmpty(HospitalAutoCompleteContent.SelectedHospital.HICode))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0345_G1_Msg_InfoChonKCBBDKhHopLe));
                this.HideBusyIndicator();
                return;
            }

            EditingHiItem.RegistrationLocation = HospitalAutoCompleteContent.SelectedHospital.HosName;
            EditingHiItem.RegistrationCode = HospitalAutoCompleteContent.SelectedHospital.HICode;
            //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
            //EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICode(HospitalAutoCompleteContent.SelectedHospital.HICode);
            EditingHiItem.HosID = HospitalAutoCompleteContent.SelectedHospital.HosID;
            //▼====== #006
            if (cboHIWardName.SelectedIndex < 0)
            {
                EditingHiItem.WardNameID = 0;
                EditingHiItem.WardName = "";
            }
            else
            {
                EditingHiItem.WardNameID = ListWardNames[cboHIWardName.SelectedIndex].WardNameID;
                EditingHiItem.WardName = ListWardNames[cboHIWardName.SelectedIndex].WardName;
            }
            //▲====== #006
            //KMx: Nếu nơi KCB-BĐ không có tỉnh thành thì không cho lưu.
            if (string.IsNullOrEmpty(_editingHiItem.CityProvinceName))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A1015_G1_Msg_InfoTheBHKhCoT_TP));
                this.HideBusyIndicator();
                return;
            }
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
            GetKVCode();
            if (_editingHiItem.HIID > 0)
            {
                if (IsEditAfterRegistration)
                {
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
                }
                UpdateHiItem(IsSaveHIAndConfirm, Reason);
            }
            else
            {
                //▼====: #003      
                //Bấm lưu thì: Hàm SaveHiInfocmd được gọi vì cả update và thêm thẻ mới đều cần phải bấm lưu để hoàn thành công việc nên:
                //- Nếu như là update thì sẽ gọi hàm UpdateHiItem() để thực hiện việc update và lưu vào database thông qua hàm UpdateHiItem() bên service gọi store (spHealthInsurance_Update) để update
                //- Nếu như là lưu thẻ mới thì: sẽ gọi hàm Do_AddHiItem_Routine() hàm này sẽ tiếp tục gọi AddHiItem_Action() và hàm này sẽ gọi hàm AddHiItem() bên service để add vào database thông qua store (spHealthInsurance_Add)
                //Ban đầu hàm này dùng để kiểm tra thẻ lưu vào (nội/ngoại trú) có còn hạn sử dụng không 
                //Sau này không còn cần thiết vì đã đổi lại là có thể nhập thẻ quá khứ ... nên cần comment lại.

                //if (!(AxHelper.CompareDate(EditingHiItem.ValidDateTo.GetValueOrDefault().Date, Globals.ServerDate.Value.Date) <= 1))
                //{
                //    MessageBox.Show(string.Format("{0}!", eHCMSResources.A1013_G1_Msg_InfoTheBHHetHan));
                //    this.HideBusyIndicator();
                //    return;
                //}
                //▲====: #003

                //AddHiItem();
                Coroutine.BeginExecute(Do_AddHiItem_Routine(IsSaveHIAndConfirm));
            }

            HospitalAutoCompleteContent.ResetSelectedHospitalInit();
            CanEditAddressForHICard = false;
            this.HideBusyIndicator();
        }

        private bool bAddHiItemAction_FoundDuplicateHiCardNum = false;
        string AddHiItem_ErrorMsg = "";
        MessageWarningShowDialogTask warnDlgTask = null;
        private IEnumerator<IResult> Do_AddHiItem_Routine(bool IsSaveHIAndConfirm)
        {
            bAddHiItemAction_FoundDuplicateHiCardNum = false;
            yield return GenericCoRoutineTask.StartTask(AddHiItem_Action, IsSaveHIAndConfirm);
            if (bAddHiItemAction_FoundDuplicateHiCardNum)
            {
                warnDlgTask = new MessageWarningShowDialogTask(AddHiItem_ErrorMsg, "Tiếp Tục Lưu Thẻ BHYT.");
                yield return warnDlgTask;
                if (warnDlgTask.IsAccept)
                {                                        
                    EditingHiItem.CofirmDuplicate = true;
                    yield return GenericCoRoutineTask.StartTask(AddHiItem_Action, IsSaveHIAndConfirm);                    
                }                
            }
            yield break;
        }


        public void AddHiItem_Action(GenericCoRoutineTask genTask, object IsSaveHIAndConfirm)
        {
            AxErrorEventArgs outError = null;
            AxErrorEventArgs inError = null;
            //▼====: #002
            if (EditingHiItem.ValidDateTo != null)
            {
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.Date;
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.AddDays(1);
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.AddSeconds(-1);
            }
            //▲====: #002
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddHiItem(_editingHiItem, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                        Globals.DispatchCallback(asyncResult =>
                        {
                            var userState = asyncResult.AsyncState as HealthInsurance;
                            bool bOK;
                            long hiId = -1;
                            try
                            {
                                contract.EndAddHiItem(out hiId, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                bOK = false; 
                                inError = new AxErrorEventArgs(fault);
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show(inError.ServerError.Message);
                                });

                                bAddHiItemAction_FoundDuplicateHiCardNum = true;
                                AddHiItem_ErrorMsg = inError.ServerError.Message;

                            }
                            catch (Exception ex)
                            {
                                bOK = false;
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show(ex.Message);
                                });

                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            if (bOK)
                            {
                                if (hiId > 0)
                                {
                                    if (userState != null && userState.HIID <= 0)
                                    {
                                        userState.HIID = hiId;
                                    }
                                    Operation = FormOperation.None;
                                    GetAllHealthInsurance(IncludeDeletedItems, true, (bool)IsSaveHIAndConfirm);
                                    EditingHiItem = null;
                                    if (AcbHICity != null)
                                    {
                                        AcbHICity.SelectedItem = new CitiesProvince();
                                    }

                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A1016_G1_Msg_InfoTheBHDaTonTai));
                                }
                            }

                            genTask.ActionComplete(true);

                        }), _editingHiItem);
                    }
                }
                // TxD 28/12/2014: The Service end has been modified to ONLY throw FaultException<AxException>
                //                  when it found a duplicate HiCardNo
                catch (FaultException<AxException> exFault) 
                {
                    outError = new AxErrorEventArgs(exFault);
                    bAddHiItemAction_FoundDuplicateHiCardNum = true;
                    this.HideBusyIndicator();
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                    this.HideBusyIndicator();
                    genTask.ActionComplete(true);
                }

                if (outError != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(outError.ToString());
                    });


                    bAddHiItemAction_FoundDuplicateHiCardNum = true;
                    AddHiItem_ErrorMsg = outError.ServerError.Message;

                    genTask.ActionComplete(true);
                }
            });
            t.Start();

        }



        public void UpdateHiItem(bool IsSaveHIAndConfirm = false, string Reason = null)
        {
            //▼====: #002*
            if (EditingHiItem.ValidDateTo != null)
            {
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.Date;
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.AddDays(1);
                EditingHiItem.ValidDateTo = EditingHiItem.ValidDateTo.Value.AddSeconds(-1);
            }
            //▲====: #002*
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsSaving = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1108_G1_DangLuuTTinTheBH)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateHiItem(_editingHiItem, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), IsEditAfterRegistration, Reason,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                bool bOK;
                                string result = "";
                                try
                                {
                                    bOK = contract.EndUpdateHiItem(out result, asyncResult);
                                    //bOK = true;  
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                }
                                if (bOK)
                                {
                                    GetAllHealthInsurance(IncludeDeletedItems,true, IsSaveHIAndConfirm);
                                    EditingHiItem = SelectedHiItem;
                                    Operation = FormOperation.None;
                                    //MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                    if (AcbHICity != null)
                                    {
                                        AcbHICity.SelectedItem = new CitiesProvince();
                                    }
                                }
                                else
                                {
                                    if (result == "ErrValidate")
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A1052_G1_Msg_InfoTTinTheBHKhHopLe));
                                    }
                                    else if (result == "exist")
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A1016_G1_Msg_InfoTheBHDaTonTai));
                                    }
                                    else
                                    {
                                        if (error != null && error.ServerError != null)
                                        {
                                            MessageBox.Show(error.ServerError.Message);
                                        }
                                        else
                                        {
                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail));
                                        }
                                    }
                                }
                                IsEditAfterRegistration = false;
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);

                }
                finally
                {
                    IsSaving = false;
                    Globals.IsBusy = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    //Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });

                }
            });
            t.Start();
        }


        public void ResetForm()
        {
            EditingHiItem = null;
            SelectedHiItem = null;
            Operation = FormOperation.None;
        }
        MessageBoxTask _msgTask;
        private void ShowMessageBox(string msg)
        {
            _msgTask = new MessageBoxTask(msg, eHCMSResources.G0442_G1_TBao);
            var lst = new List<IResult> { _msgTask };
            Coroutine.BeginExecute(lst.GetEnumerator());
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
                NotifyOfPropertyChange(() => ShowCreateNewHiCmd);
                NotifyOfPropertyChange(() => ShowBeginEditCmd);
                NotifyOfPropertyChange(() => ShowBeginEditAfterRegistrationCmd);
                NotifyOfPropertyChange(() => ShowCancelEditCmd);
                NotifyOfPropertyChange(() => ShowSaveHiInfoCmd);
                NotifyOfPropertyChange(() => IsEditMode);
                NotifyOfPropertyChange(() => CanSelectHiItem);
                NotifyOfPropertyChange(() => CanEditHiInfo);
                NotifyOfPropertyChange(() => ShowSaveHIAndConfirmCmd);
            }
        }

        public bool IsEditMode
        {
            get
            {
                switch (_operation)
                {
                    case FormOperation.AddNew:
                        return true;
                    case FormOperation.Edit:
                        return true;
                }
                return false;
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
                if (EditingHiItem != null && EditingHiItem.CityProvinceID_Address > 0)
                {
                    SelectedProvince = Provinces.Where(x => x.CityProvinceID == EditingHiItem.CityProvinceID_Address).FirstOrDefault();
                    PagingLinq((long)SelectedProvince.CityProvinceID);
                }
                else
                {
                    SelectedProvince = new CitiesProvince();
                    PagingLinq(0);
                }
                SetKVCode();
                NotifyOfPropertyChange(() => EditingHiItem);
                NotifyOfPropertyChange(() => CanSaveHiInfoCmd);
                NotifyOfPropertyChange(() => CanCancelEditCmd);
                NotifyOfPropertyChange(() => CanEditHiInfo);
            }
        }

        //KMx: IsEditAfterRegistration dùng để truyền xuống stored để biết là sửa trước khi đăng ký BH hay sau đăng ký.
        private bool _isEditAfterRegistration = false;

        public bool IsEditAfterRegistration
        {
            get { return _isEditAfterRegistration; }
            set { _isEditAfterRegistration = value; }
        }


        private bool _canEditAfterRegistration;

        public bool CanEditAfterRegistration
        {
            get { return _canEditAfterRegistration; }
            set { _canEditAfterRegistration = value; }
        }

        public bool CanEditHiInfo
        {
            get
            {
                //return (_operation == FormOperation.Edit || _operation == FormOperation.AddNew)
                //    && _editingHiItem != null && !_editingHiItem.Used
                //    && !PaperReferalContent.InfoHasChanged;
                //KMx: Nếu thẻ BH đã được sử dụng, thì không được chỉnh sửa. Nhưng nếu chọn nút "Sửa sau khi đăng ký" thì được quyền sửa. Modified Date: 20/02/2014 15:14.
                return (_operation == FormOperation.Edit || _operation == FormOperation.AddNew)
                    && _editingHiItem != null && (!_editingHiItem.Used || CanEditAfterRegistration)
                    && !PaperReferalContent.InfoHasChanged;
            }
        }


        private bool ValidateHealthInsurance(HealthInsurance hiItem, out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            bool bValid = ValidationExtensions.ValidateObject(hiItem, out validationResults);

            return bValid;
        }

        public void CancelEditing()
        {
            if (IsEditMode)
            {
                if (_editingHiItem != null)
                {
                    CancelEditCmd();
                }
            }
            InfoHasChanged = false;
            PaperReferalContent.CancelEditing();
        }


        public void Handle(ItemSelected<Hospital> message)
        {
            if (GetView() != null && message != null
                && message.Source == HospitalAutoCompleteContent)
            {
                if (EditingHiItem != null && IsEditMode)
                {
                    if (message.Item != null)
                    {
                        EditingHiItem.RegistrationLocation = message.Item.HosName;
                        EditingHiItem.RegistrationCode = message.Item.HICode;
                        //EditingHiItem.CityProvinceName = message.Item.CityProvinceName;
                        //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                        //EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICode(message.Item.HICode);
                    }
                    else
                    {
                        //EditingHiItem.RegistrationLocation = HospitalAutoCompleteContent.AcDisplayText;
                        //EditingHiItem.RegistrationCode = string.Empty;
                    }
                }
            }
        }


        public void txtHICardNo_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            if (EditingHiItem == null || IsEditMode == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(sender.Text))
            {
                return;
            }

            //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH (11/11/2014 11:33).
            EditingHiItem.CityProvinceName = Globals.GetDistrictNameFromHICardNo(sender.Text);
            // Nếu là thẻ cũ đã sử dụng thì không tự động
            if (EditingHiItem.HIID != 0 && EditingHiItem.Used)
            {
                return;
            }
            else if (Globals.ServerConfigSection.CommonItems.AutoGetHICardDataFromHIPortal)
            {
                try
                {
                    if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token)))
                    {
                        GlobalsNAV.LoginHIAPI();
                    }
                    if (GlobalsNAV.gLoggedHIAPIUser != null && GlobalsNAV.gLoggedHIAPIUser.maKetQua != 200)
                        return;
                    GetHICardInfo();
                    if (HIAPICheckedHICard.maKetQua == 401)
                    {
                        GlobalsNAV.LoginHIAPI();
                        GetHICardInfo();
                    }
                }
                catch (Exception ex)
                {
                    GlobalsNAV.ShowMessagePopup(ex.Message);
                }
            }
        }

        #region Properties
        private string HIAPICheckHICardAddress = "http://egw.baohiemxahoi.gov.vn/api/egw/KQNhanLichSuKCB2019?token={0}&id_token={1}&username={2}&password={3}";
        private HealthInsurance _gHealthInsurance;
        private HIAPICheckedHICard _HIAPICheckedHICard;
        public HealthInsurance gHealthInsurance
        {
            get => _gHealthInsurance; set
            {
                _gHealthInsurance = value;
                NotifyOfPropertyChange(() => gHealthInsurance);
            }
        }
        public HIAPICheckedHICard HIAPICheckedHICard
        {
            get => _HIAPICheckedHICard; set
            {
                _HIAPICheckedHICard = value;
                NotifyOfPropertyChange(() => HIAPICheckedHICard);
            }
        }
        #endregion

        private void GetHICardInfo()
        {
            string mHIAPICheckHICardAddress = string.Format(HIAPICheckHICardAddress
                , GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token
                , GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount
                , GlobalsNAV.gLoggedHIAPIUser.password);
            string mHIData = string.Format("{{\"maThe\":\"{0}\",\"hoTen\":\"{1}\",\"ngaySinh\":\"{2}\"}}"
                , EditingHiItem.HICardNo.ToUpper()
                , CurrentPatient.FullName.ToUpper()
                , CurrentPatient.DOB.HasValue && CurrentPatient.DOB != null 
                && CurrentPatient.DOB.Value.Day == 1 
                && CurrentPatient.DOB.Value.Month == 1 ? CurrentPatient.DOB.Value.Year.ToString() : CurrentPatient.DOBText);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(mHIAPICheckHICardAddress, mHIData);
            HIAPICheckedHICard = GlobalsNAV.ConvertJsonToObject<HIAPICheckedHICard>(mRestJson);
            // Kiểm tra thẻ còn hạn sử dụng
            if (HIAPICheckedHICard.ghiChu.Contains("Thẻ còn giá trị sử dụng"))
            {
                // Kiểm tra giới tính
                if (CurrentPatient.GenderString != HIAPICheckedHICard.gioiTinh)
                {
                    MessageBox.Show("Thông tin giới tính bệnh nhân không đúng!");
                    return;
                }
                // Kiểm tra ngày tháng năm sinh
                if (CurrentPatient.DOBText != HIAPICheckedHICard.ngaySinh)
                {
                    MessageBox.Show("Thông tin ngày sinh bệnh nhân không đúng!");
                    return;
                }
                // Kiểm tra lại thông tin thẻ hiện tại
                DateTime ValidDateFrom = DateTime.ParseExact(HIAPICheckedHICard.gtTheTu, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime ValidDateTo = DateTime.ParseExact(HIAPICheckedHICard.gtTheDen, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                // 1. Nếu thẻ hiện tại đã lưu và chưa sửa dụng thì hỏi user cần cập nhật thông tin hay không
                if (EditingHiItem.HIID != 0)
                {
                    string temp = "";
                    if (EditingHiItem.RegistrationCode != HIAPICheckedHICard.maDKBD)
                    {
                        temp += "Nơi KCBBD";
                    }
                    if (((DateTime)EditingHiItem.ValidDateFrom).ToString("dd/MM/yyyy") != ValidDateFrom.ToString("dd/MM/yyyy"))
                    {
                        temp += " - Ngày từ";
                    }
                    if (((DateTime)EditingHiItem.ValidDateFrom).ToString("dd/MM/yyyy") != ValidDateFrom.ToString("dd/MM/yyyy"))
                    {
                        temp += " - Ngày đến";
                    }
                    if (temp != "")
                    {
                        if (MessageBox.Show(string.Format("Thông tin thẻ trên cổng \"{0}\" có thay đổi với thẻ đã tạo. Có tiếp tục thay đổi không?", temp), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                        else
                        {
                            // Cập nhật thông tin thẻ: KCBBD, ngày từ ngày đến
                            EditingHiItem.RegistrationCode = HIAPICheckedHICard.maDKBD;
                            EditingHiItem.ValidDateFrom = ValidDateFrom;
                            EditingHiItem.ValidDateTo = ValidDateTo;
                            if (HIAPICheckedHICard.maKV != "")
                            {
                                string tempMaKV = "";
                                switch (HIAPICheckedHICard.maKV)
                                {
                                    case "K1":
                                        tempMaKV = "KV1";
                                        break;
                                    case "K2":
                                        tempMaKV = "KV2";
                                        break;
                                    case "K3":
                                        tempMaKV = "KV3";
                                        break;
                                    default:
                                        tempMaKV = "KV1";
                                        break;
                                }
                                SelectedKV = AllKVCode[AllKVCode.IndexOf(tempMaKV)];
                                GetKVCode();
                            }
                            LoadHospital(EditingHiItem);
                        }
                    }
                }
                else
                {
                    // Cập nhật thông tin thẻ: KCBBD, ngày từ ngày đến
                    EditingHiItem.RegistrationCode = HIAPICheckedHICard.maDKBD;
                    EditingHiItem.ValidDateFrom = ValidDateFrom;
                    EditingHiItem.ValidDateTo = ValidDateTo;
                    if (HIAPICheckedHICard.maKV != "")
                    {
                        string tempMaKV = "";
                        switch (HIAPICheckedHICard.maKV)
                        {
                            case "K1":
                                tempMaKV = "KV1";
                                break;
                            case "K2":
                                tempMaKV = "KV2";
                                break;
                            case "K3":
                                tempMaKV = "KV3";
                                break;
                            default:
                                tempMaKV = "KV1";
                                break;
                        }
                        SelectedKV = AllKVCode[AllKVCode.IndexOf(tempMaKV)];
                        GetKVCode();
                    }
                    LoadHospital(EditingHiItem);
                }
            }
            else
            {
                MessageBox.Show(HIAPICheckedHICard.ghiChu);
                return;
            }
        }

        public void txtRegistrationCode_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            if (EditingHiItem == null || IsEditMode == false)
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
            if (Globals.CrossRegionHospital != null && Globals.CrossRegionHospital.Count > 0)
            {
                return;
            }
            ShowBusyOrDlgBusy();
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
                                    HideBusyOrDlgBusy();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    HideBusyOrDlgBusy();
                }
            });
            t.Start();
        }
        public void BtnRefreshHICardList()
        {
            if (Operation != FormOperation.ReadOnly && Operation != FormOperation.None)
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0865_G1_Msg_InfoLoadDSThe, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                GetAllHealthInsurance(true, false);
            }
            else
            {
                GetAllHealthInsurance(false, false);
            }
        }

        public void BtnDeleteHICard()
        {
            if (Operation != FormOperation.ReadOnly && Operation != FormOperation.None)
            {
                return;
            }
            if (HealthInsurances.Count() == 0)
            {
                return;
            }

            if (EditingHiItem == null || EditingHiItem.RegistrationLocation == null || EditingHiItem.RegistrationLocation.Length < 1)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0362_G1_Msg_InfoChonTheBHYT));
                return;
            }

            if (EditingHiItem.Used)
            {
                //if (EditingHiItem.ValidDateTo.HasValue && EditingHiItem.ValidDateTo >= Globals.GetCurServerDateTime())
                if (EditingHiItem.ValidDateTo.HasValue && EditingHiItem.ValidDateTo.Value.Date >= Globals.ServerDate.Value.Date)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A1017_G1_Msg_InfoKhTheXoaTheBH));
                    return;
                }
            }

            if (EditingHiItem.MarkAsDeleted)            
            {
                MessageBox.Show(eHCMSResources.Z1042_G1_TheBHYTDaDuocXoa);
                return;
            }

            if (MessageBox.Show(eHCMSResources.K0465_G1_XoaTheBHYT, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            EditingHiItem.IsActive = false;
            EditingHiItem.MarkAsDeleted = true;
            UpdateHiItem();

        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mNhanBenhBH_TheBH_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_ThemMoi, (int)ePermission.mView)
                                        ||
                                        Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mReceiveInPatient,
                                                (int)oRegistrionEx.mNhanBenhNoiTru_TheBH_ThemMoi, (int)ePermission.mView);

            mNhanBenhBH_TheBH_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_XacNhan, (int)ePermission.mView);
            //KMx: Someone viết phần cấu hình thiếu nút Sửa, nếu bây giờ kiểm tra nút "Sửa" thì tất cả nhân viên Nhận bệnh bảo hiểm sẽ không thấy nút "Sửa".
            //Nên để mặc định nút "Sửa" = true.
            //mNhanBenhBH_TheBH_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mReceivePatient,
            //                                   (int)oRegistrionEx.mNhanBenhBH_TheBH_Sua, (int)ePermission.mView);
            mNhanBenhBH_TheBH_SuaSauKhiDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_SuaSauKhiDangKy, (int)ePermission.mView);

        }
        #region checking account

        private bool _mNhanBenhBH_TheBH_ThemMoi = true;
        private bool _mNhanBenhBH_TheBH_XacNhan = true;
        private bool _mNhanBenhBH_TheBH_Sua = true;
        private bool _mNhanBenhBH_TheBH_Luu = true;
        private bool _mNhanBenhBH_TheBH_SuaSauKhiDangKy = true;

        public bool mNhanBenhBH_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenhBH_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_ThemMoi == value)
                    return;
                _mNhanBenhBH_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_ThemMoi);
            }
        }

        public bool mNhanBenhBH_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenhBH_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_XacNhan == value)
                    return;
                _mNhanBenhBH_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_XacNhan);
            }
        }

        public bool mNhanBenhBH_TheBH_Sua
        {
            get
            {
                return _mNhanBenhBH_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_Sua == value)
                    return;
                _mNhanBenhBH_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_Sua);
            }
        }

        public bool mNhanBenhBH_TheBH_Luu
        {
            get
            {
                return _mNhanBenhBH_TheBH_Luu;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_Luu == value)
                    return;
                //_mNhanBenhBH_TheBH_Luu = mNhanBenhBH_TheBH_Sua || mNhanBenhBH_TheBH_ThemMoi;
                //KMx: Chỉnh sửa vì thêm chức năng "Sửa Sau Khi Đăng Ký" (21/02/2014 11:10).
                _mNhanBenhBH_TheBH_Luu = mNhanBenhBH_TheBH_Sua || mNhanBenhBH_TheBH_ThemMoi || mNhanBenhBH_TheBH_SuaSauKhiDangKy;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_Luu);
            }
        }

        public bool mNhanBenhBH_TheBH_SuaSauKhiDangKy
        {
            get
            {
                return _mNhanBenhBH_TheBH_SuaSauKhiDangKy;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_SuaSauKhiDangKy == value)
                    return;
                _mNhanBenhBH_TheBH_SuaSauKhiDangKy = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_SuaSauKhiDangKy);
            }
        }

        #endregion
        #region Địa chỉ cho thẻ bảo hiểm y tế
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

        private CitiesProvince _selectedProvince;
        public CitiesProvince SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                // HPT: dựa theo cách làm của anh Tuấn cho HospitalAutoComplete
                // AutoComplete sau khi lost focus thì tự động set SelectedItem của nó về null
                if (_selectedProvince != value)
                {
                    if (value == null && bSelectProvinceInit)
                    {
                        bSelectProvinceInit = false;
                    }
                    else
                    {
                        _selectedProvince = value;
                        NotifyOfPropertyChange(() => SelectedProvince);
                    }
                }
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
        private string _PatientStreetAddress;
        public string PatientStreetAddress
        {
            get 
            { 
                return _PatientStreetAddress; 
            }
            set
            {
                _PatientStreetAddress = value;
                NotifyOfPropertyChange(() => PatientStreetAddress);
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
            AllKVCode = new ObservableCollection<string> {"---", "KV1","KV2","KV3"};
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


        AxAutoComplete AcbHICity { get; set; }
        public void AcbHICity_Loaded(object sender, RoutedEventArgs e)
        {
            AcbHICity = sender as AxAutoComplete;
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

        public void PagingLinq(long CityProvinceID)
        {
            if (Globals.allSuburbNames == null)
            {
                return;
            }
            SuburbNames = new ObservableCollection<SuburbNames>();

            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CityProvinceID)
                {
                    SuburbNames.Add(item);
                }
            }
            IsProcessing = false;
            
        }
        //▼====== #006
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
        AxComboBox cboHIWardName { get; set; }
        public void cboHIWardName_Loaded(object sender, RoutedEventArgs e)
        {
            cboHIWardName = sender as AxComboBox;
        }
        public void PagingLinqForWardName(long SuburbNameID)
        {
            if (Globals.allWardNames == null)
            {
                return;
            }
            ListWardNames = new ObservableCollection<WardNames>();
            WardNames DefaultWardNames = new WardNames();
            if (!Globals.ServerConfigSection.CommonItems.ShowAddressPKBSHuan)
            {
                DefaultWardNames.WardName = eHCMSResources.Z2338_G1_KhongXacDinh;
                DefaultWardNames.WardNameID = -1;
            }
            else
            {
                DefaultWardNames.WardName = "";
                DefaultWardNames.WardNameID = -2;
            }
            DefaultWardNames.SuburbNameID = SuburbNameID;
            ListWardNames.Add(DefaultWardNames);
            foreach (var item in Globals.allWardNames)
            {
                if (item.SuburbNameID == SuburbNameID)
                {
                    ListWardNames.Add(item);
                }
            }
        }
        public void cboSuburb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditingHiItem != null)
            {
                PagingLinqForWardName(EditingHiItem.SuburbNameID);
            }
        }
        //▲====== #006
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

        public void AcbHICity_Populating(object sender, PopulatingEventArgs e)
        {
            bSelectProvinceInit = false;
            if (sender == null || Provinces == null)
            {
                return;
            }
            string SearchText = ((AutoCompleteBox)sender).SearchText;
            // TxD 09/11/2014: For some unknown reason if Search text is set to empty string ie. Length = 0
            //                  the autocompletebox will fail to populate the next time ie. this function is not called  
            //                  So the following block of code is to prevent that from happening
            if (SearchText == null || SearchText.Length == 0)
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

       
        AxComboBox cboHISuburb { get; set; }
        public void cboHISuburb_Loaded(object sender, RoutedEventArgs e)
        {
            cboHISuburb = sender as AxComboBox;           
        }
        public void cboHISuburb_GotFocus(object sender, RoutedEventArgs e)
        {

        }
        public void AcbHICity_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
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
                EditingHiItem.CityProvinceID_Address = Convert.ToInt16(CityProvinceID);
                EditingHiItem.SuburbNameID = -1;
                PagingLinq((long)EditingHiItem.CityProvinceID_Address);
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
                SelectedProvince = new CitiesProvince();
                PagingLinq(0);
                return;
            }
            SelectedProvince = Provinces.Where(x => x.CityProvinceID == CurrentPatient.CitiesProvince.CityProvinceID).FirstOrDefault();
            PagingLinq(SelectedProvince.CityProvinceID);
            if (CurrentPatient.SuburbNameID > 0)
            {
                EditingHiItem.SuburbNameID = CurrentPatient.SuburbNameID;
            }
            else
            {
                EditingHiItem.SuburbNameID = 0;
            }
            //▼====== #006:
            if (CurrentPatient.WardName.WardNameID > 0)
            {
                EditingHiItem.WardNameID = CurrentPatient.WardName.WardNameID;
            }
            else
            {
                EditingHiItem.WardNameID = -1; //-1 Là không xác định.
            }
            //▲====== #006
        }

        private bool bSelectProvinceInit = false;

        private string _Reason;
        public string Reason
        {
            get { return _Reason; }
            set
            {
                if (_Reason != value)
                {
                    _Reason = value;
                    NotifyOfPropertyChange(() => Reason);
                }
            }
        }
        #endregion
        public void Handle(ItemSelected<HIAPICheckedHICard> message)
        {
            if (GetView() != null && message != null && message.Source is ICheckedValidHICard)
            {
                if (EditingHiItem != null && message.Item != null)
                {
                    EditingHiItem.CheckedHICardValidResult = message.Item.ghiChu;
                    HealthInsurances.First(x => x.HIID == EditingHiItem.HIID).CheckedHICardValidResult = message.Item.ghiChu;
                }
            }
        }

        //▼====== #005
        AxDateTextBox DateFrom = null;
        public void DateFrom_Loaded(object sender, RoutedEventArgs e)
        {
            DateFrom = sender as AxDateTextBox;
        }

        AxDateTextBox DateTo = null;
        public void DateTo_Loaded(object sender, RoutedEventArgs e)
        {
            DateTo = sender as AxDateTextBox;
        }
        public void DateFrom_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            string tmpDateFrom = DateFrom.Text;
            // nếu đã tự động lấy thông tin thì không cần cập nhật ngày đến
            if (!Globals.ServerConfigSection.CommonItems.AutoGetHICardDataFromHIPortal)
            {
                if (!string.IsNullOrEmpty(tmpDateFrom))
                {
                    DateTime tmpdate = Convert.ToDateTime(DateFrom.Text).AddYears(1);
                    DateTo.Text = tmpdate.AddSeconds(-1).ToString();
                    DateTo.GetBindingExpression(AxDateTextBox.TextProperty).UpdateSource();
                }
                else
                {
                    DateTo.Clear();
                }
            }
        }
        public void DateTo_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            BindingExpression be = DateTo.GetBindingExpression(AxDateTextBox.TextProperty);
            be.UpdateSource();
        }
        //▲====== #005
        public void GridHIItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedHiItem == null)
            {
                return;
            }
            Globals.EventAggregator.Publish(new DoubleClickEvent());
        }
        //▼===== #007: Declare ra 2 hàm để phục vụ việc phân chia cho busy của popup hay của view.
        private void ShowBusyOrDlgBusy()
        {
            if (this.IsDialogView)
            {
                this.DlgShowBusyIndicator();
            }
            else
            {
                this.ShowBusyIndicator();
            }
        }
        private void HideBusyOrDlgBusy()
        {
            if (this.IsDialogView)
            {
                this.DlgHideBusyIndicator();
            }
            else
            {
                this.HideBusyIndicator();
            }
        }
        //▲===== #007
        private ObservableCollection<Lookup> _ReceiveMethod;

        public ObservableCollection<Lookup> ReceiveMethod
        {
            get { return _ReceiveMethod; }
            set
            {
                _ReceiveMethod = value;
                NotifyOfPropertyChange(() => ReceiveMethod);
            }
        }
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
    }
}

