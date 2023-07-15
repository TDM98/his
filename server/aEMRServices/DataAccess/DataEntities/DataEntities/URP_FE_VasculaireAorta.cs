using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_VasculaireAorta : NotifyChangedBase, IEditableObject
    {
        public URP_FE_VasculaireAorta()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_VasculaireAorta info = obj as URP_FE_VasculaireAorta;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_VasculaireAortaID > 0 && this.URP_FE_VasculaireAortaID == info.URP_FE_VasculaireAortaID;
        }
        private URP_FE_VasculaireAorta _tempURP_FE_VasculaireAorta;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_VasculaireAorta = (URP_FE_VasculaireAorta)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_VasculaireAorta)
                CopyFrom(_tempURP_FE_VasculaireAorta);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_VasculaireAorta p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_VasculaireAorta object.

        /// <param name="URP_FE_VasculaireAortaID">Initial value of the URP_FE_VasculaireAortaID property.</param>
        /// <param name="URP_FE_VasculaireAortaName">Initial value of the URP_FE_VasculaireAortaName property.</param>
        public static URP_FE_VasculaireAorta CreateURP_FE_VasculaireAorta(Byte URP_FE_VasculaireAortaID, String URP_FE_VasculaireAortaName)
        {
            URP_FE_VasculaireAorta URP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            URP_FE_VasculaireAorta.URP_FE_VasculaireAortaID = URP_FE_VasculaireAortaID;
            
            return URP_FE_VasculaireAorta;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_VasculaireAortaID
        {
            get
            {
                return _URP_FE_VasculaireAortaID;
            }
            set
            {
                if (_URP_FE_VasculaireAortaID != value)
                {
                    OnURP_FE_VasculaireAortaIDChanging(value);
                    _URP_FE_VasculaireAortaID = value;
                    RaisePropertyChanged("URP_FE_VasculaireAortaID");
                    OnURP_FE_VasculaireAortaIDChanged();
                }
            }
        }
        private long _URP_FE_VasculaireAortaID;
        partial void OnURP_FE_VasculaireAortaIDChanging(long value);
        partial void OnURP_FE_VasculaireAortaIDChanged();

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
        public double DMCNgang
        {
            get
            {
                return _DMCNgang;
            }
            set
            {
                OnDMCNgangChanging(value);
                _DMCNgang = value;
                RaisePropertyChanged("DMCNgang");
                OnDMCNgangChanged();
            }
        }
        private double _DMCNgang;
        partial void OnDMCNgangChanging(double value);
        partial void OnDMCNgangChanged();




        [DataMemberAttribute()]
        public double DMCLen
        {
            get
            {
                return _DMCLen;
            }
            set
            {
                OnDMCLenChanging(value);
                _DMCLen = value;
                RaisePropertyChanged("DMCLen");
                OnDMCLenChanged();
            }
        }
        private double _DMCLen;
        partial void OnDMCLenChanging(double value);
        partial void OnDMCLenChanged();




        [DataMemberAttribute()]
        public double EoDMC
        {
            get
            {
                return _EoDMC;
            }
            set
            {
                OnEoDMCChanging(value);
                _EoDMC = value;
                RaisePropertyChanged("EoDMC");
                OnEoDMCChanged();
            }
        }
        private double _EoDMC;
        partial void OnEoDMCChanging(double value);
        partial void OnEoDMCChanged();




        [DataMemberAttribute()]
        public double DMCXuong
        {
            get
            {
                return _DMCXuong;
            }
            set
            {
                OnDMCXuongChanging(value);
                _DMCXuong = value;
                RaisePropertyChanged("DMCXuong");
                OnDMCXuongChanged();
            }
        }
        private double _DMCXuong;
        partial void OnDMCXuongChanging(double value);
        partial void OnDMCXuongChanged();




        [DataMemberAttribute()]
        public double DMThanP_v
        {
            get
            {
                return _DMThanP_v;
            }
            set
            {
                OnDMThanP_vChanging(value);
                _DMThanP_v = value;
                RaisePropertyChanged("DMThanP_v");
                OnDMThanP_vChanged();
            }
        }
        private double _DMThanP_v;
        partial void OnDMThanP_vChanging(double value);
        partial void OnDMThanP_vChanged();




        [DataMemberAttribute()]
        public double DMThanP_RI
        {
            get
            {
                return _DMThanP_RI;
            }
            set
            {
                OnDMThanP_RIChanging(value);
                _DMThanP_RI = value;
                RaisePropertyChanged("DMThanP_RI");
                OnDMThanP_RIChanged();
            }
        }
        private double _DMThanP_RI;
        partial void OnDMThanP_RIChanging(double value);
        partial void OnDMThanP_RIChanged();




        [DataMemberAttribute()]
        public double DMThanT_v
        {
            get
            {
                return _DMThanT_v;
            }
            set
            {
                OnDMThanT_vChanging(value);
                _DMThanT_v = value;
                RaisePropertyChanged("DMThanT_v");
                OnDMThanT_vChanged();
            }
        }
        private double _DMThanT_v;
        partial void OnDMThanT_vChanging(double value);
        partial void OnDMThanT_vChanged();




        [DataMemberAttribute()]
        public double DMThanT_RI
        {
            get
            {
                return _DMThanT_RI;
            }
            set
            {
                OnDMThanT_RIChanging(value);
                _DMThanT_RI = value;
                RaisePropertyChanged("DMThanT_RI");
                OnDMThanT_RIChanged();
            }
        }
        private double _DMThanT_RI;
        partial void OnDMThanT_RIChanging(double value);
        partial void OnDMThanT_RIChanged();




        [DataMemberAttribute()]
        public double DMChauP_v
        {
            get
            {
                return _DMChauP_v;
            }
            set
            {
                OnDMChauP_vChanging(value);
                _DMChauP_v = value;
                RaisePropertyChanged("DMChauP_v");
                OnDMChauP_vChanged();
            }
        }
        private double _DMChauP_v;
        partial void OnDMChauP_vChanging(double value);
        partial void OnDMChauP_vChanged();




        [DataMemberAttribute()]
        public double DMChauT_v
        {
            get
            {
                return _DMChauT_v;
            }
            set
            {
                OnDMChauT_vChanging(value);
                _DMChauT_v = value;
                RaisePropertyChanged("DMChauT_v");
                OnDMChauT_vChanged();
            }
        }
        private double _DMChauT_v;
        partial void OnDMChauT_vChanging(double value);
        partial void OnDMChauT_vChanged();




        [DataMemberAttribute()]
        public string KetLuan
        {
            get
            {
                return _KetLuan;
            }
            set
            {
                OnKetLuanChanging(value);
                _KetLuan = value;
                RaisePropertyChanged("KetLuan");
                OnKetLuanChanged();
            }
        }
        private string _KetLuan;
        partial void OnKetLuanChanging(string value);
        partial void OnKetLuanChanged();




        [DataMemberAttribute()]
        public long V_KetLuan
        {
            get
            {
                return _V_KetLuan;
            }
            set
            {
                OnV_KetLuanChanging(value);
                _V_KetLuan = value;
                RaisePropertyChanged("V_KetLuan");
                OnV_KetLuanChanged();
            }
        }
        private long _V_KetLuan;
        partial void OnV_KetLuanChanging(long value);
        partial void OnV_KetLuanChanged();



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
                return this.URP_FE_VasculaireAortaID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
