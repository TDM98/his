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
using System.Windows.Media;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.Converters;
using aEMR.Common.BaseModel;
/*
 * 20181018 #001 TTM:   BM0002188 Bỏ việc Focus trong AutoComplete khi người dùng tìm kiếm theo tên thuốc. Điều này không cần thiết vì sẽ gây khó khăn cho người dùng khi tìm kiếm thuốc 
 *                      (Do tự động Focus và bôi đen => không gõ ký tự thứ 2 đc).
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEditPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditPrescriptionViewModel : ViewModelBase, IEditPrescription
         , IHandle<EditChooseBatchNumberVisitorEvent>
        , IHandle<EditChooseBatchNumberVisitorResetQtyEvent>
        , IHandle<PharmacyCloseSearchEditPrescriptionEvent>
    {
        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInfoPatient = false;
        public bool isLoadingInfoPatient
        {
            get { return _isLoadingInfoPatient; }
            set
            {
                if (_isLoadingInfoPatient != value)
                {
                    _isLoadingInfoPatient = value;
                    NotifyOfPropertyChange(() => isLoadingInfoPatient);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator1 = false;
        public bool isLoadingFullOperator1
        {
            get { return _isLoadingFullOperator1; }
            set
            {
                if (_isLoadingFullOperator1 != value)
                {
                    _isLoadingFullOperator1 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator1);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator2 = false;
        public bool isLoadingFullOperator2
        {
            get { return _isLoadingFullOperator2; }
            set
            {
                if (_isLoadingFullOperator2 != value)
                {
                    _isLoadingFullOperator3 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator2);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator3 = false;
        public bool isLoadingFullOperator3
        {
            get { return _isLoadingFullOperator3; }
            set
            {
                if (_isLoadingFullOperator3 != value)
                {
                    _isLoadingFullOperator3 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator3);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private bool _isLoadingInfoReg = false;
        public bool isLoadingInfoReg
        {
            get { return _isLoadingInfoReg; }
            set
            {
                if (_isLoadingInfoReg != value)
                {
                    _isLoadingInfoReg = value;
                    NotifyOfPropertyChange(() => isLoadingInfoReg);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }




        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingFullOperator1 || isLoadingFullOperator2 || isLoadingFullOperator3 || isLoadingInfoReg || isLoadingInfoPatient || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EditPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetStaffLogin();

            RefeshData();

            //20191211 TTM: Không set default mặc định mà phải load dữ liệu theo phiếu xuất cập nhật.
            //SetDefaultForStore();
        }
        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice();
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;

            SearchCriteria = null;
            SearchCriteria = new PrescriptionSearchCriteria();

            SearchInvoiceCriteria = null;
            SearchInvoiceCriteria = new SearchOutwardInfo();
            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;

            PatientInfo = null;
            SelectedPrescription = null;
            SelectedPrescription = new Prescription();
            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }


        }
        private void ClearData()
        {
            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }

        }

        public void SetDefaultForStore()
        {
            //if (StoreCbx != null)
            //{
            //    StoreID = StoreCbx.FirstOrDefault().StoreID;
            //}
            if (SelectedOutInvoice != null)
            {
                StoreID = (long)SelectedOutInvoice.StoreID; 
            }
        }

        #region Properties Member

        public class ListDrugAndQtySell
        {
            public long DrugID;
            public int xban;
        }
        private ObservableCollection<ListDrugAndQtySell> ListDrugTemp;

        private string _strNgayDung;
        public string strNgayDung
        {
            get { return _strNgayDung; }
            set
            {
                if (_strNgayDung != value)
                {
                    _strNgayDung = value;
                    NotifyOfPropertyChange(() => strNgayDung);
                }
            }
        }


        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private SearchOutwardInfo _SearchInvoiceCriteria;
        public SearchOutwardInfo SearchInvoiceCriteria
        {
            get
            {
                return _SearchInvoiceCriteria;
            }
            set
            {
                if (_SearchInvoiceCriteria != value)
                {
                    _SearchInvoiceCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchInvoiceCriteria);
            }
        }

        private PrescriptionSearchCriteria _SearchCriteria;
        public PrescriptionSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private Prescription _SelectedPrescription;
        public Prescription SelectedPrescription
        {
            get
            {
                return _SelectedPrescription;
            }
            set
            {
                if (_SelectedPrescription != value)
                {
                    _SelectedPrescription = value;
                }
                NotifyOfPropertyChange(() => SelectedPrescription);
            }
        }

        private ObservableCollection<OutwardDrug> _OutwardDrugsCopy;
        public ObservableCollection<OutwardDrug> OutwardDrugsCopy
        {
            get
            {
                return _OutwardDrugsCopy;
            }
            set
            {
                if (_OutwardDrugsCopy != value)
                {
                    _OutwardDrugsCopy = value;
                }
                NotifyOfPropertyChange(() => OutwardDrugsCopy);
            }
        }

        private OutwardDrugInvoice _SelectedOutInvoiceCopy;
        public OutwardDrugInvoice SelectedOutInvoiceCopy
        {
            get
            {
                return _SelectedOutInvoiceCopy;
            }
            set
            {
                if (_SelectedOutInvoiceCopy != value)
                {
                    _SelectedOutInvoiceCopy = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoiceCopy);
            }
        }

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
            }
        }

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
                if (_Visibility == Visibility.Collapsed)
                {
                    IsVisibility = Visibility.Visible;
                }
                else
                {
                    IsVisibility = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                }
                NotifyOfPropertyChange(() => IsVisibility);
            }
        }

        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private bool _IsOther = false;
        public bool IsOther
        {
            get
            {
                return _IsOther;
            }
            set
            {
                _IsOther = value;
                NotifyOfPropertyChange(() => IsOther);
            }
        }
        #endregion


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;
        private bool _bTinhTien = true;
        private bool _bLuuTinhTien = true;
        private bool _bSuaToa = true;
        public bool bSuaToa
        {
            get
            {
                return _bSuaToa;
            }
            set
            {
                if (_bSuaToa == value)
                    return;
                _bSuaToa = value;
                NotifyOfPropertyChange(() => bSuaToa);
            }
        }
        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }
        public bool bTinhTien
        {
            get
            {
                return _bTinhTien;
            }
            set
            {
                if (_bTinhTien == value)
                    return;
                _bTinhTien = value;
            }
        }
        public bool bLuuTinhTien
        {
            get
            {
                return _bLuuTinhTien;
            }
            set
            {
                if (_bLuuTinhTien == value)
                    return;
                _bLuuTinhTien = value;
            }
        }
        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        #region Load Danh Sach Kho Ngoai Tru Member
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;

            //20191211 TTM: Không set default mặc định mà phải load dữ liệu theo phiếu xuất cập nhật.
            //SetDefaultForStore();

            isLoadingGetStore = false;
            yield break;
        }

        #endregion

        #region Tim Toa Thuoc Load Len De Ban member

        public void Search_KeyUp_Pre(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchPrescription(0, Globals.PageSize);
            }
        }
        public void Search_KeyUp_PreHICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = (sender as TextBox).Text;
                }
                SearchPrescription(0, Globals.PageSize);
            }
        }

        private void SearchPrescription(int PageIndex, int PageSize)
        {
            int Total = 0;
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchPrescription(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchPrescription(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {

                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    //var proAlloc = Globals.GetViewModel<IPrescriptionSearch>();
                                    //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                    //proAlloc.FormType = 2;
                                    //proAlloc.PrescriptionList.Clear();
                                    //proAlloc.PrescriptionList.TotalItemCount = Total;
                                    //proAlloc.PrescriptionList.PageIndex = 0;
                                    //foreach (Prescription p in results)
                                    //{
                                    //    proAlloc.PrescriptionList.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<IPrescriptionSearch> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.FormType = 2;
                                        proAlloc.PrescriptionList.Clear();
                                        proAlloc.PrescriptionList.TotalItemCount = Total;
                                        proAlloc.PrescriptionList.PageIndex = 0;
                                        foreach (Prescription p in results)
                                        {
                                            proAlloc.PrescriptionList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IPrescriptionSearch>(onInitDlg);
                                }
                                else
                                {
                                    SelectedPrescription = results.FirstOrDefault();
                                    LoadInfoCommon();
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void spGetInBatchNumberAndPrice_ByPresciption(long IssueID, long StoreID, Int16 IsObject)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginspGetInBatchNumberAndPrice_ByPresciption(IssueID, StoreID, IsObject, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAndPrice_ByPresciption(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                OutwardDrugsCopy = null;
                                SumTotalPrice();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearchPrescription(object sender, RoutedEventArgs e)
        {
            SearchPrescription(0, Globals.PageSize);
        }

        #region IHandle<PharmacyCloseSearchEditPrescriptionEvent> Members

        public void Handle(PharmacyCloseSearchEditPrescriptionEvent message)
        {
            if (this.IsActive && message != null)
            {
                SelectedPrescription = message.SelectedPrescription as Prescription;
                //Đọc thông tin coi đc Sửa không
                // Prescription_ByPrescriptIDIssueID(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID);
                LoadInfoCommon();
            }
        }

        #endregion

        #endregion

        #region Load Thong Tin Benh Nhan Member

        private void LoadInfoCommon()
        {
            if (SelectedPrescription != null)
            {
                if (SelectedOutInvoice == null)
                {
                    SelectedOutInvoice = new OutwardDrugInvoice();
                    SelectedOutInvoice.OutDate = Globals.ServerDate.Value;

                }
                SelectedOutInvoice.SelectedPrescription = SelectedPrescription;
                SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                SelectedOutInvoice.OutInvID = "";
                SelectedOutInvoice.outiID = 0;
                SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                SelectedOutInvoice.CheckedPoint = false;
                if (SelectedPrescription.PtRegistrationID.HasValue)
                {
                    Coroutine.BeginExecute(DoGetInfoPatientPrescription());
                }

            }
        }

        int findPatient = 0;
        private IEnumerator<IResult> DoGetInfoPatientPrescription()
        {
            isLoadingInfoPatient = true;
            var paymentTypeTask = new LoadPatientInfoByRegistrationTask(SelectedPrescription.PtRegistrationID, SelectedPrescription.PatientID, findPatient);
            yield return paymentTypeTask;
            PatientInfo = paymentTypeTask.CurrentPatient;
            GetClassicationPatientPrescription();
            if (grdPrescription != null)
            {
                HideShowColumns(grdPrescription);
            }
            spGetInBatchNumberAndPrice_ByPresciption(SelectedPrescription.IssueID, StoreID, 0);
            isLoadingInfoPatient = false;
            yield break;
        }

        #endregion

        private bool CheckQuantity(object outward)
        {
            try
            {
                OutwardDrug p = outward as OutwardDrug;
                if (p.QtyOffer == p.OutQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (!CheckQuantity(e.Row.DataContext))
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public void grdPrescription_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            SumTotalPrice();
        }

        int DayRpts = 0;
        private void GetClassicationPatientPrescription()
        {
            if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= Globals.ServerDate.Value.Date
             && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.Issue_HisID > 0 && !SelectedOutInvoice.SelectedPrescription.IsSold)
            {
                TimeSpan t = PatientInfo.CurrentHealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.ServerDate.Value.Date;
                DayRpts = Convert.ToInt32(t.TotalDays + 1);
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.IsHICount = IsHIPatient;
            }
        }

        //KMx: Khi load phiếu xuất cũ (bán thuốc theo toa) thì mới gọi hàm này. Còn trong phần cập nhật toa thuốc không có load phiếu xuất cũ nên hàm này không có sử dụng.
        //Nhưng do trong interface đã khai báo hàm này nên không comment.
        public void GetClassicationPatientInvoice()
        {
            //Kiên thêm đk SelectedOutInvoice.IsHICount == true.
            //Khi cập nhật thì PrescriptionViewModel sẽ truyền SelectedOutInvoice muốn cập nhật qua đây, phải dựa vào IsHICount của phiếu được truyền qua.
            //Nếu phiếu đó được tính BH thì phiếu sau khi cập nhật sẽ được tính BH, ngược lại thì không.
            if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= Globals.ServerDate.Value.Date
             && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.Issue_HisID > 0 && SelectedOutInvoice.IsHICount == true)
            {
                TimeSpan t = PatientInfo.CurrentHealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.ServerDate.Value.Date;
                DayRpts = Convert.ToInt32(t.TotalDays + 1);
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.IsHICount = IsHIPatient;
            }
        }

        private void HideShowColumns(DataGrid dg)
        {
            //if (dg == null)
            //{
            //    return;
            //}
            //if (!IsHIPatient)
            //{
            //    dg.Columns[1].Visibility = Visibility.Collapsed;
            //    dg.Columns[11].Visibility = Visibility.Collapsed;
            //    dg.Columns[12].Visibility = Visibility.Collapsed;
            //    dg.Columns[14].Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    dg.Columns[1].Visibility = Visibility.Visible;
            //    dg.Columns[11].Visibility = Visibility.Visible;
            //    dg.Columns[12].Visibility = Visibility.Visible;
            //    dg.Columns[14].Visibility = Visibility.Visible;
            //}
        }

        private bool CheckValid()
        {
            bool result = true;
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                if (eHCMS.Services.Core.AxHelper.CompareDate((DateTime)SelectedPrescription.IssuedDateTime, SelectedOutInvoice.OutDate) == 1)
                {
                    MessageBox.Show(eHCMSResources.A0861_G1_Msg_InfoNgXuatKhHopLe2, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (eHCMS.Services.Core.AxHelper.CompareDate(SelectedOutInvoice.OutDate, Globals.ServerDate.Value) == 1)
                {
                    MessageBox.Show(eHCMSResources.A0861_G1_Msg_InfoNgXuatKhHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugs.Count == 0)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                {
                    if (item.OutPrice <= 0)
                    {
                        MessageBox.Show("Giá bán phải > 0", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.OutQuantity <= 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0837_G1_SLgXuatMoiDongLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.Validate() == false)
                    {
                        MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    //neu ngay het han lon hon ngay hien tai
                    if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, item.InExpiryDate) == 1)
                    {
                        strError += item.GetDrugForSellVisitor.BrandName + string.Format(" {0}!", eHCMSResources.Z0868_G1_DaHetHanDung) + Environment.NewLine;
                    }
                }
                if (!string.IsNullOrEmpty(strError))
                {
                    if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private void SaveDrugs(OutwardDrugInvoice Invoice, bool value)
        {
            isLoadingFullOperator3 = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new CommonServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginSaveDrugs(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                    //    Globals.DeptLocation.DeptLocationID, null, Invoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginSaveDrugs_Pst(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                        Globals.DeptLocation.DeptLocationID, null, Invoice, Globals.DispatchCallback((asyncResult) =>

                    {
                        try
                        {
                            OutwardDrugInvoice OutInvoice;
                            contract.EndSaveDrugs_Pst(out OutInvoice, asyncResult);
                            //phat su kien de form o duoi load lai du lieu 
                            if (OutInvoice != null && OutInvoice.outiID > 0)
                            {
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayedPrescription { SelectedOutwardInvoice = OutInvoice });
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator3 = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        //KMx: 2 hàm GetValueFromAnonymousType(), CalScheduleQty() giống với 2 hàm cùng tên ở PrescriptionViewModel (08/06/2014 17:41).
        //Ban đầu tính đem 2 hàm này sang Globals để 2 ViewModels sử dụng chung. Nhưng đem qua Globals thì bị lỗi ở dòng (T)type.GetProperty(itemKey).GetValue(dataItem, null);
        public static T GetValueFromAnonymousType<T>(object dataItem, string itemKey)
        {
            System.Type type = dataItem.GetType();

            T itemValue = (T)type.GetProperty(itemKey).GetValue(dataItem, null);

            return itemValue;
        }

        public static float CalScheduleQty(object item, int nNumDay)
        {
            if (item == null)
            {
                return 0;
            }

            float[] WeeklySchedule = new float[7];
            byte? SchedBeginDOW;
            double DispenseVolume;

            SchedBeginDOW = GetValueFromAnonymousType<byte?>(item, "SchedBeginDOW");

            DispenseVolume = GetValueFromAnonymousType<double>(item, "DispenseVolume");

            WeeklySchedule[0] += GetValueFromAnonymousType<float>(item, "QtySchedMon");
            WeeklySchedule[1] += GetValueFromAnonymousType<float>(item, "QtySchedTue");
            WeeklySchedule[2] += GetValueFromAnonymousType<float>(item, "QtySchedWed");
            WeeklySchedule[3] += GetValueFromAnonymousType<float>(item, "QtySchedThu");
            WeeklySchedule[4] += GetValueFromAnonymousType<float>(item, "QtySchedFri");
            WeeklySchedule[5] += GetValueFromAnonymousType<float>(item, "QtySchedSat");
            WeeklySchedule[6] += GetValueFromAnonymousType<float>(item, "QtySchedSun");

            return Globals.CalcWeeklySchedulePrescription(SchedBeginDOW, nNumDay, WeeklySchedule, (float)DispenseVolume);
        }


        //KMx: Hàm này được viết lại từ hàm CheckInsurance. Lý do đổi cách tính thuốc lịch (08/06/2014 15:54)
        private bool CheckInsurance_New(int BHValidDays)
        {
            if (SelectedPrescription == null)
            {
                return true;
            }
            if (SelectedOutInvoice == null)
            {
                return true;
            }
            if (ListDrugTemp == null)
            {
                ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
            }
            ListDrugTemp.Clear();

            var hhh = from hd in SelectedOutInvoice.OutwardDrugs
                      where hd.GetDrugForSellVisitor.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
                      group hd by new
                      {
                          hd.DrugID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetDrugForSellVisitor.InsuranceCover,
                          hd.GetDrugForSellVisitor.BrandName
                      } into hdgroup

                      select new
                      {
                          QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          DrugID = hdgroup.Key.DrugID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          InsuranceCover = hdgroup.Key.InsuranceCover,
                          BrandName = hdgroup.Key.BrandName
                      };

            string strError = "";
            int xNgayToiDa = 30;

            xNgayToiDa = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;

            //Cảnh báo cho thuốc Cần.
            string strDrugsNotHaveDayRpts = "";
            //Những thuốc mà hết hạn bảo hiểm. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
            string strInvalidDrugs = "";

            for (int i = 0; i < hhh.Count(); i++)
            {
                //QtyMaxAllowed: Số lượng tối đa được bán (Minimum của SLYC và SL BH cho phép).
                //QtyHIValidateTo: Số lượng thuốc tính đến ngày BH hết hạn. Nếu SL bán > SL tính đến hết ngày BH hết hạn thì cảnh báo.
                //QtyOffer: Số lượng bác sĩ yêu cầu. Bác sĩ yêu cầu bao nhiêu viên thì đưa lên bấy nhiêu.

                int QtyMaxAllowed = 0;
                int QtyHIValidateTo = 0;

                string strReasonCannotSell;

                if (hhh.ToList()[i].QtyOffer <= 0)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(" {0}", eHCMSResources.K0015_G1_ThuocKhongCoSLgYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }

                QtyMaxAllowed = Convert.ToInt32(hhh.ToList()[i].QtyMaxAllowed);

                //KMx: Nếu là thuốc Cần thì so sánh "số lượng xuất" và "số lượng được bán tối đa."
                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
                {
                    if (hhh.ToList()[i].OutQuantity > QtyMaxAllowed)
                    {
                        strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0}: ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].QtyOffer + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.xban = QtyMaxAllowed;
                        ListDrugTemp.Add(item);
                    }
                    else
                    {
                        strDrugsNotHaveDayRpts += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                }

                //KMx: Nếu là thuốc Thường hoặc thuốc Lịch.
                else
                {
                    //KMx: QtyHIValidateTo: Số lượng tính đến ngày BH còn hiệu lực.
                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                    {
                        //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
                        double QtyRounded = Math.Round((float)hhh.ToList()[i].QtyForDay * BHValidDays, 2);
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(QtyRounded));
                    }

                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], BHValidDays)));
                    }

                    //Nếu số lượng muốn bán > số lượng mà bảo hiểm còn hiệu lực. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
                    if (hhh.ToList()[i].OutQuantity > QtyHIValidateTo)
                    {
                        strInvalidDrugs += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }

                    //Lấy Min của SLYC và SL tối đa để khống chế số lượng thực xuất.
                    int QtyAllowSell = Math.Min(hhh.ToList()[i].QtyOffer, QtyMaxAllowed);

                    if (hhh.ToList()[i].OutQuantity > QtyAllowSell)
                    {
                        if (hhh.ToList()[i].QtyOffer > QtyMaxAllowed)
                        {
                            strReasonCannotSell = string.Format(eHCMSResources.Z0876_G1_BHChiTraToiDa, xNgayToiDa.ToString());
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0}: ", eHCMSResources.Z0877_G1_SLgDuocBan) + QtyMaxAllowed + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }
                        else
                        {
                            strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0}: ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].QtyOffer + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }

                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.xban = QtyAllowSell;
                        ListDrugTemp.Add(item);
                    }
                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
                //sua lai so luong xuat cho hop li
                for (int i = 0; i < ListDrugTemp.Count; i++)
                {
                    var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == ListDrugTemp[i].DrugID).OrderBy(x => x.InExpiryDate);

                    if (values == null || values.Count() <= 0)
                    {
                        continue;
                    }

                    int xban = ListDrugTemp[i].xban;

                    foreach (OutwardDrug p in values)
                    {
                        if (xban <= 0)
                        {
                            DeleteOutwardDrugs(p);
                            continue;
                        }

                        if (p.OutQuantity <= xban)
                        {
                            if (p.QtyOffer <= xban)
                            {
                                p.OutQuantity = p.QtyOffer;
                            }
                            xban = xban - p.OutQuantity;
                        }
                        else //p.OutQuantity > xban
                        {
                            p.OutQuantity = xban;
                            xban = 0;
                        }
                    }
                }

                SumTotalPrice();
                return false;
            }

            if (!string.IsNullOrEmpty(strInvalidDrugs))
            {
                if (MessageBox.Show(string.Format("Bảo hiểm của bệnh nhân còn hiệu lực {0} ngày. Nhưng thuốc {1} được bán hơn số ngày bảo hiểm còn hiệu lực. \n Bạn có đồng ý bán không?", BHValidDays, strInvalidDrugs), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(strDrugsNotHaveDayRpts))
            {
                if (MessageBox.Show(string.Format("{0}: \n{1}. \n{2}", eHCMSResources.Z0882_G1_I, strDrugsNotHaveDayRpts, eHCMSResources.Z0881_G1_CoDongYBanKg), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }


        public void btnNgayDung_New()
        {
            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedOutInvoice == null)
            {
                return;
            }

            int NgayDung = 0;

            if (!Int32.TryParse(strNgayDung, out NgayDung))
            {
                MessageBox.Show(eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (NgayDung <= 0)
            {
                MessageBox.Show(eHCMSResources.A0837_G1_Msg_InfoNgDungLonHon0);
                return;
            }

            if (IsHIPatient) //toa bao hiem
            {
                int xNgayBHToiDa_NgoaiTru = 30;

                xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;

                //KMx: Lấy thuốc có ngày dùng nhỏ nhất. Nếu ngày dùng muốn cập nhật > ngày dùng nhỏ nhất thì không cho cập nhật.
                var DrugMinDayRpts = SelectedOutInvoice.OutwardDrugs.Where(x => x.DayRpts > 0).OrderBy(x => x.DayRpts).Take(1);

                if (NgayDung > xNgayBHToiDa_NgoaiTru)
                {
                    MessageBox.Show(string.Format("Bảo hiểm chỉ trả tối đa {0} ngày thuốc! Vui lòng nhập lại ngày dùng!", xNgayBHToiDa_NgoaiTru), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return;
                }

                foreach (OutwardDrug d in DrugMinDayRpts)
                {
                    if (NgayDung > d.DayRpts)
                    {
                        MessageBox.Show(string.Format("Bác sỹ yêu cầu thuốc {0} ({1} ngày). \nKhông được nhập ngày dùng lớn hơn BS yêu cầu!", d.GetDrugForSellVisitor.BrandName, d.DayRpts), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        return;
                    }
                }

            }


            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }

            //dung de lay ton hien tai va ton ban dau cua tung thuoc theo lo
            ObservableCollection<GetDrugForSellVisitor> temp = GetDrugForSellVisitorListByPrescriptID.DeepCopy();
            LaySoLuongTheoNgayDung(temp);

            //dung de lay ton hien tai va ton ban dau cua tung thuoc da duoc sum
            var ObjSumRemaining = from hd in GetDrugForSellVisitorList
                                  group hd by new { hd.DrugID, hd.BrandName } into hdgroup
                                  select new
                                  {
                                      Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                                      RemainingFirst = hdgroup.Sum(groupItem => groupItem.RemainingFirst),
                                      DrugID = hdgroup.Key.DrugID,
                                      BrandName = hdgroup.Key.BrandName
                                  };

            //lay tong so luong yc cua cac thuoc co trong toa (dua vao ngay dung > 0) 
            var hhh = from hd in SelectedOutInvoice.OutwardDrugs
                      where hd.DayRpts > 0 && hd.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN
                      group hd by new
                      {
                          hd.DrugID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetDrugForSellVisitor.BrandName
                      } into hdgroup
                      select new
                      {
                          QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          DrugID = hdgroup.Key.DrugID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          BrandName = hdgroup.Key.BrandName
                      };

            string ThongBao = "";
            //Lưu ý: Khi SelectedOutInvoice.OutwardDrugs add thêm 1 đối tượng (OutwardDrug) thì hhh.Count() tự động tăng lên 1.
            for (int i = 0; i < hhh.Count(); i++)
            {
                int QtyWillChange = 0;

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                {
                    //KMx: Sau khi lấy QtyForDay * NgayDung thì phải Round lại rồi sau đó mới Ceiling.
                    //Nếu không sẽ bị sai trong trường hợp thuốc có QtyForDay = 0.1666667. Khi nhân lên 30 ngày sẽ ra kết quả 5.00001 và Ceiling liền thì sẽ ra 6.
                    double QtyRounded = Math.Round((double)hhh.ToList()[i].QtyForDay * NgayDung, 2);
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(QtyRounded));
                }

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                {
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], NgayDung)));
                }

                //KMx: Dòng thuốc có số lượng muốn thay đổi bằng 0 thì sẽ xóa dòng thuốc đó đi (14/06/2014 16:54)
                //if (QtyWillChange <= 0)
                //{
                //    continue;
                //}

                var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault()).OrderBy(x => x.InExpiryDate);

                if (values == null || values.Count() <= 0)
                {
                    continue;
                }

                //Neu so luong < So luong hien co tren luoi thi chi can ham ben duoi
                foreach (OutwardDrug p in values)
                {
                    if (QtyWillChange <= 0)
                    {
                        DeleteOutwardDrugs(p);
                    }
                    else if (p.OutQuantity > QtyWillChange)
                    {
                        p.OutQuantity = QtyWillChange;
                        QtyWillChange = 0;
                    }
                    else if (p.OutQuantity <= QtyWillChange)
                    {
                        QtyWillChange = QtyWillChange - p.OutQuantity;
                    }
                }

                if (QtyWillChange <= 0)
                {
                    continue;
                }

                //else neu > so luong hien co tren luoi thi them vao

                var Obj = ObjSumRemaining.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault());
                if (Obj != null && Obj.Count() > 0)
                {
                    if (Obj.ToArray()[0].Remaining > 0 && Obj.ToArray()[0].Remaining >= QtyWillChange)
                    {
                        GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.BrandName = hhh.ToList()[i].BrandName;
                        //KMx: Phải assign Remaining trước RequiredNumber. Nếu không sẽ bị lỗi, vì khi assign RequiredNumber thì nó sẽ so sánh với Remaining.
                        item.Remaining = Obj.ToArray()[0].Remaining;
                        item.RequiredNumber = QtyWillChange;
                        item.Qty = hhh.ToList()[i].QtyOffer;
                        //item.Remaining = Obj.ToArray()[0].Remaining;

                        ReCountQtyAndAddList(item);
                    }
                    else
                    {
                        if (Obj.ToArray()[0].RemainingFirst > 0)
                        {
                            ThongBao = ThongBao + string.Format("\nThuốc {0}: SL cần bán = {1}, SL còn lại = {2}", Obj.ToArray()[0].BrandName, QtyWillChange.ToString(), Obj.ToArray()[0].RemainingFirst.ToString());
                        }
                        else
                        {
                            ThongBao = ThongBao + string.Format("\nThuốc {0}: SL cần bán = {1}, SL còn lại = {2}", Obj.ToArray()[0].BrandName, QtyWillChange.ToString(), "0");
                        }
                    }
                }
                else
                {
                    ThongBao = ThongBao + string.Format("\nThuốc {0} đã hết!", hhh.ToList()[i].BrandName);
                }

            }
            //KMx: Nếu để dòng dưới thì sẽ bị lỗi khi thêm cùng 1 loại thuốc có trong toa nhưng khác lô.
            //SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
            if (!string.IsNullOrEmpty(ThongBao))
            {
                MessageBox.Show(ThongBao);
            }
        }


        private void SaveCmd(bool value)
        {
            //phai cho biet day la ham cap nhat hay them moi de khoi cap lai so
            isLoadingFullOperator2 = true;
            //true la save xong roi thu tien, false thi binh thuong : chi luu thoi
            if (SelectedOutInvoiceNew != null)
            {
                SelectedOutInvoiceNew.OutDate = SelectedOutInvoice.OutDate;
                //Kiên thêm ModFromOutiID để biết phiếu phiếu mới vừa cập nhật được tạo ra từ phiếu nào.
                SelectedOutInvoiceNew.ModFromOutiID = SelectedOutInvoice.outiID;
                //tam thoi huy roi them moi lai 
                //SelectedOutInvoiceNew.RecordState = RecordState.DELETED;
                SelectedOutInvoiceNew.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                SelectedOutInvoiceNew.outiID = 0;
                SelectedOutInvoiceNew.IssueID = SelectedPrescription.IssueID;
                SelectedOutInvoiceNew.SelectedPrescription = SelectedPrescription;

                SaveDrugs(SelectedOutInvoiceNew, value);
            }
            isLoadingFullOperator2 = false;
        }

        private void CancalOutwardInvoice()
        {
            isLoadingFullOperator1 = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
            try
            {
                //using (var serviceFactory = new CommonServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginCancelOutwardDrugInvoice(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                    //Globals.DeptLocation.DeptLocationID,null, SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginCancelOutwardDrugInvoice_Pst(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                        Globals.DeptLocation.DeptLocationID, null, SelectedOutInvoice, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                        //var results = contract.EndCancelOutwardDrugInvoice(asyncResult);
                        var results = contract.EndCancelOutwardDrugInvoice_Pst(asyncResult);
                            SaveCmd(true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            isLoadingFullOperator1 = false;
                        //Globals.IsBusy = false;
                    }

                    }), null);

                }
            }
            catch (Exception ex)
            {
                this.DlgHideBusyIndicator();
                MessageBox.Show(ex.Message);
                _logger.Info(ex.Message);
            }

            });

            t.Start();
        }

        private void CancalOutwardInvoice_ChuaThuTien()
        {
            isLoadingFullOperator1 = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        //contract.BeginOutWardDrugInvoicePrescriptChuaThuTien_Cancel(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginOutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                            //var results = contract.EndOutWardDrugInvoicePrescriptChuaThuTien_Cancel(asyncResult);
                            var results = contract.EndOutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(asyncResult);
                                SaveCmd(true);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                isLoadingFullOperator1 = false;
                            //Globals.IsBusy = false;
                        }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    MessageBox.Show(ex.Message);
                    _logger.Info(ex.Message);
                }

            });

            t.Start();
        }

        private PatientRegistration RegistrationInfo;
        private void GetRegistrationInfo(long PtRegistrationID)
        {
            isLoadingInfoReg = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRegistrationInfo(PtRegistrationID, 0, false, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            RegistrationInfo = contract.EndGetRegistrationInfo(asyncResult);
                            //RegistrationInfo.DrugInvoices.Add(SelectedOutInvoice);
                            //load form tinh tien
                            //var vm = Globals.GetViewModel<IPay>();
                            //vm.Registration = RegistrationInfo;
                            //vm.SetRegistration(RegistrationInfo);

                            //Globals.ShowDialog(vm as Conductor<object>);

                            //var vm = Globals.GetViewModel<ISimplePay>();
                            //vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            //vm.Registration = RegistrationInfo;
                            //vm.FormMode = PaymentFormMode.PAY;
                            //if (RegistrationInfo == null)
                            //{
                            //    MessageBox.Show(eHCMSResources.A0380_G1_Msg_InfoChuaChonDK);
                            //    return;
                            //}

                            //vm.RegistrationDetails = null;
                            //vm.PclRequests = null;

                            //vm.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                            //vm.StartCalculating();

                            //if (vm.TotalPayForSelectedItem != vm.TotalPaySuggested)
                            //{
                            //    Globals.ShowDialog(vm as Conductor<object>);
                            //}
                            //else
                            //{
                            //    var vm2 = Globals.GetViewModel<ISimplePay2>();
                            //    vm2.Registration = RegistrationInfo;
                            //    vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            //    vm2.RegistrationDetails = null;
                            //    vm2.PclRequests = null;

                            //    vm2.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                            //    vm2.StartCalculating();
                            //    Globals.ShowDialog(vm2 as Conductor<object>);
                            //}
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingInfoReg = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();

        }
        private bool flag = true;
        public void btnClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (flag)
                {
                    btnSaveMoney();
                }
            }
        }

        private void SaveDrugIndependents(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        //contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginOutwardDrugInvoice_SaveByType_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long OutID = 0;
                                string StrError;
                            //bool value = contract.EndOutwardDrugInvoice_SaveByType(out OutID, out StrError, asyncResult);
                            bool value = contract.EndOutwardDrugInvoice_SaveByType_Pst(out OutID, out StrError, asyncResult);
                                if (OutID > 0)
                                {
                                    OutwardInvoice.outiID = OutID;
                                }

                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                //goi ham tinh tien
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayedPrescription { SelectedOutwardInvoice = OutwardInvoice });
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    _logger.Info(ex.Message);
                }

            });

            t.Start();
        }

        private void UpdateInvoicePayed(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginUpdateInvoicePayed(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginUpdateInvoicePayed_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            long PaymemtID = 0;
                            string StrError = "";
                            //bool value = contract.EndUpdateInvoicePayed(out OutID, out PaymemtID, out StrError, asyncResult);
                            bool value = contract.EndUpdateInvoicePayed_Pst(out OutID, out PaymemtID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                OutwardInvoice.outiID = OutID;
                                //phat su kien de form o duoi load lai du lieu 
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayedPrescription { SelectedOutwardInvoice = OutwardInvoice });
                                TryClose();
                                //  CountMoneyForVisitorPharmacy(OutID);
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private OutwardDrugInvoice SelectedOutInvoiceNew;
        private void btnSaveMoney()
        {
            isLoadingFullOperator = true;
            if (CheckValid())
            {
                if (SelectedPrescription != null)
                {
                    SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                    SelectedOutInvoice.IssueID = SelectedPrescription.IssueID;
                    SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                    SelectedOutInvoice.SelectedStorage = new RefStorageWarehouseLocation();
                    SelectedOutInvoice.SelectedStorage.StoreID = StoreID;
                    SelectedOutInvoice.SelectedStaff = GetStaffLogin();

                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    if (IsHIPatient)
                    {
                        //if (CheckInsurance(DayRpts))
                        if (CheckInsurance_New(DayRpts))
                        {
                            UpdateInvoice();
                        }
                    }
                    else
                    {
                        UpdateInvoice();
                    }
                }
            }
            isLoadingFullOperator = false;
            flag = true;

        }

        private void UpdateInvoice()
        {
            SelectedOutInvoiceNew = SelectedOutInvoice.DeepCopy();
            SelectedOutInvoiceNew.ColectDrugSeqNumType = SelectedOutInvoice.ColectDrugSeqNumType;
            SelectedOutInvoiceNew.ColectDrugSeqNum = SelectedOutInvoice.ColectDrugSeqNum;
            SelectedOutInvoiceNew.IsUpdate = true;
            SelectedOutInvoiceNew.SelectedStaff = GetStaffLogin();

            if (SelectedOutInvoice.PaidTime == null)
            {
                //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
                // Txd 25/05/2014 Replaced ConfigList
                if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
                {
                    SaveDrugIndependents(SelectedOutInvoiceNew);
                }
                else
                {
                    CancalOutwardInvoice_ChuaThuTien();
                }
            }
            else
            {
                //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
                // Txd 25/05/2014 Replaced ConfigList
                if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
                {
                    UpdateInvoicePayed(SelectedOutInvoiceNew);
                }
                else
                {
                    CancalOutwardInvoice();
                }
            }
        }
        private bool Equal(OutwardDrug a, OutwardDrug b)
        {
            if (a.InID == b.InID && a.DrugID == b.DrugID && a.InBatchNumber == b.InBatchNumber && a.InExpiryDate == b.InExpiryDate
                && a.OutPrice == b.OutPrice && a.OutQuantity == b.OutQuantity && a.HIAllowedPrice == b.HIAllowedPrice)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //Kiên: Thêm sự kiện: Nếu thay đổi ngày thì hiện lên nút "Lưu".
        public bool dateTimePickerChanged()
        {
            //Nếu chọn ngày khác với ngày xuất hoặc thay đổi thuốc, số lượng thì hiện nút "Lưu"
            if (SelectedOutInvoiceCopy.OutDate != SelectedOutInvoice.OutDate || !Compare2Object())
            {
                IsOther = true;
                return true;
            }
            IsOther = false;
            return false;
        }

        private bool Compare2Object()
        {
            if (SelectedOutInvoiceCopy.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count == SelectedOutInvoiceCopy.OutwardDrugs.Count && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                int icount = 0;
                for (int i = 0; i < SelectedOutInvoiceCopy.OutwardDrugs.Count; i++)
                {
                    for (int j = 0; j < SelectedOutInvoice.OutwardDrugs.Count; j++)
                    {
                        if (Equal(SelectedOutInvoiceCopy.OutwardDrugs[i], SelectedOutInvoice.OutwardDrugs[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == SelectedOutInvoiceCopy.OutwardDrugs.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        #region Total Price Member
        private decimal _TotalInvoicePrice;
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }
        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }
        private void SumTotalPrice()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugs == null)
            {
                return;
            }

            if (dateTimePickerChanged() || !Compare2Object())
            {
                IsOther = true;
            }
            else
            {
                IsOther = false;
            }

            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            double HIBenefit = 0;
            if (PatientInfo != null && PatientInfo.LatestRegistration != null)
            {
                HIBenefit = PatientInfo.LatestRegistration.PtInsuranceBenefit.GetValueOrDefault();
            }
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

            if (!onlyRoundResultForOutward)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    if (SelectedOutInvoice.OutwardDrugs[i].InwardDrug != null && SelectedOutInvoice.OutwardDrugs[i].InwardDrug.HIAllowedPrice.GetValueOrDefault(0) > 0)
                    {
                        SelectedOutInvoice.OutwardDrugs[i].PriceDifference = SelectedOutInvoice.OutwardDrugs[i].OutPrice - SelectedOutInvoice.OutwardDrugs[i].InwardDrug.HIAllowedPrice.GetValueOrDefault(0);
                    }
                    SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;
                    SelectedOutInvoice.OutwardDrugs[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                    if (SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment > 0)
                    {
                        SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity);
                    }

                    TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                    TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;

                }
                TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
            }
            else
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    //if (SelectedOutInvoice.OutwardDrugs[i].InwardDrug != null && SelectedOutInvoice.OutwardDrugs[i].InwardDrug.HIAllowedPrice.GetValueOrDefault(0) > 0)
                    //{
                    //    SelectedOutInvoice.OutwardDrugs[i].PriceDifference = SelectedOutInvoice.OutwardDrugs[i].OutPrice - SelectedOutInvoice.OutwardDrugs[i].InwardDrug.HIAllowedPrice.GetValueOrDefault(0);
                    //}

                    SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;
                    SelectedOutInvoice.OutwardDrugs[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                    if (SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment > 0)
                    {
                        SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                    }

                    TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                    TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                }
                TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, Common.Converters.MidpointRounding.AwayFromZero);
                TotalHIPayment = MathExt.Round(TotalHIPayment, Common.Converters.MidpointRounding.AwayFromZero);

                TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;

            }

        }
        #endregion

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void GridInward_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        #region printing member

        public void btnPreview()
        {
            //var proAlloc = Globals.GetViewModel<IReportDocumentPreview>();
            //proAlloc.PatientID = PatientInfo.PatientID;
            //proAlloc.ID = SelectedOutInvoice.outiID;
            //proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientID = PatientInfo.PatientID;
                proAlloc.ID = SelectedOutInvoice.outiID;
                proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }

        private void PrintSilient()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCollectionDrugInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetCollectionDrugInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, "A5");
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        public void btnPrint()
        {
            PrintSilient();
        }
        #endregion

        #region auto Drug For Prescription member

        public void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            HI = 0;
        }
        public void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            HI = 1;
        }
        public void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            HI = 2;
        }

        private string BrandName;
        private byte HI;
        private bool IsHIPatient = false;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorListByPrescriptID;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptID
        {
            get { return _GetDrugForSellVisitorListByPrescriptID; }
            set
            {
                _GetDrugForSellVisitorListByPrescriptID = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListByPrescriptID);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorSum;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListSum
        {
            get { return _GetDrugForSellVisitorSum; }
            set
            {
                if (_GetDrugForSellVisitorSum != value)
                    _GetDrugForSellVisitorSum = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListSum);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp;

        private GetDrugForSellVisitor _SelectedSellVisitor;
        public GetDrugForSellVisitor SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetDrugForSellVisitor(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new
                      { 
                          hd.DrugID,
                          hd.DrugCode,
                          hd.BrandName,
                          hd.UnitName,
                          hd.Qty,
                          hd.DayRpts,

                          hd.QtyForDay,
                          hd.V_DrugType,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,

                          hd.HIPaymentPercent
                      } into hdgroup

                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          DrugID = hdgroup.Key.DrugID,
                          DrugCode = hdgroup.Key.DrugCode,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Qty = hdgroup.Key.Qty,
                          DayRpts = hdgroup.Key.DayRpts,

                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,
                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,
                          HIPaymentPercent = hdgroup.Key.HIPaymentPercent
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                item.DrugID = hhh.ToList()[i].DrugID;
                item.DrugCode = hhh.ToList()[i].DrugCode;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                item.Qty = hhh.ToList()[i].Qty;
                item.DayRpts = hhh.ToList()[i].DayRpts;

                item.V_DrugType = hhh.ToList()[i].V_DrugType;
                item.QtyForDay = hhh.ToList()[i].QtyForDay;
                item.QtyMaxAllowed = hhh.ToList()[i].QtyMaxAllowed;
                item.QtySchedMon = hhh.ToList()[i].QtySchedMon;
                item.QtySchedTue = hhh.ToList()[i].QtySchedTue;
                item.QtySchedWed = hhh.ToList()[i].QtySchedWed;
                item.QtySchedThu = hhh.ToList()[i].QtySchedThu;
                item.QtySchedFri = hhh.ToList()[i].QtySchedFri;
                item.QtySchedSat = hhh.ToList()[i].QtySchedSat;
                item.QtySchedSun = hhh.ToList()[i].QtySchedSun;
                item.SchedBeginDOW = hhh.ToList()[i].SchedBeginDOW;
                item.DispenseVolume = hhh.ToList()[i].DispenseVolume;

                item.HIPaymentPercent = hhh.ToList()[i].HIPaymentPercent;

                GetDrugForSellVisitorListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (GetDrugForSellVisitorListSum != null && GetDrugForSellVisitorListSum.Count > 0)
                {
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode == txt);
                    if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = item.ToList()[0];
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetDrugForSellVisitorListSum;
                    au.PopulateComplete();
                }
            }
        }

        private void SearchGetDrugForSellVisitor(string name, bool? IsCode)
        {
            long? PrescriptID = null;
            if (SelectedOutInvoice != null)
            {
                PrescriptID = SelectedOutInvoice.PrescriptID;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForPrescription(HI, IsHIPatient, name, StoreID, PrescriptID, IsCode, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForPrescription(asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorListSum.Clear();
                            LaySoLuongTheoNgayDung(results);
                            ListDisplayAutoComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //▼====== #001
                            //if (au != null)
                            //{
                            //    au.Focus();
                            //}
                            //▲====== #001
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void LaySoLuongTheoNgayDung(IList<GetDrugForSellVisitor> results)
        {
            if (results != null)
            {
                GetDrugForSellVisitorTemp = results.ToObservableCollection();
                if (GetDrugForSellVisitorTemp == null)
                {
                    GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
                }
                if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugsCopy)
                    {
                        var value = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                        if (value.Count() > 0)
                        {
                            foreach (GetDrugForSellVisitor s in value.ToList())
                            {
                                s.Remaining = s.Remaining + d.OutQuantityOld;
                                s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                            }
                        }
                        else
                        {
                            GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                            p.Remaining = d.OutQuantity;
                            p.RemainingFirst = d.OutQuantity;
                            p.InBatchNumber = d.InBatchNumber;
                            p.SellingPrice = d.OutPrice;
                            p.InID = Convert.ToInt64(d.InID);
                            p.STT = d.STT;
                            GetDrugForSellVisitorTemp.Add(p);
                            // d = null;
                        }
                    }
                }
                foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                {
                    if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                    {
                        foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
                        {
                            if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                            {
                                s.Remaining = s.Remaining - d.OutQuantity;
                            }
                        }
                    }
                    GetDrugForSellVisitorList.Add(s);
                }
            }
        }

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p1 in SelectedOutInvoice.OutwardDrugs)
                {
                    if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        p1.QtyOffer += p.QtyOffer;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetDrugForSellVisitor.InsuranceCover;

                    if (p.InwardDrug == null)
                    {
                        p.InwardDrug = new InwardDrug();
                        p.InwardDrug.InID = p.InID.GetValueOrDefault();
                        p.InwardDrug.DrugID = p.DrugID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrug.NormalPrice = p.OutPrice;
                    p.InwardDrug.HIPatientPrice = p.OutPrice;
                    p.InwardDrug.HIAllowedPrice = p.HIAllowedPrice;
                    if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                    {
                        p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
                    }

                    SelectedOutInvoice.OutwardDrugs.Add(p);
                }
                txt = "";
                SelectedSellVisitor = null;
                if (IsCode.GetValueOrDefault())
                {
                    if (tbx != null)
                    {
                        tbx.Text = "";
                        tbx.Focus();
                    }

                }
                else
                {
                    if (au != null)
                    {
                        au.Text = "";
                        au.Focus();
                    }
                }
            }
        }


        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();

                if (item.Remaining <= 0)
                {
                    continue;
                }

                item.DrugIDChanged = value.DrugIDChanged;
                p.GetDrugForSellVisitor = item;
                p.DrugID = item.DrugID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.OutPrice = item.OutPrice;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.HIAllowedPrice = item.HIAllowedPrice;

                //KMx: Nếu thuốc muốn thêm vào giống với thuốc đã có trong phiếu phải gán lại những thuộc tính bên dưới.
                //Nếu không gán lại thì hàm CheckInsurance_New(), btnNgayDung_New() và ListDisplayAutoComplete() sẽ sai ở dòng 
                //group hd by new
                //    { 
                //        hd.DrugID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.QtyMaxAllowed, hd.QtySchedMon, hd.QtySchedTue,
                //        hd.QtySchedWed, hd.QtySchedThu, hd.QtySchedFri, hd.QtySchedSat, hd.QtySchedSun, hd.SchedBeginDOW, hd.DispenseVolume,
                //        hd.GetDrugForSellVisitor.InsuranceCover, hd.GetDrugForSellVisitor.BrandName
                //    } into hdgroup

                p.V_DrugType = item.V_DrugType;
                p.QtyForDay = (decimal)item.QtyForDay;
                p.DayRpts = item.DayRpts;
                p.QtyMaxAllowed = item.QtyMaxAllowed;
                p.QtySchedMon = item.QtySchedMon;
                p.QtySchedTue = item.QtySchedTue;
                p.QtySchedWed = item.QtySchedWed;
                p.QtySchedThu = item.QtySchedThu;
                p.QtySchedFri = item.QtySchedFri;
                p.QtySchedSat = item.QtySchedSat;
                p.QtySchedSun = item.QtySchedSun;
                p.SchedBeginDOW = item.SchedBeginDOW;
                p.DispenseVolume = item.DispenseVolume;

                p.HIPaymentPercent = item.HIPaymentPercent;

                if (item.Remaining - value.RequiredNumber < 0)
                {
                    if (value.Qty > item.Remaining)
                    {
                        p.QtyOffer = item.Remaining;
                        value.Qty = value.Qty - item.Remaining;
                    }
                    else
                    {
                        p.QtyOffer = value.Qty;
                        value.Qty = 0;
                    }
                    
                    value.RequiredNumber = value.RequiredNumber - item.Remaining;
                    
                    p.OutQuantity = item.Remaining;

                    CheckBatchNumberExists(p);
                    item.Remaining = 0;
                }
                else
                {
                    p.QtyOffer = value.Qty;
                    value.Qty = 0;

                    p.OutQuantity = (int)value.RequiredNumber;
                    CheckBatchNumberExists(p);
                    item.Remaining = item.Remaining - (int)value.RequiredNumber;
                    break;
                }
            }
            SumTotalPrice();
        }


        //private void ChooseBatchNumber(GetDrugForSellVisitor value)
        //{
        //    var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
        //    foreach (GetDrugForSellVisitor item in items)
        //    {
        //        OutwardDrug p = new OutwardDrug();

        //        if (item.Remaining <= 0)
        //        {
        //            continue;
        //        }

        //        if (item.Remaining - value.RequiredNumber < 0)
        //        {
        //            if (value.Qty > item.Remaining)
        //            {
        //                p.QtyOffer = item.Remaining;
        //                value.Qty = value.Qty - item.Remaining;
        //            }
        //            else
        //            {
        //                p.QtyOffer = value.Qty;
        //                value.Qty = 0;
        //            }
        //            item.DrugIDChanged = value.DrugIDChanged;
        //            value.RequiredNumber = value.RequiredNumber - item.Remaining;
        //            p.GetDrugForSellVisitor = item;
        //            p.DrugID = item.DrugID;
        //            p.InBatchNumber = item.InBatchNumber;
        //            p.InID = item.InID;
        //            p.OutPrice = item.OutPrice;
        //            p.OutQuantity = item.Remaining;
        //            p.InExpiryDate = item.InExpiryDate;
        //            p.SdlDescription = item.SdlDescription;
        //            p.HIAllowedPrice = item.HIAllowedPrice;
        //            CheckBatchNumberExists(p);
        //            item.Remaining = 0;
        //        }
        //        else
        //        {
        //            item.DrugIDChanged = value.DrugIDChanged;
        //            p.GetDrugForSellVisitor = item;
        //            p.DrugID = item.DrugID;
        //            p.InBatchNumber = item.InBatchNumber;
        //            p.InID = item.InID;

        //            p.QtyOffer = value.Qty;
        //            value.Qty = 0;

        //            p.OutQuantity = (int)value.RequiredNumber;
        //            p.OutPrice = item.OutPrice;
        //            p.InExpiryDate = item.InExpiryDate;
        //            p.SdlDescription = item.SdlDescription;
        //            p.HIAllowedPrice = item.HIAllowedPrice;
        //            CheckBatchNumberExists(p);
        //            item.Remaining = item.Remaining - (int)value.RequiredNumber;
        //            break;
        //        }
        //    }
        //    SumTotalPrice();
        //}

        private void AddListOutwardDrug(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (CheckValidDrugAuto(SelectedDrugForSell))
            {
                ChooseBatchNumber(SelectedDrugForSell);
            }
            else
            {
                string ErrorMessage = eHCMSResources.Z0888_G1_ThuocThaoTacBiLoi;
                if (SelectedDrugForSell.BrandName != null)
                {
                    ErrorMessage = string.Format("Thuốc {0} đã bị lỗi! Vui lòng kiểm tra lại!", SelectedDrugForSell.BrandName);
                }
                MessageBox.Show(ErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            }
        }

        private void ReCountQtyRequest(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (SelectedOutInvoice.OutwardDrugs == null)
            {
                SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
            }
            var results1 = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == SelectedDrugForSell.DrugID);
            if (results1 != null && results1.Count() > 0)
            {
                foreach (OutwardDrug p in results1)
                {
                    if (p.QtyOffer > p.OutQuantity)
                    {
                        p.QtyOffer = p.OutQuantity;
                    }
                    SelectedDrugForSell.Qty = SelectedDrugForSell.Qty - p.QtyOffer;
                }
            }
        }

        public bool CheckValidQty(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (SelectedOutInvoice != null && SelectedDrugForSell != null)
            {
                int intOutput = 0;
                if (SelectedDrugForSell.RequiredNumber <= 0
                    || !Int32.TryParse(SelectedDrugForSell.RequiredNumber.ToString(), out intOutput) //Nếu số lượng không phải là số nguyên.
                    || SelectedDrugForSell.RequiredNumber > SelectedDrugForSell.Remaining) //Nếu số lượng muốn thêm > số lượng còn trong kho.
                {
                    MessageBox.Show(eHCMSResources.Z0890_G1_SLgKgHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0400_G1_ChonThuocCanThem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
        }

        //Kết hợp 2 hàm ReCountQtyRequest() và AddListOutwardDrug() (09/06/2014 16:40)
        public void ReCountQtyAndAddList(GetDrugForSellVisitor SelectedSellVisitor)
        {
            if (!CheckValidQty(SelectedSellVisitor))
                return;
            else
            {
                ReCountQtyRequest(SelectedSellVisitor);
                AddListOutwardDrug(SelectedSellVisitor);
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            ReCountQtyAndAddList(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }
        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForSellVisitorBatchNumber(DrugID, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByDrugIDFirst = OutwardDrugsCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                }
                GetDrugForSellVisitorBatchNumber(DrugID);
            }
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugListByDrugIDFirst != null)
            {
                foreach (OutwardDrug d in OutwardDrugListByDrugIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetDrugForSellVisitor s in BatchNumberListTemp)
            {
                if (OutwardDrugListByDrugID.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugListByDrugID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //f (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                //var proAlloc = Globals.GetViewModel<IChooseBatchNumberVisitor>();
                //proAlloc.FormType = 2;//phat su kien cho form chinh sua bat
                //proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                //if (BatchNumberListShow != null)
                //{
                //    proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                //}
                //if (OutwardDrugListByDrugID != null)
                //{
                //    proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                //}
                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<IChooseBatchNumberVisitor> onInitDlg = (proAlloc) =>
                {
                    proAlloc.FormType = 2;//phat su kien cho form chinh sua bat
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByDrugID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac);
            }
        }
        #endregion

        #region delete outward drug detals member
        private void DeleteOutwardDrugs(OutwardDrug temp)
        {
            OutwardDrug p = temp.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(temp);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrug p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(SelectedOutwardDrug);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0892_CoMuonXoaThuocKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (SelectedOutwardDrug != null)
                {
                    DeleteInvoiceDrugInObject();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0543_G1_Msg_InfoLoi);
                }
            }
        }

        #endregion

        public void btnNew()
        {
            TryClose();
        }
        #region IHandle<EditChooseBatchNumberVisitorEvent> Members

        public void Handle(EditChooseBatchNumberVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetDrugForSellVisitor.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetDrugForSellVisitor.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetDrugForSellVisitor.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetDrugForSellVisitor.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetDrugForSellVisitor.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetDrugForSellVisitor.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                //SelectedOutwardDrug.GetDrugForSellVisitor.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<EditChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(EditChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetDrugForSellVisitor.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetDrugForSellVisitor.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetDrugForSellVisitor.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetDrugForSellVisitor.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetDrugForSellVisitor.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetDrugForSellVisitor.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                //SelectedOutwardDrug.GetDrugForSellVisitor.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForSellVisitor(txt, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        private bool? IsCode = false;
        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }

        public void btnUp()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex > 0)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrug Up = SelectedOutInvoice.OutwardDrugs[i - 1].DeepCopy();
                OutwardDrug Down = SelectedOutInvoice.OutwardDrugs[i].DeepCopy();
                SelectedOutInvoice.OutwardDrugs[i] = Up;
                SelectedOutInvoice.OutwardDrugs[i - 1] = Down;
                grdPrescription.SelectedIndex = i - 1;
            }
        }

        public void btnDown()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex < SelectedOutInvoice.OutwardDrugs.Count() - 1)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrug Up = SelectedOutInvoice.OutwardDrugs[i].DeepCopy();
                OutwardDrug Down = SelectedOutInvoice.OutwardDrugs[i + 1].DeepCopy();
                SelectedOutInvoice.OutwardDrugs[i] = Down;
                SelectedOutInvoice.OutwardDrugs[i + 1] = Up;
                grdPrescription.SelectedIndex = i + 1;
            }
        }
    }
}



//neu la BN bao hiem : nhug thuoc thuoc DMBH can kiem tra ngay con han cua the BH
//neu ngay con han < ngay dung bac si ke toa thi bat buoc phai ban it hon so luong bac si ra toa
//private bool CheckInsurance(int Days)
//{
//    if (SelectedPrescription == null)
//    {
//        return true;
//    }
//    if (SelectedOutInvoice == null)
//    {
//        return true;
//    }
//    if (ListDrugTemp == null)
//    {
//        ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
//    }
//    ListDrugTemp.Clear();

//    // if (!SelectedPrescription.IsSold && SelectedOutInvoice.outiID==0)
//    {
//        var hhh = from hd in SelectedOutInvoice.OutwardDrugs
//                  where hd.GetDrugForSellVisitor.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
//                  group hd by new { hd.DrugID, hd.DayRpts, hd.QtyForDay, hd.GetDrugForSellVisitor.InsuranceCover, hd.GetDrugForSellVisitor.BrandName } into hdgroup
//                  select new
//                  {
//                      QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
//                      OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
//                      DrugID = hdgroup.Key.DrugID,
//                      DayRpts = hdgroup.Key.DayRpts,
//                      QtyForDay = hdgroup.Key.QtyForDay,
//                      InsuranceCover = hdgroup.Key.InsuranceCover,
//                      BrandName = hdgroup.Key.BrandName
//                  };

//        string strError = "";
//        int xNgayToiDa = 30;
//        if (Globals.ConfigList != null)
//        {
//            xNgayToiDa = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
//        }
//        Days = xNgayToiDa + 1;//tam thoi de vay de khong gioi han het han bao hiem
//        for (int i = 0; i < hhh.Count(); i++)
//        {
//            int xban = 0;
//            int xbantoida = 0;
//            string str = "Lý do :Hết hạn BHYT";
//            if (hhh.ToList()[i].DayRpts != 0)
//            {
//                //xbantoida = Convert.ToInt32(Math.Ceiling(((double)hhh.ToList()[i].QtyOffer / hhh.ToList()[i].DayRpts) * xNgayToiDa));
//                //xban = Convert.ToInt32(Math.Ceiling(((double)hhh.ToList()[i].QtyOffer / hhh.ToList()[i].DayRpts) * Days));

//                xbantoida = Convert.ToInt32(Math.Ceiling((double)(hhh.ToList()[i].QtyForDay * xNgayToiDa)));
//                xban = Convert.ToInt32(Math.Ceiling((double)(hhh.ToList()[i].QtyForDay * Days)));

//                if (hhh.ToList()[i].QtyOffer > 0 && hhh.ToList()[i].QtyOffer == Math.Min(hhh.ToList()[i].QtyOffer, Math.Min(xban, xbantoida)))
//                {
//                    if (hhh.ToList()[i].OutQuantity > hhh.ToList()[i].QtyOffer)
//                    {
//                        str = "Lý do :SL bán vượt SL Bác Sĩ YC";
//                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ".SL B.sĩ kê " + hhh.ToList()[i].QtyOffer + Environment.NewLine + str + Environment.NewLine;
//                        ListDrugAndQtySell item = new ListDrugAndQtySell();
//                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
//                        item.xban = hhh.ToList()[i].QtyOffer;
//                        ListDrugTemp.Add(item);
//                    }
//                }
//                //else if (xban == Math.Min(hhh.ToList()[i].QtyOffer, Math.Min(xban, xbantoida)))
//                //{
//                //    if (hhh.ToList()[i].OutQuantity > xban)
//                //    {
//                //        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ".SL được bán " + xban + Environment.NewLine + str + Environment.NewLine;
//                //        ListDrugAndQtySell item = new ListDrugAndQtySell();
//                //        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
//                //        item.xban = xban;
//                //        ListDrugTemp.Add(item);
//                //    }
//                //}
//                else if (hhh.ToList()[i].QtyOffer > 0 && xbantoida == Math.Min(hhh.ToList()[i].QtyOffer, xbantoida))//Math.Min(xban, xbantoida)
//                {
//                    if (hhh.ToList()[i].OutQuantity > xbantoida)
//                    {
//                        str = "Lý do : BH chi trả tối đa " + xNgayToiDa.ToString() + " ngày thuốc";
//                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ".SL được bán " + xbantoida + Environment.NewLine + str + Environment.NewLine;
//                        ListDrugAndQtySell item = new ListDrugAndQtySell();
//                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
//                        item.xban = xbantoida;
//                        ListDrugTemp.Add(item);
//                    }
//                }


//            }

//        }
//        if (!string.IsNullOrEmpty(strError))
//        {
//            if (MessageBox.Show(strError + Environment.NewLine + "Bạn có muốn điều chỉnh số lượng cho chính xác không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            {
//                //sua lai so luong xuat cho hop li
//                for (int i = 0; i < ListDrugTemp.Count; i++)
//                {
//                    var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == ListDrugTemp[i].DrugID).OrderByDescending(x => x.Qty);
//                    if (values != null)
//                    {
//                        int xban = ListDrugTemp[i].xban;
//                        foreach (OutwardDrug p in values)
//                        {
//                            if (xban <= 0)
//                            {
//                                DeleteOutwardDrugs(p);
//                            }
//                            else if (p.OutQuantity > xban)
//                            {
//                                p.OutQuantity = xban;
//                                xban = 0;
//                            }
//                            else if (p.OutQuantity <= xban)
//                            {
//                                xban = xban - p.OutQuantity;
//                                // DeleteOutwardDrugs(p);
//                            }
//                        }
//                    }
//                }
//                SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
//                SumTotalPrice();
//            }
//            return false;
//        }
//    }
//    return true;
//}

//Kiên: Sửa lại, nếu chạy ổn định thì xóa cái CheckInsurance() cũ.
/*private bool CheckInsurance(int BHValidDays)
{
    if (SelectedPrescription == null)
    {
        return true;
    }
    if (SelectedOutInvoice == null)
    {
        return true;
    }
    if (ListDrugTemp == null)
    {
        ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
    }
    ListDrugTemp.Clear();

    // if (!SelectedPrescription.IsSold && SelectedOutInvoice.outiID==0)
    {
        var hhh = from hd in SelectedOutInvoice.OutwardDrugs
                  where hd.GetDrugForSellVisitor.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
                  group hd by new { hd.DrugID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.GetDrugForSellVisitor.InsuranceCover, hd.GetDrugForSellVisitor.BrandName } into hdgroup
                  select new
                  {
                      QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
                      OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                      DrugID = hdgroup.Key.DrugID,
                      DayRpts = hdgroup.Key.DayRpts,
                      V_DrugType = hdgroup.Key.V_DrugType,
                      QtyForDay = hdgroup.Key.QtyForDay,
                      InsuranceCover = hdgroup.Key.InsuranceCover,
                      BrandName = hdgroup.Key.BrandName
                  };

        string strError = "";
        int xNgayToiDa = 30;
                
        //if (Globals.ConfigList != null)
        //{
        //    xNgayToiDa = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
        //}
        // Txd 25/05/2014 Replaced ConfigList
        xNgayToiDa = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;

        //BHValidDays = xNgayToiDa + 1;//tam thoi de vay de khong gioi han het han bao hiem
        //Những thuốc mà bác sĩ không cho ngày dùng (Thuốc Cần).
        string strDrugsNotHaveDayRpts = "";
        //Những thuốc mà hết hạn bảo hiểm. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
        string strInvalidDrugs = "";
        for (int i = 0; i < hhh.Count(); i++)
        {
            int xban = 0;
            int xbantoida = 0;
            string str = "Lý do: Hết hạn BHYT";
            if (hhh.ToList()[i].QtyOffer >= 0)
            {
                //Nếu thuốc có số ngày yêu cầu > 0
                if (hhh.ToList()[i].DayRpts > 0)
                {
                    //Kiên: Nếu như là thuốc CẦN thì sẽ không có QtyForDay, cho nên xài xbantoida, xban ở dưới.
                    //xbantoida = Convert.ToInt32(Math.Ceiling((double)(hhh.ToList()[i].QtyForDay * xNgayToiDa)));
                    //xban = Convert.ToInt32(Math.Ceiling((double)(hhh.ToList()[i].QtyForDay * Days)));
                    xbantoida = Convert.ToInt32(Math.Ceiling(((double)hhh.ToList()[i].QtyOffer / hhh.ToList()[i].DayRpts) * xNgayToiDa));
                    xban = Convert.ToInt32(Math.Ceiling(((double)hhh.ToList()[i].QtyOffer / hhh.ToList()[i].DayRpts) * BHValidDays));
                    //Nếu số lượng muốn bán > số lượng mà bảo hiểm còn hiệu lực. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
                    if (hhh.ToList()[i].OutQuantity > xban)
                    {
                        strInvalidDrugs += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                    //Nếu SLYC > Số lượng bán tối đa.
                    if (hhh.ToList()[i].QtyOffer > xbantoida)
                    {
                        if (hhh.ToList()[i].OutQuantity > xbantoida)
                        {
                            str = "Lý do: BH chi trả tối đa " + xNgayToiDa.ToString() + " ngày thuốc";
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ". SL được bán " + xbantoida + Environment.NewLine + str + Environment.NewLine;
                            ListDrugAndQtySell item = new ListDrugAndQtySell();
                            item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                            item.xban = xbantoida;
                            ListDrugTemp.Add(item);
                        }
                    }

                    //Nếu SLYC <= Số lượng bán tối đa.
                    else
                    {
                        //Nếu thực xuất > SLYC.
                        if (hhh.ToList()[i].OutQuantity > hhh.ToList()[i].QtyOffer)
                        {
                            str = "Lý do: SL bán vượt SL Bác Sĩ YC";
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ". SL B.sĩ kê " + hhh.ToList()[i].QtyOffer + Environment.NewLine + str + Environment.NewLine;
                            ListDrugAndQtySell item = new ListDrugAndQtySell();
                            item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                            item.xban = hhh.ToList()[i].QtyOffer;
                            ListDrugTemp.Add(item);
                        }
                    }
                }
                //Nếu thuốc có số ngày yêu cầu = 0
                else
                {
                    //Nếu thực xuất > SLYC thì không cho bán.
                    if (hhh.ToList()[i].OutQuantity > hhh.ToList()[i].QtyOffer)
                    {
                        str = "Lý do: SL bán vượt SL Bác Sĩ YC";
                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + ". SL B.sĩ kê " + hhh.ToList()[i].QtyOffer + Environment.NewLine + str + Environment.NewLine;
                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.xban = hhh.ToList()[i].QtyOffer;
                        ListDrugTemp.Add(item);
                    }
                    //Nếu thực xuất <= SLYC thì hiện thông báo: Bảo hiểm chỉ trả tối đa 30 ngày thuốc, nhưng bác sĩ không nhập số ngày cho nên số thuốc này có thể ít hơn hoặc nhiều hơn số lượng thuốc bảo hiểm đồng ý chi trả. Bạn có đồng ý bán không?
                    else
                    {
                        //Những thuốc không có số ngày yêu cầu.
                        strDrugsNotHaveDayRpts += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + " có SLYC < 0 nên không được bán. Nếu muốn bán toa này phải xóa thuốc đó đi.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
        }
        if (!string.IsNullOrEmpty(strError))
        {
            if (MessageBox.Show(strError + Environment.NewLine + "Bạn có muốn điều chỉnh số lượng cho chính xác không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //sua lai so luong xuat cho hop li
                for (int i = 0; i < ListDrugTemp.Count; i++)
                {
                    //var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == ListDrugTemp[i].DrugID).OrderByDescending(x => x.Qty);
                    //Kiên: Nếu thuốc đó có 2 lô trở lên thì sắp xếp theo QtyOffer giảm dần (vì dòng nào có QtyOffer nhiều nhất là thuốc của dòng đó hết hạn trước).
                    var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == ListDrugTemp[i].DrugID).OrderByDescending(x => x.QtyOffer);
                    if (values != null)
                    {
                        int xban = ListDrugTemp[i].xban;
                        foreach (OutwardDrug p in values)
                        {
                            if (xban <= 0)
                            {
                                DeleteOutwardDrugs(p);
                            }
                            else
                            {
                                if (p.OutQuantity <= xban)
                                {
                                    //Kiên: Trường hợp thuốc có 2 lô trở lên.
                                    if (p.QtyOffer <= xban)
                                    {
                                        p.OutQuantity = p.QtyOffer;
                                    }
                                    xban = xban - p.OutQuantity;
                                }
                                else //p.OutQuantity > xban
                                {
                                    p.OutQuantity = xban;
                                    xban = 0;
                                }
                            }
                        }
                    }
                }
                SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
                SumTotalPrice();
            }
            return false;
        }
        else
        {
            if (!string.IsNullOrEmpty(strInvalidDrugs))
            {
                if (MessageBox.Show("Bảo hiểm của bệnh nhân còn hiệu lực: " + BHValidDays + " ngày." + Environment.NewLine + "Nhưng thuốc: " + Environment.NewLine + strInvalidDrugs + "được bán hơn số ngày bảo hiểm còn hiệu lực." + Environment.NewLine + "Bạn có đồng ý bán không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(strDrugsNotHaveDayRpts))
            {
                if (MessageBox.Show("Bảo hiểm chỉ trả tối đa 30 ngày, nhưng bác sĩ không nhập số ngày cho thuốc: " + Environment.NewLine + strDrugsNotHaveDayRpts + "Nên số thuốc này có thể ít hoặc nhiều hơn số lượng thuốc bảo hiểm đồng ý trả." + Environment.NewLine + "Bạn có đồng ý bán không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
        }
    }
    return true;
}*/




