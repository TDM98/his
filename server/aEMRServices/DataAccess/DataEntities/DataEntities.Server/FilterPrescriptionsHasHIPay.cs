using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class FilterPrescriptionsHasHIPay : NotifyChangedBase
    {

        #region Factory Method
        /// Create a new filterPrescriptions object.

        /// <param name="FilterID">Initial value of the FilterID property.</param>
        /// <param name="FilterType">Initial value of the FilterType property.</param>
        /// <param name="HIPayFrom">Initial value of the HIPayFrom property.</param>
        /// <param name="HIPayTo">Initial value of the HIPayTo property.</param>
        /// <param name="IsBlock">Initial value of the IsBlock property.</param>
        /// <param name="ListICDSkip">Initial value of the ListICDSkip property.</param>
        /// <param name="ListPatientCodeSkip">Initial value of the ListPatientCodeSkip property.</param>
        /// <param name="IsUsed">Initial value of the IsUsed property.</param>
        /// <param name="Day">Initial value of the Day property.</param>
        /// <param name="AllPrescription">Initial value of the AllPrescription property.</param>
        /// <param name="ListDrugCodeSkip">Initial value of the ListDrugCodeSkip property.</param>
        public static FilterPrescriptionsHasHIPay CreateFilterPrescriptionsHasHIPay(long FilterID, int FilterType, int HIPayFrom
            , int HIPayTo, bool IsBlock, string ListICDSkip, string ListPatientCodeSkip, bool IsUsed, int Day, string ListDrugCodeSkip)
        {
            FilterPrescriptionsHasHIPay filterPrescriptions = new FilterPrescriptionsHasHIPay
            {
                FilterID = FilterID,
                FilterType = FilterType,
                HIPayFrom = HIPayFrom,
                HIPayTo = HIPayTo,
                IsBlock = IsBlock,
                ListICDSkip = ListICDSkip,
                ListPatientCodeSkip = ListPatientCodeSkip,
                IsUsed = IsUsed,
                Day = Day,
                ListDrugCodeSkip = ListDrugCodeSkip
            };
            return filterPrescriptions;
        }

        public int GetHIPayFrom(string CurICD10Main, ObservableCollection<PrescriptionDetail> PrescriptionDetails)
        {
            if( AnotherHIPayFrom == 0 
                || string.IsNullOrEmpty(ListSpecialDrugCode) 
                || string.IsNullOrEmpty(ListSpecialICD) 
                || string.IsNullOrEmpty(CurICD10Main) 
                || PrescriptionDetails == null)
            {
                return HIPayFrom;
            }

            if( ListSpecialICD.Contains(CurICD10Main)
                && PrescriptionDetails.Where(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null 
                && ListSpecialDrugCode.Contains(x.SelectedDrugForPrescription.DrugCode)).Count() > 0)
            {
                return AnotherHIPayFrom;
            }
                
            return HIPayFrom;
        }
        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long FilterID
        {
            get
            {
                return _FilterID;
            }
            set
            {
                if (_FilterID != value)
                {
                    _FilterID = value;
                    RaisePropertyChanged("FilterID");
                }
            }
        }
        private long _FilterID;

        [DataMemberAttribute()]
        public int FilterType
        {
            get
            {
                return _FilterType;
            }
            set
            {
                _FilterType = value;
                RaisePropertyChanged("FilterType");
            }
        }
        private int _FilterType;

        [DataMemberAttribute()]
        public int HIPayFrom
        {
            get
            {
                return _HIPayFrom;
            }
            set
            {
                _HIPayFrom = value;
                RaisePropertyChanged("HIPayFrom");
            }
        }
        private int _HIPayFrom;

        [DataMemberAttribute()]
        public int HIPayTo
        {
            get
            {
                return _HIPayTo;
            }
            set
            {
                _HIPayTo = value;
                RaisePropertyChanged("HIPayTo");
            }
        }
        private int _HIPayTo;

        [DataMemberAttribute()]
        public string ListICDSkip
        {
            get
            {
                return _ListICDSkip;
            }
            set
            {
                _ListICDSkip = value;
                RaisePropertyChanged("ListICDSkip");
            }
        }
        private string _ListICDSkip;

        [DataMemberAttribute()]
        public string ListPatientCodeSkip
        {
            get
            {
                return _ListPatientCodeSkip;
            }
            set
            {
                _ListPatientCodeSkip = value;
                RaisePropertyChanged("ListPatientCodeSkip");
            }
        }
        private string _ListPatientCodeSkip;

        [DataMemberAttribute()]
        public bool IsBlock
        {
            get
            {
                return _IsBlock;
            }
            set
            {
                _IsBlock = value;
                RaisePropertyChanged("IsBlock");
            }
        }
        private bool _IsBlock;

        [DataMemberAttribute()]
        public bool IsUsed
        {
            get
            {
                return _IsUsed;
            }
            set
            {
                _IsUsed = value;
                RaisePropertyChanged("IsUsed");
            }
        }
        private bool _IsUsed = false;

        [DataMemberAttribute()]
        public bool AllPrescription
        {
            get
            {
                return _AllPrescription;
            }
            set
            {
                _AllPrescription = value;
                RaisePropertyChanged("AllPrescription");
            }
        }
        private bool _AllPrescription = false;

        [DataMemberAttribute()]
        public int Day
        {
            get
            {
                return _Day;
            }
            set
            {
                _Day = value;
                RaisePropertyChanged("Day");
            }
        }
        private int _Day;
        [DataMemberAttribute()]
        public string ListDrugCodeSkip
        {
            get
            {
                return _ListDrugCodeSkip;
            }
            set
            {
                _ListDrugCodeSkip = value;
                RaisePropertyChanged("ListDrugCodeSkip");
            }
        }
        private string _ListDrugCodeSkip;
        [DataMemberAttribute()]
        public int AnotherHIPayFrom
        {
            get
            {
                return _AnotherHIPayFrom;
            }
            set
            {
                _AnotherHIPayFrom = value;
                RaisePropertyChanged("AnotherHIPayFrom");
            }
        }
        private int _AnotherHIPayFrom;
        [DataMemberAttribute()]
        public string ListSpecialDrugCode
        {
            get
            {
                return _ListSpecialDrugCode;
            }
            set
            {
                _ListSpecialDrugCode = value;
                RaisePropertyChanged("ListSpecialDrugCode");
            }
        }
        private string _ListSpecialDrugCode;
        [DataMemberAttribute()]
        public string ListSpecialICD
        {
            get
            {
                return _ListSpecialICD;
            }
            set
            {
                _ListSpecialICD = value;
                RaisePropertyChanged("ListSpecialICD");
            }
        }
        private string _ListSpecialICD;
        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is FilterPrescriptionsHasHIPay seletedfilterPrescriptions))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return FilterID == seletedfilterPrescriptions.FilterID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
