using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HIService : NotifyChangedBase, IEditableObject
    {
        public HIService()
            : base()
        {

        }

        private HIService _tempHIService;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHIService = (HIService)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHIService)
                CopyFrom(_tempHIService);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HIService p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HIService object.

        /// <param name="hIServiceID">Initial value of the HIServiceID property.</param>
        /// <param name="hITTypeID">Initial value of the HITTypeID property.</param>
        /// <param name="medServiceID">Initial value of the MedServiceID property.</param>
        public static HIService CreateHIService(long hIServiceID, long hITTypeID, long medServiceID)
        {
            HIService hIService = new HIService();
            hIService.HIServiceID = hIServiceID;
            hIService.HITTypeID = hITTypeID;
            hIService.MedServiceID = medServiceID;
            return hIService;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HIServiceID
        {
            get
            {
                return _HIServiceID;
            }
            set
            {
                if (_HIServiceID != value)
                {
                    OnHIServiceIDChanging(value);
                    ////ReportPropertyChanging("HIServiceID");
                    _HIServiceID = value;
                    RaisePropertyChanged("HIServiceID");
                    OnHIServiceIDChanged();
                }
            }
        }
        private long _HIServiceID;
        partial void OnHIServiceIDChanging(long value);
        partial void OnHIServiceIDChanged();





        [DataMemberAttribute()]
        public long HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                OnHITTypeIDChanging(value);
                ////ReportPropertyChanging("HITTypeID");
                _HITTypeID = value;
                RaisePropertyChanged("HITTypeID");
                OnHITTypeIDChanged();
            }
        }
        private long _HITTypeID;
        partial void OnHITTypeIDChanging(long value);
        partial void OnHITTypeIDChanged();





        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                ////ReportPropertyChanging("MedServiceID");
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private long _MedServiceID;
        partial void OnMedServiceIDChanging(long value);
        partial void OnMedServiceIDChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HISERVIC_REL_HOSFM_IHTRANSA", "IHTransactionType")]
        public IHTransactionType IHTransactionType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HISERVIC_REL_HOSFM_REFMEDIC", "RefMedicalServiceItems")]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        #endregion
    }
}
