using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using aEMR.CommonTasks;
/*
 * 20190522 #001 TNHX:   [BM0010766] Thêm trường ngày xuất của phiếu xuất để nhập vào kho + refactor code
 * 20210825 #002 QTD: Thêm loại dinh dưỡng
 * 20211102 #003 QTD: Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IUpdateReceipt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UpdateReceiptViewModel : Conductor<object>, IUpdateReceipt
    {
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NGUON_KHAC;
        [ImportingConstructor]
        public UpdateReceiptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);
        }
     
        private InwardDrugMedDeptInvoice _CurrentInwardDrugMedDeptInvoice;
        public InwardDrugMedDeptInvoice CurrentInwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentInwardDrugMedDeptInvoice;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptInvoice != value)
                {
                    _CurrentInwardDrugMedDeptInvoice = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptInvoice);
                }
            }
        }

        public void btnSave()
        {

            if (string.IsNullOrEmpty(CurrentInwardDrugMedDeptInvoice.InvInvoiceNumber))
            {
                Globals.ShowMessage("Chưa nhập mã hóa đơn", eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentInwardDrugMedDeptInvoice.InvDateInvoice==null)
            {
                Globals.ShowMessage("Chưa nhập ngày hóa đơn", eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (string.IsNullOrEmpty(CurrentInwardDrugMedDeptInvoice.SerialNumber))
            {
                Globals.ShowMessage("Chưa nhập số serial", eHCMSResources.G0442_G1_TBao);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateReceipt(CurrentInwardDrugMedDeptInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool results = contract.EndUpdateReceipt(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage("Lưu thành công", "Thông báo");
                                }
                                else
                                {
                                    Globals.ShowMessage("Lưu thất bại", "Thông báo");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
    }
}
