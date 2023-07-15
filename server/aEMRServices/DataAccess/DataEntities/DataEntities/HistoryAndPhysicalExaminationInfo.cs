using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
/*
 * 20210428 #001 BLQ: thêm trường cho HistoryAndPhysicalExaminationInfo
 * 20220509 #002 DatTB: Chỉnh sửa HSBA RHM: Bệnh chuyên khoa long -> string
 * 20220829 #003 QTD: Thêm trường loại kết quả điều trị, loại tình trạng ra viện, khoá chỉnh sửa khi đã đưa vào kho
*/
namespace DataEntities
{
    public class HistoryAndPhysicalExaminationInfo : NotifyChangedBase
    {
        private long _HistoryAndPhysicalExaminationInfoID;
        private long _PtRegDetailID;
        private string _HistoryAndPhysicalExamination;
        private string _PastMedicalHistory;
        private string _PastMedicalHistoryOfFamily;
        private string _PhysicalExamination;
        private string _PhysicalExaminationAllParts;
        private string _ParaclinicalNote;
        private string _MedicalInProcessed;
        private string _LaboratoryNote;
        private string _TreatmentMethod;
        private string _DischargeStatus;
        private string _TreatmentSolution;
        private long _StaffID;
        private DateTime _RecCreatedDate;
        private long _SentStaffID;
        //▼====: #001
        private long _OutPtTreatmentProgramID;
        private string _ReasonAdmission;
        private string _FirstDiagnostic;
        private string _DischargeDiagnostic;
        private DateTime _ProgDateFrom;
        private DateTime? _ProgDateTo;
        private string _PathologicalProcessAndClinicalCourse;
        private string _PCLResultsHaveDiagnosticValue;
        private string _DischargeDiagnostic_MainDisease;
        private string _DischargeDiagnostic_IncludingDisease;
        private string _Treatments;
        private string _ConditionDischarge;
        private string _DirectionOfTreatment;
        private long _V_SpecialistType;
        private string _MedicalRecordNote;
        private string _DiagnosisOfOutpatientDept;
        private string _ProcessedByDownline;
        private string _XQuangNote;
        private string _OrientedTreatment;
        private long _PatientID;
        private string _SpecialistDisease; //<====: #002
        private string _Diagnosis;

