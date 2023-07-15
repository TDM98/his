using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class DeptLocation : NotifyChangedBase, IEditableObject
    {
        public DeptLocation()
            : base()
        {

        }

        private DeptLocation _tempDeptLocation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDeptLocation = (DeptLocation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDeptLocation)
                CopyFrom(_tempDeptLocation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DeptLocation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DeptLocation object.

        /// <param name="deptLocationID">Initial value of the DeptLocationID property.</param>
        /// <param name="lID">Initial value of the LID property.</param>
        /// <param name="deptID">Initial value of the DeptID property.</param>
        public static DeptLocation CreateDeptLocation(Int64 deptLocationID, long lID, long deptID)
        {
            DeptLocation deptLocation = new DeptLocation();
            deptLocation.DeptLocationID = deptLocationID;
            deptLocation.LID = lID;
            deptLocation.DeptID = deptID;
            return deptLocation;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DeptLocationID
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
        private Int64 _DeptLocationID;
        partial void OnDeptLocationIDChanging(Int64 value);
        partial void OnDeptLocationIDChanged();

        [DataMemberAttribute()]
        public long LID
        {
            get
            {
                return _LID;
            }
            set
            {
                OnLIDChanging(value);
                _LID = value;
                RaisePropertyChanged("LID");
                OnLIDChanged();
            }
        }
        private long _LID;
        partial void OnLIDChanging(long value);
        partial void OnLIDChanged();

        private Location _Location;
        [DataMemberAttribute()]
        public Location Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                RaisePropertyChanged("Location");
            }
        }


        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }

        #endregion

        //For Insert XML
        private ObservableCollection<DeptLocation> _ObjDeptLocation_List;
        public ObservableCollection<DeptLocation> ObjDeptLocation_List
        {
            get 
            { 
                return _ObjDeptLocation_List; 
            }
            set
            {
                if (_ObjDeptLocation_List != value)
                {
                    _ObjDeptLocation_List = value;
                    RaisePropertyChanged("ObjDeptLocation_List");
                }
            }
        }

        //public string ConvertObjDeptLocation_ListToXml(Int64)
        //{
        //    return ConvertObjDeptLocation_ListToXml(_ObjDeptLocation_List);
        //}
        //For Insert XML


        //For Appointment, khi chọn 1 DV biết Phòng có ? Ca đó đến số TT hẹn nhiêu rồi
        [DataMemberAttribute()]
        public ConsultationTimeSegments ObjConsultationTimeSegments
        {
            get
            {
                return _ObjConsultationTimeSegments;
            }
            set
            {
                OnObjConsultationTimeSegmentsChanging(value);
                _ObjConsultationTimeSegments = value;
                RaisePropertyChanged("ObjConsultationTimeSegments");
                OnObjConsultationTimeSegmentsChanged();
            }
        }
        private ConsultationTimeSegments _ObjConsultationTimeSegments;
        partial void OnObjConsultationTimeSegmentsChanging(ConsultationTimeSegments value);
        partial void OnObjConsultationTimeSegmentsChanged();
        //For Appointment, khi chọn 1 DV biết Phòng có ? Ca đó đến số TT hẹn nhiêu rồi


        [DataMemberAttribute()]
        public Int16 CurrentSeqNumber
        {
            get
            {
                return _CurrentSeqNumber;
            }
            set
            {
                OnCurrentSeqNumberChanging(value);
                _CurrentSeqNumber = value;
                RaisePropertyChanged("CurrentSeqNumber");
                OnCurrentSeqNumberChanged();
            }
        }
        private Int16 _CurrentSeqNumber;
        partial void OnCurrentSeqNumberChanging(Int16 value);
        partial void OnCurrentSeqNumberChanged();

        [DataMemberAttribute()]
        public Int16 NumberOfSeq
        {
            get
            {
                return _NumberOfSeq;
            }
            set
            {
                OnNumberOfSeqChanging(value);
                _NumberOfSeq = value;
                RaisePropertyChanged("NumberOfSeq");
                OnNumberOfSeqChanged();
            }
        }
        private Int16 _NumberOfSeq;
        partial void OnNumberOfSeqChanging(Int16 value);
        partial void OnNumberOfSeqChanged();

        public override bool Equals(object obj)
        {
            var info = obj as DeptLocation;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return DeptLocationID > 0 && this.DeptLocationID == info.DeptLocationID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
