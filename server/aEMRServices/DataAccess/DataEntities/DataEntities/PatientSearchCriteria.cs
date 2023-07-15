/*
 * 20170113 #001 CMN: Add QRCode
 * 20181122 #002 TTM: Thêm mới thuộc tính DOBNumIndex để phục vụ việc tìm kiếm bệnh nhân bằng tên đi kèm DOB
 * 20230530 #003 DatTB: Thêm QRCode CCCD
 */
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class PatientSearchCriteria : SearchCriteriaBase
    {
        public PatientSearchCriteria()
        {

        }

        private string _PatientNameString;
        /// <summary>
        /// Thuoc tinh nay khong dung de tim kiem
        /// Chi de lay thong tin nguoi dung nhap tren form (trong o ten benh nhan)
        /// roi extract no ra thanh FullName, hay HICardNumber, hay la PatientCode
        /// </summary>
        public string PatientNameString
        {
            get
            {
                return _PatientNameString;
            }
            set
            {
                _PatientNameString = value;
                RaisePropertyChanged("PatientNameString");
            }
        }
        private string _orderBy;

        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        private string _patientCode;
        public string PatientCode
        {
            get
            {
                return _patientCode;
            }
            set
            {
                _patientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }

        private string _PCLRequestNumID;
        public string PCLRequestNumID
        {
            get 
            { 
                return _PCLRequestNumID; 
            }
            set
            {
                _PCLRequestNumID = value;
                RaisePropertyChanged("PCLRequestNumID");
            }
        }

        private string _insuranceCard;

        public string InsuranceCard
        {
            get
            {
                return _insuranceCard;
            }
            set
            {
                _insuranceCard = value;
                RaisePropertyChanged("InsuranceCard");
            }
        }

        private string _patientBarCode;

        public string PatientBarCode
        {
            get
            {
                return _patientBarCode;
            }
            set
            {
                _patientBarCode = value;
                RaisePropertyChanged("PatientBarCode");
            }
        }

        private string _fullName;

        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private string _firstName;

        public string PMFCode
        {
            get
            {
                return _PMFCode;
            }
            set
            {
                _PMFCode = value;
                RaisePropertyChanged("PMFCode");
            }
        }
        private string _PMFCode;

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                RaisePropertyChanged("FirstName");
            }
        }

        private string _lastName;

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                RaisePropertyChanged("LastName");
            }
        }

        private string _idCard;

        public string IDCard
        {
            get
            {
                return _idCard;
            }
            set
            {
                _idCard = value;
                RaisePropertyChanged("IDCard");
            }
        }

        private Nullable<DateTime> _entryDateBegin;

        public Nullable<DateTime> EntryDateBegin
        {
            get
            {
                return _entryDateBegin;
            }
            set
            {
                _entryDateBegin = value;
                RaisePropertyChanged("EntryDateBegin");
            }
        }
        private Nullable<DateTime> _entryDateEnd;

        public Nullable<DateTime> EntryDateEnd
        {
            get
            {
                return _entryDateEnd;
            }
            set
            {
                _entryDateEnd = value;
                RaisePropertyChanged("EntryDateEnd");
            }
        }
        private bool _entryDateEnabled;

        public bool EntryDateEnabled
        {
            get
            {
                return _entryDateEnabled;
            }
            set
            {
                _entryDateEnabled = value;
                RaisePropertyChanged("EntryDateEnabled");
            }
        }

        private Nullable<DateTime> _birthDateBegin;

        public Nullable<DateTime> BirthDateBegin
        {
            get
            {
                return _birthDateBegin;
            }
            set
            {
                _birthDateBegin = value;
                RaisePropertyChanged("BirthDateBegin");
            }
        }
        private Nullable<DateTime> _birthDateEnd;

        public Nullable<DateTime> BirthDateEnd
        {
            get
            {
                return _birthDateEnd;
            }
            set
            {
                _birthDateEnd = value;
                RaisePropertyChanged("BirthDateEnd");
            }
        }
        private bool _birthDateEnabled;

        public bool BirthDateEnabled
        {
            get
            {
                return _birthDateEnabled;
            }
            set
            {
                _birthDateEnabled = value;
                RaisePropertyChanged("BirthDateEnabled");
            }
        }

        private Nullable<DateTime> _releaseDateBegin;

        public Nullable<DateTime> ReleaseDateBegin
        {
            get
            {
                return _releaseDateBegin;
            }
            set
            {
                _releaseDateBegin = value;
                RaisePropertyChanged("ReleaseDateBegin");
            }
        }
        private Nullable<DateTime> _releaseDateEnd;

        public Nullable<DateTime> ReleaseDateEnd
        {
            get
            {
                return _releaseDateEnd;
            }
            set
            {
                _releaseDateEnd = value;
                RaisePropertyChanged("ReleaseDateEnd");
            }
        }
        private bool _releaseDateEnabled;

        public bool ReleaseDateEnabled
        {
            get
            {
                return _releaseDateEnabled;
            }
            set
            {
                _releaseDateEnabled = value;
                RaisePropertyChanged("ReleaseDateEnabled");
            }
        }

        private Gender _gender;

        public Gender Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
                RaisePropertyChanged("Gender");
            }
        }

        private bool _genderEnabled;

        public bool GenderEnabled
        {
            get
            {
                return _genderEnabled;
            }
            set
            {
                _genderEnabled = value;
                RaisePropertyChanged("GenderEnabled");
            }
        }

        private int? _SequenceNo;

        public int? SequenceNo
        {
            get
            {
                return _SequenceNo;
            }
            set
            {
                _SequenceNo = value;
                RaisePropertyChanged("SequenceNo");
            }
        }
        //==== #001
        private HIQRCode _QRCode;
        public HIQRCode QRCode
        {
            get
            {
                return _QRCode;
            }
            set
            {
                _QRCode = value;
                RaisePropertyChanged("QRCode");
            }
        }
        //==== #001
        private bool _IsShowHICardNo;
        public bool IsShowHICardNo
        {
            get
            {
                return _IsShowHICardNo;
            }
            set
            {
                _IsShowHICardNo = value;
                RaisePropertyChanged("IsShowHICardNo");
            }
        }

        private string _PtRegistrationCode;
        public string PtRegistrationCode
        {
            get
            {
                return _PtRegistrationCode;
            }
            set
            {
                _PtRegistrationCode = value;
                RaisePropertyChanged("PtRegistrationCode");
            }
        }
        //▼====== #002
        [DataMemberAttribute()]
        public int DOBNumIndex
        {
            get
            {
                return _DOBNumIndex;
            }
            set
            {
                if (_DOBNumIndex != value)
                {
                    _DOBNumIndex = value;
                    RaisePropertyChanged("DOBNumIndex");
                }
            }
        }
        private int _DOBNumIndex;
        //▲====== #002

        private string _QMSSerial;
        [DataMemberAttribute()]
        public string QMSSerial
        {
            get
            {
                return _QMSSerial;
            }
            set
            {
                _QMSSerial = value;
                RaisePropertyChanged("QMSSerial");
            }
        }
        private long _CityProvinceID;
        public long CityProvinceID
        {
            get
            {
                return _CityProvinceID;
            }
            set
            {
                _CityProvinceID = value;
                RaisePropertyChanged("CityProvinceID");
            }
        }
        private long _SuburbNameID;
        public long SuburbNameID
        {
            get
            {
                return _SuburbNameID;
            }
            set
            {
                _SuburbNameID = value;
                RaisePropertyChanged("SuburbNameID");
            }
        }

        private long _OrderNumber;
        public long OrderNumber
        {
            get { return _OrderNumber; }
            set {
                _OrderNumber = value;
                RaisePropertyChanged("OrderNumber");
            }
        }
        
        //▼==== #003
        private IDCardQRCode _IDCardQRCode;
        public IDCardQRCode IDCardQRCode
        {
            get
            {
                return _IDCardQRCode;
            }
            set
            {
                _IDCardQRCode = value;
                RaisePropertyChanged("IDCardQRCode");
            }
        }

        private string _IDNumber;
        public string IDNumber
        {
            get
            {
                return _IDNumber;
            }
            set
            {
                _IDNumber = value;
                RaisePropertyChanged("IDNumber");
            }
        }

        private string _IDNumberOld;
        public string IDNumberOld
        {
            get
            {
                return _IDNumberOld;
            }
            set
            {
                _IDNumberOld = value;
                RaisePropertyChanged("IDNumberOld");
            }
        }
        //▲==== #003
    }
}