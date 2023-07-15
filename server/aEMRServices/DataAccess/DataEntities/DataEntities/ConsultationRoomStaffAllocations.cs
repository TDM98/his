using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class ConsultationRoomStaffAllocations : NotifyChangedBase
    {
        #region Factory Method

        public ConsultationRoomStaffAllocations()
        {
            AllocationDate = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientClassID"></param>
        /// <param name="patientClassName"></param>
        /// <returns></returns>
        public static ConsultationRoomStaffAllocations CreateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID)
        {
            ConsultationRoomStaffAllocations ConsultationRoomStaffAllocations = new ConsultationRoomStaffAllocations();
            ConsultationRoomStaffAllocations.ConsultationRoomStaffAllocID = ConsultationRoomStaffAllocID;
            
            return ConsultationRoomStaffAllocations;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public DateTime AllocationDate
        {
            get
            {
                return _AllocationDate;
            }
            set
            {
                if (_AllocationDate != value)
                {
                    OnAllocationDateChanging(value);
                    _AllocationDate = value;
                    RaisePropertyChanged("AllocationDate");
                    OnAllocationDateChanged();
                }
            }
        }
        private DateTime _AllocationDate;
        partial void OnAllocationDateChanging(DateTime value);
        partial void OnAllocationDateChanged();

	

        [DataMemberAttribute()]
        public long ConsultationRoomStaffAllocID
        {
            get
            {
                return _ConsultationRoomStaffAllocID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocID != value)
                {
                    OnConsultationRoomStaffAllocIDChanging(value);
                    _ConsultationRoomStaffAllocID = value;
                    RaisePropertyChanged("ConsultationRoomStaffAllocID");
                    OnConsultationRoomStaffAllocIDChanged();
                }
            }
        }
        private long _ConsultationRoomStaffAllocID;
        partial void OnConsultationRoomStaffAllocIDChanging(long value);
        partial void OnConsultationRoomStaffAllocIDChanged();

        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                if (_DeptLocationID != value)
                {
                    OnDeptLocationIDChanging(value);
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                    OnDeptLocationIDChanged();
                }
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();

        [DataMemberAttribute()]
        public long ConsultationTimeSegmentID
        {
            get
            {
                return _ConsultationTimeSegmentID;
            }
            set
            {
                if (_ConsultationTimeSegmentID != value)
                {
                    OnConsultationTimeSegmentIDChanging(value);
                    _ConsultationTimeSegmentID = value;
                    RaisePropertyChanged("ConsultationTimeSegmentID");
                    OnConsultationTimeSegmentIDChanged();
                }
            }
        }
        private long _ConsultationTimeSegmentID;
        partial void OnConsultationTimeSegmentIDChanging(long value);
        partial void OnConsultationTimeSegmentIDChanged();

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public int StaffCatgID
        {
            get
            {
                return _StaffCatgID;
            }
            set
            {
                if (_StaffCatgID != value)
                {
                    OnStaffCatgIDChanging(value);
                    _StaffCatgID = value;
                    RaisePropertyChanged("StaffCatgID");
                    OnStaffCatgIDChanged();
                }
            }
        }
        private int _StaffCatgID;
        partial void OnStaffCatgIDChanging(int value);
        partial void OnStaffCatgIDChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        [DataMemberAttribute()]
        public DateTime CurDate = DateTime.Now;

        [DataMemberAttribute()]
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                OnStatusChanging(value);
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private string _Status = "Chưa Được Phân Bố";
        partial void OnStatusChanging(string value);
        partial void OnStatusChanged();

        [DataMemberAttribute()]
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                OnisEditChanging(value);
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                OnisEditChanged();
            }
        }
        private bool _isEdit;
        partial void OnisEditChanging(bool value);
        partial void OnisEditChanged();

        [DataMemberAttribute()]
        public string StaffList
        {
            get
            {
                return _StaffList;
            }
            set
            {
                OnStaffListChanging(value);
                if (_StaffList == value)
                    return;
                _StaffList = value;
                RaisePropertyChanged("StaffList");
                OnStaffListChanged();
            }
        }
        private string _StaffList;
        partial void OnStaffListChanging(string value);
        partial void OnStaffListChanged();

        #endregion

        #region Navigation Properties


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTC_REL_PTINF_PATIENTC", "ConsultationRoomStaffAllocations")]
        public ObservableCollection<ConsultationRoomStaffAllocations> consultationRoomStaffAllocations
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ConsultationTimeSegments ConsultationTimeSegments
        {
            get
            {
                return _ConsultationTimeSegments;
            }
            set
            {
                if (_ConsultationTimeSegments != value)
                {
                    OnConsultationTimeSegmentsChanging(value);
                    _ConsultationTimeSegments = value;
                    RaisePropertyChanged("ConsultationTimeSegments");
                    OnConsultationTimeSegmentsChanged();
                }
            }
        }
        private ConsultationTimeSegments _ConsultationTimeSegments;
        partial void OnConsultationTimeSegmentsChanging(ConsultationTimeSegments value);
        partial void OnConsultationTimeSegmentsChanged();


        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                if (_Staff != value)
                {
                    OnStaffChanging(value);
                    _Staff = value;
                    RaisePropertyChanged("Staff");
                    OnStaffChanged();
                }
            }
        }
        private Staff _Staff;
        partial void OnStaffChanging(Staff value);
        partial void OnStaffChanged();

        [DataMemberAttribute()]
        public DeptLocation DeptLocation
        {
            get
            {
                return _DeptLocation;
            }
            set
            {
                if (_DeptLocation != value)
                {
                    OnDeptLocationChanging(value);
                    _DeptLocation = value;
                    RaisePropertyChanged("DeptLocation");
                    OnDeptLocationChanged();
                }
            }
        }
        private DeptLocation _DeptLocation;
        partial void OnDeptLocationChanging(DeptLocation value);
        partial void OnDeptLocationChanged();

        [DataMemberAttribute()]
        public RefStaffCategory RefStaffCategory
        {
            get
            {
                return _RefStaffCategory;
            }
            set
            {
                if (_RefStaffCategory != value)
                {
                    OnRefStaffCategoryChanging(value);
                    _RefStaffCategory = value;
                    RaisePropertyChanged("RefStaffCategory");
                    OnRefStaffCategoryChanged();
                }
            }
        }
        private RefStaffCategory _RefStaffCategory;
        partial void OnRefStaffCategoryChanging(RefStaffCategory value);
        partial void OnRefStaffCategoryChanged();
        #endregion

        private List<RefMedicalServiceItem> _ServiceItemCollection;
        [DataMemberAttribute]
        public List<RefMedicalServiceItem> ServiceItemCollection
        {
            get
            {
                return _ServiceItemCollection;
            }
            set
            {
                if (_ServiceItemCollection == value)
                {
                    return;
                }
                _ServiceItemCollection = value;
                RaisePropertyChanged("ServiceItemCollection");
            }
        }

        private long _ConsultationRoomStaffAllocationServiceListID;
        [DataMemberAttribute]
        public long ConsultationRoomStaffAllocationServiceListID
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceListID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceListID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceListID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocationServiceListID");
            }
        }

        public override bool Equals(object obj)
        {
            ConsultationRoomStaffAllocations info = obj as ConsultationRoomStaffAllocations;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ConsultationRoomStaffAllocID == info.ConsultationRoomStaffAllocID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class ConsultationRoomStaffAllocationServiceList : NotifyChangedBase
    {
        private long _ConsultationRoomStaffAllocationServiceListID;
        private string _ConsultationRoomStaffAllocationServiceListTitle;
        public long ConsultationRoomStaffAllocationServiceListID
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceListID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceListID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceListID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocationServiceListID");
            }
        }
        public string ConsultationRoomStaffAllocationServiceListTitle
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceListTitle;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceListTitle == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceListTitle = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocationServiceListTitle");
            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocationService> _ServiceCollection;
        public ObservableCollection<ConsultationRoomStaffAllocationService> ServiceCollection
        {
            get
            {
                return _ServiceCollection;
            }
            set
            {
                if (_ServiceCollection == value)
                {
                    return;
                }
                _ServiceCollection = value;
                RaisePropertyChanged("ServiceCollection");
            }
        }
    }
    public class ConsultationRoomStaffAllocationService : NotifyChangedBase
    {
        private long _ConsultationRoomStaffAllocationServiceID;
        public long ConsultationRoomStaffAllocationServiceID
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocationServiceID");
            }
        }
        private long _ConsultationRoomStaffAllocationServiceListID;
        public long ConsultationRoomStaffAllocationServiceListID
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceListID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceListID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceListID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocationServiceListID");
            }
        }
        private RefMedicalServiceItem _MedicalService;
        public RefMedicalServiceItem MedicalService
        {
            get
            {
                return _MedicalService;
            }
            set
            {
                if (_MedicalService == value)
                {
                    return;
                }
                _MedicalService = value;
                RaisePropertyChanged("MedicalService");
            }
        }
    }
}