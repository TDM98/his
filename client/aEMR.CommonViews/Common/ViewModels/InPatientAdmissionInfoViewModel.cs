using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using  System.Linq;
using aEMR.Common;
using eHCMS.CommonUserControls.CommonTasks;
using aEMR.Controls;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
/*
* 20171107 #002 CMN: Added IsConfirmEmergencyTreatment into AdmissionInfo
* 20200417 #003 TNHX: Add PatientFullAddress into ICommonPreviewView
* 20200718 #004 TNHX: Đổi input nhập ngày qua content IMinHourDateControl
* 20200923 #005 TNHX: Thêm địa chỉ short cho PatientInfo
* 20220416 #006 DatTB: Thêm cấu hình xác nhận hoãn tạm ứng
* 20220623 #007 DatTB: Khóa trường Loại đối tượng sau khi nhập viện
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientAdmissionInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientAdmissionInfoViewModel : Conductor<object>, IInPatientAdmissionInfo
        , IHandle<ItemSelected<IcwdBedPatientCommon, BedPatientAllocs>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientAdmissionInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            ClientLoggerHelper.LogInfo("====================================> InPatientAdmissionInfoViewModel <====================================");
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //KMx: Lấy giá trị trực tiếp từ Globals.AllLookupValueList luôn. Không cần về server nữa (27/02/2016 15:42).
            //AdmissionReasonContent = Globals.GetViewModel<ILookupValueListing>();
            //AdmissionReasonContent.Type = LookupValues.ADMISSION_TYPE;
            //AdmissionReasonContent.AddSelectOneItem = true;

            AdmissionDateTime = Globals.GetViewModel<IMinHourDateControl>();
            AdmissionDateTime.DateTime = Globals.GetCurServerDateTime();

            InitData();

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            //DepartmentContent.LstRefDepartment = LstRefDepartment;
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.IsLoadDrugDept = false;
            IsRefDept = false;
            IsObjectType = false; // #007

            //if(!DesignerProperties.IsInDesignTool 
            //    //&& LstRefDepartment!=null
            //    //&& LstRefDepartment.Count>0
            //    )
            //{
            //    AdmissionReasonContent.LoadData();
            //    //KMx: Không tự động load Khoa nữa. Bị lỗi chạy đua (11/09/2014 14:47).
            //    //DepartmentContent.LoadData();
            //}
            Globals.EventAggregator.Subscribe(this);
            //(AdmissionReasonContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(AdmissionReasonContent_PropertyChanged);
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            LoadDeptTransferDocTypeReq();
        }

        public void InitData()
        {
            AdmissionTypeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.ADMISSION_TYPE).ToObservableCollection();
            AccidentCodeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_AccidentCode).ToObservableCollection();
            ObjectTypeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ObjectType).ToObservableCollection(); //---- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
            AdmissionTypeList.Insert(0, firstItem);
            AccidentCodeList.Insert(0, firstItem);

            if (CurrentAdmission == null)
            {
                CurrentAdmission = new InPatientAdmDisDetails();
            }
            CurrentAdmission.V_AdmissionType = AdmissionTypeList.FirstOrDefault().LookupID;
            CurrentAdmission.V_AccidentCode = AccidentCodeList.FirstOrDefault().LookupID;
            CurrentAdmission.V_ObjectType = ObjectTypeList.FirstOrDefault().LookupID; //---- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
            /*▼====: #002*/
            TreatmentTypeCollection = new ObservableCollection<Lookup> {
                new Lookup{LookupID = -1, ObjectValue = eHCMSResources.A0015_G1_Chon},
                new Lookup{LookupID = 1, ObjectValue = eHCMSResources.K0774_G1_1CapCuu},
                new Lookup{LookupID = 0, ObjectValue = eHCMSResources.Z2152_G1_KhongCapCuu}
            };
            SelectedTreatmentType = TreatmentTypeCollection.First();
            /*▲====: #002*/
            AdmissionDateTime.DateTime = CurrentAdmission.AdmissionDate != null ? CurrentAdmission.AdmissionDate : Globals.GetCurServerDateTime();
            //▼====: #006
            if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
            {
                EnablePostponementAdvancePayment = true;
                IsPtNomal = true;
            }
            else
            {
                EnablePostponementAdvancePayment = false;
                IsPtNomal = false;
            }
            //▲====: #006
        }

        public void InitViewContent()
        {

        }

        void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                DepartmentChange();
            }
        }

        private long PrevSelectedDeptID = 0;

        // TxD 24/05/2015 The following method is called when DepartmentListingViewModel SelectedItem property is Set and NotifyPropertyChanged is triggered.
        public void DepartmentChange()
        {
            if (CurrentAdmission == null || DepartmentContent.SelectedItem == null)
                return;
            
            if (PrevSelectedDeptID > 0 && PrevSelectedDeptID == DepartmentContent.SelectedItem.DeptID)
            {
                // TxD 24/05/2015: Should get in here after 'Nhap Vien' successfully and CurrentAdmission is being reset to the newly saved one
                SetSelectedLocation(CurrentAdmission.DeptLocationID);
                return;
            }

            PrevSelectedDeptID = DepartmentContent.SelectedItem.DeptID;
            CurrentAdmission.Department = DepartmentContent.SelectedItem;
            if (CurrentAdmission.PatientRegistration != null)
            {
                CurrentAdmission.PatientRegistration.RefDepartment = DepartmentContent.SelectedItem;
            }

            long? deptId = null;
            if (DepartmentContent.SelectedItem != null)
            {
                deptId = DepartmentContent.SelectedItem.DeptID;
            }
            if (deptId.HasValue && deptId.Value > 0)
            {
                //LoadLocations(deptId);
                Coroutine.BeginExecute(DoLoadLocations(deptId.Value));
            }
            else
            {
                if (Locations != null)
                {
                    Locations.Clear();

                    var itemDefault = new DeptLocation();
                    itemDefault.DeptID = -1;
                    itemDefault.Location = new Location();
                    itemDefault.Location.LID = -1;
                    itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1098_G1_TatCaPhg);
                    Locations.Insert(0, itemDefault);
                    if (SelectedLocation == null)
                    {
                        ClientLoggerHelper.LogInfo("DepartmentChange SelectedLocation = null");
                    }
                    else
                    {
                        ClientLoggerHelper.LogInfo("DepartmentChange SelectedLocation = " + " " + SelectedLocation.Location.LocationName.ToString());
                    }
                    if (SelectedLocation == null || !Locations.Contains(SelectedLocation))
                    {
                        SelectedLocation = itemDefault;
                    }
                }
            }
            
        }

        //void AdmissionReasonContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if(e.PropertyName == "SelectedItem")
        //    {
        //        if (CurrentAdmission != null)
        //        {
        //            CurrentAdmission.VAdmissionType = AdmissionReasonContent.SelectedItem;
        //            CurrentAdmission.V_AdmissionType = AdmissionReasonContent.SelectedItem != null ? AdmissionReasonContent.SelectedItem.LookupID : 0;
        //        }
        //    }
        //}

        private IMinHourDateControl _AdmissionDateTime;
        public IMinHourDateControl AdmissionDateTime
        {
            get { return _AdmissionDateTime; }
            set
            {
                _AdmissionDateTime = value;
                NotifyOfPropertyChange(() => AdmissionDateTime);
            }
        }

        public RefDepartment CurrentDepartment
        {
            get
            {
                return DepartmentContent == null ? null : DepartmentContent.SelectedItem;
            }
            set
            {
                if (DepartmentContent != null)
                {
                    DepartmentContent.SelectedItem = value;
                }
            }
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

        private ObservableCollection<Lookup> _AdmissionTypeList;
        public ObservableCollection<Lookup> AdmissionTypeList
        {
            get { return _AdmissionTypeList; }
            set
            {
                if (_AdmissionTypeList != value)
                    _AdmissionTypeList = value;
                NotifyOfPropertyChange(() => AdmissionTypeList);
            }
        }

        private ObservableCollection<Lookup> _AccidentCodeList;
        public ObservableCollection<Lookup> AccidentCodeList
        {
            get { return _AccidentCodeList; }
            set
            {
                if (_AccidentCodeList != value)
                    _AccidentCodeList = value;
                NotifyOfPropertyChange(() => AccidentCodeList);
            }
        }

        //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
        private ObservableCollection<Lookup> _ObjectTypeList;
        public ObservableCollection<Lookup> ObjectTypeList
        {
            get { return _ObjectTypeList; }
            set
            {
                if (_ObjectTypeList != value)
                    _ObjectTypeList = value;
                NotifyOfPropertyChange(() => ObjectTypeList);
            }
        }
        //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do

        private bool _isAdmision;
        public bool isAdmision
        {
            get { return _isAdmision; }
            set
            {
                _isAdmision = value;
                NotifyOfPropertyChange(() => isAdmision);
            }
        }

        private bool _isRead=true;
        public bool isRead
        {
            get { return _isRead; }
            set
            {
                _isRead = value;
                NotifyOfPropertyChange(() => isRead);
            }
        }
        
        //private ObservableCollection<long> _LstRefDepartment;

        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get
        //    {
        //        return _LstRefDepartment;
        //    }
        //    set
        //    {
        //        _LstRefDepartment = value;

        //        //KMx: Không tự động load khoa nữa, khi nào parent kiu thì load, nếu không sẽ bị lỗi chạy đua (11/09/2014 14:49).
        //        //DepartmentContent.LstRefDepartment = LstRefDepartment;
        //        //DepartmentContent.AddSelectOneItem = true;

        //        //if (!DesignerProperties.IsInDesignTool)
        //        //{
        //        //    DepartmentContent.LoadData();
        //        //}

        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        //private ILookupValueListing _admissionReasonContent;
        //public ILookupValueListing AdmissionReasonContent
        //{
        //    get { return _admissionReasonContent; }
        //    set
        //    {
        //        _admissionReasonContent = value;
        //        NotifyOfPropertyChange(()=>AdmissionReasonContent);
        //    }
        //}


        private InPatientAdmDisDetails _currentAdmission;
        public InPatientAdmDisDetails CurrentAdmission
        {
            get { return _currentAdmission; }
            set
            {
                //CommitEdit();
                if(_currentAdmission != null)
                {
                    _currentAdmission.PropertyChanged -= _currentAdmission_PropertyChanged;
                }

                _currentAdmission = value;
                CommitEdit();
                //IsEditing = false;

                if (_currentAdmission != null)
                {
                    _currentAdmission.PropertyChanged += _currentAdmission_PropertyChanged; 
                }

                NotifyOfPropertyChange(() => CurrentAdmission);
                NotifyOfPropertyChange(() => ShowButtons);
                
                NotifyOfPropertyChange(() => CanSaveChangesCmd);
                NotifyOfPropertyChange(() => CanBeginEditCmd);
                NotifyOfPropertyChange(() => CanCanceEditCmd);
                NotifyOfPropertyChange(() => IsBeginNewOutPtTreatment);
                if (_currentAdmission != null)
                {
                    //AdmissionReasonContent.SetSelectedID(_currentAdmission.V_AdmissionType);
                    if (_currentAdmission.InPatientDeptDetails != null && _currentAdmission.InPatientDeptDetails.Count > 0)
                    {
                        // TxD 24/05/2015: The sequence of actions will occur after the following method is called:
                        //                 Inside DepartmentListingViewModel:
                        //                 1. SelectedItem is set then call NotifyPropertyChange
                        //                 2. NotifyPropertyChange will in turn trigger a call to DepartmentChange of this ViewModel
                        //                 3. Thus we do not need to call SetSelectedLocation here but inside DepartmentChange to determine whether a call to LoadLocation is required.
                        DepartmentContent.SetSelectedDeptItem(_currentAdmission.Department.DeptID);                                                       
                        AdmissionDateTime.DateTime = CurrentAdmission.AdmissionDate;
                        //SetSelectedLocation(_currentAdmission.DeptLocationID);
                    }
                    /*▼====: #002*/
                    if (TreatmentTypeCollection != null)
                        SelectedTreatmentType = TreatmentTypeCollection.Where(x => x.LookupID == (long)(CurrentAdmission.IsConfirmEmergencyTreatment == null ? -1 : Convert.ToInt32(CurrentAdmission.IsConfirmEmergencyTreatment))).FirstOrDefault();
                    /*▲====: #002*/
                    //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                    if (_currentAdmission.V_ObjectType == null || _currentAdmission.V_ObjectType == 0)
                    {
                        _currentAdmission.V_ObjectType = ObjectTypeList.FirstOrDefault().LookupID;
                    }
                    //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                }
                else
                {
                    DepartmentContent.SelectedItem = null;
                }
            }
        }

        private bool _IsNotGuestEmergencyAdmission = true;
        public bool IsNotGuestEmergencyAdmission
        {
            get
            {
                return _IsNotGuestEmergencyAdmission;
            }
            set
            {
                if (_IsNotGuestEmergencyAdmission == value)
                {
                    return;
                }
                _IsNotGuestEmergencyAdmission = value;
                NotifyOfPropertyChange(() => IsNotGuestEmergencyAdmission);
                NotifyOfPropertyChange(() => IsNotGuestEmergencyAdmissionEnable);
            }
        }
        public bool IsNotGuestEmergencyAdmissionEnable
        {
            get
            {
                return IsNotGuestEmergencyAdmission && IsEditing;
            }
        }
        public void SetEmergencyAmissionInfo(long aDeptID)
        {
            IsRefDept = (aDeptID == 0);
            NotifyOfPropertyChange(() => ShowButtons);
            IsNotGuestEmergencyAdmission = (aDeptID == 0);
            if (aDeptID > 0)
            {
                DepartmentContent.SetSelectedDeptItem(aDeptID);
                SelectedTreatmentType = TreatmentTypeCollection.FirstOrDefault(x => x.LookupID == 1);
                CurrentAdmission.V_AdmissionType = (long)AllLookupValues.V_AdmissionType.Emergency;
            }
        }

        void _currentAdmission_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AdmissionNote":
                case "V_AdmissionType":
                case "Department":
                case "AdmissionDate":
                case "HosTransferIn":
                case "ReferralDiagnosis":
                case "V_AccidentCode":
                case "IsConfirmEmergencyTreatment":
                    if(IsEditing)
                    {
                        InfoHasChanged = true;
                    }
                    break;
                //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                case "V_ObjectType":
                    //▼====: #006
                    if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
                    {
                        if (CurrentAdmission.V_ObjectType == 874001 || CurrentAdmission.V_ObjectType == 874003)
                        {
                            IsPtNomal = true;
                        }
                        else
                        {
                            IsPtNomal = false;
                            CurrentAdmission.IsPostponementAdvancePayment = false;
                        }

                        if (IsEditing)
                        {
                            InfoHasChanged = true;
                        }
                    }
                    //▲====: #006
                    break;
                case "IsPostponementAdvancePayment":
                    //▼====: #006
                    if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
                    {
                        if (CurrentAdmission.IsPostponementAdvancePayment == false)
                        {
                            CurrentAdmission.PostponementAdvancePaymentNote = null;
                        }
                        if (IsEditing)
                        {
                            InfoHasChanged = true;
                        }
                    }
                    //▲====: #006
                    break;
                case "PostponementAdvancePaymentNote":
                    //▼====: #006
                    if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
                    {
                        if (IsEditing)
                        {
                            InfoHasChanged = true;
                        }
                    }
                    //▲====: #006
                    break;
                    //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
            }
        }


        //KMx: Sau khi kiểm tra, thấy hàm này không còn dùng nữa (05/09/2014 17:03).
        //public void SelectBedAllocationCmd()
        //{
        //    if(CurrentAdmission == null)
        //    {
        //        MessageBox.Show("Chưa chọn đăng ký. Không thể đặt giường");
        //        return;
        //    }
        //    var bedAllocVm = Globals.GetViewModel<IBedPatientAlloc>();
            
        //    bedAllocVm.curPatientRegistration = CurrentAdmission.PatientRegistration;
        //    bedAllocVm.BookBedAllocOnly = true;
        //    bedAllocVm.DefaultDepartment = DepartmentContent.SelectedItem;

        //    Globals.ShowDialog(bedAllocVm as Conductor<object>);
        //}

        public bool checkExistBedPatient(ObservableCollection<BedPatientAllocs> lstBp,BedPatientAllocs bp )
        {
            if(lstBp!=null && lstBp.Count>0)
            {
                foreach (BedPatientAllocs bedPatientAlloc in lstBp)
                {
                    if (bedPatientAlloc.VBedAllocation.BedAllocationID == bp.VBedAllocation.BedAllocationID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Handle(ItemSelected<IcwdBedPatientCommon, BedPatientAllocs> message)
        {
            if (GetView() != null && message != null)
            {
                if (CurrentAdmission != null && CurrentAdmission.PatientRegistration != null)
                {
                    if (CurrentAdmission.PatientRegistration.BedAllocations == null)
                    {
                        CurrentAdmission.PatientRegistration.BedAllocations = new BindableCollection<BedPatientAllocs>();
                    }
                    //Tam thoi lam the nay. Ben Dinh dua qua khong dung.
                    //Kiem tra xem co trung giuong o day khong
                    if(checkExistBedPatient(CurrentAdmission.PatientRegistration.BedAllocations,(CloneBedPatientAllocs(message.Item))))
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1022_G1_BNDaDatGiuongNay));
                        return;
                    }
                    if (SelectedLocation == null)
                    {
                        ClientLoggerHelper.LogInfo("DepartmentChange ItemSelected<IcwdBedPatientCommon, BedPatientAllocs>  SelectedLocation = null");
                    }
                    else
                    {
                        ClientLoggerHelper.LogInfo("DepartmentChange ItemSelected<IcwdBedPatientCommon, BedPatientAllocs>  SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
                    }
                    
                    SelectedLocation = message.Item.VBedAllocation.VDeptLocation;

                    CurrentAdmission.PatientRegistration.BedAllocations.Add(CloneBedPatientAllocs(message.Item)); 
                }
            }
        }
        private  BedPatientAllocs CloneBedPatientAllocs(BedPatientAllocs bedAlloc)
        {
            var retVal = new BedPatientAllocs();
            retVal.VBedAllocation = bedAlloc.VBedAllocation;
            retVal.ResponsibleDepartment = bedAlloc.ResponsibleDepartment;
            retVal.CheckInDate = bedAlloc.CheckInDate;
            retVal.CanDelete = true;

            return  retVal;
        }

        public bool checkBedLocation()
        {
            if(CurrentAdmission.PatientRegistration.BedAllocations == null || CurrentAdmission.PatientRegistration.BedAllocations.Count < 2)
            {
                return true;
            }
            long deptlocationID=CurrentAdmission.PatientRegistration.BedAllocations.FirstOrDefault()
                .VBedAllocation.VDeptLocation.DeptLocationID;
            for (int i = 1; i < CurrentAdmission.PatientRegistration.BedAllocations.Count;i++ )
            {
                if(deptlocationID!=CurrentAdmission.PatientRegistration.BedAllocations[i].VBedAllocation.VDeptLocation.DeptLocationID)
                {
                    return false;
                }
            }
            return true;
        }
        

        public bool ValidateInfo(out ObservableCollection<ValidationResult> validationResults)
        {
            // TxD 23/05/2015 Added the following just incase because these fields DO NOT BIND directly to the corresponding properties of CurrentAdmission
            if (CurrentAdmission != null && DepartmentContent != null && DepartmentContent.SelectedItem != null && DepartmentContent.SelectedItem.DeptID > 0)
            {
                CurrentAdmission.Department = DepartmentContent.SelectedItem;
            }
            if (CurrentAdmission != null && SelectedLocation != null && SelectedLocation.DeptLocationID > 0)
            {
                CurrentAdmission.DeptLocationID = SelectedLocation.DeptLocationID;
            }
            if (CurrentAdmission != null)
            {
                CurrentAdmission.AdmissionDate = AdmissionDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
            }

            validationResults = null;
            if (_currentAdmission == null)
            {
                return false;
            }
           
            var result = new ObservableCollection<ValidationResult>();
            
            if (_currentAdmission.PatientRegistration == null)
            {
                var item = new ValidationResult(eHCMSResources.K0299_G1_ChonDK, new[] { "PatientRegistration" });
                result.Add(item);
            }

            // TxD 23/01/2015 Commented the following out because it doesn't look right looks like a bogus checking
            //if (!checkBedLocation())
            //{
            //    var item = new ValidationResult("Bạn chọn giường ở nhiều phòng khác nhau. Chọn giường trong cùng một phòng để nhập viện.."
            //        , new[] { "BedPatientAlloc" });
            //    result.Add(item);
            //}

            if (!_currentAdmission.AdmissionDate.HasValue || _currentAdmission.AdmissionDate.Value == DateTime.MinValue)
            {
                var item = new ValidationResult("Hãy chọn Ngày nhập viện.", new[] { "AdmissionDate" });
                result.Add(item);
            }

            if (_currentAdmission.AdmissionDate.GetValueOrDefault() > Globals.GetCurServerDateTime())
            {
                var item = new ValidationResult("Ngày nhập viện không được lớn hơn ngày hiện tại.", new[] { "AdmissionDate" });
                result.Add(item);
            }

            if (CurrentAdmission.Department == null || CurrentAdmission.Department.DeptID <= 0)
            {
                var item = new ValidationResult("Hãy chọn Khoa nhập viện.", new[] { "Department" });
                result.Add(item);
            }

            if (SelectedLocation == null || SelectedLocation.DeptLocationID <= 0)
            {
                var item = new ValidationResult(eHCMSResources.Z0116_G1_HayChonPg, new[] { "SelectedLocation" });
                result.Add(item);
            }

            if (_currentAdmission.V_AdmissionType <= 0)
            {
                var item = new ValidationResult("Hãy chọn Loại nhập viện.", new[] { "V_AdmissionReason" });
                result.Add(item);
            }

            if (_currentAdmission.V_AccidentCode <= 0)
            {
                var item = new ValidationResult("Hãy chọn tai nạn thương tích.", new[] { "V_AccidentCode" });
                result.Add(item);
            }
            //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
            if (_currentAdmission.V_ObjectType <= 0)
            {
                var item = new ValidationResult("Hãy chọn Loại bệnh nhân.", new[] { "V_ObjectType" });
                result.Add(item);
            }
            //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
            if (CurrentAdmission.InPatientDeptDetails != null && CurrentAdmission.InPatientDeptDetails.Count > 0)
            {
                var minDate = Globals.GetCurServerDateTime();
                //Huyen 11/08/2015: lấy ngày nhập khoa nhỏ nhất. Khi dời ngày nhập viện lên không được lớn hơn ngày nhập khoa nhỏ nhất.
                foreach (var itemadm in CurrentAdmission.InPatientDeptDetails)
                {
                    if (itemadm.FromDate < minDate && itemadm.IsAdmittedRecord != true)
                    {
                        minDate = itemadm.FromDate;
                    }
                }
                if (CurrentAdmission.AdmissionDate > minDate)
                {
                    var item = new ValidationResult("Ngày nhập viện phải nhỏ hơn ngày nhập khoa đầu tiên.", new[] { "AdmissionDate" });
                    result.Add(item);    
                }
                //Huyen 11/08/2015:Tìm ngày xuất khoa đầu tiên nếu có
                DateTime? OutFirstDeptDate = Globals.GetCurServerDateTime();
                foreach (var itemadm in CurrentAdmission.InPatientDeptDetails)
                {
                    if (itemadm.IsAdmittedRecord == true)
                    {
                        OutFirstDeptDate = itemadm.ToDate;
                        break;
                    }
                }
                //Huyen 11/08/2015: ngày nhập viện không được lớn hơn ngày xuất khoa đầu tiên
                if (OutFirstDeptDate != null && CurrentAdmission.AdmissionDate > OutFirstDeptDate)
                {
                    var item = new ValidationResult(string.Format("Ngày nhập viện phải nhỏ hơn ngày xuất khoa đầu tiên.\n Ngày xuất khoa đầu tiên: {0}", OutFirstDeptDate), new[] { "AdmissionDate" });
                    result.Add(item);  
                }
                //if (_currentAdmission.AdmissionDate.GetValueOrDefault(DateTime.MinValue) > minDate)
                //{
                //    var item = new ValidationResult("Ngày nhập viện phải nhỏ hơn ngày nhập khoa đầu tiên.", new[] { "AdmissionDate" });
                //    result.Add(item);    
                //}
            }

            if(result.Count > 0)
            {
                validationResults = result;
                return false;
            }
            return true;
        }
        public void RemoveBedAllocItem(object source,EventArgs<object> eventArgs)
        {
            if(CurrentAdmission.PatientRegistration.BedAllocations != null)
            {
                var bedPatientAlloc = eventArgs.Value as BedPatientAllocs;
                if(bedPatientAlloc != null && bedPatientAlloc.BedPatientID <= 0)
                {
                    CurrentAdmission.PatientRegistration.BedAllocations.Remove(bedPatientAlloc);    
                }
            }
        }


        public void LoadData()
        {
            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                //DepartmentContent.LstRefDepartment = LstRefDepartment;
                DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
                // TxD 23/05/2015: DepartmentContent needs to load all departments for InPatientManagement View
                //                  and only limited to authorised departments for Admission View
                if (isAdmision)
                {
                    DepartmentContent.LoadData();
                }
                else
                {
                    DepartmentContent.LoadData(0, true);
                }
                
                if ((Locations == null || Locations.Count() == 0) && DepartmentContent.SelectedItem != null)
                {
                    Coroutine.BeginExecute(DoLoadLocations(DepartmentContent.SelectedItem.DeptID));
                }
            }
        }


        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }
        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                
                _selectedLocation = value;
                if (SelectedLocation == null)
                {
                    ClientLoggerHelper.LogInfo("SelectedLocation = NULL");
                }
                else
                {
                    ClientLoggerHelper.LogInfo("SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
                }
                NotifyOfPropertyChange(() => SelectedLocation);
                if (IsEditing)
                {
                    InfoHasChanged = true;
                }
                
                // TxD 23/07/2014: DeptLocationID has been added to InPatientAdmDisDetails but it's NOT bound directly with
                //                  the Combobox so it has to be set indirectly via selecteditem of the combobox
                if (_selectedLocation != null)
                {
                    CurrentAdmission.DeptLocationID = _selectedLocation.DeptLocationID;
                }

            }
        }

        AxComboBox LocationCombo = null;
        public void Location_Combo_Loaded(object source, RoutedEventArgs eventArgs)
        {
            if (source != null)
            {
                LocationCombo = (AxComboBox)source;
            }
        }

        public void SetSelectedLocation(long selDeptLocationID)
        {
            if (selDeptLocationID <= 0)
            {
                LocationCombo.SelectedIndex = 0;
                return;
            }
            ClientLoggerHelper.LogInfo("SetSelectedLocation 1st selDeptLocationID = " + " " + selDeptLocationID.ToString());
            bool bFound = false;
            foreach (var deptLocItem in Locations)
            {
                if (deptLocItem.DeptLocationID == selDeptLocationID)
                {
                    bFound = true;
                    SelectedLocation = deptLocItem;
                    break;
                }
            }
            if (!bFound)
            {
                LocationCombo.SelectedIndex = 0;
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            if (CurrentAdmission != null && CurrentAdmission.InPatientAdmDisDetailID > 0 && CurrentAdmission.DeptLocationID > 0)
            {
                SetSelectedLocation(CurrentAdmission.DeptLocationID);
            }
            else
            {
                var itemDefault = new DeptLocation();
                itemDefault.Location = new Location();
                itemDefault.Location.LID = -1;
                itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                Locations.Insert(0, itemDefault);
                SelectedLocation = itemDefault;
            }
            yield break;
        }

        public void AdmDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        public void AdmDatePicker_CalendarClosed(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        public void LoadLocations(long? deptId)
        {
            ClientLoggerHelper.LogInfo("LoadLocations deptId =" + " " + deptId.ToString());
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}.", eHCMSResources.Z0115_G1_LayDSPgBan) });

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1098_G1_TatCaPhg);
                            Locations.Insert(0, itemDefault);
                            if (SelectedLocation == null)
                            {
                                ClientLoggerHelper.LogInfo("LoadLocations 2nd SelectedLocation = NULL");
                            }
                            else
                            {
                                ClientLoggerHelper.LogInfo("LoadLocations 2nd SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
                            }
                            
                            if (SelectedLocation == null || !Locations.Contains(SelectedLocation))
                            {
                                SelectedLocation = itemDefault;
                                if (SelectedLocation == null)
                                {
                                    ClientLoggerHelper.LogInfo("LoadLocations 3rd SelectedLocation = NULL");
                                }
                                else
                                {
                                    ClientLoggerHelper.LogInfo("LoadLocations 3rd SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set 
            {
                _isEditing = value; 
                NotifyOfPropertyChange(() => IsEditing);
                //IsRefDept = IsEditing && isAdmision;
                NotifyOfPropertyChange(() => ShowButtons);
                NotifyOfPropertyChange(() => CanSaveChangesCmd);
                NotifyOfPropertyChange(() => CanBeginEditCmd);
                NotifyOfPropertyChange(() => CanCanceEditCmd);
                NotifyOfPropertyChange(() => IsNotGuestEmergencyAdmissionEnable);
            }
        }

        private bool _IsRefDept;
        public bool IsRefDept
        {
            get { return _IsRefDept; }
            set 
            {
                _IsRefDept = value;
                NotifyOfPropertyChange(() => IsRefDept);
            }
        }

        //▼==== #007
        private bool _IsObjectType;
        public bool IsObjectType
        {
            get { return _IsObjectType; }
            set
            {
                _IsObjectType = value;
                NotifyOfPropertyChange(() => IsObjectType);
            }
        }
        //▲==== #007

        private bool _InfoHasChanged;
        public bool InfoHasChanged
        {
            get { return _InfoHasChanged; }
            set
            {
                _InfoHasChanged = value;
                NotifyOfPropertyChange(() => InfoHasChanged);
                NotifyOfPropertyChange(() => CanSaveChangesCmd);
                NotifyOfPropertyChange(() => CanBeginEditCmd);
                NotifyOfPropertyChange(() => CanCanceEditCmd);
            }
        }

        public bool IsBeginNewOutPtTreatment
        {
            get { return CurrentAdmission != null && CurrentAdmission.PatientRegistration != null && CurrentAdmission.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU; }
        }

        #region COMMANDS
        public bool CanSaveChangesCmd
        {
            get
            {
                return CurrentAdmission != null
                    && CurrentAdmission.InPatientAdmDisDetailID > 0
                    && IsEditing
                    && InfoHasChanged;
            }
        }

        /// <summary>
        /// Lưu những thay đổi trên thông tin nhập viện
        /// </summary>
        public void SaveChangesCmd()
        {
            if (Globals.IsLockRegistration(RegLockFlag, "lưu sửa đổi thông tin nhập viện"))
            {
                return;
            }
            var validationResults = new ObservableCollection<ValidationResult>();
            ValidateInfo(out validationResults);
            if(validationResults != null && validationResults.Count > 0)
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent { ValidationResults = validationResults });
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        CurrentAdmission.IsGuestEmergencyAdmission = !IsNotGuestEmergencyAdmission;
                        contract.BeginUpdateInPatientAdmDisDetails(CurrentAdmission, SelectedLocation, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndUpdateInPatientAdmDisDetails(asyncResult);
                                CommitEdit();
                                Globals.EventAggregator.Publish(new UpdateCompleted<InPatientAdmDisDetails>{Item = asyncResult.AsyncState as InPatientAdmDisDetails});
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                CanceEditCmd();
                                
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                this.HideBusyIndicator();
                            }
                        }), CurrentAdmission);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        InPatientAdmDisDetails tempObject = null;
        DeptLocation tempDeptLoc = null;

        private void AssignAdmissionGeneralInfo(InPatientAdmDisDetails source, InPatientAdmDisDetails  dest)
        {
            if (source == null || dest == null)
            {
                return;
            }
            dest.AdmissionDate = source.AdmissionDate;
            dest.Department = source.Department;
            dest.V_AdmissionType = source.V_AdmissionType;
            dest.AdmissionNote = source.AdmissionNote;
        }
        public bool CanBeginEditCmd
        {
            get
            {
                return CurrentAdmission != null
                    && CurrentAdmission.InPatientAdmDisDetailID > 0
                    && !IsEditing
                    && !InfoHasChanged;
            }
        }
        public void BeginEditCmd()
        {
            if (Globals.IsLockRegistration(RegLockFlag, "sửa đổi thông tin nhập viện"))
            {
                return;
            }
            IsEditing = true;
            //IsRefDept = false;

            //tempObject = new InPatientAdmDisDetails();
            //AssignAdmissionGeneralInfo(CurrentAdmission,tempObject);

            tempObject = CurrentAdmission.DeepCopy();
            if (SelectedLocation == null)
            {
                ClientLoggerHelper.LogInfo("BeginEditCmd SelectedLocation = NULL");
            }
            else
            {
                ClientLoggerHelper.LogInfo("BeginEditCmd SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
            }
            
            tempDeptLoc = SelectedLocation;
            
            if (CurrentAdmission != null && CurrentAdmission.InPatientAdmDisDetailID <= 0)
            {
                //20191018 TBL: Ngày nhập viện không cần lấy giây
                //CurrentAdmission.AdmissionDate = Globals.ServerDate.Value;
                CurrentAdmission.AdmissionDate = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            }
            
        }
        public bool CanCanceEditCmd
        {
            get
            {
                return CurrentAdmission != null
                    && CurrentAdmission.InPatientAdmDisDetailID > 0
                    && IsEditing;
            }
        }
        public void CanceEditCmd()
        {
            IsEditing = false;
            InfoHasChanged = false;
            if (SelectedLocation == null)
            {
                ClientLoggerHelper.LogInfo("CanceEditCmd 1st SelectedLocation = NULL");
            }
            else
            {
                ClientLoggerHelper.LogInfo("CanceEditCmd 1st SelectedLocation =" + " " + SelectedLocation.Location.LocationName.ToString());
            }
            SelectedLocation = tempDeptLoc;

            
            //AssignAdmissionGeneralInfo(tempObject, CurrentAdmission);
            CurrentAdmission = tempObject.DeepCopy();
            if(CurrentAdmission != null)
            {
                DepartmentContent.SelectedItem = CurrentAdmission.Department;
                //AdmissionReasonContent.SetSelectedID(CurrentAdmission.V_AdmissionType);    
            }
            else
            {
                DepartmentContent.SelectedItem = null;
                //AdmissionReasonContent.SelectedItem = null;
            }

            tempObject = null;
            tempDeptLoc = null;
        }

        protected void CommitEdit()
        {
            IsEditing = false;
            InfoHasChanged = false;
            tempObject = null;
            tempDeptLoc = null;
        }

        public void BeginNewOutPtTreatment()
        {
            Globals.EventAggregator.Publish(new CallBeginNewOutPtTreatment());
        }
        #endregion
        public bool ShowButtons
        {
            get
            {
                if (isRead && _currentAdmission != null && _currentAdmission.InPatientAdmDisDetailID > 0)
                {
                    IsRefDept = false;
                    IsObjectType = false; // #007
                    return true;
                }
                IsRefDept = true && IsEditing;
                IsObjectType = true; // #007
                return  false;
            }
        }

        private int _RegLockFlag = 0;
        public int RegLockFlag
        {
            get
            {
                return _RegLockFlag;
            }
            set
            {
                _RegLockFlag = value;
                NotifyOfPropertyChange(() => RegLockFlag);
            }
        }
        private bool _IsAdmissionFromSuggestion = true;
        public bool IsAdmissionFromSuggestion
        {
            get
            {
                return _IsAdmissionFromSuggestion;
            }
            set
            {
                _IsAdmissionFromSuggestion = value;
                NotifyOfPropertyChange(() => IsAdmissionFromSuggestion);
            }
        }
        //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
        private bool _IsPtNomal = true;
        public bool IsPtNomal
        {
            get
            {
                return _IsPtNomal;
            }
            set
            {
                _IsPtNomal = value;
                NotifyOfPropertyChange(() => IsPtNomal);
            }
        }
        //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
        //▼====: #006
        private bool _EnablePostponementAdvancePayment;
        public bool EnablePostponementAdvancePayment
        {
            get
            {
                return _EnablePostponementAdvancePayment;
            }
            set
            {
                _EnablePostponementAdvancePayment = value;
                NotifyOfPropertyChange(() => EnablePostponementAdvancePayment);
            }
        }
        //▲====: #006

        public void PrintPatientInfoCmd()
        {
            if (CurrentAdmission == null || CurrentAdmission.PatientRegistration == null || CurrentAdmission.PatientRegistration.Patient == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0666_G1_Msg_InfoKhCoTTinBN), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //if (string.IsNullOrWhiteSpace(CurrentAdmission.PatientRegistration.Patient.FileCodeNumber))
            //{
            //    if (MessageBox.Show("Bệnh nhân này không có số hồ sơ. Bạn có muốn in không?", eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            //    {
            //        return;
            //    }
            //}

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 1)
            //{
            //    proAlloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.PatientCode;
            //}
            //else if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 2)
            //{
            //    proAlloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.FileCodeNumber;
            //}
            //else
            //{
            //    if (!string.IsNullOrWhiteSpace(CurrentAdmission.PatientRegistration.Patient.FileCodeNumber))
            //    {
            //        proAlloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.FileCodeNumber;
            //    }
            //    else
            //    {
            //        proAlloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.PatientCode;
            //    }
            //}

            //int MinAge = 2;
            //int Age = 0;
            //string strUnit = "";

            //DateTime DOB = CurrentAdmission.PatientRegistration.Patient.DOB.GetValueOrDefault();
            //bool AgeOnly = CurrentAdmission.PatientRegistration.Patient.AgeOnly.GetValueOrDefault();

            //if (DOB != DateTime.MinValue && DOB <= Globals.GetCurServerDateTime())
            //{
            //    if (AgeOnly || Globals.GetCurServerDateTime().Year - DOB.Year >= MinAge)
            //    {
            //        Age = Globals.GetCurServerDateTime().Year - DOB.Year;
            //        strUnit = eHCMSResources.G2057_G1_Tuoi.ToLower();
            //    }
            //    else
            //    {
            //        Age = (Globals.GetCurServerDateTime().Month + Globals.GetCurServerDateTime().Year * 12) - (DOB.Month + DOB.Year * 12);
            //        strUnit = eHCMSResources.G0039_G1_Th.ToLower();
            //    }

            //    if (Age <= 0)
            //    {
            //        Age = 1;
            //    }
            //}

            //proAlloc.PatientName = CurrentAdmission.PatientRegistration.Patient.FullName;
            //proAlloc.DOB = CurrentAdmission.PatientRegistration.Patient.DOBText;
            //if (Age > 0)
            //{
            //    proAlloc.Age = Age.ToString() + " " + strUnit;
            //}
            //else
            //{
            //    proAlloc.Age = "";
            //}
            //proAlloc.Gender = CurrentAdmission.PatientRegistration.Patient.GenderObj.Name;
            //proAlloc.AdmissionDate = CurrentAdmission.AdmissionDate;
            ///*==== #001 ====*/
            //proAlloc.PatientCode = CurrentAdmission.PatientRegistration.Patient.PatientCode;
            ///*==== #001 ====*/
            //proAlloc.eItem = ReportName.PATIENT_INFO;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            Action<ICommonPreviewView> onInitDlg = (Alloc) =>
            {
                if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 1)
                {
                    Alloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.PatientCode;
                }
                else if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 2)
                {
                    Alloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.FileCodeNumber;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(CurrentAdmission.PatientRegistration.Patient.FileCodeNumber))
                    {
                        Alloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.FileCodeNumber;
                    }
                    else
                    {
                        Alloc.FileCodeNumber = CurrentAdmission.PatientRegistration.Patient.PatientCode;
                    }
                }

                int MinAge = 2;
                int Age = 0;
                string strUnit = "";

                DateTime DOB = CurrentAdmission.PatientRegistration.Patient.DOB.GetValueOrDefault();
                bool AgeOnly = CurrentAdmission.PatientRegistration.Patient.AgeOnly.GetValueOrDefault();

                if (DOB != DateTime.MinValue && DOB <= Globals.GetCurServerDateTime())
                {
                    if (AgeOnly || Globals.GetCurServerDateTime().Year - DOB.Year >= MinAge)
                    {
                        Age = Globals.GetCurServerDateTime().Year - DOB.Year;
                        strUnit = eHCMSResources.G2057_G1_Tuoi.ToLower();
                    }
                    else
                    {
                        Age = (Globals.GetCurServerDateTime().Month + Globals.GetCurServerDateTime().Year * 12) - (DOB.Month + DOB.Year * 12);
                        strUnit = eHCMSResources.G0039_G1_Th.ToLower();
                    }

                    if (Age <= 0)
                    {
                        Age = 1;
                    }
                }

                Alloc.PatientName = CurrentAdmission.PatientRegistration.Patient.FullName;
                Alloc.DOB = CurrentAdmission.PatientRegistration.Patient.DOBText;
                if (Age > 0)
                {
                    Alloc.Age = Age.ToString() + " " + strUnit;
                }
                else
                {
                    Alloc.Age = "";
                }
                Alloc.Gender = CurrentAdmission.PatientRegistration.Patient.GenderObj.Name;
                Alloc.AdmissionDate = CurrentAdmission.AdmissionDate;
                Alloc.PatientCode = CurrentAdmission.PatientRegistration.Patient.PatientCode;
                //▼====: #003
                Alloc.PatientFullAddress = CurrentAdmission.PatientRegistration.Patient.PatientFullStreetAddress;
                //▲====: #003
                //▼====: #005
                if (CurrentAdmission.PatientRegistration.Patient.SuburbName != null && CurrentAdmission.PatientRegistration.Patient.CitiesProvince != null)
                {
                    Alloc.PatientShortAddress = (CurrentAdmission.PatientRegistration.Patient.PatientStreetAddress ?? "") + ", " + CurrentAdmission.PatientRegistration.Patient.SuburbName.SuburbName + ", " + CurrentAdmission.PatientRegistration.Patient.CitiesProvince.CityProvinceName;
                }
                //▲====: #005
                Alloc.eItem = ReportName.PATIENT_INFO;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }



        public void LoadDeptTransferDocTypeReq()
        {
            if (Globals.allDeptTransDocTypeReq != null && Globals.allDeptTransDocTypeReq.Count > 0)
            {
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDocTypeRequire(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DeptTransferDocReq> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllDocTypeRequire(asyncResult);
                                if (Globals.allDeptTransDocTypeReq == null)
                                {
                                    Globals.allDeptTransDocTypeReq = new List<DeptTransferDocReq>(allItems);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0691_G1_Msg_InfoKhTheLayDSTaiLieuYC);
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
        /*▼====: #002*/
        private ObservableCollection<Lookup> _TreatmentTypeCollection;
        public ObservableCollection<Lookup> TreatmentTypeCollection
        {
            get
            {
                return _TreatmentTypeCollection;
            }
            set
            {
                _TreatmentTypeCollection = value;
                NotifyOfPropertyChange("TreatmentTypeCollection");
            }
        }
        private Lookup _SelectedTreatmentType;
        public Lookup SelectedTreatmentType
        {
            get
            {
                return _SelectedTreatmentType;
            }
            set
            {
                _SelectedTreatmentType = value;
                NotifyOfPropertyChange("SelectedTreatmentType");
                CurrentAdmission.IsConfirmEmergencyTreatment = SelectedTreatmentType.LookupID == -1 ? null : (bool?)(SelectedTreatmentType.LookupID == 1);
            }
        }
        /*▲====: #002*/
        public void MedicalFileInfoCmd()
        {
            if (CurrentAdmission == null || CurrentAdmission.PtRegistrationID == 0)
            {
                return;
            }
            ISpecialistTypeSelect DialogView = Globals.GetViewModel<ISpecialistTypeSelect>();
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.ConfirmedSpecialistType == null ||
                (DialogView.ConfirmedSpecialistType.LookupID != (long)AllLookupValues.V_SpecialistType.San && DialogView.ConfirmedSpecialistType.LookupID != (long)AllLookupValues.V_SpecialistType.Nhi))
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                if (DialogView.ConfirmedSpecialistType.LookupID == (long)AllLookupValues.V_SpecialistType.Nhi)
                {
                    aView.eItem = ReportName.PediatricsMedicalFile;
                }
                else
                {
                    aView.eItem = ReportName.ObstetricsMedicalFile;
                }
                aView.RegistrationID = CurrentAdmission.PtRegistrationID;
            });
        }
        public void btnDeNghiTamUng()
        {
            if (CurrentAdmission == null || CurrentAdmission.PtRegistrationID == 0)
            {
                return;
            }
            Globals.PageName = Globals.TitleForm;
            //LeftMenuByPTType = eLeftMenuByPTType.IN_PT;
            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var vm = Globals.GetViewModel<ISuggestCashAdvance>();
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            vm.UsedByTaiVuOffice = true;
            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            vm.SearchRegistrationContent.IsSearchForCashAdvance = true;
            regModule.MainContent = vm;
            ((Conductor<object>)regModule).ActivateItem(vm);
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;

            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>()
            { Item = new PatientRegistration { PtRegistrationID = CurrentAdmission.PtRegistrationID } });
        }
    }
}