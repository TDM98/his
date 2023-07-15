using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.ServiceClient;
using DataEntities;
using System.Collections.ObjectModel;
using System;
using System.Windows.Controls;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
/*
 * 20180906 #001 TTM:   Gọi InitPatientInfo() để set up dữ liệu cho biến PrescriptionList vì OnActive không được gọi (Nếu gọi OnActive thì bị lỗi PrescriptionDetails IS NULL bên 
 *                      => ePrescriptionOldNewViewModel Line 853.). Nếu không set up thì khi người sử dụng click vào Refresh => Lỗi do biến PrescriptionList bị NULL.
 * 20181004 #002 TTM:   BM 0000138: Thay đổi thông tin thuốc để hiện lên bên phần review, vì nếu làm theo cách cũ thông tin thuốc toàn bộ đc lấy lên kể cả chi tiết không đầy đủ của thuốc.
 *                      => Nếu bệnh nhân có nhiều toa thuốc cũ thì sẽ load rất lâu có thể dẫn đến time out.
 *                      => Thay đổi thành khi nào click vào toa thuốc nào thì mới lấy details toa thuốc đó.
 * 20181012 #003 TTM:   Việc kiểm tra toa thuốc có được edit hay không đang được gộp chung với việc load tất cả toa thuốc => Click Refresh sẽ đi kiểm tra từng bill => không cần thiết 
 *                      => Chậm chương trình.
 *                      => Chuyển việc kiểm tra này từ lúc người dùng click refresh -> khi người dùng double click đem bill sang màn hình ra toa => Chỉ kiểm tra bill đưa sang
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IePrescriptionList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ePrescriptionListViewModel : ViewModelBase, IePrescriptionList
        //, IHandle<SelectPatientChangeEvent>
        //, IHandle<ReloadDataePrescriptionEvent>
        //, IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ClearPrescriptionListAfterAddNewEvent>
        , IHandle<ClearPrescriptionListAfterUpdateEvent>
        , IHandle<ClearPrescriptionListAfterSelectPatientEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        private bool _isInPatient;
        public bool IsInPatient
        {
            get { return _isInPatient; }
            set
            {
                if (_isInPatient != value)
                {
                    _isInPatient = value;
                    NotifyOfPropertyChange(() => IsInPatient);
                }
            }
        }

        private const string ALLITEMS = "--Tất Cả--";


        [ImportingConstructor]
        public ePrescriptionListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();

            GetLookupPrescriptionType();

            //▼====== #001
            InitPatientInfo();
            //▲====== #001
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            InitPatientInfo();
        }
        public void hplRefresh()
        {
            PrescriptionList.PageIndex = 0;
            GetPrescriptsByPtIDPaging(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        }

        //▼====== #002
        private void GetPrescriptDetailsStr_FromPrescriptID(long PrescriptID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPrescriptDetailsStr_FromPrescriptID(PrescriptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedPrescription.PrescriptDetailsStr = contract.EndGetPrescriptDetailsStr_FromPrescriptID(asyncResult);
                            GetPrescriptionDetailXml getfuns = new GetPrescriptionDetailXml();
                            getfuns.getPendingClientGrid(PendingClientsGrid, SelectedPrescription);
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
        //▲====== #002

        //KMx: Không sử dụng chung event, vì khó kiểm soát (26/05/2014 10:51).
        //public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    //initPatientInfo();
        //}

        public void Handle(ClearPrescriptionListAfterSelectPatientEvent message)
        {
            ResetPrescriptionList();
        }

        public void Handle(ClearPrescriptionListAfterAddNewEvent message)
        {
            ResetPrescriptionList();
        }

        public void Handle(ClearPrescriptionListAfterUpdateEvent message)
        {
            ResetPrescriptionList();
        }

        public void InitPatientInfo()
        {
            PrescriptionList = new PagedSortableCollectionView<Prescription>();
            PrescriptionList.OnRefresh += PrescriptionList_OnRefresh;
            PrescriptionList.PageSize = Globals.PageSize;

            //KMx: Trước đây, khi click vào ra toa thì sẽ tự động lấy tất cả danh sách toa thuốc của BN dẫn đến chậm.
            //Bây giờ không tự load nữa, chỉ khi nào người dùng click refresh thì mới lấy (26/05/2014 11:05).
            ResetPrescriptionList();
        }

        //public void initPatientInfo()
        //{
        //    GetLookupPrescriptionType();

        //    PrescriptionList = new PagedSortableCollectionView<Prescription>();
        //    PrescriptionList.OnRefresh += PrescriptionList_OnRefresh;
        //    PrescriptionList.PageSize = Globals.PageSize;
        //    //
        //    V_PrescriptionType = 0;
        //    if (Registration_DataStorage.CurrentPatient != null)
        //    {
        //        if (!mRaToa_DSToaThuocPhatHanh_ThongTin)
        //        {
        //            return;
        //        }

        //        PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //        PrescriptionList.PageIndex = 0;
        //        GetPrescriptsByPtIDPaging(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        //    }
        //}
        private bool ProgLoading = false;
        private void ResetPrescriptionList()
        {
            ProgLoading = true;
            V_PrescriptionType = 0;
            ProgLoading = false;
            if (PrescriptionList != null)
            {
                PrescriptionList.Clear();
            }

            if (!mRaToa_DSToaThuocPhatHanh_ThongTin || Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }

            PatientID = Registration_DataStorage.CurrentPatientRegistration.IsAdmission ? 0 : Registration_DataStorage.CurrentPatient.PatientID;
        }

        void PrescriptionList_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetPrescriptsByPtIDPaging(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (26/05/2014 09:16).
        //public void Handle(SelectPatientChangeEvent obj)
        //{
        //    if (obj != null)
        //    {
        //        PatientID = (long)obj.PatientID;
        //    }
        //}

        #region Properties Member
        private PagedSortableCollectionView<Prescription> _PrescriptionList;
        public PagedSortableCollectionView<Prescription> PrescriptionList
        {
            get
            {
                return _PrescriptionList;
            }
            set
            {
                if (_PrescriptionList != value)
                {
                    _PrescriptionList = value;
                    NotifyOfPropertyChange(() => PrescriptionList);
                }
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
                    if (_SelectedPrescription != null)
                    {
                        PrescriptID = _SelectedPrescription.PrescriptID;
                    }
                    else
                    {
                        PrescriptID = null;
                    }
                    //▼====== #002: Giá trị của SelectedPrescription thay đổi khi click vào Grid nên đặt hàm lấy Details thuốc ở đây.
                    if (PrescriptID.GetValueOrDefault() > 0)
                    {
                        GetPrescriptDetailsStr_FromPrescriptID(PrescriptID.GetValueOrDefault());
                    }
                    //▲====== #002
                    NotifyOfPropertyChange(() => SelectedPrescription);
                    NotifyOfPropertyChange(() => CurrentPrescriptionTemplate);
                }
            }
        }

        public ObservableCollection<Prescription> CurrentPrescriptionTemplate
        {
            get
            {
                return new ObservableCollection<Prescription> { SelectedPrescription };
            }
        }

        private long? _PrescriptID;
        public long? PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                if (_PrescriptID != value)
                {
                    _PrescriptID = value;
                    NotifyOfPropertyChange(() => PrescriptID);
                }
            }
        }
        private long? _V_PrescriptionType;
        public long? V_PrescriptionType
        {
            get
            {
                return _V_PrescriptionType;
            }
            set
            {
                if (_V_PrescriptionType != value)
                {
                    _V_PrescriptionType = value;
                    NotifyOfPropertyChange(() => V_PrescriptionType);
                }
            }
        }

        private ObservableCollection<Lookup> _PrescriptionType;
        public ObservableCollection<Lookup> PrescriptionTypeList
        {
            get
            {
                return _PrescriptionType;
            }
            set
            {
                if (_PrescriptionType != value)
                {
                    _PrescriptionType = value;
                    NotifyOfPropertyChange(() => PrescriptionTypeList);
                }
            }
        }

        #endregion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mRaToa_DSToaThuocPhatHanh_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                              , (int)eConsultation.mPtePrescriptionTab,
                                              (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_ThongTin, (int)ePermission.mView);
            mRaToa_DSToaThuocPhatHanh_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtePrescriptionTab,
                                                   (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_ChinhSua, (int)ePermission.mView);
            mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtePrescriptionTab,
                                                   (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS, (int)ePermission.mView);
            mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtePrescriptionTab,
                                                   (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh, (int)ePermission.mView);
            mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtePrescriptionTab,
                                                   (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc, (int)ePermission.mView);
        }
        #region account checking

        private bool _mRaToa_DSToaThuocPhatHanh_ThongTin = true;
        private bool _mRaToa_DSToaThuocPhatHanh_ChinhSua = true;
        private bool _mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS = true;
        private bool _mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh = true;
        private bool _mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc = true;

        public bool mRaToa_DSToaThuocPhatHanh_ThongTin
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_ThongTin;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_ThongTin == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_ThongTin = value;
            }
        }
        public bool mRaToa_DSToaThuocPhatHanh_ChinhSua
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_ChinhSua;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_ChinhSua == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_ChinhSua = value;
            }
        }
        public bool mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_LinkXemKquaXetNghiemCLS = value;
            }
        }
        public bool mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_LinkXemToaThuocHienHanh = value;
            }
        }
        public bool mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_LinkXemLichSuToaThuoc = value;
            }
        }

        //   private bool _bhpkViewPCLResult = true;
        //private bool _bhpkViewEPrescription = true;
        //private bool _bhpkViewEPrescriptIssueHis = true;
        //private bool _blnkDelete = true;
        //public bool bhpkViewPCLResult
        //{
        //    get
        //    {
        //        return _bhpkViewPCLResult;
        //    }
        //    set
        //    {
        //        if (_bhpkViewPCLResult == value)
        //            return;
        //        _bhpkViewPCLResult = value;
        //    }
        //}
        //public bool bhpkViewEPrescription
        //{
        //    get
        //    {
        //        return _bhpkViewEPrescription;
        //    }
        //    set
        //    {
        //        if (_bhpkViewEPrescription == value)
        //            return;
        //        _bhpkViewEPrescription = value;
        //    }
        //}
        //public bool bhpkViewEPrescriptIssueHis
        //{
        //    get
        //    {
        //        return _bhpkViewEPrescriptIssueHis;
        //    }
        //    set
        //    {
        //        if (_bhpkViewEPrescriptIssueHis == value)
        //            return;
        //        _bhpkViewEPrescriptIssueHis = value;
        //    }
        //}
        //public bool blnkDelete
        //{
        //    get
        //    {
        //        return _blnkDelete;
        //    }
        //    set
        //    {
        //        if (_blnkDelete == value)
        //            return;
        //        _blnkDelete = value;
        //    }
        //}
        #endregion

        private void GetPrescriptsByPtIDPaging(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int TotalCount = 0;
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPrescriptionByPtID_Paging(PatientID, V_PrescriptionType, IsInPatient, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetPrescriptionByPtID_Paging(out TotalCount, asyncResult);
                            PrescriptionList.Clear();
                            PrescriptionList.TotalItemCount = TotalCount;
                            if (results != null)
                            {
                                foreach (Prescription p in results)
                                {
                                    PrescriptionList.Add(p);
                                }
                                NotifyOfPropertyChange(() => PrescriptionList);
                            }
                            SelectedPrescription = PrescriptionList.FirstOrDefault();
                            Grid_Loaded(PendingClientsGrid);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetLookupPrescriptionType()
        {
            ObservableCollection<Lookup> PrescriptLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PRESCRIPTION_TYPE).ToObservableCollection();

            if (PrescriptLookupList == null || PrescriptLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0742_G1_Msg_InfoKhTimThayLoaiToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (PrescriptionTypeList == null)
            {
                PrescriptionTypeList = new ObservableCollection<Lookup>();
            }
            else
            {
                PrescriptionTypeList.Clear();
            }
            Lookup ite = new Lookup();
            ite.LookupID = 0;
            ite.ObjectValue = ALLITEMS;
            PrescriptionTypeList.Add(ite);
            foreach (Lookup p in PrescriptLookupList)
            {
                PrescriptionTypeList.Add(p);
            }
            NotifyOfPropertyChange(() => PrescriptionTypeList);
        }

        public void grdPrescriptions_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void cboVPrescriptType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProgLoading) return;
            ComboBox cboVBehaving = sender as ComboBox;
            if (cboVBehaving != null)
            {
                PrescriptionList.PageIndex = 0;
                GetPrescriptsByPtIDPaging(PrescriptionList.PageIndex, PrescriptionList.PageSize);
            }
        }

        public void grdPrescriptions_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien de form cha bat 
            //  Kiem tra xem co quyen sua toa thuoc chon nay ko
            if (!mRaToa_DSToaThuocPhatHanh_ChinhSua)
            {
                Globals.ShowMessage(eHCMSResources.Z0511_G1_I, "");
                return;
            }

            if ((sender as DataGrid).SelectedItem != null)
            {
                SelectedPrescription = (sender as DataGrid).SelectedItem as Prescription;
                if (IsInPatient)
                {
                    GetPrescriptionDetailsByPrescriptID_InPt(SelectedPrescription.PrescriptID);
                }
                else
                {
                    //▼====== #003
                    //GetPrescriptionDetailsByPrescriptID(SelectedPrescription.PrescriptID);
                    GetPrescriptionDetailsByPrescriptID_V2(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID, SelectedPrescription.AppointmentID);
                    //▲====== #003
                }
            }

        }

        public void GetPrescriptionDetailsByPrescriptID(long prescriptID, bool GetRemaining = false)
        {
            IsLoading = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, GetRemaining, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
                            SelectedPrescription.PrescriptionDetails = Results.ToObservableCollection();
                            if (SelectedPrescription.PrescriptionDetails == null || SelectedPrescription.PrescriptionDetails.Count < 1)
                            {
                                MessageBox.Show(eHCMSResources.Z2319_G1_KhongTheTaoTacToaKhongThuoc);
                            }
                            else
                            {
                                Globals.EventAggregator.Publish(new ePrescriptionDoubleClickEvent { SelectedPrescription = this.SelectedPrescription });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;

                        }

                    }), null);

                }

            });

            t.Start();
        }
        //▼====== #003
        public void GetPrescriptionDetailsByPrescriptID_V2(long prescriptID, long? issueID, long? appointmentID)
        {
            IsLoading = true;
            bool CanEdit = false;
            bool IsEdit = false;
            string ReasonCanEdit = "";
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_V2(prescriptID, issueID, appointmentID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_V2(out CanEdit, out ReasonCanEdit, out IsEdit, asyncResult);
                            SelectedPrescription.PrescriptionDetails = Results.ToObservableCollection();
                            SelectedPrescription.ReasonCanEdit = ReasonCanEdit;
                            SelectedPrescription.CanEdit = CanEdit;
                            SelectedPrescription.IsAllowEditNDay = IsEdit;
                            if (SelectedPrescription.PrescriptionDetails == null || SelectedPrescription.PrescriptionDetails.Count < 1)
                            {
                                MessageBox.Show(eHCMSResources.Z2319_G1_KhongTheTaoTacToaKhongThuoc);
                            }
                            else
                            {
                                Globals.EventAggregator.Publish(new ePrescriptionDoubleClickEvent { SelectedPrescription = this.SelectedPrescription });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }
        //▲====== #003
        public void GetPrescriptionDetailsByPrescriptID_InPt(long prescriptID, bool GetRemaining = false)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(prescriptID, GetRemaining, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            SelectedPrescription.PrescriptionDetails = Results.ToObservableCollection();
                            if (SelectedPrescription.PrescriptionDetails == null || SelectedPrescription.PrescriptionDetails.Count < 1)
                            {
                                MessageBox.Show(eHCMSResources.Z2319_G1_KhongTheTaoTacToaKhongThuoc);
                            }
                            else
                            {
                                Globals.EventAggregator.Publish(new ePrescriptionDoubleClickEvent_InPt_1());
                                Globals.EventAggregator.Publish(new ePrescriptionDoubleClickEvent_InPt_2 { SelectedPrescription = this.SelectedPrescription });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
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




        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (26/05/2014 11:06).
        //public void Handle(ReloadDataePrescriptionEvent message)
        //{
        //    if (message != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //            PrescriptionList.PageIndex = 0;
        //            GetPrescriptsByPtIDPaging(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        //        }
        //    }
        //}


        #region link member

        public void hpkViewPCLResult()
        { }

        public void hpkViewEPrescription()
        {
            if (SelectedPrescription != null && SelectedPrescription.IssueID > 0)
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.IssueID = SelectedPrescription.IssueID;
                //if (IsInPatient)
                //{
                //    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT;
                //}
                //else
                //{
                //    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
                //}

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.IssueID = SelectedPrescription.IssueID;
                    if (IsInPatient)
                    {
                        proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
                    }
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0298_G1_Msg_InfoChonToaThuocDaPHanhDeXemIn);
            }

        }
        public void hpkViewEPrescriptIssueHis()
        {
            //var proAlloc = Globals.GetViewModel<IPrescriptIssueHistory>();
            //proAlloc.PrescriptID = SelectedPrescription.PrescriptID;
            //proAlloc.GetPrescriptionIssueHistory();
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IPrescriptIssueHistory> onInitDlg = (proAlloc) =>
            {
                proAlloc.PrescriptID = SelectedPrescription.PrescriptID;
                proAlloc.GetPrescriptionIssueHistory();
            };
            GlobalsNAV.ShowDialog<IPrescriptIssueHistory>(onInitDlg);
        }
        #endregion

        StackPanel PendingClientsGrid;
        public void Grid_Loaded(object sender)
        {
            PendingClientsGrid = sender as StackPanel;
            //▼====== #002: Dời code này lên Line 117
            //GetPrescriptionDetailXml getfuns = new GetPrescriptionDetailXml();
            //getfuns.getPendingClientGrid(PendingClientsGrid, SelectedPrescription);
            //▲====== #002
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
