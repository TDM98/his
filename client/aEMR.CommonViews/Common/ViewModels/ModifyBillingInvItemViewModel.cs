using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.Collections;
using System.Windows;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IModifyBillingInvItem)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ModifyBillingInvItemViewModel : ViewModelBase, IModifyBillingInvItem
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ModifyBillingInvItemViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ModifyBillingInvItem = new MedRegItemBase();
        }

        public void Init()
        {
            if (ModifyBillingInvItem == null || ModifyBillingInvItem.ChargeableItem == null)
            {
                return;
            }  
          
            ReasonChangePrice = ModifyBillingInvItem.ReasonChangePrice;
            if (PopupType == 1)
            {
                InvoicePrice = Convert.ToInt64(ModifyBillingInvItem.ChargeableItem.NormalPrice);
                InvoicePriceBackup = Convert.ToInt64(ModifyBillingInvItem.ChargeableItem.NormalPrice);
                HIAllowedPrice = Convert.ToInt64(ModifyBillingInvItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault());
                HIApproved = HIAllowedPrice > 0 ? true : false;
            }
            else
            {
                InvoicePrice = Convert.ToInt64(ModifyBillingInvItem.InvoicePrice);
                InvoicePriceBackup = Convert.ToInt64(ModifyBillingInvItem.InvoicePrice);
                HIAllowedPrice = Convert.ToInt64(ModifyBillingInvItem.HIAllowedPrice.GetValueOrDefault());
                HIApproved = HIAllowedPrice > 0 ? true : false;
            }
        }

        public void SetParentInPtBillingInvDetailsLst(object parentVM)
        {
            ParentVM = (IInPatientBillingInvoiceDetailsListing)parentVM;
        }

        public IInPatientBillingInvoiceDetailsListing ParentVM { get; set; }

        private MedRegItemBase _ModifyBillingInvItem;
        public MedRegItemBase ModifyBillingInvItem
        {
            get
            {
                return _ModifyBillingInvItem;
            }
            set
            {
                _ModifyBillingInvItem = value;
                NotifyOfPropertyChange(() => ModifyBillingInvItem);
            }
        }

        // Hpt 21/11/2015: nhận biết VM được gọi trong trường hợp nào để bắn sự kiện cho đúng
        // 20191127 TNHX: 1 là thêm mới, 2 là cập nhật.
        private int _PopupType;
        public int PopupType
        {
            get
            {
                return _PopupType;
            }
            set
            {
                _PopupType = value;
                NotifyOfPropertyChange(() => PopupType);
            }
        }

        private long _InvoicePrice;
        public long InvoicePrice
        {
            get
            {
                return _InvoicePrice;
            }
            set
            {
                _InvoicePrice = value;
                NotifyOfPropertyChange(() => InvoicePrice);
            }
        }

        private long _InvoicePriceBackup;
        public long InvoicePriceBackup
        {
            get
            {
                return _InvoicePriceBackup;
            }
            set
            {
                _InvoicePriceBackup = value;
                NotifyOfPropertyChange(() => InvoicePriceBackup);
            }
        }

        private long _HIAllowedPrice = 0;
        public long HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                NotifyOfPropertyChange(() => HIAllowedPrice);
            }
        }

        private string _ReasonChangePrice = "";
        public string ReasonChangePrice
        {
            get
            {
                return _ReasonChangePrice;
            }
            set
            {
                _ReasonChangePrice = value;
                NotifyOfPropertyChange(() => ReasonChangePrice);
            }
        }

        private bool _HIApproved = true;
        public bool HIApproved
        {
            get
            {
                return _HIApproved;
            }
            set
            {
                _HIApproved = value;
                NotifyOfPropertyChange(() => HIApproved);
            }
        }

        public void CancelCmd()
        {
            ModifyBillingInvItem.IsModItemOK = false;
            TryClose();
        }

        public void OkCmd()
        {
            // tạm thời không kiểm tra sự khác biệt, không có thời gian. Mở popup lên rồi lưu xuống bất kể, chỉ cần dữ liệu hợp lệ

            //if (ModifyBillingInvItem.InvoicePrice == InvoicePrice && ModifyBillingInvItem.HIAllowedPrice == HIPrice)
            //{
            //    return; // Nếu không điều chỉnh gì hết thì không cần kiểm tra
            //}

            if (InvoicePrice <= 0)
            {
                MessageBox.Show(eHCMSResources.K0432_G1_NhapGiaLonHon0);
                return;
            }

            if (InvoicePrice < HIAllowedPrice && HIApproved)
            {
                MessageBox.Show(eHCMSResources.A0557_G1_Msg_InfoDGiaLonHonGiaBH);
                return;
            }

            if (InvoicePrice != InvoicePriceBackup && (ReasonChangePrice == null || ReasonChangePrice.Length < 8))
            {
                MessageBox.Show(eHCMSResources.A0903_G1_Msg_InfoNhapLyDoDChinhGia);
                return;
            }
            ModifyBillingInvItem.IsModItemOK = true;

            ModifyBillingInvItem.ChargeableItem.NormalPrice = ModifyBillingInvItem.ChargeableItem.HIPatientPrice = InvoicePrice;
            ModifyBillingInvItem.ChargeableItem.HIAllowedPrice = HIAllowedPrice;
            ModifyBillingInvItem.ReasonChangePrice = ReasonChangePrice;

            if (PopupType == 2)
            {
                // TxD 16/07/2016 The following event is NOT used anymore because the IInPatientBillingInvoiceDetailsListing view model 
                // is also used by a dialog to show the billing invoice details (click i in the list of billing invoices). So if the dialog is displayed 
                // even after it is closed the event will still be received wrongly and causing havoc.
                // 
                //Globals.EventAggregator.Publish(new ModifyPriceToUpdate_Completed());
                if (ParentVM != null)
                {
                    ParentVM.ModOfUpdatableItemPriceDone();
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ModifyPriceToInsert_Completed { ModifyItem = ModifyBillingInvItem });
            }
            TryClose();
        }
    }
}
