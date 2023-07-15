using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PrescriptionSearchCriteria : NotifyChangedBase
    {
        public PrescriptionSearchCriteria()
        {

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

        [DataMemberAttribute()]
        public String FullName
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
        private String _FullName;

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

        [DataMemberAttribute()]
        public Nullable<Int64> PrescriptID
        {
            get
            {
                return _prescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _prescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private Nullable<Int64> _prescriptID;
        partial void OnPrescriptIDChanging(Nullable<Int64> value);
        partial void OnPrescriptIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PatientID
        {
            get
            {
                return _patientID;
            }
            set
            {
                _patientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private Nullable<Int64> _patientID;

        [DataMemberAttribute()]
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
        private string _patientCode;

        [DataMemberAttribute()]
        public string HICardCode
        {
            get
            {
                return _HICardCode;
            }
            set
            {
                _HICardCode = value;
                RaisePropertyChanged("HICardCode");
            }
        }
        private string _HICardCode;

        [DataMemberAttribute()]
        public Nullable<DateTime> FromDate
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
        private Nullable<DateTime> _FromDate;

        [DataMemberAttribute()]
        public Nullable<DateTime> ToDate
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
        private Nullable<DateTime> _ToDate;

        private string _CreatorStaffIDName;
        public string CreatorStaffIDName
        {
            get
            {
                return _CreatorStaffIDName;
            }
            set
            {
                _CreatorStaffIDName = value;
                RaisePropertyChanged("CreatorStaffIDName");
            }
        }

        private string _Diagnosis;
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


        private string _DoctorStaffIDName;
        public string DoctorStaffIDName
        {
            get
            {
                return _DoctorStaffIDName;
            }
            set
            {
                _DoctorStaffIDName = value;
                RaisePropertyChanged("DoctorStaffIDName");
            }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set
            {
                _OrderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        [DataMemberAttribute()]
        public long IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                _IssueID = value;
                RaisePropertyChanged("IssueID");
            }
        }
        private long _IssueID;
        [DataMemberAttribute()]
        public bool? IsInsurance
        {
            get
            {
                return _IsInsurance;
            }
            set
            {
                _IsInsurance = value;
                RaisePropertyChanged("IsInsurance");
            }
        }
        private bool? _IsInsurance;

        private string _PrescriptionIssueCode;
        [DataMemberAttribute()]
        public string PrescriptionIssueCode
        {
            get
            {
                return _PrescriptionIssueCode;
            }
            set
            {
                _PrescriptionIssueCode = value;
                RaisePropertyChanged("PrescriptionIssueCode");
            }
        }

        private long? _PtRegistrationID;
        [DataMemberAttribute]
        public long? PtRegistrationID
        {
            get => _PtRegistrationID; set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
    }
}