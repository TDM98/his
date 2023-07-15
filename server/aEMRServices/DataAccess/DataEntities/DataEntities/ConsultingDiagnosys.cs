using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using eHCMSLanguage;

/*
 * 20180914 #001 TBL: Added IsCancelSurgery
 */

namespace DataEntities
{
    public class ConsultingDiagnosys : NotifyChangedBase
    {
        private long _ConsultingDiagnosysID;
        [DataMemberAttribute]
        public long ConsultingDiagnosysID
        {
            get
            {
                return _ConsultingDiagnosysID;
            }
            set
            {
                _ConsultingDiagnosysID = value;
                RaisePropertyChanged("ConsultingDiagnosysID");
            }
        }

        private long _PatientID;
        [DataMemberAttribute]
        public long PatientID
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

        private DateTime? _FirstExamDate;
        [DataMemberAttribute]
        public DateTime? FirstExamDate
        {
            get
            {
                return _FirstExamDate;
            }
            set
            {
                _FirstExamDate = value;
                RaisePropertyChanged("FirstExamDate");
            }
        }

        private DateTime? _NextExamDate;
        [DataMemberAttribute]
        public DateTime? NextExamDate
        {
            get
            {
                return _NextExamDate;
            }
            set
            {
                _NextExamDate = value;
                RaisePropertyChanged("NextExamDate");
            }
        }

        private DateTime? _ConsultingDate;
        [DataMemberAttribute]
        public DateTime? ConsultingDate
        {
            get
            {
                return _ConsultingDate;
            }
            set
            {
                _ConsultingDate = value;
                RaisePropertyChanged("ConsultingDate");
            }
        }

        private long _ConsultingDoctor;
        [DataMemberAttribute]
        public long ConsultingDoctor
        {
            get
            {
                return _ConsultingDoctor;
            }
            set
            {
                _ConsultingDoctor = value;
                RaisePropertyChanged("ConsultingDoctor");
            }
        }

