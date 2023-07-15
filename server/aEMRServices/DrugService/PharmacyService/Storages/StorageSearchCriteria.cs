using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using EFData;
using eHCMS.LinqKit;

namespace PharmacyService
{
    public class StorageSearchCriteria:SearchCriteriaBase
    {
        public StorageSearchCriteria()
        {

        }
        private string _swhlname;
        public string swhlName
        {
            get
            {
                return _swhlname;
            }
            set
            {
                _swhlname = value;
                RaisePropertyChanged("swhlName");
            }
        }
        public Expression<Func<RefStorageWarehouseLocation, bool>> NameFilter
        {
            get
            {
                return storage => storage.swhlName.Contains(_swhlname);
            }
        }
    }
}
