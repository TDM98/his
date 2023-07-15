using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RefStorageWarehouseLocation : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefStorageWarehouseLocation object.

        /// <param name="storeID">Initial value of the StoreID property.</param>
        /// <param name="swhlName">Initial value of the swhlName property.</param>
        /// <param name="swhlActive">Initial value of the swhlActive property.</param>
        public static RefStorageWarehouseLocation CreateRefStorageWarehouseLocation(long storeID, String swhlName, Boolean swhlActive)
        {
            RefStorageWarehouseLocation refStorageWarehouseLocation = new RefStorageWarehouseLocation();
            refStorageWarehouseLocation.StoreID = storeID;
            refStorageWarehouseLocation.swhlName = swhlName;
            refStorageWarehouseLocation.swhlActive = swhlActive;
            return refStorageWarehouseLocation;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                if (_StoreID != value)
                {
                    OnStoreIDChanging(value);
                    _StoreID = value;
                    RaisePropertyChanged("StoreID");
                    OnStoreIDChanged();
                }
            }
        }
        private long _StoreID;
        partial void OnStoreIDChanging(long value);
        partial void OnStoreIDChanged();

        [Required(ErrorMessage = "Vui lòng chọn loại kho!")]
        [DataMemberAttribute()]
        public long? StoreTypeID
        {
            get
            {
                return _StoreTypeID;
            }
            set
            {
                if (_StoreTypeID != value)
                {
                    ValidateProperty("StoreTypeID", value);
                    _StoreTypeID = value;
                    RaisePropertyChanged("StoreTypeID");
                }
            }
        }
        private long? _StoreTypeID;

        [DataMemberAttribute()]
        public long? DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID != value)
                {
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                }
            }
        }
        private long? _DeptID;

        [Required(ErrorMessage = "Vui lòng nhập tên kho!")]
        [DataMemberAttribute()]
        public String swhlName
        {
            get
            {
                return _swhlName;
            }
            set
            {
                OnswhlNameChanging(value);
                ValidateProperty("swhlName", value);
                _swhlName = value;
                RaisePropertyChanged("swhlName");
                OnswhlNameChanged();
            }
        }
        private String _swhlName;
        partial void OnswhlNameChanging(String value);
        partial void OnswhlNameChanged();

        [DataMemberAttribute()]
        public String swhlNotes
        {
            get
            {
                return _swhlNotes;
            }
            set
            {
                OnswhlNotesChanging(value);
                _swhlNotes = value;
                RaisePropertyChanged("swhlNotes");
                OnswhlNotesChanged();
            }
        }
        private String _swhlNotes;
        partial void OnswhlNotesChanging(String value);
        partial void OnswhlNotesChanged();

        [DataMemberAttribute()]
        public Boolean swhlActive
        {
            get
            {
                return _swhlActive;
            }
            set
            {
                OnswhlActiveChanging(value);
                _swhlActive = value;
                RaisePropertyChanged("swhlActive");
                OnswhlActiveChanged();
            }
        }
        private Boolean _swhlActive;
        partial void OnswhlActiveChanging(Boolean value);
        partial void OnswhlActiveChanged();

        [DataMemberAttribute()]
        public Boolean IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }
        private Boolean _IsMain;

        [DataMemberAttribute()]
        public bool IsMedicineStore
        {
            get
            {
                return _IsMedicineStore;
            }
            set
            {
                _IsMedicineStore = value;
                RaisePropertyChanged("IsMedicineStore");
            }
        }
        private bool _IsMedicineStore;

        [DataMemberAttribute()]
        public bool IsUtilStore
        {
            get
            {
                return _IsUtilStore;
            }
            set
            {
                _IsUtilStore = value;
                RaisePropertyChanged("IsUtilStore");
            }
        }
        private bool _IsUtilStore;

        [DataMemberAttribute()]
        public bool IsChemicalStore
        {
            get
            {
                return _IsChemicalStore;
            }
            set
            {
                _IsChemicalStore = value;
                RaisePropertyChanged("IsChemicalStore");
            }
        }
        private bool _IsChemicalStore;
        #endregion
        #region Navigation Properties

        [DataMemberAttribute()]
        public RefStorageWarehouseType RefStorageWarehouseType
        {
            get
            {
                return _RefStorageWarehouseType;
            }
            set
            {
              
                _RefStorageWarehouseType = value;
                RaisePropertyChanged("RefStorageWarehouseType");
            }
        }
        private RefStorageWarehouseType _RefStorageWarehouseType;

        [DataMemberAttribute()]
        public RefDepartment RefDepartment
        {
            get
            {
                return _RefDepartment;
            }
            set
            {
                if (_RefDepartment != value)
                {
                    _RefDepartment = value;
                    RaisePropertyChanged("RefDepartment");
                }
            }
        }
        private RefDepartment _RefDepartment;

        private bool _IsSubStorage = false;
        [DataMemberAttribute]
        public bool IsSubStorage
        {
            get => _IsSubStorage; set
            {
                _IsSubStorage = value;
                RaisePropertyChanged("IsSubStorage");
            }
        }
        #endregion
        public override bool Equals(object obj)
        {
            RefStorageWarehouseLocation seletedStore = obj as RefStorageWarehouseLocation;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StoreID > 0 && this.StoreID == seletedStore.StoreID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [DataMemberAttribute()]
        public string ListV_MedProductType
        {
            get
            {
                return _ListV_MedProductType;
            }
            set
            {
                _ListV_MedProductType = value;
                RaisePropertyChanged("ListV_MedProductType");
            }
        }
        private string _ListV_MedProductType;

        [DataMemberAttribute()]
        public bool IsVATCreditStorage
        {
            get
            {
                return _IsVATCreditStorage;
            }
            set
            {
                _IsVATCreditStorage = value;
                RaisePropertyChanged("IsVATCreditStorage");
            }
        }
        private bool _IsVATCreditStorage;

        //--▼-- 28/12/2020 DatTB
        public long V_GroupTypes
        {
            get
            {
                return _V_GroupTypes;
            }
            set
            {
                _V_GroupTypes = value;
                RaisePropertyChanged("V_GroupTypes");
            }
        }
        private long _V_GroupTypes;
        //--▲-- 28/12/2020 DatTB

        [DataMemberAttribute()]
        public bool SkipSendToFAST
        {
            get
            {
                return _SkipSendToFAST;
            }
            set
            {
                _SkipSendToFAST = value;
                RaisePropertyChanged("SkipSendToFAST");
            }
        }
        private bool _SkipSendToFAST;

        [DataMemberAttribute()]
        public String StoreCode
        {
            get
            {
                return _StoreCode;
            }
            set
            {
                _StoreCode = value;
                RaisePropertyChanged("StoreCode");
            }
        }
        private String _StoreCode;
        [DataMemberAttribute()]
        public bool IsConsignment
        {
            get
            {
                return _IsConsignment;
            }
            set
            {
                _IsConsignment = value;
                RaisePropertyChanged("IsConsignment");
            }
        }
        private bool _IsConsignment;
        [DataMemberAttribute()]
        public bool IsConsignmentReturn
        {
            get
            {
                return _IsConsignmentReturn;
            }
            set
            {
                _IsConsignmentReturn = value;
                RaisePropertyChanged("IsConsignmentReturn");
            }
        }
        private bool _IsConsignmentReturn;
    }
}