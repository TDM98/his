using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class ResourceMaintenanceLogSearchCriteria : SearchCriteriaBase
    {
        public ResourceMaintenanceLogSearchCriteria()
        { 
        }

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
          get { return _FromDate; }
          set { _FromDate = value; 
            RaisePropertyChanged("FromDate");
          }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
          get { return _ToDate; }
          set { _ToDate = value; 
            RaisePropertyChanged("ToDate");
          }
        }

        private long _V_StatusIssueMaintenance;
        public long V_StatusIssueMaintenance
        {
            get { return _V_StatusIssueMaintenance; }
            set { 
                _V_StatusIssueMaintenance = value;
                RaisePropertyChanged("V_StatusIssueMaintenance");
            }
        }
        

        private string _LoggingIssue;
        public string LoggingIssue
        {
          get { return _LoggingIssue; }
          set { _LoggingIssue = value;
            RaisePropertyChanged("LoggingIssue");
          }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value;
            RaisePropertyChanged("OrderBy");
            }
        }

    }
}
