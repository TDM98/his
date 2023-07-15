using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
/*
 * 20210717 #001 TNHX: Thêm mã code thanh toan toan online + mã phương thức thanh toán
 */
namespace aEMR.CommonTasks
{
    public class PayForRegistrationTask:IResult
    {
        public Exception Error { get; private set; }
        public string ErrorMesage { get; set; }
        private PatientTransaction _patientTransaction;

        /// <summary>
        /// Transaction kết quả sau khi thực hiện tính tiền.
        /// </summary>
        public  PatientTransaction PatientTransaction
        {
            get { return _patientTransaction; }
        }

        private PatientTransactionPayment _retPayment;
        /// <summary>
        /// Thông tin tính tiền do server trả về sau khi thực hiện xong.
        /// </summary>
        public PatientTransactionPayment PatientPayment
        {
            get { return _retPayment; }
        }

        private List<PaymentAndReceipt> _retPaymentList;
        /// <summary>
        /// Thông tin tính tiền do server trả về sau khi thực hiện xong.
        /// </summary>
        public List<PaymentAndReceipt> PatientPaymentList
        {
            get { return _retPaymentList; }
        }
        private readonly PatientRegistration _curRegistration;

        public PatientRegistration Registration
        {
            get
            {
                return _curRegistration;
            }
        }
        private PatientTransactionPayment _paymentDetails;
        
        /// <summary>
        /// Danh sách những chi tiết đăng ký muốn trả tiền.
        /// </summary>
        private List<PatientRegistrationDetail> _paidRegDetailsList;

        /// <summary>
        /// Danh sách những PCL Request muốn trả tiền.
        /// </summary>
        private List<PatientPCLRequest> _paidPclRequestList;
        private List<OutwardDrugInvoice> _paidDrugInvoiceList;
        private List<OutwardDrugClinicDeptInvoice> _paidMedItemList;
        private List<OutwardDrugClinicDeptInvoice> _paidChemicalItemList;
        private IList<InPatientBillingInvoice> _billingInvoiceList;
        private int? _Apply15HIPercent;
        private bool _checkBeforePay ;
        private long? ConfirmHIStaffID = null;
        private string OutputBalanceServicesXML;
        private bool IsReported = false;
        private bool IsUpdateHisID = false;

        public PayForRegistrationTask(PatientRegistration regInfo, PatientTransactionPayment paymentDetails,
                                List<PatientRegistrationDetail> paidRegDetailsList,
                                List<PatientPCLRequest> paidPclRequestList,
                                List<OutwardDrugInvoice> paidDrugInvoiceList,
                                IList<InPatientBillingInvoice> billingInvoiceList,int? Apply15HIPercent
            ,bool checkBeforePay=false)
        {
            _curRegistration = regInfo;

            _paymentDetails = paymentDetails;
            _paidRegDetailsList = paidRegDetailsList;
            _paidPclRequestList = paidPclRequestList;
            _paidDrugInvoiceList = paidDrugInvoiceList;
            _billingInvoiceList = billingInvoiceList;
            _Apply15HIPercent = Apply15HIPercent;
            _checkBeforePay = checkBeforePay;
        }

        public PayForRegistrationTask(PatientRegistration regInfo, PatientTransactionPayment paymentDetails,
                                        List<PatientRegistrationDetail> paidRegDetailsList,
                                        List<PatientPCLRequest> paidPclRequestList,
                                        List<OutwardDrugInvoice> paidDrugInvoiceList,
                                        List<OutwardDrugClinicDeptInvoice> paidMedItemList,
                                        List<OutwardDrugClinicDeptInvoice> paidChemicalItemList, int? Apply15HIPercent
            , bool checkBeforePay = false
            , long? ConfirmHIStaffID = null
            , string OutputBalanceServicesXML = null
            , bool aIsReported = false
            , bool aIsUpdateHisID = false)
        {
            _curRegistration = regInfo;

            _paymentDetails = paymentDetails;
            _paidRegDetailsList = paidRegDetailsList;
            _paidPclRequestList = paidPclRequestList;
            _paidDrugInvoiceList = paidDrugInvoiceList;
            _paidMedItemList = paidMedItemList;
            _paidChemicalItemList = paidChemicalItemList;
            _Apply15HIPercent = Apply15HIPercent;
            _checkBeforePay = checkBeforePay;
            this.ConfirmHIStaffID = ConfirmHIStaffID;
            this.OutputBalanceServicesXML = OutputBalanceServicesXML;
            IsReported = aIsReported;
            IsUpdateHisID = aIsUpdateHisID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPayForRegistration_V3(Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , Globals.DeptLocation.DeptLocationID, _Apply15HIPercent, _curRegistration.PtRegistrationID
                            , _curRegistration.FindPatient
                            , _paymentDetails,_paidRegDetailsList,_paidPclRequestList
                            , _paidDrugInvoiceList,_billingInvoiceList
                            , _curRegistration.PromoDiscountProgramObj
                            , _checkBeforePay
                            , ConfirmHIStaffID
                            , OutputBalanceServicesXML
                            , IsReported
                            , IsUpdateHisID
                            , _curRegistration.HealthInsurance != null ? (long?)_curRegistration.HealthInsurance.HIID : null
                            , _curRegistration.PtInsuranceBenefit
                            , false
                            , null
                            , false, false
                            //▼====: #001
                            , _paymentDetails.TranPaymtNote, _paymentDetails.PaymentMode.LookupID
                            //▲====: #001
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    V_RegistrationError error = V_RegistrationError.mRefresh;
									string responseMsg = string.Empty;
									contract.EndPayForRegistration_V3(out _patientTransaction, out _retPayment, out _retPaymentList, out error, out responseMsg, asyncResult);
									if (!String.IsNullOrEmpty(responseMsg))
									{
										Globals.ShowMessage(responseMsg, "Thông Báo");
									}
                                    if (error == V_RegistrationError.mRefresh)
                                    {
                                        ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = false
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
        
    }
}
