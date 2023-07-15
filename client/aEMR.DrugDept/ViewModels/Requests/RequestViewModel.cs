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
using Service.Core.Common;
using System.Text;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR;
using aEMR.Common.BaseModel;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestViewModel : ViewModelBase, IRequest, IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<DrugDeptLoadAgainReqOutwardClinicDeptEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            eventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
            SearchCriteria = new RequestSearchCriteria();
            RequestDrug = new RequestDrugInwardClinicDept();
            RefeshRequest();

            CommonGlobals.GetAllPositionInHospital(this);
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

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }
        #endregion

        #region 3. Function Member

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
                            if (!(item.ReqQty > 0))
                            {
                                st.AppendLine(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

            Action<IRequestSearch> onInitDlg = (proAlloc) =>
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardClinicDept p in results)
                    {
                        proAlloc.RequestDruglist.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IRequestSearch>(onInitDlg);
        }

        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }
            //to do
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchRequestDrugInwardClinicDept(out int Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetReqOutwardDrugClinicDeptPatientByID(long RequestID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //IsLoading = true;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                    if (!RequestDrug.IsApproved.GetValueOrDefault())
                                    {
                                        foreach (ReqOutwardDrugClinicDeptPatient p in results)
                                        {
                                            p.ApprovedQty = p.ReqQty;
                                        }
                                    }
                                    RequestDrug.ReqOutwardDetails = results.ToObservableCollection();

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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private PagedCollectionView _pclServiceDetails;
        public PagedCollectionView PclServiceDetails
        {
            get { return _pclServiceDetails; }
            set
            {
                if (_pclServiceDetails != value)
                {
                    _pclServiceDetails = value;
                    NotifyOfPropertyChange(() => PclServiceDetails);
                }
            }
        }

        private void GetRequestDrugInwardClinicDeptByID(long RequestID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetReqOutwardDetailsByID(long ID)
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
            //if (Axheper.CompareDate(RequestDrug.ReqDate, DateTime.Now) == 1)
            //{
            //    MessageBox.Show(eHCMSResources.A0794_G1_Msg_InfoLoiNgLapPh);
            //    return false;
            //}
            if (RequestDrug.FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || RequestDrug.ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || RequestDrug.ReqDate.Year <= DateTime.Now.Year - 50)
            {
                MessageBox.Show(eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe);
                return false;
            }

            if (RequestDrug.ReqOutwardDetails != null)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0 && RequestDrug.ReqOutwardDetails[i].ReqQty <= 0)
                    {
                        results = false;
                        MessageBox.Show(string.Format(eHCMSResources.Z1297_G1_DLieuDongThu0KgHopLe, (i + 1).ToString(), eHCMSResources.A0538_G1_Msg_InfoSLgYCLonHon0));
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
                    if (MessageBox.Show(eHCMSResources.A0136_G1_Msg_ConfCNhatPhDaDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        RequestDrugInwardClinicDept_Approved();
                    }
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
            SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
            //  Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestEvent { SelectedRequest = RequestDrug });

            Action<IXuatNoiBo> onInitDlg = (proAlloc) =>
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
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z1296_G1_XuatHChatKhPhg;
                }
                proAlloc.V_MedProductType = V_MedProductType;
            };
            GlobalsNAV.ShowDialog<IXuatNoiBo>(onInitDlg);
            //23062018 TTM 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestEvent { SelectedRequest = RequestDrug });
        }

        public void Handle(DrugDeptLoadAgainReqOutwardClinicDeptEvent message)
        {
            if (message != null && message.RequestID > 0)
            {
                GetRequestDrugInwardClinicDeptByID(message.RequestID);
            }
        }
    }
}
