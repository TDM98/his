using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Service.Core.Common;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    [DataContract]
    public partial class RefGenMedProductSummaryInfo : EntityBase
    {
        public RefGenMedProductSummaryInfo()
        {
            EntityState = EntityState.NEW;
        }
        private RefGenMedProductDetails _medProductDetails;
        [DataMemberAttribute()]
        public RefGenMedProductDetails MedProductDetails
        {
            get
            {
                return _medProductDetails;
            }
            set
            {
                _medProductDetails = value;
                RaisePropertyChanged("MedProductDetails");
            }
        }
        private decimal _totalQty;
        [DataMemberAttribute()]
        public decimal TotalQty
        {
            get
            {
                return _totalQty;
            }
            set
            {
                if (_totalQty != value)
                {
                    ValidateProperty("TotalQty", value);
                    _totalQty = value;
                    RaisePropertyChanged("TotalQty");
                    ValidateProperty("QtyReturn", _qtyReturn);
                }
            }
        }

        private decimal _totalQtyReturned;
        [DataMemberAttribute()]
        public decimal TotalQtyReturned
        {
            get
            {
                return _totalQtyReturned;
            }
            set
            {
                if (_totalQtyReturned != value)
                {
                    ValidateProperty("TotalQtyReturned", value);
                    
                    _totalQtyReturned = value;
                    RaisePropertyChanged("TotalQtyReturned");
                    ValidateProperty("QtyReturn", _qtyReturn);
                }
            }
        }

        private decimal _qtyReturn;

        [CustomValidation(typeof(RefGenMedProductSummaryInfo), "ValidateQtyReturn")]
        [DataMemberAttribute()]
        public decimal QtyReturn
        {
            get
            {
                return _qtyReturn;
            }
            set
            {
                if (_qtyReturn != value)
                {
                    ValidateProperty("QtyReturn",value);
                    _qtyReturn = value;
                    RaisePropertyChanged("QtyReturn");
                }
            }
        }
        public static ValidationResult ValidateQtyReturn(int numReturn, ValidationContext context)
        {
            if (numReturn < 0)
            {
                return new ValidationResult("Số lượng trả phải lớn hơn hoặc bằng 0.", new string[] { "QtyReturn" });
            }

            var info = (RefGenMedProductSummaryInfo)context.ObjectInstance;
            if(info.TotalQty >= 0 && info.TotalQtyReturned >= 0 && info.TotalQty - info.TotalQtyReturned >= 0)
            {
                if(numReturn > info.TotalQty - info.TotalQtyReturned)
                {
                    return new ValidationResult(string.Format("Số lượng trả không hợp lệ. Giá trị phải nhỏ hơn hoặc bằng {0}",info.TotalQty-info.TotalQtyReturned), new string[] { "QtyReturn" });
                }
            }
            else
            {
                return new ValidationResult("Số lượng trả không hợp lệ.", new string[] { "QtyReturn" });
            }

            return ValidationResult.Success;
        }
    }

}
