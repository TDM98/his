using eHCMSLanguage;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure;
/*
* 20180103 #001 CMN:   Loaded only departments with permissions
* 20191207 #002 TTM:    BM 0019704: [Đề nghị nhập viện] Không hạn chế khoa đề nghị của bác sĩ để bác sĩ khoa Nội được phép đề nghị vào khoa Nhi mặc dù không cấu hình trách nhiệm.
* 20210308 #003 TNHX: 219 Thêm Khoa Khám bệnh trong khi tạo bill viện phí nội trú
* 20220807 #004 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
* + Tạo màn hình, thêm các trường lọc dữ liệu.
* + Thêm trường phòng khám sau khi chọn khoa.
* + Validate các trường lọc.
* + Thêm điều kiện để lấy khoa theo list DeptID.
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDepartmentListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DepartmentListingViewModel : Conductor<object>, IDepartmentListing
    {
        private bool _IsShowOnlyAllowableInTemp = false;
        public bool IsShowOnlyAllowableInTemp
        {
            get
            {
                return _IsShowOnlyAllowableInTemp;
            }
            set
            {
                if (_IsShowOnlyAllowableInTemp != value)
                {
                    _IsShowOnlyAllowableInTemp = value;
                    NotifyOfPropertyChange(() => IsShowOnlyAllowableInTemp);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public DepartmentListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        public void LoadData(long curDeptID = 0, bool bLoadAllDepts = false, bool isPKVVRhm = false, bool isLoadAllDeptsForKHTH = false)
        {
            LoadDepartments(curDeptID, bLoadAllDepts, isPKVVRhm, isLoadAllDeptsForKHTH);
        }

        private ObservableCollection<RefDepartment> _departments;
        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }

        private long _curDeptID;
        public long curDeptID
        {
            get { return _curDeptID; }
            set
            {
                _curDeptID = value;
                NotifyOfPropertyChange(() => curDeptID);
            }
        }

        private ObservableCollection<long> _LstRefDepartment;
        public ObservableCollection<long> LstRefDepartment
        {
            get
            {
                return _LstRefDepartment;
            }
            set
            {
                _LstRefDepartment = value;
                NotifyOfPropertyChange(() => LstRefDepartment);
            }
        }

        public bool SetSelDeptExt { get; set; } = false;

        private RefDepartment _selectedItem;

        public RefDepartment SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                {
                    return;
                }
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public bool AddSelectOneItem { get; set; }

        public bool AddSelectedAllItem { get; set; }

        private void SetDefaultRefDept()
        {
            // TxD 13/01/2015: Added the following to allow SelectedItem being set from outside of this viewmodel
            if (SetSelDeptExt)
            {
                return;
            }
            if (Departments == null || Departments.Count() == 0)
                return;

            long defDeptID = 0;
            if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0)
                defDeptID = Globals.ObjRefDepartment.DeptID;

            var temp = Departments.Where(x => x.DeptID == defDeptID);
            SelectedItem = temp != null && temp.Count() > 0 ? Globals.ObjRefDepartment : Departments.FirstOrDefault();
        }

        private void LoadDepartments(long curDeptID = 0, bool bLoadAllDepts = false, bool isPKVVRhm = false, bool isLoadAllDeptsForKTHT = false)
        {
            if (Globals.AllRefDepartmentList == null || Globals.AllRefDepartmentList.Count == 0)
            {
                return;
            }

            Departments = null;
            ObservableCollection<RefDepartment> loadingDepts = new ObservableCollection<RefDepartment>();

            if (LstRefDepartment != null && LstRefDepartment.Count > 0)
            {
                if (bLoadAllDepts)
                {
                    foreach (var globalDept in Globals.AllRefDepartmentList)
                    {
                        loadingDepts.Add(globalDept);
                    }
                }
                else
                {
                    foreach (var globalDept in Globals.AllRefDepartmentList)
                    {
                        foreach (var authDeptID in LstRefDepartment)
                        {
                            if ((globalDept.V_DeptTypeOperation.HasValue && (globalDept.V_DeptTypeOperation.Value == (long)(V_DeptTypeOperation.KhoaNoi))
                                                                        && globalDept.DeptID == authDeptID)
                                                                        //▼==== #004
                                                                        || (globalDept.DeptID == authDeptID && isPKVVRhm))
                                                                        //▲==== #004
                            {
                                loadingDepts.Add(globalDept);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (curDeptID > 0)
                {
                    /*▼====: #001*/
                    //foreach (var globalDept2 in Globals.AllRefDepartmentList)
                    foreach (var globalDept2 in Globals.AllRefDepartmentList.Where(x => bLoadAllDepts || !Globals.isAccountCheck || Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(x.DeptID)).ToList())
                    /*▲====: #001*/
                    {
                        if (globalDept2.V_DeptTypeOperation.HasValue && (globalDept2.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi)
                                                                    && globalDept2.DeptID == curDeptID)
                        {
                            loadingDepts.Add(globalDept2);
                            break;
                        }
                    }
                }
                //▼===== #010
                else if (IsAdmRequest)
                {
                    //Đề nghị nhập viện không được phép hạn chế khoa bác sĩ khi đề nghị.
                    //Ví dụ: Bác sĩ ở khoa ngoại không được phân quyền trách nhiệm khoa nhi hoặc khoa sản thì vẫn đề nghị vào nhi và sản đc.
                    loadingDepts = new ObservableCollection<RefDepartment>();
                    foreach (var gDept in Globals.AllRefDepartmentList)
                    {
                        if (gDept.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi)
                        {
                            loadingDepts.Add(gDept);
                        }
                    }
                }
                //▲===== #010
                else
                {
                    loadingDepts = new ObservableCollection<RefDepartment>();
                    /*▼====: #001*/
                    //foreach (var gDept in Globals.AllRefDepartmentList)
                    foreach (var gDept in Globals.AllRefDepartmentList.Where(x => bLoadAllDepts || !Globals.isAccountCheck || Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(x.DeptID)).ToList())
                    /*▲====: #001*/
                    {
                        //20200103 TTM: Thêm điều kiện kiểm tra nếu là màn hình chọn khoa để đề nghị chuyển thì không lấy khoa đã xoá.
                        //              Vì anh Tuấn nói để lại để xem các trường hợp cũ. Ở chức năng khác.
                        if (gDept.IsDeleted == true && IsNotShowDeptDeleted == true)
                        {
                            continue;
                        }
                        if (bLoadAllDepts && !isLoadAllDeptsForKTHT)
                        {
                            loadingDepts.Add(gDept);
                        }
                        else if (gDept.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi 
                            || (gDept.DeptID == (long)Globals.ServerConfigSection.Hospitals.KhoaPhongKham && (isPKVVRhm || isLoadAllDeptsForKTHT)))
                        {
                            loadingDepts.Add(gDept);
                        }
                    }
                }
            }

            if (loadingDepts.Count > 0)
            {
                if (AddSelectOneItem)
                {
                    RefDepartment firstItem = new RefDepartment();
                    firstItem.DeptID = 0;
                    firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
                    loadingDepts.Insert(0, firstItem);
                }
                else if (AddSelectedAllItem)
                {
                    RefDepartment firstItem = new RefDepartment();
                    firstItem.DeptID = 0;
                    firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                    loadingDepts.Insert(0, firstItem);
                }
                //▼====: #003
                if (IsShowExaminationDept)
                {
                    RefDepartment ExaminationDept = Globals.AllRefDepartmentList.Where(x => x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).FirstOrDefault();
                    loadingDepts.Add(ExaminationDept);
                }
                //▲====: #003
            }
            if (IsLoadDrugDept)
            {
                Departments = new ObservableCollection<RefDepartment>(loadingDepts.Where(x => !IsShowOnlyAllowableInTemp || x.IsAllowableInTemp).ToList());
            }
            else
            {
                Departments = new ObservableCollection<RefDepartment>(loadingDepts.Where(x => (!IsShowOnlyAllowableInTemp || x.IsAllowableInTemp) && x.DeptID != (long)AllLookupValues.DeptID.KHOA_DUOC).ToList());
            }
            SetDefaultRefDept();
        }

        public void SetSelectedDeptItem(long selDeptID)
        {
            if (Departments == null || Departments.Count() == 0 || selDeptID <= 0)
                return;

            SelectedItem = Departments.FirstOrDefault(item => item.DeptID == selDeptID);
        }

        private bool _IsLoadDrugDept = true;
        public bool IsLoadDrugDept
        {
            get
            {
                return _IsLoadDrugDept;
            }
            set
            {
                if (_IsLoadDrugDept != value)
                {
                    _IsLoadDrugDept = value;
                    NotifyOfPropertyChange(() => IsLoadDrugDept);
                }
            }
        }
        private bool _IsAdmRequest = false;
        public bool IsAdmRequest
        {
            get
            {
                return _IsAdmRequest;
            }
            set
            {
                if (_IsAdmRequest != value)
                {
                    _IsAdmRequest = value;
                    NotifyOfPropertyChange(() => IsAdmRequest);
                }
            }
        }
        private bool _IsNotShowDeptDeleted = false;
        public bool IsNotShowDeptDeleted
        {
            get
            {
                return _IsNotShowDeptDeleted;
            }
            set
            {
                if (_IsNotShowDeptDeleted != value)
                {
                    _IsNotShowDeptDeleted = value;
                    NotifyOfPropertyChange(() => IsNotShowDeptDeleted);
                }
            }
        }

        //▼====: #003
        private bool _IsShowExaminationDept = false;
        public bool IsShowExaminationDept
        {
            get
            {
                return _IsShowExaminationDept;
            }
            set
            {
                if (_IsShowExaminationDept != value)
                {
                    _IsShowExaminationDept = value;
                    NotifyOfPropertyChange(() => IsShowExaminationDept);
                }
            }
        }
        //▲====: #003
    }
}

