using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_VasculaireLeg : NotifyChangedBase, IEditableObject
    {
        public URP_FE_VasculaireLeg()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_VasculaireLeg info = obj as URP_FE_VasculaireLeg;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_VasculaireLegID > 0 && this.URP_FE_VasculaireLegID == info.URP_FE_VasculaireLegID;
        }
        private URP_FE_VasculaireLeg _tempURP_FE_VasculaireLeg;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_VasculaireLeg = (URP_FE_VasculaireLeg)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_VasculaireLeg)
                CopyFrom(_tempURP_FE_VasculaireLeg);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_VasculaireLeg p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_VasculaireLeg object.

        /// <param name="URP_FE_VasculaireLegID">Initial value of the URP_FE_VasculaireLegID property.</param>
        /// <param name="URP_FE_VasculaireLegName">Initial value of the URP_FE_VasculaireLegName property.</param>
        public static URP_FE_VasculaireLeg CreateURP_FE_VasculaireLeg(Byte URP_FE_VasculaireLegID, String URP_FE_VasculaireLegName)
        {
            URP_FE_VasculaireLeg URP_FE_VasculaireLeg = new URP_FE_VasculaireLeg();
            URP_FE_VasculaireLeg.URP_FE_VasculaireLegID = URP_FE_VasculaireLegID;
            
            return URP_FE_VasculaireLeg;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_VasculaireLegID
        {
            get
            {
                return _URP_FE_VasculaireLegID;
            }
            set
            {
                if (_URP_FE_VasculaireLegID != value)
                {
                    OnURP_FE_VasculaireLegIDChanging(value);
                    _URP_FE_VasculaireLegID = value;
                    RaisePropertyChanged("URP_FE_VasculaireLegID");
                    OnURP_FE_VasculaireLegIDChanged();
                }
            }
        }
        private long _URP_FE_VasculaireLegID;
        partial void OnURP_FE_VasculaireLegIDChanging(long value);
        partial void OnURP_FE_VasculaireLegIDChanged();

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
        public double CT_EI_P
        {
            get
            {
                return _CT_EI_P;
            }
            set
            {
                OnCT_EI_PChanging(value);
                _CT_EI_P = value;
                RaisePropertyChanged("CT_EI_P");
                OnCT_EI_PChanged();
            }
        }
        private double _CT_EI_P;
        partial void OnCT_EI_PChanging(double value);
        partial void OnCT_EI_PChanged();




        [DataMemberAttribute()]
        public double CT_EI_T
        {
            get
            {
                return _CT_EI_T;
            }
            set
            {
                OnCT_EI_TChanging(value);
                _CT_EI_T = value;
                RaisePropertyChanged("CT_EI_T");
                OnCT_EI_TChanged();
            }
        }
        private double _CT_EI_T;
        partial void OnCT_EI_TChanging(double value);
        partial void OnCT_EI_TChanged();




        [DataMemberAttribute()]
        public double CT_CF_P
        {
            get
            {
                return _CT_CF_P;
            }
            set
            {
                OnCT_CF_PChanging(value);
                _CT_CF_P = value;
                RaisePropertyChanged("CT_CF_P");
                OnCT_CF_PChanged();
            }
        }
        private double _CT_CF_P;
        partial void OnCT_CF_PChanging(double value);
        partial void OnCT_CF_PChanged();




        [DataMemberAttribute()]
        public double CT_CF_T
        {
            get
            {
                return _CT_CF_T;
            }
            set
            {
                OnCT_CF_TChanging(value);
                _CT_CF_T = value;
                RaisePropertyChanged("CT_CF_T");
                OnCT_CF_TChanged();
            }
        }
        private double _CT_CF_T;
        partial void OnCT_CF_TChanging(double value);
        partial void OnCT_CF_TChanged();




        [DataMemberAttribute()]
        public double CT_SF_P
        {
            get
            {
                return _CT_SF_P;
            }
            set
            {
                OnCT_SF_PChanging(value);
                _CT_SF_P = value;
                RaisePropertyChanged("CT_SF_P");
                OnCT_SF_PChanged();
            }
        }
        private double _CT_SF_P;
        partial void OnCT_SF_PChanging(double value);
        partial void OnCT_SF_PChanged();




        [DataMemberAttribute()]
        public double CT_SF_T
        {
            get
            {
                return _CT_SF_T;
            }
            set
            {
                OnCT_SF_TChanging(value);
                _CT_SF_T = value;
                RaisePropertyChanged("CT_SF_T");
                OnCT_SF_TChanged();
            }
        }
        private double _CT_SF_T;
        partial void OnCT_SF_TChanging(double value);
        partial void OnCT_SF_TChanged();




        [DataMemberAttribute()]
        public double CT_POP_P
        {
            get
            {
                return _CT_POP_P;
            }
            set
            {
                OnCT_POP_PChanging(value);
                _CT_POP_P = value;
                RaisePropertyChanged("CT_POP_P");
                OnCT_POP_PChanged();
            }
        }
        private double _CT_POP_P;
        partial void OnCT_POP_PChanging(double value);
        partial void OnCT_POP_PChanged();




        [DataMemberAttribute()]
        public double CT_POP_T
        {
            get
            {
                return _CT_POP_T;
            }
            set
            {
                OnCT_POP_TChanging(value);
                _CT_POP_T = value;
                RaisePropertyChanged("CT_POP_T");
                OnCT_POP_TChanged();
            }
        }
        private double _CT_POP_T;
        partial void OnCT_POP_TChanging(double value);
        partial void OnCT_POP_TChanged();




        [DataMemberAttribute()]
        public double CT_AT_P
        {
            get
            {
                return _CT_AT_P;
            }
            set
            {
                OnCT_AT_PChanging(value);
                _CT_AT_P = value;
                RaisePropertyChanged("CT_AT_P");
                OnCT_AT_PChanged();
            }
        }
        private double _CT_AT_P;
        partial void OnCT_AT_PChanging(double value);
        partial void OnCT_AT_PChanged();




        [DataMemberAttribute()]
        public double CT_AT_T
        {
            get
            {
                return _CT_AT_T;
            }
            set
            {
                OnCT_AT_TChanging(value);
                _CT_AT_T = value;
                RaisePropertyChanged("CT_AT_T");
                OnCT_AT_TChanged();
            }
        }
        private double _CT_AT_T;
        partial void OnCT_AT_TChanging(double value);
        partial void OnCT_AT_TChanged();




        [DataMemberAttribute()]
        public double CT_PER_P
        {
            get
            {
                return _CT_PER_P;
            }
            set
            {
                OnCT_PER_PChanging(value);
                _CT_PER_P = value;
                RaisePropertyChanged("CT_PER_P");
                OnCT_PER_PChanged();
            }
        }
        private double _CT_PER_P;
        partial void OnCT_PER_PChanging(double value);
        partial void OnCT_PER_PChanged();




        [DataMemberAttribute()]
        public double CT_PER_T
        {
            get
            {
                return _CT_PER_T;
            }
            set
            {
                OnCT_PER_TChanging(value);
                _CT_PER_T = value;
                RaisePropertyChanged("CT_PER_T");
                OnCT_PER_TChanged();
            }
        }
        private double _CT_PER_T;
        partial void OnCT_PER_TChanging(double value);
        partial void OnCT_PER_TChanged();




        [DataMemberAttribute()]
        public double CT_GrSaph_P
        {
            get
            {
                return _CT_GrSaph_P;
            }
            set
            {
                OnCT_GrSaph_PChanging(value);
                _CT_GrSaph_P = value;
                RaisePropertyChanged("CT_GrSaph_P");
                OnCT_GrSaph_PChanged();
            }
        }
        private double _CT_GrSaph_P;
        partial void OnCT_GrSaph_PChanging(double value);
        partial void OnCT_GrSaph_PChanged();




        [DataMemberAttribute()]
        public double CT_GrSaph_T
        {
            get
            {
                return _CT_GrSaph_T;
            }
            set
            {
                OnCT_GrSaph_TChanging(value);
                _CT_GrSaph_T = value;
                RaisePropertyChanged("CT_GrSaph_T");
                OnCT_GrSaph_TChanged();
            }
        }
        private double _CT_GrSaph_T;
        partial void OnCT_GrSaph_TChanging(double value);
        partial void OnCT_GrSaph_TChanged();




        [DataMemberAttribute()]
        public double CT_PT_P
        {
            get
            {
                return _CT_PT_P;
            }
            set
            {
                OnCT_PT_PChanging(value);
                _CT_PT_P = value;
                RaisePropertyChanged("CT_PT_P");
                OnCT_PT_PChanged();
            }
        }
        private double _CT_PT_P;
        partial void OnCT_PT_PChanging(double value);
        partial void OnCT_PT_PChanged();




        [DataMemberAttribute()]
        public double CT_PT_T
        {
            get
            {
                return _CT_PT_T;
            }
            set
            {
                OnCT_PT_TChanging(value);
                _CT_PT_T = value;
                RaisePropertyChanged("CT_PT_T");
                OnCT_PT_TChanged();
            }
        }
        private double _CT_PT_T;
        partial void OnCT_PT_TChanging(double value);
        partial void OnCT_PT_TChanged();




        [DataMemberAttribute()]
        public double CD_EI_P
        {
            get
            {
                return _CD_EI_P;
            }
            set
            {
                OnCD_EI_PChanging(value);
                _CD_EI_P = value;
                RaisePropertyChanged("CD_EI_P");
                OnCD_EI_PChanged();
            }
        }
        private double _CD_EI_P;
        partial void OnCD_EI_PChanging(double value);
        partial void OnCD_EI_PChanged();




        [DataMemberAttribute()]
        public double CD_EI_T
        {
            get
            {
                return _CD_EI_T;
            }
            set
            {
                OnCD_EI_TChanging(value);
                _CD_EI_T = value;
                RaisePropertyChanged("CD_EI_T");
                OnCD_EI_TChanged();
            }
        }
        private double _CD_EI_T;
        partial void OnCD_EI_TChanging(double value);
        partial void OnCD_EI_TChanged();




        [DataMemberAttribute()]
        public double CD_CF_P
        {
            get
            {
                return _CD_CF_P;
            }
            set
            {
                OnCD_CF_PChanging(value);
                _CD_CF_P = value;
                RaisePropertyChanged("CD_CF_P");
                OnCD_CF_PChanged();
            }
        }
        private double _CD_CF_P;
        partial void OnCD_CF_PChanging(double value);
        partial void OnCD_CF_PChanged();




        [DataMemberAttribute()]
        public double CD_CF_T
        {
            get
            {
                return _CD_CF_T;
            }
            set
            {
                OnCD_CF_TChanging(value);
                _CD_CF_T = value;
                RaisePropertyChanged("CD_CF_T");
                OnCD_CF_TChanged();
            }
        }
        private double _CD_CF_T;
        partial void OnCD_CF_TChanging(double value);
        partial void OnCD_CF_TChanged();




        [DataMemberAttribute()]
        public double CD_SF_P
        {
            get
            {
                return _CD_SF_P;
            }
            set
            {
                OnCD_SF_PChanging(value);
                _CD_SF_P = value;
                RaisePropertyChanged("CD_SF_P");
                OnCD_SF_PChanged();
            }
        }
        private double _CD_SF_P;
        partial void OnCD_SF_PChanging(double value);
        partial void OnCD_SF_PChanged();




        [DataMemberAttribute()]
        public double CD_SF_T
        {
            get
            {
                return _CD_SF_T;
            }
            set
            {
                OnCD_SF_TChanging(value);
                _CD_SF_T = value;
                RaisePropertyChanged("CD_SF_T");
                OnCD_SF_TChanged();
            }
        }
        private double _CD_SF_T;
        partial void OnCD_SF_TChanging(double value);
        partial void OnCD_SF_TChanged();




        [DataMemberAttribute()]
        public double CD_POP_P
        {
            get
            {
                return _CD_POP_P;
            }
            set
            {
                OnCD_POP_PChanging(value);
                _CD_POP_P = value;
                RaisePropertyChanged("CD_POP_P");
                OnCD_POP_PChanged();
            }
        }
        private double _CD_POP_P;
        partial void OnCD_POP_PChanging(double value);
        partial void OnCD_POP_PChanged();




        [DataMemberAttribute()]
        public double CD_POP_T
        {
            get
            {
                return _CD_POP_T;
            }
            set
            {
                OnCD_POP_TChanging(value);
                _CD_POP_T = value;
                RaisePropertyChanged("CD_POP_T");
                OnCD_POP_TChanged();
            }
        }
        private double _CD_POP_T;
        partial void OnCD_POP_TChanging(double value);
        partial void OnCD_POP_TChanged();




        [DataMemberAttribute()]
        public double CD_AT_P
        {
            get
            {
                return _CD_AT_P;
            }
            set
            {
                OnCD_AT_PChanging(value);
                _CD_AT_P = value;
                RaisePropertyChanged("CD_AT_P");
                OnCD_AT_PChanged();
            }
        }
        private double _CD_AT_P;
        partial void OnCD_AT_PChanging(double value);
        partial void OnCD_AT_PChanged();




        [DataMemberAttribute()]
        public double CD_AT_T
        {
            get
            {
                return _CD_AT_T;
            }
            set
            {
                OnCD_AT_TChanging(value);
                _CD_AT_T = value;
                RaisePropertyChanged("CD_AT_T");
                OnCD_AT_TChanged();
            }
        }
        private double _CD_AT_T;
        partial void OnCD_AT_TChanging(double value);
        partial void OnCD_AT_TChanged();




        [DataMemberAttribute()]
        public double CD_PER_P
        {
            get
            {
                return _CD_PER_P;
            }
            set
            {
                OnCD_PER_PChanging(value);
                _CD_PER_P = value;
                RaisePropertyChanged("CD_PER_P");
                OnCD_PER_PChanged();
            }
        }
        private double _CD_PER_P;
        partial void OnCD_PER_PChanging(double value);
        partial void OnCD_PER_PChanged();




        [DataMemberAttribute()]
        public double CD_PER_T
        {
            get
            {
                return _CD_PER_T;
            }
            set
            {
                OnCD_PER_TChanging(value);
                _CD_PER_T = value;
                RaisePropertyChanged("CD_PER_T");
                OnCD_PER_TChanged();
            }
        }
        private double _CD_PER_T;
        partial void OnCD_PER_TChanging(double value);
        partial void OnCD_PER_TChanged();




        [DataMemberAttribute()]
        public double CD_GrSaph_P
        {
            get
            {
                return _CD_GrSaph_P;
            }
            set
            {
                OnCD_GrSaph_PChanging(value);
                _CD_GrSaph_P = value;
                RaisePropertyChanged("CD_GrSaph_P");
                OnCD_GrSaph_PChanged();
            }
        }
        private double _CD_GrSaph_P;
        partial void OnCD_GrSaph_PChanging(double value);
        partial void OnCD_GrSaph_PChanged();




        [DataMemberAttribute()]
        public double CD_GrSaph_T
        {
            get
            {
                return _CD_GrSaph_T;
            }
            set
            {
                OnCD_GrSaph_TChanging(value);
                _CD_GrSaph_T = value;
                RaisePropertyChanged("CD_GrSaph_T");
                OnCD_GrSaph_TChanged();
            }
        }
        private double _CD_GrSaph_T;
        partial void OnCD_GrSaph_TChanging(double value);
        partial void OnCD_GrSaph_TChanged();




        [DataMemberAttribute()]
        public double CD_PT_P
        {
            get
            {
                return _CD_PT_P;
            }
            set
            {
                OnCD_PT_PChanging(value);
                _CD_PT_P = value;
                RaisePropertyChanged("CD_PT_P");
                OnCD_PT_PChanged();
            }
        }
        private double _CD_PT_P;
        partial void OnCD_PT_PChanging(double value);
        partial void OnCD_PT_PChanged();




        [DataMemberAttribute()]
        public double CD_PT_T
        {
            get
            {
                return _CD_PT_T;
            }
            set
            {
                OnCD_PT_TChanging(value);
                _CD_PT_T = value;
                RaisePropertyChanged("CD_PT_T");
                OnCD_PT_TChanged();
            }
        }
        private double _CD_PT_T;
        partial void OnCD_PT_TChanging(double value);
        partial void OnCD_PT_TChanged();




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
    }
}
