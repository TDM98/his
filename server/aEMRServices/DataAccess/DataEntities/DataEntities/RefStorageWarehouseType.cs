using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefStorageWarehouseType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefStorageWarehouseType object.

        /// <param name="storeTypeID">Initial value of the StoreTypeID property.</param>
        /// <param name="storeTypeName">Initial value of the StoreTypeName property.</param>
        public static RefStorageWarehouseType CreateRefStorageWarehouseType(Int64 storeTypeID, String storeTypeName)
        {
            RefStorageWarehouseType refStorageWarehouseType = new RefStorageWarehouseType();
            refStorageWarehouseType.StoreTypeID = storeTypeID;
            refStorageWarehouseType.StoreTypeName = storeTypeName;
            return refStorageWarehouseType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 StoreTypeID
        {
            get
            {
                return _StoreTypeID;
            }
            set
            {
                if (_StoreTypeID != value)
                {
                    OnStoreTypeIDChanging(value);
                    ////ReportPropertyChanging("StoreTypeID");
                    _StoreTypeID = value;
                    RaisePropertyChanged("StoreTypeID");
                    OnStoreTypeIDChanged();
                }
            }
        }
        private Int64 _StoreTypeID;
        partial void OnStoreTypeIDChanging(Int64 value);
        partial void OnStoreTypeIDChanged();





        [DataMemberAttribute()]
        public String StoreTypeName
        {
            get
            {
                return _StoreTypeName;
            }
            set
            {
                OnStoreTypeNameChanging(value);
                ////ReportPropertyChanging("StoreTypeName");
                _StoreTypeName = value;
                RaisePropertyChanged("StoreTypeName");
                OnStoreTypeNameChanged();
            }
        }
        private String _StoreTypeName;
        partial void OnStoreTypeNameChanging(String value);
        partial void OnStoreTypeNameChanged();





        [DataMemberAttribute()]
        public String StoreTypeDescription
        {
            get
            {
                return _StoreTypeDescription;
            }
            set
            {
                OnStoreTypeDescriptionChanging(value);
                ////ReportPropertyChanging("StoreTypeDescription");
                _StoreTypeDescription = value;
                RaisePropertyChanged("StoreTypeDescription");
                OnStoreTypeDescriptionChanged();
            }
        }
        private String _StoreTypeDescription;
        partial void OnStoreTypeDescriptionChanging(String value);
        partial void OnStoreTypeDescriptionChanged();

        #endregion

        public override bool Equals(object obj)
        {
            RefStorageWarehouseType seletedStore = obj as RefStorageWarehouseType;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StoreTypeID == seletedStore.StoreTypeID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
