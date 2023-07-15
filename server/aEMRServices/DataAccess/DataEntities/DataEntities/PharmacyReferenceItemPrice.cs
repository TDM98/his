using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Text;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public class PharmacyReferenceItemPrice : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 ReferenceItemPriceID
        {
            get { return _ReferenceItemPriceID; }
            set
            {
                if (_ReferenceItemPriceID != value)
                {
                    _ReferenceItemPriceID = value;
                    RaisePropertyChanged("ReferenceItemPriceID");
                }
            }
        }
        private Int64 _ReferenceItemPriceID;

        [DataMemberAttribute()]
        public Int64 ReferencePriceListID
        {
            get { return _ReferencePriceListID; }
            set
            {
                if (_ReferencePriceListID != value)
                {
                    _ReferencePriceListID = value;
                    RaisePropertyChanged("ReferencePriceListID");
                }
            }
        }
        private Int64 _ReferencePriceListID;

        [DataMemberAttribute()]
        public RefGenericDrugDetail Drug
        {
            get { return _Drug; }
            set
            {
                if (_Drug != value)
                {
                    _Drug = value;
                    RaisePropertyChanged("Drug");
                }
            }
        }
        private RefGenericDrugDetail _Drug;


        [DataMemberAttribute()]
        public decimal ContractPriceAfterVAT
        {
            get { return _ContractPriceAfterVAT; }
            set
            {
                if (_ContractPriceAfterVAT != value)
                {
                    _ContractPriceAfterVAT = value;
                    RaisePropertyChanged("ContractPriceAfterVAT");
                }
            }
        }
        private decimal _ContractPriceAfterVAT;

        [DataMemberAttribute()]
        public decimal ContractPriceAfterVAT_Old
        {
            get { return _ContractPriceAfterVAT_Old; }
            set
            {
                if (_ContractPriceAfterVAT_Old != value)
                {
                    _ContractPriceAfterVAT_Old = value;
                    RaisePropertyChanged("ContractPriceAfterVAT_Old");
                }
            }
        }
        private decimal _ContractPriceAfterVAT_Old;

        [DataMemberAttribute()]
        public decimal HIAllowedPrice
        {
            get { return _HIAllowedPrice; }
            set
            {
                if (_HIAllowedPrice != value)
                {
                    _HIAllowedPrice = value;
                    RaisePropertyChanged("HIAllowedPrice");
                }
            }
        }
        private decimal _HIAllowedPrice;

        [DataMemberAttribute()]
        public decimal HIAllowedPrice_Old
        {
            get { return _HIAllowedPrice_Old; }
            set
            {
                if (_HIAllowedPrice_Old != value)
                {
                    _HIAllowedPrice_Old = value;
                    RaisePropertyChanged("HIAllowedPrice_Old");
                }
            }
        }
        private decimal _HIAllowedPrice_Old;


        private CommonRecordState _RecordState = CommonRecordState.UNCHANGED;
        [DataMemberAttribute()]
        public CommonRecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                _RecordState = value;
                RaisePropertyChanged("RecordState");
            }
        }
    }
}
