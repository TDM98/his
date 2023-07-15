using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalSpecimen : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new MedicalSpecimen object.
     
        /// <param name="medSpecID">Initial value of the MedSpecID property.</param>
        /// <param name="medSpecName">Initial value of the MedSpecName property.</param>
        public static MedicalSpecimen CreateMedicalSpecimen(Int64 medSpecID, String medSpecName)
        {
            MedicalSpecimen medicalSpecimen = new MedicalSpecimen();
            medicalSpecimen.MedSpecID = medSpecID;
            medicalSpecimen.MedSpecName = medSpecName;
            return medicalSpecimen;
        }

        #endregion
        #region Primitive Properties

     
        
     
        [DataMemberAttribute()]
        public Int64 MedSpecID
        {
            get
            {
                return _MedSpecID;
            }
            set
            {
                if (_MedSpecID != value)
                {
                    OnMedSpecIDChanging(value);
                    _MedSpecID = value;
                    RaisePropertyChanged("MedSpecID");
                    OnMedSpecIDChanged();
                }
            }
        }
        private Int64 _MedSpecID;
        partial void OnMedSpecIDChanging(Int64 value);
        partial void OnMedSpecIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int16> MedSpecCatID
        {
            get
            {
                return _MedSpecCatID;
            }
            set
            {
                OnMedSpecCatIDChanging(value);
                _MedSpecCatID = value;
                RaisePropertyChanged("MedSpecCatID");
                OnMedSpecCatIDChanged();
            }
        }
        private Nullable<Int16> _MedSpecCatID;
        partial void OnMedSpecCatIDChanging(Nullable<Int16> value);
        partial void OnMedSpecCatIDChanged();

     
        
     
        [DataMemberAttribute()]
        public String MedSpecName
        {
            get
            {
                return _MedSpecName;
            }
            set
            {
                OnMedSpecNameChanging(value);
                _MedSpecName = value;
                RaisePropertyChanged("MedSpecName");
                OnMedSpecNameChanged();
            }
        }
        private String _MedSpecName;
        partial void OnMedSpecNameChanging(String value);
        partial void OnMedSpecNameChanged();

     
        
     
        [DataMemberAttribute()]
        public String StorageConditions
        {
            get
            {
                return _StorageConditions;
            }
            set
            {
                OnStorageConditionsChanging(value);
                _StorageConditions = value;
                RaisePropertyChanged("StorageConditions");
                OnStorageConditionsChanged();
            }
        }
        private String _StorageConditions;
        partial void OnStorageConditionsChanging(String value);
        partial void OnStorageConditionsChanged();

     
        
     
        [DataMemberAttribute()]
        public String MedSpecNotes
        {
            get
            {
                return _MedSpecNotes;
            }
            set
            {
                OnMedSpecNotesChanging(value);
                _MedSpecNotes = value;
                RaisePropertyChanged("MedSpecNotes");
                OnMedSpecNotesChanged();
            }
        }
        private String _MedSpecNotes;
        partial void OnMedSpecNotesChanging(String value);
        partial void OnMedSpecNotesChanged();

        #endregion

        #region Navigation Properties

     
        
     
        [DataMemberAttribute()]
        public MedicalSpecimensCategory MedicalSpecimensCategory
        {
            get;
            set;
        }
        #endregion

    }
}
