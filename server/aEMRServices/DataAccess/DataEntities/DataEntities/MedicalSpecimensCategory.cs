using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalSpecimensCategory : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new MedicalSpecimensCategory object.
     
        /// <param name="medSpecCatID">Initial value of the MedSpecCatID property.</param>
        /// <param name="medSpecCatName">Initial value of the MedSpecCatName property.</param>
        public static MedicalSpecimensCategory CreateMedicalSpecimensCategory(Int16 medSpecCatID, String medSpecCatName)
        {
            MedicalSpecimensCategory medicalSpecimensCategory = new MedicalSpecimensCategory();
            medicalSpecimensCategory.MedSpecCatID = medSpecCatID;
            medicalSpecimensCategory.MedSpecCatName = medSpecCatName;
            return medicalSpecimensCategory;
        }

        #endregion
        #region Primitive Properties

     
        
     
        [DataMemberAttribute()]
        public Int16 MedSpecCatID
        {
            get
            {
                return _MedSpecCatID;
            }
            set
            {
                if (_MedSpecCatID != value)
                {
                    OnMedSpecCatIDChanging(value);
                    _MedSpecCatID = value;
                    RaisePropertyChanged("MedSpecCatID");
                    OnMedSpecCatIDChanged();
                }
            }
        }
        private Int16 _MedSpecCatID;
        partial void OnMedSpecCatIDChanging(Int16 value);
        partial void OnMedSpecCatIDChanged();

     
        
     
        [DataMemberAttribute()]
        public String MedSpecCatName
        {
            get
            {
                return _MedSpecCatName;
            }
            set
            {
                OnMedSpecCatNameChanging(value);
                _MedSpecCatName = value;
                RaisePropertyChanged("MedSpecCatName");
                OnMedSpecCatNameChanged();
            }
        }
        private String _MedSpecCatName;
        partial void OnMedSpecCatNameChanging(String value);
        partial void OnMedSpecCatNameChanged();

     
        
     
        [DataMemberAttribute()]
        public String MedSpecCatNotes
        {
            get
            {
                return _MedSpecCatNotes;
            }
            set
            {
                OnMedSpecCatNotesChanging(value);
                _MedSpecCatNotes = value;
                RaisePropertyChanged("MedSpecCatNotes");
                OnMedSpecCatNotesChanged();
            }
        }
        private String _MedSpecCatNotes;
        partial void OnMedSpecCatNotesChanging(String value);
        partial void OnMedSpecCatNotesChanged();

        #endregion

        #region Navigation Properties


        #endregion

    }
}
