using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public class ActivityClasses : NotifyChangedBase
    {
        public Nullable<long> ActivityClassID
        {
            get
            {
                return _ActivityClassID;
            }
            set
            {
                _ActivityClassID = value;
                RaisePropertyChanged("ActivityClassID");
            }
        }
        private Nullable<long> _ActivityClassID;

        [DataMemberAttribute()]
        public string ActivityClassName
        {
            get
            {
                return _ActivityClassName;
            }
            set
            {
                _ActivityClassName = value;
                RaisePropertyChanged("ActivityClassName");
            }
        }
        private string _ActivityClassName;

        public Nullable<float> TotalMonth
        {
            get
            {
                return _TotalMonth;
            }
            set
            {
                _TotalMonth = value;
                RaisePropertyChanged("TotalMonth");
            }
        }
        private Nullable<float> _TotalMonth;

        public Nullable<long> V_ActivityClassType
        {
            get
            {
                return _V_ActivityClassType;
            }
            set
            {
                _V_ActivityClassType = value;
                RaisePropertyChanged("V_ActivityClassType");
            }
        }
        private Nullable<long> _V_ActivityClassType;


    }
}
