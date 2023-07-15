using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    //KMx: Khi load Registration thì dùng Switch này để lấy những thông tin cần thiết thôi, không lấy hết như trước đây (17/09/2014 15:32).
    [DataContract]
    public class LoadRegistrationSwitch : NotifyChangedBase
    {
        //KMx: Mặc định = true, vì đa số đều phải load Registration (17/09/2014 15:32).
        private bool _isGetRegistration = true;
        [DataMemberAttribute()]
        public bool IsGetRegistration
        {
            get
            {
                return _isGetRegistration;
            }
            set
            {
                if (_isGetRegistration != value)
                {
                    _isGetRegistration = value;
                    RaisePropertyChanged("IsGetRegistration");
                }
            }
        }

        //KMx: Mặc định = true, vì đa số đều phải load Patient (17/09/2014 15:32).
        private bool _isGetPatient = true;
        [DataMemberAttribute()]
        public bool IsGetPatient
        {
            get
            {
                return _isGetPatient;
            }
            set
            {
                if (_isGetPatient != value)
                {
                    _isGetPatient = value;
                    RaisePropertyChanged("IsGetPatient");
                }
            }
        }

        /// <summary>
        /// KMx: Load bill. (18/09/2014 16:08).
        /// </summary>
        private bool _isGetBillingInvoices;
        [DataMemberAttribute()]
        public bool IsGetBillingInvoices
        {
            get
            {
                return _isGetBillingInvoices;
            }
            set
            {
                if (_isGetBillingInvoices != value)
                {
                    _isGetBillingInvoices = value;
                    RaisePropertyChanged("IsGetBillingInvoices");
                }
            }
        }

        /// <summary>
        /// KMx: Load thông tin nhập viện và các khoa đã từng ở. (18/09/2014 16:08).
        /// </summary>
        private bool _isGetAdmissionInfo;
        [DataMemberAttribute()]
        public bool IsGetAdmissionInfo
        {
            get
            {
                return _isGetAdmissionInfo;
            }
            set
            {
                if (_isGetAdmissionInfo != value)
                {
                    _isGetAdmissionInfo = value;
                    RaisePropertyChanged("IsGetAdmissionInfo");
                }
            }
        }

        /// <summary>
        /// KMx: Load các giường mà bệnh nhân đã nằm (18/09/2014 16:08).
        /// </summary>
        private bool _isGetBedAllocations;
        [DataMemberAttribute()]
        public bool IsGetBedAllocations
        {
            get
            {
                return _isGetBedAllocations;
            }
            set
            {
                if (_isGetBedAllocations != value)
                {
                    _isGetBedAllocations = value;
                    RaisePropertyChanged("IsGetBedAllocations");
                }
            }
        }

        /// <summary>
        /// KMx: Load các dịch vụ (18/09/2014 16:08).
        /// </summary>
        private bool _isGetRegistrationDetails;
        [DataMemberAttribute()]
        public bool IsGetRegistrationDetails
        {
            get
            {
                return _isGetRegistrationDetails;
            }
            set
            {
                if (_isGetRegistrationDetails != value)
                {
                    _isGetRegistrationDetails = value;
                    RaisePropertyChanged("IsGetRegistrationDetails");
                }
            }
        }

        /// <summary>
        /// KMx: Load PCL (18/09/2014 16:08).
        /// </summary>
        private bool _isGetPCLRequests;
        [DataMemberAttribute()]
        public bool IsGetPCLRequests
        {
            get
            {
                return _isGetPCLRequests;
            }
            set
            {
                if (_isGetPCLRequests != value)
                {
                    _isGetPCLRequests = value;
                    RaisePropertyChanged("IsGetPCLRequests");
                }
            }
        }

        /// <summary>
        /// KMx: Load phiếu xuất ngoại trú (18/09/2014 16:08).
        /// </summary>
        private bool _isGetDrugInvoices;
        [DataMemberAttribute()]
        public bool IsGetDrugInvoices
        {
            get
            {
                return _isGetDrugInvoices;
            }
            set
            {
                if (_isGetDrugInvoices != value)
                {
                    _isGetDrugInvoices = value;
                    RaisePropertyChanged("IsGetDrugInvoices");
                }
            }
        }

        /// <summary>
        /// KMx: Load thông tin thanh toán (18/09/2014 16:08).
        /// </summary>
        private bool _isGetPatientTransactions;
        [DataMemberAttribute()]
        public bool IsGetPatientTransactions
        {
            get
            {
                return _isGetPatientTransactions;
            }
            set
            {
                if (_isGetPatientTransactions != value)
                {
                    _isGetPatientTransactions = value;
                    RaisePropertyChanged("IsGetPatientTransactions");
                }
            }
        }

        /// <summary>
        /// KMx: Load các lần tạm ứng (18/09/2014 16:08).
        /// </summary>
        private bool _isGetCashAdvances;
        [DataMemberAttribute()]
        public bool IsGetCashAdvances
        {
            get
            {
                return _isGetCashAdvances;
            }
            set
            {
                if (_isGetCashAdvances != value)
                {
                    _isGetCashAdvances = value;
                    RaisePropertyChanged("IsGetCashAdvances");
                }
            }
        }

        /// <summary>
        /// KMx: Load các phiếu xuất thuốc, y cụ, hóa chất của nội trú (18/09/2014 16:08).
        /// </summary>
        private bool _isGetDrugClinicDeptInvoices;
        [DataMemberAttribute()]
        public bool IsGetDrugClinicDeptInvoices
        {
            get
            {
                return _isGetDrugClinicDeptInvoices;
            }
            set
            {
                if (_isGetDrugClinicDeptInvoices != value)
                {
                    _isGetDrugClinicDeptInvoices = value;
                    RaisePropertyChanged("IsGetDrugClinicDeptInvoices");
                }
            }
        }

        private bool _IsGetPromoDiscountPrograms = false;
        [DataMemberAttribute]
        public bool IsGetPromoDiscountPrograms
        {
            get
            {
                return _IsGetPromoDiscountPrograms;
            }
            set
            {
                _IsGetPromoDiscountPrograms = value;
                RaisePropertyChanged("IsGetPromoDiscountPrograms");
            }
        }
        //▼===== 20191010 TTM: Lấy danh sách Hội chẩn.
        private bool _isGetDiagnosysConsultationSummary = false;
        [DataMemberAttribute()]
        public bool IsGetDiagnosysConsultationSummary
        {
            get
            {
                return _isGetDiagnosysConsultationSummary;
            }
            set
            {
                if (_isGetDiagnosysConsultationSummary != value)
                {
                    _isGetDiagnosysConsultationSummary = value;
                    RaisePropertyChanged("IsGetDiagnosysConsultationSummary");
                }
            }
        }
        //▲=====
    }
}