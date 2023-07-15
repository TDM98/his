using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.ViewContracts.GuiContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using Service.Core.Common;
using System;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Text;
using eHCMSLanguage;

/*
 * 20181217 #001 TNHX:  [BM0005436] Create button to Show report PhieuMienGiam
 * 20181221 #002 TNHX:  [BM0005452] Apply re-print report PhieuChiDinh when click service name
 * 20181225 #003 TNHX:  [BM0005462] Re-make report PhieuChiDinh
 * 20181226 #004 TNHX:  [BM0005467] Can print PhieuChiDinh after save Service or PCL by click into item name
 * 20181227 #005 TNHX:  [BM0005470] Add condition print PhieuChiDinh (BN tra sau - khong in lai) && Khong in doi voi DV + CLS huy/hoan tien
 * 20200113 #006 TTM:   BM 0021787: Fix lỗi không lưu thông tin miễn giảm và bảo hiểm nếu lưu và trả tiền cho dịch vụ đã được lưu và thay đổi thông tin miễn giảm và bảo hiểm.
 * 20200205 #007 TTM:   BM 0022818: Fix 1 số lỗi liên quan đến miễn giảm.
 *                      1. Khi miễn giảm trực tiếp chọn số tiền miễn giảm rồi bỏ miễn giảm thì số tiền không trở về 0.
 *                      2. Miễn giảm trực tiếp => Lưu lại => Bỏ tích MG => Chọn lại miễn giảm theo % => Không cho tích vào.
 * 20200212 #008 TTM:   BM 0023912: Fix lỗi khi lưu (Chưa trả tiền) tích BH thay đổi => giá trị của dòng bị sai (Dịch vụ).
 */
namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Quản lý danh sách dịch vụ của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    [Export(typeof(IOutPatientServiceManage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientServiceManageViewModel : Conductor<object>, IOutPatientServiceManage
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        [ImportingConstructor]
        public OutPatientServiceManageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        IOutPatientServiceManageView _currentView;
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _currentView = view as IOutPatientServiceManageView;
            if (_currentView != null)
            {
                _currentView.SetVisibilityBindingForHiColumns();
            }
        }

        public void UpdateServiceItemList(List<PatientRegistrationDetail> updatedList)
        {
            if (_registrationDetails == null)
            {
                _registrationDetails = new ObservableCollection<PatientRegistrationDetail>(updatedList);
                _cvs_RegDetailItems = new CollectionViewSource { Source = _registrationDetails };
                CV_RegDetailItems = (CollectionView)_cvs_RegDetailItems.View;
                if (IsOldList)
                {
                    CV_RegDetailItems.GroupDescriptions.Add(new PropertyGroupDescription("RefMedicalServiceItem.RefMedicalServiceType"));
                }
                NotifyOfPropertyChange(() => CV_RegDetailItems);
            }
            else
            {
                _registrationDetails.Clear();
                updatedList.ForEach(listItem => _registrationDetails.Add(listItem));
                CV_RegDetailItems.Refresh();
            }
        }

        #region PROPERTIES

        private CollectionViewSource _cvs_RegDetailItems;

        public CollectionView CV_RegDetailItems { get; set; }

        private ObservableCollection<PatientRegistrationDetail> _registrationDetails;
        //private aEMR.Common.PagedCollectionView.PagedCollectionView _registrationDetails;

        //public aEMR.Common.PagedCollectionView.PagedCollectionView RegistrationDetails
        public ObservableCollection<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _registrationDetails; }
            set
            {
                if (_registrationDetails != value)
                {
                    _registrationDetails = value;
                    NotifyOfPropertyChange(() => RegistrationDetails);
                    AllServiceItemsSelected = IsAllRegDetailsSelected();
                    if (_registrationDetails != null)
                    {
                        foreach (var item in _registrationDetails)
                        {
                            var details = item as INotifyPropertyChanged;
                            if (details != null)
                            {
                                details.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(details_PropertyChanged).Handler;
                            }
                        }
                    }
                }
            }
        }

        public void details_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                AllServiceItemsSelected = IsAllRegDetailsSelected();
            }
        }

        private bool _allServiceItemsSelected;
        public bool AllServiceItemsSelected
        {
            get { return _allServiceItemsSelected; }
            set
            {
                if (_allServiceItemsSelected != value)
                {
                    _allServiceItemsSelected = value;
                    NotifyOfPropertyChange(() => AllServiceItemsSelected);
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

        private bool _CanDelete = true;
        public bool CanDelete
        {
            get { return _CanDelete; }
            set
            {
                _CanDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        private bool _IsPerformingTMVFunctionsA = Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA;
        public bool IsPerformingTMVFunctionsA
        {
            get
            {
                return _IsPerformingTMVFunctionsA;
            }
            set
            {
                if (_IsPerformingTMVFunctionsA == value)
                    return;
                _IsPerformingTMVFunctionsA = value;
                NotifyOfPropertyChange(() => IsPerformingTMVFunctionsA);
            }
        }
        #endregion

        #region COMMANDS
        public void RemoveServiceDetailsCmd(object sender, object eventArgs)
        {
            if (!CanDelete)
            {
                return;
            }
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                Globals.EventAggregator.Publish(new RemoveItem<PatientRegistrationDetail>() { Item = elem.DataContext as PatientRegistrationDetail });
            }
        }

        public void ToggleSelectAllRegistrationDetails(object sender, RoutedEventArgs eventArgs)
        {
            CheckBox chk = sender as CheckBox;
            bool check = chk.IsChecked.Value;
            if (_registrationDetails != null)
            {
                foreach (IEntityState item in _registrationDetails)
                {
                    item.IsChecked = check;
                }
            }
        }
        #endregion

        private bool IsAllRegDetailsSelected()
        {
            if (_registrationDetails == null || _registrationDetails.Count == 0)
            {
                return false;
            }

            return _registrationDetails.Cast<object>().All(t => ((IEntityState)t).IsChecked);
        }

        public void gridRegDetails_Click(object sender, EventArgs<object> eventArgs)
        {
            if (currentColumn == null)
            {
                return;
            }

            var regDetail = eventArgs.Value as PatientRegistrationDetail;

            if (regDetail == null || regDetail.PtRegDetailID > 0)
            {
                return;
            }

            if (currentColumn.DisplayIndex == (int)comlumIndex.Phong
            && regDetail.RefMedicalServiceItem.ServiceMainTime != (long)AllLookupValues.V_ServicePrice.Changeable)
            {
                //KMx: Nếu tài khoản thường và không được cấp quyền gán STT thì return (06/11/2014 11:52).
                if (!Globals.ServerConfigSection.OutRegisElements.AssignSequenceNumberManually
                    && Globals.isAccountCheck
                    && !Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mRegister,
                                            (int)oRegistrionEx.mDangKyDV_DichVuMoi_GanSTT, (int)ePermission.mView))
                {
                    return;
                }

                //var vm = Globals.GetViewModel<IRegistrationDetail>();
                //vm.RegistrationDetail = eventArgs.Value as PatientRegistrationDetail;
                //Globals.ShowDialog(vm as Conductor<object>);
                Action<IRegistrationDetail> onInitDlg = (vm) =>
                {
                    vm.RegistrationDetail = eventArgs.Value as PatientRegistrationDetail;
                };
                GlobalsNAV.ShowDialog<IRegistrationDetail>(onInitDlg);
            }

            if (currentColumn.DisplayIndex == (int)comlumIndex.Gia)
            {
                if (regDetail.RefMedicalServiceItem.ServiceMainTime != (long)AllLookupValues.V_ServicePrice.Changeable)
                {
                    return;
                }

                Coroutine.BeginExecute(PriceAdapt(regDetail));
            }
        }

        private enum comlumIndex
        {
            None = 0,
            Phong = 6,
            Gia = 8,
        }

        DataGridColumn currentColumn { get; set; }

        public void gridServices_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((sender) != null)
            {
                currentColumn = ((DataGrid)sender).CurrentColumn;
            }
        }

        public IEnumerator<IResult> PriceAdapt(PatientRegistrationDetail ptRegDetail)
        {
            var priceAdapt = new PriceAdaptShowDialogTask(ptRegDetail.RefMedicalServiceItem.MedServiceName, ptRegDetail.InvoicePrice);
            yield return priceAdapt;
            if (priceAdapt.IsOk)
            {
                ptRegDetail.InvoicePrice = priceAdapt.NewPrice;
                ptRegDetail.PatientPayment = ptRegDetail.ChargeableItem.NormalPrice = priceAdapt.NewPrice;
                ptRegDetail.SpecialNote = priceAdapt.Comments;
                ptRegDetail.MedServiceName = ptRegDetail.RefMedicalServiceItem.MedServiceName + " - " + priceAdapt.Comments;
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
                    if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.Yes)
                    {
                        RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((sender as CheckBox).IsChecked == true));
                    }
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
            mPromoDiscountProgramEdit.PtRegistrationID = (mbtnDiscount.DataContext as PatientRegistrationDetail).PtRegistrationID.Value;
            //▲====: #001
            mPromoDiscountProgramEdit.PromoDiscountProgramObj = new PromoDiscountProgram { PromoDiscProgID = (mbtnDiscount.DataContext as MedRegItemBase).PromoDiscProgID.Value, V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU };
            GlobalsNAV.ShowDialog_V3<IPromoDiscountProgramEdit>(mPromoDiscountProgramEdit);
        }
        //▼===== 20190105 TTM: Kiểm tra thông tin phiếu xuất của dịch vụ khám bệnh.
        public bool CheckStatusOutwardDrugs(MedRegItemBase medItem)
        {
            if (medItem == null || RegistrationObj == null)
            {
                return false;
            }
            if (RegistrationObj.ConfirmHIStaffID > 0)
            {
                MessageBox.Show(eHCMSResources.Z2969_G1_DaXacNhanQLKhongThayDoiThongTinDV);
                return false;
            }
            if (RegistrationObj.AllSaveOutwardDrugs != null && RegistrationObj.AllSaveOutwardDrugs.Count > 0)
            {
                foreach (var item in RegistrationObj.DrugInvoices)
                {
                    if ((medItem as PatientRegistrationDetail).prescriptionIssueHistory != null && (medItem as PatientRegistrationDetail).prescriptionIssueHistory.IssueID == item.SelectedPrescription.IssueID && item.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                    {
                        MessageBox.Show(eHCMSResources.Z2970_G1_DichVuCoToaChuaHuyPhieuXuat);
                        return false;
                    }
                }
            }
            //▼===== 20200115 TTM: Bổ sung thêm điều kiện không cho dịch vụ không thuộc bảo hiểm được tích lại BH.
            if (RegistrationObj.HisID == null ||
                ((medItem as PatientRegistrationDetail).ChargeableItem.HIAllowedPriceNew == 0))
            {
                MessageBox.Show(eHCMSResources.Z2971_G1_KhongTinhBHDichVuKhongThuocDMBH);
                return false;
            }
            //▲=====
            return true;
        }
        //▲===== 
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
                if (CheckStatusOutwardDrugs(ckbIsCountHI.DataContext is PatientRegistrationDetail ? ckbIsCountHI.DataContext as MedRegItemBase : null))
                {
                    MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                    mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                    mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                    mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                    mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                    //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState
                    //mUpdateInvoiceItem.RecordState = mUpdateInvoiceItem.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : mUpdateInvoiceItem.RecordState;
                    CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                    //▲===== #006
                    CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                    NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                    CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                    if (ckbIsCountHI.DataContext is PatientRegistrationDetail)
                    {
                        Globals.EventAggregator.Publish(new ChangeHIStatus<PatientRegistrationDetail>() { Item = ckbIsCountHI.DataContext as PatientRegistrationDetail });
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
                CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                //▲===== #006
                CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
            }
        }
        //▼====: #005
        //▼====: #004
        //▼====: #003
        //▼====: #002
        public void BtnName_Click(object sender, RoutedEventArgs e)
        {
            Button mbtnDiscount = sender as Button;
            if (!(mbtnDiscount.DataContext is MedRegItemBase) || (mbtnDiscount.DataContext as PatientRegistrationDetail).PtRegDetailID == 0 || (mbtnDiscount.DataContext as PatientRegistrationDetail).RegDetailCancelStaffID != 0 || (mbtnDiscount.DataContext as PatientRegistrationDetail).MarkedAsDeleted)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            PatientRegistrationDetail currentRegistrationDetail = mbtnDiscount.DataContext as PatientRegistrationDetail;
            long? deptLocID = currentRegistrationDetail.DeptLocID;
            string listID = "";
            sb.Append("<Root><IDList><ServiceIDList><DeptData>");
            sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", deptLocID);
            foreach (PatientRegistrationDetail registrationDetail in RegistrationDetails)
            {
                if (registrationDetail.DeptLocID == deptLocID && registrationDetail.RegDetailCancelStaffID == 0 && !registrationDetail.MarkedAsDeleted)
                {
                    if (listID == "")
                    {
                        listID += registrationDetail.ID;
                    }
                    else listID = listID + "," + registrationDetail.ID;
                }
            }
            sb.AppendFormat("<IDList>{0}</IDList>", listID);
            sb.Append("</DeptData></ServiceIDList></IDList></Root>");
            void onInitDlg(ICommonPreviewView reportVm)
            {
                reportVm.Result = sb.ToString();
                reportVm.eItem = ReportName.REGISTRATION_SPECIFY_VOTES_XML;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
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
        #region Code Tích BH

        public bool EnableForCheckBH
        {
            get
            {
                return IsOldList && !Globals.ServerConfigSection.CommonItems.ChangeHIAfterSaveAndPayRule;
            }
        }
        public void ckbIsChecked_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            if (!(ckbIsChecked.DataContext is MedRegItemBase) || RegistrationObj == null)
            {
                return;
            }
            MedRegItemBase mUpdateInvoiceItem = (ckbIsChecked.DataContext as MedRegItemBase);
            mUpdateInvoiceItem.IsChecked = ckbIsChecked.IsChecked.GetValueOrDefault(true);
        }
        //▼===== #007: Chuyển từ event Checked => event Click của tích BH và tích MG.
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
                if (CheckStatusOutwardDrugs(ckbIsCountHI.DataContext is PatientRegistrationDetail ? ckbIsCountHI.DataContext as MedRegItemBase : null))
                {
                    MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                    mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                    if (mUpdateInvoiceItem is PatientRegistrationDetail)
                    {
                        PatientRegistrationDetail detail = mUpdateInvoiceItem as PatientRegistrationDetail;
                        if (!detail.IsCountHI && detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID > 0)
                        {
                            //20200530 TBL: Nếu bỏ tick BH của dịch vụ là Ekip đầu tiên thì những dịch vụ trong cùng Ekip sẽ bỏ hết Ekip ra 
                            if (detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.DauTien && MessageBox.Show(eHCMSResources.Z3021_G1_MsgXoaEkipDauTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                long V_Ekip = detail.V_Ekip.LookupID; //20200602 TBL: Cần biết Ekip của dịch vụ đang bỏ tick BH để tìm những dịch vụ có cùng Ekip
                                foreach (PatientRegistrationDetail item in RegistrationObj.AllSaveRegistrationDetails)
                                {
                                    if (item.V_Ekip != null && item.V_Ekip.LookupID == V_Ekip)
                                    {
                                        item.V_Ekip = new Lookup();
                                        item.V_EkipIndex = new Lookup();
                                        item.HIPaymentPercent = 1;
                                        CommonGlobals.ChangeItemsRecordState(item);
                                        CommonGlobals.ChangeHIBenefit(RegistrationObj, item, item);
                                    }
                                }
                            }
                            else if (detail.V_EkipIndex.LookupID != (long)AllLookupValues.V_EkipIndex.DauTien && MessageBox.Show(eHCMSResources.Z2977_G1_XoaEkip, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                detail.V_Ekip = new Lookup();
                                detail.V_EkipIndex = new Lookup();
                                detail.HIPaymentPercent = 1;
                            }
                            else
                            {
                                detail.IsCountHI = true;
                                return;
                            }
                        }
                    }
                    mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                    mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                    mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                    //SetHIPaymentPercentForService(ckbIsCountHI.DataContext is PatientRegistrationDetail ? ckbIsCountHI.DataContext as PatientRegistrationDetail : null, (bool)ckbIsCountHI.IsChecked);
                    CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                    CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                    NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                    //CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                    CommonGlobals.CorrectRegistrationDetails_V2(RegistrationObj);
                    if (ckbIsCountHI.DataContext is PatientRegistrationDetail)
                    {
                        Globals.EventAggregator.Publish(new ChangeHIStatus<PatientRegistrationDetail>() { Item = ckbIsCountHI.DataContext as PatientRegistrationDetail });
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
                if (ckbIsCountHI.DataContext is PatientRegistrationDetail)
                {
                    PatientRegistrationDetail detail = ckbIsCountHI.DataContext as PatientRegistrationDetail;
                    if ((detail.PtRegDetailID > 0 && detail.PaidTime != null && detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && (long)detail.RecordState != 4)
                        || (detail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && detail.PaidTime != null))
                    {
                        ckbIsCountHI.IsChecked = !ckbIsCountHI.IsChecked;
                        return;
                    }
                    if (detail.PtRegDetailID > 0 && detail.PaidTime != null && (detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN || detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.HOAN_TAT))
                    {
                        MessageBox.Show(eHCMSResources.Z3022_G1_MsgTickBH);
                        ckbIsCountHI.IsChecked = !ckbIsCountHI.IsChecked;
                        return;
                    }
                }
                MedRegItemBase mUpdateInvoiceItem = (ckbIsCountHI.DataContext as MedRegItemBase);
                mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
                if (mUpdateInvoiceItem is PatientRegistrationDetail)
                {
                    PatientRegistrationDetail detail = mUpdateInvoiceItem as PatientRegistrationDetail;
                    if (!detail.IsCountHI && detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID > 0)
                    {
                        //20200530 TBL: Nếu bỏ tick BH của dịch vụ là Ekip đầu tiên thì những dịch vụ trong cùng Ekip sẽ bỏ hết Ekip ra 
                        if (detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.DauTien && MessageBox.Show(eHCMSResources.Z3021_G1_MsgXoaEkipDauTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            long V_Ekip = detail.V_Ekip.LookupID; //20200602 TBL: Cần biết Ekip của dịch vụ đang bỏ tick BH để tìm những dịch vụ có cùng Ekip
                            foreach (PatientRegistrationDetail item in RegistrationObj.AllSaveRegistrationDetails)
                            {
                                if (item.V_Ekip != null && item.V_Ekip.LookupID == V_Ekip)
                                {
                                    item.V_Ekip = new Lookup();
                                    item.V_EkipIndex = new Lookup();
                                    item.HIPaymentPercent = 1;
                                    CommonGlobals.ChangeItemsRecordState(item);
                                    CommonGlobals.ChangeHIBenefit(RegistrationObj, item, item);
                                }
                            }
                        }
                        else if (detail.V_EkipIndex.LookupID != (long)AllLookupValues.V_EkipIndex.DauTien && MessageBox.Show(eHCMSResources.Z2977_G1_XoaEkip, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            detail.V_Ekip = new Lookup();
                            detail.V_EkipIndex = new Lookup();
                            detail.HIPaymentPercent = 1;
                        }
                        else
                        {
                            detail.IsCountHI = true;
                            return;
                        }
                    }
                }
                mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? (mUpdateInvoiceItem.HIBenefit.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.HIBenefit : RegistrationObj.PtInsuranceBenefit) : null;
                mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? RegistrationObj.HisID : null;
                mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? (mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPriceNew : mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice) : null;
                //SetHIPaymentPercentForService(ckbIsCountHI.DataContext is PatientRegistrationDetail ? ckbIsCountHI.DataContext as PatientRegistrationDetail : null, (bool)ckbIsCountHI.IsChecked);
                CommonGlobals.ChangeItemsRecordState(mUpdateInvoiceItem);
                CommonGlobals.ChangeHIBenefit(RegistrationObj, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
                NotifyOfPropertyChange(() => mUpdateInvoiceItem.IsCountHI);
                //CommonGlobals.CorrectRegistrationDetails(RegistrationObj);
                CommonGlobals.CorrectRegistrationDetails_V2(RegistrationObj);
                if (ckbIsCountHI.DataContext is PatientRegistrationDetail)
                {
                    Globals.EventAggregator.Publish(new ChangeHIStatus<PatientRegistrationDetail>() { Item = ckbIsCountHI.DataContext as PatientRegistrationDetail });
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
            if (ckbDiscount.DataContext is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = ckbDiscount.DataContext as PatientRegistrationDetail;
                if ((detail.PtRegDetailID > 0 && detail.PaidTime != null && detail.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && (long)detail.RecordState != 4)
                     || (detail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && detail.PaidTime != null))
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
            //▼===== #007:  Set lại vì nếu trường hợp dịch vụ đã lưu kèm thông tin miễn giảm mà chọn loại miễn giảm khác trong hàm ApplyDiscount sẽ lấy miễn giảm đang lưu ra làm việc.
            //              do PromoDiscProgID của dịch vụ đang chọn = với loại miễn giảm đã lưu trong danh sách miễn giảm của bệnh nhân.
            if ((ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID != RegistrationObj.PromoDiscountProgramObj.PromoDiscProgID)
            {
                (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = 0;
            }
            //▲===== #007
            if (RegistrationObj.PromoDiscountProgramObj.IsOnPriceDiscount)
            {
                (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = ckbDiscount.IsChecked.GetValueOrDefault(false);
                //▼===== #007: Lý do tích bỏ ra nhưng không có apply thông tin bằng hàm ApplyDiscount => Màn hình thấy đã bỏ thực tế chưa tính lại.
                if (!(ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked)
                {
                    if ((ckbDiscount.DataContext as PatientRegistrationDetail).PtRegDetailID > 0)
                    {
                        if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                        {
                            ckbDiscount.IsChecked = !(bool)copierChecked;
                            (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                            RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                            CommonGlobals.CorrectRegistrationDetails_V2(RegistrationObj);
                            CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                            return;
                        }
                    }
                }
                RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                CommonGlobals.CorrectRegistrationDetails_V2(RegistrationObj);
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                if (copierChecked == true)
                {
                    ckbDiscount.IsChecked = true;
                }
                if (ckbDiscount.DataContext is PatientRegistrationDetail)
                {
                    Globals.EventAggregator.Publish(new ChangeDiscountStatus<PatientRegistrationDetail>() { Item = ckbDiscount.DataContext as PatientRegistrationDetail });
                }
                return;
                //▲===== #007
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).DiscountAmt == 0 && RegistrationObj.PromoDiscountProgramObj.DiscountPercent == 0)
            {
                ckbDiscount.IsChecked = false;
                return;
            }
            if (!ckbDiscount.IsChecked.GetValueOrDefault(false))
            {
                if ((ckbDiscount.DataContext as PatientRegistrationDetail).PtRegDetailID > 0)
                {
                    if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                    {
                        ckbDiscount.IsChecked = !(bool)copierChecked;
                        (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                    }
                }
            }
            if (ckbDiscount.DataContext is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = ckbDiscount.DataContext as PatientRegistrationDetail;
                //BLQ: Kiểm tra không có dịch vụ thì return
                if (RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems != null
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.MedServiceID > 0).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //BLQ: Kiểm tra dịch vụ đang check có trong phiếu miễn giảm không
                if (RegistrationObj.PromoDiscountProgramObj != null && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems != null
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && RegistrationObj.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.ObjRefMedicalServiceItem != null && x.ObjRefMedicalServiceItem.MedServiceName == detail.MedServiceName).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            RegistrationObj.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
            CommonGlobals.CorrectRegistrationDetails_V2(RegistrationObj);
            //▼===== #006: Khi thay đổi thông tin miễn giảm thì set giá trị cho RecoreState
            CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
            //▲===== #006
            if (ckbDiscount.DataContext is PatientRegistrationDetail)
            {
                Globals.EventAggregator.Publish(new ChangeDiscountStatus<PatientRegistrationDetail>() { Item = ckbDiscount.DataContext as PatientRegistrationDetail });
            }
        }
        //▲===== #007
        #endregion
        //▼===== #008
        public void SetHIPaymentPercentForService(PatientRegistrationDetail SelectedItem, bool Checked)
        {
            if (SelectedItem == null) return;
            if (!Checked)
            {
                SelectedItem.HIPaymentPercent = 0;
                return;
            }

            List<PatientRegistrationDetail> ListPatientRegistrationDetail = RegistrationObj.AllSaveRegistrationDetails.Where(x => x.IsCountHI
            && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                && x.RecordState != RecordState.DELETED
                && !x.MarkedAsDeleted
                && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                && x.HisID.HasValue && x.HisID.Value > 0
                && x.HIAllowedPrice.HasValue
                && x.HIAllowedPrice.Value > 0).ToList();

            if (ListPatientRegistrationDetail.Count > 0)
            {
                var firstItem = ListPatientRegistrationDetail.First(x => x.IsCountHI);
                if (firstItem.PaidTime == null)
                {
                    firstItem.ChangeHIBenefit(firstItem.HIBenefit.GetValueOrDefault(0), RegistrationObj, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                    firstItem.ApplyHIPaymentPercent(1.0, RegistrationObj, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                }
            }
            if (Checked && SelectedItem.HIPaymentPercent == 0)
            {
                List<long> SpecialCollection = new List<long>();
                foreach (var aItem in ListPatientRegistrationDetail)
                {
                    if ((aItem.RefMedicalServiceItem != null && aItem.RefMedicalServiceItem.V_SpecialistType > 0 && SpecialCollection.Contains(aItem.RefMedicalServiceItem.V_SpecialistType))
                        || (aItem.RefMedicalServiceItem == null && aItem.RefMedicalServiceItem.V_SpecialistType == 0))
                    {
                        //Không tính quyền lợi BHYT cho các dịch vụ không có chuyên khoa hoặc bị trùng chuyên khoa với các dịch vụ trước
                        if (aItem.PaidTime == null)
                        {
                            aItem.ApplyHIPaymentPercent(0, RegistrationObj, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                        }
                    }
                    else
                    {
                        if (SpecialCollection.Count > 0)
                        {
                            if (Globals.ServerConfigSection.HealthInsurances.HIPercentOnDifDept.Length > SpecialCollection.Count - 1)
                            {
                                //Tính quyền lợi BHYT theo tỉ lệ cấu hình cho dịch vụ khác chuyên khoa theo thứ tự
                                if (aItem.PaidTime == null)
                                {
                                    aItem.ApplyHIPaymentPercent(Math.Round(Globals.ServerConfigSection.HealthInsurances.HIPercentOnDifDept[SpecialCollection.Count - 1], 4), RegistrationObj, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                                }
                            }
                            else if (aItem.PaidTime == null)
                            {
                                //Không tính quyền lợi BHYT cho Dịch vụ vượt quá số lượng chuyên khoa cho phép một lần khám
                                aItem.ApplyHIPaymentPercent(0, RegistrationObj, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                            }
                        }
                        SpecialCollection.Add(aItem.RefMedicalServiceItem.V_SpecialistType);
                    }
                }
            }
        }
        //▲===== #008

        public void CheckClearData(MedRegItemBase SelectedRegistrationItem, out bool IsClearEkip, out bool IsClearMG, out string ErrorStr)
        {
            IsClearEkip = false;
            IsClearMG = false;
            ErrorStr = "";
            if (SelectedRegistrationItem == null)
            {
                return;
            }
            if (SelectedRegistrationItem is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = SelectedRegistrationItem as PatientRegistrationDetail;
                if (!detail.IsCountHI && detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID > 0)
                {
                    ErrorStr += string.Format("\n - {0}", eHCMSResources.Z2977_G1_XoaEkip);
                    IsClearEkip = true;
                }
                if (!detail.IsCountHI && detail.DiscountAmt > 0 || (detail.DiscountAmt == 0 && detail.PromoDiscProgID > 0))
                {
                    ErrorStr += string.Format("\n - {0}", eHCMSResources.Z2976_G1_XoaThongTinMienGiam);
                    IsClearMG = true;
                }
            }
        }
        public void ClearDataMGOrEkip(MedRegItemBase mUpdateInvoiceItem)
        {
            bool IsClearEkip = false;
            bool IsClearMG = false;
            string ErrorStr = "";
            if (mUpdateInvoiceItem is PatientRegistrationDetail)
            {
                CheckClearData(mUpdateInvoiceItem, out IsClearEkip, out IsClearMG, out ErrorStr);
                if (IsClearMG || IsClearEkip)
                {
                    if (!mUpdateInvoiceItem.IsCountHI && !string.IsNullOrEmpty(ErrorStr) && MessageBox.Show(ErrorStr, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (IsClearEkip)
                        {
                            (mUpdateInvoiceItem as PatientRegistrationDetail).V_Ekip = new Lookup();
                            (mUpdateInvoiceItem as PatientRegistrationDetail).V_EkipIndex = new Lookup();
                        }
                        if (IsClearMG)
                        {
                            mUpdateInvoiceItem.PromoDiscProgID = 0;
                            RegistrationObj.ApplyDiscount(mUpdateInvoiceItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, true);
                        }
                    }
                    else
                    {
                        (mUpdateInvoiceItem as PatientRegistrationDetail).IsCountHI = true;
                    }
                }
            }
        }
        public void ChangeItemsRecordState(PatientRegistration RegistrationObj)
        {
            if (RegistrationObj == null)
            {
                return;
            }
            List<PatientRegistrationDetail> ListPtRegistrationDetails = RegistrationObj.AllSaveRegistrationDetails.Where(x => x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                && x.RecordState != RecordState.DELETED
                && !x.MarkedAsDeleted
                && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).ToList();
            foreach (PatientRegistrationDetail item in ListPtRegistrationDetails)
            {
                if (item.PaidTime == null || IsOldList)
                {
                    item.RecordState = item.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : item.RecordState;
                }
            }
        }
    }
}