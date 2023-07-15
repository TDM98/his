using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_OesophagienneDiagnosis : NotifyChangedBase, IEditableObject
    {
        public URP_FE_OesophagienneDiagnosis()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_OesophagienneDiagnosis info = obj as URP_FE_OesophagienneDiagnosis;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_OesophagienneDiagnosisID > 0 && this.URP_FE_OesophagienneDiagnosisID == info.URP_FE_OesophagienneDiagnosisID;
        }
        private URP_FE_OesophagienneDiagnosis _tempURP_FE_OesophagienneDiagnosis;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_OesophagienneDiagnosis = (URP_FE_OesophagienneDiagnosis)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_OesophagienneDiagnosis)
                CopyFrom(_tempURP_FE_OesophagienneDiagnosis);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_OesophagienneDiagnosis p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_OesophagienneDiagnosis object.

        /// <param name="URP_FE_OesophagienneDiagnosisID">Initial value of the URP_FE_OesophagienneDiagnosisID property.</param>
        /// <param name="URP_FE_OesophagienneDiagnosisName">Initial value of the URP_FE_OesophagienneDiagnosisName property.</param>
        public static URP_FE_OesophagienneDiagnosis CreateURP_FE_OesophagienneDiagnosis(Byte URP_FE_OesophagienneDiagnosisID, String URP_FE_OesophagienneDiagnosisName)
        {
            URP_FE_OesophagienneDiagnosis URP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
            URP_FE_OesophagienneDiagnosis.URP_FE_OesophagienneDiagnosisID = URP_FE_OesophagienneDiagnosisID;
            
            return URP_FE_OesophagienneDiagnosis;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_OesophagienneDiagnosisID
        {
            get
            {
                return _URP_FE_OesophagienneDiagnosisID;
            }
            set
            {
                if (_URP_FE_OesophagienneDiagnosisID != value)
                {
                    OnURP_FE_OesophagienneDiagnosisIDChanging(value);
                    _URP_FE_OesophagienneDiagnosisID = value;
                    RaisePropertyChanged("URP_FE_OesophagienneDiagnosisID");
                    OnURP_FE_OesophagienneDiagnosisIDChanged();
                }
            }
        }
        private long _URP_FE_OesophagienneDiagnosisID;
        partial void OnURP_FE_OesophagienneDiagnosisIDChanging(long value);
        partial void OnURP_FE_OesophagienneDiagnosisIDChanged();

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


        private string _ChanDoanQuaThucQuan;
       
        //private long _V_ChanDoanQuaThucQuanID;

        [DataMemberAttribute()]
        public string ChanDoanQuaThucQuan
        {
            get
            {
                return _ChanDoanQuaThucQuan;
            }
            set
            {
                if (_ChanDoanQuaThucQuan == value)
                    return;
                _ChanDoanQuaThucQuan = value;
                RaisePropertyChanged("ChanDoanQuaThucQuan");
            }
        }

        //[DataMemberAttribute()]
        //public long V_ChanDoanQuaThucQuanID
        //{
        //    get
        //    {
        //        return _V_ChanDoanQuaThucQuanID;
        //    }
        //    set
        //    {
        //        if (_V_ChanDoanQuaThucQuanID == value)
        //            return;
        //        _V_ChanDoanQuaThucQuanID = value;
        //        RaisePropertyChanged("V_ChanDoanQuaThucQuanID");
        //    }
        //}
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public ObservableCollection<Donor> Donors
        {
            get;
            set;
        }

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
                    RaisePropertyChanged("CreateDate");
                    OnDoctorStaffIDChanged();
                }
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();

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
                return this.URP_FE_OesophagienneDiagnosisID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
