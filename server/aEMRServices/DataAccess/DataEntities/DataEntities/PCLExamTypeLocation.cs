using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLExamTypeLocation : NotifyChangedBase
    {
        private long _pclExamTypeLocID;
        [DataMemberAttribute]
        public long PCLExamTypeLocID
        {
            get { return _pclExamTypeLocID; }
            set
            {
                _pclExamTypeLocID = value;
                RaisePropertyChanged("PCLExamTypeLocID");
            }
        }     
        private long _pclExamTypeID;
        [DataMemberAttribute]
        public long PCLExamTypeID
        {
            get { return _pclExamTypeID; }
            set
            {
                _pclExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        private long _deptLocationID;
        [DataMemberAttribute]
        public long DeptLocationID
        {
            get { return _deptLocationID; }
            set
            {
                _deptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
            }
        }
        private bool? _isDeleted;
        [DataMemberAttribute]
        public bool? IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private DeptLocation _deptLocation;
        [DataMemberAttribute]
        public DeptLocation DeptLocation
        {
            get { return _deptLocation; }
            set
            {
                _deptLocation = value;
                RaisePropertyChanged("DeptLocation");
            }
        }
    }
}