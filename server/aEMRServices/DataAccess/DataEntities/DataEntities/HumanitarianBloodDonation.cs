using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HumanitarianBloodDonation : NotifyChangedBase, IEditableObject
    {
        public HumanitarianBloodDonation()
            : base()
        {

        }

        private HumanitarianBloodDonation _tempHumanitarianBloodDonation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHumanitarianBloodDonation = (HumanitarianBloodDonation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHumanitarianBloodDonation)
                CopyFrom(_tempHumanitarianBloodDonation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HumanitarianBloodDonation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HumanitarianBloodDonation object.

        /// <param name="hBDID">Initial value of the HBDID property.</param>
        public static HumanitarianBloodDonation CreateHumanitarianBloodDonation(long hBDID)
        {
            HumanitarianBloodDonation humanitarianBloodDonation = new HumanitarianBloodDonation();
            humanitarianBloodDonation.HBDID = hBDID;
            return humanitarianBloodDonation;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HBDID
        {
            get
            {
                return _HBDID;
            }
            set
            {
                if (_HBDID != value)
                {
                    OnHBDIDChanging(value);
                    ////ReportPropertyChanging("HBDID");
                    _HBDID = value;
                    RaisePropertyChanged("HBDID");
                    OnHBDIDChanged();
                }
            }
        }
        private long _HBDID;
        partial void OnHBDIDChanging(long value);
        partial void OnHBDIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> HBDDateTime
        {
            get
            {
                return _HBDDateTime;
            }
            set
            {
                OnHBDDateTimeChanging(value);
                ////ReportPropertyChanging("HBDDateTime");
                _HBDDateTime = value;
                RaisePropertyChanged("HBDDateTime");
                OnHBDDateTimeChanged();
            }
        }
        private Nullable<DateTime> _HBDDateTime;
        partial void OnHBDDateTimeChanging(Nullable<DateTime> value);
        partial void OnHBDDateTimeChanged();





        [DataMemberAttribute()]
        public String Purpose
        {
            get
            {
                return _Purpose;
            }
            set
            {
                OnPurposeChanging(value);
                ////ReportPropertyChanging("Purpose");
                _Purpose = value;
                RaisePropertyChanged("Purpose");
                OnPurposeChanged();
            }
        }
        private String _Purpose;
        partial void OnPurposeChanging(String value);
        partial void OnPurposeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_BLOODDON_REL_BB03_HUMANITA", "BloodDonations")]
        public ObservableCollection<BloodDonation> BloodDonations
        {
            get;
            set;
        }

        #endregion
    }
}
