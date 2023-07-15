using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class IncurredMedicalExpens : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new IncurredMedicalExpens object.

        /// <param name="iMEID">Initial value of the IMEID property.</param>
        /// <param name="iMEReason">Initial value of the IMEReason property.</param>
        /// <param name="iMEDateTime">Initial value of the IMEDateTime property.</param>
        public static IncurredMedicalExpens CreateIncurredMedicalExpens(long iMEID, String iMEReason, DateTime iMEDateTime)
        {
            IncurredMedicalExpens incurredMedicalExpens = new IncurredMedicalExpens();
            incurredMedicalExpens.IMEID = iMEID;
            incurredMedicalExpens.IMEReason = iMEReason;
            incurredMedicalExpens.IMEDateTime = iMEDateTime;
            return incurredMedicalExpens;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long IMEID
        {
            get
            {
                return _IMEID;
            }
            set
            {
                if (_IMEID != value)
                {
                    OnIMEIDChanging(value);
                    ////ReportPropertyChanging("IMEID");
                    _IMEID = value;
                    RaisePropertyChanged("IMEID");
                    OnIMEIDChanged();
                }
            }
        }
        private long _IMEID;
        partial void OnIMEIDChanging(long value);
        partial void OnIMEIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                ////ReportPropertyChanging("PtRegDetailID");
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();





        [DataMemberAttribute()]
        public String IMEReason
        {
            get
            {
                return _IMEReason;
            }
            set
            {
                OnIMEReasonChanging(value);
                ////ReportPropertyChanging("IMEReason");
                _IMEReason = value;
                RaisePropertyChanged("IMEReason");
                OnIMEReasonChanged();
            }
        }
        private String _IMEReason;
        partial void OnIMEReasonChanging(String value);
        partial void OnIMEReasonChanged();





        [DataMemberAttribute()]
        public DateTime IMEDateTime
        {
            get
            {
                return _IMEDateTime;
            }
            set
            {
                OnIMEDateTimeChanging(value);
                ////ReportPropertyChanging("IMEDateTime");
                _IMEDateTime = value;
                RaisePropertyChanged("IMEDateTime");
                OnIMEDateTimeChanged();
            }
        }
        private DateTime _IMEDateTime;
        partial void OnIMEDateTimeChanging(DateTime value);
        partial void OnIMEDateTimeChanged();





        [DataMemberAttribute()]
        public String Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                OnDiagnosisChanging(value);
                ////ReportPropertyChanging("Diagnosis");
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis;
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();





        [DataMemberAttribute()]
        public String OrientedTreatment
        {
            get
            {
                return _OrientedTreatment;
            }
            set
            {
                OnOrientedTreatmentChanging(value);
                ////ReportPropertyChanging("OrientedTreatment");
                _OrientedTreatment = value;
                RaisePropertyChanged("OrientedTreatment");
                OnOrientedTreatmentChanged();
            }
        }
        private String _OrientedTreatment;
        partial void OnOrientedTreatmentChanging(String value);
        partial void OnOrientedTreatmentChanged();





        [DataMemberAttribute()]
        public String Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                ////ReportPropertyChanging("Comment");
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INCURRED_REL_AEREC_PATIENTR", "PatientRegistrationDetails")]
        public PatientRegistrationDetail PatientRegistrationDetail
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_AEREC_INCURRED", "OutwardDMedRscrInvoices")]
        public ObservableCollection<OutwardDMedRscrInvoice> OutwardDMedRscrInvoices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDI_REL_AEREC_INCURRED", "OutwardInvoiceBlood")]
        public ObservableCollection<OutwardInvoiceBlood> OutwardInvoiceBloods
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWD_REL_AEREC_INCUREXP", "OutwardDrugInvoices")]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
    }

}
