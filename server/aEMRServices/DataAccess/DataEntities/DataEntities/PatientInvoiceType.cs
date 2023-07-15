using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientInvoiceType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientInvoiceType object.

        /// <param name="ptInvoiceTypeID">Initial value of the PtInvoiceTypeID property.</param>
        /// <param name="ptInvoiceTypeName">Initial value of the PtInvoiceTypeName property.</param>
        public static PatientInvoiceType CreatePatientInvoiceType(long ptInvoiceTypeID, String ptInvoiceTypeName)
        {
            PatientInvoiceType patientInvoiceType = new PatientInvoiceType();
            patientInvoiceType.PtInvoiceTypeID = ptInvoiceTypeID;
            patientInvoiceType.PtInvoiceTypeName = ptInvoiceTypeName;
            return patientInvoiceType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PtInvoiceTypeID
        {
            get
            {
                return _PtInvoiceTypeID;
            }
            set
            {
                if (_PtInvoiceTypeID != value)
                {
                    OnPtInvoiceTypeIDChanging(value);
                    ////ReportPropertyChanging("PtInvoiceTypeID");
                    _PtInvoiceTypeID = value;
                    RaisePropertyChanged("PtInvoiceTypeID");
                    OnPtInvoiceTypeIDChanged();
                }
            }
        }
        private long _PtInvoiceTypeID;
        partial void OnPtInvoiceTypeIDChanging(long value);
        partial void OnPtInvoiceTypeIDChanged();





        [DataMemberAttribute()]
        public String PtInvoiceTypeName
        {
            get
            {
                return _PtInvoiceTypeName;
            }
            set
            {
                OnPtInvoiceTypeNameChanging(value);
                ////ReportPropertyChanging("PtInvoiceTypeName");
                _PtInvoiceTypeName = value;
                RaisePropertyChanged("PtInvoiceTypeName");
                OnPtInvoiceTypeNameChanged();
            }
        }
        private String _PtInvoiceTypeName;
        partial void OnPtInvoiceTypeNameChanging(String value);
        partial void OnPtInvoiceTypeNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTI_REL_HOSFM_PATIENTI", "PatientInvoices")]
        public ObservableCollection<PatientInvoice> PatientInvoices
        {
            get;
            set;
        }

        #endregion
    }
}
