using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HospitalizationHistoryDetail : NotifyChangedBase, IEditableObject
    {
        public HospitalizationHistoryDetail()
            : base()
        {

        }

        private HospitalizationHistoryDetail _tempHospitalizationHistoryDetail;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHospitalizationHistoryDetail = (HospitalizationHistoryDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHospitalizationHistoryDetail)
                CopyFrom(_tempHospitalizationHistoryDetail);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HospitalizationHistoryDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistoryDetail object.

        /// <param name="hosHisDetailID">Initial value of the HosHisDetailID property.</param>
        /// <param name="inDateTime">Initial value of the InDateTime property.</param>
        public static HospitalizationHistoryDetail CreateHospitalizationHistoryDetail(long hosHisDetailID, DateTime inDateTime)
        {
            HospitalizationHistoryDetail hospitalizationHistoryDetail = new HospitalizationHistoryDetail();
            hospitalizationHistoryDetail.HosHisDetailID = hosHisDetailID;
            hospitalizationHistoryDetail.InDateTime = inDateTime;
            return hospitalizationHistoryDetail;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long HosHisDetailID
        {
            get
            {
                return _HosHisDetailID;
            }
            set
            {
                if (_HosHisDetailID != value)
                {
                    OnHosHisDetailIDChanging(value);
                    _HosHisDetailID = value;
                    RaisePropertyChanged("HosHisDetailID");
                    OnHosHisDetailIDChanged();
                }
            }
        }
        private long _HosHisDetailID;
        partial void OnHosHisDetailIDChanging(long value);
        partial void OnHosHisDetailIDChanged();

        [DataMemberAttribute()]
        public String BedLocNumber
        {
            get
            {
                return _BedLocNumber;
            }
            set
            {
                OnBedLocNumberChanging(value);
                _BedLocNumber = value;
                RaisePropertyChanged("BedLocNumber");
                OnBedLocNumberChanged();
            }
        }
        private String _BedLocNumber;
        partial void OnBedLocNumberChanging(String value);
        partial void OnBedLocNumberChanged();

        [DataMemberAttribute()]
        public Nullable<long> HHID
        {
            get
            {
                return _HHID;
            }
            set
            {
                OnHHIDChanging(value);
                _HHID = value;
                RaisePropertyChanged("HHID");
                OnHHIDChanged();
            }
        }
        private Nullable<long> _HHID;
        partial void OnHHIDChanging(Nullable<long> value);
        partial void OnHHIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<long> _DeptID;
        partial void OnDeptIDChanging(Nullable<long> value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public DateTime InDateTime
        {
            get
            {
                return _InDateTime;
            }
            set
            {
                OnInDateTimeChanging(value);
                _InDateTime = value;
                RaisePropertyChanged("InDateTime");
                OnInDateTimeChanged();
            }
        }
        private DateTime _InDateTime;
        partial void OnInDateTimeChanging(DateTime value);
        partial void OnInDateTimeChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> OutDateTime
        {
            get
            {
                return _OutDateTime;
            }
            set
            {
                OnOutDateTimeChanging(value);
                _OutDateTime = value;
                RaisePropertyChanged("OutDateTime");
                OnOutDateTimeChanged();
            }
        }
        private Nullable<DateTime> _OutDateTime;
        partial void OnOutDateTimeChanging(Nullable<DateTime> value);
        partial void OnOutDateTimeChanged();

        [DataMemberAttribute()]
        public String HosHisDetailNotes
        {
            get
            {
                return _HosHisDetailNotes;
            }
            set
            {
                OnHosHisDetailNotesChanging(value);
                _HosHisDetailNotes = value;
                RaisePropertyChanged("HosHisDetailNotes");
                OnHosHisDetailNotesChanged();
            }
        }
        private String _HosHisDetailNotes;
        partial void OnHosHisDetailNotesChanging(String value);
        partial void OnHosHisDetailNotesChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public BedLocation BedLocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public HospitalizationHistory HospitalizationHistory
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }

        #endregion
    }
}
