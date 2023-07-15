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
using System.Windows.Media;
using Service.Core.Common;
using System.Text;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using Castle.Windsor;

namespace aEMR.StoreDept.Requests.ViewModels
{
    [Export(typeof(IStoreDeptRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestViewModel : Conductor<object>, IStoreDeptRequest, IHandle<DrugDeptCloseSearchRequestEvent>
    {
        [ImportingConstructor]
        public RequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            //Coroutine.BeginExecute(DoGetStore_All());
            StoreCbxStaff = Globals.checkStoreWareHouse(V_MedProductType, false, true);
            if (StoreCbxStaff == null || StoreCbxStaff.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
            else
            {
                
            }

            Coroutine.BeginExecute(DoGetStore_MedDept());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            GetAllStaffContrain();

            SearchCriteria = new RequestSearchCriteria();
            ListRequestDrugDelete = new ObservableCollection<RequestDrugInwardClinicDeptDetail>();
            RequestDrug = new RequestDrugInwardClinicDept();
            RefeshRequest();

            RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductSimple>();
            RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            RefGenMedProductDetails.PageSize = Globals.PageSize;

        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        void RefGenMedProductDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenMedProductDetails_Auto(BrandName, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            
        }
        #region checking account
        private bool _mPhieuYeuCau_Tim = true;
        private bool _mPhieuYeuCau_Them = true;
        private bool _mPhieuYeuCau_Xoa = true;
        private bool _mPhieuYeuCau_XemIn = true;
        private bool _mPhieuYeuCau_In = true;

        public bool mPhieuYeuCau_Tim
        {
            get
            {
                return _mPhieuYeuCau_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_Tim == value)
                    return;
                _mPhieuYeuCau_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Tim);
            }
        }

        public bool mPhieuYeuCau_Them
        {
            get
            {
                return _mPhieuYeuCau_Them;
            }
            set
            {
                if (_mPhieuYeuCau_Them == value)
                    return;
                _mPhieuYeuCau_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Them);
            }
        }

        public bool mPhieuYeuCau_Xoa
        {
            get
            {
                return _mPhieuYeuCau_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_Xoa == value)
                    return;
                _mPhieuYeuCau_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Xoa);
            }
        }

