using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
/*
 * 20230622 #001 DatTB: Thêm trường thông tin
 */
namespace DataEntities
{
    [DataContract]
    public partial class OutpatientTreatmentTypeICD10Link : EntityBase
    {
        public OutpatientTreatmentTypeICD10Link()
         : base()
        {

        }
        private OutpatientTreatmentTypeICD10Link _tempOutpatientTreatmentType;
        public override bool Equals(object obj)
        {
            OutpatientTreatmentTypeICD10Link info = obj as OutpatientTreatmentTypeICD10Link;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.OutpatientTreatmentTypeICD10LinkID > 0 && this.OutpatientTreatmentTypeICD10LinkID == info.OutpatientTreatmentTypeICD10LinkID;
        }

        public void BeginEdit()
        {
            _tempOutpatientTreatmentType = (OutpatientTreatmentTypeICD10Link)this.MemberwiseClone();
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

        public void CopyFrom(OutpatientTreatmentTypeICD10Link p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        private long _OutpatientTreatmentTypeICD10LinkID;
        private string _ICD10;
        private long _OutpatientTreatmentTypeID;
        private long _IDCode;
        [DataMemberAttribute]
        public long OutpatientTreatmentTypeICD10LinkID
        {
            get
            {
                return _OutpatientTreatmentTypeICD10LinkID;
            }
            set
            {
                if (_OutpatientTreatmentTypeICD10LinkID != value)
                {
                    _OutpatientTreatmentTypeICD10LinkID = value;
                    RaisePropertyChanged("OutpatientTreatmentTypeICD10LinkID");
                }
            }
        }
        [DataMemberAttribute]
        public string ICD10
        {
            get
            {
                return _ICD10;
            }
            set
            {
                if (_ICD10 != value)
                {
                    _ICD10 = value;
                    RaisePropertyChanged("ICD10");
                }
            }
        }
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
        public long IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                if (_IDCode != value)
                {
                    _IDCode = value;
                    RaisePropertyChanged("IDCode");
                }
            }
        }

        private string _DiseaseNameVN;
        [DataMemberAttribute]
        public string DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                if (_DiseaseNameVN != value)
                {
                    _DiseaseNameVN = value;
                    RaisePropertyChanged("DiseaseNameVN");
                }
            }
        }

        private string _DiseaseDescription;
        [DataMemberAttribute]
        public string DiseaseDescription
        {
            get
            {
                return _DiseaseDescription;
            }
            set
            {
                if (_DiseaseDescription != value)
                {
                    _DiseaseDescription = value;
                    RaisePropertyChanged("DiseaseDescription");
                }
            }
        }

        private bool _IsActive;
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        //▼==== #001
        private bool _ICD_IsActive;
        [DataMemberAttribute]
        public bool ICD_IsActive
        {
            get
            {
                return _ICD_IsActive;
            }
            set
            {
                if (_ICD_IsActive != value)
                {
                    _ICD_IsActive = value;
                    RaisePropertyChanged("ICD_IsActive");
                }
            }
        }

        private string _Note;
        [DataMemberAttribute]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (_Note != value)
                {
                    _Note = value;
                    RaisePropertyChanged("Note");
                }
            }
        }
        //▲==== #001
    }
}
