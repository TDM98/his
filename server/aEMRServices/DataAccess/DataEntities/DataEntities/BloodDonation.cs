using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class BloodDonation : NotifyChangedBase, IEditableObject
    {
        public BloodDonation()
            : base()
        {

        }

        private BloodDonation _tempBloodDonation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBloodDonation = (BloodDonation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBloodDonation)
                CopyFrom(_tempBloodDonation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BloodDonation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BloodDonation object.

        /// <param name="bDID">Initial value of the BDID property.</param>
        /// <param name="bDNumber">Initial value of the BDNumber property.</param>
        /// <param name="bDDate">Initial value of the BDDate property.</param>
        /// <param name="donatedBloodUnits">Initial value of the DonatedBloodUnits property.</param>
        public static BloodDonation CreateBloodDonation(long bDID, String bDNumber, DateTime bDDate, Double donatedBloodUnits)
        {
            BloodDonation bloodDonation = new BloodDonation();
            bloodDonation.BDID = bDID;
            bloodDonation.BDNumber = bDNumber;
            bloodDonation.BDDate = bDDate;
            bloodDonation.DonatedBloodUnits = donatedBloodUnits;
            return bloodDonation;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long BDID
        {
            get
            {
                return _BDID;
            }
            set
            {
                if (_BDID != value)
                {
                    OnBDIDChanging(value);
                    _BDID = value;
                    RaisePropertyChanged("BDID");
                    OnBDIDChanged();
                }
            }
        }
        private long _BDID;
        partial void OnBDIDChanging(long value);
        partial void OnBDIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DonorID
        {
            get
            {
                return _DonorID;
            }
            set
            {
                OnDonorIDChanging(value);
                _DonorID = value;
                RaisePropertyChanged("DonorID");
                OnDonorIDChanged();
            }
        }
        private Nullable<long> _DonorID;
        partial void OnDonorIDChanging(Nullable<long> value);
        partial void OnDonorIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]
        public String TransEventID
        {
            get
            {
                return _TransEventID;
            }
            set
            {
                OnTransEventIDChanging(value);
                _TransEventID = value;
                RaisePropertyChanged("TransEventID");
                OnTransEventIDChanged();
            }
        }
        private String _TransEventID;
        partial void OnTransEventIDChanging(String value);
        partial void OnTransEventIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> HBDID
        {
            get
            {
                return _HBDID;
            }
            set
            {
                OnHBDIDChanging(value);
                _HBDID = value;
                RaisePropertyChanged("HBDID");
                OnHBDIDChanged();
            }
        }
        private Nullable<long> _HBDID;
        partial void OnHBDIDChanging(Nullable<long> value);
        partial void OnHBDIDChanged();

        [DataMemberAttribute()]
        public String BDNumber
        {
            get
            {
                return _BDNumber;
            }
            set
            {
                OnBDNumberChanging(value);
                _BDNumber = value;
                RaisePropertyChanged("BDNumber");
                OnBDNumberChanged();
            }
        }
        private String _BDNumber;
        partial void OnBDNumberChanging(String value);
        partial void OnBDNumberChanged();

        [DataMemberAttribute()]
        public DateTime BDDate
        {
            get
            {
                return _BDDate;
            }
            set
            {
                OnBDDateChanging(value);
                _BDDate = value;
                RaisePropertyChanged("BDDate");
                OnBDDateChanged();
            }
        }
        private DateTime _BDDate;
        partial void OnBDDateChanging(DateTime value);
        partial void OnBDDateChanged();

        [DataMemberAttribute()]
        public String OtherDeatails
        {
            get
            {
                return _OtherDeatails;
            }
            set
            {
                OnOtherDeatailsChanging(value);
                _OtherDeatails = value;
                RaisePropertyChanged("OtherDeatails");
                OnOtherDeatailsChanged();
            }
        }
        private String _OtherDeatails;
        partial void OnOtherDeatailsChanging(String value);
        partial void OnOtherDeatailsChanged();


        [DataMemberAttribute()]
        public String Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                OnCommentsChanging(value);
                _Comments = value;
                RaisePropertyChanged("Comments");
                OnCommentsChanged();
            }
        }
        private String _Comments;
        partial void OnCommentsChanging(String value);
        partial void OnCommentsChanged();

        [DataMemberAttribute()]
        public Double DonatedBloodUnits
        {
            get
            {
                return _DonatedBloodUnits;
            }
            set
            {
                OnDonatedBloodUnitsChanging(value);
                _DonatedBloodUnits = value;
                RaisePropertyChanged("DonatedBloodUnits");
                OnDonatedBloodUnitsChanged();
            }
        }
        private Double _DonatedBloodUnits;
        partial void OnDonatedBloodUnitsChanging(Double value);
        partial void OnDonatedBloodUnitsChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsSellingBlood
        {
            get
            {
                return _IsSellingBlood;
            }
            set
            {
                OnIsSellingBloodChanging(value);
                _IsSellingBlood = value;
                RaisePropertyChanged("IsSellingBlood");
                OnIsSellingBloodChanged();
            }
        }
        private Nullable<Boolean> _IsSellingBlood;
        partial void OnIsSellingBloodChanging(Nullable<Boolean> value);
        partial void OnIsSellingBloodChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public TransactionEvent TransactionEvent
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public HumanitarianBloodDonation HumanitarianBloodDonation
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public Donor Donor
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardBlood> InwardBloods
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public ObservableCollection<PatientTransaction> PatientTransactions
        {
            get;
            set;
        }

        #endregion
    }
}
