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
using aEMR.Common.Converters;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common.Utilities;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Infrastructure.GlobalFuncs;

/*
 * 20181105 #001 TTM:   Do RecalHIBenefit có khả năng sẽ throw lỗi nên phải bỏ try catch để chụp lỗi lại.
 * 20181113 #002 TNHX:  [BM0003235] Doesn't print PhieuChiDinh if Doctor already printed it and When call by IsConfirmHIView
 * 20181129 #004 TNHX:  [BM0003235] Doesn't print PhieuChiDinh if Doctor already printed it and When call by IsConfirmHIView
 * 20181212 #005 TNHX:  [BM0005404] Doesn't print PhieuChiDinh if Doctor already printed it 
 * 20181225 #006 TNHX:  [BM0005462] Re-make report PhieuChiDinh
 * 20190521 #007 TNHX:  [BM0006874] Using new event after paycomplete at ConfirmHIView
 * 20190812 #008 TTM:   Bổ sung thêm Enter là trả tiền cho SimplePay.
 * 20191025 #009 TBL:   BM 0018467: Thêm IsNotCheckInvalid để khỏi kiểm tra khoảng thời gian giữa 2 lần làm CLS được tính BHYT
 * 20200520 #010 TTM:   Bổ sung thêm QMS system vào hệ thống aEMR
 * 20210717 #011 TNHX:  Truyền loại hình thanh toán + mã code thanh toán online
 * 20230306 #012 QTD:   Thêm thông báo đăng ký đã quyết toán
 */
namespace aEMR.Common.ViewModels
{
   

