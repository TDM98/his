using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
/*
* 20190822 #001 TTM:   BM 0013217: Cho phép sử dụng Grid phiếu chỉ định trong đợt đăng ký để thực hiện hiệu chỉnh.
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPCLRequestDetails)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLRequestDetailsViewModel : ViewModelBase, IPCLRequestDetails
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PCLRequestDetailsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {

        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private ObservableCollection<PatientPCLRequest> _ObjPatientPCLRequest_ByRegistrationID;
        public ObservableCollection<PatientPCLRequest> ObjPatientPCLRequest_ByRegistrationID
        {
            get { return _ObjPatientPCLRequest_ByRegistrationID; }
            set
            {
                _ObjPatientPCLRequest_ByRegistrationID = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_ByRegistrationID);
            }
        }

        private PatientPCLRequest _pclRequest;

        public PatientPCLRequest PCLRequest
        {
            get { return _pclRequest; }
            set
            {
                _pclRequest = value;
                NotifyOfPropertyChange(() => PCLRequest);
                if (_pclRequest == null || _pclRequest.PatientPCLRequestIndicators == null)
                {
                    MaskedDetailList = null;
                }
                else
                {
                    var col = new ObservableCollection<PatientPCLRequestDetail>();
                    foreach (var item in _pclRequest.PatientPCLRequestIndicators)
                    {
                        col.Add(item);
                    }
                    MaskedDetailList = col;
                }
            }
        }

        private ObservableCollection<PatientPCLRequestDetail> _maskedDetailList;

        public ObservableCollection<PatientPCLRequestDetail> MaskedDetailList
        {
            get { return _maskedDetailList; }
            private set
            {
                _maskedDetailList = value;
                NotifyOfPropertyChange(() => MaskedDetailList);
            }
        }

        private PatientRegistration _PatientReg;
        public PatientRegistration PatientReg
        {
            get { return _PatientReg; }
            set
            {
                _PatientReg = value;
                NotifyOfPropertyChange(() => PatientReg);
            }
        }

        public void ResetCollection()
        {
            MaskedDetailList = new ObservableCollection<PatientPCLRequestDetail>();
            NotifyOfPropertyChange(() => MaskedDetailList);
        }

        public void AddItemToView(PatientPCLRequestDetail requestDetail)
        {
            if (MaskedDetailList == null)
            {
                MaskedDetailList = new ObservableCollection<PatientPCLRequestDetail>();
            }
            MaskedDetailList.Add(requestDetail);

            PatientReg = new PatientRegistration();
            PatientReg = Registration_DataStorage.CurrentPatientRegistration;
            MedRegItemBase mUpdateInvoiceItem = (MedRegItemBase)requestDetail;

            mUpdateInvoiceItem.IsCountHI = requestDetail.IsCountHI;
            mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? PatientReg.PtInsuranceBenefit : null;
            mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? PatientReg.HisID : null;
            mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice : null;
            mUpdateInvoiceItem.CreatedDate = Globals.GetCurServerDateTime();
            CommonGlobals.ChangeHIBenefit(PatientReg, requestDetail, mUpdateInvoiceItem);
            NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
        }

        public void RemoveItemFromView(PatientPCLRequestDetail requestDetail)
        {
            if (MaskedDetailList != null)
            {
                MaskedDetailList.Remove(requestDetail);
            }
        }

        public void hplDelete_Click(PatientPCLRequestDetail requestDetail)
        {
            if (requestDetail.RecordState == RecordState.DETACHED)
            {
                PCLRequest.PatientPCLRequestIndicators.Remove(requestDetail);
            }
            else
            {
                requestDetail.RecordState = RecordState.DELETED;
                requestDetail.MarkedAsDeleted = true;
            }
            RemoveItemFromView(requestDetail);

            //phát sự kiện để List CurrentPclRequest.PatientPCLRequestIndicators remove item này ra, ko hiểu sao luôn, thêm mới thì ok, mà Sửa thì bị nên đành dùng cách này
            Globals.EventAggregator.Publish(new PCLRequestDetailRemoveEvent<PatientPCLRequestDetail> { PCLRequestDetail = requestDetail });
            //phát sự kiện để List CurrentPclRequest.PatientPCLRequestIndicators remove item này ra, ko hiểu sao luôn, thêm mới thì ok, mà Sửa thì bị nên đành dùng cách này

            //Phát sự kiện tính tổng lại tiền dự trù
            Globals.EventAggregator.Publish(new TinhTongTienDuTruEvent());
            //Phát sự kiện tính tổng lại tiền dự trù
        }

        public void tbQty_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);

            if (Ctr != null)
            {
                PatientPCLRequestDetail ObjEdit = Ctr.DataContext as PatientPCLRequestDetail;
                if (ObjEdit != null)
                {
                    if (ObjEdit.RecordState == RecordState.DELETED)
                        return;

                    ObjEdit.RecordState = RecordState.MODIFIED;
                    ObjEdit.MarkedAsDeleted = false;
                    //Phát sự kiện tính tổng lại tiền dự trù
                    Globals.EventAggregator.Publish(new TinhTongTienDuTruEvent());

                }
            }
        }

        //public void cboDeptLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    AxComboBox Ctr = (sender as AxComboBox);

        //    if (Ctr != null)
        //    {
        //        PatientPCLRequestDetail Objtmp = (Ctr.DataContext) as PatientPCLRequestDetail;

        //        if(Objtmp==null)
        //            return; 

        //        if (Objtmp.RecordState == RecordState.UNCHANGED)
        //        {
        //            Objtmp.RecordState = RecordState.MODIFIED;
        //        }
        //    }
        //}

        public void dtgList_DblClick(object sender, EventArgs<object> eventArgs)
        {
            //Kiem tra neu phieu nay co ID >0 thi khong cho Rut ra,khong cho Sua, ma chi duoc Detele 
            var vm = Globals.GetViewModel<IPCLRequestDetail>();
            vm.PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;

            if (vm.PCLRequestDetail.PatientPCLReqID > 0)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1391_G1_KgDuocThayDoiPhg, vm.PCLRequestDetail.PCLExamType.PCLExamTypeName.Trim(), vm.PCLRequestDetail.PatientPCLReqID.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            else
            {
                //Globals.ShowDialog(vm as Conductor<object>);

                Action<IPCLRequestDetail> onInitDlg = (Alloc) =>
                {
                    Alloc.PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;
                };
                GlobalsNAV.ShowDialog<IPCLRequestDetail>(onInitDlg);
            }
        }

        public void ckbIsCountHI_CheckedChanged(object sender, RoutedEventArgs e)
        {
            PatientReg = new PatientRegistration();
            PatientReg = Registration_DataStorage.CurrentPatientRegistration;
            CheckBox ckbIsCountHI = sender as CheckBox;
            if (!(ckbIsCountHI.DataContext is PatientPCLRequestDetail) || PatientReg == null)
            {
                e.Handled = true;
                return;
            }
            MedRegItemBase mUpdateInvoiceItem = (MedRegItemBase)ckbIsCountHI.DataContext;
            mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
            mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? PatientReg.PtInsuranceBenefit : null;
            mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? PatientReg.HisID : null;
            mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice : null;
            mUpdateInvoiceItem.CreatedDate = Globals.GetCurServerDateTime();
            CommonGlobals.ChangeHIBenefit(PatientReg, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
            NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
        }

        private bool _IsEnableListPCL = false;
        public bool IsEnableListPCL
        {
            get { return _IsEnableListPCL; }
            set
            {
                if (_IsEnableListPCL != value)
                {
                    _IsEnableListPCL = value;
                    NotifyOfPropertyChange(() => IsEnableListPCL);
                }
            }
        }
        //▼===== #001
        public void dtgListDetails_DblClick(object sender, EventArgs<object> eventArgs)
        {
            PatientPCLRequest Objtmp = eventArgs.Value as PatientPCLRequest;
            switch (Objtmp.V_PCLRequestStatus)
            {
                case AllLookupValues.V_PCLRequestStatus.CANCEL:
                    {
                        MessageBox.Show(string.Format("{0}! ", eHCMSResources.Z1030_G1_PhNayBNTraLaiKgLamXN));
                        break;
                    }
                case AllLookupValues.V_PCLRequestStatus.PROCESSING:
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0920_G1_Msg_InfoKhDcSuaPhDaXong));
                        break;
                    }
            }
            if (Objtmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEventWithKeyToShowDetailsForImage<PatientPCLRequest, String> { ObjA = Objtmp, ObjB = eHCMSResources.Z0055_G1_Edit });
            }
            else if (Objtmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEventWithKeyToShowDetails<PatientPCLRequest, String> { ObjA = Objtmp, ObjB = eHCMSResources.Z0055_G1_Edit });
            }
        }
        //▲===== #001
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
    }
}
