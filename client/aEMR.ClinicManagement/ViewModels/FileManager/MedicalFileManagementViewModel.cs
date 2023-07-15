using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.Windows;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Controls;
using aEMR.Infrastructure.Events;
using System.Windows.Controls;
using System.Collections.Generic;
/*
* 20220818 #001 QTD: Chỉnh sửa màn hình quản lý hồ sơ
*/
namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IMedicalFileManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalFileManagementViewModel : Conductor<object>, IMedicalFileManagement, IHandle<MedicalFileManagerEdit_Event>
    {
        #region Properties
        private ObservableCollection<RefStorageWarehouseLocation> _AllStores;
        public ObservableCollection<RefStorageWarehouseLocation> AllStores
        {
            get
            {
                return _AllStores;
            }
            set
            {
                if (_AllStores != value)
                {
                    _AllStores = value;
                    NotifyOfPropertyChange(() => AllStores);
                }
            }
        }
        private RefStorageWarehouseLocation _SelectedStore;
        public RefStorageWarehouseLocation SelectedStore
        {
            get
            {
                return _SelectedStore;
            }
            set
            {
                if (_SelectedStore != value)
                {
                    _SelectedStore = value;
                    NotifyOfPropertyChange(() => SelectedStore);
                    //SelectedRefShelfDetail = new RefShelfDetails();
                    //SelectedRefShelf = new RefShelves();
                    //LoadShelfDetails();
                    LoadRefRows();
                }
            }
        }
        private ObservableCollection<RefShelfDetails> _AllRefShelfDetails;
        public ObservableCollection<RefShelfDetails> AllRefShelfDetails
        {
            get
            {
                return _AllRefShelfDetails;
            }
            set
            {
                _AllRefShelfDetails = value;
                NotifyOfPropertyChange(() => AllRefShelfDetails);
            }
        }
        private RefShelfDetails _SelectedRefShelfDetail;
        public RefShelfDetails SelectedRefShelfDetail
        {
            get
            {
                return _SelectedRefShelfDetail;
            }
            set
            {
                if (_SelectedRefShelfDetail != value)
                {
                    _SelectedRefShelfDetail = value;
                    NotifyOfPropertyChange(() => SelectedRefShelfDetail);
                }
                //if (value != null && value != _SelectedRefShelfDetail)
                //{
                //    if (txtShelfDetail == null || String.IsNullOrEmpty(txtShelfDetail.Text))
                //        value = null;
                //    _SelectedRefShelfDetail = value;
                //    NotifyOfPropertyChange(() => SelectedRefShelfDetail);
                //    LoadShelf(SelectedRefShelfDetail == null ? null : (long?)SelectedRefShelfDetail.RefShelfID);
                //}
            }
        }
        private ObservableCollection<RefShelves> _AllRefShelfs;
        public ObservableCollection<RefShelves> AllRefShelfs
        {
            get
            {
                return _AllRefShelfs;
            }
            set
            {
                if (_AllRefShelfs != value)
                {
                    _AllRefShelfs = value;
                    NotifyOfPropertyChange(() => AllRefShelfs);
                }
            }
        }
        private RefShelves _SelectedRefShelf;
        public RefShelves SelectedRefShelf
        {
            get
            {
                return _SelectedRefShelf;
            }
            set
            {
                if (_SelectedRefShelf != value)
                {
                    _SelectedRefShelf = value;
                    NotifyOfPropertyChange(() => SelectedRefShelf);
                    LoadShelfDetails();
                }
            }
        }
        private string _FileCodeNumber;
        public string FileCodeNumber
        {
            get
            {
                return _FileCodeNumber;
            }
            set
            {
                _FileCodeNumber = value;
                NotifyOfPropertyChange(() => FileCodeNumber);
            }
        }
        public int PageSize { get { return 40; } }
        private PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> _AllPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
        public PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> AllPatientMedicalFileStorageCheckOut
        {
            get
            {
                return _AllPatientMedicalFileStorageCheckOut;
            }
            set
            {
                _AllPatientMedicalFileStorageCheckOut = value;
                NotifyOfPropertyChange(() => AllPatientMedicalFileStorageCheckOut);
            }
        }
        AxAutoComplete txtShelfDetail;
        //TMA
        public void hplPrint_Click(object sender)
        {
            var mRefShelfDetails = sender as PatientMedicalFileStorageCheckInCheckOut;
            //Xuat Report
            print(mRefShelfDetails);
        }
        public void print(PatientMedicalFileStorageCheckInCheckOut curReport)
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientCode = curReport.PatientCode;
            //proAlloc.FileCodeNumber = curReport.FileCodeNumber;
            //proAlloc.PatientName = curReport.FullName;
            //if (curReport.IsAgeOnly == true)
            //{
            //    proAlloc.DOB = Convert.ToString(curReport.yDOB.Year);
            //}
            //else
            //{
            //    proAlloc.DOB = curReport.yDOB.ToString("dd/MM/yyyy");
            //}
            //proAlloc.eItem = ReportName.CLINIC_INFO;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientCode = curReport.PatientCode;
                proAlloc.FileCodeNumber = curReport.FileCodeNumber;
                proAlloc.PatientName = curReport.FullName;
                if (curReport.IsAgeOnly == true)
                {
                    proAlloc.DOB = Convert.ToString(curReport.yDOB.Year);
                }
                else
                {
                    proAlloc.DOB = curReport.yDOB.ToString("dd/MM/yyyy");
                }
                proAlloc.eItem = ReportName.CLINIC_INFO;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

        }
        //TMA
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedicalFileManagementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            LoadStores();
            AllPatientMedicalFileStorageCheckOut.OnRefresh += AllPatientMedicalFileStorageCheckOut_OnRefresh;
            V_MedicalFileType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedicalFileType).ToObservableCollection();
            SetDefaultV_MedicalFileType();
            V_MedicalFileStatus = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedicalFileStatus).ToObservableCollection();
            SetDefaultV_MedicalFileStatus();
        }
        public void btnSearch()
        {
            if (SelectedMedicalFileType == null || SelectedMedicalFileType.LookupID <= 0)
            {
                MessageBox.Show("Chưa chọn loại hồ sơ!");
                return;
            }
            if(SelectedStore == null || SelectedRow == null || SelectedRefShelf == null || SelectedRefShelfDetail == null || SelectedMedicalFileType == null)
            {
                MessageBox.Show(eHCMSResources.T0432_G1_Error);
                return;
            }    
            SearchFileDetails(0);
        }
        public void txtShelfDetail_Loaded(object sender, RoutedEventArgs e)
        {
            txtShelfDetail = sender as AxAutoComplete;
        }
        #endregion
        #region Method
        private void LoadShelfDetails()
        {
            if (SelectedRefShelf == null || SelectedRefShelf.RefShelfID <= 0)
            {
                AllRefShelfDetails = new ObservableCollection<RefShelfDetails>();
                AddFirstItemRefShelfDetails();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelfDetails(SelectedRefShelf.RefShelfID, null, 0, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelfDetails = contract.EndGetRefShelfDetails(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                AddFirstItemRefShelfDetails();
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //private void LoadShelf(long? RefShelfID)
        //{
        //    if (RefShelfID == null)
        //    {
        //        SelectedRefShelf = null;
        //        return;
        //    }
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ClinicManagementServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetRefShelves(RefShelfID.Value, 0, null,
        //                Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    AllRefShelfs = contract.EndGetRefShelves(asyncResult);
        //                    if (AllRefShelfs.Count == 1)
        //                    {
        //                        SelectedRefShelf = AllRefShelfs.First();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //        }
        //        finally
        //        {
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}
        private void LoadStores()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStoragesNotPaging((long)AllLookupValues.StoreType.STORAGE_FILES, false, null, null,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    AllStores = new ObservableCollection<RefStorageWarehouseLocation>(contract.EndGetAllStoragesNotPaging(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    if (AllStores != null && AllStores.Count > 0)
                                    {
                                        SelectedStore = AllStores.FirstOrDefault();
                                    }
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //private void LoadFileDetails(int PageIndex)
        //{
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ClinicManagementServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetMedicalFileDetails(FileCodeNumber, SelectedRefShelfDetail == null ? null : (long?)SelectedRefShelfDetail.RefShelfDetailID, PageSize, PageIndex,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            int TotalRecord = 0;
        //                            var GettedItems = contract.EndGetMedicalFileDetails(out TotalRecord, asyncResult);
        //                            AllPatientMedicalFileStorageCheckOut.Clear();
        //                            foreach (var item in GettedItems)
        //                                AllPatientMedicalFileStorageCheckOut.Add(item);
        //                            AllPatientMedicalFileStorageCheckOut.PageSize = PageSize;
        //                            AllPatientMedicalFileStorageCheckOut.PageIndex = PageIndex;
        //                            AllPatientMedicalFileStorageCheckOut.TotalItemCount = TotalRecord;
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message);
        //                        }
        //                        finally
        //                        {
        //                            this.HideBusyIndicator();
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}
        private void AllPatientMedicalFileStorageCheckOut_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchFileDetails(AllPatientMedicalFileStorageCheckOut.PageIndex);
        }
        //▼====: #002
        #endregion
        private ObservableCollection<RefRows> _AllRefRows;
        public ObservableCollection<RefRows> AllRefRows
        {
            get
            {
                return _AllRefRows;
            }
            set
            {
                if (_AllRefRows != value)
                {
                    _AllRefRows = value;
                    NotifyOfPropertyChange(() => AllRefRows);
                }
            }
        }
        private Lookup _SelectedMedicalFileType;
        public Lookup SelectedMedicalFileType
        {
            get
            {
                return _SelectedMedicalFileType;
            }
            set
            {
                if (_SelectedMedicalFileType != value)
                {
                    _SelectedMedicalFileType = value;
                    NotifyOfPropertyChange(() => SelectedMedicalFileType);
                }
            }
        }
        private void SetDefaultV_MedicalFileType()
        {
            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, "Chọn loại");
            V_MedicalFileType.Insert(0, firstItem);
            if (V_MedicalFileType != null)
            {
                SelectedMedicalFileType = V_MedicalFileType.FirstOrDefault();
            }
        }

        public void Handle(MedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                SearchFileDetails(AllPatientMedicalFileStorageCheckOut.PageIndex);
            }
        }

        private ObservableCollection<Lookup> _V_MedicalFileType;
        public ObservableCollection<Lookup> V_MedicalFileType
        {
            get
            {
                return _V_MedicalFileType;
            }
            set
            {
                if (_V_MedicalFileType != value)
                {
                    _V_MedicalFileType = value;
                    NotifyOfPropertyChange(() => V_MedicalFileType);
                }
            }
        }
        private ObservableCollection<Lookup> _V_MedicalFileStatus;
        public ObservableCollection<Lookup> V_MedicalFileStatus
        {
            get
            {
                return _V_MedicalFileStatus;
            }
            set
            {
                if (_V_MedicalFileStatus != value)
                {
                    _V_MedicalFileStatus = value;
                    NotifyOfPropertyChange(() => V_MedicalFileStatus);
                }
            }
        }
        private void SetDefaultV_MedicalFileStatus()
        {
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = eHCMSResources.T0822_G1_TatCa;
            V_MedicalFileStatus.Insert(0, firstItem);
            if (V_MedicalFileStatus != null)
            {
                SelectedMedicalFileStatus = V_MedicalFileStatus.FirstOrDefault();
            }
        }
        private Lookup _SelectedMedicalFileStatus;
        public Lookup SelectedMedicalFileStatus
        {
            get
            {
                return _SelectedMedicalFileStatus;
            }
            set
            {
                if (_SelectedMedicalFileStatus != value)
                {
                    _SelectedMedicalFileStatus = value;
                    NotifyOfPropertyChange(() => SelectedMedicalFileStatus);
                }
            }
        }
        private void LoadRefRows()
        {
            if (SelectedStore == null || SelectedStore.StoreID <= 0)
            {
                AllRefRows = new ObservableCollection<RefRows>();
                AddFirstItemRefRow();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefRows(SelectedStore.StoreID, null, 0,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    AllRefRows = contract.EndGetRefRows(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    AddFirstItemRefRow();
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadShelf()
        {
            if (SelectedRow == null || SelectedRow.RefRowID <= 0)
            {
                AllRefShelfs = new ObservableCollection<RefShelves>();
                AddFirstItemRefShelf();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelves(0, SelectedRow.RefRowID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelfs = contract.EndGetRefShelves(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                AddFirstItemRefShelf();
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private RefRows _SelectedRow;
        public RefRows SelectedRow
        {
            get
            {
                return _SelectedRow;
            }
            set
            {
                if (_SelectedRow != value)
                {
                    _SelectedRow = value;
                    NotifyOfPropertyChange(() => SelectedRow);
                    LoadShelf();
                }
            }
        }
        private void SearchFileDetails(int PageIndex)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchMedicalFileDetails(SelectedStore.StoreID, SelectedRow.RefRowID, SelectedRefShelf.RefShelfID, SelectedRefShelfDetail.RefShelfDetailID,
                            SelectedMedicalFileType.LookupID, FileCodeNumber, PatientCode, PatientName, SelectedMedicalFileStatus.LookupID, PageSize, PageIndex,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    int TotalRecord = 0;
                                    var GettedItems = contract.EndSearchMedicalFileDetails(out TotalRecord, asyncResult);
                                    AllPatientMedicalFileStorageCheckOut.Clear();
                                    if(GettedItems != null)
                                    {
                                        if (SelectedMedicalFileType.LookupID == (long)AllLookupValues.V_MedicalFileType.Noi_Tru)
                                        {
                                            foreach (var item in GettedItems)
                                            {
                                                item.ListPtRegistration = new List<PatientRegistration>();
                                                PatientRegistration registration = new PatientRegistration
                                                {
                                                    PtRegistrationID = item.PtRegistrationID,
                                                    ExamDate = item.ExamDate,
                                                    DischargeDate = item.DischargeDate
                                                };
                                                item.ListPtRegistration.Add(registration);
                                                AllPatientMedicalFileStorageCheckOut.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in GettedItems.GroupBy(x => x.OutPtTreatmentProgramID).Select(y => y.FirstOrDefault()))
                                            {
                                                item.ListPtRegistration = new List<PatientRegistration>();
                                                foreach (var lsPtRegistration in GettedItems.Where(x => x.OutPtTreatmentProgramID == item.OutPtTreatmentProgramID))
                                                {
                                                    PatientRegistration registration = new PatientRegistration
                                                    {
                                                        PtRegistrationID = lsPtRegistration.PtRegistrationID,
                                                        ExamDate = lsPtRegistration.ExamDate,
                                                        DischargeDate = lsPtRegistration.DischargeDate
                                                    };
                                                    item.ListPtRegistration.Add(registration);
                                                }
                                                AllPatientMedicalFileStorageCheckOut.Add(item);
                                            }
                                        }
                                    }

                                    AllPatientMedicalFileStorageCheckOut.PageSize = PageSize;
                                    AllPatientMedicalFileStorageCheckOut.PageIndex = PageIndex;
                                    AllPatientMedicalFileStorageCheckOut.TotalItemCount = TotalRecord;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    NotifyOfPropertyChange(() => IsEnableMedicalFileInfo);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private string _PatientName;
        public string PatientName
        {
            get
            {
                return _PatientName;
            }
            set
            {
                if (_PatientName != value)
                {
                    _PatientName = value;
                    NotifyOfPropertyChange(() => PatientName);
                }
            }
        }
        private string _PatientCode;
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                if (_PatientCode != value)
                {
                    _PatientCode = value;
                    NotifyOfPropertyChange(() => PatientCode);
                }
            }
        }
        public void grd_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void hplMedicalFileInfo_Click(object sender)
        {
            var curMedicalFile = sender as PatientMedicalFileStorageCheckInCheckOut;
            if (curMedicalFile == null || curMedicalFile.OutPtTreatmentProgramID == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                aView.eItem = ReportName.GeneralOutPtMedicalFile;
                aView.OutPtTreatmentProgramID = curMedicalFile.OutPtTreatmentProgramID;
            });
        }
        public bool IsEnableMedicalFileInfo
        {
            get
            {
                return SelectedMedicalFileType.LookupID == (long)AllLookupValues.V_MedicalFileType.DT_Ngoai_Tru;
            }
        }
        public void hplPreview_Click(object sender)
        {
            var mRefShelfDetails = sender as PatientRegistration;
            if (mRefShelfDetails == null)
            {
                return;
            }
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = mRefShelfDetails.PtRegistrationID;
                if(SelectedMedicalFileType.LookupID == (long)AllLookupValues.V_MedicalFileType.Noi_Tru)
                {
                    proAlloc.eItem = ReportName.TEMP12_TONGHOP;
                    proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                }
                else
                {
                    proAlloc.eItem = ReportName.TEMP12;
                }
                
                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog(onInitDlg, null, false, true, new Size(1500, 1000));
        }

        private void AddFirstItemRefRow()
        {
            RefRows firstItem = new RefRows();
            firstItem.RefRowID = 0;
            firstItem.RefRowName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3283_G1_ChonDay);
            AllRefRows.Insert(0, firstItem);
            SelectedRow = AllRefRows.FirstOrDefault();
        }
        private void AddFirstItemRefShelf()
        {
            RefShelves firstItem = new RefShelves();
            firstItem.RefShelfID = 0;
            firstItem.RefShelfName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3284_G1_ChonKe);
            AllRefShelfs.Insert(0, firstItem);
            SelectedRefShelf = AllRefShelfs.FirstOrDefault();
        }
        private void AddFirstItemRefShelfDetails()
        {
            RefShelfDetails firstItem = new RefShelfDetails();
            firstItem.RefShelfDetailID = 0;
            firstItem.LocName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3285_G1_ChonNgan);
            AllRefShelfDetails.Insert(0, firstItem);
            SelectedRefShelfDetail = AllRefShelfDetails.FirstOrDefault();
        }
        public void Expander_Process(object sender)
        {
            if (sender == null)
            {
                return;
            }
            var row = DataGridRow.GetRowContainingElement((FrameworkElement)sender);
            if (row != null)
            {
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
