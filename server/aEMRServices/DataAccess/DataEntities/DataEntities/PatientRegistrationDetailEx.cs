using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class PatientRegistrationDetailEx : PatientRegistrationDetail
    {
        public PatientRegistrationDetailEx()
            : base()
        {
            
        }
        #region Factory Method

        #endregion
        #region Primitive Properties
        [DataMemberAttribute]
        private bool _isBenhMoi;
        public bool isBenhMoi
        {
            get
            {
                return _isBenhMoi;
            }
            set
            {
                if (_isBenhMoi != value)
                {
                    _isBenhMoi = value;
                    RaisePropertyChanged("isBenhMoi");
                }
            }
        }

        [DataMemberAttribute]
        private bool _isHasHI;
        public bool IsHasHI
        {
            get
            {
                return _isHasHI;
            }
            set
            {
                if (_isHasHI != value)
                {
                    _isHasHI = value;
                    RaisePropertyChanged("IsHasHI");
                }
            }
        }

        private bool _isPO;
        public bool isPO
        {
            get
            {
                return _isPO;
            }
            set
            {
                if (_isPO != value)
                {
                    _isPO = value;
                    RaisePropertyChanged("isPO");
                }
            }
        }

        [DataMemberAttribute]
        private bool _isTaiKham;
        public bool isTaiKham
        {
            get
            {
                return _isTaiKham;
            }
            set
            {
                if (_isTaiKham != value)
                {
                    _isTaiKham = value;
                    RaisePropertyChanged("isTaiKham");
                }
            }
        }

        [DataMemberAttribute()]
        public PatientAppointment curPatientAppointments
        {
            get
            {
                return _curPatientAppointments;
            }
            set
            {
                if (_curPatientAppointments != value)
                {
                    _curPatientAppointments = value;
                    RaisePropertyChanged("curPatientAppointments");
                }
            }
        }
        private PatientAppointment _curPatientAppointments;

        [DataMemberAttribute()]
        public DiagnosisTreatment DiagnosisTreatment
        {
            get
            {
                return _DiagnosisTreatment;
            }
            set
            {
                if (_DiagnosisTreatment != value)
                {
                    _DiagnosisTreatment = value;
                    RaisePropertyChanged("DiagnosisTreatment");
                }
            }
        }
        private DiagnosisTreatment _DiagnosisTreatment;

        [DataMemberAttribute()]
        public PatientApptServiceDetails PatientApptServiceDetails
        {
            get
            {
                return _PatientApptServiceDetails;
            }
            set
            {
                if (_PatientApptServiceDetails != value)
                {
                    _PatientApptServiceDetails = value;
                    RaisePropertyChanged("PatientApptServiceDetails");
                }
            }
        }
        private PatientApptServiceDetails _PatientApptServiceDetails;

        [DataMemberAttribute()]
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                if (_ICD10Code != value)
                {
                    _ICD10Code = value;
                    RaisePropertyChanged("ICD10Code");
                }
            }
        }

        private string _ICD10Code;

        #endregion

       private PatientRegistrationDetailEx _tempRegDetails;
        #region IEditableObject Members

        public new void BeginEdit()
        {
            _tempRegDetails = (PatientRegistrationDetailEx)this.MemberwiseClone();
        }

        public new void CancelEdit()
        {
            if (null != _tempRegDetails)
            {
                CopyFrom(_tempRegDetails);

            }
        }

        public new void EndEdit()
        {
        }

        public void CopyFrom(PatientRegistrationDetailEx p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion


        public override bool Equals(object obj)
        {
            PatientRegistrationDetailEx info = obj as PatientRegistrationDetailEx;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtRegDetailID > 0 && this.PtRegDetailID == info.PtRegDetailID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    
}
