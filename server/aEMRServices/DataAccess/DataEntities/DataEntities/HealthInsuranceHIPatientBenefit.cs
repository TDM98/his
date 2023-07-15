using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    public partial class HealthInsuranceHIPatientBenefit : EntityBase
    {
        public HealthInsuranceHIPatientBenefit()
            : base()
        {
            
        }
        private long _HIBenefitID;
        [DataMemberAttribute()]
        public long HIBenefitID
        {
            get
            {
                return _HIBenefitID;
            }
            set
            {
                if (_HIBenefitID !=value)
                {
                    _HIBenefitID = value;
                    RaisePropertyChanged("HIBenefitID"); 
                }
            }
        }

        private long _HisID;
        [DataMemberAttribute()]
        public long HisID
        {
            get
            {
                return _HisID;
            }
            set
            {
                if (_HisID != value)
                {
                    _HisID = value;
                    RaisePropertyChanged("HisID");
                }
            }
        }

        private long _StaffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private DateTime _RecCreatedDate;
        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }

        private double _Benefit;
        [DataMemberAttribute()]
        public double Benefit
        {
            get
            {
                return _Benefit;
            }
            set
            {
                if (_Benefit != value)
                {
                    _Benefit = value;
                    RaisePropertyChanged("Benefit");
                }
            }
        }

        private bool _IsActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }
    }
}
