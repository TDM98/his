using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLExamTypeExamTestPrintSearchCriteria : SearchCriteriaBase
    {
        public PCLExamTypeExamTestPrintSearchCriteria()
        {
        }

        private String _Code;
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                RaisePropertyChanged("Code");
            }
        }


        private String _Name;
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
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
