using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDipyridamoleImage : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDipyridamoleImage()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDipyridamoleImage info = obj as URP_FE_StressDipyridamoleImage;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDipyridamoleImageID > 0 && this.URP_FE_StressDipyridamoleImageID == info.URP_FE_StressDipyridamoleImageID;
        }
        private URP_FE_StressDipyridamoleImage _tempURP_FE_StressDipyridamoleImage;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDipyridamoleImage = (URP_FE_StressDipyridamoleImage)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDipyridamoleImage)
                CopyFrom(_tempURP_FE_StressDipyridamoleImage);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDipyridamoleImage p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDipyridamoleImage object.

        /// <param name="URP_FE_StressDipyridamoleImageID">Initial value of the URP_FE_StressDipyridamoleImageID property.</param>
        /// <param name="URP_FE_StressDipyridamoleImageName">Initial value of the URP_FE_StressDipyridamoleImageName property.</param>
        public static URP_FE_StressDipyridamoleImage CreateURP_FE_StressDipyridamoleImage(Byte URP_FE_StressDipyridamoleImageID, String URP_FE_StressDipyridamoleImageName)
        {
            URP_FE_StressDipyridamoleImage URP_FE_StressDipyridamoleImage = new URP_FE_StressDipyridamoleImage();
            URP_FE_StressDipyridamoleImage.URP_FE_StressDipyridamoleImageID = URP_FE_StressDipyridamoleImageID;
            
            return URP_FE_StressDipyridamoleImage;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDipyridamoleImageID
        {
            get
            {
                return _URP_FE_StressDipyridamoleImageID;
            }
            set
            {
                if (_URP_FE_StressDipyridamoleImageID != value)
                {
                    OnURP_FE_StressDipyridamoleImageIDChanging(value);
                    _URP_FE_StressDipyridamoleImageID = value;
                    RaisePropertyChanged("URP_FE_StressDipyridamoleImageID");
                    OnURP_FE_StressDipyridamoleImageIDChanged();
                }
            }
        }
        private long _URP_FE_StressDipyridamoleImageID;
        partial void OnURP_FE_StressDipyridamoleImageIDChanging(long value);
        partial void OnURP_FE_StressDipyridamoleImageIDChanged();

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
        public string KetLuan
        {
            get
            {
                return _KetLuan;
            }
            set
            {
                if (_KetLuan != value)
                {
                    OnKetLuanChanging(value);
                    _KetLuan = value;
                    RaisePropertyChanged("KetLuan");
                    OnKetLuanChanged();
                }
            }
        }
        private string _KetLuan;
        partial void OnKetLuanChanging(string value);
        partial void OnKetLuanChanged();

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
