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
    public partial class DeptLocInfo : NotifyChangedBase, IEditableObject
    {
        public DeptLocInfo()
            : base()
        {

        }

        private DeptLocInfo _tempDeptLocInfo;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDeptLocInfo = (DeptLocInfo)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDeptLocInfo)
                CopyFrom(_tempDeptLocInfo);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DeptLocInfo p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DeptLocInfo object.

        /// <param name="DeptLocInfoID">Initial value of the DeptLocInfoID property.</param>
        /// <param name="lID">Initial value of the LID property.</param>
        /// <param name="deptID">Initial value of the DeptID property.</param>
        public static DeptLocInfo CreateDeptLocInfo(Int64 DeptLocationID, long lID, long deptID)
        {
            DeptLocInfo DeptLocInfo = new DeptLocInfo();
            DeptLocInfo.DeptLocationID = DeptLocationID;
            DeptLocInfo.LID = lID;
            DeptLocInfo.DeptID = deptID;
            return DeptLocInfo;
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


        [DataMemberAttribute()]
        public int ChoKham
        {
            get
            {
                return _ChoKham;
            }
            set
            {
                OnChoKhamChanging(value);
                _ChoKham = value;
                RaisePropertyChanged("ChoKham");
                OnChoKhamChanged();
            }
        }
        private int _ChoKham;
        partial void OnChoKhamChanging(int value);
        partial void OnChoKhamChanged();

        [DataMemberAttribute()]
        public int KhamRoi
        {
            get
            {
                return _KhamRoi;
            }
            set
            {
                OnKhamRoiChanging(value);
                _KhamRoi = value;
                RaisePropertyChanged("KhamRoi");
                OnKhamRoiChanged();
            }
        }
        private int _KhamRoi;
        partial void OnKhamRoiChanging(int value);
        partial void OnKhamRoiChanged();

        [DataMemberAttribute()]
        public VRoomType V_RoomType
        {
            get
            {
                return _V_RoomType;
            }
            set
            {
                OnV_RoomTypeChanging(value);
                _V_RoomType = value;
                RaisePropertyChanged("V_RoomType");
                OnV_RoomTypeChanged();
            }
        }
        private VRoomType _V_RoomType=VRoomType.Khoa;
        partial void OnV_RoomTypeChanging(VRoomType value);
        partial void OnV_RoomTypeChanged();


        [DataMemberAttribute()]
        public ConsultationTimeSegments ConsultTimeSeg
        {
            get
            {
                return _ConsultTimeSeg;
            }
            set
            {
                OnConsultTimeSegChanging(value);
                _ConsultTimeSeg = value;
                RaisePropertyChanged("ConsultTimeSeg");
                OnConsultTimeSegChanged();
            }
        }
        private ConsultationTimeSegments _ConsultTimeSeg;
        partial void OnConsultTimeSegChanging(ConsultationTimeSegments value);
        partial void OnConsultTimeSegChanged();


        [DataMemberAttribute()]
        public ObservableCollection<Staff> lstStaff
        {
            get
            {
                return _lstStaff;
            }
            set
            {
                OnlstStaffChanging(value);
                _lstStaff = value;
                RaisePropertyChanged("lstStaff");
                OnlstStaffChanged();
            }
        }
        private ObservableCollection<Staff> _lstStaff;
        partial void OnlstStaffChanging(ObservableCollection<Staff> value);
        partial void OnlstStaffChanged();

        [DataMemberAttribute()]
        public string staffName
        {
            get
            {
                return _staffName;
            }
            set
            {
                OnstaffNameChanging(value);
                _staffName = value;
                RaisePropertyChanged("staffName");
                OnstaffNameChanged();
            }
        }
        private string _staffName;
        partial void OnstaffNameChanging(string value);
        partial void OnstaffNameChanged();
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

    }
}
