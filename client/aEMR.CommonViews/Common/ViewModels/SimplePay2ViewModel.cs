using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common.Utilities;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISimplePay2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SimplePay2ViewModel : ViewModelBase, ISimplePay2
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SimplePay2ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoadPaymentTypes());
                Coroutine.BeginExecute(LoadPaymentModes());
                Coroutine.BeginExecute(LoadCurrency());
                Coroutine.BeginExecute(LoadPatientPaymentAccounts());
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

        public object ObjectState { get; set; }

        public bool PayForSelectedItemOnly { get; set; }

        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.Z1242_G1_ThuTienVPhi;
            }
        }

        public override bool IsProcessing
        {
            get
            {
                return _isPaying;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_isPaying)
                {
                    return eHCMSResources.Z1243_G1_DangLuuTTinTinhTien;
                }
                return "";
            }
        }

        public IList<PatientRegistrationDetail> RegistrationDetails { get; set; }

        public IList<PatientPCLRequest> PclRequests { get; set; }

        public IList<OutwardDrugInvoice> DrugInvoices { get; set; }

        public long V_TradingPlaces { get; set; }

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
                NotifyWhenBusy();
            }
        }
        //private bool _canPayCmd;
        public bool CanPayCmd
        {
            get { return !IsPaying; }
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

        private void ResetPatientPayment()
        {
            CurrentPayment = new PatientTransactionPayment
            {
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PaymentMode = new Lookup() { LookupID = DefaultPaymentModeID },
                PaymentType = new Lookup() { LookupID = DefaultPaymentTypeID },
                Currency = new Lookup() { LookupID = DefaultCurrencyID },
                PtPmtAccID = 1,
                V_TradingPlaces = V_TradingPlaces
            };
            //CurrentPayment.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            //CurrentPayment.V_TradingPlaces = V_TradingPlaces;

            TotalPayForSelectedItem = CalcPatientPayment();
            if (PayForSelectedItemOnly)
            {
                CurrentPayment.PayAmount = TotalPayForSelectedItem;
            }
            else
            {
                CurrentPayment.PayAmount = Registration.PayableSum.TotalPaymentForTransaction - Registration.PayableSum.TotalPatientPaid + TotalPayForSelectedItem;
            }
            TotalPaySuggested = CurrentPayment.PayAmount;
        }

        /// <summary>
        /// Tinh tong so tien BN phai tra (cho cac item duoc chon)
        /// </summary>
        private decimal CalcPatientPayment()
        {
            //Tinh tong so tien benh nhan phai tra cho dang ky
            decimal payment = 0;
            if (Registration == null)
            {
                return payment;
            }
            if (RegistrationDetails != null)
            {
                foreach (var item in RegistrationDetails)
                {
                    if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        payment += CalPaymentForMedRegItem(item);
                    }
                }
            }
            if (PclRequests != null)
            {
                foreach (var request in PclRequests)
                {
                    if (request.PatientPCLRequestIndicators != null)
                    {
                        foreach (var item in request.PatientPCLRequestIndicators)
                        {
                            if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                            {
                                payment += CalPaymentForMedRegItem(item);
                            }
                        }
                    }
                }
            }

            if (DrugInvoices != null)
            {
                foreach (var invoice in DrugInvoices)
                {
                    if (invoice.ReturnID.GetValueOrDefault(0) <= 0)//Phieu xuat.
                    {
                        if (invoice.OutwardDrugs != null)
                        {
                            if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE
                                                || invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED)
                            {
                                if (invoice.PaidTime == null)//Chua tra tien
                                {
                                    foreach (var item in invoice.OutwardDrugs)
                                    {
                                        payment += item.TotalPatientPayment;
                                    }
                                }
                            }
                            else if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED
                                                || invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN)
                            {
                                if (invoice.RefundTime == null)//Chua hoan tien
                                {
                                    foreach (var item in invoice.OutwardDrugs)
                                    {
                                        payment += item.TotalPatientPayment;
                                    }
                                }
                            }
                        }

                    }
                    else //Phieu tra
                    {
                        if (invoice.OutwardDrugs != null
                            && invoice.PaidTime == null)
                        {
                            foreach (var item in invoice.OutwardDrugs)
                            {
                                payment -= item.TotalPatientPayment;
                            }
                        }
                    }
                }
            }
            //Chua tinh tien thuoc.
            return payment;
        }

        /// <summary>
        /// Tinh tien phai tra doi voi moi item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal CalPaymentForMedRegItem(MedRegItemBase item)
        {
            if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
            {
                if (item.PaidTime == null)//Chua tinh tien
                {
                    return item.TotalPatientPayment;
                }
            }
            //else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //{
            //    if (item.RefundTime == null)
            //    {
            //        return -item.TotalPatientPayment;
            //    }
            //}

            return 0;
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
                return (long)AllLookupValues.PaymentType.TRA_DU;
            }
        }
        public void SetRegistration(object registrationInfo)
        {
            if (registrationInfo != null)
            {
                Registration = registrationInfo as PatientRegistration;

                //Bat su kien khi thuoc tinh IsCheck thay doi => Tinh lai so tien.
                if (Registration.PatientRegistrationDetails != null)
                {
                    foreach (var item in Registration.PatientRegistrationDetails)
                    {
                        item.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(item_PropertyChanged).Handler;
                    }
                }
                if (Registration.PCLRequests != null)
                {
                    foreach (var request in Registration.PCLRequests)
                    {
                        if (request.PatientPCLRequestIndicators != null)
                        {
                            foreach (var item in request.PatientPCLRequestIndicators)
                            {
                                item.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(item_PropertyChanged).Handler;
                            }
                        }
                    }
                }
            }
            else
            {
                Registration = null;
                //TotalAmount = 0;
            }
        }

        public void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                CurrentPayment.PayAmount = CalcPatientPayment();
            }
        }

        private decimal _totalPayForSelectedItem;
        public decimal TotalPayForSelectedItem
        {
            get
            {
                return _totalPayForSelectedItem;
            }
            set
            {
                if (_totalPayForSelectedItem != value)
                {
                    _totalPayForSelectedItem = value;
                    NotifyOfPropertyChange(() => TotalPayForSelectedItem);
                }
            }
        }

        private decimal _totalPaySuggested;
        public decimal TotalPaySuggested
        {
            get
            {
                return _totalPaySuggested;
            }
            set
            {
                if (_totalPaySuggested != value)
                {
                    _totalPaySuggested = value;
                    NotifyOfPropertyChange(() => TotalPaySuggested);
                }
            }
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
        private bool _isSavingAndPaying;
        public bool IsSavingAndPaying
        {
            get
            {
                return _isSavingAndPaying;
            }
            set
            {
                _isSavingAndPaying = value;
                NotifyOfPropertyChange(() => IsSavingAndPaying);
            }
        }
        public void SaveAndPayCmd()
        {
            /*20170818 CMN: Checked that not working*/
            /*
            //Kiem tra hop le du lieu.
            ObservableCollection<ValidationResult> validationResults;
            if (!ValidatePaymentInfo(out validationResults))
            {
                var errorVm = Globals.GetViewModel<IValidationError>();
                errorVm.SetErrors(validationResults);
                Globals.ShowDialog(errorVm as Conductor<object>);

                return;
            }
            if (IsSavingAndPaying)
            {
                return;
            }
            //Bat dau luu va tinh tien.
            var t = new Thread(() =>
            {
                IsPaying = true;
               
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginSaveRegistrationAndPay(Registration, CurrentPayment,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                PatientTransactionPayment payment = null;
                                PatientRegistration savedRegistration = null;
                                List<OutwardDrugInvoice> invoiceList = null;
                                bool bOK = false;
                                try
                                {
                                    contract.EndSaveRegistrationAndPay(out savedRegistration, out payment, out invoiceList, asyncResult);
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                }
                                if (bOK)
                                {
                                    if (invoiceList != null && invoiceList.Count > 0)
                                    {
                                        //Thong bao cho ben thuoc Ny su dung.
                                        Globals.EventAggregator.Publish(new AddCompleted<List<OutwardDrugInvoice>>() { Item = invoiceList});
                                    }
                                    if (payment != null)
                                    {
                                        if (savedRegistration.PatientTransaction != null)
                                        {
                                            payment.PatientTransaction = savedRegistration.PatientTransaction;
                                            if (!payment.PatientTransaction.PtRegistrationID.HasValue)
                                            {
                                                payment.PatientTransaction.PtRegistrationID = 0;
                                            }
                                            payment.PatientTransaction.PatientRegistration = savedRegistration;
                                        }
                                    }

                                    Globals.EventAggregator.Publish(new SaveAndPayForRegistrationCompleted() { Payment = payment, RegistrationInfo = savedRegistration });
                                    TryClose();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsPaying = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
            */
        }
        public void StartCalculating()
        {
            ResetPatientPayment();
        }

        #region COROUTINES
        private IEnumerator<IResult> DoLoadRegistration(long regID)
        {
            var loadRegInfoTask = new LoadRegistrationInfoTask(regID, Registration.FindPatient);
            yield return loadRegInfoTask;

            if (loadRegInfoTask.Error == null)
            {
                SetRegistration(loadRegInfoTask.Registration);
                StartCalculating();
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            yield break;
        }

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
            var result = new YieldValidationResult();
            IEnumerator e = DoValidatePaymentInfo(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            //OK mới làm tiếp
            if (!result.IsValid)
            {
                yield break;
            }
            List<PatientRegistrationDetail> paidRegDetailsList = null;
            List<PatientPCLRequest> paidPclRequestList = null;
            List<OutwardDrugInvoice> paidDrugInvoiceList = null;
            List<OutwardDrugClinicDeptInvoice> paidMedItemList = null;
            List<OutwardDrugClinicDeptInvoice> paidChemicalItemList = null;

            if (RegistrationDetails != null)
            {
                paidRegDetailsList = RegistrationDetails.ToList();
            }

            if (PclRequests != null)
            {
                paidPclRequestList = PclRequests.ToList();
            }

            if (DrugInvoices != null)
            {
                paidDrugInvoiceList = DrugInvoices.ToList();
            }
            //Hiện tại chưa tính đăng ký nội trú.
            var payTask = new PayForRegistrationTask(Registration, CurrentPayment, paidRegDetailsList, paidPclRequestList
                , paidDrugInvoiceList, paidMedItemList, paidChemicalItemList, null);

            yield return payTask;
            if (payTask.Error == null)
            {
                if (payTask.PatientPayment != null)
                {
                    payTask.PatientPayment.PatientTransaction = payTask.PatientTransaction;
                }
                Globals.EventAggregator.Publish(new PayForRegistrationCompleted() { Payment = payTask.PatientPayment, Registration = payTask.Registration, ObjectState = ObjectState });
                TryClose();
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(payTask.Error) });
            }
        }

        MessageBoxTask msgTask;
        private IEnumerator<IResult> DoValidatePaymentInfo(YieldValidationResult result)
        {
            result.IsValid = false;

            if (CurrentPayment.PaymentType.LookupID < 0)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1176_G1_ChonLoaiTToan), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (CurrentPayment.PayAmount == 0)
            {
                if (TotalPaySuggested != 0)
                {
                    msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1206_G1_KgTheTToanChuaNhapTien), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
                else
                {
                    if (Registration.HealthInsurance == null)
                    {
                        msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1207_G1_BNTraDu), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                        yield return msgTask;
                        yield break;
                    }
                    else //Dang ky bao hiem.
                    {
                        if (!CurrentPayment.HiDelegation)
                        {
                            msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1207_G1_BNTraDu), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                            yield return msgTask;
                            yield break;
                        }
                    }
                }
            }

            if (TotalPaySuggested >= 0 && CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.HOAN_TIEN)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1208_G1_KgTheHoanTien), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
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
                //Hoàn tiền cho bệnh nhân.
                if (TotalPaySuggested > CurrentPayment.PayAmount)
                {
                    msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1210_G1_KgTheTraLaiBNSoTien, Math.Abs(TotalPaySuggested).ToString("N0")), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
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

                //BN trả tiền cho BV
                if (TotalPaySuggested > CurrentPayment.PayAmount)
                {
                    msgTask = new MessageBoxTask(eHCMSResources.Z1211_G1_SoTienNhapChuaDu, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
                if (TotalPaySuggested < CurrentPayment.PayAmount && CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.TRA_DU)
                {
                    msgTask = new MessageBoxTask(eHCMSResources.Z1212_G1_SoTienNhapVaoQuaNhieu, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }

            result.IsValid = true;
        }
        #endregion

        #region COMMANDS
        public void PayCmd()
        {
            if (CanPayCmd)
            {
                IsPaying = true;
                Coroutine.BeginExecute(DoPayForRegistration(), null, (o, e) => { IsPaying = false; });
            }
        }
        #endregion

        public void LoadRegistrationByID(long registrationID)
        {
            Coroutine.BeginExecute(DoLoadRegistration(registrationID));
        }
    }
}
