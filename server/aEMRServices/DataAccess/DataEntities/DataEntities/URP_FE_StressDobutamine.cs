using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDobutamine : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDobutamine()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDobutamine info = obj as URP_FE_StressDobutamine;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDobutamineID > 0 && this.URP_FE_StressDobutamineID == info.URP_FE_StressDobutamineID;
        }
        private URP_FE_StressDobutamine _tempURP_FE_StressDobutamine;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDobutamine = (URP_FE_StressDobutamine)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDobutamine)
                CopyFrom(_tempURP_FE_StressDobutamine);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDobutamine p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDobutamine object.

        /// <param name="URP_FE_StressDobutamineID">Initial value of the URP_FE_StressDobutamineID property.</param>
        /// <param name="URP_FE_StressDobutamineName">Initial value of the URP_FE_StressDobutamineName property.</param>
        public static URP_FE_StressDobutamine CreateURP_FE_StressDobutamine(Byte URP_FE_StressDobutamineID, String URP_FE_StressDobutamineName)
        {
            URP_FE_StressDobutamine URP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            URP_FE_StressDobutamine.URP_FE_StressDobutamineID = URP_FE_StressDobutamineID;
            
            return URP_FE_StressDobutamine;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDobutamineID
        {
            get
            {
                return _URP_FE_StressDobutamineID;
            }
            set
            {
                if (_URP_FE_StressDobutamineID != value)
                {
                    OnURP_FE_StressDobutamineIDChanging(value);
                    _URP_FE_StressDobutamineID = value;
                    RaisePropertyChanged("URP_FE_StressDobutamineID");
                    OnURP_FE_StressDobutamineIDChanged();
                }
            }
        }
        private long _URP_FE_StressDobutamineID;
        partial void OnURP_FE_StressDobutamineIDChanging(long value);
        partial void OnURP_FE_StressDobutamineIDChanged();

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
        public bool TruyenTinhMach
        {
            get
            {
                return _TruyenTinhMach;
            }
            set
            {
                OnTruyenTinhMachChanging(value);
                _TruyenTinhMach = value;
                RaisePropertyChanged("TruyenTinhMach");
                OnTruyenTinhMachChanged();
            }
        }
        private bool _TruyenTinhMach;
        partial void OnTruyenTinhMachChanging(bool value);
        partial void OnTruyenTinhMachChanged();




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
        public double TD_TNP_HuyetAp_TT
        {
            get
            {
                return _TD_TNP_HuyetAp_TT;
            }
            set
            {
                OnTD_TNP_HuyetAp_TTChanging(value);
                _TD_TNP_HuyetAp_TT = value;
                RaisePropertyChanged("TD_TNP_HuyetAp_TT");
                OnTD_TNP_HuyetAp_TTChanged();
            }
        }
        private double _TD_TNP_HuyetAp_TT;
        partial void OnTD_TNP_HuyetAp_TTChanging(double value);
        partial void OnTD_TNP_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TD_TNP_HuyetAp_TTr
        {
            get
            {
                return _TD_TNP_HuyetAp_TTr;
            }
            set
            {
                OnTD_TNP_HuyetAp_TTrChanging(value);
                _TD_TNP_HuyetAp_TTr = value;
                RaisePropertyChanged("TD_TNP_HuyetAp_TTr");
                OnTD_TNP_HuyetAp_TTrChanged();
            }
        }
        private double _TD_TNP_HuyetAp_TTr;
        partial void OnTD_TNP_HuyetAp_TTrChanging(double value);
        partial void OnTD_TNP_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TD_TNP_HuyetAp_TanSoTim
        {
            get
            {
                return _TD_TNP_HuyetAp_TanSoTim;
            }
            set
            {
                OnTD_TNP_HuyetAp_TanSoTimChanging(value);
                _TD_TNP_HuyetAp_TanSoTim = value;
                RaisePropertyChanged("TD_TNP_HuyetAp_TanSoTim");
                OnTD_TNP_HuyetAp_TanSoTimChanged();
            }
        }
        private double _TD_TNP_HuyetAp_TanSoTim;
        partial void OnTD_TNP_HuyetAp_TanSoTimChanging(double value);
        partial void OnTD_TNP_HuyetAp_TanSoTimChanged();




        [DataMemberAttribute()]
        public double TD_TNP_HuyetAp_DoChenhApMin
        {
            get
            {
                return _TD_TNP_HuyetAp_DoChenhApMin;
            }
            set
            {
                OnTD_TNP_HuyetAp_DoChenhApMinChanging(value);
                _TD_TNP_HuyetAp_DoChenhApMin = value;
                RaisePropertyChanged("TD_TNP_HuyetAp_DoChenhApMin");
                OnTD_TNP_HuyetAp_DoChenhApMinChanged();
            }
        }
        private double _TD_TNP_HuyetAp_DoChenhApMin;
        partial void OnTD_TNP_HuyetAp_DoChenhApMinChanging(double value);
        partial void OnTD_TNP_HuyetAp_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double TD_TNP_HuyetAp_DoChenhApMax
        {
            get
            {
                return _TD_TNP_HuyetAp_DoChenhApMax;
            }
            set
            {
                OnTD_TNP_HuyetAp_DoChenhApMaxChanging(value);
                _TD_TNP_HuyetAp_DoChenhApMax = value;
                RaisePropertyChanged("TD_TNP_HuyetAp_DoChenhApMax");
                OnTD_TNP_HuyetAp_DoChenhApMaxChanged();
            }
        }
        private double _TD_TNP_HuyetAp_DoChenhApMax;
        partial void OnTD_TNP_HuyetAp_DoChenhApMaxChanging(double value);
        partial void OnTD_TNP_HuyetAp_DoChenhApMaxChanged();




        [DataMemberAttribute()]
        public double FiveMicro_DungLuong
        {
            get
            {
                return _FiveMicro_DungLuong;
            }
            set
            {
                OnFiveMicro_DungLuongChanging(value);
                _FiveMicro_DungLuong = value;
                RaisePropertyChanged("FiveMicro_DungLuong");
                OnFiveMicro_DungLuongChanged();
            }
        }
        private double _FiveMicro_DungLuong;
        partial void OnFiveMicro_DungLuongChanging(double value);
        partial void OnFiveMicro_DungLuongChanged();




        [DataMemberAttribute()]
        public double FiveMicro_HuyetAp_TT
        {
            get
            {
                return _FiveMicro_HuyetAp_TT;
            }
            set
            {
                OnFiveMicro_HuyetAp_TTChanging(value);
                _FiveMicro_HuyetAp_TT = value;
                RaisePropertyChanged("FiveMicro_HuyetAp_TT");
                OnFiveMicro_HuyetAp_TTChanged();
            }
        }
        private double _FiveMicro_HuyetAp_TT;
        partial void OnFiveMicro_HuyetAp_TTChanging(double value);
        partial void OnFiveMicro_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double FiveMicro_HuyetAp_TTr
        {
            get
            {
                return _FiveMicro_HuyetAp_TTr;
            }
            set
            {
                OnFiveMicro_HuyetAp_TTrChanging(value);
                _FiveMicro_HuyetAp_TTr = value;
                RaisePropertyChanged("FiveMicro_HuyetAp_TTr");
                OnFiveMicro_HuyetAp_TTrChanged();
            }
        }
        private double _FiveMicro_HuyetAp_TTr;
        partial void OnFiveMicro_HuyetAp_TTrChanging(double value);
        partial void OnFiveMicro_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double FiveMicro_TanSoTim
        {
            get
            {
                return _FiveMicro_TanSoTim;
            }
            set
            {
                OnFiveMicro_TanSoTimChanging(value);
                _FiveMicro_TanSoTim = value;
                RaisePropertyChanged("FiveMicro_TanSoTim");
                OnFiveMicro_TanSoTimChanged();
            }
        }
        private double _FiveMicro_TanSoTim;
        partial void OnFiveMicro_TanSoTimChanging(double value);
        partial void OnFiveMicro_TanSoTimChanged();




        [DataMemberAttribute()]
        public double FiveMicro_DoChenhApMin
        {
            get
            {
                return _FiveMicro_DoChenhApMin;
            }
            set
            {
                OnFiveMicro_DoChenhApMinChanging(value);
                _FiveMicro_DoChenhApMin = value;
                RaisePropertyChanged("FiveMicro_DoChenhApMin");
                OnFiveMicro_DoChenhApMinChanged();
            }
        }
        private double _FiveMicro_DoChenhApMin;
        partial void OnFiveMicro_DoChenhApMinChanging(double value);
        partial void OnFiveMicro_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double FiveMicro_DoChenhApMax
        {
            get
            {
                return _FiveMicro_DoChenhApMax;
            }
            set
            {
                OnFiveMicro_DoChenhApMaxChanging(value);
                _FiveMicro_DoChenhApMax = value;
                RaisePropertyChanged("FiveMicro_DoChenhApMax");
                OnFiveMicro_DoChenhApMaxChanged();
            }
        }
        private double _FiveMicro_DoChenhApMax;
        partial void OnFiveMicro_DoChenhApMaxChanging(double value);
        partial void OnFiveMicro_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double TenMicro_DungLuong
        {
            get
            {
                return _TenMicro_DungLuong;
            }
            set
            {
                OnTenMicro_DungLuongChanging(value);
                _TenMicro_DungLuong = value;
                RaisePropertyChanged("TenMicro_DungLuong");
                OnTenMicro_DungLuongChanged();
            }
        }
        private double _TenMicro_DungLuong;
        partial void OnTenMicro_DungLuongChanging(double value);
        partial void OnTenMicro_DungLuongChanged();




        [DataMemberAttribute()]
        public double TenMicro_HuyetAp_TT
        {
            get
            {
                return _TenMicro_HuyetAp_TT;
            }
            set
            {
                OnTenMicro_HuyetAp_TTChanging(value);
                _TenMicro_HuyetAp_TT = value;
                RaisePropertyChanged("TenMicro_HuyetAp_TT");
                OnTenMicro_HuyetAp_TTChanged();
            }
        }
        private double _TenMicro_HuyetAp_TT;
        partial void OnTenMicro_HuyetAp_TTChanging(double value);
        partial void OnTenMicro_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TenMicro_HuyetAp_TTr
        {
            get
            {
                return _TenMicro_HuyetAp_TTr;
            }
            set
            {
                OnTenMicro_HuyetAp_TTrChanging(value);
                _TenMicro_HuyetAp_TTr = value;
                RaisePropertyChanged("TenMicro_HuyetAp_TTr");
                OnTenMicro_HuyetAp_TTrChanged();
            }
        }
        private double _TenMicro_HuyetAp_TTr;
        partial void OnTenMicro_HuyetAp_TTrChanging(double value);
        partial void OnTenMicro_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TenMicro_TanSoTim
        {
            get
            {
                return _TenMicro_TanSoTim;
            }
            set
            {
                OnTenMicro_TanSoTimChanging(value);
                _TenMicro_TanSoTim = value;
                RaisePropertyChanged("TenMicro_TanSoTim");
                OnTenMicro_TanSoTimChanged();
            }
        }
        private double _TenMicro_TanSoTim;
        partial void OnTenMicro_TanSoTimChanging(double value);
        partial void OnTenMicro_TanSoTimChanged();




        [DataMemberAttribute()]
        public double TenMicro_DoChenhApMin
        {
            get
            {
                return _TenMicro_DoChenhApMin;
            }
            set
            {
                OnTenMicro_DoChenhApMinChanging(value);
                _TenMicro_DoChenhApMin = value;
                RaisePropertyChanged("TenMicro_DoChenhApMin");
                OnTenMicro_DoChenhApMinChanged();
            }
        }
        private double _TenMicro_DoChenhApMin;
        partial void OnTenMicro_DoChenhApMinChanging(double value);
        partial void OnTenMicro_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double TenMicro_DoChenhApMax
        {
            get
            {
                return _TenMicro_DoChenhApMax;
            }
            set
            {
                OnTenMicro_DoChenhApMaxChanging(value);
                _TenMicro_DoChenhApMax = value;
                RaisePropertyChanged("TenMicro_DoChenhApMax");
                OnTenMicro_DoChenhApMaxChanged();
            }
        }
        private double _TenMicro_DoChenhApMax;
        partial void OnTenMicro_DoChenhApMaxChanging(double value);
        partial void OnTenMicro_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double TwentyMicro_DungLuong
        {
            get
            {
                return _TwentyMicro_DungLuong;
            }
            set
            {
                OnTwentyMicro_DungLuongChanging(value);
                _TwentyMicro_DungLuong = value;
                RaisePropertyChanged("TwentyMicro_DungLuong");
                OnTwentyMicro_DungLuongChanged();
            }
        }
        private double _TwentyMicro_DungLuong;
        partial void OnTwentyMicro_DungLuongChanging(double value);
        partial void OnTwentyMicro_DungLuongChanged();




        [DataMemberAttribute()]
        public double TwentyMicro_HuyetAp_TT
        {
            get
            {
                return _TwentyMicro_HuyetAp_TT;
            }
            set
            {
                OnTwentyMicro_HuyetAp_TTChanging(value);
                _TwentyMicro_HuyetAp_TT = value;
                RaisePropertyChanged("TwentyMicro_HuyetAp_TT");
                OnTwentyMicro_HuyetAp_TTChanged();
            }
        }
        private double _TwentyMicro_HuyetAp_TT;
        partial void OnTwentyMicro_HuyetAp_TTChanging(double value);
        partial void OnTwentyMicro_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double TwentyMicro_HuyetAp_TTr
        {
            get
            {
                return _TwentyMicro_HuyetAp_TTr;
            }
            set
            {
                OnTwentyMicro_HuyetAp_TTrChanging(value);
                _TwentyMicro_HuyetAp_TTr = value;
                RaisePropertyChanged("TwentyMicro_HuyetAp_TTr");
                OnTwentyMicro_HuyetAp_TTrChanged();
            }
        }
        private double _TwentyMicro_HuyetAp_TTr;
        partial void OnTwentyMicro_HuyetAp_TTrChanging(double value);
        partial void OnTwentyMicro_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double TwentyMicro_TanSoTim
        {
            get
            {
                return _TwentyMicro_TanSoTim;
            }
            set
            {
                OnTwentyMicro_TanSoTimChanging(value);
                _TwentyMicro_TanSoTim = value;
                RaisePropertyChanged("TwentyMicro_TanSoTim");
                OnTwentyMicro_TanSoTimChanged();
            }
        }
        private double _TwentyMicro_TanSoTim;
        partial void OnTwentyMicro_TanSoTimChanging(double value);
        partial void OnTwentyMicro_TanSoTimChanged();




        [DataMemberAttribute()]
        public double TwentyMicro_DoChenhApMin
        {
            get
            {
                return _TwentyMicro_DoChenhApMin;
            }
            set
            {
                OnTwentyMicro_DoChenhApMinChanging(value);
                _TwentyMicro_DoChenhApMin = value;
                RaisePropertyChanged("TwentyMicro_DoChenhApMin");
                OnTwentyMicro_DoChenhApMinChanged();
            }
        }
        private double _TwentyMicro_DoChenhApMin;
        partial void OnTwentyMicro_DoChenhApMinChanging(double value);
        partial void OnTwentyMicro_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double TwentyMicro_DoChenhApMax
        {
            get
            {
                return _TwentyMicro_DoChenhApMax;
            }
            set
            {
                OnTwentyMicro_DoChenhApMaxChanging(value);
                _TwentyMicro_DoChenhApMax = value;
                RaisePropertyChanged("TwentyMicro_DoChenhApMax");
                OnTwentyMicro_DoChenhApMaxChanged();
            }
        }
        private double _TwentyMicro_DoChenhApMax;
        partial void OnTwentyMicro_DoChenhApMaxChanging(double value);
        partial void OnTwentyMicro_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double ThirtyMicro_DungLuong
        {
            get
            {
                return _ThirtyMicro_DungLuong;
            }
            set
            {
                OnThirtyMicro_DungLuongChanging(value);
                _ThirtyMicro_DungLuong = value;
                RaisePropertyChanged("ThirtyMicro_DungLuong");
                OnThirtyMicro_DungLuongChanged();
            }
        }
        private double _ThirtyMicro_DungLuong;
        partial void OnThirtyMicro_DungLuongChanging(double value);
        partial void OnThirtyMicro_DungLuongChanged();




        [DataMemberAttribute()]
        public double ThirtyMicro_HuyetAp_TT
        {
            get
            {
                return _ThirtyMicro_HuyetAp_TT;
            }
            set
            {
                OnThirtyMicro_HuyetAp_TTChanging(value);
                _ThirtyMicro_HuyetAp_TT = value;
                RaisePropertyChanged("ThirtyMicro_HuyetAp_TT");
                OnThirtyMicro_HuyetAp_TTChanged();
            }
        }
        private double _ThirtyMicro_HuyetAp_TT;
        partial void OnThirtyMicro_HuyetAp_TTChanging(double value);
        partial void OnThirtyMicro_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double ThirtyMicro_HuyetAp_TTr
        {
            get
            {
                return _ThirtyMicro_HuyetAp_TTr;
            }
            set
            {
                OnThirtyMicro_HuyetAp_TTrChanging(value);
                _ThirtyMicro_HuyetAp_TTr = value;
                RaisePropertyChanged("ThirtyMicro_HuyetAp_TTr");
                OnThirtyMicro_HuyetAp_TTrChanged();
            }
        }
        private double _ThirtyMicro_HuyetAp_TTr;
        partial void OnThirtyMicro_HuyetAp_TTrChanging(double value);
        partial void OnThirtyMicro_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double ThirtyMicro_TanSoTim
        {
            get
            {
                return _ThirtyMicro_TanSoTim;
            }
            set
            {
                OnThirtyMicro_TanSoTimChanging(value);
                _ThirtyMicro_TanSoTim = value;
                RaisePropertyChanged("ThirtyMicro_TanSoTim");
                OnThirtyMicro_TanSoTimChanged();
            }
        }
        private double _ThirtyMicro_TanSoTim;
        partial void OnThirtyMicro_TanSoTimChanging(double value);
        partial void OnThirtyMicro_TanSoTimChanged();




        [DataMemberAttribute()]
        public double ThirtyMicro_DoChenhApMin
        {
            get
            {
                return _ThirtyMicro_DoChenhApMin;
            }
            set
            {
                OnThirtyMicro_DoChenhApMinChanging(value);
                _ThirtyMicro_DoChenhApMin = value;
                RaisePropertyChanged("ThirtyMicro_DoChenhApMin");
                OnThirtyMicro_DoChenhApMinChanged();
            }
        }
        private double _ThirtyMicro_DoChenhApMin;
        partial void OnThirtyMicro_DoChenhApMinChanging(double value);
        partial void OnThirtyMicro_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double ThirtyMicro_DoChenhApMax
        {
            get
            {
                return _ThirtyMicro_DoChenhApMax;
            }
            set
            {
                OnThirtyMicro_DoChenhApMaxChanging(value);
                _ThirtyMicro_DoChenhApMax = value;
                RaisePropertyChanged("ThirtyMicro_DoChenhApMax");
                OnThirtyMicro_DoChenhApMaxChanged();
            }
        }
        private double _ThirtyMicro_DoChenhApMax;
        partial void OnThirtyMicro_DoChenhApMaxChanging(double value);
        partial void OnThirtyMicro_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double FortyMicro_DungLuong
        {
            get
            {
                return _FortyMicro_DungLuong;
            }
            set
            {
                OnFortyMicro_DungLuongChanging(value);
                _FortyMicro_DungLuong = value;
                RaisePropertyChanged("FortyMicro_DungLuong");
                OnFortyMicro_DungLuongChanged();
            }
        }
        private double _FortyMicro_DungLuong;
        partial void OnFortyMicro_DungLuongChanging(double value);
        partial void OnFortyMicro_DungLuongChanged();




        [DataMemberAttribute()]
        public double FortyMicro_HuyetAp_TT
        {
            get
            {
                return _FortyMicro_HuyetAp_TT;
            }
            set
            {
                OnFortyMicro_HuyetAp_TTChanging(value);
                _FortyMicro_HuyetAp_TT = value;
                RaisePropertyChanged("FortyMicro_HuyetAp_TT");
                OnFortyMicro_HuyetAp_TTChanged();
            }
        }
        private double _FortyMicro_HuyetAp_TT;
        partial void OnFortyMicro_HuyetAp_TTChanging(double value);
        partial void OnFortyMicro_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double FortyMicro_HuyetAp_TTr
        {
            get
            {
                return _FortyMicro_HuyetAp_TTr;
            }
            set
            {
                OnFortyMicro_HuyetAp_TTrChanging(value);
                _FortyMicro_HuyetAp_TTr = value;
                RaisePropertyChanged("FortyMicro_HuyetAp_TTr");
                OnFortyMicro_HuyetAp_TTrChanged();
            }
        }
        private double _FortyMicro_HuyetAp_TTr;
        partial void OnFortyMicro_HuyetAp_TTrChanging(double value);
        partial void OnFortyMicro_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double FortyMicro_TanSoTim
        {
            get
            {
                return _FortyMicro_TanSoTim;
            }
            set
            {
                OnFortyMicro_TanSoTimChanging(value);
                _FortyMicro_TanSoTim = value;
                RaisePropertyChanged("FortyMicro_TanSoTim");
                OnFortyMicro_TanSoTimChanged();
            }
        }
        private double _FortyMicro_TanSoTim;
        partial void OnFortyMicro_TanSoTimChanging(double value);
        partial void OnFortyMicro_TanSoTimChanged();




        [DataMemberAttribute()]
        public double FortyMicro_DoChenhApMin
        {
            get
            {
                return _FortyMicro_DoChenhApMin;
            }
            set
            {
                OnFortyMicro_DoChenhApMinChanging(value);
                _FortyMicro_DoChenhApMin = value;
                RaisePropertyChanged("FortyMicro_DoChenhApMin");
                OnFortyMicro_DoChenhApMinChanged();
            }
        }
        private double _FortyMicro_DoChenhApMin;
        partial void OnFortyMicro_DoChenhApMinChanging(double value);
        partial void OnFortyMicro_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double FortyMicro_DoChenhApMax
        {
            get
            {
                return _FortyMicro_DoChenhApMax;
            }
            set
            {
                OnFortyMicro_DoChenhApMaxChanging(value);
                _FortyMicro_DoChenhApMax = value;
                RaisePropertyChanged("FortyMicro_DoChenhApMax");
                OnFortyMicro_DoChenhApMaxChanged();
            }
        }
        private double _FortyMicro_DoChenhApMax;
        partial void OnFortyMicro_DoChenhApMaxChanging(double value);
        partial void OnFortyMicro_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double Atropine_DungLuong
        {
            get
            {
                return _Atropine_DungLuong;
            }
            set
            {
                OnAtropine_DungLuongChanging(value);
                _Atropine_DungLuong = value;
                RaisePropertyChanged("Atropine_DungLuong");
                OnAtropine_DungLuongChanged();
            }
        }
        private double _Atropine_DungLuong;
        partial void OnAtropine_DungLuongChanging(double value);
        partial void OnAtropine_DungLuongChanged();




        [DataMemberAttribute()]
        public double Atropine_HuyetAp_TT
        {
            get
            {
                return _Atropine_HuyetAp_TT;
            }
            set
            {
                OnAtropine_HuyetAp_TTChanging(value);
                _Atropine_HuyetAp_TT = value;
                RaisePropertyChanged("Atropine_HuyetAp_TT");
                OnAtropine_HuyetAp_TTChanged();
            }
        }
        private double _Atropine_HuyetAp_TT;
        partial void OnAtropine_HuyetAp_TTChanging(double value);
        partial void OnAtropine_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double Atropine_HuyetAp_TTr
        {
            get
            {
                return _Atropine_HuyetAp_TTr;
            }
            set
            {
                OnAtropine_HuyetAp_TTrChanging(value);
                _Atropine_HuyetAp_TTr = value;
                RaisePropertyChanged("Atropine_HuyetAp_TTr");
                OnAtropine_HuyetAp_TTrChanged();
            }
        }
        private double _Atropine_HuyetAp_TTr;
        partial void OnAtropine_HuyetAp_TTrChanging(double value);
        partial void OnAtropine_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double Atropine_TanSoTim
        {
            get
            {
                return _Atropine_TanSoTim;
            }
            set
            {
                OnAtropine_TanSoTimChanging(value);
                _Atropine_TanSoTim = value;
                RaisePropertyChanged("Atropine_TanSoTim");
                OnAtropine_TanSoTimChanged();
            }
        }
        private double _Atropine_TanSoTim;
        partial void OnAtropine_TanSoTimChanging(double value);
        partial void OnAtropine_TanSoTimChanged();




        [DataMemberAttribute()]
        public double Atropine_DoChenhApMin
        {
            get
            {
                return _Atropine_DoChenhApMin;
            }
            set
            {
                OnAtropine_DoChenhApMinChanging(value);
                _Atropine_DoChenhApMin = value;
                RaisePropertyChanged("Atropine_DoChenhApMin");
                OnAtropine_DoChenhApMinChanged();
            }
        }
        private double _Atropine_DoChenhApMin;
        partial void OnAtropine_DoChenhApMinChanging(double value);
        partial void OnAtropine_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double Atropine_DoChenhApMax
        {
            get
            {
                return _Atropine_DoChenhApMax;
            }
            set
            {
                OnAtropine_DoChenhApMaxChanging(value);
                _Atropine_DoChenhApMax = value;
                RaisePropertyChanged("Atropine_DoChenhApMax");
                OnAtropine_DoChenhApMaxChanged();
            }
        }
        private double _Atropine_DoChenhApMax;
        partial void OnAtropine_DoChenhApMaxChanging(double value);
        partial void OnAtropine_DoChenhApMaxChanged();


        [DataMemberAttribute()]
        public double NgungNP_ThoiGian
        {
            get
            {
                return _NgungNP_ThoiGian;
            }
            set
            {
                OnNgungNP_ThoiGianChanging(value);
                _NgungNP_ThoiGian = value;
                RaisePropertyChanged("NgungNP_ThoiGian");
                OnNgungNP_ThoiGianChanged();
            }
        }
        private double _NgungNP_ThoiGian;
        partial void OnNgungNP_ThoiGianChanging(double value);
        partial void OnNgungNP_ThoiGianChanged();




        [DataMemberAttribute()]
        public double NgungNP_HuyetAp_TT
        {
            get
            {
                return _NgungNP_HuyetAp_TT;
            }
            set
            {
                OnNgungNP_HuyetAp_TTChanging(value);
                _NgungNP_HuyetAp_TT = value;
                RaisePropertyChanged("NgungNP_HuyetAp_TT");
                OnNgungNP_HuyetAp_TTChanged();
            }
        }
        private double _NgungNP_HuyetAp_TT;
        partial void OnNgungNP_HuyetAp_TTChanging(double value);
        partial void OnNgungNP_HuyetAp_TTChanged();




        [DataMemberAttribute()]
        public double NgungNP_HuyetAp_TTr
        {
            get
            {
                return _NgungNP_HuyetAp_TTr;
            }
            set
            {
                OnNgungNP_HuyetAp_TTrChanging(value);
                _NgungNP_HuyetAp_TTr = value;
                RaisePropertyChanged("NgungNP_HuyetAp_TTr");
                OnNgungNP_HuyetAp_TTrChanged();
            }
        }
        private double _NgungNP_HuyetAp_TTr;
        partial void OnNgungNP_HuyetAp_TTrChanging(double value);
        partial void OnNgungNP_HuyetAp_TTrChanged();




        [DataMemberAttribute()]
        public double NgungNP_TanSoTim
        {
            get
            {
                return _NgungNP_TanSoTim;
            }
            set
            {
                OnNgungNP_TanSoTimChanging(value);
                _NgungNP_TanSoTim = value;
                RaisePropertyChanged("NgungNP_TanSoTim");
                OnNgungNP_TanSoTimChanged();
            }
        }
        private double _NgungNP_TanSoTim;
        partial void OnNgungNP_TanSoTimChanging(double value);
        partial void OnNgungNP_TanSoTimChanged();




        [DataMemberAttribute()]
        public double NgungNP_DoChenhApMin
        {
            get
            {
                return _NgungNP_DoChenhApMin;
            }
            set
            {
                OnNgungNP_DoChenhApMinChanging(value);
                _NgungNP_DoChenhApMin = value;
                RaisePropertyChanged("NgungNP_DoChenhApMin");
                OnNgungNP_DoChenhApMinChanged();
            }
        }
        private double _NgungNP_DoChenhApMin;
        partial void OnNgungNP_DoChenhApMinChanging(double value);
        partial void OnNgungNP_DoChenhApMinChanged();

        [DataMemberAttribute()]
        public double NgungNP_DoChenhApMax
        {
            get
            {
                return _NgungNP_DoChenhApMax;
            }
            set
            {
                OnNgungNP_DoChenhApMaxChanging(value);
                _NgungNP_DoChenhApMax = value;
                RaisePropertyChanged("NgungNP_DoChenhApMax");
                OnNgungNP_DoChenhApMaxChanged();
            }
        }
        private double _NgungNP_DoChenhApMax;
        partial void OnNgungNP_DoChenhApMaxChanging(double value);
        partial void OnNgungNP_DoChenhApMaxChanged();

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
