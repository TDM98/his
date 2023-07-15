using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Controls;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code, fix error when click button
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IStaffDeptResponsibility)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffDeptResponsibilityViewModel : Conductor<object>, IStaffDeptResponsibility
    {
        private long StaffCatType = (long)V_StaffCatType.BacSi;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffDeptResponsibilityViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            isKhoa = true;
            //LoadDepartments();
            Globals.EventAggregator.Subscribe(this);
            //chon radio bac si dau tien
            _allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            _allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            allStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allTempStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allEditStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allDeletedStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();

            allNewStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allNewStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
            SelectedStaff = new Staff();
            //Coroutine.BeginExecute(DoGetStore_ClinicDept());
            Coroutine.BeginExecute(DoGetStore_ClinicDept(StoreTypeID));
            Load_V_DeptTypeOperation();
            authorization();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public DateTime CurDateTime = DateTime.Now;

        private string _curDate = DateTime.Now.Date.ToShortDateString();
        public string curDate
        {
            get
            {
                return _curDate;
            }
            set
            {
                if (_curDate == value)
                    return;
                _curDate = value;
            }
        }

        #region properties

        private bool _isKhoa = true;
        public bool isKhoa
        {
            get
            {
                return _isKhoa;
            }
            set
            {
                if (_isKhoa == value)
                    return;
                _isKhoa = value;
                NotifyOfPropertyChange(() => isKhoa);
            }
        }

        private bool _isKho;
        public bool isKho
        {
            get
            {
                return _isKho;
            }
            set
            {
                if (_isKho == value)
                    return;
                _isKho = value;
                NotifyOfPropertyChange(() => isKho);
            }
        }

        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        private RefStaffCategory _SelectedRefStaffCategory;
        public RefStaffCategory SelectedRefStaffCategory
        {
            get
            {
                return _SelectedRefStaffCategory;
            }
            set
            {
                if (_SelectedRefStaffCategory == value)
                    return;
                _SelectedRefStaffCategory = value;
                NotifyOfPropertyChange(() => SelectedRefStaffCategory);
                if (SelectedRefStaffCategory != null)
                {
                    GetAllStaff(SelectedRefStaffCategory.StaffCatgID);
                    TxtUserAccountName.Clear();
                }
            }
        }

        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        private ObservableCollection<Staff> _allTempStaff;
        public ObservableCollection<Staff> allTempStaff
        {
            get
            {
                return _allTempStaff;
            }
            set
            {
                if (_allTempStaff == value)
                    return;
                _allTempStaff = value;
                NotifyOfPropertyChange(() => allTempStaff);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
            }
        }


        private StaffDeptResponsibilities _curStaffDeptResponsibilities;
        public StaffDeptResponsibilities curStaffDeptResponsibilities
        {
            get
            {
                return _curStaffDeptResponsibilities;
            }
            set
            {
                if (_curStaffDeptResponsibilities == value)
                    return;
                _curStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => curStaffDeptResponsibilities);
            }
        }

        private StaffDeptResponsibilities _selectedStaffDeptResponsibilities;
        public StaffDeptResponsibilities selectedStaffDeptResponsibilities
        {
            get
            {
                return _selectedStaffDeptResponsibilities;
            }
            set
            {
                if (_selectedStaffDeptResponsibilities == value)
                    return;
                _selectedStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => selectedStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allStaffDeptResponsibilities
        {
            get
            {
                return _allStaffDeptResponsibilities;
            }
            set
            {
                if (_allStaffDeptResponsibilities == value)
                    return;
                _allStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allTempStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allTempStaffDeptResponsibilities
        {
            get
            {
                return _allTempStaffDeptResponsibilities;
            }
            set
            {
                if (_allTempStaffDeptResponsibilities == value)
                    return;
                _allTempStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allTempStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allNewStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allNewStaffDeptResponsibilities
        {
            get
            {
                return _allNewStaffDeptResponsibilities;
            }
            set
            {
                if (_allNewStaffDeptResponsibilities == value)
                    return;
                _allNewStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allNewStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allEditStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allEditStaffDeptResponsibilities
        {
            get
            {
                return _allEditStaffDeptResponsibilities;
            }
            set
            {
                if (_allEditStaffDeptResponsibilities == value)
                    return;
                _allEditStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allEditStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allDeletedStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allDeletedStaffDeptResponsibilities
        {
            get
            {
                return _allDeletedStaffDeptResponsibilities;
            }
            set
            {
                if (_allDeletedStaffDeptResponsibilities == value)
                    return;
                _allDeletedStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allDeletedStaffDeptResponsibilities);
            }
        }

        private ObservableCollection<RefDepartment> _allRefDepartments;
        public ObservableCollection<RefDepartment> allRefDepartments
        {
            get
            {
                return _allRefDepartments;
            }
            set
            {
                if (_allRefDepartments == value)
                    return;
                _allRefDepartments = value;
                NotifyOfPropertyChange(() => allRefDepartments);
            }
        }

        private RefDepartment _curRefDepartments;
        public RefDepartment curRefDepartments
        {
            get
            {
                return _curRefDepartments;
            }
            set
            {
                if (_curRefDepartments == value)
                    return;
                _curRefDepartments = value;
                NotifyOfPropertyChange(() => curRefDepartments);
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _allRefStorageWarehouseLocation;
        public ObservableCollection<RefStorageWarehouseLocation> allRefStorageWarehouseLocation
        {
            get
            {
                return _allRefStorageWarehouseLocation;
            }
            set
            {
                if (_allRefStorageWarehouseLocation != value)
                {
                    _allRefStorageWarehouseLocation = value;
                    NotifyOfPropertyChange(() => allRefStorageWarehouseLocation);
                }
            }
        }

        private RefStorageWarehouseLocation _CurRefStorageWarehouseLocation;
        public RefStorageWarehouseLocation CurRefStorageWarehouseLocation
        {
            get
            {
                return _CurRefStorageWarehouseLocation;
            }
            set
            {
                if (_CurRefStorageWarehouseLocation != value)
                {
                    _CurRefStorageWarehouseLocation = value;
                    NotifyOfPropertyChange(() => CurRefStorageWarehouseLocation);
                }
            }
        }


        private StaffStoreDeptResponsibilities _selectedStaffStoreDeptResponsibilities;
        public StaffStoreDeptResponsibilities selectedStaffStoreDeptResponsibilities
        {
            get
            {
                return _selectedStaffStoreDeptResponsibilities;
            }
            set
            {
                if (_selectedStaffStoreDeptResponsibilities == value)
                    return;
                _selectedStaffStoreDeptResponsibilities = value;
                NotifyOfPropertyChange(() => selectedStaffStoreDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffStoreDeptResponsibilities> _allStaffStoreDeptResponsibilities;
        public ObservableCollection<StaffStoreDeptResponsibilities> allStaffStoreDeptResponsibilities
        {
            get
            {
                return _allStaffStoreDeptResponsibilities;
            }
            set
            {
                if (_allStaffStoreDeptResponsibilities == value)
                    return;
                _allStaffStoreDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allStaffStoreDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffStoreDeptResponsibilities> _allTempStaffStoreDeptResponsibilities;
        public ObservableCollection<StaffStoreDeptResponsibilities> allTempStaffStoreDeptResponsibilities
        {
            get
            {
                return _allTempStaffStoreDeptResponsibilities;
            }
            set
            {
                if (_allTempStaffStoreDeptResponsibilities == value)
                    return;
                _allTempStaffStoreDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allTempStaffStoreDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffStoreDeptResponsibilities> _allNewStaffStoreDeptResponsibilities;
        public ObservableCollection<StaffStoreDeptResponsibilities> allNewStaffStoreDeptResponsibilities
        {
            get
            {
                return _allNewStaffStoreDeptResponsibilities;
            }
            set
            {
                if (_allNewStaffStoreDeptResponsibilities == value)
                    return;
                _allNewStaffStoreDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allNewStaffStoreDeptResponsibilities);
            }
        }

        private ObservableCollection<StaffStoreDeptResponsibilities> _allDeleteStaffStoreDeptResponsibilities;
        public ObservableCollection<StaffStoreDeptResponsibilities> allDeleteStaffStoreDeptResponsibilities
        {
            get
            {
                return _allDeleteStaffStoreDeptResponsibilities;
            }
            set
            {
                if (_allDeleteStaffStoreDeptResponsibilities == value)
                    return;
                _allDeleteStaffStoreDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allDeleteStaffStoreDeptResponsibilities);
            }
        }

        //List V_DeptTypeOperation
        private Lookup _ObjV_DeptTypeOperation;
        public Lookup ObjV_DeptTypeOperation
        {
            get { return _ObjV_DeptTypeOperation; }
            set
            {
                _ObjV_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => ObjV_DeptTypeOperation);

                if (ObjV_DeptTypeOperation.LookupID > 0)
                {
                    LoadData();
                }
                else
                {
                    curRefDepartments = null;
                }
            }
        }

        private ObservableCollection<Lookup> _ListV_DeptTypeOperation;
        public ObservableCollection<Lookup> ListV_DeptTypeOperation
        {
            get { return _ListV_DeptTypeOperation; }
            set
            {
                _ListV_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => ListV_DeptTypeOperation);
            }
        }

        #endregion
        //---add loai nhan vien vao combobox
        public void GetRefStaffCategories()
        {
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            GetRefStaffCategoriesByType((long)V_StaffCatType.PhuTa);
        }

        public void radBacSi_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.BacSi;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
        }

        public void radDangKy_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.NhanVienQuayDangKy;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
        }

        public void radTroLy_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.PhuTa;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
        }

        public void radKyThuat_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.NhanVienKyThuat;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
        }

        public void radDuocSi_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.Duoc;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
        }

        /*▼====: #001*/
        public TextBox TxtUserAccountName { get; set; }

        public void TxtUserAccountName_Loaded(object sender)
        {
            TxtUserAccountName = (TextBox)sender;
        }
        /*▲====: #001*/

        public void txtUserAccountNameLostFocus(object sender)
        {
            butSearch();
        }

        public void butSearch()
        {
            PagingLinq(0, 1000);
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            if (allTempStaff == null || allTempStaff.Count < 1)
                return;
            var ResultAll = from p in allTempStaff.ToObservableCollection()
                            select p;
            List<Staff> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<Staff> ObjCollect)
        {
            allStaff.Clear();
            foreach (Staff item in ObjCollect)
            {
                if (item.UserAccountsName.ToUpper().Contains(TxtUserAccountName.Text.ToUpper()))
                {
                    allStaff.Add(item);
                }
            }
        }

        private IEnumerator<IResult> DoGetStore_ClinicDept(long StoreTypeID)
        {
            //var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, true, false);
            var paymentTypeTask = new LoadStoreListTask(StoreTypeID, false, null, true, false);
            yield return paymentTypeTask;
            allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            if (allRefStorageWarehouseLocation != null)
            {
                CurRefStorageWarehouseLocation = allRefStorageWarehouseLocation.FirstOrDefault();
            }
            yield break;
        }

        public void LoadData()
        {
            Coroutine.BeginExecute(NewLoadDepartments());
        }

        private IEnumerator<IResult> NewLoadDepartments()
        {
            if (ObjV_DeptTypeOperation != null && ObjV_DeptTypeOperation.LookupID > 0)
            {
                var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(ObjV_DeptTypeOperation.LookupID);
                yield return departmentTask;
                allRefDepartments = departmentTask.Departments;

                var itemDefault = new RefDepartment();
                itemDefault.DeptID = -1;
                itemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa);
                allRefDepartments.Insert(0, itemDefault);
                curRefDepartments = itemDefault;

                yield break;

            }
        }

        public void butUpdate()
        {
            if (allNewStaffDeptResponsibilities.Count > 0)
            {
                UpdateStaffDeptResponsibilitiesXML(allNewStaffDeptResponsibilities);
            }
            {
                if (allDeletedStaffDeptResponsibilities.Count > 0)
                {
                    DeleteStaffDeptResponsibilitiesXML(allDeletedStaffDeptResponsibilities);
                }
                allEditStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                foreach (var item in allStaffDeptResponsibilities)
                {
                    if (item.StaffDeptResponsibilitiesID == 0)
                    {
                        continue;
                    }
                    item.Responsibilities_32 = item.GetTotalValue();
                    foreach (var temp in allTempStaffDeptResponsibilities)
                    {
                        if (item.StaffDeptResponsibilitiesID == temp.StaffDeptResponsibilitiesID)
                        {
                            if (!item.CheckEquals(temp))
                            {
                                allEditStaffDeptResponsibilities.Add(item);
                            }
                            continue;
                        }
                    }
                }
                if (allEditStaffDeptResponsibilities.Count > 0)
                {
                    UpdateStaffDeptResponsibilitiesXML(allEditStaffDeptResponsibilities);
                }
            }
        }

        public void butAddNew()
        {
            curStaffDeptResponsibilities = new StaffDeptResponsibilities();
            curStaffDeptResponsibilities.Staff = SelectedStaff;
            curStaffDeptResponsibilities.StaffID = SelectedStaff.StaffID;
            /*▼====: #001*/
            if (curRefDepartments == null || curRefDepartments.DeptID < 1)
            {
                MessageBox.Show("Chưa chọn khoa chịu trách nhiệm cho nhân viên này!");
                return;
            }
            /*▲====: #001*/
            curStaffDeptResponsibilities.RefDepartment = curRefDepartments;
            //allDeletedStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            //GetStaffDeptResponsibilitiesByDeptID(curStaffDeptResponsibilities, false);
            if (allStaffDeptResponsibilities.Count > 0)
            {
                foreach (var item in allStaffDeptResponsibilities)
                {
                    if (curStaffDeptResponsibilities.CheckExist(item))
                    {
                        MessageBox.Show("Đã có cấu hình nhân viên này với khoa này rồi!");
                        return;
                    }
                }
            }
            allStaffDeptResponsibilities.Add(curStaffDeptResponsibilities);
            allNewStaffDeptResponsibilities.Add(curStaffDeptResponsibilities);
        }

        public void btnAddNewAll()
        {
            if (SelectedStaff == null)
            {
                MessageBox.Show("Chưa chọn nhân viên để cấu hình.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (allRefDepartments == null || allRefDepartments.Count <= 0)
            {
                MessageBox.Show("Không có khoa phòng để thêm.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            foreach (RefDepartment dept in allRefDepartments)
            {
                if (dept.DeptID <= 0)
                {
                    continue;
                }

                var ExistsItems = allStaffDeptResponsibilities.Where(x => x.StaffID == SelectedStaff.StaffID && x.DeptID == dept.DeptID);

                //KMx: Nếu khoa đã tồn tại thì không add nữa (26/12/2014 15:55).
                if (ExistsItems.Count() > 0)
                {
                    continue;
                }

                curStaffDeptResponsibilities = new StaffDeptResponsibilities();
                curStaffDeptResponsibilities.Staff = SelectedStaff;
                curStaffDeptResponsibilities.StaffID = SelectedStaff.StaffID;

                curStaffDeptResponsibilities.RefDepartment = dept;

                allStaffDeptResponsibilities.Add(curStaffDeptResponsibilities);
                allNewStaffDeptResponsibilities.Add(curStaffDeptResponsibilities);
            }
        }

        public void butReset()
        {
            allStaffDeptResponsibilities = ObjectCopier.DeepCopy(allTempStaffDeptResponsibilities);
            allDeletedStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allEditStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
            allNewStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
        }

        public void butAddNewKho()
        {
            if (CurRefStorageWarehouseLocation.StoreID < 1)
            {
                MessageBox.Show(eHCMSResources.A0386_G1_Msg_InfoChuaChonKhoChoNhVien);
                return;
            }
            /*▼====: #001*/
            if (SelectedStaff.StaffID < 1)
            {
                MessageBox.Show("Chưa chọn nhân viên");
                return;
            }
            /*▲====: #001*/
            var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
            curStaffStoreDeptResponsibilities.Staff = SelectedStaff;
            curStaffStoreDeptResponsibilities.StaffID = SelectedStaff.StaffID;
            curStaffStoreDeptResponsibilities.RefStorageWarehouseLocation = CurRefStorageWarehouseLocation;
            curStaffStoreDeptResponsibilities.StoreID = CurRefStorageWarehouseLocation.StoreID;
            //curStaffStoreDeptResponsibilities.DeptID = (long)CurRefStorageWarehouseLocation.DeptID;
            if (allStaffStoreDeptResponsibilities != null && allStaffStoreDeptResponsibilities.Count > 0)
            {
                foreach (var item in allStaffStoreDeptResponsibilities)
                {
                    if (curStaffStoreDeptResponsibilities.CheckExist(item))
                    {
                        MessageBox.Show("Đã có cấu hình nhân viên này với kho này rồi!");
                        return;
                    }
                }
            }
            if (allStaffStoreDeptResponsibilities == null)
            {
                allStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
            }
            allStaffStoreDeptResponsibilities.Add(curStaffStoreDeptResponsibilities);
            allNewStaffStoreDeptResponsibilities.Add(curStaffStoreDeptResponsibilities);
        }
        public void butAddAllNewKho()
        {

            /*▼====: #001*/
            if (SelectedStaff.StaffID < 1)
            {
                MessageBox.Show("Chưa chọn nhân viên");
                return;
            }
            /*▲====: #001*/
            foreach (var storage in allRefStorageWarehouseLocation.Where(x=>x.StoreID>0))
            {
                
                var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
                curStaffStoreDeptResponsibilities.Staff = SelectedStaff;
                curStaffStoreDeptResponsibilities.StaffID = SelectedStaff.StaffID;
                curStaffStoreDeptResponsibilities.RefStorageWarehouseLocation = storage;
                curStaffStoreDeptResponsibilities.StoreID = storage.StoreID;
                if (allStaffStoreDeptResponsibilities != null)
                {
                    var ExistsItems = allStaffStoreDeptResponsibilities.Where(x => x.StaffID == SelectedStaff.StaffID && x.StoreID == storage.StoreID);
                    if (ExistsItems.Count() > 0)
                    {
                        continue;
                    }
                }
               
                if (allStaffStoreDeptResponsibilities == null)
                {
                    allStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
                }
                allStaffStoreDeptResponsibilities.Add(curStaffStoreDeptResponsibilities);
                allNewStaffStoreDeptResponsibilities.Add(curStaffStoreDeptResponsibilities);
            }
        }

        public void butUpdateKho()
        {
            if (allNewStaffStoreDeptResponsibilities != null && allNewStaffStoreDeptResponsibilities.Count > 0)
            {
                UpdateStaffStoreDeptResponsibilitiesXML(allNewStaffStoreDeptResponsibilities);
            }
            if (allDeleteStaffStoreDeptResponsibilities != null && allDeleteStaffStoreDeptResponsibilities.Count > 0)
            {
                DeleteStaffStoreDeptResponsibilitiesXML(allDeleteStaffStoreDeptResponsibilities);
            }

        }

        public void butResetKho()
        {
            allStaffStoreDeptResponsibilities = ObjectCopier.DeepCopy(allTempStaffStoreDeptResponsibilities);
            allDeleteStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
            allNewStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
        }

        private DatePicker dtTargetDay { get; set; }
        public void dtTargetDay_OnLoaded(object sender, RoutedEventArgs e)
        {
            dtTargetDay = sender as DatePicker;
        }

        public void DoubleClick(object sender)
        {

        }

        public void radKhoaPhong_Click(object sender)
        {
            isKhoa = true;
            isKho = false;
            TabKhoaPhong.IsSelected = true;
            TabKhoPhong.IsSelected = false;
            allStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
        }

        public void radKho_Click(object sender)
        {
            isKhoa = false;
            isKho = true;
            TabKhoaPhong.IsSelected = false;
            TabKhoPhong.IsSelected = true;
            allStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
        }

        public TabItem TabKhoaPhong { get; set; }

        public TabItem TabKhoPhong { get; set; }

        public void TabKhoaPhong_Loaded(object sender)
        {
            TabKhoaPhong = (TabItem)sender;
        }

        public void TabKhoPhong_Loaded(object sender)
        {
            TabKhoPhong = (TabItem)sender;
        }

        public void grdListStaffDoubleClick(object sender)
        {
            if (isKhoa)
            {
                curStaffDeptResponsibilities = new StaffDeptResponsibilities();
                curStaffDeptResponsibilities.Staff = SelectedStaff;
                curStaffDeptResponsibilities.StaffID = SelectedStaff.StaffID;
                allDeletedStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                allNewStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                GetStaffDeptResponsibilitiesByDeptID(curStaffDeptResponsibilities, false);
            }
            else
            {
                var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
                curStaffStoreDeptResponsibilities.Staff = SelectedStaff;
                curStaffStoreDeptResponsibilities.StaffID = SelectedStaff.StaffID;
                allDeleteStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
                allNewStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();

                GetStaffStoreDeptResponsibilitiesByDeptID(curStaffStoreDeptResponsibilities, false);
            }

        }
        public void lnkDeleteClick(object sender)
        {
            allDeletedStaffDeptResponsibilities.Add(selectedStaffDeptResponsibilities);
            allStaffDeptResponsibilities.Remove(selectedStaffDeptResponsibilities);
            allNewStaffDeptResponsibilities.Remove(selectedStaffDeptResponsibilities);
        }

        public void lnkKhoDeleteClick(object sender)
        {
            allDeleteStaffStoreDeptResponsibilities.Add(selectedStaffStoreDeptResponsibilities);
            allStaffStoreDeptResponsibilities.Remove(selectedStaffStoreDeptResponsibilities);
            allNewStaffStoreDeptResponsibilities.Remove(selectedStaffStoreDeptResponsibilities);
        }

        #region method
        /*▼====: #001*/
        public void Load_V_DeptTypeOperation()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator("Loại Khoa...");
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_DeptTypeOperation, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Lookup> allItems = new ObservableCollection<Lookup>();
                            try
                            {
                                allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                ListV_DeptTypeOperation = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                ListV_DeptTypeOperation.Insert(0, firstItem);

                                ObjV_DeptTypeOperation = firstItem;
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

        private void GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefStaffCategoriesByType(V_StaffCatType, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndGetRefStaffCategoriesByType(asyncResult);
                                 if (results != null && results.Count > 0)
                                 {
                                     allRefStaffCategory = results.ToObservableCollection();
                                     NotifyOfPropertyChange(() => allRefStaffCategory);
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

        public string DecryptAccountName(string encryptString)
        {
            string accountName = "";
            if (encryptString != "")
            {
                string[] arr = encryptString.Split(',');
                foreach (var s in arr)
                {
                    if (accountName == "")
                    {
                        accountName += EncryptExtension.Decrypt(s, Globals.AxonKey, Globals.AxonPass);
                    }
                    else
                    {
                        accountName += ", " + EncryptExtension.Decrypt(s, Globals.AxonKey, Globals.AxonPass);
                    }
                }
            }

            return accountName;
        }

        private void GetAllStaff(long StaffCatgID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllStaff(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    allStaff = new ObservableCollection<Staff>();
                                    allTempStaff = new ObservableCollection<Staff>();

                                    foreach (var p in results)
                                    {
                                        p.UserAccountsName = DecryptAccountName(p.UserAccountsName);
                                        allTempStaff.Add(p);
                                        allStaff.Add(p);
                                    }

                                    NotifyOfPropertyChange(() => allStaff);
                                    NotifyOfPropertyChange(() => allTempStaff);

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

        private void GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStaffDeptResponsibilitiesByDeptID(p, isHis, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetStaffDeptResponsibilitiesByDeptID(asyncResult);
                                allTempStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                                allStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();

                                if (results != null && results.Count > 0)
                                {
                                    foreach (var item in results)
                                    {
                                        allTempStaffDeptResponsibilities.Add(ObjectCopier.DeepCopy(item));
                                        allStaffDeptResponsibilities.Add(ObjectCopier.DeepCopy(item));
                                    }
                                    NotifyOfPropertyChange(() => allTempStaffDeptResponsibilities);
                                    NotifyOfPropertyChange(() => allStaffDeptResponsibilities);
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

        private void GetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStaffStoreDeptResponsibilitiesByDeptID(p, isHis, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetStaffStoreDeptResponsibilitiesByDeptID(asyncResult);
                                allTempStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
                                allStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();

                                if (results != null && results.Count > 0)
                                {
                                    foreach (var item in results)
                                    {
                                        allTempStaffStoreDeptResponsibilities.Add(ObjectCopier.DeepCopy(item));
                                        allStaffStoreDeptResponsibilities.Add(ObjectCopier.DeepCopy(item));
                                    }
                                    NotifyOfPropertyChange(() => allTempStaffStoreDeptResponsibilities);
                                    NotifyOfPropertyChange(() => allTempStaffDeptResponsibilities);
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

        private void InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInsertStaffDeptResponsibilitiesXML(lstStaffDeptRes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndInsertStaffDeptResponsibilitiesXML(asyncResult);
                                if (results)
                                {

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

        private void UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateStaffDeptResponsibilitiesXML(lstStaffDeptRes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndUpdateStaffDeptResponsibilitiesXML(asyncResult);
                                if (results)
                                {
                                    allNewStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                                    allEditStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                var curStaffDeptResSearch = new StaffDeptResponsibilities();
                                curStaffDeptResSearch.Staff = curStaffDeptResponsibilities.Staff;
                                GetStaffDeptResponsibilitiesByDeptID(curStaffDeptResSearch, false);
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

        private void DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteStaffDeptResponsibilitiesXML(lstStaffDeptRes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteStaffDeptResponsibilitiesXML(asyncResult);
                                if (results)
                                {
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


        private void UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateStaffStoreDeptResponsibilitiesXML(lstStaffDeptRes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndUpdateStaffStoreDeptResponsibilitiesXML(asyncResult);
                                if (results)
                                {
                                    allNewStaffStoreDeptResponsibilities = new ObservableCollection<StaffStoreDeptResponsibilities>();
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
                                curStaffStoreDeptResponsibilities.Staff = SelectedStaff;
                                curStaffStoreDeptResponsibilities.StaffID = SelectedStaff.StaffID;
                                GetStaffStoreDeptResponsibilitiesByDeptID(curStaffStoreDeptResponsibilities, false);
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

        private void DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteStaffStoreDeptResponsibilitiesXML(lstStaffDeptRes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteStaffStoreDeptResponsibilitiesXML(asyncResult);
                                if (results)
                                {
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

        //public void LoadDepartments()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Lấy Danh Sách Khoa..." });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            //2 các khoa
        //            contract.BeginRefDepartments_RecursiveByDeptID(2, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndRefDepartments_RecursiveByDeptID(asyncResult);
        //                    if (items != null)
        //                    {
        //                        allRefDepartments = new ObservableCollection<RefDepartment>(items);

        //                        //Item Default
        //                        var itemDefault = new RefDepartments();
        //                        itemDefault.DeptID = -1;
        //                        itemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa);
        //                        allRefDepartments.Insert(0, itemDefault);

        //                        curRefDepartments = itemDefault;
        //                    }
        //                    else
        //                    {
        //                        allRefDepartments = null;
        //                        curRefDepartments = null;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        /*▲====: #001*/
        #endregion

        #region subcribe
        public void Handle(RoomSelectedEvent Obj)
        {
            if (Obj != null)
            {

            }
        }

        #endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bQuanEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
            //                                   , (int)eClinicManagement.mQuanLyPhongKham,
            //                                   (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mEdit);
        }
        #endregion
        #region checking account

        private bool _bQuanEdit = true;
        private bool _bQuanAdd = true;
        private bool _bQuanDelete = true;
        private bool _bQuanView = true;

        private bool _bStaffEdit = true;
        private bool _bStaffAdd = true;
        private bool _bStaffDelete = true;
        private bool _bStaffView = true;

        public bool bQuanEdit
        {
            get
            {
                return _bQuanEdit;
            }
            set
            {
                if (_bQuanEdit == value)
                    return;
                _bQuanEdit = value;
            }
        }

        public bool bQuanAdd
        {
            get
            {
                return _bQuanAdd;
            }
            set
            {
                if (_bQuanAdd == value)
                    return;
                _bQuanAdd = value;
            }
        }

        public bool bQuanDelete
        {
            get
            {
                return _bQuanDelete;
            }
            set
            {
                if (_bQuanDelete == value)
                    return;
                _bQuanDelete = value;
            }
        }

        public bool bQuanView
        {
            get
            {
                return _bQuanView;
            }
            set
            {
                if (_bQuanView == value)
                    return;
                _bQuanView = value;
            }
        }

        public bool bStaffEdit
        {
            get
            {
                return _bStaffEdit;
            }
            set
            {
                if (_bStaffEdit == value)
                    return;
                _bStaffEdit = value;
            }
        }

        public bool bStaffAdd
        {
            get
            {
                return _bStaffAdd;
            }
            set
            {
                if (_bStaffAdd == value)
                    return;
                _bStaffAdd = value;
            }
        }

        public bool bStaffDelete
        {
            get
            {
                return _bStaffDelete;
            }
            set
            {
                if (_bStaffDelete == value)
                    return;
                _bStaffDelete = value;
            }
        }

        public bool bStaffView
        {
            get
            {
                return _bStaffView;
            }
            set
            {
                if (_bStaffView == value)
                    return;
                _bStaffView = value;
            }
        }

        #endregion
        #region binding visibilty
        #endregion

        private long StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT;

        public void radStoreTypeIDDrugDept_Click(object sender, RoutedEventArgs e)
        {
            StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT;
            Coroutine.BeginExecute(DoGetStore_ClinicDept(StoreTypeID));
        }

        public void radStoreTypeIDPharmacy_Click(object sender, RoutedEventArgs e)
        {
            StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_EXTERNAL;
            Coroutine.BeginExecute(DoGetStore_ClinicDept(StoreTypeID));
        }

        public void radStoreTypeIDClinicDept_Click(object sender, RoutedEventArgs e)
        {
            StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_CLINIC;
            Coroutine.BeginExecute(DoGetStore_ClinicDept(StoreTypeID));
        }
    }
}
