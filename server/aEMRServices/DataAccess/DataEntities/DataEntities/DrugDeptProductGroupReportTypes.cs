using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System;
namespace DataEntities
{
    public class DrugDeptProductGroupReportType : EntityBase
    {
        private long _DrugDeptProductGroupReportTypeID;
        [DataMemberAttribute()]
        public long DrugDeptProductGroupReportTypeID
        {
            get
            {
                return _DrugDeptProductGroupReportTypeID;
            }
            set
            {
                if (_DrugDeptProductGroupReportTypeID != value)
                {
                    _DrugDeptProductGroupReportTypeID = value;
                    RaisePropertyChanged("DrugDeptProductGroupReportTypeID");
                }
            }
        }

        private string _DrugDeptProductGroupReportTypeCode;
        [DataMemberAttribute()]
        public string DrugDeptProductGroupReportTypeCode
        {
            get
            {
                return _DrugDeptProductGroupReportTypeCode;
            }
            set
            {
                if (_DrugDeptProductGroupReportTypeCode != value)
                {
                    _DrugDeptProductGroupReportTypeCode = value;
                    RaisePropertyChanged("DrugDeptProductGroupReportTypeCode");
                }
            }
        }

        private string _DrugDeptProductGroupReportTypeName;
        [DataMemberAttribute()]
        public string DrugDeptProductGroupReportTypeName
        {
            get
            {
                return _DrugDeptProductGroupReportTypeName;
            }
            set
            {
                if (_DrugDeptProductGroupReportTypeName != value)
                {
                    _DrugDeptProductGroupReportTypeName = value;
                    RaisePropertyChanged("DrugDeptProductGroupReportTypeName");
                }
            }
        }

        private bool _IsDeleted;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                }
            }
        }

        private DateTime _CreatedDate;
        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate != value)
                {
                    _CreatedDate = value;
                    RaisePropertyChanged("CreatedDate");
                }
            }
        }
    }
}
