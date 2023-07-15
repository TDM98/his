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

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IePrescriptionTemplateDoctor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ePrescriptionTemplateDoctorViewModel : ViewModelBase, IePrescriptionTemplateDoctor
        , IHandle<ReloadePrescriptionTemplateEvent>
        , IHandle<ClearPrescriptTemplateAfterSelectPatientEvent>
        //, IHandle<SelectPatientChangeEvent>
        //, IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
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

        private const string ALLITEMS = "--Tất Cả--";


        [ImportingConstructor]
        public ePrescriptionTemplateDoctorViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            //Globals.EventAggregator.Subscribe(this);
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            InitPatientInfo();
        }
        public void hplRefresh()
        {
            allPrescriptionTemplate.PageIndex = 0;
            GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
        }

        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (26/05/2014 11:20)
        //public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    initPatientInfo();
        //}

        public void Handle(ClearPrescriptTemplateAfterSelectPatientEvent message)
        {
            ResetPrescriptionTemplate();
        }

        private void ResetPrescriptionTemplate()
        {
            if (allPrescriptionTemplate != null)
            {
                allPrescriptionTemplate.Clear();
            }

            if (!mRaToa_DSToaThuocPhatHanh_ThongTin || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }

            PatientID = Registration_DataStorage.CurrentPatientRegistration.IsAdmission ? 0 : Registration_DataStorage.CurrentPatient.PatientID;
        }

        public void InitPatientInfo()
        {
            //GetLookupPrescriptionType();

            SelectedPrescriptionTemplate = new PrescriptionTemplate();
            allPrescriptionTemplate = new PagedSortableCollectionView<PrescriptionTemplate>();
            allPrescriptionTemplate.OnRefresh += allPrescriptionTemplate_OnRefresh;
            allPrescriptionTemplate.PageSize = Globals.PageSize;

            //KMx: Trước đây, khi click vào ra toa thì sẽ tự động lấy tất cả danh sách toa thuốc mẫu của BS dẫn đến chậm.
            //Bây giờ không tự load nữa, chỉ khi nào người dùng click refresh thì mới lấy (26/05/2014 11:05).
            ResetPrescriptionTemplate();
        }

        //public void initPatientInfo()
        //{
        //    GetLookupPrescriptionType();
        //    SelectedPrescriptionTemplate = new PrescriptionTemplate();
        //    allPrescriptionTemplate = new PagedSortableCollectionView<PrescriptionTemplate>();
        //    allPrescriptionTemplate.OnRefresh += allPrescriptionTemplate_OnRefresh;
        //    allPrescriptionTemplate.PageSize = Globals.PageSize;
        //    //
        //    V_PrescriptionType = 0;
        //    if (Registration_DataStorage.CurrentPatient != null)
        //    {
        //        if (!mRaToa_DSToaThuocPhatHanh_ThongTin)
        //        {
        //            return;
        //        }

        //        PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //        allPrescriptionTemplate.PageIndex = 0;
        //        GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
        //    }
        //}

        void allPrescriptionTemplate_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
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

        private PagedSortableCollectionView<PrescriptionTemplate> _allPrescriptionTemplate;
        public PagedSortableCollectionView<PrescriptionTemplate> allPrescriptionTemplate
        {
            get
            {
                return _allPrescriptionTemplate;
            }
            set
            {
                if (_allPrescriptionTemplate != value)
                {
                    _allPrescriptionTemplate = value;
                    NotifyOfPropertyChange(() => allPrescriptionTemplate);
                }
            }
        }

        private PrescriptionTemplate _SelectedPrescriptionTemplate;
        public PrescriptionTemplate SelectedPrescriptionTemplate
        {
            get
            {
                return _SelectedPrescriptionTemplate;
            }
            set
            {
                if (_SelectedPrescriptionTemplate != value)
                {
                    _SelectedPrescriptionTemplate = value;
                    NotifyOfPropertyChange(() => SelectedPrescriptionTemplate);
                    NotifyOfPropertyChange(() => CurrentPrescriptionTemplate);
                }
            }
        }

        public ObservableCollection<PrescriptionTemplate> CurrentPrescriptionTemplate
        {
            get
            {
                return new ObservableCollection<PrescriptionTemplate> { SelectedPrescriptionTemplate };
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

        //private long? _V_PrescriptionType;
        //public long? V_PrescriptionType
        //{
        //    get
        //    {
        //        return _V_PrescriptionType;
        //    }
        //    set
        //    {
        //        if (_V_PrescriptionType != value)
        //        {
        //            _V_PrescriptionType = value;
        //            NotifyOfPropertyChange(() => V_PrescriptionType);
        //        }
        //    }
        //}

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


        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionsTemplateDelete();
        }

        private void GetPrescriptsTemplate(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int TotalCount = 0;
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPrescriptionsTemplateGetAll(new PrescriptionTemplate { DoctorStaffID = Globals.LoggedUserAccount.StaffID }
                        //, PageIndex, PageSize
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPrescriptionsTemplateGetAll(//out TotalCount, 
                                asyncResult);
                            allPrescriptionTemplate.Clear();
                            allPrescriptionTemplate.TotalItemCount = TotalCount;
                            if (results != null
                                && results.Count > 0)
                            {
                                foreach (PrescriptionTemplate p in results)
                                {
                                    allPrescriptionTemplate.Add(p);
                                }
                                NotifyOfPropertyChange(() => allPrescriptionTemplate);
                                SelectedPrescriptionTemplate = allPrescriptionTemplate.FirstOrDefault();
                                Grid_Loaded(PendingClientsGrid);
                            }

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

        private void PrescriptionsTemplateDelete()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPrescriptionsTemplateDelete(SelectedPrescriptionTemplate
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPrescriptionsTemplateDelete(asyncResult);
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        //private void GetLookupPrescriptionType()
        //{
        //    ObservableCollection<Lookup> PrescriptLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PRESCRIPTION_TYPE).ToObservableCollection();

        //    if (PrescriptLookupList == null || PrescriptLookupList.Count <= 0)
        //    {
        //        MessageBox.Show("Không tìm thấy Loại Toa Thuốc.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }

        //    if (PrescriptionTypeList == null)
        //    {
        //        PrescriptionTypeList = new ObservableCollection<Lookup>();
        //    }
        //    else
        //    {
        //        PrescriptionTypeList.Clear();
        //    }
        //    Lookup ite = new Lookup();
        //    ite.LookupID = 0;
        //    ite.ObjectValue = ALLITEMS;
        //    PrescriptionTypeList.Add(ite);
        //    foreach (Lookup p in PrescriptLookupList)
        //    {
        //        PrescriptionTypeList.Add(p);
        //    }
        //    NotifyOfPropertyChange(() => PrescriptionTypeList);
        //}

        public void grdPrescriptions_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //public void cboVPrescriptType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox cboVBehaving = sender as ComboBox;
        //    if (cboVBehaving != null)
        //    {
        //        allPrescriptionTemplate.PageIndex = 0;
        //        GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
        //    }
        //}

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
                SelectedPrescriptionTemplate = (sender as DataGrid).SelectedItem as PrescriptionTemplate;
                GetPrescriptionDetailsByPrescriptID(SelectedPrescriptionTemplate.PrescriptID);
            }

        }

        public void GetPrescriptionDetailsByPrescriptID(long prescriptID)
        {
            IsLoading = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_WithNDay(prescriptID, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_WithNDay(out int NDay, asyncResult);
                            SelectedPrescriptionTemplate.prescription.PrescriptionDetails = Results.ToObservableCollection();
                            if (SelectedPrescriptionTemplate.prescription.PrescriptionDetails == null || SelectedPrescriptionTemplate.prescription.PrescriptionDetails.Count < 1)
                            {
                                MessageBox.Show(eHCMSResources.A0505_G1_Msg_InfoRefreshDSToaThuoc);
                            }
                            else
                            {
                                SelectedPrescriptionTemplate.prescription.NDay = NDay;
                                Globals.EventAggregator.Publish(new ePrescriptionDoubleClickEvent
                                {
                                    SelectedPrescription = SelectedPrescriptionTemplate.prescription,
                                    isTemplate = true
                                });
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

        public void Handle(ReloadePrescriptionTemplateEvent message)
        {
            if (message != null)
            {
                PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                allPrescriptionTemplate.PageIndex = 0;
                GetPrescriptsTemplate(allPrescriptionTemplate.PageIndex, allPrescriptionTemplate.PageSize);
            }
        }


        #region link member

        public void hpkViewPCLResult()
        { }

        public void hpkViewEPrescription()
        {
            if (SelectedPrescriptionTemplate != null && SelectedPrescriptionTemplate.prescription.IssueID > 0)
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.IssueID = SelectedPrescriptionTemplate.prescription.IssueID;
                //proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.IssueID = SelectedPrescriptionTemplate.prescription.IssueID;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
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
            //proAlloc.PrescriptID = SelectedPrescriptionTemplate.PrescriptID;
            //proAlloc.GetPrescriptionIssueHistory();
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IPrescriptIssueHistory> onInitDlg = (proAlloc) =>
            {
                proAlloc.PrescriptID = SelectedPrescriptionTemplate.PrescriptID;
                proAlloc.GetPrescriptionIssueHistory();
            };
            GlobalsNAV.ShowDialog<IPrescriptIssueHistory>(onInitDlg);
        }
        #endregion

        StackPanel PendingClientsGrid;
        public void Grid_Loaded(object sender)
        {
            PendingClientsGrid = sender as StackPanel;
            if (SelectedPrescriptionTemplate != null)
            {
                GetPrescriptionDetailXml getfuns = new GetPrescriptionDetailXml();
                getfuns.getPendingClientGrid(PendingClientsGrid, SelectedPrescriptionTemplate.prescription);
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