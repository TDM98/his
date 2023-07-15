using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
// 20180921 TNHX: Add class for Modules Tree
namespace DataEntities
{
    public partial class Um_ModuleFunctionOperations : NotifyChangedBase
    {
        public Um_ModuleFunctionOperations()
        {
        }

        private Int32 _ModuleID;

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
                    _ModuleID = value;
                    RaisePropertyChanged("ModuleID");
                }
            }
        }

        private int _eNumModule;

        [DataMemberAttribute()]
        public int eNumModule
        {
            get
            {
                return _eNumModule;
            }
            set
            {
                if (_eNumModule != value)
                {
                    _eNumModule = value;
                    RaisePropertyChanged("eNumModule");
                }
            }
        }

        private String _ModuleName;

        [DataMemberAttribute()]
        public String ModuleName
        {
            get
            {
                return _ModuleName;
            }
            set
            {
                _ModuleName = value;
                RaisePropertyChanged("ModuleName");
            }
        }

        private String _ModuleDescription;

        [DataMemberAttribute()]
        public String ModuleDescription
        {
            get
            {
                return _ModuleDescription;
            }
            set
            {
                _ModuleDescription = value;
                RaisePropertyChanged("ModuleDescription");
            }
        }

        private long _FunctionID;

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
                    _FunctionID = value;
                    RaisePropertyChanged("FunctionID");
                }
            }
        }

        private Int32 _eNumFunction;

        [DataMemberAttribute()]
        public Int32 eNumFunction
        {
            get
            {
                return _eNumFunction;
            }
            set
            {
                if (_eNumFunction != value)
                {
                    _eNumFunction = value;
                    RaisePropertyChanged("eNumFunction");
                }
            }
        }

        private String _FunctionName;

        [DataMemberAttribute()]
        public String FunctionName
        {
            get
            {
                return _FunctionName;
            }
            set
            {
                _FunctionName = value;
                RaisePropertyChanged("FunctionName");
            }
        }

        private String _FunctionDescription;

        [DataMemberAttribute()]
        public String FunctionDescription
        {
            get
            {
                return _FunctionDescription;
            }
            set
            {
                _FunctionDescription = value;
                RaisePropertyChanged("FunctionDescription");
            }
        }

        private Int32 _OperationID;

        [DataMemberAttribute()]
        public Int32 OperationID
        {
            get
            {
                return _OperationID;
            }
            set
            {
                if (_OperationID != value)
                {
                    _OperationID = value;
                    RaisePropertyChanged("OperationID");
                }
            }
        }

        private String _OperationName;

        [DataMemberAttribute()]
        public String OperationName
        {
            get
            {
                return _OperationName;
            }
            set
            {
                _OperationName = value;
                RaisePropertyChanged("OperationName");
            }
        }

        private String _OperationDescription;

        [DataMemberAttribute()]
        public String OperationDescription
        {
            get
            {
                return _OperationDescription;
            }
            set
            {
                _OperationDescription = value;
                RaisePropertyChanged("OperationDescription");
            }
        }

        private int _EnumOperation;

        [DataMemberAttribute()]
        public int EnumOperation
        {
            get
            {
                return _EnumOperation;
            }
            set
            {
                _EnumOperation = value;
                RaisePropertyChanged("EnumOperation");
            }
        }
    }
}
