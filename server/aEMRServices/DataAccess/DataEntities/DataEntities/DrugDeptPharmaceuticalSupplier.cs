using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DrugDeptPharmaceuticalSupplier : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DrugDeptPharmaceuticalSupplier object.

        /// <param name="pCOID">Initial value of the PCOID property.</param>
        /// <param name="pCOName">Initial value of the PCOName property.</param>
        public static DrugDeptPharmaceuticalSupplier CreateDrugDeptPharmaceuticalSupplier(long pCOID,long SupplierID, long SupplierPCOID)
        {
            DrugDeptPharmaceuticalSupplier DrugDeptPharmaceuticalSupplier = new DrugDeptPharmaceuticalSupplier();
            DrugDeptPharmaceuticalSupplier.PCOID = pCOID;
            DrugDeptPharmaceuticalSupplier.SupplierPCOID = SupplierPCOID;
            DrugDeptPharmaceuticalSupplier.SupplierID = SupplierID;
            return DrugDeptPharmaceuticalSupplier;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long SupplierPCOID
        {
            get
            {
                return _SupplierPCOID;
            }
            set
            {
                if (_SupplierPCOID != value)
                {
                    OnSupplierPCOIDChanging(value);
                    _SupplierPCOID = value;
                    RaisePropertyChanged("SupplierPCOID");
                    OnSupplierPCOIDChanged();
                }
            }
        }
        private long _SupplierPCOID;
        partial void OnSupplierPCOIDChanging(long value);
        partial void OnSupplierPCOIDChanged();

        [DataMemberAttribute()]
        public long PCOID
        {
            get
            {
                return _PCOID;
            }
            set
            {
                if (_PCOID != value)
                {
                    OnPCOIDChanging(value);
                    _PCOID = value;
                    RaisePropertyChanged("PCOID");
                    OnPCOIDChanged();
                }
            }
        }
        private long _PCOID;
        partial void OnPCOIDChanging(long value);
        partial void OnPCOIDChanged();

        [DataMemberAttribute()]
        public long SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                if (_SupplierID != value)
                {
                    OnSupplierIDChanging(value);
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                    OnSupplierIDChanged();
                }
            }
        }
        private long _SupplierID;
        partial void OnSupplierIDChanging(long value);
        partial void OnSupplierIDChanged();

        #endregion
        public override bool Equals(object obj)
        {
            DrugDeptPharmaceuticalSupplier seletedStore = obj as DrugDeptPharmaceuticalSupplier;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCOID == seletedStore.PCOID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
