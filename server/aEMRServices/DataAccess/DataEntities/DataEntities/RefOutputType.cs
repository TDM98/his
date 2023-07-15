using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefOutputType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefOutputType object.

        /// <param name="typID">Initial value of the TypID property.</param>
        /// <param name="typName">Initial value of the TypName property.</param>
        /// <param name="typActive">Initial value of the TypActive property.</param>
        public static RefOutputType CreateRefOutputType(long typID, String typName, Boolean typActive)
        {
            RefOutputType refOutputType = new RefOutputType();
            refOutputType.TypID = typID;
            refOutputType.TypName = typName;
            refOutputType.TypActive = typActive;
            return refOutputType;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                if (_TypID != value)
                {
                    OnTypIDChanging(value);
                    _TypID = value;
                    RaisePropertyChanged("TypID");
                    OnTypIDChanged();
                }
            }
        }
        private long _TypID;
        partial void OnTypIDChanging(long value);
        partial void OnTypIDChanged();

        [DataMemberAttribute()]
        public String TypName
        {
            get
            {
                return _TypName;
            }
            set
            {
                OnTypNameChanging(value);
                _TypName = value;
                RaisePropertyChanged("TypName");
                OnTypNameChanged();
            }
        }
        private String _TypName;
        partial void OnTypNameChanging(String value);
        partial void OnTypNameChanged();

        [DataMemberAttribute()]
        public Boolean TypActive
        {
            get
            {
                return _TypActive;
            }
            set
            {
                OnTypActiveChanging(value);
                _TypActive = value;
                RaisePropertyChanged("TypActive");
                OnTypActiveChanged();
            }
        }
        private Boolean _TypActive;
        partial void OnTypActiveChanging(Boolean value);
        partial void OnTypActiveChanged();


        [DataMemberAttribute()]
        public bool? IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                OnIsSelectedChanging(value);
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
                OnIsSelectedChanged();
            }
        }
        private bool? _IsSelected;
        partial void OnIsSelectedChanging(bool? value);
        partial void OnIsSelectedChanged();

        [DataMemberAttribute()]
        public bool? IsSelectedPharmacyInternal
        {
            get
            {
                return _IsSelectedPharmacyInternal;
            }
            set
            {
                OnIsSelectedPharmacyInternalChanging(value);
                _IsSelectedPharmacyInternal = value;
                RaisePropertyChanged("IsSelectedPharmacyInternal");
                OnIsSelectedPharmacyInternalChanged();
            }
        }
        private bool? _IsSelectedPharmacyInternal;
        partial void OnIsSelectedPharmacyInternalChanging(bool? value);
        partial void OnIsSelectedPharmacyInternalChanged();

        [DataMemberAttribute()]
        public String TypNamePharmacy
        {
            get
            {
                return _TypNamePharmacy;
            }
            set
            {
                OnTypNamePharmacyChanging(value);
                _TypNamePharmacy = value;
                RaisePropertyChanged("TypNamePharmacy");
                OnTypNamePharmacyChanged();
            }
        }
        private String _TypNamePharmacy;
        partial void OnTypNamePharmacyChanging(String value);
        partial void OnTypNamePharmacyChanged();


        [DataMemberAttribute()]
        public bool IsSelectedClinicDept
        {
            get
            {
                return _IsSelectedClinicDept;
            }
            set
            {
                _IsSelectedClinicDept = value;
                RaisePropertyChanged("IsSelectedClinicDept");
            }
        }
        private bool _IsSelectedClinicDept;

        #endregion
    }
}
