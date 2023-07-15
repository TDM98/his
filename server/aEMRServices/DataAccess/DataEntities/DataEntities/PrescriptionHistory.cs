using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PrescriptionHistory : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PrescriptionHistory object.

        /// <param name="keepTrackID">Initial value of the KeepTrackID property.</param>
        /// <param name="keepTrackDateTime">Initial value of the KeepTrackDateTime property.</param>
        public static PrescriptionHistory CreatePrescriptionHistory(long keepTrackID, DateTime keepTrackDateTime)
        {
            PrescriptionHistory prescriptionHistory = new PrescriptionHistory();
            prescriptionHistory.KeepTrackID = keepTrackID;
            prescriptionHistory.KeepTrackDateTime = keepTrackDateTime;
            return prescriptionHistory;
        }

        #endregion
        #region Primitive Properties

        
        [DataMemberAttribute()]
        public long KeepTrackID
        {
            get
            {
                return _KeepTrackID;
            }
            set
            {
                if (_KeepTrackID != value)
                {
                    OnKeepTrackIDChanging(value);
                    ////ReportPropertyChanging("KeepTrackID");
                    _KeepTrackID = value;
                    RaisePropertyChanged("KeepTrackID");
                    OnKeepTrackIDChanged();
                }
            }
        }
        private long _KeepTrackID;
        partial void OnKeepTrackIDChanging(long value);
        partial void OnKeepTrackIDChanged();

        

        [DataMemberAttribute()]
        public Nullable<long> PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                ////ReportPropertyChanging("PrescriptID");
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private Nullable<long> _PrescriptID;
        partial void OnPrescriptIDChanging(Nullable<long> value);
        partial void OnPrescriptIDChanged();





        [DataMemberAttribute()]
        public DateTime KeepTrackDateTime
        {
            get
            {
                return _KeepTrackDateTime;
            }
            set
            {
                OnKeepTrackDateTimeChanging(value);
                ////ReportPropertyChanging("KeepTrackDateTime");
                _KeepTrackDateTime = value;
                RaisePropertyChanged("KeepTrackDateTime");
                OnKeepTrackDateTimeChanged();
            }
        }
        private DateTime _KeepTrackDateTime;
        partial void OnKeepTrackDateTimeChanging(DateTime value);
        partial void OnKeepTrackDateTimeChanged();

        
        [DataMemberAttribute()]
        public Nullable<Byte> TimesNumberIsPrinted
        {
            get
            {
                return _TimesNumberIsPrinted;
            }
            set
            {
                OnTimesNumberIsPrintedChanging(value);
                ////ReportPropertyChanging("TimesNumberIsPrinted");
                _TimesNumberIsPrinted = value;
                RaisePropertyChanged("TimesNumberIsPrinted");
                OnTimesNumberIsPrintedChanged();
            }
        }
        private Nullable<Byte> _TimesNumberIsPrinted;
        partial void OnTimesNumberIsPrintedChanging(Nullable<Byte> value);
        partial void OnTimesNumberIsPrintedChanged();





        [DataMemberAttribute()]
        public String ChangeLogs
        {
            get
            {
                return _ChangeLogs;
            }
            set
            {
                OnChangeLogsChanging(value);
                ////ReportPropertyChanging("ChangeLogs");
                _ChangeLogs = value;
                RaisePropertyChanged("ChangeLogs");
                OnChangeLogsChanged();
            }
        }
        private String _ChangeLogs;
        partial void OnChangeLogsChanging(String value);
        partial void OnChangeLogsChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PRESCRIP_REL_PMR17_PRESCRIP", "Prescriptions")]
        public Prescription Prescription
        {
            get;
            set;
        }


        #endregion
    }
}
