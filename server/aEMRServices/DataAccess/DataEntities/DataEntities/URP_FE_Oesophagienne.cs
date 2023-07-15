using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class URP_FE_Oesophagienne : NotifyChangedBase, IEditableObject
    {
        public URP_FE_Oesophagienne()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            URP_FE_Oesophagienne info = obj as URP_FE_Oesophagienne;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.URP_FE_OesophagienneID > 0 && this.URP_FE_OesophagienneID == info.URP_FE_OesophagienneID;
        }
        private URP_FE_Oesophagienne _tempURP_FE_Oesophagienne;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempURP_FE_Oesophagienne = (URP_FE_Oesophagienne)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempURP_FE_Oesophagienne)
                CopyFrom(_tempURP_FE_Oesophagienne);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(URP_FE_Oesophagienne p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new URP_FE_Oesophagienne object.

        /// <param name="URP_FE_OesophagienneID">Initial value of the URP_FE_OesophagienneID property.</param>
        /// <param name="URP_FE_OesophagienneName">Initial value of the URP_FE_OesophagienneName property.</param>
        public static URP_FE_Oesophagienne CreateURP_FE_Oesophagienne(Byte URP_FE_OesophagienneID, String URP_FE_OesophagienneName)
        {
            URP_FE_Oesophagienne URP_FE_Oesophagienne = new URP_FE_Oesophagienne();
            URP_FE_Oesophagienne.URP_FE_OesophagienneID = URP_FE_OesophagienneID;

            return URP_FE_Oesophagienne;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long URP_FE_OesophagienneID
        {
            get
            {
                return _URP_FE_OesophagienneID;
            }
            set
            {
                if (_URP_FE_OesophagienneID != value)
                {
                    OnURP_FE_OesophagienneIDChanging(value);
                    _URP_FE_OesophagienneID = value;
                    RaisePropertyChanged("URP_FE_OesophagienneID");
                    OnURP_FE_OesophagienneIDChanged();
                }
            }
        }
        private long _URP_FE_OesophagienneID;
        partial void OnURP_FE_OesophagienneIDChanging(long value);
        partial void OnURP_FE_OesophagienneIDChanged();

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
        private DateTime _CreateDate=DateTime.Now;
        partial void OnCreateDateChanging(DateTime value);
        partial void OnCreateDateChanged();

        [DataMemberAttribute()]
        private string _ChiDinh;
        public string ChiDinh
        {
            get
            {
                return _ChiDinh;
            }
            set
            {
                if (_ChiDinh == value)
                    return;
                _ChiDinh = value;
                RaisePropertyChanged("ChiDinh");
            }
        }

        [DataMemberAttribute()]
        private string _ChanDoanThanhNguc;
        public string ChanDoanThanhNguc
        {
            get
            {
                return _ChanDoanThanhNguc;
            }
            set
            {
                if (_ChanDoanThanhNguc == value)
                    return;
                _ChanDoanThanhNguc = value;
                RaisePropertyChanged("ChanDoanThanhNguc");
            }
        }
       
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
                return this.URP_FE_OesophagienneID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
