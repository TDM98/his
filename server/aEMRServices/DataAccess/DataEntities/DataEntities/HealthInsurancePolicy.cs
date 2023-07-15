using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HealthInsurancePolicy : NotifyChangedBase, IEditableObject
    {
        public HealthInsurancePolicy()
            : base()
        {

        }

        private HealthInsurancePolicy _tempHealthInsurancePolicy;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHealthInsurancePolicy = (HealthInsurancePolicy)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHealthInsurancePolicy)
                CopyFrom(_tempHealthInsurancePolicy);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HealthInsurancePolicy p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HealthInsurancePolicy object.

        /// <param name="hIPCode">Initial value of the HIPCode property.</param>
        /// <param name="hIPDescription">Initial value of the HIPDescription property.</param>
        public static HealthInsurancePolicy CreateHealthInsurancePolicy(String hIPCode, String hIPDescription)
        {
            HealthInsurancePolicy healthInsurancePolicy = new HealthInsurancePolicy();
            healthInsurancePolicy.HIPCode = hIPCode;
            healthInsurancePolicy.HIPDescription = hIPDescription;
            return healthInsurancePolicy;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public String HIPCode
        {
            get
            {
                return _HIPCode;
            }
            set
            {
                if (_HIPCode != value)
                {
                    OnHIPCodeChanging(value);
                    ////ReportPropertyChanging("HIPCode");
                    _HIPCode = value;
                    RaisePropertyChanged("HIPCode");
                    OnHIPCodeChanged();
                }
            }
        }
        private String _HIPCode;
        partial void OnHIPCodeChanging(String value);
        partial void OnHIPCodeChanged();





        [DataMemberAttribute()]
        public Nullable<Int32> IBID
        {
            get
            {
                return _IBID;
            }
            set
            {
                OnIBIDChanging(value);
                ////ReportPropertyChanging("IBID");
                _IBID = value;
                RaisePropertyChanged("IBID");
                OnIBIDChanged();
            }
        }
        private Nullable<Int32> _IBID;
        partial void OnIBIDChanging(Nullable<Int32> value);
        partial void OnIBIDChanged();





        [DataMemberAttribute()]
        public String HIPDescription
        {
            get
            {
                return _HIPDescription;
            }
            set
            {
                OnHIPDescriptionChanging(value);
                ////ReportPropertyChanging("HIPDescription");
                _HIPDescription = value;
                RaisePropertyChanged("HIPDescription");
                OnHIPDescriptionChanged();
            }
        }
        private String _HIPDescription;
        partial void OnHIPDescriptionChanging(String value);
        partial void OnHIPDescriptionChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                OnIdxChanging(value);
                ////ReportPropertyChanging("Idx");
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HI_REL_PTINF_HIPOLOCY", "HealthInsurance")]
        public ObservableCollection<HealthInsurance> HealthInsurances
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HEALTHIN_REL_PTINF_INSURANC", "InsuranceBenefit")]
        public InsuranceBenefit InsuranceBenefit
        {
            get;
            set;
        }

        #endregion
    }
}
