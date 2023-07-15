using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using eHCMSLanguage;

namespace DataEntities
{
    public class DiagnosysConsultationSummary: NotifyChangedBase
    {
        private long _DiagConsultationSummaryID;
        [DataMemberAttribute]
        public long DiagConsultationSummaryID
        {
            get
            {
                return _DiagConsultationSummaryID;
            }
            set
            {
                _DiagConsultationSummaryID = value;
                RaisePropertyChanged("DiagConsultationSummaryID");
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
        private DateTime _ConsultationDate;
        [DataMemberAttribute]
        public DateTime ConsultationDate
        {
            get
            {
                return _ConsultationDate;
            }
            set
            {
                _ConsultationDate = value;
                RaisePropertyChanged("ConsultationDate");
            }
        }

        private string _ConsultationDiagnosis;
        [DataMemberAttribute]
        public string ConsultationDiagnosis
        {
            get
            {
                return _ConsultationDiagnosis;
            }
            set
            {
                _ConsultationDiagnosis = value;
                RaisePropertyChanged("ConsultationDiagnosis");
            }
        }
        private string _ConsultationResult;
        [DataMemberAttribute]
        public string ConsultationResult
        {
            get
            {
                return _ConsultationResult;
            }
            set
            {
                _ConsultationResult = value;
                RaisePropertyChanged("ConsultationResult");
            }
        }
        private DateTime _RecCreateDate;
        [DataMemberAttribute]
        public DateTime RecCreateDate
        {
            get
            {
                return _RecCreateDate;
            }
            set
            {
                _RecCreateDate = value;
                RaisePropertyChanged("RecCreateDate");
            }
        }
        private long _StaffID;
        [DataMemberAttribute]
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
        private string _FullName;
        [DataMemberAttribute]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private string _V_DiagnosysConsultationMask;
        [DataMemberAttribute]
        public string V_DiagnosysConsultationMask
        {
            get
            {
                return _V_DiagnosysConsultationMask;
            }
            set
            {
                _V_DiagnosysConsultationMask = value;
                RaisePropertyChanged("V_DiagnosysConsultationMask");
            }
        }
        private DateTime _ModifiedDate;
        [DataMemberAttribute]
        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                _ModifiedDate = value;
                RaisePropertyChanged("ModifiedDate");
            }
        }
        private long _ModifiedStaffID;
        [DataMemberAttribute]
        public long ModifiedStaffID
        {
            get
            {
                return _ModifiedStaffID;
            }
            set
            {
                _ModifiedStaffID = value;
                RaisePropertyChanged("ModifiedStaffID");
            }
        }
        private bool _MarkDeleted;
        [DataMemberAttribute]
        public bool MarkDeleted
        {
            get
            {
                return _MarkDeleted;
            }
            set
            {
                _MarkDeleted = value;
                RaisePropertyChanged("MarkDeleted");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute]
        public long PtRegistrationID
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
        private long _V_RegistrationType;
        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private List<Staff> _StaffList;
        [DataMemberAttribute]
        public List<Staff> StaffList
        {
            get
            {
                return _StaffList;
            }
            set
            {
                _StaffList = value;
                RaisePropertyChanged("StaffList");
            }
        }

        private List<DiagnosisIcd10Items> _ICD10List;
        [DataMemberAttribute]
        public List<DiagnosisIcd10Items> ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                _ICD10List = value;
                RaisePropertyChanged("ICD10List");
            }
        }
        private long _V_DiagnosysConsultation;
        [DataMemberAttribute]
        public long V_DiagnosysConsultation
        {
            get
            {
                return _V_DiagnosysConsultation;
            }
            set
            {
                _V_DiagnosysConsultation = value;
                RaisePropertyChanged("V_DiagnosysConsultation");
            }
        }
        private string _Title;
        [DataMemberAttribute]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        private long _PresiderStaffID;
        [DataMemberAttribute]
        public long PresiderStaffID
        {
            get
            {
                return _PresiderStaffID;
            }
            set
            {
                _PresiderStaffID = value;
                RaisePropertyChanged("PresiderStaffID");
            }
        }
        private long _SecretaryStaffID;
        [DataMemberAttribute]
        public long SecretaryStaffID
        {
            get
            {
                return _SecretaryStaffID;
            }
            set
            {
                _SecretaryStaffID = value;
                RaisePropertyChanged("SecretaryStaffID");
            }
        }
        private string _ConsultationSummary;
        [DataMemberAttribute]
        public string ConsultationSummary
        {
            get
            {
                return _ConsultationSummary;
            }
            set
            {
                _ConsultationSummary = value;
                RaisePropertyChanged("ConsultationSummary");
            }
        }
        private string _ConsultationTreatment;
        [DataMemberAttribute]
        public string ConsultationTreatment
        {
            get
            {
                return _ConsultationTreatment;
            }
            set
            {
                _ConsultationTreatment = value;
                RaisePropertyChanged("ConsultationTreatment");
            }
        }
    }
}
