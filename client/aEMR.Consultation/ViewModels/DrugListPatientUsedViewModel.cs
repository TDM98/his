using eHCMSLanguage;
using System.ComponentModel.Composition;
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
using System.Windows;
using System.Linq;
using System.Text;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDrugListPatientUsed)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugListPatientUsedViewModel : ViewModelBase, IDrugListPatientUsed
        //, IHandle<SelectPatientChangeEvent>
        //, IHandle<ReloadDataePrescriptionEvent>
        //, IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ClearDrugUsedAfterSelectPatientEvent>, IHandle<ClearDrugUsedAfterAddNewEvent>, IHandle<ClearDrugUsedAfterUpdateEvent>
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

        private long? StoreID = 2;//tam thoi mac dinh kho ban(nha thuoc benh vien)

        [ImportingConstructor]
        public DrugListPatientUsedViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            //Globals.EventAggregator.Subscribe(this);
            authorization();

            GetDrugForSellVisitorList = new PagedSortableCollectionView<GetDrugForSellVisitor>();
            GetDrugForSellVisitorList.OnRefresh += GetDrugForSellVisitorList_OnRefresh;
            GetDrugForSellVisitorList.PageSize = Globals.PageSize;

            ResetListDrugPatientUsed();
            //initPatientInfo();

        }

        public void Handle(ClearDrugUsedAfterSelectPatientEvent message)
        {
            ResetListDrugPatientUsed();
        }

        public void Handle(ClearDrugUsedAfterAddNewEvent message)
        {
            ResetListDrugPatientUsed();
        }

        public void Handle(ClearDrugUsedAfterUpdateEvent message)
        {
            ResetListDrugPatientUsed();
        }

        //KMx: Không sử dụng chung event này nữa, vì khó kiểm soát (26/05/2014 13:56)
        //public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    initPatientInfo();
        //}

        private void ResetListDrugPatientUsed()
        {
            if (GetDrugForSellVisitorList != null)
            {
                GetDrugForSellVisitorList.Clear();
            }

            if (!mRaToa_DSToaThuocPhatHanh_ThongTin || Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }

            PatientID = Registration_DataStorage.CurrentPatientRegistration.IsAdmission ? 0 : Registration_DataStorage.CurrentPatient.PatientID;
        }

        //KMx: Khi click vào Ra toa, không tự động load Danh sách thuốc BN đã dùng, chỉ khi nào người dùng click Refresh thì mới load (26/05/2014 14:08).
        //public void initPatientInfo()
        //{
        //    if (Registration_DataStorage.CurrentPatient != null)
        //    {
        //        if (!mRaToa_DSToaThuocPhatHanh_ThongTin)
        //        {
        //            return;
        //        }

        //        PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //        GetDrugForSellVisitorList.PageIndex = 0;
        //        GetListDrugPatientUsed(GetDrugForSellVisitorList.PageIndex, GetDrugForSellVisitorList.PageSize);
        //    }
        //}

        void GetDrugForSellVisitorList_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetListDrugPatientUsed(GetDrugForSellVisitorList.PageIndex, GetDrugForSellVisitorList.PageSize);
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
        private PagedSortableCollectionView<GetDrugForSellVisitor> _GetDrugForSellVisitorList;
        public PagedSortableCollectionView<GetDrugForSellVisitor> GetDrugForSellVisitorList
        {
            get
            {
                return _GetDrugForSellVisitorList;
            }
            set
            {
                if (_GetDrugForSellVisitorList != value)
                {
                    _GetDrugForSellVisitorList = value;
                    NotifyOfPropertyChange(() => GetDrugForSellVisitorList);
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
                    NotifyOfPropertyChange(() => SelectedPrescription);
                }
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


        private void GetListDrugPatientUsed(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int TotalCount = 0;
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetListDrugPatientUsed(PatientID, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetListDrugPatientUsed(out TotalCount, asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorList.TotalItemCount = TotalCount;
                            if (results != null)
                            {
                                foreach (GetDrugForSellVisitor p in results)
                                {
                                    GetDrugForSellVisitorList.Add(p);
                                }
                                NotifyOfPropertyChange(() => GetDrugForSellVisitorList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateDrugNotDisplayInList(long DrugID, bool? NotDisplayInList)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateDrugNotDisplayInList(PatientID, DrugID, NotDisplayInList, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndUpdateDrugNotDisplayInList(asyncResult);
                            GetListDrugPatientUsed(GetDrugForSellVisitorList.PageIndex, GetDrugForSellVisitorList.PageSize);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        private string convertXML(ObservableCollection<GetDrugForSellVisitor> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Drugs>");
            foreach (GetDrugForSellVisitor details in items)
            {
                sb.Append("<RecInfo>");
                sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                sb.Append("</RecInfo>");
            }
            sb.Append("</Drugs>");
            return sb.ToString();
        }

        private void GetDrugForPrescription_Remaining(long? StoreID, ObservableCollection<GetDrugForSellVisitor> items)
        {
            string xml = convertXML(items);
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForPrescription_Remaining(StoreID, xml, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugForPrescription_Remaining(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                for (int j = 0; j < items.Count;j++ )
                                {
                                    for (int i = 0; i < results.Count; i++)
                                    {
                                        if (items[j].DrugID == results[i].DrugID)
                                        {
                                            items[j] = results[i];
                                            break;
                                        }
                                    }
                                }
                            }
                            Globals.EventAggregator.Publish(new SelectListDrugDoubleClickEvent { GetDrugForSellVisitorList = items });
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        DataGrid grdPrescriptions = null;
        public void grdPrescriptions_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescriptions = sender as DataGrid;
        }

        public void grdPrescriptions_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void grdPrescriptions_DblClick(object sender, Common.EventArgs<object> e)
        {
            if (grdPrescriptions.SelectedItem != null)
            {
                GetDrugForSellVisitor item = grdPrescriptions.SelectedItem as GetDrugForSellVisitor;
                GetPrescriptionID(item.PrescriptID.GetValueOrDefault());
            }
        }
        StackPanel PendingClientsGrid = null;
        public void gvPendingClients_Loaded(object sender, RoutedEventArgs e)
        {
            PendingClientsGrid = sender as StackPanel;
        }

        private void GetPrescriptionID(long PrescriptID)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPrescriptionID(PrescriptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedPrescription = contract.EndGetPrescriptionID(asyncResult);
                            GetPrescriptionDetailXml getfuns = new GetPrescriptionDetailXml();
                            getfuns.getPendingClientGrid(PendingClientsGrid, SelectedPrescription);

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

        public void hplRefresh()
        {
            GetDrugForSellVisitorList.PageIndex = 0;
            GetListDrugPatientUsed(GetDrugForSellVisitorList.PageIndex, GetDrugForSellVisitorList.PageSize);
        }

        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (26/05/2014 12:02)
        //public void Handle(ReloadDataePrescriptionEvent message)
        //{
        //    if (message != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //            GetDrugForSellVisitorList.PageIndex = 0;
        //            GetListDrugPatientUsed(GetDrugForSellVisitorList.PageIndex, GetDrugForSellVisitorList.PageSize);
        //        }
        //    }
        //}

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0125_G1_Msg_ConfKhongHienThuThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                GetDrugForSellVisitor item = grdPrescriptions.SelectedItem as GetDrugForSellVisitor;
                UpdateDrugNotDisplayInList(item.DrugID, true);
            }

        }

        public void btnRaToa()
        {
            var items = GetDrugForSellVisitorList.Where(x => x.Checked == true);
            if (items != null && items.Count() > 0)
            {
                GetDrugForPrescription_Remaining(StoreID,items.ToObservableCollection());
            
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0102_G1_Msg_InfoChuaChonThuoc);
            }
        }

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
            if (GetDrugForSellVisitorList != null && GetDrugForSellVisitorList.Count > 0)
            {
                for (int i = 0; i < GetDrugForSellVisitorList.Count; i++)
                {
                    GetDrugForSellVisitorList[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (GetDrugForSellVisitorList != null && GetDrugForSellVisitorList.Count > 0)
            {
                for (int i = 0; i < GetDrugForSellVisitorList.Count; i++)
                {
                    GetDrugForSellVisitorList[i].Checked = false;
                }
            }
        }

        #endregion

        public void InitPatientInfo()
        {
            ResetListDrugPatientUsed();
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