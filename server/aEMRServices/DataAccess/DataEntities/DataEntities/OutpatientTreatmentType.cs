using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
/*
 * 20220624 #001 DatTB: Thêm các thông tin: Mã số HSBA, Ngày đẩy cổng, Ngày tổng kết
 */
namespace DataEntities
{
    [DataContract]
    public partial class OutPtTreatmentProgram : NotifyChangedBase
    {
        private long _OutPtTreatmentProgramID;
        private string _TreatmentProgName;
        private DateTime _ProgDateFrom;
        private DateTime? _ProgDateTo;
        private long _DoctorStaffID;
        private DateTime _RecCreatedDate;
        private bool _IsDeleted = false;
        private long _PatientID;
        private Staff _CreatorStaff;
        private bool _IsChecked = false;
        [DataMemberAttribute]
        public long OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                if (_OutPtTreatmentProgramID != value)
                {
                    _OutPtTreatmentProgramID = value;
                    RaisePropertyChanged("OutPtTreatmentProgramID");
                }
            }
        }
        [DataMemberAttribute]
        public string TreatmentProgName
        {
            get
            {
                return _TreatmentProgName;
            }
            set
            {
                if (_TreatmentProgName != value)
                {
                    _TreatmentProgName = value;
                    RaisePropertyChanged("TreatmentProgName");
                }
            }
        }
        [DataMemberAttribute]
        public DateTime ProgDateFrom
        {
            get
            {
                return _ProgDateFrom;
            }
            set
            {
                if (_ProgDateFrom != value)
                {
                    _ProgDateFrom = value;
                    RaisePropertyChanged("ProgDateFrom");
                }
            }
        }
        [DataMemberAttribute]
        public DateTime? ProgDateTo
        {
            get
            {
                return _ProgDateTo;
            }
            set
            {
                if (_ProgDateTo != value)
                {
                    _ProgDateTo = value;
                    RaisePropertyChanged("ProgDateTo");
                }
            }
        }
        [DataMemberAttribute]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID != value)
                {
                    _DoctorStaffID = value;
                    RaisePropertyChanged("DoctorStaffID");
                }
            }
        }
        [DataMemberAttribute]
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
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                }
            }
        }
        [DataMemberAttribute]
        public long PatientID { get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                }
            }
        }
        [DataMemberAttribute]
        public Staff CreatorStaff
        {
            get
            {
                return _CreatorStaff;
            }
            set
            {
                if (_CreatorStaff != value)
                {
                    _CreatorStaff = value;
                    RaisePropertyChanged("CreatorStaff");
                    RaisePropertyChanged("CreatorStaffID");
                }
            }
        }
        public long CreatorStaffID
        {
            get
            {
                return CreatorStaff.StaffID;
            }
        }
        [DataMemberAttribute]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        //▼==== #001
        private string _TreatmentProgCode;
        [DataMemberAttribute]
        public string TreatmentProgCode
        {
            get
            {
                return _TreatmentProgCode;
            }
            set
            {
                if (_TreatmentProgCode != value)
                {
                    _TreatmentProgCode = value;
                    RaisePropertyChanged("TreatmentProgCode");
                }
            }
        }

        private int _ProgDatePush;
        [DataMemberAttribute]
        public int ProgDatePush
        {
            get
            {
                return _ProgDatePush;
            }
            set
            {
                if (_ProgDatePush != value)
                {
                    _ProgDatePush = value;
                    RaisePropertyChanged("ProgDatePush");
                }
            }
        }

        private int _ProgDateFinal;
        [DataMemberAttribute]
        public int ProgDateFinal
        {
            get
            {
                return _ProgDateFinal;
            }
            set
            {
                if (_ProgDateFinal != value)
                {
                    _ProgDateFinal = value;
                    RaisePropertyChanged("ProgDateFinal");
                }
            }
        }
        //▲==== #001
    }
}