using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_OesophagienneCheck : NotifyChangedBase, IEditableObject
    {
        public URP_FE_OesophagienneCheck()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_OesophagienneCheck info = obj as URP_FE_OesophagienneCheck;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_OesophagienneCheckID > 0 && this.URP_FE_OesophagienneCheckID == info.URP_FE_OesophagienneCheckID;
        }
        private URP_FE_OesophagienneCheck _tempURP_FE_OesophagienneCheck;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_OesophagienneCheck = (URP_FE_OesophagienneCheck)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_OesophagienneCheck)
                CopyFrom(_tempURP_FE_OesophagienneCheck);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_OesophagienneCheck p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_OesophagienneCheck object.

        /// <param name="URP_FE_OesophagienneCheckID">Initial value of the URP_FE_OesophagienneCheckID property.</param>
        /// <param name="URP_FE_OesophagienneCheckName">Initial value of the URP_FE_OesophagienneCheckName property.</param>
        public static URP_FE_OesophagienneCheck CreateURP_FE_OesophagienneCheck(Byte URP_FE_OesophagienneCheckID, String URP_FE_OesophagienneCheckName)
        {
            URP_FE_OesophagienneCheck URP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
            URP_FE_OesophagienneCheck.URP_FE_OesophagienneCheckID = URP_FE_OesophagienneCheckID;

            return URP_FE_OesophagienneCheck;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_OesophagienneCheckID
        {
            get
            {
                return _URP_FE_OesophagienneCheckID;
            }
            set
            {
                if (_URP_FE_OesophagienneCheckID != value)
                {
                    OnURP_FE_OesophagienneCheckIDChanging(value);
                    _URP_FE_OesophagienneCheckID = value;
                    RaisePropertyChanged("URP_FE_OesophagienneCheckID");
                    OnURP_FE_OesophagienneCheckIDChanged();
                }
            }
        }
        private long _URP_FE_OesophagienneCheckID;
        partial void OnURP_FE_OesophagienneCheckIDChanging(long value);
        partial void OnURP_FE_OesophagienneCheckIDChanged();

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
        public bool CatNghia
        {
            get
            {
                return _CatNghia;
            }
            set
            {
                if (_CatNghia != value)
                {
                    OnCatNghiaChanging(value);
                    _CatNghia = value;
                    RaisePropertyChanged("CatNghia");
                    OnCatNghiaChanged();
                }
            }
        }
        private bool _CatNghia;
        partial void OnCatNghiaChanging(bool value);
        partial void OnCatNghiaChanged();

	    [DataMemberAttribute()]
        public bool NuotNghen
        {
            get
            {
                return _NuotNghen;
            }
            set
            {
                if (_NuotNghen != value)
                {
                    OnNuotNghenChanging(value);
                    _NuotNghen = value;
                    RaisePropertyChanged("NuotNghen");
                    OnNuotNghenChanged();
                }
            }
        }
        private bool _NuotNghen;
        partial void OnNuotNghenChanging(bool value);
        partial void OnNuotNghenChanged();
    
       [DataMemberAttribute()]
        public bool NuotDau
        {
            get
            {
                return _NuotDau;
            }
            set
            {
                if (_NuotDau != value)
                {
                    OnNuotDauChanging(value);
                    _NuotDau = value;
                    RaisePropertyChanged("NuotDau");
                    OnNuotDauChanged();
                }
            }
        }
        private bool _NuotDau;
        partial void OnNuotDauChanging(bool value);
        partial void OnNuotDauChanged();
    
       [DataMemberAttribute()]
        public bool OiMau
        {
            get
            {
                return _OiMau;
            }
            set
            {
                if (_OiMau != value)
                {
                    OnOiMauChanging(value);
                    _OiMau = value;
                    RaisePropertyChanged("OiMau");
                    OnOiMauChanged();
                }
            }
        }
        private bool _OiMau;
        partial void OnOiMauChanging(bool value);
        partial void OnOiMauChanged();
   
       [DataMemberAttribute()]
        public bool XaTriTrungThat
        {
            get
            {
                return _XaTriTrungThat;
            }
            set
            {
                if (_XaTriTrungThat != value)
                {
                    OnXaTriTrungThatChanging(value);
                    _XaTriTrungThat = value;
                    RaisePropertyChanged("XaTriTrungThat");
                    OnXaTriTrungThatChanged();
                }
            }
        }
        private bool _XaTriTrungThat;
        partial void OnXaTriTrungThatChanging(bool value);
        partial void OnXaTriTrungThatChanged();
   
       [DataMemberAttribute()]
        public bool CotSongCo
        {
            get
            {
                return _CotSongCo;
            }
            set
            {
                if (_CotSongCo != value)
                {
                    OnCotSongCoChanging(value);
                    _CotSongCo = value;
                    RaisePropertyChanged("CotSongCo");
                    OnCotSongCoChanged();
                }
            }
        }
        private bool  _CotSongCo;
        partial void OnCotSongCoChanging(bool value);
        partial void OnCotSongCoChanged();
    
       [DataMemberAttribute()]
        public bool ChanThuongLongNguc
        {
            get
            {
                return _ChanThuongLongNguc;
            }
            set
            {
                if (_ChanThuongLongNguc != value)
                {
                    OnChanThuongLongNgucChanging(value);
                    _ChanThuongLongNguc = value;
                    RaisePropertyChanged("ChanThuongLongNguc");
                    OnChanThuongLongNgucChanged();
                }
            }
        }
        private bool _ChanThuongLongNguc;
        partial void OnChanThuongLongNgucChanging(bool value);
        partial void OnChanThuongLongNgucChanged();
    
       [DataMemberAttribute()]
        public bool LanKhamNoiSoiGanDay
        {
            get
            {
                return _LanKhamNoiSoiGanDay;
            }
            set
            {
                if (_LanKhamNoiSoiGanDay != value)
                {
                    OnLanKhamNoiSoiGanDayChanging(value);
                    _LanKhamNoiSoiGanDay = value;
                    RaisePropertyChanged("LanKhamNoiSoiGanDay");
                    OnLanKhamNoiSoiGanDayChanged();
                }
            }
        }
        private bool _LanKhamNoiSoiGanDay;
        partial void OnLanKhamNoiSoiGanDayChanging(bool value);
        partial void OnLanKhamNoiSoiGanDayChanged();
    
       [DataMemberAttribute()]
        public bool DiUngThuoc
        {
            get
            {
                return _DiUngThuoc;
            }
            set
            {
                if (_DiUngThuoc != value)
                {
                    OnDiUngThuocChanging(value);
                    _DiUngThuoc = value;
                    RaisePropertyChanged("DiUngThuoc");
                    OnDiUngThuocChanged();
                }
            }
        }
        private bool _DiUngThuoc;
        partial void OnDiUngThuocChanging(bool value);
        partial void OnDiUngThuocChanged();
    
       [DataMemberAttribute()]
        public bool NghienRuou
        {
            get
            {
                return _NghienRuou;
            }
            set
            {
                if (_NghienRuou != value)
                {
                    OnNghienRuouChanging(value);
                    _NghienRuou = value;
                    RaisePropertyChanged("NghienRuou");
                    OnNghienRuouChanged();
                }
            }
        }
        private bool _NghienRuou;
        partial void OnNghienRuouChanging(bool value);
        partial void OnNghienRuouChanged();
    
       [DataMemberAttribute()]
        public bool BiTieu
        {
            get
            {
                return _BiTieu;
            }
            set
            {
                if (_BiTieu != value)
                {
                    OnBiTieuChanging(value);
                    _BiTieu = value;
                    RaisePropertyChanged("BiTieu");
                    OnBiTieuChanged();
                }
            }
        }
        private bool _BiTieu;
        partial void OnBiTieuChanging(bool value);
        partial void OnBiTieuChanged();
    
       [DataMemberAttribute()]
        public bool TangNhanApGocHep
        {
            get
            {
                return _TangNhanApGocHep;
            }
            set
            {
                if (_TangNhanApGocHep != value)
                {
                    OnTangNhanApGocHepChanging(value);
                    _TangNhanApGocHep = value;
                    RaisePropertyChanged("TangNhanApGocHep");
                    OnTangNhanApGocHepChanged();
                }
            }
        }
        private bool _TangNhanApGocHep;
        partial void OnTangNhanApGocHepChanging(bool value);
        partial void OnTangNhanApGocHepChanged();
    
       [DataMemberAttribute()]
        public bool Suyen
        {
            get
            {
                return _Suyen;
            }
            set
            {
                if (_Suyen != value)
                {
                    OnSuyenChanging(value);
                    _Suyen = value;
                    RaisePropertyChanged("Suyen");
                    OnSuyenChanged();
                }
            }
        }
        private bool _Suyen;
        partial void OnSuyenChanging(bool value);
        partial void OnSuyenChanged();

        [DataMemberAttribute()]
        public bool LanAnSauCung
        {
            get
            {
                return _LanAnSauCung;
            }
            set
            {
                if (_LanAnSauCung != value)
                {
                    OnLanAnSauCungChanging(value);
                    _LanAnSauCung = value;
                    RaisePropertyChanged("LanAnSauCung");
                    OnLanAnSauCungChanged();
                }
            }
        }
        private bool _LanAnSauCung;
        partial void OnLanAnSauCungChanging(bool value);
        partial void OnLanAnSauCungChanged();
    
       [DataMemberAttribute()]
        public bool RangGiaHamGia
        {
            get
            {
                return _RangGiaHamGia;
            }
            set
            {
                if (_RangGiaHamGia != value)
                {
                    OnRangGiaHamGiaChanging(value);
                    _RangGiaHamGia = value;
                    RaisePropertyChanged("RangGiaHamGia");
                    OnRangGiaHamGiaChanged();
                }
            }
        }
        private bool _RangGiaHamGia;
        partial void OnRangGiaHamGiaChanging(bool value);
        partial void OnRangGiaHamGiaChanged();
    
       [DataMemberAttribute()]
        public double HuyetApTT
        {
            get
            {
                return _HuyetApTT;
            }
            set
            {
                if (_HuyetApTT != value)
                {
                    OnHuyetApTTChanging(value);
                    _HuyetApTT = value;
                    RaisePropertyChanged("HuyetApTT");
                    OnHuyetApTTChanged();
                }
            }
        }
        private double _HuyetApTT;
        partial void OnHuyetApTTChanging(double value);
        partial void OnHuyetApTTChanged();
    
       [DataMemberAttribute()]
        public double HuyetApTTr
        {
            get
            {
                return _HuyetApTTr;
            }
            set
            {
                if (_HuyetApTTr != value)
                {
                    OnHuyetApTTrChanging(value);
                    _HuyetApTTr = value;
                    RaisePropertyChanged("HuyetApTTr");
                    OnHuyetApTTrChanged();
                }
            }
        }
        private double _HuyetApTTr;
        partial void OnHuyetApTTrChanging(double value);
        partial void OnHuyetApTTrChanged();
   
       [DataMemberAttribute()]
        public double Mach
        {
            get
            {
                return _Mach;
            }
            set
            {
                if (_Mach != value)
                {
                    OnMachChanging(value);
                    _Mach = value;
                    RaisePropertyChanged("Mach");
                    OnMachChanged();
                }
            }
        }
        private double _Mach;
        partial void OnMachChanging(double value);
        partial void OnMachChanged();
    
       [DataMemberAttribute()]
        public double DoBaoHoaOxy
        {
            get
            {
                return _DoBaoHoaOxy;
            }
            set
            {
                if (_DoBaoHoaOxy != value)
                {
                    OnDoBaoHoaOxyChanging(value);
                    _DoBaoHoaOxy = value;
                    RaisePropertyChanged("DoBaoHoaOxy");
                    OnDoBaoHoaOxyChanged();
                }
            }
        }
        private double _DoBaoHoaOxy;
        partial void OnDoBaoHoaOxyChanging(double value);
        partial void OnDoBaoHoaOxyChanged();
    
       [DataMemberAttribute()]
        public bool ThucHienDuongTruyenTinhMach
        {
            get
            {
                return _ThucHienDuongTruyenTinhMach;
            }
            set
            {
                if (_ThucHienDuongTruyenTinhMach != value)
                {
                    OnThucHienDuongTruyenTinhMachChanging(value);
                    _ThucHienDuongTruyenTinhMach = value;
                    RaisePropertyChanged("ThucHienDuongTruyenTinhMach");
                    OnThucHienDuongTruyenTinhMachChanged();
                }
            }
        }
        private bool _ThucHienDuongTruyenTinhMach;
        partial void OnThucHienDuongTruyenTinhMachChanging(bool value);
        partial void OnThucHienDuongTruyenTinhMachChanged();
   
       [DataMemberAttribute()]
        public bool KiemTraDauDoSieuAm
        {
            get
            {
                return _KiemTraDauDoSieuAm;
            }
            set
            {
                if (_KiemTraDauDoSieuAm != value)
                {
                    OnKiemTraDauDoSieuAmChanging(value);
                    _KiemTraDauDoSieuAm = value;
                    RaisePropertyChanged("KiemTraDauDoSieuAm");
                    OnKiemTraDauDoSieuAmChanged();
                }
            }
        }
        private bool _KiemTraDauDoSieuAm;
        partial void OnKiemTraDauDoSieuAmChanging(bool value);
        partial void OnKiemTraDauDoSieuAmChanged();
    
       [DataMemberAttribute()]
        public bool ChinhDauDoTrungTinh
        {
            get
            {
                return _ChinhDauDoTrungTinh;
            }
            set
            {
                if (_ChinhDauDoTrungTinh != value)
                {
                    OnChinhDauDoTrungTinhChanging(value);
                    _ChinhDauDoTrungTinh = value;
                    RaisePropertyChanged("ChinhDauDoTrungTinh");
                    OnChinhDauDoTrungTinhChanged();
                }
            }
        }
        private bool _ChinhDauDoTrungTinh;
        partial void OnChinhDauDoTrungTinhChanging(bool value);
        partial void OnChinhDauDoTrungTinhChanged();
    
       [DataMemberAttribute()]
        public bool TeMeBenhNhan
        {
            get
            {
                return _TeMeBenhNhan;
            }
            set
            {
                if (_TeMeBenhNhan != value)
                {
                    OnTeMeBenhNhanChanging(value);
                    _TeMeBenhNhan = value;
                    RaisePropertyChanged("TeMeBenhNhan");
                    OnTeMeBenhNhanChanged();
                }
            }
        }
        private bool _TeMeBenhNhan;
        partial void OnTeMeBenhNhanChanging(bool value);
        partial void OnTeMeBenhNhanChanged();
    
       [DataMemberAttribute()]
        public bool DatBenhNhanNghiengTrai
        {
            get
            {
                return _DatBenhNhanNghiengTrai;
            }
            set
            {
                if (_DatBenhNhanNghiengTrai != value)
                {
                    OnDatBenhNhanNghiengTraiChanging(value);
                    _DatBenhNhanNghiengTrai = value;
                    RaisePropertyChanged("DatBenhNhanNghiengTrai");
                    OnDatBenhNhanNghiengTraiChanged();
                }
            }
        }
        private bool _DatBenhNhanNghiengTrai;
        partial void OnDatBenhNhanNghiengTraiChanging(bool value);
        partial void OnDatBenhNhanNghiengTraiChanged();
   
       [DataMemberAttribute()]
        public bool CotDay
        {
            get
            {
                return _CotDay;
            }
            set
            {
                if (_CotDay != value)
                {
                    OnCotDayChanging(value);
                    _CotDay = value;
                    RaisePropertyChanged("CotDay");
                    OnCotDayChanged();
                }
            }
        }
        private bool _CotDay;
        partial void OnCotDayChanging(bool value);
        partial void OnCotDayChanged();
    
       [DataMemberAttribute()]
        public bool BenhNhanThoaiMai
        {
            get
            {
                return _BenhNhanThoaiMai;
            }
            set
            {
                if (_BenhNhanThoaiMai != value)
                {
                    OnBenhNhanThoaiMaiChanging(value);
                    _BenhNhanThoaiMai = value;
                    RaisePropertyChanged("BenhNhanThoaiMai");
                    OnBenhNhanThoaiMaiChanged();
                }
            }
        }
        private bool _BenhNhanThoaiMai;
        partial void OnBenhNhanThoaiMaiChanging(bool value);
        partial void OnBenhNhanThoaiMaiChanged();
    
       [DataMemberAttribute()]
        public bool BoiTronDauDo
        {
            get
            {
                return _BoiTronDauDo;
            }
            set
            {
                if (_BoiTronDauDo != value)
                {
                    OnBoiTronDauDoChanging(value);
                    _BoiTronDauDo = value;
                    RaisePropertyChanged("BoiTronDauDo");
                    OnBoiTronDauDoChanged();
                }
            }
        }
       private bool _BoiTronDauDo;
       partial void OnBoiTronDauDoChanging(bool value);
       partial void OnBoiTronDauDoChanged();
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
                return this.URP_FE_OesophagienneCheckID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
