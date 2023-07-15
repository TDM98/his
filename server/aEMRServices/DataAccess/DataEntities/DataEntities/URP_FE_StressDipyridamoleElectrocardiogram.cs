using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDipyridamoleElectrocardiogram : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDipyridamoleElectrocardiogram()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDipyridamoleElectrocardiogram info = obj as URP_FE_StressDipyridamoleElectrocardiogram;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDipyridamoleElectrocardiogramID > 0 && this.URP_FE_StressDipyridamoleElectrocardiogramID == info.URP_FE_StressDipyridamoleElectrocardiogramID;
        }
        private URP_FE_StressDipyridamoleElectrocardiogram _tempURP_FE_StressDipyridamoleElectrocardiogram;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDipyridamoleElectrocardiogram = (URP_FE_StressDipyridamoleElectrocardiogram)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDipyridamoleElectrocardiogram)
                CopyFrom(_tempURP_FE_StressDipyridamoleElectrocardiogram);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDipyridamoleElectrocardiogram p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDipyridamoleElectrocardiogram object.

        /// <param name="URP_FE_StressDipyridamoleElectrocardiogramID">Initial value of the URP_FE_StressDipyridamoleElectrocardiogramID property.</param>
        /// <param name="URP_FE_StressDipyridamoleElectrocardiogramName">Initial value of the URP_FE_StressDipyridamoleElectrocardiogramName property.</param>
        public static URP_FE_StressDipyridamoleElectrocardiogram CreateURP_FE_StressDipyridamoleElectrocardiogram(Byte URP_FE_StressDipyridamoleElectrocardiogramID, String URP_FE_StressDipyridamoleElectrocardiogramName)
        {
            URP_FE_StressDipyridamoleElectrocardiogram URP_FE_StressDipyridamoleElectrocardiogram = new URP_FE_StressDipyridamoleElectrocardiogram();
            URP_FE_StressDipyridamoleElectrocardiogram.URP_FE_StressDipyridamoleElectrocardiogramID = URP_FE_StressDipyridamoleElectrocardiogramID;
            
            return URP_FE_StressDipyridamoleElectrocardiogram;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDipyridamoleElectrocardiogramID
        {
            get
            {
                return _URP_FE_StressDipyridamoleElectrocardiogramID;
            }
            set
            {
                if (_URP_FE_StressDipyridamoleElectrocardiogramID != value)
                {
                    OnURP_FE_StressDipyridamoleElectrocardiogramIDChanging(value);
                    _URP_FE_StressDipyridamoleElectrocardiogramID = value;
                    RaisePropertyChanged("URP_FE_StressDipyridamoleElectrocardiogramID");
                    OnURP_FE_StressDipyridamoleElectrocardiogramIDChanged();
                }
            }
        }
        private long _URP_FE_StressDipyridamoleElectrocardiogramID;
        partial void OnURP_FE_StressDipyridamoleElectrocardiogramIDChanging(long value);
        partial void OnURP_FE_StressDipyridamoleElectrocardiogramIDChanged();

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
        public bool DieuTriConDauThatNguc
        {
            get
            {
                return _DieuTriConDauThatNguc;
            }
            set
            {
                OnDieuTriConDauThatNgucChanging(value);
                _DieuTriConDauThatNguc = value;
                RaisePropertyChanged("DieuTriConDauThatNguc");
                OnDieuTriConDauThatNgucChanged();
            }
        }
        private bool _DieuTriConDauThatNguc;
        partial void OnDieuTriConDauThatNgucChanging(bool value);
        partial void OnDieuTriConDauThatNgucChanged();




        [DataMemberAttribute()]
        public bool DieuTriDIGITALIS
        {
            get
            {
                return _DieuTriDIGITALIS;
            }
            set
            {
                OnDieuTriDIGITALISChanging(value);
                _DieuTriDIGITALIS = value;
                RaisePropertyChanged("DieuTriDIGITALIS");
                OnDieuTriDIGITALISChanged();
            }
        }
        private bool _DieuTriDIGITALIS;
        partial void OnDieuTriDIGITALISChanging(bool value);
        partial void OnDieuTriDIGITALISChanged();




        [DataMemberAttribute()]
        public string LyDoKhongThucHienDuoc
        {
            get
            {
                return _LyDoKhongThucHienDuoc;
            }
            set
            {
                OnLyDoKhongThucHienDuocChanging(value);
                _LyDoKhongThucHienDuoc = value;
                RaisePropertyChanged("LyDoKhongThucHienDuoc");
                OnLyDoKhongThucHienDuocChanged();
            }
        }
        private string _LyDoKhongThucHienDuoc;
        partial void OnLyDoKhongThucHienDuocChanging(string value);
        partial void OnLyDoKhongThucHienDuocChanged();




        [DataMemberAttribute()]
        public double MucGangSuc
        {
            get
            {
                return _MucGangSuc;
            }
            set
            {
                OnMucGangSucChanging(value);
                _MucGangSuc = value;
                RaisePropertyChanged("MucGangSuc");
                OnMucGangSucChanged();
            }
        }
        private double _MucGangSuc;
        partial void OnMucGangSucChanging(double value);
        partial void OnMucGangSucChanged();




        [DataMemberAttribute()]
        public double ThoiGianGangSuc
        {
            get
            {
                return _ThoiGianGangSuc;
            }
            set
            {
                OnThoiGianGangSucChanging(value);
                _ThoiGianGangSuc = value;
                RaisePropertyChanged("ThoiGianGangSuc");
                OnThoiGianGangSucChanged();
            }
        }
        private double _ThoiGianGangSuc;
        partial void OnThoiGianGangSucChanging(double value);
        partial void OnThoiGianGangSucChanged();




        [DataMemberAttribute()]
        public double TanSoTim
        {
            get
            {
                return _TanSoTim;
            }
            set
            {
                OnTanSoTimChanging(value);
                _TanSoTim = value;
                RaisePropertyChanged("TanSoTim");
                OnTanSoTimChanged();
            }
        }
        private double _TanSoTim;
        partial void OnTanSoTimChanging(double value);
        partial void OnTanSoTimChanged();




        [DataMemberAttribute()]
        public double HuyetApToiDa
        {
            get
            {
                return _HuyetApToiDa;
            }
            set
            {
                OnHuyetApToiDaChanging(value);
                _HuyetApToiDa = value;
                RaisePropertyChanged("HuyetApToiDa");
                OnHuyetApToiDaChanged();
            }
        }
        private double _HuyetApToiDa;
        partial void OnHuyetApToiDaChanging(double value);
        partial void OnHuyetApToiDaChanged();




        [DataMemberAttribute()]
        public int ConDauThatNguc
        {
            get
            {
                return _ConDauThatNguc;
            }
            set
            {
                OnConDauThatNgucChanging(value);
                _ConDauThatNguc = value;
                RaisePropertyChanged("ConDauThatNguc");
                V_ConDauThatNguc = (AllLookupValues.ChoiceEnum)_ConDauThatNguc;
                OnConDauThatNgucChanged();
            }
        }
        private int _ConDauThatNguc;
        partial void OnConDauThatNgucChanging(int value);
        partial void OnConDauThatNgucChanged();




        [DataMemberAttribute()]
        public string STChenhXuong
        {
            get
            {
                return _STChenhXuong;
            }
            set
            {
                OnSTChenhXuongChanging(value);
                _STChenhXuong = value;
                RaisePropertyChanged("STChenhXuong");
                OnSTChenhXuongChanged();
            }
        }
        private string _STChenhXuong;
        partial void OnSTChenhXuongChanging(string value);
        partial void OnSTChenhXuongChanged();




        [DataMemberAttribute()]
        public bool RoiLoanNhipTim
        {
            get
            {
                return _RoiLoanNhipTim;
            }
            set
            {
                OnRoiLoanNhipTimChanging(value);
                _RoiLoanNhipTim = value;
                RaisePropertyChanged("RoiLoanNhipTim");
                OnRoiLoanNhipTimChanged();
            }
        }
        private bool _RoiLoanNhipTim;
        partial void OnRoiLoanNhipTimChanging(bool value);
        partial void OnRoiLoanNhipTimChanged();




        [DataMemberAttribute()]
        public string RoiLoanNhipTimChiTiet
        {
            get
            {
                return _RoiLoanNhipTimChiTiet;
            }
            set
            {
                OnRoiLoanNhipTimChiTietChanging(value);
                _RoiLoanNhipTimChiTiet = value;
                RaisePropertyChanged("RoiLoanNhipTimChiTiet");
                OnRoiLoanNhipTimChiTietChanged();
            }
        }
        private string _RoiLoanNhipTimChiTiet;
        partial void OnRoiLoanNhipTimChiTietChanging(string value);
        partial void OnRoiLoanNhipTimChiTietChanged();




        [DataMemberAttribute()]
        public string XetNghiemKhac
        {
            get
            {
                return _XetNghiemKhac;
            }
            set
            {
                OnXetNghiemKhacChanging(value);
                _XetNghiemKhac = value;
                RaisePropertyChanged("XetNghiemKhac");
                OnXetNghiemKhacChanged();
            }
        }
        private string _XetNghiemKhac;
        partial void OnXetNghiemKhacChanging(string value);
        partial void OnXetNghiemKhacChanged();

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

        [DataMemberAttribute()]
        public AllLookupValues.ChoiceEnum V_ConDauThatNguc
        {
            get
            {
                return _V_ConDauThatNguc;
            }
            set
            {
                if (_V_ConDauThatNguc != value)
                {
                    OnV_ConDauThatNgucChanging(value);
                    _V_ConDauThatNguc = value;
                    RaisePropertyChanged("V_ConDauThatNguc");
                    ConDauThatNguc = (int)_V_ConDauThatNguc;
                    OnV_ConDauThatNgucChanged();
                }
            }
        }
        private AllLookupValues.ChoiceEnum _V_ConDauThatNguc;
        partial void OnV_ConDauThatNgucChanging(AllLookupValues.ChoiceEnum value);
        partial void OnV_ConDauThatNgucChanged();
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
