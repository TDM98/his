using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class PCLExamTypeContactDrugs : EntityBase
    {
        private long _PCLExamTypeContactDrugID;
        [DataMemberAttribute()]
        public long PCLExamTypeContactDrugID
        {
            get
            {
                return _PCLExamTypeContactDrugID;
            }
            set
            {
                _PCLExamTypeContactDrugID = value;
                RaisePropertyChanged("PCLExamTypeContactDrugID");
            }
        }

        private long _GenMedProductID;
        [DataMemberAttribute()]
        public long GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
            }
        }

        private RefGenMedProductDetails _GenMedProduct;
        [DataMemberAttribute()]
        public RefGenMedProductDetails GenMedProduct
        {
            get
            {
                return _GenMedProduct;
            }
            set
            {
                _GenMedProduct = value;
                RaisePropertyChanged("GenMedProduct");
            }
        }

        private long _PCLExamTypeID;
        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }

        private bool _Is_Delete;
        [DataMemberAttribute()]
        public bool Is_Delete
        {
            get
            {
                return _Is_Delete;
            }
            set
            {
                _Is_Delete = value;
                RaisePropertyChanged("Is_Delete");
            }
        }

        private long? _StaffCreatedID;
        [DataMemberAttribute()]
        public long? StaffCreatedID
        {
            get
            {
                return _StaffCreatedID;
            }
            set
            {
                _StaffCreatedID = value;
                RaisePropertyChanged("StaffCreatedID");
            }
        }

        private DateTime? _CreatedDate;
        [DataMemberAttribute()]
        public DateTime? CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }

        private long? _StaffDeletedID;
        [DataMemberAttribute()]
        public long? StaffDeletedID
        {
            get
            {
                return _StaffDeletedID;
            }
            set
            {
                _StaffDeletedID = value;
                RaisePropertyChanged("StaffDeletedID");
            }
        }

        private DateTime? _DeletedDate;
        [DataMemberAttribute()]
        public DateTime? DeletedDate
        {
            get
            {
                return _DeletedDate;
            }
            set
            {
                _DeletedDate = value;
                RaisePropertyChanged("DeletedDate");
            }
        }
      
    }
}
