using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDipyridamoleExam : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDipyridamoleExam()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDipyridamoleExam info = obj as URP_FE_StressDipyridamoleExam;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDipyridamoleExamID > 0 && this.URP_FE_StressDipyridamoleExamID == info.URP_FE_StressDipyridamoleExamID;
        }
        private URP_FE_StressDipyridamoleExam _tempURP_FE_StressDipyridamoleExam;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDipyridamoleExam = (URP_FE_StressDipyridamoleExam)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDipyridamoleExam)
                CopyFrom(_tempURP_FE_StressDipyridamoleExam);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDipyridamoleExam p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDipyridamoleExam object.

        /// <param name="URP_FE_StressDipyridamoleExamID">Initial value of the URP_FE_StressDipyridamoleExamID property.</param>
        /// <param name="URP_FE_StressDipyridamoleExamName">Initial value of the URP_FE_StressDipyridamoleExamName property.</param>
        public static URP_FE_StressDipyridamoleExam CreateURP_FE_StressDipyridamoleExam(Byte URP_FE_StressDipyridamoleExamID, String URP_FE_StressDipyridamoleExamName)
        {
            URP_FE_StressDipyridamoleExam URP_FE_StressDipyridamoleExam = new URP_FE_StressDipyridamoleExam();
            URP_FE_StressDipyridamoleExam.URP_FE_StressDipyridamoleExamID = URP_FE_StressDipyridamoleExamID;
            
            return URP_FE_StressDipyridamoleExam;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDipyridamoleExamID
        {
            get
            {
                return _URP_FE_StressDipyridamoleExamID;
            }
            set
            {
                if (_URP_FE_StressDipyridamoleExamID != value)
                {
                    OnURP_FE_StressDipyridamoleExamIDChanging(value);
                    _URP_FE_StressDipyridamoleExamID = value;
                    RaisePropertyChanged("URP_FE_StressDipyridamoleExamID");
                    OnURP_FE_StressDipyridamoleExamIDChanged();
                }
            }
        }
        private long _URP_FE_StressDipyridamoleExamID;
        partial void OnURP_FE_StressDipyridamoleExamIDChanging(long value);
        partial void OnURP_FE_StressDipyridamoleExamIDChanged();

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
        public string TrieuChungHienTai
        {
            get
            {
                return _TrieuChungHienTai;
            }
            set
            {
                OnTrieuChungHienTaiChanging(value);
                _TrieuChungHienTai = value;
                RaisePropertyChanged("TrieuChungHienTai");
                OnTrieuChungHienTaiChanged();
            }
        }
        private string _TrieuChungHienTai;
        partial void OnTrieuChungHienTaiChanging(string value);
        partial void OnTrieuChungHienTaiChanged();




        [DataMemberAttribute()]
        public bool ChiDinhSATGSDipy
        {
            get
            {
                return _ChiDinhSATGSDipy;
            }
            set
            {
                OnChiDinhSATGSDipyChanging(value);
                _ChiDinhSATGSDipy = value;
                RaisePropertyChanged("ChiDinhSATGSDipy");
                OnChiDinhSATGSDipyChanged();
            }
        }
        private bool _ChiDinhSATGSDipy;
        partial void OnChiDinhSATGSDipyChanging(bool value);
        partial void OnChiDinhSATGSDipyChanged();




        [DataMemberAttribute()]
        public string ChiDinhDetail
        {
            get
            {
                return _ChiDinhDetail;
            }
            set
            {
                OnChiDinhDetailChanging(value);
                _ChiDinhDetail = value;
                RaisePropertyChanged("ChiDinhDetail");
                OnChiDinhDetailChanged();
            }
        }
        private string _ChiDinhDetail;
        partial void OnChiDinhDetailChanging(string value);
        partial void OnChiDinhDetailChanged();


        [DataMemberAttribute()]
        public string TDDTruocNgayKham
        {
            get
            {
                return _TDDTruocNgayKham;
            }
            set
            {
                OnTDDTruocNgayKhamChanging(value);
                _TDDTruocNgayKham = value;
                RaisePropertyChanged("TDDTruocNgayKham");
                OnTDDTruocNgayKhamChanged();
            }
        }
        private string _TDDTruocNgayKham;
        partial void OnTDDTruocNgayKhamChanging(string value);
        partial void OnTDDTruocNgayKhamChanged();




        [DataMemberAttribute()]
        public string TDDTrongNgaySATGSDipy
        {
            get
            {
                return _TDDTrongNgaySATGSDipy;
            }
            set
            {
                OnTDDTrongNgaySATGSDipyChanging(value);
                _TDDTrongNgaySATGSDipy = value;
                RaisePropertyChanged("TDDTrongNgaySATGSDipy");
                OnTDDTrongNgaySATGSDipyChanged();
            }
        }
        private string _TDDTrongNgaySATGSDipy;
        partial void OnTDDTrongNgaySATGSDipyChanging(string value);
        partial void OnTDDTrongNgaySATGSDipyChanged();


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
