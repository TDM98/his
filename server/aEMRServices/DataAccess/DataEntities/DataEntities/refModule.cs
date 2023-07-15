using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class refModule : NotifyChangedBase, IEditableObject
    {
        public refModule()
            : base()
        {

        }

        private refModule _temprefModule;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _temprefModule = (refModule)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _temprefModule)
                CopyFrom(_temprefModule);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(refModule p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method
        public static refModule CreaterefModule(ObservableCollection<refFunction> lstFunction)
        {
            refModule refModule = new refModule();
            refModule.lstFunction = lstFunction;
            return refModule;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public ObservableCollection<refFunction> lstFunction
        {
            get
            {
                return _lstFunction;
            }
            set
            {
                if (_lstFunction != value)
                {
                    OnlstFunctionChanging(value);
                    _lstFunction = value;
                    RaisePropertyChanged("lstFunction");
                    OnlstFunctionChanged();
                }
            }
        }
        private ObservableCollection<refFunction> _lstFunction;
        partial void OnlstFunctionChanging(ObservableCollection<refFunction> value);
        partial void OnlstFunctionChanged();

        [DataMemberAttribute()]
        public Module mModule
        {
            get
            {
                return _mModule;
            }
            set
            {
                if (_mModule != value)
                {
                    OnmModuleChanging(value);
                    _mModule = value;
                    RaisePropertyChanged("mModule");
                    OnmModuleChanged();
                }
            }
        }
        private Module _mModule;
        partial void OnmModuleChanging(Module value);
        partial void OnmModuleChanged();
        #endregion

    }
}
