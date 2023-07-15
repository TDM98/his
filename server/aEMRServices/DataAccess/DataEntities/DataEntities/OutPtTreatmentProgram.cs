using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
/*
 * 20220624 #001 DatTB: Thêm các thông tin: Mã số HSBA, Ngày đẩy cổng, Ngày tổng kết
 * 20220625 #002 DatTB: Thêm function lấy loại điều trị ngoại trú
 * 20220628 #003 DatTB: Thêm Ngày đẩy cổng, Ngày tổng kết convert qua datetime.
 * 20220811 #004 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm trường Ngày dự kiến tổng kết
 */
namespace DataEntities
{
    [DataContract]
    public partial class OutPtTreatmentProgram : EntityBase
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
        //▼==== #002
        private long _OutpatientTreatmentTypeID;
        [DataMemberAttribute]
        public long OutpatientTreatmentTypeID
        {
            get
            {
                return _OutpatientTreatmentTypeID;
            }
            set
            {
                if (_OutpatientTreatmentTypeID != value)
                {
                    _OutpatientTreatmentTypeID = value;
                    RaisePropertyChanged("OutpatientTreatmentTypeID");
                }
            }
        }
        private OutpatientTreatmentType _OutpatientTreatmentType;
        [DataMemberAttribute]
        public OutpatientTreatmentType OutpatientTreatmentType
        {
            get
            {
                return _OutpatientTreatmentType;
            }
            set
            {
                if (_OutpatientTreatmentType != value)
                {
                    _OutpatientTreatmentType = value;
                    RaisePropertyChanged("OutpatientTreatmentType");
                }
            }
        }
        //▲==== #002

        //▼==== #003
        private string _OutpatientTreatmentName;
        [DataMemberAttribute]
        public string OutpatientTreatmentName
        {
            get
            {
                return _OutpatientTreatmentName;
            }
            set
            {
                if (_OutpatientTreatmentName != value)
                {
                    _OutpatientTreatmentName = value;
                    RaisePropertyChanged("OutpatientTreatmentName");
}
            }
        }

        private DateTime? _ProgDatePushToDate;
        [DataMemberAttribute]
        public DateTime? ProgDatePushToDate
        {
            get
            {
                return _ProgDatePushToDate;
            }
            set
            {
                if (_ProgDatePushToDate != value)
                {
                    _ProgDatePushToDate = value;
                    RaisePropertyChanged("ProgDatePushToDate");
                }
            }
        }

        private DateTime? _ProgDateFinalToDate;
        [DataMemberAttribute]
        public DateTime? ProgDateFinalToDate
        {
            get
            {
                return _ProgDateFinalToDate;
            }
            set
            {
                if (_ProgDateFinalToDate != value)
                {
                    _ProgDateFinalToDate = value;
                    RaisePropertyChanged("ProgDateFinalToDate");
                }
            }
        }
        //▲==== #003


        //▼==== #004
        private DateTime? _ProgDateFinalExpect;
        [DataMemberAttribute]
        public DateTime? ProgDateFinalExpect
        {
            get
            {
                return _ProgDateFinalExpect;
            }
            set
            {
                if (_ProgDateFinalExpect != value)
                {
                    _ProgDateFinalExpect = value;
                    RaisePropertyChanged("ProgDateFinalExpect");
                }
            }
        }
        //▲==== #004
        private long _DeletedStaffID;
        [DataMemberAttribute]
        public long DeletedStaffID
        {
            get
            {
                return _DeletedStaffID;
            }
            set
            {
                if (_DeletedStaffID != value)
                {
                    _DeletedStaffID = value;
                    RaisePropertyChanged("DeletedStaffID");
                }
            }
        }
        private string _DeletedReason;
        [DataMemberAttribute]
        public string DeletedReason
        {
            get
            {
                return _DeletedReason;
            }
            set
            {
                if (_DeletedReason != value)
                {
                    _DeletedReason = value;
                    RaisePropertyChanged("DeletedReason");
                }
            }
        }
    }
}