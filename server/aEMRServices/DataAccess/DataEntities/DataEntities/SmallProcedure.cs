using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
/*
* 20190908 #001 TNHX: [BM0013263] Add field of SmallProcedure
* 20210712 #002 TNHX: thêm trường nhập bsi tính tiền cho SmallProcedure
* 20211228 #003 TNHX: thêm trường nhập bsi gây mê 2 cho SmallProcedure
* 20220913 #004 BLQ: Thêm trường bác sĩ gây mê chính thức
* 20230329 #005 QTD: Thêm trường vị trí thực hiện DVKT
*/
namespace DataEntities
{
    public class SmallProcedure : NotifyChangedBase
    {
        private long _SmallProcedureID = 0;
        private DateTime _ProcedureDateTime;
        private DateTime _CompletedDateTime;
        private string _Diagnosis;
        private string _ProcedureMethod;
        private string _NarcoticMethod;
        private long? _ProcedureDoctorStaffID;
        private Staff _ProcedureDoctorStaff;
        private long? _ProcedureDoctorStaffID2;
        private Staff _ProcedureDoctorStaff2;
        private long? _NarcoticDoctorStaffID;
        private Staff _NarcoticDoctorStaff;
        private long? _NarcoticDoctorStaffID2;
        private Staff _NarcoticDoctorStaff2;
        private long? _CheckRecordDoctorStaffID;
        private Staff _CheckRecordDoctorStaff;
        private long? _NurseStaffID;
        private Staff _NurseStaff;
        private long? _NurseStaffID2;
        private Staff _NurseStaff2;
        private long? _NurseStaffID3;
        private Staff _NurseStaff3;
        private DateTime _RecCreatedDate;
        private long _CreatedStaffID;
        private DateTime _ModifiedDate;
        private long _ModifiedStaffID;
        private long _PtRegDetailID;
        private string _TrinhTu;
        private long? _V_RegistrationType;
        private Lookup _V_Surgery_Tips_Type;
        private DeptLocation _DeptLocation;
        //▼====: #001
        private string _Notes;
        private DateTime? _DateOffStitches;
        private string _Drainage;
        private long _V_DeathReason = 0;
        private long _V_SurgicalMode = 0;
        private long _V_CatactropheType = 0;
        private long _V_AnesthesiaType = 0;
        private DiseasesReference _BeforeICD10;
        private DiseasesReference _AfterICD10;
        private string _DepartmentName;
        private long? _DeptLocID;
        //▲====: #001
        private string _ProcedureDescription;
        private string _ProcedureDescriptionContent;
        private long _ServiceRecID;
        public long SmallProcedureID
        {
            get => _SmallProcedureID; set
            {
                _SmallProcedureID = value;
                RaisePropertyChanged("SmallProcedureID");
            }
        }
        public DateTime ProcedureDateTime
        {
            get => _ProcedureDateTime; set
            {
                _ProcedureDateTime = value;
                RaisePropertyChanged("ProcedureDateTime");
            }
        }
        public DateTime CompletedDateTime
        {
            get => _CompletedDateTime; set
            {
                _CompletedDateTime = value;
                RaisePropertyChanged("CompletedDateTime");
            }
        }
        public string Diagnosis
        {
            get => _Diagnosis; set
            {
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
            }
        }
        public string ProcedureMethod
        {
            get => _ProcedureMethod; set
            {
                _ProcedureMethod = value;
                RaisePropertyChanged("ProcedureMethod");
            }
        }
        public string NarcoticMethod
        {
            get => _NarcoticMethod; set
            {
                _NarcoticMethod = value;
                RaisePropertyChanged("NarcoticMethod");
            }
        }
        public string TrinhTu
        {
            get => _TrinhTu; set
            {
                _TrinhTu = value;
                RaisePropertyChanged("TrinhTu");
            }
        }
        public long? ProcedureDoctorStaffID
        {
            get => _ProcedureDoctorStaffID; set
            {
                _ProcedureDoctorStaffID = value;
                RaisePropertyChanged("ProcedureDoctorStaffID");
            }
        }
        public long? ProcedureDoctorStaffID2
        {
            get => _ProcedureDoctorStaffID2; set
            {
                _ProcedureDoctorStaffID2 = value;
                RaisePropertyChanged("ProcedureDoctorStaffID2");
            }
        }
        public long? NarcoticDoctorStaffID
        {
            get => _NarcoticDoctorStaffID; set
            {
                _NarcoticDoctorStaffID = value;
                RaisePropertyChanged("NarcoticDoctorStaffID");
            }
        }
        public long? NarcoticDoctorStaffID2
        {
            get => _NarcoticDoctorStaffID2; set
            {
                _NarcoticDoctorStaffID2 = value;
                RaisePropertyChanged("NarcoticDoctorStaffID2");
            }
        }
        public long? CheckRecordDoctorStaffID
        {
            get => _CheckRecordDoctorStaffID; set
            {
                _CheckRecordDoctorStaffID = value;
                RaisePropertyChanged("CheckRecordDoctorStaffID");
            }
        }
        public long? NurseStaffID
        {
            get => _NurseStaffID; set
            {
                _NurseStaffID = value;
                RaisePropertyChanged("NurseStaffID");
            }
        }
        public long? NurseStaffID2
        {
            get => _NurseStaffID2; set
            {
                _NurseStaffID2 = value;
                RaisePropertyChanged("NurseStaffID2");
            }
        }
        public long? NurseStaffID3
        {
            get => _NurseStaffID3; set
            {
                _NurseStaffID3 = value;
                RaisePropertyChanged("NurseStaffID3");
            }
        }
        public DateTime RecCreatedDate
        {
            get => _RecCreatedDate; set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        public long CreatedStaffID
        {
            get => _CreatedStaffID; set
            {
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        public DateTime ModifiedDate
        {
            get => _ModifiedDate; set
            {
                _ModifiedDate = value;
                RaisePropertyChanged("ModifiedDate");
            }
        }
        public long ModifiedStaffID
        {
            get => _ModifiedStaffID; set
            {
                _ModifiedStaffID = value;
                RaisePropertyChanged("ModifiedStaffID");
            }
        }
        public long PtRegDetailID
        {
            get => _PtRegDetailID; set
            {
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }
        public long? V_RegistrationType
        {
            get => _V_RegistrationType; set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        public Lookup V_Surgery_Tips_Type
        {
            get
            {
                return _V_Surgery_Tips_Type;
            }
            set
            {
                if (_V_Surgery_Tips_Type == value)
                {
                    return;
                }
                _V_Surgery_Tips_Type = value;
                RaisePropertyChanged("V_Surgery_Tips_Type");
            }
        }
        public Staff ProcedureDoctorStaff
        {
            get
            {
                return _ProcedureDoctorStaff;
            }
            set
            {
                if (_ProcedureDoctorStaff == value)
                {
                    return;
                }
                _ProcedureDoctorStaff = value;
                RaisePropertyChanged("ProcedureDoctorStaff");
            }
        }
        public Staff ProcedureDoctorStaff2
        {
            get
            {
                return _ProcedureDoctorStaff2;
            }
            set
            {
                if (_ProcedureDoctorStaff2 == value)
                {
                    return;
                }
                _ProcedureDoctorStaff2 = value;
                RaisePropertyChanged("ProcedureDoctorStaff2");
            }
        }
        public Staff NarcoticDoctorStaff
        {
            get
            {
                return _NarcoticDoctorStaff;
            }
            set
            {
                if (_NarcoticDoctorStaff == value)
                {
                    return;
                }
                _NarcoticDoctorStaff = value;
                RaisePropertyChanged("NarcoticDoctorStaff");
            }
        }
        public Staff NarcoticDoctorStaff2
        {
            get
            {
                return _NarcoticDoctorStaff2;
            }
            set
            {
                if (_NarcoticDoctorStaff2 == value)
                {
                    return;
                }
                _NarcoticDoctorStaff2 = value;
                RaisePropertyChanged("NarcoticDoctorStaff2");
            }
        }
        public Staff CheckRecordDoctorStaff
        {
            get
            {
                return _CheckRecordDoctorStaff;
            }
            set
            {
                if (_CheckRecordDoctorStaff == value)
                {
                    return;
                }
                _CheckRecordDoctorStaff = value;
                RaisePropertyChanged("CheckRecordDoctorStaff");
            }
        }
        public Staff NurseStaff
        {
            get
            {
                return _NurseStaff;
            }
            set
            {
                if (_NurseStaff == value)
                {
                    return;
                }
                _NurseStaff = value;
                RaisePropertyChanged("NurseStaff");
            }
        }
        public Staff NurseStaff2
        {
            get
            {
                return _NurseStaff2;
            }
            set
            {
                if (_NurseStaff2 == value)
                {
                    return;
                }
                _NurseStaff2 = value;
                RaisePropertyChanged("NurseStaff2");
            }
        }
        public Staff NurseStaff3
        {
            get
            {
                return _NurseStaff3;
            }
            set
            {
                if (_NurseStaff3 == value)
                {
                    return;
                }
                _NurseStaff3 = value;
                RaisePropertyChanged("NurseStaff3");
            }
        }
        public DeptLocation DeptLocation
        {
            get
            {
                return _DeptLocation;
            }
            set
            {
                if (_DeptLocation == value)
                {
                    return;
                }
                _DeptLocation = value;
                RaisePropertyChanged("DeptLocation");
            }
        }

        //▼====: #001
        public string Notes
        {
            get => _Notes; set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }

        public DateTime? DateOffStitches
        {
            get => _DateOffStitches; set
            {
                _DateOffStitches = value;
                RaisePropertyChanged("DateOffStitches");
            }
        }

        public string Drainage
        {
            get => _Drainage; set
            {
                _Drainage = value;
                RaisePropertyChanged("Drainage");
            }
        }

        public long V_DeathReason
        {
            get
            {
                return _V_DeathReason;
            }
            set
            {
                if (_V_DeathReason == value)
                {
                    return;
                }
                _V_DeathReason = value;
                RaisePropertyChanged("V_DeathReason");
            }
        }
        public long V_SurgicalMode
        {
            get
            {
                return _V_SurgicalMode;
            }
            set
            {
                if (_V_SurgicalMode == value)
                {
                    return;
                }
                _V_SurgicalMode = value;
                RaisePropertyChanged("V_SurgicalMode");
            }
        }
        public long V_CatactropheType
        {
            get
            {
                return _V_CatactropheType;
            }
            set
            {
                if (_V_CatactropheType == value)
                {
                    return;
                }
                _V_CatactropheType = value;
                RaisePropertyChanged("V_CatactropheType");
            }
        }
        public long V_AnesthesiaType
        {
            get
            {
                return _V_AnesthesiaType;
            }
            set
            {
                if (_V_AnesthesiaType == value)
                {
                    return;
                }
                _V_AnesthesiaType = value;
                RaisePropertyChanged("V_AnesthesiaType");
            }
        }
        [DataMemberAttribute]
        public DiseasesReference BeforeICD10
        {
            get => _BeforeICD10; set
            {
                _BeforeICD10 = value;
                RaisePropertyChanged("BeforeICD10");
            }
        }
        [DataMemberAttribute]
        public DiseasesReference AfterICD10
        {
            get => _AfterICD10; set
            {
                _AfterICD10 = value;
                RaisePropertyChanged("AfterICD10");
            }
        }
        public string DepartmentName
        {
            get => _DepartmentName; set
            {
                _DepartmentName = value;
                RaisePropertyChanged("DepartmentName");
            }
        }
        public long? DeptLocID
        {
            get => _DeptLocID; set
            {
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        [DataMemberAttribute]
        public string ProcedureDescription
        {
            get
            {
                return _ProcedureDescription;
            }
            set
            {
                if (_ProcedureDescription == value)
                {
                    return;
                }
                _ProcedureDescription = value;
                RaisePropertyChanged("ProcedureDescription");
            }
        }
        [DataMemberAttribute]
        public string ProcedureDescriptionContent
        {
            get
            {
                return _ProcedureDescriptionContent;
            }
            set
            {
                if (_ProcedureDescriptionContent == value)
                {
                    return;
                }
                _ProcedureDescriptionContent = value;
                RaisePropertyChanged("ProcedureDescriptionContent");
            }
        }
        //▲====: #001
        [DataMemberAttribute]
        public long ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                if (_ServiceRecID == value)
                {
                    return;
                }
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
            }
        }
        private string _HIRepResourceCode;
        [DataMemberAttribute]
        public string HIRepResourceCode
        {
            get
            {
                return _HIRepResourceCode;
            }
            set
            {
                if (_HIRepResourceCode == value)
                {
                    return;
                }
                _HIRepResourceCode = value;
                RaisePropertyChanged("HIRepResourceCode");
            }
        }

        //▼====: #002
        private long? _UserOfficialPDStaffID;
        [DataMemberAttribute]
        public long? UserOfficialPDStaffID
        {
            get
            {
                return _UserOfficialPDStaffID;
            }
            set
            {
                if (_UserOfficialPDStaffID == value)
                {
                    return;
                }
                _UserOfficialPDStaffID = value;
                RaisePropertyChanged("UserOfficialPDStaffID");
            }
        }

        private long? _UserOfficialPDStaffID2;
        [DataMemberAttribute]
        public long? UserOfficialPDStaffID2
        {
            get
            {
                return _UserOfficialPDStaffID2;
            }
            set
            {
                if (_UserOfficialPDStaffID2 == value)
                {
                    return;
                }
                _UserOfficialPDStaffID2 = value;
                RaisePropertyChanged("UserOfficialPDStaffID2");
            }
        }

        private Staff _UserOfficialPDStaff;
        public Staff UserOfficialPDStaff
        {
            get
            {
                return _UserOfficialPDStaff;
            }
            set
            {
                if (_UserOfficialPDStaff == value)
                {
                    return;
                }
                _UserOfficialPDStaff = value;
                RaisePropertyChanged("UserOfficialPDStaff");
            }
        }

        private Staff _UserOfficialPDStaff2;
        public Staff UserOfficialPDStaff2
        {
            get
            {
                return _UserOfficialPDStaff2;
            }
            set
            {
                if (_UserOfficialPDStaff2 == value)
                {
                    return;
                }
                _UserOfficialPDStaff2 = value;
                RaisePropertyChanged("UserOfficialPDStaff2");
            }
        }
        //▲====: #002
        //▼====: #003
        private long? _NarcoticDoctorStaffID3;
        private Staff _NarcoticDoctorStaff3;
        public long? NarcoticDoctorStaffID3
        {
            get => _NarcoticDoctorStaffID3; set
            {
                _NarcoticDoctorStaffID3 = value;
                RaisePropertyChanged("NarcoticDoctorStaffID3");
            }
        }

        public Staff NarcoticDoctorStaff3
        {
            get
            {
                return _NarcoticDoctorStaff3;
            }
            set
            {
                if (_NarcoticDoctorStaff3 == value)
                {
                    return;
                }
                _NarcoticDoctorStaff3 = value;
                RaisePropertyChanged("NarcoticDoctorStaff3");
            }
        }
        //▲====: #003
        private int _ServiceMainTime;
        [DataMemberAttribute]
        public int ServiceMainTime
        {
            get
            {
                return _ServiceMainTime;
            }
            set
            {
                if (_ServiceMainTime == value)
                {
                    return;
                }
                _ServiceMainTime = value;
                RaisePropertyChanged("ServiceMainTime");
            }
        }
        //▼====: #004
        private long? _NarcoticDoctorOfficialStaffID;
        public long? NarcoticDoctorOfficialStaffID
        {
            get => _NarcoticDoctorOfficialStaffID; set
            {
                _NarcoticDoctorOfficialStaffID = value;
                RaisePropertyChanged("NarcoticDoctorOfficialStaffID");
            }
        }

        private Staff _NarcoticDoctorOfficialStaff;
        public Staff NarcoticDoctorOfficialStaff
        {
            get
            {
                return _NarcoticDoctorOfficialStaff;
            }
            set
            {
                if (_NarcoticDoctorOfficialStaff == value)
                {
                    return;
                }
                _NarcoticDoctorOfficialStaff = value;
                RaisePropertyChanged("NarcoticDoctorOfficialStaff");
            }
        }
        //▲====: #004
        //▼====: #005
        private long _V_SurgicalSite = 0;
        public long V_SurgicalSite
        {
            get
            {
                return _V_SurgicalSite;
            }
            set
            {
                if (_V_SurgicalSite == value)
                {
                    return;
                }
                _V_SurgicalSite = value;
                RaisePropertyChanged("V_SurgicalSite");
            }
        }
        //▲====: #005
    }
}
