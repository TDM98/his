using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PCLExamTypeSubCategory : NotifyChangedBase
    {
        #region Primitive Properties
        
        [DataMemberAttribute()]
        public Int64 PCLExamTypeSubCategoryID
        {
            get
            {
                return _PCLExamTypeSubCategoryID;
            }
            set
            {
                if (_PCLExamTypeSubCategoryID != value)
                {
                    OnPCLExamTypeSubCategoryIDChanging(value);
                    _PCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("PCLExamTypeSubCategoryID");
                    OnPCLExamTypeSubCategoryIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeSubCategoryID;
        partial void OnPCLExamTypeSubCategoryIDChanging(Int64 value);
        partial void OnPCLExamTypeSubCategoryIDChanged();

        
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if(_V_PCLMainCategory!=value)
                {
                    OnV_PCLMainCategoryChanging(value);
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                    OnV_PCLMainCategoryChanged();
                }
            }
        }
        private Int64 _V_PCLMainCategory;
        partial void OnV_PCLMainCategoryChanging(Int64 value);
        partial void OnV_PCLMainCategoryChanged();


        
        public Lookup ObjV_PCLMainCategory
        {
            get
            {
                return _ObjV_PCLMainCategory;
            }
            set
            {
                if(_ObjV_PCLMainCategory!=value)
                {
                    OnObjV_PCLMainCategoryChanging(value);
                    _ObjV_PCLMainCategory = value;
                    RaisePropertyChanged("ObjV_PCLMainCategory");
                    OnObjV_PCLMainCategoryChanged();
                }
            }
        }
        private Lookup _ObjV_PCLMainCategory;
        partial void OnObjV_PCLMainCategoryChanging(Lookup value);
        partial void OnObjV_PCLMainCategoryChanged();

        
        [Required(ErrorMessage = "Nhập Tên PCLSubCategoryName!")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Tên PCLSubCategoryName Phải <= 100 Ký Tự")]
        [DataMemberAttribute()]
        public String PCLSubCategoryName
        {
            get
            {
                return _PCLSubCategoryName;
            }
            set
            {
                if (_PCLSubCategoryName != value)
                {
                    OnPCLSubCategoryNameChanging(value);
                    ValidateProperty("PCLSubCategoryName", value);
                   _PCLSubCategoryName = value;
                   RaisePropertyChanged("PCLSubCategoryName");
                    OnPCLSubCategoryNameChanged();
                }
            }
        }
        private String _PCLSubCategoryName;
        partial void OnPCLSubCategoryNameChanging(String value);
        partial void OnPCLSubCategoryNameChanged();


        private String _PCLSubCategoryDescription;
        [DataMemberAttribute()]
        public String PCLSubCategoryDescription
        {
            get
            {
                return _PCLSubCategoryDescription;
            }
            set
            {
                if (_PCLSubCategoryDescription != value)
                {
                    OnPCLSubCategoryDescriptionChanging(value);
                    _PCLSubCategoryDescription = value;
                    RaisePropertyChanged("PCLSubCategoryDescription");
                    OnPCLSubCategoryDescriptionChanged();
                }
            }
        }
        partial void OnPCLSubCategoryDescriptionChanging(String value);
        partial void OnPCLSubCategoryDescriptionChanged();
        private string _SubCategoryCodeToPAC;
        [DataMemberAttribute()]
        public string SubCategoryCodeToPAC
        {
            get
            {
                return _SubCategoryCodeToPAC;
            }
            set
            {
                if (_SubCategoryCodeToPAC != value)
                {
                    OnSubCategoryCodeToPACChanging(value);
                    _SubCategoryCodeToPAC = value;
                    RaisePropertyChanged("SubCategoryCodeToPAC");
                    OnSubCategoryCodeToPACChanged();
                }
            }
        }
        partial void OnSubCategoryCodeToPACChanging(String value);
        partial void OnSubCategoryCodeToPACChanged();
        private bool _IsSendToPAC;
        [DataMemberAttribute()]
        public bool IsSendToPAC
        {
            get
            {
                return _IsSendToPAC;
            }
            set
            {
                if (_IsSendToPAC != value)
                {
                    OnIsSendToPACChanging(value);
                    _IsSendToPAC = value;
                    RaisePropertyChanged("IsSendToPAC");
                    OnIsSendToPACChanged();
                }
            }
        }
        partial void OnIsSendToPACChanging(bool value);
        partial void OnIsSendToPACChanged();
        #endregion

        public override bool Equals(object obj)
        {
            var selectedExamGroup = obj as PCLExamTypeSubCategory;
            if (selectedExamGroup == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeSubCategoryID > 0 && this.PCLExamTypeSubCategoryID == selectedExamGroup.PCLExamTypeSubCategoryID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
