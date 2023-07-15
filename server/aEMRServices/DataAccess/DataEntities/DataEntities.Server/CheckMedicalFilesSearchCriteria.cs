using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class CheckMedicalFilesSearchCriteria : NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
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
        private long _StaffID;

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                _DeptID = value;
                RaisePropertyChanged("DeptID");
            }
        }
        private long _DeptID;

        [DataMemberAttribute()]
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
        private string _PatientCode;

        [DataMemberAttribute()]
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
        private string _FullName;


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
        private DateTime _FromDate;


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
        private DateTime _ToDate;

        [DataMemberAttribute()]
        public long V_CheckMedicalFilesStatus
        {
            get
            {
                return _V_CheckMedicalFilesStatus;
            }
            set
            {
                _V_CheckMedicalFilesStatus = value;
                RaisePropertyChanged("V_CheckMedicalFilesStatus");
            }
        }
        private long _V_CheckMedicalFilesStatus;

        [DataMemberAttribute()]
        public bool NotDischarge
        {
            get
            {
                return _NotDischarge;
            }
            set
            {
                _NotDischarge = value;
                RaisePropertyChanged("NotDischarge");
            }
        }
        private bool _NotDischarge;
        #endregion

    }
}
