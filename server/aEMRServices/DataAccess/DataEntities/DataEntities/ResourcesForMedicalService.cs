using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ResourcesForMedicalService : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ResourcesForMedicalService object.

        /// <param name="rFMSID">Initial value of the RFMSID property.</param>
        /// <param name="medServiceID">Initial value of the MedServiceID property.</param>
        /// <param name="rscrID">Initial value of the RscrID property.</param>
        public static ResourcesForMedicalService CreateResourcesForMedicalService(long rFMSID, long medServiceID, Int64 rscrID)
        {
            ResourcesForMedicalService resourcesForMedicalService = new ResourcesForMedicalService();
            resourcesForMedicalService.RFMSID = rFMSID;
            resourcesForMedicalService.MedServiceID = medServiceID;
            resourcesForMedicalService.RscrID = rscrID;
            return resourcesForMedicalService;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long RFMSID
        {
            get
            {
                return _RFMSID;
            }
            set
            {
                if (_RFMSID != value)
                {
                    OnRFMSIDChanging(value);
                    ////ReportPropertyChanging("RFMSID");
                    _RFMSID = value;
                    RaisePropertyChanged("RFMSID");
                    OnRFMSIDChanged();
                }
            }
        }
        private long _RFMSID;
        partial void OnRFMSIDChanging(long value);
        partial void OnRFMSIDChanged();





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





        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                OnRscrIDChanging(value);
                ////ReportPropertyChanging("RscrID");
                _RscrID = value;
                RaisePropertyChanged("RscrID");
                OnRscrIDChanged();
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> NumberOfUses
        {
            get
            {
                return _NumberOfUses;
            }
            set
            {
                OnNumberOfUsesChanging(value);
                ////ReportPropertyChanging("NumberOfUses");
                _NumberOfUses = value;
                RaisePropertyChanged("NumberOfUses");
                OnNumberOfUsesChanged();
            }
        }
        private Nullable<Byte> _NumberOfUses;
        partial void OnNumberOfUsesChanging(Nullable<Byte> value);
        partial void OnNumberOfUsesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM09_REFMEDIC", "RefMedicalServiceItems")]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM08_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        #endregion
    }

}
