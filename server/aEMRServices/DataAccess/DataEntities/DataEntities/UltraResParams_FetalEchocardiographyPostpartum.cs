using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UltraResParams_FetalEchocardiographyPostpartum : NotifyChangedBase, IEditableObject
    {
        public UltraResParams_FetalEchocardiographyPostpartum()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            UltraResParams_FetalEchocardiographyPostpartum info = obj as UltraResParams_FetalEchocardiographyPostpartum;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UltraResParams_FetalEchocardiographyPostpartumID > 0 && this.UltraResParams_FetalEchocardiographyPostpartumID == info.UltraResParams_FetalEchocardiographyPostpartumID;
        }
        private UltraResParams_FetalEchocardiographyPostpartum _tempUltraResParams_FetalEchocardiographyPostpartum;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempUltraResParams_FetalEchocardiographyPostpartum = (UltraResParams_FetalEchocardiographyPostpartum)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempUltraResParams_FetalEchocardiographyPostpartum)
                CopyFrom(_tempUltraResParams_FetalEchocardiographyPostpartum);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(UltraResParams_FetalEchocardiographyPostpartum p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new UltraResParams_FetalEchocardiographyPostpartum object.

        /// <param name="UltraResParams_FetalEchocardiographyPostpartumID">Initial value of the UltraResParams_FetalEchocardiographyPostpartumID property.</param>

        public static UltraResParams_FetalEchocardiographyPostpartum CreateUltraResParams_FetalEchocardiographyPostpartum(Byte UltraResParams_FetalEchocardiographyPostpartumID, String UltraResParams_FetalEchocardiographyPostpartumName)
        {
            UltraResParams_FetalEchocardiographyPostpartum UltraResParams_FetalEchocardiographyPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            UltraResParams_FetalEchocardiographyPostpartum.UltraResParams_FetalEchocardiographyPostpartumID = UltraResParams_FetalEchocardiographyPostpartumID;

            return UltraResParams_FetalEchocardiographyPostpartum;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UltraResParams_FetalEchocardiographyPostpartumID
        {
            get
            {
                return _UltraResParams_FetalEchocardiographyPostpartumID;
            }
            set
            {
                if (_UltraResParams_FetalEchocardiographyPostpartumID != value)
                {
                    OnUltraResParams_FetalEchocardiographyPostpartumIDChanging(value);
                    _UltraResParams_FetalEchocardiographyPostpartumID = value;
                    RaisePropertyChanged("UltraResParams_FetalEchocardiographyPostpartumID");
                    OnUltraResParams_FetalEchocardiographyPostpartumIDChanged();
                }
            }
        }
        private long _UltraResParams_FetalEchocardiographyPostpartumID;
        partial void OnUltraResParams_FetalEchocardiographyPostpartumIDChanging(long value);
        partial void OnUltraResParams_FetalEchocardiographyPostpartumIDChanged();

        [DataMemberAttribute()]
        public long PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                if (_PCLImgResultID != value)
                {
                    OnPCLImgResultIDChanging(value);
                    _PCLImgResultID = value;
                    RaisePropertyChanged("PCLImgResultID");
                    OnPCLImgResultIDChanged();
                }
            }
        }
        private long _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(long value);
        partial void OnPCLImgResultIDChanged();
        
	    [DataMemberAttribute()]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                if (_CreateDate != value)
                {
                    OnCreateDateChanging(value);
                    _CreateDate = value;
                    RaisePropertyChanged("CreateDate");
                    OnCreateDateChanged();
                }
            }
        }
        private DateTime _CreateDate;
        partial void OnCreateDateChanging(DateTime value);
        partial void OnCreateDateChanged();

	
        [DataMemberAttribute()]
        public DateTime BabyBirthday
        {
            get
            {
                return _BabyBirthday;
            }
            set
            {
                if (_BabyBirthday != value)
                {
                    OnBabyBirthdayChanging(value);
                    _BabyBirthday = value;
                    RaisePropertyChanged("BabyBirthday");
                    OnBabyBirthdayChanged();
                }
            }
        }
        private DateTime _BabyBirthday=DateTime.Now;
        partial void OnBabyBirthdayChanging(DateTime value);
        partial void OnBabyBirthdayChanged();

	
        [DataMemberAttribute()]
        public double BabyWeight
        {
            get
            {
                return _BabyWeight;
            }
            set
            {
                if (_BabyWeight != value)
                {
                    OnBabyWeightChanging(value);
                    _BabyWeight = value;
                    RaisePropertyChanged("BabyWeight");
                    OnBabyWeightChanged();
                }
            }
        }
        private double _BabyWeight;
        partial void OnBabyWeightChanging(double value);
        partial void OnBabyWeightChanged();

	
        [DataMemberAttribute()]
        public bool BabySex
        {
            get
            {
                return _BabySex;
            }
            set
            {
                if (_BabySex != value)
                {
                    OnBabySexChanging(value);
                    _BabySex = value;
                    RaisePropertyChanged("BabySex");
                    OnBabySexChanged();
                }
            }
        }
        private bool _BabySex=true;
        partial void OnBabySexChanging(bool value);
        partial void OnBabySexChanged();

	
        [DataMemberAttribute()]
        public DateTime URP_Date
        {
            get
            {
                return _URP_Date;
            }
            set
            {
                if (_URP_Date != value)
                {
                    OnURP_DateChanging(value);
                    _URP_Date = value;
                    RaisePropertyChanged("URP_Date");
                    OnURP_DateChanged();
                }
            }
        }
        private DateTime _URP_Date = DateTime.Now;
        partial void OnURP_DateChanging(DateTime value);
        partial void OnURP_DateChanged();

	
        [DataMemberAttribute()]
        public bool PFO
        {
            get
            {
                return _PFO;
            }
            set
            {
                if (_PFO != value)
                {
                    OnPFOChanging(value);
                    _PFO = value;
                    RaisePropertyChanged("PFO");
                    OnPFOChanged();
                }
            }
        }
        private bool _PFO=false;
        partial void OnPFOChanging(bool value);
        partial void OnPFOChanged();

	
        [DataMemberAttribute()]
        public bool PCA
        {
            get
            {
                return _PCA;
            }
            set
            {
                if (_PCA != value)
                {
                    OnPCAChanging(value);
                    _PCA = value;
                    RaisePropertyChanged("PCA");
                    OnPCAChanged();
                }
            }
        }
        private bool _PCA=false;
        partial void OnPCAChanging(bool value);
        partial void OnPCAChanged();

	
        [DataMemberAttribute()]
        public string AnotherDiagnosic
        {
            get
            {
                return _AnotherDiagnosic;
            }
            set
            {
                if (_AnotherDiagnosic != value)
                {
                    OnAnotherDiagnosicChanging(value);
                    _AnotherDiagnosic = value;
                    RaisePropertyChanged("AnotherDiagnosic");
                    OnAnotherDiagnosicChanged();
                }
            }
        }
        private string _AnotherDiagnosic;
        partial void OnAnotherDiagnosicChanging(string value);
        partial void OnAnotherDiagnosicChanged();

	
        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes != value)
                {
                    OnNotesChanging(value);
                    _Notes = value;
                    RaisePropertyChanged("Notes");
                    OnNotesChanged();
                }
            }
        }
        private string _Notes;
        partial void OnNotesChanging(string value);
        partial void OnNotesChanged();

	
        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID != value)
                {
                    OnDoctorStaffIDChanging(value);
                    _DoctorStaffID = value;
                    RaisePropertyChanged("DoctorStaffID");
                    OnDoctorStaffIDChanged();
                }
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();

        //==== 20161209 CMN Begin: Add PatientPCLReqID
        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();
        //==== 20161209 End.
        #endregion


        #region Navigation Properties
        [DataMemberAttribute()]
        public Staff VStaff
        {
            get
            {
                return _VStaff;
            }
            set
            {
                if (_VStaff != value)
                {
                    OnVStaffChanging(value);
                    _VStaff = value;
                    RaisePropertyChanged("VStaff");
                    DoctorStaffID = VStaff.StaffID;
                    OnVStaffChanged();
                }
            }
        }
        private Staff _VStaff;
        partial void OnVStaffChanging(Staff value);
        partial void OnVStaffChanged();

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        [DataMemberAttribute()]
        public bool Tab_Update_Required
        {
            get
            {
                return this.UltraResParams_FetalEchocardiographyPostpartumID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
