using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class PromoDiscountItems : NotifyChangedBase
    {
        private long _PromoDiscItemID;
        [DataMemberAttribute()]
        public long PromoDiscItemID
        {
            get
            {
                return _PromoDiscItemID;
            }
            set
            {
                _PromoDiscItemID = value;
                RaisePropertyChanged("PromoDiscItemID");
            }
        }
        private long _PromoDiscProgID;
        [DataMemberAttribute()]
        public long PromoDiscProgID
        {
            get
            {
                return _PromoDiscProgID;
            }
            set
            {
                _PromoDiscProgID = value;
                RaisePropertyChanged("PromoDiscProgID");
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
    }
}
