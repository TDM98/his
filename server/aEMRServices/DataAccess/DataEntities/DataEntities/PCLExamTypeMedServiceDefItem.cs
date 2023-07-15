using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
namespace DataEntities
{
    [DataContract]
    public partial class PCLExamTypeMedServiceDefItem : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new PCLExamTypeMedServiceDefItem object.
     
        /// <param name="pCLExamTypeMedServDefID">Initial value of the PCLExamTypeMedServDefID property.</param>
        public static PCLExamTypeMedServiceDefItem CreatePCLExamTypeMedServiceDefItem(Int64 pCLExamTypeMedServDefID)
        {
            PCLExamTypeMedServiceDefItem pCLExamTypeMedServiceDefItem = new PCLExamTypeMedServiceDefItem();
            pCLExamTypeMedServiceDefItem.PCLExamTypeMedServDefID = pCLExamTypeMedServDefID;
            return pCLExamTypeMedServiceDefItem;
        }

        #endregion
        #region Primitive Properties
       
        [DataMemberAttribute()]
        public Int64 PCLExamTypeMedServDefID
        {
            get
            {
                return _PCLExamTypeMedServDefID;
            }
            set
            {
                if (_PCLExamTypeMedServDefID != value)
                {
                    OnPCLExamTypeMedServDefIDChanging(value);
                    _PCLExamTypeMedServDefID =value;
                    RaisePropertyChanged("PCLExamTypeMedServDefID");
                    OnPCLExamTypeMedServDefIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeMedServDefID;
        partial void OnPCLExamTypeMedServDefIDChanging(Int64 value);
        partial void OnPCLExamTypeMedServDefIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID =value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Nullable<Int64> _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Nullable<Int64> value);
        partial void OnPCLExamTypeIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID =value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<Int64> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<Int64> value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate =value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _RecCreatedDate;
        partial void OnRecCreatedDateChanging(Nullable<DateTime> value);
        partial void OnRecCreatedDateChanged();

     
        [DataMemberAttribute()]
        public Nullable<DateTime> ActiveFromDate
        {
            get
            {
                return _ActiveFromDate;
            }
            set
            {
                OnActiveFromDateChanging(value);
                _ActiveFromDate =value;
                RaisePropertyChanged("ActiveFromDate");
                OnActiveFromDateChanged();
            }
        }
        private Nullable<DateTime> _ActiveFromDate;
        partial void OnActiveFromDateChanging(Nullable<DateTime> value);
        partial void OnActiveFromDateChanged();

       
        [DataMemberAttribute()]
        public Nullable<DateTime> ActiveToDate
        {
            get
            {
                return _ActiveToDate;
            }
            set
            {
                OnActiveToDateChanging(value);
                _ActiveToDate =value;
                RaisePropertyChanged("ActiveToDate");
                OnActiveToDateChanged();
            }
        }
        private Nullable<DateTime> _ActiveToDate;
        partial void OnActiveToDateChanging(Nullable<DateTime> value);
        partial void OnActiveToDateChanged();

      
        [DataMemberAttribute()]
        public Nullable<Boolean> IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive =value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Nullable<Boolean> _IsActive;
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();

        #endregion

        #region Navigate
        [DataMemberAttribute()]        
        public PCLExamType ObjPCLExamType
        {
            get 
            { 
                return _ObjPCLExamType; 
            }
            set 
            {
                OnObjPCLExamTypeChanging(value);
                _ObjPCLExamType = value;
                RaisePropertyChanged("ObjPCLExamType");
                OnObjPCLExamTypeChanged();
            }
        }
        private PCLExamType _ObjPCLExamType;
        partial void OnObjPCLExamTypeChanging(PCLExamType value);
        partial void OnObjPCLExamTypeChanged();
        #endregion

    }
}
