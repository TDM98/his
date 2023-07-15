using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDobutamineResult : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDobutamineResult()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDobutamineResult info = obj as URP_FE_StressDobutamineResult;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDobutamineResultID > 0 && this.URP_FE_StressDobutamineResultID == info.URP_FE_StressDobutamineResultID;
        }
        private URP_FE_StressDobutamineResult _tempURP_FE_StressDobutamineResult;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDobutamineResult = (URP_FE_StressDobutamineResult)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDobutamineResult)
                CopyFrom(_tempURP_FE_StressDobutamineResult);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDobutamineResult p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDobutamineResult object.

        /// <param name="URP_FE_StressDobutamineResultID">Initial value of the URP_FE_StressDobutamineResultID property.</param>
        /// <param name="URP_FE_StressDobutamineResultName">Initial value of the URP_FE_StressDobutamineResultName property.</param>
        public static URP_FE_StressDobutamineResult CreateURP_FE_StressDobutamineResult(Byte URP_FE_StressDobutamineResultID, String URP_FE_StressDobutamineResultName)
        {
            URP_FE_StressDobutamineResult URP_FE_StressDobutamineResult = new URP_FE_StressDobutamineResult();
            URP_FE_StressDobutamineResult.URP_FE_StressDobutamineResultID = URP_FE_StressDobutamineResultID;
            
            return URP_FE_StressDobutamineResult;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDobutamineResultID
        {
            get
            {
                return _URP_FE_StressDobutamineResultID;
            }
            set
            {
                if (_URP_FE_StressDobutamineResultID != value)
                {
                    OnURP_FE_StressDobutamineResultIDChanging(value);
                    _URP_FE_StressDobutamineResultID = value;
                    RaisePropertyChanged("URP_FE_StressDobutamineResultID");
                    OnURP_FE_StressDobutamineResultIDChanged();
                }
            }
        }
        private long _URP_FE_StressDobutamineResultID;
        partial void OnURP_FE_StressDobutamineResultIDChanging(long value);
        partial void OnURP_FE_StressDobutamineResultIDChanged();

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
        public bool ThayDoiDTD
        {
            get
            {
                return _ThayDoiDTD;
            }
            set
            {
                OnThayDoiDTDChanging(value);
                _ThayDoiDTD = value;
                RaisePropertyChanged("ThayDoiDTD");
                OnThayDoiDTDChanged();
            }
        }
        private bool _ThayDoiDTD;
        partial void OnThayDoiDTDChanging(bool value);
        partial void OnThayDoiDTDChanged();




        [DataMemberAttribute()]
        public string ThayDoiDTDChiTiet
        {
            get
            {
                return _ThayDoiDTDChiTiet;
            }
            set
            {
                OnThayDoiDTDChiTietChanging(value);
                _ThayDoiDTDChiTiet = value;
                RaisePropertyChanged("ThayDoiDTDChiTiet");
                OnThayDoiDTDChiTietChanged();
            }
        }
        private string _ThayDoiDTDChiTiet;
        partial void OnThayDoiDTDChiTietChanging(string value);
        partial void OnThayDoiDTDChiTietChanged();




        [DataMemberAttribute()]
        public bool RoiLoanNhip
        {
            get
            {
                return _RoiLoanNhip;
            }
            set
            {
                OnRoiLoanNhipChanging(value);
                _RoiLoanNhip = value;
                RaisePropertyChanged("RoiLoanNhip");
                OnRoiLoanNhipChanged();
            }
        }
        private bool _RoiLoanNhip;
        partial void OnRoiLoanNhipChanging(bool value);
        partial void OnRoiLoanNhipChanged();




        [DataMemberAttribute()]
        public string RoiLoanNhipChiTiet
        {
            get
            {
                return _RoiLoanNhipChiTiet;
            }
            set
            {
                OnRoiLoanNhipChiTietChanging(value);
                _RoiLoanNhipChiTiet = value;
                RaisePropertyChanged("RoiLoanNhipChiTiet");
                OnRoiLoanNhipChiTietChanged();
            }
        }
        private string _RoiLoanNhipChiTiet;
        partial void OnRoiLoanNhipChiTietChanging(string value);
        partial void OnRoiLoanNhipChiTietChanged();




        [DataMemberAttribute()]
        public int TDPHayBienChung
        {
            get
            {
                return _TDPHayBienChung;
            }
            set
            {
                OnTDPHayBienChungChanging(value);
                _TDPHayBienChung = value;
                RaisePropertyChanged("TDPHayBienChung");
                _V_ConDauThatNguc=(AllLookupValues.ChoiceEnum) _TDPHayBienChung;
                OnTDPHayBienChungChanged();
            }
        }
        private int _TDPHayBienChung;
        partial void OnTDPHayBienChungChanging(int value);
        partial void OnTDPHayBienChungChanged();




        [DataMemberAttribute()]
        public string TrieuChungKhac
        {
            get
            {
                return _TrieuChungKhac;
            }
            set
            {
                OnTrieuChungKhacChanging(value);
                _TrieuChungKhac = value;
                RaisePropertyChanged("TrieuChungKhac");
                OnTrieuChungKhacChanged();
            }
        }
        private string _TrieuChungKhac;
        partial void OnTrieuChungKhacChanging(string value);
        partial void OnTrieuChungKhacChanged();




        [DataMemberAttribute()]
        public string BienPhapDieuTri
        {
            get
            {
                return _BienPhapDieuTri;
            }
            set
            {
                OnBienPhapDieuTriChanging(value);
                _BienPhapDieuTri = value;
                RaisePropertyChanged("BienPhapDieuTri");
                OnBienPhapDieuTriChanged();
            }
        }
        private string _BienPhapDieuTri;
        partial void OnBienPhapDieuTriChanging(string value);
        partial void OnBienPhapDieuTriChanged();




        [DataMemberAttribute()]
        public long V_KetQuaSieuAmTim
        {
            get
            {
                return _V_KetQuaSieuAmTim;
            }
            set
            {
                OnV_KetQuaSieuAmTimChanging(value);
                _V_KetQuaSieuAmTim = value;
                RaisePropertyChanged("V_KetQuaSieuAmTim");
                OnV_KetQuaSieuAmTimChanged();
            }
        }
        private long _V_KetQuaSieuAmTim;
        partial void OnV_KetQuaSieuAmTimChanging(long value);
        partial void OnV_KetQuaSieuAmTimChanged();




        [DataMemberAttribute()]
        public string KetQuaSieuAmTim
        {
            get
            {
                return _KetQuaSieuAmTim;
            }
            set
            {
                OnKetQuaSieuAmTimChanging(value);
                _KetQuaSieuAmTim = value;
                RaisePropertyChanged("KetQuaSieuAmTim");
                OnKetQuaSieuAmTimChanged();
            }
        }
        private string _KetQuaSieuAmTim;
        partial void OnKetQuaSieuAmTimChanging(string value);
        partial void OnKetQuaSieuAmTimChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Mom_TNP
        {
            get
            {
                return _ThanhTruoc_Mom_TNP;
            }
            set
            {
                OnThanhTruoc_Mom_TNPChanging(value);
                _ThanhTruoc_Mom_TNP = value;
                RaisePropertyChanged("ThanhTruoc_Mom_TNP");
                OnThanhTruoc_Mom_TNPChanged();
            }
        }
        private long _ThanhTruoc_Mom_TNP;
        partial void OnThanhTruoc_Mom_TNPChanging(long value);
        partial void OnThanhTruoc_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Mom_DobuLieuThap
        {
            get
            {
                return _ThanhTruoc_Mom_DobuLieuThap;
            }
            set
            {
                OnThanhTruoc_Mom_DobuLieuThapChanging(value);
                _ThanhTruoc_Mom_DobuLieuThap = value;
                RaisePropertyChanged("ThanhTruoc_Mom_DobuLieuThap");
                OnThanhTruoc_Mom_DobuLieuThapChanged();
            }
        }
        private long _ThanhTruoc_Mom_DobuLieuThap;
        partial void OnThanhTruoc_Mom_DobuLieuThapChanging(long value);
        partial void OnThanhTruoc_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Mom_DobuLieuCao
        {
            get
            {
                return _ThanhTruoc_Mom_DobuLieuCao;
            }
            set
            {
                OnThanhTruoc_Mom_DobuLieuCaoChanging(value);
                _ThanhTruoc_Mom_DobuLieuCao = value;
                RaisePropertyChanged("ThanhTruoc_Mom_DobuLieuCao");
                OnThanhTruoc_Mom_DobuLieuCaoChanged();
            }
        }
        private long _ThanhTruoc_Mom_DobuLieuCao;
        partial void OnThanhTruoc_Mom_DobuLieuCaoChanging(long value);
        partial void OnThanhTruoc_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Mom_KetLuan
        {
            get
            {
                return _ThanhTruoc_Mom_KetLuan;
            }
            set
            {
                OnThanhTruoc_Mom_KetLuanChanging(value);
                _ThanhTruoc_Mom_KetLuan = value;
                RaisePropertyChanged("ThanhTruoc_Mom_KetLuan");
                OnThanhTruoc_Mom_KetLuanChanged();
            }
        }
        private long _ThanhTruoc_Mom_KetLuan;
        partial void OnThanhTruoc_Mom_KetLuanChanging(long value);
        partial void OnThanhTruoc_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Giua_TNP
        {
            get
            {
                return _ThanhTruoc_Giua_TNP;
            }
            set
            {
                OnThanhTruoc_Giua_TNPChanging(value);
                _ThanhTruoc_Giua_TNP = value;
                RaisePropertyChanged("ThanhTruoc_Giua_TNP");
                OnThanhTruoc_Giua_TNPChanged();
            }
        }
        private long _ThanhTruoc_Giua_TNP;
        partial void OnThanhTruoc_Giua_TNPChanging(long value);
        partial void OnThanhTruoc_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Giua_DobuLieuThap
        {
            get
            {
                return _ThanhTruoc_Giua_DobuLieuThap;
            }
            set
            {
                OnThanhTruoc_Giua_DobuLieuThapChanging(value);
                _ThanhTruoc_Giua_DobuLieuThap = value;
                RaisePropertyChanged("ThanhTruoc_Giua_DobuLieuThap");
                OnThanhTruoc_Giua_DobuLieuThapChanged();
            }
        }
        private long _ThanhTruoc_Giua_DobuLieuThap;
        partial void OnThanhTruoc_Giua_DobuLieuThapChanging(long value);
        partial void OnThanhTruoc_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Giua_DobuLieuCao
        {
            get
            {
                return _ThanhTruoc_Giua_DobuLieuCao;
            }
            set
            {
                OnThanhTruoc_Giua_DobuLieuCaoChanging(value);
                _ThanhTruoc_Giua_DobuLieuCao = value;
                RaisePropertyChanged("ThanhTruoc_Giua_DobuLieuCao");
                OnThanhTruoc_Giua_DobuLieuCaoChanged();
            }
        }
        private long _ThanhTruoc_Giua_DobuLieuCao;
        partial void OnThanhTruoc_Giua_DobuLieuCaoChanging(long value);
        partial void OnThanhTruoc_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Giua_KetLuan
        {
            get
            {
                return _ThanhTruoc_Giua_KetLuan;
            }
            set
            {
                OnThanhTruoc_Giua_KetLuanChanging(value);
                _ThanhTruoc_Giua_KetLuan = value;
                RaisePropertyChanged("ThanhTruoc_Giua_KetLuan");
                OnThanhTruoc_Giua_KetLuanChanged();
            }
        }
        private long _ThanhTruoc_Giua_KetLuan;
        partial void OnThanhTruoc_Giua_KetLuanChanging(long value);
        partial void OnThanhTruoc_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Day_TNP
        {
            get
            {
                return _ThanhTruoc_Day_TNP;
            }
            set
            {
                OnThanhTruoc_Day_TNPChanging(value);
                _ThanhTruoc_Day_TNP = value;
                RaisePropertyChanged("ThanhTruoc_Day_TNP");
                OnThanhTruoc_Day_TNPChanged();
            }
        }
        private long _ThanhTruoc_Day_TNP;
        partial void OnThanhTruoc_Day_TNPChanging(long value);
        partial void OnThanhTruoc_Day_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Day_DobuLieuThap
        {
            get
            {
                return _ThanhTruoc_Day_DobuLieuThap;
            }
            set
            {
                OnThanhTruoc_Day_DobuLieuThapChanging(value);
                _ThanhTruoc_Day_DobuLieuThap = value;
                RaisePropertyChanged("ThanhTruoc_Day_DobuLieuThap");
                OnThanhTruoc_Day_DobuLieuThapChanged();
            }
        }
        private long _ThanhTruoc_Day_DobuLieuThap;
        partial void OnThanhTruoc_Day_DobuLieuThapChanging(long value);
        partial void OnThanhTruoc_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Day_DobuLieuCao
        {
            get
            {
                return _ThanhTruoc_Day_DobuLieuCao;
            }
            set
            {
                OnThanhTruoc_Day_DobuLieuCaoChanging(value);
                _ThanhTruoc_Day_DobuLieuCao = value;
                RaisePropertyChanged("ThanhTruoc_Day_DobuLieuCao");
                OnThanhTruoc_Day_DobuLieuCaoChanged();
            }
        }
        private long _ThanhTruoc_Day_DobuLieuCao;
        partial void OnThanhTruoc_Day_DobuLieuCaoChanging(long value);
        partial void OnThanhTruoc_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhTruoc_Day_KetLuan
        {
            get
            {
                return _ThanhTruoc_Day_KetLuan;
            }
            set
            {
                OnThanhTruoc_Day_KetLuanChanging(value);
                _ThanhTruoc_Day_KetLuan = value;
                RaisePropertyChanged("ThanhTruoc_Day_KetLuan");
                OnThanhTruoc_Day_KetLuanChanged();
            }
        }
        private long _ThanhTruoc_Day_KetLuan;
        partial void OnThanhTruoc_Day_KetLuanChanging(long value);
        partial void OnThanhTruoc_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Mom_TNP
        {
            get
            {
                return _VanhLienThat_Mom_TNP;
            }
            set
            {
                OnVanhLienThat_Mom_TNPChanging(value);
                _VanhLienThat_Mom_TNP = value;
                RaisePropertyChanged("VanhLienThat_Mom_TNP");
                OnVanhLienThat_Mom_TNPChanged();
            }
        }
        private long _VanhLienThat_Mom_TNP;
        partial void OnVanhLienThat_Mom_TNPChanging(long value);
        partial void OnVanhLienThat_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Mom_DobuLieuThap
        {
            get
            {
                return _VanhLienThat_Mom_DobuLieuThap;
            }
            set
            {
                OnVanhLienThat_Mom_DobuLieuThapChanging(value);
                _VanhLienThat_Mom_DobuLieuThap = value;
                RaisePropertyChanged("VanhLienThat_Mom_DobuLieuThap");
                OnVanhLienThat_Mom_DobuLieuThapChanged();
            }
        }
        private long _VanhLienThat_Mom_DobuLieuThap;
        partial void OnVanhLienThat_Mom_DobuLieuThapChanging(long value);
        partial void OnVanhLienThat_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Mom_DobuLieuCao
        {
            get
            {
                return _VanhLienThat_Mom_DobuLieuCao;
            }
            set
            {
                OnVanhLienThat_Mom_DobuLieuCaoChanging(value);
                _VanhLienThat_Mom_DobuLieuCao = value;
                RaisePropertyChanged("VanhLienThat_Mom_DobuLieuCao");
                OnVanhLienThat_Mom_DobuLieuCaoChanged();
            }
        }
        private long _VanhLienThat_Mom_DobuLieuCao;
        partial void OnVanhLienThat_Mom_DobuLieuCaoChanging(long value);
        partial void OnVanhLienThat_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Mom_KetLuan
        {
            get
            {
                return _VanhLienThat_Mom_KetLuan;
            }
            set
            {
                OnVanhLienThat_Mom_KetLuanChanging(value);
                _VanhLienThat_Mom_KetLuan = value;
                RaisePropertyChanged("VanhLienThat_Mom_KetLuan");
                OnVanhLienThat_Mom_KetLuanChanged();
            }
        }
        private long _VanhLienThat_Mom_KetLuan;
        partial void OnVanhLienThat_Mom_KetLuanChanging(long value);
        partial void OnVanhLienThat_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Giua_TNP
        {
            get
            {
                return _VanhLienThat_Giua_TNP;
            }
            set
            {
                OnVanhLienThat_Giua_TNPChanging(value);
                _VanhLienThat_Giua_TNP = value;
                RaisePropertyChanged("VanhLienThat_Giua_TNP");
                OnVanhLienThat_Giua_TNPChanged();
            }
        }
        private long _VanhLienThat_Giua_TNP;
        partial void OnVanhLienThat_Giua_TNPChanging(long value);
        partial void OnVanhLienThat_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Giua_DobuLieuThap
        {
            get
            {
                return _VanhLienThat_Giua_DobuLieuThap;
            }
            set
            {
                OnVanhLienThat_Giua_DobuLieuThapChanging(value);
                _VanhLienThat_Giua_DobuLieuThap = value;
                RaisePropertyChanged("VanhLienThat_Giua_DobuLieuThap");
                OnVanhLienThat_Giua_DobuLieuThapChanged();
            }
        }
        private long _VanhLienThat_Giua_DobuLieuThap;
        partial void OnVanhLienThat_Giua_DobuLieuThapChanging(long value);
        partial void OnVanhLienThat_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Giua_DobuLieuCao
        {
            get
            {
                return _VanhLienThat_Giua_DobuLieuCao;
            }
            set
            {
                OnVanhLienThat_Giua_DobuLieuCaoChanging(value);
                _VanhLienThat_Giua_DobuLieuCao = value;
                RaisePropertyChanged("VanhLienThat_Giua_DobuLieuCao");
                OnVanhLienThat_Giua_DobuLieuCaoChanged();
            }
        }
        private long _VanhLienThat_Giua_DobuLieuCao;
        partial void OnVanhLienThat_Giua_DobuLieuCaoChanging(long value);
        partial void OnVanhLienThat_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Giua_KetLuan
        {
            get
            {
                return _VanhLienThat_Giua_KetLuan;
            }
            set
            {
                OnVanhLienThat_Giua_KetLuanChanging(value);
                _VanhLienThat_Giua_KetLuan = value;
                RaisePropertyChanged("VanhLienThat_Giua_KetLuan");
                OnVanhLienThat_Giua_KetLuanChanged();
            }
        }
        private long _VanhLienThat_Giua_KetLuan;
        partial void OnVanhLienThat_Giua_KetLuanChanging(long value);
        partial void OnVanhLienThat_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Day_TNP
        {
            get
            {
                return _VanhLienThat_Day_TNP;
            }
            set
            {
                OnVanhLienThat_Day_TNPChanging(value);
                _VanhLienThat_Day_TNP = value;
                RaisePropertyChanged("VanhLienThat_Day_TNP");
                OnVanhLienThat_Day_TNPChanged();
            }
        }
        private long _VanhLienThat_Day_TNP;
        partial void OnVanhLienThat_Day_TNPChanging(long value);
        partial void OnVanhLienThat_Day_TNPChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Day_DobuLieuThap
        {
            get
            {
                return _VanhLienThat_Day_DobuLieuThap;
            }
            set
            {
                OnVanhLienThat_Day_DobuLieuThapChanging(value);
                _VanhLienThat_Day_DobuLieuThap = value;
                RaisePropertyChanged("VanhLienThat_Day_DobuLieuThap");
                OnVanhLienThat_Day_DobuLieuThapChanged();
            }
        }
        private long _VanhLienThat_Day_DobuLieuThap;
        partial void OnVanhLienThat_Day_DobuLieuThapChanging(long value);
        partial void OnVanhLienThat_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Day_DobuLieuCao
        {
            get
            {
                return _VanhLienThat_Day_DobuLieuCao;
            }
            set
            {
                OnVanhLienThat_Day_DobuLieuCaoChanging(value);
                _VanhLienThat_Day_DobuLieuCao = value;
                RaisePropertyChanged("VanhLienThat_Day_DobuLieuCao");
                OnVanhLienThat_Day_DobuLieuCaoChanged();
            }
        }
        private long _VanhLienThat_Day_DobuLieuCao;
        partial void OnVanhLienThat_Day_DobuLieuCaoChanging(long value);
        partial void OnVanhLienThat_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long VanhLienThat_Day_KetLuan
        {
            get
            {
                return _VanhLienThat_Day_KetLuan;
            }
            set
            {
                OnVanhLienThat_Day_KetLuanChanging(value);
                _VanhLienThat_Day_KetLuan = value;
                RaisePropertyChanged("VanhLienThat_Day_KetLuan");
                OnVanhLienThat_Day_KetLuanChanged();
            }
        }
        private long _VanhLienThat_Day_KetLuan;
        partial void OnVanhLienThat_Day_KetLuanChanging(long value);
        partial void OnVanhLienThat_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Mom_TNP
        {
            get
            {
                return _ThanhDuoi_Mom_TNP;
            }
            set
            {
                OnThanhDuoi_Mom_TNPChanging(value);
                _ThanhDuoi_Mom_TNP = value;
                RaisePropertyChanged("ThanhDuoi_Mom_TNP");
                OnThanhDuoi_Mom_TNPChanged();
            }
        }
        private long _ThanhDuoi_Mom_TNP;
        partial void OnThanhDuoi_Mom_TNPChanging(long value);
        partial void OnThanhDuoi_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Mom_DobuLieuThap
        {
            get
            {
                return _ThanhDuoi_Mom_DobuLieuThap;
            }
            set
            {
                OnThanhDuoi_Mom_DobuLieuThapChanging(value);
                _ThanhDuoi_Mom_DobuLieuThap = value;
                RaisePropertyChanged("ThanhDuoi_Mom_DobuLieuThap");
                OnThanhDuoi_Mom_DobuLieuThapChanged();
            }
        }
        private long _ThanhDuoi_Mom_DobuLieuThap;
        partial void OnThanhDuoi_Mom_DobuLieuThapChanging(long value);
        partial void OnThanhDuoi_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Mom_DobuLieuCao
        {
            get
            {
                return _ThanhDuoi_Mom_DobuLieuCao;
            }
            set
            {
                OnThanhDuoi_Mom_DobuLieuCaoChanging(value);
                _ThanhDuoi_Mom_DobuLieuCao = value;
                RaisePropertyChanged("ThanhDuoi_Mom_DobuLieuCao");
                OnThanhDuoi_Mom_DobuLieuCaoChanged();
            }
        }
        private long _ThanhDuoi_Mom_DobuLieuCao;
        partial void OnThanhDuoi_Mom_DobuLieuCaoChanging(long value);
        partial void OnThanhDuoi_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Mom_KetLuan
        {
            get
            {
                return _ThanhDuoi_Mom_KetLuan;
            }
            set
            {
                OnThanhDuoi_Mom_KetLuanChanging(value);
                _ThanhDuoi_Mom_KetLuan = value;
                RaisePropertyChanged("ThanhDuoi_Mom_KetLuan");
                OnThanhDuoi_Mom_KetLuanChanged();
            }
        }
        private long _ThanhDuoi_Mom_KetLuan;
        partial void OnThanhDuoi_Mom_KetLuanChanging(long value);
        partial void OnThanhDuoi_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Giua_TNP
        {
            get
            {
                return _ThanhDuoi_Giua_TNP;
            }
            set
            {
                OnThanhDuoi_Giua_TNPChanging(value);
                _ThanhDuoi_Giua_TNP = value;
                RaisePropertyChanged("ThanhDuoi_Giua_TNP");
                OnThanhDuoi_Giua_TNPChanged();
            }
        }
        private long _ThanhDuoi_Giua_TNP;
        partial void OnThanhDuoi_Giua_TNPChanging(long value);
        partial void OnThanhDuoi_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Giua_DobuLieuThap
        {
            get
            {
                return _ThanhDuoi_Giua_DobuLieuThap;
            }
            set
            {
                OnThanhDuoi_Giua_DobuLieuThapChanging(value);
                _ThanhDuoi_Giua_DobuLieuThap = value;
                RaisePropertyChanged("ThanhDuoi_Giua_DobuLieuThap");
                OnThanhDuoi_Giua_DobuLieuThapChanged();
            }
        }
        private long _ThanhDuoi_Giua_DobuLieuThap;
        partial void OnThanhDuoi_Giua_DobuLieuThapChanging(long value);
        partial void OnThanhDuoi_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Giua_DobuLieuCao
        {
            get
            {
                return _ThanhDuoi_Giua_DobuLieuCao;
            }
            set
            {
                OnThanhDuoi_Giua_DobuLieuCaoChanging(value);
                _ThanhDuoi_Giua_DobuLieuCao = value;
                RaisePropertyChanged("ThanhDuoi_Giua_DobuLieuCao");
                OnThanhDuoi_Giua_DobuLieuCaoChanged();
            }
        }
        private long _ThanhDuoi_Giua_DobuLieuCao;
        partial void OnThanhDuoi_Giua_DobuLieuCaoChanging(long value);
        partial void OnThanhDuoi_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Giua_KetLuan
        {
            get
            {
                return _ThanhDuoi_Giua_KetLuan;
            }
            set
            {
                OnThanhDuoi_Giua_KetLuanChanging(value);
                _ThanhDuoi_Giua_KetLuan = value;
                RaisePropertyChanged("ThanhDuoi_Giua_KetLuan");
                OnThanhDuoi_Giua_KetLuanChanged();
            }
        }
        private long _ThanhDuoi_Giua_KetLuan;
        partial void OnThanhDuoi_Giua_KetLuanChanging(long value);
        partial void OnThanhDuoi_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Day_TNP
        {
            get
            {
                return _ThanhDuoi_Day_TNP;
            }
            set
            {
                OnThanhDuoi_Day_TNPChanging(value);
                _ThanhDuoi_Day_TNP = value;
                RaisePropertyChanged("ThanhDuoi_Day_TNP");
                OnThanhDuoi_Day_TNPChanged();
            }
        }
        private long _ThanhDuoi_Day_TNP;
        partial void OnThanhDuoi_Day_TNPChanging(long value);
        partial void OnThanhDuoi_Day_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Day_DobuLieuThap
        {
            get
            {
                return _ThanhDuoi_Day_DobuLieuThap;
            }
            set
            {
                OnThanhDuoi_Day_DobuLieuThapChanging(value);
                _ThanhDuoi_Day_DobuLieuThap = value;
                RaisePropertyChanged("ThanhDuoi_Day_DobuLieuThap");
                OnThanhDuoi_Day_DobuLieuThapChanged();
            }
        }
        private long _ThanhDuoi_Day_DobuLieuThap;
        partial void OnThanhDuoi_Day_DobuLieuThapChanging(long value);
        partial void OnThanhDuoi_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Day_DobuLieuCao
        {
            get
            {
                return _ThanhDuoi_Day_DobuLieuCao;
            }
            set
            {
                OnThanhDuoi_Day_DobuLieuCaoChanging(value);
                _ThanhDuoi_Day_DobuLieuCao = value;
                RaisePropertyChanged("ThanhDuoi_Day_DobuLieuCao");
                OnThanhDuoi_Day_DobuLieuCaoChanged();
            }
        }
        private long _ThanhDuoi_Day_DobuLieuCao;
        partial void OnThanhDuoi_Day_DobuLieuCaoChanging(long value);
        partial void OnThanhDuoi_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhDuoi_Day_KetLuan
        {
            get
            {
                return _ThanhDuoi_Day_KetLuan;
            }
            set
            {
                OnThanhDuoi_Day_KetLuanChanging(value);
                _ThanhDuoi_Day_KetLuan = value;
                RaisePropertyChanged("ThanhDuoi_Day_KetLuan");
                OnThanhDuoi_Day_KetLuanChanged();
            }
        }
        private long _ThanhDuoi_Day_KetLuan;
        partial void OnThanhDuoi_Day_KetLuanChanging(long value);
        partial void OnThanhDuoi_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Mom_TNP
        {
            get
            {
                return _ThanhSau_Mom_TNP;
            }
            set
            {
                OnThanhSau_Mom_TNPChanging(value);
                _ThanhSau_Mom_TNP = value;
                RaisePropertyChanged("ThanhSau_Mom_TNP");
                OnThanhSau_Mom_TNPChanged();
            }
        }
        private long _ThanhSau_Mom_TNP;
        partial void OnThanhSau_Mom_TNPChanging(long value);
        partial void OnThanhSau_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Mom_DobuLieuThap
        {
            get
            {
                return _ThanhSau_Mom_DobuLieuThap;
            }
            set
            {
                OnThanhSau_Mom_DobuLieuThapChanging(value);
                _ThanhSau_Mom_DobuLieuThap = value;
                RaisePropertyChanged("ThanhSau_Mom_DobuLieuThap");
                OnThanhSau_Mom_DobuLieuThapChanged();
            }
        }
        private long _ThanhSau_Mom_DobuLieuThap;
        partial void OnThanhSau_Mom_DobuLieuThapChanging(long value);
        partial void OnThanhSau_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Mom_DobuLieuCao
        {
            get
            {
                return _ThanhSau_Mom_DobuLieuCao;
            }
            set
            {
                OnThanhSau_Mom_DobuLieuCaoChanging(value);
                _ThanhSau_Mom_DobuLieuCao = value;
                RaisePropertyChanged("ThanhSau_Mom_DobuLieuCao");
                OnThanhSau_Mom_DobuLieuCaoChanged();
            }
        }
        private long _ThanhSau_Mom_DobuLieuCao;
        partial void OnThanhSau_Mom_DobuLieuCaoChanging(long value);
        partial void OnThanhSau_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Mom_KetLuan
        {
            get
            {
                return _ThanhSau_Mom_KetLuan;
            }
            set
            {
                OnThanhSau_Mom_KetLuanChanging(value);
                _ThanhSau_Mom_KetLuan = value;
                RaisePropertyChanged("ThanhSau_Mom_KetLuan");
                OnThanhSau_Mom_KetLuanChanged();
            }
        }
        private long _ThanhSau_Mom_KetLuan;
        partial void OnThanhSau_Mom_KetLuanChanging(long value);
        partial void OnThanhSau_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Giua_TNP
        {
            get
            {
                return _ThanhSau_Giua_TNP;
            }
            set
            {
                OnThanhSau_Giua_TNPChanging(value);
                _ThanhSau_Giua_TNP = value;
                RaisePropertyChanged("ThanhSau_Giua_TNP");
                OnThanhSau_Giua_TNPChanged();
            }
        }
        private long _ThanhSau_Giua_TNP;
        partial void OnThanhSau_Giua_TNPChanging(long value);
        partial void OnThanhSau_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Giua_DobuLieuThap
        {
            get
            {
                return _ThanhSau_Giua_DobuLieuThap;
            }
            set
            {
                OnThanhSau_Giua_DobuLieuThapChanging(value);
                _ThanhSau_Giua_DobuLieuThap = value;
                RaisePropertyChanged("ThanhSau_Giua_DobuLieuThap");
                OnThanhSau_Giua_DobuLieuThapChanged();
            }
        }
        private long _ThanhSau_Giua_DobuLieuThap;
        partial void OnThanhSau_Giua_DobuLieuThapChanging(long value);
        partial void OnThanhSau_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Giua_DobuLieuCao
        {
            get
            {
                return _ThanhSau_Giua_DobuLieuCao;
            }
            set
            {
                OnThanhSau_Giua_DobuLieuCaoChanging(value);
                _ThanhSau_Giua_DobuLieuCao = value;
                RaisePropertyChanged("ThanhSau_Giua_DobuLieuCao");
                OnThanhSau_Giua_DobuLieuCaoChanged();
            }
        }
        private long _ThanhSau_Giua_DobuLieuCao;
        partial void OnThanhSau_Giua_DobuLieuCaoChanging(long value);
        partial void OnThanhSau_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Giua_KetLuan
        {
            get
            {
                return _ThanhSau_Giua_KetLuan;
            }
            set
            {
                OnThanhSau_Giua_KetLuanChanging(value);
                _ThanhSau_Giua_KetLuan = value;
                RaisePropertyChanged("ThanhSau_Giua_KetLuan");
                OnThanhSau_Giua_KetLuanChanged();
            }
        }
        private long _ThanhSau_Giua_KetLuan;
        partial void OnThanhSau_Giua_KetLuanChanging(long value);
        partial void OnThanhSau_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Day_TNP
        {
            get
            {
                return _ThanhSau_Day_TNP;
            }
            set
            {
                OnThanhSau_Day_TNPChanging(value);
                _ThanhSau_Day_TNP = value;
                RaisePropertyChanged("ThanhSau_Day_TNP");
                OnThanhSau_Day_TNPChanged();
            }
        }
        private long _ThanhSau_Day_TNP;
        partial void OnThanhSau_Day_TNPChanging(long value);
        partial void OnThanhSau_Day_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Day_DobuLieuThap
        {
            get
            {
                return _ThanhSau_Day_DobuLieuThap;
            }
            set
            {
                OnThanhSau_Day_DobuLieuThapChanging(value);
                _ThanhSau_Day_DobuLieuThap = value;
                RaisePropertyChanged("ThanhSau_Day_DobuLieuThap");
                OnThanhSau_Day_DobuLieuThapChanged();
            }
        }
        private long _ThanhSau_Day_DobuLieuThap;
        partial void OnThanhSau_Day_DobuLieuThapChanging(long value);
        partial void OnThanhSau_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Day_DobuLieuCao
        {
            get
            {
                return _ThanhSau_Day_DobuLieuCao;
            }
            set
            {
                OnThanhSau_Day_DobuLieuCaoChanging(value);
                _ThanhSau_Day_DobuLieuCao = value;
                RaisePropertyChanged("ThanhSau_Day_DobuLieuCao");
                OnThanhSau_Day_DobuLieuCaoChanged();
            }
        }
        private long _ThanhSau_Day_DobuLieuCao;
        partial void OnThanhSau_Day_DobuLieuCaoChanging(long value);
        partial void OnThanhSau_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhSau_Day_KetLuan
        {
            get
            {
                return _ThanhSau_Day_KetLuan;
            }
            set
            {
                OnThanhSau_Day_KetLuanChanging(value);
                _ThanhSau_Day_KetLuan = value;
                RaisePropertyChanged("ThanhSau_Day_KetLuan");
                OnThanhSau_Day_KetLuanChanged();
            }
        }
        private long _ThanhSau_Day_KetLuan;
        partial void OnThanhSau_Day_KetLuanChanging(long value);
        partial void OnThanhSau_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Mom_TNP
        {
            get
            {
                return _ThanhBen_Mom_TNP;
            }
            set
            {
                OnThanhBen_Mom_TNPChanging(value);
                _ThanhBen_Mom_TNP = value;
                RaisePropertyChanged("ThanhBen_Mom_TNP");
                OnThanhBen_Mom_TNPChanged();
            }
        }
        private long _ThanhBen_Mom_TNP;
        partial void OnThanhBen_Mom_TNPChanging(long value);
        partial void OnThanhBen_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Mom_DobuLieuThap
        {
            get
            {
                return _ThanhBen_Mom_DobuLieuThap;
            }
            set
            {
                OnThanhBen_Mom_DobuLieuThapChanging(value);
                _ThanhBen_Mom_DobuLieuThap = value;
                RaisePropertyChanged("ThanhBen_Mom_DobuLieuThap");
                OnThanhBen_Mom_DobuLieuThapChanged();
            }
        }
        private long _ThanhBen_Mom_DobuLieuThap;
        partial void OnThanhBen_Mom_DobuLieuThapChanging(long value);
        partial void OnThanhBen_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Mom_DobuLieuCao
        {
            get
            {
                return _ThanhBen_Mom_DobuLieuCao;
            }
            set
            {
                OnThanhBen_Mom_DobuLieuCaoChanging(value);
                _ThanhBen_Mom_DobuLieuCao = value;
                RaisePropertyChanged("ThanhBen_Mom_DobuLieuCao");
                OnThanhBen_Mom_DobuLieuCaoChanged();
            }
        }
        private long _ThanhBen_Mom_DobuLieuCao;
        partial void OnThanhBen_Mom_DobuLieuCaoChanging(long value);
        partial void OnThanhBen_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Mom_KetLuan
        {
            get
            {
                return _ThanhBen_Mom_KetLuan;
            }
            set
            {
                OnThanhBen_Mom_KetLuanChanging(value);
                _ThanhBen_Mom_KetLuan = value;
                RaisePropertyChanged("ThanhBen_Mom_KetLuan");
                OnThanhBen_Mom_KetLuanChanged();
            }
        }
        private long _ThanhBen_Mom_KetLuan;
        partial void OnThanhBen_Mom_KetLuanChanging(long value);
        partial void OnThanhBen_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Giua_TNP
        {
            get
            {
                return _ThanhBen_Giua_TNP;
            }
            set
            {
                OnThanhBen_Giua_TNPChanging(value);
                _ThanhBen_Giua_TNP = value;
                RaisePropertyChanged("ThanhBen_Giua_TNP");
                OnThanhBen_Giua_TNPChanged();
            }
        }
        private long _ThanhBen_Giua_TNP;
        partial void OnThanhBen_Giua_TNPChanging(long value);
        partial void OnThanhBen_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Giua_DobuLieuThap
        {
            get
            {
                return _ThanhBen_Giua_DobuLieuThap;
            }
            set
            {
                OnThanhBen_Giua_DobuLieuThapChanging(value);
                _ThanhBen_Giua_DobuLieuThap = value;
                RaisePropertyChanged("ThanhBen_Giua_DobuLieuThap");
                OnThanhBen_Giua_DobuLieuThapChanged();
            }
        }
        private long _ThanhBen_Giua_DobuLieuThap;
        partial void OnThanhBen_Giua_DobuLieuThapChanging(long value);
        partial void OnThanhBen_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Giua_DobuLieuCao
        {
            get
            {
                return _ThanhBen_Giua_DobuLieuCao;
            }
            set
            {
                OnThanhBen_Giua_DobuLieuCaoChanging(value);
                _ThanhBen_Giua_DobuLieuCao = value;
                RaisePropertyChanged("ThanhBen_Giua_DobuLieuCao");
                OnThanhBen_Giua_DobuLieuCaoChanged();
            }
        }
        private long _ThanhBen_Giua_DobuLieuCao;
        partial void OnThanhBen_Giua_DobuLieuCaoChanging(long value);
        partial void OnThanhBen_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Giua_KetLuan
        {
            get
            {
                return _ThanhBen_Giua_KetLuan;
            }
            set
            {
                OnThanhBen_Giua_KetLuanChanging(value);
                _ThanhBen_Giua_KetLuan = value;
                RaisePropertyChanged("ThanhBen_Giua_KetLuan");
                OnThanhBen_Giua_KetLuanChanged();
            }
        }
        private long _ThanhBen_Giua_KetLuan;
        partial void OnThanhBen_Giua_KetLuanChanging(long value);
        partial void OnThanhBen_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Day_TNP
        {
            get
            {
                return _ThanhBen_Day_TNP;
            }
            set
            {
                OnThanhBen_Day_TNPChanging(value);
                _ThanhBen_Day_TNP = value;
                RaisePropertyChanged("ThanhBen_Day_TNP");
                OnThanhBen_Day_TNPChanged();
            }
        }
        private long _ThanhBen_Day_TNP;
        partial void OnThanhBen_Day_TNPChanging(long value);
        partial void OnThanhBen_Day_TNPChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Day_DobuLieuThap
        {
            get
            {
                return _ThanhBen_Day_DobuLieuThap;
            }
            set
            {
                OnThanhBen_Day_DobuLieuThapChanging(value);
                _ThanhBen_Day_DobuLieuThap = value;
                RaisePropertyChanged("ThanhBen_Day_DobuLieuThap");
                OnThanhBen_Day_DobuLieuThapChanged();
            }
        }
        private long _ThanhBen_Day_DobuLieuThap;
        partial void OnThanhBen_Day_DobuLieuThapChanging(long value);
        partial void OnThanhBen_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Day_DobuLieuCao
        {
            get
            {
                return _ThanhBen_Day_DobuLieuCao;
            }
            set
            {
                OnThanhBen_Day_DobuLieuCaoChanging(value);
                _ThanhBen_Day_DobuLieuCao = value;
                RaisePropertyChanged("ThanhBen_Day_DobuLieuCao");
                OnThanhBen_Day_DobuLieuCaoChanged();
            }
        }
        private long _ThanhBen_Day_DobuLieuCao;
        partial void OnThanhBen_Day_DobuLieuCaoChanging(long value);
        partial void OnThanhBen_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long ThanhBen_Day_KetLuan
        {
            get
            {
                return _ThanhBen_Day_KetLuan;
            }
            set
            {
                OnThanhBen_Day_KetLuanChanging(value);
                _ThanhBen_Day_KetLuan = value;
                RaisePropertyChanged("ThanhBen_Day_KetLuan");
                OnThanhBen_Day_KetLuanChanged();
            }
        }
        private long _ThanhBen_Day_KetLuan;
        partial void OnThanhBen_Day_KetLuanChanging(long value);
        partial void OnThanhBen_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public long TruocVach_Mom_TNP
        {
            get
            {
                return _TruocVach_Mom_TNP;
            }
            set
            {
                OnTruocVach_Mom_TNPChanging(value);
                _TruocVach_Mom_TNP = value;
                RaisePropertyChanged("TruocVach_Mom_TNP");
                OnTruocVach_Mom_TNPChanged();
            }
        }
        private long _TruocVach_Mom_TNP;
        partial void OnTruocVach_Mom_TNPChanging(long value);
        partial void OnTruocVach_Mom_TNPChanged();




        [DataMemberAttribute()]
        public long TruocVach_Mom_DobuLieuThap
        {
            get
            {
                return _TruocVach_Mom_DobuLieuThap;
            }
            set
            {
                OnTruocVach_Mom_DobuLieuThapChanging(value);
                _TruocVach_Mom_DobuLieuThap = value;
                RaisePropertyChanged("TruocVach_Mom_DobuLieuThap");
                OnTruocVach_Mom_DobuLieuThapChanged();
            }
        }
        private long _TruocVach_Mom_DobuLieuThap;
        partial void OnTruocVach_Mom_DobuLieuThapChanging(long value);
        partial void OnTruocVach_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long TruocVach_Mom_DobuLieuCao
        {
            get
            {
                return _TruocVach_Mom_DobuLieuCao;
            }
            set
            {
                OnTruocVach_Mom_DobuLieuCaoChanging(value);
                _TruocVach_Mom_DobuLieuCao = value;
                RaisePropertyChanged("TruocVach_Mom_DobuLieuCao");
                OnTruocVach_Mom_DobuLieuCaoChanged();
            }
        }
        private long _TruocVach_Mom_DobuLieuCao;
        partial void OnTruocVach_Mom_DobuLieuCaoChanging(long value);
        partial void OnTruocVach_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long TruocVach_Mom_KetLuan
        {
            get
            {
                return _TruocVach_Mom_KetLuan;
            }
            set
            {
                OnTruocVach_Mom_KetLuanChanging(value);
                _TruocVach_Mom_KetLuan = value;
                RaisePropertyChanged("TruocVach_Mom_KetLuan");
                OnTruocVach_Mom_KetLuanChanged();
            }
        }
        private long _TruocVach_Mom_KetLuan;
        partial void OnTruocVach_Mom_KetLuanChanging(long value);
        partial void OnTruocVach_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public long TruocVach_Giua_TNP
        {
            get
            {
                return _TruocVach_Giua_TNP;
            }
            set
            {
                OnTruocVach_Giua_TNPChanging(value);
                _TruocVach_Giua_TNP = value;
                RaisePropertyChanged("TruocVach_Giua_TNP");
                OnTruocVach_Giua_TNPChanged();
            }
        }
        private long _TruocVach_Giua_TNP;
        partial void OnTruocVach_Giua_TNPChanging(long value);
        partial void OnTruocVach_Giua_TNPChanged();




        [DataMemberAttribute()]
        public long TruocVach_Giua_DobuLieuThap
        {
            get
            {
                return _TruocVach_Giua_DobuLieuThap;
            }
            set
            {
                OnTruocVach_Giua_DobuLieuThapChanging(value);
                _TruocVach_Giua_DobuLieuThap = value;
                RaisePropertyChanged("TruocVach_Giua_DobuLieuThap");
                OnTruocVach_Giua_DobuLieuThapChanged();
            }
        }
        private long _TruocVach_Giua_DobuLieuThap;
        partial void OnTruocVach_Giua_DobuLieuThapChanging(long value);
        partial void OnTruocVach_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long TruocVach_Giua_DobuLieuCao
        {
            get
            {
                return _TruocVach_Giua_DobuLieuCao;
            }
            set
            {
                OnTruocVach_Giua_DobuLieuCaoChanging(value);
                _TruocVach_Giua_DobuLieuCao = value;
                RaisePropertyChanged("TruocVach_Giua_DobuLieuCao");
                OnTruocVach_Giua_DobuLieuCaoChanged();
            }
        }
        private long _TruocVach_Giua_DobuLieuCao;
        partial void OnTruocVach_Giua_DobuLieuCaoChanging(long value);
        partial void OnTruocVach_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long TruocVach_Giua_KetLuan
        {
            get
            {
                return _TruocVach_Giua_KetLuan;
            }
            set
            {
                OnTruocVach_Giua_KetLuanChanging(value);
                _TruocVach_Giua_KetLuan = value;
                RaisePropertyChanged("TruocVach_Giua_KetLuan");
                OnTruocVach_Giua_KetLuanChanged();
            }
        }
        private long _TruocVach_Giua_KetLuan;
        partial void OnTruocVach_Giua_KetLuanChanging(long value);
        partial void OnTruocVach_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public long TruocVach_Day_TNP
        {
            get
            {
                return _TruocVach_Day_TNP;
            }
            set
            {
                OnTruocVach_Day_TNPChanging(value);
                _TruocVach_Day_TNP = value;
                RaisePropertyChanged("TruocVach_Day_TNP");
                OnTruocVach_Day_TNPChanged();
            }
        }
        private long _TruocVach_Day_TNP;
        partial void OnTruocVach_Day_TNPChanging(long value);
        partial void OnTruocVach_Day_TNPChanged();




        [DataMemberAttribute()]
        public long TruocVach_Day_DobuLieuThap
        {
            get
            {
                return _TruocVach_Day_DobuLieuThap;
            }
            set
            {
                OnTruocVach_Day_DobuLieuThapChanging(value);
                _TruocVach_Day_DobuLieuThap = value;
                RaisePropertyChanged("TruocVach_Day_DobuLieuThap");
                OnTruocVach_Day_DobuLieuThapChanged();
            }
        }
        private long _TruocVach_Day_DobuLieuThap;
        partial void OnTruocVach_Day_DobuLieuThapChanging(long value);
        partial void OnTruocVach_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public long TruocVach_Day_DobuLieuCao
        {
            get
            {
                return _TruocVach_Day_DobuLieuCao;
            }
            set
            {
                OnTruocVach_Day_DobuLieuCaoChanging(value);
                _TruocVach_Day_DobuLieuCao = value;
                RaisePropertyChanged("TruocVach_Day_DobuLieuCao");
                OnTruocVach_Day_DobuLieuCaoChanged();
            }
        }
        private long _TruocVach_Day_DobuLieuCao;
        partial void OnTruocVach_Day_DobuLieuCaoChanging(long value);
        partial void OnTruocVach_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public long TruocVach_Day_KetLuan
        {
            get
            {
                return _TruocVach_Day_KetLuan;
            }
            set
            {
                OnTruocVach_Day_KetLuanChanging(value);
                _TruocVach_Day_KetLuan = value;
                RaisePropertyChanged("TruocVach_Day_KetLuan");
                OnTruocVach_Day_KetLuanChanged();
            }
        }
        private long _TruocVach_Day_KetLuan;
        partial void OnTruocVach_Day_KetLuanChanging(long value);
        partial void OnTruocVach_Day_KetLuanChanged();


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
                    _TDPHayBienChung = (int)_V_ConDauThatNguc;
                    OnV_ConDauThatNgucChanged();
                }
            }
        }
        private AllLookupValues.ChoiceEnum _V_ConDauThatNguc;
        partial void OnV_ConDauThatNgucChanging(AllLookupValues.ChoiceEnum value);
        partial void OnV_ConDauThatNgucChanged();

        #endregion
        #region Lookup Navigation
        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Mom_TNP
        {
            get
            {
                return _V_ThanhTruoc_Mom_TNP;
            }
            set
            {
                OnV_ThanhTruoc_Mom_TNPChanging(value);
                _V_ThanhTruoc_Mom_TNP = value;
                RaisePropertyChanged("V_ThanhTruoc_Mom_TNP");
                OnV_ThanhTruoc_Mom_TNPChanged();
                if (V_ThanhTruoc_Mom_TNP != null)
                {
                    ThanhTruoc_Mom_TNP = V_ThanhTruoc_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Mom_TNP;
        partial void OnV_ThanhTruoc_Mom_TNPChanging(Lookup value);
        partial void OnV_ThanhTruoc_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Mom_DobuLieuThap
        {
            get
            {
                return _V_ThanhTruoc_Mom_DobuLieuThap;
            }
            set
            {
                OnV_ThanhTruoc_Mom_DobuLieuThapChanging(value);
                _V_ThanhTruoc_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhTruoc_Mom_DobuLieuThap");
                OnV_ThanhTruoc_Mom_DobuLieuThapChanged();
                if (V_ThanhTruoc_Mom_DobuLieuThap != null)
                {
                    ThanhTruoc_Mom_DobuLieuThap = V_ThanhTruoc_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Mom_DobuLieuThap;
        partial void OnV_ThanhTruoc_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhTruoc_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Mom_DobuLieuCao
        {
            get
            {
                return _V_ThanhTruoc_Mom_DobuLieuCao;
            }
            set
            {
                OnV_ThanhTruoc_Mom_DobuLieuCaoChanging(value);
                _V_ThanhTruoc_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhTruoc_Mom_DobuLieuCao");
                OnV_ThanhTruoc_Mom_DobuLieuCaoChanged();
                if (V_ThanhTruoc_Mom_DobuLieuCao != null)
                {
                    ThanhTruoc_Mom_DobuLieuCao = V_ThanhTruoc_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Mom_DobuLieuCao;
        partial void OnV_ThanhTruoc_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhTruoc_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Mom_KetLuan
        {
            get
            {
                return _V_ThanhTruoc_Mom_KetLuan;
            }
            set
            {
                OnV_ThanhTruoc_Mom_KetLuanChanging(value);
                _V_ThanhTruoc_Mom_KetLuan = value;
                RaisePropertyChanged("V_ThanhTruoc_Mom_KetLuan");
                OnV_ThanhTruoc_Mom_KetLuanChanged();
                if (V_ThanhTruoc_Mom_KetLuan != null)
                {
                    ThanhTruoc_Mom_KetLuan = V_ThanhTruoc_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Mom_KetLuan;
        partial void OnV_ThanhTruoc_Mom_KetLuanChanging(Lookup value);
        partial void OnV_ThanhTruoc_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Giua_TNP
        {
            get
            {
                return _V_ThanhTruoc_Giua_TNP;
            }
            set
            {
                OnV_ThanhTruoc_Giua_TNPChanging(value);
                _V_ThanhTruoc_Giua_TNP = value;
                RaisePropertyChanged("V_ThanhTruoc_Giua_TNP");
                OnV_ThanhTruoc_Giua_TNPChanged();
                if (V_ThanhTruoc_Giua_TNP != null)
                {
                    ThanhTruoc_Giua_TNP = V_ThanhTruoc_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Giua_TNP;
        partial void OnV_ThanhTruoc_Giua_TNPChanging(Lookup value);
        partial void OnV_ThanhTruoc_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Giua_DobuLieuThap
        {
            get
            {
                return _V_ThanhTruoc_Giua_DobuLieuThap;
            }
            set
            {
                OnV_ThanhTruoc_Giua_DobuLieuThapChanging(value);
                _V_ThanhTruoc_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhTruoc_Giua_DobuLieuThap");
                OnV_ThanhTruoc_Giua_DobuLieuThapChanged();
                if (V_ThanhTruoc_Giua_DobuLieuThap != null)
                {
                    ThanhTruoc_Giua_DobuLieuThap = V_ThanhTruoc_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Giua_DobuLieuThap;
        partial void OnV_ThanhTruoc_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhTruoc_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Giua_DobuLieuCao
        {
            get
            {
                return _V_ThanhTruoc_Giua_DobuLieuCao;
            }
            set
            {
                OnV_ThanhTruoc_Giua_DobuLieuCaoChanging(value);
                _V_ThanhTruoc_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhTruoc_Giua_DobuLieuCao");
                OnV_ThanhTruoc_Giua_DobuLieuCaoChanged();
                if (V_ThanhTruoc_Giua_DobuLieuCao != null)
                {
                    ThanhTruoc_Giua_DobuLieuCao = V_ThanhTruoc_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Giua_DobuLieuCao;
        partial void OnV_ThanhTruoc_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhTruoc_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Giua_KetLuan
        {
            get
            {
                return _V_ThanhTruoc_Giua_KetLuan;
            }
            set
            {
                OnV_ThanhTruoc_Giua_KetLuanChanging(value);
                _V_ThanhTruoc_Giua_KetLuan = value;
                RaisePropertyChanged("V_ThanhTruoc_Giua_KetLuan");
                OnV_ThanhTruoc_Giua_KetLuanChanged();
                if (V_ThanhTruoc_Giua_KetLuan != null)
                {
                    ThanhTruoc_Giua_KetLuan = V_ThanhTruoc_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Giua_KetLuan;
        partial void OnV_ThanhTruoc_Giua_KetLuanChanging(Lookup value);
        partial void OnV_ThanhTruoc_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Day_TNP
        {
            get
            {
                return _V_ThanhTruoc_Day_TNP;
            }
            set
            {
                OnV_ThanhTruoc_Day_TNPChanging(value);
                _V_ThanhTruoc_Day_TNP = value;
                RaisePropertyChanged("V_ThanhTruoc_Day_TNP");
                OnV_ThanhTruoc_Day_TNPChanged();
                if (V_ThanhTruoc_Day_TNP != null)
                {
                    ThanhTruoc_Day_TNP = V_ThanhTruoc_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Day_TNP;
        partial void OnV_ThanhTruoc_Day_TNPChanging(Lookup value);
        partial void OnV_ThanhTruoc_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Day_DobuLieuThap
        {
            get
            {
                return _V_ThanhTruoc_Day_DobuLieuThap;
            }
            set
            {
                OnV_ThanhTruoc_Day_DobuLieuThapChanging(value);
                _V_ThanhTruoc_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhTruoc_Day_DobuLieuThap");
                OnV_ThanhTruoc_Day_DobuLieuThapChanged();
                if (V_ThanhTruoc_Day_DobuLieuThap != null)
                {
                    ThanhTruoc_Day_DobuLieuThap = V_ThanhTruoc_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Day_DobuLieuThap;
        partial void OnV_ThanhTruoc_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhTruoc_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Day_DobuLieuCao
        {
            get
            {
                return _V_ThanhTruoc_Day_DobuLieuCao;
            }
            set
            {
                OnV_ThanhTruoc_Day_DobuLieuCaoChanging(value);
                _V_ThanhTruoc_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhTruoc_Day_DobuLieuCao");
                OnV_ThanhTruoc_Day_DobuLieuCaoChanged();
                if (V_ThanhTruoc_Day_DobuLieuCao != null)
                {
                    ThanhTruoc_Day_DobuLieuCao = V_ThanhTruoc_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Day_DobuLieuCao;
        partial void OnV_ThanhTruoc_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhTruoc_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhTruoc_Day_KetLuan
        {
            get
            {
                return _V_ThanhTruoc_Day_KetLuan;
            }
            set
            {
                OnV_ThanhTruoc_Day_KetLuanChanging(value);
                _V_ThanhTruoc_Day_KetLuan = value;
                RaisePropertyChanged("V_ThanhTruoc_Day_KetLuan");
                OnV_ThanhTruoc_Day_KetLuanChanged();
                if (V_ThanhTruoc_Day_KetLuan != null)
                {
                    ThanhTruoc_Day_KetLuan = V_ThanhTruoc_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhTruoc_Day_KetLuan;
        partial void OnV_ThanhTruoc_Day_KetLuanChanging(Lookup value);
        partial void OnV_ThanhTruoc_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Mom_TNP
        {
            get
            {
                return _V_VanhLienThat_Mom_TNP;
            }
            set
            {
                OnV_VanhLienThat_Mom_TNPChanging(value);
                _V_VanhLienThat_Mom_TNP = value;
                RaisePropertyChanged("V_VanhLienThat_Mom_TNP");
                OnV_VanhLienThat_Mom_TNPChanged();
                if (V_VanhLienThat_Mom_TNP != null)
                {
                    VanhLienThat_Mom_TNP = V_VanhLienThat_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Mom_TNP;
        partial void OnV_VanhLienThat_Mom_TNPChanging(Lookup value);
        partial void OnV_VanhLienThat_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Mom_DobuLieuThap
        {
            get
            {
                return _V_VanhLienThat_Mom_DobuLieuThap;
            }
            set
            {
                OnV_VanhLienThat_Mom_DobuLieuThapChanging(value);
                _V_VanhLienThat_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_VanhLienThat_Mom_DobuLieuThap");
                OnV_VanhLienThat_Mom_DobuLieuThapChanged();
                if (V_VanhLienThat_Mom_DobuLieuThap != null)
                {
                    VanhLienThat_Mom_DobuLieuThap = V_VanhLienThat_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Mom_DobuLieuThap;
        partial void OnV_VanhLienThat_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_VanhLienThat_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Mom_DobuLieuCao
        {
            get
            {
                return _V_VanhLienThat_Mom_DobuLieuCao;
            }
            set
            {
                OnV_VanhLienThat_Mom_DobuLieuCaoChanging(value);
                _V_VanhLienThat_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_VanhLienThat_Mom_DobuLieuCao");
                OnV_VanhLienThat_Mom_DobuLieuCaoChanged();
                if (V_VanhLienThat_Mom_DobuLieuCao != null)
                {
                    VanhLienThat_Mom_DobuLieuCao = V_VanhLienThat_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Mom_DobuLieuCao;
        partial void OnV_VanhLienThat_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_VanhLienThat_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Mom_KetLuan
        {
            get
            {
                return _V_VanhLienThat_Mom_KetLuan;
            }
            set
            {
                OnV_VanhLienThat_Mom_KetLuanChanging(value);
                _V_VanhLienThat_Mom_KetLuan = value;
                RaisePropertyChanged("V_VanhLienThat_Mom_KetLuan");
                OnV_VanhLienThat_Mom_KetLuanChanged();
                if (V_VanhLienThat_Mom_KetLuan != null)
                {
                    VanhLienThat_Mom_KetLuan = V_VanhLienThat_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Mom_KetLuan;
        partial void OnV_VanhLienThat_Mom_KetLuanChanging(Lookup value);
        partial void OnV_VanhLienThat_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Giua_TNP
        {
            get
            {
                return _V_VanhLienThat_Giua_TNP;
            }
            set
            {
                OnV_VanhLienThat_Giua_TNPChanging(value);
                _V_VanhLienThat_Giua_TNP = value;
                RaisePropertyChanged("V_VanhLienThat_Giua_TNP");
                OnV_VanhLienThat_Giua_TNPChanged();
                if (V_VanhLienThat_Giua_TNP != null)
                {
                    VanhLienThat_Giua_TNP = V_VanhLienThat_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Giua_TNP;
        partial void OnV_VanhLienThat_Giua_TNPChanging(Lookup value);
        partial void OnV_VanhLienThat_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Giua_DobuLieuThap
        {
            get
            {
                return _V_VanhLienThat_Giua_DobuLieuThap;
            }
            set
            {
                OnV_VanhLienThat_Giua_DobuLieuThapChanging(value);
                _V_VanhLienThat_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_VanhLienThat_Giua_DobuLieuThap");
                OnV_VanhLienThat_Giua_DobuLieuThapChanged();
                if (V_VanhLienThat_Giua_DobuLieuThap != null)
                {
                    VanhLienThat_Giua_DobuLieuThap = V_VanhLienThat_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Giua_DobuLieuThap;
        partial void OnV_VanhLienThat_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_VanhLienThat_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Giua_DobuLieuCao
        {
            get
            {
                return _V_VanhLienThat_Giua_DobuLieuCao;
            }
            set
            {
                OnV_VanhLienThat_Giua_DobuLieuCaoChanging(value);
                _V_VanhLienThat_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_VanhLienThat_Giua_DobuLieuCao");
                OnV_VanhLienThat_Giua_DobuLieuCaoChanged();
                if (V_VanhLienThat_Giua_DobuLieuCao != null)
                {
                    VanhLienThat_Giua_DobuLieuCao = V_VanhLienThat_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Giua_DobuLieuCao;
        partial void OnV_VanhLienThat_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_VanhLienThat_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Giua_KetLuan
        {
            get
            {
                return _V_VanhLienThat_Giua_KetLuan;
            }
            set
            {
                OnV_VanhLienThat_Giua_KetLuanChanging(value);
                _V_VanhLienThat_Giua_KetLuan = value;
                RaisePropertyChanged("V_VanhLienThat_Giua_KetLuan");
                OnV_VanhLienThat_Giua_KetLuanChanged();
                if (V_VanhLienThat_Giua_KetLuan != null)
                {
                    VanhLienThat_Giua_KetLuan = V_VanhLienThat_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Giua_KetLuan;
        partial void OnV_VanhLienThat_Giua_KetLuanChanging(Lookup value);
        partial void OnV_VanhLienThat_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Day_TNP
        {
            get
            {
                return _V_VanhLienThat_Day_TNP;
            }
            set
            {
                OnV_VanhLienThat_Day_TNPChanging(value);
                _V_VanhLienThat_Day_TNP = value;
                RaisePropertyChanged("V_VanhLienThat_Day_TNP");
                OnV_VanhLienThat_Day_TNPChanged();
                if (V_VanhLienThat_Day_TNP != null)
                {
                    VanhLienThat_Day_TNP = V_VanhLienThat_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Day_TNP;
        partial void OnV_VanhLienThat_Day_TNPChanging(Lookup value);
        partial void OnV_VanhLienThat_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Day_DobuLieuThap
        {
            get
            {
                return _V_VanhLienThat_Day_DobuLieuThap;
            }
            set
            {
                OnV_VanhLienThat_Day_DobuLieuThapChanging(value);
                _V_VanhLienThat_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_VanhLienThat_Day_DobuLieuThap");
                OnV_VanhLienThat_Day_DobuLieuThapChanged();
                if (V_VanhLienThat_Day_DobuLieuThap != null)
                {
                    VanhLienThat_Day_DobuLieuThap = V_VanhLienThat_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Day_DobuLieuThap;
        partial void OnV_VanhLienThat_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_VanhLienThat_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Day_DobuLieuCao
        {
            get
            {
                return _V_VanhLienThat_Day_DobuLieuCao;
            }
            set
            {
                OnV_VanhLienThat_Day_DobuLieuCaoChanging(value);
                _V_VanhLienThat_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_VanhLienThat_Day_DobuLieuCao");
                OnV_VanhLienThat_Day_DobuLieuCaoChanged();
                if (V_VanhLienThat_Day_DobuLieuCao != null)
                {
                    VanhLienThat_Day_DobuLieuCao = V_VanhLienThat_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Day_DobuLieuCao;
        partial void OnV_VanhLienThat_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_VanhLienThat_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_VanhLienThat_Day_KetLuan
        {
            get
            {
                return _V_VanhLienThat_Day_KetLuan;
            }
            set
            {
                OnV_VanhLienThat_Day_KetLuanChanging(value);
                _V_VanhLienThat_Day_KetLuan = value;
                RaisePropertyChanged("V_VanhLienThat_Day_KetLuan");
                OnV_VanhLienThat_Day_KetLuanChanged();
                if (V_VanhLienThat_Day_KetLuan != null)
                {
                    VanhLienThat_Day_KetLuan = V_VanhLienThat_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_VanhLienThat_Day_KetLuan;
        partial void OnV_VanhLienThat_Day_KetLuanChanging(Lookup value);
        partial void OnV_VanhLienThat_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Mom_TNP
        {
            get
            {
                return _V_ThanhDuoi_Mom_TNP;
            }
            set
            {
                OnV_ThanhDuoi_Mom_TNPChanging(value);
                _V_ThanhDuoi_Mom_TNP = value;
                RaisePropertyChanged("V_ThanhDuoi_Mom_TNP");
                OnV_ThanhDuoi_Mom_TNPChanged();
                if (V_ThanhDuoi_Mom_TNP != null)
                {
                    ThanhDuoi_Mom_TNP = V_ThanhDuoi_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Mom_TNP;
        partial void OnV_ThanhDuoi_Mom_TNPChanging(Lookup value);
        partial void OnV_ThanhDuoi_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Mom_DobuLieuThap
        {
            get
            {
                return _V_ThanhDuoi_Mom_DobuLieuThap;
            }
            set
            {
                OnV_ThanhDuoi_Mom_DobuLieuThapChanging(value);
                _V_ThanhDuoi_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhDuoi_Mom_DobuLieuThap");
                OnV_ThanhDuoi_Mom_DobuLieuThapChanged();
                if (V_ThanhDuoi_Mom_DobuLieuThap != null)
                {
                    ThanhDuoi_Mom_DobuLieuThap = V_ThanhDuoi_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Mom_DobuLieuThap;
        partial void OnV_ThanhDuoi_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhDuoi_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Mom_DobuLieuCao
        {
            get
            {
                return _V_ThanhDuoi_Mom_DobuLieuCao;
            }
            set
            {
                OnV_ThanhDuoi_Mom_DobuLieuCaoChanging(value);
                _V_ThanhDuoi_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhDuoi_Mom_DobuLieuCao");
                OnV_ThanhDuoi_Mom_DobuLieuCaoChanged();
                if (V_ThanhDuoi_Mom_DobuLieuCao != null)
                {
                    ThanhDuoi_Mom_DobuLieuCao = V_ThanhDuoi_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Mom_DobuLieuCao;
        partial void OnV_ThanhDuoi_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhDuoi_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Mom_KetLuan
        {
            get
            {
                return _V_ThanhDuoi_Mom_KetLuan;
            }
            set
            {
                OnV_ThanhDuoi_Mom_KetLuanChanging(value);
                _V_ThanhDuoi_Mom_KetLuan = value;
                RaisePropertyChanged("V_ThanhDuoi_Mom_KetLuan");
                OnV_ThanhDuoi_Mom_KetLuanChanged();
                if (V_ThanhDuoi_Mom_KetLuan != null)
                {
                    ThanhDuoi_Mom_KetLuan = V_ThanhDuoi_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Mom_KetLuan;
        partial void OnV_ThanhDuoi_Mom_KetLuanChanging(Lookup value);
        partial void OnV_ThanhDuoi_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Giua_TNP
        {
            get
            {
                return _V_ThanhDuoi_Giua_TNP;
            }
            set
            {
                OnV_ThanhDuoi_Giua_TNPChanging(value);
                _V_ThanhDuoi_Giua_TNP = value;
                RaisePropertyChanged("V_ThanhDuoi_Giua_TNP");
                OnV_ThanhDuoi_Giua_TNPChanged();
                if (V_ThanhDuoi_Giua_TNP != null)
                {
                    ThanhDuoi_Giua_TNP = V_ThanhDuoi_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Giua_TNP;
        partial void OnV_ThanhDuoi_Giua_TNPChanging(Lookup value);
        partial void OnV_ThanhDuoi_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Giua_DobuLieuThap
        {
            get
            {
                return _V_ThanhDuoi_Giua_DobuLieuThap;
            }
            set
            {
                OnV_ThanhDuoi_Giua_DobuLieuThapChanging(value);
                _V_ThanhDuoi_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhDuoi_Giua_DobuLieuThap");
                OnV_ThanhDuoi_Giua_DobuLieuThapChanged();
                if (V_ThanhDuoi_Giua_DobuLieuThap != null)
                {
                    ThanhDuoi_Giua_DobuLieuThap = V_ThanhDuoi_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Giua_DobuLieuThap;
        partial void OnV_ThanhDuoi_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhDuoi_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Giua_DobuLieuCao
        {
            get
            {
                return _V_ThanhDuoi_Giua_DobuLieuCao;
            }
            set
            {
                OnV_ThanhDuoi_Giua_DobuLieuCaoChanging(value);
                _V_ThanhDuoi_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhDuoi_Giua_DobuLieuCao");
                OnV_ThanhDuoi_Giua_DobuLieuCaoChanged();
                if (V_ThanhDuoi_Giua_DobuLieuCao != null)
                {
                    ThanhDuoi_Giua_DobuLieuCao = V_ThanhDuoi_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Giua_DobuLieuCao;
        partial void OnV_ThanhDuoi_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhDuoi_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Giua_KetLuan
        {
            get
            {
                return _V_ThanhDuoi_Giua_KetLuan;
            }
            set
            {
                OnV_ThanhDuoi_Giua_KetLuanChanging(value);
                _V_ThanhDuoi_Giua_KetLuan = value;
                RaisePropertyChanged("V_ThanhDuoi_Giua_KetLuan");
                OnV_ThanhDuoi_Giua_KetLuanChanged();
                if (V_ThanhDuoi_Giua_KetLuan != null)
                {
                    ThanhDuoi_Giua_KetLuan = V_ThanhDuoi_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Giua_KetLuan;
        partial void OnV_ThanhDuoi_Giua_KetLuanChanging(Lookup value);
        partial void OnV_ThanhDuoi_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Day_TNP
        {
            get
            {
                return _V_ThanhDuoi_Day_TNP;
            }
            set
            {
                OnV_ThanhDuoi_Day_TNPChanging(value);
                _V_ThanhDuoi_Day_TNP = value;
                RaisePropertyChanged("V_ThanhDuoi_Day_TNP");
                OnV_ThanhDuoi_Day_TNPChanged();
                if (V_ThanhDuoi_Day_TNP != null)
                {
                    ThanhDuoi_Day_TNP = V_ThanhDuoi_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Day_TNP;
        partial void OnV_ThanhDuoi_Day_TNPChanging(Lookup value);
        partial void OnV_ThanhDuoi_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Day_DobuLieuThap
        {
            get
            {
                return _V_ThanhDuoi_Day_DobuLieuThap;
            }
            set
            {
                OnV_ThanhDuoi_Day_DobuLieuThapChanging(value);
                _V_ThanhDuoi_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhDuoi_Day_DobuLieuThap");
                OnV_ThanhDuoi_Day_DobuLieuThapChanged();
                if (V_ThanhDuoi_Day_DobuLieuThap != null)
                {
                    ThanhDuoi_Day_DobuLieuThap = V_ThanhDuoi_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Day_DobuLieuThap;
        partial void OnV_ThanhDuoi_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhDuoi_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Day_DobuLieuCao
        {
            get
            {
                return _V_ThanhDuoi_Day_DobuLieuCao;
            }
            set
            {
                OnV_ThanhDuoi_Day_DobuLieuCaoChanging(value);
                _V_ThanhDuoi_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhDuoi_Day_DobuLieuCao");
                OnV_ThanhDuoi_Day_DobuLieuCaoChanged();
                if (V_ThanhDuoi_Day_DobuLieuCao != null)
                {
                    ThanhDuoi_Day_DobuLieuCao = V_ThanhDuoi_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Day_DobuLieuCao;
        partial void OnV_ThanhDuoi_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhDuoi_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhDuoi_Day_KetLuan
        {
            get
            {
                return _V_ThanhDuoi_Day_KetLuan;
            }
            set
            {
                OnV_ThanhDuoi_Day_KetLuanChanging(value);
                _V_ThanhDuoi_Day_KetLuan = value;
                RaisePropertyChanged("V_ThanhDuoi_Day_KetLuan");
                OnV_ThanhDuoi_Day_KetLuanChanged();
                if (V_ThanhDuoi_Day_KetLuan != null)
                {
                    ThanhDuoi_Day_KetLuan = V_ThanhDuoi_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhDuoi_Day_KetLuan;
        partial void OnV_ThanhDuoi_Day_KetLuanChanging(Lookup value);
        partial void OnV_ThanhDuoi_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Mom_TNP
        {
            get
            {
                return _V_ThanhSau_Mom_TNP;
            }
            set
            {
                OnV_ThanhSau_Mom_TNPChanging(value);
                _V_ThanhSau_Mom_TNP = value;
                RaisePropertyChanged("V_ThanhSau_Mom_TNP");
                OnV_ThanhSau_Mom_TNPChanged();
                if (V_ThanhSau_Mom_TNP != null)
                {
                    ThanhSau_Mom_TNP = V_ThanhSau_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Mom_TNP;
        partial void OnV_ThanhSau_Mom_TNPChanging(Lookup value);
        partial void OnV_ThanhSau_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Mom_DobuLieuThap
        {
            get
            {
                return _V_ThanhSau_Mom_DobuLieuThap;
            }
            set
            {
                OnV_ThanhSau_Mom_DobuLieuThapChanging(value);
                _V_ThanhSau_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhSau_Mom_DobuLieuThap");
                OnV_ThanhSau_Mom_DobuLieuThapChanged();
                if (V_ThanhSau_Mom_DobuLieuThap != null)
                {
                    ThanhSau_Mom_DobuLieuThap = V_ThanhSau_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Mom_DobuLieuThap;
        partial void OnV_ThanhSau_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhSau_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Mom_DobuLieuCao
        {
            get
            {
                return _V_ThanhSau_Mom_DobuLieuCao;
            }
            set
            {
                OnV_ThanhSau_Mom_DobuLieuCaoChanging(value);
                _V_ThanhSau_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhSau_Mom_DobuLieuCao");
                OnV_ThanhSau_Mom_DobuLieuCaoChanged();
                if (V_ThanhSau_Mom_DobuLieuCao != null)
                {
                    ThanhSau_Mom_DobuLieuCao = V_ThanhSau_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Mom_DobuLieuCao;
        partial void OnV_ThanhSau_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhSau_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Mom_KetLuan
        {
            get
            {
                return _V_ThanhSau_Mom_KetLuan;
            }
            set
            {
                OnV_ThanhSau_Mom_KetLuanChanging(value);
                _V_ThanhSau_Mom_KetLuan = value;
                RaisePropertyChanged("V_ThanhSau_Mom_KetLuan");
                OnV_ThanhSau_Mom_KetLuanChanged();
                if (V_ThanhSau_Mom_KetLuan != null)
                {
                    ThanhSau_Mom_KetLuan = V_ThanhSau_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Mom_KetLuan;
        partial void OnV_ThanhSau_Mom_KetLuanChanging(Lookup value);
        partial void OnV_ThanhSau_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Giua_TNP
        {
            get
            {
                return _V_ThanhSau_Giua_TNP;
            }
            set
            {
                OnV_ThanhSau_Giua_TNPChanging(value);
                _V_ThanhSau_Giua_TNP = value;
                RaisePropertyChanged("V_ThanhSau_Giua_TNP");
                OnV_ThanhSau_Giua_TNPChanged();
                if (V_ThanhSau_Giua_TNP != null)
                {
                    ThanhSau_Giua_TNP = V_ThanhSau_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Giua_TNP;
        partial void OnV_ThanhSau_Giua_TNPChanging(Lookup value);
        partial void OnV_ThanhSau_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Giua_DobuLieuThap
        {
            get
            {
                return _V_ThanhSau_Giua_DobuLieuThap;
            }
            set
            {
                OnV_ThanhSau_Giua_DobuLieuThapChanging(value);
                _V_ThanhSau_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhSau_Giua_DobuLieuThap");
                OnV_ThanhSau_Giua_DobuLieuThapChanged();
                if (V_ThanhSau_Giua_DobuLieuThap != null)
                {
                    ThanhSau_Giua_DobuLieuThap = V_ThanhSau_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Giua_DobuLieuThap;
        partial void OnV_ThanhSau_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhSau_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Giua_DobuLieuCao
        {
            get
            {
                return _V_ThanhSau_Giua_DobuLieuCao;
            }
            set
            {
                OnV_ThanhSau_Giua_DobuLieuCaoChanging(value);
                _V_ThanhSau_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhSau_Giua_DobuLieuCao");
                OnV_ThanhSau_Giua_DobuLieuCaoChanged();
                if (V_ThanhSau_Giua_DobuLieuCao != null)
                {
                    ThanhSau_Giua_DobuLieuCao = V_ThanhSau_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Giua_DobuLieuCao;
        partial void OnV_ThanhSau_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhSau_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Giua_KetLuan
        {
            get
            {
                return _V_ThanhSau_Giua_KetLuan;
            }
            set
            {
                OnV_ThanhSau_Giua_KetLuanChanging(value);
                _V_ThanhSau_Giua_KetLuan = value;
                RaisePropertyChanged("V_ThanhSau_Giua_KetLuan");
                OnV_ThanhSau_Giua_KetLuanChanged();
                if (V_ThanhSau_Giua_KetLuan != null)
                {
                    ThanhSau_Giua_KetLuan = V_ThanhSau_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Giua_KetLuan;
        partial void OnV_ThanhSau_Giua_KetLuanChanging(Lookup value);
        partial void OnV_ThanhSau_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Day_TNP
        {
            get
            {
                return _V_ThanhSau_Day_TNP;
            }
            set
            {
                OnV_ThanhSau_Day_TNPChanging(value);
                _V_ThanhSau_Day_TNP = value;
                RaisePropertyChanged("V_ThanhSau_Day_TNP");
                OnV_ThanhSau_Day_TNPChanged();
                if (V_ThanhSau_Day_TNP != null)
                {
                    ThanhSau_Day_TNP = V_ThanhSau_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Day_TNP;
        partial void OnV_ThanhSau_Day_TNPChanging(Lookup value);
        partial void OnV_ThanhSau_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Day_DobuLieuThap
        {
            get
            {
                return _V_ThanhSau_Day_DobuLieuThap;
            }
            set
            {
                OnV_ThanhSau_Day_DobuLieuThapChanging(value);
                _V_ThanhSau_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhSau_Day_DobuLieuThap");
                OnV_ThanhSau_Day_DobuLieuThapChanged();
                if (V_ThanhSau_Day_DobuLieuThap != null)
                {
                    ThanhSau_Day_DobuLieuThap = V_ThanhSau_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Day_DobuLieuThap;
        partial void OnV_ThanhSau_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhSau_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Day_DobuLieuCao
        {
            get
            {
                return _V_ThanhSau_Day_DobuLieuCao;
            }
            set
            {
                OnV_ThanhSau_Day_DobuLieuCaoChanging(value);
                _V_ThanhSau_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhSau_Day_DobuLieuCao");
                OnV_ThanhSau_Day_DobuLieuCaoChanged();
                if (V_ThanhSau_Day_DobuLieuCao != null)
                {
                    ThanhSau_Day_DobuLieuCao = V_ThanhSau_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Day_DobuLieuCao;
        partial void OnV_ThanhSau_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhSau_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhSau_Day_KetLuan
        {
            get
            {
                return _V_ThanhSau_Day_KetLuan;
            }
            set
            {
                OnV_ThanhSau_Day_KetLuanChanging(value);
                _V_ThanhSau_Day_KetLuan = value;
                RaisePropertyChanged("V_ThanhSau_Day_KetLuan");
                OnV_ThanhSau_Day_KetLuanChanged();
                if (V_ThanhSau_Day_KetLuan != null)
                {
                    ThanhSau_Day_KetLuan = V_ThanhSau_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhSau_Day_KetLuan;
        partial void OnV_ThanhSau_Day_KetLuanChanging(Lookup value);
        partial void OnV_ThanhSau_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Mom_TNP
        {
            get
            {
                return _V_ThanhBen_Mom_TNP;
            }
            set
            {
                OnV_ThanhBen_Mom_TNPChanging(value);
                _V_ThanhBen_Mom_TNP = value;
                RaisePropertyChanged("V_ThanhBen_Mom_TNP");
                OnV_ThanhBen_Mom_TNPChanged();
                if (V_ThanhBen_Mom_TNP != null)
                {
                    ThanhBen_Mom_TNP = V_ThanhBen_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Mom_TNP;
        partial void OnV_ThanhBen_Mom_TNPChanging(Lookup value);
        partial void OnV_ThanhBen_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Mom_DobuLieuThap
        {
            get
            {
                return _V_ThanhBen_Mom_DobuLieuThap;
            }
            set
            {
                OnV_ThanhBen_Mom_DobuLieuThapChanging(value);
                _V_ThanhBen_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhBen_Mom_DobuLieuThap");
                OnV_ThanhBen_Mom_DobuLieuThapChanged();
                if (V_ThanhBen_Mom_DobuLieuThap != null)
                {
                    ThanhBen_Mom_DobuLieuThap = V_ThanhBen_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Mom_DobuLieuThap;
        partial void OnV_ThanhBen_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhBen_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Mom_DobuLieuCao
        {
            get
            {
                return _V_ThanhBen_Mom_DobuLieuCao;
            }
            set
            {
                OnV_ThanhBen_Mom_DobuLieuCaoChanging(value);
                _V_ThanhBen_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhBen_Mom_DobuLieuCao");
                OnV_ThanhBen_Mom_DobuLieuCaoChanged();
                if (V_ThanhBen_Mom_DobuLieuCao != null)
                {
                    ThanhBen_Mom_DobuLieuCao = V_ThanhBen_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Mom_DobuLieuCao;
        partial void OnV_ThanhBen_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhBen_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Mom_KetLuan
        {
            get
            {
                return _V_ThanhBen_Mom_KetLuan;
            }
            set
            {
                OnV_ThanhBen_Mom_KetLuanChanging(value);
                _V_ThanhBen_Mom_KetLuan = value;
                RaisePropertyChanged("V_ThanhBen_Mom_KetLuan");
                OnV_ThanhBen_Mom_KetLuanChanged();
                if (V_ThanhBen_Mom_KetLuan != null)
                {
                    ThanhBen_Mom_KetLuan = V_ThanhBen_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Mom_KetLuan;
        partial void OnV_ThanhBen_Mom_KetLuanChanging(Lookup value);
        partial void OnV_ThanhBen_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Giua_TNP
        {
            get
            {
                return _V_ThanhBen_Giua_TNP;
            }
            set
            {
                OnV_ThanhBen_Giua_TNPChanging(value);
                _V_ThanhBen_Giua_TNP = value;
                RaisePropertyChanged("V_ThanhBen_Giua_TNP");
                OnV_ThanhBen_Giua_TNPChanged();
                if (V_ThanhBen_Giua_TNP != null)
                {
                    ThanhBen_Giua_TNP = V_ThanhBen_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Giua_TNP;
        partial void OnV_ThanhBen_Giua_TNPChanging(Lookup value);
        partial void OnV_ThanhBen_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Giua_DobuLieuThap
        {
            get
            {
                return _V_ThanhBen_Giua_DobuLieuThap;
            }
            set
            {
                OnV_ThanhBen_Giua_DobuLieuThapChanging(value);
                _V_ThanhBen_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhBen_Giua_DobuLieuThap");
                OnV_ThanhBen_Giua_DobuLieuThapChanged();
                if (V_ThanhBen_Giua_DobuLieuThap != null)
                {
                    ThanhBen_Giua_DobuLieuThap = V_ThanhBen_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Giua_DobuLieuThap;
        partial void OnV_ThanhBen_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhBen_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Giua_DobuLieuCao
        {
            get
            {
                return _V_ThanhBen_Giua_DobuLieuCao;
            }
            set
            {
                OnV_ThanhBen_Giua_DobuLieuCaoChanging(value);
                _V_ThanhBen_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhBen_Giua_DobuLieuCao");
                OnV_ThanhBen_Giua_DobuLieuCaoChanged();
                if (V_ThanhBen_Giua_DobuLieuCao != null)
                {
                    ThanhBen_Giua_DobuLieuCao = V_ThanhBen_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Giua_DobuLieuCao;
        partial void OnV_ThanhBen_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhBen_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Giua_KetLuan
        {
            get
            {
                return _V_ThanhBen_Giua_KetLuan;
            }
            set
            {
                OnV_ThanhBen_Giua_KetLuanChanging(value);
                _V_ThanhBen_Giua_KetLuan = value;
                RaisePropertyChanged("V_ThanhBen_Giua_KetLuan");
                OnV_ThanhBen_Giua_KetLuanChanged();
                if (V_ThanhBen_Giua_KetLuan != null)
                {
                    ThanhBen_Giua_KetLuan = V_ThanhBen_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Giua_KetLuan;
        partial void OnV_ThanhBen_Giua_KetLuanChanging(Lookup value);
        partial void OnV_ThanhBen_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Day_TNP
        {
            get
            {
                return _V_ThanhBen_Day_TNP;
            }
            set
            {
                OnV_ThanhBen_Day_TNPChanging(value);
                _V_ThanhBen_Day_TNP = value;
                RaisePropertyChanged("V_ThanhBen_Day_TNP");
                OnV_ThanhBen_Day_TNPChanged();
                if (V_ThanhBen_Day_TNP != null)
                {
                    ThanhBen_Day_TNP = V_ThanhBen_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Day_TNP;
        partial void OnV_ThanhBen_Day_TNPChanging(Lookup value);
        partial void OnV_ThanhBen_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Day_DobuLieuThap
        {
            get
            {
                return _V_ThanhBen_Day_DobuLieuThap;
            }
            set
            {
                OnV_ThanhBen_Day_DobuLieuThapChanging(value);
                _V_ThanhBen_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_ThanhBen_Day_DobuLieuThap");
                OnV_ThanhBen_Day_DobuLieuThapChanged();
                if (V_ThanhBen_Day_DobuLieuThap != null)
                {
                    ThanhBen_Day_DobuLieuThap = V_ThanhBen_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Day_DobuLieuThap;
        partial void OnV_ThanhBen_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_ThanhBen_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Day_DobuLieuCao
        {
            get
            {
                return _V_ThanhBen_Day_DobuLieuCao;
            }
            set
            {
                OnV_ThanhBen_Day_DobuLieuCaoChanging(value);
                _V_ThanhBen_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_ThanhBen_Day_DobuLieuCao");
                OnV_ThanhBen_Day_DobuLieuCaoChanged();
                if (V_ThanhBen_Day_DobuLieuCao != null)
                {
                    ThanhBen_Day_DobuLieuCao = V_ThanhBen_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Day_DobuLieuCao;
        partial void OnV_ThanhBen_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_ThanhBen_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_ThanhBen_Day_KetLuan
        {
            get
            {
                return _V_ThanhBen_Day_KetLuan;
            }
            set
            {
                OnV_ThanhBen_Day_KetLuanChanging(value);
                _V_ThanhBen_Day_KetLuan = value;
                RaisePropertyChanged("V_ThanhBen_Day_KetLuan");
                OnV_ThanhBen_Day_KetLuanChanged();
                if (V_ThanhBen_Day_KetLuan != null)
                {
                    ThanhBen_Day_KetLuan = V_ThanhBen_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_ThanhBen_Day_KetLuan;
        partial void OnV_ThanhBen_Day_KetLuanChanging(Lookup value);
        partial void OnV_ThanhBen_Day_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Mom_TNP
        {
            get
            {
                return _V_TruocVach_Mom_TNP;
            }
            set
            {
                OnV_TruocVach_Mom_TNPChanging(value);
                _V_TruocVach_Mom_TNP = value;
                RaisePropertyChanged("V_TruocVach_Mom_TNP");
                OnV_TruocVach_Mom_TNPChanged();
                if (V_TruocVach_Mom_TNP != null)
                {
                    TruocVach_Mom_TNP = V_TruocVach_Mom_TNP.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Mom_TNP;
        partial void OnV_TruocVach_Mom_TNPChanging(Lookup value);
        partial void OnV_TruocVach_Mom_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Mom_DobuLieuThap
        {
            get
            {
                return _V_TruocVach_Mom_DobuLieuThap;
            }
            set
            {
                OnV_TruocVach_Mom_DobuLieuThapChanging(value);
                _V_TruocVach_Mom_DobuLieuThap = value;
                RaisePropertyChanged("V_TruocVach_Mom_DobuLieuThap");
                OnV_TruocVach_Mom_DobuLieuThapChanged();
                if (V_TruocVach_Mom_DobuLieuThap != null)
                {
                    TruocVach_Mom_DobuLieuThap = V_TruocVach_Mom_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Mom_DobuLieuThap;
        partial void OnV_TruocVach_Mom_DobuLieuThapChanging(Lookup value);
        partial void OnV_TruocVach_Mom_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Mom_DobuLieuCao
        {
            get
            {
                return _V_TruocVach_Mom_DobuLieuCao;
            }
            set
            {
                OnV_TruocVach_Mom_DobuLieuCaoChanging(value);
                _V_TruocVach_Mom_DobuLieuCao = value;
                RaisePropertyChanged("V_TruocVach_Mom_DobuLieuCao");
                OnV_TruocVach_Mom_DobuLieuCaoChanged();
                if (V_TruocVach_Mom_DobuLieuCao != null)
                {
                    TruocVach_Mom_DobuLieuCao = V_TruocVach_Mom_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Mom_DobuLieuCao;
        partial void OnV_TruocVach_Mom_DobuLieuCaoChanging(Lookup value);
        partial void OnV_TruocVach_Mom_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Mom_KetLuan
        {
            get
            {
                return _V_TruocVach_Mom_KetLuan;
            }
            set
            {
                OnV_TruocVach_Mom_KetLuanChanging(value);
                _V_TruocVach_Mom_KetLuan = value;
                RaisePropertyChanged("V_TruocVach_Mom_KetLuan");
                OnV_TruocVach_Mom_KetLuanChanged();
                if (V_TruocVach_Mom_KetLuan != null)
                {
                    TruocVach_Mom_KetLuan = V_TruocVach_Mom_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Mom_KetLuan;
        partial void OnV_TruocVach_Mom_KetLuanChanging(Lookup value);
        partial void OnV_TruocVach_Mom_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Giua_TNP
        {
            get
            {
                return _V_TruocVach_Giua_TNP;
            }
            set
            {
                OnV_TruocVach_Giua_TNPChanging(value);
                _V_TruocVach_Giua_TNP = value;
                RaisePropertyChanged("V_TruocVach_Giua_TNP");
                OnV_TruocVach_Giua_TNPChanged();
                if (V_TruocVach_Giua_TNP != null)
                {
                    TruocVach_Giua_TNP = V_TruocVach_Giua_TNP.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Giua_TNP;
        partial void OnV_TruocVach_Giua_TNPChanging(Lookup value);
        partial void OnV_TruocVach_Giua_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Giua_DobuLieuThap
        {
            get
            {
                return _V_TruocVach_Giua_DobuLieuThap;
            }
            set
            {
                OnV_TruocVach_Giua_DobuLieuThapChanging(value);
                _V_TruocVach_Giua_DobuLieuThap = value;
                RaisePropertyChanged("V_TruocVach_Giua_DobuLieuThap");
                OnV_TruocVach_Giua_DobuLieuThapChanged();
                if (V_TruocVach_Giua_DobuLieuThap != null)
                {
                    TruocVach_Giua_DobuLieuThap = V_TruocVach_Giua_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Giua_DobuLieuThap;
        partial void OnV_TruocVach_Giua_DobuLieuThapChanging(Lookup value);
        partial void OnV_TruocVach_Giua_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Giua_DobuLieuCao
        {
            get
            {
                return _V_TruocVach_Giua_DobuLieuCao;
            }
            set
            {
                OnV_TruocVach_Giua_DobuLieuCaoChanging(value);
                _V_TruocVach_Giua_DobuLieuCao = value;
                RaisePropertyChanged("V_TruocVach_Giua_DobuLieuCao");
                OnV_TruocVach_Giua_DobuLieuCaoChanged();
                if (V_TruocVach_Giua_DobuLieuCao != null)
                {
                    TruocVach_Giua_DobuLieuCao = V_TruocVach_Giua_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Giua_DobuLieuCao;
        partial void OnV_TruocVach_Giua_DobuLieuCaoChanging(Lookup value);
        partial void OnV_TruocVach_Giua_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Giua_KetLuan
        {
            get
            {
                return _V_TruocVach_Giua_KetLuan;
            }
            set
            {
                OnV_TruocVach_Giua_KetLuanChanging(value);
                _V_TruocVach_Giua_KetLuan = value;
                RaisePropertyChanged("V_TruocVach_Giua_KetLuan");
                OnV_TruocVach_Giua_KetLuanChanged();
                if (V_TruocVach_Giua_KetLuan != null)
                {
                    TruocVach_Giua_KetLuan = V_TruocVach_Giua_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Giua_KetLuan;
        partial void OnV_TruocVach_Giua_KetLuanChanging(Lookup value);
        partial void OnV_TruocVach_Giua_KetLuanChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Day_TNP
        {
            get
            {
                return _V_TruocVach_Day_TNP;
            }
            set
            {
                OnV_TruocVach_Day_TNPChanging(value);
                _V_TruocVach_Day_TNP = value;
                RaisePropertyChanged("V_TruocVach_Day_TNP");
                OnV_TruocVach_Day_TNPChanged();
                if (V_TruocVach_Day_TNP != null)
                {
                    TruocVach_Day_TNP = V_TruocVach_Day_TNP.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Day_TNP;
        partial void OnV_TruocVach_Day_TNPChanging(Lookup value);
        partial void OnV_TruocVach_Day_TNPChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Day_DobuLieuThap
        {
            get
            {
                return _V_TruocVach_Day_DobuLieuThap;
            }
            set
            {
                OnV_TruocVach_Day_DobuLieuThapChanging(value);
                _V_TruocVach_Day_DobuLieuThap = value;
                RaisePropertyChanged("V_TruocVach_Day_DobuLieuThap");
                OnV_TruocVach_Day_DobuLieuThapChanged();
                if (V_TruocVach_Day_DobuLieuThap != null)
                {
                    TruocVach_Day_DobuLieuThap = V_TruocVach_Day_DobuLieuThap.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Day_DobuLieuThap;
        partial void OnV_TruocVach_Day_DobuLieuThapChanging(Lookup value);
        partial void OnV_TruocVach_Day_DobuLieuThapChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Day_DobuLieuCao
        {
            get
            {
                return _V_TruocVach_Day_DobuLieuCao;
            }
            set
            {
                OnV_TruocVach_Day_DobuLieuCaoChanging(value);
                _V_TruocVach_Day_DobuLieuCao = value;
                RaisePropertyChanged("V_TruocVach_Day_DobuLieuCao");
                OnV_TruocVach_Day_DobuLieuCaoChanged();
                if (V_TruocVach_Day_DobuLieuCao != null)
                {
                    TruocVach_Day_DobuLieuCao = V_TruocVach_Day_DobuLieuCao.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Day_DobuLieuCao;
        partial void OnV_TruocVach_Day_DobuLieuCaoChanging(Lookup value);
        partial void OnV_TruocVach_Day_DobuLieuCaoChanged();




        [DataMemberAttribute()]
        public Lookup V_TruocVach_Day_KetLuan
        {
            get
            {
                return _V_TruocVach_Day_KetLuan;
            }
            set
            {
                OnV_TruocVach_Day_KetLuanChanging(value);
                _V_TruocVach_Day_KetLuan = value;
                RaisePropertyChanged("V_TruocVach_Day_KetLuan");
                OnV_TruocVach_Day_KetLuanChanged();
                if (V_TruocVach_Day_KetLuan != null)
                {
                    TruocVach_Day_KetLuan = V_TruocVach_Day_KetLuan.LookupID;
                }
            }
        }
        private Lookup _V_TruocVach_Day_KetLuan;
        partial void OnV_TruocVach_Day_KetLuanChanging(Lookup value);
        partial void OnV_TruocVach_Day_KetLuanChanged();


        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
