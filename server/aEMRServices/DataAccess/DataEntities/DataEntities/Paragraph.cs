using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Paragraph : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Paragraph object.

        /// <param name="paragraphID">Initial value of the ParagraphID property.</param>
        /// <param name="paragraphName">Initial value of the ParagraphName property.</param>
        public static Paragraph CreateParagraph(long paragraphID, String paragraphName)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.ParagraphID = paragraphID;
            paragraph.ParagraphName = paragraphName;
            return paragraph;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long ParagraphID
        {
            get
            {
                return _ParagraphID;
            }
            set
            {
                if (_ParagraphID != value)
                {
                    OnParagraphIDChanging(value);
                    ////ReportPropertyChanging("ParagraphID");
                    _ParagraphID = value;
                    RaisePropertyChanged("ParagraphID");
                    OnParagraphIDChanged();
                }
            }
        }
        private long _ParagraphID;
        partial void OnParagraphIDChanging(long value);
        partial void OnParagraphIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> SectionID
        {
            get
            {
                return _SectionID;
            }
            set
            {
                OnSectionIDChanging(value);
                ////ReportPropertyChanging("SectionID");
                _SectionID = value;
                RaisePropertyChanged("SectionID");
                OnSectionIDChanged();
            }
        }
        private Nullable<long> _SectionID;
        partial void OnSectionIDChanging(Nullable<long> value);
        partial void OnSectionIDChanged();





        [DataMemberAttribute()]
        public String ParagraphName
        {
            get
            {
                return _ParagraphName;
            }
            set
            {
                OnParagraphNameChanging(value);
                ////ReportPropertyChanging("ParagraphName");
                _ParagraphName = value;
                RaisePropertyChanged("ParagraphName");
                OnParagraphNameChanged();
            }
        }
        private String _ParagraphName;
        partial void OnParagraphNameChanging(String value);
        partial void OnParagraphNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMQUES_REL_PMR15_PARAGRAP", "ExamQuestionaire")]
        public ObservableCollection<ExamQuestionaire> ExamQuestionaires
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALR_REL_PMR34_PARAGRAP", "MedicalRecordTemplateOutline")]
        public ObservableCollection<MedicalRecordTemplateOutline> MedicalRecordTemplateOutlines
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PARAGRAP_REL_PMR14_SECTIONS", "Sections")]
        public Section Section
        {
            get;
            set;
        }

        #endregion
    }
}
