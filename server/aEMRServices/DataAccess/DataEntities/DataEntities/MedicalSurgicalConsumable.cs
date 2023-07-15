using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalSurgicalConsumable : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedicalSurgicalConsumable object.

        /// <param name="mSCID">Initial value of the MSCID property.</param>
        /// <param name="mSCDateTime">Initial value of the MSCDateTime property.</param>
        public static MedicalSurgicalConsumable CreateMedicalSurgicalConsumable(long mSCID, DateTime mSCDateTime)
        {
            MedicalSurgicalConsumable medicalSurgicalConsumable = new MedicalSurgicalConsumable();
            medicalSurgicalConsumable.MSCID = mSCID;
            medicalSurgicalConsumable.MSCDateTime = mSCDateTime;
            return medicalSurgicalConsumable;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MSCID
        {
            get
            {
                return _MSCID;
            }
            set
            {
                if (_MSCID != value)
                {
                    OnMSCIDChanging(value);
                    ////ReportPropertyChanging("MSCID");
                    _MSCID = value;
                    RaisePropertyChanged("MSCID");
                    OnMSCIDChanged();
                }
            }
        }
        private long _MSCID;
        partial void OnMSCIDChanging(long value);
        partial void OnMSCIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> SurgeryID
        {
            get
            {
                return _SurgeryID;
            }
            set
            {
                OnSurgeryIDChanging(value);
                ////ReportPropertyChanging("SurgeryID");
                _SurgeryID = value;
                RaisePropertyChanged("SurgeryID");
                OnSurgeryIDChanged();
            }
        }
        private Nullable<long> _SurgeryID;
        partial void OnSurgeryIDChanging(Nullable<long> value);
        partial void OnSurgeryIDChanged();





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
        public DateTime MSCDateTime
        {
            get
            {
                return _MSCDateTime;
            }
            set
            {
                OnMSCDateTimeChanging(value);
                ////ReportPropertyChanging("MSCDateTime");
                _MSCDateTime = value;
                RaisePropertyChanged("MSCDateTime");
                OnMSCDateTimeChanged();
            }
        }
        private DateTime _MSCDateTime;
        partial void OnMSCDateTimeChanging(DateTime value);
        partial void OnMSCDateTimeChanged();





        [DataMemberAttribute()]
        public String MSCReason
        {
            get
            {
                return _MSCReason;
            }
            set
            {
                OnMSCReasonChanging(value);
                ////ReportPropertyChanging("MSCReason");
                _MSCReason = value;
                RaisePropertyChanged("MSCReason");
                OnMSCReasonChanged();
            }
        }
        private String _MSCReason;
        partial void OnMSCReasonChanging(String value);
        partial void OnMSCReasonChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALS_REL_PCMD2_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALS_REL_PCMD2_SURGERIE", "Surgeries")]
        public Surgery Surgery
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_AEREC_MEDICALS", "OutwardDMedRscrInvoices")]
        public ObservableCollection<OutwardDMedRscrInvoice> OutwardDMedRscrInvoices
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_AEREC_MEDICALS", "OutwardInvoiceBlood")]
        public ObservableCollection<OutwardInvoiceBlood> OutwardInvoiceBloods
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWD_REL_AEREC_MEDSUR", "OutwardDrugInvoices")]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get;
            set;
        }

        #endregion
    }
}
