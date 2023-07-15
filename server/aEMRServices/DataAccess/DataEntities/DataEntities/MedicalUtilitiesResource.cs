using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalUtilitiesResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedicalUtilitiesResource object.

        /// <param name="rscrID">Initial value of the RscrID property.</param>
        public static MedicalUtilitiesResource CreateMedicalUtilitiesResource(Int64 rscrID)
        {
            MedicalUtilitiesResource medicalUtilitiesResource = new MedicalUtilitiesResource();
            medicalUtilitiesResource.RscrID = rscrID;
            return medicalUtilitiesResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID != value)
                {
                    OnRscrIDChanging(value);
                    ////ReportPropertyChanging("RscrID");
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Double> SurgicalInstrumnetSize
        {
            get
            {
                return _SurgicalInstrumnetSize;
            }
            set
            {
                OnSurgicalInstrumnetSizeChanging(value);
                ////ReportPropertyChanging("SurgicalInstrumnetSize");
                _SurgicalInstrumnetSize = value;
                RaisePropertyChanged("SurgicalInstrumnetSize");
                OnSurgicalInstrumnetSizeChanged();
            }
        }
        private Nullable<Double> _SurgicalInstrumnetSize;
        partial void OnSurgicalInstrumnetSizeChanging(Nullable<Double> value);
        partial void OnSurgicalInstrumnetSizeChanged();





        [DataMemberAttribute()]
        public Nullable<Single> ProcessPeriod
        {
            get
            {
                return _ProcessPeriod;
            }
            set
            {
                OnProcessPeriodChanging(value);
                ////ReportPropertyChanging("ProcessPeriod");
                _ProcessPeriod = value;
                RaisePropertyChanged("ProcessPeriod");
                OnProcessPeriodChanged();
            }
        }
        private Nullable<Single> _ProcessPeriod;
        partial void OnProcessPeriodChanging(Nullable<Single> value);
        partial void OnProcessPeriodChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_TimeUnit
        {
            get
            {
                return _V_TimeUnit;
            }
            set
            {
                OnV_TimeUnitChanging(value);
                ////ReportPropertyChanging("V_TimeUnit");
                _V_TimeUnit = value;
                RaisePropertyChanged("V_TimeUnit");
                OnV_TimeUnitChanged();
            }
        }
        private Nullable<Int64> _V_TimeUnit;
        partial void OnV_TimeUnitChanging(Nullable<Int64> value);
        partial void OnV_TimeUnitChanged();





        [DataMemberAttribute()]
        public Nullable<Double> WarrantyTime
        {
            get
            {
                return _WarrantyTime;
            }
            set
            {
                OnWarrantyTimeChanging(value);
                ////ReportPropertyChanging("WarrantyTime");
                _WarrantyTime = value;
                RaisePropertyChanged("WarrantyTime");
                OnWarrantyTimeChanged();
            }
        }
        private Nullable<Double> _WarrantyTime;
        partial void OnWarrantyTimeChanging(Nullable<Double> value);
        partial void OnWarrantyTimeChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsLocatable
        {
            get
            {
                return _IsLocatable;
            }
            set
            {
                OnIsLocatableChanging(value);
                ////ReportPropertyChanging("IsLocatable");
                _IsLocatable = value;
                RaisePropertyChanged("IsLocatable");
                OnIsLocatableChanged();
            }
        }
        private Nullable<Boolean> _IsLocatable;
        partial void OnIsLocatableChanging(Nullable<Boolean> value);
        partial void OnIsLocatableChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALU_REL_INHER_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        #endregion
    }
}
