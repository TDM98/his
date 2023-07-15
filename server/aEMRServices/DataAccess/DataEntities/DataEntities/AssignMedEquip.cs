using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class AssignMedEquip : NotifyChangedBase, IEditableObject
    {
        public AssignMedEquip()
            : base()
        {

        }

        private AssignMedEquip _tempAssignMedEquip;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAssignMedEquip = (AssignMedEquip)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAssignMedEquip)
                CopyFrom(_tempAssignMedEquip);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(AssignMedEquip p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new AssignMedEquip object.

        /// <param name="assignMedEquipID">Initial value of the AssignMedEquipID property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static AssignMedEquip CreateAssignMedEquip(long assignMedEquipID, long allocationID)
        {
            AssignMedEquip assignMedEquip = new AssignMedEquip();
            assignMedEquip.AssignMedEquipID = assignMedEquipID;
            assignMedEquip.AllocationID = allocationID;
            return assignMedEquip;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long AssignMedEquipID
        {
            get
            {
                return _AssignMedEquipID;
            }
            set
            {
                if (_AssignMedEquipID != value)
                {
                    OnAssignMedEquipIDChanging(value);
                    _AssignMedEquipID = value;
                    RaisePropertyChanged("AssignMedEquipID");
                    OnAssignMedEquipIDChanged();
                }
            }
        }
        private long _AssignMedEquipID;
        partial void OnAssignMedEquipIDChanging(long value);
        partial void OnAssignMedEquipIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> OutwardRscrID
        {
            get
            {
                return _OutwardRscrID;
            }
            set
            {
                OnOutwardRscrIDChanging(value);
                _OutwardRscrID = value;
                RaisePropertyChanged("OutwardRscrID");
                OnOutwardRscrIDChanged();
            }
        }
        private Nullable<long> _OutwardRscrID;
        partial void OnOutwardRscrIDChanging(Nullable<long> value);
        partial void OnOutwardRscrIDChanged();

        [DataMemberAttribute()]
        public long AllocationID
        {
            get
            {
                return _AllocationID;
            }
            set
            {
                OnAllocationIDChanging(value);
                _AllocationID = value;
                RaisePropertyChanged("AllocationID");
                OnAllocationIDChanged();
            }
        }
        private long _AllocationID;
        partial void OnAllocationIDChanging(long value);
        partial void OnAllocationIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> InwardRscrID
        {
            get
            {
                return _InwardRscrID;
            }
            set
            {
                OnInwardRscrIDChanging(value);
                _InwardRscrID = value;
                RaisePropertyChanged("InwardRscrID");
                OnInwardRscrIDChanged();
            }
        }
        private Nullable<long> _InwardRscrID;
        partial void OnInwardRscrIDChanging(Nullable<long> value);
        partial void OnInwardRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> InDate
        {
            get
            {
                return _InDate;
            }
            set
            {
                OnInDateChanging(value);
                _InDate = value;
                RaisePropertyChanged("InDate");
                OnInDateChanged();
            }
        }
        private Nullable<DateTime> _InDate;
        partial void OnInDateChanging(Nullable<DateTime> value);
        partial void OnInDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                OnOutDateChanging(value);
                _OutDate = value;
                RaisePropertyChanged("OutDate");
                OnOutDateChanged();
            }
        }
        private Nullable<DateTime> _OutDate;
        partial void OnOutDateChanging(Nullable<DateTime> value);
        partial void OnOutDateChanged();

        [DataMemberAttribute()]
        public String SerialCodeXML
        {
            get
            {
                return _SerialCodeXML;
            }
            set
            {
                OnSerialCodeXMLChanging(value);
                _SerialCodeXML = value;
                RaisePropertyChanged("SerialCodeXML");
                OnSerialCodeXMLChanged();
            }
        }
        private String _SerialCodeXML;
        partial void OnSerialCodeXMLChanging(String value);
        partial void OnSerialCodeXMLChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Allocation Allocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public OutwardResource OutwardResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public InwardResource InwardResource
        {
            get;
            set;
        }

        #endregion
    }
}