        public bool mPhieuYeuCau_XemIn
        {
            get
            {
                return _mPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_XemIn == value)
                    return;
                _mPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_XemIn);
            }
        }

        public bool mPhieuYeuCau_In
        {
            get
            {
                return _mPhieuYeuCau_In;
            }
            set
            {
                if (_mPhieuYeuCau_In == value)
                    return;
                _mPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_In);
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


        private RefGenMedProductSimple _SelectRefGenMedProductDetail;
        public RefGenMedProductSimple SelectRefGenMedProductDetail
        {
            get
            {
                return _SelectRefGenMedProductDetail;
            }
            set
            {
                if (_SelectRefGenMedProductDetail != value)
                {
                    _SelectRefGenMedProductDetail = value;
                    NotifyOfPropertyChange(() => SelectRefGenMedProductDetail);
                }
            }
        }

        private PagedSortableCollectionView<RefGenMedProductSimple> _RefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductSimple> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                }
            }
        }

        private ObservableCollection<Staff> _ListStaff;
        public ObservableCollection<Staff> ListStaff
        {
            get { return _ListStaff; }
            set
            {
                if (_ListStaff != value)
                {
                    _ListStaff = value;
                    NotifyOfPropertyChange(() => ListStaff);
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

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbxStaff;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbxStaff
        {
            get
            {
                return _StoreCbxStaff;
            }
            set
            {
                if (_StoreCbxStaff != value)
                {
                    _StoreCbxStaff = value;
                    NotifyOfPropertyChange(() => StoreCbxStaff);
                }
            }
        }

        private ObservableCollection<RequestDrugInwardClinicDeptDetail> ListRequestDrugDelete;

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

        private RequestDrugInwardClinicDeptDetail _CurrentRequestDrugInwardClinicDeptDetail;
        public RequestDrugInwardClinicDeptDetail CurrentRequestDrugInwardClinicDeptDetail
        {
            get { return _CurrentRequestDrugInwardClinicDeptDetail; }
            set
            {
                if (_CurrentRequestDrugInwardClinicDeptDetail != value)
                {
                    _CurrentRequestDrugInwardClinicDeptDetail = value;
                    NotifyOfPropertyChange(() => CurrentRequestDrugInwardClinicDeptDetail);
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
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;}
        }

        private Visibility _VisibilityName = Visibility.Visible;
        public Visibility VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                if (_VisibilityName != value)
                {
                    _VisibilityName = value;
                    NotifyOfPropertyChange(() => VisibilityName);
                }
                if (VisibilityName == Visibility.Visible)
                {
                    _VisibilityCode = Visibility.Collapsed;
                }
                else
                {
                    _VisibilityCode = Visibility.Visible;
                }
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private Visibility _VisibilityCode = Visibility.Collapsed;
        public Visibility VisibilityCode
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
        #endregion

        #region 3. Function Member

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, true);
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
            RequestDrugInwardClinicDeptDetail p = item as RequestDrugInwardClinicDeptDetail;
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
                if (RequestDrug.RequestDetails != null)
                {
                    foreach (RequestDrugInwardClinicDeptDetail item in RequestDrug.RequestDetails)
                    {
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (!(item.Qty > 0))
                            {
                                st.AppendLine("Qty must > 0");
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

        private void FullOperatorRequestDrugInwardClinicDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginFullOperatorRequestDrugInwardClinicDept(RequestDrug, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            RequestDrugInwardClinicDept RequestOut;
                            contract.EndFullOperatorRequestDrugInwardClinicDept(out RequestOut, asyncResult);
                            RequestDrug = RequestOut;
                            ListRequestDrugDelete.Clear();
                            Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SaveRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfLuuThayDoiTrenPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SaveFullOp();
                }
            }
            else
            {
                SaveFullOp();
            }
        }

        private bool CheckValidData()
        {
            if (RequestDrug.OutFromStoreObject == null || RequestDrug.OutFromStoreObject.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0318_G1_ChonKhoCC);
                return false;
            }
            if (RequestDrug.RequestDetails == null || RequestDrug.RequestDetails.Count == 0)
            {
                MessageBox.Show(eHCMSResources.K0422_G1_NhapCTietPhYC);
                return false;
            }
            else
            {
                if (RequestDrug.RequestDetails.Count == 1)
                {
                    if (RequestDrug.RequestDetails[0].RefGenericDrugDetail == null)
                    {
                        MessageBox.Show(eHCMSResources.K0303_G1_ChonHgYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrug.RequestDetails.Count; i++)
                {
                    if (RequestDrug.RequestDetails[i].RefGenericDrugDetail != null && RequestDrug.RequestDetails[i].Qty <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0538_G1_Msg_InfoSLgYCLonHon0);
                        return false;
                    }

                }
            }

            return true;
        }

        private void SaveFullOp()
        {
            if (ListRequestDrugDelete.Count > 0)
            {
                if (RequestDrug.RequestDetails == null)
                {
                    RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardClinicDeptDetail>();
                }
                foreach (RequestDrugInwardClinicDeptDetail item in ListRequestDrugDelete)
                {
                    RequestDrug.RequestDetails.Add(item);
                }
            }
            if (CheckValidData())
            {
                FullOperatorRequestDrugInwardClinicDept();
            }
        }

        private void RemoveRowBlank()
        {
            try
            {
                int idx = RequestDrug.RequestDetails.Count;
                if (idx > 0)
                {
                    idx--;
                    RequestDrugInwardClinicDeptDetail obj = (RequestDrugInwardClinicDeptDetail)RequestDrug.RequestDetails[idx];
                    if (obj.GenMedProductID == null || obj.GenMedProductID == 0)
                    {
                        RequestDrug.RequestDetails.RemoveAt(idx);
                    }
                }
                NotifyOfPropertyChange(() => RequestDrug);
            }
            catch
            { }
        }

        private void RemoveMark(object item)
        {
            RequestDrugInwardClinicDeptDetail obj = item as RequestDrugInwardClinicDeptDetail;
            if (obj != null)
            {
                RequestDrug.RequestDetails.Remove(obj);
                if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                {
                    obj.EntityState = EntityState.DETACHED;
                    ListRequestDrugDelete.Add(obj);
                }
            }
        }

        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            //to do
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
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
                                    Action<IStoreDeptRequestSearch> onInitDlg = delegate (IStoreDeptRequestSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria;
                                        proAlloc.RequestDruglist.Clear();
                                        proAlloc.RequestDruglist.TotalItemCount = Total;
                                        proAlloc.RequestDruglist.PageIndex = 0;
                                        foreach (RequestDrugInwardClinicDept p in results)
                                        {
                                            proAlloc.RequestDruglist.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
                                }
                                else
                                {
                                    if (RequestDrug != null)
                                    {
                                        ChangeValue(RequestDrug.RefGenDrugCatID_1,results.FirstOrDefault().RefGenDrugCatID_1);
                                    }
                                    RequestDrug = results.FirstOrDefault();
                                    ListRequestDrugDelete.Clear();
                                    GetRequestDetailsByID(RequestDrug.ReqDrugInClinicDeptID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRequestDrugInwardClinicDeptDetailByID(long RequestID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRequestDrugInwardClinicDeptDetailByID(RequestID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRequestDrugInwardClinicDeptDetailByID(asyncResult);
                            if (results != null)
                            {
                                RequestDrug.RequestDetails = results.ToObservableCollection();
                            }
                            else
                            {
                                if (RequestDrug.RequestDetails != null)
                                {
                                    RequestDrug.RequestDetails.Clear();
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRequestDetailsByID(long ID)
        {
            GetRequestDrugInwardClinicDeptDetailByID(ID);
        }

        private void RefeshRequest()
        {
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardClinicDeptDetail>();
            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;
            ListRequestDrugDelete.Clear();
            if (StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            SetDefultRefGenericDrugCategory();
        }

        private void DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID)
        {
            //GetStoragesByStaffID(StaffID, null);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRequestDrugInwardClinicDept(ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndDeleteRequestDrugInwardClinicDept(asyncResult);
                            RefeshRequest();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void DeleteRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                DeleteRequestDrugInwardClinicDept(RequestDrug.ReqDrugInClinicDeptID);
            }
        }

        private bool CheckDeleted(object item)
        {
            RequestDrugInwardClinicDeptDetail temp = item as RequestDrugInwardClinicDeptDetail;
            if (temp != null && temp.EntityState == EntityState.DETACHED)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckValidGrid()
        {
            bool results = true;
            if (RequestDrug.InDeptStoreID == RequestDrug.OutFromStoreID)
            {
                Globals.ShowMessage(eHCMSResources.Z1131_G1_KhoYCKhoCCKgDcTrung, eHCMSResources.T0074_G1_I);
                return false;
            }
            if (RequestDrug.RequestDetails != null)
            {
                for (int i = 0; i < RequestDrug.RequestDetails.Count; i++)
                {
                    if (RequestDrug.RequestDetails[i].GenMedProductID > 0 && RequestDrug.RequestDetails[i].Qty <= 0)
                    {
                        results = false;
                        MessageBox.Show(string.Format(eHCMSResources.Z1283_G1_DLieuKgHopLe, (i + 1).ToString()));
                        break;
                    }
                }
            }
            return results;
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (RequestDrug != null && StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            yield break;
        }
        private IEnumerator<IResult> DoGetStore_All()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false,null, false, true);
            yield return paymentTypeTask;
            StoreCbxStaff = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetStorageByStaffID(long StaffID)
        {
            //GetStoragesByStaffID(StaffID, null);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyStoragesServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetStoragesByStaffID(StaffID, null, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetStoragesByStaffID(asyncResult);
                            if (results != null)
                            {
                                StoreCbxStaff = results.ToObservableCollection();
                                if (RequestDrug != null)
                                {
                                    RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
                                }
                            }
                            else
                            {
                                if (StoreCbxStaff != null)
                                {
                                    StoreCbxStaff.Clear();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllStaffContrain()
        {
            //GetAllStaffContain();
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaffContain(Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetAllStaffContain(asyncResult);
                            if (results != null)
                            {
                                ListStaff = results.ToObservableCollection();
                            }
                            else
                            {
                                if (ListStaff != null)
                                {
                                    ListStaff.Clear();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();

        }
        #endregion

        #region client member

        #region AutoGenMedProduct Member
        private string BrandName;

        private void GetRefGenMedProductDetails_Auto(string BrandName, int PageIndex, int PageSize)
        {
             long? RefGenDrugCatID_1=null;
             if (RequestDrug != null)
             {
                 RefGenDrugCatID_1 = RequestDrug.RefGenDrugCatID_1;
             }
             //IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SimpleAutoPaging(IsCode, BrandName,V_MedProductType,RefGenDrugCatID_1, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SimpleAutoPaging(out Total, asyncResult);
                            RefGenMedProductDetails.Clear();
                            RefGenMedProductDetails.TotalItemCount = Total;
                            foreach (RefGenMedProductSimple p in results)
                            {
                                RefGenMedProductDetails.Add(p);
                            }
                            AutoGenMedProduct.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        AutoCompleteBox AutoGenMedProduct;
        public void acbDrug_Populating1(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                AutoGenMedProduct = sender as AutoCompleteBox;
                RefGenMedProductDetails.PageIndex = 0;
                BrandName = e.Parameter;
                GetRefGenMedProductDetails_Auto(e.Parameter, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
            }
        }
        public void acbDrug_Populating2(object sender, PopulatingEventArgs e)
        {
            if (IsCode.GetValueOrDefault())
            {
                AutoGenMedProduct = sender as AutoCompleteBox;
                RefGenMedProductDetails.PageIndex = 0;
                BrandName = e.Parameter;
                GetRefGenMedProductDetails_Auto(e.Parameter, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
            }
        }
     
        #endregion
        DataGrid grdRequestDetails = null;
        public void grdRequestDetails_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            grdRequestDetails = sender as DataGrid;
        }

        public void grdRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (CheckDeleted(e.Row.DataContext))
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
        }
        public void grdRequestDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (grdRequestDetails != null)
            {
                ischanged(grdRequestDetails.SelectedItem);
            }
        }

        public void grdRequestDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!CheckValidationEditor())
            {
                e.Cancel = true;
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (grdRequestDetails != null)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa dòng dữ liệu này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(grdRequestDetails.SelectedItem);
                }
            }
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (CheckValidGrid())
            {
                SaveRequest();
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
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
            }
        }

        public void btnDelete(object sender, RoutedEventArgs e)
        {
            //call delete 
            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRequest();
            }
        }

        #region printing member

        public void btnPreview()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                    }
                    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC HƯỚNG TÂM THẦN";
                    }
                    else
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC";
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                }
                else
                {
                    proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
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
                            IsLoading = false;
                            Globals.IsBusy = false;
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
                RequestDrugInwardClinicDept item = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (RequestDrug != null &&item !=null)
                {
                    ChangeValue(RequestDrug.RefGenDrugCatID_1, item.RefGenDrugCatID_1);
                }
                RequestDrug = item;
                ListRequestDrugDelete.Clear();
                GetRequestDetailsByID(RequestDrug.ReqDrugInClinicDeptID);
            }
        }

        #endregion

        bool? IsCode = false;
        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = Visibility.Collapsed;
        }
        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = Visibility.Visible;
        }

        public void btnAddItem()
        {
            if (SelectRefGenMedProductDetail == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (SelectRefGenMedProductDetail.RequestQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1774_G1_SLgYCKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (SelectRefGenMedProductDetail != null && SelectRefGenMedProductDetail.RequestQty >0)
            {
                //kiem tra co ton tai chua?neu roi thi chi can cong so luong thoi
                if (RequestDrug == null)
                {
                    RequestDrug = new RequestDrugInwardClinicDept();
                }
                if (RequestDrug.RequestDetails == null)
                {
                    RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardClinicDeptDetail>();
                }
                var temp = RequestDrug.RequestDetails.Where(x => x.RefGenericDrugDetail !=null && x.RefGenericDrugDetail.GenMedProductID == SelectRefGenMedProductDetail.GenMedProductID);
                if (temp != null && temp.Count() > 0)
                {
                    temp.ToList()[0].Qty += Convert.ToInt32(SelectRefGenMedProductDetail.RequestQty);
                }
                else
                {
                    RequestDrugInwardClinicDeptDetail item = new RequestDrugInwardClinicDeptDetail();
                    item.RefGenericDrugDetail = new DataEntities.RefGenMedProductDetails();
                    item.RefGenericDrugDetail.GenMedProductID = SelectRefGenMedProductDetail.GenMedProductID;
                    item.RefGenericDrugDetail.Code = SelectRefGenMedProductDetail.Code;
                    item.RefGenericDrugDetail.BrandName = SelectRefGenMedProductDetail.BrandName;
                    item.RefGenericDrugDetail.GenericName = SelectRefGenMedProductDetail.GenericName;
                    item.RefGenericDrugDetail.SelectedUnit = new RefUnit();
                    item.RefGenericDrugDetail.SelectedUnit.UnitName = SelectRefGenMedProductDetail.UnitName;
                    item.Qty = Convert.ToInt32(SelectRefGenMedProductDetail.RequestQty);
                    RequestDrug.RequestDetails.Add(item);
                }
                if (AutoGenMedProduct != null)
                {
                    AutoGenMedProduct.Text = "";
                    AutoGenMedProduct.Focus();
                }
               
            }
            
        }

        private void ChangeValue(long value1, long value2)
        {
            if (value1 != value2)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
        }

        private bool flag = true;
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                if (RequestDrug != null && RequestDrug.RequestDetails != null)
                {
                    for (int i = 0; i < RequestDrug.RequestDetails.Count; i++)
                    {
                        if (RequestDrug.RequestDetails[i].EntityState == EntityState.PERSITED || RequestDrug.RequestDetails[i].EntityState == EntityState.MODIFIED)
                        {
                            RequestDrug.RequestDetails[i].EntityState = EntityState.DETACHED;
                            ListRequestDrugDelete.Add(RequestDrug.RequestDetails[i]);
                        }
                    }
                    RequestDrug.RequestDetails.Clear();
                }
            }
            flag = true;
        }
    }
}
