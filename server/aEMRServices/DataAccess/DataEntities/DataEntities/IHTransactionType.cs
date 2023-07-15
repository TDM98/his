using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class IHTransactionType : NotifyChangedBase, IEditableObject
    {
        public IHTransactionType()
            : base()
        {

        }

        private IHTransactionType _tempIHTransactionType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempIHTransactionType = (IHTransactionType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempIHTransactionType)
                CopyFrom(_tempIHTransactionType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(IHTransactionType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new IHTransactionType object.

        /// <param name="hITTypeID">Initial value of the HITTypeID property.</param>
        /// <param name="hITypeName">Initial value of the HITypeName property.</param>
        public static IHTransactionType CreateIHTransactionType(long hITTypeID, String hITypeName)
        {
            IHTransactionType iHTransactionType = new IHTransactionType();
            iHTransactionType.HITTypeID = hITTypeID;
            iHTransactionType.HITypeName = hITypeName;
            return iHTransactionType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                if (_HITTypeID != value)
                {
                    OnHITTypeIDChanging(value);
                    ////ReportPropertyChanging("HITTypeID");
                    _HITTypeID = value;
                    RaisePropertyChanged("HITTypeID");
                    OnHITTypeIDChanged();
                }
            }
        }
        private long _HITTypeID;
        partial void OnHITTypeIDChanging(long value);
        partial void OnHITTypeIDChanged();





        [DataMemberAttribute()]
        public String HITypeName
        {
            get
            {
                return _HITypeName;
            }
            set
            {
                OnHITypeNameChanging(value);
                ////ReportPropertyChanging("HITypeName");
                _HITypeName = value;
                RaisePropertyChanged("HITypeName");
                OnHITypeNameChanged();
            }
        }
        private String _HITypeName;
        partial void OnHITypeNameChanging(String value);
        partial void OnHITypeNameChanged();





        [DataMemberAttribute()]
        public String HITypeDescription
        {
            get
            {
                return _HITypeDescription;
            }
            set
            {
                OnHITypeDescriptionChanging(value);
                ////ReportPropertyChanging("HITypeDescription");
                _HITypeDescription = value;
                RaisePropertyChanged("HITypeDescription");
                OnHITypeDescriptionChanged();
            }
        }
        private String _HITypeDescription;
        partial void OnHITypeDescriptionChanging(String value);
        partial void OnHITypeDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HISERVIC_REL_HOSFM_IHTRANSA", "HIServices")]
        public ObservableCollection<HIService> HIServices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_HOSFM_IHTRANSA", "OutwardDMedRscrInvoices")]
        public ObservableCollection<OutwardDMedRscrInvoice> OutwardDMedRscrInvoices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_HOSFM_IHTRANSA", "OutwardInvoiceBlood")]
        public ObservableCollection<OutwardInvoiceBlood> OutwardInvoiceBloods
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWD_REL_HOSFM_IHTRANT", "OutwardDrugInvoices")]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get;
            set;
        }

        #endregion
    }
}
