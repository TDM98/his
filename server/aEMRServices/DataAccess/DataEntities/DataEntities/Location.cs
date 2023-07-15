using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
using Service.Core.Common;
/*
* 20221118 #001 BLQ: Thêm đánh dấu làm việc ngoài giờ
* 20230509 #002 DatTB: Thêm trường ghi chú cho danh mục phòng
*/
namespace DataEntities
{
    public partial class Location : EntityBase, IEditableObject
    {
        #region Factory Method


        /// Create a new Location object.

        /// <param name="lID">Initial value of the LID property.</param>
        /// <param name="locationName">Initial value of the LocationName property.</param>
        public static Location CreateLocation(long lID, String locationName)
        {
            Location location = new Location();
            location.LID = lID;
            location.LocationName = locationName;
            return location;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long LID
        {
            get
            {
                return _LID;
            }
            set
            {
                if (_LID != value)
                {
                    OnLIDChanging(value);
                    ////ReportPropertyChanging("LID");
                    _LID = value;
                    RaisePropertyChanged("LID");
                    OnLIDChanged();
                }
            }
        }
        private long _LID;
        partial void OnLIDChanging(long value);
        partial void OnLIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> RmTypeID
        {
            get
            {
                return _RmTypeID;
            }
            set
            {
                OnRmTypeIDChanging(value);
                ////ReportPropertyChanging("RmTypeID");
                _RmTypeID = value;
                RaisePropertyChanged("RmTypeID");
                OnRmTypeIDChanged();
            }
        }
        private Nullable<Int64> _RmTypeID;
        partial void OnRmTypeIDChanging(Nullable<Int64> value);
        partial void OnRmTypeIDChanged();

        [Required(ErrorMessage = "Nhập Tên Phòng!")]
        [StringLength(64, MinimumLength = 0, ErrorMessage = "Tên Phòng Phải <= 64 Ký Tự")]
        [DataMemberAttribute()]
        public String LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                if (_LocationName  != value)
                {
                    OnLocationNameChanging(value);
                    ValidateProperty("LocationName", value);
                    _LocationName = value;
                    RaisePropertyChanged("LocationName");
                    RaisePropertyChanged("OutstandingString");
                    OnLocationNameChanged(); 
                }
            }
        }
        private String _LocationName;
        partial void OnLocationNameChanging(String value);
        partial void OnLocationNameChanged();
        

        [DataMemberAttribute()]
        public String LocationDescription
        {
            get
            {
                return _LocationDescription;
            }
            set
            {
                if (_LocationDescription != value)
                {
                    OnLocationDescriptionChanging(value);
                    ////ReportPropertyChanging("LocationDescription");
                    _LocationDescription = value;
                    RaisePropertyChanged("LocationDescription");
                    OnLocationDescriptionChanged(); 
                }
            }
        }
        private String _LocationDescription;
        partial void OnLocationDescriptionChanging(String value);
        partial void OnLocationDescriptionChanged();

        [DataMemberAttribute()]
        public Nullable<long> V_LocationType
        {
            get
            {
                return _V_LocationType;
            }
            set
            {
                OnV_LocationTypeChanging(value);
                ////ReportPropertyChanging("V_LocationType");
                _V_LocationType = value;
                RaisePropertyChanged("V_LocationType");
                OnV_LocationTypeChanged();
            }
        }
        private Nullable<long> _V_LocationType;
        partial void OnV_LocationTypeChanging(Nullable<long> value);
        partial void OnV_LocationTypeChanged();

        [DataMemberAttribute()]
        public Location VLocationType
        {
            get
            {
                return _VLocationType;
            }
            set
            {
                OnVLocationTypeChanging(value);
                ////ReportPropertyChanging("VLocationType");
                _VLocationType = value;
                RaisePropertyChanged("VLocationType");
                OnVLocationTypeChanged();
            }
        }
        private Location _VLocationType;
        partial void OnVLocationTypeChanging(Location  value);
        partial void OnVLocationTypeChanged();

        [DataMemberAttribute()]
        public Lookup V_SpecialistClinicType
        {
            get
            {
                return _V_SpecialistClinicType;
            }
            set
            {
                OnV_SpecialistClinicTypeChanging(value);
                _V_SpecialistClinicType = value;
                RaisePropertyChanged("V_SpecialistClinicType");
                OnV_SpecialistClinicTypeChanged();
            }
        }
        private Lookup _V_SpecialistClinicType;
        partial void OnV_SpecialistClinicTypeChanging(Lookup value);
        partial void OnV_SpecialistClinicTypeChanged();
        //▼====: #001
        private bool _IsOvertimeWorking;
        [DataMemberAttribute()]
        public bool IsOvertimeWorking
        {
            get
            {
                return _IsOvertimeWorking;
            }
            set
            {
                _IsOvertimeWorking = value;
                RaisePropertyChanged("IsOvertimeWorking");
            }
        }
        //▲====: #001

