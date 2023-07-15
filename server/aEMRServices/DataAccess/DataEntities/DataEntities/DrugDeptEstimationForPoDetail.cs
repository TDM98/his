using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using Service.Core.Common;

namespace DataEntities
{
    public partial class DrugDeptEstimationForPoDetail : EntityBase
    {
        #region Factory Method
        public static DrugDeptEstimationForPoDetail CreateDrugDeptEstimationForPoDetail(Int64 drugDeptEstPoDetailID, Int64 drugDeptEstimatePoID, Int64 genMedProductID, Int32 outQtyPrevFirstMonth, Int32 outQtyPrevSecondMonth, Int32 outQtyPrevThirdMonth, Int32 outQtyPrevFourthMonth, Int32 remainQty, double EstimatedQty_F, Int32 adjustedQty, double NumberOfEstimatedMonths_F, Int32 outQtyLastTwelveMonths, Int32 toDateOutQty, Int32 OutQty_F)
        {
            DrugDeptEstimationForPoDetail drugDeptEstimationForPoDetail = new DrugDeptEstimationForPoDetail();
            drugDeptEstimationForPoDetail.DrugDeptEstPoDetailID = drugDeptEstPoDetailID;
            drugDeptEstimationForPoDetail.DrugDeptEstimatePoID = drugDeptEstimatePoID;
            drugDeptEstimationForPoDetail.GenMedProductID = genMedProductID;
            drugDeptEstimationForPoDetail.OutQtyPrevFirstMonth = outQtyPrevFirstMonth;
            drugDeptEstimationForPoDetail.OutQtyPrevSecondMonth = outQtyPrevSecondMonth;
            drugDeptEstimationForPoDetail.OutQtyPrevThirdMonth = outQtyPrevThirdMonth;
            drugDeptEstimationForPoDetail.OutQtyPrevFourthMonth = outQtyPrevFourthMonth;
            drugDeptEstimationForPoDetail.RemainQty = remainQty;
            drugDeptEstimationForPoDetail.EstimatedQty_F = EstimatedQty_F;
            drugDeptEstimationForPoDetail.AdjustedQty = adjustedQty;
            drugDeptEstimationForPoDetail.NumberOfEstimatedMonths_F = NumberOfEstimatedMonths_F;
            drugDeptEstimationForPoDetail.OutQtyLastTwelveMonths = outQtyLastTwelveMonths;
            drugDeptEstimationForPoDetail.ToDateOutQty = toDateOutQty;
            drugDeptEstimationForPoDetail.OutQty_F = OutQty_F;
            return drugDeptEstimationForPoDetail;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptEstPoDetailID
        {
            get
            {
                return _DrugDeptEstPoDetailID;
            }
            set
            {
                if (_DrugDeptEstPoDetailID != value)
                {
                    OnDrugDeptEstPoDetailIDChanging(value);
                    _DrugDeptEstPoDetailID = value;
                    RaisePropertyChanged("DrugDeptEstPoDetailID");
                    OnDrugDeptEstPoDetailIDChanged();
                }
            }
        }
        private Int64 _DrugDeptEstPoDetailID;
        partial void OnDrugDeptEstPoDetailIDChanging(Int64 value);
        partial void OnDrugDeptEstPoDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 DrugDeptEstimatePoID
        {
            get
            {
                return _DrugDeptEstimatePoID;
            }
            set
            {
                OnDrugDeptEstimatePoIDChanging(value);
                _DrugDeptEstimatePoID = value;
                RaisePropertyChanged("DrugDeptEstimatePoID");
                OnDrugDeptEstimatePoIDChanged();
            }
        }
        private Int64 _DrugDeptEstimatePoID;
        partial void OnDrugDeptEstimatePoIDChanging(Int64 value);
        partial void OnDrugDeptEstimatePoIDChanged();


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
                OnAdjustedQtyChanging(value);
                _AdjustedQty = value;
                RaisePropertyChanged("AdjustedQty");
                //KMx: Không tính toán trong đây, dễ bị sai (19/11/2014 17:30).
                //_TotalPrice = _AdjustedQty * _UnitPrice;
                //RaisePropertyChanged("TotalPrice");

                //if (RefGenMedProductDetails != null)
                //{
                //    if (RefGenMedProductDetails.UnitPackaging > 0)
                //    {
                //        _QtyPackaging = _AdjustedQty / RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                //    }
                //    else
                //    {
                //        _QtyPackaging = _AdjustedQty;
                //    }
                //    RaisePropertyChanged("QtyPackaging");
                //}
                if (EntityState == EntityState.PERSITED || EntityState == EntityState.MODIFIED)
                {
                    EntityState = EntityState.MODIFIED;
                }
                OnAdjustedQtyChanged();
            }
        }
        private Int32 _AdjustedQty;
        partial void OnAdjustedQtyChanging(Int32 value);
        partial void OnAdjustedQtyChanged();

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
        private RefGenMedProductDetails _RefGenMedProductDetails;
        public RefGenMedProductDetails RefGenMedProductDetails
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
                    _RefGenMedProductDetails = value;
                    if (_RefGenMedProductDetails != null)
                    {
                        GenMedProductID = _RefGenMedProductDetails.GenMedProductID;
                        _UnitPrice = RefGenMedProductDetails.UnitPrice;
                        _PackagePrice = RefGenMedProductDetails.PackagePrice;

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
                        GenMedProductID = 0;
                    }
                    RaisePropertyChanged("GenMedProductID");
                    RaisePropertyChanged("SupplierID");
                    RaisePropertyChanged("UnitPrice");
                    RaisePropertyChanged("PackagePrice");
                    RaisePropertyChanged("RefGenMedProductDetails");
                }
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
        private double _QtyPackaging;
        public double QtyPackaging
        {
            get
            {
                return _QtyPackaging;
            }
            set
            {
                _QtyPackaging = value;
                RaisePropertyChanged("QtyPackaging");
                //KMx: Không tính toán trong đây, dễ bị sai (19/11/2014 17:30).
                //if (RefGenMedProductDetails != null)
                //{
                //    _AdjustedQty = (int)_QtyPackaging * RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
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

        private int _BidRemainingQty;
        private long? _BidDetailID;
        [DataMemberAttribute]
        public int BidRemainingQty
        {
            get
            {
                return _BidRemainingQty;
            }
            set
            {
                _BidRemainingQty = value;
                RaisePropertyChanged("BidRemainingQty");
            }
        }
        [DataMemberAttribute]
        public long? BidDetailID
        {
            get
            {
                return _BidDetailID;
            }
            set
            {
                _BidDetailID = value;
                RaisePropertyChanged("BidDetailID");
            }
        }

        [DataMemberAttribute()]
        public Int32 OutQty_F
        {
            get
            {
                return _OutQty_F;
            }
            set
            {
                OnOutQty_FChanging(value);
                _OutQty_F = value;
                RaisePropertyChanged("OutQty_F");
                OnOutQty_FChanged();
            }
        }
        private Int32 _OutQty_F;
        partial void OnOutQty_FChanging(Int32 value);
        partial void OnOutQty_FChanged();

        [DataMemberAttribute()]
        public String EstimationNote
        {
            get
            {
                return _EstimationNote;
            }
            set
            {
                OnEstimationNoteChanging(value);
                _EstimationNote = value;
                RaisePropertyChanged("EstimationNote");
                OnEstimationNoteChanged();
            }
        }
        private String _EstimationNote;
        partial void OnEstimationNoteChanging(String value);
        partial void OnEstimationNoteChanged();

        private string _ReqDrugInClinicDeptIDList;
        [DataMemberAttribute]
        public string ReqDrugInClinicDeptIDList
        {
            get
            {
                return _ReqDrugInClinicDeptIDList;
            }
            set
            {
                _ReqDrugInClinicDeptIDList = value;
                RaisePropertyChanged("ReqDrugInClinicDeptIDList");
            }
        }
    }
}