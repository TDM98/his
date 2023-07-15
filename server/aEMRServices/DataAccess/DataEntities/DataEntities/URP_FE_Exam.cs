using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_Exam : NotifyChangedBase, IEditableObject
    {
        public URP_FE_Exam()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_Exam info = obj as URP_FE_Exam;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_ExamID > 0 && this.URP_FE_ExamID == info.URP_FE_ExamID;
        }
        private URP_FE_Exam _tempURP_FE_Exam;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_Exam = (URP_FE_Exam)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_Exam)
                CopyFrom(_tempURP_FE_Exam);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_Exam p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_Exam object.

        /// <param name="URP_FE_ExamID">Initial value of the URP_FE_ExamID property.</param>
        /// <param name="URP_FE_ExamName">Initial value of the URP_FE_ExamName property.</param>
        public static URP_FE_Exam CreateURP_FE_Exam(Byte URP_FE_ExamID, String URP_FE_ExamName)
        {
            URP_FE_Exam URP_FE_Exam = new URP_FE_Exam();
            URP_FE_Exam.URP_FE_ExamID = URP_FE_ExamID;
            
            return URP_FE_Exam;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_ExamID
        {
            get
            {
                return _URP_FE_ExamID;
            }
            set
            {
                if (_URP_FE_ExamID != value)
                {
                    OnURP_FE_ExamIDChanging(value);
                    _URP_FE_ExamID = value;
                    RaisePropertyChanged("URP_FE_ExamID");
                    OnURP_FE_ExamIDChanged();
                }
            }
        }
        private long _URP_FE_ExamID;
        partial void OnURP_FE_ExamIDChanging(long value);
        partial void OnURP_FE_ExamIDChanged();

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
        public bool CaoHuyetAp
        {
            get
            {
                return _CaoHuyetAp;
            }
            set
            {
                if (_CaoHuyetAp != value)
                {
                    OnCaoHuyetApChanging(value);
                    _CaoHuyetAp = value;
                    RaisePropertyChanged("CaoHuyetAp");
                    OnCaoHuyetApChanged();
                }
            }
        }
        private bool _CaoHuyetAp;
        partial void OnCaoHuyetApChanging(bool value);
        partial void OnCaoHuyetApChanged();
    
        [DataMemberAttribute()]
        public string CaoHuyetApDetail
        {
            get
            {
                return _CaoHuyetApDetail;
            }
            set
            {
                if (_CaoHuyetApDetail != value)
                {
                    OnCaoHuyetApDetailChanging(value);
                    _CaoHuyetApDetail = value;
                    RaisePropertyChanged("CaoHuyetApDetail");
                    OnCaoHuyetApDetailChanged();
                }
            }
        }
        private string _CaoHuyetApDetail;
        partial void OnCaoHuyetApDetailChanging(string value);
        partial void OnCaoHuyetApDetailChanged();
    
        [DataMemberAttribute()]
        public string Cholesterol
        {
            get
            {
                return _Cholesterol;
            }
            set
            {
                if (_Cholesterol != value)
                {
                    OnCholesterolChanging(value);
                    _Cholesterol = value;
                    RaisePropertyChanged("Cholesterol");
                    OnCholesterolChanged();
                }
            }
        }
        private string _Cholesterol;
        partial void OnCholesterolChanging(string value);
        partial void OnCholesterolChanged();
    
       [DataMemberAttribute()]
        public double Triglyceride
        {
            get
            {
                return _Triglyceride;
            }
            set
            {
                if (_Triglyceride != value)
                {
                    OnTriglycerideChanging(value);
                    _Triglyceride = value;
                    RaisePropertyChanged("Triglyceride");
                    OnTriglycerideChanged();
                }
            }
        }
        private double _Triglyceride;
        partial void OnTriglycerideChanging(double value);
        partial void OnTriglycerideChanged();
   
       [DataMemberAttribute()]
        public double HDL
        {
            get
            {
                return _HDL;
            }
            set
            {
                if (_HDL != value)
                {
                    OnHDLChanging(value);
                    _HDL = value;
                    RaisePropertyChanged("HDL");
                    OnHDLChanged();
                }
            }
        }
        private double _HDL;
        partial void OnHDLChanging(double value);
        partial void OnHDLChanged();
   
       [DataMemberAttribute()]
        public double LDL
        {
            get
            {
                return _LDL;
            }
            set
            {
                if (_LDL != value)
                {
                    OnLDLChanging(value);
                    _LDL = value;
                    RaisePropertyChanged("LDL");
                    OnLDLChanged();
                }
            }
        }
        private double _LDL;
        partial void OnLDLChanging(double value);
        partial void OnLDLChanged();
    
       [DataMemberAttribute()]
        public bool TieuDuong
        {
            get
            {
                return _TieuDuong;
            }
            set
            {
                if (_TieuDuong != value)
                {
                    OnTieuDuongChanging(value);
                    _TieuDuong = value;
                    RaisePropertyChanged("TieuDuong");
                    OnTieuDuongChanged();
                }
            }
        }
        private bool _TieuDuong;
        partial void OnTieuDuongChanging(bool value);
        partial void OnTieuDuongChanged();
    
        [DataMemberAttribute()]
        public string TieuDuongDetail
        {
            get
            {
                return _TieuDuongDetail;
            }
            set
            {
                if (_TieuDuongDetail != value)
                {
                    OnTieuDuongDetailChanging(value);
                    _TieuDuongDetail = value;
                    RaisePropertyChanged("TieuDuongDetail");
                    OnTieuDuongDetailChanged();
                }
            }
        }
        private string _TieuDuongDetail;
        partial void OnTieuDuongDetailChanging(string value);
        partial void OnTieuDuongDetailChanged();
   
       [DataMemberAttribute()]
        public bool ThuocLa
        {
            get
            {
                return _ThuocLa;
            }
            set
            {
                if (_ThuocLa != value)
                {
                    OnThuocLaChanging(value);
                    _ThuocLa = value;
                    RaisePropertyChanged("ThuocLa");
                    OnThuocLaChanged();
                }
            }
        }
        private bool _ThuocLa;
        partial void OnThuocLaChanging(bool value);
        partial void OnThuocLaChanged();
    
        [DataMemberAttribute()]
        public string Detail
        {
            get
            {
                return _Detail;
            }
            set
            {
                if (_Detail != value)
                {
                    OnDetailChanging(value);
                    _Detail = value;
                    RaisePropertyChanged("Detail");
                    OnDetailChanged();
                }
            }
        }
        private string _Detail;
        partial void OnDetailChanging(string value);
        partial void OnDetailChanged();
    
       [DataMemberAttribute()]
        public bool ThuocNguaThai
        {
            get
            {
                return _ThuocNguaThai;
            }
            set
            {
                if (_ThuocNguaThai != value)
                {
                    OnThuocNguaThaiChanging(value);
                    _ThuocNguaThai = value;
                    RaisePropertyChanged("ThuocNguaThai");
                    OnThuocNguaThaiChanged();
                }
            }
        }
        private bool _ThuocNguaThai;
        partial void OnThuocNguaThaiChanging( bool value);
        partial void OnThuocNguaThaiChanged();
    
        [DataMemberAttribute()]
        public string ThuocNguaThaiDetail
        {
            get
            {
                return _ThuocNguaThaiDetail;
            }
            set
            {
                if (_ThuocNguaThaiDetail != value)
                {
                    OnThuocNguaThaiDetailChanging(value);
                    _ThuocNguaThaiDetail = value;
                    RaisePropertyChanged("ThuocNguaThaiDetail");
                    OnThuocNguaThaiDetailChanged();
                }
            }
        }
        private string _ThuocNguaThaiDetail;
        partial void OnThuocNguaThaiDetailChanging(string value);
        partial void OnThuocNguaThaiDetailChanged();
   
   
        [DataMemberAttribute()]
        public string NhanApMP
        {
            get
            {
                return _NhanApMP;
            }
            set
            {
                if (_NhanApMP != value)
                {
                    OnNhanApMPChanging(value);
                    _NhanApMP = value;
                    RaisePropertyChanged("NhanApMP");
                    OnNhanApMPChanged();
                }
            }
        }
        private string _NhanApMP;
        partial void OnNhanApMPChanging(string value);
        partial void OnNhanApMPChanged();
    
        [DataMemberAttribute()]
        public string NhanApMT
        {
            get
            {
                return _NhanApMT;
            }
            set
            {
                if (_NhanApMT != value)
                {
                    OnNhanApMTChanging(value);
                    _NhanApMT = value;
                    RaisePropertyChanged("NhanApMT");
                    OnNhanApMTChanged();
                }
            }
        }
        private string _NhanApMT;
        partial void OnNhanApMTChanging(string value);
        partial void OnNhanApMTChanged();

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
