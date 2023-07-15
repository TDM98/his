using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardDMedRscrInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OutwardDMedRscrInvoice object.

        /// <param name="outDMedRscrID">Initial value of the OutDMedRscrID property.</param>
        /// <param name="outDMedRscrNumber">Initial value of the OutDMedRscrNumber property.</param>
        /// <param name="dateOutDMedRscr">Initial value of the DateOutDMedRscr property.</param>
        public static OutwardDMedRscrInvoice CreateOutwardDMedRscrInvoice(long outDMedRscrID, String outDMedRscrNumber, DateTime dateOutDMedRscr)
        {
            OutwardDMedRscrInvoice outwardDMedRscrInvoice = new OutwardDMedRscrInvoice();
            outwardDMedRscrInvoice.OutDMedRscrID = outDMedRscrID;
            outwardDMedRscrInvoice.OutDMedRscrNumber = outDMedRscrNumber;
            outwardDMedRscrInvoice.DateOutDMedRscr = dateOutDMedRscr;
            return outwardDMedRscrInvoice;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long OutDMedRscrID
        {
            get
            {
                return _OutDMedRscrID;
            }
            set
            {
                if (_OutDMedRscrID != value)
                {
                    OnOutDMedRscrIDChanging(value);
                    ////ReportPropertyChanging("OutDMedRscrID");
                    _OutDMedRscrID = value;
                    RaisePropertyChanged("OutDMedRscrID");
                    OnOutDMedRscrIDChanged();
                }
            }
        }
        private long _OutDMedRscrID;
        partial void OnOutDMedRscrIDChanging(long value);
        partial void OnOutDMedRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                OnHITTypeIDChanging(value);
                ////ReportPropertyChanging("HITTypeID");
                _HITTypeID = value;
                RaisePropertyChanged("HITTypeID");
                OnHITTypeIDChanged();
            }
        }
        private Nullable<long> _HITTypeID;
        partial void OnHITTypeIDChanging(Nullable<long> value);
        partial void OnHITTypeIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> MSCID
        {
            get
            {
                return _MSCID;
            }
            set
            {
                OnMSCIDChanging(value);
                ////ReportPropertyChanging("MSCID");
                _MSCID = value;
                RaisePropertyChanged("MSCID");
                OnMSCIDChanged();
            }
        }
        private Nullable<long> _MSCID;
        partial void OnMSCIDChanging(Nullable<long> value);
        partial void OnMSCIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                ////ReportPropertyChanging("StoreID");
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Nullable<long> _StoreID;
        partial void OnStoreIDChanging(Nullable<long> value);
        partial void OnStoreIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> IMEID
        {
            get
            {
                return _IMEID;
            }
            set
            {
                OnIMEIDChanging(value);
                ////ReportPropertyChanging("IMEID");
                _IMEID = value;
                RaisePropertyChanged("IMEID");
                OnIMEIDChanged();
            }
        }
        private Nullable<long> _IMEID;
        partial void OnIMEIDChanging(Nullable<long> value);
        partial void OnIMEIDChanged();





        [DataMemberAttribute()]
        public String OutDMedRscrNumber
        {
            get
            {
                return _OutDMedRscrNumber;
            }
            set
            {
                OnOutDMedRscrNumberChanging(value);
                ////ReportPropertyChanging("OutDMedRscrNumber");
                _OutDMedRscrNumber = value;
                RaisePropertyChanged("OutDMedRscrNumber");
                OnOutDMedRscrNumberChanged();
            }
        }
        private String _OutDMedRscrNumber;
        partial void OnOutDMedRscrNumberChanging(String value);
        partial void OnOutDMedRscrNumberChanged();





        [DataMemberAttribute()]
        public DateTime DateOutDMedRscr
        {
            get
            {
                return _DateOutDMedRscr;
            }
            set
            {
                OnDateOutDMedRscrChanging(value);
                ////ReportPropertyChanging("DateOutDMedRscr");
                _DateOutDMedRscr = value;
                RaisePropertyChanged("DateOutDMedRscr");
                OnDateOutDMedRscrChanged();
            }
        }
        private DateTime _DateOutDMedRscr;
        partial void OnDateOutDMedRscrChanging(DateTime value);
        partial void OnDateOutDMedRscrChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_Reason
        {
            get
            {
                return _V_Reason;
            }
            set
            {
                OnV_ReasonChanging(value);
                ////ReportPropertyChanging("V_Reason");
                _V_Reason = value;
                RaisePropertyChanged("V_Reason");
                OnV_ReasonChanged();
            }
        }
        private Nullable<Int64> _V_Reason;
        partial void OnV_ReasonChanging(Nullable<Int64> value);
        partial void OnV_ReasonChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_HOSFM_IHTRANSA", "IHTransactionType")]
        public IHTransactionType IHTransactionType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_AEREC_INCURRED", "IncurredMedicalExpenses")]
        public IncurredMedicalExpens IncurredMedicalExpens
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_AEREC_MEDICALS", "MedicalSurgicalConsumables")]
        public MedicalSurgicalConsumable MedicalSurgicalConsumable
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_DMEDR_OUTWARDD", "OutwardDMedRscrs")]
        public ObservableCollection<OutwardDMedRscr> OutwardDMedRscrs
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_DMEDR_REFSTORA", "RefStorageWarehouseLocation")]
        public RefStorageWarehouseLocation RefStorageWarehouseLocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTT_REL_HOSFM_OUTWARDD", "PatientTransactionDetails")]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        #endregion
    }
}