//public void btnNgayDung()
//{
//    int NgayDung = 0;
//    if (Int32.TryParse(strNgayDung, out NgayDung))
//    {
//        if (NgayDung > 0)
//        {
//            if (IsHIPatient) //toa bao hiem
//            {
//                int xNgayBHToiDa_NgoaiTru = 30;

//                //if (Globals.ConfigList != null)
//                //{
//                //    xNgayBHToiDa_NgoaiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
//                //}
//                // Txd 25/05/2014 Replaced ConfigList
//                xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;

//                //KMx: Lấy thuốc có ngày dùng nhỏ nhất. Nếu ngày dùng muốn cập nhật > ngày dùng nhỏ nhất thì không cho cập nhật.
//                var DrugMinDayRpts = SelectedOutInvoice.OutwardDrugs.Where(x => x.DayRpts > 0).OrderBy(x => x.DayRpts).Take(1);

//                if (NgayDung > xNgayBHToiDa_NgoaiTru)
//                {
//                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0207_G1_Msg_InfoBHTraToiDa) + xNgayBHToiDa_NgoaiTru + " ngày thuốc, vui lòng nhập lại ngày dùng.", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
//                    return;
//                }

//                foreach (OutwardDrug d in DrugMinDayRpts)
//                {
//                    if (NgayDung > d.DayRpts)
//                    {
//                        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0080_G1_Msg_SubBacSiYCThuoc) + d.GetDrugForSellVisitor.BrandName + " (" + d.DayRpts + " ngày). " + Environment.NewLine + "Không được nhập ngày dùng lớn hơn BS yêu cầu.", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
//                        return;
//                    }
//                }
//            }

