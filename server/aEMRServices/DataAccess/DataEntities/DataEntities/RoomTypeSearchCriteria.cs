using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RoomTypeSearchCriteria : SearchCriteriaBase
    {
        public RoomTypeSearchCriteria()
        {
        }

        private Int64 _V_RoomFunction;
        public Int64 V_RoomFunction
        {
            get { return _V_RoomFunction; }
            set 
            { 
                _V_RoomFunction = value;
                RaisePropertyChanged("V_RoomFunction");
            }
        }

        private string _RmTypeName;
        public string RmTypeName
        {
            get 
            { 
                return _RmTypeName; 
            }
            set 
            {
                _RmTypeName = value;
                RaisePropertyChanged("RmTypeName");
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

    }
}