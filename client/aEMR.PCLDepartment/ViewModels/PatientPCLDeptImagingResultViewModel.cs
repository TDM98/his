using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using PCLsService;
using System.Windows.Media.Imaging;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.CommonTasks;
using eHCMSLanguage;
using Microsoft.Win32;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
* 20170414 #001 CMN: Added PCLImagingResult in Save Button
* 20170512 #002 CMN: Added DeptLocation to PCLImagingResult
* 20180613 #003 TBLD: Added HIRepResourceCode to PatientPCLImagingResults
* 20182508 #004 TTM: Vì đây là viewmodel NonShared nếu không có OnActive và OnDeActive thì xe rác sẽ không hốt kịp => Chụp, bắn sự kiện liên tục 
* 20182708 #005 TTM: Tạo mới hàm GetImageInAnotherViewModel, lý do: Vì bên các ViewModel siêu âm không còn bắn sự kiện ReaderInfoPatientFromPatientPCLReqEvent nữa nên không thể chụp sự kiện để load 
*                    hình ảnh => Dùng hàm mới này để load.
* 20181122 #006 TBL: BM 0005301: Luu NumberOfFilmsReceived theo DefaultNumFilmsRecv      
* 20230607 #007 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm           
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IPatientPCLDeptImagingResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLDeptImagingResultViewModel : ViewModelBase, IPatientPCLDeptImagingResult
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream>>
    {
        #region Indicator Member

        private bool _isLoadingConfig = false;
        public bool isLoadingConfig
        {
            get { return _isLoadingConfig; }
            set
            {
                if (_isLoadingConfig != value)
                {
                    _isLoadingConfig = value;
                    NotifyOfPropertyChange(() => isLoadingConfig);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingParent = false;
        public bool isLoadingParent
        {
            get { return _isLoadingParent; }
            set
            {
                if (_isLoadingParent != value)
                {
                    _isLoadingParent = value;
                    NotifyOfPropertyChange(() => isLoadingParent);
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

        private bool _isLoadingOperator = false;
        public bool isLoadingOperator
        {
            get { return _isLoadingOperator; }
            set
            {
                if (_isLoadingOperator != value)
                {
                    _isLoadingOperator = value;
                    NotifyOfPropertyChange(() => isLoadingOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        public bool IsLoading
        {
            get { return (isLoadingParent || isLoadingDetail || isLoadingOperator || isLoadingConfig); }
        }

        #endregion

        private string ImagePool;
        private string ImageStore;
        private string ImageThumbTemp;
        private string _FolderListTextSelect;
        private ObservableCollection<String> _Folders;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPCLDeptImagingResultView;
        }
        IPCLDeptImagingResultView _currentView;

        /// <summary>
        /// Các Biến Giá Trị
        /// </summary>

        private ObservableCollection<PCLResultFileStorageDetailClient> _ObjGetPCLResultFileStoreDetails;
        private ObservableCollection<PCLExamType> _ObjPCLExamTypes_ByPatientPCLReqID;
        private ObservableCollection<PatientPCLRequest> _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory;
        private int _OptionValue;
        private PCLResultFileStorageDetailClient _PCLResultFileStorageDetailSelected;
        private long _V_ResultType;
        private string _cboListFolderSelectedValue;
        //==== 20161013 CMN Begin: Add PCL Image Method
        public int _TotalFile = 0;
        public int TotalFile
        {
            get { return ObjGetPCLResultFileStoreDetails.Count; }
        }
        //==== 20161013 CMN End.
        //==== 20161007 CMN Begin: Add List for delete file
        public List<PCLResultFileStorageDetail> _FileForStore = new List<PCLResultFileStorageDetail>();
        public List<PCLResultFileStorageDetail> FileForStore
        {
            get { return ObjGetPCLResultFileStoreDetails.Where(x => x.ObjectResult.PCLImgResultID == null).Select(x => x.ObjectResult).ToList(); }
            set
            {
                if (_FileForStore != value)
                {
                    _FileForStore = value;

                    NotifyOfPropertyChange(() => FileForStore);
                }
            }
        }
        public List<PCLResultFileStorageDetail> _FileForDelete = new List<PCLResultFileStorageDetail>();
        public List<PCLResultFileStorageDetail> FileForDelete
        {
            get { return _FileForDelete; }
            set
            {
                if (_FileForDelete != value)
                {
                    _FileForDelete = value;

                    NotifyOfPropertyChange(() => FileForDelete);
                }
            }
        }
        public List<PCLResultFileStorageDetail> FileForUpdate
        {
            get { return ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Any(x => x.ObjectResult.PCLImgResultID.GetValueOrDefault(0) > 0) ? ObjGetPCLResultFileStoreDetails.Where(x => x.ObjectResult.PCLImgResultID.GetValueOrDefault(0) > 0).Select(x => x.ObjectResult).ToList() : null; }
        }
        //==== 20161007 CMN End.


        #region Property Enable

        private bool _CtrIsEnabled;

        private bool _HasPatient;

        public bool CtrIsEnabled
        {
            get { return _CtrIsEnabled && IsEnableButton; }
            set
            {
                if (_CtrIsEnabled != value)
                {
                    _CtrIsEnabled = value;
                    NotifyOfPropertyChange(() => CtrIsEnabled);
                }
            }
        }

        public bool HasPatient
        {
            get { return _HasPatient; }
            set
            {
                if (_HasPatient != value)
                {
                    _HasPatient = value;
                    NotifyOfPropertyChange(() => HasPatient);
                }
            }
        }

        #endregion
        [ImportingConstructor]
        public PatientPCLDeptImagingResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Initialize();
        }

        //▼===== #004
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        //▲===== #004
        public void Initialize()
        {

            //KMx: Sau khi kiểm tra, thấy View này không sử dụng View IPatientMedicalRecords_ByPatientID (25/05/2014 14:48).
            //var uc3 = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            //UCHeaderInfoPMR = uc3;
            //ActivateItem(uc3);
            //Load UC
            ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = new ObservableCollection<PatientPCLRequest>();
            ObjPCLExamTypes_ByPatientPCLReqID = new ObservableCollection<PCLExamType>();
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();

            FolderListTextSelect = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);

            GetTestingAgency_All();

            CheckFormStartValid();

            KhoiTao();

            GetAppConfigValue();

            //if (Globals.PatientPCLRequest_Imaging != null && Globals.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            //{
            //    LoadData(Globals.PatientPCLRequest_Imaging);
            //}
            /*▼====: #003*/
            GetResourcesForMedicalServicesListByTypeID(Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID);
            /*▲====: #003*/
        }

        public ObservableCollection<PatientPCLRequest> ObjPatientPCLRequest_ByPtRegIDV_PCLCategory
        {
            get { return _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory; }
            set
            {
                _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_ByPtRegIDV_PCLCategory);
            }
        }

        public ObservableCollection<PCLExamType> ObjPCLExamTypes_ByPatientPCLReqID
        {
            get { return _ObjPCLExamTypes_ByPatientPCLReqID; }
            set
            {
                _ObjPCLExamTypes_ByPatientPCLReqID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_ByPatientPCLReqID);
            }

        }

        public int GetNumOfImageResultFiles()
        {
            if (ObjGetPCLResultFileStoreDetails != null)
                return ObjGetPCLResultFileStoreDetails.Count();
            return 0;
        }

        public string GetImageResultFileStoragePath(int nIdx)
        {
            if (ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Count() > 0 && (nIdx < ObjGetPCLResultFileStoreDetails.Count()))
            {
                return ObjGetPCLResultFileStoreDetails[nIdx].ObjectResult.FullPath;
            }
            return "";
        }

        public ObservableCollection<PCLResultFileStorageDetailClient> ObjGetPCLResultFileStoreDetails
        {
            get { return _ObjGetPCLResultFileStoreDetails; }
            set
            {
                _ObjGetPCLResultFileStoreDetails = value;
                NotifyOfPropertyChange(() => ObjGetPCLResultFileStoreDetails);
            }
        }

        public string FolderListTextSelect
        {
            get { return _FolderListTextSelect; }
            set
            {
                if (_FolderListTextSelect != value)
                {
                    _FolderListTextSelect = value;
                    NotifyOfPropertyChange(() => FolderListTextSelect);
                }
            }
        }

        public ObservableCollection<String> FolderList
        {
            get { return _Folders; }
            set
            {
                if (_Folders != value)
                {
                    _Folders = value;
                    //NotifyOfPropertyChange(() => FolderList);
                }
            }
        }

        public int OptionValue
        {
            get { return _OptionValue; }
            set
            {
                if (_OptionValue != value)
                {
                    _OptionValue = value;
                    NotifyOfPropertyChange(() => OptionValue);
                }
            }
        }

        public string cboListFolderSelectedValue
        {
            get { return _cboListFolderSelectedValue; }
            set
            {
                if (_cboListFolderSelectedValue != value)
                {
                    _cboListFolderSelectedValue = value;
                    NotifyOfPropertyChange(() => cboListFolderSelectedValue);
                }
            }
        }

        public long V_ResultType
        {
            get { return _V_ResultType; }
            set
            {
                if (_V_ResultType != value)
                {
                    _V_ResultType = value;
                    NotifyOfPropertyChange(() => V_ResultType);
                }
            }
        }

        public PCLResultFileStorageDetailClient PCLResultFileStorageDetailSelected
        {
            get { return _PCLResultFileStorageDetailSelected; }
            set
            {
                if (_PCLResultFileStorageDetailSelected != value)
                {
                    _PCLResultFileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => PCLResultFileStorageDetailSelected);
                }
            }
        }

        #region IHandle<btChooseFileResultForPCL_Click<PCLResultFileStorageDetail,int,Stream>> Members

        public void Handle(btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream> message)
        {
            if (message != null)
            {
                message.File.Flag = 1;
                AddFile(message);
            }
        }

        private void AddFile(btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream> message)
        {
            try
            {
                //ObjGetPCLResultFileStoreDetails.Add(message.File);
                //CheckKetQuaReturn(message.StreamFile, message.TypeFile);
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A1024_G1_Msg_InfoThemFileFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public int GetType(object item)
        {
            int type = 0;
            if (item != null)
            {
                PCLResultFileStorageDetailClient p = item as PCLResultFileStorageDetailClient;
                if (p.ObjectResult.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
                {
                    type = 1;
                }
                else if (p.ObjectResult.V_ResultType == (long)AllLookupValues.FileStorageResultType.VIDEO_RECORDING)
                {
                    type = 2;
                }
            }
            return type;
        }
        //==== 20161008 CMN Begin: Change Image View to WriteableBitmap
        //private BitmapImage _ObjBitmapImage;
        //public BitmapImage ObjBitmapImage
        private WriteableBitmap _ObjBitmapImage;
        public WriteableBitmap ObjBitmapImage
        {
            get { return _ObjBitmapImage; }
            set
            {
                if (_ObjBitmapImage != value)
                {
                    _ObjBitmapImage = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage);
                }
            }
        }
        //==== 20161008 CMN End.

        private Stream _VideoAndImageStream;
        public Stream VideoAndImageStream
        {
            get { return _VideoAndImageStream; }
            set
            {
                _VideoAndImageStream = value;
                NotifyOfPropertyChange(() => VideoAndImageStream);
            }
        }

        private Visibility _ControlVideoVisibility;
        public Visibility ControlVideoVisibility
        {
            get { return _ControlVideoVisibility; }
            set
            {
                if (_ControlVideoVisibility != value)
                {
                    _ControlVideoVisibility = value;
                    NotifyOfPropertyChange(() => ControlVideoVisibility);
                }
            }
        }
        private Visibility _ControlImgVisibility;
        public Visibility ControlImgVisibility
        {
            get { return _ControlImgVisibility; }
            set
            {
                if (_ControlImgVisibility != value)
                {
                    _ControlImgVisibility = value;
                    NotifyOfPropertyChange(() => ControlImgVisibility);
                }
            }
        }
        private void SetVisibleForVideoControls(Visibility pV)
        {
            ControlVideoVisibility = pV;
        }
        private void SetVisibleForImgControls(System.Windows.Visibility pV)
        {
            ControlImgVisibility = pV;
        }

        private void DisplayResultFile(Stream pStream, int itype)
        {
            VideoAndImageStream = pStream;

            if (pStream != null)
            {
                if (itype == 1)/*Images*/
                {
                    //==== 20161008 CMN Begin: Change Image View to WriteableBitmap
                    //System.Windows.Media.Imaging.BitmapImage imgsource = new System.Windows.Media.Imaging.BitmapImage();
                    //imgsource.SetSource(pStream);
                    //ObjBitmapImage = imgsource;
                    ObjBitmapImage = Globals.GetWriteableBitmapFromStream(pStream);
                    //==== 20161008 CMN End.
                    SetVisibleForVideoControls(Visibility.Collapsed);
                    SetVisibleForImgControls(Visibility.Visible);
                }
                else if (itype == 2)/*Video*/
                {
                    SetVisibleForVideoControls(Visibility.Visible);
                    SetVisibleForImgControls(Visibility.Collapsed);

                    if (_currentView != null)
                    {
                        _currentView.SetObjectSource(pStream);
                    }

                }
                else/*Document*/
                {
                    SetVisibleForVideoControls(Visibility.Collapsed);
                    SetVisibleForImgControls(Visibility.Collapsed);
                }
            }
            else
            {
                SetVisibleForVideoControls(Visibility.Collapsed);
                SetVisibleForImgControls(Visibility.Collapsed);
            }
        }
        #endregion

        //#region IHandle<ItemSelected<PatientRegistration>> Members

        //public void Handle(ItemSelected<PatientRegistration> message)
        //{
        //    if (message != null)
        //    {
        //        PatientPCLRequest_ByPtRegIDV_PCLCategory(message.Item.PtRegistrationID, 27001); /*PCLImaging*/
        //    }
        //}

        //#endregion

        #region IPCLDeptImagingResult Members

        //KMx: Sau khi kiểm tra, thấy biến này không được sử dụng nữa (25/05/2014 14:48).
        //public object UCHeaderInfoPMR { get; set; }

        #endregion

        private void PatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Yêu Cầu..." });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new PCLsImportClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        contract.BeginPatientPCLRequest_ByPtRegIDV_PCLCategory(PtRegistrationID, V_PCLCategory, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var items = contract.EndPatientPCLRequest_ByPtRegIDV_PCLCategory(asyncResult);
            //                if (items != null)
            //                {
            //                    ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = new ObservableCollection<DataEntities.PatientPCLRequest>(items);


            //                    //Item Default
            //                    DataEntities.PatientPCLRequest ItemDefault = new DataEntities.PatientPCLRequest();
            //                    ItemDefault.PatientPCLReqID = -1;
            //                    ItemDefault.Diagnosis = "--Chọn Phiếu--";
            //                    ObjPatientPCLRequest_ByPtRegIDV_PCLCategory.Insert(0, ItemDefault);
            //                    //Item Default

            //                }
            //                else
            //                {
            //                    ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = null;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                Globals.IsBusy = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }

        public void cboPMRRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var Objtmp = ((sender as ComboBox).SelectedItem as PatientPCLRequest);
                if (Objtmp.PatientPCLReqID > 0)
                {
                    PCLExamTypes_ByPatientPCLReqID(Objtmp.PatientPCLReqID);
                }
                else
                {
                    ObjPCLExamTypes_ByPatientPCLReqID.Clear();
                    ObjPatientPCLImagingResult.PCLExamTypeID = -1;
                    ObjGetPCLResultFileStoreDetails.Clear();
                }
            }
        }

        private void PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Yêu Cầu..." });

            var t = new Thread(() =>
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        IPCLsImport contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLExamTypes_ByPatientPCLReqID(PatientPCLReqID, Globals.DispatchCallback(
                            (asyncResult) =>
                            {
                                try
                                {
                                    IList<PCLExamType> items = contract.EndPCLExamTypes_ByPatientPCLReqID(asyncResult);
                                    if (items != null)
                                    {
                                        ObjPCLExamTypes_ByPatientPCLReqID = new ObservableCollection<PCLExamType>(items);
                                        //Item Default
                                        var ItemDefault = new PCLExamType();
                                        ItemDefault.PCLExamTypeID = -1;
                                        ItemDefault.PCLExamTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2033_G1_ChonLoaiXN2);
                                        ObjPCLExamTypes_ByPatientPCLReqID.Insert(0, ItemDefault);
                                        //Item Default
                                    }
                                    else
                                    {
                                        ObjPCLExamTypes_ByPatientPCLReqID = null;
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

        public void cboPCLExamType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var Objtmp = ((sender as ComboBox).SelectedItem as PCLExamType);
                if (Objtmp.PCLExamTypeID > 0)
                {
                    Coroutine.BeginExecute(LoadDataCoroutine_NotInit(ObjPatientPCLImagingResult.PatientPCLRequest.PatientID,
                                                 ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault(),
                                                 ObjPatientPCLImagingResult.PCLExamTypeID));
                }
                else
                {
                    ObjGetPCLResultFileStoreDetails.Clear();
                }
            }
        }


        private void GetAppConfigValue()
        {
            //ImagePool = Globals.ConfigList[(int)ConfigItemKey.PCLResourcePool];
            //ImageStore = Globals.ConfigList[(int)ConfigItemKey.PCLStorePool];
            //ImageThumbTemp = Globals.ConfigList[(int)ConfigItemKey.PCLThumbTemp];

            // Txd 25/05/2014 Replaced ConfigList
            ImagePool = Globals.ServerConfigSection.Hospitals.PCLResourcePool;
            ImageStore = Globals.ServerConfigSection.Hospitals.PCLStorePool;
            ImageThumbTemp = Globals.ServerConfigSection.Hospitals.PCLThumbTemp;

            GetFolderList(ImagePool);
        }

        private void GetFolderList(string Path)
        {
            // Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = "FolderList..."});
            isLoadingConfig = true;
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsImportClient())
                                       {
                                           IPCLsImport contract = serviceFactory.ServiceInstance;

                                           contract.BeginGetFolderList(Path, Globals.DispatchCallback((asyncResult) =>
                                            {
                                                try
                                                {
                                                    IList<string> items = contract.EndGetFolderList(asyncResult);
                                                    if (items != null)
                                                    {
                                                        FolderList = new ObservableCollection<string>(items);
                                                        //Item Default
                                                        string ItemDefault = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                                        FolderList.Insert(0, ItemDefault);
                                                        //Item Default
                                                    }
                                                    else
                                                    {
                                                        FolderList = null;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    isLoadingConfig = false;
                                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                                }
                                                finally
                                                {
                                                    isLoadingConfig = false;
                                                    // Globals.IsBusy=false;
                                                }
                                            }), null);
                                       }
                                   });
            t.Start();
        }

        public void gOption0_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                OptionValue = 1;
                cboListFolderSelectedValue = "Images";
                V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
            }
        }

        public void gOption1_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                OptionValue = 2;
                cboListFolderSelectedValue = "Videos";
                V_ResultType = (long)AllLookupValues.FileStorageResultType.VIDEO_RECORDING;
            }
        }

        public void gOption2_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                OptionValue = 3;
                cboListFolderSelectedValue = "Documents";
                V_ResultType = (long)AllLookupValues.FileStorageResultType.DOCUMENTS;
            }
        }

        public void cboListFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                cboListFolderSelectedValue = (sender as ComboBox).SelectedItem.ToString();
            }
        }



        private bool IsValidImage(string FullName)
        {
            var allowedExtensions = new[] { ".png", ".gif", ".jpg" };
            var fileName = Path.GetFileName(FullName);
            var extension = Path.GetExtension(fileName);
            if (allowedExtensions.Contains(extension))
            {
                return true;
            }
            return false;
        }



        public void btnBrowse()
        {
            if (ObjPatientPCLImagingResult == null || ObjPatientPCLImagingResult.PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0380_G1_ChonPhYC);
                return;
            }

            //if (!string.IsNullOrEmpty(FolderListTextSelect))
            //{
            //if (FolderListTextSelect != string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon))
            //{
            //var typeInfo = Globals.GetViewModel<IFileExplorer>();
            //typeInfo.dir = ImagePool + @"\" + cboListFolderSelectedValue;
            //typeInfo.itype = OptionValue;
            //typeInfo.InitPCLResultFileStorageDetail(cboListFolderSelectedValue, V_ResultType);

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //                                 {
            //                                     //lam gi do
            //                                 });


            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            if (V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
            {
                //dlg.Filter = "All files (*.*)|*.*|PNG Images (*.png)|*.png";
                //20221019 BLQ: Thêm jpeg vào bộ lọc và All file để có thẽ chọn nhiều file khác  
                dlg.Filter = "Images (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            }
            else
            {
                dlg.Filter = "Media Files|*.mp4;*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3;*.wmv";
                //openFileDialog.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3|All Files|*.*";
            }

            bool? retval = dlg.ShowDialog();

            if (retval != null && retval == true)
            {
                //ChangedWPF-CMN
                //Stream stream = (Stream)dlg.File.OpenRead();
                Stream stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                PCLResultFileStorageDetailClient itemClient = new PCLResultFileStorageDetailClient();
                itemClient.IOStream = stream;
                itemClient.ObjectResult = new PCLResultFileStorageDetail();
                itemClient.ObjectResult.PCLResultFileName = dlg.FileName;
                itemClient.ObjectResult.IsImage = true;
                itemClient.ObjectResult.File = bytes;
                itemClient.ObjectResult.PCLResultLocation = cboListFolderSelectedValue;
                itemClient.ObjectResult.V_ResultType = V_ResultType;
                itemClient.ObjectResult.FullPath = ImageStore + @"\" + cboListFolderSelectedValue;
                int index = 0;
                if (dlg.FileName.LastIndexOf(".") > 0)
                {
                    index = dlg.FileName.LastIndexOf(".");
                }
                //itemClient.ObjectResult.PCLResultFileName = Globals.PatientAllDetails.PatientInfo.PatientID.ToString() + "-" + Guid.NewGuid().ToString() +
                //                     dlg.File.Name.Substring(index, dlg.File.Name.Length - index);

                itemClient.ObjectResult.PCLResultFileName = Guid.NewGuid().ToString();

                ObjGetPCLResultFileStoreDetails.Add(itemClient);
                // SendFile(dlg.File.Name, bytes, false, ImagePool + @"\" + cboListFolderSelectedValue);
            }
            else
            {
            }
            //}
            //else
            //{
            //    MessageBox.Show(eHCMSResources.A0291_G1_Msg_CDinhThuMucChuaFile, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //}
            //}
        }


        void SendFile(string name, byte[] contents, bool append, string dir)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDoUpload(name, contents, append, dir, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Result = contract.EndDoUpload(asyncResult);
                            MessageBox.Show(eHCMSResources.K0258_G1_Uploaded);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void chkExternalExam_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as CheckBox);
            if (Ctr.IsChecked.Value)
            {
                CtrIsEnabled = true;
            }
            else
            {
                CtrIsEnabled = false;
            }
        }

        #region Thông tin bệnh viện ngoài

        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;

        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get { return _ObjTestingAgencyList; }
            set
            {
                if (_ObjTestingAgencyList != value)
                {
                    _ObjTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }

        private void GetTestingAgency_All()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Bệnh Viện Ngoài..." });

            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsImportClient())
                                       {
                                           IPCLsImport contract = serviceFactory.ServiceInstance;

                                           contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                                                                                                           {
                                                                                                               try
                                                                                                               {
                                                                                                                   IList
                                                                                                                       <
                                                                                                                           TestingAgency
                                                                                                                           >
                                                                                                                       items
                                                                                                                           =
                                                                                                                           contract
                                                                                                                               .
                                                                                                                               EndGetTestingAgency_All
                                                                                                                               (asyncResult);
                                                                                                                   if (
                                                                                                                       items !=
                                                                                                                       null)
                                                                                                                   {
                                                                                                                       ObjTestingAgencyList
                                                                                                                           =
                                                                                                                           new ObservableCollection
                                                                                                                               <
                                                                                                                                   TestingAgency
                                                                                                                                   >
                                                                                                                               (items);

                                                                                                                       //Item Default
                                                                                                                       var
                                                                                                                           ItemDefault
                                                                                                                               =
                                                                                                                               new TestingAgency
                                                                                                                                   ();
                                                                                                                       ItemDefault
                                                                                                                           .
                                                                                                                           AgencyID
                                                                                                                           =
                                                                                                                           -1;
                                                                                                                       ItemDefault
                                                                                                                           .
                                                                                                                           AgencyName
                                                                                                                           =
                                                                                                                           "--Chọn--";
                                                                                                                       ObjTestingAgencyList
                                                                                                                           .
                                                                                                                           Insert
                                                                                                                           (0,
                                                                                                                            ItemDefault);
                                                                                                                       //Item Default
                                                                                                                   }
                                                                                                                   else
                                                                                                                   {
                                                                                                                       ObjTestingAgencyList
                                                                                                                           =
                                                                                                                           null;
                                                                                                                   }
                                                                                                               }
                                                                                                               catch (
                                                                                                                   Exception
                                                                                                                       ex
                                                                                                                   )
                                                                                                               {
                                                                                                                   Globals
                                                                                                                       .
                                                                                                                       ShowMessage
                                                                                                                       (ex
                                                                                                                            .
                                                                                                                            Message,
                                                                                                                        eHCMSResources.T0432_G1_Error);
                                                                                                               }
                                                                                                               finally
                                                                                                               {
                                                                                                                   Globals
                                                                                                                       .
                                                                                                                       IsBusy
                                                                                                                       =
                                                                                                                       false;
                                                                                                               }
                                                                                                           }), null);
                                       }
                                   });
            t.Start();
        }

        #endregion

        #region InitObject

        private PatientPCLImagingResult _ObjPatientPCLImagingResult;

        public PatientPCLImagingResult ObjPatientPCLImagingResult
        {
            get { return _ObjPatientPCLImagingResult; }
            set
            {
                if (_ObjPatientPCLImagingResult != value)
                {
                    _ObjPatientPCLImagingResult = value;

                    /*==== #002 ====*/
                    ObjPatientPCLImagingResult.DeptLocationID = Globals.DeptLocation != null ? (long?)Globals.DeptLocation.DeptLocationID : null;
                    /*==== #002 ====*/
                    
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult);
                }
            }
        }


        private void CheckFormStartValid()
        {
            CtrIsEnabled = false;
            if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.PatientPCLRequest != null && ObjPatientPCLImagingResult.PatientPCLRequest.PatientID > 0)
            {
                HasPatient = true;
            }
            else
            {
                HasPatient = false;
            }
        }

        private void KhoiTao()
        {
            // Already created in SieuAmTim VM
            //ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            //==== 20160710 CMN Begin:Already created in SieuAmTim VM
            //ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            //ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            //ObjPatientPCLImagingResult.PCLExamTypeID = -1;

            //ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            //ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            //ObjPatientPCLImagingResult.IsExternalExam = false;
            //==== 20160710 CMN End.
            OptionValue = 1;

            PCLResultFileStorageDetailSelected = new PCLResultFileStorageDetailClient();
            cboListFolderSelectedValue = "Images";
            V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
        }

        #endregion

        public void LoadData(PatientPCLRequest message)
        {
            Coroutine.BeginExecute(LoadDataCoroutine(message));
        }

        private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest p)
        {
            //isLoadingDetail = true;

            if (ObjPatientPCLImagingResult == null)
            {
                KhoiTao();
            }
            ObjPatientPCLImagingResult.PatientPCLRequest = p;

            ObjPatientPCLImagingResult.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            ObjPatientPCLImagingResult.PatientPCLReqID = p.PatientPCLReqID;
            ObjPatientPCLImagingResult.PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault();
            ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
            {
                PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault(),
                PCLExamTypeName = p.PCLExamTypeName,
                V_PCLMainCategory = p.V_PCLMainCategory,
                V_ReportForm = p.V_ReportForm,
                PCLExamTypeTemplateResult = string.IsNullOrEmpty(p.PCLExamTypeTemplateResult)? "": p.PCLExamTypeTemplateResult.ToUpper()
            };
            //==== 20161007 CMN Begin: Add List for delete file
            FileForDelete = new List<PCLResultFileStorageDetail>();
            //==== 20161007 CMN End.
            /*▼====: #003*/
            //var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(p.PatientID, p.PatientPCLReqID, p.PCLExamTypeID);
            var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(p.PatientID, p.PatientPCLReqID, p.PCLExamTypeID, (long)p.V_PCLRequestType);
            /*▲====: #003*/
            yield return ResultFileStoreDetails;
            if (ObjGetPCLResultFileStoreDetails == null)
            {
                ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
            }
            ObjGetPCLResultFileStoreDetails.Clear();
            if (ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails != null)
            {
                foreach (var item in ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails)
                {
                    ObjGetPCLResultFileStoreDetails.Add(new PCLResultFileStorageDetailClient { IOStream = null, ObjectResult = item });
                }
            }
            /*▼====: #003*/
            //var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(p.PatientPCLReqID, p.PCLExamTypeID.GetValueOrDefault());
            var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(p.PatientPCLReqID, p.PCLExamTypeID.GetValueOrDefault(), (long)p.V_PCLRequestType);
            /*▲====: #003*/
            yield return LoadPCLImagingResultsID;
            if (LoadPCLImagingResultsID.ObjPatientPCLImagingResult != null && LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault() > 0)
            {
                ObjPatientPCLImagingResult.PCLImgResultID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLImgResultID;
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.DiagnoseOnPCLExam;
                ObjPatientPCLImagingResult.ResultExplanation = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultExplanation;
                ObjPatientPCLImagingResult.ICD10List = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ICD10List;
                ObjPatientPCLImagingResult.Height = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Height;
                ObjPatientPCLImagingResult.Weight = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Weight;
                /*▼====: #003*/
                ObjPatientPCLImagingResult.HIRepResourceCode = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.HIRepResourceCode;
                /*▲====: #003*/
                ObjPatientPCLImagingResult.TemplateResultString = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResultString;
                //==== #006 ====
                //TBL: Lay NumberOfFilmsReceived cua ket qua da luu
                ObjPatientPCLImagingResult.NumberOfFilmsReceived = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.NumberOfFilmsReceived;
                //==== #006 ====
                ObjPatientPCLImagingResult.PtRegistrationCode = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PtRegistrationCode;
                ObjPatientPCLImagingResult.Staff = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Staff;
                //20181207 TTM: Load thêm Ngày thực hiện, ID bác sĩ thực hiện, họ tên bác sĩ thực hiện.
                ObjPatientPCLImagingResult.PerformedDate = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformedDate;
                ObjPatientPCLImagingResult.PerformStaffID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformStaffID;
                ObjPatientPCLImagingResult.PerformStaffFullName = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformStaffFullName;
                ObjPatientPCLImagingResult.PatientPCLRequest.ResultDate = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLExamDate;
                ObjPatientPCLImagingResult.Suggest = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Suggest;
                ObjPatientPCLImagingResult.ResultStaffID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultStaffID;
                ObjPatientPCLImagingResult.ResultStaffFullName = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultStaffFullName;
                ObjPatientPCLImagingResult.TemplateResult = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResult;
                //▼===== #007
                ObjPatientPCLImagingResult.SpecimenID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.SpecimenID;
                if (ObjPatientPCLImagingResult.PatientPCLRequest != null)
                {
                    ObjPatientPCLImagingResult.PatientPCLRequest.ReceptionTime = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLRequest.ReceptionTime;
                }
                //▲===== #007
            }
            else
            {
                if (ObjPatientPCLImagingResult != null)
                {
                    ObjPatientPCLImagingResult.PCLImgResultID = 0;
                    ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
                    ObjPatientPCLImagingResult.ResultExplanation = "";
                    ObjPatientPCLImagingResult.ICD10List = null;
                    ObjPatientPCLImagingResult.Height = 0;
                    ObjPatientPCLImagingResult.Weight = 0;
                    //==== #006 ====
                    //TBL: Khi chua luu cls thi lay DefaultNumFilmsRecv set cho NumberOfFilmsReceived
                    ObjPatientPCLImagingResult.NumberOfFilmsReceived = ObjPatientPCLImagingResult.PatientPCLRequest.DefaultNumFilmsRecv;
                    //==== #006 ====
                    ObjPatientPCLImagingResult.TemplateResultString = null;
                    ObjPatientPCLImagingResult.PerformStaffID = null;
                    ObjPatientPCLImagingResult.PerformStaffFullName = null;
                    ObjPatientPCLImagingResult.Suggest = null;
                    ObjPatientPCLImagingResult.ResultStaffID = null;
                    ObjPatientPCLImagingResult.ResultStaffFullName = null;
                    //▼===== #007
                    ObjPatientPCLImagingResult.SpecimenID = 0;
                    //▲===== #007
                }
            }
            Globals.EventAggregator.Publish(new LoadPatientPCLImagingResultDataCompletedEvent());
            //isLoadingDetail = false;
            yield break;

        }

        private IEnumerator<IResult> LoadDataCoroutine_NotInit(long patientID, long PatientPCLReqID, long? PCLExamTypeID)
        {
            isLoadingDetail = true;

            var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(patientID, PatientPCLReqID, PCLExamTypeID);
            yield return ResultFileStoreDetails;
            if (ObjGetPCLResultFileStoreDetails == null)
            {
                ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
            }
            ObjGetPCLResultFileStoreDetails.Clear();
            if (ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails != null)
            {
                foreach (var item in ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails)
                {
                    ObjGetPCLResultFileStoreDetails.Add(new PCLResultFileStorageDetailClient { IOStream = null, ObjectResult = item });
                }
            }
            var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(PatientPCLReqID, PCLExamTypeID.GetValueOrDefault());
            yield return LoadPCLImagingResultsID;
            if (LoadPCLImagingResultsID.ObjPatientPCLImagingResult != null && LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault() > 0)
            {
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.DiagnoseOnPCLExam;
                ObjPatientPCLImagingResult.ResultExplanation = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultExplanation;
            }
            else
            {
                if (ObjPatientPCLImagingResult != null)
                {
                    ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
                    ObjPatientPCLImagingResult.ResultExplanation = "";
                }
            }
            isLoadingDetail = false;
            yield break;

        }

        private PCLResultFileStorageDetailClient _ObjPCLResultFileStorageDetailSelected;
        public PCLResultFileStorageDetailClient ObjPCLResultFileStorageDetailSelected
        {
            get
            {
                return _ObjPCLResultFileStorageDetailSelected;
            }
            set
            {
                if (_ObjPCLResultFileStorageDetailSelected != value)
                {
                    _ObjPCLResultFileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => ObjPCLResultFileStorageDetailSelected);
                }
            }
        }

        public void lnkView_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLResultFileStorageDetailSelected != null)
            {
                int iType = GetType(ObjPCLResultFileStorageDetailSelected);
                //==== 20161010 CMN Begin: Save stream to file for view
                //if (ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileItemID > 0)
                if (ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileItemID > 0 && ObjPCLResultFileStorageDetailSelected.IOStream == null)
                {
                    //20161006 CMN Begin: Change for new SaveFile Method
                    //ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath = ImageStore + @"\" + ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultLocation + @"\" + ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileName;
                    ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath = Path.Combine(ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultLocation, ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileName);
                    //20161006 CMN End
                    GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath, iType);
                }
                //==== 20161010 CMN End.
                else
                {
                    DisplayResultFile(ObjPCLResultFileStorageDetailSelected.IOStream, iType);
                }
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLResultFileStorageDetailSelected != null)
            {
                //==== 20161007 CMN Begin: Add List for delete file
                FileForDelete.Add(ObjPCLResultFileStorageDetailSelected.ObjectResult);
                //==== 20161007 CMN End.
                ObjGetPCLResultFileStoreDetails.Remove(ObjPCLResultFileStorageDetailSelected);
            }
        }

        #region Control media
        public void btPlay()
        {
            if (_currentView != null)
            {
                _currentView.btPlay();
            }
        }
        public void btPause()
        {
            if (_currentView != null)
            {
                _currentView.btPause();
            }
        }

        public void btStop()
        {
            if (_currentView != null)
            {
                _currentView.btStop();
            }
        }

        public void btMute(object sender, RoutedEventArgs e)
        {
            if (_currentView != null)
            {
                _currentView.btMute();
            }
        }

        public void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_currentView != null)
            {
                _currentView.btVolume_ValueChanged(sender, e);
            }
        }
        #endregion

        private Stream _ObjGetVideoAndImage;
        public Stream ObjGetVideoAndImage
        {
            get { return _ObjGetVideoAndImage; }
            set
            {
                if (_ObjGetVideoAndImage != value)
                {
                    _ObjGetVideoAndImage = value;
                    NotifyOfPropertyChange(() => ObjGetVideoAndImage);
                }
            }
        }

        private void GetVideoAndImage(string path, int itype)
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            //isLoadingOperator = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetVideoAndImage(asyncResult);
                            if (items != null && items.Length > 0)
                            {
                                ObjGetVideoAndImage = new MemoryStream(items);
                                //==== 20161010 CMN Begin: Save stream to file for view
                                ObjPCLResultFileStorageDetailSelected.IOStream = ObjGetVideoAndImage;
                                //==== 20161010 CMN End.
                                DisplayResultFile(ObjGetVideoAndImage, itype);
                            }
                            else
                            {
                                ObjGetVideoAndImage = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            //isLoadingOperator = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingOperator = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplDiagnoticsVideo()
        {
            if (VideoAndImageStream != null)
            {
                Action<IVideoDiagnosticExplorer> onInitDlg = delegate (IVideoDiagnosticExplorer typeInfo)
                {
                    typeInfo.VideoStream = VideoAndImageStream;
                    typeInfo.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
                };
                GlobalsNAV.ShowDialog<IVideoDiagnosticExplorer>(onInitDlg);
            }
        }

        public void hplDiagnoticsImg()
        {
            if (ObjBitmapImage != null)
            {
                Action<IImageDisgnosticExplorer> onInitDlg = delegate (IImageDisgnosticExplorer typeInfo)
                {
                    typeInfo.TypeOfBitmapImage = 1;
                    typeInfo.ObjWBitmapImage = ObjBitmapImage;
                };
                GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
            }
        }

        public void ClearBtn()
        {
            if (ObjPatientPCLImagingResult != null)
            {
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
                ObjPatientPCLImagingResult.ResultExplanation = "";
            }
            //20161109 CMN Begin: Add file to file for delete when clear all
            FileForDelete.AddRange(ObjGetPCLResultFileStoreDetails.Where(x => x.ObjectResult.PCLImgResultID != null).Select(x => x.ObjectResult).ToList());
            //20161109 CMN End.
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
            ObjBitmapImage = null;
            if (_currentView != null)
            {
                _currentView.SetObjectSource(null);
            }
        }

        public void btSave()
        {
            if (ObjPatientPCLImagingResult == null || ObjPatientPCLImagingResult.PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0380_G1_ChonPhYC);
                return;
            }
            //==== 20161005 CMN Begin: Combine Upload file
            //List<PCLResultFileStorageDetail> lst = ObjGetPCLResultFileStoreDetails.Select(x => x.ObjectResult).Where(x => x.PCLResultFileItemID <= 0).ToList();
            //if (lst != null && lst.Count > 0)
            //{
            //    SaveImageToStore(lst);
            //}
            //else
            //{
            //    SaveDatabase();
            //}
            //isLoadingOperator = true;
            if (ObjGetPCLResultFileStoreDetails.Count > Globals.ServerConfigSection.Pcls.MaxEchogramImageFile)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.K0457_G1_VuotQuaSLgFileToiDaChoPhep));
                return;
            }
            /*▼====: #003*/
            //Kiem tra da chon ma may chua
            if (ObjPatientPCLImagingResult.HIRepResourceCode == null && Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
            {
                MessageBox.Show(eHCMSResources.Z2242_G1_ChuaChonMaMay);
                return;
            }
            /*▲====: #003*/
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUploadImageToDatabase(ObjPatientPCLImagingResult, ObjGetPCLResultFileStoreDetails.Where(x => x.ObjectResult.PCLImgResultID == null).Select(x => x.ObjectResult).ToList(), FileForDelete, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndUploadImageToDatabase(asyncResult);
                                if (items)
                                {
                                    MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                    //20161006 CMN Begin: Reload Image Information
                                    LoadData(ObjPatientPCLImagingResult.PatientPCLRequest);
                                    //20161006 CMN End.
                                    Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
                                }
                            }
                            catch (Exception ex)
                            {
                                //isLoadingOperator = false;
                                //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                //isLoadingOperator = false;
                                //Globals.IsBusy = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }

            });
            t.Start();
            //20161005 CMN End.
        }

        private void SaveDatabase()
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            isLoadingOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddPCLResultFileStorageDetails(ObjPatientPCLImagingResult, ObjGetPCLResultFileStoreDetails.Select(x => x.ObjectResult).ToList(), new List<PCLResultFileStorageDetail>(), null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndAddPCLResultFileStorageDetails(asyncResult);
                            if (items)
                            {
                                MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
                            }
                        }
                        catch (Exception ex)
                        {
                            isLoadingOperator = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingOperator = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void SaveImageToStore(List<PCLResultFileStorageDetail> lst)
        {
            //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            isLoadingDetail = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDoListUpload(lst, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var bOK = contract.EndDoListUpload(asyncResult);
                            if (bOK)
                            {
                                SaveDatabase();
                            }
                            else
                            {
                                MessageBox.Show("Error!!!");
                            }
                        }
                        catch (Exception ex)
                        {
                            isLoadingDetail = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private bool _IsEnableButton = true;
        public bool IsEnableButton
        {
            get { return _IsEnableButton; }
            set
            {
                if (_IsEnableButton != value)
                {
                    _IsEnableButton = value;
                    NotifyOfPropertyChange(() => IsEnableButton);
                }
            }
        }
        //▼====== #004
        public void GetImageInAnotherViewModel(PatientPCLRequest message)
        {
            if (message != null)
            {
                if (_currentView != null)
                {
                    _currentView.SetObjectSource(null);
                }
                ObjBitmapImage = null;
                LoadData(message);
            }
            Globals.PatientPCLRequest_Result = message;
        }
        //▲====== #004

        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                if (_currentView != null)
                {
                    _currentView.SetObjectSource(null);
                }
                ObjBitmapImage = null;
                LoadData(message.PCLRequest);
            }
            Globals.PatientPCLRequest_Result = message.PCLRequest;
        }

        public void RefreshBtn()
        {
            if (Globals.PatientPCLRequest_Imaging != null && Globals.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            {
                Coroutine.BeginExecute(LoadDataCoroutine(Globals.PatientPCLRequest_Imaging));
            }
        }

        /*▼====: #003*/
        private ObservableCollection<Resources> _HIRepResourceCode;
        public ObservableCollection<Resources> HIRepResourceCode
        {
            get { return _HIRepResourceCode; }
            set
            {
                _HIRepResourceCode = value;
                NotifyOfPropertyChange(() => HIRepResourceCode);
            }
        }
        
        //Lay cac ma may dua vao PCLResultParamImpID
        private void GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Resources> results = contract.EndGetResourcesForMedicalServicesListByTypeID(asyncResult);
                                if (results != null)
                                {
                                    if (HIRepResourceCode == null)
                                    {
                                        HIRepResourceCode = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        HIRepResourceCode.Clear();
                                    }
                                    HIRepResourceCode = results.ToObservableCollection();
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
        /*▲====: #003*/

        public IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection
        {
            get
            {
                return ObjGetPCLResultFileStoreDetails == null || ObjGetPCLResultFileStoreDetails.Count == 0 ? null : ObjGetPCLResultFileStoreDetails.Select(x => x.ObjectResult).ToList();
            }
        }
    }
    public class PCLResultFileStorageDetailClient
    {
        public Stream IOStream { get; set; }
        public PCLResultFileStorageDetail ObjectResult { get; set; }
    }
}