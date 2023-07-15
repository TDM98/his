using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class StaffStoreDeptResponsibilities : NotifyChangedBase, IEditableObject
    {
        public StaffStoreDeptResponsibilities()
            : base()
        {

        }

        private StaffStoreDeptResponsibilities _tempStaffStoreDeptResponsibilities;
        public override bool Equals(object obj)
        {
            StaffStoreDeptResponsibilities info = obj as StaffStoreDeptResponsibilities;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffStoreDeptResponsibilitiesID > 0 && this.StaffStoreDeptResponsibilitiesID == info.StaffStoreDeptResponsibilitiesID;
        }

        public bool CheckExist(object obj)
        {
            StaffStoreDeptResponsibilities info = obj as StaffStoreDeptResponsibilities;
            if (info == null)
                return false;
            if (this.StoreID == info.StoreID
                && this.StaffID == info.StaffID)
                //&& this.Responsibilities_32 == info.Responsibilities_32)
            {
                return true;
            }
            return false;
        }
        public bool CheckEquals(object obj)
        {
            StaffStoreDeptResponsibilities info = obj as StaffStoreDeptResponsibilities;
            if (info == null)
                return false;
            if (this.StoreID == info.StoreID
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
            _tempStaffStoreDeptResponsibilities = (StaffStoreDeptResponsibilities)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempStaffStoreDeptResponsibilities)
                CopyFrom(_tempStaffStoreDeptResponsibilities);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(StaffStoreDeptResponsibilities p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new StaffStoreDeptResponsibilities object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static StaffStoreDeptResponsibilities CreateStaffStoreDeptResponsibilities(String bedLocNumber, long allocationID)
        {
            StaffStoreDeptResponsibilities StaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
            //StaffStoreDeptResponsibilities.BedLocNumber = bedLocNumber;
            //StaffStoreDeptResponsibilities.AllocationID = allocationID;
            return StaffStoreDeptResponsibilities;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long StaffStoreDeptResponsibilitiesID
        {
            get
            {
                return _StaffStoreDeptResponsibilitiesID;
            }
            set
            {
                OnStaffStoreDeptResponsibilitiesIDChanging(value);
                _StaffStoreDeptResponsibilitiesID = value;
                RaisePropertyChanged("StaffStoreDeptResponsibilitiesID");
                OnStaffStoreDeptResponsibilitiesIDChanged();
            }
        }
        private long _StaffStoreDeptResponsibilitiesID;
        partial void OnStaffStoreDeptResponsibilitiesIDChanging(long value);
        partial void OnStaffStoreDeptResponsibilitiesIDChanged();

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
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private long _StoreID;
        partial void OnStoreIDChanging(long value);
        partial void OnStoreIDChanged();

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
        public string StoreName
        {
            get
            {
                return _StoreName;
            }
            set
            {
                OnStoreNameChanging(value);
                _StoreName = value;
                RaisePropertyChanged("StoreName");
                OnStoreNameChanged();
            }
        }
        private string _StoreName;
        partial void OnStoreNameChanging(string value);
        partial void OnStoreNameChanged();

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
        public RefStorageWarehouseLocation RefStorageWarehouseLocation
        {
            get
            {
                return _RefStorageWarehouseLocation;
            }
            set
            {
                OnRefStorageWarehouseLocationChanging(value);
                _RefStorageWarehouseLocation = value;
                RaisePropertyChanged("RefStorageWarehouseLocation");
                OnRefStorageWarehouseLocationChanged();
            }
        }
        private RefStorageWarehouseLocation _RefStorageWarehouseLocation;
        partial void OnRefStorageWarehouseLocationChanging(RefStorageWarehouseLocation value);
        partial void OnRefStorageWarehouseLocationChanged();
        
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
    public partial class StaffDeptPresence : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long StaffDeptPresenceID
        {
            get
            {
                return _StaffDeptPresenceID;
            }
            set
            {
                _StaffDeptPresenceID = value;
                RaisePropertyChanged("StaffDeptPresenceID");
            }
        }
        private long _StaffDeptPresenceID;

        [DataMemberAttribute()]
        public DateTime StaffCountDate
        {
            get
            {
                return _StaffCountDate;
            }
            set
            {
                _StaffCountDate = value;
                RaisePropertyChanged("StaffCountDate");
            }
        }
        private DateTime _StaffCountDate = DateTime.Now;

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                _DeptID = value;
                RaisePropertyChanged("DeptID");
            }
        }
        private long _DeptID;

        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [DataMemberAttribute()]
        public Int16 NumberOfDoctorRequired
        {
            get
            {
                return _NumberOfDoctorRequired;
            }
            set
            {
                _NumberOfDoctorRequired = value;
                RaisePropertyChanged("NumberOfDoctorRequired");
            }
        }
        private Int16 _NumberOfDoctorRequired;

        [DataMemberAttribute()]
        public Int16 NumberOfNurseRequired
        {
            get
            {
                return _NumberOfNurseRequired;
            }
            set
            {
                _NumberOfNurseRequired = value;
                RaisePropertyChanged("NumberOfNurseRequired");
            }
        }
        private Int16 _NumberOfNurseRequired;

        [DataMemberAttribute()]
        public Int16 NumberOfTechnicianRequired
        {
            get
            {
                return _NumberOfTechnicianRequired;
            }
            set
            {
                _NumberOfTechnicianRequired = value;
                RaisePropertyChanged("NumberOfTechnicianRequired");
            }
        }
        private Int16 _NumberOfTechnicianRequired;

        [DataMemberAttribute()]
        public Int16 NumberOfClinicalAsstRequired
        {
            get
            {
                return _NumberOfClinicalAsstRequired;
            }
            set
            {
                _NumberOfClinicalAsstRequired = value;
                RaisePropertyChanged("NumberOfClinicalAsstRequired");
            }
        }
        private Int16 _NumberOfClinicalAsstRequired;

        [DataMemberAttribute()]
        public Int16 NumberOfAsstNurseRequired
        {
            get
            {
                return _NumberOfAsstNurseRequired;
            }
            set
            {
                _NumberOfAsstNurseRequired = value;
                RaisePropertyChanged("NumberOfAsstNurseRequired");
            }
        }
        private Int16 _NumberOfAsstNurseRequired;

        [DataMemberAttribute()]
        public Int16 NumberOfDoctorPresent
        {
            get
            {
                return _NumberOfDoctorPresent;
            }
            set
            {
                _NumberOfDoctorPresent = value;
                RaisePropertyChanged("NumberOfDoctorPresent");
            }
        }
        private Int16 _NumberOfDoctorPresent;

        [DataMemberAttribute()]
        public Int16 NumberOfNursePresent
        {
            get
            {
                return _NumberOfNursePresent;
            }
            set
            {
                _NumberOfNursePresent = value;
                RaisePropertyChanged("NumberOfNursePresent");
            }
        }
        private Int16 _NumberOfNursePresent;

        [DataMemberAttribute()]
        public Int16 NumberOfTechnicianPresent
        {
            get
            {
                return _NumberOfTechnicianPresent;
            }
            set
            {
                _NumberOfTechnicianPresent = value;
                RaisePropertyChanged("NumberOfTechnicianPresent");
            }
        }
        private Int16 _NumberOfTechnicianPresent;

        [DataMemberAttribute()]
        public Int16 NumberOfClinicalAsstPresent
        {
            get
            {
                return _NumberOfClinicalAsstPresent;
            }
            set
            {
                _NumberOfClinicalAsstPresent = value;
                RaisePropertyChanged("NumberOfClinicalAsstPresent");
            }
        }
        private Int16 _NumberOfClinicalAsstPresent;

        [DataMemberAttribute()]
        public Int16 NumberOfAsstNursePresent
        {
            get
            {
                return _NumberOfAsstNursePresent;
            }
            set
            {
                _NumberOfAsstNursePresent = value;
                RaisePropertyChanged("NumberOfAsstNursePresent");
            }
        }
        private Int16 _NumberOfAsstNursePresent;

        [DataMemberAttribute()]
        public Int16 NumberOfPatientPresent
        {
            get
            {
                return _NumberOfPatientPresent;
            }
            set
            {
                _NumberOfPatientPresent = value;
                RaisePropertyChanged("NumberOfPatientPresent");
            }
        }
        private Int16 _NumberOfPatientPresent;

        [DataMemberAttribute()]
        public Int16 NumberOfCurrentPatient
        {
            get
            {
                return _NumberOfCurrentPatient;
            }
            set
            {
                _NumberOfCurrentPatient = value;
                RaisePropertyChanged("NumberOfCurrentPatient");
            }
        }
        private Int16 _NumberOfCurrentPatient;

        [DataMemberAttribute()]
        public Int16 NumberOfPatientTxfrHospital
        {
            get
            {
                return _NumberOfPatientTxfrHospital;
            }
            set
            {
                _NumberOfPatientTxfrHospital = value;
                RaisePropertyChanged("NumberOfPatientTxfrHospital");
            }
        }
        private Int16 _NumberOfPatientTxfrHospital;

        [DataMemberAttribute()]
        public Int16 NumberOfPatientTxfrDept
        {
            get
            {
                return _NumberOfPatientTxfrDept;
            }
            set
            {
                _NumberOfPatientTxfrDept = value;
                RaisePropertyChanged("NumberOfPatientTxfrDept");
            }
        }
        private Int16 _NumberOfPatientTxfrDept;
         
        [DataMemberAttribute()]
        public Int16 NumberOfPatientDeceased
        {
            get
            {
                return _NumberOfPatientDeceased;
            }
            set
            {
                _NumberOfPatientDeceased = value;
                RaisePropertyChanged("NumberOfPatientDeceased");
            }
        }
        private Int16 _NumberOfPatientDeceased;

        [DataMemberAttribute()]
        public Int16 NumberOfPatientDischarged
        {
            get
            {
                return _NumberOfPatientDischarged;
            }
            set
            {
                _NumberOfPatientDischarged = value;
                RaisePropertyChanged("NumberOfPatientDischarged");
            }
        }
        private Int16 _NumberOfPatientDischarged;

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

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        private DateTime _RecCreatedDate;
    }
}
