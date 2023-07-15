using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
/*
 * 20180828 #001 TTM: Bổ sung Thông báo tạm ứng thành công khi thực hiện thành công tạm ứng cho bệnh nhân
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientPayBill)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientPayBillViewModel : Conductor<object>, IInPatientPayBill
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientPayBillViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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
        /// Tổng tiền đã hoàn lại cho bệnh nhân.
        /// </summary>
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

        /// <summary>
        /// Tổng số tiền còn dư lại của bệnh nhân.
        /// </summary>
        public decimal DebtRemaining
        {
            get
            {
                return _sumOfAdvance - _totalLiabilities - _TotalRefundMoney;
            }
        }

        /// <summary>
        /// Tổng tiền đề nghị bệnh nhân sẽ trả.
        /// </summary>
        private decimal _amountSuggestion;
        public decimal AmountSuggestion
        {
            get { return _amountSuggestion; }
            set
            {
                _amountSuggestion = value;
                NotifyOfPropertyChange(() => AmountSuggestion);
            }
        }

        /// <summary>
        /// Tổng tiền bệnh nhân sẽ trả.
        /// </summary>
        private decimal _payAmount;
        public decimal PayAmount
        {
            get { return _payAmount; }
            set
            {
                _payAmount = value;
                NotifyOfPropertyChange(() => PayAmount);
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


        private IList<InPatientBillingInvoice> _billingInvoices;

        public IList<InPatientBillingInvoice> BillingInvoices
        {
            get { return _billingInvoices; }
            set
            {
                _billingInvoices = value;
                NotifyOfPropertyChange(() => BillingInvoices);
            }
        }

        /// <summary>
        /// Tinh tong so tien BN phai tra (cho cac item duoc chon)
        /// </summary>
        private void CalcPatientPayment()
        {
            if (BillingInvoices == null || BillingInvoices.Count == 0)
            {
                AmountSuggestion = 0;
                PayAmount = 0;
            }
            else
            {
                AmountSuggestion = BillingInvoices.Sum(obj => obj.TotalPatientPayment - obj.TotalPatientPaid);
                PayAmount = AmountSuggestion;
            }
        }


        public void SetValues(PatientRegistration regInfo, IList<InPatientBillingInvoice> billingInvoiceList)
        {
            if (regInfo == null || regInfo.PtRegistrationID <= 0)
            {
                return;
            }

            Registration = regInfo;

            BillingInvoices = billingInvoiceList.Where(item => item.PaidTime == null && item.TotalPatientPayment > item.TotalPatientPaid).ToList();
        }

        public void CancelCmd()
        {
            TryClose();
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
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0767_G1_Msg_ErrLoadTTinTToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.Message);
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

        public void PatientPayForBill()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginInPatientPayForBill(Registration, BillingInvoices, PayAmount, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0),
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndInPatientPayForBill(asyncResult);
                                    Globals.EventAggregator.Publish(new LoadInPatientBillingInvoice { });
                                    //▼====== #001
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0996_G1_Msg_InfoTUOK);
                                    //▲====== #001
                                    TryClose();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        private bool ValidatePaymentInfo()
        {
            if (Registration == null || Registration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0411_G1_Msg_InfoChuaCoTTinDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (BillingInvoices == null || BillingInvoices.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0412_G1_Msg_InfoChuaCoTTinHDon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (PayAmount <= 0)
            {
                MessageBox.Show(eHCMSResources.Z1028_G1_SoTienTraLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (PayAmount != AmountSuggestion)
            {
                MessageBox.Show(eHCMSResources.A0990_G1_Msg_InfoTienTraBangTienDN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;            
            }


            return true;
        }


        #region COMMANDS
        public void PayCmd()
        {
            if (ValidatePaymentInfo())
            {
                PatientPayForBill();
            }
        }
        #endregion




    }
}