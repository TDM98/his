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
    public class SupplierSearchCriteria:SearchCriteriaBase
    {
        public SupplierSearchCriteria()
        {

        }
        private string _supplierName;
        public string SupplierName
        {
            get
            {
                return _supplierName;
            }
            set
            {
                _supplierName = value;
                RaisePropertyChanged("SupplierName");
            }
        }
        public Expression<Func<Supplier, bool>> SupplierNameFilter
        {
            get
            {
                return supplier => supplier.SupplierName.Contains(_supplierName);
            }
        }
    }
}
