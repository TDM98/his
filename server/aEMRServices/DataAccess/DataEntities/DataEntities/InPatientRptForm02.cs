using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    [DataContract]
    public partial class InPatientRptForm02 : NotifyChangedBase
    {
        private long _rptForm02_InPtID;
        [DataMemberAttribute()]
        public long RptForm02_InPtID
        {
            get
            {
                return _rptForm02_InPtID;
            }
            set
            {
                if (_rptForm02_InPtID != value)
                {
                    _rptForm02_InPtID = value;
                    RaisePropertyChanged("RptForm02_InPtID");
                }
            }
        }

        private long _ptRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _ptRegistrationID;
            }
            set
            {
                if (_ptRegistrationID != value)
                {
                    _ptRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                }
            }
        }

        private string _description;
        [DataMemberAttribute()]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        private DateTime? _fromDate;
        [DataMemberAttribute()]
        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }


        private DateTime? _toDate;
        [DataMemberAttribute()]
        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }

        private long _V_Form02Type;
        [DataMemberAttribute()]
        public long V_Form02Type
        {
            get
            {
                return _V_Form02Type;
            }
            set
            {
                if (_V_Form02Type != value)
                {
                    _V_Form02Type = value;
                    RaisePropertyChanged("V_Form02Type");
                }
            }
        }

        private RefDepartment _department;
        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get { return _department; }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    RaisePropertyChanged("Department");
                }
            }
        }

        private long _staffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _staffID;
            }
            set
            {
                if (_staffID != value)
                {
                    _staffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private string _staffName;
        [DataMemberAttribute()]
        public string StaffName
        {
            get
            {
                return _staffName;
            }
            set
            {
                if (_staffName != value)
                {
                    _staffName = value;
                    RaisePropertyChanged("StaffName");
                }
            }
        }

        private DateTime _recCreatedDate;
        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _recCreatedDate;
            }
            set
            {
                if (_recCreatedDate != value)
                {
                    _recCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }

        private bool _checked;
        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    RaisePropertyChanged("Checked");
                }
            }
        }

        private string _note;
        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                if (_note != value)
                {
                    _note = value;
                    RaisePropertyChanged("Note");
                }
            }
        }

    }
}