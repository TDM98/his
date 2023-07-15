using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using System.IO;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Threading;
using aEMR.ServiceClient;
using PCLsService;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using aEMR.Common;
using eHCMSLanguage;
using Microsoft.Win32;
using Castle.Windsor;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.PCL_ViewResults.ViewModels
{
    [Export(typeof(IPatientPCLDeptImagingExtResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLDeptImagingExtResultViewModel : ViewModelBase, IPatientPCLDeptImagingExtResult
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream>>
        , IHandle<ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<DbClickPtPCLReqExtDo<PatientPCLRequest_Ext, PatientPCLRequestDetail_Ext, String>>
        , IHandle<IPatientPCLDeptImagingExtResultDoneEvent>
    {
        #region Indicator Member

        public object UCDoctorProfileInfo { get; set; }

        public object UCPatientProfileInfo { get; set; }

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
        private ObservableCollection<PatientPCLRequest_Ext> _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory;
        private int _OptionValue;

        private long _V_ResultType;
        private string _cboListFolderSelectedValue;

        #region Property Enable

        private bool _CtrIsEnabled;

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

        private bool _HasPatient;
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

        private bool _PCLPropertyChange;
        public bool PCLPropertyChange
        {
            get { return _PCLPropertyChange; }
            set
            {
                if (_PCLPropertyChange != value)
                {
                    _PCLPropertyChange = value;
                    NotifyOfPropertyChange(() => PCLPropertyChange);
                }
            }
        }

        #endregion
        [ImportingConstructor]
        public PatientPCLDeptImagingExtResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            //KMx: Sau khi kiểm tra, thấy View này không sử dụng View IPatientMedicalRecords_ByPatientID (25/05/2014 14:48).
            //var uc3 = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            //UCHeaderInfoPMR = uc3;
            //ActivateItem(uc3);
            //Load UC
            ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = new ObservableCollection<PatientPCLRequest_Ext>();
            ObjPCLExamTypes_ByPatientPCLReqID = new ObservableCollection<PCLExamType>();
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();

            FolderListTextSelect = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);

            GetTestingAgency_All();

            CheckFormStartValid();

            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                InitPatientInfo(Registration_DataStorage.CurrentPatient);
            }

            GetAppConfigValue();

            if (Globals.PatientPCLRequest_Imaging != null && Globals.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            {
                //LoadData(Globals.PatientPCLRequest_Imaging);
            }
        }

        public ObservableCollection<PatientPCLRequest_Ext> ObjPatientPCLRequest_ByPtRegIDV_PCLCategory
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
                    NotifyOfPropertyChange(() => FolderList);
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

        #region IHandle<btChooseFileResultForPCL_Click<PCLResultFileStorageDetail,int,Stream>> Members

        public void Handle(btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream> message)
        {
            if (message != null)
            {
                message.File.Flag = 1;
                AddFile(message);
            }
        }
        public void Handle(ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                if (Registration_DataStorage.CurrentPatient != null)
                {
                    InitPatientInfo(Registration_DataStorage.CurrentPatient);
                    //SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
                    //Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
                }
            }
        }

        public void Handle(DbClickPtPCLReqExtDo<PatientPCLRequest_Ext, PatientPCLRequestDetail_Ext, String> message)
        {
            if (message != null)
            {
                formWork = FormWork.Disable;
                ObjPatientPCLImagingResult.PatientPCLReqID = ((PatientPCLRequest_Ext)message.ObjA).PatientPCLReqExtID;
                ObjPatientPCLImagingResult.PCLExamTypeID = ((PatientPCLRequestDetail_Ext)message.ObjB).PCLExamTypeID;
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = ObjPatientPCLImagingResult.PatientPCLRequest.Diagnosis;
                ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
                {
                    PCLExamTypeID = ((PatientPCLRequestDetail_Ext)message.ObjB).PCLExamTypeID,
                    PCLExamTypeName = ((PatientPCLRequestDetail_Ext)message.ObjB).PCLExamType.PCLExamTypeName
                };

                ObjPatientPCLImagingResultBackup = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLImagingResult);
                ObjPatientPCLRequestBackup = aEMR.Common.ObjectCopier.DeepCopy((PatientPCLRequest_Ext)message.ObjA);
                LoadData((PatientPCLRequest_Ext)message.ObjA);
                ObjBitmapImage = null;
                if (_currentView != null)
                {
                    _currentView.SetObjectSource(null);
                }
            }
        }

        public void AfterSave(PatientPCLRequest_Ext message)
        {
            if (message != null)
            {
                formWork = FormWork.Disable;
                ObjPatientPCLImagingResult.PatientPCLReqID = ((PatientPCLRequest_Ext)message).PatientPCLReqExtID;
                ObjPatientPCLImagingResult.PCLExamTypeID = ((PatientPCLRequest_Ext)message).PCLExamTypeID.GetValueOrDefault();
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = ObjPatientPCLImagingResult.PatientPCLRequest.Diagnosis;
                ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
                {
                    PCLExamTypeID = ((PatientPCLRequest_Ext)message).PCLExamTypeID.GetValueOrDefault(),
                    PCLExamTypeName = ((PatientPCLRequest_Ext)message).PCLExamTypeName
                };

                ObjPatientPCLImagingResultBackup = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLImagingResult);
                ObjPatientPCLRequestBackup = aEMR.Common.ObjectCopier.DeepCopy((PatientPCLRequest_Ext)message);
                LoadData((PatientPCLRequest_Ext)message);
                ObjBitmapImage = null;
                if (_currentView != null)
                {
                    _currentView.SetObjectSource(null);
                }
            }
        }

        public void Handle(IPatientPCLDeptImagingExtResultDoneEvent message) 
        {
            ///do nothing
            ///            
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
                    //System.Windows.Media.Imaging.BitmapImage imgsource = new System.Windows.Media.Imaging.BitmapImage();
                    //imgsource.SetSource(pStream);
                    ObjBitmapImage = Globals.GetWriteableBitmapFromStream(pStream);
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
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0527_G1_DSYC });

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
                var Objtmp = ((sender as ComboBox).SelectedItem as PatientPCLRequest_Ext);
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0527_G1_DSYC });

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
                    Coroutine.BeginExecute(LoadDataCoroutine_NotInit(Registration_DataStorage.CurrentPatient.PatientID,
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



        public void btBrowse()
        {
            //if (ObjPatientPCLImagingResult == null || ObjPatientPCLImagingResult.PatientPCLReqID <= 0)
            //{
            //    MessageBox.Show(eHCMSResources.K0380_G1_ChonPhYC);
            //    return;
            //}

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
                        dlg.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                    }
                    else
                    {
                        dlg.Filter = "Media Files|*.mp4;*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3;*.wmv";
                        //openFileDialog.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3|All Files|*.*";
                    }

                    bool? retval = dlg.ShowDialog();

                    if (retval != null && retval == true)
                    {
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
                        itemClient.ObjectResult.PCLResultFileName = Registration_DataStorage.CurrentPatient.PatientID.ToString() + "-" + Guid.NewGuid().ToString() +
                                             dlg.FileName.Substring(index, dlg.FileName.Length - index);

                        ObjGetPCLResultFileStoreDetails.Add(itemClient);

                        // SendFile(dlg.File.Name, bytes, false, ImagePool + @"\" + cboListFolderSelectedValue);
                    //}
                    //else
                    //{
                    //}
                //}
                //else
                //{
                //    MessageBox.Show(eHCMSResources.A0291_G1_Msg_CDinhThuMucChuaFile, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);                    
                //}
            }
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0535_G1_DSBVNgoai) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    IPCLsImport contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<TestingAgency> items = contract.EndGetTestingAgency_All(asyncResult);
                            if (items != null)
                            {
                                ObjTestingAgencyList = new ObservableCollection<TestingAgency>(items);

                                //Item Default
                                var ItemDefault = new TestingAgency();
                                ItemDefault.AgencyID = -1;
                                ItemDefault.AgencyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                ObjTestingAgencyList.Insert(0, ItemDefault);
                                //Item Default
                            }
                            else
                            {
                                ObjTestingAgencyList = null;
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
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult);
                }
            }
        }

        private PatientPCLImagingResult _ObjPatientPCLImagingResultNewBackup;
        public PatientPCLImagingResult ObjPatientPCLImagingResultNewBackup
        {
            get { return _ObjPatientPCLImagingResultNewBackup; }
            set
            {
                if (_ObjPatientPCLImagingResultNewBackup != value)
                {
                    _ObjPatientPCLImagingResultNewBackup = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResultNewBackup);
                }
            }
        }
        

        private PatientPCLImagingResult _ObjPatientPCLImagingResultBackup;
        public PatientPCLImagingResult ObjPatientPCLImagingResultBackup
        {
            get { return _ObjPatientPCLImagingResultBackup; }
            set
            {
                if (_ObjPatientPCLImagingResultBackup != value)
                {
                    _ObjPatientPCLImagingResultBackup = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResultBackup);
                }
            }
        }


        private void CheckFormStartValid()
        {
            CtrIsEnabled = false;

            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                HasPatient = true;
            }
            else
            {
                HasPatient = false;
            }
        }
        public void InitPatientInfo(Patient CurrentPatient)
        {
            if (CurrentPatient != null)/*Làm CLS chỉ cần kiểm tra BN !=null*/
            {
                formWork = FormWork.Edit;
                InitPCLRequest();
                KhoiTao();

                if (!Globals.isConsultationStateEdit)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0391_G1_BNDuocChonTuLSBA));
                    FormIsEnabled = false;
                    formWork = FormWork.Disable;
                    return;
                }

                ObjGetDiagnosisTreatmentByPtID = new DiagnosisTreatment();
                if (Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
                {
                    GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
                }
                else
                {
                    MessageBox.Show(string.Format("{0} (PtRegistrationID): ", eHCMSResources.A0747_G1_Msg_InfoKhTimThayMaDK) + Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                //FormIsEnabled = false;
            }
        }
        private void KhoiTao()
        {
            this.ShowBusyIndicator();
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjPatientPCLImagingResult.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            //CreateNewPCLReq();
            ObjPatientPCLRequestNew = NewPCLReq();
            ObjPatientPCLImagingResult.PatientPCLRequest = copyPatientPCLRequest(ObjPatientPCLRequestNew);
            ObjPatientPCLRequestNewBackup = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew);
            ObjPatientPCLImagingResult.AgencyID = ObjPatientPCLRequestNew.AgencyID;
            ObjPatientPCLImagingResult.DiagnoseOnPCLExam = ObjPatientPCLRequestNew.Diagnosis == null ? "" : ObjPatientPCLRequestNew.Diagnosis;
            ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
            {
                PCLExamTypeID = (long)AllLookupValues.V_PCLExamTypeExt.NgoaiVien,
                PCLExamTypeName = Helpers.GetEnumDescription(AllLookupValues.V_PCLExamTypeExt.NgoaiVien)
            };
            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            ObjPatientPCLImagingResult.PCLExamTypeID = (long)AllLookupValues.V_PCLExamTypeExt.NgoaiVien;

            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            ObjPatientPCLImagingResult.IsExternalExam = true;
            ObjPatientPCLImagingResult.ResultExplanation = "";
            OptionValue = 1;

            cboListFolderSelectedValue = "Images";
            V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;

            if (ObjPatientPCLImagingResult.DiagnoseOnPCLExam != null)
            {
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
            }
            ObjPatientPCLImagingResultNewBackup = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLImagingResult);
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
            ObjBitmapImage = null;
            if (_currentView != null)
            {
                _currentView.SetObjectSource(null);
            }
            this.HideBusyIndicator();
        }

        private void BackupNew()
        {
            this.ShowBusyIndicator();
            ObjPatientPCLRequestNew = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNewBackup);
            ObjPatientPCLImagingResult = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLImagingResultNewBackup);
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
            ObjBitmapImage = null;
            if (_currentView != null)
            {
                _currentView.SetObjectSource(null);
            }
            this.HideBusyIndicator();
        }
        private void InitPCLRequest()
        {
            ObjPatientPCLRequestNew = NewPCLReq();
            ObjPatientPCLRequestNew.AgencyID = -1;
        }
        #endregion

        public void LoadData(PatientPCLRequest_Ext message)
        {
            Coroutine.BeginExecute(LoadDataCoroutine(message));
        }
        private PatientPCLRequest copyPatientPCLRequest(PatientPCLRequest_Ext p)
        {
            PatientPCLRequest temp = new PatientPCLRequest();
            temp.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
            temp.DeptID = p.DeptID;
            temp.PtRegistrationID = p.PtRegistrationID;

            temp.PCLRequestNumID = p.PCLRequestNumID;
            temp.Diagnosis = p.Diagnosis;

            temp.StaffIDName = p.FullName;
            temp.StaffID = p.StaffID;
            temp.DoctorStaffID = p.DoctorStaffID;

            temp.ReqFromDeptLocID = p.ReqFromDeptLocID;

            temp.DoctorComments = p.DoctorComments;
            temp.IsCaseOfEmergency = p.IsCaseOfEmergency;

            temp.IsExternalExam = p.IsExternalExam;
            temp.IsImported = p.IsImported;

            temp.PatientServiceRecord = new PatientServiceRecord();
            temp.PatientServiceRecord.ServiceRecID = p.PatientServiceRecord.ServiceRecID;

            temp.PatientServiceRecord.StaffID = p.PatientServiceRecord.StaffID;
            temp.V_PCLRequestStatus = p.V_PCLRequestStatus;
            return temp;
        }
        private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest_Ext p)
        {
            isLoadingDetail = true;

            if (ObjPatientPCLImagingResult == null)
            {
                KhoiTao();
            }
            ObjPatientPCLImagingResult.PatientPCLRequest = copyPatientPCLRequest(p);
            ObjPatientPCLRequestNew = p;
            PCLPropertyChange = false;
            p.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(p_PropertyChanged);
            NotifyOfPropertyChange(() => ObjPatientPCLRequestNew.AgencyID);
            //==== 20161018 CMN Begin: Add List for delete file
            FileForDelete = new List<PCLResultFileStorageDetail>();
            //==== 20161018 CMN End.
            var pclresSearch = new PCLResultFileStorageDetailSearchCriteria
            {
                PatientPCLReqID = ObjPatientPCLImagingResult.PatientPCLReqID.Value,
                PCLExamTypeID = ObjPatientPCLImagingResult.PCLExamTypeID,
                IsExternalExam = true
            };
            var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsExtTask(pclresSearch);
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
            var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDExtTask(pclresSearch);
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

        void p_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Diagnosis":
                case "AgencyID":
                case "PCLExamTypeName":
                    PCLPropertyChange = true;
                    break;
            }
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
        //==== 20161018 CMN Begin: Combine Upload file
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
        //==== 20161018 CMN End.
        private IEnumerator<IResult> SaveResult()
        {
            //if (!PCLRequestValid())
            //{
            //    CanbtSave = true;
            //    yield break;
            //}
            if (ObjGetPCLResultFileStoreDetails.Count > Globals.ServerConfigSection.Pcls.MaxEchogramImageFile)
            {
                MessageBox.Show(eHCMSResources.K0457_G1_VuotQuaSLgFileToiDaChoPhep);
                this.HideBusyIndicator();
                yield break;
            }

            if (ObjPatientPCLImagingResult.PatientPCLReqID < 1)
            {
                var ResultFileStoreDetails = new AddPCLRequestExtWithDetailsTask(ObjPatientPCLRequestNew);
                yield return ResultFileStoreDetails;

                if (ObjPatientPCLImagingResult == null || ResultFileStoreDetails._PatientPCLReqExtID < 1)
                {
                    MessageBox.Show(eHCMSResources.A0681_G1_Msg_InfoLuuPhFail);
                    CanbtSave = true;
                    yield break;
                }
                ObjPatientPCLImagingResult.PatientPCLReqID = ResultFileStoreDetails._PatientPCLReqExtID;
                ObjPatientPCLRequestNew = ResultFileStoreDetails.resPatientPCLRequestNew;
            }
            else //chinh sua lai PatientPclrequest
            {
                if (PCLPropertyChange)
                {
                    var o = new PCLRequestExtUpdateTask(ObjPatientPCLRequestNew);
                    yield return o;
                }

            }

            List<PCLResultFileStorageDetail> lst = ObjGetPCLResultFileStoreDetails.Select(x => x.ObjectResult).Where(x => x.PCLResultFileItemID <= 0).ToList();
            //CanbtSave = true;
            //==== 20161018 CMN Begin: Combine Upload file
            //if (lst != null && lst.Count > 0)
            //{
            //    SaveImageToStore(lst);
            //}
            //else
            //{
            //    SaveDatabase();
            //}
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
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                    AfterSave(ObjPatientPCLRequestNew);
                                    Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
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
            //==== 20161018 CMN End.
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
                //20161018 CMN Begin: Change for new SaveFile Method
                //if (ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileItemID > 0)
                //{
                //    ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath = ImageStore + @"\" + ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultLocation + @"\" + ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileName;
                //    GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath, iType);
                //}
                //else
                //{
                //    DisplayResultFile(ObjPCLResultFileStorageDetailSelected.IOStream, iType);
                //}
                if (ObjPCLResultFileStorageDetailSelected.ObjectResult.PCLResultFileItemID > 0 && ObjPCLResultFileStorageDetailSelected.IOStream == null)
                {
                    GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.ObjectResult.FullPath, iType);
                }
                else
                {
                    DisplayResultFile(ObjPCLResultFileStorageDetailSelected.IOStream, iType);
                }
                //20161018 CMN End.
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLResultFileStorageDetailSelected != null)
            {
                //==== 20161018 CMN Begin: Add List for delete file
                FileForDelete.Add(ObjPCLResultFileStorageDetailSelected.ObjectResult);
                //==== 20161018 CMN End.
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
            isLoadingOperator = true;
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
                            if (items != null)
                            {
                                ObjGetVideoAndImage = new MemoryStream(items);
                                DisplayResultFile(ObjGetVideoAndImage, itype);
                            }
                            else
                            {
                                ObjGetVideoAndImage = null;
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
                    typeInfo.ObjWBitmapImage = ObjBitmapImage;
                    typeInfo.TypeOfBitmapImage = 1;
                };
                GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
            }
        }

        public void ClearBtn()
        {
            formWork = FormWork.Disable;
            if (ObjPatientPCLImagingResultBackup != null
                 && ObjPatientPCLImagingResultBackup.PatientPCLReqID > 0)
            {
                ObjPatientPCLImagingResult = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLImagingResultBackup);
                ObjPatientPCLRequestNew = aEMR.Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestBackup);
                LoadData(ObjPatientPCLRequestNew);
            }
            else
            {
                BackupNew();
            }
        }

        private bool _CanbtSave=true;
        public bool CanbtSave
        {
            get { return _CanbtSave && IsEnableButton; }
            set
            {
                _CanbtSave = value;
                NotifyOfPropertyChange(() => CanbtSave);
            }
        }

        public void btSaveOld()
        {
            CanbtSave = false;
            Coroutine.BeginExecute(SaveResult());
        }

        public void btSave_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ClickCount>1)
            {
                return;
            }
            Coroutine.BeginExecute(SaveResult());
        }

        private void SaveDatabase()
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            CanbtSave = false;
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
                                //Deployment.Current.Dispatcher.BeginInvoke(()=>
                                //{
                                //    Console.WriteLine("Current Thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                                //    MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                //});                                
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, "");
                                Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
                                //Globals.EventAggregator.Publish(new IPatientPCLDeptImagingExtResultDoneEvent());
                                AfterSave(ObjPatientPCLRequestNew);
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
                            CanbtSave = true;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void SaveImageToStore(List<PCLResultFileStorageDetail> lst)
        {
            //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            isLoadingDetail = false;
            CanbtSave = false;
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
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.T0432_G1_Error));
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
                            CanbtSave = true;
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
                    NotifyOfPropertyChange(() => CanbtSave);
                }
            }
        }

        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            //if (message != null)
            //{
            //    if (_currentView != null)
            //    {
            //        _currentView.SetObjectSource(null);
            //    }
            //    ObjBitmapImage = null;
            //    LoadData(message.PCLRequest);
            //}
        }

        #region ceate PatientPCLRequest_Ext
        private PatientPCLRequest_Ext _ObjPatientPCLRequestNew;
        public PatientPCLRequest_Ext ObjPatientPCLRequestNew
        {
            get
            {
                return _ObjPatientPCLRequestNew;
            }
            set
            {
                if (_ObjPatientPCLRequestNew != value)
                {
                    _ObjPatientPCLRequestNew = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestNew);
                }
            }
        }

        private PatientPCLRequest_Ext _ObjPatientPCLRequestNewBackup;
        public PatientPCLRequest_Ext ObjPatientPCLRequestNewBackup
        {
            get
            {
                return _ObjPatientPCLRequestNewBackup;
            }
            set
            {
                if (_ObjPatientPCLRequestNewBackup != value)
                {
                    _ObjPatientPCLRequestNewBackup = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestNewBackup);
                }
            }
        }

        private PatientPCLRequest_Ext _ObjPatientPCLRequestBackup;
        public PatientPCLRequest_Ext ObjPatientPCLRequestBackup
        {
            get
            {
                return _ObjPatientPCLRequestBackup;
            }
            set
            {
                if (_ObjPatientPCLRequestBackup != value)
                {
                    _ObjPatientPCLRequestBackup = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestBackup);
                }
            }
        }

        public bool PCLRequestValid()
        {
            //if (ObjPatientPCLRequestNew.AgencyID == null
            //   || ObjPatientPCLRequestNew.AgencyID.Value < 0)
            //{
            //    MessageBox.Show(eHCMSResources.A0379_G1_Msg_InfoChuaChonBVLamXNNgoaiVien);
            //    return false;
            //}

            //if (allPatientPCLRequestDetail_Ext.NewObject.Count < 0)
            //{
            //    MessageBox.Show(eHCMSResources.A0389_G1_Msg_InfoChuaChonLoaiSA);
            //    return false;
            //}

            //ObjPatientPCLRequestNew.PatientPCLRequestIndicatorsExt =
            //    ObjectCopier.DeepCopy(allPatientPCLRequestDetail_Ext.NewObject);
            if (ObjGetPCLResultFileStoreDetails == null || ObjGetPCLResultFileStoreDetails.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0384_G1_Msg_InfoChuaChonHASA);
                return false;
            }

            ObjPatientPCLImagingResult.AgencyID = ObjPatientPCLRequestNew.AgencyID;
            return true;
        }

        public PatientPCLRequestDetail_Ext createPatientPCLRequestDetail_Ext(PCLExamType item)
        {
            var requestDetail = new PatientPCLRequestDetail_Ext();
            requestDetail.PCLExamType = item;
            requestDetail.NumberOfTest = 1;
            requestDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;

            requestDetail.CreatedDate = DateTime.Now;
            requestDetail.DeptLocation = new DeptLocation();
            requestDetail.patientPCLRequest_Ext = ObjPatientPCLRequestNew;

            return requestDetail;
        }

        public void btEdit()
        {
            formState = FormState.New;
            formWork = FormWork.Edit;

        }

        public void btNew()
        {
            formState = FormState.New;
            formWork = FormWork.Edit;
            if (ObjPatientPCLRequestNewBackup == null)
            {
                KhoiTao();
            }
            else 
            {
                BackupNew();
            }
            
        }

        private void CreateNewPCLReq()
        {
            ObjPatientPCLRequestNew = NewPCLReq();
        }
        private PatientPCLRequest_Ext NewPCLReq()
        {
            var ObjStaff = Globals.LoggedUserAccount.Staff;

            var ObjNew = new PatientPCLRequest_Ext();
            ObjNew.PatientPCLRequestIndicatorsExt = new ObservableCollection<PatientPCLRequestDetail_Ext>();
            ObjNew.DeptID = Globals.ObjRefDepartment.DeptID;
            ObjNew.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;


            ObjNew.StaffIDName = ObjStaff.FullName;
            ObjNew.StaffID = ObjStaff.StaffID;
            ObjNew.DoctorStaffID = ObjStaff.StaffID;

            ObjNew.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            ObjNew.DoctorComments = "";
            ObjNew.IsCaseOfEmergency = false;

            ObjNew.IsExternalExam = false;
            ObjNew.IsImported = false;

            ObjNew.PatientServiceRecord = new PatientServiceRecord();
            if (ObjGetDiagnosisTreatmentByPtID != null
                && ObjGetDiagnosisTreatmentByPtID.ServiceRecID > 0)
            {
                ObjNew.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
            }

            ObjNew.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.StaffID;
            ObjNew.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
            ObjNew.PatientPCLRequestIndicatorsExt.Add(createPatientPCLRequestDetail_Ext(new PCLExamType
            {
                PCLExamTypeID = (long)AllLookupValues.V_PCLExamTypeExt.NgoaiVien
                ,
                PCLExamTypeName = Helpers.GetEnumDescription(AllLookupValues.V_PCLExamTypeExt.NgoaiVien)
            }));
            return ObjNew;
        }

        private DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
        public DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
        {
            get
            {
                return _ObjGetDiagnosisTreatmentByPtID;
            }
            set
            {
                if (_ObjGetDiagnosisTreatmentByPtID != value)
                {
                    _ObjGetDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => ObjGetDiagnosisTreatmentByPtID);
                }
            }
        }

        /*opt:-- 0: Query by PatientID, 1: Query by PtRegistrationID, 2: Query By NationalMedicalCode  */
        private void GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    long ServiceRecID = Registration_DataStorage.CurrentPatientRegistrationDetail != null ? Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID : 0; 
                    var contract = serviceFactory.ServiceInstance;
                    //HPT 03/10/2016: thêm tham số ServiceRecID để lấy chẩn đoán cho phiếu chỉ định đúng với dịch vụ khám bệnh (đa khoa) mà hàm này dùng chung nên ở những chỗ ngoài màn hình phiếu chỉ định cận lâm sàng, truyền tham số ServiceRecID = null
                    contract.BeginGetDiagnosisTreatmentByPtID(patientID, PtRegistrationID, null, opt, true, V_RegistrationType, ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentByPtID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                ObjGetDiagnosisTreatmentByPtID = results.ToObservableCollection()[0];

                                ObjPatientPCLRequestNew.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID;
                                ObjPatientPCLRequestNew.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
                                ObjPatientPCLRequestNew.Diagnosis = string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis.Trim() : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal.Trim();

                            }
                            else
                            {

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;                            
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private FormState _formState;
        public FormState formState
        {
            get { return _formState; }
            set
            {
                if (_formState != value)
                {
                    _formState = value;
                    NotifyOfPropertyChange(() => formState);
                }
            }
        }

        private FormWork _formWork;
        public FormWork formWork
        {
            get { return _formWork; }
            set
            {
                if (_formWork != value)
                {
                    _formWork = value;
                    NotifyOfPropertyChange(() => formWork);
                    switch (formWork)
                    {
                        case FormWork.Disable:
                            btNewIsEnabled = true;
                            IsEnableButton = false;
                            IsEdit = false;
                            break;
                        case FormWork.Edit:
                            btNewIsEnabled = false;
                            IsEnableButton = true;
                            IsEdit = true;
                            break;
                    }

                }
            }
        }

        private bool _btNewIsEnabled;
        public bool btNewIsEnabled
        {
            get { return _btNewIsEnabled; }
            set
            {
                if (_btNewIsEnabled != value)
                {
                    _btNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btNewIsEnabled);
                }
            }
        }

        private bool _FormIsEnabled = true;
        public bool FormIsEnabled
        {
            get { return _FormIsEnabled; }
            set
            {
                if (_FormIsEnabled != value)
                {
                    _FormIsEnabled = value;
                    NotifyOfPropertyChange(() => FormIsEnabled);
                }
            }
        }

        private bool _IsNew = true;
        public bool IsNew
        {
            get { return _IsNew; }
            set
            {
                if (_IsNew != value)
                {
                    _IsNew = value;
                    NotifyOfPropertyChange(() => IsNew);
                }
            }
        }

        private bool _IsEdit = true;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        #endregion
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

    public class PCLResultFileStorageDetailClient
    {
        public Stream IOStream { get; set; }
        public PCLResultFileStorageDetail ObjectResult { get; set; }
    }

    public enum FormState
    {
        New = 1,
        Edit = 2,
        Restore = 3
    }
    public enum FormWork
    {
        Disable = 1,
        Edit = 2
    }
}
