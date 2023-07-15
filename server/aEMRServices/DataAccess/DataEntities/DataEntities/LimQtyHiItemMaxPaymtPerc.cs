using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class LimQtyHiItemMaxPaymtPerc : NotifyChangedBase
    {
        private long _LimQtyHiItemMaxPaymtID = 0;
        [DataMemberAttribute]
        public long LimQtyHiItemMaxPaymtID
        {
            get
            {
                return _LimQtyHiItemMaxPaymtID;
            }
            set
            {
                if (_LimQtyHiItemMaxPaymtID != value)
                {
                    _LimQtyHiItemMaxPaymtID = value;
                    RaisePropertyChanged("LimQtyHiItemMaxPaymtID");
                }
            }
        }

        private string _LimQtyHiItemMaxPaymtName = "";
        [DataMemberAttribute]
        public string LimQtyHiItemMaxPaymtName
        {
            get
            {
                return _LimQtyHiItemMaxPaymtName;
            }
            set
            {
                if (_LimQtyHiItemMaxPaymtName != value)
                {
                    _LimQtyHiItemMaxPaymtName = value;
                    RaisePropertyChanged("LimQtyHiItemMaxPaymtName");
                }
            }
        }

        private decimal _ItemNumber1MaxPayAmt = 0;
        [DataMemberAttribute]
        public decimal ItemNumber1MaxPayAmt
        {
            get
            {
                return _ItemNumber1MaxPayAmt;
            }
            set
            {
                if (_ItemNumber1MaxPayAmt != value)
                {
                    _ItemNumber1MaxPayAmt = value;
                    RaisePropertyChanged("ItemNumber1MaxPayAmt");
                }
            }
        }

        private decimal _ItemNumber1MaxPayPerc = 0;
        [DataMemberAttribute]
        public decimal ItemNumber1MaxPayPerc
        {
            get
            {
                return _ItemNumber1MaxPayPerc;
            }
            set
            {
                if (_ItemNumber1MaxPayPerc != value)
                {
                    _ItemNumber1MaxPayPerc = value;
                    RaisePropertyChanged("ItemNumber1MaxPayPerc");
                }
            }
        }

        private bool _ItemNumber1InHiGroup = false;
        [DataMemberAttribute]
        public bool ItemNumber1InHiGroup
        {
            get
            {
                return _ItemNumber1InHiGroup;
            }
            set
            {
                if (_ItemNumber1InHiGroup != value)
                {
                    _ItemNumber1InHiGroup = value;
                    RaisePropertyChanged("ItemNumber1InHiGroup");
                }
            }
        }


        private decimal _ItemNumber2MaxPayAmt = 0;
        [DataMemberAttribute]
        public decimal ItemNumber2MaxPayAmt
        {
            get
            {
                return _ItemNumber2MaxPayAmt;
            }
            set
            {
                if (_ItemNumber2MaxPayAmt != value)
                {
                    _ItemNumber2MaxPayAmt = value;
                    RaisePropertyChanged("ItemNumber2MaxPayAmt");
                }
            }
        }

        private decimal _ItemNumber2MaxPayPerc = 0;
        [DataMemberAttribute]
        public decimal ItemNumber2MaxPayPerc
        {
            get
            {
                return _ItemNumber2MaxPayPerc;
            }
            set
            {
                if (_ItemNumber2MaxPayPerc != value)
                {
                    _ItemNumber2MaxPayPerc = value;
                    RaisePropertyChanged("ItemNumber2MaxPayPerc");
                }
            }
        }

        private bool _ItemNumber2InHiGroup = false;
        [DataMemberAttribute]
        public bool ItemNumber2InHiGroup
        {
            get
            {
                return _ItemNumber2InHiGroup;
            }
            set
            {
                if (_ItemNumber2InHiGroup != value)
                {
                    _ItemNumber2InHiGroup = value;
                    RaisePropertyChanged("ItemNumber2InHiGroup");
                }
            }
        }

        private decimal _ItemNumber3MaxPayAmt = 0;
        [DataMemberAttribute]
        public decimal ItemNumber3MaxPayAmt
        {
            get
            {
                return _ItemNumber3MaxPayAmt;
            }
            set
            {
                if (_ItemNumber3MaxPayAmt != value)
                {
                    _ItemNumber3MaxPayAmt = value;
                    RaisePropertyChanged("ItemNumber3MaxPayAmt");
                }
            }
        }

        private decimal _ItemNumber3MaxPayPerc = 0;
        [DataMemberAttribute]
        public decimal ItemNumber3MaxPayPerc
        {
            get
            {
                return _ItemNumber3MaxPayPerc;
            }
            set
            {
                if (_ItemNumber3MaxPayPerc != value)
                {
                    _ItemNumber3MaxPayPerc = value;
                    RaisePropertyChanged("ItemNumber3MaxPayPerc");
                }
            }
        }

        private bool _ItemNumber3InHiGroup = false;
        [DataMemberAttribute]
        public bool ItemNumber3InHiGroup
        {
            get
            {
                return _ItemNumber3InHiGroup;
            }
            set
            {
                if (_ItemNumber3InHiGroup != value)
                {
                    _ItemNumber3InHiGroup = value;
                    RaisePropertyChanged("ItemNumber3InHiGroup");
                }
            }
        }

        private bool _IsActive = false;
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        private bool _IsDeleted = false;
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                }
            }
        }

        private decimal _ItemNumber1MaxBenefit = 0;
        [DataMemberAttribute]
        public decimal ItemNumber1MaxBenefit
        {
            get
            {
                return _ItemNumber1MaxBenefit;
            }
            set
            {
                if (_ItemNumber1MaxBenefit != value)
                {
                    _ItemNumber1MaxBenefit = value;
                    RaisePropertyChanged("ItemNumber1MaxBenefit");
                }
            }
        }

        private decimal _ItemNumber2MaxBenefit = 0;
        [DataMemberAttribute]
        public decimal ItemNumber2MaxBenefit
        {
            get
            {
                return _ItemNumber2MaxBenefit;
            }
            set
            {
                if (_ItemNumber2MaxBenefit != value)
                {
                    _ItemNumber2MaxBenefit = value;
                    RaisePropertyChanged("ItemNumber2MaxBenefit");
                }
            }
        }

        private decimal _ItemNumber3MaxBenefit = 0;
        [DataMemberAttribute]
        public decimal ItemNumber3MaxBenefit
        {
            get
            {
                return _ItemNumber3MaxBenefit;
            }
            set
            {
                if (_ItemNumber3MaxBenefit != value)
                {
                    _ItemNumber3MaxBenefit = value;
                    RaisePropertyChanged("ItemNumber3MaxBenefit");
                }
            }
        }
    }
}
