using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class DrugSellingPriceTable : NotifyChangedBase, IEditableObject
    {
        public DrugSellingPriceTable()
            : base()
        {

        }

        private DrugSellingPriceTable _tempDrugSellingPriceTable;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDrugSellingPriceTable = (DrugSellingPriceTable)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDrugSellingPriceTable)
                CopyFrom(_tempDrugSellingPriceTable);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DrugSellingPriceTable p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method
        /// Create a new DrugSellingPriceTable object.
        /// <param name="dSPTItemID">Initial value of the DSPTItemID property.</param>
        /// <param name="dSPTModifiedDate">Initial value of the DSPTModifiedDate property.</param>
        /// <param name="unitPrice">Initial value of the UnitPrice property.</param>
        public static DrugSellingPriceTable CreateDrugSellingPriceTable(long dSPTItemID, DateTime dSPTModifiedDate, Decimal unitPrice)
        {
            DrugSellingPriceTable drugSellingPriceTable = new DrugSellingPriceTable();
            drugSellingPriceTable.DSPTItemID = dSPTItemID;
            drugSellingPriceTable.DSPTModifiedDate = dSPTModifiedDate;
            drugSellingPriceTable.UnitPrice = unitPrice;
            return drugSellingPriceTable;
        }
        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long DSPTItemID
        {
            get
            {
                return _DSPTItemID;
            }
            set
            {
                if (_DSPTItemID != value)
                {
                    OnDSPTItemIDChanging(value);
                    ////ReportPropertyChanging("DSPTItemID");
                    _DSPTItemID = value;
                    RaisePropertyChanged("DSPTItemID");
                    OnDSPTItemIDChanged();
                }
            }
        }
        private long _DSPTItemID;
        partial void OnDSPTItemIDChanging(long value);
        partial void OnDSPTItemIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                ////ReportPropertyChanging("DrugID");
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();

        [DataMemberAttribute()]
        public DateTime DSPTModifiedDate
        {
            get
            {
                return _DSPTModifiedDate;
            }
            set
            {
                OnDSPTModifiedDateChanging(value);
                _DSPTModifiedDate = value;
                RaisePropertyChanged("DSPTModifiedDate");
                OnDSPTModifiedDateChanged();
            }
        }
        private DateTime _DSPTModifiedDate;
        partial void OnDSPTModifiedDateChanging(DateTime value);
        partial void OnDSPTModifiedDateChanged();

        [DataMemberAttribute()]
        public Decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                OnUnitPriceChanging(value);
                _UnitPrice = value;
                RaisePropertyChanged("UnitPrice");
                OnUnitPriceChanged();
            }
        }
        private Decimal _UnitPrice;
        partial void OnUnitPriceChanging(Decimal value);
        partial void OnUnitPriceChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Nullable<Boolean> _IsActive;
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }

}
