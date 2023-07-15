using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class DeceasedInfo : EntityBase, IEditableObject
    {
        public DeceasedInfo()
            : base()
        {

        }

        private DeceasedInfo _tempDeceasedInfo;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDeceasedInfo = (DeceasedInfo)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDeceasedInfo)
                CopyFrom(_tempDeceasedInfo);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DeceasedInfo p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
 
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DSNumber
        {
            get
            {
                return _DSNumber;
            }
            set
            {
                if (_DSNumber != value)
                {
                    OnDSNumberChanging(value);
                    _DSNumber = value;
                    RaisePropertyChanged("DSNumber");
                    OnDSNumberChanged();
                }
            }
        }
        private long _DSNumber;
        partial void OnDSNumberChanging(long value);
        partial void OnDSNumberChanged();





        [DataMemberAttribute()]
        public Nullable<long> CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                OnCommonMedRecIDChanging(value);
                _CommonMedRecID = value;
                RaisePropertyChanged("CommonMedRecID");
                OnCommonMedRecIDChanged();
            }
        }
        private Nullable<long> _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(Nullable<long> value);
        partial void OnCommonMedRecIDChanged();





        [DataMemberAttribute()]
        public DateTime DeceasedDateTime
        {
            get
            {
                return _DeceasedDateTime;
            }
            set
            {
                OnDeceasedDateTimeChanging(value);
                _DeceasedDateTime = value;
                RaisePropertyChanged("DeceasedDateTime");
                OnDeceasedDateTimeChanged();
            }
        }
        private DateTime _DeceasedDateTime;
        partial void OnDeceasedDateTimeChanging(DateTime value);
        partial void OnDeceasedDateTimeChanged();





        [DataMemberAttribute()]
        public AllLookupValues.CategoryOfDecease V_CategoryOfDecease
        {
            get
            {
                return _V_CategoryOfDecease;
            }
            set
            {
                _V_CategoryOfDecease = value;
                RaisePropertyChanged("V_CategoryOfDecease");
            }
        }
        private AllLookupValues.CategoryOfDecease _V_CategoryOfDecease;





        [DataMemberAttribute()]
        public String MainReasonOfDecease
        {
            get
            {
                return _mainReasonOfDecease;
            }
            set
            {
                OnMainCauseOfDeceaseChanging(value);
                _mainReasonOfDecease = value;
                RaisePropertyChanged("MainReasonOfDecease");
                OnMainCauseOfDeceaseChanged();
            }
        }
        private String _mainReasonOfDecease;
        partial void OnMainCauseOfDeceaseChanging(String value);
        partial void OnMainCauseOfDeceaseChanged();





        [DataMemberAttribute()]
        public string MainCauseOfDeceaseCode
        {
            get
            {
                return _MainCauseOfDeceaseCode;
            }
            set
            {
                _MainCauseOfDeceaseCode = value;
                RaisePropertyChanged("MainCauseOfDeceaseCode");
            }
        }
        private string _MainCauseOfDeceaseCode;
        
        [DataMemberAttribute()]
        public Nullable<Boolean> IsPostMorternExam
        {
            get
            {
                return _IsPostMorternExam;
            }
            set
            {
                OnIsPostMorternExamChanging(value);
                _IsPostMorternExam = value;
                RaisePropertyChanged("IsPostMorternExam");
                OnIsPostMorternExamChanged();
            }
        }
        private Nullable<Boolean> _IsPostMorternExam;
        partial void OnIsPostMorternExamChanging(Nullable<Boolean> value);
        partial void OnIsPostMorternExamChanged();





        [DataMemberAttribute()]
        public String PostMortemExamDiagnosis
        {
            get
            {
                return _PostMortemExamDiagnosis;
            }
            set
            {
                OnPostMortemExamDiagnosisChanging(value);
                _PostMortemExamDiagnosis = value;
                RaisePropertyChanged("PostMortemExamDiagnosis");
                OnPostMortemExamDiagnosisChanged();
            }
        }
        private String _PostMortemExamDiagnosis;
        partial void OnPostMortemExamDiagnosisChanging(String value);
        partial void OnPostMortemExamDiagnosisChanged();





        [DataMemberAttribute()]
        public string PostMortemExamCode
        {
            get
            {
                return _PostMortemExamCode;
            }
            set
            {
                _PostMortemExamCode = value;
                RaisePropertyChanged("PostMortemExamCode");
            }
        }
        private string _PostMortemExamCode;

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        public CommonMedicalRecord CommonMedicalRecord
        {
            get;
            set;

        }

        #endregion

        private long _ptRegistrationID;

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get { return _ptRegistrationID; }
            set
            {
                _ptRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
    }
}
