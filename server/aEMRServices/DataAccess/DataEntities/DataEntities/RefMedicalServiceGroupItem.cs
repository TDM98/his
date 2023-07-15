using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace DataEntities
{
    public class RefMedicalServiceGroupItem : NotifyChangedBase
    {
        public RefMedicalServiceGroupItem() { }
        public RefMedicalServiceGroupItem(MedRegItemBase aMedRegItemBase)
        {
            if (aMedRegItemBase is PatientPCLRequestDetail && (aMedRegItemBase as PatientPCLRequestDetail).PCLExamType != null)
            {
                PCLExamTypeID = (aMedRegItemBase as PatientPCLRequestDetail).PCLExamType.PCLExamTypeID;
            }
            else if (aMedRegItemBase is PatientRegistrationDetail && (aMedRegItemBase as PatientRegistrationDetail).RefMedicalServiceItem != null)
            {
                MedServiceID = (aMedRegItemBase as PatientRegistrationDetail).RefMedicalServiceItem.MedServiceID;
            }
        }
        private long _MedicalServiceGroupItemID;
        private long _MedicalServiceGroupID;
        private long? _MedServiceID;
        private long? _PCLExamTypeID;
        private bool? _IsDeleted;
        private DateTime? _RecCreatedDate;
        private RefMedicalServiceItem _RefMedicalServiceItemObj;
        [DataMemberAttribute]
        public long MedicalServiceGroupItemID
        {
            get
            {
                return _MedicalServiceGroupItemID;
            }
            set
            {
                _MedicalServiceGroupItemID = value;
                RaisePropertyChanged("MedicalServiceGroupItemID");
            }
        }
        [DataMemberAttribute]
        public long MedicalServiceGroupID
        {
            get
            {
                return _MedicalServiceGroupID;
            }
            set
            {
                _MedicalServiceGroupID = value;
                RaisePropertyChanged("MedicalServiceGroupID");
            }
        }
        [DataMemberAttribute]
        public long? MedServiceID
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
        [DataMemberAttribute]
        public long? PCLExamTypeID
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
        [DataMemberAttribute]
        public bool? IsDeleted
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
        [DataMemberAttribute]
        public DateTime? RecCreatedDate
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

        private int _Qty;
        [DataMemberAttribute]
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
        public RefMedicalServiceItem RefMedicalServiceItemObj
        {
            get
            {
                return _RefMedicalServiceItemObj;
            }
            set
            {
                _RefMedicalServiceItemObj = value;
                RaisePropertyChanged("RefMedicalServiceItemObj");
            }
        }

        public XElement ConvertToXElement()
        {
            if (this == null)
            {
                return null;
            }
            return new XElement("RefMedicalServiceGroupItem",
                 new XElement("MedicalServiceGroupItemID", MedicalServiceGroupItemID),
                 new XElement("MedServiceID", MedServiceID),
                 new XElement("PCLExamTypeID", PCLExamTypeID),
                 new XElement("Qty", Qty),
                 new XElement("IsDeleted", IsDeleted));
        }
    }
}