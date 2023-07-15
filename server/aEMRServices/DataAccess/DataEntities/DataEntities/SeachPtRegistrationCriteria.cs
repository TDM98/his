using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class SeachPtRegistrationCriteria : SearchCriteriaBase
    {
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

        public SeachPtRegistrationCriteria()
        {
        }
        private string _FullName;
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
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

        private Nullable<DateTime> _Fromdate;
        public Nullable<DateTime> FromDate
        {
            get
            {
                return _Fromdate;
            }
            set
            {
                _Fromdate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private Nullable<DateTime> _Todate;
        public Nullable<DateTime> ToDate
        {
            get
            {
                return _Todate;
            }
            set
            {
                _Todate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private bool? _IsNgoaiTru;
        public bool? IsNgoaiTru
        {
            get
            {
                return _IsNgoaiTru;
            }
            set
            {
                if (_IsNgoaiTru != value)
                {
                    _IsNgoaiTru = value;
                    RaisePropertyChanged("IsNgoaiTru");
                }
            }
        }

        private string _HICard;
        public string HICard
        {
            get
            {
                return _HICard;
            }
            set
            {
                if (_HICard != value)
                {
                    _HICard = value;
                    RaisePropertyChanged("HICard");
                }
            }
        }

        private long? _RegStatus;
        public long? RegStatus
        {
            get
            {
                return _RegStatus;
            }
            set
            {
                if (_RegStatus != value)
                {
                    _RegStatus = value;
                    RaisePropertyChanged("RegStatus");
                }
            }
        }

        private long? _regType;
        public long? RegType
        {
            get
            {
                return _regType;
            }
            set
            {
                if (_regType != value)
                {
                    _regType = value;
                    RaisePropertyChanged("RegType");
                }
            }
        }

        private long _SearchByVregForPtOfType;
        public long SearchByVregForPtOfType
        {
            get
            {
                return _SearchByVregForPtOfType;
            }
            set
            {
                _SearchByVregForPtOfType = value;
                RaisePropertyChanged("SearchByVregForPtOfType");
            }
        }

        private long? _paymentStatus;
        public long? PaymentStatus
        {
            get
            {
                return _paymentStatus;
            }
            set
            {
                if (_paymentStatus != value)
                {
                    _paymentStatus = value;
                    RaisePropertyChanged("PaymentStatus");
                }
            }
        }

        private long? _StaffID;
        public long? StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get
            {
                return _OrderBy;
            }
            set
            {
                if (_OrderBy != value)
                {
                    _OrderBy = value;
                    RaisePropertyChanged("OrderBy");
                }
            }
        }

        private bool _HiChecked;

        private bool? _IsHoanTat;
        public bool? IsHoanTat
        {
            get
            {
                return _IsHoanTat;
            }
            set
            {
                if (_IsHoanTat != value)
                {
                    _IsHoanTat = value;
                    RaisePropertyChanged("IsHoanTat");
                }
            }
        }

        private bool? _IsAdmission;
        public bool? IsAdmission
        {
            get
            {
                return _IsAdmission;
            }
            set
            {
                if (_IsAdmission != value)
                {
                    _IsAdmission = value;
                    RaisePropertyChanged("IsAdmission");
                }
            }
        }

        /// <summary>
        /// True: Đã xuất viện.
        /// False: Chưa xuất viện,
        /// Null: Cả hai.
        /// </summary>
        private bool? _isDischarge;
        public bool? IsDischarge
        {
            get
            {
                return _isDischarge;
            }
            set
            {
                if (_isDischarge != value)
                {
                    _isDischarge = value;
                    RaisePropertyChanged("IsDischarge");
                }
            }
        }

        private bool? _IsAppointment;
        public bool? IsAppointment
        {
            get
            {
                return _IsAppointment;
            }
            set
            {
                if (_IsAppointment != value)
                {
                    _IsAppointment = value;
                    RaisePropertyChanged("IsAppointment");
                }
            }
        }

        /// <summary>
        /// Chỉ tìm các đăng ký có bảo hiểm
        /// </summary>
        public bool HiChecked
        {
            get
            {
                return _HiChecked;
            }
            set
            {
                if (_HiChecked != value)
                {
                    _HiChecked = value;
                    RaisePropertyChanged("HiChecked");
                }
            }
        }

        private bool _ServiceChecked;
        /// <summary>
        /// Chỉ tìm các đăng ký dịch vụ
        /// </summary>
        public bool ServiceChecked
        {
            get
            {
                return _ServiceChecked;
            }
            set
            {
                if (_ServiceChecked != value)
                {
                    _ServiceChecked = value;
                    RaisePropertyChanged("ServiceChecked");
                }
            }
        }

        private bool _HiAndServiceChecked;
        /// <summary>
        /// Tìm tất cả (Đăng ký có bảo hiểm + dịch vụ)
        /// </summary>
        public bool HiAndServiceChecked
        {
            get
            {
                return _HiAndServiceChecked;
            }
            set
            {
                if (_HiAndServiceChecked != value)
                {
                    _HiAndServiceChecked = value;
                    RaisePropertyChanged("HiAndServiceChecked");
                }
            }
        }


        private Nullable<long> _DeptID;
        public Nullable<long> DeptID
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


        private Nullable<long> _DeptLocationID;
        public Nullable<long> DeptLocationID
        {
            get { return _DeptLocationID; }
            set
            {
                if (_DeptLocationID != value)
                {
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                }
            }
        }


        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get { return _PatientFindBy; }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                    RaisePropertyChanged("PatientFindBy");
                }
            }
        }


        private bool _KhamBenh;
        public bool KhamBenh
        {
            get { return _KhamBenh; }
            set
            {
                if (_KhamBenh != value)
                {
                    _KhamBenh = value;
                    RaisePropertyChanged("KhamBenh");
                }
            }
        }

        private int _pageIndex;
        public int pageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (_pageIndex != value)
                {
                    _pageIndex = value;
                    RaisePropertyChanged("pageIndex");
                }
            }
        }

        private int _pageSize;
        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    RaisePropertyChanged("pageSize");
                }
            }
        }

        /// <summary>
        /// Loại search
        /// 0 : Tất cả
        /// 1 : Không BH
        /// 2 : Có BH
        /// 3 : Đúng Tuyến
        /// 4 : Trái Tuyến
        /// </summary>
        private int _TypeSearch;
        public int TypeSearch
        {
            get { return _TypeSearch; }
            set
            {
                if (_TypeSearch != value)
                {
                    _TypeSearch = value;
                    RaisePropertyChanged("TypeSearch");
                }
            }
        }

        private bool _IsCancel;
        public bool IsCancel
        {
            get { return _IsCancel; }
            set
            {
                if (_IsCancel != value)
                {
                    _IsCancel = value;
                    RaisePropertyChanged("IsCancel");
                }
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

        private bool? _IsReported;
        public bool? IsReported
        {
            get
            {
                return _IsReported;
            }
            set
            {
                _IsReported = value;
                RaisePropertyChanged("IsReported");
            }
        }

        private long? _V_TreatmentType;
        public long? V_TreatmentType
        {
            get
            {
                return _V_TreatmentType;
            }
            set
            {
                _V_TreatmentType = value;
                RaisePropertyChanged("V_TreatmentType");
            }
        }
        private bool _IsSearchByRegistrationDetails = false;
        public bool IsSearchByRegistrationDetails
        {
            get
            {
                return _IsSearchByRegistrationDetails;
            }
            set
            {
                _IsSearchByRegistrationDetails = value;
                RaisePropertyChanged("IsSearchByRegistrationDetails");
            }
        }
        private bool _IsExportEInvoiceView = false;
        public bool IsExportEInvoiceView
        {
            get
            {
                return _IsExportEInvoiceView;
            }
            set
            {
                _IsExportEInvoiceView = value;
                RaisePropertyChanged("IsExportEInvoiceView");
            }
        }
        private bool _IsHasInvoice = false;
        public bool IsHasInvoice
        {
            get
            {
                return _IsHasInvoice;
            }
            set
            {
                _IsHasInvoice = value;
                RaisePropertyChanged("IsHasInvoice");
            }
        }

        private long? _V_RegistrationType;
        public long? V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (value == _V_RegistrationType)
                {
                    return;
                }
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private byte _ViewCase = 0;
        public byte ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                _ViewCase = value;
                RaisePropertyChanged("ViewCase");
            }
        }

        private bool _IsExportData;
        public bool IsExportData
        {
            get
            {
                return _IsExportData;
            }
            set
            {
                _IsExportData = value;
                RaisePropertyChanged("IsExportData");
            }
        }

        private string _SafeFileName;
        public string SafeFileName
        {
            get
            {
                return _SafeFileName;
            }
            set
            {
                _SafeFileName = value;
                RaisePropertyChanged("SafeFileName");
            }
        }

        private long _HosClientID;
        public long HosClientID
        {
            get
            {
                return _HosClientID;
            }
            set
            {
                _HosClientID = value;
                RaisePropertyChanged("HosClientID");
            }
        }

        private bool _IsSearchOnlyProcedure;
        public bool IsSearchOnlyProcedure
        {
            get { return _IsSearchOnlyProcedure; }
            set
            {
                if (_IsSearchOnlyProcedure != value)
                {
                    _IsSearchOnlyProcedure = value;
                    RaisePropertyChanged("IsSearchOnlyProcedure");
                }
            }
        }
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
        private bool _IsSearchForCashAdvance = false;
        public bool IsSearchForCashAdvance
        {
            get
            {
                return _IsSearchForCashAdvance;
            }
            set
            {
                _IsSearchForCashAdvance = value;
                RaisePropertyChanged("IsSearchForCashAdvance");
            }
        }
        private bool _IsCancelDTDT = false;
        public bool IsCancelDTDT
        {
            get
            {
                return _IsCancelDTDT;
            }
            set
            {
                _IsCancelDTDT = value;
                RaisePropertyChanged("IsCancelDTDT");
            }
        }
    }
}