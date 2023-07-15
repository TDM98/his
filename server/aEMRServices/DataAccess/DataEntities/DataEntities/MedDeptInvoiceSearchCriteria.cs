using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class MedDeptInvoiceSearchCriteria : SearchCriteriaBase
    {
        public MedDeptInvoiceSearchCriteria()
        {

        }

        private string _PatientNameString;
        public string PatientNameString
        {
            get
            {
                return _PatientNameString;
            }
            set
            {
                _PatientNameString = value;
                RaisePropertyChanged("PatientNameString");
            }
        }

        public string PMFCode
        {
            get
            {
                return _PMFCode;
            }
            set
            {
                _PMFCode = value;
                RaisePropertyChanged("PMFCode");
            }
        }
        private string _PMFCode;

        private string _CustomerName;
        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
                RaisePropertyChanged("CustomerName");
            }
        }

        private string _PatientCode;
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }

        private string _HICardNo;
        public string HICardNo
        {
            get
            {
                return _HICardNo;
            }
            set
            {
                _HICardNo = value;
                RaisePropertyChanged("HICardNo");
            }
        }

        private string _CodeInvoice;
        public string CodeInvoice
        {
            get
            {
                return _CodeInvoice;
            }
            set
            {
                _CodeInvoice = value;
                RaisePropertyChanged("CodeInvoice");
            }
        }

        private string _CodeRequest;
        public string CodeRequest
        {
            get
            {
                return _CodeRequest;
            }
            set
            {
                _CodeRequest = value;
                RaisePropertyChanged("CodeRequest");
            }
        }
     
        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

         private long? _StoreID;
        public long? StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }

          private long? _TypID;
        public long? TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                _TypID = value;
                RaisePropertyChanged("TypID");
            }
        }
          private long? _V_MedProductType;
        public long? V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
            }
        }

    }
}
