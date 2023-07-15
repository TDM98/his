using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
/*
 * 20230502 #001 QTD:   Thêm trường cho phiếu sơ kết 15 ngày điều trị
 */
namespace DataEntities
{
    public partial class TreatmentProcess : EntityBase
    {
        private HealthInsurance _UsingHealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance UsingHealthInsurance
        {
            get
            {
                return _UsingHealthInsurance;
            }
            set
            {
                _UsingHealthInsurance = value;
                RaisePropertyChanged("UsingHealthInsurance");
            }
        }
        private PatientRegistration _CurPatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration CurPatientRegistration
        {
            get
            {
                return _CurPatientRegistration;
            }
            set
            {
                _CurPatientRegistration = value;
                RaisePropertyChanged("CurPatientRegistration");
            }
        }
        private long _TreatmentProcessID;
        [DataMemberAttribute()]
        public long TreatmentProcessID
        {
            get
            {
                return _TreatmentProcessID;
            }
            set
            {
                _TreatmentProcessID = value;
                RaisePropertyChanged("TreatmentProcessID");
            }
        }
        private string _PathologicalProcess;
        [DataMemberAttribute()]
        public string PathologicalProcess
        {
            get
            {
                return _PathologicalProcess;
            }
            set
            {
                _PathologicalProcess = value;
                RaisePropertyChanged("PathologicalProcess");
            }
        }
        private string _PCLResult;
        [DataMemberAttribute()]
        public string PCLResult
        {
            get
            {
                return _PCLResult;
            }
            set
            {
                _PCLResult = value;
                RaisePropertyChanged("PCLResult");
            }
        }
        private string _Diagnosis;
        [DataMemberAttribute()]
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
            }
        }
        private string _Treatments;
        [DataMemberAttribute()]
        public string Treatments
        {
            get
            {
                return _Treatments;
            }
            set
            {
                _Treatments = value;
                RaisePropertyChanged("Treatments");
            }
        }
        private string _DischargedCondition;
        [DataMemberAttribute()]
        public string DischargedCondition
        {
            get
            {
                return _DischargedCondition;
            }
            set
            {
                _DischargedCondition = value;
                RaisePropertyChanged("DischargedCondition");
            }
        }
        private DateTime _FromDate;
        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private DateTime _ToDate;
        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }
        private string _Note;
        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }

        //▼====: #001
        private string _TreatmentsProcess;
        [DataMemberAttribute()]
        public string TreatmentsProcess
        {
            get
            {
                return _TreatmentsProcess;
            }
            set
            {
                _TreatmentsProcess = value;
                RaisePropertyChanged("TreatmentsProcess");
            }
        }

        private string _ResultsEvaluation;
        [DataMemberAttribute()]
        public string ResultsEvaluation
        {
            get
            {
                return _ResultsEvaluation;
            }
            set
            {
                _ResultsEvaluation = value;
                RaisePropertyChanged("ResultsEvaluation");
            }
        }

        private string _Prognosis;
        [DataMemberAttribute()]
        public string Prognosis
        {
            get
            {
                return _Prognosis;
            }
            set
            {
                _Prognosis = value;
                RaisePropertyChanged("Prognosis");
            }
        }

        private long _DoctorStaffID;
        [DataMemberAttribute()]
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

        private long _HeadOfDepartmentDoctorStaffID;
        [DataMemberAttribute()]
        public long HeadOfDepartmentDoctorStaffID
        {
            get
            {
                return _HeadOfDepartmentDoctorStaffID;
            }
            set
            {
                _HeadOfDepartmentDoctorStaffID = value;
                RaisePropertyChanged("HeadOfDepartmentDoctorStaffID");
            }
        }

        private string _DeptName;
        [DataMemberAttribute()]
        public string DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                _DeptName = value;
                RaisePropertyChanged("DeptName");
            }
        }

        private string _LocationName;
        [DataMemberAttribute()]
        public string LocationName
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

        private string _BedNumber;
        [DataMemberAttribute()]
        public string BedNumber
        {
            get
            {
                return _BedNumber;
            }
            set
            {
                _BedNumber = value;
                RaisePropertyChanged("BedNumber");
            }
        }
        //▲====: #001
    }
}
