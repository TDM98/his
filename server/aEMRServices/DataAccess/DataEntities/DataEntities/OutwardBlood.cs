using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardBlood : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OutwardBlood object.

        /// <param name="outwBloodID">Initial value of the OutwBloodID property.</param>
        /// <param name="outwNumOfBloodUnits">Initial value of the OutwNumOfBloodUnits property.</param>
        public static OutwardBlood CreateOutwardBlood(long outwBloodID, Double outwNumOfBloodUnits)
        {
            OutwardBlood outwardBlood = new OutwardBlood();
            outwardBlood.OutwBloodID = outwBloodID;
            outwardBlood.OutwNumOfBloodUnits = outwNumOfBloodUnits;
            return outwardBlood;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long OutwBloodID
        {
            get
            {
                return _OutwBloodID;
            }
            set
            {
                if (_OutwBloodID != value)
                {
                    OnOutwBloodIDChanging(value);
                    ////ReportPropertyChanging("OutwBloodID");
                    _OutwBloodID = value;
                    RaisePropertyChanged("OutwBloodID");
                    OnOutwBloodIDChanged();
                }
            }
        }
        private long _OutwBloodID;
        partial void OnOutwBloodIDChanging(long value);
        partial void OnOutwBloodIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> OutwBloodInvoiceID
        {
            get
            {
                return _OutwBloodInvoiceID;
            }
            set
            {
                OnOutwBloodInvoiceIDChanging(value);
                ////ReportPropertyChanging("OutwBloodInvoiceID");
                _OutwBloodInvoiceID = value;
                RaisePropertyChanged("OutwBloodInvoiceID");
                OnOutwBloodInvoiceIDChanged();
            }
        }
        private Nullable<long> _OutwBloodInvoiceID;
        partial void OnOutwBloodInvoiceIDChanging(Nullable<long> value);
        partial void OnOutwBloodInvoiceIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> BloodTypeID
        {
            get
            {
                return _BloodTypeID;
            }
            set
            {
                OnBloodTypeIDChanging(value);
                ////ReportPropertyChanging("BloodTypeID");
                _BloodTypeID = value;
                RaisePropertyChanged("BloodTypeID");
                OnBloodTypeIDChanged();
            }
        }
        private Nullable<Byte> _BloodTypeID;
        partial void OnBloodTypeIDChanging(Nullable<Byte> value);
        partial void OnBloodTypeIDChanged();





        [DataMemberAttribute()]
        public Double OutwNumOfBloodUnits
        {
            get
            {
                return _OutwNumOfBloodUnits;
            }
            set
            {
                OnOutwNumOfBloodUnitsChanging(value);
                ////ReportPropertyChanging("OutwNumOfBloodUnits");
                _OutwNumOfBloodUnits = value;
                RaisePropertyChanged("OutwNumOfBloodUnits");
                OnOutwNumOfBloodUnitsChanged();
            }
        }
        private Double _OutwNumOfBloodUnits;
        partial void OnOutwNumOfBloodUnitsChanging(Double value);
        partial void OnOutwNumOfBloodUnitsChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> OutWUnitPrice
        {
            get
            {
                return _OutWUnitPrice;
            }
            set
            {
                OnOutWUnitPriceChanging(value);
                ////ReportPropertyChanging("OutWUnitPrice");
                _OutWUnitPrice = value;
                RaisePropertyChanged("OutWUnitPrice");
                OnOutWUnitPriceChanged();
            }
        }
        private Nullable<Decimal> _OutWUnitPrice;
        partial void OnOutWUnitPriceChanging(Nullable<Decimal> value);
        partial void OnOutWUnitPriceChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDB_REL_BB08_BLOODTYP", "BloodTypes")]
        public BloodType BloodType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDB_REL_BB09_OUTWARDI", "OutwardInvoiceBlood")]
        public OutwardInvoiceBlood OutwardInvoiceBlood
        {
            get;
            set;
        }

        #endregion
    }
}
