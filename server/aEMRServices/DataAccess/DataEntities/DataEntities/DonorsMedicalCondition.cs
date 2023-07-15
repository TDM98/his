using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DonorsMedicalCondition : NotifyChangedBase, IEditableObject
    {
        public DonorsMedicalCondition()
            : base()
        {

        }

        private DonorsMedicalCondition _tempDonorsMedicalCondition;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDonorsMedicalCondition = (DonorsMedicalCondition)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDonorsMedicalCondition)
                CopyFrom(_tempDonorsMedicalCondition);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DonorsMedicalCondition p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new DonorsMedicalCondition object.

        /// <param name="dorMedCondID">Initial value of the DorMedCondID property.</param>
        /// <param name="medHistCode">Initial value of the MedHistCode property.</param>
        /// <param name="donorID">Initial value of the DonorID property.</param>
        public static DonorsMedicalCondition CreateDonorsMedicalCondition(long dorMedCondID, long medHistCode, long donorID)
        {
            DonorsMedicalCondition donorsMedicalCondition = new DonorsMedicalCondition();
            donorsMedicalCondition.DorMedCondID = dorMedCondID;
            donorsMedicalCondition.MedHistCode = medHistCode;
            donorsMedicalCondition.DonorID = donorID;
            return donorsMedicalCondition;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DorMedCondID
        {
            get
            {
                return _DorMedCondID;
            }
            set
            {
                if (_DorMedCondID != value)
                {
                    OnDorMedCondIDChanging(value);
                    ////ReportPropertyChanging("DorMedCondID");
                    _DorMedCondID = value;
                    RaisePropertyChanged("DorMedCondID");
                    OnDorMedCondIDChanged();
                }
            }
        }
        private long _DorMedCondID;
        partial void OnDorMedCondIDChanging(long value);
        partial void OnDorMedCondIDChanged();





        [DataMemberAttribute()]
        public long MedHistCode
        {
            get
            {
                return _MedHistCode;
            }
            set
            {
                OnMedHistCodeChanging(value);
                ////ReportPropertyChanging("MedHistCode");
                _MedHistCode = value;
                RaisePropertyChanged("MedHistCode");
                OnMedHistCodeChanged();
            }
        }
        private long _MedHistCode;
        partial void OnMedHistCodeChanging(long value);
        partial void OnMedHistCodeChanged();





        [DataMemberAttribute()]
        public long DonorID
        {
            get
            {
                return _DonorID;
            }
            set
            {
                OnDonorIDChanging(value);
                ////ReportPropertyChanging("DonorID");
                _DonorID = value;
                RaisePropertyChanged("DonorID");
                OnDonorIDChanged();
            }
        }
        private long _DonorID;
        partial void OnDonorIDChanging(long value);
        partial void OnDonorIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_Seriousness
        {
            get
            {
                return _V_Seriousness;
            }
            set
            {
                OnV_SeriousnessChanging(value);
                ////ReportPropertyChanging("V_Seriousness");
                _V_Seriousness = value;
                RaisePropertyChanged("V_Seriousness");
                OnV_SeriousnessChanged();
            }
        }
        private Nullable<Int64> _V_Seriousness;
        partial void OnV_SeriousnessChanging(Nullable<Int64> value);
        partial void OnV_SeriousnessChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORSME_REL_BB11_DONORS", "Donors")]
        public Donor Donor
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORSME_REL_BB10_REFMEDIC", "RefMedicalHistory")]
        public RefMedicalHistory RefMedicalHistory
        {
            get;
            set;
        }

        #endregion
    }
}