        //▼==== #002
        private string _Notes;
        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        //▲==== #002
        #endregion

        #region Navigation Properties

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
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_ALLOCATI_REL_RM01_LOCATION", "Allocations")]
        public ObservableCollection<Allocation> Allocations
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DEPTLOCA_REL_DM07_LOCATION", "DeptLocation")]
        public ObservableCollection<DeptLocation> DeptLocations
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "RoomType")]
        public RoomType RoomType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_LOCATION", "PatientRegistrationDetails")]
        public ObservableCollection<PatientRegistrationDetail> PatientRegistrationDetails
        {
            get;
            set;
        }

        #endregion

        private int _NumOfWaitingPatients;
        public int NumOfWaitingPatients
        {
            get
            {
                return _NumOfWaitingPatients;
            }
            set
            {
                _NumOfWaitingPatients = value;
                RaisePropertyChanged("NumOfWaitingPatients");
                RaisePropertyChanged("OutstandingString");
            }
        }

        public string OutstandingString
        {
            get
            {
                if (_NumOfWaitingPatients <= 0)
                    return _LocationName;
                return _LocationName + string.Format(" ({0})",_NumOfWaitingPatients);
            }
        }
     
        /// return a list of Departments (this location belongs to).
     
        /// 
        private ObservableCollection<RefDepartment> _Departments;
        public ObservableCollection<RefDepartment> Departments
        {
            get
            {
                return _Departments;
            }
            set
            {
                _Departments = value;
                RaisePropertyChanged("Departments");
            }
        }

        //For Insert XML
        private ObservableCollection<Location> _ObjLocation_List;
        public ObservableCollection<Location> ObjLocation_List
        {
            get { return _ObjLocation_List; }
            set 
            {
                if (_ObjLocation_List != value)
                {
                    _ObjLocation_List = value;
                    RaisePropertyChanged("ObjLocation_List");
                }
            }
        }
        public string ConvertObjLocation_ListToXml()
        {
            return ConvertObjLocation_ListToXml(_ObjLocation_List);
        }
        public string ConvertObjLocation_ListToXml(IEnumerable<Location> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (Location details in items)
                {
                    sb.Append("<Locations>");
                    sb.AppendFormat("<V_LocationType>{0}</V_LocationType>",details.V_LocationType==null?"null":details.V_LocationType.ToString());
                    sb.AppendFormat("<RmTypeID>{0}</RmTypeID>", details.RmTypeID);
                    sb.AppendFormat("<LocationName>{0}</LocationName>", details.LocationName);
                    sb.AppendFormat("<LocationDescription>{0}</LocationDescription>", details.LocationDescription);
                    sb.Append("</Locations>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }        
        //For Insert XML

        //For Check add on dtg DeptLocation
        public override bool IsChecked
        {
            get
            {
                return base.IsChecked;
            }
            set
            {
                base.IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
   
        private EntityState _EntityState;

        [DataMember]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
            }
        }
        private Location _tempObj;

        public void BeginEdit()
        {
            _tempObj = (Location)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempObj)
            {
                CopyFrom(_tempObj);
                _tempObj = null;
            }
        }

        public void EndEdit()
        {
            _tempObj = null;
        }

        public void CopyFrom(Location p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        public override bool Equals(object obj)
        {
            Location info = obj as Location;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.LID > 0 && this.LID == info.LID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region For check choose from dataList with checkbox
        [DataMemberAttribute()]
        public Nullable<Boolean> IsCheckedInDataList
        {
            get
            {
                return _IsCheckedInDataList;
            }
            set
            {
                if (_IsCheckedInDataList != value)
                {
                    OnIsCheckedInDataListChanging(value);
                    _IsCheckedInDataList = value;
                    RaisePropertyChanged("IsCheckedInDataList");
                    OnIsCheckedInDataListChanged();
                }
            }
        }
        private Nullable<Boolean> _IsCheckedInDataList;
        partial void OnIsCheckedInDataListChanging(Nullable<Boolean> value);
        partial void OnIsCheckedInDataListChanged();
        #endregion
    }
}
