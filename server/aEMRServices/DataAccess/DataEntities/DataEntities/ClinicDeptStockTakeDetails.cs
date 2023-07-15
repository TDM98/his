using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class ClinicDeptStockTakeDetails : EntityBase
    {
        #region Factory Method
        public static ClinicDeptStockTakeDetails CreateClinicDeptStockTakeDetails(Int64 ClinicDeptStockTakeDetailID, Int64 ClinicDeptStockTakeID, Int64 GenMedProductID, Int32 outQtyPrevFirstMonth, Int32 outQtyPrevSecondMonth, Int32 outQtyPrevThirdMonth, Int32 outQtyPrevFourthMonth, Int32 CaculatedQty, Int32 ActualQty, Int32 adjustedQty, Int32 numberOfEstimatedMonths, Int32 outQtyLastTwelveMonths, Int32 toDateOutQty)
        {
            ClinicDeptStockTakeDetails ClinicDeptStockTakeDetails = new ClinicDeptStockTakeDetails();
            ClinicDeptStockTakeDetails.ClinicDeptStockTakeDetailID = ClinicDeptStockTakeDetailID;
            ClinicDeptStockTakeDetails.ClinicDeptStockTakeID = ClinicDeptStockTakeID;
            ClinicDeptStockTakeDetails.GenMedProductID = GenMedProductID;
            ClinicDeptStockTakeDetails.CaculatedQty = CaculatedQty;
            ClinicDeptStockTakeDetails.ActualQty = ActualQty;
            return ClinicDeptStockTakeDetails;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ClinicDeptStockTakeDetailID
        {
            get
            {
                return _ClinicDeptStockTakeDetailID;
            }
            set
            {
                if (_ClinicDeptStockTakeDetailID != value)
                {
                    OnClinicDeptStockTakeDetailIDChanging(value);
                    _ClinicDeptStockTakeDetailID = value;
                    RaisePropertyChanged("ClinicDeptStockTakeDetailID");
                    OnClinicDeptStockTakeDetailIDChanged();
                }
            }
        }
        private Int64 _ClinicDeptStockTakeDetailID;
        partial void OnClinicDeptStockTakeDetailIDChanging(Int64 value);
        partial void OnClinicDeptStockTakeDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 ClinicDeptStockTakeID
        {
            get
            {
                return _ClinicDeptStockTakeID;
            }
            set
            {
                OnClinicDeptStockTakeIDChanging(value);
                _ClinicDeptStockTakeID = value;
                RaisePropertyChanged("ClinicDeptStockTakeID");
                OnClinicDeptStockTakeIDChanged();
            }
        }
        private Int64 _ClinicDeptStockTakeID;
        partial void OnClinicDeptStockTakeIDChanging(Int64 value);
        partial void OnClinicDeptStockTakeIDChanged();


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
        public Int16 RowActionStatusFlag
        {
            get
            {
                return _RowActionStatusFlag;
            }
            set
            {
                OnRowActionStatusFlagChanging(value);
                _RowActionStatusFlag = value;
                RaisePropertyChanged("RowActionStatusFlag");
                OnRowActionStatusFlagChanged();
            }
        }
        private Int16 _RowActionStatusFlag;
        partial void OnRowActionStatusFlagChanging(Int64 value);
        partial void OnRowActionStatusFlagChanged();


        [DataMemberAttribute()]
        public Decimal CaculatedQty
        {
            get
            {
                return _CaculatedQty;
            }
            set
            {
                OnCaculatedQtyChanging(value);
                _CaculatedQty = value;
                RaisePropertyChanged("CaculatedQty");
                OnCaculatedQtyChanged();
            }
        }
        private Decimal _CaculatedQty;
        partial void OnCaculatedQtyChanging(Decimal value);
        partial void OnCaculatedQtyChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng thực tế không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Decimal ActualQty
        {
            get
            {
                return _ActualQty;
            }
            set
            {
                OnActualQtyChanging(value);
                ValidateProperty("ActualQty", value);
                _ActualQty = value;
                AdjustQty = _ActualQty - CaculatedQty;
                RaisePropertyChanged("AdjustQty");
                RaisePropertyChanged("ActualQty");
                OnActualQtyChanged();
            }
        }
        private Decimal _ActualQty;
        partial void OnActualQtyChanging(Decimal value);
        partial void OnActualQtyChanged();

        [DataMemberAttribute()]
        public Decimal AdjustQty
        {
            get
            {
                return _AdjustQty;
            }
            set
            {
                _AdjustQty = value;
                RaisePropertyChanged("AdjustQty");
            }
        }
        private Decimal _AdjustQty;


        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        private string _Notes;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                RaisePropertyChanged("Code");
            }
        }
        private string _Code;

        [DataMemberAttribute()]
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private string _BrandName;

        [DataMemberAttribute()]
        public string GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                _GenericName = value;
                RaisePropertyChanged("GenericName");
            }
        }
        private string _GenericName;

        [DataMemberAttribute()]
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                _UnitName = value;
                RaisePropertyChanged("UnitName");
            }
        }
        private string _UnitName;


        //[DataMemberAttribute()]
        //private RefGenMedProductDetails _RefGenMedProductDetails;
        //public RefGenMedProductDetails RefGenMedProductDetails
        //{
        //    get
        //    {
        //        return _RefGenMedProductDetails;
        //    }
        //    set
        //    {
        //        _RefGenMedProductDetails= value;
        //        if (_RefGenMedProductDetails != null)
        //        {
        //            GenMedProductID = _RefGenMedProductDetails.GenMedProductID;
        //        }
        //        else
        //        {
        //            GenMedProductID = 0;
        //        }
        //        RaisePropertyChanged("GenMedProductID");
        //        RaisePropertyChanged("RefGenMedProductDetails");
        //    }
        //}


        #endregion

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }
        [DataMemberAttribute()]
        public Decimal FinalAmount
        {
            get
            {
                return _FinalAmount;
            }
            set
            {
                _FinalAmount = value;
                RaisePropertyChanged("FinalAmount");
            }
        }
        private Decimal _FinalAmount;
    }
}
