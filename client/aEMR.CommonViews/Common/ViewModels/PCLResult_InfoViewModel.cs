//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel.Composition;
//using System.IO;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;
//using aEMR.Infrastructure.Events;
//using aEMR.ServiceClient;
//using aEMR.ViewContracts;
//using Caliburn.Micro;
//using DataEntities;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.CachingUtils;
//using Castle.Windsor;
//using Castle.Core.Logging;
//using eHCMSLanguage;

//namespace aEMR.Common.ViewModels
//{
//    [Export(typeof(IPCLResult_Info)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class PCLResult_InfoViewModel : Conductor<object>, IPCLResult_Info
//        , IHandle<hplInputKetQua_Click<PatientPCLRequest, PatientPCLRequestDetail>>
//        , IHandle<btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream>>
//    {
//        private readonly INavigationService _navigationService;
//        private readonly ISalePosCaching _salePosCaching;
//        private readonly ILogger _logger;
//        [ImportingConstructor]
//        public PCLResult_InfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
//        {
//            _navigationService = navigationService;
//            _logger = container.Resolve<ILogger>();
//            _salePosCaching = salePosCaching;

//            eventAgr.Subscribe(this);
//        }
//        protected override void OnViewLoaded(object view)
//        {
//            base.OnViewLoaded(view);
//            _currentView = view as IPCLResult_InfoView;
//        }
//        IPCLResult_InfoView _currentView;


//        private PatientPCLRequest _ObjPatientPCLRequestSelected;
//        public PatientPCLRequest ObjPatientPCLRequestSelected
//        {
//            get { return _ObjPatientPCLRequestSelected; }
//            set
//            {
//                _ObjPatientPCLRequestSelected = value;
//                NotifyOfPropertyChange(() => ObjPatientPCLRequestSelected);
//            }
//        }

//        private PatientPCLRequestDetail _ObjPatientPCLRequestDetailSelected;
//        public PatientPCLRequestDetail ObjPatientPCLRequestDetailSelected
//        {
//            get { return _ObjPatientPCLRequestDetailSelected; }
//            set
//            {
//                _ObjPatientPCLRequestDetailSelected = value;
//                NotifyOfPropertyChange(() => ObjPatientPCLRequestDetailSelected);
//            }
//        }

//        public void Handle(hplInputKetQua_Click<PatientPCLRequest, PatientPCLRequestDetail> message)
//        {
//            if(message!=null)
//            {
//                ObjPatientPCLRequestSelected = message.PCLReq;
//                ObjPatientPCLRequestDetailSelected = message.PCLReqDetail;

//                //Đọc ds kết quả...
//                GetPCLResultFileStoreDetails(Globals.PatientAllDetails.PatientInfo.PatientID, ObjPatientPCLRequestSelected.PatientPCLReqID, null, ObjPatientPCLRequestDetailSelected.PCLExamTypeID);
//            }
//        }


//        string ImagePool;
//        string ImageStore;
//        string ImageThumbTemp;
      

//        #region Property Enable
//        private bool _CtrIsEnabled;
//        public bool CtrIsEnabled
//        {
//            get { return _CtrIsEnabled; }
//            set
//            {
//                if(_CtrIsEnabled!=value)
//                {
//                    _CtrIsEnabled = value;
//                    NotifyOfPropertyChange(()=>CtrIsEnabled);
//                }
//            }
//        }

//        private bool _HasPatient;
//        public bool HasPatient
//        {
//            get { return _HasPatient; }
//            set
//            {
//                if (_HasPatient != value)
//                {
//                    _HasPatient = value;
//                    NotifyOfPropertyChange(() => HasPatient);
//                }
//            }
//        }

//        #endregion
        

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            Globals.EventAggregator.Subscribe(this);

//            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetail>();

//            FolderListTextSelect = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);

//            GetTestingAgency_All();

//            CheckFormStartValid();

//            KhoiTao();

//            //GetAllConfigItemValues();
//            GetAppConfigValue();
//        }

