using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DMedRscrSellingPriceTable : NotifyChangedBase, IEditableObject
    {
        public DMedRscrSellingPriceTable()
            : base()
        {

        }

        private DMedRscrSellingPriceTable _tempDMedRscrSellingPriceTable;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDMedRscrSellingPriceTable = (DMedRscrSellingPriceTable)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDMedRscrSellingPriceTable)
                CopyFrom(_tempDMedRscrSellingPriceTable);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DMedRscrSellingPriceTable p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DMedRscrSellingPriceTable object.

        /// <param name="dMRTblPriceID">Initial value of the DMRTblPriceID property.</param>
        /// <param name="dMedRscrID">Initial value of the DMedRscrID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        public static DMedRscrSellingPriceTable CreateDMedRscrSellingPriceTable(long dMRTblPriceID, long dMedRscrID, Int64 staffID)
        {
            DMedRscrSellingPriceTable dMedRscrSellingPriceTable = new DMedRscrSellingPriceTable();
            dMedRscrSellingPriceTable.DMRTblPriceID = dMRTblPriceID;
            dMedRscrSellingPriceTable.DMedRscrID = dMedRscrID;
            dMedRscrSellingPriceTable.StaffID = staffID;
            return dMedRscrSellingPriceTable;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DMRTblPriceID
        {
            get
            {
                return _DMRTblPriceID;
            }
            set
            {
                if (_DMRTblPriceID != value)
                {
                    OnDMRTblPriceIDChanging(value);
                    ////ReportPropertyChanging("DMRTblPriceID");
                    _DMRTblPriceID = value;
                    RaisePropertyChanged("DMRTblPriceID");
                    OnDMRTblPriceIDChanged();
                }
            }
        }
        private long _DMRTblPriceID;
        partial void OnDMRTblPriceIDChanging(long value);
        partial void OnDMRTblPriceIDChanged();





        [DataMemberAttribute()]
        public long DMedRscrID
        {
            get
            {
                return _DMedRscrID;
            }
            set
            {
                OnDMedRscrIDChanging(value);
                ////ReportPropertyChanging("DMedRscrID");
                _DMedRscrID = value;
                RaisePropertyChanged("DMedRscrID");
                OnDMedRscrIDChanged();
            }
        }
        private long _DMedRscrID;
        partial void OnDMedRscrIDChanging(long value);
        partial void OnDMedRscrIDChanged();





        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> DMRModifiedDate
        {
            get
            {
                return _DMRModifiedDate;
            }
            set
            {
                OnDMRModifiedDateChanging(value);
                ////ReportPropertyChanging("DMRModifiedDate");
                _DMRModifiedDate = value;
                RaisePropertyChanged("DMRModifiedDate");
                OnDMRModifiedDateChanged();
            }
        }
        private Nullable<DateTime> _DMRModifiedDate;
        partial void OnDMRModifiedDateChanging(Nullable<DateTime> value);
        partial void OnDMRModifiedDateChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> ContainerPrice
        {
            get
            {
                return _ContainerPrice;
            }
            set
            {
                OnContainerPriceChanging(value);
                ////ReportPropertyChanging("ContainerPrice");
                _ContainerPrice = value;
                RaisePropertyChanged("ContainerPrice");
                OnContainerPriceChanged();
            }
        }
        private Nullable<Decimal> _ContainerPrice;
        partial void OnContainerPriceChanging(Nullable<Decimal> value);
        partial void OnContainerPriceChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> PiecePrice
        {
            get
            {
                return _PiecePrice;
            }
            set
            {
                OnPiecePriceChanging(value);
                ////ReportPropertyChanging("PiecePrice");
                _PiecePrice = value;
                RaisePropertyChanged("PiecePrice");
                OnPiecePriceChanged();
            }
        }
        private Nullable<Decimal> _PiecePrice;
        partial void OnPiecePriceChanging(Nullable<Decimal> value);
        partial void OnPiecePriceChanged();





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
                ////ReportPropertyChanging("IsActive");
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
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DMEDRSCR_REL_DMEDR_REFDISPO", "RefDisposableMedicalResources")]
        public RefDisposableMedicalResource RefDisposableMedicalResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DMEDRSCR_REL_DMEDR_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }


        #endregion
    }
}
