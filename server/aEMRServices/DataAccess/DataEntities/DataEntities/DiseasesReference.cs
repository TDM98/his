using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20210324 #001 TNHX: Add NotBeMain
 * 20221001 #002 QTD:  Add IsNewInYear
 * 20230322 #003 BLQ: Thêm trường bệnh dài ngày
 * 20230330 #004 BLQ: Thêm trường lưu mã ICD10 của mã YHCT
 */
namespace DataEntities
{
    public partial class DiseasesReference : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DiseasesReference object.

        /// <param name="iDCode">Initial value of the IDCode property.</param>
        /// <param name="diseaseNameVN">Initial value of the DiseaseNameVN property.</param>
        public static DiseasesReference CreateDiseasesReference(Int64 iDCode, String diseaseNameVN)
        {
            DiseasesReference diseasesReference = new DiseasesReference();
            diseasesReference.IDCode = iDCode;
            diseasesReference.DiseaseNameVN = diseaseNameVN;
            return diseasesReference;
        }

        #endregion
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

        [DataMemberAttribute()]
        public Nullable<long> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<long> _DeptID;
        partial void OnDeptIDChanging(Nullable<long> value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ParIDCode
        {
            get
            {
                return _ParIDCode;
            }
            set
            {
                OnParIDCodeChanging(value);
                _ParIDCode = value;
                RaisePropertyChanged("ParIDCode");
                OnParIDCodeChanged();
            }
        }
        private Nullable<Int64> _ParIDCode;
        partial void OnParIDCodeChanging(Nullable<Int64> value);
        partial void OnParIDCodeChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

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
        public String DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                OnDiseaseNameVNChanging(value);
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
                OnDiseaseNameVNChanged();
            }
        }
        private String _DiseaseNameVN;
        partial void OnDiseaseNameVNChanging(String value);
        partial void OnDiseaseNameVNChanged();

        [DataMemberAttribute()]
        public String DiseaseName
        {
            get
            {
                return _DiseaseName;
            }
            set
            {
                OnDiseaseNameChanging(value);
                _DiseaseName = value;
                RaisePropertyChanged("DiseaseName");
                OnDiseaseNameChanged();
            }
        }
        private String _DiseaseName;
        partial void OnDiseaseNameChanging(String value);
        partial void OnDiseaseNameChanged();





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
                RaisePropertyChanged("DiseaseDescription");
                OnDiseaseDescriptionChanged();
            }
        }
        private String _DiseaseDescription;
        partial void OnDiseaseDescriptionChanging(String value);
        partial void OnDiseaseDescriptionChanged();





        [DataMemberAttribute()]
        public String DiseaseNotes
        {
            get
            {
                return _DiseaseNotes;
            }
            set
            {
                OnDiseaseNotesChanging(value);
                _DiseaseNotes = value;
                RaisePropertyChanged("DiseaseNotes");
                OnDiseaseNotesChanged();
            }
        }
        private String _DiseaseNotes;
        partial void OnDiseaseNotesChanging(String value);
        partial void OnDiseaseNotesChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsUserDefined
        {
            get
            {
                return _IsUserDefined;
            }
            set
            {
                OnIsUserDefinedChanging(value);
                _IsUserDefined = value;
                RaisePropertyChanged("IsUserDefined");
                OnIsUserDefinedChanged();
            }
        }
        private Nullable<Boolean> _IsUserDefined;
        partial void OnIsUserDefinedChanging(Nullable<Boolean> value);
        partial void OnIsUserDefinedChanged();

        private string _DiagnosisFinal;
        [DataMemberAttribute()]
        public string DiagnosisFinal
        {
            get
            {
                return _DiagnosisFinal;
            }
            set
            {
                _DiagnosisFinal = value;
                RaisePropertyChanged("DiagnosisFinal");
            }
        }

        private bool _IsActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }




        [DataMemberAttribute()]
        public ObservableCollection<FamilyHistory> FamilyHistories
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistory> HospitalizationHistories
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PCLExamType> PCLExamTypes
        {
            get;
            set;
        }

        #endregion
        private int _TotalQuantity;
        private int _TotalMinute;
        private decimal _TotalAmount;
        private decimal _Frequency;
        [DataMemberAttribute]
        public int TotalQuantity
        {
            get
            {
                return _TotalQuantity;
            }
            set
            {
                if (_TotalQuantity == value)
                {
                    return;
                }
                _TotalQuantity = value;
                RaisePropertyChanged("TotalQuantity");
            }
        }
        [DataMemberAttribute]
        public int TotalMinute
        {
            get
            {
                return _TotalMinute;
            }
            set
            {
                if (_TotalMinute == value)
                {
                    return;
                }
                _TotalMinute = value;
                RaisePropertyChanged("TotalMinute");
            }
        }
        [DataMemberAttribute]
        public decimal TotalAmount
        {
            get
            {
                return _TotalAmount;
            }
            set
            {
                if (_TotalAmount == value)
                {
                    return;
                }
                _TotalAmount = value;
                RaisePropertyChanged("TotalAmount");
            }
        }
        [DataMemberAttribute]
        public decimal Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                if (_Frequency == value)
                {
                    return;
                }
                _Frequency = value;
                RaisePropertyChanged("Frequency");
            }
        }

        private bool _NotBeMain;
        [DataMemberAttribute]
        public bool NotBeMain
        {
            get
            {
                return _NotBeMain;
            }
            set
            {
                if (_NotBeMain == value)
                {
                    return;
                }
                _NotBeMain = value;
                RaisePropertyChanged("NotBeMain");
            }
        }
        private bool _IsNewInYear;
        [DataMemberAttribute]
        public bool IsNewInYear
        {
            get
            {
                return _IsNewInYear;
            }
            set
            {
                if (_IsNewInYear == value)
                {
                    return;
                }
                _IsNewInYear = value;
                RaisePropertyChanged("IsNewInYear");
            }
        }
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
    }
}
