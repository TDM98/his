using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OtherResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OtherResource object.

        /// <param name="rscrID">Initial value of the RscrID property.</param>
        public static OtherResource CreateOtherResource(Int64 rscrID)
        {
            OtherResource otherResource = new OtherResource();
            otherResource.RscrID = rscrID;
            return otherResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID != value)
                {
                    OnRscrIDChanging(value);
                    ////ReportPropertyChanging("RscrID");
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsLocatable
        {
            get
            {
                return _IsLocatable;
            }
            set
            {
                OnIsLocatableChanging(value);
                ////ReportPropertyChanging("IsLocatable");
                _IsLocatable = value;
                RaisePropertyChanged("IsLocatable");
                OnIsLocatableChanged();
            }
        }
        private Nullable<Boolean> _IsLocatable;
        partial void OnIsLocatableChanging(Nullable<Boolean> value);
        partial void OnIsLocatableChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OTHERRES_REL_INHER_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        #endregion
    }
}
