using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace DataEntities
{
    public class HospitalClientContract : NotifyChangedBase
    {
        private long _HosClientContractID;
        private HospitalClient _HosClient;
        private string _ContractName;
        private string _ContractNo;
        private DateTime _ContractDate;
        private string _ContractDescription;
        private DateTime _ValidDateFrom;
        private DateTime _ValidDateTo;
        private DateTime _ActivationDate;
        private Staff _CreatedStaff;
        private Staff _ModifiedStaff;
        private RefDocument _ContractDocument;
        private DateTime _RecCreatedDate;
        private IList<ClientContractServiceItem> _ContractServiceItemCollection;
        private IList<HosClientContractPatient> _ContractPatientCollection;
        private IList<ClientContractServiceItemPatientLink> _ServiceItemPatientLinkCollection;
        private bool _IsPayAddingMoreSvs;
        private DateTime? _CompletedDate;
        private decimal _TotalContractAmount;
        private DateTime? _FinalizedDate;
        private IList<HosClientContractFinalization> _ClientContractFinalizationCollection;
        private byte _DiscountPercent;
        private bool _IsCreateNewToOld;
        [DataMemberAttribute]
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                _HosClientContractID = value;
                RaisePropertyChanged("HosClientContractID");
                RaisePropertyChanged("IsDBSaved");
                RaisePropertyChanged("IsDBSavedButNotCompleted");
            }
        }
        [DataMemberAttribute]
        public HospitalClient HosClient
        {
            get
            {
                return _HosClient;
            }
            set
            {
                if (_HosClient == value)
                {
                    return;
                }
                _HosClient = value;
                RaisePropertyChanged("HosClient");
                RaisePropertyChanged("IsHealthyOrganization");
            }
        }
        [DataMemberAttribute]
        public string ContractName
        {
            get
            {
                return _ContractName;
            }
            set
            {
                _ContractName = value;
                RaisePropertyChanged("ContractName");
            }
        }
        [DataMemberAttribute]
        public string ContractNo
        {
            get
            {
                return _ContractNo;
            }
            set
            {
                _ContractNo = value;
                RaisePropertyChanged("ContractNo");
            }
        }
        [DataMemberAttribute]
        public DateTime ContractDate
        {
            get
            {
                return _ContractDate;
            }
            set
            {
                _ContractDate = value;
                RaisePropertyChanged("ContractDate");
            }
        }
        [DataMemberAttribute]
        public string ContractDescription
        {
            get
            {
                return _ContractDescription;
            }
            set
            {
                _ContractDescription = value;
                RaisePropertyChanged("ContractDescription");
            }
        }
        [DataMemberAttribute]
        public DateTime ValidDateFrom
        {
            get
            {
                return _ValidDateFrom;
            }
            set
            {
                _ValidDateFrom = value;
                RaisePropertyChanged("ValidDateFrom");
            }
        }
        [DataMemberAttribute]
        public DateTime ValidDateTo
        {
            get
            {
                return _ValidDateTo;
            }
            set
            {
                _ValidDateTo = value;
                RaisePropertyChanged("ValidDateTo");
            }
        }
        [DataMemberAttribute]
        public DateTime ActivationDate
        {
            get
            {
                return _ActivationDate;
            }
            set
            {
                _ActivationDate = value;
                RaisePropertyChanged("ActivationDate");
                RaisePropertyChanged("IsActivated");
                RaisePropertyChanged("IsActivatedButNotCompleted");
                RaisePropertyChanged("IsActivatedButNotFinalized");
            }
        }
        [DataMemberAttribute]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        [DataMemberAttribute]
        public Staff ModifiedStaff
        {
            get
            {
                return _ModifiedStaff;
            }
            set
            {
                _ModifiedStaff = value;
                RaisePropertyChanged("ModifiedStaff");
            }
        }
        [DataMemberAttribute]
        public RefDocument ContractDocument
        {
            get
            {
                return _ContractDocument;
            }
            set
            {
                _ContractDocument = value;
                RaisePropertyChanged("ContractDocument");
            }
        }
        [DataMemberAttribute]
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
        [DataMemberAttribute]
        public IList<ClientContractServiceItem> ContractServiceItemCollection
        {
            get
            {
                return _ContractServiceItemCollection;
            }
            set
            {
                _ContractServiceItemCollection = value;
                RaisePropertyChanged("ContractServiceItemCollection");
            }
        }
        [DataMemberAttribute]
        public IList<HosClientContractPatient> ContractPatientCollection
        {
            get
            {
                return _ContractPatientCollection;
            }
            set
            {
                _ContractPatientCollection = value;
                RaisePropertyChanged("ContractPatientCollection");
            }
        }
        [DataMemberAttribute]
        public IList<ClientContractServiceItemPatientLink> ServiceItemPatientLinkCollection
        {
            get
            {
                return _ServiceItemPatientLinkCollection;
            }
            set
            {
                _ServiceItemPatientLinkCollection = value;
                RaisePropertyChanged("ServiceItemPatientLinkCollection");
            }
        }
        public void InitContractPatientItemPrice(HosClientContractPatient aContractPatient)
        {
            aContractPatient.TotalInvoicePrice = ServiceItemPatientLinkCollection.Where(x => x.ContractPatient == aContractPatient).Select(x => x.ContractMedRegItem.MedRegItem).Sum(x => x.InvoicePrice);
            aContractPatient.TotalHIPrice = ServiceItemPatientLinkCollection.Where(x => x.ContractPatient == aContractPatient).Select(x => x.ContractMedRegItem.MedRegItem).Sum(x => x.HIAllowedPrice.GetValueOrDefault(0));
        }
        public string ConvertToXml()
        {
            if (ContractServiceItemCollection == null)
            {
                ContractServiceItemCollection = new List<ClientContractServiceItem>();
            }
            if (ContractPatientCollection == null)
            {
                ContractPatientCollection = new List<HosClientContractPatient>();
            }
            if (ServiceItemPatientLinkCollection == null)
            {
                ServiceItemPatientLinkCollection = new List<ClientContractServiceItemPatientLink>();
            }
            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("HospitalClientContract", new XElement[] {
                new XElement("HosClientContractID", HosClientContractID),
                new XElement("HosClientID", HosClient == null ? null : (long?)HosClient.HosClientID),
                new XElement("ContractName", ContractName),
                new XElement("ContractNo", ContractNo),
                new XElement("ContractDate", ContractDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new XElement("ContractDescription", ContractDescription),
                new XElement("ValidDateFrom", ValidDateFrom.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new XElement("ValidDateTo", ValidDateTo.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new XElement("ModifiedStaffID", ModifiedStaff == null ? null : (long?)ModifiedStaff.StaffID),
                new XElement("IsPayAddingMoreSvs", IsPayAddingMoreSvs),
                new XElement("DiscountPercent", DiscountPercent),
                new XElement("ServiceItemCollection", from aItem in ContractServiceItemCollection
                    select new XElement("ServiceItem",
                    new XElement("ClientContractSvcID", aItem.ClientContractSvcID),
                    new XElement("MedServiceID", aItem.MedRegItem == null || !(aItem.MedRegItem is PatientRegistrationDetail) ? null : (long?)(aItem.MedRegItem as PatientRegistrationDetail).MedServiceID),
                    new XElement("PCLExamTypeID", aItem.MedRegItem == null || !(aItem.MedRegItem is PatientPCLRequestDetail) ? null : (long?)(aItem.MedRegItem as PatientPCLRequestDetail).PCLExamTypeID),
                    new XElement("ServiceName", aItem.MedRegItem.ChargeableItemName),
                    new XElement("NormalPrice", aItem.MedRegItem.ChargeableItem == null ? 0 : aItem.MedRegItem.ChargeableItem.NormalPrice),
                    new XElement("UnitPrice", aItem.MedRegItem.InvoicePrice),
                    new XElement("Qty", aItem.MedRegItem.Qty),
                    new XElement("TotalAmount", aItem.MedRegItem.TotalInvoicePrice))),
                new XElement("PatientCollection", from aItem in ContractPatientCollection
                    select new XElement("Patient",
                        new XElement("HosContractPtID", aItem.HosContractPtID),
                        new XElement("PatientID", aItem.PatientObj == null ? null : (long?)aItem.PatientObj.PatientID),
                        new XElement("ClientClassification", aItem.ClientClassification),
                        new XElement("ClientGroup", aItem.ClientGroup),
                        new XElement("GroupCollection", aItem.PatientGroupCollection == null ? null : aItem.PatientGroupCollection.Select(x => new XElement("PatientGroupID", x.HosClientContractPatientGroupID))))),
                new XElement("ServiceItemPatientLinkCollection", from aItem in ServiceItemPatientLinkCollection
                    select new XElement("ServiceItemPatientLink",
                    new XElement("ClientContractSvcPtID", aItem.ClientContractSvcPtID),
                    new XElement("MedServiceID", aItem.ContractMedRegItem == null || aItem.ContractMedRegItem.MedRegItem == null || !(aItem.ContractMedRegItem.MedRegItem is PatientRegistrationDetail) ? null : (long?)(aItem.ContractMedRegItem.MedRegItem as PatientRegistrationDetail).MedServiceID),
                    new XElement("PCLExamTypeID", aItem.ContractMedRegItem == null || aItem.ContractMedRegItem.MedRegItem == null || !(aItem.ContractMedRegItem.MedRegItem is PatientPCLRequestDetail) ? null : (long?)(aItem.ContractMedRegItem.MedRegItem as PatientPCLRequestDetail).PCLExamTypeID),
                    new XElement("PatientID", aItem.ContractPatient == null || aItem.ContractPatient.PatientObj == null ? null : (long?)aItem.ContractPatient.PatientObj.PatientID)))
            }));
            return xmlDocument.ToString();
        }
        public bool IsDBSaved
        {
            get
            {
                return HosClientContractID > 0;
            }
        }
        public bool IsActivated
        {
            get
            {
                return ActivationDate != null && ActivationDate > DateTime.MinValue;
            }
        }
        [DataMemberAttribute]
        public bool IsPayAddingMoreSvs
        {
            get
            {
                return _IsPayAddingMoreSvs;
            }
            set
            {
                if (_IsPayAddingMoreSvs == value)
                {
                    return;
                }
                _IsPayAddingMoreSvs = value;
                RaisePropertyChanged("IsPayAddingMoreSvs");
            }
        }
        [DataMemberAttribute]
        public DateTime? CompletedDate
        {
            get
            {
                return _CompletedDate;
            }
            set
            {
                if (_CompletedDate == value)
                {
                    return;
                }
                _CompletedDate = value;
                RaisePropertyChanged("CompletedDate");
                RaisePropertyChanged("IsCompleted");
                RaisePropertyChanged("IsActivatedButNotCompleted");
                RaisePropertyChanged("IsCompletedButNotFinalized");
                RaisePropertyChanged("IsDBSavedButNotCompleted");
            }
        }
        public bool IsCompleted
        {
            get
            {
                return CompletedDate != null && CompletedDate > DateTime.MinValue;
            }
        }
        [DataMemberAttribute]
        public decimal TotalContractAmount
        {
            get
            {
                return _TotalContractAmount;
            }
            set
            {
                _TotalContractAmount = value;
                RaisePropertyChanged("TotalContractAmount");
            }
        }
        [DataMemberAttribute]
        public DateTime? FinalizedDate
        {
            get
            {
                return _FinalizedDate;
            }
            set
            {
                if (_FinalizedDate == value)
                {
                    return;
                }
                _FinalizedDate = value;
                RaisePropertyChanged("FinalizedDate");
                RaisePropertyChanged("IsFinalized");
                RaisePropertyChanged("IsActivatedButNotFinalized");
                RaisePropertyChanged("IsCompletedButNotFinalized");
            }
        }
        public bool IsFinalized
        {
            get
            {
                return FinalizedDate != null && FinalizedDate > DateTime.MinValue;
            }
        }
        public bool IsActivatedButNotCompleted
        {
            get
            {
                return IsActivated && !IsCompleted;
            }
        }
        public bool IsActivatedButNotFinalized
        {
            get
            {
                return IsActivated && !IsFinalized;
            }
        }
        public bool IsCompletedButNotFinalized
        {
            get
            {
                return IsCompleted && !IsFinalized;
            }
        }
        public bool IsDBSavedButNotCompleted
        {
            get
            {
                return IsDBSaved && !IsCompleted;
            }
        }
        public IList<HosClientContractFinalization> ClientContractFinalizationCollection
        {
            get
            {
                return _ClientContractFinalizationCollection;
            }
            set
            {
                if (_ClientContractFinalizationCollection == value)
                {
                    return;
                }
                _ClientContractFinalizationCollection = value;
                RaisePropertyChanged("ClientContractFinalizationCollection");
            }
        }
        public bool IsHealthyOrganization
        {
            get
            {
                return HosClient != null && HosClient.V_HosClientType == (long)AllLookupValues.V_HosClientType.HealthyOrganization;
            }
        }
        [DataMemberAttribute]
        public byte DiscountPercent
        {
            get
            {
                return _DiscountPercent;
            }
            set
            {
                if (_DiscountPercent == value)
                {
                    return;
                }
                _DiscountPercent = value;
                RaisePropertyChanged("DiscountPercent");
            }
        }
        [DataMemberAttribute]
        public bool IsCreateNewToOld
        {
            get
            {
                return _IsCreateNewToOld;
            }
            set
            {
                if (_IsCreateNewToOld == value)
                {
                    return;
                }
                _IsCreateNewToOld = value;
                RaisePropertyChanged("IsCreateNewToOld");
            }
        }
    }
}