using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public abstract class OutstandingItem : NotifyChangedBase
    {
        public OutstandingItem() : base() { }
        public abstract string HeaderText
        {
            get;
            set;
        }

        public abstract object ID
        {
            get;
            set;
        }

        private OutstandingItemState _State = OutstandingItemState.IsNew;
        
        public OutstandingItemState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                RaisePropertyChanged("State");
            }
        }
    }
   
    public enum OutstandingItemState
    {
       
        IsNew = 1,
        
        IsOld = 2,
       
        IsSelected = 3
    }
}
