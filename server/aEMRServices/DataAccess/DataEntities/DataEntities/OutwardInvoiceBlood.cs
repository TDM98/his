using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardInvoiceBlood : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OutwardInvoiceBlood object.

        /// <param name="outwBloodInvoiceID">Initial value of the OutwBloodInvoiceID property.</param>
        /// <param name="outwBloodInvNumber">Initial value of the OutwBloodInvNumber property.</param>
        /// <param name="outwBloodDateTime">Initial value of the OutwBloodDateTime property.</param>
        public static OutwardInvoiceBlood CreateOutwardInvoiceBlood(long outwBloodInvoiceID, String outwBloodInvNumber, DateTime outwBloodDateTime)
        {
            OutwardInvoiceBlood outwardInvoiceBlood = new OutwardInvoiceBlood();
            outwardInvoiceBlood.OutwBloodInvoiceID = outwBloodInvoiceID;
            outwardInvoiceBlood.OutwBloodInvNumber = outwBloodInvNumber;
            outwardInvoiceBlood.OutwBloodDateTime = outwBloodDateTime;
            return outwardInvoiceBlood;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long OutwBloodInvoiceID
        {
            get
            {
                return _OutwBloodInvoiceID;
            }
            set
            {
                if (_OutwBloodInvoiceID != value)
                {
                    OnOutwBloodInvoiceIDChanging(value);
                    ////ReportPropertyChanging("OutwBloodInvoiceID");
                    _OutwBloodInvoiceID = value;
                    RaisePropertyChanged("OutwBloodInvoiceID");
                    OnOutwBloodInvoiceIDChanged();
                }
            }
        }
        private long _OutwBloodInvoiceID;
        partial void OnOutwBloodInvoiceIDChanging(long value);
        partial void OnOutwBloodInvoiceIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                ////ReportPropertyChanging("ServiceRecID");
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();





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
        public String OutwBloodInvNumber
        {
            get
            {
                return _OutwBloodInvNumber;
            }
            set
            {
                OnOutwBloodInvNumberChanging(value);
                ////ReportPropertyChanging("OutwBloodInvNumber");
                _OutwBloodInvNumber = value;
                RaisePropertyChanged("OutwBloodInvNumber");
                OnOutwBloodInvNumberChanged();
            }
        }
        private String _OutwBloodInvNumber;
        partial void OnOutwBloodInvNumberChanging(String value);
        partial void OnOutwBloodInvNumberChanged();





        [DataMemberAttribute()]
        public DateTime OutwBloodDateTime
        {
            get
            {
                return _OutwBloodDateTime;
            }
            set
            {
                OnOutwBloodDateTimeChanging(value);
                ////ReportPropertyChanging("OutwBloodDateTime");
                _OutwBloodDateTime = value;
                RaisePropertyChanged("OutwBloodDateTime");
                OnOutwBloodDateTimeChanged();
            }
        }
        private DateTime _OutwBloodDateTime;
        partial void OnOutwBloodDateTimeChanging(DateTime value);
        partial void OnOutwBloodDateTimeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_HOSFM_IHTRANSA", "IHTransactionType")]
        public IHTransactionType IHTransactionType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_AEREC_INCURRED", "IncurredMedicalExpenses")]
        public IncurredMedicalExpens IncurredMedicalExpens
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_AEREC_MEDICALS", "MedicalSurgicalConsumables")]
        public MedicalSurgicalConsumable MedicalSurgicalConsumable
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDB_REL_BB09_OUTWARDI", "OutwardBlood")]
        public ObservableCollection<OutwardBlood> OutwardBloods
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_BB14_PATIENTS", "PatientServiceRecords")]
        public PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTT_REL_HOSFM_OUTWARDI", "PatientTransactionDetails")]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        #endregion
    }
}
