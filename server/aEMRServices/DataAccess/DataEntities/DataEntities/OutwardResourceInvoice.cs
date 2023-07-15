using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardResourceInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OutwardResourceInvoice object.

        /// <param name="outRscrInvID">Initial value of the OutRscrInvID property.</param>
        /// <param name="outInvoiceNumber">Initial value of the OutInvoiceNumber property.</param>
        /// <param name="outDate">Initial value of the OutDate property.</param>
        public static OutwardResourceInvoice CreateOutwardResourceInvoice(long outRscrInvID, String outInvoiceNumber, DateTime outDate)
        {
            OutwardResourceInvoice outwardResourceInvoice = new OutwardResourceInvoice();
            outwardResourceInvoice.OutRscrInvID = outRscrInvID;
            outwardResourceInvoice.OutInvoiceNumber = outInvoiceNumber;
            outwardResourceInvoice.OutDate = outDate;
            return outwardResourceInvoice;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long OutRscrInvID
        {
            get
            {
                return _OutRscrInvID;
            }
            set
            {
                if (_OutRscrInvID != value)
                {
                    OnOutRscrInvIDChanging(value);
                    _OutRscrInvID = value;
                    RaisePropertyChanged("OutRscrInvID");
                    OnOutRscrInvIDChanged();
                }
            }
        }
        private long _OutRscrInvID;
        partial void OnOutRscrInvIDChanging(long value);
        partial void OnOutRscrInvIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                OnTypIDChanging(value);
                _TypID = value;
                RaisePropertyChanged("TypID");
                OnTypIDChanged();
            }
        }
        private Nullable<long> _TypID;
        partial void OnTypIDChanging(Nullable<long> value);
        partial void OnTypIDChanged();

        [DataMemberAttribute()]
        public String OutInvoiceNumber
        {
            get
            {
                return _OutInvoiceNumber;
            }
            set
            {
                OnOutInvoiceNumberChanging(value);
                _OutInvoiceNumber = value;
                RaisePropertyChanged("OutInvoiceNumber");
                OnOutInvoiceNumberChanged();
            }
        }
        private String _OutInvoiceNumber;
        partial void OnOutInvoiceNumberChanging(String value);
        partial void OnOutInvoiceNumberChanged();

        [DataMemberAttribute()]
        public DateTime OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                OnOutDateChanging(value);
                _OutDate = value;
                RaisePropertyChanged("OutDate");
                OnOutDateChanged();
            }
        }
        private DateTime _OutDate;
        partial void OnOutDateChanging(DateTime value);
        partial void OnOutDateChanged();

        [DataMemberAttribute()]
        public Nullable<Int32> ToStorage
        {
            get
            {
                return _ToStorage;
            }
            set
            {
                OnToStorageChanging(value);
                _ToStorage = value;
                RaisePropertyChanged("ToStorage");
                OnToStorageChanged();
            }
        }
        private Nullable<Int32> _ToStorage;
        partial void OnToStorageChanging(Nullable<Int32> value);
        partial void OnToStorageChanged();


        [DataMemberAttribute()]
        public String EprescriptionOrPtFullName
        {
            get
            {
                return _EprescriptionOrPtFullName;
            }
            set
            {
                OnEprescriptionOrPtFullNameChanging(value);
                _EprescriptionOrPtFullName = value;
                RaisePropertyChanged("EprescriptionOrPtFullName");
                OnEprescriptionOrPtFullNameChanged();
            }
        }
        private String _EprescriptionOrPtFullName;
        partial void OnEprescriptionOrPtFullNameChanging(String value);
        partial void OnEprescriptionOrPtFullNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM22_REFOUTPU", "RefOutputType")]
        public RefOutputType RefOutputType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM23_OUTWARDR", "OutwardResources")]
        public ObservableCollection<OutwardResource> OutwardResources
        {
            get;
            set;
        }

        #endregion
    }
}
