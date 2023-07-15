using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class EquipHistory : NotifyChangedBase, IEditableObject
    {
        public EquipHistory()
            : base()
        {

        }

        private EquipHistory _tempEquipHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempEquipHistory = (EquipHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempEquipHistory)
                CopyFrom(_tempEquipHistory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(EquipHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new EquipHistory object.

        /// <param name="equipHisItermID">Initial value of the EquipHisItermID property.</param>
        /// <param name="v_ExamRegStatus">Initial value of the V_ExamRegStatus property.</param>
        public static EquipHistory CreateEquipHistory(long equipHisItermID, Int64 v_ExamRegStatus)
        {
            EquipHistory equipHistory = new EquipHistory();
            equipHistory.EquipHisItermID = equipHisItermID;
            equipHistory.V_ExamRegStatus = v_ExamRegStatus;
            return equipHistory;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long EquipHisItermID
        {
            get
            {
                return _EquipHisItermID;
            }
            set
            {
                if (_EquipHisItermID != value)
                {
                    OnEquipHisItermIDChanging(value);
                    ////ReportPropertyChanging("EquipHisItermID");
                    _EquipHisItermID = value;
                    RaisePropertyChanged("EquipHisItermID");
                    OnEquipHisItermIDChanged();
                }
            }
        }
        private long _EquipHisItermID;
        partial void OnEquipHisItermIDChanging(long value);
        partial void OnEquipHisItermIDChanged();





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
        public Int64 V_ExamRegStatus
        {
            get
            {
                return _V_ExamRegStatus;
            }
            set
            {
                OnV_ExamRegStatusChanging(value);
                ////ReportPropertyChanging("V_ExamRegStatus");
                _V_ExamRegStatus = value;
                RaisePropertyChanged("V_ExamRegStatus");
                OnV_ExamRegStatusChanged();
            }
        }
        private Int64 _V_ExamRegStatus;
        partial void OnV_ExamRegStatusChanging(Int64 value);
        partial void OnV_ExamRegStatusChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> EquipHisModifiedDate
        {
            get
            {
                return _EquipHisModifiedDate;
            }
            set
            {
                OnEquipHisModifiedDateChanging(value);
                ////ReportPropertyChanging("EquipHisModifiedDate");
                _EquipHisModifiedDate = value;
                RaisePropertyChanged("EquipHisModifiedDate");
                OnEquipHisModifiedDateChanged();
            }
        }
        private Nullable<DateTime> _EquipHisModifiedDate;
        partial void OnEquipHisModifiedDateChanging(Nullable<DateTime> value);
        partial void OnEquipHisModifiedDateChanged();





        [DataMemberAttribute()]
        public String ReasonOrNotes
        {
            get
            {
                return _ReasonOrNotes;
            }
            set
            {
                OnReasonOrNotesChanging(value);
                ////ReportPropertyChanging("ReasonOrNotes");
                _ReasonOrNotes = value;
                RaisePropertyChanged("ReasonOrNotes");
                OnReasonOrNotesChanged();
            }
        }
        private String _ReasonOrNotes;
        partial void OnReasonOrNotesChanging(String value);
        partial void OnReasonOrNotesChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EQUIPHIS_REL_RM20_INWARDRE", "InwardResources")]
        public InwardResource InwardResource
        {
            get;
            set;
        }

        #endregion
    }
}
