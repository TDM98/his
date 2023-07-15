using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ProfessionalDegree : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ProfessionalDegree object.

        /// <param name="profDegID">Initial value of the ProfDegID property.</param>
        /// <param name="profDegName">Initial value of the ProfDegName property.</param>
        public static ProfessionalDegree CreateProfessionalDegree(long profDegID, String profDegName)
        {
            ProfessionalDegree professionalDegree = new ProfessionalDegree();
            professionalDegree.ProfDegID = profDegID;
            professionalDegree.ProfDegName = profDegName;
            return professionalDegree;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long ProfDegID
        {
            get
            {
                return _ProfDegID;
            }
            set
            {
                if (_ProfDegID != value)
                {
                    OnProfDegIDChanging(value);
                    ////ReportPropertyChanging("ProfDegID");
                    _ProfDegID = value;
                    RaisePropertyChanged("ProfDegID");
                    OnProfDegIDChanged();
                }
            }
        }
        private long _ProfDegID;
        partial void OnProfDegIDChanging(long value);
        partial void OnProfDegIDChanged();





        [DataMemberAttribute()]
        public String ProfDegName
        {
            get
            {
                return _ProfDegName;
            }
            set
            {
                OnProfDegNameChanging(value);
                ////ReportPropertyChanging("ProfDegName");
                _ProfDegName = value;
                RaisePropertyChanged("ProfDegName");
                OnProfDegNameChanged();
            }
        }
        private String _ProfDegName;
        partial void OnProfDegNameChanging(String value);
        partial void OnProfDegNameChanged();





        [DataMemberAttribute()]
        public String ProfDegNotes
        {
            get
            {
                return _ProfDegNotes;
            }
            set
            {
                OnProfDegNotesChanging(value);
                ////ReportPropertyChanging("ProfDegNotes");
                _ProfDegNotes = value;
                RaisePropertyChanged("ProfDegNotes");
                OnProfDegNotesChanged();
            }
        }
        private String _ProfDegNotes;
        partial void OnProfDegNotesChanging(String value);
        partial void OnProfDegNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROFESSI_REL_HR01_PROFESSI", "ProfessionalSkill")]
        public ObservableCollection<ProfessionalSkill> ProfessionalSkills
        {
            get;
            set;
        }

        #endregion
    }
}
