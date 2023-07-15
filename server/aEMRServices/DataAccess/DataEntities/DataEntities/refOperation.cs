using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class refOperation : NotifyChangedBase, IEditableObject
    {
        public refOperation()
            : base()
        {

        }

        private refOperation _temprefOperation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _temprefOperation = (refOperation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _temprefOperation)
                CopyFrom(_temprefOperation);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(refOperation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method
        public static refOperation CreaterefOperation(ObservableCollection<Permission> lstPermission)
        {
            refOperation refOperation = new refOperation();
            refOperation.lstPermission = lstPermission;            
            return refOperation;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public ObservableCollection<Permission> lstPermission
        {
            get
            {
                return _lstPermission;
            }
            set
            {
                if (_lstPermission != value)
                {
                    OnlstPermissionChanging(value);
                    _lstPermission = value;
                    RaisePropertyChanged("lstPermission");
                    OnlstPermissionChanged();
                }
            }
        }
        private ObservableCollection<Permission> _lstPermission;
        partial void OnlstPermissionChanging(ObservableCollection<Permission> value);
        partial void OnlstPermissionChanged();

        [DataMemberAttribute()]
        public Operation mOperation
        {
            get
            {
                return _mOperation;
            }
            set
            {
                if (_mOperation != value)
                {
                    OnmOperationChanging(value);
                    _mOperation = value;
                    RaisePropertyChanged("mOperation");
                    OnmOperationChanged();
                }
            }
        }
        private Operation _mOperation;
        partial void OnmOperationChanging(Operation value);
        partial void OnmOperationChanged();

        [DataMemberAttribute()]
        public Permission mPermission
        {
            get
            {
                return _mPermission;
            }
            set
            {
                if (_mPermission != value)
                {
                    OnmPermissionChanging(value);
                    _mPermission = value;
                    RaisePropertyChanged("mPermission");
                    OnmPermissionChanged();
                }
            }
        }
        private Permission _mPermission;
        partial void OnmPermissionChanging(Permission value);
        partial void OnmPermissionChanged();

        #endregion

    }
}
