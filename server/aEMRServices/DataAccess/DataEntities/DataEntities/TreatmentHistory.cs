using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class TreatmentHistory : NotifyChangedBase
    {
        private DateTime _AdmissionDate;
        private DateTime? _DischargeDate;
        private string _MedServiceName;
        private bool _InPt;
        private long _V_PCLMainCategory;
        private string _GroupText;
        private int _Ordinal;
        private long _PrescriptID;
        private long _DTItemID;
        private long _PatientPCLReqID;
        private long _PCLExamTypeID;
        private long _PtRegistrationID;
        private long _ServiceRecID;
        private long? _SmallProcedureID;
        private TreatmentHistoryGroupContent _TreatmentHistoryGroupContent = new TreatmentHistoryGroupContent();
        private long? _OutPtTreatmentProgramID;
        [DataMember]
        public DateTime AdmissionDate
        {
            get
            {
                return _AdmissionDate;
            }
            set
            {
                if (_AdmissionDate == value)
                {
                    return;
                }
                _AdmissionDate = value;
                RaisePropertyChanged("AdmissionDate");
                TreatmentHistoryGroupContent.AdmissionDate = AdmissionDate;
                RaisePropertyChanged("TreatmentHistoryGroupContent");
            }
        }
        [DataMember]
        public DateTime? DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                if (_DischargeDate == value)
                {
                    return;
                }
                _DischargeDate = value;
                RaisePropertyChanged("DischargeDate");
                TreatmentHistoryGroupContent.DischargeDate = DischargeDate;
                RaisePropertyChanged("TreatmentHistoryGroupContent");
            }
        }
        [DataMember]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                if (_MedServiceName == value)
                {
                    return;
                }
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
            }
        }
        [DataMember]
        public bool InPt
        {
            get
            {
                return _InPt;
            }
            set
            {
                if (_InPt == value)
                {
                    return;
                }
                _InPt = value;
                RaisePropertyChanged("InPt");
                TreatmentHistoryGroupContent.InPt = InPt;
                RaisePropertyChanged("TreatmentHistoryGroupContent");
            }
        }
        [DataMember]
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                if (_V_PCLMainCategory == value)
                {
                    return;
                }
                _V_PCLMainCategory = value;
                RaisePropertyChanged("V_PCLMainCategory");
            }
        }
        [DataMember]
        public string GroupText
        {
            get
            {
                return _GroupText;
            }
            set
            {
                if (_GroupText == value)
                {
                    return;
                }
                _GroupText = value;
                RaisePropertyChanged("GroupText");
            }
        }
        [DataMember]
        public int Ordinal
        {
            get
            {
                return _Ordinal;
            }
            set
            {
                if (_Ordinal == value)
                {
                    return;
                }
                _Ordinal = value;
                RaisePropertyChanged("Ordinal");
            }
        }
        [DataMember]
        public long PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                if (_PrescriptID == value)
                {
                    return;
                }
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
            }
        }
        [DataMember]
        public long DTItemID
        {
            get
            {
                return _DTItemID;
            }
            set
            {
                if (_DTItemID == value)
                {
                    return;
                }
                _DTItemID = value;
                RaisePropertyChanged("DTItemID");
            }
        }
        [DataMember]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID == value)
                {
                    return;
                }
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
            }
        }
        [DataMember]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID == value)
                {
                    return;
                }
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        [DataMember]
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
                TreatmentHistoryGroupContent.PtRegistrationID = PtRegistrationID;
                RaisePropertyChanged("TreatmentHistoryGroupContent");
            }
        }
        [DataMember]
        public TreatmentHistoryGroupContent TreatmentHistoryGroupContent
        {
            get
            {
                return _TreatmentHistoryGroupContent;
            }
            set
            {
                if (_TreatmentHistoryGroupContent == value)
                {
                    return;
                }
                _TreatmentHistoryGroupContent = value;
                RaisePropertyChanged("TreatmentHistoryGroupContent");
            }
        }
        [DataMember]
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
        [DataMember]
        public long? SmallProcedureID
        {
            get
            {
                return _SmallProcedureID;
            }
            set
            {
                if (_SmallProcedureID == value)
                {
                    return;
                }
                _SmallProcedureID = value;
                RaisePropertyChanged("SmallProcedureID");
            }
        }
        [DataMemberAttribute]
        public long? OutPtTreatmentProgramID
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
                    TreatmentHistoryGroupContent.IsInTreatmentProgramID = OutPtTreatmentProgramID.HasValue && OutPtTreatmentProgramID.Value > 0;
                }
            }
        }

        private bool _IsWarning;
        [DataMemberAttribute]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                if (_IsWarning != value)
                {
                    _IsWarning = value;
                    RaisePropertyChanged("IsWarning");
                }
            }
        }
        private long? _DiagConsultationSummaryID;
        [DataMember]
        public long? DiagConsultationSummaryID
        {
            get
            {
                return _DiagConsultationSummaryID;
            }
            set
            {
                if (_DiagConsultationSummaryID == value)
                {
                    return;
                }
                _DiagConsultationSummaryID = value;
                RaisePropertyChanged("DiagConsultationSummaryID");
            }
        }
    }
    public class TreatmentHistoryGroupContent : NotifyChangedBase
    {
        private DateTime _AdmissionDate;
        private DateTime? _DischargeDate;
        private long _PtRegistrationID;
        private bool _InPt;
        private bool _IsInTreatmentProgramID = false;
        [DataMember]
        public DateTime AdmissionDate
        {
            get
            {
                return _AdmissionDate;
            }
            set
            {
                if (_AdmissionDate == value)
                {
                    return;
                }
                _AdmissionDate = value;
                RaisePropertyChanged("AdmissionDate");
            }
        }
        [DataMember]
        public DateTime? DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                if (_DischargeDate == value)
                {
                    return;
                }
                _DischargeDate = value;
                RaisePropertyChanged("DischargeDate");
            }
        }
        [DataMember]
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
        [DataMember]
        public bool InPt
        {
            get
            {
                return _InPt;
            }
            set
            {
                if (_InPt == value)
                {
                    return;
                }
                _InPt = value;
                RaisePropertyChanged("InPt");
            }
        }
        [DataMember]
        public bool IsInTreatmentProgramID
        {
            get
            {
                return _IsInTreatmentProgramID;
            }
            set
            {
                if (_IsInTreatmentProgramID == value)
                {
                    return;
                }
                _IsInTreatmentProgramID = value;
                RaisePropertyChanged("IsInTreatmentProgramID");
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TreatmentHistoryGroupContent))
            {
                return false;
            }
            return (obj as TreatmentHistoryGroupContent).PtRegistrationID == PtRegistrationID
                && (obj as TreatmentHistoryGroupContent).InPt == InPt;
        }
    }
}