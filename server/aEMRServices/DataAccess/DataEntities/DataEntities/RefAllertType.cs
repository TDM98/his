using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefAllertType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefAllertType object.

        /// <param name="alertTypeID">Initial value of the AlertTypeID property.</param>
        /// <param name="alertTypeName">Initial value of the AlertTypeName property.</param>
        public static RefAllertType CreateRefAllertType(Int32 alertTypeID, String alertTypeName)
        {
            RefAllertType refAllertType = new RefAllertType();
            refAllertType.AlertTypeID = alertTypeID;
            refAllertType.AlertTypeName = alertTypeName;
            return refAllertType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 AlertTypeID
        {
            get
            {
                return _AlertTypeID;
            }
            set
            {
                if (_AlertTypeID != value)
                {
                    OnAlertTypeIDChanging(value);
                    ////ReportPropertyChanging("AlertTypeID");
                    _AlertTypeID = value;
                    RaisePropertyChanged("AlertTypeID");
                    OnAlertTypeIDChanged();
                }
            }
        }
        private Int32 _AlertTypeID;
        partial void OnAlertTypeIDChanging(Int32 value);
        partial void OnAlertTypeIDChanged();





        [DataMemberAttribute()]
        public String AlertTypeName
        {
            get
            {
                return _AlertTypeName;
            }
            set
            {
                OnAlertTypeNameChanging(value);
                ////ReportPropertyChanging("AlertTypeName");
                _AlertTypeName = value;
                RaisePropertyChanged("AlertTypeName");
                OnAlertTypeNameChanged();
            }
        }
        private String _AlertTypeName;
        partial void OnAlertTypeNameChanging(String value);
        partial void OnAlertTypeNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_ALERT_REL_PTAPP_REFALLER", "Alert")]
        public ObservableCollection<Alert> Alerts
        {
            get;
            set;
        }

        #endregion
    }
}
