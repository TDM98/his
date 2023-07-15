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
using aEMR.Pharmacy.Views;
using System.Text;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using Castle.Windsor;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRequestPharmacy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestViewModel : Conductor<object>, IRequestPharmacy, IHandle<PharmacyCloseSearchRequestEvent>
    {
        #region Indicator Member

        private bool _isLoadingStaff = false;
        public bool isLoadingStaff
        {
            get { return _isLoadingStaff; }
            set
            {
                if (_isLoadingStaff != value)
                {
                    _isLoadingStaff = value;
                    NotifyOfPropertyChange(() => isLoadingStaff);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

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

        private bool _isLoadingStoreByStaffID = false;
        public bool isLoadingStoreByStaffID
        {
            get { return _isLoadingStoreByStaffID; }
            set
            {
                if (_isLoadingStoreByStaffID != value)
                {
                    _isLoadingStoreByStaffID = value;
                    NotifyOfPropertyChange(() => isLoadingStoreByStaffID);
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

        private bool _isLoadingStore1 = false;
        public bool isLoadingStore1
        {
            get { return _isLoadingStore1; }
            set
            {
                if (_isLoadingStore1 != value)
                {
                    _isLoadingStore1 = value;
                    NotifyOfPropertyChange(() => isLoadingStore1);
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
            get { return (isLoadingStaff || isLoadingGetStore || isLoadingFullOperator || isLoadingStore1 || isLoadingStoreByStaffID || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        [ImportingConstructor]
        public RequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_All());
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetAllStaffContrain();

            SearchCriteria = new RequestSearchCriteria();
            ListRequestDrugDelete = new ObservableCollection<RequestDrugInwardDetail>();
            RequestDrug = new RequestDrugInward();
            RefeshRequest();

            RefGenericDrugDetail = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetail.OnRefresh += RefGenericDrugDetail_OnRefresh;
            RefGenericDrugDetail.PageSize = Globals.PageSize;
            RequestDrug.RequestDetails.Count();
        }

        void RefGenericDrugDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenericDrugDetail_Auto(BrandName, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize);
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mPhieuYeuCau,
                                               (int)oPharmacyEx.mPhieuYeuCau_Tim, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mPhieuYeuCau,
                                               (int)oPharmacyEx.mPhieuYeuCau_ChinhSua, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mPhieuYeuCau,
                                               (int)oPharmacyEx.mPhieuYeuCau_In, (int)ePermission.mView);
            

        }
        #region checking account

        private bool _bTim = true;
        private bool _bChinhSua = true;
        private bool _bIn = true;


        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }
        public bool bIn
        {
            get
            {
                return _bIn;
            }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
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

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetail;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    NotifyOfPropertyChange(() => RefGenericDrugDetail);
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

        private ObservableCollection<RequestDrugInwardDetail> ListRequestDrugDelete;


        private RequestDrugInward _RequestDrug;
        public RequestDrugInward RequestDrug
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

        private RequestDrugInwardDetail _CurrentRequestDrugInwardDetail;
        public RequestDrugInwardDetail CurrentRequestDrugInwardDetail
        {
            get { return _CurrentRequestDrugInwardDetail; }
            set
            {
                if (_CurrentRequestDrugInwardDetail != value)
                {
                    _CurrentRequestDrugInwardDetail = value;
                    NotifyOfPropertyChange(() => CurrentRequestDrugInwardDetail);
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

        private long _V_MedProductType = 11001; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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
                }

            }
        }
        #endregion

        #region 3. Function Member

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        //TTM 06072018
        //    Comment AddNewBlankFirstRow vì bảng thân AxDataGridNy đã tạo 1 dòng trống mặc định
        private void AddNewBlankFirstRow()
        {
            if (RequestDrug != null)
            {
                if (RequestDrug.RequestDetails == null)
                {
                    RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardDetail>();
                }

                RequestDrugInwardDetail NewItem = new RequestDrugInwardDetail();
                NewItem.DrugID = 0;
                if (RequestDrug.RequestDetails.Count == 0)
                {
                    RequestDrug.RequestDetails.Add(NewItem);
                }

            }
        }

        private void AddNewBlankRow()
        {
            RequestDrugInwardDetail NewItem = new RequestDrugInwardDetail();
            NewItem.DrugID = 0;
            if (RequestDrug.RequestDetails != null)
            {
                RequestDrug.RequestDetails.Add(NewItem);
            }
        }

        private bool ischanged(object item)
        {
            RequestDrugInwardDetail p = item as RequestDrugInwardDetail;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
                if (p.DrugID != null && p.DrugID != 0)
                {
                    if (RequestDrug.RequestDetails != null)
                    {
                        var vars = RequestDrug.RequestDetails.Where(x => x.DrugID == p.DrugID && x.EntityState != EntityState.DETACHED);
                        if (vars.Count() > 1)
                        {
                            MessageBox.Show(eHCMSResources.K0046_G1_ThuocDaCoTrongPhYC);
                            p.RefGenericDrugDetail = null;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
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

        private bool CheckValidationEditor()
        {
            bool result = true;
            StringBuilder st = new StringBuilder();
            if (RequestDrug != null)
            {
                if (RequestDrug.RequestDetails != null)
                {
                    foreach (RequestDrugInwardDetail item in RequestDrug.RequestDetails)
                    {
                        if (item.DrugID != null && item.DrugID != 0)
                        {
                            if (!(item.Qty > 0))
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

        private void FullOperatorRequestDrugInward()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginFullOperatorRequestDrugInward(RequestDrug, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            RequestDrugInward RequestOut;
                            contract.EndFullOperatorRequestDrugInward(out RequestOut, asyncResult);
                            RequestDrug = RequestOut;
                            AddNewBlankRow();
                            ListRequestDrugDelete.Clear();
                            Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void SaveRequest()
        {
            if (RequestDrug.ReqDrugInID > 0)
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

        private void SaveFullOp()
        {
            
            if (RequestDrug.RequestDetails != null)
            {
                if (RequestDrug.RequestDetails.Count > 0)
                {
                    if (RequestDrug.RequestDetails[0].RefGenericDrugDetail == null)
                    {
                        MessageBox.Show(eHCMSResources.K0409_G1_ChonThuocCanYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                }

                for (int i = 0; i < RequestDrug.RequestDetails.Count; i++)
                {
                    if (RequestDrug.RequestDetails[i].RefGenericDrugDetail != null && RequestDrug.RequestDetails[i].Qty <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0538_G1_Msg_InfoSLgYCLonHon0);
                        return;
                    }

                }

                if (ListRequestDrugDelete.Count > 0)
                {
                    if (RequestDrug.RequestDetails == null)
                    {
                        RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardDetail>();
                    }
                    foreach (RequestDrugInwardDetail item in ListRequestDrugDelete)
                    {
                        RequestDrug.RequestDetails.Add(item);
                    }
                }

                FullOperatorRequestDrugInward();
            }

            else
            {
                AddNewBlankFirstRow();
                MessageBox.Show(eHCMSResources.K0422_G1_NhapCTietPhYC);
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
                    RequestDrugInwardDetail obj = (RequestDrugInwardDetail)RequestDrug.RequestDetails[idx];
                    if (obj.DrugID == null || obj.DrugID == 0)
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
            RequestDrugInwardDetail obj = item as RequestDrugInwardDetail;
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

        private void SearchRequestDrugInward(int PageIndex, int PageSize)
        {
           
            //to do
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRequestDrugInward(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSearchRequestDrugInward(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IRequestSearchPharmacy> onInitDlg = delegate (IRequestSearchPharmacy proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria;
                                        proAlloc.RequestDruglist.Clear();
                                        proAlloc.RequestDruglist.TotalItemCount = Total;
                                        proAlloc.RequestDruglist.PageIndex = 0;
                                        foreach (RequestDrugInward p in results)
                                        {
                                            proAlloc.RequestDruglist.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IRequestSearchPharmacy>(onInitDlg);
                                }
                                else
                                {
                                    RequestDrug = results.FirstOrDefault();
                                    ListRequestDrugDelete.Clear();
                                    GetRequestDetailsByID(RequestDrug.ReqDrugInID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRequestDrugInwardDetailByID(long RequestID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRequestDrugInwardDetailByID(RequestID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRequestDrugInwardDetailByID(asyncResult);
                            if (results != null)
                            {
                                RequestDrug.RequestDetails = results.ToObservableCollection();
                                if (RequestDrug.CanSave)
                                {
                                    AddNewBlankRow();
                                }
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
                                RequestDrug = new RequestDrugInward();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        private void GetRequestDetailsByID(long ID)
        {
            GetRequestDrugInwardDetailByID(ID);
        }

        public void RefeshRequest()
        {
            RequestDrug = null;
            RequestDrug = new RequestDrugInward();
            RequestDrug.ReqDate = Globals.ServerDate.Value;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.RequestDetails = new ObservableCollection<RequestDrugInwardDetail>();
            ListRequestDrugDelete.Clear();
            AddNewBlankFirstRow();
            if (StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
        }

        private void DeleteRequestDrugInward(long ReqDrugInID)
        {
            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRequestDrugInward(ReqDrugInID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndDeleteRequestDrugInward(asyncResult);
                            RefeshRequest();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void DeleteRequest()
        {
            if (RequestDrug.ReqDrugInID > 0)
            {
                DeleteRequestDrugInward(RequestDrug.ReqDrugInID);
            }
        }

        private bool CheckDeleted(object item)
        {
            RequestDrugInwardDetail temp = item as RequestDrugInwardDetail;
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
                    if (RequestDrug.RequestDetails[i].DrugID > 0 && RequestDrug.RequestDetails[i].Qty <= 0)
                    {
                        results = false;
                        MessageBox.Show(string.Format(eHCMSResources.Z1283_G1_DLieuKgHopLe, (i + 1).ToString()));
                        break;
                    }
                }
            }
            return results;
        }

        private IEnumerator<IResult> DoGetStore_All()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask(null, false,null, false, true);
            yield return paymentTypeTask;
            StoreCbxStaff = paymentTypeTask.LookupList;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingStore1 = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (RequestDrug != null && StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            isLoadingStore1 = false;
            yield break;
        }

        private void GetStorageByStaffID(long StaffID)
        {
            isLoadingStoreByStaffID = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                            isLoadingStoreByStaffID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllStaffContrain()
        {
            isLoadingStaff = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                            isLoadingStaff = false;
                            //Globals.IsBusy = false;
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

        private void GetRefGenericDrugDetail_Auto(string BrandName, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_AutoPaging(null,BrandName, 0, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndSearchRefDrugGenericDetails_AutoPaging(out Total, asyncResult);
                            RefGenericDrugDetail.Clear();
                            RefGenericDrugDetail.TotalItemCount = Total;
                            foreach (RefGenericDrugDetail p in results)
                            {
                                RefGenericDrugDetail.Add(p);
                            }
                            AutoGenMedProduct.ItemsSource = RefGenericDrugDetail;
                            AutoGenMedProduct.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }

                    }), null);

                }

            });

            t.Start();
        }

        AutoCompleteBox AutoGenMedProduct;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            AutoGenMedProduct = sender as AutoCompleteBox;
            RefGenericDrugDetail.PageIndex = 0;
            BrandName = e.Parameter;
            GetRefGenericDrugDetail_Auto(e.Parameter, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize);
        }

        public void acbDrug_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutoGenMedProduct != null)
            {
                if (CurrentRequestDrugInwardDetail != null)
                {
                    CurrentRequestDrugInwardDetail.RefGenericDrugDetail = AutoGenMedProduct.SelectedItem as RefGenericDrugDetail;
                }
            }
        }

        #endregion

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
            DataGrid grdRequestDetails = sender as DataGrid;
            if (ischanged(grdRequestDetails.SelectedItem))
            {
                if (e.Row.GetIndex() == (RequestDrug.RequestDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    AddNewBlankRow();
                }
            }
            else
            {
                CurrentRequestDrugInwardDetail = null;
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
            //Kiên: Kiểm tra nếu row (row cuối cùng) rỗng thì không cho xóa.
            if (CurrentRequestDrugInwardDetail == null || CurrentRequestDrugInwardDetail.RefGenericDrugDetail == null)
            {
                MessageBox.Show(eHCMSResources.A0678_G1_Msg_InfoKhDcXoaDongRong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (RequestDrug != null && RequestDrug.CanSave)
            {
                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(((RequestView)this.GetView()).grdRequestDetails.SelectedItem);
                    AddNewBlankFirstRow();
                }
            }
            else
            {
                MessageBox.Show("Phiếu này đã nhận hàng.Không được thêm/xóa/sửa");
            }
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (((RequestView)this.GetView()).grdRequestDetails.IsValid)
            {
                if (CheckValidGrid())
                {
                    SaveRequest();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0539_G1_Msg_InfoDataKhDung);
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
            SearchRequestDrugInward(0, Globals.PageSize);
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0127_G1_Msg_ConfTaoPhYCMoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
            }
        }

        public void btnDelete(object sender, RoutedEventArgs e)
        {
            //call delete 
            if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRequest();
            }
        }

        #region printing member

        public void btnPreview()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInID;
                proAlloc.eItem = ReportName.PHARMACY_REQUESTDRUGPHARMACY;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRequestPharmacyInPdfFormat(RequestDrug.ReqDrugInID, Globals.DispatchCallback((asyncResult) =>
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
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #endregion
        #endregion

        #region IHandle<PharmacyCloseSearchRequestEvent> Members

        public void Handle(PharmacyCloseSearchRequestEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrug = message.SelectedRequest as RequestDrugInward;
                ListRequestDrugDelete.Clear();
                GetRequestDetailsByID(RequestDrug.ReqDrugInID);
            }
        }

        #endregion
    }
}
