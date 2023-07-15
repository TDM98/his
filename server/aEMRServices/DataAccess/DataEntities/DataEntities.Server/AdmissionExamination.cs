using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
/*
 * 20220309 #001 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 */

namespace DataEntities
{
    public partial class AdmissionExamination : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long AdmissionExaminationID
        {
            get
            {
                return _AdmissionExaminationID;
            }
            set
            {
                if (_AdmissionExaminationID == value)
                {
                    return;
                }
                _AdmissionExaminationID = value;
                RaisePropertyChanged("AdmissionExaminationID");
            }
        }
        private long _AdmissionExaminationID;

        [DataMemberAttribute()]
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
        private long _PtRegistrationID;


        [DataMemberAttribute()]
        public string ReferralDiagnosis
        {
            get
            {
                return _ReferralDiagnosis;
            }
            set
            {
                if (_ReferralDiagnosis == value)
                {
                    return;
                }
                _ReferralDiagnosis = value;
                RaisePropertyChanged("ReferralDiagnosis");
            }
        }
        private string _ReferralDiagnosis;

        [DataMemberAttribute]
        public string PathologicalProcess
        {
            get
            {
                return _PathologicalProcess;
            }
            set
            {
                if (_PathologicalProcess == value)
                {
                    return;
                }
                _PathologicalProcess = value;
                RaisePropertyChanged("PathologicalProcess");
            }
        }
        private string _PathologicalProcess;

        [DataMemberAttribute]
        public string MedicalHistory
        {
            get
            {
                return _MedicalHistory;
            }
            set
            {
                if (_MedicalHistory == value)
                {
                    return;
                }
                _MedicalHistory = value;
                RaisePropertyChanged("MedicalHistory");
            }
        }
        private string _MedicalHistory;
        [DataMemberAttribute]
        public string FamilyMedicalHistory
        {
            get
            {
                return _FamilyMedicalHistory;
            }
            set
            {
                if (_FamilyMedicalHistory == value)
                {
                    return;
                }
                _FamilyMedicalHistory = value;
                RaisePropertyChanged("FamilyMedicalHistory");
            }
        }
        private string _FamilyMedicalHistory;

        [DataMemberAttribute()]
        public string FullBodyExamination
        {
            get
            {
                return _FullBodyExamination;
            }
            set
            {
                if (_FullBodyExamination == value)
                {
                    return;
                }
                _FullBodyExamination = value;
                RaisePropertyChanged("FullBodyExamination");
            }
        }
        private string _FullBodyExamination;

        [DataMemberAttribute()]
        public string PartialExamination
        {
            get
            {
                return _PartialExamination;
            }
            set
            {
                if (_PartialExamination == value)
                {
                    return;
                }
                _PartialExamination = value;
                RaisePropertyChanged("PartialExamination");
            }
        }
        private string _PartialExamination;

        [DataMemberAttribute()]
        public string PclResult
        {
            get
            {
                return _PclResult;
            }
            set
            {
                if (_PclResult == value)
                {
                    return;
                }
                _PclResult = value;
                RaisePropertyChanged("PclResult");
            }
        }
        private string _PclResult;

        [DataMemberAttribute()]
        public string DrugTreatment
        {
            get
            {
                return _DrugTreatment;
            }
            set
            {
                if (_DrugTreatment == value)
                {
                    return;
                }
                _DrugTreatment = value;
                RaisePropertyChanged("DrugTreatment");
            }
        }
        private string _DrugTreatment;

        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public string LogModified
        {
            get
            {
                return _LogModified;
            }
            set
            {
                if (_LogModified == value)
                {
                    return;
                }
                _LogModified = value;
                RaisePropertyChanged("LogModified");
            }
        }
        private string _LogModified;

        #endregion
        //==== #001
        [DataMemberAttribute]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID == value)
                {
                    return;
                }
                _DeptID = value;
                RaisePropertyChanged("DeptID");
            }
        }
        private long _DeptID;

        [DataMemberAttribute]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes == value)
                {
                    return;
                }
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        private string _Notes;

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
        private string _ReasonAdmission;

        [DataMemberAttribute]
        public string DiagnosisResult
        {
            get
            {
                return _DiagnosisResult;
            }
            set
            {
                if (_DiagnosisResult == value)
                {
                    return;
                }
                _DiagnosisResult = value;
                RaisePropertyChanged("DiagnosisResult");
            }
        }
        private string _DiagnosisResult;

        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        private long _V_RegistrationType;
        //==== #001
    }
}
