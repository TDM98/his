using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class UserSearchCriteria : SearchCriteriaBase
    {
        public UserSearchCriteria()
        {

        }
        private string _userName;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        //private string _groupID;

        //public string GroupID
        //{
        //    get
        //    {
        //        return _groupID;
        //    }
        //    set
        //    {
        //        _groupID = value;
        //        RaisePropertyChanged("GroupID");
        //    }
        //}

        private int _groupID=-1;

        public int GroupID
        {
            get
            {
                return _groupID;
            }
            set
            {
                _groupID = value;
                RaisePropertyChanged("GroupID");
            }
        }
    }
}