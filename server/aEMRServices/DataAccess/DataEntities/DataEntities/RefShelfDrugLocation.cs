using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefShelfDrugLocation : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefShelfDrugLocation object.

        /// <param name="sdlID">Initial value of the SdlID property.</param>
        public static RefShelfDrugLocation CreateRefShelfDrugLocation(long sdlID)
        {
            RefShelfDrugLocation refShelfDrugLocation = new RefShelfDrugLocation();
            refShelfDrugLocation.SdlID = sdlID;
            return refShelfDrugLocation;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SdlID
        {
            get
            {
                return _SdlID;
            }
            set
            {
                if (_SdlID != value)
                {
                    OnSdlIDChanging(value);
                    ////ReportPropertyChanging("SdlID");
                    _SdlID = value;
                    RaisePropertyChanged("SdlID");
                    OnSdlIDChanged();
                }
            }
        }
        private long _SdlID;
        partial void OnSdlIDChanging(long value);
        partial void OnSdlIDChanged();





        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                OnSdlDescriptionChanging(value);
                ////ReportPropertyChanging("SdlDescription");
                _SdlDescription = value;
                RaisePropertyChanged("SdlDescription");
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDDR_REL_DMGMT_REFSHELF", "InwardDrugs")]
        public ObservableCollection<InwardDrug> InwardDrugs
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            RefShelfDrugLocation seletedStore = obj as RefShelfDrugLocation;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.SdlID == seletedStore.SdlID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
