using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

/*
* 20230105 #001 DatTB: Thêm các thông tin người thân cho nội trú
*/
namespace DataEntities
{
    public partial class SelfDeclaration : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long SelfDeclarationSheetID
        {
            get
            {
                return _SelfDeclarationSheetID;
            }
            set
            {
                if (_SelfDeclarationSheetID == value)
                {
                    return;
                }
                _SelfDeclarationSheetID = value;
                RaisePropertyChanged("SelfDeclarationSheetID");
            }
        }
        private long _SelfDeclarationSheetID;

        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PatientID;

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;

        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        private long _V_RegistrationType;

        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

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

        [DataMemberAttribute]
        public long LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime _LastUpdateDate;

        [DataMemberAttribute]
        public bool Answer1
        {
            get
            {
                return _Answer1;
            }
            set
            {
                if (_Answer1 == value)
                {
                    return;
                }
                _Answer1 = value;
                RaisePropertyChanged("Answer1");
            }
        }
        private bool _Answer1;

        [DataMemberAttribute]
        public int Answer1_Count
        {
            get
            {
                return _Answer1_Count;
            }
            set
            {
                if (_Answer1_Count == value)
                {
                    return;
                }
                _Answer1_Count = value;
                RaisePropertyChanged("Answer1_Count");
            }
        }
        private int _Answer1_Count;

        [DataMemberAttribute]
        public string Answer1_Drug
        {
            get
            {
                return _Answer1_Drug;
            }
            set
            {
                if (_Answer1_Drug == value)
                {
                    return;
                }
                _Answer1_Drug = value;
                RaisePropertyChanged("Answer1_Drug");
            }
        }
        private string _Answer1_Drug;

        [DataMemberAttribute]
        public string Answer1_Solve
        {
            get
            {
                return _Answer1_Solve;
            }
            set
            {
                if (_Answer1_Solve == value)
                {
                    return;
                }
                _Answer1_Solve = value;
                RaisePropertyChanged("Answer1_Solve");
            }
        }
        private string _Answer1_Solve;

        [DataMemberAttribute]
        public bool Answer2
        {
            get
            {
                return _Answer2;
            }
            set
            {
                if (_Answer2 == value)
                {
                    return;
                }
                _Answer2 = value;
                RaisePropertyChanged("Answer2");
            }
        }
        private bool _Answer2;

        [DataMemberAttribute]
        public int Answer2_Count
        {
            get
            {
                return _Answer2_Count;
            }
            set
            {
                if (_Answer2_Count == value)
                {
                    return;
                }
                _Answer2_Count = value;
                RaisePropertyChanged("Answer2_Count");
            }
        }
        private int _Answer2_Count;

        [DataMemberAttribute]
        public string Answer2_Drug
        {
            get
            {
                return _Answer2_Drug;
            }
            set
            {
                if (_Answer2_Drug == value)
                {
                    return;
                }
                _Answer2_Drug = value;
                RaisePropertyChanged("Answer2_Drug");
            }
        }
        private string _Answer2_Drug;

        [DataMemberAttribute]
        public string Answer2_Solve
        {
            get
            {
                return _Answer2_Solve;
            }
            set
            {
                if (_Answer2_Solve == value)
                {
                    return;
                }
                _Answer2_Solve = value;
                RaisePropertyChanged("Answer2_Solve");
            }
        }
        private string _Answer2_Solve;

        [DataMemberAttribute]
        public bool Answer3
        {
            get
            {
                return _Answer3;
            }
            set
            {
                if (_Answer3 == value)
                {
                    return;
                }
                _Answer3 = value;
                RaisePropertyChanged("Answer3");
            }
        }
        private bool _Answer3;

        [DataMemberAttribute]
        public int Answer3_Count
        {
            get
            {
                return _Answer3_Count;
            }
            set
            {
                if (_Answer3_Count == value)
                {
                    return;
                }
                _Answer3_Count = value;
                RaisePropertyChanged("Answer3_Count");
            }
        }
        private int _Answer3_Count;

        [DataMemberAttribute]
        public string Answer3_Drug
        {
            get
            {
                return _Answer3_Drug;
            }
            set
            {
                if (_Answer3_Drug == value)
                {
                    return;
                }
                _Answer3_Drug = value;
                RaisePropertyChanged("Answer3_Drug");
            }
        }
        private string _Answer3_Drug;

        [DataMemberAttribute]
        public string Answer3_Solve
        {
            get
            {
                return _Answer3_Solve;
            }
            set
            {
                if (_Answer3_Solve == value)
                {
                    return;
                }
                _Answer3_Solve = value;
                RaisePropertyChanged("Answer3_Solve");
            }
        }
        private string _Answer3_Solve;

        [DataMemberAttribute]
        public bool Answer4
        {
            get
            {
                return _Answer4;
            }
            set
            {
                if (_Answer4 == value)
                {
                    return;
                }
                _Answer4 = value;
                RaisePropertyChanged("Answer4");
            }
        }
        private bool _Answer4;

        [DataMemberAttribute]
        public int Answer4_Count
        {
            get
            {
                return _Answer4_Count;
            }
            set
            {
                if (_Answer4_Count == value)
                {
                    return;
                }
                _Answer4_Count = value;
                RaisePropertyChanged("Answer4_Count");
            }
        }
        private int _Answer4_Count;

