using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriterionAttachSymptom : EntityBase, IEditableObject
    {
        public static AdmissionCriterionAttachSymptom CreateSymptomCategory(Int64 ACAS_ID)
        {
            AdmissionCriterionAttachSymptom ac = new AdmissionCriterionAttachSymptom();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long ACAS_ID
        {
            get
            {
                return _ACAS_ID;
            }
            set
            {
                if (_ACAS_ID == value)
                {
                    return;
                }
                _ACAS_ID = value;
                RaisePropertyChanged("ACAS_ID");
            }
        }
        private long _ACAS_ID;

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
        public long SymptomCategoryID
        {
            get
            {
                return _SymptomCategoryID;
            }
            set
            {
                if (_SymptomCategoryID == value)
                {
                    return;
                }
                _SymptomCategoryID = value;
                RaisePropertyChanged("SymptomCategoryID");
            }
        }
        private long _SymptomCategoryID;

        [DataMemberAttribute()]
        public string SymptomCategoryName
        {
            get
            {
                return _SymptomCategoryName;
            }
            set
            {
                if (_SymptomCategoryName == value)
                {
                    return;
                }
                _SymptomCategoryName = value;
                RaisePropertyChanged("SymptomCategoryName");
            }
        }
        private string _SymptomCategoryName;

        [DataMemberAttribute()]
        public long V_SymptomType
        {
            get
            {
                return _V_SymptomType;
            }
            set
            {
                if (_V_SymptomType == value)
                {
                    return;
                }
                _V_SymptomType = value;
                RaisePropertyChanged("V_SymptomType");
            }
        }
        private long _V_SymptomType;


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