//            if (SelectedPrescription == null)
//            {
//                return;
//            }
//            if (SelectedOutInvoice == null)
//            {
//                return;
//            }

//            if (GetDrugForSellVisitorList == null)
//            {
//                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
//            }
//            else
//            {
//                GetDrugForSellVisitorList.Clear();
//            }

//            //dung de lay ton hien tai va ton ban dau cua tung thuoc theo lo
//            ObservableCollection<GetDrugForSellVisitor> temp = GetDrugForSellVisitorListByPrescriptID.DeepCopy();
//            LaySoLuongTheoNgayDung(temp);

//            //dung de lay ton hien tai va ton ban dau cua tung thuoc da duoc sum
//            var ObjSumRemaining = from hd in GetDrugForSellVisitorList
//                                  group hd by new { hd.DrugID, hd.BrandName } into hdgroup
//                                  select new
//                                  {
//                                      Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
//                                      RemainingFirst = hdgroup.Sum(groupItem => groupItem.RemainingFirst),
//                                      DrugID = hdgroup.Key.DrugID,
//                                      BrandName = hdgroup.Key.BrandName
//                                  };

//            //lay tong so luong yc cua cac thuoc co trong toa (dua vao ngay dung > 0) 
//            var hhh = from hd in SelectedOutInvoice.OutwardDrugs
//                      where hd.DayRpts > 0
//                      group hd by new { hd.DrugID, hd.DayRpts, hd.QtyForDay, hd.GetDrugForSellVisitor.BrandName } into hdgroup
//                      select new
//                      {
//                          QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
//                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
//                          DrugID = hdgroup.Key.DrugID,
//                          DayRpts = hdgroup.Key.DayRpts,
//                          QtyForDay = hdgroup.Key.QtyForDay,
//                          BrandName = hdgroup.Key.BrandName
//                      };

