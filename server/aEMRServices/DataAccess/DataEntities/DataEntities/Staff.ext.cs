using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-06
 * Contents: patient extend
/*******************************************************************/
#endregion

namespace DataEntities
{
    public partial class Staff
    {
        public string FullNameString
        {
            get
            {
                string st = string.Empty;
                st += this.SLastName;
                if (this.SMiddleName != null && this.SMiddleName.Length > 0)
                    st += " " + this.SMiddleName;
                st += " " + this.SFirstName;
                return st;
            }
            set
            {
            }
        }
        public string IDAndFullName
        {
            get
            {
                string st = string.Empty;
                st += this.StaffID.ToString();
                if (this.RefStaffCategory != null && this.RefStaffCategory.StaffCatgDescription != null && this.RefStaffCategory.StaffCatgDescription.Length > 0)
                {
                    st += "_" + this.RefStaffCategory.StaffCatgDescription + " ";
                }
                else
                {
                    st += "_";
                }
                if (this.FullName != null && this.FullName.Length > 0)
                {
                    st += this.FullName;
                }
                return st;
            }
            set
            {
            }
        }

        public string TypeAndFullName
        {
            get
            {
                string st = string.Empty;
                if (this.RefStaffCategory != null && this.RefStaffCategory.StaffCatgDescription != null && this.RefStaffCategory.StaffCatgDescription.Length > 0)
                {
                    st += this.RefStaffCategory.StaffCatgDescription;
                }
                if (st.Length > 0)
                {
                    st += " ";
                }
                if (this.FullName != null && this.FullName.Length > 0)
                {
                    st += this.FullName;
                }
                return st;
            }
            set
            {
            }
        }
        public string PhoneNumberString
        {
            get
            {
                string st = string.Empty;
                if (SPhoneNumber != null && SPhoneNumber.Length > 0)
                    st += SPhoneNumber.Trim();
                if (SMobiPhoneNumber != null && SMobiPhoneNumber.Length > 0)
                {
                    if (st != null && st.Length > 0)
                        st += "-" + SMobiPhoneNumber.Trim();
                    else
                        st += SMobiPhoneNumber.Trim();
                }
                return st;
            }
            set
            {
            }

        }
    }

    public partial class StaffPosition : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Reward object.

        /// <param name="rDID">Initial value of the RDID property.</param>
        public static StaffPosition CreateStaffPosition(long staffPositionID)
        {
            StaffPosition reward = new StaffPosition();
            reward.StaffPositionID = staffPositionID;
            return reward;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 StaffPositionID
        {
            get
            {
                return _StaffPositionID;
            }
            set
            {
                if (_StaffPositionID != value)
                {
                    _StaffPositionID = value;
                    RaisePropertyChanged("StaffPositionID");
                }
            }
        }
        private Int64 _StaffPositionID;

        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public int PositionRefID
        {
            get
            {
                return _PositionRefID;
            }
            set
            {
                if (_PositionRefID != value)
                {
                    _PositionRefID = value;
                    RaisePropertyChanged("PositionRefID");
                }
            }
        }
        private int _PositionRefID;

        [DataMemberAttribute()]
        public String PrefixDes
        {
            get
            {
                return _PrefixDes;
            }
            set
            {
                _PrefixDes = value;
                RaisePropertyChanged("PrefixDes");
            }
        }
        private String _PrefixDes;

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private String _FullName;

        [DataMemberAttribute()]
        public String PositionName
        {
            get
            {
                return _PositionName;
            }
            set
            {
                _PositionName = value;
                RaisePropertyChanged("PositionName");
            }
        }
        private String _PositionName;

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        private bool _IsActive;

        public string FullNameString
        {
            get
            {
                return this.PrefixDes + this.FullName;
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            StaffPosition seletedStore = obj as StaffPosition;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffPositionID == seletedStore.StaffPositionID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