        private string _ConsultingResult;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2085_G1_KetLuanHCNoi")]
        [DataMemberAttribute]
        public string ConsultingResult
        {
            get
            {
                return _ConsultingResult;
            }
            set
            {
                if (value != null && value.Length > 2500)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 2500));
                _ConsultingResult = value;
                RaisePropertyChanged("ConsultingResult");
            }
        }

        private DateTime? _OutPtConsultingDate;
        [DataMemberAttribute]
        public DateTime? OutPtConsultingDate
        {
            get
            {
                return _OutPtConsultingDate;
            }
            set
            {
                _OutPtConsultingDate = value;
                RaisePropertyChanged("OutPtConsultingDate");
            }
        }

        private long? _OutPtConsultingDoctor;
        [DataMemberAttribute]
        public long? OutPtConsultingDoctor
        {
            get
            {
                return _OutPtConsultingDoctor;
            }
            set
            {
                _OutPtConsultingDoctor = value;
                RaisePropertyChanged("OutPtConsultingDoctor");
            }
        }

        private string _OutPtConsultingResult;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2086_G1_KetLuanHCNgoai")]
        [DataMemberAttribute]
        public string OutPtConsultingResult
        {
            get
            {
                return _OutPtConsultingResult;
            }
            set
            {
                if (value != null && value.Length > 2500)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 2500));
                _OutPtConsultingResult = value;
                RaisePropertyChanged("OutPtConsultingResult");
            }
        }

        private Lookup _V_DiagnosticType;
        [DataMemberAttribute]
        public Lookup V_DiagnosticType
        {
            get
            {
                return _V_DiagnosticType;
            }
            set
            {
                _V_DiagnosticType = value;
                RaisePropertyChanged("V_DiagnosticType");
            }
        }

        private Lookup _V_TreatmentMethod;
        [DataMemberAttribute]
        public Lookup V_TreatmentMethod
        {
            get
            {
                return _V_TreatmentMethod;
            }
            set
            {
                _V_TreatmentMethod = value;
                RaisePropertyChanged("V_TreatmentMethod");
            }
        }

        private Lookup _V_HeartSurgicalType;
        [DataMemberAttribute]
        public Lookup V_HeartSurgicalType
        {
            get
            {
                return _V_HeartSurgicalType;
            }
            set
            {
                _V_HeartSurgicalType = value;
                RaisePropertyChanged("V_HeartSurgicalType");
                RaisePropertyChanged("IsOpenedSugery");
            }
        }

        private bool _ValveIncluded;
        [DataMemberAttribute]
        public bool ValveIncluded
        {
            get
            {
                return _ValveIncluded;
            }
            set
            {
                _ValveIncluded = value;
                RaisePropertyChanged("ValveIncluded");
            }
        }

        private Int16 _ValveQty;
        [DataMemberAttribute]
        public Int16 ValveQty
        {
            get
            {
                return _ValveQty;
            }
            set
            {
                _ValveQty = value;
                RaisePropertyChanged("ValveQty");
            }
        }

        private Lookup _V_ValveType;
        [DataMemberAttribute]
        public Lookup V_ValveType
        {
            get
            {
                return _V_ValveType;
            }
            set
            {
                _V_ValveType = value;
                RaisePropertyChanged("V_ValveType");
            }
        }

        private bool _RingIncluded;
        [DataMemberAttribute]
        public bool RingIncluded
        {
            get
            {
                return _RingIncluded;
            }
            set
            {
                _RingIncluded = value;
                RaisePropertyChanged("RingIncluded");
            }
        }

        private Int16 _RingQty;
        [DataMemberAttribute]
        public Int16 RingQty
        {
            get
            {
                return _RingQty;
            }
            set
            {
                _RingQty = value;
                RaisePropertyChanged("RingQty");
            }
        }

        private bool _CoronaryArtery;
        [DataMemberAttribute]
        public bool CoronaryArtery
        {
            get
            {
                return _CoronaryArtery;
            }
            set
            {
                _CoronaryArtery = value;
                RaisePropertyChanged("CoronaryArtery");
            }
        }

        private bool _MitralIncompetence;
        [DataMemberAttribute]
        public bool MitralIncompetence
        {
            get
            {
                return _MitralIncompetence;
            }
            set
            {
                _MitralIncompetence = value;
                RaisePropertyChanged("MitralIncompetence");
            }
        }

        private Int16 _BloodDonorNumber;
        [DataMemberAttribute]
        public Int16 BloodDonorNumber
        {
            get
            {
                return _BloodDonorNumber;
            }
            set
            {
                _BloodDonorNumber = value;
                RaisePropertyChanged("BloodDonorNumber");
            }
        }

        private decimal _EstimationPrice;
        [DataMemberAttribute]
        public decimal EstimationPrice
        {
            get
            {
                return _EstimationPrice;
            }
            set
            {
                _EstimationPrice = value;
                RaisePropertyChanged("EstimationPrice");
            }
        }

        private DateTime? _AdminProcessExpDate;
        [DataMemberAttribute]
        public DateTime? AdminProcessExpDate
        {
            get
            {
                return _AdminProcessExpDate;
            }
            set
            {
                _AdminProcessExpDate = value;
                RaisePropertyChanged("AdminProcessExpDate");
            }
        }

        private string _Note;
        [Display(ResourceType = typeof(eHCMSResources), Name = "T0723_G1_GChu")]
        [DataMemberAttribute]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (value != null && value.Length > 1000)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 1000));
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }

        private DateTime? _CoronarographyExpPaidTime;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2088_G1_NgayDKDongTienCMV")]
        [DataMemberAttribute]
        public DateTime? CoronarographyExpPaidTime
        {
            get
            {
                return _CoronarographyExpPaidTime;
            }
            set
            {
                _CoronarographyExpPaidTime = value;
                RaisePropertyChanged("CoronarographyExpPaidTime");
            }
        }

        private DateTime? _SurgeryExpPaidTime;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2055_G1_NgayDKDongPThuat")]
        [DataMemberAttribute]
        public DateTime? SurgeryExpPaidTime
        {
            get
            {
                return _SurgeryExpPaidTime;
            }
            set
            {
                _SurgeryExpPaidTime = value;
                RaisePropertyChanged("SurgeryExpPaidTime");
            }
        }

        private DateTime? _AdditionalItemsPaidTime;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2054_G1_NgayDKDongBSung")]
        [DataMemberAttribute]
        public DateTime? AdditionalItemsPaidTime
        {
            get
            {
                return _AdditionalItemsPaidTime;
            }
            set
            {
                _AdditionalItemsPaidTime = value;
                RaisePropertyChanged("AdditionalItemsPaidTime");
            }
        }

        private bool _IsEnoughBloodDonor;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2090_G1_DuNguoiChoMau")]
        [DataMemberAttribute]
        public bool IsEnoughBloodDonor
        {
            get
            {
                return _IsEnoughBloodDonor;
            }
            set
            {
                _IsEnoughBloodDonor = value;
                RaisePropertyChanged("IsEnoughBloodDonor");
            }
        }

        private bool _TMH_ExamDate;
        //[Display(ResourceType = typeof(eHCMSResources), Name = "Z2058_G1_NgayKhamTMH")]
        [DataMemberAttribute]
        public bool TMH_ExamDate
        {
            get
            {
                return _TMH_ExamDate;
            }
            set
            {
                _TMH_ExamDate = value;
                RaisePropertyChanged("TMH_ExamDate");
            }
        }

        private bool _RHM_ExamDate;
        //[Display(ResourceType = typeof(eHCMSResources), Name = "Z2059_G1_NgayKhamRHM")]
        [DataMemberAttribute]
        public bool RHM_ExamDate
        {
            get
            {
                return _RHM_ExamDate;
            }
            set
            {
                _RHM_ExamDate = value;
                RaisePropertyChanged("RHM_ExamDate");
            }
        }

        private bool _Transferred_KT_Date;
        //[Display(ResourceType = typeof(eHCMSResources), Name = "Z2057_G1_NgayChuyenKT")]
        [DataMemberAttribute]
        public bool Transferred_KT_Date
        {
            get
            {
                return _Transferred_KT_Date;
            }
            set
            {
                _Transferred_KT_Date = value;
                RaisePropertyChanged("Transferred_KT_Date");
            }
        }

        private DateTime? _CoronarographyDate;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2060_G1_NgayChupMVanh")]
        [DataMemberAttribute]
        public DateTime? CoronarographyDate
        {
            get
            {
                return _CoronarographyDate;
            }
            set
            {
                _CoronarographyDate = value;
                RaisePropertyChanged("CoronarographyDate");
            }
        }

        private DateTime? _AngioDate;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2061_G1_NgayChupMMau")]
        [DataMemberAttribute]
        public DateTime? AngioDate
        {
            get
            {
                return _AngioDate;
            }
            set
            {
                _AngioDate = value;
                RaisePropertyChanged("AngioDate");
            }
        }

        private bool _RVPDate;
        //[Display(ResourceType = typeof(eHCMSResources), Name = "Z2062_G1_NgayDoKhangLuc")]
        [DataMemberAttribute]
        public bool RVPDate
        {
            get
            {
                return _RVPDate;
            }
            set
            {
                _RVPDate = value;
                RaisePropertyChanged("RVPDate");
            }
        }

        private string _Remark;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2091_G1_ThongTinKhac")]
        [DataMemberAttribute]
        public string Remark
        {
            get
            {
                return _Remark;
            }
            set
            {
                if (value != null && value.Length > 1000)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 1000));
                _Remark = value;
                RaisePropertyChanged("Remark");
            }
        }

        private DateTime? _ExpAdmissionDate;
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2064_G1_NgayDuKienNhapVien")]
        [DataMemberAttribute]
        public DateTime? ExpAdmissionDate
        {
            get
            {
                return _ExpAdmissionDate;
            }
            set
            {
                _ExpAdmissionDate = value;
                RaisePropertyChanged("ExpAdmissionDate");
            }
        }

        private long? _PtRegistrationID;
        [DataMemberAttribute]
        public long? PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }

        private long _Createdby;
        [DataMemberAttribute]
        public long Createdby
        {
            get
            {
                return _Createdby;
            }
            set
            {
                _Createdby = value;
                RaisePropertyChanged("Createdby");
            }
        }

        private DateTime _CreatedDate;
        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }

        private Staff _DoctorStaff;
        [DataMemberAttribute]
        public Staff DoctorStaff
        {
            get
            {
                return _DoctorStaff;
            }
            set
            {
                _DoctorStaff = value;
                RaisePropertyChanged("DoctorStaff");
            }
        }

        private CharityOrganization _SupportCharityOrganization;
        [DataMemberAttribute]
        public CharityOrganization SupportCharityOrganization
        {
            get
            {
                return _SupportCharityOrganization;
            }
            set
            {
                if (_SupportCharityOrganization == value) return;
                _SupportCharityOrganization = value;
                RaisePropertyChanged("SupportCharityOrganization");
            }
        }

        private string _PrevSugeryDiagnostic;
        [DataMemberAttribute]
        public string PrevSugeryDiagnostic
        {
            get
            {
                return _PrevSugeryDiagnostic;
            }
            set
            {
                if (value != null && value.Length > 4000)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 4000));
                _PrevSugeryDiagnostic = value;
                RaisePropertyChanged("PrevSugeryDiagnostic");
            }
        }

        private string _FinalConsultingDiagnosys;
        [DataMemberAttribute]
        public string FinalConsultingDiagnosys
        {
            get
            {
                return _FinalConsultingDiagnosys;
            }
            set
            {
                if (value != null && value.Length > 4000)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 4000));
                _FinalConsultingDiagnosys = value;
                RaisePropertyChanged("FinalConsultingDiagnosys");
            }
        }

        private string _Intervention;
        [DataMemberAttribute]
        public string Intervention
        {
            get
            {
                return _Intervention;
            }
            set
            {
                if (value != null && value.Length > 2500)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 2500));
                _Intervention = value;
                RaisePropertyChanged("Intervention");
            }
        }

        private string _ShortIntervention;
        [DataMemberAttribute]
        public string ShortIntervention
        {
            get
            {
                return _ShortIntervention;
            }
            set
            {
                if (value != null && value.Length > 2500)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 2500));
                _ShortIntervention = value;
                RaisePropertyChanged("ShortIntervention");
            }
        }

        private bool _PlastieMitrale;
        [DataMemberAttribute]
        public bool PlastieMitrale
        {
            get
            {
                return _PlastieMitrale;
            }
            set
            {
                _PlastieMitrale = value;
                RaisePropertyChanged("PlastieMitrale");
            }
        }

        private bool _PTMaze;
        [DataMemberAttribute]
        public bool PTMaze
        {
            get
            {
                return _PTMaze;
            }
            set
            {
                _PTMaze = value;
                RaisePropertyChanged("PTMaze");
            }
        }

        private bool _DuraGraft;
        [DataMemberAttribute]
        public bool DuraGraft
        {
            get
            {
                return _DuraGraft;
            }
            set
            {
                _DuraGraft = value;
                RaisePropertyChanged("DuraGraft");
            }
        }

        private DateTime? _PCLExamStartDated;
        [DataMemberAttribute]
        public DateTime? PCLExamStartDated
        {
            get
            {
                return _PCLExamStartDated;
            }
            set
            {
                _PCLExamStartDated = value;
                RaisePropertyChanged("PCLExamStartDated");
            }
        }

        private string _PrevSugeryNote;
        [DataMemberAttribute]
        public string PrevSugeryNote
        {
            get
            {
                return _PrevSugeryNote;
            }
            set
            {
                if (value != null && value.Length > 4000)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 4000));
                _PrevSugeryNote = value;
                RaisePropertyChanged("PrevSugeryNote");
            }
        }

        private DateTime? _BloodInfoGettedDate;
        [DataMemberAttribute]
        public DateTime? BloodInfoGettedDate
        {
            get
            {
                return _BloodInfoGettedDate;
            }
            set
            {
                _BloodInfoGettedDate = value;
                RaisePropertyChanged("BloodInfoGettedDate");
            }
        }
        /*▼====: #001*/
        private bool _IsCancelSurgery;
        [DataMemberAttribute]
        public bool IsCancelSurgery
        {
            get
            {
                return _IsCancelSurgery;
            }
            set
            {
                _IsCancelSurgery = value;
                RaisePropertyChanged("IsCancelSurgery");
            }
        }
        /*▲====: #001*/

        private string _ReasonCancelSurgery;
        [DataMemberAttribute]
        public string ReasonCancelSurgery
        {
            get
            {
                return _ReasonCancelSurgery;
            }
            set
            {
                if (value != null && value.Length > 512)
                    throw new FormatException(string.Format(eHCMSResources.Z2087_G1_KhongDaiHon, 512));
                _ReasonCancelSurgery = value;
                RaisePropertyChanged("ReasonCancelSurgery");
            }
        }

        #region AdditionProperties
        private bool _IsAllExamCompleted;
        [DataMemberAttribute]
        public bool IsAllExamCompleted
        {
            get
            {
                return _IsAllExamCompleted;
            }
            set
            {
                if (_IsAllExamCompleted == value) return;
                _IsAllExamCompleted = value;
                RaisePropertyChanged("IsAllExamCompleted");
            }
        }

        private DateTime? _ExamCompletedDate;
        [DataMemberAttribute]
        public DateTime? ExamCompletedDate
        {
            get
            {
                return _ExamCompletedDate;
            }
            set
            {
                if (_ExamCompletedDate == value) return;
                _ExamCompletedDate = value;
                RaisePropertyChanged("ExamCompletedDate");
            }
        }

        private DateTime? _AdminProcessApprovedDate;
        [DataMemberAttribute]
        public DateTime? AdminProcessApprovedDate
        {
            get
            {
                return _AdminProcessApprovedDate;
            }
            set
            {
                if (_AdminProcessApprovedDate == value) return;
                _AdminProcessApprovedDate = value;
                RaisePropertyChanged("AdminProcessApprovedDate");
            }
        }

        private DateTime? _SurgeryDate;
        [DataMemberAttribute]
        public DateTime? SurgeryDate
        {
            get
            {
                return _SurgeryDate;
            }
            set
            {
                if (_SurgeryDate == value) return;
                _SurgeryDate = value;
                RaisePropertyChanged("SurgeryDate");
            }
        }

        public DateTime? AdmissionDate
        {
            get
            {
                return Registration != null && Registration.AdmissionInfo != null ? Registration.AdmissionInfo.AdmissionDate : null;
            }
        }
        public DateTime? DischargeDate
        {
            get
            {
                return Registration != null && Registration.AdmissionInfo != null ? Registration.AdmissionInfo.DischargeDate : null;
            }
        }
        public long? DischargeNote
        {
            get
            {
                return Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.V_DischargeType != null ? (long?)Registration.AdmissionInfo.V_DischargeType : null;
            }
        }

        public string AdmissionDeptName
        {
            get
            {
                return Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.Department != null ? Registration.AdmissionInfo.Department.DeptName : null;
            }
        }

        public string DischargeReason
        {
            get
            {
                return Registration != null && Registration.AdmissionInfo != null ? Registration.AdmissionInfo.DischargeNote : null;
            }
        }

        private Patient _Patient = null;
        [DataMemberAttribute]
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                _Patient = value;
                if (Patient != null)
                    PatientID = Patient.PatientID;
                RaisePropertyChanged("Patient");
                RaisePropertyChanged("PatientID");
            }
        }

        private string _PatientCode;
        [DataMemberAttribute]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        public string PatientName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Patient == null)
                        Patient = new Patient { FullName = value };
                    else
                        Patient.FullName = value;
                }
            }
        }

        private PatientRegistration _Registration = null;
        [DataMemberAttribute]
        public PatientRegistration Registration
        {
            get
            {
                return _Registration;
            }
            set
            {
                _Registration = value;
                RaisePropertyChanged("Registration");
            }
        }

        private PhysicalExamination _PhysicalExamination;
        [DataMemberAttribute]
        public PhysicalExamination PhysicalExamination
        {
            get
            {
                return _PhysicalExamination;
            }
            set
            {
                _PhysicalExamination = value;
                RaisePropertyChanged("PhysicalExamination");
            }
        }

        private Staff _ConsultingDoctorStaff;
        [DataMemberAttribute]
        public Staff ConsultingDoctorStaff
        {
            get
            {
                return _ConsultingDoctorStaff;
            }
            set
            {
                _ConsultingDoctorStaff = value;
                if (ConsultingDoctorStaff != null)
                    ConsultingDoctor = ConsultingDoctorStaff.StaffID;
                RaisePropertyChanged("ConsultingDoctorStaff");
                RaisePropertyChanged("ConsultingDoctor");
            }
        }

        public string ConsultingDoctorName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (ConsultingDoctorStaff == null)
                        ConsultingDoctorStaff = new Staff { FullName = value };
                }
            }
        }

        private Staff _OutPtConsultingDoctorStaff;
        [DataMemberAttribute]
        public Staff OutPtConsultingDoctorStaff
        {
            get
            {
                return _OutPtConsultingDoctorStaff;
            }
            set
            {
                _OutPtConsultingDoctorStaff = value;
                if (OutPtConsultingDoctorStaff != null)
                    OutPtConsultingDoctor = OutPtConsultingDoctorStaff.StaffID;
                RaisePropertyChanged("OutPtConsultingDoctorStaff");
                RaisePropertyChanged("OutPtConsultingDoctor");
            }
        }

        public string OutPtConsultingDoctorName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (OutPtConsultingDoctorStaff == null)
                        OutPtConsultingDoctorStaff = new Staff { FullName = value };
                }
            }
        }

        public bool IsOpenedSugery
        {
            get
            {
                return V_HeartSurgicalType != null && V_HeartSurgicalType.LookupID == (long)AllLookupValues.V_HeartSurgicalType.Opened;
            }
            set
            {
                if (value)
                    V_HeartSurgicalType = new Lookup { LookupID = 64002, ObjectTypeID = 640, ObjectName = "V_HeartSurgicalType", ObjectValue = "Mổ tim hở" };
                else
                    V_HeartSurgicalType = new Lookup { LookupID = 64001, ObjectTypeID = 640, ObjectName = "V_HeartSurgicalType", ObjectValue = "Mổ tim kín" };
                RaisePropertyChanged("V_HeartSurgicalType");
                RaisePropertyChanged("IsOpenedSugery");
            }
        }

        private SurgeryScheduleDetail _SurgeryScheduleDetail;
        public SurgeryScheduleDetail SurgeryScheduleDetail
        {
            get { return _SurgeryScheduleDetail; }
            set
            {
                _SurgeryScheduleDetail = value;
                RaisePropertyChanged("SurgeryScheduleDetail");
            }
        }
        #endregion

        #region ValidateProperties
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2079_G1_SoLuongVan")]
        public object ValveQtyDisplay
        {
            get { return this.ValveQty; }
            set
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    this.ValveQty = 0;
                }
                else
                {
                    Int16 mResult;
                    if (Int16.TryParse(value.ToString(), out mResult))
                    {
                        this.ValveQty = mResult;
                    }
                    else
                    {
                        throw new FormatException(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                    }
                }
                RaisePropertyChanged("ValveQtyDisplay");
                RaisePropertyChanged("ValveQty");
            }
        }
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2080_G1_SoLuongVong")]
        public object RingQtyDisplay
        {
            get { return this.RingQty; }
            set
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    this.RingQty = 0;
                }
                else
                {
                    Int16 mResult;
                    if (Int16.TryParse(value.ToString(), out mResult))
                    {
                        this.RingQty = mResult;
                    }
                    else
                    {
                        throw new FormatException(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                    }
                }
                RaisePropertyChanged("RingQtyDisplay");
                RaisePropertyChanged("RingQty");
            }
        }
        [Display(ResourceType = typeof(eHCMSResources), Name = "Z2093_G1_SoLuongNguoiChoMau")]
        public object BloodDonorNumberDisplay
        {
            get { return this.BloodDonorNumber; }
            set
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    this.BloodDonorNumber = 0;
                }
                else
                {
                    Int16 mResult;
                    if (Int16.TryParse(value.ToString(), out mResult))
                    {
                        this.BloodDonorNumber = mResult;
                    }
                    else
                    {
                        throw new FormatException(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                    }
                }
                RaisePropertyChanged("BloodDonorNumberDisplay");
                RaisePropertyChanged("BloodDonorNumber");
            }
        }
        #endregion
    }
}
