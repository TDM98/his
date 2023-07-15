using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class AppointmentSearchCriteria : SearchCriteriaBase
    {
        public AppointmentSearchCriteria()
        {
            this.ApptTimeSegment = new ConsultationTimeSegments();
            this.ApptTimeSegment.ConsultationTimeSegmentID = -1;
            this.DeptLocationID = -1;
            this.V_ApptStatus = -1;
        }
        private string _orderBy;

        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }

        private DateTime? _DateFrom;
        public DateTime? DateFrom
        {
            get
            {
                return _DateFrom;
            }
            set
            {
                _DateFrom = value;
                RaisePropertyChanged("DateFrom");
            }
        }
        private DateTime? _DateTo;
        public DateTime? DateTo
        {
            get
            {
                return _DateTo;
            }
            set
            {
                _DateTo = value;
                RaisePropertyChanged("DateTo");
            }
        }
        private long? _V_ApptStatus;
        public long? V_ApptStatus
        {
            get
            {
                return _V_ApptStatus;
            }
            set
            {
                _V_ApptStatus = value;
                RaisePropertyChanged("V_ApptStatus");
            }
        }

        private String _V_ApptStatusName;
        public String V_ApptStatusName
        {
            get
            {
                return _V_ApptStatusName;
            }
            set
            {
                _V_ApptStatusName = value;
                RaisePropertyChanged("V_ApptStatusName");
            }
        }
        private string _PatientNameString;
        /// <summary>
        /// Thuoc tinh nay khong dung de tim kiem
        /// Chi de lay thong tin nguoi dung nhap tren form (trong o ten benh nhan)
        /// roi extract no ra thanh FullName, hay HICardNumber, hay la PatientCode
        /// </summary>
        public string PatientNameString
        {
            get
            {
                return _PatientNameString;
            }
            set
            {
                _PatientNameString = value;
                RaisePropertyChanged("PatientNameString");
            }
        }
        private string _patientCode;
        public string PatientCode
        {
            get
            {
                return _patientCode;
            }
            set
            {
                _patientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _insuranceCard;

        public string InsuranceCard
        {
            get
            {
                return _insuranceCard;
            }
            set
            {
                _insuranceCard = value;
                RaisePropertyChanged("InsuranceCard");
            }
        }
        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
                RaisePropertyChanged("FullName");
            }
        }

        public string PMFCode
        {
            get
            {
                return _PMFCode;
            }
            set
            {
                _PMFCode = value;
                RaisePropertyChanged("PMFCode");
            }
        }
        private string _PMFCode;

        private Int16 _LoaiDV;
        public Int16 LoaiDV
        {
            get
            {
                return _LoaiDV;
            }
            set
            {
                _LoaiDV = value;
                RaisePropertyChanged("LoaiDV");
            }
        }

        private String _LoaiDVName;
        public String LoaiDVName
        {
            get
            {
                return _LoaiDVName;
            }
            set
            {
                _LoaiDVName = value;
                RaisePropertyChanged("LoaiDVName");
            }
        }
        private long? _DeptLocationID;
        public long? DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
            }
        }

        private String _LocationName;
        public String LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                _LocationName = value;
                RaisePropertyChanged("LocationName");
            }
        }


        private bool _IsConsultation=true;
        public bool IsConsultation
        {
            get
            {
                return _IsConsultation;
            }
            set
            {
                _IsConsultation = value;
                RaisePropertyChanged("IsConsultation");
            }
        }

        private long _StaffID;
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }

        private long _DoctorStaffID;
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        private ConsultationTimeSegments _ApptTimeSegment;
        public ConsultationTimeSegments ApptTimeSegment
        {
            get
            {
                return _ApptTimeSegment;
            }
            set
            {
                _ApptTimeSegment = value;
                RaisePropertyChanged("ApptTimeSegment");
            }
        }

        private bool _IsAppointmentKSK;
        public bool IsAppointmentKSK
        {
            get
            {
                return _IsAppointmentKSK;
            }
            set
            {
                _IsAppointmentKSK = value;
                RaisePropertyChanged("IsAppointmentKSK");
            }
        }

        private bool _IsSearchPatient;
        public bool IsSearchPatient
        {
            get
            {
                return _IsSearchPatient;
            }
            set
            {
                _IsSearchPatient = value;
                RaisePropertyChanged("IsSearchPatient");
            }
        }

        private long _HosClientID;
        public long HosClientID
        {
            get
            {
                return _HosClientID;
            }
            set
            {
                _HosClientID = value;
                RaisePropertyChanged("HosClientID");
            }
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }

        public long ConsultationRoomStaffAllocID { get; set; }

        public bool IsHasEndDate { get; set; } = false;

        private long _HosClientContractID;
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                _HosClientContractID = value;
                RaisePropertyChanged("HosClientContractID");
            }
        }
    }
}