using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDobutamineExam : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDobutamineExam()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDobutamineExam info = obj as URP_FE_StressDobutamineExam;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDobutamineExamID > 0 && this.URP_FE_StressDobutamineExamID == info.URP_FE_StressDobutamineExamID;
        }
        private URP_FE_StressDobutamineExam _tempURP_FE_StressDobutamineExam;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDobutamineExam = (URP_FE_StressDobutamineExam)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDobutamineExam)
                CopyFrom(_tempURP_FE_StressDobutamineExam);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDobutamineExam p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDobutamineExam object.

        /// <param name="URP_FE_StressDobutamineExamID">Initial value of the URP_FE_StressDobutamineExamID property.</param>
        /// <param name="URP_FE_StressDobutamineExamName">Initial value of the URP_FE_StressDobutamineExamName property.</param>
        public static URP_FE_StressDobutamineExam CreateURP_FE_StressDobutamineExam(Byte URP_FE_StressDobutamineExamID, String URP_FE_StressDobutamineExamName)
        {
            URP_FE_StressDobutamineExam URP_FE_StressDobutamineExam = new URP_FE_StressDobutamineExam();
            URP_FE_StressDobutamineExam.URP_FE_StressDobutamineExamID = URP_FE_StressDobutamineExamID;
            
            return URP_FE_StressDobutamineExam;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDobutamineExamID
        {
            get
            {
                return _URP_FE_StressDobutamineExamID;
            }
            set
            {
                if (_URP_FE_StressDobutamineExamID != value)
                {
                    OnURP_FE_StressDobutamineExamIDChanging(value);
                    _URP_FE_StressDobutamineExamID = value;
                    RaisePropertyChanged("URP_FE_StressDobutamineExamID");
                    OnURP_FE_StressDobutamineExamIDChanged();
                }
            }
        }
        private long _URP_FE_StressDobutamineExamID;
        partial void OnURP_FE_StressDobutamineExamIDChanging(long value);
        partial void OnURP_FE_StressDobutamineExamIDChanged();

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
        public bool ChiDinhSATGSDobu
        {
            get
            {
                return _ChiDinhSATGSDobu;
            }
            set
            {
                OnChiDinhSATGSDobuChanging(value);
                _ChiDinhSATGSDobu = value;
                RaisePropertyChanged("ChiDinhSATGSDobu");
                OnChiDinhSATGSDobuChanged();
            }
        }
        private bool _ChiDinhSATGSDobu;
        partial void OnChiDinhSATGSDobuChanging(bool value);
        partial void OnChiDinhSATGSDobuChanged();




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
        public string TDDTrongNgaySATGSDobu
        {
            get
            {
                return _TDDTrongNgaySATGSDobu;
            }
            set
            {
                OnTDDTrongNgaySATGSDobuChanging(value);
                _TDDTrongNgaySATGSDobu = value;
                RaisePropertyChanged("TDDTrongNgaySATGSDobu");
                OnTDDTrongNgaySATGSDobuChanged();
            }
        }
        private string _TDDTrongNgaySATGSDobu;
        partial void OnTDDTrongNgaySATGSDobuChanging(string value);
        partial void OnTDDTrongNgaySATGSDobuChanged();


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
