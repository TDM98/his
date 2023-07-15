using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class TrainingInstitution : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new TrainingInstitution object.

        /// <param name="trainingID">Initial value of the TrainingID property.</param>
        /// <param name="trainingCodeName">Initial value of the TrainingCodeName property.</param>
        /// <param name="tFromDate">Initial value of the TFromDate property.</param>
        /// <param name="tToDate">Initial value of the TToDate property.</param>
        public static TrainingInstitution CreateTrainingInstitution(long trainingID, String trainingCodeName, DateTime tFromDate, DateTime tToDate)
        {
            TrainingInstitution trainingInstitution = new TrainingInstitution();
            trainingInstitution.TrainingID = trainingID;
            trainingInstitution.TrainingCodeName = trainingCodeName;
            trainingInstitution.TFromDate = tFromDate;
            trainingInstitution.TToDate = tToDate;
            return trainingInstitution;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TrainingID
        {
            get
            {
                return _TrainingID;
            }
            set
            {
                if (_TrainingID != value)
                {
                    OnTrainingIDChanging(value);
                    ////ReportPropertyChanging("TrainingID");
                    _TrainingID = value;
                    RaisePropertyChanged("TrainingID");
                    OnTrainingIDChanged();
                }
            }
        }
        private long _TrainingID;
        partial void OnTrainingIDChanging(long value);
        partial void OnTrainingIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public String TrainingCodeName
        {
            get
            {
                return _TrainingCodeName;
            }
            set
            {
                OnTrainingCodeNameChanging(value);
                ////ReportPropertyChanging("TrainingCodeName");
                _TrainingCodeName = value;
                RaisePropertyChanged("TrainingCodeName");
                OnTrainingCodeNameChanged();
            }
        }
        private String _TrainingCodeName;
        partial void OnTrainingCodeNameChanging(String value);
        partial void OnTrainingCodeNameChanged();





        [DataMemberAttribute()]
        public DateTime TFromDate
        {
            get
            {
                return _TFromDate;
            }
            set
            {
                OnTFromDateChanging(value);
                ////ReportPropertyChanging("TFromDate");
                _TFromDate = value;
                RaisePropertyChanged("TFromDate");
                OnTFromDateChanged();
            }
        }
        private DateTime _TFromDate;
        partial void OnTFromDateChanging(DateTime value);
        partial void OnTFromDateChanged();





        [DataMemberAttribute()]
        public DateTime TToDate
        {
            get
            {
                return _TToDate;
            }
            set
            {
                OnTToDateChanging(value);
                ////ReportPropertyChanging("TToDate");
                _TToDate = value;
                RaisePropertyChanged("TToDate");
                OnTToDateChanged();
            }
        }
        private DateTime _TToDate;
        partial void OnTToDateChanging(DateTime value);
        partial void OnTToDateChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> TOverSea
        {
            get
            {
                return _TOverSea;
            }
            set
            {
                OnTOverSeaChanging(value);
                ////ReportPropertyChanging("TOverSea");
                _TOverSea = value;
                RaisePropertyChanged("TOverSea");
                OnTOverSeaChanged();
            }
        }
        private Nullable<Boolean> _TOverSea;
        partial void OnTOverSeaChanging(Nullable<Boolean> value);
        partial void OnTOverSeaChanged();





        [DataMemberAttribute()]
        public String TSchool
        {
            get
            {
                return _TSchool;
            }
            set
            {
                OnTSchoolChanging(value);
                ////ReportPropertyChanging("TSchool");
                _TSchool = value;
                RaisePropertyChanged("TSchool");
                OnTSchoolChanged();
            }
        }
        private String _TSchool;
        partial void OnTSchoolChanging(String value);
        partial void OnTSchoolChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> TCost
        {
            get
            {
                return _TCost;
            }
            set
            {
                OnTCostChanging(value);
                ////ReportPropertyChanging("TCost");
                _TCost = value;
                RaisePropertyChanged("TCost");
                OnTCostChanged();
            }
        }
        private Nullable<Decimal> _TCost;
        partial void OnTCostChanging(Nullable<Decimal> value);
        partial void OnTCostChanged();





        [DataMemberAttribute()]
        public String TNotes
        {
            get
            {
                return _TNotes;
            }
            set
            {
                OnTNotesChanging(value);
                ////ReportPropertyChanging("TNotes");
                _TNotes = value;
                RaisePropertyChanged("TNotes");
                OnTNotesChanged();
            }
        }
        private String _TNotes;
        partial void OnTNotesChanging(String value);
        partial void OnTNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFTYPET_REL_HR19_TRAINING", "RefTypeTraining")]
        public ObservableCollection<RefTypeTraining> RefTypeTrainings
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_TRAINING_REL_HR02_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
