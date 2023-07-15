using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PCLResultParamImplementations : NotifyChangedBase
    {

        [DataMemberAttribute()]
        public Int64 PCLResultParamImpID
        {
            get
            {
                return _PCLResultParamImpID;
            }
            set
            {
                if (_PCLResultParamImpID != value)
                {
                    OnPCLResultParamImpIDChanging(value);
                    _PCLResultParamImpID = value;
                    RaisePropertyChanged("PCLResultParamImpID");
                    OnPCLResultParamImpIDChanged();
                }
            }
        }
        private long _PCLResultParamImpID;
        partial void OnPCLResultParamImpIDChanging(long value);
        partial void OnPCLResultParamImpIDChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Nhập Tên ParamName!")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Tên ParamName Phải <= 100 Ký Tự")]
        public String ParamName
        {
            get
            {
                return _ParamName;
            }
            set
            {
                OnParamNameChanging(value);
                ValidateProperty("ParamName", value);
                _ParamName = value;
                RaisePropertyChanged("ParamName");
                OnParamNameChanged();
            }
        }
        private String _ParamName;
        partial void OnParamNameChanging(String value);
        partial void OnParamNameChanged();


        [DataMemberAttribute()]
        public int ParamEnum
        {
            get
            {
                return _ParamEnum;
            }
            set
            {
                if (_ParamEnum != value)
                {
                    OnParamEnumChanging(value);
                    _ParamEnum = value;
                    RaisePropertyChanged("ParamEnum");
                    OnParamEnumChanged();
                }
            }
        }
        private int _ParamEnum;
        partial void OnParamEnumChanging(int value);
        partial void OnParamEnumChanged();
        

        [DataMemberAttribute()]
        public Boolean ReleaseEnabled
        {
            get
            {
                return _ReleaseEnabled;
            }
            set
            {
                if (_ReleaseEnabled != value)
                {
                    OnReleaseEnabledChanging(value);
                    _ReleaseEnabled = value;
                    RaisePropertyChanged("ReleaseEnabled");
                    OnReleaseEnabledChanged();
                }
            }
        }
        private Boolean _ReleaseEnabled;
        partial void OnReleaseEnabledChanging(Boolean value);
        partial void OnReleaseEnabledChanged();


        [DataMemberAttribute()]
        public long? PCLExamTypeSubCategoryID
        {
            get
            {
                return _PCLExamTypeSubCategoryID;
            }
            set
            {
                if (_PCLExamTypeSubCategoryID != value)
                {
                    _PCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("PCLExamTypeSubCategoryID");
                }
            }
        }
        private long? _PCLExamTypeSubCategoryID;

        public override bool Equals(object obj)
        {
            PCLResultParamImplementations SelectedItem= obj as PCLResultParamImplementations;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLResultParamImpID == SelectedItem.PCLResultParamImpID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
