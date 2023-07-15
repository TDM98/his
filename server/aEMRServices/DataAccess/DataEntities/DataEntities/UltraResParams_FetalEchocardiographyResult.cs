using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UltraResParams_FetalEchocardiographyResult : NotifyChangedBase, IEditableObject
    {
        public UltraResParams_FetalEchocardiographyResult()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            UltraResParams_FetalEchocardiographyResult info = obj as UltraResParams_FetalEchocardiographyResult;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UltraResParams_FetalEchocardiographyResultID > 0 && this.UltraResParams_FetalEchocardiographyResultID == info.UltraResParams_FetalEchocardiographyResultID;
        }
        private UltraResParams_FetalEchocardiographyResult _tempUltraResParams_FetalEchocardiographyResult;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempUltraResParams_FetalEchocardiographyResult = (UltraResParams_FetalEchocardiographyResult)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempUltraResParams_FetalEchocardiographyResult)
                CopyFrom(_tempUltraResParams_FetalEchocardiographyResult);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(UltraResParams_FetalEchocardiographyResult p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new UltraResParams_FetalEchocardiographyResult object.

        /// <param name="UltraResParams_FetalEchocardiographyResultID">Initial value of the UltraResParams_FetalEchocardiographyResultID property.</param>

        public static UltraResParams_FetalEchocardiographyResult CreateUltraResParams_FetalEchocardiographyResult(Byte UltraResParams_FetalEchocardiographyResultID, String UltraResParams_FetalEchocardiographyResultName)
        {
            UltraResParams_FetalEchocardiographyResult UltraResParams_FetalEchocardiographyResult = new UltraResParams_FetalEchocardiographyResult();
            UltraResParams_FetalEchocardiographyResult.UltraResParams_FetalEchocardiographyResultID = UltraResParams_FetalEchocardiographyResultID;

            return UltraResParams_FetalEchocardiographyResult;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UltraResParams_FetalEchocardiographyResultID
        {
            get
            {
                return _UltraResParams_FetalEchocardiographyResultID;
            }
            set
            {
                if (_UltraResParams_FetalEchocardiographyResultID != value)
                {
                    OnUltraResParams_FetalEchocardiographyResultIDChanging(value);
                    _UltraResParams_FetalEchocardiographyResultID = value;
                    RaisePropertyChanged("UltraResParams_FetalEchocardiographyResultID");
                    OnUltraResParams_FetalEchocardiographyResultIDChanged();
                }
            }
        }
        private long _UltraResParams_FetalEchocardiographyResultID;
        partial void OnUltraResParams_FetalEchocardiographyResultIDChanging(long value);
        partial void OnUltraResParams_FetalEchocardiographyResultIDChanged();

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
        public bool CardialAbnormal
        {
            get
            {
                return _CardialAbnormal;
            }
            set
            {
                if (_CardialAbnormal != value)
                {
                    OnCardialAbnormalChanging(value);
                    _CardialAbnormal = value;
                    RaisePropertyChanged("CardialAbnormal");
                    OnCardialAbnormalChanged();
                }
            }
        }
        private bool _CardialAbnormal=false;
        partial void OnCardialAbnormalChanging(bool value);
        partial void OnCardialAbnormalChanged();

	
        [DataMemberAttribute()]
        public string CardialAbnormalDetail
        {
            get
            {
                return _CardialAbnormalDetail;
            }
            set
            {
                if (_CardialAbnormalDetail != value)
                {
                    OnCardialAbnormalDetailChanging(value);
                    _CardialAbnormalDetail = value;
                    RaisePropertyChanged("CardialAbnormalDetail");
                    OnCardialAbnormalDetailChanged();
                }
            }
        }
        private string _CardialAbnormalDetail;
        partial void OnCardialAbnormalDetailChanging(string value);
        partial void OnCardialAbnormalDetailChanged();

	
        [DataMemberAttribute()]
        public string Susgest
        {
            get
            {
                return _Susgest;
            }
            set
            {
                if (_Susgest != value)
                {
                    OnSusgestChanging(value);
                    _Susgest = value;
                    RaisePropertyChanged("Susgest");
                    OnSusgestChanged();
                }
            }
        }
        private string _Susgest;
        partial void OnSusgestChanging(string value);
        partial void OnSusgestChanged();

	
        [DataMemberAttribute()]
        public DateTime UltraResParamDate
        {
            get
            {
                return _UltraResParamDate;
            }
            set
            {
                if (_UltraResParamDate != value)
                {
                    OnUltraResParamDateChanging(value);
                    _UltraResParamDate = value;
                    RaisePropertyChanged("UltraResParamDate");
                    OnUltraResParamDateChanged();
                }
            }
        }
        private DateTime _UltraResParamDate=DateTime.Now;
        partial void OnUltraResParamDateChanging(DateTime value);
        partial void OnUltraResParamDateChanged();

	
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
                return this.UltraResParams_FetalEchocardiographyResultID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
