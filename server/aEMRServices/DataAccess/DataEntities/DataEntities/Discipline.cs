using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Discipline : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Discipline object.

        /// <param name="disID">Initial value of the DisID property.</param>
        public static Discipline CreateDiscipline(long disID)
        {
            Discipline discipline = new Discipline();
            discipline.DisID = disID;
            return discipline;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DisID
        {
            get
            {
                return _DisID;
            }
            set
            {
                if (_DisID != value)
                {
                    OnDisIDChanging(value);
                    _DisID = value;
                    RaisePropertyChanged("DisID");
                    OnDisIDChanged();
                }
            }
        }
        private long _DisID;
        partial void OnDisIDChanging(long value);
        partial void OnDisIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public String Seriousness
        {
            get
            {
                return _Seriousness;
            }
            set
            {
                OnSeriousnessChanging(value);
                _Seriousness = value;
                RaisePropertyChanged("Seriousness");
                OnSeriousnessChanged();
            }
        }
        private String _Seriousness;
        partial void OnSeriousnessChanging(String value);
        partial void OnSeriousnessChanged();

        [DataMemberAttribute()]
        public String DisType
        {
            get
            {
                return _DisType;
            }
            set
            {
                OnDisTypeChanging(value);
                _DisType = value;
                RaisePropertyChanged("DisType");
                OnDisTypeChanged();
            }
        }
        private String _DisType;
        partial void OnDisTypeChanging(String value);
        partial void OnDisTypeChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> DisDate
        {
            get
            {
                return _DisDate;
            }
            set
            {
                OnDisDateChanging(value);
                _DisDate = value;
                RaisePropertyChanged("DisDate");
                OnDisDateChanged();
            }
        }
        private Nullable<DateTime> _DisDate;
        partial void OnDisDateChanging(Nullable<DateTime> value);
        partial void OnDisDateChanged();

        [DataMemberAttribute()]
        public String DisNotes
        {
            get
            {
                return _DisNotes;
            }
            set
            {
                OnDisNotesChanging(value);
                _DisNotes = value;
                RaisePropertyChanged("DisNotes");
                OnDisNotesChanged();
            }
        }
        private String _DisNotes;
        partial void OnDisNotesChanging(String value);
        partial void OnDisNotesChanged();

        #endregion

        #region Navigation Properties



        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
