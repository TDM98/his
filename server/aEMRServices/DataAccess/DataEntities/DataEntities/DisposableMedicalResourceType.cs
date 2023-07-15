using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DisposableMedicalResourceType : NotifyChangedBase, IEditableObject
    {
        public DisposableMedicalResourceType()
            : base()
        {

        }

        private DisposableMedicalResourceType _tempDisposableMedicalResourceType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDisposableMedicalResourceType = (DisposableMedicalResourceType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDisposableMedicalResourceType)
                CopyFrom(_tempDisposableMedicalResourceType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DisposableMedicalResourceType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DisposableMedicalResourceType object.

        /// <param name="dMedRscrTypeID">Initial value of the DMedRscrTypeID property.</param>
        /// <param name="parDMedRscrTypeID">Initial value of the ParDMedRscrTypeID property.</param>
        /// <param name="dMedRscrTypeName">Initial value of the DMedRscrTypeName property.</param>
        public static DisposableMedicalResourceType CreateDisposableMedicalResourceType(long dMedRscrTypeID, long parDMedRscrTypeID, String dMedRscrTypeName)
        {
            DisposableMedicalResourceType disposableMedicalResourceType = new DisposableMedicalResourceType();
            disposableMedicalResourceType.DMedRscrTypeID = dMedRscrTypeID;
            disposableMedicalResourceType.ParDMedRscrTypeID = parDMedRscrTypeID;
            disposableMedicalResourceType.DMedRscrTypeName = dMedRscrTypeName;
            return disposableMedicalResourceType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DMedRscrTypeID
        {
            get
            {
                return _DMedRscrTypeID;
            }
            set
            {
                if (_DMedRscrTypeID != value)
                {
                    OnDMedRscrTypeIDChanging(value);
                    ////ReportPropertyChanging("DMedRscrTypeID");
                    _DMedRscrTypeID = value;
                    RaisePropertyChanged("DMedRscrTypeID");
                    OnDMedRscrTypeIDChanged();
                }
            }
        }
        private long _DMedRscrTypeID;
        partial void OnDMedRscrTypeIDChanging(long value);
        partial void OnDMedRscrTypeIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ParDMedRscrTypeID
        {
            get
            {
                return _ParDMedRscrTypeID;
            }
            set
            {
                OnParDMedRscrTypeIDChanging(value);
                ////ReportPropertyChanging("ParDMedRscrTypeID");
                _ParDMedRscrTypeID = value;
                RaisePropertyChanged("ParDMedRscrTypeID");
                OnParDMedRscrTypeIDChanged();
            }
        }
        private Nullable<long> _ParDMedRscrTypeID;
        partial void OnParDMedRscrTypeIDChanging(Nullable<long> value);
        partial void OnParDMedRscrTypeIDChanged();





        [DataMemberAttribute()]
        public String DMedRscrTypeName
        {
            get
            {
                return _DMedRscrTypeName;
            }
            set
            {
                OnDMedRscrTypeNameChanging(value);
                ////ReportPropertyChanging("DMedRscrTypeName");
                _DMedRscrTypeName = value;
                RaisePropertyChanged("DMedRscrTypeName");
                OnDMedRscrTypeNameChanged();
            }
        }
        private String _DMedRscrTypeName;
        partial void OnDMedRscrTypeNameChanging(String value);
        partial void OnDMedRscrTypeNameChanged();





        [DataMemberAttribute()]
        public String DMedRscrTypeDescription
        {
            get
            {
                return _DMedRscrTypeDescription;
            }
            set
            {
                OnDMedRscrTypeDescriptionChanging(value);
                ////ReportPropertyChanging("DMedRscrTypeDescription");
                _DMedRscrTypeDescription = value;
                RaisePropertyChanged("DMedRscrTypeDescription");
                OnDMedRscrTypeDescriptionChanged();
            }
        }
        private String _DMedRscrTypeDescription;
        partial void OnDMedRscrTypeDescriptionChanging(String value);
        partial void OnDMedRscrTypeDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFDISPO_REL_DMEDR_DISPOSAB", "RefDisposableMedicalResources")]
        public ObservableCollection<RefDisposableMedicalResource> RefDisposableMedicalResources
        {
            get;
            set;
        }

        #endregion
    }
}