    [Export(typeof(IComplicatedPay)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ComplicatedPayViewModel : ViewModelBase, IComplicatedPay
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ComplicatedPayViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //Cho phép trả tiền khi tính tiền = 0 (chỉ dùng cho bán thuốc theo toa).
            AllowZeroPayment = false;

            //Đăng ký: Nếu trả tiền ở tab "Các dịch vụ đã thanh toán tiền" thì = true.
            //Nếu Refundable = true và số tiền cần thanh toán <= 0 thì chỉ được chọn "Hoàn tiền", không được chọn "Trả đủ".
            //Trường hợp hủy dịch vụ được bảo hiểm trả 100% (hoàn tiền 0 đồng không được nên phải thêm biến này).
            Refundable = false;

            AllowZeroRefund = false;

            FormMode = PaymentFormMode.PAY;

            //if(!DesignerProperties.IsInDesignTool)
            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoaddAllSelectionRefValues());
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

        public bool IsSaveRegisDetailsThenPay { get; set; } = false;

        public IList<PatientRegistrationDetail> RegistrationDetails { get; set; }

        public IList<PatientPCLRequest> PclRequests { get; set; }

        public IList<OutwardDrugInvoice> DrugInvoices { get; set; }

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

        public bool AllowZeroPayment
        {
            get;
            set;
        }

        public bool Refundable
        {
            get;
            set;
        }

        public bool AllowZeroRefund
        {
            get;
            set;
        }

        public long V_TradingPlaces { get; set; }

        private bool _PayNewService = false;
        public bool PayNewService
        {
            get { return _PayNewService; }
            set
            {
                _PayNewService = value;
                NotifyOfPropertyChange(() => PayNewService);
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
                NotifyOfPropertyChange(() => FormMode);
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
                NotifyOfPropertyChange(() => CanPayCmd);
                NotifyWhenBusy();
            }
        }

        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.Z1244_G1_TinhTienVPhi;
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

        //private bool _canPayCmd;
        public bool CanPayCmd
        {
            get { return !IsPaying; }
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
        //20200222 TBL Mod TMV1: Cờ liệu trình
        private bool _IsProcess;
        public bool IsProcess
        {
            get { return _IsProcess; }
            set
            {
                if (_IsProcess != value)
                {
                    _IsProcess = value;
                    NotifyOfPropertyChange(() => IsProcess);
                }
            }
        }

        private void ResetPatientPayment()
        {
            if (Registration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.COMPLETED && V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC)
            {
                if (DrugInvoices.Where(x => x.PaidTime == null && (x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE || x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED)).SelectMany(x => x.OutwardDrugs).Any(x => x.HIAllowedPrice > 0 || x.OutHIRebate > 0))
                {
                    MessageBox.Show(eHCMSResources.K2860_G1_DKDaHTat, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    IsPayEnable = false;
                    return;
                }
            }
            //▼===== #012
            else if (Registration.IsSettlement)
            {
                MessageBox.Show("Đăng ký đã quyết toán", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                IsPayEnable = false;
                return;
            }
            //▲===== #012
            else if (Registration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.COMPLETED)
            {
                MessageBox.Show(eHCMSResources.K2860_G1_DKDaHTat, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                IsPayEnable = false;
                return;
            }
            if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN)
            {
                Registration = Registration.DeepCopy();
                //▼====== #001
                try
                {
                    if (!IsUpdateHisID)
                    {
                        Registration.RecalHIBenefit(Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !IsViewOnly);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▲====== #001
                DrugInvoices = Registration.DrugInvoices?.Where(x => x.PaidTime == null).ToList();
                if (!IsUpdateHisID && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
                {
                    Registration.ConfirmHIStaffID = int.MaxValue;
                    Registration.CalcPayableSum(Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward);
                    Registration.ConfirmHIStaffID = null;
                }
                else
                {
                    Registration.CalcPayableSum(Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward);
                }
            }
            else if ((V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC || V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY) && Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance
                && Registration.PatientTransaction != null
                && Registration.PatientTransaction.PatientTransactionDetails != null
                && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
            {
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0
                    && Registration.PatientTransaction.PatientCashAdvances != null
                    && Registration.PtInsuranceBenefit.GetValueOrDefault(0) == 0)
                {
                    Registration.PatientTransaction.PatientCashAdvances = Registration.PatientTransaction.PatientCashAdvances.Where(mCashAdvance => mCashAdvance.OutPatientCashAdvanceLinks != null
                        && mCashAdvance.OutPatientCashAdvanceLinks.Any(mLink => Registration.PatientTransaction.PatientTransactionDetails.Any(mTransactions => mTransactions.outiID.GetValueOrDefault(0) == 0 && mTransactions.TransItemID == mLink.TransItemID))).ToObservableCollection();
                }

                // TxD 20/11/2019: Because at this moment AllSaveRegistrationDetails excludes All RegistrationDetails with RecordState.DELETED and AllSaveRegistrationDetails is being referenced 
                //                 at a few places in PatientSummaryInfoV3ViewModel and this class SimplePayViewModel so to keep that INTENTION the following is ONLY to WORK AROUND the 
                //                 Problem of Items already SAVED BUT NOT YET PAID then get DELETED and because of AllSaveRegistrationDetails NOT INCLUDED in Registration.PatientRegistrationDetails
                //Registration.PatientRegistrationDetails = Registration.AllSaveRegistrationDetails.ToObservableCollection();
                // 1. Get the List of AllSaveRegistrationDetails as before
                List<PatientRegistrationDetail> theRegisDetailList = Registration.AllSaveRegistrationDetails.ToList();
                // 2. Then add to that List all Items WITH RecordState.DELETED
                theRegisDetailList.AddRange(Registration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList());
                // 3. Now Re-Assign the new List to PatientRegistrationDetails
                Registration.PatientRegistrationDetails = theRegisDetailList.ToObservableCollection();

                //foreach (var item in Registration.PCLRequests)
                //{
                //    item.PatientPCLRequestIndicators = Registration.AllSavePCLRequestDetails.Where(x => x.PatientPCLReqID == item.PatientPCLReqID).ToObservableCollection();
                //}
                //Registration.PatientTransaction.PatientTransactionDetails = Registration.PatientTransaction.PatientTransactionDetails.Where(x => x.Amount > 0).ToObservableCollection();

                Registration.CalcPayableSum(Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward);
                //▼===== 20191118 TTM: Vì chưa biết lý do gì mà phải set để loại bỏ toàn bộ các phiếu xuất có tình trạng là CANCELED (15003
                //                      Nhưng khi huỷ phiếu cần DrugInvoice có giá trị của OutwardDrugInvoice CANCELED để cập nhật refund time.
                if (!IsRefundFromPharmacy)
                {
                    Registration.DrugInvoices = Registration.DrugInvoices.Where(x => x.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED).ToObservableCollection();
                    DrugInvoices = Registration.DrugInvoices;
                }
                //▲=====
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC)
                {
                    PclRequests = null;
                }

                //20191210 TTM: DrugInvoices là biến input đưa vào CalcPatientPayment để tính tổng tiền bệnh nhân cần chi trả cho lần trả tiền đó.
                //          Do mình muốn quầy đăng ký không có thông tin tiền thuốc nên nếu V_TradingPlaces == Đăng ký thì loại bỏ thuốc ra khỏi việc tính toán tiền thuốc vào tổng bệnh nhân chi trả
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY)
                {
                    DrugInvoices = null;
                }
            }
            else if (Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance && V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC)
            {
                PclRequests = null;
            }
            else
            {
                Registration.CalcPayableSum(Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward);
            }

            CurrentPayment = new PatientTransactionPayment
            {
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PaymentMode = new Lookup() { LookupID = DefaultPaymentModeID },
                PaymentType = new Lookup() { LookupID = DefaultPaymentTypeID },
                Currency = new Lookup() { LookupID = DefaultCurrencyID },
                PtPmtAccID = 1,
                V_TradingPlaces = V_TradingPlaces
            };

            TotalPayForSelectedItem = CalcPatientPayment();

            // TxD 29/12/2031 BEGIN: Adjusting the following PayAmount according to V_TradingPlaces :
            // 1. AllLookupValues.V_TradingPlaces.NHA_THUOC 
            // 2. AllLookupValues.V_TradingPlaces.DANG_KY
            if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC)
            {
                CurrentPayment.PayAmount = TotalPayForSelectedItem + Registration.PayableSum.TotalAmtOutwardDrugInvoices - Registration.PayableSum.TotalPaidForOutwardDrugInvoices;
            }
            else if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY)
            {
                CurrentPayment.PayAmount = TotalPayForSelectedItem + Registration.PayableSum.TotalAmtRegDetailServices + Registration.PayableSum.TotalAmtRegPCLRequests
                                            - Registration.PayableSum.TotalPaidForRegDetailServices - Registration.PayableSum.TotalPaidForRegPCLRequests
                                            - Registration.PayableSum.TotalDiscountAmount;
            }
            else if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN)
            {
                var DrugPayAmount = TotalPayForSelectedItem + Registration.PayableSum.TotalAmtOutwardDrugInvoices - Registration.PayableSum.TotalPaidForOutwardDrugInvoices;
                var RegistrationPayAmount = Registration.PayableSum.TotalAmtRegDetailServices + Registration.PayableSum.TotalAmtRegPCLRequests
                                            - Registration.PayableSum.TotalPaidForRegDetailServices - Registration.PayableSum.TotalPaidForRegPCLRequests
                                            - Registration.PayableSum.TotalDiscountAmount;
                CurrentPayment.PayAmount = DrugPayAmount + RegistrationPayAmount;
            }
            else  // then Use the OLD method
            {
                CurrentPayment.PayAmount = Registration.PayableSum.TotalPaymentForTransaction - Registration.PayableSum.TotalPatientPaid + TotalPayForSelectedItem;
            }

            //if (Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance && V_TradingPlaces != (long)AllLookupValues.V_TradingPlaces.NHA_THUOC && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
            //{
            //    CurrentPayment.PayAmount -= Registration.PayableSum.TotalPatientCashAdvance;
            //}

            TotalPaySuggested = CurrentPayment.PayAmount;

            if (TotalPaySuggested == 0)
            {
                CurrentPayment.HiDelegation = true;
            }
            else
            {
                CurrentPayment.HiDelegation = false;
            }

            if (TotalPaySuggested < 0)
            {
                CurrentPayment.PaymentType = PaymentTypeList.Where(x => x.LookupID == (long)AllLookupValues.PaymentType.HOAN_TIEN).FirstOrDefault();
            }
        }

        /// <summary>
        /// Tinh tong so tien BN phai tra (cho cac item duoc chon)
        /// </summary>
        private decimal CalcPatientPayment()
        {
            //Tinh tong so tien benh nhan phai tra cho dang ky
            CurrentPayment.hasDetail = false;
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
                        CurrentPayment.hasDetail = true;
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
                                CurrentPayment.hasDetail = true;
                            }
                        }
                    }
                }
            }
            if (DrugInvoices != null)
            {
                //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
                bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;
                if (DrugInvoices.Where(x => x.OutwardDrugs != null && x.PaidTime == null).SelectMany(x => x.OutwardDrugs).Any(x => x.PaidTime == null) && Registration.PtInsuranceBenefit.GetValueOrDefault(0) == 0)
                {
                    payment = 0;
                }
                foreach (var invoice in DrugInvoices)
                {
                    if (invoice.ReturnID.GetValueOrDefault(0) <= 0)//Phieu xuat.
                    {
                        if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE
                            || invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED)
                        {
                            if (invoice.OutwardDrugs != null && invoice.PaidTime == null)
                            {

                                if (!onlyRoundResultForOutward)
                                {
                                    foreach (var item in invoice.OutwardDrugs)
                                    {
                                        payment += item.TotalPatientPayment;
                                        CurrentPayment.hasDetail = true;
                                    }
                                }
                                else
                                {
                                    decimal TotalHIPayment = 0;
                                    decimal TotalInvoicePrice = 0;
                                    foreach (var item in invoice.OutwardDrugs)
                                    {
                                        TotalHIPayment += item.TotalHIPayment;
                                        TotalInvoicePrice += item.TotalInvoicePrice;
                                        //payment += item.TotalPatientPayment;
                                        CurrentPayment.hasDetail = true;
                                    }

                                    payment += MathExt.Round(TotalInvoicePrice, Converters.MidpointRounding.AwayFromZero) - MathExt.Round(TotalHIPayment, Converters.MidpointRounding.AwayFromZero);
                                }
                            }
                        }
                        //else if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED
                        //    || invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN)
                        //{
                        //    if (invoice.OutwardDrugs != null
                        //                        && invoice.RefundTime == null)
                        //    {
                        //        foreach (var item in invoice.OutwardDrugs)
                        //        {
                        //            payment += item.TotalPatientPayment;
                        //        }
                        //    }
                        //} 
                    }
                    else //Phieu tra
                    {
                        //Khong can doan nay vi Qty cua phieu xuat da tru ra nhung thang duoc return roi
                        if (invoice.OutwardDrugs != null
                            && invoice.PaidTime == null)
                        {
                            if (!onlyRoundResultForOutward)
                            {
                                foreach (var item in invoice.OutwardDrugs)
                                {
                                    payment -= item.TotalPatientPayment;
                                }
                            }
                            else
                            {
                                decimal TotalHIPayment = 0;
                                decimal TotalInvoicePrice = 0;
                                foreach (var item in invoice.OutwardDrugs)
                                {
                                    TotalHIPayment -= item.TotalHIPayment;
                                    TotalInvoicePrice -= item.TotalInvoicePrice;
                                }

                                payment = MathExt.Round(TotalInvoicePrice, Converters.MidpointRounding.AwayFromZero) - MathExt.Round(TotalHIPayment, Converters.MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }
            }
            //Chua tinh tien thuoc.
            if (Registration.InPatientBillingInvoices != null && Registration.InPatientBillingInvoices.Count > 0
                && Registration.InPatientBillingInvoices.Any(x => x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI))
            {
                foreach (var aItem in Registration.InPatientBillingInvoices.Where(x => x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI).ToList())
                {
                    payment += CalPaymentForMedRegItem(aItem);
                    //CurrentPayment.hasDetail = true;
                }
            }
            return payment;
        }

        /// <summary>
        /// Tinh tien phai tra doi voi moi item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal CalPaymentForMedRegItem(MedRegItemBase item)
        {
            if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                if (item.PaidTime == null)//Chua tinh tien
                {
                    return item.TotalPatientPayment + item.DiscountAmt;
                }
            }
            return 0;
        }

        private decimal CalPaymentForMedRegItem(InPatientBillingInvoice item)
        {
            if (item.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
            {
                if (item.PaidTime == null)
                {
                    return item.TotalPatientPayment;
                }
                else if (item.RefundTime == null && item.RecordState == RecordState.DELETED)
                {
                    return -item.TotalPatientPayment;
                }
            }
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

        //public void SetRegistration(object registrationInfo)
        //{
        //    if (registrationInfo != null)
        //    {
        //        Registration = registrationInfo as PatientRegistration;

        //        //Bat su kien khi thuoc tinh IsCheck thay doi => Tinh lai so tien.
        //        if (Registration.PatientRegistrationDetails != null)
        //        {
        //            foreach (var item in Registration.PatientRegistrationDetails)
        //            {
        //                item.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(item_PropertyChanged).Handler;
        //            }
        //        }
        //        if (Registration.PCLRequests != null)
        //        {
        //            foreach (var request in Registration.PCLRequests)
        //            {
        //                if (request.PatientPCLRequestIndicators != null)
        //                {
        //                    foreach (var item in request.PatientPCLRequestIndicators)
        //                    {
        //                        item.PropertyChanged+=new WeakEventHandler<PropertyChangedEventArgs>(item_PropertyChanged).Handler;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        Registration = null;
        //        //TotalAmount = 0;
        //    }
        //}

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

        public void StartCalculating()
        {
            ResetPatientPayment();
        }

        private bool _IsEnablePatientPmtAcc = true;
        public bool IsEnablePatientPmtAcc
        {
            get
            {
                return _IsEnablePatientPmtAcc;
            }
            set
            {
                if (_IsEnablePatientPmtAcc == value)
                {
                    return;
                }
                _IsEnablePatientPmtAcc = value;
                NotifyOfPropertyChange(() => IsEnablePatientPmtAcc);
            }
        }

        private bool _IsNotCheckInvalid;
        public bool IsNotCheckInvalid
        {
            get { return _IsNotCheckInvalid; }
            set
            {
                _IsNotCheckInvalid = value;
                NotifyOfPropertyChange(() => IsNotCheckInvalid);
            }
        }

        #region COROUTINES

        private IEnumerator<IResult> LoaddAllSelectionRefValues()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_TYPE);
            yield return paymentTypeTask;
            PaymentTypeList = new ObservableCollection<Lookup>(paymentTypeTask.LookupList.Where(item => item.LookupID != 5000 && item.LookupID != 5002));

            var patientpaymentaccounts = new LoadPatientPaymentAccountListTask();
            yield return patientpaymentaccounts;
            PatientPaymentAccounts = patientpaymentaccounts.PatientPaymentAccountList;

            if (Registration != null
                && Registration.PtRegistrationID == 0
                && Registration.AppointmentID > 0
                && Registration.Appointment != null
                && Registration.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE
                && CurrentPayment != null)
            {
                CurrentPayment.PtPmtAccID = 4;
                IsEnablePatientPmtAcc = false;
            }
            else if (Registration != null
              && Registration.AppointmentID > 0
              && Registration.Appointment != null
              && Registration.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE
              && CurrentPayment != null
              && (((Registration.AllSavePCLRequestDetails != null && Registration.AllSavePCLRequestDetails.Any(x => x.PaidTime == null))
                      || (Registration.AllSaveRegistrationDetails != null && Registration.AllSaveRegistrationDetails.Any(x => x.PaidTime == null)))
                  && (Registration.AllSavePCLRequestDetails == null || !Registration.AllSavePCLRequestDetails.Any(x => x.PaidTime == null && x.AppointmentID.GetValueOrDefault(0) == 0))
                  && (Registration.AllSaveRegistrationDetails == null || !Registration.AllSaveRegistrationDetails.Any(x => x.PaidTime == null && x.AppointmentID.GetValueOrDefault(0) == 0))))
            {
                CurrentPayment.PtPmtAccID = 4;
                IsEnablePatientPmtAcc = false;
            }
            else if (Registration != null
                && Registration.Appointment != null
                && Registration.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE
                && Registration.Appointment.ClientContract != null
                && Registration.Appointment.ClientContract.IsPayAddingMoreSvs)
            {
                CurrentPayment.PtPmtAccID = 4;
                IsEnablePatientPmtAcc = false;
            }
            else if (Registration != null
                && (((Registration.AllCanceledPCLRequestDetails != null && Registration.AllCanceledPCLRequestDetails.Any(x => x.RefundTime == null))
                        || (RegistrationDetails != null && RegistrationDetails.Any(x => x.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null)))
                    && (Registration.AllCanceledPCLRequestDetails == null || !Registration.AllCanceledPCLRequestDetails.Any(x => x.RefundTime == null && x.AppointmentID.GetValueOrDefault(0) == 0))
                    && (RegistrationDetails == null || !RegistrationDetails.Any(x => x.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null && x.AppointmentID.GetValueOrDefault(0) == 0)))
                && Registration.AppointmentID > 0
                && Registration.Appointment != null
                && Registration.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE
                && CurrentPayment != null)
            {
                CurrentPayment.PtPmtAccID = 4;
                IsEnablePatientPmtAcc = false;
            }
            else
            {
                IsEnablePatientPmtAcc = true;
            }

            var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
            yield return paymentModeTask;
            PaymentModeList = paymentModeTask.LookupList;

            var currencyTask = new LoadLookupListTask(LookupValues.CURRENCY);
            yield return currencyTask;
            CurrencyList = currencyTask.LookupList;

            yield break;
        }

        private void PayForRegistrationGenTask(GenericCoRoutineTask genTask, object _regPaymtDet)
        {
            PayForRegistrationDetails regPaymtDet = (PayForRegistrationDetails)_regPaymtDet;

            PatientTransaction retPtTransaction = null;
            PatientTransactionPayment retPtTranPayment = null;
            List<PaymentAndReceipt> retPatientPaymentList = null;
            Exception Error = null;
            string ErrorMesage = "";

            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bActionCompleteOk = false;
                        contract.BeginPayForRegistration_V3(Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , Globals.DeptLocation.DeptLocationID, regPaymtDet._Apply15HIPercent, regPaymtDet.CurRegistration.PtRegistrationID
                            , regPaymtDet.CurRegistration.FindPatient
                            , regPaymtDet._paymentDetails, regPaymtDet._paidRegDetailsList, regPaymtDet._paidPclRequestList
                            , regPaymtDet._paidDrugInvoiceList, regPaymtDet._billingInvoiceList
                            , regPaymtDet.CurRegistration.PromoDiscountProgramObj
                            , regPaymtDet._checkBeforePay
                            , regPaymtDet.ConfirmHIStaffID
                            , regPaymtDet.OutputBalanceServicesXML
                            , regPaymtDet.IsReported
                            , regPaymtDet.IsUpdateHisID
                            , regPaymtDet.CurRegistration.HealthInsurance != null ? (long?)regPaymtDet.CurRegistration.HealthInsurance.HIID : null
                            , regPaymtDet.CurRegistration.PtInsuranceBenefit
                            , IsNotCheckInvalid
                            , null
                            , false, IsProcess
                            //▼====: #011
                            , regPaymtDet._paymentDetails.TranPaymtNote, regPaymtDet._paymentDetails.PaymentMode.LookupID
                            //▲====: #011
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    V_RegistrationError error = V_RegistrationError.mRefresh;
									string responseMsg = string.Empty;
									contract.EndPayForRegistration_V3(out retPtTransaction, out retPtTranPayment, out retPatientPaymentList, out error, out responseMsg, asyncResult);
									if (!String.IsNullOrEmpty(responseMsg))
									{
										Globals.ShowMessage(responseMsg, "Thông Báo");
									}
									if (error == V_RegistrationError.mRefresh)
                                    {
                                        ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                    }
                                    bActionCompleteOk = true;
                                    IsNotCheckInvalid = false;
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                    //MessageBox.Show(ex.Message);
                                    //▼===== #009
                                    //TBL: 19090601 là ID để xác định thông báo
                                    if (ex.Message.Contains("19090601") && MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                    {
                                        //20191025 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                                        IsNotCheckInvalid = true;
                                        IsPaying = false;
                                        PayCmd();
                                    }
                                    else if (!ex.Message.Contains("19090601"))
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                                    }
                                    //▲===== #009
                                }
                                finally
                                {
                                    genTask.AddResultObj(Error);
                                    genTask.AddResultObj(ErrorMesage);
                                    genTask.AddResultObj(retPtTransaction);
                                    genTask.AddResultObj(retPtTranPayment);
                                    genTask.AddResultObj(retPatientPaymentList);
                                    genTask.ActionComplete(bActionCompleteOk);
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Error = ex;
                    genTask.AddResultObj(Error);
                    genTask.ActionComplete(false);
                }
            });
            t.Start();

        }


        private void SaveThenPayForServicesAndPCLReqs_GenTask(GenericCoRoutineTask genTask, object _regPaymtDet)
        {
            PayForRegistrationDetails regPaymtDet = (PayForRegistrationDetails)_regPaymtDet;

            PatientTransaction retPtTransaction = null;
            PatientTransactionPayment retPtTranPayment = null;
            List<PaymentAndReceipt> retPatientPaymentList = null;

            List<PatientRegistrationDetail> regDetailsHaveBeenPaidFor = null;
            List<PatientPCLRequest> pclReqsHaveBeenPaidFor = null;

            Exception Error = null;
            string ErrorMesage = "";

            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        // TxD 26/04/2019: the purpose of adding ===> item.RecordState == RecordState.UNCHANGED && item.PaidTime == null
                        //                  so to PAY for Items that have been SAVED previously and now button 'Luu & Tra Tien' button is pressed
                        var newServiceList = regPaymtDet.CurRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED
                                                                                            || (item.RecordState == RecordState.UNCHANGED && item.PaidTime == null)).ToList();
                        var newPclRequestList = regPaymtDet.CurRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED
                                                                                            || (item.RecordState == RecordState.UNCHANGED && item.PaidTime == null)).ToList();
                        var deletedServiceList = regPaymtDet.CurRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();
                        var deletedPclRequestList = regPaymtDet.CurRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();
                        PatientRegistration savedRegisReturn = null;
                        var contract = serviceFactory.ServiceInstance;
                        bool bActionCompleteOk = false;

                        contract.BeginSaveThenPayForServicesAndPCLReqs(Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , Globals.DeptLocation.DeptLocationID, regPaymtDet._Apply15HIPercent, regPaymtDet.CurRegistration
                            , newServiceList, newPclRequestList, deletedServiceList, deletedPclRequestList, regPaymtDet._paymentDetails, regPaymtDet.CurRegistration.PromoDiscountProgramObj
                            , Globals.GetCurServerDateTime(), regPaymtDet._checkBeforePay
                            , regPaymtDet.ConfirmHIStaffID, regPaymtDet.OutputBalanceServicesXML, regPaymtDet.IsReported
                            , regPaymtDet.IsUpdateHisID
                            , regPaymtDet.CurRegistration.HealthInsurance != null ? (long?)regPaymtDet.CurRegistration.HealthInsurance.HIID : null
                            , regPaymtDet.CurRegistration.PtInsuranceBenefit, IsNotCheckInvalid, IsProcess
                            //▼====: #011
                            , CurrentPayment.TranPaymtNote, CurrentPayment.PaymentMode.LookupID
                            //▲====: #011
                            , null
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    V_RegistrationError RegisError = V_RegistrationError.mRefresh;
                                    V_RegistrationError PayError = V_RegistrationError.mRefresh;
									string responseMsg = string.Empty;
									long regId;
                                    savedRegisReturn = contract.EndSaveThenPayForServicesAndPCLReqs(
                                                out regId, out regDetailsHaveBeenPaidFor, out pclReqsHaveBeenPaidFor,
                                                out retPtTransaction, out retPtTranPayment, out retPatientPaymentList,
                                                out RegisError, out PayError, out responseMsg, asyncResult);
									if (!String.IsNullOrEmpty(responseMsg))
									{
										Globals.ShowMessage(responseMsg, "Thông Báo");
									}
									if (RegisError == V_RegistrationError.mRefresh)
                                    {
                                        ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                        bActionCompleteOk = false;
                                    }
                                    else
                                    {
                                        if (savedRegisReturn.PtRegistrationID <= 0)
                                        {
                                            savedRegisReturn.PtRegistrationID = regId;
                                        }
                                        else
                                        {
                                            //Apply quyen xoa tren danh sach registration details
                                            PermissionManager.ApplyPermissionToRegistration(savedRegisReturn);
                                        }
                                        Globals.EventAggregator.Publish(new SaveRegisFromSimplePayCompleted { RegistrationInfo = savedRegisReturn });
                                        Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = savedRegisReturn, RefreshItemFromReturnedObj = true });
                                        if (PayError == V_RegistrationError.mRefresh)
                                        {
                                            ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                            bActionCompleteOk = false;
                                        }
                                        else
                                        {
                                            bActionCompleteOk = true;
                                        }
                                    }
                                    IsNotCheckInvalid = false;
                                }
                                catch (Exception ex)
                                {
                                    bActionCompleteOk = false;
                                    Error = ex;
                                    //▼===== #009
                                    //TBL: 19090601 là ID để xác định thông báo
                                    if (ex.Message.Contains("19090601") && MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                    {
                                        //20191025 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                                        IsNotCheckInvalid = true;
                                        IsPaying = false;
                                        PayCmd();
                                    }
                                    else if (!ex.Message.Contains("19090601"))
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                                    }
                                    //▲===== #009
                                }
                                finally
                                {
                                    genTask.AddResultObj(Error);
                                    genTask.AddResultObj(ErrorMesage);
                                    genTask.AddResultObj(retPtTransaction);
                                    genTask.AddResultObj(retPtTranPayment);
                                    genTask.AddResultObj(retPatientPaymentList);
                                    genTask.AddResultObj(savedRegisReturn);
                                    genTask.AddResultObj(regDetailsHaveBeenPaidFor);
                                    genTask.AddResultObj(pclReqsHaveBeenPaidFor);

                                    genTask.ActionComplete(bActionCompleteOk);
                                    this.DlgHideBusyIndicator();
                                    if (!bActionCompleteOk)
                                    {
                                        TryClose();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Error = ex;
                    genTask.AddResultObj(Error);
                    genTask.ActionComplete(false);
                    MessageBox.Show(ex.Message, "");
                }
            });
            t.Start();

        }

        private string OutputBalanceServicesXML;
        private void ConfirmHIBenefitTask(GenericCoRoutineTask genTask, object _aRegistration)
        {
            this.DlgShowBusyIndicator();
            PatientRegistration aRegistration = _aRegistration as PatientRegistration;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalRegistrationHIBenefit(aRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID
                            , null
                            , aRegistration.PtInsuranceBenefit
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                OutputBalanceServicesXML = null;
                                try
                                {
                                    var retval = contract.EndRecalRegistrationHIBenefit(out OutputBalanceServicesXML, asyncResult);
                                    genTask.ActionComplete(true);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                }
            });
            t.Start();
        }
        public bool CallComfirmOnPay { get; set; }
        private IEnumerator<IResult> DoPayForRegistration()
        {
            if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN && CallComfirmOnPay)
            {
                if (Registration == null) yield break;
                if (Registration != null && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
                {
                    yield return GenericCoRoutineTask.StartTask(ConfirmHIBenefitTask, Registration);
                    //▼====:
                    //Reload Registration for correct paying
                    var loadRegTask = new LoadRegistrationInfoTask(Registration.PtRegistrationID, true);
                    yield return loadRegTask;
                    Registration = loadRegTask.Registration;
                    RegistrationDetails = Registration.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                    PclRequests = Registration.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                    DrugInvoices = Registration.DrugInvoices.Where(x => x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE).ToList();
                    StartCalculating();
                    //▲====:
                }
            }

            YieldValidationResult result = new YieldValidationResult();
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
            PayForRegistrationDetails payRegDetails = new PayForRegistrationDetails(Registration, CurrentPayment, paidRegDetailsList, paidPclRequestList
                , paidDrugInvoiceList, paidMedItemList, paidChemicalItemList, null, PayNewService, V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN ? Globals.LoggedUserAccount.Staff.StaffID : (long?)null, OutputBalanceServicesXML
                , IsReported
                , IsUpdateHisID
                , Registration.InPatientBillingInvoices == null || !Registration.InPatientBillingInvoices.Any(x => x.PaidTime == null || (x.RecordState == RecordState.DELETED && x.RefundTime == null)) ? null : Registration.InPatientBillingInvoices.Where(x => x.PaidTime == null || (x.RecordState == RecordState.DELETED && x.RefundTime == null)).ToList());

            //var payTask = new PayForRegistrationTask(Registration, CurrentPayment, paidRegDetailsList, paidPclRequestList
            //    , paidDrugInvoiceList, paidMedItemList, paidChemicalItemList, null, PayNewService, V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN ? Globals.LoggedUserAccount.Staff.StaffID : (long?)null, OutputBalanceServicesXML
            //    , IsReported
            //    , IsUpdateHisID);

            //yield return payTask;

            var payTask = new GenericCoRoutineTask(PayForRegistrationGenTask, payRegDetails);
            yield return payTask;

            Exception exError = payTask.GetResultObj(0) as Exception;
            string retErrorMesage = payTask.GetResultObj(1) as string;


            if (!string.IsNullOrEmpty(retErrorMesage))
            {
                var message = new MessageWarningShowDialogTask(retErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = Registration });

                TryClose();
                yield break;
            }

            if (exError == null)
            {
                PatientTransaction retPtTransaction = (PatientTransaction)payTask.GetResultObj(2);
                PatientTransactionPayment retPtTranPayment = (PatientTransactionPayment)payTask.GetResultObj(3);
                List<PaymentAndReceipt> retPatientPaymentList = (List<PaymentAndReceipt>)payTask.GetResultObj(4);

                if (retPtTransaction != null)
                {
                    retPtTranPayment.PatientTransaction = retPtTransaction;
                }
                var strServices = string.Empty;
                var sTemp = string.Empty;
                if (paidRegDetailsList != null && paidRegDetailsList.Count > 0)
                {
                    sTemp = string.Join(",", paidRegDetailsList.Select(item => item.ChargeableItemName));
                }
                strServices += sTemp;
                sTemp = string.Empty;
                if (paidPclRequestList != null && paidPclRequestList.Count > 0)
                {
                    sTemp = string.Join(",", paidPclRequestList.SelectMany(item => item.PatientPCLRequestIndicators.Where(obj => obj.PCLExamType != null))
                        .Select(reqDetail => reqDetail.PCLExamType.PCLExamTypeName));
                }
                if (!string.IsNullOrWhiteSpace(sTemp))
                {
                    if (strServices.Length > 0)
                    {
                        strServices = strServices + ",";
                    }
                    strServices += string.Format(" {0} (", eHCMSResources.G2600_G1_XN) + sTemp + ")";
                }
                sTemp = string.Empty;
                if (paidDrugInvoiceList != null && paidDrugInvoiceList.Count > 0)
                {
                    sTemp = string.Join(",", paidDrugInvoiceList.SelectMany(item => item.OutwardDrugs.Where(obj => obj.RefGenericDrugDetail != null))
                        .Select(reqDetail => reqDetail.RefGenericDrugDetail.BrandName));
                }
                if (!string.IsNullOrWhiteSpace(sTemp))
                {
                    if (strServices.Length > 0)
                    {
                        strServices = strServices + ",";
                    }
                    strServices += string.Format(" {0} (", eHCMSResources.G0787_G1_Thuoc) + sTemp + ")";
                }
                //▼====: #007
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN && Globals.ServerConfigSection.CommonItems.PrintingReceiptWithDrugBill)
                {
                    Globals.EventAggregator.Publish(new PayForRegistrationCompletedAtConfirmHIView() { Payment = retPtTranPayment, PaymentList = retPatientPaymentList, Registration = Registration, ObjectState = strServices, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });
                }
                else
                {
                    Globals.EventAggregator.Publish(new PayForRegistrationCompleted() { Payment = retPtTranPayment, PaymentList = retPatientPaymentList, Registration = Registration, ObjectState = strServices, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });
                }
                //▲====: #007

                //▼===== #010
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY && Registration.TicketIssue != null)
                {
                    if (Globals.ServerConfigSection.CommonItems.UpdateTicketStatusAfterRegister)
                    {
                        UpdateTicketStatusAfterRegister(Registration.TicketIssue.TicketNumberText, Registration.TicketIssue.TicketGetTime);
                    }

                    Globals.EventAggregator.Publish(new ItemSelected<TicketIssue> { Item = new TicketIssue() });
                }
                //▲===== #010
                // 20181005 TNHX: [BM0000034] Add Event for print PhieuChiDinh
                // 20181218 TNHX: [BM0005433] Add condition don't show PhieuChiDinh when "Hoan tien"
                if (!IsConfirmHIView)
                {
                    Globals.EventAggregator.Publish(new PhieuChiDinhForRegistrationCompleted() { Registration = Registration, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });
                }

                Globals.EventAggregator.Publish(new PhieuMienGiamForRegistrationCompleted() { Payment = retPtTranPayment, PaymentList = retPatientPaymentList, Registration = Registration, ObjectState = strServices, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });

                // TxD 30/12/2012: Comment the following publishing of Event Because It triggers the PatientRegistrationViewModel to 
                // OpenRegistration twice ie. The Above Event already trigger the same Action. NOT SURE IF this Event is required elsewhere TO BE MONITORED...
                //Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = payTask.Registration });

                TryClose();
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(payTask.Error) });
            }
        }


        private IEnumerator<IResult> DoPayForRegisNew_CombiningTwoStoreProcs()
        {
            if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN && CallComfirmOnPay)
            {
                if (Registration == null)
                    yield break;
                if (Registration != null && Registration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
                {
                    yield return GenericCoRoutineTask.StartTask(ConfirmHIBenefitTask, Registration);
                    //▼====:
                    //Reload Registration for correct paying
                    var loadRegTask = new LoadRegistrationInfoTask(Registration.PtRegistrationID, true);
                    yield return loadRegTask;
                    Registration = loadRegTask.Registration;
                    RegistrationDetails = Registration.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                    PclRequests = Registration.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                    DrugInvoices = Registration.DrugInvoices.Where(x => x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE).ToList();
                    StartCalculating();
                    //▲====:
                }
            }

            YieldValidationResult result = new YieldValidationResult();
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

            PayForRegistrationDetails payRegDetails = new PayForRegistrationDetails(Registration, CurrentPayment, paidRegDetailsList, paidPclRequestList
                , paidDrugInvoiceList, paidMedItemList, paidChemicalItemList, null, PayNewService
                , V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN ? Globals.LoggedUserAccount.Staff.StaffID : (long?)null
                , OutputBalanceServicesXML
                , IsReported
                , IsUpdateHisID);

            var payTask = new GenericCoRoutineTask(SaveThenPayForServicesAndPCLReqs_GenTask, payRegDetails);
            yield return payTask;

            Exception exError = payTask.GetResultObj(0) as Exception;
            string retErrorMesage = payTask.GetResultObj(1) as string;


            if (!string.IsNullOrEmpty(retErrorMesage))
            {
                var message = new MessageWarningShowDialogTask(retErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = Registration });

                TryClose();
                yield break;
            }

            if (exError == null)
            {
                PatientTransaction retPtTransaction = (PatientTransaction)payTask.GetResultObj(2);
                PatientTransactionPayment retPtTranPayment = (PatientTransactionPayment)payTask.GetResultObj(3);
                List<PaymentAndReceipt> retPatientPaymentList = (List<PaymentAndReceipt>)payTask.GetResultObj(4);
                PatientRegistration savedRegisReturn = (PatientRegistration)payTask.GetResultObj(5);

                List<PatientRegistrationDetail> lstRegDetailsJustBeenPaidFor = (List<PatientRegistrationDetail>)payTask.GetResultObj(6);
                List<PatientPCLRequest> lstPCLReqsJustBeenPaidFor = (List<PatientPCLRequest>)payTask.GetResultObj(7);

                paidRegDetailsList = lstRegDetailsJustBeenPaidFor;
                paidPclRequestList = lstPCLReqsJustBeenPaidFor;

                if (retPtTransaction != null)
                {
                    retPtTranPayment.PatientTransaction = retPtTransaction;
                }
                var strServices = string.Empty;
                var sTemp = string.Empty;
                if (paidRegDetailsList != null && paidRegDetailsList.Count > 0)
                {
                    sTemp = string.Join(",", paidRegDetailsList.Select(item => item.ChargeableItemName));
                }
                strServices += sTemp;
                sTemp = string.Empty;
                if (paidPclRequestList != null && paidPclRequestList.Count > 0)
                {
                    sTemp = string.Join(",", paidPclRequestList.SelectMany(item => item.PatientPCLRequestIndicators.Where(obj => obj.PCLExamType != null))
                        .Select(reqDetail => reqDetail.PCLExamType.PCLExamTypeName));
                }
                if (!string.IsNullOrWhiteSpace(sTemp))
                {
                    if (strServices.Length > 0)
                    {
                        strServices = strServices + ",";
                    }
                    strServices += string.Format(" {0} (", eHCMSResources.G2600_G1_XN) + sTemp + ")";
                }
                sTemp = string.Empty;
                if (paidDrugInvoiceList != null && paidDrugInvoiceList.Count > 0)
                {
                    sTemp = string.Join(",", paidDrugInvoiceList.SelectMany(item => item.OutwardDrugs.Where(obj => obj.RefGenericDrugDetail != null))
                        .Select(reqDetail => reqDetail.RefGenericDrugDetail.BrandName));
                }
                if (!string.IsNullOrWhiteSpace(sTemp))
                {
                    if (strServices.Length > 0)
                    {
                        strServices = strServices + ",";
                    }
                    strServices += string.Format(" {0} (", eHCMSResources.G0787_G1_Thuoc) + sTemp + ")";
                }

                Globals.EventAggregator.Publish(new PayForRegistrationCompleted() { RefreshItemFromReturnedObj = true, Payment = retPtTranPayment, PaymentList = retPatientPaymentList, Registration = savedRegisReturn, ObjectState = strServices, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });
                //▼===== #010
                if (V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.DANG_KY && Registration.TicketIssue != null)
                {
                    if (Globals.ServerConfigSection.CommonItems.UpdateTicketStatusAfterRegister)
                    {
                        UpdateTicketStatusAfterRegister(Registration.TicketIssue.TicketNumberText, Registration.TicketIssue.TicketGetTime);
                    }
                    Globals.EventAggregator.Publish(new ItemSelected<TicketIssue> { Item = new TicketIssue() });
                }
                //▲===== #010
                // 20181005 TNHX: [BM0000034] Add Event for print PhieuChiDinh
                // 20181218 TNHX: [BM0005433] Add condition don't show PhieuChiDinh when "Hoan tien"
                if (!IsConfirmHIView)
                {
                    // 20190522 TNHX: [BM0006500] Doesn't print Services on PhieuChiDinh when PrintingPhieuChiDinhForService = true
                    if (Globals.ServerConfigSection.CommonItems.PrintingPhieuChiDinhForService)
                    {
                        Globals.EventAggregator.Publish(new PhieuChiDinhForRegistrationCompleted() { Registration = savedRegisReturn, RegDetailsList = new List<PatientRegistrationDetail>(), PCLRequestList = paidPclRequestList });
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new PhieuChiDinhForRegistrationCompleted() { Registration = savedRegisReturn, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });
                    }
                }

                Globals.EventAggregator.Publish(new PhieuMienGiamForRegistrationCompleted() { Payment = retPtTranPayment, PaymentList = retPatientPaymentList, Registration = savedRegisReturn, ObjectState = strServices, RegDetailsList = paidRegDetailsList, PCLRequestList = paidPclRequestList });

                // TxD 30/12/2012: Comment the following publishing of Event Because It triggers the PatientRegistrationViewModel to 
                // OpenRegistration twice ie. The Above Event already trigger the same Action. NOT SURE IF this Event is required elsewhere TO BE MONITORED...
                //Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = payTask.Registration });

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
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1176_G1_ChonLoaiTToan), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (CurrentPayment.PayAmount == 0 && V_TradingPlaces == (long)AllLookupValues.V_TradingPlaces.NHA_THUOC)//&& !CurrentPayment.hasDetail)
            {
                if (TotalPaySuggested != 0)
                {
                    msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1206_G1_KgTheTToanChuaNhapTien), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
                else if (!AllowZeroPayment && !AllowZeroRefund)
                {
                    if (Registration.HealthInsurance == null)
                    {
                        msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1207_G1_BNTraDu), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                        yield return msgTask;
                        yield break;
                    }
                    else //Dang ky bao hiem.
                    {
                        if (!CurrentPayment.HiDelegation && !Refundable)
                        {
                            msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1207_G1_BNTraDu), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                            yield return msgTask;
                            yield break;
                        }
                    }
                }
            }

            if (AllowZeroRefund)
            {
                if (TotalPaySuggested != 0)
                {
                    msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1213_G1_KgTheHoanTien0Dong), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
                if (CurrentPayment.PayAmount != 0)
                {
                    msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1214_G1_HoanTien0DongNhapKhac0), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }

            if (TotalPaySuggested >= 0 && CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.HOAN_TIEN && !AllowZeroRefund)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1208_G1_KgTheHoanTien), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            //KMx: Trường hợp hủy dịch vụ (bên đăng ký) mà phải trả lại cho BN <= 0 đồng thì không được chọn trả đủ mà chỉ được hoàn tiền.
            if (TotalPaySuggested <= 0 && Refundable && CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.TRA_DU)
            {
                msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1207_G1_BNTraDu), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
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
                //if (TotalPaySuggested > CurrentPayment.PayAmount)
                //{
                //    msgTask = new MessageBoxTask("Không thể trả lại BN số tiền vượt quá " + Math.Abs(TotalPaySuggested).ToString("N0"), eHCMSResources.G0442_G1_TBao, MessageBoxOptions.Ok);
                //    yield return msgTask;
                //    yield break;
                //}
                if (TotalPaySuggested != CurrentPayment.PayAmount)
                {
                    msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1238_G1_SoTienTraLaiBN, Math.Abs(TotalPaySuggested).ToString("N0")), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }
            else
            {
                //Nếu số tiền là dương, mà loại thanh toán là hoàn tiền thì không được.
                //Nếu 
                if (CurrentPayment.PaymentType.LookupID == (long)AllLookupValues.PaymentType.HOAN_TIEN && !AllowZeroRefund)
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
            if (!CanPayCmd)
            {
                MessageBox.Show("CanPayCmd == FALSE ====> RETURN");
                return;
            }

            IsPaying = true;
            if (IsSaveRegisDetailsThenPay)
            {
                Coroutine.BeginExecute(DoPayForRegisNew_CombiningTwoStoreProcs(), null, (o, e) => { IsPaying = false; });
            }
            else
            {
                Coroutine.BeginExecute(DoPayForRegistration(), null, (o, e) => { IsPaying = false; });
            }

        }
        #endregion

        private bool _IsPayEnable = true;
        public bool IsPayEnable
        {
            get => _IsPayEnable; set
            {
                _IsPayEnable = value;
                NotifyOfPropertyChange(() => IsPayEnable);
            }
        }

        private bool _IsConfirmHIView = false;
        private bool _IsViewOnly = false;
        private bool _IsReported = false;
        private bool _IsUpdateHisID = false;

        public bool IsConfirmHIView
        {
            get => _IsConfirmHIView; set
            {
                _IsConfirmHIView = value;
                NotifyOfPropertyChange(() => IsConfirmHIView);
            }
        }

        public bool IsViewOnly
        {
            get => _IsViewOnly; set
            {
                _IsViewOnly = value;
                NotifyOfPropertyChange(() => IsViewOnly);
            }
        }

        public bool IsReported
        {
            get => _IsReported; set
            {
                _IsReported = value;
                NotifyOfPropertyChange(() => IsReported);
            }
        }

        public bool IsUpdateHisID
        {
            get => _IsUpdateHisID; set
            {
                _IsUpdateHisID = value;
                NotifyOfPropertyChange(() => IsUpdateHisID);
            }
        }

        private bool CheckHIExpired()
        {
            DateTime Now = Globals.GetCurServerDateTime().Date;

            if (Registration.HisID.GetValueOrDefault(0) > 0 && Registration.HealthInsurance != null
                && (Now < Registration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date || Now > Registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date))
            {
                return true;
            }
            return false;
        }

        private bool CheckHIServiceAndPCLNotPaidYet()
        {
            if (Registration == null)
            {
                return false;
            }

            if (Registration.PatientRegistrationDetails != null && Registration.PatientRegistrationDetails.Any(item => item.PaidTime == null && item.HIBenefit > 0))
            {
                return true;
            }

            if (Registration.PCLRequests != null)
            {
                foreach (var request in Registration.PCLRequests)
                {
                    if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Any(item => item.PaidTime == null && item.HIBenefit > 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void RecalServiceAndPCLWithoutHI()
        {
            if (Registration == null)
            {
                return;
            }

            if (Registration.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in Registration.PatientRegistrationDetails)
                {
                    if (regDetail.PaidTime == null && regDetail.HIBenefit > 0)
                    {
                        regDetail.GetItemPrice(Registration, Globals.GetCurServerDateTime());
                        regDetail.GetItemTotalPrice();
                    }
                }
            }

            if (Registration.PCLRequests != null)
            {
                foreach (var request in Registration.PCLRequests)
                {
                    if (request.RecordState == RecordState.DELETED || request.PatientPCLRequestIndicators == null)
                    {
                        continue;
                    }

                    foreach (PatientPCLRequestDetail pclDetail in request.PatientPCLRequestIndicators)
                    {
                        if (pclDetail.PaidTime == null && pclDetail.HIBenefit > 0)
                        {
                            pclDetail.GetItemPrice(Registration, Globals.GetCurServerDateTime());
                            pclDetail.GetItemTotalPrice();
                        }
                    }
                }
            }

            // TxD 02/04/2019: Commented out the following and REVIEW with Tan
            //TinhTongGiaTien();

            ////Refresh để tính lại tổng tiền trên từng phiếu (12/11/2014 14:38).

            //if (NewServiceContent != null && NewServiceContent.RegistrationDetails != null)
            //{
            //    NewServiceContent.CV_RegDetailItems.Refresh();
            //}

            //if (NewPclContent != null && NewPclContent.PtPclReqDetailItems != null)
            //{
            //    //NewPclContent.PtPclReqDetailItems.Refresh();
            //}

        }
        //▼====== #008
        System.Windows.Controls.Button btnPay { get; set; }
        public void btnPay_Loaded(object sender, RoutedEventArgs e)
        {
            btnPay = sender as System.Windows.Controls.Button;
            btnPay.Focus();
        }
        //public void btnPay_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        PayCmd();
        //    }
        //}

        public void btnPay_Click(object sender, object e)
        {
            PayCmd();
        }
        //▲====== #008
        private bool _IsRefundFromPharmacy = false;
        public bool IsRefundFromPharmacy 
        {
            get
            {
                return _IsRefundFromPharmacy;
            }
            set
            {
                if (_IsRefundFromPharmacy != value)
                {
                    _IsRefundFromPharmacy = value;
                }
                NotifyOfPropertyChange(() => IsRefundFromPharmacy);
            }
        }

        //▼===== #010
        #region TICKET
        private TicketIssue _TicketIssue;
        public TicketIssue TicketIssueObj
        {
            get
            {
                return _TicketIssue;
            }
            set
            {
                _TicketIssue = value;
                NotifyOfPropertyChange(() => TicketIssueObj);
            }
        }
        private void UpdateTicketStatusAfterRegister(string TicketNumberText, DateTime TicketGetTime)
        {

            var t = new Thread(() =>
            {
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateTicketStatusAfterRegister(TicketNumberText, TicketGetTime, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool IsOK = mContract.EndUpdateTicketStatusAfterRegister(asyncResult);
                                if (IsOK)
                                {
                                    TicketIssueObj.V_TicketStatus = (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS;                                   
                                    NotifyOfPropertyChange(() => TicketIssueObj);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    }
                }
            });
            t.Start();
        }
        #endregion
        //▲===== #010
    }
}

//public void LoadRegistrationByID(long registrationID)
//{
//    Coroutine.BeginExecute(DoLoadRegistration(registrationID));
//}

//private IEnumerator<IResult> DoLoadRegistration(long regID)
//{
//    var loadRegInfoTask = new LoadRegistrationInfoTask(regID, Registration.FindPatient);
//    yield return loadRegInfoTask;

//    if (loadRegInfoTask.Error == null)
//    {
//        SetRegistration(loadRegInfoTask.Registration);
//        StartCalculating();
//    }
//    else
//    {
//        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
//    }
//    yield break;
//}


//private IEnumerator<IResult> LoadPaymentTypes()
//{
//    var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_TYPE);
//    yield return paymentTypeTask;
//    //PaymentTypeList = paymentTypeTask.LookupList;
//    PaymentTypeList = new ObservableCollection<Lookup>(paymentTypeTask.LookupList.Where(item => item.LookupID != 5000 && item.LookupID != 5002));                
//    yield break;
//}

//private IEnumerator<IResult> LoadPaymentModes()
//{
//    var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
//    yield return paymentModeTask;
//    PaymentModeList = paymentModeTask.LookupList;
//    yield break;
//}

//private IEnumerator<IResult> LoadCurrency()
//{
//    var currencyTask = new LoadLookupListTask(LookupValues.CURRENCY);
//    yield return currencyTask;
//    CurrencyList = currencyTask.LookupList;
//    yield break;
//}

//private IEnumerator<IResult> LoadPatientPaymentAccounts()
//{
//    var patientpaymentaccounts = new LoadPatientPaymentAccountListTask();
//    yield return patientpaymentaccounts;
//    PatientPaymentAccounts = patientpaymentaccounts.PatientPaymentAccountList;
//    yield break;
//}


//public void SaveAndPayCmd()
//{
//    //Kiem tra hop le du lieu.
//    ObservableCollection<ValidationResult> validationResults;
//    if (!ValidatePaymentInfo(out validationResults))
//    {
//        var errorVm = Globals.GetViewModel<IValidationError>();
//        errorVm.SetErrors(validationResults);
//        Globals.ShowDialog(errorVm as Conductor<object>);

//        return;
//    }
//    if (IsSavingAndPaying)
//    {
//        return;            
//    }


//    //Bat dau luu va tinh tien.
//    var t = new Thread(() =>
//    {
//        IsPaying = true;
//        Globals.EventAggregator.Publish(new BusyEvent
//        {
//            IsBusy = true,
//            Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1118_G1_DangLuuDKVaTToan)
//        });
//        AxErrorEventArgs error = null;
//        try
//        {
//            using (var serviceFactory = new CommonServiceClient())
//            {
//                var contract = serviceFactory.ServiceInstance;

//                contract.BeginSaveRegistrationAndPay(Registration, CurrentPayment,
//                    Globals.DispatchCallback((asyncResult) =>
//                    {
//                        PatientTransactionPayment payment = null;
//                        PatientRegistration savedRegistration = null;
//                        List<OutwardDrugInvoice> invoiceList = null;
//                        bool bOK = false;
//                        try
//                        {
//                            contract.EndSaveRegistrationAndPay(out savedRegistration, out payment, out invoiceList, asyncResult);
//                            bOK = true;
//                        }
//                        catch (FaultException<AxException> fault)
//                        {
//                            bOK = false;
//                            error = new AxErrorEventArgs(fault);
//                        }
//                        catch (Exception ex)
//                        {
//                            bOK = false;
//                            error = new AxErrorEventArgs(ex);
//                        }
//                        if (bOK)
//                        {
//                            if (invoiceList != null && invoiceList.Count > 0)
//                            {
//                                //Thong bao cho ben thuoc Ny su dung.
//                                Globals.EventAggregator.Publish(new AddCompleted<List<OutwardDrugInvoice>>() { Item = invoiceList});
//                            }
//                            if (payment != null)
//                            {
//                                if (savedRegistration.PatientTransaction != null)
//                                {
//                                    payment.PatientTransaction = savedRegistration.PatientTransaction;
//                                    if (!payment.PatientTransaction.PtRegistrationID.HasValue)
//                                    {
//                                        payment.PatientTransaction.PtRegistrationID = 0;
//                                    }
//                                    payment.PatientTransaction.PatientRegistration = savedRegistration;
//                                }
//                            }

//                            Globals.EventAggregator.Publish(new SaveAndPayForRegistrationCompleted() { Payment = payment, RegistrationInfo = savedRegistration });                                    
//                            TryClose();
//                        }

//                    }), null);
//            }
//        }
//        catch (Exception ex)
//        {
//            error = new AxErrorEventArgs(ex);
//        }
//        finally
//        {
//            IsPaying = false;
//            Globals.IsBusy = false;
//        }
//        if (error != null)
//        {
//            Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
//        }
//    });
//    t.Start();
//}