using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.ViewContracts.GuiContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.CommonTasks;
using eHCMSLanguage;
using System.Text;

/*
 * 20181217 #001 TNHX:  [BM0005436] Create button to Show report PhieuMienGiam
 * 20181221 #002 TNHX:  [BM0005452] Apply re-print report PhieuChiDinh when click PCL name
 * 20181225 #003 TNHX:  [BM0005462] Re-make report PhieuChiDinh
 * 20181226 #004 TNHX:  [BM0005467] Can print PhieuChiDinh after save Service or PCL by click into item name
 * 20181227 #005 TNHX:  [BM0005470] Add condition print PhieuChiDinh (BN tra sau - khong in lai) && Khong in doi voi DV + CLS huy/hoan tien
 * 20200113 #006 TTM:   BM 0021787: Fix lỗi không lưu thông tin miễn giảm và bảo hiểm nếu lưu và trả tiền cho dịch vụ đã được lưu và thay đổi thông tin miễn giảm và bảo hiểm.
 * 20200205 #007 TTM:   BM 0022818: Fix 1 số lỗi liên quan đến miễn giảm.
 *                      1. Khi miễn giảm trực tiếp chọn số tiền miễn giảm rồi bỏ miễn giảm thì số tiền không trở về 0.
 *                      2. Miễn giảm trực tiếp => Lưu lại => Bỏ tích MG => Chọn lại miễn giảm theo % => Không cho tích vào.
 */
namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Quản lý danh sách CLS của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    [Export(typeof(IOutPatientPclRequestManage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientPclRequestManageViewModel : Conductor<object>, IOutPatientPclRequestManage
    {
        private bool _IsOldList;
        public bool IsOldList
        {
            get
            {
                return _IsOldList;
            }
            set
            {
                _IsOldList = value;
                NotifyOfPropertyChange(() => IsOldList);
                NotifyOfPropertyChange(() => CanApplyIsOnPriceDiscount);
                NotifyOfPropertyChange(() => IsShowPrintDiscountButton);
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OutPatientPclRequestManageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        IOutPatientPclRequestManageView _currentView;
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _currentView = view as IOutPatientPclRequestManageView;
            if (_currentView != null)
            {
                _currentView.SetVisibilityBindingForHiColumns();
            }
        }
        #region PROPERTIES

        private ObservableCollection<PatientPCLRequest> _pclRequests;
        public ObservableCollection<PatientPCLRequest> PCLRequests
        {
            get { return _pclRequests; }
            set
            {
                if (_pclRequests != value)
                {
                    _pclRequests = value;
                    NotifyOfPropertyChange(() => PCLRequests);

                    var list = new List<PatientPCLRequestDetail>();
                    if (_pclRequests != null)
                    {
                        foreach (PatientPCLRequest request in _pclRequests)
                        {
                            if (request.RecordState != RecordState.DELETED && request.PatientPCLRequestIndicators != null)
                            {
                                foreach (PatientPCLRequestDetail item in request.PatientPCLRequestIndicators)
                                {
                                    if (item.RecordState != RecordState.DELETED)
                                    {
                                        list.Add(item);
                                    }

                                    item.PatientPCLRequest = request;
                                }
                                request.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(details_PropertyChanged).Handler;
                            }
                        }
                    }

                    if (PtPclReqDetailItems == null)
                    {
                        PtPclReqDetailItems = new ObservableCollection<PatientPCLRequestDetail>();
                        list.ForEach(listItem => PtPclReqDetailItems.Add(listItem));

                        _pclServiceDetails = new CollectionViewSource { Source = PtPclReqDetailItems };
                        PclDetailsListView = (CollectionView)_pclServiceDetails.View;
                        PclDetailsListView.Filter = DefaultFilter;
                        if (IsOldList)
                        {
                            PclDetailsListView.GroupDescriptions.Add(new PropertyGroupDescription("PatientPCLRequest"));
                        }
                        NotifyOfPropertyChange(() => PclDetailsListView);
                    }
                    else
                    {
                        PtPclReqDetailItems.Clear();
                        list.ForEach(listItem => PtPclReqDetailItems.Add(listItem));
                        PclDetailsListView.Refresh();
                    }
                }
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        private bool DefaultFilter(object item)
        {
            return true;
        }

        public CollectionView PclDetailsListView { get; set; }

        private CollectionViewSource _pclServiceDetails;

        private ObservableCollection<PatientPCLRequestDetail> _ptPclReqDetailItems;
        public ObservableCollection<PatientPCLRequestDetail> PtPclReqDetailItems
        {
            get
            {
                return _ptPclReqDetailItems;
            }
            set
            {
                _ptPclReqDetailItems = value;
                NotifyOfPropertyChange(() => PtPclReqDetailItems);
                AllPclItemsSelected = IsAllPclItemsSelected();
            }
        }

        public void details_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                AllPclItemsSelected = IsAllPclItemsSelected();
            }
        }

        private bool _allPclItemsSelected;
        public bool AllPclItemsSelected
        {
            get { return _allPclItemsSelected; }
            set
            {
                if (_allPclItemsSelected != value)
                {
                    _allPclItemsSelected = value;
                    NotifyOfPropertyChange(() => AllPclItemsSelected);
                }
            }
        }

        private bool _hiServiceBeingUsed;
        public bool HiServiceBeingUsed
        {
            get { return _hiServiceBeingUsed; }
            set
            {
                if (_hiServiceBeingUsed != value)
                {
                    _hiServiceBeingUsed = value;
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);
                    if (_currentView != null)
                    {
                        _currentView.SetVisibilityBindingForHiColumns();
                    }
                }
            }
        }
        private bool _showCheckBoxColumn;
        public bool ShowCheckBoxColumn
        {
            get { return _showCheckBoxColumn; }
            set
            {
                _showCheckBoxColumn = value;
                NotifyOfPropertyChange(() => ShowCheckBoxColumn);
            }
        }
        #endregion

        #region COMMANDS
        public void RemovePclRequestDetailsCmd(object sender, object eventArgs)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                Globals.EventAggregator.Publish(new RemoveItem<PatientPCLRequestDetail> { Item = elem.DataContext as PatientPCLRequestDetail });
            }
        }

        public void RemovePclRequestCmd(object sender, object eventArgs)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                Globals.EventAggregator.Publish(new RemoveItem<PatientPCLRequest> { Item = elem.DataContext as PatientPCLRequest });
            }
        }

        public void PrintCmd(object sender, object eventArgs)
        {
            var elem = sender as FrameworkElement;
            var ObjPatientPCLRequest = elem.DataContext as PatientPCLRequest;
            if (ObjPatientPCLRequest.PatientPCLReqID > 0)
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.PatientPCLReqID = (int)ObjPatientPCLRequest.PatientPCLReqID;
                //proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });
                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.PatientPCLReqID = (int)ObjPatientPCLRequest.PatientPCLReqID;
                    proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {

            }
        }



        public void ToggleSelectAllPclItems(object sender, RoutedEventArgs eventArgs)
        {
            var chk = sender as CheckBox;
            bool check = chk.IsChecked.Value;
            if (_pclRequests != null)
            {
                foreach (var item in _pclRequests)
                {
                    item.IsChecked = check;
                }
            }
        }
        #endregion

        private bool IsAllPclItemsSelected()
        {
            if (_pclRequests == null || _pclRequests.Count == 0)
            {
                return false;
            }

            return _pclRequests.All(t => t.IsChecked);
        }

        public void gridPCLRequests_Click(object sender, EventArgs<object> eventArgs)
        {
            if (currentColumn == null)
            {
                return;
            }
            if (currentColumn.DisplayIndex == (int)comlumIndex.Phong)
            {
                //Kiem tra neu phieu nay co ID >0 thi khong cho Rut ra,khong cho Sua, ma chi duoc Detele 
                //var vm = Globals.GetViewModel<IPCLRequestDetail>();
                //vm.PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;

                //if (vm.PCLRequestDetail.PatientPCLReqID > 0)
                //{
                //    MessageBox.Show(string.Format(eHCMSResources.Z1391_G1_KgDuocThayDoiPhg, vm.PCLRequestDetail.PCLExamType.PCLExamTypeName.Trim(), vm.PCLRequestDetail.PatientPCLReqID.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //}
                //else
                //{
                //    Globals.ShowDialog(vm as Conductor<object>);
                //}
                Action<IPCLRequestDetail> onInitDlg = (vm) =>
                {
                    vm.PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;
                    if (vm.PCLRequestDetail.PatientPCLReqID > 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1391_G1_KgDuocThayDoiPhg, vm.PCLRequestDetail.PCLExamType.PCLExamTypeName.Trim(), vm.PCLRequestDetail.PatientPCLReqID.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                };
                GlobalsNAV.ShowDialog<IPCLRequestDetail>(onInitDlg);
            }
            else
                if (currentColumn.DisplayIndex == (int)comlumIndex.Gia)
            {
                //var PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;
                //if (PCLRequestDetail == null || PCLRequestDetail.PCLReqItemID> 0
                //    || PCLRequestDetail.PCLExamType.IDCode != (long)AllLookupValues.V_ServicePrice.Changeable)
                //    return;

                //Coroutine.BeginExecute(PriceAdapt(PCLRequestDetail));
            }
        }

        private enum comlumIndex
        {
            None = 0,
            Phong = 4,
            Gia = 6,
        }

        DataGridColumn currentColumn { get; set; }
        public void gridPCLRequests_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((sender) != null)
            {
                currentColumn = ((DataGrid)sender).CurrentColumn;
            }
        }

        public IEnumerator<IResult> PriceAdapt(PatientPCLRequestDetail pclRegDetail)
        {
            var priceAdapt = new PriceAdaptShowDialogTask(pclRegDetail.PCLExamType.PCLExamTypeName, pclRegDetail.InvoicePrice);
            yield return priceAdapt;
            if (priceAdapt.IsOk)
            {
                pclRegDetail.InvoicePrice = priceAdapt.NewPrice;
                pclRegDetail.SpecialNote = priceAdapt.Comments;
            }
            yield break;
        }

        public void ckbDiscount_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
            {
                return;
            }
            CheckBox ckbDiscount = sender as CheckBox;
            if (!(ckbDiscount.DataContext is MedRegItemBase) || RegistrationObj == null || RegistrationObj.PromoDiscountProgramObj == null)
            {
                e.Handled = true;
                ckbDiscount.IsChecked = false;
                return;
            }
            if (RegistrationObj.PromoDiscountProgramObj.IsOnPriceDiscount)
            {
                (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = ckbDiscount.IsChecked.GetValueOrDefault(false);
                //▼===== #007: Lý do tích bỏ ra nhưng không có apply thông tin bằng hàm ApplyDiscount => Màn hình thấy đã bỏ thực tế chưa tính lại.
                if (!(ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked)
                {
                    MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam);
                    RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((sender as CheckBox).IsChecked == true));
                }
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                //▲===== #007
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).DiscountAmt == 0 && RegistrationObj.PromoDiscountProgramObj.DiscountPercent == 0)
            {
                e.Handled = true;
                ckbDiscount.IsChecked = false;
                return;
            }
            //▼===== #007:  Set lại vì nếu trường hợp dịch vụ đã lưu kèm thông tin miễn giảm mà chọn loại miễn giảm khác trong hàm ApplyDiscount sẽ lấy miễn giảm đang lưu ra làm việc.
            //              do PromoDiscProgID của dịch vụ đang chọn = với loại miễn giảm đã lưu trong danh sách miễn giảm của bệnh nhân.
            if ((ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID != RegistrationObj.PromoDiscountProgramObj.PromoDiscProgID)
            {
                (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = 0;
            }
            //▲===== #007
            RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((sender as CheckBox).IsChecked == true));
            //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState
            CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
            //▲===== #006
        }
        private PatientRegistration _RegistrationObj;
        public PatientRegistration RegistrationObj
        {
            get => _RegistrationObj; set
            {
                _RegistrationObj = value;
                NotifyOfPropertyChange(() => RegistrationObj);
            }
        }
        public void btnDiscount_Click(object sender, RoutedEventArgs e)
        {
            Button mbtnDiscount = sender as Button;
            if (!(mbtnDiscount.DataContext is MedRegItemBase) || (mbtnDiscount.DataContext as MedRegItemBase).PromoDiscProgID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            IPromoDiscountProgramEdit mPromoDiscountProgramEdit = Globals.GetViewModel<IPromoDiscountProgramEdit>();
            mPromoDiscountProgramEdit.IsViewOld = true;
            //▼====: #001
            mPromoDiscountProgramEdit.CanBtnViewPrint = true;
            mPromoDiscountProgramEdit.PatientPCLReqID = (mbtnDiscount.DataContext as PatientPCLRequestDetail).PatientPCLReqID;
            //▲====: #001
            mPromoDiscountProgramEdit.PromoDiscountProgramObj = new PromoDiscountProgram { PromoDiscProgID = (mbtnDiscount.DataContext as MedRegItemBase).PromoDiscProgID.Value, V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU };
            GlobalsNAV.ShowDialog_V3<IPromoDiscountProgramEdit>(mPromoDiscountProgramEdit);
        }
        //▼===== 20190106 TTM: Bỏ tích BH cho dịch vụ CLS khi đã thu tiền
        public bool CheckPCLHIStatus()
        {
            if (RegistrationObj != null && RegistrationObj.ConfirmHIStaffID > 0)
            {
                MessageBox.Show(eHCMSResources.Z2969_G1_DaXacNhanQLKhongThayDoiThongTinDV);
                return false;
            }
            return true;
        }
        public void ckbIsCountHI_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (Globals.ServerConfigSection.CommonItems.ChangeHIAfterSaveAndPayRule)
            {
                CheckBox ckbIsCountHI = sender as CheckBox;
                if (!(ckbIsCountHI.DataContext is MedRegItemBase) || RegistrationObj == null)
                {
                    e.Handled = true;
                    return;
                }
                if (CheckPCLHIStatus())
                {
                    MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                    mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                    mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                    mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                    mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                    //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState                    
                    //mUpdateInvoiceItem.RecordState = mUpdateInvoiceItem.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : mUpdateInvoiceItem.RecordState;
                    CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                    //▲===== #006
                    CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                    NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                    CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                    if (ckbIsCountHI.DataContext is PatientPCLRequestDetail)
                    {
                        Globals.EventAggregator.Publish(new ChangePCLHIStatus<PatientPCLRequestDetail>() { Item = ckbIsCountHI.DataContext as PatientPCLRequestDetail });
                    }
                }
            }
            else
            {
                CheckBox ckbIsCountHI = sender as CheckBox;
                if (!(ckbIsCountHI.DataContext is MedRegItemBase) || RegistrationObj == null)
                {
                    e.Handled = true;
                    return;
                }
                MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState
                //mUpdateInvoiceItem.RecordState = mUpdateInvoiceItem.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : mUpdateInvoiceItem.RecordState;
                //if (mUpdateInvoiceItem is PatientPCLRequestDetail)
                //{
                //    (mUpdateInvoiceItem as PatientPCLRequestDetail).PatientPCLRequest.RecordState = (mUpdateInvoiceItem as PatientPCLRequestDetail).PatientPCLRequest.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : (mUpdateInvoiceItem as PatientPCLRequestDetail).PatientPCLRequest.RecordState;
                //}
                CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                //▲====== #006
                CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
            }
        }
        //▲=====
        //▼====: #005
        //▼====: #004
        //▼====: #003
        //▼====: #002
        public void BtnName_Click(object sender, RoutedEventArgs e)
        {
            if (ViewCase == RegistrationViewCase.RegistrationRequestView)
            {
                Button CurrentButton = sender as Button;
                if (!(CurrentButton.DataContext is MedRegItemBase) || (CurrentButton.DataContext as PatientPCLRequestDetail).DeptLocation == null || (CurrentButton.DataContext as PatientPCLRequestDetail).PCLReqDetailCancelStaffID != 0 || (CurrentButton.DataContext as PatientPCLRequestDetail).MarkedAsDeleted)
                {
                    return;
                }
                PatientPCLRequestDetail CurrentPclRequest = CurrentButton.DataContext as PatientPCLRequestDetail;
                ICommonPreviewView aView = Globals.GetViewModel<ICommonPreviewView>();
                aView.PatientPCLReqID = (int)CurrentPclRequest.PatientPCLReqID;
                aView.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                aView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                GlobalsNAV.ShowDialog_V3(aView);
            }
            else
            {
                Button mbtnDiscount = sender as Button;
                if (!(mbtnDiscount.DataContext is MedRegItemBase) || (mbtnDiscount.DataContext as PatientPCLRequestDetail).DeptLocation == null || (mbtnDiscount.DataContext as PatientPCLRequestDetail).PCLReqDetailCancelStaffID != 0 || (mbtnDiscount.DataContext as PatientPCLRequestDetail).MarkedAsDeleted)
                {
                    return;
                }
                PatientPCLRequestDetail currentRegistrationDetail = mbtnDiscount.DataContext as PatientPCLRequestDetail;
                StringBuilder sb = new StringBuilder();
                string listID = "";
                sb.Append("<Root><IDList><PCLIDList><DeptData>");
                long? deptLocID = currentRegistrationDetail.DeptLocation.DeptLocationID;
                sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", deptLocID);
                if (!IsOldList)
                {
                    foreach (PatientPCLRequestDetail registrationDetail in PtPclReqDetailItems)
                    {
                        if (registrationDetail.DeptLocation != null && registrationDetail.DeptLocation.DeptLocationID == deptLocID)
                        {
                            if (listID == "")
                            {
                                listID += registrationDetail.ID;
                            }
                            else listID = listID + "," + registrationDetail.ID;
                        }
                    }
                }
                else
                {
                    string PCLRequestNumID = currentRegistrationDetail.PatientPCLRequest.PCLRequestNumID;
                    foreach (PatientPCLRequestDetail registrationDetail in PtPclReqDetailItems)
                    {
                        // 20200824 TNHX Sửa lỗi CLS không có phòng thì không xem in phiếu chỉ định được
                        if (registrationDetail.DeptLocation != null && registrationDetail.DeptLocation.DeptLocationID == deptLocID && registrationDetail.PatientPCLRequest.PCLRequestNumID == PCLRequestNumID)
                        {
                            if (listID == "")
                            {
                                listID += registrationDetail.ID;
                            }
                            else listID = listID + "," + registrationDetail.ID;
                        }
                    }
                }
                sb.AppendFormat("<IDList>{0}</IDList>", listID);
                sb.Append("</DeptData></PCLIDList></IDList></Root>");
                void onInitDlg(ICommonPreviewView reportVm)
                {
                    reportVm.Result = sb.ToString();
                    reportVm.eItem = ReportName.REGISTRATION_SPECIFY_VOTES_XML;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        //▲====: #002
        //▲====: #003
        //▲====: #004
        //▲====: #005
        public bool CanApplyIsOnPriceDiscount
        {
            get
            {
                //return !IsOldList && RegistrationObj != null && RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.IsOnPriceDiscount;
                return RegistrationObj != null && RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.IsOnPriceDiscount;
            }
        }
        public bool IsShowPrintDiscountButton
        {
            get
            {
                //return IsOldList || !CanApplyIsOnPriceDiscount;
                return !CanApplyIsOnPriceDiscount;
            }
        }
        public void NotifyOfCanApplyIsOnPriceDiscount()
        {
            NotifyOfPropertyChange(() => CanApplyIsOnPriceDiscount);
            NotifyOfPropertyChange(() => IsShowPrintDiscountButton);
        }
        public void txtDiscount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null || !(sender is TextBox))
            {
                return;
            }
            decimal mDiscountAmount = 0m;
            if (!decimal.TryParse((sender as TextBox).Text, out mDiscountAmount))
            {
                (sender as TextBox).Text = "0";
            }
            ((sender as TextBox).DataContext as MedRegItemBase).DiscountAmt = mDiscountAmount;
            RegistrationObj.ApplyDiscount(((sender as TextBox).DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
        }
        private RegistrationViewCase _ViewCase = RegistrationViewCase.RegistrationView;
        public RegistrationViewCase ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                if (_ViewCase == value)
                {
                    return;
                }
                _ViewCase = value;
                NotifyOfPropertyChange(() => ViewCase);
            }
        }
        #region Code Tích BH
        public bool EnableForCheckBH
        {
            get
            {
                return IsOldList && !Globals.ServerConfigSection.CommonItems.ChangeHIAfterSaveAndPayRule;
            }
        }
        #endregion

        //▼===== #007: Đôi event Checked của tích BH và MG sang event Click.
        public void ckbIsCountHI_Click(object source, object sender)
        {
            if (Globals.ServerConfigSection.CommonItems.ChangeHIAfterSaveAndPayRule)
            {
                CheckBox ckbIsCountHI = source as CheckBox;
                bool? copier = ckbIsCountHI.IsChecked;
                if (!(ckbIsCountHI.DataContext is MedRegItemBase) || RegistrationObj == null)
                {
                    return;
                }
                if (CheckPCLHIStatus())
                {
                    MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                    mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                    mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                    mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                    mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                    //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState                    
                    CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                    //▲===== #006
                    CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                    NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                    //CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                    if (mUpdateInvoiceItem.IsDiscounted)
                    {
                        RegistrationObj.ApplyDiscount(mUpdateInvoiceItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                    }
                    if (ckbIsCountHI.DataContext is PatientPCLRequestDetail)
                    {
                        Globals.EventAggregator.Publish(new ChangePCLHIStatus<PatientPCLRequestDetail>() { Item = ckbIsCountHI.DataContext as PatientPCLRequestDetail });
                    }
                }
                else
                {
                    ckbIsCountHI.IsChecked = !(bool)copier;
                }
            }
            else
            {
                CheckBox ckbIsCountHI = source as CheckBox;
                if (!(ckbIsCountHI.DataContext is MedRegItemBase) || RegistrationObj == null)
                {
                    return;
                }
                //20200602 TBL: BM 0038230: Nếu dịch vụ đã bị hủy thì không cho tick
                if (ckbIsCountHI.DataContext is PatientPCLRequestDetail)
                {
                    PatientPCLRequestDetail detail = ckbIsCountHI.DataContext as PatientPCLRequestDetail;
                    if(detail.MarkedAsDeleted || detail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI || detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        ckbIsCountHI.IsChecked = !ckbIsCountHI.IsChecked;
                        return;
                    }
                }
                MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState
                CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                //▲====== #006
                CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                //CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                if (mUpdateInvoiceItem.IsDiscounted)
                {
                    RegistrationObj.ApplyDiscount(mUpdateInvoiceItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                }
                if (ckbIsCountHI.DataContext is PatientPCLRequestDetail)
                {
                    Globals.EventAggregator.Publish(new ChangePCLHIStatus<PatientPCLRequestDetail>() { Item = ckbIsCountHI.DataContext as PatientPCLRequestDetail });
                }
            }
        }

        public void ckbDiscount_Click(object source, object sender)
        {
            if (!(source is CheckBox))
            {
                return;
            }
            CheckBox ckbDiscount = source as CheckBox;
            //20200602 TBL: BM 0038230: Nếu dịch vụ đã bị hủy thì không cho tick
            if (ckbDiscount.DataContext is PatientPCLRequestDetail)
            {
                PatientPCLRequestDetail detail = ckbDiscount.DataContext as PatientPCLRequestDetail;
                if (detail.MarkedAsDeleted || detail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI || detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    return;
                }
            }
            bool? copierChecked = ckbDiscount.IsChecked;
            if (!(ckbDiscount.DataContext is MedRegItemBase) || RegistrationObj == null || RegistrationObj.PromoDiscountProgramObj == null)
            {
                ckbDiscount.IsChecked = false;
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID != RegistrationObj.PromoDiscountProgramObj.PromoDiscProgID)
            {
                (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = 0;
            }
            if (RegistrationObj.PromoDiscountProgramObj.IsOnPriceDiscount)
            {
                (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = ckbDiscount.IsChecked.GetValueOrDefault(false);
                if (!(ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked)
                {
                    if ((ckbDiscount.DataContext as PatientPCLRequestDetail) != null && (ckbDiscount.DataContext as PatientPCLRequestDetail).PatientPCLReqID > 0)
                    {
                        if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                        {
                            ckbDiscount.IsChecked = !(bool)copierChecked;
                            (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                            RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                            CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                            return;
                        }
                    }
                }
                RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                if (copierChecked == true)
                {
                    ckbDiscount.IsChecked = true;
                }
                if (ckbDiscount.DataContext is PatientPCLRequestDetail)
                {
                    Globals.EventAggregator.Publish(new ChangeDiscountStatus<PatientPCLRequestDetail>() { Item = ckbDiscount.DataContext as PatientPCLRequestDetail });
                }
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).DiscountAmt == 0 && RegistrationObj.PromoDiscountProgramObj.DiscountPercent == 0)
            {
                ckbDiscount.IsChecked = false;
                return;
            }
            if (!ckbDiscount.IsChecked.GetValueOrDefault(false))
            {
                if ((ckbDiscount.DataContext as PatientPCLRequestDetail) != null && (ckbDiscount.DataContext as PatientPCLRequestDetail).PatientPCLReqID > 0)
                {
                    if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                    {
                        ckbDiscount.IsChecked = !(bool)copierChecked;
                        (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                    }
                }
            }
            if (ckbDiscount.DataContext is PatientPCLRequestDetail)
            {
                PatientPCLRequestDetail detail = ckbDiscount.DataContext as PatientPCLRequestDetail;
                //BLQ: Kiểm tra miễn giảm không có cls nào thì return
                if (RegistrationObj != null && RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems != null
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.PCLExamTypeID > 0).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //BLQ: Kiểm tra CLS đang check có trong phiếu miễn giảm không
                if (RegistrationObj != null && RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems != null
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.ObjPCLExamType != null && x.ObjPCLExamType.PCLExamTypeName == detail.PCLExamType.PCLExamTypeName).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
            CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
            if (ckbDiscount.DataContext is PatientPCLRequestDetail)
            {
                Globals.EventAggregator.Publish(new ChangeDiscountStatus<PatientPCLRequestDetail>() { Item = ckbDiscount.DataContext as PatientPCLRequestDetail });
            }
        }
        //▲===== #007
    }
}