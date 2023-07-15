using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Function : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Function object.

        /// <param name="functionID">Initial value of the FunctionID property.</param>
        /// <param name="functionName">Initial value of the FunctionName property.</param>
        public static Function CreateFunction(long functionID, String functionName)
        {
            Function function = new Function();
            function.FunctionID = functionID;
            function.FunctionName = functionName;
            return function;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long FunctionID
        {
            get
            {
                return _FunctionID;
            }
            set
            {
                if (_FunctionID != value)
                {
                    OnFunctionIDChanging(value);
                    ////ReportPropertyChanging("FunctionID");
                    _FunctionID = value;
                    RaisePropertyChanged("FunctionID");
                    OnFunctionIDChanged();
                }
            }
        }
        private long _FunctionID;
        partial void OnFunctionIDChanging(long value);
        partial void OnFunctionIDChanged();


        [DataMemberAttribute()]
        public Int32 eNum
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
        private Int32 _eNum;
        partial void OneNumChanging(Int32 value);
        partial void OneNumChanged();


        [DataMemberAttribute()]
        public Nullable<Int32> ModuleID
        {
            get
            {
                return _ModuleID;
            }
            set
            {
                OnModuleIDChanging(value);
                ////ReportPropertyChanging("ModuleID");
                _ModuleID = value;
                RaisePropertyChanged("ModuleID");
                OnModuleIDChanged();
            }
        }
        private Nullable<Int32> _ModuleID;
        partial void OnModuleIDChanging(Nullable<Int32> value);
        partial void OnModuleIDChanged();





        [DataMemberAttribute()]
        public String FunctionName
        {
            get
            {
                return _FunctionName;
            }
            set
            {
                OnFunctionNameChanging(value);
                ////ReportPropertyChanging("FunctionName");
                _FunctionName = value;
                RaisePropertyChanged("FunctionName");
                OnFunctionNameChanged();
            }
        }
        private String _FunctionName;
        partial void OnFunctionNameChanging(String value);
        partial void OnFunctionNameChanged();





        [DataMemberAttribute()]
        public String FunctionDescription
        {
            get
            {
                return _FunctionDescription;
            }
            set
            {
                OnFunctionDescriptionChanging(value);
                ////ReportPropertyChanging("FunctionDescription");
                _FunctionDescription = value;
                RaisePropertyChanged("FunctionDescription");
                OnFunctionDescriptionChanged();
            }
        }
        private String _FunctionDescription;
        partial void OnFunctionDescriptionChanging(String value);
        partial void OnFunctionDescriptionChanged();





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
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_FUNCTION_REL_UMGMT_MODULES", "Modules")]
        public Module Module
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OPERATIO_REL_UMGMT_FUNCTION", "Operation")]
        public ObservableCollection<Operation> Operations
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Function info = obj as Function;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.FunctionID > 0 && this.FunctionID == info.FunctionID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