//        private ObservableCollection<DataEntities.PCLResultFileStorageDetail> _ObjGetPCLResultFileStoreDetails;
//        public ObservableCollection<DataEntities.PCLResultFileStorageDetail> ObjGetPCLResultFileStoreDetails
//        {
//            get { return _ObjGetPCLResultFileStoreDetails; }
//            set
//            {
//                _ObjGetPCLResultFileStoreDetails = value;
//                NotifyOfPropertyChange(() => ObjGetPCLResultFileStoreDetails);
//            }
//        }
      
//        private void GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID)
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0528_G1_DSKQua) });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new PCLsClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetPCLResultFileStoreDetails(ptID,ptPCLReqID,PCLGroupID,PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var items = contract.EndGetPCLResultFileStoreDetails(asyncResult);
//                            if (items != null)
//                            {
//                                ObjGetPCLResultFileStoreDetails = new ObservableCollection<DataEntities.PCLResultFileStorageDetail>(items);
//                            }
//                            else
//                            {
//                                ObjGetPCLResultFileStoreDetails = null;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }

//        #region InitObject
//        private PatientPCLImagingResult _ObjPatientPCLImagingResult;
//        public PatientPCLImagingResult ObjPatientPCLImagingResult
//        {
//            get
//            {
//                return _ObjPatientPCLImagingResult;
//            }
//            set
//            {
//                if (_ObjPatientPCLImagingResult != value)
//                {
//                    _ObjPatientPCLImagingResult = value;
//                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult);
//                }
//            }
//        }


//        private void CheckFormStartValid()
//        {
//            CtrIsEnabled = false;

//            if (Globals.PatientAllDetails.PatientInfo != null && Globals.PatientAllDetails.PatientInfo.PatientID > 0)
//            {
//                HasPatient = true;
//            }
//            else
//            {
//                HasPatient = false;
//            }
//        }

//        private void KhoiTao()
//        {
//            ObjPatientPCLImagingResult=new PatientPCLImagingResult();
//            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.StaffID;
//            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
//            ObjPatientPCLImagingResult.PCLExamTypeID = -1;

//            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.Value;
//            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
//            ObjPatientPCLImagingResult.IsExternalExam = false;
//            OptionValue = 1;

//            PCLResultFileStorageDetailSelected=new PCLResultFileStorageDetail();
//            cboListFolderSelectedValue = eHCMSResources.Z0541_G1_Images;
//            V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
//        }

//        #endregion


//        /// <summary>
//        /// Các Biến Giá Trị
//        /// </summary>
        
//        //private IList<string> _newObjGetAllConfigItemValues;
//        //public IList<string> NewObjGetAllConfigItemValues
//        //{
//        //    get
//        //    {
//        //        return _newObjGetAllConfigItemValues;
//        //    }
//        //    set
//        //    {
//        //        if (_newObjGetAllConfigItemValues != value)
//        //        {
//        //            _newObjGetAllConfigItemValues = value;
//        //            NotifyOfPropertyChange(() => NewObjGetAllConfigItemValues);
//        //        }
//        //    }
//        //}

        
//        private void GetAppConfigValue()
//        {
//            //o day anh Bang co the su dung Globals.ConfigList 
//            ImagePool = Globals.ServerConfigSection.Hospitals.PCLResourcePool;
//            ImageStore = Globals.ServerConfigSection.Hospitals.PCLStorePool;
//            ImageThumbTemp = Globals.ServerConfigSection.Hospitals.PCLThumbTemp;
//            GetFolderList(ImagePool);
//        }
        
//        private string _FolderListTextSelect;
//        public string FolderListTextSelect
//        {
//            get { return _FolderListTextSelect; }
//            set
//            {
//                if (_FolderListTextSelect != value)
//                {
//                    _FolderListTextSelect = value;
//                    NotifyOfPropertyChange(() => FolderListTextSelect);
//                }
//            }
//        }

//        private ObservableCollection<String> _Folders;
//        public ObservableCollection<String> FolderList
//        {
//            get
//            {
//                return _Folders;
//            }
//            set
//            {
//                if (_Folders != value)
//                {
//                    _Folders = value;
//                    NotifyOfPropertyChange(() => FolderList);
//                }
//            }
//        }
//        private void GetFolderList(string Path)
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1119_G1_FolderList) });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new PCLsImportClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetFolderList(Path, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var items = contract.EndGetFolderList(asyncResult);
//                            if (items != null)
//                            {
//                                FolderList =new ObservableCollection<string>(items);

//                                //Item Default
//                                string ItemDefault= eHCMSResources.A0015_G1_Chon;
//                                FolderList.Insert(0, ItemDefault);
//                                //Item Default

//                            }
//                            else
//                            {
//                                FolderList = null;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }
        
//        private int _OptionValue;
//        public int OptionValue
//        {
//            get
//            {
//                return _OptionValue;
//            }
//            set
//            {
//                if (_OptionValue != value)
//                {
//                    _OptionValue = value;
//                    NotifyOfPropertyChange(() => OptionValue);
//                }
//            }
//        }

//        private string _cboListFolderSelectedValue;
//        public string cboListFolderSelectedValue
//        {
//            get
//            {
//                return _cboListFolderSelectedValue;
//            }
//            set
//            {
//                if (_cboListFolderSelectedValue != value)
//                {
//                    _cboListFolderSelectedValue = value;
//                    NotifyOfPropertyChange(() => cboListFolderSelectedValue);
//                }
//            }
//        }
        
//        private long _V_ResultType;
//        public long V_ResultType
//        {
//            get { return _V_ResultType; }
//            set
//            {
//                if (_V_ResultType != value)
//                {
//                    _V_ResultType = value;
//                    NotifyOfPropertyChange(() => V_ResultType);
//                }
//            }
//        }


//        public void gOption0_Click(object sender, RoutedEventArgs e)
//        {
//            RadioButton Ctr=(sender as RadioButton);
//            if(Ctr.IsChecked.Value)
//            {
//                OptionValue = 1;
//                cboListFolderSelectedValue = eHCMSResources.Z0541_G1_Images;
//                V_ResultType =(long)AllLookupValues.FileStorageResultType.IMAGES;
//            }
//        }
//        public void gOption1_Click(object sender, RoutedEventArgs e)
//        {
//            RadioButton Ctr = (sender as RadioButton);
//            if (Ctr.IsChecked.Value)
//            {
//                OptionValue = 2;
//                cboListFolderSelectedValue = eHCMSResources.Z0542_G1_Videos;
//                V_ResultType = (long)AllLookupValues.FileStorageResultType.VIDEO_RECORDING;
//            }
//        }
//        public void gOption2_Click(object sender, RoutedEventArgs e)
//        {
//            RadioButton Ctr = (sender as RadioButton);
//            if (Ctr.IsChecked.Value)
//            {
//                OptionValue = 3;
//                cboListFolderSelectedValue = eHCMSResources.Z0543_G1_Documents;
//                V_ResultType = (long)AllLookupValues.FileStorageResultType.DOCUMENTS;
//            }
//        }

//        private PCLResultFileStorageDetail _PCLResultFileStorageDetailSelected;
//        public PCLResultFileStorageDetail PCLResultFileStorageDetailSelected
//        {
//            get
//            {
//                return _PCLResultFileStorageDetailSelected;
//            }
//            set
//            {
//                if (_PCLResultFileStorageDetailSelected != value)
//                {
//                    _PCLResultFileStorageDetailSelected = value;
//                    NotifyOfPropertyChange(()=>PCLResultFileStorageDetailSelected);
//                }
//            }
//        }


//        public void cboListFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if ((sender as ComboBox).SelectedItem != null)
//            {
//                cboListFolderSelectedValue = (sender as ComboBox).SelectedItem.ToString();
//            }
//        }

//        public void btBrowse()
//        {
//            if (!string.IsNullOrEmpty(FolderListTextSelect))
//            {
//                if (FolderListTextSelect != eHCMSResources.A0015_G1_Chon)
//                {
//                    //var typeInfo = Globals.GetViewModel<IFileExplorer>();
//                    //typeInfo.dir = ImagePool + @"\" + cboListFolderSelectedValue;
//                    //typeInfo.itype = OptionValue;
//                    //typeInfo.InitPCLResultFileStorageDetail(cboListFolderSelectedValue, V_ResultType);

//                    //var instance = typeInfo as Conductor<object>;

//                    //Globals.ShowDialog(instance, (o) =>
//                    //{
//                    //    //lam gi do
//                    //});

//                    Action<IFileExplorer> onInitDlg = (typeInfo) =>
//                    {
//                        typeInfo.dir = ImagePool + @"\" + cboListFolderSelectedValue;
//                        typeInfo.itype = OptionValue;
//                        typeInfo.InitPCLResultFileStorageDetail(cboListFolderSelectedValue, V_ResultType);
//                    };

//                    GlobalsNAV.ShowDialog<IFileExplorer>(onInitDlg);
//                }
//                else
//                {
//                    MessageBox.Show(eHCMSResources.A0291_G1_Msg_CDinhThuMucChuaFile, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                }
//            }

//        }

//        public void chkExternalExam_Click(object sender, RoutedEventArgs e)
//        {
//            CheckBox Ctr = (sender as CheckBox);
//            if(Ctr.IsChecked.Value)
//            {
//                CtrIsEnabled = true;
//            }
//            else
//            {
//                CtrIsEnabled = false;
//            }

//        }


//        #region Thông tin bệnh viện ngoài 
//        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;
//        public ObservableCollection<TestingAgency> ObjTestingAgencyList
//        {
//            get
//            {
//                return _ObjTestingAgencyList;
//            }
//            set
//            {
//                if (_ObjTestingAgencyList != value)
//                {
//                    _ObjTestingAgencyList = value;
//                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
//                }
//            }
//        }
//        private void GetTestingAgency_All()
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0535_G1_DSBVNgoai) });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new PCLsImportClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var items = contract.EndGetTestingAgency_All(asyncResult);
//                            if (items != null)
//                            {
//                                ObjTestingAgencyList = new ObservableCollection<TestingAgency>(items);

