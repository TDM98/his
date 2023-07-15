using eHCMSLanguage;
using System.ComponentModel;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Service.Core.Common;
using aEMR.DrugDept.Views;
using System.Text;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Data;
using aEMR.Controls;
using aEMR.Common.BaseModel;
/*
* 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
* 20181117 #002 TBL:   BM 0005260: Lam tron so luong duyet khi click vao button                    
* 20201030 #003 TNHX:   BM: 
*/
public static class Extensions
{
    public static T FindParentOfType<T>(this FrameworkElement element)
    {
        var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

        while (parent != null)
        {
            if (parent is T)
                return (T)(object)parent;

            parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
        }
        return default(T);
    }

    // Methods
    public static List<T> GetChildrenByType<T>(this UIElement element) where T : UIElement
    {
        return element.GetChildrenByType<T>(null);
    }

    public static List<T> GetChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
    {
        List<T> results = new List<T>();
        GetChildrenByType<T>(element, condition, results);
        return results;
    }

    private static void GetChildrenByType<T>(UIElement element, Func<T, bool> condition, List<T> results) where T : UIElement
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
            if (child != null)
            {
                T t = child as T;
                if (t != null)
                {
                    if (condition == null)
                    {
                        results.Add(t);
                    }
                    else if (condition(t))
                    {
                        results.Add(t);
                    }
                }
                GetChildrenByType<T>(child, condition, results);
            }
        }
    }

    public static bool HasChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
    {
        return (element.GetChildrenByType<T>(condition).Count != 0);
    }
}

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRequestNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestNewViewModel : ViewModelBase, IRequestNew
        , IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<DrugDeptLoadAgainReqOutwardClinicDeptEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RequestNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
            SearchCriteria = new RequestSearchCriteria();
            RequestDrug = new RequestDrugInwardClinicDept();
            RefeshRequest();

            CommonGlobals.GetAllPositionInHospital(this);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //GetOutstandingTaskContent();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        private bool _mDuyetPhieuLinhHang_Tim = true;
        private bool _mDuyetPhieuLinhHang_PhieuMoi = true;
        private bool _mDuyetPhieuLinhHang_XuatHang = true;
        private bool _mDuyetPhieuLinhHang_XemInTH = true;
        private bool _mDuyetPhieuLinhHang_XemInCT = true;

        //--▼--16/12/2020 DatTB
        private bool _CanApprove = true;
        public bool CanApprove
        {
            get { return _CanApprove; }
            set
            {
                if (_CanApprove != value)
                    _CanApprove = value;
                NotifyOfPropertyChange(() => CanApprove);
            }
        }
        //--▲--16/12/2020 DatTB

        public bool mDuyetPhieuLinhHang_Tim
        {
            get
            {
                return _mDuyetPhieuLinhHang_Tim;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_Tim == value)
                    return;
                _mDuyetPhieuLinhHang_Tim = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_Tim);
            }
        }

        public bool mDuyetPhieuLinhHang_PhieuMoi
        {
            get
            {
                return _mDuyetPhieuLinhHang_PhieuMoi;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_PhieuMoi == value)
                    return;
                _mDuyetPhieuLinhHang_PhieuMoi = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_PhieuMoi);
            }
        }

        public bool mDuyetPhieuLinhHang_XuatHang
        {
            get
            {
                return _mDuyetPhieuLinhHang_XuatHang;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_XuatHang == value)
                    return;
                _mDuyetPhieuLinhHang_XuatHang = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_XuatHang);
            }
        }

        public bool mDuyetPhieuLinhHang_XemInTH
        {
            get
            {
                return _mDuyetPhieuLinhHang_XemInTH;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_XemInTH == value)
                    return;
                _mDuyetPhieuLinhHang_XemInTH = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_XemInTH);
            }
        }

        public bool mDuyetPhieuLinhHang_XemInCT
        {
            get
            {
                return _mDuyetPhieuLinhHang_XemInCT;
            }
            set
            {
                if (_mDuyetPhieuLinhHang_XemInCT == value)
                    return;
                _mDuyetPhieuLinhHang_XemInCT = value;
                NotifyOfPropertyChange(() => mDuyetPhieuLinhHang_XemInCT);
            }
        }

        #endregion

        #region 1. Property

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }


        private RequestDrugInwardClinicDept _RequestDrug;
        public RequestDrugInwardClinicDept RequestDrug
        {
            get
            {
                return _RequestDrug;
            }
            set
            {
                if (_RequestDrug != value)
                {
                    _RequestDrug = value;
                    NotifyOfPropertyChange(() => RequestDrug);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }

            }
        }
        private ICheckStatusRequestInvoiceOutStandingTask _ostvm;
        public ICheckStatusRequestInvoiceOutStandingTask ostvm
        {
            get
            {
                return _ostvm;
            }
            set
            {
                if (_ostvm != value)
                {
                    _ostvm = value;
                    NotifyOfPropertyChange(() => ostvm);
                }
            }
        }
        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }
        #endregion

        #region 3. Function Member
        //private void GetOutstandingTaskContent()
        //{
        //    var homevm = Globals.GetViewModel<IHome>();
        //    ostvm = Globals.GetViewModel<ICheckStatusRequestInvoiceOutStandingTask>();
        //    ostvm.V_MedProductType = V_MedProductType;
        //    ostvm.LoadStore();
        //    homevm.OutstandingTaskContent = ostvm;
        //    homevm.IsExpandOST = true;
        //}
        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }
        private void SetDefultRefGenericDrugCategory()
        {
            if (RequestDrug != null && RefGenericDrugCategory_1s != null)
            {
                RequestDrug.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }
        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void ischanged(object item)
        {
            ReqOutwardDrugClinicDeptPatient p = item as ReqOutwardDrugClinicDeptPatient;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }

        private bool CheckValidationEditor()
        {
            bool result = true;
            StringBuilder st = new StringBuilder();
            if (RequestDrug != null)
            {
                if (RequestDrug.ReqOutwardDetails != null)
                {
                    foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                    {
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (!(item.ReqQty >= 0))
                            {
                                st.AppendLine(eHCMSResources.A0538_G1_Msg_InfoSLgYCLonHon0);
                                result = false;
                            }
                        }
                    }
                }
                if (!result)
                {
                    MessageBox.Show(st.ToString());
                }
            }
            return result;
        }

        private void RequestDrugInwardClinicDept_Approved()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRequestDrugInwardClinicDept_Approved(RequestDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndRequestDrugInwardClinicDept_Approved(asyncResult);
                            Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                            if (result)
                            {
                                RequestDrug.IsApproved = true;
                                //--▼--16/12/2020 DatTB
                                CanApprove = false;
                                //--▲--16/12/2020 DatTB
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckValidData()
        {
            //var items = RequestDrug.ReqOutwardDetails.Where(x=>x.Checked==false);
            //if (items != null && items.Count() > 0)
            //{
            //    MessageBox.Show("Bạn phải đánh dấu vào các hàng đã duyệt qua!");
            //    return false;
            //}
            return true;
        }


        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardClinicDept> results, int Totalcount)
        {
            //var proAlloc = Globals.GetViewModel<IRequestSearch>();
            //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            //proAlloc.V_MedProductType = V_MedProductType;
            //proAlloc.RequestDruglist.Clear();
            //proAlloc.RequestDruglist.TotalItemCount = Totalcount;
            //proAlloc.RequestDruglist.PageIndex = 0;
            //if (results != null && results.Count > 0)
            //{
            //    foreach (RequestDrugInwardClinicDept p in results)
            //    {
            //        proAlloc.RequestDruglist.Add(p);
            //    }
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            IRequestSearch proAlloc = Globals.GetViewModel<IRequestSearch>();
            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            proAlloc.V_MedProductType = V_MedProductType;
            proAlloc.RequestDruglist.Clear();
            proAlloc.RequestDruglist.TotalItemCount = Totalcount;
            proAlloc.RequestDruglist.PageIndex = 0;
            proAlloc.RequestDruglist.PageSize = 20;
            proAlloc.IsFromApprovePage = true;
            if (results != null && results.Count > 0)
            {
                foreach (RequestDrugInwardClinicDept p in results)
                {
                    proAlloc.RequestDruglist.Add(p);
                }
            }
            GlobalsNAV.ShowDialog_V3(proAlloc, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            this.ShowBusyIndicator();

            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-30); // 24/12/2020 DatTB Sửa AddDays -7 thành -30
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    bool IsRunHideBusy = false;
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;

                            var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    this.HideBusyIndicator();
                                    IsRunHideBusy = true;
                                    OpenPopUpSearchRequestInvoice(results.ToList(), Total);
                                }
                                else
                                {
                                    RequestDrug = results.FirstOrDefault();
                                    GetReqOutwardDetailsByID(RequestDrug.ReqDrugInClinicDeptID);
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    this.HideBusyIndicator();
                                    IsRunHideBusy = true;
                                    SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                    SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                    OpenPopUpSearchRequestInvoice(null, 0);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (!IsRunHideBusy)
                            {
                                this.HideBusyIndicator();
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetReqOutwardDrugClinicDeptPatientByID(long RequestID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetReqOutwardDrugClinicDeptPatientByID(RequestID, false, (long)AllLookupValues.RegistrationType.NOI_TRU
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetReqOutwardDrugClinicDeptPatientByID(asyncResult);
                            if (results != null)
                            {
                                //if(RequestDrug.stor)
                                if (RequestDrug.OutFromStoreID == 1 && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2444_G1_PhieuLinhDangYCTuKD);
                                }
                                if (!RequestDrug.IsApproved.GetValueOrDefault())
                                {
                                    //--▼--16/12/2020 DatTB
                                    CanApprove = true;
                                    //--▲--16/12/2020 DatTB
                                    foreach (ReqOutwardDrugClinicDeptPatient p in results)
                                    {
                                        p.ApprovedQty = Convert.ToDecimal(p.ReqQtyStr); //20191029 TBL: Số lượng duyệt sẽ được lấy theo số lượng YC bên phiếu lĩnh
                                    }
                                }
                                //--▼--16/12/2020 DatTB
                                if (RequestDrug.IsApproved.GetValueOrDefault())
                                {
                                    CanApprove = false;
                                }
                                //--▲--16/12/2020 DatTB
                                RequestDrug.ReqOutwardDetails = results.ToObservableCollection();
                                FillPagedCollectionAndGroup();
                                //PclServiceDetails = new PagedCollectionView(RequestDrug.ReqOutwardDetails);
                                //PclServiceDetails.GroupDescriptions.Add(new PropertyGroupDescription("RefGenericDrugDetail"));
                            }
                            else
                            {
                                if (RequestDrug.ReqOutwardDetails != null)
                                {
                                    RequestDrug.ReqOutwardDetails.Clear();
                                }
                            }
                            if (RequestDrug == null)
                            {
                                RequestDrug = new RequestDrugInwardClinicDept();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRequestDrugInwardClinicDeptByID(long RequestID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRequestDrugInwardClinicDeptByID(RequestID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            RequestDrug = contract.EndGetRequestDrugInwardClinicDeptByID(asyncResult);
                            GetReqOutwardDrugClinicDeptPatientByID(RequestID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GetReqOutwardDetailsByID(long ID)
        {
            GetReqOutwardDrugClinicDeptPatientByID(ID);
        }

        public void RefeshRequest()
        {
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            SetDefultRefGenericDrugCategory();
        }

        private bool CheckValidGrid()
        {
            bool results = true;
            if (RequestDrug == null)
            {
                MessageBox.Show(eHCMSResources.A0800_G1_Msg_ErrPhYCNull);
                return false;
            }
            if (RequestDrug.FromDate == null || RequestDrug.ToDate == null || RequestDrug.ReqDate == null)
            {
                MessageBox.Show(eHCMSResources.A0780_G1_Msg_InfoNhapDayDuTTin);
                return false;
            }
            if (eHCMS.Services.Core.AxHelper.CompareDate(RequestDrug.ReqDate, DateTime.Now) == 1)
            {
                MessageBox.Show(eHCMSResources.A0794_G1_Msg_InfoLoiNgLapPh);
                return false;
            }
            if (RequestDrug.FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || RequestDrug.ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || RequestDrug.ReqDate.Year <= DateTime.Now.Year - 50)
            {
                MessageBox.Show(eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe);
                return false;
            }

            if (RequestDrug.ReqOutwardDetails != null)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0 && RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                    {
                        results = false;
                        MessageBox.Show(string.Format(eHCMSResources.Z1283_G1_DLieuKgHopLe, (i + 1).ToString()));
                        break;
                    }
                }
            }
            return results;
        }
        #endregion

        #region client member

        public void btnApprove(object sender, RoutedEventArgs e)
        {
            if (CheckValidGrid() && CheckValidData())
            {
                RequestDrug.ApprovedStaffID = GetStaffLogin().StaffID;
                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    //--▼--16/12/2020 DatTB
                    //if (MessageBox.Show(eHCMSResources.Z1284_G1_CoMuonCNhatPhDaDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    //{
                    //    RequestDrugInwardClinicDept_Approved();
                    //}
                    MessageBox.Show("Phiếu đang Duyệt không thể duyệt nữa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //--▲--16/12/2020 DatTB
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.A131_G1_Msg_ConfDongYDuyetPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        RequestDrugInwardClinicDept_Approved();
                    }
                }

            }
        }

        public void btnCancel(object sender, RoutedEventArgs e)
        {
            if (CheckValidGrid() && CheckValidData())
            {
                RequestDrug.ApprovedStaffID = GetStaffLogin().StaffID;
                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginRequestDrugInwardClinicDept_Cancel(RequestDrug, (long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var result = contract.EndRequestDrugInwardClinicDept_Cancel(asyncResult);
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                    }
                                    catch (Exception ex)
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                else
                {
                    MessageBox.Show(eHCMSResources.Z3077_G1_PhieuChuaDuyetKhongTheHuy);
                }
            }
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                btnFindRequest();
            }
        }

        public void btnFindRequest()
        {
            SearchRequestDrugInwardClinicDept(0, 20);
        }


        public void btnNew(object sender, RoutedEventArgs e)
        {
            RefeshRequest();
        }

        #region printing member

        public void btnPreviewTH()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
            //    }
            //    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
            //    }
            //    else
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
            //    }
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z1144_G1_PhLinhYCu;
            //}
            //else
            //{
            //    proAlloc.LyDo = eHCMSResources.Z1145_G1_PhLinhHChat;
            //}

            //if (RequestDrug.IsApproved.GetValueOrDefault())
            //{
            //    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_APPROVED;
            //}
            //else
            //{
            //    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST;
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
                    }
                    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z1144_G1_PhLinhYCu;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3217_G1_PhLinhDDuong;
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z1145_G1_PhLinhHChat;
                }
                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_APPROVED;
                }
                else
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST;
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
            //    }
            //    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
            //    }
            //    else
            //    {
            //        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
            //    }
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z1144_G1_PhLinhYCu;
            //}
            //else
            //{
            //    proAlloc.LyDo = eHCMSResources.Z1145_G1_PhLinhHChat;
            //}
            //if (RequestDrug.IsApproved.GetValueOrDefault())
            //{
            //    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS_APPROVED;
            //}
            //else
            //{
            //    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS;
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
                    }
                    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z1144_G1_PhLinhYCu;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3217_G1_PhLinhDDuong;
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z1145_G1_PhLinhHChat;
                }
                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS_APPROVED;
                }
                else
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS;
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRequestPharmacyInPdfFormat(RequestDrug.ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRequestPharmacyInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #endregion

        #endregion

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrug = message.SelectedRequest as RequestDrugInwardClinicDept;
                GetReqOutwardDetailsByID(RequestDrug.ReqDrugInClinicDeptID);
            }
        }

        #endregion

        #region Checked All Member
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }


        private void AllCheckedfc()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    RequestDrug.ReqOutwardDetails[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    RequestDrug.ReqOutwardDetails[i].Checked = false;
                }
            }
        }
        #endregion

        public void btnOutward()
        {
            // var proAlloc = Globals.GetViewModel<IXuatNoiBo>();
            //  proAlloc.IsOutClinicDept = false;
            //  if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //  {
            //      proAlloc.strHienThi = eHCMSResources.Z1294_G1_XuatThuocKhPhg;
            //  }
            //  else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //  {
            //      proAlloc.strHienThi = eHCMSResources.Z1295_G1_XuatYCuKhPhg;
            //  }
            //  else
            //  {
            //      proAlloc.strHienThi = eHCMSResources.Z1296_G1_XuatHChatKhPhg;
            //  }
            //  proAlloc.V_MedProductType = V_MedProductType;

            ////  proAlloc.spGetInBatchNumberAndPrice_ByRequestID(RequestDrug.ReqDrugInClinicDeptID, RequestDrug.OutFromStoreID.GetValueOrDefault(), false, RequestDrug);
            //  var instance = proAlloc as Conductor<object>;
            //  Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(IXuatNoiBo proAlloc)
            {
                proAlloc.IsOutClinicDept = false;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.strHienThi = eHCMSResources.Z1294_G1_XuatThuocKhPhg;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.strHienThi = eHCMSResources.Z1295_G1_XuatYCuKhPhg;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.strHienThi = eHCMSResources.Z3218_G1_XuatDDuongKhPhg;
                }
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z1296_G1_XuatHChatKhPhg;
                }
                proAlloc.V_MedProductType = V_MedProductType;
            }
            GlobalsNAV.ShowDialog<IXuatNoiBo>(onInitDlg);

            Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestEvent { SelectedRequest = RequestDrug });
        }

        public void Handle(DrugDeptLoadAgainReqOutwardClinicDeptEvent message)
        {
            if (message != null && message.RequestID > 0)
            {
                GetRequestDrugInwardClinicDeptByID(message.RequestID);
            }
        }

        #region Request Details Grid

        private void FillGroupName()
        {
            if (CV_ReqOutwardDrugClinicDeptPatientlst != null && CV_ReqOutwardDrugClinicDeptPatientlst.Count > 0)
            {
                CV_ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Clear();
                CV_ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Clear();
                CV_ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Add(new PropertyGroupDescription("RefGenericDrugDetail.BrandNameAndCode"));
                CV_ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new SortDescription("RefGenericDrugDetail.BrandNameAndCode", ListSortDirection.Ascending));
                CV_ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new SortDescription("CurPatientRegistration.Patient.PatientCodeAndName", ListSortDirection.Ascending));
                CV_ReqOutwardDrugClinicDeptPatientlst.Filter = null;
                //CV_ReqOutwardDrugClinicDeptPatientlst.

                CloseOrOpenGroups(true);
            }
        }

        private const int DefPageSize = 50;
        private int _GridPageSize = -1;
        public int GridPageSize
        {
            get { return _GridPageSize; }
            set
            {
                _GridPageSize = value;
                NotifyOfPropertyChange(() => GridPageSize);
            }
        }

        private int nTotNumOfRows = 0;
        private int nTotNumOfPages = 0;
        private int nCurPageIdx = 1;
        private string _CurPageIndex;
        public string CurPageIndex
        {
            get { return _CurPageIndex; }
            set
            {
                _CurPageIndex = value;
                NotifyOfPropertyChange(() => CurPageIndex);
            }
        }

        public void btnSetPageSize()
        {
            // TxD 09/07/2018: CollectionView DOES NOT HAVE PageSize so we may have to implement this property MANUALLY by filtering
            //                 Added a new var line counter into the List and use filter to Page each group
            if (CV_ReqOutwardDrugClinicDeptPatientlst == null || CV_ReqOutwardDrugClinicDeptPatientlst.Count <= 0)
                return;

            // Use default PageSize == 50 when the following control is invisible and thus the btnSetPageSize
            //  will toggle between setting 50 rows per page and full 
            if (((RequestNewView)this.GetView()).ctrlGridPageSize.Visibility == Visibility.Collapsed)
            {
                if (GridPageSize == -1)     // Initial Page Size NOT YET Set
                {
                    if (DefPageSize < nTotNumOfRows)
                        GridPageSize = DefPageSize;
                    else
                        return;
                }
                else
                {
                    if (GridPageSize == 0)
                        GridPageSize = DefPageSize;
                    else
                        GridPageSize = 0;
                }
            }
            else
            {
                if (GridPageSize < 0 || GridPageSize > nTotNumOfRows)
                    return;
            }

            // TxD 09/07/2018 PageSize is not a Property of 
            //CV_ReqOutwardDrugClinicDeptPatientlst.PageSize = GridPageSize;

            bAllGroupsAlreadyClosed = false;
            nTotNumOfPages = 1;
            CurPageIndex = "";
            nCurPageIdx = 1;

            if (GridPageSize > 0 && GridPageSize < nTotNumOfRows)
            {
                nTotNumOfPages = nTotNumOfRows / GridPageSize + (nTotNumOfRows % GridPageSize > 0 ? 1 : 0);
            }

        }

        public void btnNextPage()
        {
            if (CV_ReqOutwardDrugClinicDeptPatientlst == null || CV_ReqOutwardDrugClinicDeptPatientlst.Count <= 0)
                return;
            if (nCurPageIdx == nTotNumOfPages)
                return;
            if (GridPageSize <= 0)
                return;

            // TxD 09/07/2018 MoveToNextPage is not in CollectionView TBR
            //ReqOutwardDrugClinicDeptPatientlst.MoveToNextPage();
            CurPageIndex = (++nCurPageIdx).ToString() + "/" + nTotNumOfPages.ToString();
        }

        public void btnPrevPage()
        {
            if (CV_ReqOutwardDrugClinicDeptPatientlst == null || CV_ReqOutwardDrugClinicDeptPatientlst.Count <= 0)
                return;
            if (nCurPageIdx == 1)
                return;
            if (GridPageSize <= 0)
                return;

            // TxD 09/07/2018 MoveToPreviousPage is not in CollectionView TBR
            //CV_ReqOutwardDrugClinicDeptPatientlst.MoveToPreviousPage();            
            CurPageIndex = (--nCurPageIdx).ToString() + "/" + nTotNumOfPages.ToString();
        }

        private bool bAllGroupsAlreadyClosed = false;

        public void btnCloseOpenGroups()
        {
            bool bClose = false;
            if (!bAllGroupsAlreadyClosed)
            {
                bClose = true;
            }

            CloseOrOpenGroups(bClose);
        }

        public void btnRefresh()
        {
            if (CV_ReqOutwardDrugClinicDeptPatientlst != null && CV_ReqOutwardDrugClinicDeptPatientlst.Count > 0)
            {
                //this.ShowBusyIndicator();
                CV_ReqOutwardDrugClinicDeptPatientlst.Refresh();
                NotifyOfPropertyChange(() => CV_ReqOutwardDrugClinicDeptPatientlst);
                //this.HideBusyIndicator();
                bAllGroupsAlreadyClosed = false;
            }
        }

        private void CloseOrOpenGroups(bool bClosed)
        {
            if (theReqGrid == null)
                return;
            if (CV_ReqOutwardDrugClinicDeptPatientlst == null || CV_ReqOutwardDrugClinicDeptPatientlst.Count <= 0)
                return;

            bAllGroupsAlreadyClosed = bClosed;

            //foreach (CollectionViewGroup group in ReqOutwardDrugClinicDeptPatientlst.Groups)
            //{
            //    if (bClosed)
            //    {
            //        theReqGrid.CollapseRowGroup(group, true);
            //    }
            //    else
            //    {
            //        theReqGrid.ExpandRowGroup(group, true);
            //    }
            //}
        }
        //==== #002 ====
        public void btnCeiling()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                foreach (var item in RequestDrug.ReqOutwardDetails)
                {
                    item.ApprovedQty = Math.Ceiling(item.ApprovedQty);
                }
                FillPagedCollectionAndGroup();
            }
        }
        //==== #002 ====
        private void FillPagedCollectionAndGroup()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                CVS_ReqOutwardDrugClinicDeptPatientlst = new CollectionViewSource { Source = RequestDrug.ReqOutwardDetails };
                CV_ReqOutwardDrugClinicDeptPatientlst = (CollectionView)CVS_ReqOutwardDrugClinicDeptPatientlst.View;
                FillGroupName();
                NotifyOfPropertyChange(() => CV_ReqOutwardDrugClinicDeptPatientlst);

                nCurPageIdx = 1;
                nTotNumOfPages = 1;
                nTotNumOfRows = RequestDrug.ReqOutwardDetails.Count;
            }
        }

        private CollectionViewSource _cvs_ReqOutwardDrugClinicDeptPatientlst = null;
        public CollectionViewSource CVS_ReqOutwardDrugClinicDeptPatientlst
        {
            get
            {
                return _cvs_ReqOutwardDrugClinicDeptPatientlst;
            }
            set
            {
                _cvs_ReqOutwardDrugClinicDeptPatientlst = value;
            }
        }

        public CollectionView CV_ReqOutwardDrugClinicDeptPatientlst { get; set; }


        private DataGrid theReqGrid = null;
        private bool bTheReqGridInitDone = false;
        public void grdReqOutwardDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                theReqGrid = sender as DataGrid;
                if (theReqGrid != null && bTheReqGridInitDone == false)
                {
                    //theReqGrid.LayoutUpdated += new EventHandler(datagrid1_LayoutUpdated);

                    //theReqGrid.UnloadingRowGroup += (s1, s2) => { Resize(); };
                    //theReqGrid.LoadingRowGroup += (s1, s2) => { refreshUI = true; };
                    bTheReqGridInitDone = true;
                }
            }
        }

        bool refreshUI = true;
        List<StackPanel> headers = null;
        DataGridColumnHeadersPresenter dghc = null;
        void datagrid1_LayoutUpdated(object sender, EventArgs e)
        {
            if (refreshUI && headers == null)
            {
                dghc = theReqGrid.GetChildrenByType<DataGridColumnHeadersPresenter>().FirstOrDefault();
                //foreach (DataGridColumnHeader dgch in dghc.Children)
                //{
                //    dgch.SizeChanged += (s, args) => { Resize(); };
                //}
            }
            if (refreshUI)
                Resize();

            refreshUI = false;
        }

        void Resize()
        {
            int sg2 = 0;
            const int FirstColOffSet = 22;
            headers = theReqGrid.GetChildrenByType<StackPanel>().Where(x => x.Name == "ghsp").ToList();
            headers.ForEach(x =>
            {
                //for (int i = 1; i < dghc.Children.Count - 1; i++)
                //{                    
                //    sg2 = (int)dghc.Children[i].RenderSize.Width;
                //    if (i == 1)
                //    {
                //        sg2 += FirstColOffSet;
                //    }                    
                //    (x.Children[i - 1] as DataGridCell).Width = sg2;
                //}
            });
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //public void grdReqOutwardDetails_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if (sender == null || e == null)
        //        return;

        //    if ((sender as DataGrid).SelectedItem != null)
        //    {
        //        if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colApprovedQty")
        //        {
        //            ReqOutwardDrugClinicDeptPatient reqItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;
        //            if (reqItem.ApprovedQty < 0)
        //            {
        //                MessageBox.Show(eHCMSResources.A0973_G1_Msg_InfoSLgPhaiDuyetLonHon0);
        //            }
        //        }
        //    }
        //}

        //▼====== #001: Đổi từ CellEditEnded => CellEditEnding vì Ended bị lỗi bên WPF không xài đc.
        public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (sender == null || e == null)
                return;

            if ((sender as DataGrid).SelectedItem != null)
            {
                //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colApprovedQty")
                if (e.Column.Equals(theReqGrid.GetColumnByName("colApprovedQty")))

                {
                    ReqOutwardDrugClinicDeptPatient reqItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;
                    if (reqItem.ApprovedQty < 0)
                    {
                        MessageBox.Show(eHCMSResources.A0973_G1_Msg_InfoSLgPhaiDuyetLonHon0);
                    }
                }
            }
        }
        //▲====== #001

        #endregion

    }
}
