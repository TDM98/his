using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using eHCMSLanguage;

/*
 * 20180913 #001 TBL: Added ConsultDiagRepType
 */

namespace DataEntities
{
    public class ConsultingDiagnosysSearchCriteria : NotifyChangedBase
    {
        [DataMemberAttribute]
        public bool? IsApproved { get; set; }
        [DataMemberAttribute]
        public bool? IsLated { get; set; }
        [DataMemberAttribute]
        public bool? IsAllExamCompleted { get; set; }
        [DataMemberAttribute]
        public bool? IsSurgeryCompleted { get; set; }
        [DataMemberAttribute]
        public bool? IsWaitSurgery { get; set; }
        [DataMemberAttribute]
        public long? ConsultingDoctorStaffID { get; set; }
        [DataMemberAttribute]
        public bool? IsDuraGraft { get; set; }
        [DataMemberAttribute]
        public bool? IsWaitingExamCompleted { get; set; }
        [DataMemberAttribute]
        public bool? IsCancelSurgery { get; set; }


        private DateTime? _FromDate;
        [DataMemberAttribute]
        public DateTime? FromDate
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

        private DateTime? _ToDate;
        [DataMemberAttribute]
        public DateTime? ToDate
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

        private string _HICard;
        [DataMemberAttribute]
        public string HICard
        {
            get
            {
                return _HICard;
            }
            set
            {
                _HICard = value;
                RaisePropertyChanged("HICard");
            }
        }

        private string _PatientNameString;
        [DataMemberAttribute]
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

        private int? _PageIndex;
        [DataMemberAttribute]
        public int? PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
                RaisePropertyChanged("PageIndex");
            }
        }

        private int? _PageSize;
        [DataMemberAttribute]
        public int? PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
                RaisePropertyChanged("PageSize");
            }
        }
        [DataMemberAttribute]
        public bool? IsConsultingHistoryView { get; set; }
        /*▼====: #001*/
        private int _ConsultDiagRepType;
        [DataMemberAttribute]
        public int ConsultDiagRepType
        {
            get
            {
                return _ConsultDiagRepType;
            }
            set
            {
                _ConsultDiagRepType = value;
                RaisePropertyChanged("ConsultDiagRepType");
            }
        }
        /*▲====: #001*/
    }
}
