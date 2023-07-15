using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Module : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Module object.

        /// <param name="moduleID">Initial value of the ModuleID property.</param>
        /// <param name="moduleName">Initial value of the ModuleName property.</param>
        public static Module CreateModule(Int32 moduleID, String moduleName)
        {
            Module module = new Module();
            module.ModuleID = moduleID;
            module.ModuleName = moduleName;
            return module;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 ModuleID
        {
            get
            {
                return _ModuleID;
            }
            set
            {
                if (_ModuleID != value)
                {
                    OnModuleIDChanging(value);
                    ////ReportPropertyChanging("ModuleID");
                    _ModuleID = value;
                    RaisePropertyChanged("ModuleID");
                    OnModuleIDChanged();
                }
            }
        }
        private Int32 _ModuleID;
        partial void OnModuleIDChanging(Int32 value);
        partial void OnModuleIDChanged();

        [DataMemberAttribute()]
        public int eNum
        {
            get
            {
                return _eNum;
            }
            set
            {
                if (_eNum != value)
                {
                    OneNumChanging(value);
                    ////ReportPropertyChanging("eNum");
                    _eNum = value;
                    RaisePropertyChanged("eNum");
                    OneNumChanged();
                }
            }
        }
        private int _eNum;
        partial void OneNumChanging(int value);
        partial void OneNumChanged();


        [DataMemberAttribute()]
        public String ModuleName
        {
            get
            {
                return _ModuleName;
            }
            set
            {
                OnModuleNameChanging(value);
                ////ReportPropertyChanging("ModuleName");
                _ModuleName = value;
                RaisePropertyChanged("ModuleName");
                OnModuleNameChanged();
            }
        }
        private String _ModuleName;
        partial void OnModuleNameChanging(String value);
        partial void OnModuleNameChanged();





        [DataMemberAttribute()]
        public String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ////ReportPropertyChanging("Description");
                _Description = value;
                RaisePropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private String _Description;
        partial void OnDescriptionChanging(String value);
        partial void OnDescriptionChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                OnIdxChanging(value);
                ////ReportPropertyChanging("Idx");
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_FUNCTION_REL_UMGMT_MODULES", "Functions")]
        public ObservableCollection<Function> Functions
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Module info = obj as Module;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ModuleID > 0 && this.ModuleID == info.ModuleID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
