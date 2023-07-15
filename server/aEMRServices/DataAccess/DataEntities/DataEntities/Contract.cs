using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class Contract : NotifyChangedBase, IEditableObject
    {
        public Contract()
            : base()
        {

        }

        private Contract _tempContract;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempContract = (Contract)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempContract)
                CopyFrom(_tempContract);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Contract p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new Contract object.

        /// <param name="contarctID">Initial value of the ContarctID property.</param>
        /// <param name="contractNo">Initial value of the ContractNo property.</param>
        /// <param name="contractDate">Initial value of the ContractDate property.</param>
        public static Contract CreateContract(long contarctID, String contractNo, DateTime contractDate)
        {
            Contract contract = new Contract();
            contract.ContarctID = contarctID;
            contract.ContractNo = contractNo;
            contract.ContractDate = contractDate;
            return contract;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long ContarctID
        {
            get
            {
                return _ContarctID;
            }
            set
            {
                if (_ContarctID != value)
                {
                    OnContarctIDChanging(value);
                    _ContarctID = value;
                    RaisePropertyChanged("ContarctID");
                    OnContarctIDChanged();
                }
            }
        }
        private long _ContarctID;
        partial void OnContarctIDChanging(long value);
        partial void OnContarctIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Nullable<long> _SupplierID;
        partial void OnSupplierIDChanging(Nullable<long> value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> RefDocID
        {
            get
            {
                return _RefDocID;
            }
            set
            {
                OnRefDocIDChanging(value);
                _RefDocID = value;
                RaisePropertyChanged("RefDocID");
                OnRefDocIDChanged();
            }
        }
        private Nullable<Int64> _RefDocID;
        partial void OnRefDocIDChanging(Nullable<Int64> value);
        partial void OnRefDocIDChanged();

        [DataMemberAttribute()]
        public String ContractNo
        {
            get
            {
                return _ContractNo;
            }
            set
            {
                OnContractNoChanging(value);
                _ContractNo = value;
                RaisePropertyChanged("ContractNo");
                OnContractNoChanged();
            }
        }
        private String _ContractNo;
        partial void OnContractNoChanging(String value);
        partial void OnContractNoChanged();

        [DataMemberAttribute()]
        public DateTime ContractDate
        {
            get
            {
                return _ContractDate;
            }
            set
            {
                OnContractDateChanging(value);
                _ContractDate = value;
                RaisePropertyChanged("ContractDate");
                OnContractDateChanged();
            }
        }
        private DateTime _ContractDate;
        partial void OnContractDateChanging(DateTime value);
        partial void OnContractDateChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<ContractDetail> ContractDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public Supplier Supplier
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefDocument RefDocument
        {
            get;
            set;
        }

        #endregion
    }
}
