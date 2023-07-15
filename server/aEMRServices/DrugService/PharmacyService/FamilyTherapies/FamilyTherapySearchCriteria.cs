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
    public class FamilyTherapySearchCriteria : SearchCriteriaBase
    {
        public FamilyTherapySearchCriteria()
        {

        }
        private string _faName;
        public string FaName
        {
            get
            {
                return _faName;
            }
            set
            {
                _faName = value;
                RaisePropertyChanged("FaName");
            }
        }
        public Expression<Func<RefFamilyTherapy, bool>> FaNameFilter
        {
            get
            {
                return familytherapy => familytherapy.FaName.Contains(_faName);
            }
        }
    }

}
