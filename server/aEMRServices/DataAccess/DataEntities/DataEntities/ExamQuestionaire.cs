using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class ExamQuestionaire : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ExamQuestionaire object.

        /// <param name="quesID">Initial value of the QuesID property.</param>
        /// <param name="quesContent">Initial value of the QuesContent property.</param>
        public static ExamQuestionaire CreateExamQuestionaire(long quesID, String quesContent)
        {
            ExamQuestionaire examQuestionaire = new ExamQuestionaire();
            examQuestionaire.QuesID = quesID;
            examQuestionaire.QuesContent = quesContent;
            return examQuestionaire;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long QuesID
        {
            get
            {
                return _QuesID;
            }
            set
            {
                if (_QuesID != value)
                {
                    OnQuesIDChanging(value);
                    ////ReportPropertyChanging("QuesID");
                    _QuesID = value;
                    RaisePropertyChanged("QuesID");
                    OnQuesIDChanged();
                }
            }
        }
        private long _QuesID;
        partial void OnQuesIDChanging(long value);
        partial void OnQuesIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ParagraphID
        {
            get
            {
                return _ParagraphID;
            }
            set
            {
                OnParagraphIDChanging(value);
                ////ReportPropertyChanging("ParagraphID");
                _ParagraphID = value;
                RaisePropertyChanged("ParagraphID");
                OnParagraphIDChanged();
            }
        }
        private Nullable<long> _ParagraphID;
        partial void OnParagraphIDChanging(Nullable<long> value);
        partial void OnParagraphIDChanged();





        [DataMemberAttribute()]
        public String QuesContent
        {
            get
            {
                return _QuesContent;
            }
            set
            {
                OnQuesContentChanging(value);
                ////ReportPropertyChanging("QuesContent");
                _QuesContent = value;
                RaisePropertyChanged("QuesContent");
                OnQuesContentChanged();
            }
        }
        private String _QuesContent;
        partial void OnQuesContentChanging(String value);
        partial void OnQuesContentChanged();





        [DataMemberAttribute()]
        public String QuestDescription
        {
            get
            {
                return _QuestDescription;
            }
            set
            {
                OnQuestDescriptionChanging(value);
                ////ReportPropertyChanging("QuestDescription");
                _QuestDescription = value;
                RaisePropertyChanged("QuestDescription");
                OnQuestDescriptionChanged();
            }
        }
        private String _QuestDescription;
        partial void OnQuestDescriptionChanging(String value);
        partial void OnQuestDescriptionChanged();





        [DataMemberAttribute()]
        public String QuesNotes
        {
            get
            {
                return _QuesNotes;
            }
            set
            {
                OnQuesNotesChanging(value);
                ////ReportPropertyChanging("QuesNotes");
                _QuesNotes = value;
                RaisePropertyChanged("QuesNotes");
                OnQuesNotesChanged();
            }
        }
        private String _QuesNotes;
        partial void OnQuesNotesChanging(String value);
        partial void OnQuesNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMQUES_REL_PMR15_PARAGRAP", "Paragraphs")]
        public Paragraph Paragraph
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MRTDETAI_REL_PMR13_EXAMQUES", "MRTDetails")]
        public ObservableCollection<MRTDetail> MRTDetails
        {
            get;
            set;
        }

        #endregion
    }
}
