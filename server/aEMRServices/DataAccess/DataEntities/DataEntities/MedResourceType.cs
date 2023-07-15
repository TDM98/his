using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class MedResourceType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedResourceType object.

        /// <param name="medRescrTypeID">Initial value of the MedRescrTypeID property.</param>
        /// <param name="medRescrTypeName">Initial value of the MedRescrTypeName property.</param>
        public static MedResourceType CreateMedResourceType(long medRescrTypeID, String medRescrTypeName)
        {
            MedResourceType medResourceType = new MedResourceType();
            medResourceType.MedRescrTypeID = medRescrTypeID;
            medResourceType.MedRescrTypeName = medRescrTypeName;
            return medResourceType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MedRescrTypeID
        {
            get
            {
                return _MedRescrTypeID;
            }
            set
            {
                if (_MedRescrTypeID != value)
                {
                    OnMedRescrTypeIDChanging(value);
                    ////ReportPropertyChanging("MedRescrTypeID");
                    _MedRescrTypeID = value;
                    RaisePropertyChanged("MedRescrTypeID");
                    OnMedRescrTypeIDChanged();
                }
            }
        }
        private long _MedRescrTypeID;
        partial void OnMedRescrTypeIDChanging(long value);
        partial void OnMedRescrTypeIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ParMedRescrTypeID
        {
            get
            {
                return _ParMedRescrTypeID;
            }
            set
            {
                OnParMedRescrTypeIDChanging(value);
                ////ReportPropertyChanging("ParMedRescrTypeID");
                _ParMedRescrTypeID = value;
                RaisePropertyChanged("ParMedRescrTypeID");
                OnParMedRescrTypeIDChanged();
            }
        }
        private Nullable<long> _ParMedRescrTypeID;
        partial void OnParMedRescrTypeIDChanging(Nullable<long> value);
        partial void OnParMedRescrTypeIDChanged();





        [DataMemberAttribute()]
        public String MedRescrTypeName
        {
            get
            {
                return _MedRescrTypeName;
            }
            set
            {
                OnMedRescrTypeNameChanging(value);
                ////ReportPropertyChanging("MedRescrTypeName");
                _MedRescrTypeName = value;
                RaisePropertyChanged("MedRescrTypeName");
                OnMedRescrTypeNameChanged();
            }
        }
        private String _MedRescrTypeName;
        partial void OnMedRescrTypeNameChanging(String value);
        partial void OnMedRescrTypeNameChanged();





        [DataMemberAttribute()]
        public String MedRescrCode
        {
            get
            {
                return _MedRescrCode;
            }
            set
            {
                OnMedRescrCodeChanging(value);
                ////ReportPropertyChanging("MedRescrCode");
                _MedRescrCode = value;
                RaisePropertyChanged("MedRescrCode");
                OnMedRescrCodeChanged();
            }
        }
        private String _MedRescrCode;
        partial void OnMedRescrCodeChanging(String value);
        partial void OnMedRescrCodeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM16_MEDRESOU", "Resources")]
        public ObservableCollection<Resource> Resources
        {
            get;
            set;
        }

        #endregion
    }
}
