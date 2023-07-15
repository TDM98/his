using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefImmunization : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefImmunization object.

        /// <param name="iHCode">Initial value of the IHCode property.</param>
        /// <param name="iHVaccine">Initial value of the IHVaccine property.</param>
        public static RefImmunization CreateRefImmunization(long iHCode, String iHVaccine)
        {
            RefImmunization refImmunization = new RefImmunization();
            refImmunization.IHCode = iHCode;
            refImmunization.IHVaccine = iHVaccine;
            return refImmunization;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long IHCode
        {
            get
            {
                return _IHCode;
            }
            set
            {
                if (_IHCode != value)
                {
                    OnIHCodeChanging(value);
                    _IHCode = value;
                    RaisePropertyChanged("IHCode");
                    OnIHCodeChanged();
                }
            }
        }
        private long _IHCode;
        partial void OnIHCodeChanging(long value);
        partial void OnIHCodeChanged();

        [DataMemberAttribute()]
        public String IHVaccine
        {
            get
            {
                return _IHVaccine;
            }
            set
            {
                OnIHVaccineChanging(value);
                _IHVaccine = value;
                RaisePropertyChanged("IHVaccine");
                OnIHVaccineChanged();
            }
        }
        private String _IHVaccine;
        partial void OnIHVaccineChanging(String value);
        partial void OnIHVaccineChanged();

        #endregion

        #region Navigation Properties
        private ObservableCollection<ImmunizationHistory> _ImmunizationHistories;
        public ObservableCollection<ImmunizationHistory> ImmunizationHistories
        {
            get
            {
                return _ImmunizationHistories;
            }
            set
            {
                if (_ImmunizationHistories != value)
                {
                    _ImmunizationHistories = value;
                    RaisePropertyChanged("ImmunizationHistories");
                }
            }
        }

        #endregion
        public override bool Equals(object obj)
        {
            RefImmunization cond = obj as RefImmunization;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.IHCode == cond.IHCode;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                }
            }
        }
        private long _MedServiceID;

        [DataMemberAttribute()]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                if (_MedServiceName != value)
                {
                    _MedServiceName = value;
                    RaisePropertyChanged("MedServiceName");
                }
            }
        }
        private string _MedServiceName;
    }
}
