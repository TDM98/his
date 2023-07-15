using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class UserGroupSearchCriteria : SearchCriteriaBase
    {
        public UserGroupSearchCriteria()
        {

        }
        private string _groupName;

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                _groupName = value;
                RaisePropertyChanged("GroupName");
            }
        }
    }
}