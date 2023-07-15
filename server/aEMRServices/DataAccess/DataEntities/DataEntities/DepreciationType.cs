using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DepreciationType : NotifyChangedBase, IEditableObject
    {
        public DepreciationType()
            : base()
        {

        }

        private DepreciationType _tempDepreciationType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDepreciationType = (DepreciationType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDepreciationType)
                CopyFrom(_tempDepreciationType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DepreciationType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DepreciationType object.

        /// <param name="deprecTypeID">Initial value of the DeprecTypeID property.</param>
        /// <param name="deprecTypeName">Initial value of the DeprecTypeName property.</param>
        public static DepreciationType CreateDepreciationType(Byte deprecTypeID, String deprecTypeName)
        {
            DepreciationType depreciationType = new DepreciationType();
            depreciationType.DeprecTypeID = deprecTypeID;
            depreciationType.DeprecTypeName = deprecTypeName;
            return depreciationType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Byte DeprecTypeID
        {
            get
            {
                return _DeprecTypeID;
            }
            set
            {
                if (_DeprecTypeID != value)
                {
                    OnDeprecTypeIDChanging(value);
                    _DeprecTypeID = value;
                    RaisePropertyChanged("DeprecTypeID");
                    OnDeprecTypeIDChanged();
                }
            }
        }
        private Byte _DeprecTypeID;
        partial void OnDeprecTypeIDChanging(Byte value);
        partial void OnDeprecTypeIDChanged();





        [DataMemberAttribute()]
        public String DeprecTypeName
        {
            get
            {
                return _DeprecTypeName;
            }
            set
            {
                OnDeprecTypeNameChanging(value);
                _DeprecTypeName = value;
                RaisePropertyChanged("DeprecTypeName");
                OnDeprecTypeNameChanged();
            }
        }
        private String _DeprecTypeName;
        partial void OnDeprecTypeNameChanging(String value);
        partial void OnDeprecTypeNameChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<Resource> Resources
        {
            get;
            set;
        }

        #endregion
    }
}
