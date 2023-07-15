using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PromotionPlan : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PromotionPlan object.

        /// <param name="promID">Initial value of the PromID property.</param>
        /// <param name="promName">Initial value of the PromName property.</param>
        /// <param name="promFromDate">Initial value of the PromFromDate property.</param>
        /// <param name="promToDate">Initial value of the PromToDate property.</param>
        public static PromotionPlan CreatePromotionPlan(Byte promID, String promName, DateTime promFromDate, DateTime promToDate)
        {
            PromotionPlan promotionPlan = new PromotionPlan();
            promotionPlan.PromID = promID;
            promotionPlan.PromName = promName;
            promotionPlan.PromFromDate = promFromDate;
            promotionPlan.PromToDate = promToDate;
            return promotionPlan;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Byte PromID
        {
            get
            {
                return _PromID;
            }
            set
            {
                if (_PromID != value)
                {
                    OnPromIDChanging(value);
                    ////ReportPropertyChanging("PromID");
                    _PromID = value;
                    RaisePropertyChanged("PromID");
                    OnPromIDChanged();
                }
            }
        }
        private Byte _PromID;
        partial void OnPromIDChanging(Byte value);
        partial void OnPromIDChanged();





        [DataMemberAttribute()]
        public String PromName
        {
            get
            {
                return _PromName;
            }
            set
            {
                OnPromNameChanging(value);
                ////ReportPropertyChanging("PromName");
                _PromName = value;
                RaisePropertyChanged("PromName");
                OnPromNameChanged();
            }
        }
        private String _PromName;
        partial void OnPromNameChanging(String value);
        partial void OnPromNameChanged();





        [DataMemberAttribute()]
        public DateTime PromFromDate
        {
            get
            {
                return _PromFromDate;
            }
            set
            {
                OnPromFromDateChanging(value);
                ////ReportPropertyChanging("PromFromDate");
                _PromFromDate = value;
                RaisePropertyChanged("PromFromDate");
                OnPromFromDateChanged();
            }
        }
        private DateTime _PromFromDate;
        partial void OnPromFromDateChanging(DateTime value);
        partial void OnPromFromDateChanged();





        [DataMemberAttribute()]
        public DateTime PromToDate
        {
            get
            {
                return _PromToDate;
            }
            set
            {
                OnPromToDateChanging(value);
                ////ReportPropertyChanging("PromToDate");
                _PromToDate = value;
                RaisePropertyChanged("PromToDate");
                OnPromToDateChanged();
            }
        }
        private DateTime _PromToDate;
        partial void OnPromToDateChanging(DateTime value);
        partial void OnPromToDateChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROMOTIO_REL_HOSFM_PROMOTIO", "PromotionalServices")]
        public ObservableCollection<PromotionalService> PromotionalServices
        {
            get;
            set;
        }

        #endregion
    }
}
