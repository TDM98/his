using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriterionAttachICD : EntityBase, IEditableObject
    {
        public static AdmissionCriterionAttachICD CreateSymptomCategory(Int64 ACAI_ID)
        {
            AdmissionCriterionAttachICD ac = new AdmissionCriterionAttachICD();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long ACAI_ID
        {
            get
            {
                return _ACAI_ID;
            }
            set
            {
                if (_ACAI_ID == value)
                {
                    return;
                }
                _ACAI_ID = value;
                RaisePropertyChanged("ACAI_ID");
            }
        }
        private long _ACAI_ID;

        [DataMemberAttribute()]
        public long AdmissionCriterionID
        {
            get
            {
                return _AdmissionCriterionID;
            }
            set
            {
                if (_AdmissionCriterionID == value)
                {
                    return;
                }
                _AdmissionCriterionID = value;
                RaisePropertyChanged("AdmissionCriterionID");
            }
        }
        private long _AdmissionCriterionID;

        [DataMemberAttribute()]
        public long IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                if (_IDCode == value)
                {
                    return;
                }
                _IDCode = value;
                RaisePropertyChanged("IDCode");
            }
        }
        private long _IDCode;

        [DataMemberAttribute()]
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                if (_ICD10Code == value)
                {
                    return;
                }
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
            }
        }
        private string _ICD10Code;
        [DataMemberAttribute()]
        public string DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                if (_DiseaseNameVN == value)
                {
                    return;
                }
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
            }
        }
        private string _DiseaseNameVN;


        [DataMemberAttribute]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                if (_IsDelete == value)
                {
                    return;
                }
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
        private bool _IsDelete;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

    
        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }


        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
      
        public ObservableCollection<AdmissionCriterionAttachICD> SymptomAttachICDs
        {
            get;
            set;
        }
        #endregion
    }
}
