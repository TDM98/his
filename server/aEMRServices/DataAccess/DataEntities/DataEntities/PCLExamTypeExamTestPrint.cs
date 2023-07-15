using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class PCLExamTypeExamTestPrint : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 PCLExamTypeTestItemID
        {
            get
            {
                return _PCLExamTypeTestItemID;
            }
            set
            {
                if (_PCLExamTypeTestItemID != value)
                {
                    OnPCLExamTypeTestItemIDChanging(value);
                    _PCLExamTypeTestItemID = value;
                    RaisePropertyChanged("PCLExamTypeTestItemID");
                    OnPCLExamTypeTestItemIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeTestItemID;
        partial void OnPCLExamTypeTestItemIDChanging(Int64 value);
        partial void OnPCLExamTypeTestItemIDChanged();

        [DataMemberAttribute()]
        public Int64 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    OnIDChanging(value);
                    _ID = value;
                    RaisePropertyChanged("ID");
                    OnIDChanged();
                }
            }
        }
        private Int64 _ID;
        partial void OnIDChanging(Int64 value);
        partial void OnIDChanged();

        [Required(ErrorMessage = "Nhập Mã!")]
        [DataMemberAttribute()]
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                OnCodeChanging(value);
                ValidateProperty("Code", value);
                _Code = value;
                RaisePropertyChanged("Code");
                OnCodeChanged();
            }
        }
        private String _Code;
        partial void OnCodeChanging(String value);
        partial void OnCodeChanged();


        [DataMemberAttribute()]
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                _Name = value;
                RaisePropertyChanged("Name");
                OnNameChanged();
            }
        }
        private String _Name;
        partial void OnNameChanging(String value);
        partial void OnNameChanged();

        
        [DataMemberAttribute()]
        public Boolean IsBold
        {
            get
            {
                return _IsBold;
            }
            set
            {
                OnIsBoldChanging(value);
                _IsBold = value;
                RaisePropertyChanged("IsBold");
                OnIsBoldChanged();
            }
        }
        private Boolean _IsBold;
        partial void OnIsBoldChanging(Boolean value);
        partial void OnIsBoldChanged();


        [DataMemberAttribute()]
        public Int32 Indent
        {
            get
            {
                return _Indent;
            }
            set
            {
                OnIndentChanging(value);
                _Indent = value;
                RaisePropertyChanged("Indent");
                OnIndentChanged();
            }
        }
        private Int32 _Indent;
        partial void OnIndentChanging(Int32 value);
        partial void OnIndentChanged();

        [DataMemberAttribute()]
        public Int32 PrintIndex
        {
            get
            {
                return _PrintIndex;
            }
            set
            {
                OnPrintIndexChanging(value);
                _PrintIndex = value;
                RaisePropertyChanged("PrintIndex");
                OnPrintIndexChanged();
            }
        }
        private Int32 _PrintIndex;
        partial void OnPrintIndexChanging(Int32 value);
        partial void OnPrintIndexChanged();


        [DataMemberAttribute()]
        public Boolean IsDisplay
        {
            get
            {
                return _IsDisplay;
            }
            set
            {
                OnIsDisplayChanging(value);
                _IsDisplay = value;
                RaisePropertyChanged("IsDisplay");
                OnIsDisplayChanged();
            }
        }
        private Boolean _IsDisplay;
        partial void OnIsDisplayChanging(Boolean value);
        partial void OnIsDisplayChanged();


        [DataMemberAttribute()]
        public Boolean IsNoNeedResult
        {
            get
            {
                return _IsNoNeedResult;
            }
            set
            {
                OnIsNoNeedResultChanging(value);
                _IsNoNeedResult = value;
                RaisePropertyChanged("IsNoNeedResult");
                OnIsNoNeedResultChanged();
            }
        }
        private Boolean _IsNoNeedResult;
        partial void OnIsNoNeedResultChanging(Boolean value);
        partial void OnIsNoNeedResultChanged();


        [DataMemberAttribute()]
        public Boolean IsPCLExamType
        {
            get
            {
                return _IsPCLExamType;
            }
            set
            {
                OnIsPCLExamTypeChanging(value);
                _IsPCLExamType = value;
                RaisePropertyChanged("IsPCLExamType");
                OnIsPCLExamTypeChanged();
            }
        }
        private Boolean _IsPCLExamType;
        partial void OnIsPCLExamTypeChanging(Boolean value);
        partial void OnIsPCLExamTypeChanged();


        [DataMemberAttribute()]
        public Boolean IsPCLExamTest
        {
            get
            {
                return _IsPCLExamTest;
            }
            set
            {
                OnIsPCLExamTestChanging(value);
                _IsPCLExamTest = value;
                RaisePropertyChanged("IsPCLExamTest");
                OnIsPCLExamTestChanged();
            }
        }
        private Boolean _IsPCLExamTest;
        partial void OnIsPCLExamTestChanging(Boolean value);
        partial void OnIsPCLExamTestChanged();



        public override bool Equals(object obj)
        {
            PCLExamTypeExamTestPrint SelectedItem = obj as PCLExamTypeExamTestPrint;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.IsPCLExamType)
            {
                return this.ID > 0 && this.IsPCLExamType == SelectedItem.IsPCLExamType;
            }

            if (this.IsPCLExamTest)
            {
                return this.ID > 0 && this.IsPCLExamTest == SelectedItem.IsPCLExamTest;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