//                                //Item Default
//                                DataEntities.TestingAgency ItemDefault = new DataEntities.TestingAgency();
//                                ItemDefault.AgencyID = -1;
//                                ItemDefault.AgencyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
//                                ObjTestingAgencyList.Insert(0, ItemDefault);
//                                //Item Default

//                            }
//                            else
//                            {
//                                ObjTestingAgencyList = null;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }
         


//        #endregion


//        public void Handle(btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream> message)
//        {
//            if(message!=null)
//            {
//                message.File.Flag = 1;
//                AddFile(message);
//            }
//        }
       
//        private void AddFile(btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream> message)
//        {
//            try
//            {
//                ObjGetPCLResultFileStoreDetails.Add(message.File);
//                CheckKetQuaReturn(message.StreamFile, message.TypeFile);
//            }
//            catch
//            {
//                MessageBox.Show(eHCMSResources.A1024_G1_Msg_InfoThemFileFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            }   
//        }

//        public int GetType(object item)
//        {
//            int type = 0;
//            if (item != null)
//            {
//                PCLResultFileStorageDetail p = item as PCLResultFileStorageDetail;
//                if (p.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
//                {
//                    type = 1;
//                }
//                else if (p.V_ResultType == (long)AllLookupValues.FileStorageResultType.VIDEO_RECORDING)
//                {
//                    type = 2;
//                }
//            }
//            return type;
//        }

