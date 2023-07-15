using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DrugDeptPurchaseOrderDetail : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DrugDeptPurchaseOrderDetail object.

        /// <param name="drugDeptPoDetailID">Initial value of the DrugDeptPoDetailID property.</param>
        /// <param name="genMedProductID">Initial value of the GenMedProductID property.</param>
        /// <param name="poQty">Initial value of the PoQty property.</param>
        public static DrugDeptPurchaseOrderDetail CreateDrugDeptPurchaseOrderDetail(Int64 drugDeptPoDetailID, Int64 genMedProductID, Int32 poQty)
        {
            DrugDeptPurchaseOrderDetail drugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();
            drugDeptPurchaseOrderDetail.DrugDeptPoDetailID = drugDeptPoDetailID;
            drugDeptPurchaseOrderDetail.GenMedProductID = genMedProductID;
            drugDeptPurchaseOrderDetail.PoQty = poQty;
            return drugDeptPurchaseOrderDetail;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptPoDetailID
        {
            get
            {
                return _DrugDeptPoDetailID;
            }
            set
            {
                if (_DrugDeptPoDetailID != value)
                {
                    OnDrugDeptPoDetailIDChanging(value);
                    _DrugDeptPoDetailID = value;
                    RaisePropertyChanged("DrugDeptPoDetailID");
                    OnDrugDeptPoDetailIDChanged();
                }
            }
        }
        private Int64 _DrugDeptPoDetailID;
        partial void OnDrugDeptPoDetailIDChanging(Int64 value);
        partial void OnDrugDeptPoDetailIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> DrugDeptPoID
        {
            get
            {
                return _DrugDeptPoID;
            }
            set
            {
                OnDrugDeptPoIDChanging(value);
                _DrugDeptPoID = value;
                RaisePropertyChanged("DrugDeptPoID");
                OnDrugDeptPoIDChanged();
            }
        }
        private Nullable<Int64> _DrugDeptPoID;
        partial void OnDrugDeptPoIDChanging(Nullable<Int64> value);
        partial void OnDrugDeptPoIDChanged();

        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                OnGenMedProductIDChanging(value);
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
                OnGenMedProductIDChanged();
            }
        }
        private Int64 _GenMedProductID;
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();

        [DataMemberAttribute()]
        public Int32 PoQty
        {
            get
            {
                return _PoQty;
            }
            set
            {
                OnPoQtyChanging(value);
                _PoQty = value;
                RaisePropertyChanged("PoQty");
                OnPoQtyChanged();
            }
        }
        private Int32 _PoQty;
        partial void OnPoQtyChanging(Int32 value);
        partial void OnPoQtyChanged();

        #endregion

    }

}
