using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using System.Windows.Controls;
using Castle.Windsor;
using Caliburn.Micro;
/*
* 20170105 #001 CMN: Fix support amount
* 20170609 #002 CMN: Fix SupportFund About TT04 with TT04
* 20180309 #003 CMN: Removed default value for ObjV_CharityObjectType
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ICharitySupportFund)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CharitySupportFundViewModel : ViewModelBase, ICharitySupportFund
    {
        [ImportingConstructor]
        public CharitySupportFundViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
        }
        private ObservableCollection<CharitySupportFund> _SupportFunds;
        public ObservableCollection<CharitySupportFund> SupportFunds
        {
            get
            {
                return _SupportFunds;
            }
            set
            {
                _SupportFunds = value;
                NotifyOfPropertyChange(() => SupportFunds);
            }
        }

        private ObservableCollection<CharitySupportFund> _AllSupportFunds;
        public ObservableCollection<CharitySupportFund> AllSupportFunds
        {
            get
            {
                return _AllSupportFunds;
            }
            set
            {
                _AllSupportFunds = value;
                NotifyOfPropertyChange(() => AllSupportFunds);
            }
        }

        private CharitySupportFund _CurrentFund;
        public CharitySupportFund CurrentFund
        {
            get
            {
                return _CurrentFund;
            }
            set
            {
                _CurrentFund = value;
                NotifyOfPropertyChange(() => CurrentFund);
            }
        }

        private ObservableCollection<CharityOrganization> _ListOrganization;
        public ObservableCollection<CharityOrganization> ListOrganization
        {
            get
            {
                return _ListOrganization;
            }
            set
            {
                if (_ListOrganization != value)
                {
                    _ListOrganization = value;
                    NotifyOfPropertyChange(() => ListOrganization);
                }
            }
        }
        /*TMA 13/11/2017 : theo yêu cầu của Mr Công : Chỉ hiển thị khi tổ chức là Viện Tim*/
        public bool IsReason
        {
            get
            {
                if (SelectedOrganization != null && SelectedOrganization.CharityOrgID == 2)
                    return true;
                else
                    return false;
            } // Mr Công yêu cầu gán trực tiếp ID Viện Tim =2 
        }
        /*TMA 13/11/2017*/
        private CharityOrganization _SelectedOrganization = null;
        public CharityOrganization SelectedOrganization
        {
            get
            {
                return _SelectedOrganization;
            }
            set
            {
                _SelectedOrganization = value;
                NotifyOfPropertyChange(() => IsReason);
                NotifyOfPropertyChange(() => SelectedOrganization);
            }
        }

        public void GetAllCharityOrganization()
        {
            //20190404 TBL: Khi vao man hinh se lay chu khong lay tu Globals nua vi khi them to chuc phai tat mo chuong trinh lam mat thoi gian.
            //if (Globals.CharityOrganization != null)
            //{
            //    ListOrganization = Globals.CharityOrganization;
            //    return;
            //}
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0128_G1_DangLayDSToChucTuThien)
                });
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllCharityOrganization(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allOrganization = contract.EndGetAllCharityOrganization(asyncResult);
                                    if (allOrganization != null)
                                    {
                                        ListOrganization = new ObservableCollection<CharityOrganization>(allOrganization);
                                        Globals.CharityOrganization = ListOrganization;
                                    }
                                    else
                                    {
                                        ListOrganization = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        private ObservableCollection<CharitySupportFund> _BackupSupportFunds;
        public ObservableCollection<CharitySupportFund> BackupSupportFunds
        {
            get
            {
                return _BackupSupportFunds;
            }
            set
            {
                _BackupSupportFunds = value;
                NotifyOfPropertyChange(() => BackupSupportFunds);
            }
        }

        private InPatientBillingInvoice _SelectedBillingInv;
        public InPatientBillingInvoice SelectedBillingInv
        {
            get
            {
                return _SelectedBillingInv;
            }
            set
            {
                _SelectedBillingInv = value;
                NotifyOfPropertyChange(() => SelectedBillingInv);
            }
        }

        public long? InPatientBillingInvID
        {
            get
            {
                if (IsHighTechServiceBill == true && SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
                {
                    return SelectedBillingInv.InPatientBillingInvID;
                }
                return null;
            }
        }

        public string BillingInvNum
        {
            get
            {
                if (IsHighTechServiceBill == true && SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
                {
                    return SelectedBillingInv.BillingInvNum;
                }
                return null;
            }
        }


        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }

        public void SetSupportFunds()
        {
            if (AllSupportFunds == null || AllSupportFunds.Count() <= 0)
            {
                SupportFunds = new ObservableCollection<CharitySupportFund>();
            }
            if (IsHighTechServiceBill == true)
            {
                /*▼====: #002*/
                //SupportFunds = AllSupportFunds.Where(x => x.BillingInvID > 0).ToObservableCollection();
                SupportFunds = AllSupportFunds.Where(x => x.BillingInvID > 0 || x.IsHighTechServiceBill).ToObservableCollection();
                /*▲====: #002*/
            }
            else
            {
                SupportFunds = AllSupportFunds.Where(x => x.BillingInvID <= 0).ToObservableCollection();
            }
            BackupSupportFunds = SupportFunds.DeepCopy();
            if (IsHighTechServiceBill == true && SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
            {
                SelectedBillingInv.TotalSupportFund = SupportFunds.Sum(x=>x.AmountValue);
            }
            if (!IsHighTechServiceBill == true && BillingInvoiceListingContent != null && BillingInvoiceListingContent.BillingInvoices != null && BillingInvoiceListingContent.BillingInvoices.Count() > 0)
            {
                BillingInvoiceListingContent.CurrentSupportFund = SupportFunds;
                BillingInvoiceListingContent.SupportFund_ForHighTechServiceBill = AllSupportFunds.Where(x => x.BillingInvID > 0 || x.IsHighTechServiceBill).ToObservableCollection();
            }
        }

        public void GetCharitySupportFunds()
        {
            RefreshFundInfo();
            if (PtRegistrationID <= 0)
            {
                return;
            }       
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0129_G1_DangLayDSQuyHTChoBN)
                });
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        /*▼====: #002*/
                        //contract.BeginGetCharitySupportFundForInPt(PtRegistrationID, InPatientBillingInvID.GetValueOrDefault(),
                        contract.BeginGetCharitySupportFundForInPt_V2(PtRegistrationID, InPatientBillingInvID.GetValueOrDefault(), IsHighTechServiceBill,
                        /*▲====: #002*/
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allOrganization = contract.EndGetCharitySupportFundForInPt_V2(asyncResult);
                                    if (allOrganization != null)
                                    {
                                        AllSupportFunds = new ObservableCollection<CharitySupportFund>(allOrganization);
                                        SetSupportFunds();
                                    }
                                    else
                                    {
                                        SupportFunds = null;
                                        BackupSupportFunds = null;
                                    }
                                    CurrentFund = new CharitySupportFund();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        

        private bool _IsUsedPercent = true;
        public bool IsUsedPercent
        {
            get
            {
                return _IsUsedPercent;
            }
            set
            {
                _IsUsedPercent = value;
                if (CurrentFund != null)
                {
                    CurrentFund.IsUsedPercent = _IsUsedPercent;
                }
                NotifyOfPropertyChange(() => IsUsedPercent);
                NotifyOfPropertyChange(() => IsUsedAmount);
            }
        }

        public bool IsUsedAmount
        {
            get
            {
                return !IsUsedPercent;
            }
        }

        private bool? _IsHighTechServiceBill = null;
        public bool? IsHighTechServiceBill
        {
            get
            {
                return _IsHighTechServiceBill;
            }
            set
            {
                _IsHighTechServiceBill = value;
                NotifyOfPropertyChange(() => IsHighTechServiceBill);
            }
        }

        private InPatientBillingInvoice _BillingInvoice;
        public InPatientBillingInvoice BillingInvoice
        {
            get
            {
                return _BillingInvoice;
            }
            set
            {
                _BillingInvoice = value;
                NotifyOfPropertyChange(() => BillingInvoice);
            }
        }

        private IInPatientBillingInvoiceListingNew _OldBillingInvoiceContent;
        public IInPatientBillingInvoiceListingNew OldBillingInvoiceContent
        {
            get { return _OldBillingInvoiceContent; }
            set
            {
                _OldBillingInvoiceContent = value;
                NotifyOfPropertyChange(() => OldBillingInvoiceContent);
            }
        }

        private IInPatientBillingInvoiceListingNew _billingInvoiceListingContent;
        public IInPatientBillingInvoiceListingNew BillingInvoiceListingContent
        {
            get { return _billingInvoiceListingContent; }
            set
            {
                _billingInvoiceListingContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingContent);
            }
        }

        private decimal _TotalPatientPaidToFinalized;
        public decimal TotalPatientPaidToFinalized
        {
            get
            {
                return _TotalPatientPaidToFinalized;
            }
            set
            {
                _TotalPatientPaidToFinalized = value;
                NotifyOfPropertyChange(() => TotalPatientPaidToFinalized);
            }
        }

        //private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                if (IsHighTechServiceBill == true && SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
                {
                    return SelectedBillingInv.TotalPatientPayment;
                }
                /*▼====: #002*/
                //if (!IsHighTechServiceBill && BillingInvoiceListingContent != null && BillingInvoiceListingContent.BillingInvoices != null && BillingInvoiceListingContent.BillingInvoices.Count(x => !(x.IsHighTechServiceBill)) > 0)
                if (BillingInvoiceListingContent != null && BillingInvoiceListingContent.BillingInvoices != null)
                /*▲====: #002*/
                {
                    return BillingInvoiceListingContent.BillingInvoices.Where(x => x.IsHighTechServiceBill == IsHighTechServiceBill).Sum(y => y.TotalPatientPayment);                  
                }
                return 0;
            }         
        }

        public string BillingInvType_text
        {
            get
            {
                string BillNum = "";
                if (!string.IsNullOrEmpty(BillingInvNum) && !string.IsNullOrWhiteSpace(BillingInvNum))
                {
                    BillNum = " (" + BillingInvNum + ")";
                }
                return IsHighTechServiceBill == true ? eHCMSResources.Z0130_G1_BillPhThuatKTC + BillNum : eHCMSResources.Z0131_G1_BillTinhTienNoiTru;
            }
        }       

        public void btnAddSupportFundItem()
        {
            if (TotalBillToSupport <= 0)
            {
                if (IsHighTechServiceBill == true && (SelectedBillingInv == null || SelectedBillingInv.InPatientBillingInvID <= 0))
                {
                    MessageBox.Show(eHCMSResources.K0418_G1_LuuBillTruocKhiThemQuyHT);
                    return;
                }
                MessageBox.Show(eHCMSResources.A0687_G1_Msg_InfoKhTheHoTro);
                return;
            }
            if (CurrentFund == null)
            {
                return;
            }
            if (SupportFunds == null)
            {
                SupportFunds = new ObservableCollection<CharitySupportFund>();
            }
            if (ListOrganization == null || SelectedOrganization == null || SelectedOrganization.CharityOrgID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0339_G1_Msg_InfoChon1ToChucTuThien);
                return;
            }
            decimal TotalCurrentAmount = SupportFunds.Sum(x => x.AmountValue);
            string ErrorMsg = "";
            CalculateAmountAndPercentForFundItem(CurrentFund, out ErrorMsg, TotalCurrentAmount);
            if (!string.IsNullOrEmpty(ErrorMsg) && !string.IsNullOrWhiteSpace(ErrorMsg))
            {
                MessageBox.Show(ErrorMsg);
                CleanFundItemInfo();
                return;
            }
            CurrentFund.BillingInvID = InPatientBillingInvID.GetValueOrDefault();
            CurrentFund.CharityOrgInfo = SelectedOrganization.DeepCopy();
            CurrentFund.RecordState = CommonRecordState.INSERTED;
            //CurrentFund.V_CharityObjectType = ;
            //▼====: #003
            if (!IsReason)
                CurrentFund.V_CharityObjectType = -1;
            else if (CurrentFund.V_CharityObjectType <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2180_G1_VuiLongNhapLyDo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▲====: #003
            SupportFunds.Add(CurrentFund.DeepCopy());
            CleanFundItemInfo();
        }

        public void CleanFundItemInfo()
        {
            CurrentFund = new CharitySupportFund();
            SelectedOrganization = null;
            IsUsedPercent = true;
        }
        public void btnCalAmountValue()
        {
            CheckAndCalBeforeSave();
        }

        public void CalculateAmountAndPercent()
        {
            if (PtRegistrationID <= 0)
            {
                return;
            }
            if (TotalBillToSupport <= 0)
            {
                if (IsHighTechServiceBill == true && (SelectedBillingInv == null || SelectedBillingInv.InPatientBillingInvID <= 0))
                {
                    MessageBox.Show(eHCMSResources.K0418_G1_LuuBillTruocKhiThemQuyHT);
                    return;
                }
                MessageBox.Show(eHCMSResources.A0687_G1_Msg_InfoKhTheHoTro);
                return;
            }
            decimal TotalCurrentAmount = 0;
            if (SupportFunds != null && SupportFunds.Count > 0)
            {
                TotalCurrentAmount = SupportFunds.Sum(item => item.AmountValue);
            }
            string ErrorMsg = "";
            CalculateAmountAndPercentForFundItem(CurrentFund, out ErrorMsg, TotalCurrentAmount);
        }
        public decimal TotalSupportFinalized
        {
            get
            {
                return IsHighTechServiceBill == true ? Math.Round(TotalSupported_HighTech,0) : Math.Round(TotalSupported,0);
            }
        }
        /* Các luật đặt ra đối với quỹ hỗ trợ
         * 1. Tất cả các tính toán chỉ dựa trên tương quan giữa tổng sũy hỗ trợ và tổng số tiền bệnh nhân phải trả cho bệnh viện, không quan tâm chi tiết mỗi quỹ hỗ trợ bao nhiêu
         *      --> linh động khi chỉnh sửa. Cho phép cập nhật thông tin quỹ hỗ trợ (thêm, xóa) cả khi tiền quỹ đã được sử dụng để quyết toán
         * 2. Có hai nơi có thể thêm quỹ hỗ trợ: 
         *      (1): màn hình tạo bill DVKTC: thêm hỗ trợ cho các bill kỹ thuật cao. Mỗi bill KTC sẽ có thông tin quỹ hỗ trợ khác nhau. Có thể có nhiều bill KTC, trong đó có bill được hỗ trợ và bill không được hỗ trợ 
         *      (2): màn hình quyết toán: thêm quỹ hỗ trợ cho tất cả các bill nội trú KHÔNG PHẢI DVKTC (nội trú thường). 
         *  Các quỹ hỗ trợ thêm cho bill DVKTC được lưu mã bill cụ thể áp dụng hỗ trợ, các bill nội trú thường do hỗ trợ trên tổng số nên không lưu kèm mã bill (dùng chung)
         * 3. Về ràng buộc tổng số tiền quỹ được hỗ trợ: tính đến thời điểm quyết toán, luôn luôn phải đảm bảo tổng tiền bệnh nhân ĐÃ TRẢ (TotalPatientPaid) + tổng tiền hỗ trợ = tổng tiền bệnh PHẢI TRẢ cho bv
         *      3.1. Đối với hỗ trợ bill kỹ thuật cao: tổng tiền hỗ trợ tối đa cho một bill KTC bất kỳ đúng bằng tổng tiền bệnh nhân phải chi trả cho bill KTC đó
         *      3.2. Đối với hỗ trợ các bill nội trú thường: 
         *          (a). Thông thường, trong trường hợp chưa có bill nội trú nào được quyết toán: tổng tiền hỗ trợ tối đa cho các bill nội trú đúng bằng tổng số tiền bệnh nhân phải chi trả cho các bill nội trú (bệnh nhân không cần đóng thêm tiền)
         *          (b). Một số trường hợp đặc biệt:
         *              (b1). Giả sử bệnh nhân có 3 bill nội trú có giá trị lần lượt: 6tr, 2tr, 2tr. Ban đầu cho tổng quỹ hỗ trợ 15% tổng bill, tương đương 1tr5
         *                  - Quyết toán bill 6tr: vì bệnh nhân có hỗ trợ 1tr5, nên bệnh nhân cần đóng thêm 4tr5 nữa để quyết toán --> Quyết toán thành công
         *                  - Điều chỉnh quỹ hỗ trợ từ 15% --> 60%, tương đương 6tr
         *                  --> Lúc này: Tổng tiền bệnh nhân đã trả (4tr5) + tổng tiền hỗ trợ (6tr) = 10tr5 > tổng tiền bệnh nhân phải trả (10tr) --> SAI
         *              (b2). Bệnh nhân có 2 bill có giá trị lần lượt là: 8tr, 2tr. Ban đầu quỹ hỗ trợ 50%, tương đương 5tr
         *                  - Quyết toán bill số 8tr, quỹ có 5tr, bệnh nhân đóng thêm 3tr --> Quyết toán thành công
         *                  - Điều chỉnh quỹ giảm từ 50% --> 10%, tương đương 1tr
         *                  --> Lúc này: Tổng tiền hỗ trợ còn 1tr, trong khi đó, đợt quyết toán trước đã sử dụng hết 5tr từ quỹ --> SAI
         *      ==> TỔNG TIỀN HỖ TRỢ ĐÃ SỬ DỤNG CHO CÁC ĐỢT QUYẾT TOÁN TRƯỚC <= TỔNG TIỀN ĐƯỢC HỖ TRỢ <= TỔNG TIỀN BỆNH NHÂN PHẢI TRẢ CHO CÁC BILL CHƯA QUYẾT TOÁN + TỔNG TIỀN BỆNH NHÂN ĐÃ TRẢ ĐỂ QUYẾT TOÁN BILL
         *          (TÍNH ĐẾN THỜI ĐIỂM CẬP NHẬT THÔNG TIN QUỸ HỖ TRỢ)
         *      ==> Như vậy, tổng tiền hỗ trợ tối đa cho phép có thể nhỏ hơn 100% (do đã tồn tại các bill được quyết toán trước thời điểm thêm quỹ hỗ trợ)         
         */

        public void CalculateAmountAndPercentForFundItem(CharitySupportFund SupportFundItem, out string ErrorMsg, decimal TotalCurrentAmount = 0)
        {
            decimal MaxAmountSupportCanAdd = Math.Round(TotalBillToSupport - TotalCurrentAmount, 0);
            ErrorMsg = "";
            if (!SupportFundItem.IsUsedPercent)
            {
                if (SupportFundItem.AmountValue <= 0)
                {
                    ErrorMsg = eHCMSResources.Z0385_G1_SoTienHTroPhaiLonHon0;
                    return;
                }
                if (SupportFundItem.AmountValue > MaxAmountSupportCanAdd)
                {
                    ErrorMsg = string.Format("{0} {1}: {2}. \n {3}: {4} \n ==>{5}: {6}"
                        , eHCMSResources.Z0132_G1_TgTienDuocPhepHTCho, BillingInvType_text, Math.Round(TotalBillToSupport, 0).ToString()
                        , eHCMSResources.Z0133_G1_DaHoTro, Math.Round(TotalCurrentAmount, 0).ToString()
                        , eHCMSResources.Z0134_G1_SoTienToiDaHoTro, Math.Round(MaxAmountSupportCanAdd, 0).ToString());
                    return; 
                }
                //==== #001
                //decimal tempPercent = Math.Round(SupportFundItem.AmountValue / TotalPatientPayment * 100, 2);
                decimal tempPercent = Math.Round(SupportFundItem.AmountValue / TotalBillToSupport * 100, 2);
                //==== #001
                if (SupportFundItem.PercentValue != tempPercent)
                {
                    SupportFundItem.PercentValue = tempPercent;
                    if (SupportFundItem.RecordState == CommonRecordState.UNCHANGED)
                    {
                        SupportFundItem.RecordState = CommonRecordState.UPDATED;
                    }
                }
            }
            else
            {
                if (SupportFundItem.PercentValue <= 0)
                {
                    ErrorMsg = eHCMSResources.Z0135_G1_PhanTramHTroPhaiLonHon0;
                    return;
                }
                //==== #001
                //decimal tempAmount = Math.Round(SupportFundItem.PercentValue * TotalPatientPayment / 100, 0);
                decimal tempAmount = Math.Round(SupportFundItem.PercentValue * TotalBillToSupport / 100, 0);
                //==== #001
                if (tempAmount > MaxAmountSupportCanAdd)
                {
                    ErrorMsg = string.Format("{0} {1}: {2}. \n {3}: {4} \n ==>{5}: {6}"
                        , eHCMSResources.Z0132_G1_TgTienDuocPhepHTCho, BillingInvType_text, Math.Round(TotalBillToSupport, 0).ToString()
                        , eHCMSResources.Z0133_G1_DaHoTro, Math.Round(TotalCurrentAmount, 0).ToString()
                        , eHCMSResources.Z0134_G1_SoTienToiDaHoTro, Math.Round(MaxAmountSupportCanAdd, 0).ToString());
                    return;
                }
                if (SupportFundItem.AmountValue != tempAmount)
                {
                    SupportFundItem.AmountValue = tempAmount;
                    if (SupportFundItem.RecordState == CommonRecordState.UNCHANGED)
                    {
                        SupportFundItem.RecordState = CommonRecordState.UPDATED;
                    }
                }
            }
        }

        public decimal TotalBillToSupport
        {
            get
            {
                //if (IsHighTechServiceBill && SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
                if (SelectedBillingInv != null && SelectedBillingInv.InPatientBillingInvID > 0)
                {
                    return TotalPatientPayment;
                }
                //if (!IsHighTechServiceBill && BillingInvoiceListingContent != null && BillingInvoiceListingContent.BillingInvoices != null && BillingInvoiceListingContent.BillingInvoices.Count(x => !(x.IsHighTechServiceBill)) > 0)
                if (BillingInvoiceListingContent != null && BillingInvoiceListingContent.BillingInvoices != null)
                {
                    //TotalPatientPaidToFinalized = BillingInvoiceListingContent.BillingInvoices.Where(x => !(x.IsHighTechServiceBill) && x.BillingInvIsFinalized).Sum(y => y.TotalPatientPaid);
                    TotalPatientPaidToFinalized = BillingInvoiceListingContent.BillingInvoices.Where(x => x.BillingInvIsFinalized && x.IsHighTechServiceBill == IsHighTechServiceBill).Sum(y => y.TotalPatientPaid);
                    return TotalPatientPayment - TotalPatientPaidToFinalized;
                }
                return 0;
            }
        }

        public bool CheckAndCalBeforeSave()
        {
            if (PtRegistrationID <= 0)
            {
                return false;
            }
            if (SupportFunds != null && SupportFunds.Count > 0)
            {
                decimal TotalAmount = 0;
                string ErrorMsg = "";
                if (SupportFunds.Any(x=>x.CharityFundID > 0))
                {
                    foreach (CharitySupportFund item in SupportFunds.Where(x=>x.CharityFundID > 0).OrderBy(x => x.CharityFundID))
                    {
                        CalculateAmountAndPercentForFundItem(item, out ErrorMsg, TotalAmount);
                        if (!string.IsNullOrEmpty(ErrorMsg) && !string.IsNullOrWhiteSpace(ErrorMsg))
                        {
                            break;
                        }
                        TotalAmount += item.AmountValue;
                    }
                }
                if (SupportFunds.Any(x => x.CharityFundID <= 0))
                {
                    foreach (CharitySupportFund item in SupportFunds.Where(x => x.CharityFundID <= 0))
                    {
                        CalculateAmountAndPercentForFundItem(item, out ErrorMsg, TotalAmount);
                        if (!string.IsNullOrEmpty(ErrorMsg) && !string.IsNullOrWhiteSpace(ErrorMsg))
                        {
                            break;
                        }
                        TotalAmount += item.AmountValue;
                    }
                }
            }
            decimal TotalCurSupportFund = SupportFunds != null && SupportFunds.Count() >= 0 ? Math.Round(SupportFunds.Sum(x => x.AmountValue), 0) : 0;
            if (TotalCurSupportFund > TotalBillToSupport)
            {
                if (TotalBillToSupport <= 0)
                {
                    MessageBox.Show( string.Format("{0}: {1} \n{2}. \n{3}"
                        , eHCMSResources.K0217_G1_TgTienBNPhaiTraChoBill, TotalBillToSupport.ToString(), eHCMSResources.A0687_G1_Msg_InfoKhTheHoTro, eHCMSResources.A1028_G1_Msg_YCKtraTTin));
                }
                else
                {
                    //==== 20161208 CMN Begin: Add variable fix message missing.
                    MessageBox.Show( string.Format("{0}: {1}. \n{2}: {3}. \n{4}"
                        //, eHCMSResources.K0218_G1_TgTienHoTroHTai, TotalCurSupportFund.ToString(), eHCMSResources.Z0132_G1_TgTienDuocPhepHTCho, MaxPercentCanSupport.ToString(), eHCMSResources.Z0416_G1_VuiLongDieuChinhLai));
                    , eHCMSResources.K0218_G1_TgTienHoTroHTai, TotalCurSupportFund.ToString(), eHCMSResources.Z0132_G1_TgTienDuocPhepHTCho, TotalBillToSupport.ToString(), eHCMSResources.Z0416_G1_VuiLongDieuChinhLai));
                }
                return false;
            }
            if (TotalCurSupportFund < TotalSupportFinalized)
            {
                MessageBox.Show( string.Format("{0}: {1} {2}. \n{3}: {4} [{5}%]"
                    , eHCMSResources.A0224_G1_Msg_InfoBNDaSuDung, Math.Round(TotalSupportFinalized, 0).ToString(), eHCMSResources.Z0417_G1_QuyHTDeQToanBill
                    , eHCMSResources.Z0418_G1_TgQuyHTKgDuocNhoHon, Math.Round(TotalSupportFinalized, 0), Math.Round(TotalSupportFinalized / TotalPatientPayment * 100, 2)));
                return false;
            }
            return true;
        }

        private ObservableCollection<CharitySupportFund> _ListRemovedFund;
        public ObservableCollection<CharitySupportFund> ListRemovedFund
        {
            get
            {
                return _ListRemovedFund;
            }
            set
            {
                _ListRemovedFund = value;
                NotifyOfPropertyChange(() => ListRemovedFund);
            }
        }

        public void RemoveSupportFundItem(CharitySupportFund RemovedFund)
        {
            if (ListRemovedFund == null)
            {
                ListRemovedFund = new ObservableCollection<CharitySupportFund>();
            }
            if (RemovedFund.CharityFundID > 0)
            {
                RemovedFund.RecordState = CommonRecordState.DELETED;
                if (ListRemovedFund == null)
                {
                    ListRemovedFund = new ObservableCollection<CharitySupportFund>();
                }
                ListRemovedFund.Add(RemovedFund);
            }
        }

        public void RemoveSupportFundItemCmd(object source, object eventArgs)
        {
            var sender = source as Button;
            if (sender != null)
            {
                var RemovedFund = sender.DataContext as CharitySupportFund;

                if (RemovedFund != null)
                {
                    MessageBoxResult ConfirmToRemove = MessageBox.Show(string.Format("{0} {1} từ {2}?"
                        , eHCMSResources.K0474_G1_XoaDongQuy, RemovedFund.AmountValue, RemovedFund.CharityOrgInfo.CharityOrgName), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
                    if (ConfirmToRemove == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    RemoveSupportFundItem(RemovedFund);
                    SupportFunds.Remove(RemovedFund);
                }
            }
        }

        public void SaveCharitySupportFundForInPt(bool AutoSave = false)
        {
            //HPT_20160712: bill đã quyết toán không thể mở lên để cập nhật, chỉ được xem chi tiết thôi do đó cũng không thể load lên các thông tin quỹ hỗ trợ của bill đã quyết toán
            // Cho nên câu lệnh if dưới đây luôn false. Nếu nhìn thấy được thông báo này là đã bị sai gì đó rồi
            if (IsHighTechServiceBill == true && SelectedBillingInv != null && SelectedBillingInv.BillingInvIsFinalized)
            {
                MessageBox.Show(eHCMSResources.A0256_G1_Msg_InfoKhTheThayDoiTTQuyHT);
                return;
            }
            string SaveMsg = "";
            if (TotalBillToSupport <= 0 && SupportFunds != null && SupportFunds.Count() > 0)
            {
                SaveMsg = string.Format("{0} {1} {2}!", eHCMSResources.Z0138_G1_TgTienBillPhaiTra, BillingInvType_text, eHCMSResources.Z0139_G1_TatCaTTinQuyHTSeBiXoa);
                foreach (var item in SupportFunds)
                {
                    RemoveSupportFundItem(item);
                }
                SupportFunds = null;
            }
            else { SaveMsg = eHCMSResources.Z0140_G1_CNhatTTinQuyHT; }
            if (CheckAndCalBeforeSave() == false)
            {
                return;
            }
            if (IsModifiedSupportFund() == false)
            {                
                return;
            }
            List<CharitySupportFund> CharitySupportFunds = new List<CharitySupportFund>();
            if (SupportFunds != null && SupportFunds.Count() > 0)
            {
                foreach (var item in SupportFunds)
                {
                    CharitySupportFunds.Add(item);
                }
            }
            if (ListRemovedFund != null && ListRemovedFund.Count() > 0)
            {
                foreach (var item in ListRemovedFund)
                {
                    CharitySupportFunds.Add(item);
                }
            }
            if (CharitySupportFunds == null || CharitySupportFunds.Count() <= 0)
            {
                return;
            }
            //HPT_20160714: Anh kiên nói nên thông báo cho người ta biết mình đã thực hiện các cập nhật tự động
            if (AutoSave && !string.IsNullOrEmpty(SaveMsg))
            {
                MessageBox.Show(SaveMsg);
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginSaveCharitySupportFundForInPt(PtRegistrationID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), InPatientBillingInvID.GetValueOrDefault(), CharitySupportFunds,
                        contract.BeginSaveCharitySupportFundForInPt_V2(PtRegistrationID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), InPatientBillingInvID.GetValueOrDefault(), CharitySupportFunds, IsHighTechServiceBill == true,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allSupportFundInfo = contract.EndSaveCharitySupportFundForInPt_V2(asyncResult);
                                    if (allSupportFundInfo != null)
                                    {
                                        AllSupportFunds = new ObservableCollection<CharitySupportFund>(allSupportFundInfo);
                                        SetSupportFunds();
                                    }
                                    else
                                    {
                                        SupportFunds = null;
                                    }
                                    CleanFundItemInfo();
                                    ListRemovedFund = new ObservableCollection<CharitySupportFund>();
                                    // Chỉ khi hàm lưu được gọi từ sự kiện bấm nút lưu mới hiển thị thông báo lưu hoàn tất và load lại thông tin thanh toán. 
                                    if (!AutoSave) 
                                    {
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                        if (IParentScreen != null)
                                        {
                                            IParentScreen.LoadPaymentInfo();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
                                    this.HideBusyIndicator();
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //public bool IsModifiedBillingInvoice()
        //{
        //    if (IsHighTechServiceBill && EditingInvoiceDetailsContent != null)
        //    {
        //        return EditingInvoiceDetailsContent.CheckDifferentBillingInvoice();
        //    }
        //    return false;
        //}

        public void btnSaveSupports()
        {
            //if (IsModifiedBillingInvoice())
            //{
            //    MessageBox.Show(eHCMSResources.A1035_G1_Msg_InfoYCLuuBill);
            //    return;
            //}
            SaveCharitySupportFundForInPt();
        }

        public bool IsModifiedSupportFund()
        {
            decimal NumOfOldFundItems = BackupSupportFunds != null && BackupSupportFunds.Count() > 0 ? BackupSupportFunds.Count() : 0;
            decimal NumOfCurrentFundItems = SupportFunds != null && SupportFunds.Count() > 0 ? SupportFunds.Count() : 0;
            if (NumOfOldFundItems != NumOfCurrentFundItems)
            {
                return true;
            }
            decimal NumOfDifferrentRows = SupportFunds != null && SupportFunds.Count() > 0 ? SupportFunds.Count(x => x.RecordState != CommonRecordState.UNCHANGED) : 0;
            if (NumOfDifferrentRows > 0)
            {
                return true;
            }
            return false;
        }

        public void RefreshFundInfo()
        {
            CurrentFund = new CharitySupportFund();
            SupportFunds = new ObservableCollection<CharitySupportFund>();
            BackupSupportFunds = new ObservableCollection<CharitySupportFund>();
            ListRemovedFund = new ObservableCollection<CharitySupportFund>();
            SelectedOrganization = null;
            IsUsedPercent = true;
            /*TMA*/
            ObjV_CharityObjectType = new ObservableCollection<Lookup>();
            LoadV_CharityObjectType();
            /*TMA*/
        }

        private decimal _TotalSupported_HighTech;
        public decimal TotalSupported_HighTech
        {
            get
            {
                return _TotalSupported_HighTech;
            }
            set
            {
                _TotalSupported_HighTech = value;
                NotifyOfPropertyChange(() => TotalSupported_HighTech);
                NotifyOfPropertyChange(() => TotalSupportFinalized);
            }
        }

        private decimal _TotalSupported;
        public decimal TotalSupported
        {
            get
            {
                return _TotalSupported;
            }
            set
            {
                _TotalSupported = value;
                NotifyOfPropertyChange(() => TotalSupported);
                NotifyOfPropertyChange(() => TotalSupportFinalized);
            }
        }
        private IInPatientSettlement _IParentScreen;
        public IInPatientSettlement IParentScreen
        {
            get
            {
                return _IParentScreen;
            }
            set
            {
                _IParentScreen = value;
                NotifyOfPropertyChange(() => IParentScreen);
            }
        }

        public void btnCloseCmd()
        {
            if (IsModifiedSupportFund())
            {
                MessageBoxResult confirm = MessageBox.Show("Thông tin quỹ đã thay đổi và chưa được lưu lại. Bạn có muốn bỏ qua?", eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
                if (confirm == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            this.TryClose();
        }

        /*TMA 10/11/2017*/
        private Lookup _ObjV_CharityObjectType_Selected;
        public Lookup ObjV_CharityObjectType_Selected
        {
            get { return _ObjV_CharityObjectType_Selected; }
            set
            {
                _ObjV_CharityObjectType_Selected = value;
                NotifyOfPropertyChange(() => ObjV_CharityObjectType_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_CharityObjectType;
        public ObservableCollection<Lookup> ObjV_CharityObjectType
        {
            get { return _ObjV_CharityObjectType; }
            set
            {
                _ObjV_CharityObjectType = value;
                NotifyOfPropertyChange(() => ObjV_CharityObjectType);
            }
        }
        public void LoadV_CharityObjectType()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_CharityObjectType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_CharityObjectType = new ObservableCollection<Lookup>(allItems);
                                    //▼====: #003
                                    if (CurrentFund != null && ObjV_CharityObjectType != null)
                                        CurrentFund.V_CharityObjectType = ObjV_CharityObjectType.FirstOrDefault().LookupID;
                                    //Lookup firstItem = new Lookup();
                                    //firstItem.LookupID = -1;
                                    //firstItem.ObjectValue = "--Chọn đối tượng--";
                                    //ObjV_CharityObjectType.Insert(0, firstItem);
                                    //▲====: #003
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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

        public void AddCharityOrganization()
        {
            Action<ICharityOrganization> onInitDlg = delegate (ICharityOrganization vm) { };
            GlobalsNAV.ShowDialog<ICharityOrganization>(onInitDlg);
            GetAllCharityOrganization();
        }
    }
}