//            string ThongBao = "";
//            //Lưu ý: Khi SelectedOutInvoice.OutwardDrugs add thêm 1 đối tượng (OutwardDrug) thì hhh.Count() tự động tăng lên 1.
//            for (int i = 0; i < hhh.Count(); i++)
//            {
//                // int xbantoida = Convert.ToInt32(Math.Ceiling(((double)hhh.ToList()[i].QtyOffer / hhh.ToList()[i].DayRpts) * NgayDung));
//                int xbantoida = Convert.ToInt32(Math.Ceiling((double)(hhh.ToList()[i].QtyForDay * NgayDung)));
//                if (xbantoida > 0)
//                {
//                    //var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault()).OrderByDescending(x => x.Qty);
//                    var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault()).OrderBy(x => x.InExpiryDate);
//                    if (values != null && values.Count() > 0)
//                    {
//                        //Neu so luong < So luong hien co tren luoi thi chi can ham ben duoi
//                        int xban = xbantoida;
//                        foreach (OutwardDrug p in values)
//                        {
//                            if (xban <= 0)
//                            {
//                                DeleteOutwardDrugs(p);
//                            }
//                            else if (p.OutQuantity > xban)
//                            {
//                                p.OutQuantity = xban;
//                                xban = 0;
//                            }
//                            else if (p.OutQuantity <= xban)
//                            {
//                                xban = xban - p.OutQuantity;
//                            }
//                        }
//                        //else neu > so luong hien co tren luoi thi them vao
//                        if (xban > 0)
//                        {
//                            var Obj = ObjSumRemaining.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault());
//                            if (Obj != null && Obj.Count() > 0)
//                            {
//                                if (Obj.ToArray()[0].Remaining > 0 && Obj.ToArray()[0].Remaining >= xban)
//                                {
//                                    GetDrugForSellVisitor item = new GetDrugForSellVisitor();
//                                    item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
//                                    item.BrandName = hhh.ToList()[i].BrandName;
//                                    //KMx: Phải assign Remaining trước RequiredNumber. Nếu không sẽ bị lỗi, vì khi assign RequiredNumber thì nó sẽ so sánh với Remaining.
//                                    item.Remaining = Obj.ToArray()[0].Remaining;
//                                    item.RequiredNumber = xban;
//                                    item.Qty = hhh.ToList()[i].QtyOffer;
//                                    //item.Remaining = Obj.ToArray()[0].Remaining;
//                                    //if (xban > Obj.ToArray()[0].RemainingFirst)
//                                    //{
//                                    //    ThongBao = ThongBao + "Thuốc " + Obj.ToArray()[0].BrandName + ": SL cần bán " + xbantoida.ToString() + ", SL còn lại " + Obj.ToArray()[0].RemainingFirst.ToString() + Environment.NewLine;
//                                    //}

