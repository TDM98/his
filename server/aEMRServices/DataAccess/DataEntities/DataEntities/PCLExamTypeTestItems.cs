using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class PCLExamTypeTestItems : NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PCLExamTypeTestItemID
        {
            get
            {
                return _PCLExamTypeTestItemID;
            }
            set
            {
                if (_PCLExamTypeTestItemID != value)
                {
                    OnPCLExamTypeTestItemIDChanging(value);
                    _PCLExamTypeTestItemID = value;
                    RaisePropertyChanged("PCLExamTypeTestItemID");
                    OnPCLExamTypeTestItemIDChanged();
                }
            }
        }
        private long _PCLExamTypeTestItemID;
        partial void OnPCLExamTypeTestItemIDChanging(long value);
        partial void OnPCLExamTypeTestItemIDChanged();


        
        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);

                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                _V_PCLExamType=new PCLExamType();
                _V_PCLExamType.PCLExamTypeID = PCLExamTypeID;
                OnPCLExamTypeIDChanged();
            }
        }
        private long _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(long value);
        partial void OnPCLExamTypeIDChanged();


        [DataMemberAttribute()]
        public long PCLExamTestItemID
        {
            get
            {
                return _PCLExamTestItemID;
            }
            set
            {
                OnPCLExamTestItemIDChanging(value);
                _PCLExamTestItemID = value;
                RaisePropertyChanged("PCLExamTestItemID");
                _V_PCLExamTestItem=new PCLExamTestItems();
                _V_PCLExamTestItem.PCLExamTestItemID = PCLExamTestItemID;
                OnPCLExamTestItemIDChanged();
            }
        }
        private long _PCLExamTestItemID;
        partial void OnPCLExamTestItemIDChanging(long value);
        partial void OnPCLExamTestItemIDChanged();




        #endregion

        public override bool Equals(object obj)
        {
            PCLExamTypeTestItems SelectedItem = obj as PCLExamTypeTestItems;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeTestItemID > 0 && this.PCLExamTypeTestItemID == SelectedItem.PCLExamTypeTestItemID;
        }


        [DataMemberAttribute()]
        public PCLExamType V_PCLExamType
        {
            get
            {
                return _V_PCLExamType;
            }
            set
            {
                OnV_PCLExamTypeChanging(value);

                _V_PCLExamType = value;
                RaisePropertyChanged("V_PCLExamType");
                if (value != null)
                {
                    _PCLExamTypeID = V_PCLExamType.PCLExamTypeID;
                }
                OnV_PCLExamTypeChanged();
            }
        }
        private PCLExamType _V_PCLExamType;
        partial void OnV_PCLExamTypeChanging(PCLExamType value);
        partial void OnV_PCLExamTypeChanged();


        [DataMemberAttribute()]
        public PCLExamTestItems V_PCLExamTestItem
        {
            get
            {
                return _V_PCLExamTestItem;
            }
            set
            {
                OnV_PCLExamTestItemChanging(value);
                _V_PCLExamTestItem = value;
                if (V_PCLExamTestItem!=null)
                {
                    _PCLExamTestItemID = V_PCLExamTestItem.PCLExamTestItemID;
                }
                RaisePropertyChanged("V_PCLExamTestItem");
                OnV_PCLExamTestItemChanged();
            }
        }
        private PCLExamTestItems _V_PCLExamTestItem;
        partial void OnV_PCLExamTestItemChanging(PCLExamTestItems value);
        partial void OnV_PCLExamTestItemChanged();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
      
    }
}
