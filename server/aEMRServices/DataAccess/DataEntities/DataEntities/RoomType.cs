using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{

    public partial class RoomType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RoomType object.

        /// <param name="rmTypeID">Initial value of the RmTypeID property.</param>
        /// <param name="rmTypeName">Initial value of the RmTypeName property.</param>
        public static RoomType CreateRoomType(Int64 rmTypeID, String rmTypeName)
        {
            RoomType roomType = new RoomType();
            roomType.RmTypeID = rmTypeID;
            roomType.RmTypeName = rmTypeName;
            return roomType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RmTypeID
        {
            get
            {
                return _RmTypeID;
            }
            set
            {
                if (_RmTypeID != value)
                {
                    OnRmTypeIDChanging(value);
                    ////ReportPropertyChanging("RmTypeID");
                    _RmTypeID = value;
                    RaisePropertyChanged("RmTypeID");
                    OnRmTypeIDChanged();
                }
            }
        }
        private Int64 _RmTypeID;
        partial void OnRmTypeIDChanging(Int64 value);
        partial void OnRmTypeIDChanged();




        [Required(ErrorMessage = "Nhập Tên Loại Phòng!")]
        [StringLength(64, MinimumLength = 0, ErrorMessage = "Tên Loại Phòng! Phải <= 64 Ký Tự")]
        [DataMemberAttribute()]
        public String RmTypeName
        {
            get
            {
                return _RmTypeName;
            }
            set
            {
                OnRmTypeNameChanging(value);
                ValidateProperty("RmTypeName", value);
                _RmTypeName = value;
                RaisePropertyChanged("RmTypeName");
                OnRmTypeNameChanged();
            }
        }
        private String _RmTypeName;
        partial void OnRmTypeNameChanging(String value);
        partial void OnRmTypeNameChanged();



        [DataMemberAttribute()]
        public String RmTypeDescription
        {
            get
            {
                return _RmTypeDescription;
            }
            set
            {
                OnRmTypeDescriptionChanging(value);
                ////ReportPropertyChanging("RmTypeDescription");
                _RmTypeDescription = value;
                RaisePropertyChanged("RmTypeDescription");
                OnRmTypeDescriptionChanged();
            }
        }
        private String _RmTypeDescription;
        partial void OnRmTypeDescriptionChanging(String value);
        partial void OnRmTypeDescriptionChanged();


        [DataMemberAttribute()]        
        public Int64 V_RoomFunction
        {
            get { return _V_RoomFunction; }
            set 
            {
                if (_V_RoomFunction != value)
                {
                    OnV_RoomFunctionChanging(value);
                    _V_RoomFunction = value;
                    RaisePropertyChanged("V_RoomFunction");
                    OnV_RoomFunctionChanged();
                }
            }
        }
        private Int64 _V_RoomFunction;
        partial void OnV_RoomFunctionChanging(Int64 value);
        partial void OnV_RoomFunctionChanged();


        [DataMemberAttribute()]        
        private Lookup _ObjV_RoomFunction;
        public Lookup ObjV_RoomFunction
        {
            get { return _ObjV_RoomFunction; }
            set 
            {
                OnObjV_RoomFunctionChanging(value);
                _ObjV_RoomFunction = value;
                RaisePropertyChanged("ObjV_RoomFunction");
                OnObjV_RoomFunctionChanged();
            }
        }
        partial void OnObjV_RoomFunctionChanging(Lookup value);
        partial void OnObjV_RoomFunctionChanged();


        [DataMemberAttribute()]
        public Nullable<Boolean> IsHospitalizedRoom
        {
            get
            {
                return _IsHospitalizedRoom;
            }
            set
            {
                OnIsHospitalizedRoomChanging(value);
                ////ReportPropertyChanging("IsHospitalizedRoom");
                _IsHospitalizedRoom = value;
                RaisePropertyChanged("IsHospitalizedRoom");
                OnIsHospitalizedRoomChanged();
            }
        }
        private Nullable<Boolean> _IsHospitalizedRoom;
        partial void OnIsHospitalizedRoomChanging(Nullable<Boolean> value);
        partial void OnIsHospitalizedRoomChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> RoomCharge
        {
            get
            {
                return _RoomCharge;
            }
            set
            {
                OnRoomChargeChanging(value);
                ////ReportPropertyChanging("RoomCharge");
                _RoomCharge = value;
                RaisePropertyChanged("RoomCharge");
                OnRoomChargeChanged();
            }
        }
        private Nullable<Decimal> _RoomCharge;
        partial void OnRoomChargeChanging(Nullable<Decimal> value);
        partial void OnRoomChargeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "Locations")]
        public ObservableCollection<Location> Locations
        {
            get;
            set;
        }

        #endregion
    }
}
