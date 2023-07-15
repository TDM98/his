using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HIPremiumPaymentDetail : NotifyChangedBase, IEditableObject
    {
        public HIPremiumPaymentDetail()
            : base()
        {

        }

        private HIPremiumPaymentDetail _tempHIPremiumPaymentDetail;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHIPremiumPaymentDetail = (HIPremiumPaymentDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHIPremiumPaymentDetail)
                CopyFrom(_tempHIPremiumPaymentDetail);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HIPremiumPaymentDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

      
        /// Create a new HIPremiumPaymentDetail object.
        
        /// <param name="hIPmtItemID">Initial value of the HIPmtItemID property.</param>
        public static HIPremiumPaymentDetail CreateHIPremiumPaymentDetail(long hIPmtItemID)
        {
            HIPremiumPaymentDetail hIPremiumPaymentDetail = new HIPremiumPaymentDetail();
            hIPremiumPaymentDetail.HIPmtItemID = hIPmtItemID;
            return hIPremiumPaymentDetail;
        }

        #endregion
        #region Primitive Properties

      
        
        
       
        [DataMemberAttribute()]
        public long HIPmtItemID
        {
            get
            {
                return _HIPmtItemID;
            }
            set
            {
                if (_HIPmtItemID != value)
                {
                    OnHIPmtItemIDChanging(value);
                    ////ReportPropertyChanging("HIPmtItemID");
                    _HIPmtItemID =value;
                    RaisePropertyChanged("HIPmtItemID");
                    OnHIPmtItemIDChanged();
                }
            }
        }
        private long _HIPmtItemID;
        partial void OnHIPmtItemIDChanging(long value);
        partial void OnHIPmtItemIDChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> TransactionID
        {
            get
            {
                return _TransactionID;
            }
            set
            {
                OnTransactionIDChanging(value);
                ////ReportPropertyChanging("TransactionID");
                _TransactionID =value;
                RaisePropertyChanged("TransactionID");
                OnTransactionIDChanged();
            }
        }
        private Nullable<long> _TransactionID;
        partial void OnTransactionIDChanging(Nullable<long> value);
        partial void OnTransactionIDChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> HIPPmtID
        {
            get
            {
                return _HIPPmtID;
            }
            set
            {
                OnHIPPmtIDChanging(value);
                ////ReportPropertyChanging("HIPPmtID");
                _HIPPmtID =value;
                RaisePropertyChanged("HIPPmtID");
                OnHIPPmtIDChanged();
            }
        }
        private Nullable<long> _HIPPmtID;
        partial void OnHIPPmtIDChanging(Nullable<long> value);
        partial void OnHIPPmtIDChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> PCL
        {
            get
            {
                return _PCL;
            }
            set
            {
                OnPCLChanging(value);
                ////ReportPropertyChanging("PCL");
                _PCL =value;
                RaisePropertyChanged("PCL");
                OnPCLChanged();
            }
        }
        private Nullable<long> _PCL;
        partial void OnPCLChanging(Nullable<long> value);
        partial void OnPCLChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> ImagingDiagnosisAndFuncExam
        {
            get
            {
                return _ImagingDiagnosisAndFuncExam;
            }
            set
            {
                OnImagingDiagnosisAndFuncExamChanging(value);
                ////ReportPropertyChanging("ImagingDiagnosisAndFuncExam");
                _ImagingDiagnosisAndFuncExam =value;
                RaisePropertyChanged("ImagingDiagnosisAndFuncExam");
                OnImagingDiagnosisAndFuncExamChanged();
            }
        }
        private Nullable<long> _ImagingDiagnosisAndFuncExam;
        partial void OnImagingDiagnosisAndFuncExamChanging(Nullable<long> value);
        partial void OnImagingDiagnosisAndFuncExamChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> TreatmentMedicine
        {
            get
            {
                return _TreatmentMedicine;
            }
            set
            {
                OnTreatmentMedicineChanging(value);
                ////ReportPropertyChanging("TreatmentMedicine");
                _TreatmentMedicine =value;
                RaisePropertyChanged("TreatmentMedicine");
                OnTreatmentMedicineChanged();
            }
        }
        private Nullable<long> _TreatmentMedicine;
        partial void OnTreatmentMedicineChanging(Nullable<long> value);
        partial void OnTreatmentMedicineChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> Blood
        {
            get
            {
                return _Blood;
            }
            set
            {
                OnBloodChanging(value);
                ////ReportPropertyChanging("Blood");
                _Blood =value;
                RaisePropertyChanged("Blood");
                OnBloodChanged();
            }
        }
        private Nullable<long> _Blood;
        partial void OnBloodChanging(Nullable<long> value);
        partial void OnBloodChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> Surgery
        {
            get
            {
                return _Surgery;
            }
            set
            {
                OnSurgeryChanging(value);
                ////ReportPropertyChanging("Surgery");
                _Surgery =value;
                RaisePropertyChanged("Surgery");
                OnSurgeryChanged();
            }
        }
        private Nullable<long> _Surgery;
        partial void OnSurgeryChanging(Nullable<long> value);
        partial void OnSurgeryChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> ConsumableMedInstruments
        {
            get
            {
                return _ConsumableMedInstruments;
            }
            set
            {
                OnConsumableMedInstrumentsChanging(value);
                ////ReportPropertyChanging("ConsumableMedInstruments");
                _ConsumableMedInstruments =value;
                RaisePropertyChanged("ConsumableMedInstruments");
                OnConsumableMedInstrumentsChanged();
            }
        }
        private Nullable<long> _ConsumableMedInstruments;
        partial void OnConsumableMedInstrumentsChanging(Nullable<long> value);
        partial void OnConsumableMedInstrumentsChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> HighlyTechnicalServices
        {
            get
            {
                return _HighlyTechnicalServices;
            }
            set
            {
                OnHighlyTechnicalServicesChanging(value);
                ////ReportPropertyChanging("HighlyTechnicalServices");
                _HighlyTechnicalServices =value;
                RaisePropertyChanged("HighlyTechnicalServices");
                OnHighlyTechnicalServicesChanged();
            }
        }
        private Nullable<long> _HighlyTechnicalServices;
        partial void OnHighlyTechnicalServicesChanging(Nullable<long> value);
        partial void OnHighlyTechnicalServicesChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> Consultation
        {
            get
            {
                return _Consultation;
            }
            set
            {
                OnConsultationChanging(value);
                ////ReportPropertyChanging("Consultation");
                _Consultation =value;
                RaisePropertyChanged("Consultation");
                OnConsultationChanged();
            }
        }
        private Nullable<long> _Consultation;
        partial void OnConsultationChanging(Nullable<long> value);
        partial void OnConsultationChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> BedFee
        {
            get
            {
                return _BedFee;
            }
            set
            {
                OnBedFeeChanging(value);
                ////ReportPropertyChanging("BedFee");
                _BedFee =value;
                RaisePropertyChanged("BedFee");
                OnBedFeeChanged();
            }
        }
        private Nullable<long> _BedFee;
        partial void OnBedFeeChanging(Nullable<long> value);
        partial void OnBedFeeChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<long> AntiRejection
        {
            get
            {
                return _AntiRejection;
            }
            set
            {
                OnAntiRejectionChanging(value);
                ////ReportPropertyChanging("AntiRejection");
                _AntiRejection =value;
                RaisePropertyChanged("AntiRejection");
                OnAntiRejectionChanged();
            }
        }
        private Nullable<long> _AntiRejection;
        partial void OnAntiRejectionChanging(Nullable<long> value);
        partial void OnAntiRejectionChanged();

      
        
        
       
        [DataMemberAttribute()]
        public Nullable<Decimal> TransportationCost
        {
            get
            {
                return _TransportationCost;
            }
            set
            {
                OnTransportationCostChanging(value);
                ////ReportPropertyChanging("TransportationCost");
                _TransportationCost =value;
                RaisePropertyChanged("TransportationCost");
                OnTransportationCostChanged();
            }
        }
        private Nullable<Decimal> _TransportationCost;
        partial void OnTransportationCostChanging(Nullable<Decimal> value);
        partial void OnTransportationCostChanged();

        #endregion

        #region Navigation Properties

      
        
        
       
       
        [DataMemberAttribute()]
       // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPREMIU_REL_HOSFM_HIPREMIU", "HIPremiumPayments")]
        public HIPremiumPayment HIPremiumPayment
        {
            get;
            set;
        }
      
        [DataMemberAttribute()]
       // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPREMIU_REL_HOSFM_PATIENTT", "PatientTransaction")]
        public PatientTransaction PatientTransaction
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
