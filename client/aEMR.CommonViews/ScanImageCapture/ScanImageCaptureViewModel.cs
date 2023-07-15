using System.ComponentModel.Composition;
using aEMR.ViewContracts;

using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using DataEntities;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Shapes;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Controls;
using System.IO;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System;
using aEMR.Common.BaseModel;
using aEMR.Common;
using aEMR.Common.Collections;
using Microsoft.Win32;
/*
 * 20190727 #001 TTM:   BM 0012990: Loại bỏ điều kiện kiểm tra PtRegistrationID vì thông tin SCAN là thông tin của bệnh nhân không theo đăng ký nữa.
 */
namespace aEMR.CommonViews
{
    [Export(typeof(IScanImageCapture)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ScanImageCaptureViewModel : ViewModelBase, IScanImageCapture
    {
        public long StaffID { get;  set; }
        public long PtRegistrationID { get;  set; }
        public long PatientID { get;  set; }
        public string PatientCode { get;  set; }


        public WriteableBitmap GetCapturedImage() { return gSnapshots == null || gSnapshots.SelectedItem == null ? null : gSnapshots.SelectedItem as WriteableBitmap; }
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [ImportingConstructor]
        public ScanImageCaptureViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }
        #region Properties
        private ComboBox gAudioSources { get; set; }
        private Rectangle gWebcam { get; set; }
        private Button btnStartTwainDSM { get; set; }
        private Button btnPause { get; set; }
        private Button btnScanWithUI { get; set; }
        private Button btnStop { get; set; }
        //private Image gMap { get; set; }
        private Image gScanImageCtrl { get; set; }
        private Button gCaptureWebcam { get; set; }
        private ListBox gSnapshots { get; set; }
        private ObservableCollection<WriteableBitmap> gImages = new ObservableCollection<WriteableBitmap>();
        protected WpfTwain TwainInterface = null;

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
        private bool _IsLoadCompleted = false;
        public bool IsLoadCompleted
        {
            get
            {
                return _IsLoadCompleted;
            }
            set
            {
                _IsLoadCompleted = value;
                NotifyOfPropertyChange(() => IsLoadCompleted);
            }
        }
        #endregion
        #region Events

        protected override void OnViewLoaded(object theView)
        {
            base.OnViewLoaded(theView);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Start_TwainDSMLib();
            GetSavedImageFilesListingFromServer();
        }

        
        private void GetSavedImageFilesListingFromServer()
        {
            //▼===== #001
            //if (PtRegistrationID == 0)
            //    return;
            //▲===== #001
            this.DlgShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetSavedScanFileStorageDetails_ByPatientID(PatientID, Globals.DispatchCallback((asynResult) =>
                        {
                            try
                            {                                                        
                                var scanFilesAlreadySaved = contract.EndGetSavedScanFileStorageDetails_ByPatientID(asynResult).ToList();
                                if (scanFilesAlreadySaved != null)
                                {
                                    SavedScanFileStoreDetailListing = scanFilesAlreadySaved.ToObservableCollection();
                                }
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            };
                        }), null);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        ClientLoggerHelper.LogError(ex.ToString());
                    }
                }
            });
            t.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            GetSelectedImageToBeSaved();
            base.OnDeactivate(close);
        }

        private void TwainWin_TwainTransferReady(WpfTwain sender, List<BitmapSource> imageSources)
        {
            UpdateScanButtons(!TwainInterface.IsScanning);
            foreach (BitmapSource ims in imageSources)
            {
                AddImageThumbnail(ims);
            }

            // alteratively you can use imageSources[0] if the program should only support one-image scans
            //this.Activate();
        }

        private void TwainUIClose(WpfTwain sender)
        {
            // scan is canceled
            UpdateScanButtons(true);
        }

        void UpdateScanButtons(bool enabled)
        {
            //SelectButton.IsEnabled = ScanButton.IsEnabled = ScanUIButton.IsEnabled = enabled;
        }

        public void AddImageThumbnail(BitmapSource src)
        {
            WriteableBitmap mWriteableBitmap = new WriteableBitmap(src);
            gImages.Add(mWriteableBitmap);

            gScanImageCtrl.Source = src;
        }


        public void BtnStartTwainDSM_Loaded(object sender, RoutedEventArgs e)
        {
            btnStartTwainDSM = sender as Button;
        }
        public void butPause_Loaded(object sender, RoutedEventArgs e)
        {
            btnPause = sender as Button;
        }
        public void BtnBeginScanWithUI_Loaded(object sender, RoutedEventArgs e)
        {
            btnScanWithUI = sender as Button;
        }
        public void butStop_Loaded(object sender, RoutedEventArgs e)
        {
            btnStop = sender as Button;
        }

        public void ScanImage_Loaded(object sender, RoutedEventArgs e)
        {
            gScanImageCtrl = sender as Image;
        }

        public void BtnBeginScan_Loaded(object sender, RoutedEventArgs e)
        {
            gCaptureWebcam = sender as Button;
            gCaptureWebcam.Content = eHCMSResources.K1688_G1_CaptureImg;
        }

        public void Snapshots_Loaded(object sender, RoutedEventArgs e)
        {
            gSnapshots = sender as ListBox;
            gSnapshots.ItemsSource = gImages;
        }

        public void BtnStartTwainDSM_Click(object btnSource)
        {
            Start_TwainDSMLib();
        }

        private void Start_TwainDSMLib()
        {
            try
            {
                Window myDlgWin = this.GetView() as Window;
                TwainInterface = new WpfTwain(myDlgWin);
                TwainInterface.TwainTransferReady += new TwainTransferReadyHandler(TwainWin_TwainTransferReady);
                TwainInterface.TwainCloseRequest += new TwainEventHandler(TwainUIClose);
                TwainInterface.TheHostWindow.Loaded += TheTwainHostWindow_Loaded;
            }
            catch(Exception ex)
            {
                ClientLoggerHelper.LogError("Failed to start TWAINDSM Lib.");
                ClientLoggerHelper.LogError(ex.ToString());
            }
        }

        private void TheTwainHostWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoadCompleted = TwainInterface.IsLoadCompleted;
        }

        public void BtnBeginScan_Click(object btnSource)
        {
            TwainInterface.Acquire(false);
        }
        public void btnSave_Click(object btnSource)
        {
            //▼===== #001
            //if (PtRegistrationID == 0)
            //    return;
            //▲===== #001
            SaveScanImageFiletoServer(PtRegistrationID);
        }
        public void btSave_Click(object btnSource)
        {
            //▼===== #001
            //if (PtRegistrationID == 0)
            //    return;
            //▲===== #001
            SaveImage(PtRegistrationID);
        }
        private void SaveScanImageFiletoServer(long PtRegistrationID)
        {
            GetSelectedImageToBeSaved();
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginAddScanFileStorageDetails(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), PatientID, PtRegistrationID,
                            PatientCode, ScanImageFileToBeSaved, ScanImageFileToBeDeleted, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool resOK = contract.EndAddScanFileStorageDetails(asyncResult);
                                    if (resOK)
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1562_G1_DaLuu);
                                        ScanImageFileToBeSaved = new List<ScanImageFileStorageDetail>();
                                        ScanImageFileToBeDeleted = new List<ScanImageFileStorageDetail>();
                                        GetSavedImageFilesListingFromServer();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogError(ex.ToString());
                        this.DlgHideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        private void SaveImage(long PtRegistrationID)
        {
            GetImageToBeSaved();
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginAddScanFileStorageDetails(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), PatientID, PtRegistrationID,
                            PatientCode, ScanImageFileToBeSaved, ScanImageFileToBeDeleted, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool resOK = contract.EndAddScanFileStorageDetails(asyncResult);
                                    if (resOK)
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1562_G1_DaLuu);
                                        ScanImageFileToBeSaved = new List<ScanImageFileStorageDetail>();
                                        ScanImageFileToBeDeleted = new List<ScanImageFileStorageDetail>();
                                        GetSavedImageFilesListingFromServer();
                                        ObsFileStoreDetails.Clear();
                                        ObjBitmapImage = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogError(ex.ToString());
                        this.DlgHideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        public void BtnBeginScanWithUI_Click()
        {
            TwainInterface.Acquire(true);
        }

        public void lnkDeleteUGClick(object sender, RoutedEvent e)
        {
            var mDelSource = (sender as Button).DataContext as WriteableBitmap;
            gImages.Remove(mDelSource);
            Snapshots_SelectionChanged(null, null);
        }
        public void Snapshots_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (gSnapshots.SelectedItem != null)
            {
                gScanImageCtrl.Source = gSnapshots.SelectedItem as WriteableBitmap;                
            }
            else
            {
                gScanImageCtrl.Source = null;                
            }
        }
        #endregion
        #region Methods
        private List<WriteableBitmap> GetSelectedImage()
        {
            List<WriteableBitmap> SelectedImage = new List<WriteableBitmap>();
            if (gSnapshots != null)
            {
                for (int i = 0; i < gSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(gSnapshots.ItemContainerGenerator.ContainerFromItem(gSnapshots.Items[i]));
                    List<Control> mChildren = gSnapshots.GetChildrenByType<Control>();
                    CheckBox mCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbImage"));
                    if (mCheckBox.IsChecked == true)
                    {
                        SelectedImage.Add(gImages[i]);
                    }
                }
            }
            return SelectedImage;
        }
        private void DeleteSelectedImage(List<WriteableBitmap> SelectedImage)
        {
            foreach (WriteableBitmap mImage in SelectedImage)
                gImages.Remove(mImage);
        }

        private List<ScanImageFileStorageDetail> _ScanImageFileToBeSaved = null;
        public List<ScanImageFileStorageDetail> ScanImageFileToBeSaved
        {  
            get
            {                
                return _ScanImageFileToBeSaved;
            }
            set
            {
                if (_ScanImageFileToBeSaved != value)
                {
                    _ScanImageFileToBeSaved = value;
                    NotifyOfPropertyChange(() => ScanImageFileToBeSaved);
                }
            }
        }
        public void ClearSelectedImage()
        {
            List<WriteableBitmap> mSelectedImage = GetSelectedImage();
            DeleteSelectedImage(mSelectedImage);
        }
        private void GetSelectedImageToBeSaved()
        {
            if (gSnapshots == null)
            {
                return;
            }

            if (_ScanImageFileToBeSaved == null)
            {
                _ScanImageFileToBeSaved = new List<ScanImageFileStorageDetail>();
            }
            else
            {
                ScanImageFileToBeSaved.Clear();
            }
            for (int i = 0; i < gSnapshots.Items.Count; i++)
            {
                ListBoxItem mListBoxItem = (ListBoxItem)(gSnapshots.ItemContainerGenerator.ContainerFromItem(gSnapshots.Items[i]));
                List<CheckBox> mChildren = mListBoxItem.GetChildrenByType<CheckBox>();
                CheckBox mCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbImage"));
                if (mCheckBox.IsChecked == true)
                {
                    var ImageArray = new MemoryStream();

                    //BmpBitmapEncoder mEncoder = new BmpBitmapEncoder();

                    JpegBitmapEncoder mEncoder = new JpegBitmapEncoder();
                    mEncoder.Frames.Add(BitmapFrame.Create(gImages[i]));
                    mEncoder.Save(ImageArray);
                    ScanImageFileStorageDetail aPCLResultFileStorageDetail = new ScanImageFileStorageDetail();                        
                    aPCLResultFileStorageDetail.File = ImageArray.ToArray();
                    aPCLResultFileStorageDetail.V_ScanImageOfType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                    _ScanImageFileToBeSaved.Add(aPCLResultFileStorageDetail);
                }
            }                        
        }
        private void GetImageToBeSaved()
        {
            if (_ScanImageFileToBeSaved == null)
            {
                _ScanImageFileToBeSaved = new List<ScanImageFileStorageDetail>();
            }
            else
            {
                ScanImageFileToBeSaved.Clear();
            }
            if (ObsFileStoreDetails != null && ObsFileStoreDetails.Count > 0)
            {
                foreach (var item in ObsFileStoreDetails)
                {
                    item.IOStream = null; //20190423 TBL: Neu khong set IOStream = null thi khi luu se bi Exception
                    _ScanImageFileToBeSaved.Add(item);
                }
            }
        }

        public void ClearAllCapturedImage()
        {
            gImages.Clear();
        }


        private ScanImageFileStorageDetail _savedScanFileStorageDetailSelected;
        public ScanImageFileStorageDetail SavedScanFileStorageDetailSelected
        {
            get
            {
                return _savedScanFileStorageDetailSelected;
            }
            set
            {
                if (_savedScanFileStorageDetailSelected != value)
                {
                    _savedScanFileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => SavedScanFileStorageDetailSelected);
                }
            }
        }

        private ScanImageFileStorageDetail _FileStorageDetailSelected;
        public ScanImageFileStorageDetail FileStorageDetailSelected
        {
            get
            {
                return _FileStorageDetailSelected;
            }
            set
            {
                if (_FileStorageDetailSelected != value)
                {
                    _FileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => FileStorageDetailSelected);
                }
            }
        }

        private List<DataEntities.ScanImageFileStorageDetail> _ScanImageFileToBeDeleted = new List<ScanImageFileStorageDetail>();
        public List<DataEntities.ScanImageFileStorageDetail> ScanImageFileToBeDeleted 
        {
            get { return _ScanImageFileToBeDeleted; }
            set
            {
                _ScanImageFileToBeDeleted = value;
                NotifyOfPropertyChange(() => ScanImageFileToBeDeleted);
            }
        }


        private ObservableCollection<DataEntities.ScanImageFileStorageDetail> _savedScanFileStorageDetailListing;
        public ObservableCollection<DataEntities.ScanImageFileStorageDetail> SavedScanFileStoreDetailListing
        {
            get { return _savedScanFileStorageDetailListing; }
            set
            {
                _savedScanFileStorageDetailListing = value;
                NotifyOfPropertyChange(() => SavedScanFileStoreDetailListing);
            }
        }

        private ObservableCollection<DataEntities.ScanImageFileStorageDetail> _ObsFileStorageDetails;
        public ObservableCollection<DataEntities.ScanImageFileStorageDetail> ObsFileStoreDetails
        {
            get { return _ObsFileStorageDetails; }
            set
            {
                _ObsFileStorageDetails = value;
                NotifyOfPropertyChange(() => ObsFileStoreDetails);
            }
        }

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

        public void dtgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SavedScanFileStoreDetailListing != null && SavedScanFileStorageDetailSelected != null)
            {                
                GetScanImageFileFromServer(SavedScanFileStorageDetailSelected.FullPath);
            }
        }

        private void DisplayScanImageFile(Stream pStream)
        {
            if (pStream != null)
            {
                gScanImageCtrl.Source = Globals.GetWriteableBitmapFromStream(pStream);
            }
            else
            {
                gScanImageCtrl.Source = null;
            }
        }

        private void GetScanImageFileFromServer(string path)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1120_G1_TaiFile) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var scanImageFile = contract.EndGetVideoAndImage(asyncResult);
                            if (scanImageFile != null)
                            {
                                ObjGetVideoAndImage = new MemoryStream(scanImageFile);
                                DisplayScanImageFile(ObjGetVideoAndImage);
                            }
                            else
                            {
                                ObjGetVideoAndImage = null;
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

        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.ScanImageFileStorageDetail p = (selectedItem as DataEntities.ScanImageFileStorageDetail);

            if (p == null)
            {
                MessageBox.Show("CANNOT Delete NULL Scan Image File Storage Details from List.");
                return;
            }

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ScanImageFileName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ScanImageFileToBeDeleted.Add(p);

                SavedScanFileStoreDetailListing.Remove(p);
            }

        }

        public void hplXoa_Click(object selectedItem)
        {
            DataEntities.ScanImageFileStorageDetail p = (selectedItem as DataEntities.ScanImageFileStorageDetail);
            if (p == null)
            {
                MessageBox.Show("CANNOT Delete NULL Scan Image File Storage Details from List.");
                return;
            }
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ScanImageFileName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObsFileStoreDetails.Remove(p);
            }
        }

        public void btnBrowse()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            //20221019 BLQ: Thêm jpeg vào bộ lọc và All file để có thẽ chọn nhiều file khác
            dlg.Filter = "Images (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            bool? retval = dlg.ShowDialog();

            if (retval != null && retval == true)
            {
                Stream stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                ScanImageFileStorageDetail itemClient = new ScanImageFileStorageDetail();
                itemClient.IOStream = stream;
                itemClient.File = bytes;
                itemClient.V_ScanImageOfType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                int index = 0;
                if (dlg.FileName.LastIndexOf(".") > 0)
                {
                    index = dlg.FileName.LastIndexOf(".");
                }
                itemClient.ScanImageFileName = Guid.NewGuid().ToString();
                if (ObsFileStoreDetails == null)
                {
                    ObsFileStoreDetails = new ObservableCollection<ScanImageFileStorageDetail>();
                }
                ObsFileStoreDetails.Add(itemClient);
            }
        }

        public void lnkView_Click(object sender, RoutedEventArgs e)
        {
            if (FileStorageDetailSelected != null && FileStorageDetailSelected.IOStream != null)
            {
                ObjBitmapImage = Globals.GetWriteableBitmapFromStream(FileStorageDetailSelected.IOStream);
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

        public void hplDiagnoticsImgScan()
        {
            if (gScanImageCtrl == null || gScanImageCtrl.Source == null)
            {
                return;   
            }
            Action<IImageDisgnosticExplorer> onInitDlg = delegate (IImageDisgnosticExplorer typeInfo)
            {
                typeInfo.TypeOfBitmapImage = 1;
                typeInfo.ObjWBitmapImage = gScanImageCtrl.Source as WriteableBitmap;
            };
            GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
        }
        #endregion
    }
}