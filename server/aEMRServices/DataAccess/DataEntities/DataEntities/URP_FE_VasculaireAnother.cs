using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_VasculaireAnother : NotifyChangedBase, IEditableObject
    {
        public URP_FE_VasculaireAnother()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_VasculaireAnother info = obj as URP_FE_VasculaireAnother;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_VasculaireAnotherID > 0 && this.URP_FE_VasculaireAnotherID == info.URP_FE_VasculaireAnotherID;
        }
        private URP_FE_VasculaireAnother _tempURP_FE_VasculaireAnother;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_VasculaireAnother = (URP_FE_VasculaireAnother)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_VasculaireAnother)
                CopyFrom(_tempURP_FE_VasculaireAnother);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_VasculaireAnother p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_VasculaireAnother object.

        /// <param name="URP_FE_VasculaireAnotherID">Initial value of the URP_FE_VasculaireAnotherID property.</param>
        /// <param name="URP_FE_VasculaireAnotherName">Initial value of the URP_FE_VasculaireAnotherName property.</param>
        public static URP_FE_VasculaireAnother CreateURP_FE_VasculaireAnother(Byte URP_FE_VasculaireAnotherID, String URP_FE_VasculaireAnotherName)
        {
            URP_FE_VasculaireAnother URP_FE_VasculaireAnother = new URP_FE_VasculaireAnother();
            URP_FE_VasculaireAnother.URP_FE_VasculaireAnotherID = URP_FE_VasculaireAnotherID;
            
            return URP_FE_VasculaireAnother;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_VasculaireAnotherID
        {
            get
            {
                return _URP_FE_VasculaireAnotherID;
            }
            set
            {
                if (_URP_FE_VasculaireAnotherID != value)
                {
                    OnURP_FE_VasculaireAnotherIDChanging(value);
                    _URP_FE_VasculaireAnotherID = value;
                    RaisePropertyChanged("URP_FE_VasculaireAnotherID");
                    OnURP_FE_VasculaireAnotherIDChanged();
                }
            }
        }
        private long _URP_FE_VasculaireAnotherID;
        partial void OnURP_FE_VasculaireAnotherIDChanging(long value);
        partial void OnURP_FE_VasculaireAnotherIDChanged();


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
        public string MoTa
        {
            get
            {
                return _MoTa;
            }
            set
            {
                OnMoTaChanging(value);
                _MoTa = value;
                RaisePropertyChanged("MoTa");
                OnMoTaChanged();
            }
        }
        private string _MoTa;
        partial void OnMoTaChanging(string value);
        partial void OnMoTaChanged();




        [DataMemberAttribute()]
        public string KetLuanEx
        {
            get
            {
                return _KetLuanEx;
            }
            set
            {
                OnKetLuanExChanging(value);
                _KetLuanEx = value;
                RaisePropertyChanged("KetLuanEx");
                OnKetLuanExChanged();
            }
        }
        private string _KetLuanEx;
        partial void OnKetLuanExChanging(string value);
        partial void OnKetLuanExChanged();




        [DataMemberAttribute()]
        public long V_MotaEx
        {
            get
            {
                return _V_MotaEx;
            }
            set
            {
                OnV_MotaExChanging(value);
                _V_MotaEx = value;
                RaisePropertyChanged("V_MotaEx");
                OnV_MotaExChanged();
            }
        }
        private long _V_MotaEx;
        partial void OnV_MotaExChanging(long value);
        partial void OnV_MotaExChanged();




        [DataMemberAttribute()]
        public long V_KetLuanEx
        {
            get
            {
                return _V_KetLuanEx;
            }
            set
            {
                OnV_KetLuanExChanging(value);
                _V_KetLuanEx = value;
                RaisePropertyChanged("V_KetLuanEx");
                OnV_KetLuanExChanged();
            }
        }
        private long _V_KetLuanEx;
        partial void OnV_KetLuanExChanging(long value);
        partial void OnV_KetLuanExChanged();



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
                return this.URP_FE_VasculaireAnotherID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
