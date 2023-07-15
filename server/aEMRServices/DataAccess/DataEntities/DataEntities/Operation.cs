using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Operation : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Operation object.

        /// <param name="operationID">Initial value of the OperationID property.</param>
        /// <param name="operationName">Initial value of the OperationName property.</param>
        public static Operation CreateOperation(Int32 operationID, String operationName)
        {
            Operation operation = new Operation();
            operation.OperationID = operationID;
            operation.OperationName = operationName;
            return operation;
        }

        #endregion
        #region Primitive Properties


        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected = false;


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
                    OnOperationIDChanging(value);
                    ////ReportPropertyChanging("OperationID");
                    _OperationID = value;
                    RaisePropertyChanged("OperationID");
                    OnOperationIDChanged();
                }
            }
        }
        private Int32 _OperationID;
        partial void OnOperationIDChanging(Int32 value);
        partial void OnOperationIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> FunctionID
        {
            get
            {
                return _FunctionID;
            }
            set
            {
                OnFunctionIDChanging(value);
                ////ReportPropertyChanging("FunctionID");
                _FunctionID = value;
                RaisePropertyChanged("FunctionID");
                OnFunctionIDChanged();
            }
        }
        private Nullable<long> _FunctionID;
        partial void OnFunctionIDChanging(Nullable<long> value);
        partial void OnFunctionIDChanged();





        [DataMemberAttribute()]
        public String OperationName
        {
            get
            {
                return _OperationName;
            }
            set
            {
                OnOperationNameChanging(value);
                ////ReportPropertyChanging("OperationName");
                _OperationName = value;
                RaisePropertyChanged("OperationName");
                OnOperationNameChanged();
            }
        }
        private String _OperationName;
        partial void OnOperationNameChanging(String value);
        partial void OnOperationNameChanged();





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
        public int Enum
        {
            get
            {
                return _Enum;
            }
            set
            {
                OnEnumChanging(value);

                _Enum = value;
                RaisePropertyChanged("Enum");
                OnEnumChanged();
            }
        }
        private int _Enum;
        partial void OnEnumChanging(int value);
        partial void OnEnumChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OPERATIO_REL_UMGMT_FUNCTION", "Functions")]
        public Function Function
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PERMISSI_REL_UMGMT_OPERATIO", "Permission")]
        public ObservableCollection<Permission> Permissions
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Operation info = obj as Operation;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.OperationID > 0 && this.OperationID == info.OperationID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
