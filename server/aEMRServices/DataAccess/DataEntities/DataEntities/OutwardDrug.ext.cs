using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using System.ComponentModel;

namespace DataEntities
{
    public partial class OutwardDrug//: IInvoiceItem
    {
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
        public double DayRpts
        {
            get
            {
                return _DayRpts;
            }
            set
            {
                OnDayRptsChanging(value);
                _DayRpts = value;
                RaisePropertyChanged("DayRpts");
                OnDayRptsChanged();
            }
        }
        private double _DayRpts;
        partial void OnDayRptsChanging(double value);
        partial void OnDayRptsChanged();

        //dung cai nay de han che sai so
        [DataMemberAttribute()]
        public decimal QtyForDay
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
        private decimal _QtyForDay;


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
        public double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
            }
        }
        private double _DispenseVolume;

        //gia cho benh nhan thong thuong
        private Decimal _NormalPrice;
        [DataMemberAttribute()]
        public Decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");

            }
        }

        [DataMemberAttribute()]
        public Nullable<Boolean> HI
        {
            get
            {
                return _HI;
            }
            set
            {
                _HI = value;
                RaisePropertyChanged("HI");
            }
        }
        private Nullable<Boolean> _HI;

        [DataMemberAttribute()]
        public int OutQuantityOld
        {
            get
            {
                return _OutQuantityOld;
            }
            set
            {
                OnOutQuantityOldChanging(value);
                _OutQuantityOld = value;
                OnOutQuantityOldChanged();
            }
        }
        private int _OutQuantityOld;
        partial void OnOutQuantityOldChanging(int value);
        partial void OnOutQuantityOldChanged();

        [DataMemberAttribute()]
        public int OutQuantityReturn
        {
            get
            {
                return _OutQuantityReturn;
            }
            set
            {
                OnOutQuantityReturnChanging(value);
                _OutQuantityReturn = value;

                TotalPriceReturn = _OutQuantityReturn * _OutPrice;
                RaisePropertyChanged("OutQuantityReturn");
                RaisePropertyChanged("TotalPriceReturn");
                OnOutQuantityReturnChanged();
            }
        }
        private int _OutQuantityReturn;
        partial void OnOutQuantityReturnChanging(int value);
        partial void OnOutQuantityReturnChanged();

    

        [DataMemberAttribute()]
        public int QtyReturned
        {
            get
            {
                return _QtyReturned;
            }
            set
            {
                _QtyReturned = value;
                RaisePropertyChanged("QtyReturned");
            }
        }
        private int _QtyReturned;


        [DataMemberAttribute()]
        public int QtyOffer
        {
            get
            {
                return _QtyOffer;
            }
            set
            {
                if (_QtyOffer != value)
                {
                    _QtyOffer = value;
                    RaisePropertyChanged("QtyOffer");
                }

            }
        }
        private int _QtyOffer;


        [DataMemberAttribute()]
        public Decimal TotalPriceReturn
        {
            get
            {
                return _TotalPriceReturn;
            }
            set
            {
                OnTotalPriceReturnChanging(value);
                _TotalPriceReturn = value;
                RaisePropertyChanged("TotalPriceReturn");
                OnTotalPriceReturnChanged();
            }
        }
        private Decimal _TotalPriceReturn;
        partial void OnTotalPriceReturnChanging(Decimal value);
        partial void OnTotalPriceReturnChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> OutHIRebateReturn
        {
            get
            {
                return _OutHIRebateReturn;
            }
            set
            {
                _OutHIRebateReturn = value;
                RaisePropertyChanged("OutHIRebateReturn");
            }
        }
        private Nullable<Decimal> _OutHIRebateReturn;

        [DataMemberAttribute()]
        public Nullable<Decimal> PatientReturn
        {
            get
            {
                return _PatientReturn;
            }
            set
            {
                _PatientReturn = value;
                RaisePropertyChanged("PatientReturn");
            }
        }
        private Nullable<Decimal> _PatientReturn;

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
                RaisePropertyChanged("SdlDescription");
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

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
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();

            }
        }
        private DateTime _InExpiryDate;
        partial void OnInExpiryDateChanging(DateTime value);
        partial void OnInExpiryDateChanged();

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
        public int IsLoad
        {
            get
            {
                return _IsLoad;
            }
            set
            {
                _IsLoad = value;
                RaisePropertyChanged("IsLoad");
            }
        }
        private int _IsLoad = 0;

        private bool _IsNotVat;
        [DataMemberAttribute()]
        public bool IsNotVat
        {
            get
            {
                return _IsNotVat;
            }
            set
            {
                if (_IsNotVat != value)
                {
                    _IsNotVat = value;
                    RaisePropertyChanged("IsNotVat");
                }
            }
        }



        public static ValidationResult ValidateQtySell(int OutQuantity, ValidationContext context)
        {
            if (OutQuantity < 0)
            {
                return new ValidationResult("Số lượng không được nhỏ hơn 0!", new string[] { "OutQuantity" });
            }
            if (((OutwardDrug)context.ObjectInstance).GetDrugForSellVisitor != null)
            {
                if (OutQuantity > ((OutwardDrug)context.ObjectInstance).GetDrugForSellVisitor.RemainingFirst)
                {
                    return new ValidationResult("Số lượng còn lại " + ((OutwardDrug)context.ObjectInstance).GetDrugForSellVisitor.RemainingFirst.ToString() + " không đủ để bán!", new string[] { "OutQuantity" });
                }
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }

        private bool SoSanhNgay(DateTime t1, DateTime t2)
        {
            //false:t1>t2 ;true t2 >= t1 
            int year1 = t1.Year;
            int year2 = t2.Year;
            int month1 = t1.Month;
            int month2 = t2.Month;
            int day1 = t1.Day;
            int day2 = t2.Day;
            if (year1 > year2)
            {
                //t1>t2
                return false;
            }
            else
            {
                if (year1 == year2)
                {
                    if (month1 > month2)
                    {
                        return false;
                    }
                    else
                    {
                        if (month1 == month2)
                        {
                            if (day1 > day2)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return true;
        }

        partial void OnOutQuantityReturnChanging(int value)
        {
           // string StrError2 = "Thuốc này đã hết hạn dùng.Không thể trả được!";
            string StrError1 = "Số lượng trả phải <= " + (OutQuantityOld - QtyReturned).ToString();
            string StrError0 = "Số lượng trả không hợp lệ.";
            if (value > 0)
            {
                RemoveError("OutQuantityReturn", StrError0);
                //het han dung chi canh bao thoi.van cho tra tuy theo nhan vien ban hang
                //if (!SoSanhNgay(DateTime.Now, InExpiryDate))
                //{
                //    AddError("OutQuantityReturn", StrError2, false);
                //}
                //else
                //{
                //    RemoveError("OutQuantityReturn", StrError2);
                //}


                if (value > (OutQuantityOld - QtyReturned))
                {
                    AddError("OutQuantityReturn", StrError1, false);
                }
                else
                {
                    RemoveError("OutQuantityReturn", StrError1);
                }
            }
            else if (value < 0)
            {
                AddError("OutQuantityReturn", StrError0, false);
                RemoveError("OutQuantityReturn", StrError1);
               // RemoveError("OutQuantityReturn", StrError2);
            }
            else
            {
                RemoveError("OutQuantityReturn", StrError0);
                RemoveError("OutQuantityReturn", StrError1);
               // RemoveError("OutQuantityReturn", StrError2);
            }

        }

        #region IInvoiceItem Members

        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                return _InwardDrug;
            }
        }

        #endregion

        public override string ChargeableItemName
        {
            get
            {
                if (GetDrugForSellVisitor != null)
                {
                    return GetDrugForSellVisitor.BrandName;
                }
                return base.ChargeableItemName;
            }
        }

        private InwardDrug _InwardDrug;
        [DataMemberAttribute()]
        public InwardDrug InwardDrug
        {
            get
            {
                return _InwardDrug;
            }
            set
            {
                if (_InwardDrug != value)
                {
                    _InwardDrug = value;
                    RaisePropertyChanged("InwardDrug");
                }
            }
        }
        public override bool Equals(object obj)
        {
            OutwardDrug info = obj as OutwardDrug;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.OutID > 0 && this.OutID == info.OutID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [DataMemberAttribute()]
        public override AllLookupValues.ExamRegStatus ExamRegStatus
        {
            get
            {
                if(OutwardDrugInvoice != null)
                {
                    return OutwardDrugInvoice.ExamRegStatus;
                }
                return base.ExamRegStatus;
            }
            set
            {
                base.ExamRegStatus = value;
            }
        }
    }
}
