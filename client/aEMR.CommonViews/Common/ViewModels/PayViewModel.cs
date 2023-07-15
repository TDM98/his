using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using System.Linq;
using eHCMSLanguage;
using System.Windows;

namespace eHCMS.Common.ViewModels
{
    [Export(typeof(IPay)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PayViewModel : Conductor<object>, IPay
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PayViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var regDetailsVm = Globals.GetViewModel<IRegistrationSummaryV2>();
            RegistrationInfoContent = regDetailsVm;
            RegistrationInfoContent.ShowButtonList = false;
            RegistrationInfoContent.ShowCheckBoxColumn = false;
            ActivateItem(regDetailsVm);

            //if(!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoadPaymentTypes());
                Coroutine.BeginExecute(LoadPaymentModes());
                Coroutine.BeginExecute(LoadCurrency());
            }
            ResetPatientPayment();
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

        private IRegistrationSummaryV2 _registrationInfoContent;
        public IRegistrationSummaryV2 RegistrationInfoContent
        {
            get { return _registrationInfoContent; }
            set
            {
                _registrationInfoContent = value;
                NotifyOfPropertyChange(()=>RegistrationInfoContent);
            }
        }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                _FindPatient = value;
                NotifyOfPropertyChange(() => FindPatient);
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

        private PaymentFormMode _formMode = PaymentFormMode.PAY;
        public PaymentFormMode FormMode
        {
            get
            {
                return _formMode;
            }
            set
            {
                _formMode = value;
                NotifyOfPropertyChange(()=>FormMode);
            }
        }
#region COROUTINES
        private IEnumerator<IResult> DoLoadRegistration(long regID)
        {
            var loadRegInfoTask = new LoadRegistrationInfoTask(regID, Registration.FindPatient);
            yield return loadRegInfoTask;

            if (loadRegInfoTask.Error == null)
            {
                SetRegistration(loadRegInfoTask.Registration); 
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred(){CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error)});
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
#endregion
        public void PayCmd()
        {
            //Tu tu se validate
            //if (!this.Validate())
            //{
            //    return;
            //}
            if (CurrentPayment.PayAmount <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1253_G1_SoTienNhapLonHon0,eHCMSResources.G0442_G1_TBao);
                //Thong bao so tien nhap > 0.
                return;
            }
            //Kiem tra tuy theo payment mode.
            if (CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.TRA_DU)
            {
                //Kiem tra so tien nguoi dung nhap vao phai bang tong so tien can phai tra.
                if (TotalAmount > CurrentPayment.PayAmount)
                {
                    //Thong bao loi tra chua du tien.
                    Globals.ShowMessage(eHCMSResources.Z1211_G1_SoTienNhapChuaDu, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (TotalAmount < CurrentPayment.PayAmount)
                {
                    //Thong bao dua nhieu tien qua.
                    Globals.ShowMessage(eHCMSResources.Z1254_G1_SoTienNhapQuaNhieu, eHCMSResources.G0442_G1_TBao);
                    return;
                }
            }
            
            if (!_IsPaying)
            {
                StartPaying();
            }
        }
        public void StartPaying()
        {
            //var t = new Thread(() =>
            //{
            //    IsPaying = true;
            //    Globals.EventAggregator.Publish(new BusyEvent
            //    {
            //        IsBusy = true,
            //        Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0118_G1_DangTinhTien)
            //    });
            //    AxErrorEventArgs error = null;
            //    try
            //    {
            //        using (var serviceFactory = new CommonServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            List<long> paidRegDetailsList = new List<long>();
            //            if (Registration.PatientRegistrationDetails != null)
            //            {
            //                foreach (var item in Registration.PatientRegistrationDetails)
            //                {
            //                    if (item.IsChecked)
            //                    {
            //                        if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
            //                        {
            //                            if (!item.PaidTime.HasValue)
            //                            {
            //                                paidRegDetailsList.Add(item.PtRegDetailID);
            //                            } 
            //                        }
            //                        else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //                        {
            //                            if (!item.RefundTime.HasValue)
            //                            {
            //                                paidRegDetailsList.Add(item.PtRegDetailID);
            //                            } 
            //                        }
            //                    }
            //                }
            //            }
            //            List<long> paidPclRequestDetailsList = new List<long>();
            //            if (Registration.PCLRequests != null)
            //            {
            //                foreach (var request in Registration.PCLRequests)
            //                {
            //                    if (request.PatientPCLRequestIndicators != null)
            //                    {
            //                        //if(request.IsChecked)
            //                        {
            //                            if (request.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
            //                            {
            //                                if (!request.PaidTime.HasValue)
            //                                {
            //                                    paidPclRequestDetailsList.Add(request.PatientPCLReqID);
            //                                }
            //                            }
            //                            else if (request.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //                            {
            //                                if (!request.RefundTime.HasValue)
            //                                {
            //                                    paidPclRequestDetailsList.Add(request.PatientPCLReqID);
            //                                }
            //                            }
            //                        }
            //                        //foreach (var item in request.PatientPCLRequestIndicators)
            //                        //{
            //                        //    if (item.IsChecked)
            //                        //    {
            //                        //        if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
            //                        //        {
            //                        //            if (!item.PaidTime.HasValue)
            //                        //            {
            //                        //                paidPclRequestDetailsList.Add(item.ID);
            //                        //            }
            //                        //        }
            //                        //        else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //                        //        {
            //                        //            if (!item.RefundTime.HasValue)
            //                        //            {
            //                        //                paidPclRequestDetailsList.Add(item.ID);
            //                        //            }
            //                        //        }
            //                        //    }
            //                        //}
            //                    }
            //                }
            //            }

            //            contract.BeginPayForRegistration(Registration.PtRegistrationID,CurrentPayment,paidRegDetailsList,paidPclRequestDetailsList,
            //                null,null,null,
            //                Globals.DispatchCallback((asyncResult) =>
            //                                             {
            //                                                 PatientPayment payment = null;
            //                                                 PatientTransaction tran = null;
            //                    bool bOK = false;
            //                    try
            //                    {
            //                        contract.EndPayForRegistration(out tran, out payment, asyncResult);
            //                        bOK = true;
            //                    }
            //                    catch (FaultException<AxException> fault)
            //                    {
            //                        bOK = false;
            //                        error = new AxErrorEventArgs(fault);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        bOK = false;
            //                        error = new AxErrorEventArgs(ex);
            //                    }
            //                    if(bOK)
            //                    {
            //                        if (payment != null)
            //                        {
            //                            payment.PatientTransaction = tran;
            //                            if (!payment.PatientTransaction.PtRegistrationID.HasValue)
            //                            {
            //                                payment.PatientTransaction.PtRegistrationID = 0;
            //                            }
            //                        }
            //                        Globals.EventAggregator.Publish(new PayForRegistrationCompleted() { Payment = payment});
            //                        TryClose();
            //                    }

            //                }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        error = new AxErrorEventArgs(ex);
            //    }
            //    finally
            //    {
            //        IsPaying = false;
            //        Globals.IsBusy = false;
            //    }
            //    if (error != null)
            //    {
            //        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
            //    }
            //});
            //t.Start();
        }
        private bool _IsPaying;
        public bool IsPaying
        {
            get
            {
                return _IsPaying;
            }
            set
            {
                _IsPaying = value;
                NotifyOfPropertyChange(()=> IsPaying);
            }
        }
        //private bool _canPayCmd;
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
                NotifyOfPropertyChange(()=>CurrencyList);
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
            CurrentPayment = new PatientTransactionPayment();
            CurrentPayment.PaymentMode = new Lookup() { LookupID = DefaultPaymentModeID };
            CurrentPayment.PaymentType = new Lookup() { LookupID = DefaultPaymentTypeID };
            CurrentPayment.Currency = new Lookup() { LookupID = DefaultCurrencyID };
            if (TotalAmount.HasValue)
            {
                //CurrentPayment.PayAmount = TotalAmount.Value;
                CurrentPayment.PayAmount = CalcPatientPayment();
            }
        }

        /// <summary>
        /// Tinh tong so tien BN phai tra (cho cac item duoc chon)
        /// </summary>
        private decimal CalcPatientPayment()
        {
            decimal payment = 0;
            if(Registration == null)
            {
                return payment;
            }
            if(Registration.PatientRegistrationDetails != null)
            {
                foreach (var item in Registration.PatientRegistrationDetails)
                {
                    payment += CalPaymentForMedRegItem(item);
                }
            }
            if(Registration.PCLRequests != null)
            {
                foreach (var request in Registration.PCLRequests)
                {
                    if(request.PatientPCLRequestIndicators != null)
                    {
                        foreach (var item in request.PatientPCLRequestIndicators)
                        {
                            payment += CalPaymentForMedRegItem(item);
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
            if (item.IsChecked)
            {
                if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
                {
                    if (item.PaidTime == null)//Chua tinh tien
                    {
                        return item.TotalPatientPayment;
                    }
                }
                else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                {
                    if (item.RefundTime == null)
                    {
                        return item.TotalPatientPayment;
                    }
                }
            }
            //if (item.IsChecked)
            //{
            //    if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM)
            //    {
            //        if (item.PaidTime == null)//Chua tinh tien
            //        {
            //            return item.TotalPatientPayment;
            //        }
            //    }
            //    else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //    {
            //        if (item.RefundTime == null)
            //        {
            //            return -item.TotalPatientPayment;
            //        }
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
                if (Registration.PayableSum != null)
                {
                    TotalAmount = Registration.PayableSum.TotalPatientRemainingOwed;
                }
                else
                {
                    TotalAmount = 0;
                }

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
                                item.PropertyChanged+=new WeakEventHandler<PropertyChangedEventArgs>(item_PropertyChanged).Handler;
                            }
                        }
                    }
                }

            }
            else
            {
                Registration = null;
                TotalAmount = 0;
            }

            RegistrationInfoContent.SetRegistration(Registration); 
        }
        
        public void LoadRegistration(long registrationID)
        {
            Coroutine.BeginExecute(DoLoadRegistration(registrationID));
        }

        public void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsChecked")
            {
                CurrentPayment.PayAmount = CalcPatientPayment();
            }
        }
        private decimal? _TotalAmount;
        public decimal? TotalAmount
        {
            get
            {
                return _TotalAmount;
            }
            set
            {
                if (_TotalAmount != value)
                {
                    _TotalAmount = value;
                    NotifyOfPropertyChange(()=>TotalAmount);
                    if (_TotalAmount.HasValue)
                    {
                        //CurrentPayment.PayAmount = _TotalAmount.Value;
                        CurrentPayment.PayAmount = CalcPatientPayment();
                    }
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
                if (TotalAmount > CurrentPayment.PayAmount)
                {
                    ValidationResult item = new ValidationResult(eHCMSResources.Z1211_G1_SoTienNhapChuaDu, new string[] { "TotalAmount" });
                    result.Add(item);

                    return false;
                }
                if (TotalAmount < CurrentPayment.PayAmount)
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
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1118_G1_DangLuuDKVaTToan)
                });
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
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
            */
        }
    }
}