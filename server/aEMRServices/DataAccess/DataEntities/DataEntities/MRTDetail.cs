using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class MRTDetail : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MRTDetail object.

        /// <param name="mRTDetailsID">Initial value of the MRTDetailsID property.</param>
        /// <param name="quesID">Initial value of the QuesID property.</param>
        /// <param name="medRecTempOutlineID">Initial value of the MedRecTempOutlineID property.</param>
        public static MRTDetail CreateMRTDetail(long mRTDetailsID, long quesID, long medRecTempOutlineID)
        {
            MRTDetail mRTDetail = new MRTDetail();
            mRTDetail.MRTDetailsID = mRTDetailsID;
            mRTDetail.QuesID = quesID;
            mRTDetail.MedRecTempOutlineID = medRecTempOutlineID;
            return mRTDetail;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MRTDetailsID
        {
            get
            {
                return _MRTDetailsID;
            }
            set
            {
                if (_MRTDetailsID != value)
                {
                    OnMRTDetailsIDChanging(value);
                    ////ReportPropertyChanging("MRTDetailsID");
                    _MRTDetailsID = value;
                    RaisePropertyChanged("MRTDetailsID");
                    OnMRTDetailsIDChanged();
                }
            }
        }
        private long _MRTDetailsID;
        partial void OnMRTDetailsIDChanging(long value);
        partial void OnMRTDetailsIDChanged();





        [DataMemberAttribute()]
        public long QuesID
        {
            get
            {
                return _QuesID;
            }
            set
            {
                OnQuesIDChanging(value);
                ////ReportPropertyChanging("QuesID");
                _QuesID = value;
                RaisePropertyChanged("QuesID");
                OnQuesIDChanged();
            }
        }
        private long _QuesID;
        partial void OnQuesIDChanging(long value);
        partial void OnQuesIDChanged();





        [DataMemberAttribute()]
        public long MedRecTempOutlineID
        {
            get
            {
                return _MedRecTempOutlineID;
            }
            set
            {
                OnMedRecTempOutlineIDChanging(value);
                ////ReportPropertyChanging("MedRecTempOutlineID");
                _MedRecTempOutlineID = value;
                RaisePropertyChanged("MedRecTempOutlineID");
                OnMedRecTempOutlineIDChanged();
            }
        }
        private long _MedRecTempOutlineID;
        partial void OnMedRecTempOutlineIDChanging(long value);
        partial void OnMedRecTempOutlineIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> QuesIdx
        {
            get
            {
                return _QuesIdx;
            }
            set
            {
                OnQuesIdxChanging(value);
                ////ReportPropertyChanging("QuesIdx");
                _QuesIdx = value;
                RaisePropertyChanged("QuesIdx");
                OnQuesIdxChanged();
            }
        }
        private Nullable<Byte> _QuesIdx;
        partial void OnQuesIdxChanging(Nullable<Byte> value);
        partial void OnQuesIdxChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> QuesIsRequired
        {
            get
            {
                return _QuesIsRequired;
            }
            set
            {
                OnQuesIsRequiredChanging(value);
                ////ReportPropertyChanging("QuesIsRequired");
                _QuesIsRequired = value;
                RaisePropertyChanged("QuesIsRequired");
                OnQuesIsRequiredChanged();
            }
        }
        private Nullable<Boolean> _QuesIsRequired;
        partial void OnQuesIsRequiredChanging(Nullable<Boolean> value);
        partial void OnQuesIsRequiredChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMINAT_REL_PMR10_MRTDETAI", "Examination")]
        public ObservableCollection<Examination> Examinations
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MRTDETAI_REL_PMR13_EXAMQUES", "ExamQuestionaire")]
        public ExamQuestionaire ExamQuestionaire
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MRTDETAI_REL_PMR35_MEDICALR", "MedicalRecordTemplateOutline")]
        public MedicalRecordTemplateOutline MedicalRecordTemplateOutline
        {
            get;
            set;
        }

        #endregion
    }

}
