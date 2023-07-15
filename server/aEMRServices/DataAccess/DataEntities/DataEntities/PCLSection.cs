using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20230519 #001 DatTB: Thêm cột tên tiếng anh các danh mục xét nghiệm 
*/
namespace DataEntities
{
    public partial class PCLSection : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLSection object.

        /// <param name="pCLSectionID">Initial value of the PCLSectionID property.</param>
        /// <param name="pCLFormID">Initial value of the PCLFormID property.</param>
        /// <param name="pCLExamGroupID">Initial value of the PCLExamGroupID property.</param>
        public static PCLSection CreatePCLSection(long pCLSectionID, long pCLFormID, long pCLExamGroupID)
        {
            PCLSection pCLSection = new PCLSection();
            pCLSection.PCLSectionID = pCLSectionID;
            pCLSection.PCLFormID = pCLFormID;
            pCLSection.PCLExamGroupID = pCLExamGroupID;
            return pCLSection;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PCLSectionID
        {
            get
            {
                return _PCLSectionID;
            }
            set
            {
                if (_PCLSectionID != value)
                {
                    OnPCLSectionIDChanging(value);
                    ////ReportPropertyChanging("PCLSectionID");
                    _PCLSectionID = value;
                    RaisePropertyChanged("PCLSectionID");
                    OnPCLSectionIDChanged();
                }
            }
        }
        private long _PCLSectionID;
        partial void OnPCLSectionIDChanging(long value);
        partial void OnPCLSectionIDChanged();





        [DataMemberAttribute()]
        public long PCLFormID
        {
            get
            {
                return _PCLFormID;
            }
            set
            {
                if (_PCLFormID != value)
                {
                    OnPCLFormIDChanging(value);
                    ////ReportPropertyChanging("PCLFormID");
                    _PCLFormID = value;
                    RaisePropertyChanged("PCLFormID");
                    OnPCLFormIDChanged();
                }
            }
        }
        private long _PCLFormID;
        partial void OnPCLFormIDChanging(long value);
        partial void OnPCLFormIDChanged();





        [DataMemberAttribute()]
        public long PCLExamGroupID
        {
            get
            {
                return _PCLExamGroupID;
            }
            set
            {
                if (_PCLExamGroupID != value)
                {
                    OnPCLExamGroupIDChanging(value);
                    ////ReportPropertyChanging("PCLExamGroupID");
                    _PCLExamGroupID = value;
                    RaisePropertyChanged("PCLExamGroupID");
                    OnPCLExamGroupIDChanged();
                }
            }
        }
        private long _PCLExamGroupID;
        partial void OnPCLExamGroupIDChanging(long value);
        partial void OnPCLExamGroupIDChanged();

        
        [DataMemberAttribute()]
        public String PCLSectionCode
        {
            get
            {
                return _PCLSectionCode;
            }
            set
            {
                if (_PCLSectionCode != value)
                {
                    OnPCLSectionCodeChanging(value);
                    _PCLSectionCode = value;
                    RaisePropertyChanged("PCLSectionCode");
                    OnPCLSectionCodeChanged();
                }
            }
        }
        private String _PCLSectionCode;
        partial void OnPCLSectionCodeChanging(String value);
        partial void OnPCLSectionCodeChanged();


        [Required(ErrorMessage = "Nhập Tên PCLSection!")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Tên PCLSection Phải <= 128 Ký Tự")]
        [DataMemberAttribute()]
        public String PCLSectionName
        {
            get
            {
                return _PCLSectionName;
            }
            set
            {
                if (_PCLSectionName != value)
                {
                    OnPCLSectionNameChanging(value);
                    ValidateProperty("PCLSectionName", value);
                    _PCLSectionName = value;
                    RaisePropertyChanged("PCLSectionName");
                    OnPCLSectionNameChanged();
                }
            }
        }
        private String _PCLSectionName;
        partial void OnPCLSectionNameChanging(String value);
        partial void OnPCLSectionNameChanged();

        //▼==== #001
        [DataMemberAttribute()]
        public String PCLSectionNameEng
        {
            get
            {
                return _PCLSectionNameEng;
            }
            set
            {
                if (_PCLSectionNameEng != value)
                {
                    OnPCLSectionNameEngChanging(value);
                    ValidateProperty("PCLSectionNameEng", value);
                    _PCLSectionNameEng = value;
                    RaisePropertyChanged("PCLSectionNameEng");
                    OnPCLSectionNameEngChanged();
                }
            }
        }
        private String _PCLSectionNameEng;
        partial void OnPCLSectionNameEngChanging(String value);
        partial void OnPCLSectionNameEngChanged();
        //▲==== #001


        //[DataMemberAttribute()]
        //public long DeptID
        //{
        //    get
        //    {
        //        return _DeptID;
        //    }
        //    set
        //    {
        //        if (_DeptID != value)
        //        {
        //            OnDeptIDChanging(value);
        //            _DeptID = value;
        //            RaisePropertyChanged("DeptID");
        //            OnDeptIDChanged();
        //        }
        //    }
        //}
        //private long _DeptID;
        //partial void OnDeptIDChanging(long value);
        //partial void OnDeptIDChanged();


        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLSECTI_REL_REQPC_PCLEXAMG", "PCLExamGroup")]
        public PCLExamGroup PCLExamGroup
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLSECTI_REL_REQPC_PCLFORMS", "PCLForms")]
        public PCLForm PCLForm
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLITEMS_REL_REQPC_PCLSECTI", "PCLItems")]
        public ObservableCollection<PCLItem> PCLItems
        {
            get;
            set;
        }

        #endregion

        public override string ToString()
        {
            return this.PCLSectionName;
        }
        public override bool Equals(object obj)
        {
            PCLSection info = obj as PCLSection;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLSectionID == info.PCLSectionID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
