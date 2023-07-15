using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public class ICDSearchCriteria: SearchCriteriaBase
    {
        public ICDSearchCriteria()
        {
        }
        private string _IDCode;
        public string IDCode
        {
            get { return _IDCode; }
            set
            {
                _IDCode = value;
                RaisePropertyChanged("IDCode");
            }
        }
        private string _ICD10Code;
        public string ICD10Code
        {
            get { return _ICD10Code; }
            set
            {
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
            }
        }

        private string _DiseaseNameVN;
        public string DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
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
        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
    }
}
