using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class RefDepartmentsSearchCriteria : SearchCriteriaBase
    {
        public RefDepartmentsSearchCriteria()
        {

        }
        
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
        private string _DeptName;

        [DataMemberAttribute()]
        public bool ShowDeptLocation
        {
            get { return _ShowDeptLocation; }
            set 
            { 
                _ShowDeptLocation = value;
                RaisePropertyChanged("ShowDeptLocation");
            }
        }
        private bool _ShowDeptLocation;
    }
}
