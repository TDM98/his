using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PCLGroup : NotifyChangedBase
    {
        #region Primitive Properties

        private long _pclGroupID;

        [DataMemberAttribute()]
        public long PCLGroupID
        {
            get
            {
                return _pclGroupID;
            }
            set
            {
                if (_pclGroupID != value)
                {
                    _pclGroupID = value;
                    RaisePropertyChanged("PCLGroupID");
                }
            }
        }

        

        [Required(ErrorMessage = "Nhập Tên PCLSection!")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Tên PCLSection Phải <= 100 Ký Tự")]
        [DataMemberAttribute()]
        public String PCLGroupName
        {
            get
            {
                return _pclGroupName;
            }
            set
            {
                if (_pclGroupName != value)
                {
                    OnPCLGroupNameChanging(value);
                    ValidateProperty("PCLGroupName", value);
                   _pclGroupName = value;
                    RaisePropertyChanged("PCLGroupName");
                    OnPCLGroupNameChanged();
                }
            }
        }
        private String _pclGroupName;
        partial void OnPCLGroupNameChanging(String value);
        partial void OnPCLGroupNameChanged();


        private String _description;
        [DataMemberAttribute()]
        public String Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        private long _v_PCLCategory;

        [DataMemberAttribute()]
        public long V_PCLCategory
        {
            get
            {
                return _v_PCLCategory;
            }
            set
            {
                if (_v_PCLCategory != value)
                {
                    _v_PCLCategory = value;
                    RaisePropertyChanged("V_PCLCategory");
                }
            }
        }
        #endregion


        #region Navigate
        [DataMemberAttribute()]
        public Lookup ObjV_PCLCategory
        {
            get { return _ObjV_PCLCategory;}
            set
            {
                if(_ObjV_PCLCategory!=value)
                {
                    OnObjV_PCLCategoryChanging(value);
                    _ObjV_PCLCategory = value;
                    RaisePropertyChanged("ObjV_PCLCategory");
                    OnObjV_PCLCategoryChanged();
                }
            }
        }
        private Lookup _ObjV_PCLCategory;
        partial void OnObjV_PCLCategoryChanging(Lookup value);
        partial void OnObjV_PCLCategoryChanged();

        #endregion

        public override bool Equals(object obj)
        {
            var selectedExamGroup = obj as PCLGroup;
            if (selectedExamGroup == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLGroupID > 0 && this.PCLGroupID == selectedExamGroup.PCLGroupID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
