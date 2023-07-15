using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class StaffDeptResponsibilities : NotifyChangedBase, IEditableObject
    {
        public StaffDeptResponsibilities()
            : base()
        {

        }

        private StaffDeptResponsibilities _tempStaffDeptResponsibilities;
        public override bool Equals(object obj)
        {
            StaffDeptResponsibilities info = obj as StaffDeptResponsibilities;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffDeptResponsibilitiesID > 0 && this.StaffDeptResponsibilitiesID == info.StaffDeptResponsibilitiesID;
        }

        public bool CheckExist(object obj)
        {
            StaffDeptResponsibilities info = obj as StaffDeptResponsibilities;
            if (info == null)
                return false;
            if(this.DeptID==info.DeptID
                && this.StaffID == info.StaffID)
                //&& this.Responsibilities_32 == info.Responsibilities_32)
            {
                return true;
            }
            return false;
        }
        public bool CheckEquals(object obj)
        {
            StaffDeptResponsibilities info = obj as StaffDeptResponsibilities;
            if (info == null)
                return false;
            if (this.DeptID == info.DeptID
                && this.StaffID == info.StaffID
                && this.Responsibilities_32 == info.Responsibilities_32)
            {
                return true;
            }
            return false;
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempStaffDeptResponsibilities = (StaffDeptResponsibilities)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempStaffDeptResponsibilities)
                CopyFrom(_tempStaffDeptResponsibilities);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(StaffDeptResponsibilities p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new StaffDeptResponsibilities object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static StaffDeptResponsibilities CreateStaffDeptResponsibilities(String bedLocNumber, long allocationID)
        {
            StaffDeptResponsibilities StaffDeptResponsibilities = new StaffDeptResponsibilities();
            //StaffDeptResponsibilities.BedLocNumber = bedLocNumber;
            //StaffDeptResponsibilities.AllocationID = allocationID;
            return StaffDeptResponsibilities;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long StaffDeptResponsibilitiesID
        {
            get
            {
                return _StaffDeptResponsibilitiesID;
            }
            set
            {
                OnStaffDeptResponsibilitiesIDChanging(value);
                _StaffDeptResponsibilitiesID = value;
                RaisePropertyChanged("StaffDeptResponsibilitiesID");
                OnStaffDeptResponsibilitiesIDChanged();
            }
        }
        private long _StaffDeptResponsibilitiesID;
        partial void OnStaffDeptResponsibilitiesIDChanging(long value);
        partial void OnStaffDeptResponsibilitiesIDChanged();

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public int Responsibilities_32
        {
            get
            {
                return _Responsibilities_32;
            }
            set
            {
                OnResponsibilities_32Changing(value);
                _Responsibilities_32 = value;
                RaisePropertyChanged("Responsibilities_32");
                OnResponsibilities_32Changed();
            }
        }
        private int _Responsibilities_32;
        partial void OnResponsibilities_32Changing(int value);
        partial void OnResponsibilities_32Changed();

        [DataMemberAttribute()]
        public Nullable<DateTime> RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _RecCreatedDate;
        partial void OnRecCreatedDateChanging(Nullable<DateTime> value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();

        [DataMemberAttribute()]
        public bool ResNhapVien
        {
            get
            {
                return _ResNhapVien;
            }
            set
            {
                OnResNhapVienChanging(value);
                _ResNhapVien = value;
                RaisePropertyChanged("ResNhapVien");
                OnResNhapVienChanged();
            }
        }
        private bool _ResNhapVien;
        partial void OnResNhapVienChanging(bool value);
        partial void OnResNhapVienChanged();

        
         [DataMemberAttribute()]
        public bool ResDatGiuong
        {
            get
            {
                return _ResDatGiuong;
            }
            set
            {
                OnResDatGiuongChanging(value);
                _ResDatGiuong = value;
                RaisePropertyChanged("ResDatGiuong");
                OnResDatGiuongChanged();
            }
        }
        private bool _ResDatGiuong;
        partial void OnResDatGiuongChanging(bool value);
        partial void OnResDatGiuongChanged();
            
        [DataMemberAttribute()]
        public bool ResXuatVien 
        {
            get
            {
                return _ResXuatVien ;
            }
            set
            {
                OnResXuatVienChanging(value);
                _ResXuatVien  = value;
                RaisePropertyChanged("ResXuatVien");
                OnResXuatVienChanged();
            }
        }
        private bool _ResXuatVien ;
        partial void OnResXuatVienChanging(bool value);
        partial void OnResXuatVienChanged();
            
        [DataMemberAttribute()]
        public bool ResTraGiuong 
        {
            get
            {
                return _ResTraGiuong;
            }
            set
            {
                OnResTraGiuongChanging(value);
                _ResTraGiuong = value;
                RaisePropertyChanged("ResTraGiuong");
                OnResTraGiuongChanged();
            }
        }
        private bool _ResTraGiuong;
        partial void OnResTraGiuongChanging(bool value);
        partial void OnResTraGiuongChanged();
            
        [DataMemberAttribute()]
        public bool ResKhoNoiTru
        {
            get
            {
                return _ResKhoNoiTru;
            }
            set
            {
                OnResKhoNoiTruChanging(value);
                _ResKhoNoiTru = value;
                RaisePropertyChanged("ResKhoNoiTru");
                OnResKhoNoiTruChanged();
            }
        }
        private bool _ResKhoNoiTru;
        partial void OnResKhoNoiTruChanging(bool value);
        partial void OnResKhoNoiTruChanged();

        #endregion

        #region Navigation Properties
        
        
       [DataMemberAttribute()]
       public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                OnStaffChanging(value);
                _Staff = value;
                if (_Staff != null)
                {
                    StaffID = Staff.StaffID;
                }
                RaisePropertyChanged("Staff");
                OnStaffChanged();
            }
        }
       private Staff _Staff;
       partial void OnStaffChanging(Staff value);
       partial void OnStaffChanged();


       [DataMemberAttribute()]
       public RefDepartment RefDepartment
       {
           get
           {
               return _RefDepartment;
           }
           set
           {
               OnRefDepartmentChanging(value);
               _RefDepartment = value;
               if (_RefDepartment != null)
               {
                   DeptID = RefDepartment.DeptID;
               }
               RaisePropertyChanged("RefDepartment");
               OnRefDepartmentChanged();
           }
       }
       private RefDepartment _RefDepartment;
       partial void OnRefDepartmentChanging(RefDepartment value);
       partial void OnRefDepartmentChanged();

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public enum StaffResponsibility
        {
            ResNhapVien = 0x001,
            ResDatGiuong = 0x002,
            ResXuatVien = 0x004,
            ResTraGiuong = 0x008,
            ResKhoNoiTru = 0x010,
        }
        public int GetTotalValue()
        {
            int temp = 0;
            if (ResNhapVien)
                temp = temp | (int)StaffResponsibility.ResNhapVien;
            if (ResDatGiuong)
                temp = temp | (int)StaffResponsibility.ResDatGiuong;
            if (ResXuatVien)
                temp = temp | (int)StaffResponsibility.ResXuatVien;
            if (ResTraGiuong)
                temp = temp | (int)StaffResponsibility.ResTraGiuong;
            if (ResKhoNoiTru)
                temp = temp | (int)StaffResponsibility.ResKhoNoiTru;
            
            return temp;
        }
        public void CheckValue(int temp)
        {
            ResNhapVien = (temp & (int)StaffResponsibility.ResNhapVien) == (int)StaffResponsibility.ResNhapVien ? true : false;
            ResDatGiuong = (temp & (int)StaffResponsibility.ResDatGiuong) == (int)StaffResponsibility.ResDatGiuong ? true : false;
            ResXuatVien = (temp & (int)StaffResponsibility.ResXuatVien) == (int)StaffResponsibility.ResXuatVien ? true : false;
            ResTraGiuong = (temp & (int)StaffResponsibility.ResTraGiuong) == (int)StaffResponsibility.ResTraGiuong ? true : false;
            ResKhoNoiTru = (temp & (int)StaffResponsibility.ResKhoNoiTru) == (int)StaffResponsibility.ResKhoNoiTru ? true : false;
        }
    }
}
