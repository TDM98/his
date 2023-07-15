using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PCLExamGroup : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLExamGroup object.

        /// <param name="pCLExamGroupID">Initial value of the PCLExamGroupID property.</param>
        /// <param name="pCLExamGroupName">Initial value of the PCLExamGroupName property.</param>
        public static PCLExamGroup CreatePCLExamGroup(long pCLExamGroupID, String pCLExamGroupName)
        {
            PCLExamGroup pCLExamGroup = new PCLExamGroup();
            pCLExamGroup.PCLExamGroupID = pCLExamGroupID;
            pCLExamGroup.PCLExamGroupName = pCLExamGroupName;
            return pCLExamGroup;
        }

        #endregion

        #region Primitive Properties





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
        public Nullable<long> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                ////ReportPropertyChanging("DeptID");
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<long> _DeptID;
        partial void OnDeptIDChanging(Nullable<long> value);
        partial void OnDeptIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ParPCLExamGroupID
        {
            get
            {
                return _ParPCLExamGroupID;
            }
            set
            {
                OnParPCLExamGroupIDChanging(value);
                ////ReportPropertyChanging("ParPCLExamGroupID");
                _ParPCLExamGroupID = value;
                RaisePropertyChanged("ParPCLExamGroupID");
                OnParPCLExamGroupIDChanged();
            }
        }
        private Nullable<long> _ParPCLExamGroupID;
        partial void OnParPCLExamGroupIDChanging(Nullable<long> value);
        partial void OnParPCLExamGroupIDChanged();





        [DataMemberAttribute()]
        public String PCLExamGroupName
        {
            get
            {
                return _PCLExamGroupName;
            }
            set
            {
                OnPCLExamGroupNameChanging(value);
                ////ReportPropertyChanging("PCLExamGroupName");
                _PCLExamGroupName = value;
                RaisePropertyChanged("PCLExamGroupName");
                OnPCLExamGroupNameChanged();
            }
        }
        private String _PCLExamGroupName;
        partial void OnPCLExamGroupNameChanging(String value);
        partial void OnPCLExamGroupNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLEXAMG_REL_REQPC_REFDEPAR", "RefDepartments")]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLEXAMT_REL_REQPC_PCLEXAMG", "PCLExamTypes")]
        public ObservableCollection<PCLExamType> PCLExamTypes
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLSECTI_REL_REQPC_PCLEXAMG", "PCLSections")]
        public ObservableCollection<PCLSection> PCLSections
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            PCLExamGroup SelectedExamGroup = obj as PCLExamGroup;
            if (SelectedExamGroup == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamGroupID == SelectedExamGroup.PCLExamGroupID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
