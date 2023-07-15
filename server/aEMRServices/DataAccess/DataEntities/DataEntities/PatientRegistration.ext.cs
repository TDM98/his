using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace DataEntities
{
    [KnownType(typeof(RefGenMedProductDetails))]
    [KnownType(typeof(RefGenMedDrugDetails))]
    public partial class PatientRegistration:IEditableObject
    {
     
        /// Convert the PatientRegistrationDetails collection to xml string in order to insert
        /// to database.
     
        /// <returns></returns>
        public string ConvertDetailsListToXml(IEnumerable<PatientRegistrationDetail> items)
        {
            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                       new XElement("PatientRegistrationDetails",
                       from details in items 
                       select new XElement("RecInfo",
                       new XElement("HisID", details.HisID),
                        new XElement("DeptLocID", details.DeptLocation != null && details.DeptLocation.DeptLocationID > 0 ? details.DeptLocation.DeptLocationID : (details.DeptLocID.HasValue ? details.DeptLocID : default(long?))),
                        new XElement("StaffID", details.StaffID),
                        new XElement("MedServiceID", details.RefMedicalServiceItem.MedServiceID),
                        new XElement("Price", Math.Abs(details.InvoicePrice)),
                        new XElement("HIAllowedPrice", MathExtensions.Abs(details.HIAllowedPrice)),
                        new XElement("ServiceQty", details.Qty),
                        new XElement("V_ExamRegStatus", (long)details.ExamRegStatus),
                        new XElement("MarkedAsDeleted", details.RecordState == RecordState.DELETED ? true : false),
                        new XElement("PtRegDetailID", (long)details.PtRegDetailID),
                        new XElement("CreatedDate", details.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                        new XElement("HIBenefit", details.HIBenefit),
                        new XElement("PaidTime", details.PaidTime.HasValue ? details.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                        new XElement("RefundTime", details.RefundTime.HasValue ? details.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                         new XElement("V_Ekip", details.V_Ekip != null ? details.V_Ekip.LookupID : 0),
                         new XElement("V_EkipIndex", details.V_EkipIndex != null ? details.V_EkipIndex.LookupID : 0)
                         , new XElement("UserOfficialAccountID", details.UserOfficialAccountID)
                         , new XElement("BedPatientID", details.BedPatientID)
                         , new XElement("OtherAmt", details.OtherAmt)
                         , new XElement("IsCountPatientCOVID", details.IsCountPatientCOVID)
                        )));
            return xmlDocument.ToString();
        }

        private PatientRegistration _tempRegistration;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRegistration = (PatientRegistration)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRegistration)
                CopyFrom(_tempRegistration);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientRegistration p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            PatientRegistration info = obj as PatientRegistration;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtRegistrationID == info.PtRegistrationID && this.DeptLocationName == info.DeptLocationName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private ObservableCollection<OutwardDrugInvoice> _DrugInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugInvoice> DrugInvoices
        {
            get
            {
                return _DrugInvoices;
            }
            set
            {
                _DrugInvoices = value;
                RaisePropertyChanged("DrugInvoices");
            }
        }

        

        private ObservableCollection<PatientPCLRequest> _PCLRequests;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequest> PCLRequests
        {
            get
            {
                return _PCLRequests;
            }
            set
            {
                if (_PCLRequests != value)
                {
                    _PCLRequests = value;
                    RaisePropertyChanged("PCLRequests"); 
                }
            }
        }

        /// <summary>
        /// Dung chung cho thuoc, y cu, hoa chat
        /// </summary>
        private ObservableCollection<OutwardDrugClinicDeptInvoice> _InPatientInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDeptInvoice> InPatientInvoices
        {
            get
            {
                return _InPatientInvoices;
            }
            set
            {
                _InPatientInvoices = value;
                RaisePropertyChanged("InPatientInvoices");
            }
        }

        [DataMemberAttribute()]
        public long? AppointmentID { get; set; }

        [DataMemberAttribute()]
        public DateTime? AppointmentDate { get; set; }

        private ObservableCollection<PatientPCLRequestDetail> _lstDetail_ReUseServiceSeqNum;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
        {
            get
            {
                return _lstDetail_ReUseServiceSeqNum;
            }
            set
            {
                if (_lstDetail_ReUseServiceSeqNum != value)
                {
                    _lstDetail_ReUseServiceSeqNum = value;
                    RaisePropertyChanged("lstDetail_ReUseServiceSeqNum");
                }
            }
        }

        //Cho dang ky noi tru
        private long _ReqFromDeptLocID;
        /// <summary>
        /// Noi de nghi lam phieu CLS
        /// </summary>
        [DataMemberAttribute()]
        public long ReqFromDeptLocID
        {
            get
            {
                return _ReqFromDeptLocID;
            }
            set
            {
                _ReqFromDeptLocID = value;
                RaisePropertyChanged("ReqFromDeptLocID");
            }
        }
        //Cho dang ky noi tru


        private ObservableCollection<PatientCashAdvance> _PatientCashAdvances;
        [DataMemberAttribute()]
        public ObservableCollection<PatientCashAdvance> PatientCashAdvances
        {
            get
            {
                return _PatientCashAdvances;
            }
            set
            {
                _PatientCashAdvances = value;
                RaisePropertyChanged("PatientCashAdvances");
            }
        }

        private ObservableCollection<RptPatientCashAdvReminder> _RptPatientCashAdvReminders;
        [DataMemberAttribute()]
        public ObservableCollection<RptPatientCashAdvReminder> RptPatientCashAdvReminders
        {
            get
            {
                return _RptPatientCashAdvReminders;
            }
            set
            {
                _RptPatientCashAdvReminders = value;
                RaisePropertyChanged("RptPatientCashAdvReminders");
            }
        }

        //Cho dang ky noi tru

        // The current admission
        [DataMemberAttribute()]
        public int InPtAdmissionStatus
        {
            get
            {
                return _InPtAdmissionStatus;
            }
            set
            {
                _InPtAdmissionStatus = value;
                switch (_InPtAdmissionStatus)
                {
                    case 0:
                        InPtAdmissionStatusText = "Chưa Xác Định";
                        break;
                    case 1:
                        InPtAdmissionStatusText = "Chờ Nhập Viện";
                        break;
                    case 2:
                        InPtAdmissionStatusText = "Nhập Viện";
                        break;
                    case 3:
                        InPtAdmissionStatusText = "Làm Giấy Xuất Viện";
                        break;
                    case 4:
                        InPtAdmissionStatusText = "Đã Xuất Viện";
                        break;
                    case 5:
                        InPtAdmissionStatusText = "Tạm - Xuất Viện";
                        break; 
                    case 6:
                        InPtAdmissionStatusText = "BN - Vãng Lai";
                        break; 
                    case 7: 
                        InPtAdmissionStatusText = "BN - Tiền Giải Phẫu";
                        break;
                    case 8: 
                        InPtAdmissionStatusText = "Đăng Ký Đã Hủy";
                        break;
                    default:
                        InPtAdmissionStatusText = "Error: Chưa Xác Định";
                        break;
                }
                RaisePropertyChanged("InPtAdmissionStatus");
            }
        }
        private int _InPtAdmissionStatus;

        [DataMemberAttribute()]
        public string InPtAdmissionStatusText
        {
            get
            {
                return _InPtAdmissionStatusText;
            }
            set
            {
                _InPtAdmissionStatusText = value;
                RaisePropertyChanged("InPtAdmissionStatusText");
            }
        }
        private string _InPtAdmissionStatusText;


        private DateTime? _DischargeDate;
        /// <summary>
        /// Noi de nghi lam phieu CLS
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                _DischargeDate = value;
                RaisePropertyChanged("DischargeDate");
            }
        }

        private DateTime? _TempDischargeDate;
        /// <summary>
        /// Noi de nghi lam phieu CLS
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? TempDischargeDate
        {
            get
            {
                return _TempDischargeDate;
            }
            set
            {
                _TempDischargeDate = value;
                RaisePropertyChanged("TempDischargeDate");
            }
        }

        private DateTime? _DischargeDetailRecCreatedDate;
        /// <summary>
        /// Noi de nghi lam phieu CLS
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? DischargeDetailRecCreatedDate
        {
            get
            {
                return _DischargeDetailRecCreatedDate;
            }
            set
            {
                _DischargeDetailRecCreatedDate = value;
                RaisePropertyChanged("DischargeDetailRecCreatedDate");
            }
        }

        private DateTime _RegCancelDate;
        [DataMemberAttribute()]
        public DateTime RegCancelDate
        {
            get
            {
                return _RegCancelDate;
            }
            set
            {
                _RegCancelDate = value;
                RaisePropertyChanged("RegCancelDate");
            }
        }

        private long _RegCancelStaffID;
        [DataMemberAttribute()]
        public long RegCancelStaffID
        {
            get
            {
                return _RegCancelStaffID;
            }
            set
            {
                _RegCancelStaffID = value;
                RaisePropertyChanged("RegCancelStaffID");
            }
        }

        #region Những Properties dưới đây sử dụng cho chức năng quyết toán ngoại trú không liên quan gì đến các chức năng khác.
        /// <summary>
        /// Những Properties dưới đây sử dụng cho chức năng quyết toán ngoại trú không liên quan gì đến các chức năng khác.
        /// </summary>

        private decimal _TotalAmountForSettlement;
        [DataMemberAttribute()]
        public decimal TotalAmountForSettlement
        {
            get
            {
                return _TotalAmountForSettlement;
            }
            set
            {
                _TotalAmountForSettlement = value;
                RaisePropertyChanged("TotalAmountForSettlement");
            }
        }
        private decimal _TotalHIPaymentForSettlement;
        [DataMemberAttribute()]
        public decimal TotalHIPaymentForSettlement
        {
            get
            {
                return _TotalHIPaymentForSettlement;
            }
            set
            {
                _TotalHIPaymentForSettlement = value;
                RaisePropertyChanged("TotalHIPaymentForSettlement");
            }
        }
        private decimal _TotalPatientPaymentForSettlement;
        [DataMemberAttribute()]
        public decimal TotalPatientPaymentForSettlement
        {
            get
            {
                return _TotalPatientPaymentForSettlement;
            }
            set
            {
                _TotalPatientPaymentForSettlement = value;
                RaisePropertyChanged("TotalPatientPaymentForSettlement");
            }
        }
        private decimal _TotalDiscountForSettlement;
        [DataMemberAttribute()]
        public decimal TotalDiscountForSettlement
        {
            get
            {
                return _TotalDiscountForSettlement;
            }
            set
            {
                _TotalDiscountForSettlement = value;
                RaisePropertyChanged("TotalDiscountForSettlement");
            }
        }
        private decimal _TotalPatientPaidForSettlement;
        [DataMemberAttribute()]
        public decimal TotalPatientPaidForSettlement
        {
            get
            {
                return _TotalPatientPaidForSettlement;
            }
            set
            {
                _TotalPatientPaidForSettlement = value;
                RaisePropertyChanged("TotalPatientPaidForSettlement");
            }
        }
        private bool _IsSettlement;
        [DataMemberAttribute()]
        public bool IsSettlement
        {
            get
            {
                return _IsSettlement;
            }
            set
            {
                _IsSettlement = value;
                RaisePropertyChanged("IsSettlement");
            }
        }
        #endregion
    }
}
