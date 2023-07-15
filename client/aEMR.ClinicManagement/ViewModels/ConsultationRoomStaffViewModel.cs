using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
using eHCMSLanguage;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IConsultationRoomStaff)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationRoomStaffViewModel : Conductor<object>, IConsultationRoomStaff, IHandle<RoomSelectedEvent>
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

        private long StaffCatType = (long)V_StaffCatType.BacSi;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ConsultationRoomStaffViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            var treeDept = Globals.GetViewModel<IRoomTree>();
            GetAllConsultationTimeSegments();
            RoomTree = treeDept;
            this.ActivateItem(treeDept);
            //Globals.EventAggregator.Subscribe(this);
            //GetRefStaffCategories();
            selectedConsultationRoomTarget=new ConsultationRoomTarget();
            _tempAllStaff=new ObservableCollection<Staff>();
            _memAllStaff=new ObservableCollection<Staff>();
            _allConsultationRoomStaffAllocations=new ObservableCollection<ConsultationRoomStaffAllocations>();
            selectedConsultationRoomStaffAllocations=new ConsultationRoomStaffAllocations();
            selectedTempConsultRoomStaffAlloc=new ConsultationRoomStaffAllocations();
            CurDateTime = selectedConsultationRoomStaffAllocations.CurDate;
            //chon radio bac si dau tien
            _allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            selectedConsultTimeSeg=new ConsultationTimeSegments();
            _allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        public DateTime CurDateTime = DateTime.Now;

        private string _curDate=DateTime.Now.Date.ToShortDateString();
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

        private DateTime _appDate;
        public DateTime appDate
        {
            get
            {
                return _appDate;
            }
            set
            {
                if (_appDate == value)
                    return;
                _appDate = value;
            }
        }

        public object RoomTree { get; set; }

        private DeptLocation _curDeptLocation;
        public DeptLocation curDeptLocation
        {
            get
            {
                return _curDeptLocation;
            }
            set
            {
                if (_curDeptLocation == value)
                    return;
                _curDeptLocation = value;
            }
        }
        
        private RefDepartmentsTree _CurRefDepartmentsTree;
        public RefDepartmentsTree CurRefDepartmentsTree
        {
            get { return _CurRefDepartmentsTree; }
            set
            {
                _CurRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => CurRefDepartmentsTree);
            }

        }
        #region properties
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

        private Staff _SelectedStaffGrid;
        public Staff SelectedStaffGrid
        {
            get
            {
                return _SelectedStaffGrid;
            }
            set
            {
                if (_SelectedStaffGrid == value)
                    return;
                _SelectedStaffGrid = value;
            }
        }

        private ObservableCollection<Staff> _tempAllStaff;
        public ObservableCollection<Staff> tempAllStaff
        {
            get
            {
                return _tempAllStaff;
            }
            set
            {
                if (_tempAllStaff == value)
                    return;
                _tempAllStaff = value;
                NotifyOfPropertyChange(() => tempAllStaff);
            }
        }

        private ObservableCollection<Staff> _memAllStaff;
        public ObservableCollection<Staff> memAllStaff
        {
            get
            {
                return _memAllStaff;
            }
            set
            {
                if (_memAllStaff == value)
                    return;
                _memAllStaff = value;
                NotifyOfPropertyChange(() => memAllStaff);
            }
        }

        private ObservableCollection<Staff> _importStaff;
        public ObservableCollection<Staff> importStaff
        {
            get
            {
                return _importStaff;
            }
            set
            {
                if (_importStaff == value)
                    return;
                _importStaff = value;
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _exportStaff;
        public ObservableCollection<ConsultationRoomStaffAllocations> exportStaff
        {
            get
            {
                return _exportStaff;
            }
            set
            {
                if (_exportStaff == value)
                    return;
                _exportStaff = value;
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _tempConsRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoomStaffAlloc
        {
            get
            {
                return _tempConsRoomStaffAlloc;
            }
            set
            {
                if (_tempConsRoomStaffAlloc == value)
                    return;
                _tempConsRoomStaffAlloc = value;
            }
        }
        
        private ConsultationTimeSegments _selectedConsultTimeSeg;
        public ConsultationTimeSegments selectedConsultTimeSeg
        {
            get
            {
                return _selectedConsultTimeSeg;
            }
            set
            {
                if (_selectedConsultTimeSeg == value)
                    return;
                _selectedConsultTimeSeg = value;
                NotifyOfPropertyChange(() => selectedConsultTimeSeg);
                if (selectedConsultTimeSeg.ConsultationTimeSegmentID > 0)
                {
                    if (allConsulRoomStaffAlloc != null && allConsulRoomStaffAlloc.Count>0)
                    {
                        tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();

                        foreach (var crt in allConsulRoomStaffAlloc)
                        {
                            if (crt.ConsultationTimeSegmentID == selectedConsultTimeSeg.ConsultationTimeSegmentID
                                && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                            {
                                tempConsRoomStaffAlloc.Add(crt);
                            }
                        }    
                    }
                }
                NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _lstConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> lstConsultationTimeSegments
        {
            get
            {
                return _lstConsultationTimeSegments;
            }
            set
            {
                if (_lstConsultationTimeSegments == value)
                    return;
                _lstConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => lstConsultationTimeSegments);
            }
        }

        
        private ConsultationRoomStaffAllocations _selectedConsultationRoomStaffAllocations;
        public ConsultationRoomStaffAllocations selectedConsultationRoomStaffAllocations
        {
            get
            {
                return _selectedConsultationRoomStaffAllocations;
            }
            set
            {
                if (_selectedConsultationRoomStaffAllocations == value)
                    return;
                _selectedConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomStaffAllocations);
            }
        }

        private ConsultationRoomStaffAllocations _selectedTempConsultRoomStaffAlloc;
        public ConsultationRoomStaffAllocations selectedTempConsultRoomStaffAlloc
        {
            get
            {
                return _selectedTempConsultRoomStaffAlloc;
            }
            set
            {
                if (_selectedTempConsultRoomStaffAlloc == value)
                    return;
                _selectedTempConsultRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => selectedTempConsultRoomStaffAlloc);
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsultationRoomStaffAllocations;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsultationRoomStaffAllocations
        {
            get
            {
                return _allConsultationRoomStaffAllocations;
            }
            set
            {
                if (_allConsultationRoomStaffAllocations == value)
                    return;
                _allConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => allConsultationRoomStaffAllocations);
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsulRoomStaffAlloc
        {
            get
            {
                return _allConsulRoomStaffAlloc;
            }
            set
            {
                if (_allConsulRoomStaffAlloc == value)
                    return;
                _allConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => allConsulRoomStaffAlloc);
                GetCurRoomStaffAllocations();
                NotifyOfPropertyChange(()=>curAllConsulRoomStaffAlloc);
            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocations> _curAllConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> curAllConsulRoomStaffAlloc
        {
            get
            {
                return _curAllConsulRoomStaffAlloc;
            }
            set
            {
                if (_curAllConsulRoomStaffAlloc == value)
                    return;
                _curAllConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);

            }
        }
        public void GetCurRoomStaffAllocations()
        {
            curAllConsulRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
            foreach (var cts in lstConsultationTimeSegments)
            {
                if (cts.ConsultationTimeSegmentID < 1)
                    continue;
                bool flag = false;
                if (allConsulRoomStaffAlloc==null||
                    allConsulRoomStaffAlloc.Count<1 )
                {
                    return;
                }
                foreach (var crt in allConsulRoomStaffAlloc)
                {
                    if (crt.ConsultationTimeSegmentID == cts.ConsultationTimeSegmentID
                        && crt.AllocationDate.Date <= CurDateTime.Date
                        && crt.Staff.RefStaffCategory.V_StaffCatType==StaffCatType)
                    {
                        crt.Status = eHCMSResources.G2355_G1_X.ToUpper();
                        flag = true;
                        curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                        break;
                    }
                }
                if (!flag)
                {
                    ConsultationRoomStaffAllocations crt = new ConsultationRoomStaffAllocations();
                    crt.ConsultationTimeSegments = new ConsultationTimeSegments();
                    crt.ConsultationTimeSegments.SegmentName = cts.SegmentName;
                    curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                }
            }
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }

        
        public ObservableCollection<ConsultationRoomStaffAllocations> GetCurRoomStaff(ObservableCollection<ConsultationRoomStaffAllocations> allocationses)
        {
            if (allocationses.Count < 1)
                return null;
            ObservableCollection<ConsultationRoomStaffAllocations> curAllConsul = new ObservableCollection<ConsultationRoomStaffAllocations>();
            int i = 0;
            while (i < allocationses.Count)
            {
                int j = i + 1;
                ConsultationRoomStaffAllocations temp = allocationses[i];
                temp.StaffList = temp.Staff.FullName;
                while (j < allocationses.Count)
                {
                    if (allocationses[i].ConsultationTimeSegmentID == allocationses[j].ConsultationTimeSegmentID
                        && allocationses[i].AllocationDate.Date == allocationses[j].AllocationDate.Date
                        && allocationses[i].Staff.RefStaffCategory.V_StaffCatType == allocationses[j].Staff.RefStaffCategory.V_StaffCatType)
                    {
                        temp.StaffList += ", "+allocationses[j].Staff.FullName;
                        allocationses.RemoveAt(j);
                    }else
                    {
                        j++;    
                    }
                }   
                curAllConsul.Add(temp);
                i++;
            }
            
            return curAllConsul;
        }

        private ObservableCollection<ConsultationRoomTarget> _allConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> allConsultationRoomTarget
        {
            get
            {
                return _allConsultationRoomTarget;
            }
            set
            {
                if (_allConsultationRoomTarget == value)
                    return;
                _allConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => allConsultationRoomTarget);
                //Kiem tra chi tieu hien tai
                GetCurTimeSegment();
                NotifyOfPropertyChange(() => curAllConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _curAllConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> curAllConsultationRoomTarget
        {
            get
            {
                return _curAllConsultationRoomTarget;
            }
            set
            {
                if (_curAllConsultationRoomTarget == value)
                    return;
                _curAllConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => curAllConsultationRoomTarget);
            }
        }
        public void GetCurTimeSegment()
        {
            
        }

        private ConsultationRoomTarget _selectedConsultationRoomTarget;
        public ConsultationRoomTarget selectedConsultationRoomTarget
        {
            get
            {
                return _selectedConsultationRoomTarget;
            }
            set
            {
                if (_selectedConsultationRoomTarget == value)
                    return;
                _selectedConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomTarget);

            }
        }
        #endregion

        private bool isDoctor = true;
        //---add loai nhan vien vao combobox
        public void GetRefStaffCategories()
        {
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            GetRefStaffCategoriesByType((long)V_StaffCatType.PhuTa);
        }
        public void radBacSi_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long) V_StaffCatType.BacSi;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            isDoctor = true;
            tempAllStaff = new ObservableCollection<Staff>();
            memAllStaff=new ObservableCollection<Staff>();
            allStaff=new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public void radTroLy_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.PhuTa;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            isDoctor = false;
            allStaff = new ObservableCollection<Staff>();
            tempAllStaff=new ObservableCollection<Staff>();
            memAllStaff=new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public ObservableCollection<ConsultationRoomStaffAllocations> GetRoomStaffAlloByType(long StaffCatType)
        {
            ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoom = new ObservableCollection<ConsultationRoomStaffAllocations>();
            if (allConsulRoomStaffAlloc!=null)
            {
                foreach (var RoomStaffAllocation in allConsulRoomStaffAlloc)
                {
                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                    {
                        //tempAllStaff.Add(RoomStaffAllocation.Staff);
                        //memAllStaff.Add(RoomStaffAllocation.Staff);
                        tempConsRoom.Add(RoomStaffAllocation);
                    }
                }    
            }
            return tempConsRoom;
        }
        public bool CheckValidRoomStaBff(ConsultationRoomStaffAllocations selectedConsultationRoomStaffAllocations)
        {
            if (selectedConsultationRoomStaffAllocations.ConsultationTimeSegments==null
                || selectedConsultationRoomStaffAllocations.ConsultationTimeSegments.ConsultationTimeSegmentID < 1)
            {
                Globals.ShowMessage(eHCMSResources.Z1769_G1_ChuaChonCaKham, "");
                return false;
            }
            if (selectedConsultationRoomStaffAllocations.DeptLocationID < 1)
            {
                Globals.ShowMessage(eHCMSResources.Z1770_G1_ChuaChonPK, "");
                return false;
            }
            if (selectedConsultationRoomStaffAllocations.AllocationDate == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1771_G1_ChuaChonTGianPBo, "");
                return false;
            }
            
            return true;
        }
        
        public void butGetAll()
        {
            tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
        }

        public void cboTimeSegment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCurTimeSegmentChoice();
        }
        public void GetCurTimeSegmentChoice()
        {
            if (selectedConsultTimeSeg.ConsultationTimeSegmentID > 0)
            {
                if (allConsulRoomStaffAlloc != null && allConsulRoomStaffAlloc.Count > 0)
                {
                    tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();

                    foreach (var crt in allConsulRoomStaffAlloc)
                    {
                        if (crt.ConsultationTimeSegmentID == selectedConsultTimeSeg.ConsultationTimeSegmentID
                            && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                        {
                            tempConsRoomStaffAlloc.Add(crt);
                        }
                    }
                    NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
                }
            }
        }

        public void grdListStaffDoubleClick(object sender)
        {
            if (!checkExist(SelectedStaff))
            {
                return;
            }
            tempAllStaff.Add(SelectedStaff);
        }
        public void butLuu()
        {
            if (!CheckValidRoomStaBff(selectedConsultationRoomStaffAllocations))
            {
                return;
            }
            //Kiem tra ngay
            //Neu trung ngay la update
            //Khac ngay la them moi
            if (selectedConsultationRoomStaffAllocations.AllocationDate.Date != appDate)
            {
                selectedConsultationRoomTarget.ConsultationTimeSegmentID =
                    selectedConsultationRoomStaffAllocations.ConsultationTimeSegments.ConsultationTimeSegmentID;
                ObservableCollection<ConsultationRoomStaffAllocations> lstCRSA = new ObservableCollection<ConsultationRoomStaffAllocations>();
                foreach (var sta in tempAllStaff)
                {
                    ConsultationRoomStaffAllocations cRSA = new ConsultationRoomStaffAllocations();
                    cRSA.DeptLocationID = selectedConsultationRoomTarget.DeptLocationID;
                    cRSA.ConsultationTimeSegmentID = selectedConsultationRoomTarget.ConsultationTimeSegmentID;

                    cRSA.StaffID = sta.StaffID;
                    cRSA.StaffCatgID = (int)sta.StaffCatgID;
                    cRSA.AllocationDate = selectedConsultationRoomStaffAllocations.AllocationDate;
                    cRSA.IsActive = true;

                    lstCRSA.Add(cRSA);
                }
                InsertConsultationRoomStaffAllocationsXML(lstCRSA);
            }
            else
            {
                importStaff = new ObservableCollection<Staff>();
                exportStaff = new ObservableCollection<ConsultationRoomStaffAllocations>();
                importStaff = ObjectCopier.DeepCopy(tempAllStaff);

                long typeStaffCat = isDoctor ? (long)V_StaffCatType.BacSi : (long)V_StaffCatType.PhuTa;

                foreach (var mem in allConsultationRoomStaffAllocations)
                {
                    if (mem.Staff.RefStaffCategory.V_StaffCatType == typeStaffCat)
                    {
                        bool flag = false;
                        foreach (var temp in tempAllStaff)
                        {
                            if (temp.StaffID == mem.StaffID)
                            {
                                importStaff.Remove(temp);
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            exportStaff.Add(mem);
                        }
                    }

                }
                //Them moi
                if (importStaff != null)
                {
                    //Kiem tra xem nhan vien nao bi delete, nhan vien nao them vao
                    ObservableCollection<ConsultationRoomStaffAllocations> lstCRSA1 = new ObservableCollection<ConsultationRoomStaffAllocations>();
                    foreach (var sta in importStaff)
                    {
                        //InsertConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                        //                              , selectedConsultationRoomTarget.ConsultationTimeSegmentID
                        //                              , sta.StaffID
                        //                              , (long)sta.StaffCatgID
                        //                              , (DateTime)selectedConsultationRoomTarget.TargetDate);
                        ConsultationRoomStaffAllocations cRSA = new ConsultationRoomStaffAllocations();
                        cRSA.DeptLocationID = selectedConsultationRoomStaffAllocations.DeptLocationID;
                        cRSA.ConsultationTimeSegmentID = selectedConsultationRoomStaffAllocations.ConsultationTimeSegmentID;

                        cRSA.StaffID = sta.StaffID;
                        cRSA.StaffCatgID = (int)sta.StaffCatgID;
                        cRSA.AllocationDate = selectedConsultationRoomStaffAllocations.AllocationDate;
                        cRSA.IsActive = true;

                        lstCRSA1.Add(cRSA);
                    }
                    //Insert o day
                    InsertConsultationRoomStaffAllocationsXML(lstCRSA1);
                }

                //Xoa di
                if (exportStaff != null)
                {
                    //Kiem tra xem nhan vien nao bi delete, nhan vien nao them vao
                    ObservableCollection<ConsultationRoomStaffAllocations> lstCRSA2 = new ObservableCollection<ConsultationRoomStaffAllocations>();
                    foreach (var sta in exportStaff)
                    {
                        //UpdateConsultationRoomStaffAllocations(sta.ConsultationRoomStaffAllocID
                        //                              , false);

                        ConsultationRoomStaffAllocations cRSA = new ConsultationRoomStaffAllocations();
                        cRSA.ConsultationRoomStaffAllocID = sta.ConsultationRoomStaffAllocID;
                        cRSA.IsActive = false;

                        lstCRSA2.Add(cRSA);
                    }
                    UpdateConsultationRoomStaffAllocationsXML(lstCRSA2);
                }
            }

        }
        public void butReset()
        {
            //15072018 TTM
            //1 object dc binding thi khong the bi null.
            if (CurRefDepartmentsTree != null)
            {
                tempAllStaff.Clear();
                tempAllStaff = ObjectCopier.DeepCopy(memAllStaff);
                selectedConsultationRoomStaffAllocations = new ConsultationRoomStaffAllocations();
                selectedConsultationRoomStaffAllocations.DeptLocationID = CurRefDepartmentsTree.NodeID;
            }
            else
            {
                tempAllStaff.Clear();
            }
        }

        private DatePicker dtTargetDay { get; set; }
        public void dtTargetDay_OnLoaded(object sender,RoutedEventArgs e)
        {
            dtTargetDay = sender as DatePicker;
        }

        public void DoubleClick(object sender)
        {
            if (selectedTempConsultRoomStaffAlloc.isEdit)
            {
                appDate = selectedTempConsultRoomStaffAlloc.AllocationDate;
                selectedConsultationRoomStaffAllocations= selectedTempConsultRoomStaffAlloc;
                tempAllStaff=new ObservableCollection<Staff>();
                memAllStaff = new ObservableCollection<Staff>();
                allConsultationRoomStaffAllocations=new ObservableCollection<ConsultationRoomStaffAllocations>();
                foreach (var temp in CurRefDepartmentsTree.LstConsultationRoomStaffAllocations.ToObservableCollection())
                {
                    if (temp.ConsultationTimeSegments.ConsultationTimeSegmentID == selectedTempConsultRoomStaffAlloc.ConsultationTimeSegments.ConsultationTimeSegmentID
                        && temp.AllocationDate.Date == selectedTempConsultRoomStaffAlloc.AllocationDate.Date)
                    {
                        allConsultationRoomStaffAllocations.Add(temp);
                        tempAllStaff.Add(temp.Staff);
                        memAllStaff.Add(temp.Staff);
                    }
                }
            }
                
            //;
        }
        public void lnkDeleteClick(object sender)
        {
            if (MessageBox.Show(eHCMSResources.A0169_G1_Msg_ConfXoaPhanBo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            DeleteConsultationRoomStaffAllocations(selectedTempConsultRoomStaffAlloc.DeptLocationID
                                            , selectedTempConsultRoomStaffAlloc.ConsultationTimeSegmentID
                                            , selectedTempConsultRoomStaffAlloc.AllocationDate);
        }
        #region method
        public void grdListTarget_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ConsultationRoomStaffAllocations objRows = e.Row.DataContext as ConsultationRoomStaffAllocations;
            if (objRows != null)
            {
                switch (objRows.isEdit)
                {
                    case true:
                        {
                            e.Row.Background = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case false:
                        {
                            e.Row.Background = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                }
            }
        }

        private void InsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationTimeSegments(SegmentName, SegmentDescription, StartTime,
                                EndTime, null, null, IsActive, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     
                                     var results = contract.EndInsertConsultationTimeSegments(asyncResult);
                                     if (results )
                                     {
                                         
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     IsLoading = false;
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

        private void GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRefStaffCategoriesByType(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (allRefStaffCategory == null)
                                {
                                    //allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
                                }
                                else
                                {
                                    //allRefStaffCategory.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefStaffCategory.Add(p);
                                }

                                NotifyOfPropertyChange(() => allRefStaffCategory);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void GetAllStaff(long StaffCatgID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

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
                                if (allStaff == null)
                                {
                                    allStaff = new ObservableCollection<Staff>();
                                }
                                else
                                {
                                    allStaff.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allStaff.Add(p);
                                }

                                NotifyOfPropertyChange(() => allStaff);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void GetAllConsultationTimeSegments()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllConsultationTimeSegments(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllConsultationTimeSegments(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                lstConsultationTimeSegments = new ObservableCollection<ConsultationTimeSegments>();
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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
        private void DeleteConsultationRoomStaffAllocations(long DeptLocationID
                                                                , long ConsultationTimeSegmentID
                                                                , DateTime AllocationDate)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConsultationRoomStaffAllocations(DeptLocationID 
                                                                , ConsultationTimeSegmentID 
                                                                , AllocationDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndDeleteConsultationRoomStaffAllocations(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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
        private void InsertConsultationRoomTarget(ConsultationRoomTarget curConsultationRoomTarget)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomTarget(curConsultationRoomTarget, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                                        
                            var results = contract.EndInsertConsultationRoomTarget(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK,"");
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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
        private void GetConsultationRoomTargetByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetByDeptID(DeptLocationID
                        ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetConsultationRoomTargetByDeptID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                results[0].DeptLocationID = curDeptLocation.DeptLocationID;
                                selectedConsultationRoomTarget = results[0];
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetTimeSegment(DeptLocationID,ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetConsultationRoomTargetTimeSegment(asyncResult);
                                //if (results != null && results.Count > 0)
                                //{
                                //    allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                //    foreach (var ConsultRoomTarget in results)
                                //    {
                                //        allConsultationRoomTarget.Add(ConsultRoomTarget);
                                //        if (ConsultRoomTarget.TargetDate <= DateTime.Now)
                                //        {
                                //            selectedConsultationRoomTarget.TargetDate =ConsultRoomTarget.TargetDate;
                                //            selectedConsultationRoomTarget.TargetNumberOfCases = ConsultRoomTarget.TargetNumberOfCases;
                                //            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                //        }
                                //    }
                                //}
                                if (results != null && results.Count > 0)
                                {
                                    NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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


        private void InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomStaffAllocations( DeptLocationID, ConsultationTimeSegmentID
                                                  , StaffID, StaffCatgID, AllocationDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                
                                var results = contract.EndInsertConsultationRoomStaffAllocations(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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

        private void InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomStaffAllocationsXML(lstCRSA, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertConsultationRoomStaffAllocationsXML(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                //                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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


        private void UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomStaffAllocationsXML(lstCRSA, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndUpdateConsultationRoomStaffAllocationsXML(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                //                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                
                                var results = contract.EndUpdateConsultationRoomStaffAllocations(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                    Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                    //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                    //                    ,selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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
        private void GetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                allConsultationRoomStaffAllocations = new ObservableCollection<ConsultationRoomStaffAllocations>();
                                var results = contract.EndGetConsultationRoomStaffAllocations(asyncResult);
                                if (results != null )
                                {
                                    
                                    tempAllStaff=new ObservableCollection<Staff>();
                                    memAllStaff=new ObservableCollection<Staff>();
                                    allConsultationRoomStaffAllocations.Clear();
                                    tempAllStaff.Clear();
                                    if(results.Count>0 )
                                    {
                                        appDate = results[0].AllocationDate;
                                        selectedConsultationRoomStaffAllocations = results[0];
                                        foreach (var RoomStaffAllocation in results)
                                        {
                                            if (RoomStaffAllocation.AllocationDate.ToShortDateString() == appDate.ToShortDateString())
                                            {
                                                allConsultationRoomStaffAllocations.Add(RoomStaffAllocation);
                                                if (isDoctor)
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.BacSi)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                                else
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.PhuTa)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //selectedConsultationRoomStaffAllocations.AllocationDate = DBNull.Value;
                                    }
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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

#region subcribe
        public void Handle(RoomSelectedEvent Obj)
        {
            if (Obj != null)
            {
                selectedConsultationRoomTarget=new ConsultationRoomTarget();
                selectedConsultationRoomStaffAllocations=new ConsultationRoomStaffAllocations();
                
                CurRefDepartmentsTree= (RefDepartmentsTree)Obj.curDeptLoc;
                allConsultationRoomTarget = CurRefDepartmentsTree.LstConsultationRoomTarget.ToObservableCollection();
                selectedConsultationRoomTarget.DeptLocationID = CurRefDepartmentsTree.NodeID;
                selectedConsultationRoomStaffAllocations.DeptLocationID = CurRefDepartmentsTree.NodeID;
                //lay ra danh sach da filter theo ca
                allConsulRoomStaffAlloc = GetCurRoomStaff(CurRefDepartmentsTree.LstConsultationRoomStaffAllocations.ToObservableCollection());
                tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
                tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
                NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
                
                GetCurTimeSegmentChoice();
                curDeptLocation=new DeptLocation();
                curDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
                curDeptLocation.Location=new Location();
                curDeptLocation.Location.LocationName= CurRefDepartmentsTree.NodeText;
                tempAllStaff.Clear();
                //if (selectedConsultationRoomTarget.DeptLocationID>0)
                //{
                //    
                //    GetConsultationRoomTargetByDeptID(selectedConsultationRoomTarget.DeptLocationID);
                //    //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                //    //    ,selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                //}
            }
        }



#endregion

#region   animator

        private bool isOnGrid = false;
        Point midRec=new Point(0,0);

        public Grid LayoutRoot { get; set; }
        public StackPanel ChildRec { get; set; }
        public TranslateTransform RecTranslateTransform { get; set; }
        public void initGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        public void removeGrid()
        {
            try
            {
                List<UIElement> removedItems = new List<UIElement>();
                foreach (UIElement child in LayoutRoot.Children)
                {
                    if (child is StackPanel)
                        removedItems.Add(child);
                }
                foreach (var removedItem in removedItems)
                {
                    LayoutRoot.Children.Remove(removedItem);
                }    
            }
            catch(Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }
        Point ply=new Point(0,0);
        Point plys = new Point(0, 0);

        
        public void LayoutRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot = sender as Grid;
            LayoutRoot.MouseMove+=new MouseEventHandler(LayoutRoot_MouseMove);
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            ply = new Point(LayoutRoot.RenderTransformOrigin.X + LayoutRoot.RenderSize.Width
                , LayoutRoot.RenderTransformOrigin.Y + LayoutRoot.RenderSize.Height);
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(LayoutRoot as UIElement);
                this.RecTranslateTransform.X = PositionClick.X;
                this.RecTranslateTransform.Y = PositionClick.Y;
            }
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(sender as UIElement);
                this.RecTranslateTransform.X = PositionClick.X;
                this.RecTranslateTransform.Y = PositionClick.Y;
                this.ChildRec.ReleaseMouseCapture();
                isChildRecMouseCapture = false;
                ChildRec.Visibility = Visibility.Collapsed;
                if (!isOnGrid)
                {
                    if (PositionClick.X < ply.X)
                    {
                        if (!checkExist(SelectedStaff))
                        {
                            return;
                        }
                        tempAllStaff.Add(SelectedStaff);
                    }
                }
                else
                {
                    if (PositionClick.Y > plys.Y)
                    {
                        tempAllStaff.Remove(SelectedStaffGrid);
                    }
                }
            }
        }
        public void StaffGrid_Loaded(object sender, RoutedEventArgs e)
        {
            plys = new Point(((UIElement)sender).RenderTransformOrigin.X + ((UIElement)sender).RenderSize.Width
                , ((UIElement)sender).RenderTransformOrigin.Y + ((UIElement)sender).RenderSize.Height);
        }
        public void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ply = new Point(((UIElement)sender).RenderTransformOrigin.X + ((UIElement)sender).RenderSize.Width
                , ((UIElement)sender).RenderTransformOrigin.Y + ((UIElement)sender).RenderSize.Height);
        }
        void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(sender as UIElement);
            }
        }
        
        public void ChildRec_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChildRec = sender as StackPanel;
            RecTranslateTransform = ChildRec.FindName("RecTranslateTransform") as TranslateTransform ;
            //ChildRec.MouseLeftButtonUp+=new MouseButtonEventHandler(ChildRec_MouseLeftButtonUp);
            ChildRec.MouseLeftButtonDown+=new MouseButtonEventHandler(ChildRec_MouseLeftButtonDown);
            ChildRec.MouseMove+=new MouseEventHandler(ChildRec_MouseMove);
            ChildRec.Visibility = Visibility.Collapsed;
        }

        void ChildRec_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                this.RecTranslateTransform.X = PositionClick.X - midRec.X;
                this.RecTranslateTransform.Y = PositionClick.Y-midRec.Y;
                ChildRec.Visibility = Visibility.Visible;
            }
        }

        void ChildRec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                
                this.ChildRec.CaptureMouse();        
                
                isChildRecMouseCapture = true;
                PositionClick = e.GetPosition(LayoutRoot as UIElement);
                this.RecTranslateTransform.X = PositionClick.X-midRec.X;
                this.RecTranslateTransform.Y = PositionClick.Y-midRec.Y;
                ChildRec.MouseMove+=new MouseEventHandler(ChildRec_MouseMove);
            }
            catch(Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }
        
        
        public bool checkExist(Staff st)
        {
            foreach (var curStaff in tempAllStaff)
            {
                if(curStaff.StaffID==st.StaffID)
                {
                    Globals.ShowMessage("Đã có nhân viên này trong phòng khám","");
                    return false;
                }
            }
            return true;
        }
        
        

        public bool isChildRecMouseCapture = false;
        private Point PositionClick;

        public StackPanel imageStaff { get; set; }
        public StackPanel ImageStaffGrid { get; set; }
        
        public void ImageStaff_Loaded(object sender, RoutedEventArgs e)
        {
            imageStaff = sender as StackPanel;
            imageStaff.MouseLeftButtonDown += new MouseButtonEventHandler(imageStaff_MouseLeftButtonDown);
        }

        void imageStaff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isOnGrid = false;
            
            midRec.Y = ((StackPanel) sender).RenderSize.Height/2;
            midRec.X = ((StackPanel)sender).RenderSize.Width / 2;

            ChildRec.DataContext = ((StackPanel)sender).DataContext;
            ChildRec_MouseLeftButtonDown(sender, e);
        }

        public void ImageStaffGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ImageStaffGrid = sender as StackPanel;
            ImageStaffGrid.MouseLeftButtonDown+=new MouseButtonEventHandler(ImageStaffGrid_MouseLeftButtonDown);
        }
        void ImageStaffGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isOnGrid = true;
            
            midRec.Y = ((StackPanel)sender).RenderSize.Height / 2;
            midRec.X = ((StackPanel)sender).RenderSize.Width / 2;
            ChildRec.DataContext = ((StackPanel)sender).DataContext;
            ChildRec_MouseLeftButtonDown(sender, e);
        }

#endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bQuanEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mEdit);
            bQuanAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mAdd);
            bQuanDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mDelete);
            bQuanView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mView);

            bStaffEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mEdit);
            bStaffAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mAdd);
            bStaffDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mDelete);
            bStaffView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mView);
            
        }
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
#endregion
    }
}
