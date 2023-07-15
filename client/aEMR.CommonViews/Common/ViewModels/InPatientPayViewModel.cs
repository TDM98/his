using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Utilities;
using eHCMSLanguage;
using System.Windows;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientPay)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientPayViewModel : Conductor<object>, IInPatientPay
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientPayViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoadPaymentTypes());
                Coroutine.BeginExecute(LoadPaymentModes());
                Coroutine.BeginExecute(LoadCurrency());
                Coroutine.BeginExecute(LoadPatientPaymentAccounts());

                ResetPatientPayment();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private decimal _totalLiabilities;
        /// <summary>
        /// Tổng công nợ (Tính tổng tiền của những bill chưa tính tiền).
        /// </summary>
        public decimal TotalLiabilities
        {
            get { return _totalLiabilities; }
            set
            {
                _totalLiabilities = value;
                NotifyOfPropertyChange(() => TotalLiabilities);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }

        private decimal _minimumToPay;
        /// <summary>
        /// Số tiền ít nhất phải trả.
        /// Nếu có chọn bill thì số tiền này bằng với số tiền của bill
        /// Nếu không chọn bill thì số tiền này = 0
        /// </summary>
        public decimal MinimumToPay
        {
            get { return _minimumToPay; }
            set
            {
                _minimumToPay = value;
                NotifyOfPropertyChange(() => MinimumToPay);
            }
        }

        private decimal _sumOfAdvance;
        /// <summary>
        /// Cũng không hẳn là tiền ứng trước. (Tiền thanh toán cho những bill chưa QUYẾT TOÁN)
        /// </summary>
        public decimal SumOfAdvance
        {
            get { return _sumOfAdvance; }
            set
            {
                _sumOfAdvance = value;
                NotifyOfPropertyChange(() => SumOfAdvance);

                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }

        private decimal _sumOfPaidInvoices = 0;
        /// <summary>
        /// Tổng tiền trả đối với những bill ĐÃ THANH TOÁN nhưng CHƯA QUYẾT TOÁN
        /// </summary>
        public decimal SumOfPaidInvoices
        {
            get { return _sumOfPaidInvoices; }
            set
            {
                _sumOfPaidInvoices = value;
                NotifyOfPropertyChange(() => SumOfPaidInvoices);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }

        /// <summary>
        /// Tổng công nợ (Tính tổng tiền của những bill chưa tính tiền).
        /// </summary>
        public decimal DebtRemaining
        {
            get
            {
                return (_totalLiabilities + _TotalRefundMoney) - _sumOfAdvance;
            }
        }

        private PatientRegistration _registration;

        public PatientRegistration Registration
        {
            get { return _registration; }
            set
            {
                _registration = value;
                NotifyOfPropertyChange(() => Registration);
            }
        }

        private bool _isPaying;
        public bool IsPaying
        {
            get
            {
                return _isPaying;
            }
            set
            {
                _isPaying = value;
                NotifyOfPropertyChange(() => IsPaying);
            }
        }

        public bool CanPayCmd
        {
            get { return true; }
        }


        private ObservableCollection<Lookup> _paymentModeList;
        public ObservableCollection<Lookup> PaymentModeList
        {
            get { return _paymentModeList; }
            set
            {
                _paymentModeList = value;
                NotifyOfPropertyChange(() => PaymentModeList);
            }
        }

        private ObservableCollection<Lookup> _paymentTypeList;
        public ObservableCollection<Lookup> PaymentTypeList
        {
            get { return _paymentTypeList; }
            set
            {
                _paymentTypeList = value;
                NotifyOfPropertyChange(() => PaymentTypeList);
            }
        }

        private ObservableCollection<Lookup> _currencyList;
        public ObservableCollection<Lookup> CurrencyList
        {
            get { return _currencyList; }
            set
            {
                _currencyList = value;
                NotifyOfPropertyChange(() => CurrencyList);
            }
        }

        private PatientTransactionPayment _currentPayment;
        public PatientTransactionPayment CurrentPayment
        {
            get
            {
                return _currentPayment;
            }
            set
            {
                _currentPayment = value;
                NotifyOfPropertyChange(() => CurrentPayment);
            }
        }

        private ObservableCollection<PatientPaymentAccount> _PatientPaymentAccounts;
        public ObservableCollection<PatientPaymentAccount> PatientPaymentAccounts
        {
            get { return _PatientPaymentAccounts; }
            set
            {
                _PatientPaymentAccounts = value;
                NotifyOfPropertyChange(() => PatientPaymentAccounts);
            }
        }

        private void ResetPatientPayment()
        {
            CurrentPayment = new PatientTransactionPayment();
            CurrentPayment.PaymentMode = new Lookup() { LookupID = DefaultPaymentModeID };
            CurrentPayment.PaymentType = new Lookup() { LookupID = DefaultPaymentTypeID };
            CurrentPayment.Currency = new Lookup() { LookupID = DefaultCurrencyID };
            CurrentPayment.PtPmtAccID = 1;
            CurrentPayment.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
        }

        /// <summary>
        /// Tinh tong so tien BN phai tra (cho cac item duoc chon)
        /// </summary>
        private void CalcPatientPayment()
        {
            if (BillingInvoices.Count == 0)
            {
                MinimumToPay = 0;
            }
            else
            {
                MinimumToPay = BillingInvoices.Where(item => item.PaidTime == null).Sum(obj => obj.TotalPatientPayment);
            }

            //Truong hop so tien bn tra dang du ra:
            if (DebtRemaining < 0)
            {
                //MinimumToPay = MinimumToPay + DebtRemaining;
                //if (MinimumToPay < 0)
                //{
                //    MinimumToPay = 0;
                //}
                MinimumToPay = 0;
                CurrentPayment.PayAmount = MinimumToPay;
            }
            else //Truong hop bn con no
            {
                if (AutoPay)
                {
                    //KMx: Trước đây Set liên tục 2 lần. Thấy vô lý nên bỏ set đầu tiên ra (10/10/2014 14:20). 
                    //CurrentPayment.PayAmount = MinimumToPay;
                    CurrentPayment.PayAmount = DebtRemaining;
                }
                else
                {
                    CurrentPayment.PayAmount = MinimumToPay + DebtRemaining;

                    if (MinimumToPay > 0 && DebtRemaining > MinimumToPay)
                    {
                        CurrentPayment.PayAmount = MinimumToPay;
                    }
                    else
                    {
                        MinimumToPay = DebtRemaining;
                        CurrentPayment.PayAmount = DebtRemaining;
                    }
                }
            }


            //if (MinimumToPay > 0 && DebtRemaining > MinimumToPay)
            //{
            //    CurrentPayment.PayAmount = MinimumToPay;
            //}
            //else
            //{
            //    CurrentPayment.PayAmount = DebtRemaining;
            //}
        }

        public long DefaultCurrencyID
        {
            get
            {
                return (long)AllLookupValues.Currency.VND;
            }
        }

        public long DefaultPaymentModeID
        {
            get
            {
                return (long)AllLookupValues.PaymentMode.TIEN_MAT;
            }
        }

        public long DefaultPaymentTypeID
        {
            get
            {
                return (long)AllLookupValues.PaymentType.TAM_UNG;
            }
        }
        public void SetValues(PatientRegistration regInfo, IList<InPatientBillingInvoice> billingInvoiceList)
        {
            //Registration = regInfo;
            //Về server lấy thông tin công nợ. Nếu được thì mới tiếp tục làm.
            if (regInfo == null || regInfo.PtRegistrationID <= 0)
            {
                //DO NOTHING
                return;
            }

            Registration = regInfo;
            BillingInvoices = billingInvoiceList;
        }

        public void CancelCmd()
        {
            TryClose();
        }
        private bool ValidatePaymentInfo(out ObservableCollection<ValidationResult> result)
        {
            result = new ObservableCollection<ValidationResult>();

            if (CurrentPayment.PayAmount <= 0)
            {
                ValidationResult item = new ValidationResult(eHCMSResources.Z1253_G1_SoTienNhapLonHon0, new string[] { "PayAmount" });
                result.Add(item);

                return false;
            }
            //Kiem tra tuy theo payment mode.
            if (CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.TRA_DU)
            {
                //Kiem tra so tien nguoi dung nhap vao phai bang tong so tien can phai tra.
                if (CurrentPayment.PayAmount > CurrentPayment.PayAmount)
                {
                    ValidationResult item = new ValidationResult(eHCMSResources.Z1211_G1_SoTienNhapChuaDu, new string[] { "TotalAmount" });
                    result.Add(item);

                    return false;
                }
                if (CurrentPayment.PayAmount < CurrentPayment.PayAmount)
                {
                    ValidationResult item = new ValidationResult(eHCMSResources.Z1254_G1_SoTienNhapQuaNhieu, new string[] { "TotalAmount" });
                    result.Add(item);

                    return false;
                }
            }

            return true;
        }

        private decimal _TotalRefundMoney;
        public decimal TotalRefundMoney
        {
            get { return _TotalRefundMoney; }
            set
            {
                _TotalRefundMoney = value;
                NotifyOfPropertyChange(() => TotalRefundMoney);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }
        public void StartCalculating()
        {
            if (Registration == null || Registration.PtRegistrationID <= 0)
            {
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetInPatientRegistrationNonFinalizedLiabilities(Registration.PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                decimal liabilities;
                                decimal advance;
                                decimal totalPatientPayment_PaidInvoice;
                                decimal TotalRefundPatient;
                                try
                                {
                                    var bOK = contract.EndGetInPatientRegistrationNonFinalizedLiabilities(out liabilities, out advance, out totalPatientPayment_PaidInvoice, out TotalRefundPatient, asyncResult);

                                    if (bOK)
                                    {
                                        TotalLiabilities = liabilities;
                                        SumOfAdvance = advance;//tong so tien benh nhan ung
                                        SumOfPaidInvoices = totalPatientPayment_PaidInvoice;
                                        TotalRefundMoney = TotalRefundPatient;
                                        CalcPatientPayment();

                                    }
                                    else
                                    {
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                }
            });
            t.Start();
        }



        #region COROUTINES
        //private IEnumerator<IResult> DoLoadRegistration(long regID)
        //{
        //    var loadRegInfoTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
        //    yield return loadRegInfoTask;

        //    if (loadRegInfoTask.Error == null)
        //    {
        //        //SetRegistration(loadRegInfoTask.Registration);
        //        StartCalculating();
        //    }
        //    else
        //    {
        //        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
        //    }
        //    yield break;
        //}

        private IEnumerator<IResult> LoadPaymentTypes()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_TYPE);
            yield return paymentTypeTask;
            PaymentTypeList = paymentTypeTask.LookupList;
            yield break;
        }
        private IEnumerator<IResult> LoadPaymentModes()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
            yield return paymentModeTask;
            PaymentModeList = paymentModeTask.LookupList;
            yield break;
        }
        private IEnumerator<IResult> LoadCurrency()
        {
            var currencyTask = new LoadLookupListTask(LookupValues.CURRENCY);
            yield return currencyTask;
            CurrencyList = currencyTask.LookupList;
            yield break;
        }

        private IEnumerator<IResult> LoadPatientPaymentAccounts()
        {
            var patientpaymentaccounts = new LoadPatientPaymentAccountListTask();
            yield return patientpaymentaccounts;
            PatientPaymentAccounts = patientpaymentaccounts.PatientPaymentAccountList;
            yield break;
        }


        private IEnumerator<IResult> DoPayForRegistration()
        {
            YieldValidationResult result = new YieldValidationResult();
            List<InPatientBillingInvoice> paidBillingInvoiceList = new List<InPatientBillingInvoice>();
            IEnumerator e = DoValidatePaymentInfo(result, paidBillingInvoiceList);

            while (e.MoveNext())
                yield return e.Current as IResult;

            //OK mới làm tiếp
            if (!result.IsValid)
            {
                yield break;
            }

            var payTask = new PayForInPatientRegistrationTask(Registration, CurrentPayment, paidBillingInvoiceList, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0));

            yield return payTask;
            if (payTask.Error == null)
            {
                if (payTask.PatientPayment != null)
                {
                    payTask.PatientPayment.PatientTransaction = payTask.PatientTransaction;
                }
                Globals.EventAggregator.Publish(new PayForRegistrationCompleted
                    {
                        Payment = payTask.PatientPayment
                        ,
                        Registration = payTask.Registration
                        ,
                        ObjectState = payTask.CashAdvanceID
                    });
                TryClose();
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(payTask.Error) });
            }
        }
        MessageBoxTask msgTask;
        private IEnumerator<IResult> DoValidatePaymentInfo(YieldValidationResult result, List<InPatientBillingInvoice> paidInvoices)
        {
            result.IsValid = false;

            if (Registration == null)
            {
                msgTask = new MessageBoxTask(eHCMSResources.A0411_G1_Msg_InfoChuaCoTTinDK, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (BillingInvoices == null)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1175_G1_ChuaCoTTinBill), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (CurrentPayment.PaymentType.LookupID < 0)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1176_G1_ChonLoaiTToan), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (CurrentPayment.PayAmount < 0)
            {
                if (CurrentPayment.PaymentType.LookupID != (long)AllLookupValues.PaymentType.HOAN_TIEN)
                {
                    msgTask = new MessageBoxTask(eHCMSResources.K0343_G1_ChonLoaiTToanLaHTien, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }
            else
            {
                //Nếu số tiền là dương, mà loại thanh toán là hoàn tiền thì không được.
                if (CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.HOAN_TIEN)
                {
                    msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1178_G1_HoanTienNhapGTriAm), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }

            if (CurrentPayment.PayAmount >= MinimumToPay)
            {
                //Tra vo tu.
                paidInvoices.AddRange(BillingInvoices.Where(item => item.PaidTime == null));
                GetPayableBillingInvoiceList(Registration.InPatientBillingInvoices, paidInvoices, CurrentPayment.PayAmount);
            }
            else
            {
                //Sau nay se cho tra tien lai BN.
            }
            //if(MinimumToPay <= 0)
            //{
            //    if(DebtRemaining > 0)
            //    {
            //        if(CurrentPayment.PayAmount <= 0)
            //        {
            //            msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien), eHCMSResources.G0442_G1_TBao, MessageBoxOptions.Ok);
            //            yield return msgTask;
            //            yield break;
            //        }
            //        else
            //        {
            //            GetPayableBillingInvoiceList(Registration.InPatientBillingInvoices, paidInvoices, CurrentPayment.PayAmount);
            //        }
            //    }
            //    else
            //    {
            //        //Cho no tra luon.
            //        //msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1277_G1_KgCoGiDeTToan), eHCMSResources.G0442_G1_TBao, MessageBoxOptions.Ok);
            //        //yield return msgTask;
            //        //yield break;
            //        paidInvoices.AddRange(BillingInvoices.Where(item => item.PaidTime == null));
            //        GetPayableBillingInvoiceList(Registration.InPatientBillingInvoices, paidInvoices, CurrentPayment.PayAmount);
            //    }
            //}
            //else
            //{
            //    if (MinimumToPay > 0 && DebtRemaining > MinimumToPay)
            //    {
            //        CurrentPayment.PayAmount = MinimumToPay;
            //    }
            //    else
            //    {
            //        CurrentPayment.PayAmount = DebtRemaining;
            //    }

            //    if (CurrentPayment.PayAmount < MinimumToPay)
            //    {
            //        if(CurrentPayment.PayAmount < DebtRemaining)
            //        {
            //            msgTask = new MessageBoxTask("Phải nhập vào số tiền ít nhất là ." + DebtRemaining.ToString("N0"), eHCMSResources.G0442_G1_TBao, MessageBoxOptions.Ok);
            //            yield return msgTask;
            //            yield break;
            //        }
            //    }

            //    paidInvoices.AddRange(BillingInvoices.Where(item => item.PaidTime == null));
            //    decimal total = CurrentPayment.PayAmount - MinimumToPay;

            //    GetPayableBillingInvoiceList(Registration.InPatientBillingInvoices, paidInvoices, total);
            //}
            result.IsValid = true;
        }

        private void GetPayableBillingInvoiceList(IEnumerable<InPatientBillingInvoice> invList, List<InPatientBillingInvoice> destList, decimal totalPayment)
        {
            decimal totalPaymentSelected = 0;
            if (destList != null)
            {
                foreach (var inv in destList)
                {
                    if (inv.PaidTime == null)//Chua tra tien.
                    {
                        totalPaymentSelected += inv.TotalPatientPayment;
                    }
                }
            }

            CurrentPayment.PayAdvance = totalPaymentSelected;

            if (AutoPay)
            {
                totalPayment = totalPayment + (SumOfAdvance - SumOfPaidInvoices) - totalPaymentSelected; //Trong ngoặc là tổng số tiền bn trả dư đối với những bill ĐÃ THANH TOÁN nhưng CHƯA QUYẾT TOÁN
            }
            else
            {
                totalPayment = totalPayment - totalPaymentSelected;
            }
            if (totalPayment != 0)
            {
                foreach (var inv in invList)
                {
                    if (inv.PaidTime != null)//Đã tra tien.
                    {
                        continue;
                    }
                    if (destList.Contains(inv))//Đã co trong danh sach.
                    {
                        continue;
                    }

                    if (inv.TotalPatientPayment <= totalPayment)
                    {
                        destList.Add(inv);
                        //Ny them
                        CurrentPayment.PayAdvance += inv.TotalPatientPayment;

                        totalPayment -= inv.TotalPatientPayment;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region COMMANDS
        public void PayCmd()
        {
            Coroutine.BeginExecute(DoPayForRegistration());
        }
        #endregion


        //public void LoadRegistrationByID(long registrationID)
        //{
        //    Coroutine.BeginExecute(DoLoadRegistration(registrationID));
        //}

        public IList<InPatientBillingInvoice> BillingInvoices { get; private set; }

        public bool AutoPay { get; set; }
    }
}