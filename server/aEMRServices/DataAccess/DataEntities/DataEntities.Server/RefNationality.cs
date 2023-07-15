using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;


namespace DataEntities
{
    public partial class RefNationality: NotifyChangedBase
    {
        #region Factory Method
        public static RefNationality CreateRefNationality(long NationalityID, String NationalityName, String NationalityCode)
        {
            RefNationality RefNationality = new RefNationality();
            RefNationality.NationalityID = NationalityID;
            RefNationality.NationalityName = NationalityName;
            RefNationality.NationalityCode = NationalityCode;
            return RefNationality;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long NationalityID
        {
            get
            {
                return _NationalityID;
            }
            set
            {
                if (_NationalityID != value)
                {
                    OnNationalityIDChanging(value);
                    _NationalityID = value;
                    RaisePropertyChanged("NationalityID");
                    OnNationalityIDChanged();
                }
            }
        }
        private long _NationalityID;
        partial void OnNationalityIDChanging(long value);
        partial void OnNationalityIDChanged();

        [DataMemberAttribute()]
        public String NationalityName
        {
            get
            {
                return _NationalityName;
            }
            set
            {
                OnNationalityNameChanging(value);
                _NationalityName = value;
                RaisePropertyChanged("NationalityName");
                OnNationalityNameChanged();
            }
        }
        private String _NationalityName;
        partial void OnNationalityNameChanging(String value);
        partial void OnNationalityNameChanged();

        [DataMemberAttribute()]
        public String NationalityCode
        {
            get
            {
                return _NationalityCode;
            }
            set
            {
                OnNationalityCodeChanging(value);
                _NationalityCode = value;
                RaisePropertyChanged("NationalityCode");
                OnNationalityCodeChanged();
            }
        }
        private String _NationalityCode;
        partial void OnNationalityCodeChanging(String value);
        partial void OnNationalityCodeChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<Donor> Donors
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Patient> Patients
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            RefNationality seletedNationality = obj as RefNationality;
            if (seletedNationality == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.NationalityID == seletedNationality.NationalityID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
