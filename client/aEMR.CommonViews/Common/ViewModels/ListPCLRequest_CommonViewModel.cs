using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.Linq;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IListPCLRequest_Common)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ListPCLRequest_CommonViewModel : ViewModelBase, IListPCLRequest_Common
        //, IHandle<ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM>
        , IHandle<ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IListPCLRequest_CommonView;
        }
        IListPCLRequest_CommonView _currentView;


        #region Thuộc Tính Ẩn Hiện Cột dtg
        private Visibility _dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
        public Visibility dtgCellTemplateThucHien_Visible
        {
            get { return _dtgCellTemplateThucHien_Visible; }
            set
            {
                _dtgCellTemplateThucHien_Visible = value;
                NotifyOfPropertyChange(() => dtgCellTemplateThucHien_Visible);
            }
        }

        private Visibility _dtgCellTemplateNgung_Visible = Visibility.Collapsed;
        public Visibility dtgCellTemplateNgung_Visible
        {
            get { return _dtgCellTemplateNgung_Visible; }
            set
            {
                _dtgCellTemplateNgung_Visible = value;
                NotifyOfPropertyChange(() => dtgCellTemplateNgung_Visible);
            }
        }

        private Visibility _dtgCellTemplateKetQua_Visible = Visibility.Collapsed;
        public Visibility dtgCellTemplateKetQua_Visible
        {
            get { return _dtgCellTemplateKetQua_Visible; }
            set
            {
                _dtgCellTemplateKetQua_Visible = value;
                NotifyOfPropertyChange(() => dtgCellTemplateKetQua_Visible);
            }
        }

        private Visibility _dtgCellTemplateInputKetQua_Visible = Visibility.Collapsed;
        public Visibility dtgCellTemplateInputKetQua_Visible
        {
            get { return _dtgCellTemplateInputKetQua_Visible; }
            set
            {
                _dtgCellTemplateInputKetQua_Visible = value;
                NotifyOfPropertyChange(() => dtgCellTemplateInputKetQua_Visible);
            }
        }
        #endregion

        [ImportingConstructor]
        public ListPCLRequest_CommonViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();
            SearchCriteriaDetail = new PatientPCLRequestDetailSearchCriteria();
            ObjPatientPCLRequest_ByPatientID_Paging = new PagedSortableCollectionView<PatientPCLRequest>();
            ObjPatientPCLRequest_ByPatientID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_ByPatientID_Paging_OnRefresh);

            HasGroup = true;
        }

        public void Init()
        {
            if (_currentView != null)
            {
                _currentView.ShowHide_dtgDetailColumns();
            }
            if (Registration_DataStorage.CurrentPatient != null)
            {
                ObjPatientPCLRequest_ByPatientID_Paging.PageIndex = 0;
                PatientPCLRequest_ByPatientID_Paging(ObjPatientPCLRequest_ByPatientID_Paging.PageIndex, ObjPatientPCLRequest_ByPatientID_Paging.PageSize, true);
            }
        }


        void ObjPatientPCLRequest_ByPatientID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PatientPCLRequest_ByPatientID_Paging(ObjPatientPCLRequest_ByPatientID_Paging.PageIndex, ObjPatientPCLRequest_ByPatientID_Paging.PageSize, true);
        }

        private PatientPCLRequest _ObjPatientPCLRequestSelected;
        public PatientPCLRequest ObjPatientPCLRequestSelected
        {
            get { return _ObjPatientPCLRequestSelected; }
            set
            {
                _ObjPatientPCLRequestSelected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequestSelected);
            }
        }

        private PatientPCLRequestSearchCriteria _SearchCriteria;
        public PatientPCLRequestSearchCriteria SearchCriteria
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
                    NotifyOfPropertyChange(() => SearchCriteria);
                }

            }
        }
        private PagedSortableCollectionView<PatientPCLRequest> _ObjPatientPCLRequest_ByPatientID_Paging;
        public PagedSortableCollectionView<PatientPCLRequest> ObjPatientPCLRequest_ByPatientID_Paging
        {
            get { return _ObjPatientPCLRequest_ByPatientID_Paging; }
            set
            {
                _ObjPatientPCLRequest_ByPatientID_Paging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_ByPatientID_Paging);
            }
        }
        private void PatientPCLRequest_ByPatientID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPatientPCLRequest_ByPatientID_Paging.Clear();
            ObjPatientPCLRequestDetail_ByPatientPCLReqID = null;
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3035_G1_DSPh) });
            if (IsShowSummaryContent)
                this.ShowBusyIndicator();
            else
                this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLRequest_ByPatientID_Paging(SearchCriteria, V_RegistrationType, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            ObjPatientPCLRequest_ByPatientID_Paging.Clear();
                            var allItems = client.EndPatientPCLRequest_ByPatientID_Paging(out Total, asyncResult);
                            if (allItems != null)
                            {
                                if (CountTotal)
                                {
                                    ObjPatientPCLRequest_ByPatientID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPatientPCLRequest_ByPatientID_Paging.Add(item);
                                    }
                                }
                            }
                            if (ObjPatientPCLRequest_ByPatientID_Paging.Count > 0)
                            {
                                ObjPatientPCLRequestSelected = ObjPatientPCLRequest_ByPatientID_Paging[0];
                                SetSearchCriteriaDetail();
                                PatientPCLRequestDetail_ByPatientPCLReqID();
                                if(ObjPatientPCLRequest_ByPatientID_Paging.Where(x=>x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).Count()> 0
                                    && ObjPatientPCLRequest_ByPatientID_Paging.Where(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID &&
                                                                                            x.V_PCLRequestStatus != AllLookupValues.V_PCLRequestStatus.CLOSE).Count() == 0)
                                {
                                    if(ObjPatientPCLRequestSelected.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                                    {
                                        Globals.EventAggregator.Publish(new AllPCLRequestImageClose {AllPCLClose = true });
                                    }
                                    else if (ObjPatientPCLRequestSelected.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                    {
                                        Globals.EventAggregator.Publish(new AllPCLRequestClose { AllPCLClose = true });
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    if (IsShowSummaryContent)
                        this.HideBusyIndicator();
                    else
                        this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        #region Detail
        private PatientPCLRequestDetailSearchCriteria _SearchCriteriaDetail;
        public PatientPCLRequestDetailSearchCriteria SearchCriteriaDetail
        {
            get
            {
                return _SearchCriteriaDetail;
            }
            set
            {
                if (_SearchCriteriaDetail != value)
                {
                    _SearchCriteriaDetail = value;
                    NotifyOfPropertyChange(() => SearchCriteriaDetail);
                }

            }
        }

        private bool _HasGroup;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set
            {
                if (_HasGroup != value)
                {
                    _HasGroup = value;
                    NotifyOfPropertyChange(() => HasGroup);
                }
            }
        }

        private aEMR.Common.PagedCollectionView.PagedCollectionView _ObjPatientPCLRequestDetail_ByPatientPCLReqID;
        public aEMR.Common.PagedCollectionView.PagedCollectionView ObjPatientPCLRequestDetail_ByPatientPCLReqID
        {
            get
            {
                return _ObjPatientPCLRequestDetail_ByPatientPCLReqID;
            }
            set
            {
                _ObjPatientPCLRequestDetail_ByPatientPCLReqID = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequestDetail_ByPatientPCLReqID);
            }
        }
        public void PatientPCLRequestDetail_ByPatientPCLReqID()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0531_G1_DSLoaiXN) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteriaDetail, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPatientPCLRequestDetail_ByPatientPCLReqID(asyncResult);

                            if (items != null)
                            {
                                ObjPatientPCLRequestDetail_ByPatientPCLReqID = new aEMR.Common.PagedCollectionView.PagedCollectionView(items);

                                if (HasGroup)
                                {
                                    ObjPatientPCLRequestDetail_ByPatientPCLReqID.GroupDescriptions.Add(new PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPatientPCLRequestDetail_ByPatientPCLReqID = null;
                            }
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

        public void dtgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dtg = sender as DataGrid;
            if (dtg.SelectedItem != null)
            {
                SetSearchCriteriaDetail();
                //▼======20190108 TTM: Mở lại chức năng xem trước của danh sách CLS/ Xét nghiệm.
                PatientPCLRequestDetail_ByPatientPCLReqID();
            }
        }

        private void SetSearchCriteriaDetail()
        {
            SearchCriteriaDetail.PatientPCLReqID = ObjPatientPCLRequestSelected.PatientPCLReqID;
            //SearchCriteriaDetail.LoaiDanhSach = SearchCriteria.LoaiDanhSach;
            //switch (SearchCriteria.LoaiDanhSach)
            //{
            //    case AllLookupValues.PatientPCLRequestListType.DANHSACHPHIEUYEUCAU:
            //        {
            //            SearchCriteriaDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            //            break;
            //        }
            //    //case PatientPCLRequest.TypeList.DANHSACHPHIEU_DANGTHUCTHIEN:
            //    //    {
            //    //        SearchCriteriaDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN;
            //    //        break;
            //    //    }
            //    //case PatientPCLRequest.TypeList.DANHSACHPHIEU_THUCHIENXONG:
            //    //    {
            //    //        SearchCriteriaDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.HOAN_TAT;
            //    //        break;
            //    //    }
            //}
        }

        //private void SetShowHideColumnDataGrid()
        //{
        //    switch (SearchCriteria.LoaiDanhSach)
        //    {
        //        //Khoa Khám Bệnh xem phiếu   
        //        case PatientPCLRequest.TypeList.DANHSACHPHIEU_DOTDANGKYNAY:
        //            {
        //                dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
        //                dtgCellTemplateNgung_Visible = Visibility.Collapsed;
        //                dtgCellTemplateKetQua_Visible = Visibility.Visible;
        //                break;
        //            }

        //        case PatientPCLRequest.TypeList.DANHSACHPHIEU_DOTDANGKYTRUOC:
        //            {
        //                dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
        //                dtgCellTemplateNgung_Visible = Visibility.Collapsed;
        //                dtgCellTemplateKetQua_Visible = Visibility.Visible;
        //                break;
        //            }
        //           //Khoa Khám Bệnh xem phiếu   

        //        case PatientPCLRequest.TypeList.DANHSACHPHIEU_CHOTHUCTHIEN:
        //            {
        //                dtgCellTemplateThucHien_Visible = Visibility.Visible;
        //                dtgCellTemplateNgung_Visible = Visibility.Visible;
        //                dtgCellTemplateKetQua_Visible = Visibility.Collapsed;
        //                break;
        //            }

        //        case PatientPCLRequest.TypeList.DANHSACHPHIEU_DANGTHUCTHIEN:
        //            {
        //                dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
        //                dtgCellTemplateNgung_Visible = Visibility.Visible;
        //                dtgCellTemplateKetQua_Visible = Visibility.Collapsed;
        //                break;
        //            }

        //        case PatientPCLRequest.TypeList.DANHSACHPHIEU_THUCHIENXONG:
        //            {
        //                dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
        //                dtgCellTemplateNgung_Visible = Visibility.Collapsed;
        //                dtgCellTemplateKetQua_Visible = Visibility.Visible;
        //                break;
        //            }
        //    }

        //    if (_currentView != null)
        //    {
        //        _currentView.ShowHide_dtgDetailColumns();
        //    }

        //}

        //4 bt Thực Hiện
        private PatientPCLRequestDetail _ObjPatientPCLRequestDetailSelected;
        public PatientPCLRequestDetail ObjPatientPCLRequestDetailSelected
        {
            get { return _ObjPatientPCLRequestDetailSelected; }
            set
            {
                _ObjPatientPCLRequestDetailSelected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequestDetailSelected);
            }
        }

        public void hplThucHien_Click()
        {
            if (ObjPatientPCLRequestDetailSelected != null)
            {
                if (ObjPatientPCLRequestDetailSelected.PaidTime != null)
                {
                    if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.K0139_G1_TienHanhXN) + ObjPatientPCLRequestDetailSelected.PCLExamType.PCLExamTypeName.Trim() + Environment.NewLine + eHCMSResources.Z0389_G1_DongYKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        //////PatientPCLRequest_UpdateV_PCLRequestStatus(ObjPatientPCLRequestDetailSelected.PatientPCLReqID, (long)AllLookupValues.V_PCLRequestStatus.DANG_THUC_HIEN, ObjPatientPCLRequestDetailSelected.PCLReqItemID,(long)AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("{0}: ", eHCMSResources.T2865_G1_LoaiXN) + ObjPatientPCLRequestDetailSelected.PCLExamType.PCLExamTypeName.Trim() + eHCMSResources.A0778_G1_Msg_InfoKhTHienCLSChuaTraTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }

            }

        }

        private void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1029_G1_DangThHien) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientPCLRequest_UpdateV_PCLRequestStatus(PatientPCLReqID, V_PCLRequestStatus, PCLReqItemID, V_ExamRegStatus, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";

                            contract.EndPatientPCLRequest_UpdateV_PCLRequestStatus(out Result, asyncResult);

                            switch (Result)
                            {
                                case "1":
                                    {
                                        //Phát sự kiện load lại danh sách CLS đang thực hiện
                                        Globals.EventAggregator.Publish(new ReLoadListPCLRequestProcessing() { });

                                        //Phát sự kiện load lại danh sách CLS đang Chờ thực hiện
                                        Globals.EventAggregator.Publish(new ReLoadListPCLRequestWaiting() { });

                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1029_G1_DangThHien), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0390_G1_LoiKgTHienDuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
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


        //public void hplInputKetQua_Click()
        //{
        //    if(ObjPatientPCLRequestDetailSelected!=null)
        //    {
        //        if(ObjPatientPCLRequestDetailSelected.PaidTime!=null)
        //        {
        //            //Phát sự kiện cho ResultInfo
        //            Globals.EventAggregator.Publish(new hplInputKetQua_Click<PatientPCLRequest,PatientPCLRequestDetail>() { PCLReq = ObjPatientPCLRequestSelected, PCLReqDetail = ObjPatientPCLRequestDetailSelected });
        //        }
        //    }
        //}

        //4 bt Thực Hiện
        #endregion

        #region Nút trong GridDetail
        public void hplInputKetQua_Click()
        {
            if (ObjPatientPCLRequestDetailSelected != null)
            {
                if (ObjPatientPCLRequestDetailSelected.HasResult == false)
                {
                    MessageBox.Show(eHCMSResources.A0408_G1_Msg_InfoChuaCoKQ, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }

        #endregion

        public void DoubleClick(object args)
        {
            if (!mPCL_DSPhieuYeuCau_ChinhSua)
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0103_G1_Msg_InfoChuaDcCapQuyenSuaPh));
                return;
            }
            EventArgs<object> eventArgs = args as EventArgs<object>;

            PatientPCLRequest Objtmp = eventArgs.Value as PatientPCLRequest;


            switch (Objtmp.V_PCLRequestStatus)
            {
                case AllLookupValues.V_PCLRequestStatus.CANCEL:
                    {
                        MessageBox.Show(string.Format("{0}! ", eHCMSResources.Z1030_G1_PhNayBNTraLaiKgLamXN));
                        break;
                    }
                //20190117 TTM: Bỏ điều kiện ngăn không cho phiếu đã hoàn thành được chuyển sang màn hình tạo/ cập nhật
                //              CLS/ xét nghiệm
                //case AllLookupValues.V_PCLRequestStatus.CLOSE:
                //    {
                //        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0920_G1_Msg_InfoKhDcSuaPhDaXong));
                //        break;
                //    }
                case AllLookupValues.V_PCLRequestStatus.PROCESSING:
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0920_G1_Msg_InfoKhDcSuaPhDaXong));
                        break;
                    }
            }

            //Cảnh Báo rồi phát ra sự kiện để load lên cho xem, nếu V_PCLRequestStatus==Open thì mới được phép Sửa và Lưu
            //▼===== 20190822 TTM:  Tách sự kiện ra làm 2, vì 1 sự kiện mà sử dụng chung cho 2 tab xét nghiệm và hình ảnh là không nên.
            //                      Trước đây hoạt động tốt là vì ở 2 màn hình khác nhau, còn hiện tại gộp thành 2 tab để chung sẽ dẫn đến lỗi chọn phiếu HA mà bên XN cũng thấy và ngược lại
            if (Objtmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEventWithKeyForImage<PatientPCLRequest, String> { ObjA = Objtmp, ObjB = eHCMSResources.Z0055_G1_Edit });
            }
            else if (Objtmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEventWithKey<PatientPCLRequest, String> { ObjA = Objtmp, ObjB = eHCMSResources.Z0055_G1_Edit });
            }


        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mPCL_DSPhieuYeuCau_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_DSPhieuYeuCau_ThongTin, (int)ePermission.mView);
            mPCL_DSPhieuYeuCau_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_DSPhieuYeuCau_ChinhSua, (int)ePermission.mView);
        }
        #region checking account
        private bool _mPCL_DSPhieuYeuCau_ThongTin = true;
        private bool _mPCL_DSPhieuYeuCau_ChinhSua = true;

        public bool mPCL_DSPhieuYeuCau_ThongTin
        {
            get
            {
                return _mPCL_DSPhieuYeuCau_ThongTin;
            }
            set
            {
                if (_mPCL_DSPhieuYeuCau_ThongTin == value)
                    return;
                _mPCL_DSPhieuYeuCau_ThongTin = value;
                NotifyOfPropertyChange(() => mPCL_DSPhieuYeuCau_ThongTin);
            }
        }


        public bool mPCL_DSPhieuYeuCau_ChinhSua
        {
            get
            {
                return _mPCL_DSPhieuYeuCau_ChinhSua;
            }
            set
            {
                if (_mPCL_DSPhieuYeuCau_ChinhSua == value)
                    return;
                _mPCL_DSPhieuYeuCau_ChinhSua = value;
                NotifyOfPropertyChange(() => mPCL_DSPhieuYeuCau_ChinhSua);
            }
        }




        #endregion

        public void Handle(ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM message)
        {
            if (message == null || message.Patient == null || message.PtRegistration == null)
            {
                return;
            }

            if (SearchCriteria != null)
            {
                SearchCriteria.PatientID = message.Patient.PatientID;
                SearchCriteria.PtRegistrationID = message.PtRegistration.PtRegistrationID;
                Init();
            }
        }

        public void Handle(ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH message)
        {
            if (message == null || message.Patient == null || message.PtRegistration == null)
            {
                return;
            }

            if (SearchCriteria != null)
            {
                SearchCriteria.PatientID = message.Patient.PatientID;
                SearchCriteria.PtRegistrationID = message.PtRegistration.PtRegistrationID;
                Init();
            }
        }

        private long V_RegistrationType
        {
            get
            {
                return Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null ? (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType : (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
        }

        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
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