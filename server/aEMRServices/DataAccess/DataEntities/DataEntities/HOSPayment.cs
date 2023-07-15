using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
namespace DataEntities
{
    public class HOSPayment : NotifyChangedBase
    {
        private long _HOSPaymentID;
        [DataMemberAttribute()]
        public long HOSPaymentID
        {
            get
            {
                return _HOSPaymentID;
            }
            set
            {
                _HOSPaymentID = value;
                RaisePropertyChanged("HOSPaymentID");
            }
        }

        private long _V_PayReson;
        [DataMemberAttribute()]
        public long V_PayReson
        {
            get
            {
                return _V_PayReson;
            }
            set
            {
                _V_PayReson = value;
                RaisePropertyChanged("V_PayReson");
            }
        }

        private Lookup _PayReson;
        [DataMemberAttribute()]
        public Lookup PayReson
        {
            get
            {
                return _PayReson;
            }
            set
            {
                _PayReson = value;
                RaisePropertyChanged("PayReson");
            }
        }

        private DateTime _PaymentDate;
        [DataMemberAttribute()]
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
            }
        }

        private DateTime _TransactionDate;
        [DataMemberAttribute()]
        public DateTime TransactionDate
        {
            get
            {
                return _TransactionDate;
            }
            set
            {
                _TransactionDate = value;
                RaisePropertyChanged("TransactionDate");
            }
        }

        private string _PaymentReson;
        [DataMemberAttribute()]
        public string PaymentReson
        {
            get
            {
                return _PaymentReson;
            }
            set
            {
                _PaymentReson = value;
                RaisePropertyChanged("PaymentReson");
            }
        }

        private string _PaymentNotice;
        [DataMemberAttribute()]
        public string PaymentNotice
        {
            get
            {
                return _PaymentNotice;
            }
            set
            {
                _PaymentNotice = value;
                RaisePropertyChanged("PaymentNotice");
            }
        }

        private decimal _PaymentAmount;
        [DataMemberAttribute()]
        public decimal PaymentAmount
        {
            get
            {
                return _PaymentAmount;
            }
            set
            {
                _PaymentAmount = value;
                RaisePropertyChanged("PaymentAmount");
            }
        }

        private long _V_CharityObjectType;
        [DataMemberAttribute()]
        public long V_CharityObjectType
        {
            get
            {
                return _V_CharityObjectType;
            }
            set
            {
                _V_CharityObjectType = value;
                RaisePropertyChanged("V_CharityObjectType");
            }
        }

        private long _V_PatientSubject;
        [DataMemberAttribute()]
        public long V_PatientSubject
        {
            get
            {
                return _V_PatientSubject;
            }
            set
            {
                _V_PatientSubject = value;
                RaisePropertyChanged("V_PatientSubject");
            }
        }
        
        private int _NumbOfPerson;
        [DataMemberAttribute()]
        public int NumbOfPerson
        {
            get
            {
                return _NumbOfPerson;
            }
            set
            {
                _NumbOfPerson = value;
                RaisePropertyChanged("NumbOfPerson");
            }
        }

        private string _PatientName;
        [DataMemberAttribute()]
        public string PatientName
        {
            get
            {
                return _PatientName;
            }
            set
            {
                _PatientName = value;
                RaiseErrorsChanged("PatientName");
            }
        }

        private DateTime? _DOB;
        [DataMemberAttribute()]
        public DateTime? DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                _DOB = value;
                RaisePropertyChanged("DOB");
            }
        }
    }
}