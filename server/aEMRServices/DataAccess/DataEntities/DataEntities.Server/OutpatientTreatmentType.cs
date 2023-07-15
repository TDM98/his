using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    [DataContract]
    public partial class OutpatientTreatmentType : NotifyChangedBase
    {
        public OutpatientTreatmentType()
         : base()
        {

        }
        private OutpatientTreatmentType _tempOutpatientTreatmentType;
        public override bool Equals(object obj)
        {
            OutpatientTreatmentType info = obj as OutpatientTreatmentType;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.OutpatientTreatmentTypeID > 0 && this.OutpatientTreatmentTypeID == info.OutpatientTreatmentTypeID;
        }

        public void BeginEdit()
        {
            _tempOutpatientTreatmentType = (OutpatientTreatmentType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempOutpatientTreatmentType)
                CopyFrom(_tempOutpatientTreatmentType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(OutpatientTreatmentType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        private long _OutpatientTreatmentTypeID;
        private string _OutpatientTreatmentName;
        private int _MaxNumOfDayMedicalRecord;
        private int _MinNumOfDayMedicalRecord;
        private int _MaxNumOfDayTreatment;
        private int _MinNumOfDayTreatment;
        private int _MaxNumOfDayMedicine;
        private int _MinNumOfDayMedicine;
        private bool _IsDeleted;
        private string _Log;
        [DataMemberAttribute]
        public long OutpatientTreatmentTypeID
        {
            get
            {
                return _OutpatientTreatmentTypeID;
            }
            set
            {
                if (_OutpatientTreatmentTypeID != value)
                {
                    _OutpatientTreatmentTypeID = value;
                    RaisePropertyChanged("OutpatientTreatmentTypeID");
                }
            }
        }
        [DataMemberAttribute]
        public string OutpatientTreatmentName
        {
            get
            {
                return _OutpatientTreatmentName;
            }
            set
            {
                if (_OutpatientTreatmentName != value)
                {
                    _OutpatientTreatmentName = value;
                    RaisePropertyChanged("OutpatientTreatmentName");
                }
            }
        }
        [DataMemberAttribute]
        public int MaxNumOfDayMedicalRecord
        {
            get
            {
                return _MaxNumOfDayMedicalRecord;
            }
            set
            {
                if (_MaxNumOfDayMedicalRecord != value)
                {
                    _MaxNumOfDayMedicalRecord = value;
                    RaisePropertyChanged("MaxNumOfDayMedicalRecord");
                }
            }
        }
        [DataMemberAttribute]
        public int MinNumOfDayMedicalRecord
        {
            get
            {
                return _MinNumOfDayMedicalRecord;
            }
            set
            {
                if (_MinNumOfDayMedicalRecord != value)
                {
                    _MinNumOfDayMedicalRecord = value;
                    RaisePropertyChanged("MinNumOfDayMedicalRecord");
                }
            }
        }
        [DataMemberAttribute]
        public int MaxNumOfDayTreatment
        {
            get
            {
                return _MaxNumOfDayTreatment;
            }
            set
            {
                if (_MaxNumOfDayTreatment != value)
                {
                    _MaxNumOfDayTreatment = value;
                    RaisePropertyChanged("MaxNumOfDayTreatment");
                }
            }
        }
        [DataMemberAttribute]
        public int MinNumOfDayTreatment
        {
            get
            {
                return _MinNumOfDayTreatment;
            }
            set
            {
                if (_MinNumOfDayTreatment != value)
                {
                    _MinNumOfDayTreatment = value;
                    RaisePropertyChanged("MinNumOfDayTreatment");
                }
            }
        }
        [DataMemberAttribute]
        public int MaxNumOfDayMedicine
        {
            get
            {
                return _MaxNumOfDayMedicine;
            }
            set
            {
                if (_MaxNumOfDayMedicine != value)
                {
                    _MaxNumOfDayMedicine = value;
                    RaisePropertyChanged("MaxNumOfDayMedicine");
                }
            }
        }
        [DataMemberAttribute]
        public int MinNumOfDayMedicine
        {
            get
            {
                return _MinNumOfDayMedicine;
            }
            set
            {
                if (_MinNumOfDayMedicine != value)
                {
                    _MinNumOfDayMedicine = value;
                    RaisePropertyChanged("MinNumOfDayMedicine");
                }
            }
        }
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                }
            }
        }
        [DataMemberAttribute]
        public string Log
        {
            get
            {
                return _Log;
            }
            set
            {
                if (_Log != value)
                {
                    _Log = value;
                    RaisePropertyChanged("Log");
                }
            }
        }
        [DataMemberAttribute()]
        public string ListICD10
        {
            get { return _ListICD10; }
            set
            {
                _ListICD10 = value;
                RaisePropertyChanged("ListICD10");
            }
        }
        private string _ListICD10;

        [DataMemberAttribute()]
        public ObservableCollection<OutpatientTreatmentTypeICD10Link> ListICD10Code
        {
            get { return _ListICD10Code; }
            set
            {
                _ListICD10Code = value;
                RaisePropertyChanged("ListICD10Code");
            }
        }
        private ObservableCollection<OutpatientTreatmentTypeICD10Link> _ListICD10Code;

        private string _OutpatientTreatmentCode;
        [DataMemberAttribute]
        public string OutpatientTreatmentCode
        {
            get
            {
                return _OutpatientTreatmentCode;
            }
            set
            {
                if (_OutpatientTreatmentCode != value)
                {
                    _OutpatientTreatmentCode = value;
                    RaisePropertyChanged("OutpatientTreatmentCode");
                }
            }
        }
        private bool _IsChronic = false;
        [DataMemberAttribute]
        public bool IsChronic
        {
            get
            {
                return _IsChronic;
            }
            set
            {
                if (_IsChronic != value)
                {
                    _IsChronic = value;
                    RaisePropertyChanged("IsChronic");
                }
            }
        }
        public bool CheckValidZeroValue()
        {
            if (MaxNumOfDayMedicalRecord == 0 || MinNumOfDayMedicalRecord == 0
                || MaxNumOfDayTreatment == 0 || MinNumOfDayTreatment == 0
                || MaxNumOfDayMedicine == 0|| MinNumOfDayMedicine == 0 )
            {
                return false;
            }
            return true;
        }
        public bool CheckValidMinMaxValue()
        {
            if (MaxNumOfDayMedicalRecord < MinNumOfDayMedicalRecord
              || MaxNumOfDayTreatment < MinNumOfDayTreatment
              || MaxNumOfDayMedicine < MinNumOfDayMedicine
              )
            {
                return false;
            }
            return true;
        }
        //public void GetListICD10Code()
        //{
        //    if (string.IsNullOrEmpty(ListICD10))
        //    {
        //        return;
        //    }
        //    ListICD10Code = new List<string>();
        //    foreach (var item in ListICD10.Split(';'))
        //    {
        //        ListICD10Code.Add(item);
        //    }
        //}
    }
}
