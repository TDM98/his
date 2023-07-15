using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLExamTypeMedServiceDefItemSearchCriteria:SearchCriteriaBase
    {
        public PCLExamTypeMedServiceDefItemSearchCriteria()
        { 
        }

        private Nullable<Int64> _MedServiceID;
        public Nullable<Int64> MedServiceID
        {
          get { return _MedServiceID; }
          set { 
              _MedServiceID = value; 
              RaisePropertyChanged("MedServiceID");
          }
        }

        private Nullable<Int64> _PCLExamGroupID;
        public Nullable<Int64> PCLExamGroupID
        {
            get
            {
                return _PCLExamGroupID;
            }
            set
            {
                _PCLExamGroupID = value;
                RaisePropertyChanged("PCLExamGroupID");
            }
        }

        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set { 
                _PCLExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
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

        
        private string _strArrPCLExamTypeID;
        public string StrArrPCLExamTypeID
        {
            get { return _strArrPCLExamTypeID; }
            set { 
                _strArrPCLExamTypeID = value;
                RaisePropertyChanged("StrArrPCLExamTypeID");
            }
        }

        private string _strOrderCHARINDEX;
        public string StrOrderCHARINDEX
        {
            get { return _strOrderCHARINDEX; }
            set { 
                _strOrderCHARINDEX = value;
                RaisePropertyChanged("StrOrderCHARINDEX");
            }
        }



    }
}
