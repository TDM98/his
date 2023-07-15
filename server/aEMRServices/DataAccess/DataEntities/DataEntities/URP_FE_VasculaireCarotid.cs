using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_VasculaireCarotid : NotifyChangedBase, IEditableObject
    {
        public URP_FE_VasculaireCarotid()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_VasculaireCarotid info = obj as URP_FE_VasculaireCarotid;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_VasculaireCarotidID > 0 && this.URP_FE_VasculaireCarotidID == info.URP_FE_VasculaireCarotidID;
        }
        private URP_FE_VasculaireCarotid _tempURP_FE_VasculaireCarotid;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_VasculaireCarotid = (URP_FE_VasculaireCarotid)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_VasculaireCarotid)
                CopyFrom(_tempURP_FE_VasculaireCarotid);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_VasculaireCarotid p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_VasculaireCarotid object.

        /// <param name="URP_FE_VasculaireCarotidID">Initial value of the URP_FE_VasculaireCarotidID property.</param>
        /// <param name="URP_FE_VasculaireCarotidName">Initial value of the URP_FE_VasculaireCarotidName property.</param>
        public static URP_FE_VasculaireCarotid CreateURP_FE_VasculaireCarotid(Byte URP_FE_VasculaireCarotidID, String URP_FE_VasculaireCarotidName)
        {
            URP_FE_VasculaireCarotid URP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
            URP_FE_VasculaireCarotid.URP_FE_VasculaireCarotidID = URP_FE_VasculaireCarotidID;
            
            return URP_FE_VasculaireCarotid;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_VasculaireCarotidID
        {
            get
            {
                return _URP_FE_VasculaireCarotidID;
            }
            set
            {
                if (_URP_FE_VasculaireCarotidID != value)
                {
                    OnURP_FE_VasculaireCarotidIDChanging(value);
                    _URP_FE_VasculaireCarotidID = value;
                    RaisePropertyChanged("URP_FE_VasculaireCarotidID");
                    OnURP_FE_VasculaireCarotidIDChanged();
                }
            }
        }
        private long _URP_FE_VasculaireCarotidID;
        partial void OnURP_FE_VasculaireCarotidIDChanging(long value);
        partial void OnURP_FE_VasculaireCarotidIDChanged();

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
        public long V_KetQuaSAMMTruoc
        {
            get
            {
                return _V_KetQuaSAMMTruoc;
            }
            set
            {
                OnV_KetQuaSAMMTruocChanging(value);
                _V_KetQuaSAMMTruoc = value;
                RaisePropertyChanged("V_KetQuaSAMMTruoc");
                OnV_KetQuaSAMMTruocChanged();
            }
        }
        private long _V_KetQuaSAMMTruoc;
        partial void OnV_KetQuaSAMMTruocChanging(long value);
        partial void OnV_KetQuaSAMMTruocChanged();




        [DataMemberAttribute()]
        public string KetQuaSAMMTruoc
        {
            get
            {
                return _KetQuaSAMMTruoc;
            }
            set
            {
                OnKetQuaSAMMTruocChanging(value);
                _KetQuaSAMMTruoc = value;
                RaisePropertyChanged("KetQuaSAMMTruoc");
                OnKetQuaSAMMTruocChanged();
            }
        }
        private string _KetQuaSAMMTruoc;
        partial void OnKetQuaSAMMTruocChanging(string value);
        partial void OnKetQuaSAMMTruocChanged();




        [DataMemberAttribute()]
        public double DMCP_ECA
        {
            get
            {
                return _DMCP_ECA;
            }
            set
            {
                OnDMCP_ECAChanging(value);
                _DMCP_ECA = value;
                RaisePropertyChanged("DMCP_ECA");
                OnDMCP_ECAChanged();
            }
        }
        private double _DMCP_ECA;
        partial void OnDMCP_ECAChanging(double value);
        partial void OnDMCP_ECAChanged();




        [DataMemberAttribute()]
        public double DMCP_ICA
        {
            get
            {
                return _DMCP_ICA;
            }
            set
            {
                OnDMCP_ICAChanging(value);
                _DMCP_ICA = value;
                RaisePropertyChanged("DMCP_ICA");
                OnDMCP_ICAChanged();
            }
        }
        private double _DMCP_ICA;
        partial void OnDMCP_ICAChanging(double value);
        partial void OnDMCP_ICAChanged();




        [DataMemberAttribute()]
        public double DMCP_ICA_SR
        {
            get
            {
                return _DMCP_ICA_SR;
            }
            set
            {
                OnDMCP_ICA_SRChanging(value);
                _DMCP_ICA_SR = value;
                RaisePropertyChanged("DMCP_ICA_SR");
                OnDMCP_ICA_SRChanged();
            }
        }
        private double _DMCP_ICA_SR;
        partial void OnDMCP_ICA_SRChanging(double value);
        partial void OnDMCP_ICA_SRChanged();




        [DataMemberAttribute()]
        public double DMCP_CCA_TCC
        {
            get
            {
                return _DMCP_CCA_TCC;
            }
            set
            {
                OnDMCP_CCA_TCCChanging(value);
                _DMCP_CCA_TCC = value;
                RaisePropertyChanged("DMCP_CCA_TCC");
                OnDMCP_CCA_TCCChanged();
            }
        }
        private double _DMCP_CCA_TCC;
        partial void OnDMCP_CCA_TCCChanging(double value);
        partial void OnDMCP_CCA_TCCChanged();




        [DataMemberAttribute()]
        public double DMCP_CCA
        {
            get
            {
                return _DMCP_CCA;
            }
            set
            {
                OnDMCP_CCAChanging(value);
                _DMCP_CCA = value;
                RaisePropertyChanged("DMCP_CCA");
                OnDMCP_CCAChanged();
            }
        }
        private double _DMCP_CCA;
        partial void OnDMCP_CCAChanging(double value);
        partial void OnDMCP_CCAChanged();




        [DataMemberAttribute()]
        public double DMCT_ECA
        {
            get
            {
                return _DMCT_ECA;
            }
            set
            {
                OnDMCT_ECAChanging(value);
                _DMCT_ECA = value;
                RaisePropertyChanged("DMCT_ECA");
                OnDMCT_ECAChanged();
            }
        }
        private double _DMCT_ECA;
        partial void OnDMCT_ECAChanging(double value);
        partial void OnDMCT_ECAChanged();




        [DataMemberAttribute()]
        public double DMCT_ICA
        {
            get
            {
                return _DMCT_ICA;
            }
            set
            {
                OnDMCT_ICAChanging(value);
                _DMCT_ICA = value;
                RaisePropertyChanged("DMCT_ICA");
                OnDMCT_ICAChanged();
            }
        }
        private double _DMCT_ICA;
        partial void OnDMCT_ICAChanging(double value);
        partial void OnDMCT_ICAChanged();




        [DataMemberAttribute()]
        public double DMCT_ICA_SR
        {
            get
            {
                return _DMCT_ICA_SR;
            }
            set
            {
                OnDMCT_ICA_SRChanging(value);
                _DMCT_ICA_SR = value;
                RaisePropertyChanged("DMCT_ICA_SR");
                OnDMCT_ICA_SRChanged();
            }
        }
        private double _DMCT_ICA_SR;
        partial void OnDMCT_ICA_SRChanging(double value);
        partial void OnDMCT_ICA_SRChanged();




        [DataMemberAttribute()]
        public double DMCT_CCA_TCC
        {
            get
            {
                return _DMCT_CCA_TCC;
            }
            set
            {
                OnDMCT_CCA_TCCChanging(value);
                _DMCT_CCA_TCC = value;
                RaisePropertyChanged("DMCT_CCA_TCC");
                OnDMCT_CCA_TCCChanged();
            }
        }
        private double _DMCT_CCA_TCC;
        partial void OnDMCT_CCA_TCCChanging(double value);
        partial void OnDMCT_CCA_TCCChanged();




        [DataMemberAttribute()]
        public double DMCT_CCA
        {
            get
            {
                return _DMCT_CCA;
            }
            set
            {
                OnDMCT_CCAChanging(value);
                _DMCT_CCA = value;
                RaisePropertyChanged("DMCT_CCA");
                OnDMCT_CCAChanged();
            }
        }
        private double _DMCT_CCA;
        partial void OnDMCT_CCAChanging(double value);
        partial void OnDMCT_CCAChanged();




        [DataMemberAttribute()]
        public double DMCotSongP_d
        {
            get
            {
                return _DMCotSongP_d;
            }
            set
            {
                OnDMCotSongP_dChanging(value);
                _DMCotSongP_d = value;
                RaisePropertyChanged("DMCotSongP_d");
                OnDMCotSongP_dChanged();
            }
        }
        private double _DMCotSongP_d;
        partial void OnDMCotSongP_dChanging(double value);
        partial void OnDMCotSongP_dChanged();




        [DataMemberAttribute()]
        public double DMCotSongP_r
        {
            get
            {
                return _DMCotSongP_r;
            }
            set
            {
                OnDMCotSongP_rChanging(value);
                _DMCotSongP_r = value;
                RaisePropertyChanged("DMCotSongP_r");
                OnDMCotSongP_rChanged();
            }
        }
        private double _DMCotSongP_r;
        partial void OnDMCotSongP_rChanging(double value);
        partial void OnDMCotSongP_rChanged();




        [DataMemberAttribute()]
        public double DMCotSongT_d
        {
            get
            {
                return _DMCotSongT_d;
            }
            set
            {
                OnDMCotSongT_dChanging(value);
                _DMCotSongT_d = value;
                RaisePropertyChanged("DMCotSongT_d");
                OnDMCotSongT_dChanged();
            }
        }
        private double _DMCotSongT_d;
        partial void OnDMCotSongT_dChanging(double value);
        partial void OnDMCotSongT_dChanged();




        [DataMemberAttribute()]
        public double DMCotSongT_r
        {
            get
            {
                return _DMCotSongT_r;
            }
            set
            {
                OnDMCotSongT_rChanging(value);
                _DMCotSongT_r = value;
                RaisePropertyChanged("DMCotSongT_r");
                OnDMCotSongT_rChanged();
            }
        }
        private double _DMCotSongT_r;
        partial void OnDMCotSongT_rChanging(double value);
        partial void OnDMCotSongT_rChanged();




        [DataMemberAttribute()]
        public double DMDuoiDonP_d
        {
            get
            {
                return _DMDuoiDonP_d;
            }
            set
            {
                OnDMDuoiDonP_dChanging(value);
                _DMDuoiDonP_d = value;
                RaisePropertyChanged("DMDuoiDonP_d");
                OnDMDuoiDonP_dChanged();
            }
        }
        private double _DMDuoiDonP_d;
        partial void OnDMDuoiDonP_dChanging(double value);
        partial void OnDMDuoiDonP_dChanged();




        [DataMemberAttribute()]
        public double DMDuoiDonP_r
        {
            get
            {
                return _DMDuoiDonP_r;
            }
            set
            {
                OnDMDuoiDonP_rChanging(value);
                _DMDuoiDonP_r = value;
                RaisePropertyChanged("DMDuoiDonP_r");
                OnDMDuoiDonP_rChanged();
            }
        }
        private double _DMDuoiDonP_r;
        partial void OnDMDuoiDonP_rChanging(double value);
        partial void OnDMDuoiDonP_rChanged();




        [DataMemberAttribute()]
        public double DMDuoiDonT_d
        {
            get
            {
                return _DMDuoiDonT_d;
            }
            set
            {
                OnDMDuoiDonT_dChanging(value);
                _DMDuoiDonT_d = value;
                RaisePropertyChanged("DMDuoiDonT_d");
                OnDMDuoiDonT_dChanged();
            }
        }
        private double _DMDuoiDonT_d;
        partial void OnDMDuoiDonT_dChanging(double value);
        partial void OnDMDuoiDonT_dChanged();




        [DataMemberAttribute()]
        public double DMDuoiDonT_r
        {
            get
            {
                return _DMDuoiDonT_r;
            }
            set
            {
                OnDMDuoiDonT_rChanging(value);
                _DMDuoiDonT_r = value;
                RaisePropertyChanged("DMDuoiDonT_r");
                OnDMDuoiDonT_rChanged();
            }
        }
        private double _DMDuoiDonT_r;
        partial void OnDMDuoiDonT_rChanging(double value);
        partial void OnDMDuoiDonT_rChanged();




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
                return this.URP_FE_VasculaireCarotidID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
