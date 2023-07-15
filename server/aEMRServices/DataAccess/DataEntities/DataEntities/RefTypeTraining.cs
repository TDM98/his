using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefTypeTraining : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefTypeTraining object.

        /// <param name="tTID">Initial value of the TTID property.</param>
        /// <param name="tTName">Initial value of the TTName property.</param>
        public static RefTypeTraining CreateRefTypeTraining(long tTID, String tTName)
        {
            RefTypeTraining refTypeTraining = new RefTypeTraining();
            refTypeTraining.TTID = tTID;
            refTypeTraining.TTName = tTName;
            return refTypeTraining;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TTID
        {
            get
            {
                return _TTID;
            }
            set
            {
                if (_TTID != value)
                {
                    OnTTIDChanging(value);
                    ////ReportPropertyChanging("TTID");
                    _TTID = value;
                    RaisePropertyChanged("TTID");
                    OnTTIDChanged();
                }
            }
        }
        private long _TTID;
        partial void OnTTIDChanging(long value);
        partial void OnTTIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> TrainingID
        {
            get
            {
                return _TrainingID;
            }
            set
            {
                OnTrainingIDChanging(value);
                ////ReportPropertyChanging("TrainingID");
                _TrainingID = value;
                RaisePropertyChanged("TrainingID");
                OnTrainingIDChanged();
            }
        }
        private Nullable<long> _TrainingID;
        partial void OnTrainingIDChanging(Nullable<long> value);
        partial void OnTrainingIDChanged();





        [DataMemberAttribute()]
        public String TTName
        {
            get
            {
                return _TTName;
            }
            set
            {
                OnTTNameChanging(value);
                ////ReportPropertyChanging("TTName");
                _TTName = value;
                RaisePropertyChanged("TTName");
                OnTTNameChanged();
            }
        }
        private String _TTName;
        partial void OnTTNameChanging(String value);
        partial void OnTTNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFTYPET_REL_HR19_TRAINING", "TrainingInstitution")]
        public TrainingInstitution TrainingInstitution
        {
            get;
            set;
        }

        #endregion
    }
}