//        private Stream _ObjGetVideoAndImage;
//        public Stream ObjGetVideoAndImage
//        {
//            get { return _ObjGetVideoAndImage; }
//            set
//            {
//                if (_ObjGetVideoAndImage != value)
//                {
//                    _ObjGetVideoAndImage = value;
//                    NotifyOfPropertyChange(() => ObjGetVideoAndImage);
//                }
//            }
//        }
//        private void GetVideoAndImage(string path, int itype)
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1120_G1_TaiFile) });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonService_V2Client())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var items = contract.EndGetVideoAndImage(asyncResult);
//                            if (items != null)
//                            {
//                                ObjGetVideoAndImage = new MemoryStream(items);
//                                CheckKetQuaReturn(ObjGetVideoAndImage,itype);
//                            }
//                            else
//                            {
//                                ObjGetVideoAndImage = null;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }

//        private BitmapImage _ObjBitmapImage;
//        public BitmapImage ObjBitmapImage
//        {
//            get { return _ObjBitmapImage; }
//            set
//            {
//                if (_ObjBitmapImage != value)
//                {
//                    _ObjBitmapImage = value;
//                    NotifyOfPropertyChange(() => ObjBitmapImage);
//                }
//            }
//        }

//        private void CheckKetQuaReturn(Stream pStream, int itype)
//        {
//            VideoAndImageStream = pStream;

