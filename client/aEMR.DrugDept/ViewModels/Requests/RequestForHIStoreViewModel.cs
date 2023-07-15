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

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRequestForHIStore)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestForHIStoreViewModel : Conductor<object>, IRequestForHIStore
        , IHandle<DrugDeptCloseSearchRequestForHIStoreEvent>
        , IHandle<DrugDeptLoadAgainReqOutwardClinicDeptEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RequestForHIStoreViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
            SearchCriteria = new RequestSearchCriteria();
            RequestDrug = new RequestDrugInwardForHiStore();
            RefeshRequest();
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


        private RequestDrugInwardForHiStore _RequestDrug;
        public RequestDrugInwardForHiStore RequestDrug
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

        private RequestDrugInwardForHiStoreDetails _CurrentReqOutwardDrugClinicDeptPatient;
        public RequestDrugInwardForHiStoreDetails CurrentReqOutwardDrugClinicDeptPatient
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

        //--▼--24/12/2020 DatTB
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
        //--▲--24/12/2020 DatTB

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
            RequestDrugInwardForHiStoreDetails p = item as RequestDrugInwardForHiStoreDetails;
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
                    foreach (RequestDrugInwardForHiStoreDetails item in RequestDrug.ReqOutwardDetails)
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

        private void RequestDrugInwardHIStore_Approved()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRequestDrugInwardHIStore_Approved(RequestDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndRequestDrugInwardHIStore_Approved(asyncResult);
                            Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                            if (result)
                            {
                                RequestDrug.IsApproved = true;
                                //--▼--24/12/2020 DatTB
                                CanApprove = false;
                                //--▲--24/12/2020 DatTB
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
            return true;
        }


        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardForHiStore> results, int Totalcount)
        {
            Action<IRequestSearch> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsRequestFromHIStore = true;
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.SetList();
                proAlloc.RequestDruglistHIStore.Clear();
                proAlloc.RequestDruglistHIStore.TotalItemCount = Totalcount;
                proAlloc.SetList();
                proAlloc.RequestDruglistHIStore.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardForHiStore p in results)
                    {
                        proAlloc.RequestDruglistHIStore.Add(p);
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

            this.ShowBusyIndicator();

            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-30);// 24/12/2020 DatTB Sửa AddDays -7 thành -30
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
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRequestDrugInwardHIStore(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;
                            var results = contract.EndSearchRequestDrugInwardHIStore(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    this.HideBusyIndicator();
                                    OpenPopUpSearchRequestInvoice(results.ToList(), Total);
                                }
                                else
                                {
                                    RequestDrug = results.FirstOrDefault();
                                    GetReqOutwardDetailsByID(RequestDrug.RequestDrugInwardHiStoreID);
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    this.HideBusyIndicator();
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

            });

            t.Start();
        }

        private void GetRequestDrugListDetailsHIStoreByReqID(long RequestDrugInwardHiStoreID, bool bCreateNewListFromOld = false)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRequestDrugInwardHIStoreDetailByID(RequestDrugInwardHiStoreID, bCreateNewListFromOld, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetRequestDrugInwardHIStoreDetailByID(asyncResult);
                                if (results != null)
                                {

                                    if (!RequestDrug.IsApproved.GetValueOrDefault())
                                    {
                                        //--▼--24/12/2020 DatTB
                                        CanApprove = true;
                                        //--▲--24/12/2020 DatTB
                                        foreach (RequestDrugInwardForHiStoreDetails p in results)
                                        {
                                            p.ApprovedQty = p.ReqQty;
                                        }
                                    }
                                    //--▼--24/12/2020 DatTB
                                    if (RequestDrug.IsApproved.GetValueOrDefault())
                                    {
                                        CanApprove = false;
                                    }
                                    //--▲--24/12/2020 DatTB
                                    RequestDrug.ReqOutwardDetails = results.ToObservableCollection();

                                    FillPagedCollectionAndGroup();
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
                                    RequestDrug = new RequestDrugInwardForHiStore();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        private void GetRequestDrugInwardClinicDeptByID(long RequestID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRequestDrugInwardHIStoreByID(RequestID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            RequestDrug = contract.EndGetRequestDrugInwardHIStoreByID(asyncResult);
                            GetRequestDrugListDetailsHIStoreByReqID(RequestID);
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

        private void GetReqOutwardDetailsByID(long ID)
        {
            GetRequestDrugListDetailsHIStoreByReqID(ID);
        }

        public void RefeshRequest()
        {
            RequestDrug.RequestDrugInwardHiStoreID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.ReqOutwardDetails = new ObservableCollection<RequestDrugInwardForHiStoreDetails>();
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
                    //--▼--24/12/2020 DatTB
                    MessageBox.Show("Phiếu đang Duyệt không thể duyệt nữa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //if (MessageBox.Show(eHCMSResources.Z1284_G1_CoMuonCNhatPhDaDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    //{
                    //    RequestDrugInwardHIStore_Approved();
                    //}
                    //--▲--24/12/2020 DatTB
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.A131_G1_Msg_ConfDongYDuyetPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        RequestDrugInwardHIStore_Approved();
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
            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = RequestDrug.RequestDrugInwardHiStoreID;
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

                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    proAlloc.eItem = ReportName.DrugDept_Request_HIStore_Approved;
                }
                else
                {
                    proAlloc.eItem = ReportName.DrugDept_Request_HIStore;
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = RequestDrug.RequestDrugInwardHiStoreID;
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
                if (RequestDrug.IsApproved.GetValueOrDefault())
                {
                    proAlloc.eItem = ReportName.DrugDept_Request_HIStore_Details_Approved;
                }
                else
                {
                    proAlloc.eItem = ReportName.DrugDept_Request_HIStore_Details;
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        #endregion

        #endregion

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestForHIStoreEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrug = message.SelectedRequest as RequestDrugInwardForHiStore;
                GetReqOutwardDetailsByID(RequestDrug.RequestDrugInwardHiStoreID);
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
            if (CV_ReqOutwardDrugClinicDeptPatientlst == null || CV_ReqOutwardDrugClinicDeptPatientlst.Count <= 0)
            {
                return;
            }
            if (((RequestNewView)this.GetView()).ctrlGridPageSize.Visibility == Visibility.Collapsed)
            {
                if (GridPageSize == -1) 
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
                CV_ReqOutwardDrugClinicDeptPatientlst.Refresh();
                NotifyOfPropertyChange(() => CV_ReqOutwardDrugClinicDeptPatientlst);
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
        }
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
            });
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (sender == null || e == null)
                return;

            if ((sender as DataGrid).SelectedItem != null)
            {
                if (e.Column.Equals(theReqGrid.GetColumnByName("colApprovedQty")))

                {
                    RequestDrugInwardForHiStoreDetails reqItem = e.Row.DataContext as RequestDrugInwardForHiStoreDetails;
                    if (reqItem.ApprovedQty < 0)
                    {
                        MessageBox.Show(eHCMSResources.A0973_G1_Msg_InfoSLgPhaiDuyetLonHon0);
                    }
                }
            }
        }

        #endregion

        private bool _IsApprovedForHIStore = false;
        public bool IsApprovedForHIStore
        {
            get
            {
                return _IsApprovedForHIStore;
            }
            set
            {
                _IsApprovedForHIStore = value;
                NotifyOfPropertyChange(() => IsApprovedForHIStore);
            }
        }
        public void ckbDiscount_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
            {
                return;
            }
            CheckBox ckbDiscount = sender as CheckBox;
            if (ckbDiscount.IsChecked == true)
            {
                IsApprovedForHIStore = true;
            }
            else
            {
                IsApprovedForHIStore = false;
            }
        }
    }
}
