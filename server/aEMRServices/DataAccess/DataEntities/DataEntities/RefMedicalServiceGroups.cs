using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace DataEntities
{

    public partial class RefMedicalServiceGroups : NotifyChangedBase
    {

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 MedicalServiceGroupID
        {
            get
            {
                return _MedicalServiceGroupID;
            }
            set
            {
                if (_MedicalServiceGroupID != value)
                {
                    OnMedicalServiceGroupIDChanging(value);
                    _MedicalServiceGroupID = value;
                    RaisePropertyChanged("MedicalServiceGroupID");
                    OnMedicalServiceGroupIDChanged();
                }
            }
        }
        private Int64 _MedicalServiceGroupID;
        partial void OnMedicalServiceGroupIDChanging(Int64 value);
        partial void OnMedicalServiceGroupIDChanged();


        [StringLength(15, MinimumLength = 0, ErrorMessage = "Mã Nhóm Phải <= 15 Ký Tự")]
        [DataMemberAttribute()]
        public String MedicalServiceGroupCode
        {
            get
            {
                return _MedicalServiceGroupCode;
            }
            set
            {
                OnMedicalServiceGroupCodeChanging(value);
                ValidateProperty("MedicalServiceGroupCode", value);
                _MedicalServiceGroupCode = value;
                RaisePropertyChanged("MedicalServiceGroupCode");
                OnMedicalServiceGroupCodeChanged();
            }
        }
        private String _MedicalServiceGroupCode;
        partial void OnMedicalServiceGroupCodeChanging(String value);
        partial void OnMedicalServiceGroupCodeChanged();


        [StringLength(125, MinimumLength = 0, ErrorMessage = "Tên Nhóm Phải <= 125 Ký Tự")]
        [DataMemberAttribute()]
        public String MedicalServiceGroupName
        {
            get
            {
                return _MedicalServiceGroupName;
            }
            set
            {
                OnMedicalServiceGroupNameChanging(value);
                ValidateProperty("MedicalServiceGroupName", value);
                _MedicalServiceGroupName = value;
                RaisePropertyChanged("MedicalServiceGroupName");
                OnMedicalServiceGroupNameChanged();
            }
        }
        private String _MedicalServiceGroupName;
        partial void OnMedicalServiceGroupNameChanging(String value);
        partial void OnMedicalServiceGroupNameChanged();
        

        [DataMemberAttribute()]
        public String MedicalServiceGroupDescription
        {
            get
            {
                return _MedicalServiceGroupDescription;
            }
            set
            {
                OnMedicalServiceGroupDescriptionChanging(value);
                _MedicalServiceGroupDescription = value;
                RaisePropertyChanged("MedicalServiceGroupDescription");
                OnMedicalServiceGroupDescriptionChanged();
            }
        }
        private String _MedicalServiceGroupDescription;
        partial void OnMedicalServiceGroupDescriptionChanging(String value);
        partial void OnMedicalServiceGroupDescriptionChanged();
        #endregion

        #region Navigation Properties
        #endregion

        private List<RefMedicalServiceGroupItem> _RefMedicalServiceGroupItems;
        [DataMemberAttribute]
        public List<RefMedicalServiceGroupItem> RefMedicalServiceGroupItems
        {
            get
            {
                return _RefMedicalServiceGroupItems;
            }
            set
            {
                _RefMedicalServiceGroupItems = value;
                RaisePropertyChanged("RefMedicalServiceGroupItems");
            }
        }

        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        private string _RegistrationTypeStr;
        [DataMemberAttribute]
        public string RegistrationTypeStr
        {
            get
            {
                return _RegistrationTypeStr;
            }
            set
            {
                _RegistrationTypeStr = value;
                RaisePropertyChanged("RegistrationTypeStr");
            }
        }
        private long _V_MedicalServiceGroupType = (long)AllLookupValues.V_MedicalServiceGroupType.Kham_Benh;
        [DataMemberAttribute]
        public long V_MedicalServiceGroupType
        {
            get
            {
                return _V_MedicalServiceGroupType;
            }
            set
            {
                _V_MedicalServiceGroupType = value;
                RaisePropertyChanged("V_MedicalServiceGroupType");
            }
        }
        private string _MedicalServiceGroupTypeStr;
        [DataMemberAttribute]
        public string MedicalServiceGroupTypeStr
        {
            get
            {
                return _MedicalServiceGroupTypeStr;
            }
            set
            {
                _MedicalServiceGroupTypeStr = value;
                RaisePropertyChanged("MedicalServiceGroupTypeStr");
            }
        }
        private bool? _Gender =null;
        [DataMemberAttribute]
        public bool? Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                _Gender = value;
                RaisePropertyChanged("Gender");
            }
        }

        private byte? _AgeFrom = null;
        [DataMemberAttribute]
        public byte? AgeFrom
        {
            get
            {
                return _AgeFrom;
            }
            set
            {
                _AgeFrom = value;
                RaisePropertyChanged("AgeFrom");
            }
        }

        private byte? _AgeTo = null;
        [DataMemberAttribute]
        public byte? AgeTo
        {
            get
            {
                return _AgeTo;
            }
            set
            {
                _AgeTo = value;
                RaisePropertyChanged("AgeTo");
            }
        }

        private long? _V_MedicalServiceParentGroup = null;
        [DataMemberAttribute]
        public long? V_MedicalServiceParentGroup
        {
            get
            {
                return _V_MedicalServiceParentGroup;
            }
            set
            {
                _V_MedicalServiceParentGroup = value;
                RaisePropertyChanged("V_MedicalServiceParentGroup");
            }
        }
        private decimal _DiscountPercent = 0;
        [DataMemberAttribute]
        public decimal DiscountPercent
        {
            get
            {
                return _DiscountPercent;
            }
            set
            {
                _DiscountPercent = value;
                RaisePropertyChanged("DiscountPercent");
            }
        }
        public string ConvertToXml()
        {
            if (this == null)
                return null;
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("RefMedicalServiceGroup", new XElement[] {
                new XElement("MedicalServiceGroupID", MedicalServiceGroupID),
                new XElement("MedicalServiceGroupCode", MedicalServiceGroupCode),
                new XElement("MedicalServiceGroupName", MedicalServiceGroupName),
                new XElement("MedicalServiceGroupDescription", MedicalServiceGroupDescription),
                new XElement("V_RegistrationType", V_RegistrationType),
                new XElement("V_MedicalServiceGroupType", V_MedicalServiceGroupType),
                new XElement("Gender", Gender),
                new XElement("AgeFrom", AgeFrom),
                new XElement("AgeTo", AgeTo),
                new XElement("DiscountPercent", DiscountPercent),
                new XElement("V_MedicalServiceParentGroup", V_MedicalServiceParentGroup),
                new XElement("RefMedicalServiceGroupItems", RefMedicalServiceGroupItems.Select(x=>x.ConvertToXElement()))
            }));
            return mXDocument.ToString();
        }
    }
}