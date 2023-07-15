using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class PackageTechnicalServiceDetail : NotifyChangedBase
    {
        private long _PackageTechnicalServiceDetailID;
        [DataMemberAttribute()]
        public long PackageTechnicalServiceDetailID
        {
            get
            {
                return _PackageTechnicalServiceDetailID;
            }
            set
            {
                _PackageTechnicalServiceDetailID = value;
                RaisePropertyChanged("PackageTechnicalServiceDetailID");
            }
        }
        private long _PackageTechnicalServiceID;
        [DataMemberAttribute()]
        public long PackageTechnicalServiceID
        {
            get
            {
                return _PackageTechnicalServiceID;
            }
            set
            {
                _PackageTechnicalServiceID = value;
                RaisePropertyChanged("PackageTechnicalServiceID");
            }
        }
        private long _MedServiceID;
        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
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
        private RefMedicalServiceItem _ObjRefMedicalServiceItem;
        [DataMemberAttribute()]
        public RefMedicalServiceItem ObjRefMedicalServiceItem
        {
            get
            {
                return _ObjRefMedicalServiceItem;
            }
            set
            {
                _ObjRefMedicalServiceItem = value;
                RaisePropertyChanged("ObjRefMedicalServiceItem");
            }
        }
        private PCLExamType _ObjPCLExamType;
        [DataMemberAttribute()]
        public PCLExamType ObjPCLExamType
        {
            get
            {
                return _ObjPCLExamType;
            }
            set
            {
                _ObjPCLExamType = value;
                RaisePropertyChanged("ObjPCLExamType");
            }
        }
        private bool _IsDeleted;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private string _LogModified;
        [DataMemberAttribute()]
        public string LogModified
        {
            get
            {
                return _LogModified;
            }
            set
            {
                _LogModified = value;
                RaisePropertyChanged("LogModified");
            }
        }
        private long _CreatedStaffID;
        [DataMemberAttribute()]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private int _Qty;
        [DataMemberAttribute()]
        public int Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
    }
}
