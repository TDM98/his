using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class RefInternalReceiptType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefInternalReceiptType object.

        /// <param name="intRcptTypeID">Initial value of the IntRcptTypeID property.</param>
        /// <param name="intRcptTypeName">Initial value of the IntRcptTypeName property.</param>
        /// <param name="debitOrCredit">Initial value of the DebitOrCredit property.</param>
        public static RefInternalReceiptType CreateRefInternalReceiptType(Int64 intRcptTypeID, String intRcptTypeName, Int16 debitOrCredit)
        {
            RefInternalReceiptType refInternalReceiptType = new RefInternalReceiptType();
            refInternalReceiptType.IntRcptTypeID = intRcptTypeID;
            refInternalReceiptType.IntRcptTypeName = intRcptTypeName;
            refInternalReceiptType.DebitOrCredit = debitOrCredit;
            return refInternalReceiptType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 IntRcptTypeID
        {
            get
            {
                return _IntRcptTypeID;
            }
            set
            {
                if (_IntRcptTypeID != value)
                {
                    OnIntRcptTypeIDChanging(value);
                    ////ReportPropertyChanging("IntRcptTypeID");
                    _IntRcptTypeID = value;
                    RaisePropertyChanged("IntRcptTypeID");
                    OnIntRcptTypeIDChanged();
                }
            }
        }
        private Int64 _IntRcptTypeID;
        partial void OnIntRcptTypeIDChanging(Int64 value);
        partial void OnIntRcptTypeIDChanged();





        [DataMemberAttribute()]
        public String IntRcptTypeName
        {
            get
            {
                return _IntRcptTypeName;
            }
            set
            {
                OnIntRcptTypeNameChanging(value);
                ////ReportPropertyChanging("IntRcptTypeName");
                _IntRcptTypeName = value;
                RaisePropertyChanged("IntRcptTypeName");
                OnIntRcptTypeNameChanged();
            }
        }
        private String _IntRcptTypeName;
        partial void OnIntRcptTypeNameChanging(String value);
        partial void OnIntRcptTypeNameChanged();





        [DataMemberAttribute()]
        public Int16 DebitOrCredit
        {
            get
            {
                return _DebitOrCredit;
            }
            set
            {
                OnDebitOrCreditChanging(value);
                ////ReportPropertyChanging("DebitOrCredit");
                _DebitOrCredit = value;
                RaisePropertyChanged("DebitOrCredit");
                OnDebitOrCreditChanged();
            }
        }
        private Int16 _DebitOrCredit;
        partial void OnDebitOrCreditChanging(Int16 value);
        partial void OnDebitOrCreditChanged();

        #endregion
    }
}
