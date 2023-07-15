using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{

    public partial class ContractDetail : NotifyChangedBase, IEditableObject
    {
        public ContractDetail()
            : base()
        {

        }

        private ContractDetail _tempContractDetail;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempContractDetail = (ContractDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempContractDetail)
                CopyFrom(_tempContractDetail);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ContractDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ContractDetail object.

        /// <param name="contractDetailID">Initial value of the ContractDetailID property.</param>
        /// <param name="inwardRscrID">Initial value of the InwardRscrID property.</param>
        /// <param name="contarctID">Initial value of the ContarctID property.</param>
        public static ContractDetail CreateContractDetail(Int64 contractDetailID, long inwardRscrID, long contarctID)
        {
            ContractDetail contractDetail = new ContractDetail();
            contractDetail.ContractDetailID = contractDetailID;
            contractDetail.InwardRscrID = inwardRscrID;
            contractDetail.ContarctID = contarctID;
            return contractDetail;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ContractDetailID
        {
            get
            {
                return _ContractDetailID;
            }
            set
            {
                if (_ContractDetailID != value)
                {
                    OnContractDetailIDChanging(value);
                    _ContractDetailID = value;
                    RaisePropertyChanged("ContractDetailID");
                    OnContractDetailIDChanged();
                }
            }
        }
        private Int64 _ContractDetailID;
        partial void OnContractDetailIDChanging(Int64 value);
        partial void OnContractDetailIDChanged();

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
        public long InwardRscrID
        {
            get
            {
                return _InwardRscrID;
            }
            set
            {
                OnInwardRscrIDChanging(value);
                _InwardRscrID = value;
                RaisePropertyChanged("InwardRscrID");
                OnInwardRscrIDChanged();
            }
        }
        private long _InwardRscrID;
        partial void OnInwardRscrIDChanging(long value);
        partial void OnInwardRscrIDChanged();

        [DataMemberAttribute()]
        public long ContarctID
        {
            get
            {
                return _ContarctID;
            }
            set
            {
                OnContarctIDChanging(value);
                _ContarctID = value;
                RaisePropertyChanged("ContarctID");
                OnContarctIDChanged();
            }
        }
        private long _ContarctID;
        partial void OnContarctIDChanging(long value);
        partial void OnContarctIDChanged();

        [DataMemberAttribute()]
        public Nullable<Byte> Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                OnQtyChanging(value);
                _Qty = value;
                RaisePropertyChanged("Qty");
                OnQtyChanged();
            }
        }
        private Nullable<Byte> _Qty;
        partial void OnQtyChanging(Nullable<Byte> value);
        partial void OnQtyChanged();

        [DataMemberAttribute()]
        public Nullable<Double> InwUnitPrice
        {
            get
            {
                return _InwUnitPrice;
            }
            set
            {
                OnInwUnitPriceChanging(value);
                _InwUnitPrice = value;
                RaisePropertyChanged("InwUnitPrice");
                OnInwUnitPriceChanged();
            }
        }
        private Nullable<Double> _InwUnitPrice;
        partial void OnInwUnitPriceChanging(Nullable<Double> value);
        partial void OnInwUnitPriceChanged();


        [DataMemberAttribute()]
        public String WarrantyNo
        {
            get
            {
                return _WarrantyNo;
            }
            set
            {
                OnWarrantyNoChanging(value);
                _WarrantyNo = value;
                RaisePropertyChanged("WarrantyNo");
                OnWarrantyNoChanged();
            }
        }
        private String _WarrantyNo;
        partial void OnWarrantyNoChanging(String value);
        partial void OnWarrantyNoChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Contract Contract
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public InwardResource InwardResource
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