//            if (pStream != null)
//            {
//                if (itype == 1)/*Images*/
//                {
//                    System.Windows.Media.Imaging.BitmapImage imgsource = new System.Windows.Media.Imaging.BitmapImage();
//                    //imgsource.SetSource(pStream);
//                    ObjBitmapImage = imgsource;
//                    SetVisibleForVideoControls(Visibility.Collapsed);
//                    SetVisibleForImgControls(Visibility.Visible);
//                }
//                else if (itype == 2)/*Video*/
//                {
//                    SetVisibleForVideoControls(Visibility.Visible);
//                    SetVisibleForImgControls(Visibility.Collapsed);

//                    if (_currentView != null)
//                    {
//                        _currentView.SetObjectSource(pStream);
//                    }

//                }
//                else/*Document*/
//                {
//                    SetVisibleForVideoControls(Visibility.Collapsed);
//                    SetVisibleForImgControls(Visibility.Collapsed);
//                }
//            }
//            else
//            {
//                SetVisibleForVideoControls(Visibility.Collapsed);
//                SetVisibleForImgControls(Visibility.Collapsed);
//            }
//        }

//        private Visibility _ControlVideoVisibility;
//        public Visibility ControlVideoVisibility
//        {
//            get { return _ControlVideoVisibility; }
//            set
//            {
//                if (_ControlVideoVisibility != value)
//                {
//                    _ControlVideoVisibility = value;
//                    NotifyOfPropertyChange(() => ControlVideoVisibility);
//                }
//            }
//        }
//        private Visibility _ControlImgVisibility;
//        public Visibility ControlImgVisibility
//        {
//            get { return _ControlImgVisibility; }
//            set
//            {
//                if (_ControlImgVisibility != value)
//                {
//                    _ControlImgVisibility = value;
//                    NotifyOfPropertyChange(() => ControlImgVisibility);
//                }
//            }
//        }
//        private void SetVisibleForVideoControls(Visibility pV)
//        {
//            ControlVideoVisibility = pV;
//        }
//        private void SetVisibleForImgControls(System.Windows.Visibility pV)
//        {
//            ControlImgVisibility = pV;
//        }

//        #region Control media
//        public void btPlay()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btPlay();
//            }
//        }
//        public void btPause()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btPause();
//            }
//        }

//        public void btStop()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btStop();
//            }
//        }

//        public void btMute(object sender, RoutedEventArgs e)
//        {
//            if (_currentView != null)
//            {
//                _currentView.btMute();
//            }
//        }

//        public void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//        {
//            if (_currentView != null)
//            {
//                _currentView.btVolume_ValueChanged(sender, e);
//            }
//        }
//        #endregion

//        public void hplDelete_Click(object selectedItem)
//        {
//            DataEntities.PCLResultFileStorageDetail p = (selectedItem as DataEntities.PCLResultFileStorageDetail);

