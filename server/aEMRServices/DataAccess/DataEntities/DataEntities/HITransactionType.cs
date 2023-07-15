using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{

    public partial class HITransactionType : NotifyChangedBase
    {
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                if (_HITTypeID != value)
                {
                    OnHITTypeIDChanging(value);
                    _HITTypeID = value;
                    RaisePropertyChanged("HITTypeID");
                    OnHITTypeIDChanged();
                }
            }
        }
        private Int64 _HITTypeID;
        partial void OnHITTypeIDChanging(Int64 value);
        partial void OnHITTypeIDChanged();



        [StringLength(64, MinimumLength = 0, ErrorMessage = "Tên Phải <= 64 Ký Tự")]
        [DataMemberAttribute()]
        public String HITypeName
        {
            get
            {
                return _HITypeName;
            }
            set
            {
                OnHITypeNameChanging(value);
                ValidateProperty("HITypeName", value);
                _HITypeName = value;
                RaisePropertyChanged("HITypeName");
                OnHITypeNameChanged();
            }
        }
        private String _HITypeName;
        partial void OnHITypeNameChanging(String value);
        partial void OnHITypeNameChanged();
        

        [DataMemberAttribute()]
        public String HITypeDescription
        {
            get
            {
                return _HITypeDescription;
            }
            set
            {
                OnHITypeDescriptionChanging(value);
                _HITypeDescription = value;
                RaisePropertyChanged("HITypeDescription");
                OnHITypeDescriptionChanged();
            }
        }
        private String _HITypeDescription;
        partial void OnHITypeDescriptionChanging(String value);
        partial void OnHITypeDescriptionChanged();

        [DataMemberAttribute()]
        public String HITypeNamNgoaiTru
        {
            get
            {
                return _HITypeNamNgoaiTru;
            }
            set
            {
                OnHITypeNamNgoaiTruChanging(value);
                _HITypeNamNgoaiTru = value;
                RaisePropertyChanged("HITypeNamNgoaiTru");
                OnHITypeNamNgoaiTruChanged();
            }
        }
        private String _HITypeNamNgoaiTru;
        partial void OnHITypeNamNgoaiTruChanging(String value);
        partial void OnHITypeNamNgoaiTruChanged();


        [DataMemberAttribute()]
        public Int32 IdxNgoaiTru
        {
            get
            {
                return _IdxNgoaiTru;
            }
            set
            {
                OnIdxNgoaiTruChanging(value);
                _IdxNgoaiTru = value;
                RaisePropertyChanged("IdxNgoaiTru");
                OnIdxNgoaiTruChanged();
            }
        }
        private Int32 _IdxNgoaiTru;
        partial void OnIdxNgoaiTruChanging(Int32 value);
        partial void OnIdxNgoaiTruChanged();

        [DataMemberAttribute()]
        public String HITypeNameNoiTru
        {
            get
            {
                return _HITypeNameNoiTru;
            }
            set
            {
                OnHITypeNameNoiTruChanging(value);
                _HITypeNameNoiTru = value;
                RaisePropertyChanged("HITypeNameNoiTru");
                OnHITypeNameNoiTruChanged();
            }
        }
        private String _HITypeNameNoiTru;
        partial void OnHITypeNameNoiTruChanging(String value);
        partial void OnHITypeNameNoiTruChanged();


        [DataMemberAttribute()]
        public Int32 IdxNoiTru
        {
            get
            {
                return _IdxNoiTru;
            }
            set
            {
                OnIdxNoiTruChanging(value);
                _IdxNoiTru = value;
                RaisePropertyChanged("IdxNoiTru");
                OnIdxNoiTruChanged();
            }
        }
        private Int32 _IdxNoiTru;
        partial void OnIdxNoiTruChanging(Int32 value);
        partial void OnIdxNoiTruChanged();

        
        [DataMemberAttribute()]
        public String NameByTemp21
        {
            get
            {
                return _NameByTemp21;
            }
            set
            {
                OnNameByTemp21Changing(value);
                _NameByTemp21 = value;
                RaisePropertyChanged("NameByTemp21");
                OnNameByTemp21Changed();
            }
        }
        private String _NameByTemp21;
        partial void OnNameByTemp21Changing(String value);
        partial void OnNameByTemp21Changed();


        [DataMemberAttribute()]
        public String IdxByTemp21
        {
            get
            {
                return _IdxByTemp21;
            }
            set
            {
                OnIdxByTemp21Changing(value);
                _IdxByTemp21 = value;
                RaisePropertyChanged("IdxByTemp21");
                OnIdxByTemp21Changed();
            }
        }
        private String _IdxByTemp21;
        partial void OnIdxByTemp21Changing(String value);
        partial void OnIdxByTemp21Changed();

        [DataMemberAttribute()]
        public Nullable<Int64> ParentID
        {
            get
            {
                return _ParentID;
            }
            set
            {
                OnParentIDChanging(value);
                _ParentID = value;
                RaisePropertyChanged("ParentID");
                OnParentIDChanged();
            }
        }
        private Nullable<Int64> _ParentID;
        partial void OnParentIDChanging(Nullable<Int64> value);
        partial void OnParentIDChanged();

        [DataMemberAttribute()]
        public bool IsShowOnDrugConfig
        {
            get
            {
                return _IsShowOnDrugConfig;
            }
            set
            {
                _IsShowOnDrugConfig = value;
                RaisePropertyChanged("IsShowOnDrugConfig");
            }
        }
        private bool _IsShowOnDrugConfig;
        [DataMemberAttribute()]
        public string IdxTemp12Name
        {
            get
            {
                return _IdxTemp12Name;
            }
            set
            {
                _IdxTemp12Name = value;
                RaisePropertyChanged("IdxTemp12Name");
            }
        }
        private string _IdxTemp12Name;
        #endregion

        #region Navigation Properties
        #endregion
    }
}
