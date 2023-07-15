using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDipyridamole : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDipyridamole()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDipyridamole info = obj as URP_FE_StressDipyridamole;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDipyridamoleID > 0 && this.URP_FE_StressDipyridamoleID == info.URP_FE_StressDipyridamoleID;
        }
        private URP_FE_StressDipyridamole _tempURP_FE_StressDipyridamole;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDipyridamole = (URP_FE_StressDipyridamole)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDipyridamole)
                CopyFrom(_tempURP_FE_StressDipyridamole);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDipyridamole p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDipyridamole object.

        /// <param name="URP_FE_StressDipyridamoleID">Initial value of the URP_FE_StressDipyridamoleID property.</param>
        /// <param name="URP_FE_StressDipyridamoleName">Initial value of the URP_FE_StressDipyridamoleName property.</param>
        public static URP_FE_StressDipyridamole CreateURP_FE_StressDipyridamole(Byte URP_FE_StressDipyridamoleID, String URP_FE_StressDipyridamoleName)
        {
            URP_FE_StressDipyridamole URP_FE_StressDipyridamole = new URP_FE_StressDipyridamole();
            URP_FE_StressDipyridamole.URP_FE_StressDipyridamoleID = URP_FE_StressDipyridamoleID;
            
            return URP_FE_StressDipyridamole;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDipyridamoleID
        {
            get
            {
                return _URP_FE_StressDipyridamoleID;
            }
            set
            {
                if (_URP_FE_StressDipyridamoleID != value)
                {
                    OnURP_FE_StressDipyridamoleIDChanging(value);
                    _URP_FE_StressDipyridamoleID = value;
                    RaisePropertyChanged("URP_FE_StressDipyridamoleID");
                    OnURP_FE_StressDipyridamoleIDChanged();
                }
            }
        }
        private long _URP_FE_StressDipyridamoleID;
        partial void OnURP_FE_StressDipyridamoleIDChanging(long value);
        partial void OnURP_FE_StressDipyridamoleIDChanged();

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
        public double TanSoTimCanDat
        {
            get
            {
                return _TanSoTimCanDat;
            }
            set
            {
                OnTanSoTimCanDatChanging(value);
                _TanSoTimCanDat = value;
                RaisePropertyChanged("TanSoTimCanDat");
                OnTanSoTimCanDatChanged();
            }
        }
        private double _TanSoTimCanDat;
        partial void OnTanSoTimCanDatChanging(double value);
        partial void OnTanSoTimCanDatChanged();




        [DataMemberAttribute()]
        public double TNP_HuyetAp_TT
        {
            get
            {
                return _TNP_HuyetAp_TT;
            }
            set
            {
                OnTNP_HuyetAp_TTChanging(value);
                _TNP_HuyetAp_TT = value;
                RaisePropertyChanged("TNP_HuyetAp_TT");
                OnTNP_HuyetAp_TTChanged();
            }
        }
        private double _TNP_HuyetAp_TT;
        partial void OnTNP_HuyetAp_TTChanging(double value);
        partial void OnTNP_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TNP_HuyetAp_TTr
        {
            get
            {
                return _TNP_HuyetAp_TTr;
            }
            set
            {
                OnTNP_HuyetAp_TTrChanging(value);
                _TNP_HuyetAp_TTr = value;
                RaisePropertyChanged("TNP_HuyetAp_TTr");
                OnTNP_HuyetAp_TTrChanged();
            }
        }
        private double _TNP_HuyetAp_TTr;
        partial void OnTNP_HuyetAp_TTrChanging(double value);
        partial void OnTNP_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TNP_TanSoTim
        {
            get
            {
                return _TNP_TanSoTim;
            }
            set
            {
                OnTNP_TanSoTimChanging(value);
                _TNP_TanSoTim = value;
                RaisePropertyChanged("TNP_TanSoTim");
                OnTNP_TanSoTimChanged();
            }
        }
        private double _TNP_TanSoTim;
        partial void OnTNP_TanSoTimChanging(double value);
        partial void OnTNP_TanSoTimChanged();




        [DataMemberAttribute()]
        public string TNP_TacDungPhu
        {
            get
            {
                return _TNP_TacDungPhu;
            }
            set
            {
                OnTNP_TacDungPhuChanging(value);
                _TNP_TacDungPhu = value;
                RaisePropertyChanged("TNP_TacDungPhu");
                OnTNP_TacDungPhuChanged();
            }
        }
        private string _TNP_TacDungPhu;
        partial void OnTNP_TacDungPhuChanging(string value);
        partial void OnTNP_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double TruyenDipyridamole056_DungLuong
        {
            get
            {
                return _TruyenDipyridamole056_DungLuong;
            }
            set
            {
                OnTruyenDipyridamole056_DungLuongChanging(value);
                _TruyenDipyridamole056_DungLuong = value;
                RaisePropertyChanged("TruyenDipyridamole056_DungLuong");
                OnTruyenDipyridamole056_DungLuongChanged();
            }
        }
        private double _TruyenDipyridamole056_DungLuong;
        partial void OnTruyenDipyridamole056_DungLuongChanging(double value);
        partial void OnTruyenDipyridamole056_DungLuongChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P2_HuyetAp_TT
        {
            get
            {
                return _TruyenDipy056_P2_HuyetAp_TT;
            }
            set
            {
                OnTruyenDipy056_P2_HuyetAp_TTChanging(value);
                _TruyenDipy056_P2_HuyetAp_TT = value;
                RaisePropertyChanged("TruyenDipy056_P2_HuyetAp_TT");
                OnTruyenDipy056_P2_HuyetAp_TTChanged();
            }
        }
        private double _TruyenDipy056_P2_HuyetAp_TT;
        partial void OnTruyenDipy056_P2_HuyetAp_TTChanging(double value);
        partial void OnTruyenDipy056_P2_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P2_HuyetAp_TTr
        {
            get
            {
                return _TruyenDipy056_P2_HuyetAp_TTr;
            }
            set
            {
                OnTruyenDipy056_P2_HuyetAp_TTrChanging(value);
                _TruyenDipy056_P2_HuyetAp_TTr = value;
                RaisePropertyChanged("TruyenDipy056_P2_HuyetAp_TTr");
                OnTruyenDipy056_P2_HuyetAp_TTrChanged();
            }
        }
        private double _TruyenDipy056_P2_HuyetAp_TTr;
        partial void OnTruyenDipy056_P2_HuyetAp_TTrChanging(double value);
        partial void OnTruyenDipy056_P2_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P2_TanSoTim
        {
            get
            {
                return _TruyenDipy056_P2_TanSoTim;
            }
            set
            {
                OnTruyenDipy056_P2_TanSoTimChanging(value);
                _TruyenDipy056_P2_TanSoTim = value;
                RaisePropertyChanged("TruyenDipy056_P2_TanSoTim");
                OnTruyenDipy056_P2_TanSoTimChanged();
            }
        }
        private double _TruyenDipy056_P2_TanSoTim;
        partial void OnTruyenDipy056_P2_TanSoTimChanging(double value);
        partial void OnTruyenDipy056_P2_TanSoTimChanged();




        [DataMemberAttribute()]
        public string TruyenDipy056_P2_TacDungPhu
        {
            get
            {
                return _TruyenDipy056_P2_TacDungPhu;
            }
            set
            {
                OnTruyenDipy056_P2_TacDungPhuChanging(value);
                _TruyenDipy056_P2_TacDungPhu = value;
                RaisePropertyChanged("TruyenDipy056_P2_TacDungPhu");
                OnTruyenDipy056_P2_TacDungPhuChanged();
            }
        }
        private string _TruyenDipy056_P2_TacDungPhu;
        partial void OnTruyenDipy056_P2_TacDungPhuChanging(string value);
        partial void OnTruyenDipy056_P2_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P4_HuyetAp_TT
        {
            get
            {
                return _TruyenDipy056_P4_HuyetAp_TT;
            }
            set
            {
                OnTruyenDipy056_P4_HuyetAp_TTChanging(value);
                _TruyenDipy056_P4_HuyetAp_TT = value;
                RaisePropertyChanged("TruyenDipy056_P4_HuyetAp_TT");
                OnTruyenDipy056_P4_HuyetAp_TTChanged();
            }
        }
        private double _TruyenDipy056_P4_HuyetAp_TT;
        partial void OnTruyenDipy056_P4_HuyetAp_TTChanging(double value);
        partial void OnTruyenDipy056_P4_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P4_HuyetAp_TTr
        {
            get
            {
                return _TruyenDipy056_P4_HuyetAp_TTr;
            }
            set
            {
                OnTruyenDipy056_P4_HuyetAp_TTrChanging(value);
                _TruyenDipy056_P4_HuyetAp_TTr = value;
                RaisePropertyChanged("TruyenDipy056_P4_HuyetAp_TTr");
                OnTruyenDipy056_P4_HuyetAp_TTrChanged();
            }
        }
        private double _TruyenDipy056_P4_HuyetAp_TTr;
        partial void OnTruyenDipy056_P4_HuyetAp_TTrChanging(double value);
        partial void OnTruyenDipy056_P4_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TruyenDipy056_P4_TanSoTim
        {
            get
            {
                return _TruyenDipy056_P4_TanSoTim;
            }
            set
            {
                OnTruyenDipy056_P4_TanSoTimChanging(value);
                _TruyenDipy056_P4_TanSoTim = value;
                RaisePropertyChanged("TruyenDipy056_P4_TanSoTim");
                OnTruyenDipy056_P4_TanSoTimChanged();
            }
        }
        private double _TruyenDipy056_P4_TanSoTim;
        partial void OnTruyenDipy056_P4_TanSoTimChanging(double value);
        partial void OnTruyenDipy056_P4_TanSoTimChanged();




        [DataMemberAttribute()]
        public string TruyenDipy056_P4_TacDungPhu
        {
            get
            {
                return _TruyenDipy056_P4_TacDungPhu;
            }
            set
            {
                OnTruyenDipy056_P4_TacDungPhuChanging(value);
                _TruyenDipy056_P4_TacDungPhu = value;
                RaisePropertyChanged("TruyenDipy056_P4_TacDungPhu");
                OnTruyenDipy056_P4_TacDungPhuChanged();
            }
        }
        private string _TruyenDipy056_P4_TacDungPhu;
        partial void OnTruyenDipy056_P4_TacDungPhuChanging(string value);
        partial void OnTruyenDipy056_P4_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double SauLieuDauP6_HuyetAp_TT
        {
            get
            {
                return _SauLieuDauP6_HuyetAp_TT;
            }
            set
            {
                OnSauLieuDauP6_HuyetAp_TTChanging(value);
                _SauLieuDauP6_HuyetAp_TT = value;
                RaisePropertyChanged("SauLieuDauP6_HuyetAp_TT");
                OnSauLieuDauP6_HuyetAp_TTChanged();
            }
        }
        private double _SauLieuDauP6_HuyetAp_TT;
        partial void OnSauLieuDauP6_HuyetAp_TTChanging(double value);
        partial void OnSauLieuDauP6_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double SauLieuDauP6_HuyetAp_TTr
        {
            get
            {
                return _SauLieuDauP6_HuyetAp_TTr;
            }
            set
            {
                OnSauLieuDauP6_HuyetAp_TTrChanging(value);
                _SauLieuDauP6_HuyetAp_TTr = value;
                RaisePropertyChanged("SauLieuDauP6_HuyetAp_TTr");
                OnSauLieuDauP6_HuyetAp_TTrChanged();
            }
        }
        private double _SauLieuDauP6_HuyetAp_TTr;
        partial void OnSauLieuDauP6_HuyetAp_TTrChanging(double value);
        partial void OnSauLieuDauP6_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double SauLieuDauP6_TanSoTim
        {
            get
            {
                return _SauLieuDauP6_TanSoTim;
            }
            set
            {
                OnSauLieuDauP6_TanSoTimChanging(value);
                _SauLieuDauP6_TanSoTim = value;
                RaisePropertyChanged("SauLieuDauP6_TanSoTim");
                OnSauLieuDauP6_TanSoTimChanged();
            }
        }
        private double _SauLieuDauP6_TanSoTim;
        partial void OnSauLieuDauP6_TanSoTimChanging(double value);
        partial void OnSauLieuDauP6_TanSoTimChanged();




        [DataMemberAttribute()]
        public string SauLieuDauP6_TacDungPhu
        {
            get
            {
                return _SauLieuDauP6_TacDungPhu;
            }
            set
            {
                OnSauLieuDauP6_TacDungPhuChanging(value);
                _SauLieuDauP6_TacDungPhu = value;
                RaisePropertyChanged("SauLieuDauP6_TacDungPhu");
                OnSauLieuDauP6_TacDungPhuChanged();
            }
        }
        private string _SauLieuDauP6_TacDungPhu;
        partial void OnSauLieuDauP6_TacDungPhuChanging(string value);
        partial void OnSauLieuDauP6_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double TruyenDipyridamole028_DungLuong
        {
            get
            {
                return _TruyenDipyridamole028_DungLuong;
            }
            set
            {
                OnTruyenDipyridamole028_DungLuongChanging(value);
                _TruyenDipyridamole028_DungLuong = value;
                RaisePropertyChanged("TruyenDipyridamole028_DungLuong");
                OnTruyenDipyridamole028_DungLuongChanged();
            }
        }
        private double _TruyenDipyridamole028_DungLuong;
        partial void OnTruyenDipyridamole028_DungLuongChanging(double value);
        partial void OnTruyenDipyridamole028_DungLuongChanged();




        [DataMemberAttribute()]
        public double TruyenDipy028_P8_HuyetAp_TT
        {
            get
            {
                return _TruyenDipy028_P8_HuyetAp_TT;
            }
            set
            {
                OnTruyenDipy028_P8_HuyetAp_TTChanging(value);
                _TruyenDipy028_P8_HuyetAp_TT = value;
                RaisePropertyChanged("TruyenDipy028_P8_HuyetAp_TT");
                OnTruyenDipy028_P8_HuyetAp_TTChanged();
            }
        }
        private double _TruyenDipy028_P8_HuyetAp_TT;
        partial void OnTruyenDipy028_P8_HuyetAp_TTChanging(double value);
        partial void OnTruyenDipy028_P8_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TruyenDipy028_P8_HuyetAp_TTr
        {
            get
            {
                return _TruyenDipy028_P8_HuyetAp_TTr;
            }
            set
            {
                OnTruyenDipy028_P8_HuyetAp_TTrChanging(value);
                _TruyenDipy028_P8_HuyetAp_TTr = value;
                RaisePropertyChanged("TruyenDipy028_P8_HuyetAp_TTr");
                OnTruyenDipy028_P8_HuyetAp_TTrChanged();
            }
        }
        private double _TruyenDipy028_P8_HuyetAp_TTr;
        partial void OnTruyenDipy028_P8_HuyetAp_TTrChanging(double value);
        partial void OnTruyenDipy028_P8_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TruyenDipy028_P8_TanSoTim
        {
            get
            {
                return _TruyenDipy028_P8_TanSoTim;
            }
            set
            {
                OnTruyenDipy028_P8_TanSoTimChanging(value);
                _TruyenDipy028_P8_TanSoTim = value;
                RaisePropertyChanged("TruyenDipy028_P8_TanSoTim");
                OnTruyenDipy028_P8_TanSoTimChanged();
            }
        }
        private double _TruyenDipy028_P8_TanSoTim;
        partial void OnTruyenDipy028_P8_TanSoTimChanging(double value);
        partial void OnTruyenDipy028_P8_TanSoTimChanged();




        [DataMemberAttribute()]
        public string TruyenDipy028_P8_TacDungPhu
        {
            get
            {
                return _TruyenDipy028_P8_TacDungPhu;
            }
            set
            {
                OnTruyenDipy028_P8_TacDungPhuChanging(value);
                _TruyenDipy028_P8_TacDungPhu = value;
                RaisePropertyChanged("TruyenDipy028_P8_TacDungPhu");
                OnTruyenDipy028_P8_TacDungPhuChanged();
            }
        }
        private string _TruyenDipy028_P8_TacDungPhu;
        partial void OnTruyenDipy028_P8_TacDungPhuChanging(string value);
        partial void OnTruyenDipy028_P8_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double SauLieu2P10_HuyetAp_TT
        {
            get
            {
                return _SauLieu2P10_HuyetAp_TT;
            }
            set
            {
                OnSauLieu2P10_HuyetAp_TTChanging(value);
                _SauLieu2P10_HuyetAp_TT = value;
                RaisePropertyChanged("SauLieu2P10_HuyetAp_TT");
                OnSauLieu2P10_HuyetAp_TTChanged();
            }
        }
        private double _SauLieu2P10_HuyetAp_TT;
        partial void OnSauLieu2P10_HuyetAp_TTChanging(double value);
        partial void OnSauLieu2P10_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double SauLieu2P10_HuyetAp_TTr
        {
            get
            {
                return _SauLieu2P10_HuyetAp_TTr;
            }
            set
            {
                OnSauLieu2P10_HuyetAp_TTrChanging(value);
                _SauLieu2P10_HuyetAp_TTr = value;
                RaisePropertyChanged("SauLieu2P10_HuyetAp_TTr");
                OnSauLieu2P10_HuyetAp_TTrChanged();
            }
        }
        private double _SauLieu2P10_HuyetAp_TTr;
        partial void OnSauLieu2P10_HuyetAp_TTrChanging(double value);
        partial void OnSauLieu2P10_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double SauLieu2P10_TanSoTim
        {
            get
            {
                return _SauLieu2P10_TanSoTim;
            }
            set
            {
                OnSauLieu2P10_TanSoTimChanging(value);
                _SauLieu2P10_TanSoTim = value;
                RaisePropertyChanged("SauLieu2P10_TanSoTim");
                OnSauLieu2P10_TanSoTimChanged();
            }
        }
        private double _SauLieu2P10_TanSoTim;
        partial void OnSauLieu2P10_TanSoTimChanging(double value);
        partial void OnSauLieu2P10_TanSoTimChanged();




        [DataMemberAttribute()]
        public string SauLieu2P10_TacDungPhu
        {
            get
            {
                return _SauLieu2P10_TacDungPhu;
            }
            set
            {
                OnSauLieu2P10_TacDungPhuChanging(value);
                _SauLieu2P10_TacDungPhu = value;
                RaisePropertyChanged("SauLieu2P10_TacDungPhu");
                OnSauLieu2P10_TacDungPhuChanged();
            }
        }
        private string _SauLieu2P10_TacDungPhu;
        partial void OnSauLieu2P10_TacDungPhuChanging(string value);
        partial void OnSauLieu2P10_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP12_HuyetAp_TT
        {
            get
            {
                return _ThemAtropineP12_HuyetAp_TT;
            }
            set
            {
                OnThemAtropineP12_HuyetAp_TTChanging(value);
                _ThemAtropineP12_HuyetAp_TT = value;
                RaisePropertyChanged("ThemAtropineP12_HuyetAp_TT");
                OnThemAtropineP12_HuyetAp_TTChanged();
            }
        }
        private double _ThemAtropineP12_HuyetAp_TT;
        partial void OnThemAtropineP12_HuyetAp_TTChanging(double value);
        partial void OnThemAtropineP12_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP12_HuyetAp_TTr
        {
            get
            {
                return _ThemAtropineP12_HuyetAp_TTr;
            }
            set
            {
                OnThemAtropineP12_HuyetAp_TTrChanging(value);
                _ThemAtropineP12_HuyetAp_TTr = value;
                RaisePropertyChanged("ThemAtropineP12_HuyetAp_TTr");
                OnThemAtropineP12_HuyetAp_TTrChanged();
            }
        }
        private double _ThemAtropineP12_HuyetAp_TTr;
        partial void OnThemAtropineP12_HuyetAp_TTrChanging(double value);
        partial void OnThemAtropineP12_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP12_TanSoTim
        {
            get
            {
                return _ThemAtropineP12_TanSoTim;
            }
            set
            {
                OnThemAtropineP12_TanSoTimChanging(value);
                _ThemAtropineP12_TanSoTim = value;
                RaisePropertyChanged("ThemAtropineP12_TanSoTim");
                OnThemAtropineP12_TanSoTimChanged();
            }
        }
        private double _ThemAtropineP12_TanSoTim;
        partial void OnThemAtropineP12_TanSoTimChanging(double value);
        partial void OnThemAtropineP12_TanSoTimChanged();




        [DataMemberAttribute()]
        public string ThemAtropineP12_TacDungPhu
        {
            get
            {
                return _ThemAtropineP12_TacDungPhu;
            }
            set
            {
                OnThemAtropineP12_TacDungPhuChanging(value);
                _ThemAtropineP12_TacDungPhu = value;
                RaisePropertyChanged("ThemAtropineP12_TacDungPhu");
                OnThemAtropineP12_TacDungPhuChanged();
            }
        }
        private string _ThemAtropineP12_TacDungPhu;
        partial void OnThemAtropineP12_TacDungPhuChanging(string value);
        partial void OnThemAtropineP12_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP13_HuyetAp_TT
        {
            get
            {
                return _ThemAtropineP13_HuyetAp_TT;
            }
            set
            {
                OnThemAtropineP13_HuyetAp_TTChanging(value);
                _ThemAtropineP13_HuyetAp_TT = value;
                RaisePropertyChanged("ThemAtropineP13_HuyetAp_TT");
                OnThemAtropineP13_HuyetAp_TTChanged();
            }
        }
        private double _ThemAtropineP13_HuyetAp_TT;
        partial void OnThemAtropineP13_HuyetAp_TTChanging(double value);
        partial void OnThemAtropineP13_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP13_HuyetAp_TTr
        {
            get
            {
                return _ThemAtropineP13_HuyetAp_TTr;
            }
            set
            {
                OnThemAtropineP13_HuyetAp_TTrChanging(value);
                _ThemAtropineP13_HuyetAp_TTr = value;
                RaisePropertyChanged("ThemAtropineP13_HuyetAp_TTr");
                OnThemAtropineP13_HuyetAp_TTrChanged();
            }
        }
        private double _ThemAtropineP13_HuyetAp_TTr;
        partial void OnThemAtropineP13_HuyetAp_TTrChanging(double value);
        partial void OnThemAtropineP13_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP13_TanSoTim
        {
            get
            {
                return _ThemAtropineP13_TanSoTim;
            }
            set
            {
                OnThemAtropineP13_TanSoTimChanging(value);
                _ThemAtropineP13_TanSoTim = value;
                RaisePropertyChanged("ThemAtropineP13_TanSoTim");
                OnThemAtropineP13_TanSoTimChanged();
            }
        }
        private double _ThemAtropineP13_TanSoTim;
        partial void OnThemAtropineP13_TanSoTimChanging(double value);
        partial void OnThemAtropineP13_TanSoTimChanged();




        [DataMemberAttribute()]
        public string ThemAtropineP13_TacDungPhu
        {
            get
            {
                return _ThemAtropineP13_TacDungPhu;
            }
            set
            {
                OnThemAtropineP13_TacDungPhuChanging(value);
                _ThemAtropineP13_TacDungPhu = value;
                RaisePropertyChanged("ThemAtropineP13_TacDungPhu");
                OnThemAtropineP13_TacDungPhuChanged();
            }
        }
        private string _ThemAtropineP13_TacDungPhu;
        partial void OnThemAtropineP13_TacDungPhuChanging(string value);
        partial void OnThemAtropineP13_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP14_HuyetAp_TT
        {
            get
            {
                return _ThemAtropineP14_HuyetAp_TT;
            }
            set
            {
                OnThemAtropineP14_HuyetAp_TTChanging(value);
                _ThemAtropineP14_HuyetAp_TT = value;
                RaisePropertyChanged("ThemAtropineP14_HuyetAp_TT");
                OnThemAtropineP14_HuyetAp_TTChanged();
            }
        }
        private double _ThemAtropineP14_HuyetAp_TT;
        partial void OnThemAtropineP14_HuyetAp_TTChanging(double value);
        partial void OnThemAtropineP14_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP14_HuyetAp_TTr
        {
            get
            {
                return _ThemAtropineP14_HuyetAp_TTr;
            }
            set
            {
                OnThemAtropineP14_HuyetAp_TTrChanging(value);
                _ThemAtropineP14_HuyetAp_TTr = value;
                RaisePropertyChanged("ThemAtropineP14_HuyetAp_TTr");
                OnThemAtropineP14_HuyetAp_TTrChanged();
            }
        }
        private double _ThemAtropineP14_HuyetAp_TTr;
        partial void OnThemAtropineP14_HuyetAp_TTrChanging(double value);
        partial void OnThemAtropineP14_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP14_TanSoTim
        {
            get
            {
                return _ThemAtropineP14_TanSoTim;
            }
            set
            {
                OnThemAtropineP14_TanSoTimChanging(value);
                _ThemAtropineP14_TanSoTim = value;
                RaisePropertyChanged("ThemAtropineP14_TanSoTim");
                OnThemAtropineP14_TanSoTimChanged();
            }
        }
        private double _ThemAtropineP14_TanSoTim;
        partial void OnThemAtropineP14_TanSoTimChanging(double value);
        partial void OnThemAtropineP14_TanSoTimChanged();




        [DataMemberAttribute()]
        public string ThemAtropineP14_TacDungPhu
        {
            get
            {
                return _ThemAtropineP14_TacDungPhu;
            }
            set
            {
                OnThemAtropineP14_TacDungPhuChanging(value);
                _ThemAtropineP14_TacDungPhu = value;
                RaisePropertyChanged("ThemAtropineP14_TacDungPhu");
                OnThemAtropineP14_TacDungPhuChanged();
            }
        }
        private string _ThemAtropineP14_TacDungPhu;
        partial void OnThemAtropineP14_TacDungPhuChanging(string value);
        partial void OnThemAtropineP14_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP15_HuyetAp_TT
        {
            get
            {
                return _ThemAtropineP15_HuyetAp_TT;
            }
            set
            {
                OnThemAtropineP15_HuyetAp_TTChanging(value);
                _ThemAtropineP15_HuyetAp_TT = value;
                RaisePropertyChanged("ThemAtropineP15_HuyetAp_TT");
                OnThemAtropineP15_HuyetAp_TTChanged();
            }
        }
        private double _ThemAtropineP15_HuyetAp_TT;
        partial void OnThemAtropineP15_HuyetAp_TTChanging(double value);
        partial void OnThemAtropineP15_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP15_HuyetAp_TTr
        {
            get
            {
                return _ThemAtropineP15_HuyetAp_TTr;
            }
            set
            {
                OnThemAtropineP15_HuyetAp_TTrChanging(value);
                _ThemAtropineP15_HuyetAp_TTr = value;
                RaisePropertyChanged("ThemAtropineP15_HuyetAp_TTr");
                OnThemAtropineP15_HuyetAp_TTrChanged();
            }
        }
        private double _ThemAtropineP15_HuyetAp_TTr;
        partial void OnThemAtropineP15_HuyetAp_TTrChanging(double value);
        partial void OnThemAtropineP15_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThemAtropineP15_TanSoTim
        {
            get
            {
                return _ThemAtropineP15_TanSoTim;
            }
            set
            {
                OnThemAtropineP15_TanSoTimChanging(value);
                _ThemAtropineP15_TanSoTim = value;
                RaisePropertyChanged("ThemAtropineP15_TanSoTim");
                OnThemAtropineP15_TanSoTimChanged();
            }
        }
        private double _ThemAtropineP15_TanSoTim;
        partial void OnThemAtropineP15_TanSoTimChanging(double value);
        partial void OnThemAtropineP15_TanSoTimChanged();




        [DataMemberAttribute()]
        public string ThemAtropineP15_TacDungPhu
        {
            get
            {
                return _ThemAtropineP15_TacDungPhu;
            }
            set
            {
                OnThemAtropineP15_TacDungPhuChanging(value);
                _ThemAtropineP15_TacDungPhu = value;
                RaisePropertyChanged("ThemAtropineP15_TacDungPhu");
                OnThemAtropineP15_TacDungPhuChanged();
            }
        }
        private string _ThemAtropineP15_TacDungPhu;
        partial void OnThemAtropineP15_TacDungPhuChanging(string value);
        partial void OnThemAtropineP15_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double TheoDoiAtropineP16_HuyetAp_TT
        {
            get
            {
                return _TheoDoiAtropineP16_HuyetAp_TT;
            }
            set
            {
                OnTheoDoiAtropineP16_HuyetAp_TTChanging(value);
                _TheoDoiAtropineP16_HuyetAp_TT = value;
                RaisePropertyChanged("TheoDoiAtropineP16_HuyetAp_TT");
                OnTheoDoiAtropineP16_HuyetAp_TTChanged();
            }
        }
        private double _TheoDoiAtropineP16_HuyetAp_TT;
        partial void OnTheoDoiAtropineP16_HuyetAp_TTChanging(double value);
        partial void OnTheoDoiAtropineP16_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TheoDoiAtropineP16_HuyetAp_TTr
        {
            get
            {
                return _TheoDoiAtropineP16_HuyetAp_TTr;
            }
            set
            {
                OnTheoDoiAtropineP16_HuyetAp_TTrChanging(value);
                _TheoDoiAtropineP16_HuyetAp_TTr = value;
                RaisePropertyChanged("TheoDoiAtropineP16_HuyetAp_TTr");
                OnTheoDoiAtropineP16_HuyetAp_TTrChanged();
            }
        }
        private double _TheoDoiAtropineP16_HuyetAp_TTr;
        partial void OnTheoDoiAtropineP16_HuyetAp_TTrChanging(double value);
        partial void OnTheoDoiAtropineP16_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TheoDoiAtropineP16_TanSoTim
        {
            get
            {
                return _TheoDoiAtropineP16_TanSoTim;
            }
            set
            {
                OnTheoDoiAtropineP16_TanSoTimChanging(value);
                _TheoDoiAtropineP16_TanSoTim = value;
                RaisePropertyChanged("TheoDoiAtropineP16_TanSoTim");
                OnTheoDoiAtropineP16_TanSoTimChanged();
            }
        }
        private double _TheoDoiAtropineP16_TanSoTim;
        partial void OnTheoDoiAtropineP16_TanSoTimChanging(double value);
        partial void OnTheoDoiAtropineP16_TanSoTimChanged();




        [DataMemberAttribute()]
        public string TheoDoiAtropineP16_TacDungPhu
        {
            get
            {
                return _TheoDoiAtropineP16_TacDungPhu;
            }
            set
            {
                OnTheoDoiAtropineP16_TacDungPhuChanging(value);
                _TheoDoiAtropineP16_TacDungPhu = value;
                RaisePropertyChanged("TheoDoiAtropineP16_TacDungPhu");
                OnTheoDoiAtropineP16_TacDungPhuChanged();
            }
        }
        private string _TheoDoiAtropineP16_TacDungPhu;
        partial void OnTheoDoiAtropineP16_TacDungPhuChanging(string value);
        partial void OnTheoDoiAtropineP16_TacDungPhuChanged();




        [DataMemberAttribute()]
        public double ThemAminophyline_DungLuong
        {
            get
            {
                return _ThemAminophyline_DungLuong;
            }
            set
            {
                OnThemAminophyline_DungLuongChanging(value);
                _ThemAminophyline_DungLuong = value;
                RaisePropertyChanged("ThemAminophyline_DungLuong");
                OnThemAminophyline_DungLuongChanged();
            }
        }
        private double _ThemAminophyline_DungLuong;
        partial void OnThemAminophyline_DungLuongChanging(double value);
        partial void OnThemAminophyline_DungLuongChanged();




        [DataMemberAttribute()]
        public double ThemAminophyline_Phut
        {
            get
            {
                return _ThemAminophyline_Phut;
            }
            set
            {
                OnThemAminophyline_PhutChanging(value);
                _ThemAminophyline_Phut = value;
                RaisePropertyChanged("ThemAminophyline_Phut");
                OnThemAminophyline_PhutChanged();
            }
        }
        private double _ThemAminophyline_Phut;
        partial void OnThemAminophyline_PhutChanging(double value);
        partial void OnThemAminophyline_PhutChanged();




        [DataMemberAttribute()]
        public double ThemAminophyline_HuyetAp_TT
        {
            get
            {
                return _ThemAminophyline_HuyetAp_TT;
            }
            set
            {
                OnThemAminophyline_HuyetAp_TTChanging(value);
                _ThemAminophyline_HuyetAp_TT = value;
                RaisePropertyChanged("ThemAminophyline_HuyetAp_TT");
                OnThemAminophyline_HuyetAp_TTChanged();
            }
        }
        private double _ThemAminophyline_HuyetAp_TT;
        partial void OnThemAminophyline_HuyetAp_TTChanging(double value);
        partial void OnThemAminophyline_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThemAminophyline_HuyetAp_TTr
        {
            get
            {
                return _ThemAminophyline_HuyetAp_TTr;
            }
            set
            {
                OnThemAminophyline_HuyetAp_TTrChanging(value);
                _ThemAminophyline_HuyetAp_TTr = value;
                RaisePropertyChanged("ThemAminophyline_HuyetAp_TTr");
                OnThemAminophyline_HuyetAp_TTrChanged();
            }
        }
        private double _ThemAminophyline_HuyetAp_TTr;
        partial void OnThemAminophyline_HuyetAp_TTrChanging(double value);
        partial void OnThemAminophyline_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThemAminophyline_TanSoTim
        {
            get
            {
                return _ThemAminophyline_TanSoTim;
            }
            set
            {
                OnThemAminophyline_TanSoTimChanging(value);
                _ThemAminophyline_TanSoTim = value;
                RaisePropertyChanged("ThemAminophyline_TanSoTim");
                OnThemAminophyline_TanSoTimChanged();
            }
        }
        private double _ThemAminophyline_TanSoTim;
        partial void OnThemAminophyline_TanSoTimChanging(double value);
        partial void OnThemAminophyline_TanSoTimChanged();




        [DataMemberAttribute()]
        public string ThemAminophyline_TacDungPhu
        {
            get
            {
                return _ThemAminophyline_TacDungPhu;
            }
            set
            {
                OnThemAminophyline_TacDungPhuChanging(value);
                _ThemAminophyline_TacDungPhu = value;
                RaisePropertyChanged("ThemAminophyline_TacDungPhu");
                OnThemAminophyline_TacDungPhuChanged();
            }
        }
        private string _ThemAminophyline_TacDungPhu;
        partial void OnThemAminophyline_TacDungPhuChanging(string value);
        partial void OnThemAminophyline_TacDungPhuChanged();



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
