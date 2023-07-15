using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_VasculaireExam : NotifyChangedBase, IEditableObject
    {
        public URP_FE_VasculaireExam()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_VasculaireExam info = obj as URP_FE_VasculaireExam;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_VasculaireExamID > 0 && this.URP_FE_VasculaireExamID == info.URP_FE_VasculaireExamID;
        }
        private URP_FE_VasculaireExam _tempURP_FE_VasculaireExam;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_VasculaireExam = (URP_FE_VasculaireExam)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_VasculaireExam)
                CopyFrom(_tempURP_FE_VasculaireExam);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_VasculaireExam p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_VasculaireExam object.

        /// <param name="URP_FE_VasculaireExamID">Initial value of the URP_FE_VasculaireExamID property.</param>
        /// <param name="URP_FE_VasculaireExamName">Initial value of the URP_FE_VasculaireExamName property.</param>
        public static URP_FE_VasculaireExam CreateURP_FE_VasculaireExam(Byte URP_FE_VasculaireExamID, String URP_FE_VasculaireExamName)
        {
            URP_FE_VasculaireExam URP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            URP_FE_VasculaireExam.URP_FE_VasculaireExamID = URP_FE_VasculaireExamID;
            
            return URP_FE_VasculaireExam;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_VasculaireExamID
        {
            get
            {
                return _URP_FE_VasculaireExamID;
            }
            set
            {
                if (_URP_FE_VasculaireExamID != value)
                {
                    OnURP_FE_VasculaireExamIDChanging(value);
                    _URP_FE_VasculaireExamID = value;
                    RaisePropertyChanged("URP_FE_VasculaireExamID");
                    OnURP_FE_VasculaireExamIDChanged();
                }
            }
        }
        private long _URP_FE_VasculaireExamID;
        partial void OnURP_FE_VasculaireExamIDChanging(long value);
        partial void OnURP_FE_VasculaireExamIDChanged();

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
        public bool NoiLap
        {
            get
            {
                return _NoiLap;
            }
            set
            {
                OnNoiLapChanging(value);
                _NoiLap = value;
                RaisePropertyChanged("NoiLap");
                OnNoiLapChanged();
            }
        }
        private bool _NoiLap;
        partial void OnNoiLapChanging(bool value);
        partial void OnNoiLapChanged();




        [DataMemberAttribute()]
        public bool ChongMat
        {
            get
            {
                return _ChongMat;
            }
            set
            {
                OnChongMatChanging(value);
                _ChongMat = value;
                RaisePropertyChanged("ChongMat");
                OnChongMatChanged();
            }
        }
        private bool _ChongMat;
        partial void OnChongMatChanging(bool value);
        partial void OnChongMatChanged();




        [DataMemberAttribute()]
        public bool DotQuy
        {
            get
            {
                return _DotQuy;
            }
            set
            {
                OnDotQuyChanging(value);
                _DotQuy = value;
                RaisePropertyChanged("DotQuy");
                OnDotQuyChanged();
            }
        }
        private bool _DotQuy;
        partial void OnDotQuyChanging(bool value);
        partial void OnDotQuyChanged();




        [DataMemberAttribute()]
        public bool GiamTriNho
        {
            get
            {
                return _GiamTriNho;
            }
            set
            {
                OnGiamTriNhoChanging(value);
                _GiamTriNho = value;
                RaisePropertyChanged("GiamTriNho");
                OnGiamTriNhoChanged();
            }
        }
        private bool _GiamTriNho;
        partial void OnGiamTriNhoChanging(bool value);
        partial void OnGiamTriNhoChanged();




        [DataMemberAttribute()]
        public bool ThoangMu
        {
            get
            {
                return _ThoangMu;
            }
            set
            {
                OnThoangMuChanging(value);
                _ThoangMu = value;
                RaisePropertyChanged("ThoangMu");
                OnThoangMuChanged();
            }
        }
        private bool _ThoangMu;
        partial void OnThoangMuChanging(bool value);
        partial void OnThoangMuChanged();




        [DataMemberAttribute()]
        public bool NhinMo
        {
            get
            {
                return _NhinMo;
            }
            set
            {
                OnNhinMoChanging(value);
                _NhinMo = value;
                RaisePropertyChanged("NhinMo");
                OnNhinMoChanged();
            }
        }
        private bool _NhinMo;
        partial void OnNhinMoChanging(bool value);
        partial void OnNhinMoChanged();




        [DataMemberAttribute()]
        public bool LietNuaNguoi
        {
            get
            {
                return _LietNuaNguoi;
            }
            set
            {
                OnLietNuaNguoiChanging(value);
                _LietNuaNguoi = value;
                RaisePropertyChanged("LietNuaNguoi");
                OnLietNuaNguoiChanged();
            }
        }
        private bool _LietNuaNguoi;
        partial void OnLietNuaNguoiChanging(bool value);
        partial void OnLietNuaNguoiChanged();




        [DataMemberAttribute()]
        public bool TeYeuChanTay
        {
            get
            {
                return _TeYeuChanTay;
            }
            set
            {
                OnTeYeuChanTayChanging(value);
                _TeYeuChanTay = value;
                RaisePropertyChanged("TeYeuChanTay");
                OnTeYeuChanTayChanged();
            }
        }
        private bool _TeYeuChanTay;
        partial void OnTeYeuChanTayChanging(bool value);
        partial void OnTeYeuChanTayChanged();




        [DataMemberAttribute()]
        public bool DaPThuatDMC
        {
            get
            {
                return _DaPThuatDMC;
            }
            set
            {
                OnDaPThuatDMCChanging(value);
                _DaPThuatDMC = value;
                RaisePropertyChanged("DaPThuatDMC");
                OnDaPThuatDMCChanged();
            }
        }
        private bool _DaPThuatDMC;
        partial void OnDaPThuatDMCChanging(bool value);
        partial void OnDaPThuatDMCChanged();



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
                return this.URP_FE_VasculaireExamID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
