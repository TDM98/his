
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class GetGenMedProductForSell: NotifyChangedBase
    {
        public GetGenMedProductForSell()
        {
        }
        #region Factory Method
        /// Create a new GetGenMedProductForSell object.
        public static GetGenMedProductForSell CreateGetGenMedProductForSell(int Remaining, long GenMedProductID, int RequiredNumber)
        {
            GetGenMedProductForSell GetGenMedProductForSell = new GetGenMedProductForSell();
            GetGenMedProductForSell.Remaining = Remaining;
            GetGenMedProductForSell.GenMedProductID = GenMedProductID;
            GetGenMedProductForSell.RequiredNumber = RequiredNumber;
            return GetGenMedProductForSell;
        }
        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public int Remaining
        {
            get
            {
                return _remaining;
            }
            set
            {
                OnRemainingChanging(value);
                _remaining = value;
                RaisePropertyChanged("Remaining");
                OnRemainingChanged();
            }
        }
        private int _remaining;
        partial void OnRemainingChanging(int value);
        partial void OnRemainingChanged();

        [DataMemberAttribute()]
        public double RequiredNumber
        {
            get
            {
                return _requiredNumber;
            }
            set
            {
                OnRequiredNumberChanging(value);
                _requiredNumber = value;
                RaisePropertyChanged("RequiredNumber");
                OnRequiredNumberChanged();
            }
        }
        private double _requiredNumber;
        partial void OnRequiredNumberChanging(double value);
        partial void OnRequiredNumberChanged();
       
        [DataMemberAttribute()]
        public long GenMedProductID
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
        private long _GenMedProductID;
        partial void OnGenMedProductIDChanging(long value);
        partial void OnGenMedProductIDChanged();

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                OnBrandNameChanging(value);
                _BrandName = value;
                RaisePropertyChanged("BrandName");
                OnBrandNameChanged();
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();


        [DataMemberAttribute()]
        public String GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                OnGenericNameChanging(value);
                _GenericName = value;
                RaisePropertyChanged("GenericName");
                OnGenericNameChanged();
            }
        }
        private String _GenericName;
        partial void OnGenericNameChanging(String value);
        partial void OnGenericNameChanged();

        [DataMemberAttribute()]
        public String InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                OnInBatchNumberChanging(value);
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();
     
        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                OnUnitNameChanging(value);
                _UnitName = value;
                RaisePropertyChanged("UnitName");
                OnUnitNameChanged();
            }
        }
        private String _UnitName;
        partial void OnUnitNameChanging(String value);
        partial void OnUnitNameChanged();

        [DataMemberAttribute()]
        public Decimal SellingPrice
        {
            get
            {
                return _SellingPrice;
            }
            set
            {
                OnSellingPriceChanging(value);
                _SellingPrice = value;
                RaisePropertyChanged("SellingPrice");
                OnSellingPriceChanged();
            }
        }
        private Decimal _SellingPrice;
        partial void OnSellingPriceChanging(Decimal value);
        partial void OnSellingPriceChanged();

        [DataMemberAttribute()]
        public Decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }
        private Decimal _InCost;

        [DataMemberAttribute()]
        public Decimal OutPrice
        {
            get
            {
                return _OutPrice;
            }
            set
            {
                _OutPrice = value;
                RaisePropertyChanged("OutPrice");
            }
        }
        private Decimal _OutPrice;

        #region Sell Price Member
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                if (_NormalPrice != value)
                {
                    _NormalPrice = value;
                    RaisePropertyChanged("NormalPrice");
                }
            }
        }
        private decimal _NormalPrice;

        [DataMemberAttribute()]
        public decimal PriceForHIPatient
        {
            get
            {
                return _PriceForHIPatient;
            }
            set
            {
                if (_PriceForHIPatient != value)
                {
                    _PriceForHIPatient = value;
                    RaisePropertyChanged("PriceForHIPatient");
                }
            }
        }
        private decimal _PriceForHIPatient;

        //cot nay thay doi tuy theo gia tri truyen vao
        [DataMemberAttribute()]
        public Decimal HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
            }
        }
        private Decimal _HIAllowedPrice;

        //cot nay co dinh vi lay trong bang gia ra,khong dung de luu
        [DataMemberAttribute()]
        public Decimal HIAllowedPriceNoChange
        {
            get
            {
                return _HIAllowedPriceNoChange;
            }
            set
            {
                _HIAllowedPriceNoChange = value;
                RaisePropertyChanged("HIAllowedPriceNoChange");
            }
        }
        private Decimal _HIAllowedPriceNoChange;
        #endregion

        [DataMemberAttribute()]
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                OnSTTChanging(value);
                _STT = value;
                RaisePropertyChanged("STT");
                OnSTTChanged();
            }
        }
        private long _STT;
        partial void OnSTTChanging(long value);
        partial void OnSTTChanged();

        [DataMemberAttribute()]
        public long InID
        {
            get
            {
                return _InID;
            }
            set
            {
                OnInIDChanging(value);
                _InID = value;
                RaisePropertyChanged("InID");
                OnInIDChanged();
            }
        }
        private long _InID;
        partial void OnInIDChanging(long value);
        partial void OnInIDChanged();


        [DataMemberAttribute()]
        public String UnitUse
        {
            get
            {
                return _UnitUse;
            }
            set
            {
                _UnitUse = value;
                RaisePropertyChanged("UnitUse");
            }
        }
        private String _UnitUse;


        [DataMemberAttribute()]
        public String Dosage
        {
            get
            {
                return _Dosage;
            }
            set
            {
                OnDosageChanging(value);
                _Dosage = value;
                RaisePropertyChanged("Dosage");
                OnDosageChanged();
            }
        }
        private String _Dosage;
        partial void OnDosageChanging(String value);
        partial void OnDosageChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> InsuranceCover
        {
            get
            {
                return _InsuranceCover;
            }
            set
            {
                OnInsuranceCoverChanging(value);
                _InsuranceCover = value;
                RaisePropertyChanged("InsuranceCover");
                OnInsuranceCoverChanged();
            }
        }
        private Nullable<Boolean> _InsuranceCover;
        partial void OnInsuranceCoverChanging(Nullable<Boolean> value);
        partial void OnInsuranceCoverChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsConsult
        {
            get
            {
                return _IsConsult;
            }
            set
            {
                OnIsConsultChanging(value);
                _IsConsult = value;
                RaisePropertyChanged("IsConsult");
                OnIsConsultChanged();
            }
        }
        private Nullable<Boolean> _IsConsult;
        partial void OnIsConsultChanging(Nullable<Boolean> value);
        partial void OnIsConsultChanged();


        [DataMemberAttribute()]
        public Double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                OnDispenseVolumeChanging(value);
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
                OnDispenseVolumeChanged();
            }
        }
        private Double _DispenseVolume;
        partial void OnDispenseVolumeChanging(Double value);
        partial void OnDispenseVolumeChanged();


        [DataMemberAttribute()]
        public Double UnitVolume
        {
            get
            {
                return _UnitVolume;
            }
            set
            {
                OnUnitVolumeChanging(value);
                _UnitVolume = value;
                RaisePropertyChanged("UnitVolume");
                OnUnitVolumeChanged();
            }
        }
        private Double _UnitVolume;
        partial void OnUnitVolumeChanging(Double value);
        partial void OnUnitVolumeChanged();

        [DataMemberAttribute()]
        public Int16? MaxDayPrescribed
        {
            get
            {
                return _MaxDayPrescribed;
            }
            set
            {
                _MaxDayPrescribed = value;
                RaisePropertyChanged("MaxDayPrescribed");
            }
        }
        private Int16? _MaxDayPrescribed;


        [DataMemberAttribute()]
        public bool IsSearchByGenName
        {
            get
            {
                return _isSearchByGenName;
            }
            set
            {
                OnIsSearchByGenNameChanging(value);
                _isSearchByGenName = value;
                RaisePropertyChanged("IsSearchByGenName");
                OnIsSearchByGenNameChanged();
            }
        }
        private bool _isSearchByGenName;
        partial void OnIsSearchByGenNameChanging(bool value);
        partial void OnIsSearchByGenNameChanged();


        [DataMemberAttribute()]
        public string Packaging
        {
            get
            {
                return _Packaging;
            }
            set
            {
                _Packaging = value;
                RaisePropertyChanged("Packaging");
            }
        }
        private string _Packaging;

        [DataMemberAttribute()]
        public string Visa
        {
            get
            {
                return _Visa;
            }
            set
            {
                _Visa = value;
                RaisePropertyChanged("Visa");
            }
        }
        private string _Visa;

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
        public string HICode
        {
            get
            {
                return _HICode;
            }
            set
            {
                _HICode = value;
                RaisePropertyChanged("HICode");
            }
        }
        private string _HICode;
        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                OnSdlDescriptionChanging(value);
                _SdlDescription = value;
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

        private String _Content;
        [DataMemberAttribute()]
        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                RaisePropertyChanged("Content");
            }
        }

        private String _Administration;
        [DataMemberAttribute()]
        public String Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                _Administration = value;
                RaisePropertyChanged("Administration");
            }
        }

        [DataMemberAttribute()]
        public DateTime InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                OnInExpiryDateChanging(value);
                _InExpiryDate = value;
                OnInExpiryDateChanged();
            }
        }
        private DateTime _InExpiryDate;
        partial void OnInExpiryDateChanging(DateTime value);
        partial void OnInExpiryDateChanged();

        [DataMemberAttribute()]
        public int RemainingFirst
        {
            get
            {
                return _RemainingFirst;
            }
            set
            {
                _RemainingFirst = value;
                RaisePropertyChanged("RemainingFirst");
            }
        }
        private int _RemainingFirst;

        [DataMemberAttribute()]
        public int Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private int _Qty;

        [DataMemberAttribute()]
        public double DayRpts
        {
            get
            {
                return _DayRpts;
            }
            set
            {
                _DayRpts = value;
                RaisePropertyChanged("DayRpts");
            }
        }
        private double _DayRpts;

        //dung cai nay de han che sai so
        [DataMemberAttribute()]
        public double QtyForDay
        {
            get
            {
                return _QtyForDay;
            }
            set
            {
                _QtyForDay = value;
                RaisePropertyChanged("QtyForDay");
            }
        }
        private double _QtyForDay;

        partial void OnRequiredNumberChanging(double value)
        {
            if (value > Remaining)
            {
                if (Remaining == 0)
                {
                    AddError("RequiredNumber", "Thuốc này không còn trong kho", false);
                }
                else
                {
                    AddError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString(), false);
                }
            }
            else
            {
                RemoveError("RequiredNumber", "Thuốc này không còn trong kho");
                RemoveError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString());
            }
        }


        [DataMemberAttribute()]
        public long? PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private long? _PrescriptID;
        partial void OnPrescriptIDChanging(long? value);
        partial void OnPrescriptIDChanged();

        [DataMemberAttribute()]
        public long? IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                OnIssueIDChanging(value);
                _IssueID = value;
                RaisePropertyChanged("IssueID");
                OnIssueIDChanged();
            }
        }
        private long? _IssueID;
        partial void OnIssueIDChanging(long? value);
        partial void OnIssueIDChanged();

        [DataMemberAttribute()]
        public DateTime? IssuedDateTime
        {
            get
            {
                return _IssuedDateTime;
            }
            set
            {
                OnIssuedDateTimeChanging(value);
                _IssuedDateTime = value;
                RaisePropertyChanged("IssuedDateTime");
                OnIssuedDateTimeChanged();
            }
        }
        private DateTime? _IssuedDateTime;
        partial void OnIssuedDateTimeChanging(DateTime? value);
        partial void OnIssuedDateTimeChanged();

        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                OnIsCheckedChanging(value);
                _Checked = value;
                RaisePropertyChanged("Checked");
                OnIsCheckedChanged();
            }
        }
        private bool _Checked;
        partial void OnIsCheckedChanging(bool value);
        partial void OnIsCheckedChanged();

        [DataMemberAttribute()]
        public long? GenMedProductIDChanged
        {
            get
            {
                return _GenMedProductIDChanged;
            }
            set
            {
                _GenMedProductIDChanged = value;
                RaisePropertyChanged("GenMedProductIDChanged");
            }
        }
        private long? _GenMedProductIDChanged;

        [DataMemberAttribute()]
        public string Precaution_Warn
        {
            get
            {
                return _Precaution_Warn;
            }
            set
            {
                _Precaution_Warn = value;
                RaisePropertyChanged("Precaution_Warn");
            }
        }
        private string _Precaution_Warn;

        [DataMemberAttribute()]
        public bool IsWarningHI
        {
            get
            {
                return _IsWarningHI;
            }
            set
            {
                _IsWarningHI = value;
                RaisePropertyChanged("IsWarningHI");
            }
        }
        private bool _IsWarningHI;

        [DataMemberAttribute()]
        public Int64 V_DrugType
        {
            get
            {
                return _V_DrugType;
            }
            set
            {
                _V_DrugType = value;
            }
        }
        private Int64 _V_DrugType;


        [DataMemberAttribute()]
        public double QtyMaxAllowed
        {
            get
            {
                return _QtyMaxAllowed;
            }
            set
            {
                _QtyMaxAllowed = value;
                RaisePropertyChanged("QtyMaxAllowed");
            }
        }
        private double _QtyMaxAllowed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedMon
        {
            get
            {
                return _QtySchedMon;
            }
            set
            {
                _QtySchedMon = value;
                RaisePropertyChanged("QtySchedMon");
            }
        }
        private Nullable<Single> _QtySchedMon;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedTue
        {
            get
            {
                return _QtySchedTue;
            }
            set
            {
                _QtySchedTue = value;
                RaisePropertyChanged("QtySchedTue");
            }
        }
        private Nullable<Single> _QtySchedTue;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedWed
        {
            get
            {
                return _QtySchedWed;
            }
            set
            {
                _QtySchedWed = value;
                RaisePropertyChanged("QtySchedWed");
            }
        }
        private Nullable<Single> _QtySchedWed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedThu
        {
            get
            {
                return _QtySchedThu;
            }
            set
            {
                _QtySchedThu = value;
                RaisePropertyChanged("QtySchedThu");
            }
        }
        private Nullable<Single> _QtySchedThu;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedFri
        {
            get
            {
                return _QtySchedFri;
            }
            set
            {
                _QtySchedFri = value;
                RaisePropertyChanged("QtySchedFri");
            }
        }
        private Nullable<Single> _QtySchedFri;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSat
        {
            get
            {
                return _QtySchedSat;
            }
            set
            {
                _QtySchedSat = value;
                RaisePropertyChanged("QtySchedSat");
            }
        }
        private Nullable<Single> _QtySchedSat;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSun
        {
            get
            {
                return _QtySchedSun;
            }
            set
            {
                _QtySchedSun = value;
                RaisePropertyChanged("QtySchedSun");
            }
        }
        private Nullable<Single> _QtySchedSun;


        [DataMemberAttribute()]
        public Nullable<byte> SchedBeginDOW
        {
            get
            {
                return _SchedBeginDOW;
            }
            set
            {
                _SchedBeginDOW = value;
                RaisePropertyChanged("SchedBeginDOW");
            }
        }
        private Nullable<byte> _SchedBeginDOW;


        [DataMemberAttribute()]
        public double? HIPaymentPercent
        {
            get
            {
                return _HIPaymentPercent;
            }
            set
            {
                _HIPaymentPercent = value;
                RaisePropertyChanged("HIPaymentPercent");
            }
        }
        private double? _HIPaymentPercent; 

        #endregion

        public override bool Equals(object obj)
        {
            GetGenMedProductForSell seletedStore = obj as GetGenMedProductForSell;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.GenMedProductID == seletedStore.GenMedProductID && this.BrandName == seletedStore.BrandName
                && this.BrandName!=null && seletedStore.BrandName!=null;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

}
