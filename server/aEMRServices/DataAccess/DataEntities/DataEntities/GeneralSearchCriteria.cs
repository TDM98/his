using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class GeneralSearchCriteria : SearchCriteriaBase
    {
        public GeneralSearchCriteria()
        {

        }
        private string _FindName;
        public string FindName
        {
            get
            {
                return _FindName;
            }
            set
            {
                _FindName = value;
                RaisePropertyChanged("FindName");
            }
        }

        private string _FindCode;
        public string FindCode
        {
            get
            {
                return _FindCode;
            }
            set
            {
                _FindCode = value;
                RaisePropertyChanged("FindCode");
            }
        }

        private long? _FindID;
        public long? FindID
        {
            get
            {
                return _FindID;
            }
            set
            {
                _FindID = value;
                RaisePropertyChanged("FindID");
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
