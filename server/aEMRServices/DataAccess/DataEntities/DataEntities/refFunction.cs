using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class refFunction : NotifyChangedBase, IEditableObject
    {
        public refFunction()
            : base()
        {

        }

        private refFunction _temprefFunction;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _temprefFunction = (refFunction)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _temprefFunction)
                CopyFrom(_temprefFunction);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(refFunction p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method
        public static refFunction CreaterefFunction(ObservableCollection<refOperation> lstOperation)
        {
            refFunction refFunction = new refFunction();
            refFunction.lstOperation = lstOperation;
            return refFunction;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public ObservableCollection<refOperation> lstOperation
        {
            get
            {
                return _lstOperation;
            }
            set
            {
                if (_lstOperation != value)
                {
                    OnlstOperationChanging(value);
                    _lstOperation = value;
                    RaisePropertyChanged("lstOperation");
                    OnlstOperationChanged();
                }
            }
        }
        private ObservableCollection<refOperation> _lstOperation;
        partial void OnlstOperationChanging(ObservableCollection<refOperation> value);
        partial void OnlstOperationChanged();

        [DataMemberAttribute()]
        public Function mFunction
        {
            get
            {
                return _mFunction;
            }
            set
            {
                if (_mFunction != value)
                {
                    OnmFunctionChanging(value);
                    _mFunction = value;
                    RaisePropertyChanged("mFunction");
                    OnmFunctionChanged();
                }
            }
        }
        private Function _mFunction;
        partial void OnmFunctionChanging(Function value);
        partial void OnmFunctionChanged();
        
        #endregion
        

    }
}
