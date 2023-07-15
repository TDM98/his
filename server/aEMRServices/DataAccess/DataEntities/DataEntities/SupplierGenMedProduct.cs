using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class SupplierGenMedProduct : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SupplierGenMedProduct object.

        /// <param name="supGenMedID">Initial value of the SupGenMedID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        /// <param name="genMedProductID">Initial value of the GenMedProductID property.</param>
        /// <param name="recDateCreated">Initial value of the RecDateCreated property.</param>
        public static SupplierGenMedProduct CreateSupplierGenMedProduct(Int64 supGenMedID, Int64 supplierID, Int64 genMedProductID, DateTime recDateCreated)
        {
            SupplierGenMedProduct supplierGenMedProduct = new SupplierGenMedProduct();
            supplierGenMedProduct.SupGenMedID = supGenMedID;
            supplierGenMedProduct.SupplierID = supplierID;
            supplierGenMedProduct.GenMedProductID = genMedProductID;
            supplierGenMedProduct.RecDateCreated = recDateCreated;
            return supplierGenMedProduct;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 SupGenMedID
        {
            get
            {
                return _SupGenMedID;
            }
            set
            {
                if (_SupGenMedID != value)
                {
                    OnSupGenMedIDChanging(value);
                    _SupGenMedID = value;
                    RaisePropertyChanged("SupGenMedID");
                    OnSupGenMedIDChanged();
                }
            }
        }
        private Int64 _SupGenMedID;
        partial void OnSupGenMedIDChanging(Int64 value);
        partial void OnSupGenMedIDChanged();

        [DataMemberAttribute()]
        public Int64 SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Int64 _SupplierID;
        partial void OnSupplierIDChanging(Int64 value);
        partial void OnSupplierIDChanged();

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
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                OnRecDateCreatedChanging(value);
                _RecDateCreated = value;
                RaisePropertyChanged("RecDateCreated");
                OnRecDateCreatedChanged();
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();

        [DataMemberAttribute()]
        public Nullable<Byte> SupplierPriorityOrderNum
        {
            get
            {
                return _SupplierPriorityOrderNum;
            }
            set
            {
                OnSupplierPriorityOrderNumChanging(value);
                _SupplierPriorityOrderNum = value;
                RaisePropertyChanged("SupplierPriorityOrderNum");
                OnSupplierPriorityOrderNumChanged();
            }
        }
        private Nullable<Byte> _SupplierPriorityOrderNum;
        partial void OnSupplierPriorityOrderNumChanging(Nullable<Byte> value);
        partial void OnSupplierPriorityOrderNumChanged();

        [DataMemberAttribute()]
        public bool IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }
        private bool _IsMain;

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [Required(ErrorMessage = "Nhập Đơn Giá!")]
        [Range(0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=0")]
        [DataMemberAttribute()]
        public Decimal UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                OnUnitPriceChanging(value);
                ValidateProperty("UnitPrice", value);
                _UnitPrice = value;
                RaisePropertyChanged("UnitPrice");
                OnUnitPriceChanged();
            }
        }
        private Decimal _UnitPrice;
        partial void OnUnitPriceChanging(Decimal value);
        partial void OnUnitPriceChanged();

        [Required(ErrorMessage = "Nhập Đơn Giá!")]
        [Range(0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=0")]
        [DataMemberAttribute()]
        public Decimal PackagePrice
        {
            get { return _PackagePrice; }
            set
            {

                OnPackagePriceChanging(value);
                ValidateProperty("PackagePrice", value);
                _PackagePrice = value;
                RaisePropertyChanged("PackagePrice");
                OnPackagePriceChanged();

            }
        }
        private Decimal _PackagePrice;
        partial void OnPackagePriceChanging(Decimal value);
        partial void OnPackagePriceChanged();

        [Required(ErrorMessage = "Nhập VAT!")]
        [Range(0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=0")]
        [DataMemberAttribute()]
        public double VAT
        {
            get { return _VAT; }
            set
            {

                ValidateProperty("VAT", value);
                _VAT = value;
                RaisePropertyChanged("VAT");

            }
        }
        private double _VAT = 1;
     
        #endregion

        #region Navigation Properties

        //[Required(ErrorMessage = "Vui lòng chọn NCC")]
        [DataMemberAttribute()]
        public DrugDeptSupplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                if (_SelectedSupplier != value)
                {
                    OnSelectedSupplierChanging(value);
                    //ValidateProperty("SelectedSupplier", value);
                    _SelectedSupplier = value;
                    if (_SelectedSupplier != null)
                    {
                        _SupplierID = _SelectedSupplier.SupplierID;
                    }
                    else
                    {
                        _SupplierID = 0;
                    }
                    RaisePropertyChanged("SupplierID");
                    RaisePropertyChanged("SelectedSupplier");
                    OnSelectedSupplierChanged();
                }
            }
        }
        private DrugDeptSupplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(DrugDeptSupplier unit);
        partial void OnSelectedSupplierChanged();


        //[Required(ErrorMessage = "Vui lòng chọn Thuốc/ Y Cụ / Hóa Chất")]
        [DataMemberAttribute()]
        public RefGenMedProductDetails SelectedGenMedProduct
        {
            get
            {
                return _SelectedGenMedProduct;
            }
            set
            {
                if (_SelectedGenMedProduct != value)
                {
                    //ValidateProperty("SelectedGenMedProduct", value);
                    _SelectedGenMedProduct = value;
                    if (_SelectedGenMedProduct != null)
                    {
                        _GenMedProductID = _SelectedGenMedProduct.GenMedProductID;
                    }
                    else
                    {
                        _GenMedProductID = 0;
                    }

                    RaisePropertyChanged("GenMedProductID");
                    RaisePropertyChanged("SelectedGenMedProduct");
                }
            }
        }
        private RefGenMedProductDetails _SelectedGenMedProduct;

        #endregion

    }
}
