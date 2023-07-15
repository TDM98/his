using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{


    public partial class Section : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Section object.

        /// <param name="sectionID">Initial value of the SectionID property.</param>
        /// <param name="sectionName">Initial value of the SectionName property.</param>
        public static Section CreateSection(long sectionID, String sectionName)
        {
            Section section = new Section();
            section.SectionID = sectionID;
            section.SectionName = sectionName;
            return section;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SectionID
        {
            get
            {
                return _SectionID;
            }
            set
            {
                if (_SectionID != value)
                {
                    OnSectionIDChanging(value);
                    ////ReportPropertyChanging("SectionID");
                    _SectionID = value;
                    RaisePropertyChanged("SectionID");
                    OnSectionIDChanged();
                }
            }
        }
        private long _SectionID;
        partial void OnSectionIDChanging(long value);
        partial void OnSectionIDChanged();





        [DataMemberAttribute()]
        public String SectionName
        {
            get
            {
                return _SectionName;
            }
            set
            {
                OnSectionNameChanging(value);
                ////ReportPropertyChanging("SectionName");
                _SectionName = value;
                RaisePropertyChanged("SectionName");
                OnSectionNameChanged();
            }
        }
        private String _SectionName;
        partial void OnSectionNameChanging(String value);
        partial void OnSectionNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PARAGRAP_REL_PMR14_SECTIONS", "Paragraphs")]
        public ObservableCollection<Paragraph> Paragraphs
        {
            get;
            set;
        }

        #endregion
    }
}
