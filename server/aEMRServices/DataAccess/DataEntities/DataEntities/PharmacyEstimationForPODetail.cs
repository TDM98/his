using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PharmacyEstimationForPODetail : EntityBase
    {
        #region Factory Method
        public static PharmacyEstimationForPODetail CreatePharmacyEstimationForPODetail(Int64 PharmacyEstimatePoDetailID, Int64 PharmacyEstimatePoID, Int64 DrugID, Int32 outQtyPrevFirstMonth, Int32 outQtyPrevSecondMonth, Int32 outQtyPrevThirdMonth, Int32 outQtyPrevFourthMonth, Int32 remainQty, double EstimatedQty_F, Int32 adjustedQty, double NumberOfEstimatedMonths_F, Int32 outQtyLastTwelveMonths, Int32 toDateOutQty)
        {
            PharmacyEstimationForPODetail PharmacyEstimationForPODetail = new PharmacyEstimationForPODetail();
            PharmacyEstimationForPODetail.PharmacyEstimatePoDetailID = PharmacyEstimatePoDetailID;
            PharmacyEstimationForPODetail.PharmacyEstimatePoID = PharmacyEstimatePoID;
            PharmacyEstimationForPODetail.DrugID = DrugID;
            PharmacyEstimationForPODetail.OutQtyPrevFirstMonth = outQtyPrevFirstMonth;
            PharmacyEstimationForPODetail.OutQtyPrevSecondMonth = outQtyPrevSecondMonth;
            PharmacyEstimationForPODetail.OutQtyPrevThirdMonth = outQtyPrevThirdMonth;
            PharmacyEstimationForPODetail.OutQtyPrevFourthMonth = outQtyPrevFourthMonth;
            PharmacyEstimationForPODetail.RemainQty = remainQty;
            PharmacyEstimationForPODetail.EstimatedQty_F = EstimatedQty_F;
            PharmacyEstimationForPODetail.AdjustedQty = adjustedQty;
            PharmacyEstimationForPODetail.NumberOfEstimatedMonths_F = NumberOfEstimatedMonths_F;
            PharmacyEstimationForPODetail.OutQtyLastTwelveMonths = outQtyLastTwelveMonths;
            PharmacyEstimationForPODetail.ToDateOutQty = toDateOutQty;
            return PharmacyEstimationForPODetail;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyEstimatePoDetailID
        {
            get
            {
                return _PharmacyEstimatePoDetailID;
            }
            set
            {
                if (_PharmacyEstimatePoDetailID != value)
                {
                    OnPharmacyEstimatePoDetailIDChanging(value);
                    _PharmacyEstimatePoDetailID = value;
                    RaisePropertyChanged("PharmacyEstimatePoDetailID");
                    OnPharmacyEstimatePoDetailIDChanged();
                }
            }
        }
        private Int64 _PharmacyEstimatePoDetailID;
        partial void OnPharmacyEstimatePoDetailIDChanging(Int64 value);
        partial void OnPharmacyEstimatePoDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 PharmacyEstimatePoID
        {
            get
            {
                return _PharmacyEstimatePoID;
            }
            set
            {
                OnPharmacyEstimatePoIDChanging(value);
                _PharmacyEstimatePoID = value;
                RaisePropertyChanged("PharmacyEstimatePoID");
                OnPharmacyEstimatePoIDChanged();
            }
        }
        private Int64 _PharmacyEstimatePoID;
        partial void OnPharmacyEstimatePoIDChanging(Int64 value);
        partial void OnPharmacyEstimatePoIDChanged();


        [DataMemberAttribute()]
        public Int64 DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Int64 _DrugID;
        partial void OnDrugIDChanging(Int64 value);
        partial void OnDrugIDChanged();

        [DataMemberAttribute()]
        public Int32 OutQtyPrevFirstMonth
        {
            get
            {
                return _OutQtyPrevFirstMonth;
            }
            set
            {
                OnOutQtyPrevFirstMonthChanging(value);
                _OutQtyPrevFirstMonth = value;
                RaisePropertyChanged("OutQtyPrevFirstMonth");
                OnOutQtyPrevFirstMonthChanged();
            }
        }
        private Int32 _OutQtyPrevFirstMonth;
        partial void OnOutQtyPrevFirstMonthChanging(Int32 value);
        partial void OnOutQtyPrevFirstMonthChanged();

        [DataMemberAttribute()]
        public Int32 OutQtyPrevSecondMonth
        {
            get
            {
                return _OutQtyPrevSecondMonth;
            }
            set
            {
                OnOutQtyPrevSecondMonthChanging(value);
                _OutQtyPrevSecondMonth = value;
                RaisePropertyChanged("OutQtyPrevSecondMonth");
                OnOutQtyPrevSecondMonthChanged();
            }
        }
        private Int32 _OutQtyPrevSecondMonth;
        partial void OnOutQtyPrevSecondMonthChanging(Int32 value);
        partial void OnOutQtyPrevSecondMonthChanged();

        [DataMemberAttribute()]
        public Int32 OutQtyPrevThirdMonth
        {
            get
            {
                return _OutQtyPrevThirdMonth;
            }
            set
            {
                OnOutQtyPrevThirdMonthChanging(value);
                _OutQtyPrevThirdMonth = value;
                RaisePropertyChanged("OutQtyPrevThirdMonth");
                OnOutQtyPrevThirdMonthChanged();
            }
        }
        private Int32 _OutQtyPrevThirdMonth;
        partial void OnOutQtyPrevThirdMonthChanging(Int32 value);
        partial void OnOutQtyPrevThirdMonthChanged();

        [DataMemberAttribute()]
        public Int32 OutQtyPrevFourthMonth
        {
            get
            {
                return _OutQtyPrevFourthMonth;
            }
            set
            {
                OnOutQtyPrevFourthMonthChanging(value);
                _OutQtyPrevFourthMonth = value;
                RaisePropertyChanged("OutQtyPrevFourthMonth");
                OnOutQtyPrevFourthMonthChanged();
            }
        }
        private Int32 _OutQtyPrevFourthMonth;
        partial void OnOutQtyPrevFourthMonthChanging(Int32 value);
        partial void OnOutQtyPrevFourthMonthChanged();

        [DataMemberAttribute()]
        public Int32 RemainQty
        {
            get
            {
                return _RemainQty;
            }
            set
            {
                OnRemainQtyChanging(value);
                _RemainQty = value;
                RaisePropertyChanged("RemainQty");
                OnRemainQtyChanged();
            }
        }
        private Int32 _RemainQty;
        partial void OnRemainQtyChanging(Int32 value);
        partial void OnRemainQtyChanged();

        [DataMemberAttribute()]
        public double EstimatedQty_F
        {
            get
            {
                return _EstimatedQty_F;
            }
            set
            {
                OnEstimatedQty_FChanging(value);
                _EstimatedQty_F = value;
                RaisePropertyChanged("EstimatedQty_F");
                OnEstimatedQty_FChanged();
            }
        }
        private double _EstimatedQty_F;
        partial void OnEstimatedQty_FChanging(double value);
        partial void OnEstimatedQty_FChanged();

        [DataMemberAttribute()]
        public Int32 AdjustedQty
        {
            get
            {
                return _AdjustedQty;
            }
            set
            {
                if (_AdjustedQty != value)
                {
                    OnAdjustedQtyChanging(value);
                    _AdjustedQty = value;
                    RaisePropertyChanged("AdjustedQty");
                    //KMx: Không tính toán trong đây, dễ bị sai (30/10/2015 15:41).
                    //_TotalPrice = _AdjustedQty * _UnitPrice;
                    //RaisePropertyChanged("TotalPrice");
                    //if (RefGenMedProductDetails != null)
                    //{
                    //    if (RefGenMedProductDetails.UnitPackaging > 0)
                    //    {
                    //        _PackageQty = _AdjustedQty / RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    //    }
                    //    else
                    //    {
                    //        _PackageQty = _AdjustedQty;
                    //    }
                    //    RaisePropertyChanged("PackageQty");
                    //}
                    if (EntityState == EntityState.PERSITED || EntityState == EntityState.MODIFIED)
                    {
                        EntityState = EntityState.MODIFIED;
                    }
                    OnAdjustedQtyChanged();
                }
            }
        }
        private Int32 _AdjustedQty;
        partial void OnAdjustedQtyChanging(Int32 value);
        partial void OnAdjustedQtyChanged();

        [Range(0.0, 999999999.0, ErrorMessage = "Hệ số nhân không hợp lệ!")]
        [DataMemberAttribute()]
        public double NumberOfEstimatedMonths_F
        {
            get
            {
                return _NumberOfEstimatedMonths_F;
            }
            set
            {
                OnNumberOfEstimatedMonths_FChanging(value);
                ValidateProperty("NumberOfEstimatedMonths_F", value);
                _NumberOfEstimatedMonths_F = value;
                RaisePropertyChanged("NumberOfEstimatedMonths_F");
                OnNumberOfEstimatedMonths_FChanged();
            }
        }
        private double _NumberOfEstimatedMonths_F;
        partial void OnNumberOfEstimatedMonths_FChanging(double value);
        partial void OnNumberOfEstimatedMonths_FChanged();

        [DataMemberAttribute()]
        public Int32 OutQtyLastTwelveMonths
        {
            get
            {
                return _OutQtyLastTwelveMonths;
            }
            set
            {
                OnOutQtyLastTwelveMonthsChanging(value);
                _OutQtyLastTwelveMonths = value;
                RaisePropertyChanged("OutQtyLastTwelveMonths");
                OnOutQtyLastTwelveMonthsChanged();
            }
        }
        private Int32 _OutQtyLastTwelveMonths;
        partial void OnOutQtyLastTwelveMonthsChanging(Int32 value);
        partial void OnOutQtyLastTwelveMonthsChanged();

        [DataMemberAttribute()]
        public Int32 ToDateOutQty
        {
            get
            {
                return _ToDateOutQty;
            }
            set
            {
                OnToDateOutQtyChanging(value);
                _ToDateOutQty = value;
                RaisePropertyChanged("ToDateOutQty");
                OnToDateOutQtyChanged();
            }
        }
        private Int32 _ToDateOutQty;
        partial void OnToDateOutQtyChanging(Int32 value);
        partial void OnToDateOutQtyChanged();

        [DataMemberAttribute()]
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                _UnitPrice = value;
                //KMx: Không tính toán trong đây, dễ bị sai (19/11/2014 17:30).
                //TotalPrice = _AdjustedQty * _UnitPrice;
                RaisePropertyChanged("UnitPrice");
                //RaisePropertyChanged("TotalPrice");

            }
        }
        private decimal _UnitPrice;

        [DataMemberAttribute()]
        public decimal PackagePrice
        {
            get
            {
                return _PackagePrice;
            }
            set
            {
                if (_PackagePrice != value)
                {
                    _PackagePrice = value;
                    RaisePropertyChanged("PackagePrice");
                }
            }
        }
        private decimal _PackagePrice;

        [DataMemberAttribute()]
        public long? SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                if (_SupplierID != value)
                {
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                }
            }
        }
        private long? _SupplierID;

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        private RefGenericDrugDetail _RefGenMedProductDetails;
        public RefGenericDrugDetail RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                _RefGenMedProductDetails = value;
                if (_RefGenMedProductDetails != null)
                {
                    _DrugID = _RefGenMedProductDetails.DrugID;
                    _UnitPrice = RefGenMedProductDetails.UnitPrice;
                    _PackagePrice = _RefGenMedProductDetails.PackagePrice;

                    if (_RefGenMedProductDetails.SupplierMain != null)
                    {
                        SupplierID = _RefGenMedProductDetails.SupplierMain.SupplierID;
                    }
                    else
                    {
                        SupplierID = 0;
                    }
                }
                else
                {
                    SupplierID = 0;
                    UnitPrice = 0;
                    PackagePrice = 0;
                    DrugID = 0;
                }
                RaisePropertyChanged("DrugID");
                RaisePropertyChanged("SupplierID");
                RaisePropertyChanged("UnitPrice");
                RaisePropertyChanged("PackagePrice");
                RaisePropertyChanged("RefGenMedProductDetails");
            }
        }
        #endregion

        #region Extension Properties

        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                RaisePropertyChanged("Checked");
            }
        }
        private bool _Checked;

        [DataMemberAttribute()]
        private Double _PackageQty;
        public Double PackageQty
        {
            get
            {
                return _PackageQty;
            }
            set
            {
                if (_PackageQty != value)
                {
                    _PackageQty = value;
                    RaisePropertyChanged("PackageQty");
                    //KMx: Không tính toán trong đây, dễ bị sai (19/11/2014 17:30).
                    //if (RefGenMedProductDetails != null)
                    //{
                    //    _AdjustedQty = (int)_PackageQty * RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    //    _TotalPrice = _AdjustedQty * _UnitPrice;
                    //    RaisePropertyChanged("AdjustedQty");
                    //    RaisePropertyChanged("TotalPrice");
                    //}
                    if (EntityState == EntityState.PERSITED || EntityState == EntityState.MODIFIED)
                    {
                        EntityState = EntityState.MODIFIED;
                    }
                }
            }
        }


        [DataMemberAttribute()]
        private int _OutAverageQty;
        public int OutAverageQty
        {
            get
            {
                return _OutAverageQty;
            }
            set
            {
                _OutAverageQty = value;
                RaisePropertyChanged("OutAverageQty");
            }
        }

        private double _Priority;
        public double Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
                RaisePropertyChanged("Priority");
            }
        }


        [DataMemberAttribute()]
        public decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                if (_TotalPrice != value)
                {
                    _TotalPrice = value;
                    RaisePropertyChanged("TotalPrice");
                }
            }
        }
        private decimal _TotalPrice;

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

        public override bool Equals(object obj)
        {
            PharmacyEstimationForPODetail seletedStore = obj as PharmacyEstimationForPODetail;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DrugID == seletedStore.DrugID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