//            if (p != null)
//            {
//                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLResultFileName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    ObjGetPCLResultFileStoreDetails.Remove(p);

//                    if(ObjGetPCLResultFileStoreDetails.Count<=0)
//                    {
//                        ObjBitmapImage = null;
//                        if (_currentView != null)
//                        {
//                            _currentView.SetObjectSource(null);
//                        }
//                        ControlImgVisibility = Visibility.Collapsed;
//                        ControlVideoVisibility = Visibility.Collapsed;
//                    }
//                }
//            }

//        }


//        private PCLResultFileStorageDetail _ObjPCLResultFileStorageDetailSelected;
//        public PCLResultFileStorageDetail ObjPCLResultFileStorageDetailSelected
//        {
//            get
//            {
//                return _ObjPCLResultFileStorageDetailSelected;
//            }
//            set
//            {
//                if (_ObjPCLResultFileStorageDetailSelected != value)
//                {
//                    _ObjPCLResultFileStorageDetailSelected = value;
//                    NotifyOfPropertyChange(() => ObjPCLResultFileStorageDetailSelected);
//                }
//            }
//        }
//        public void dtgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {   
//            if(ObjPCLResultFileStorageDetailSelected!=null)
//            {
//                int iType = GetType(ObjPCLResultFileStorageDetailSelected);
//                GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.FullPath,iType);
//            }
//        }

//        public void hplDiagnoticsImg()
//        {
//            if (ObjBitmapImage != null)
//            {
//                //var typeInfo = Globals.GetViewModel<IImageDisgnosticExplorer>();
//                //typeInfo.ObjBitmapImage=ObjBitmapImage;
//                //var instance = typeInfo as Conductor<object>;

//                //Globals.ShowDialog(instance, (o) =>
//                //{
//                //    //lam gi do
//                //});

//                Action<IImageDisgnosticExplorer> onInitDlg = (typeInfo) =>
//                {
//                    typeInfo.ObjBitmapImage = ObjBitmapImage;
//                };

//                GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
//            }
//        }

//        private Stream _VideoAndImageStream;
//        public Stream VideoAndImageStream
//        {
//            get { return _VideoAndImageStream; }
//            set
//            {
//                _VideoAndImageStream = value;
//                NotifyOfPropertyChange(() => VideoAndImageStream);
//            }
//        }

//        public void hplDiagnoticsVideo()
//        {
//            if (VideoAndImageStream != null)
//            {
//                //var typeInfo = Globals.GetViewModel<IVideoDiagnosticExplorer>();
//                //typeInfo.VideoStream = VideoAndImageStream;
//                //var instance = typeInfo as Conductor<object>;

//                //Globals.ShowDialog(instance, (o) =>
//                //{
//                //    //lam gi do
//                //});

//                Action<IVideoDiagnosticExplorer> onInitDlg = (typeInfo) =>
//                {
//                    typeInfo.VideoStream = VideoAndImageStream;
//                };

//                GlobalsNAV.ShowDialog<IVideoDiagnosticExplorer>(onInitDlg);
//            }
//        }

//        public void btSave()
//        {

//        }
//    }
    
//}

////private void GetAllConfigItemValues()
////{
////    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "ConfigItem..." });

////    var t = new Thread(() =>
////    {
////        using (var serviceFactory = new CommonServiceClient())
////        {
////            var contract = serviceFactory.ServiceInstance;

////            contract.BeginGetAllConfigItemValues(Globals.DispatchCallback((asyncResult) =>
////            {
////                try
////                {
////                    var items = contract.EndGetAllConfigItemValues(asyncResult);
////                    if (items != null)
////                    {
////                        ObjGetAllConfigItemValues = items;
////                        GetAppConfigValue();
////                    }
////                    else
////                    {
////                        ObjGetAllConfigItemValues = null;
////                    }
////                }
////                catch (Exception ex)
////                {
////                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
////                }
////                finally
////                {
////                    Globals.IsBusy = false;
////                }
////            }), null);
////        }


////    });
////    t.Start();
////}