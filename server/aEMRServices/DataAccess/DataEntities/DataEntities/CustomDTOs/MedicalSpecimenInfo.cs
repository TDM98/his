using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalSpecimenInfo : NotifyChangedBase
    {
        #region Primitive Properties

        private long _MedSpecID;
        public long MedSpecID
        {
            get
            {
                return _MedSpecID;
            }
            set
            {
                if (_MedSpecID != value)
                {
                    _MedSpecID = value;
                    RaisePropertyChanged("MedSpecID");
                }
            }
        }

        private String _MedSpecName;
        public String MedSpecName
        {
            get
            {
                return _MedSpecName;
            }
            set
            {
                _MedSpecName = value;
                RaisePropertyChanged("MedSpecName");
            }
        }

        private String _StorageConditions;
        public String StorageConditions
        {
            get
            {
                return _StorageConditions;
            }
            set
            {
                _StorageConditions = value;
                RaisePropertyChanged("StorageConditions");
            }
        }

        private String _MedSpecNotes;
        public String MedSpecNotes
        {
            get
            {
                return _MedSpecNotes;
            }
            set
            {
                _MedSpecNotes = value;
                RaisePropertyChanged("MedSpecNotes");
            }
        }

        //From MedicalSpecimensCategory
        private short _MedSpecCatID;
        public short MedSpecCatID
        {
            get
            {
                return _MedSpecCatID;
            }
            set
            {
                _MedSpecCatID = value;
                RaisePropertyChanged("MedSpecCatID");
            }
        }

        private String _MedSpecCatName;
        public String MedSpecCatName
        {
            get
            {
                return _MedSpecCatName;
            }
            set
            {
                _MedSpecCatName = value;
                RaisePropertyChanged("MedSpecCatName");
            }
        }

        //Sampling Info
        private Nullable<DateTime> _SamplingDate;
        public Nullable<DateTime>  SamplingDate
        {
            get
            {
                return _SamplingDate;
            }
            set
            {
                _SamplingDate = value;
                RaisePropertyChanged("SamplingDate");
            }
        }

        private String _SampleCode;
        public String SampleCode
        {
            get
            {
                return _SampleCode;
            }
            set
            {
                _SampleCode = value;
                RaisePropertyChanged("SampleCode");
            }
        }

        private Nullable<long> _AgencyID;
        public Nullable<long> AgencyID
        {
            get
            {
                return _AgencyID;
            }
            set
            {
                _AgencyID = value;
                RaisePropertyChanged("AgencyID");
            }
        }

        private String _AgencyName;
        public String AgencyName
        {
            get
            {
                return _AgencyName;
            }
            set
            {
                _AgencyName = value;
                RaisePropertyChanged("AgencyName");
            }
        }

        private String _LaboratorianName;
        public String LaboratorianName
        {
            get
            {
                return _LaboratorianName;
            }
            set
            {
                _LaboratorianName = value;
                RaisePropertyChanged("LaboratorianName");
            }
        }


        private Nullable<long> _LatestEPrescriptionID;
        public Nullable<long> LatestEPrescriptionID
        {
            get
            {
                return _LatestEPrescriptionID;
            }
            set
            {
                _LatestEPrescriptionID = value;
                RaisePropertyChanged("LatestEPrescriptionID");
            }
        }

        #endregion


    }
}