        [DataMemberAttribute]
        public long OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                if (_OutPtTreatmentProgramID == value)
                {
                    return;
                }
                _OutPtTreatmentProgramID = value;
                RaisePropertyChanged("OutPtTreatmentProgramID");
            }
        }

        [DataMemberAttribute]
        public string ReasonAdmission
        {
            get
            {
                return _ReasonAdmission;
            }
            set
            {
                if (_ReasonAdmission == value)
                {
                    return;
                }
                _ReasonAdmission = value;
                RaisePropertyChanged("ReasonAdmission");
            }
        }

        [DataMemberAttribute]
        public string FirstDiagnostic
        {
            get
            {
                return _FirstDiagnostic;
            }
            set
            {
                if (_FirstDiagnostic == value)
                {
                    return;
                }
                _FirstDiagnostic = value;
                RaisePropertyChanged("FirstDiagnostic");
            }
        }

        [DataMemberAttribute]
        public string DischargeDiagnostic
        {
            get
            {
                return _DischargeDiagnostic;
            }
            set
            {
                if (_DischargeDiagnostic == value)
                {
                    return;
                }
                _DischargeDiagnostic = value;
                RaisePropertyChanged("DischargeDiagnostic");
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
                if (_ProgDateFrom == value)
                {
                    return;
                }
                _ProgDateFrom = value;
                RaisePropertyChanged("ProgDateFrom");
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
                if (_ProgDateTo == value)
                {
                    return;
                }
                _ProgDateTo = value;
                RaisePropertyChanged("ProgDateTo");
            }
        }

        [DataMemberAttribute]
        public string PathologicalProcessAndClinicalCourse
        {
            get
            {
                return _PathologicalProcessAndClinicalCourse;
            }
            set
            {
                if (_PathologicalProcessAndClinicalCourse == value)
                {
                    return;
                }
                _PathologicalProcessAndClinicalCourse = value;
                RaisePropertyChanged("PathologicalProcessAndClinicalCourse");
            }
        }

        [DataMemberAttribute]
        public string PCLResultsHaveDiagnosticValue
        {
            get
            {
                return _PCLResultsHaveDiagnosticValue;
            }
            set
            {
                if (_PCLResultsHaveDiagnosticValue == value)
                {
                    return;
                }
                _PCLResultsHaveDiagnosticValue = value;
                RaisePropertyChanged("PCLResultsHaveDiagnosticValue");
            }
        }

        [DataMemberAttribute]
        public string DischargeDiagnostic_MainDisease
        {
            get
            {
                return _DischargeDiagnostic_MainDisease;
            }
            set
            {
                if (_DischargeDiagnostic_MainDisease == value)
                {
                    return;
                }
                _DischargeDiagnostic_MainDisease = value;
                RaisePropertyChanged("DischargeDiagnostic_MainDisease");
            }
        }

        [DataMemberAttribute]
        public string DischargeDiagnostic_IncludingDisease
        {
            get
            {
                return _DischargeDiagnostic_IncludingDisease;
            }
            set
            {
                if (_DischargeDiagnostic_IncludingDisease == value)
                {
                    return;
                }
                _DischargeDiagnostic_IncludingDisease = value;
                RaisePropertyChanged("DischargeDiagnostic_IncludingDisease");
            }
        }

        [DataMemberAttribute]
        public string Treatments
        {
            get
            {
                return _Treatments;
            }
            set
            {
                if (_Treatments == value)
                {
                    return;
                }
                _Treatments = value;
                RaisePropertyChanged("Treatments");
            }
        }

        [DataMemberAttribute]
        public string ConditionDischarge
        {
            get
            {
                return _ConditionDischarge;
            }
            set
            {
                if (_ConditionDischarge == value)
                {
                    return;
                }
                _ConditionDischarge = value;
                RaisePropertyChanged("ConditionDischarge");
            }
        }

        [DataMemberAttribute]
        public string DirectionOfTreatment
        {
            get
            {
                return _DirectionOfTreatment;
            }
            set
            {
                if (_DirectionOfTreatment == value)
                {
                    return;
                }
                _DirectionOfTreatment = value;
                RaisePropertyChanged("DirectionOfTreatment");
            }
        }
        //▲====: #001
        [DataMemberAttribute]
        public long HistoryAndPhysicalExaminationInfoID
        {
            get
            {
                return _HistoryAndPhysicalExaminationInfoID;
            }
            set
            {
                if (_HistoryAndPhysicalExaminationInfoID == value)
                {
                    return;
                }
                _HistoryAndPhysicalExaminationInfoID = value;
                RaisePropertyChanged("HistoryAndPhysicalExaminationInfoID");
            }
        }
        [DataMemberAttribute]
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                if (_PtRegDetailID == value)
                {
                    return;
                }
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }
        [DataMemberAttribute]
        public string HistoryAndPhysicalExamination
        {
            get
            {
                return _HistoryAndPhysicalExamination;
            }
            set
            {
                if (_HistoryAndPhysicalExamination == value)
                {
                    return;
                }
                _HistoryAndPhysicalExamination = value;
                RaisePropertyChanged("HistoryAndPhysicalExamination");
            }
        }
        [DataMemberAttribute]
        public string PastMedicalHistory
        {
            get
            {
                return _PastMedicalHistory;
            }
            set
            {
                if (_PastMedicalHistory == value)
                {
                    return;
                }
                _PastMedicalHistory = value;
                RaisePropertyChanged("PastMedicalHistory");
            }
        }
        [DataMemberAttribute]
        public string PastMedicalHistoryOfFamily
        {
            get
            {
                return _PastMedicalHistoryOfFamily;
            }
            set
            {
                if (_PastMedicalHistoryOfFamily == value)
                {
                    return;
                }
                _PastMedicalHistoryOfFamily = value;
                RaisePropertyChanged("PastMedicalHistoryOfFamily");
            }
        }
        [DataMemberAttribute]
        public string PhysicalExamination
        {
            get
            {
                return _PhysicalExamination;
            }
            set
            {
                if (_PhysicalExamination == value)
                {
                    return;
                }
                _PhysicalExamination = value;
                RaisePropertyChanged("PhysicalExamination");
            }
        }
        [DataMemberAttribute]
        public string PhysicalExaminationAllParts
        {
            get
            {
                return _PhysicalExaminationAllParts;
            }
            set
            {
                if (_PhysicalExaminationAllParts == value)
                {
                    return;
                }
                _PhysicalExaminationAllParts = value;
                RaisePropertyChanged("PhysicalExaminationAllParts");
            }
        }
        [DataMemberAttribute]
        public string ParaclinicalNote
        {
            get
            {
                return _ParaclinicalNote;
            }
            set
            {
                if (_ParaclinicalNote == value)
                {
                    return;
                }
                _ParaclinicalNote = value;
                RaisePropertyChanged("ParaclinicalNote");
            }
        }
        [DataMemberAttribute]
        public string MedicalInProcessed
        {
            get
            {
                return _MedicalInProcessed;
            }
            set
            {
                if (_MedicalInProcessed == value)
                {
                    return;
                }
                _MedicalInProcessed = value;
                RaisePropertyChanged("MedicalInProcessed");
            }
        }
        [DataMemberAttribute]
        public string LaboratoryNote
        {
            get
            {
                return _LaboratoryNote;
            }
            set
            {
                if (_LaboratoryNote == value)
                {
                    return;
                }
                _LaboratoryNote = value;
                RaisePropertyChanged("LaboratoryNote");
            }
        }
        [DataMemberAttribute]
        public string TreatmentMethod
        {
            get
            {
                return _TreatmentMethod;
            }
            set
            {
                if (_TreatmentMethod == value)
                {
                    return;
                }
                _TreatmentMethod = value;
                RaisePropertyChanged("TreatmentMethod");
            }
        }
        [DataMemberAttribute]
        public string DischargeStatus
        {
            get
            {
                return _DischargeStatus;
            }
            set
            {
                if (_DischargeStatus == value)
                {
                    return;
                }
                _DischargeStatus = value;
                RaisePropertyChanged("DischargeStatus");
            }
        }
        [DataMemberAttribute]
        public string TreatmentSolution
        {
            get
            {
                return _TreatmentSolution;
            }
            set
            {
                if (_TreatmentSolution == value)
                {
                    return;
                }
                _TreatmentSolution = value;
                RaisePropertyChanged("TreatmentSolution");
            }
        }
        [DataMemberAttribute]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID == value)
                {
                    return;
                }
                _StaffID = value;
                RaisePropertyChanged("StaffID");
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
                if (_RecCreatedDate == value)
                {
                    return;
                }
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        [DataMemberAttribute]
        public long SentStaffID
        {
            get
            {
                return _SentStaffID;
            }
            set
            {
                if (_SentStaffID == value)
                {
                    return;
                }
                _SentStaffID = value;
                RaisePropertyChanged("SentStaffID");
            }
        }

        //DatTB 20220307: Thêm V_SpecialistType để hiển thị tab cho khám RHM
        [DataMemberAttribute]
        public long V_SpecialistType
        {
            get
            {
                return _V_SpecialistType;
            }
            set
            {
                if (_V_SpecialistType == value)
                {
                    return;
                }
                _V_SpecialistType = value;
                RaisePropertyChanged("V_SpecialistType");
            }
        }

        //DatTB 20220307: Thêm chẩn đoán cho phiếu RHM
        [DataMemberAttribute]
        public string MedicalRecordNote
        {
            get
            {
                return _MedicalRecordNote;
            }
            set
            {
                if (_MedicalRecordNote == value)
                {
                    return;
                }
                _MedicalRecordNote = value;
                RaisePropertyChanged("MedicalRecordNote");
            }
        }

        [DataMemberAttribute]
        public string DiagnosisOfOutpatientDept
        {
            get
            {
                return _DiagnosisOfOutpatientDept;
            }
            set
            {
                if (_DiagnosisOfOutpatientDept == value)
                {
                    return;
                }
                _DiagnosisOfOutpatientDept = value;
                RaisePropertyChanged("DiagnosisOfOutpatientDept");
            }
        }

        [DataMemberAttribute]
        public string ProcessedByDownline
        {
            get
            {
                return _ProcessedByDownline;
            }
            set
            {
                if (_ProcessedByDownline == value)
                {
                    return;
                }
                _ProcessedByDownline = value;
                RaisePropertyChanged("ProcessedByDownline");
            }
        }
        [DataMemberAttribute]
        public string XQuangNote
        {
            get
            {
                return _XQuangNote;
            }
            set
            {
                if (_XQuangNote == value)
                {
                    return;
                }
                _XQuangNote = value;
                RaisePropertyChanged("XQuangNote");
            }
        }
        [DataMemberAttribute]
        public string OrientedTreatment
        {
            get
            {
                return _OrientedTreatment;
            }
            set
            {
                if (_OrientedTreatment == value)
                {
                    return;
                }
                _OrientedTreatment = value;
                RaisePropertyChanged("OrientedTreatment");
            }
        }
        [DataMemberAttribute]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        [DataMemberAttribute]
        public string SpecialistDisease
        {
            get
            {
                return _SpecialistDisease;
            }
            set
            {
                if (_SpecialistDisease == value)
                {
                    return;
                }
                _SpecialistDisease = value;
                RaisePropertyChanged("SpecialistDisease");
            }
        }

        [DataMemberAttribute]
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if (_Diagnosis == value)
                {
                    return;
                }
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
            }
        }
        //▼====: #003
        private long _V_OutDischargeCondition;
        [DataMemberAttribute]
        public long V_OutDischargeCondition
        {
            get
            {
                return _V_OutDischargeCondition;
            }
            set
            {
                if (_V_OutDischargeCondition != value)
                {
                    _V_OutDischargeCondition = value;
                    RaisePropertyChanged("V_OutDischargeCondition");
                }
            }
        }
        private long _V_OutDischargeType;
        [DataMemberAttribute]
        public long V_OutDischargeType
        {
            get
            {
                return _V_OutDischargeType;
            }
            set
            {
                if (_V_OutDischargeType != value)
                {
                    _V_OutDischargeType = value;
                    RaisePropertyChanged("V_OutDischargeType");
                }
            }
        }
        private bool _IsBorrowing;
        [DataMemberAttribute]
        public bool IsBorrowing
        {
            get
            {
                return _IsBorrowing;
            }
            set
            {
                if (_IsBorrowing != value)
                {
                    _IsBorrowing = value;
                    RaisePropertyChanged("IsBorrowing");
                }
            }
        }
        //▲====: #003
    }
}