//public void SetSelectedID(long itemID)
//{
//    if (Departments != null)
//    {
//        RefDepartment foundItem = Departments.FirstOrDefault(item => item.DeptID == itemID);
//        SelectedItem = foundItem;
//    }
//}

//private IEnumerator<IResult> LoadDepartments_TBD()
//{
//    if (LstRefDepartment != null)
//    {
//        var departmentTask = new LoadDepartmentsTask(LstRefDepartment,AddSelectOneItem,AddSelectedAllItem);
//        yield return departmentTask;
//        Departments = departmentTask.Departments;
//        //SelectedItem = Globals.ObjRefDepartment;
//        SetDefaultRefDept(Departments);
//        yield break;
//    }
//    else
//    {
//        var departmentTask = new LoadDepartmentsTask(AddSelectOneItem,AddSelectedAllItem);
//        yield return departmentTask;
//        Departments = departmentTask.Departments;
//        //SelectedItem = Globals.DeptLocation.RefDepartment;
//        SetDefaultRefDept(Departments);
//        yield break;
//    }
//}

//private IEnumerator<IResult> LoadDepartments_TBD(long curDeptID)
//{
//    if (curDeptID > 0)
//    {
//        var departmentTask = new LoadDepartmentsTask(curDeptID, AddSelectOneItem, AddSelectedAllItem);
//        yield return departmentTask;
//        Departments = departmentTask.Departments;
//        SetDefaultRefDept(Departments);
//        //SelectedItem = Globals.DeptLocation.RefDepartment;
//        yield break;
//    }
//}