using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class ResourceMaintenanceLogStatusSearchCriteria : SearchCriteriaBase
    {
        public ResourceMaintenanceLogStatusSearchCriteria()
        { 
        }

        private Int64 _RscrMaintLogID;
        public Int64 RscrMaintLogID
        {
            get { return _RscrMaintLogID; }
            set { 
                _RscrMaintLogID = value;
                RaisePropertyChanged("RscrMaintLogID");
            }
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
