using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ExamMaintenanceHistory : NotifyChangedBase, IEditableObject
    {
        public ExamMaintenanceHistory()
            : base()
        {

        }

        private ExamMaintenanceHistory _tempExamMaintenanceHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempExamMaintenanceHistory = (ExamMaintenanceHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempExamMaintenanceHistory)
                CopyFrom(_tempExamMaintenanceHistory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ExamMaintenanceHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ExamMaintenanceHistory object.

        /// <param name="mECode">Initial value of the MECode property.</param>
        /// <param name="mEDate">Initial value of the MEDate property.</param>
        /// <param name="mECause">Initial value of the MECause property.</param>
        /// <param name="mEResult">Initial value of the MEResult property.</param>
        /// <param name="mEResponsibleItem">Initial value of the MEResponsibleItem property.</param>
        /// <param name="isMaintain">Initial value of the IsMaintain property.</param>
        public static ExamMaintenanceHistory CreateExamMaintenanceHistory(long mECode, DateTime mEDate, String mECause, String mEResult, String mEResponsibleItem, Boolean isMaintain)
        {
            ExamMaintenanceHistory examMaintenanceHistory = new ExamMaintenanceHistory();
            examMaintenanceHistory.MECode = mECode;
            examMaintenanceHistory.MEDate = mEDate;
            examMaintenanceHistory.MECause = mECause;
            examMaintenanceHistory.MEResult = mEResult;
            examMaintenanceHistory.MEResponsibleItem = mEResponsibleItem;
            examMaintenanceHistory.IsMaintain = isMaintain;
            return examMaintenanceHistory;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MECode
        {
            get
            {
                return _MECode;
            }
            set
            {
                if (_MECode != value)
                {
                    OnMECodeChanging(value);
                    ////ReportPropertyChanging("MECode");
                    _MECode = value;
                    RaisePropertyChanged("MECode");
                    OnMECodeChanged();
                }
            }
        }
        private long _MECode;
        partial void OnMECodeChanging(long value);
        partial void OnMECodeChanged();





        [DataMemberAttribute()]
        public Nullable<long> InwardRscrID
        {
            get
            {
                return _InwardRscrID;
            }
            set
            {
                OnInwardRscrIDChanging(value);
                ////ReportPropertyChanging("InwardRscrID");
                _InwardRscrID = value;
                RaisePropertyChanged("InwardRscrID");
                OnInwardRscrIDChanged();
            }
        }
        private Nullable<long> _InwardRscrID;
        partial void OnInwardRscrIDChanging(Nullable<long> value);
        partial void OnInwardRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> TIANumber
        {
            get
            {
                return _TIANumber;
            }
            set
            {
                OnTIANumberChanging(value);
                ////ReportPropertyChanging("TIANumber");
                _TIANumber = value;
                RaisePropertyChanged("TIANumber");
                OnTIANumberChanged();
            }
        }
        private Nullable<long> _TIANumber;
        partial void OnTIANumberChanging(Nullable<long> value);
        partial void OnTIANumberChanged();





        [DataMemberAttribute()]
        public DateTime MEDate
        {
            get
            {
                return _MEDate;
            }
            set
            {
                OnMEDateChanging(value);
                ////ReportPropertyChanging("MEDate");
                _MEDate = value;
                RaisePropertyChanged("MEDate");
                OnMEDateChanged();
            }
        }
        private DateTime _MEDate;
        partial void OnMEDateChanging(DateTime value);
        partial void OnMEDateChanged();





        [DataMemberAttribute()]
        public String MECause
        {
            get
            {
                return _MECause;
            }
            set
            {
                OnMECauseChanging(value);
                ////ReportPropertyChanging("MECause");
                _MECause = value;
                RaisePropertyChanged("MECause");
                OnMECauseChanged();
            }
        }
        private String _MECause;
        partial void OnMECauseChanging(String value);
        partial void OnMECauseChanged();





        [DataMemberAttribute()]
        public String MEResult
        {
            get
            {
                return _MEResult;
            }
            set
            {
                OnMEResultChanging(value);
                ////ReportPropertyChanging("MEResult");
                _MEResult = value;
                RaisePropertyChanged("MEResult");
                OnMEResultChanged();
            }
        }
        private String _MEResult;
        partial void OnMEResultChanging(String value);
        partial void OnMEResultChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> MECost
        {
            get
            {
                return _MECost;
            }
            set
            {
                OnMECostChanging(value);
                ////ReportPropertyChanging("MECost");
                _MECost = value;
                RaisePropertyChanged("MECost");
                OnMECostChanged();
            }
        }
        private Nullable<Decimal> _MECost;
        partial void OnMECostChanging(Nullable<Decimal> value);
        partial void OnMECostChanged();





        [DataMemberAttribute()]
        public String MEResponsibleItem
        {
            get
            {
                return _MEResponsibleItem;
            }
            set
            {
                OnMEResponsibleItemChanging(value);
                ////ReportPropertyChanging("MEResponsibleItem");
                _MEResponsibleItem = value;
                RaisePropertyChanged("MEResponsibleItem");
                OnMEResponsibleItemChanged();
            }
        }
        private String _MEResponsibleItem;
        partial void OnMEResponsibleItemChanging(String value);
        partial void OnMEResponsibleItemChanged();





        [DataMemberAttribute()]
        public Boolean IsMaintain
        {
            get
            {
                return _IsMaintain;
            }
            set
            {
                OnIsMaintainChanging(value);
                ////ReportPropertyChanging("IsMaintain");
                _IsMaintain = value;
                RaisePropertyChanged("IsMaintain");
                OnIsMaintainChanged();
            }
        }
        private Boolean _IsMaintain;
        partial void OnIsMaintainChanging(Boolean value);
        partial void OnIsMaintainChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMMAIN_REL_RM19_INWARDRE", "InwardResources")]
        public InwardResource InwardResource
        {
            get;
            set;
        }








        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMMAIN_REL_RM21_TECHNICA", "TechnicalInspectionAgency")]
        public TechnicalInspectionAgency TechnicalInspectionAgency
        {
            get;
            set;
        }



       

        #endregion
    }
}
