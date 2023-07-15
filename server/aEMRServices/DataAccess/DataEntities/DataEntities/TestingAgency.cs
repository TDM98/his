using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class TestingAgency : NotifyChangedBase, IEditableObject
    {
        public TestingAgency()
            : base()
        {

        }

        private TestingAgency _tempRefUnit;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefUnit = (TestingAgency)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefUnit)
                CopyFrom(_tempRefUnit);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(TestingAgency p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Factory Method


        /// Create a new TestingAgency object.

        /// <param name="agencyID">Initial value of the AgencyID property.</param>
        /// <param name="agencyName">Initial value of the AgencyName property.</param>
        public static TestingAgency CreateTestingAgency(long agencyID, String agencyName)
        {
            TestingAgency testingAgency = new TestingAgency();
            testingAgency.AgencyID = agencyID;
            testingAgency.AgencyName = agencyName;
            return testingAgency;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long AgencyID
        {
            get
            {
                return _AgencyID;
            }
            set
            {
                if (_AgencyID != value)
                {
                    OnAgencyIDChanging(value);
                    
                    _AgencyID = value;
                    RaisePropertyChanged("AgencyID");
                    OnAgencyIDChanged();
                }
            }
        }
        private long _AgencyID;
        partial void OnAgencyIDChanging(long value);
        partial void OnAgencyIDChanged();


        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);

                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();


        [DataMemberAttribute()]
        public Nullable<long> HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                OnHosIDChanging(value);
                
                _HosID = value;
                RaisePropertyChanged("HosID");
                OnHosIDChanged();
            }
        }
        private Nullable<long> _HosID;
        partial void OnHosIDChanging(Nullable<long> value);
        partial void OnHosIDChanged();




        [Required(ErrorMessage = "Agency Name is Required")]
        [DataMemberAttribute()]
        public String AgencyName
        {
            get
            {
                return _AgencyName;
            }
            set
            {
                OnAgencyNameChanging(value);
                ValidateProperty("AgencyName", value);
                _AgencyName = value;
                RaisePropertyChanged("AgencyName");
                OnAgencyNameChanged();
            }
        }
        private String _AgencyName;
        partial void OnAgencyNameChanging(String value);
        partial void OnAgencyNameChanged();





        [DataMemberAttribute()]
        public String AgencyAddress
        {
            get
            {
                return _AgencyAddress;
            }
            set
            {
                OnAgencyAddressChanging(value);
                _AgencyAddress = value;
                RaisePropertyChanged("AgencyAddress");
                OnAgencyAddressChanged();
            }
        }
        private String _AgencyAddress;
        partial void OnAgencyAddressChanging(String value);
        partial void OnAgencyAddressChanged();





        [DataMemberAttribute()]
        public String AgencyNotes
        {
            get
            {
                return _AgencyNotes;
            }
            set
            {
                OnAgencyNotesChanging(value);
                _AgencyNotes = value;
                RaisePropertyChanged("AgencyNotes");
                OnAgencyNotesChanged();
            }
        }
        private String _AgencyNotes;
        partial void OnAgencyNotesChanging(String value);
        partial void OnAgencyNotesChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Hospital Hospital
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLImagingResult> PatientPCLExamResults
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            TestingAgency seletedTesingAgency = obj as TestingAgency;
            if (seletedTesingAgency == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.AgencyID == seletedTesingAgency.AgencyID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