        [DataMemberAttribute]
        public string Answer4_Drug
        {
            get
            {
                return _Answer4_Drug;
            }
            set
            {
                if (_Answer4_Drug == value)
                {
                    return;
                }
                _Answer4_Drug = value;
                RaisePropertyChanged("Answer4_Drug");
            }
        }
        private string _Answer4_Drug;

        [DataMemberAttribute]
        public string Answer4_Solve
        {
            get
            {
                return _Answer4_Solve;
            }
            set
            {
                if (_Answer4_Solve == value)
                {
                    return;
                }
                _Answer4_Solve = value;
                RaisePropertyChanged("Answer4_Solve");
            }
        }
        private string _Answer4_Solve;
        
        [DataMemberAttribute]
        public bool Answer5
        {
            get
            {
                return _Answer5;
            }
            set
            {
                if (_Answer5 == value)
                {
                    return;
                }
                _Answer5 = value;
                RaisePropertyChanged("Answer5");
            }
        }
        private bool _Answer5;

        [DataMemberAttribute]
        public int Answer5_Count
        {
            get
            {
                return _Answer5_Count;
            }
            set
            {
                if (_Answer5_Count == value)
                {
                    return;
                }
                _Answer5_Count = value;
                RaisePropertyChanged("Answer5_Count");
            }
        }
        private int _Answer5_Count;

        [DataMemberAttribute]
        public string Answer5_Drug
        {
            get
            {
                return _Answer5_Drug;
            }
            set
            {
                if (_Answer5_Drug == value)
                {
                    return;
                }
                _Answer5_Drug = value;
                RaisePropertyChanged("Answer5_Drug");
            }
        }
        private string _Answer5_Drug;

        [DataMemberAttribute]
        public string Answer5_Solve
        {
            get
            {
                return _Answer5_Solve;
            }
            set
            {
                if (_Answer5_Solve == value)
                {
                    return;
                }
                _Answer5_Solve = value;
                RaisePropertyChanged("Answer5_Solve");
            }
        }
        private string _Answer5_Solve;

        [DataMemberAttribute]
        public bool Answer6
        {
            get
            {
                return _Answer6;
            }
            set
            {
                if (_Answer6 == value)
                {
                    return;
                }
                _Answer6 = value;
                RaisePropertyChanged("Answer6");
            }
        }
        private bool _Answer6;

        [DataMemberAttribute]
        public int Answer6_Count
        {
            get
            {
                return _Answer6_Count;
            }
            set
            {
                if (_Answer6_Count == value)
                {
                    return;
                }
                _Answer6_Count = value;
                RaisePropertyChanged("Answer6_Count");
            }
        }
        private int _Answer6_Count;

        [DataMemberAttribute]
        public string Answer6_Drug
        {
            get
            {
                return _Answer6_Drug;
            }
            set
            {
                if (_Answer6_Drug == value)
                {
                    return;
                }
                _Answer6_Drug = value;
                RaisePropertyChanged("Answer6_Drug");
            }
        }
        private string _Answer6_Drug;

        [DataMemberAttribute]
        public string Answer6_Solve
        {
            get
            {
                return _Answer6_Solve;
            }
            set
            {
                if (_Answer6_Solve == value)
                {
                    return;
                }
                _Answer6_Solve = value;
                RaisePropertyChanged("Answer6_Solve");
            }
        }
        private string _Answer6_Solve;

        //▼==== #001
        [DataMemberAttribute]
        public string RelativeFullName
        {
            get
            {
                return _RelativeFullName;
            }
            set
            {
                if (_RelativeFullName == value)
                {
                    return;
                }
                _RelativeFullName = value;
                RaisePropertyChanged("RelativeFullName");
            }
        }
        private string _RelativeFullName;

        [DataMemberAttribute]
        public string RelativeAge
        {
            get
            {
                return _RelativeAge;
            }
            set
            {
                if (_RelativeAge == value)
                {
                    return;
                }
                _RelativeAge = value;
                RaisePropertyChanged("RelativeAge");
            }
        }
        private string _RelativeAge;

        [DataMemberAttribute]
        public string Relationship
        {
            get
            {
                return _Relationship;
            }
            set
            {
                if (_Relationship == value)
                {
                    return;
                }
                _Relationship = value;
                RaisePropertyChanged("Relationship");
            }
        }
        private string _Relationship;

        [DataMemberAttribute]
        public string RelativePhone
        {
            get
            {
                return _RelativePhone;
            }
            set
            {
                if (_RelativePhone == value)
                {
                    return;
                }
                _RelativePhone = value;
                RaisePropertyChanged("RelativePhone");
            }
        }
        private string _RelativePhone;

        [DataMemberAttribute]
        public decimal TotalCost
        {
            get
            {
                return _TotalCost;
            }
            set
            {
                if (_TotalCost == value)
                {
                    return;
                }
                _TotalCost = value;
                RaisePropertyChanged("TotalCost");
            }
        }
        private decimal _TotalCost;
        //▲==== #001
        #endregion
    }
}
