using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    [DataContract]
    public partial class PatientApptServiceDetails : EntityBase, IEditableObject
    {
        public PatientApptServiceDetails()
            : base()
        {

        }

        private PatientApptServiceDetails _tempAppointment;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAppointment = (PatientApptServiceDetails)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAppointment)
                CopyFrom(_tempAppointment);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientApptServiceDetails p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private long _AppointmentID;
        [DataMemberAttribute()]
        public long AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                if (_AppointmentID != value)
                {
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                }
            }
        }

        private Patient _patient;
        [DataMemberAttribute()]
        public Patient patient
        {
            get
            {
                return _patient;
            }
            set
            {
                if (_patient != value)
                {
                    _patient = value;
                    RaisePropertyChanged("patient");
                }
            }
        }

        private long _ApptSvcDetailID;
        [DataMemberAttribute()]
        public long ApptSvcDetailID
        {
            get
            {
                return _ApptSvcDetailID;
            }
            set
            {
                if (_ApptSvcDetailID != value)
                {
                    _ApptSvcDetailID = value;
                    RaisePropertyChanged("ApptSvcDetailID");
                }
            }
        }

        private long _MedServiceID;
        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                }
            }
        }

        private RefMedicalServiceItem _MedService;
        [Required(ErrorMessage = "Chọn dịch vụ")]
        [DataMemberAttribute()]
        public RefMedicalServiceItem MedService
        {
            get
            {
                return _MedService;
            }
            set
            {
                if (_MedService != value)
                {
                    ValidateProperty("MedService", value);
                    _MedService = value;
                    RaisePropertyChanged("MedService");
                }
            }
        }

        private long _DeptLocationID;
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
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                }
            }
        }


        private DeptLocation _DeptLocation;
        [Required(ErrorMessage = "Chọn Phòng")]
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
                    ValidateProperty("DeptLocation", value);
                    _DeptLocation = value;
                    RaisePropertyChanged("DeptLocation");
                    if (_DeptLocation != null
                        && _DeptLocation.DeptLocationID > 0)
                    {
                        DeptLocationID = _DeptLocation.DeptLocationID;
                    }
                }
            }
        }

        private short _ApptTimeSegmentID;
        [DataMemberAttribute()]
        public short ApptTimeSegmentID
        {
            get
            {
                return _ApptTimeSegmentID;
            }
            set
            {
                if (_ApptTimeSegmentID != value)
                {
                    _ApptTimeSegmentID = value;
                    RaisePropertyChanged("ApptTimeSegmentID");
                }
            }
        }

        private ConsultationTimeSegments _ApptTimeSegment;
        [Required(ErrorMessage = "Chọn Ca")]
        [DataMemberAttribute()]
        public ConsultationTimeSegments ApptTimeSegment
        {
            get
            {
                return _ApptTimeSegment;
            }
            set
            {
                if (_ApptTimeSegment != value)
                {
                    ValidateProperty("ApptTimeSegment", value);
                    _ApptTimeSegment = value;
                    RaisePropertyChanged("ApptTimeSegment");
                }
            }
        }


        //Danh sách chọn để làm hẹn
        private ObservableCollection<DeptLocation> _DeptLocationList;
        [DataMemberAttribute()]
        public ObservableCollection<DeptLocation> DeptLocationList
        {
            get
            {
                return _DeptLocationList;
            }
            set
            {
                _DeptLocationList = value;
                RaisePropertyChanged("DeptLocationList");
            }
        }


        private ObservableCollection<ConsultationTimeSegments> _ApptTimeSegmentList;
        [DataMemberAttribute()]
        public ObservableCollection<ConsultationTimeSegments> ApptTimeSegmentList
        {
            get
            {
                return _ApptTimeSegmentList;
            }
            set
            {
                _ApptTimeSegmentList = value;
                RaisePropertyChanged("ApptTimeSegmentList");
            }
        }
        //Danh sách chọn để làm hẹn



        [DataMemberAttribute()]
        public Int16 ServiceSeqNum
        {
            get
            {
                return _ServiceSeqNum;
            }
            set
            {
                if (_ServiceSeqNum != value)
                {
                    _ServiceSeqNum = value;
                    RaisePropertyChanged("ServiceSeqNum");
                }
            }
        }
        private Int16 _ServiceSeqNum;


        [DataMemberAttribute()]
        public Byte ServiceSeqNumType
        {
            get
            {
                return _ServiceSeqNumType;
            }
            set
            {
                if (_ServiceSeqNumType != value)
                {
                    _ServiceSeqNumType = value;
                    RaisePropertyChanged("ServiceSeqNumType");
                }
            }
        }
        private Byte _ServiceSeqNumType;



        private long _V_AppointmentType;
        [DataMemberAttribute()]
        public long V_AppointmentType
        {
            get
            {
                return _V_AppointmentType;
            }
            set
            {
                if (_V_AppointmentType != value)
                {
                    _V_AppointmentType = value;
                    RaisePropertyChanged("V_AppointmentType");
                }
            }
        }


        [DataMemberAttribute()]
        public Lookup AppointmentType
        {
            get
            {
                return _AppointmentType;
            }
            set
            {
                if (_AppointmentType != value)
                {
                    _AppointmentType = value;
                    RaisePropertyChanged("AppointmentType");
                }
            }
        }
        private Lookup _AppointmentType;

        private string _staffFullName;
        [DataMemberAttribute()]
        public string staffFullName
        {
            get
            {
                return _staffFullName;
            }
            set
            {
                if (_staffFullName != value)
                {
                    _staffFullName = value;
                    RaisePropertyChanged("staffFullName");
                }
            }
        }


        //trả ra khi chưa config, khi bị đầy phòng
        [DataMemberAttribute()]
        public String Result
        {
            get
            {
                return _Result;
            }
            set
            {
                if (_Result != value)
                {
                    _Result = value;
                    RaisePropertyChanged("Result");
                }
            }
        }
        private String _Result;
        //trả ra khi chưa config, khi bị đầy phòng

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        private long? _ConsultationRoomStaffAllocID;
        [DataMemberAttribute]
        public long? ConsultationRoomStaffAllocID
        {
            get
            {
                return _ConsultationRoomStaffAllocID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocID");
            }
        }

        private DateTime? _ApptStartDate;
        [DataMemberAttribute]
        public DateTime? ApptStartDate
        {
            get
            {
                return _ApptStartDate;
            }
            set
            {
                if (_ApptStartDate == value)
                {
                    return;
                }
                _ApptStartDate = value;
                RaisePropertyChanged("ApptStartDate");
            }
        }

        private DateTime? _ApptEndDate;
        [DataMemberAttribute]
        public DateTime? ApptEndDate
        {
            get
            {
                return _ApptEndDate;
            }
            set
            {
                if (_ApptEndDate == value)
                {
                    return;
                }
                _ApptEndDate = value;
                RaisePropertyChanged("ApptEndDate");
            }
        }

        private long? _ClientContractSvcPtID;
        [DataMemberAttribute]
        public long? ClientContractSvcPtID
        {
            get
            {
                return _ClientContractSvcPtID;
            }
            set
            {
                if (_ClientContractSvcPtID == value)
                {
                    return;
                }
                _ClientContractSvcPtID = value;
                RaisePropertyChanged("ClientContractSvcPtID");
            }
        }
        private long _PackServDetailID;
        [DataMemberAttribute()]
        public long PackServDetailID
        {
            get
            {
                return _PackServDetailID;
            }
            set
            {
                if (_PackServDetailID != value)
                {
                    _PackServDetailID = value;
                    RaisePropertyChanged("PackServDetailID");
                }
            }
        }

        public override bool Equals(object obj)
        {
            PatientApptServiceDetails info = obj as PatientApptServiceDetails;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ApptSvcDetailID > 0 && this.ApptSvcDetailID == info.ApptSvcDetailID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}