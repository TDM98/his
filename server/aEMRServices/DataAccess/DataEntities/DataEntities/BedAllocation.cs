using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20180413 #001 TBLD: Add get set HIBedCode 
*/
namespace DataEntities
{
    public partial class BedAllocation : NotifyChangedBase, IEditableObject
    {
        public BedAllocation()
            : base()
        {

        }

        private BedAllocation _tempBedAllocation;
        public override bool Equals(object obj)
        {
            BedAllocation info = obj as BedAllocation;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.BedAllocationID > 0 && this.BedAllocationID == info.BedAllocationID;
        }
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBedAllocation = (BedAllocation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBedAllocation)
                CopyFrom(_tempBedAllocation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BedAllocation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BedAllocation object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static BedAllocation CreateBedAllocation(String bedLocNumber, long allocationID)
        {
            BedAllocation BedAllocation = new BedAllocation();
            //BedAllocation.BedLocNumber = bedLocNumber;
            //BedAllocation.AllocationID = allocationID;
            return BedAllocation;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long BedAllocationID
        {
            get
            {
                return _BedAllocationID;
            }
            set
            {
                OnBedAllocationIDChanging(value);
                _BedAllocationID = value;
                RaisePropertyChanged("BedAllocationID");
                OnBedAllocationIDChanged();
            }
        }
        private long _BedAllocationID;
        partial void OnBedAllocationIDChanging(long value);
        partial void OnBedAllocationIDChanged();

        [DataMemberAttribute()]
        public int BedQuantity
        {
            get
            {
                return _BedQuantity;
            }
            set
            {
                OnBedQuantityChanging(value);
                _BedQuantity = value;
                RaisePropertyChanged("BedQuantity");
                OnBedQuantityChanged();
            }
        }
        private int _BedQuantity=0;
        partial void OnBedQuantityChanging(int value);
        partial void OnBedQuantityChanged();

        [DataMemberAttribute()]
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                OnStatusChanging(value);
                _Status = value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private int _Status = 0;//0-exist,1-new,2-delete
        partial void OnStatusChanging(int value);
        partial void OnStatusChanged();

        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                OnDeptLocationIDChanging(value);
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
                OnDeptLocationIDChanged();
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();




        [DataMemberAttribute()]
        public string BedNumber
        {
            get
            {
                return _BedNumber;
            }
            set
            {
                OnBedNumberChanging(value);
                _BedNumber = value;
                RaisePropertyChanged("BedNumber");
                OnBedNumberChanged();
            }
        }
        private string _BedNumber;
        partial void OnBedNumberChanging(string value);
        partial void OnBedNumberChanged();




        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private long _MedServiceID;
        partial void OnMedServiceIDChanging(long value);
        partial void OnMedServiceIDChanged();




        [DataMemberAttribute()]
        public long V_BedLocType
        {
            get
            {
                return _V_BedLocType;
            }
            set
            {
                OnV_BedLocTypeChanging(value);
                _V_BedLocType = value;
                RaisePropertyChanged("V_BedLocType");
                OnV_BedLocTypeChanged();
            }
        }
        private long _V_BedLocType;
        partial void OnV_BedLocTypeChanging(long value);
        partial void OnV_BedLocTypeChanged();




        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        /*▼====: #001*/
        [DataMemberAttribute()]
        public string HIBedCode
        {
            get
            {
                return _HIBedCode;
            }
            set
            {
                OnHIBedCodeChanging(value);
                _HIBedCode = value;
                RaisePropertyChanged("HIBedCode");
                OnHIBedCodeChanged();
            }
        }
        private string _HIBedCode;
        partial void OnHIBedCodeChanging(string value);
        partial void OnHIBedCodeChanged();
        /*▲====: #001*/



        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public DeptMedServiceItems VDeptMedServiceItems
        {
            get
            {
                return _VDeptMedServiceItems;
            }
            set
            {
                OnVDeptMedServiceItemsChanging(value);
                _VDeptMedServiceItems = value;
                RaisePropertyChanged("VDeptMedServiceItems");
                OnVDeptMedServiceItemsChanged();
            }
        }
        private DeptMedServiceItems _VDeptMedServiceItems;
        partial void OnVDeptMedServiceItemsChanging(DeptMedServiceItems value);
        partial void OnVDeptMedServiceItemsChanged();

        
        [DataMemberAttribute()]
        public DeptLocation VDeptLocation
        {
            get
            {
                return _VDeptLocation;
            }
            set
            {
                OnVDeptLocationChanging(value);
                _VDeptLocation = value;
                RaisePropertyChanged("VDeptLocation");
                OnVDeptLocationChanged();
            }
        }
        private DeptLocation _VDeptLocation;
        partial void OnVDeptLocationChanging(DeptLocation value);
        partial void OnVDeptLocationChanged();

        [DataMemberAttribute()]
        public Lookup VBedLocType
        {
            get
            {
                return _VBedLocType;
            }
            set
            {
                OnVBedLocTypeChanging(value);
                _VBedLocType = value;
                RaisePropertyChanged("VBedLocType");
                OnVBedLocTypeChanged();
            }
        }
        private Lookup _VBedLocType;
        partial void OnVBedLocTypeChanging(Lookup value);
        partial void OnVBedLocTypeChanged();

        [DataMemberAttribute()]
        public RoomType VRoomType
        {
            get
            {
                return _VRoomType;
            }
            set
            {
                OnVRoomTypeChanging(value);
                _VRoomType = value;
                RaisePropertyChanged("VRoomType");
                OnVRoomTypeChanged();
            }
        }
        private RoomType _VRoomType;
        partial void OnVRoomTypeChanging(RoomType value);
        partial void OnVRoomTypeChanged();

        [DataMemberAttribute()]
        public RefMedicalServiceItem VRefMedicalServiceItem
        {
            get
            {
                return _VRefMedicalServiceItem;
            }
            set
            {
                OnVRefMedicalServiceItemChanging(value);
                _VRefMedicalServiceItem = value;
                RaisePropertyChanged("VRefMedicalServiceItem");
                OnVRefMedicalServiceItemChanged();
            }
        }
        private RefMedicalServiceItem _VRefMedicalServiceItem;
        partial void OnVRefMedicalServiceItemChanging(RefMedicalServiceItem value);
        partial void OnVRefMedicalServiceItemChanged();

        [DataMemberAttribute()]
        public int TotalRecord
        {
            get
            {
                return _TotalRecord;
            }
            set
            {
                OnTotalRecordChanging(value);
                _TotalRecord = value;
                RaisePropertyChanged("TotalRecord");
                OnTotalRecordChanged();
            }
        }
        private int _TotalRecord;
        partial void OnTotalRecordChanging(int value);
        partial void OnTotalRecordChanged();

        [DataMemberAttribute()]
        public string BAGuid
        {
            get
            {
                return _BAGuid;
            }
            set
            {
                OnBAGuidChanging(value);
                _BAGuid = value;
                RaisePropertyChanged("BAGuid");
                OnBAGuidChanged();
            }
        }
        private string _BAGuid;
        partial void OnBAGuidChanging(string value);
        partial void OnBAGuidChanged();

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