//                                    ReCountQtyAndAddList(item, false);
//                                }
//                                else
//                                {
//                                    if (Obj.ToArray()[0].RemainingFirst > 0)
//                                    {
//                                        ThongBao = ThongBao + "Thuốc " + Obj.ToArray()[0].BrandName + ": SL cần bán " + xbantoida.ToString() + ", SL còn lại " + Obj.ToArray()[0].RemainingFirst.ToString() + Environment.NewLine;
//                                    }
//                                    else
//                                    {
//                                        ThongBao = ThongBao + "Thuốc " + Obj.ToArray()[0].BrandName + ": SL cần bán " + xbantoida.ToString() + ", SL còn lại 0" + Environment.NewLine;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                ThongBao = ThongBao + "Thuốc " + hhh.ToList()[i].BrandName + " đã hết!" + Environment.NewLine;
//                            }
//                        }
//                    }
//                }
//            }
//            //KMx: Nếu để dòng dưới thì sẽ bị lỗi khi thêm cùng 1 loại thuốc có trong toa nhưng khác lô.
//            //SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
//            SumTotalPrice();
//            if (!string.IsNullOrEmpty(ThongBao))
//            {
//                MessageBox.Show(ThongBao);
//            }
//        }
//        else
//        {
//            MessageBox.Show("Ngày dùng phải > 0!");
//        }
//    }
//    else
//    {
//        MessageBox.Show(eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe);
//    }
//}