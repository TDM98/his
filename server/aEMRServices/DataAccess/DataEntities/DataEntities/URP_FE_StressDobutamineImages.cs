using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_StressDobutamineImages : NotifyChangedBase, IEditableObject
    {
        public URP_FE_StressDobutamineImages()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_StressDobutamineImages info = obj as URP_FE_StressDobutamineImages;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_StressDobutamineImagesID > 0 && this.URP_FE_StressDobutamineImagesID == info.URP_FE_StressDobutamineImagesID;
        }
        private URP_FE_StressDobutamineImages _tempURP_FE_StressDobutamineImages;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_StressDobutamineImages = (URP_FE_StressDobutamineImages)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_StressDobutamineImages)
                CopyFrom(_tempURP_FE_StressDobutamineImages);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_StressDobutamineImages p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_StressDobutamineImages object.

        /// <param name="URP_FE_StressDobutamineImagesID">Initial value of the URP_FE_StressDobutamineImagesID property.</param>
        /// <param name="URP_FE_StressDobutamineImagesName">Initial value of the URP_FE_StressDobutamineImagesName property.</param>
        public static URP_FE_StressDobutamineImages CreateURP_FE_StressDobutamineImages(Byte URP_FE_StressDobutamineImagesID, String URP_FE_StressDobutamineImagesName)
        {
            URP_FE_StressDobutamineImages URP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();
            URP_FE_StressDobutamineImages.URP_FE_StressDobutamineImagesID = URP_FE_StressDobutamineImagesID;
            
            return URP_FE_StressDobutamineImages;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_StressDobutamineImagesID
        {
            get
            {
                return _URP_FE_StressDobutamineImagesID;
            }
            set
            {
                if (_URP_FE_StressDobutamineImagesID != value)
                {
                    OnURP_FE_StressDobutamineImagesIDChanging(value);
                    _URP_FE_StressDobutamineImagesID = value;
                    RaisePropertyChanged("URP_FE_StressDobutamineImagesID");
                    OnURP_FE_StressDobutamineImagesIDChanged();
                }
            }
        }
        private long _URP_FE_StressDobutamineImagesID;
        partial void OnURP_FE_StressDobutamineImagesIDChanging(long value);
        partial void OnURP_FE_StressDobutamineImagesIDChanged();

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
        //==== 20161129 CMN Begin: Add button save for all pages
        [DataMemberAttribute()]
        public bool Tab_Update_Required
        {
            get
            {
                return this.URP_FE_StressDobutamineImagesID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
