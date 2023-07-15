using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class StaffSearchCriteria : SearchCriteriaBase
    {
        public StaffSearchCriteria()
        {

        }
        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
                RaisePropertyChanged("FullName");
            }
        }

        private long _departmentID = -1;

        public long DepartmentID
        {
            get
            {
                return _departmentID;
            }
            set
            {
                _departmentID = value;
                RaisePropertyChanged("DepartmentID");
            }
        }
    }
}