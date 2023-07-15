using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    /*
    * 20210325 #001 BLQ: 247 Thêm trường cho ICD
    * 20211001 #002 BLQ: 247 Đánh dấu mã bệnh mới trong năm
    * 20230322 #003 BLQ: Thêm trường bệnh dài ngày
    * 20230330 #004 BLQ: Thêm trường lưu mã ICD10 của mã YHCT
    */
    public partial class ICD: EntityBase, IEditableObject
    {
        public static ICD CreateICD(Int64 IDCode,String ICD10Code, String DiseaseNameVN)
        {
            ICD icd = new ICD();
            icd.IDCode = IDCode;
            icd.ICD10Code = ICD10Code;
            icd.DiseaseNameVN = DiseaseNameVN;
            return icd;
        }
        #region Primitive Properties


        [DataMemberAttribute()]
        public Int64 IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                if (_IDCode != value)
                {
                    OnIDCodeChanging(value);
                    _IDCode = value;
                    RaisePropertyChanged("IDCode");
                    OnIDCodeChanged();
                }
            }
        }
        private Int64 _IDCode;
        partial void OnIDCodeChanging(Int64 value);
        partial void OnIDCodeChanged();

        

        [Required(ErrorMessage = "Nhập chẩn đoán!")]
        [DataMemberAttribute()]
        public String DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                OnDiseaseNameVNChanging(value);
                ValidateProperty("DiseaseNameVN", value);
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
                OnDiseaseNameVNChanged();
            }
        }
        private String _DiseaseNameVN;
        partial void OnDiseaseNameVNChanging(String value);
        partial void OnDiseaseNameVNChanged();
        


        [DataMemberAttribute()]
        public String ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                OnICD10CodeChanging(value);
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
                OnICD10CodeChanged();
            }
        }
        private String _ICD10Code;
        partial void OnICD10CodeChanging(String value);
        partial void OnICD10CodeChanged();



        [DataMemberAttribute()]
        public String DiseaseDescription
        {
            get
            {
                return _DiseaseDescription;
            }
            set
            {
                OnDiseaseDescriptionChanging(value);
                _DiseaseDescription = value;
                RaisePropertyChanged("ICD10Code");
                OnDiseaseDescriptionChanged();
            }
        }
        private String _DiseaseDescription;
        partial void OnDiseaseDescriptionChanging(String value);
        partial void OnDiseaseDescriptionChanged();


        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        [DataMemberAttribute()]
        public String DoctorAdvice
        {
            get
            {
                return _DoctorAdvice;
            }
            set
            {
                OnDoctorAdviceChanging(value);
                _DoctorAdvice = value;
                RaisePropertyChanged("DoctorAdvice");
                OnDoctorAdviceChanged();
            }
        }
        private String _DoctorAdvice;
        partial void OnDoctorAdviceChanging(String value);
        partial void OnDoctorAdviceChanged();

        [DataMemberAttribute()]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                OnDateModifiedChanging(value);
                _DateModified = value;
                RaisePropertyChanged("DateModified");
                OnDateModifiedChanged();
            }
        }
        private DateTime _DateModified;
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();

        [DataMemberAttribute()]
        public String ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                OnModifiedLogChanging(value);
                _ModifiedLog = value;
                RaisePropertyChanged("ModifiedLog");
                OnModifiedLogChanged();
            }
        }
        private String _ModifiedLog;
        partial void OnModifiedLogChanging(String value);
        partial void OnModifiedLogChanged();

        //▼====: #001
        [DataMemberAttribute()]
        public Gender Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                OnGenderChanging(value);
                _Gender = value;
                RaisePropertyChanged("Gender");
                OnGenderChanged();
            }
        }
        private Gender _Gender;
        partial void OnGenderChanging(Gender value);
        partial void OnGenderChanged();

        [DataMemberAttribute()]
        public Int64 AgeFrom
        {
            get
            {
                return _AgeFrom;
            }
            set
            {
                if (_AgeFrom != value)
                {
                    OnAgeFromChanging(value);
                    _AgeFrom = value;
                    RaisePropertyChanged("AgeFrom");
                    OnAgeFromChanged();
                }
            }
        }
        private Int64 _AgeFrom;
        partial void OnAgeFromChanging(Int64 value);
        partial void OnAgeFromChanged();
        [DataMemberAttribute()]
        public Int64 AgeTo
        {
            get
            {
                return _AgeTo;
            }
            set
            {
                if (_AgeTo != value)
                {
                    OnAgeToChanging(value);
                    _AgeTo = value;
                    RaisePropertyChanged("AgeTo");
                    OnAgeToChanged();
                }
            }
        }
        private Int64 _AgeTo;
        partial void OnAgeToChanging(Int64 value);
        partial void OnAgeToChanged();

        [DataMemberAttribute()]
        public bool NotBeMain
        {
            get
            {
                return _NotBeMain;
            }
            set
            {
                OnNotBeMainChanging(value);
                _NotBeMain = value;
                RaisePropertyChanged("NotBeMain");
                OnNotBeMainChanged();
            }
        }
        private bool _NotBeMain;
        partial void OnNotBeMainChanging(bool value);
        partial void OnNotBeMainChanged();
        //▲====: #001
        [DataMemberAttribute()]
        public bool IsNewInYear
        {
            get
            {
                return _IsNewInYear;
            }
            set
            {
                OnIsNewInYearChanging(value);
                _IsNewInYear = value;
                RaisePropertyChanged("IsNewInYear");
                OnIsNewInYearChanged();
            }
        }
        private bool _IsNewInYear;
        partial void OnIsNewInYearChanging(bool value);
        partial void OnIsNewInYearChanged();

        //▼====: #003
        private bool _IsLongTermIllness;
        [DataMemberAttribute]
        public bool IsLongTermIllness
        {
            get
            {
                return _IsLongTermIllness;
            }
            set
            {
                if (_IsLongTermIllness == value)
                {
                    return;
                }
                _IsLongTermIllness = value;
                RaisePropertyChanged("IsLongTermIllness");
            }
        }
        //▲====: #003
        //▼====: #004
        private bool _IsICD10CodeYHCT;
        [DataMemberAttribute]
        public bool IsICD10CodeYHCT
        {
            get
            {
                return _IsICD10CodeYHCT;
            }
            set
            {
                if (_IsICD10CodeYHCT == value)
                {
                    return;
                }
                _IsICD10CodeYHCT = value;
                RaisePropertyChanged("IsICD10CodeYHCT");
            }
        }
        private string _ICD10CodeFromYHCT;
        [DataMemberAttribute]
        public string ICD10CodeFromYHCT
        {
            get
            {
                return _ICD10CodeFromYHCT;
            }
            set
            {
                if (_ICD10CodeFromYHCT == value)
                {
                    return;
                }
                _ICD10CodeFromYHCT = value;
                RaisePropertyChanged("ICD10CodeFromYHCT");
            }
        }
        //▲====: #004
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
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "Locations")]
        public ObservableCollection<ICD> ICDs
        {
            get;
            set;
        }
        #endregion
    }
}
