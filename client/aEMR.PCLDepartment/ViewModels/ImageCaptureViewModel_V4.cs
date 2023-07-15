using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using DataEntities;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Controls;
using System.IO;
using DirectShowLib;
using aEMR.Common;
using aEMR.Common.Collections;
using System;
using aEMR.Infrastructure;
using System.Diagnostics;
using System.Text;
/*
* 20190611 #001 TTM:   BM 0010772: Capture là lưu ảnh tại máy trạm (có cấu hình)
* 20220808 #002 TNHX: 2002 Tăng kích cỡ khung hình 640x480
* 20220921 #003 BLQ: Thêm chức năng tự check In số lượng hình theo cấu hình loại report
*/
//[Obsolete("ImageCaptureViewModel is deprecated, please use ImageCapture_V2ViewModel instead")]

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IImageCapture_V4)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageCapture_V4ViewModel : Conductor<object>, IImageCapture_V4
    {
        public WriteableBitmap GetCapturedImage() { return gSnapshots == null || gSnapshots.SelectedItem == null ? null : gSnapshots.SelectedItem as WriteableBitmap; }

        [ImportingConstructor]
        public ImageCapture_V4ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }

        #region Properties
        private Button StartStopCaptureDevBtn { get; set; }
        private Button btnPause { get; set; }
        private Button btnRecord { get; set; }
        private Button btnStop { get; set; }
        public Image gMap { get; set; }
        private Button btnCaptureImage { get; set; }
        private ComboBox CboCaptureDevices { get; set; }
        private ListBox gSnapshots { get; set; }
        private Button gSaveImage { get; set; }
        private WPFMediaKit.DirectShow.Controls.VideoCaptureElement VideoCaptureElemCtrl { get; set; }
        private ObservableCollection<WriteableBitmap> _gImages = new ObservableCollection<WriteableBitmap>();
        public ObservableCollection<WriteableBitmap> gImages
        {
            get => _gImages; set
            {
                _gImages = value;
                NotifyOfPropertyChange(() => gImages);
            }
        }

        private string strLocalStorageFolderPath = "";

        private ObservableCollection<DsDevice> _gInputDeviceCollection;
        public ObservableCollection<DsDevice> gInputDeviceCollection
        {
            get => _gInputDeviceCollection;
            set
            {
                _gInputDeviceCollection = value;
                NotifyOfPropertyChange(() => gInputDeviceCollection);
            }
        }
        private DsDevice _captureInputDevice;
        public DsDevice CaptureInputDevice
        {
            get => _captureInputDevice;
            set
            {
                _captureInputDevice = value;
                NotifyOfPropertyChange(() => CaptureInputDevice);
            }
        }

        private bool m_bCaptureManInitialized = false;

        private bool m_bMediaCapturePlaying = false;

        private void GetAllCaptureDevicesForDeviceComboBox()
        {
            gInputDeviceCollection = WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.ToObservableCollection();
            if (gInputDeviceCollection != null && gInputDeviceCollection.Count > 0)
            {
                CaptureInputDevice = gInputDeviceCollection.FirstOrDefault();
            }
        }
        #endregion

        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllCaptureDevicesForDeviceComboBox();
            CheckAndSetLocalStorageFolderPath();
        }

        protected override void OnDeactivate(bool close)
        {
            DeActivateCaptureDevice();
            base.OnDeactivate(close);
        }

        // TxD 13/12/2019: Added the following method to check if the Config Folder for saving local files DOES EXIST
        //                  IF NOT then Default it to Drive C for now. Further coding is required to SELECT the Drive with largest FREE storage space if the configed drive not there
        private void CheckAndSetLocalStorageFolderPath()
        {
            try
            {
                strLocalStorageFolderPath = Globals.ServerConfigSection.Pcls.ImageCaptureFileLocalStorePath;
                char chConfigDrive = strLocalStorageFolderPath[0];
                var allDrives = DriveInfo.GetDrives();
                bool bFoundConfigDrive = false;
                foreach (var theDrive in allDrives)
                {
                    if (theDrive.DriveType == DriveType.Fixed && theDrive.Name[0] == chConfigDrive)
                        return;
                }
                // TxD 13/12/2019: For the time being If the Config Drive for storage DOES NOT EXIST ON Client PC then Default it to Drive C
                if (!bFoundConfigDrive)
                {
                    StringBuilder sb1 = new StringBuilder(strLocalStorageFolderPath);
                    sb1[0] = 'C';
                    strLocalStorageFolderPath = sb1.ToString();
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.Message);
            }
        }

        private void DeActivateCaptureDevice()
        {
            if (m_bCaptureManInitialized && VideoCaptureElemCtrl != null)
            {
                if (m_bMediaCapturePlaying && VideoCaptureElemCtrl.IsPlaying)
                {
                    VideoCaptureElemCtrl.Stop();
                }
                //VideoCaptureElemCtrl.VideoCaptureDevice = null;
                m_bCaptureManInitialized = false;
            }
        }

        public void VideoCaptureElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (m_bCaptureManInitialized && VideoCaptureElemCtrl != null && VideoCaptureElemCtrl.IsPlaying)
            {
                //StartStopCaptureDevBtn.Content = eHCMSResources.S0729_G1_StopCamera;
                //btnCaptureImage.IsEnabled = true;
                //m_bMediaCapturePlaying = true;
            }
        }

        public void VideoSourcesCbo_Loaded(object sender, RoutedEventArgs e)
        {
            CboCaptureDevices = sender as ComboBox;
        }

        public void VideoCaptureElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_bCaptureManInitialized == false)
            {
                VideoCaptureElemCtrl = sender as WPFMediaKit.DirectShow.Controls.VideoCaptureElement;
                VideoCaptureElemCtrl.UpdateLayout();
                m_bCaptureManInitialized = true;
            }
        }

        public void StartStopCaptureDev_Loaded(object sender, RoutedEventArgs e)
        {
            StartStopCaptureDevBtn = sender as Button;
        }

        public void butPause_Loaded(object sender, RoutedEventArgs e)
        {
            btnPause = sender as Button;
        }

        public void butRecord_Loaded(object sender, RoutedEventArgs e)
        {
            btnRecord = sender as Button;
        }

        public void butStop_Loaded(object sender, RoutedEventArgs e)
        {
            btnStop = sender as Button;
        }

        public void CaptureImageBtn_Loaded(object sender, RoutedEventArgs e)
        {
            btnCaptureImage = sender as Button;
            btnCaptureImage.Content = eHCMSResources.K1688_G1_CaptureImg;
        }

        public void Snapshots_Loaded(object sender, RoutedEventArgs e)
        {
            gSnapshots = sender as ListBox;
            gSnapshots.ItemsSource = gImages;
        }

        public void SaveImage_Loaded(object sender, RoutedEventArgs e)
        {
            gSaveImage = sender as Button;
            gSaveImage.Content = eHCMSResources.S0544_G1_Save;
        }

        public void StartStopWebcam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!VideoCaptureElemCtrl.IsPlaying)
                {
                    CaptureInputDevice = (DsDevice)CboCaptureDevices.SelectedItem;
                    VideoCaptureElemCtrl.VideoCaptureDevice = CaptureInputDevice;
                    VideoCaptureElemCtrl.Play();
                    StartStopCaptureDevBtn.Content = eHCMSResources.S0729_G1_StopCamera;
                    btnCaptureImage.IsEnabled = true;
                    m_bMediaCapturePlaying = true;
                }
                else
                {
                    VideoCaptureElemCtrl.Stop();

                    StartStopCaptureDevBtn.Content = eHCMSResources.S0724_G1_StartCamera;
                    btnCaptureImage.IsEnabled = false;

                    m_bMediaCapturePlaying = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CANNOT Start/Stop Media Device.");
                ClientLoggerHelper.LogError(ex.ToString());
            }
        }

        private void CaptureImageFromDevice()
        {
            //▼====: #002
            int ImgWidth = 480;
            int ImgHeight = 360;
            double dpi = 96d;
            RenderTargetBitmap bmp = new RenderTargetBitmap(ImgWidth, ImgHeight,
                dpi, dpi, PixelFormats.Default);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(VideoCaptureElemCtrl);
                context.DrawRectangle(brush, null, new Rect(new Point(),
                    new Size(VideoCaptureElemCtrl.ActualWidth, VideoCaptureElemCtrl.ActualHeight)));

            }
            visual.Transform = new ScaleTransform(ImgWidth / VideoCaptureElemCtrl.ActualWidth, ImgHeight / VideoCaptureElemCtrl.ActualHeight);

            bmp.Render(visual);

            //RenderTargetBitmap bmp = new RenderTargetBitmap((int)VideoCaptureElemCtrl.ActualWidth, (int)VideoCaptureElemCtrl.ActualHeight, 96, 96, PixelFormats.Default);
            //bmp.Render(VideoCaptureElemCtrl);
            //▲====: #002
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            WriteableBitmap mWriteableBitmap = new WriteableBitmap(bmp);
            gImages.Add(mWriteableBitmap);
            //▼====== #001
            if (Globals.ServerConfigSection.Pcls.SaveImgWhenCapturing_Local)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
                SaveCapturedImageToLocalDrive(PatientCode, gImages.Last());
                this.HideBusyIndicator();
            }
            //▲====== #001
        }

        public void CaptureImageBtn_Click(object sender, RoutedEventArgs e)
        {
            CaptureImageFromDevice();
        }

        public void CaptureVideoImage()
        {
            if (!m_bMediaCapturePlaying)
                return;
            try
            {
                CaptureImageFromDevice();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I);
            }
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
                //gMap.Source = gSnapshots.SelectedItem as WriteableBitmap;
                //gSaveImage.IsEnabled = true;
            }
            else
            {
                //gMap.Source = null;
                //gSaveImage.IsEnabled = false;
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
                    CheckBox mSaveCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbSaveImage"));
                    CheckBox mRepCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbRepImage"));
                    if (mSaveCheckBox.IsChecked == true || mRepCheckBox.IsChecked == true)
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

        //private List<PCLResultFileStorageDetail> _FileForStore = new List<PCLResultFileStorageDetail>();
        //public List<PCLResultFileStorageDetail> FileForStore
        //{
        //    get { return GetSelectedImageForStore(""); }
        //    set
        //    {
        //        if (_FileForStore != value)
        //        {
        //            _FileForStore = value;

        //            NotifyOfPropertyChange(() => FileForStore);
        //        }
        //    }
        //}

        // TxD 22/02/2019: The following method is to replace for the OLD FileForStore attribute to enable passing in parameters used for SAVING Images to local drive
        public List<PCLResultFileStorageDetail> GetFileForStore(string strPatientCode, bool SaveImageToLocalDrv, bool SaveImageToLocalDrv_WhenCapturing, int CountImagePrintSelect)
        {
            return GetSelectedImageForStore(strPatientCode, SaveImageToLocalDrv, SaveImageToLocalDrv_WhenCapturing, CountImagePrintSelect);
        }

        public void ClearSelectedImage()
        {
            List<WriteableBitmap> mSelectedImage = GetSelectedImage();
            DeleteSelectedImage(mSelectedImage);
        }

        private List<PCLResultFileStorageDetail> GetSelectedImageForStore(string strPatientCode, bool SaveImageToLocalDrv, bool SaveImageToLocalDrv_WhenCapturing
            //▼====: #003
            , int CountImagePrintSelect)
            //▲====: #003
        {
            List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = new List<PCLResultFileStorageDetail>();
            if (gSnapshots != null)
            {
                for (int i = 0; i < gSnapshots.Items.Count; i++)
                {
                    //▼====== #001
                    if (SaveImageToLocalDrv && !SaveImageToLocalDrv_WhenCapturing)
                    {
                        SaveCapturedImageToLocalDrive(strPatientCode, gImages[i]);
                    }
                    //▲====== #001
                    ListBoxItem mListBoxItem = (ListBoxItem)(gSnapshots.ItemContainerGenerator.ContainerFromItem(gSnapshots.Items[i]));
                    List<CheckBox> mChildren = mListBoxItem.GetChildrenByType<CheckBox>();
                    CheckBox mSaveCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbSaveImage"));
                    CheckBox mRepCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbRepImage"));
                    if (mSaveCheckBox.IsChecked == true || mRepCheckBox.IsChecked == true)
                    {
                        var ImageArray = new MemoryStream();
                        BmpBitmapEncoder mEncoder = new BmpBitmapEncoder();
                        mEncoder.Frames.Add(BitmapFrame.Create(gImages[i]));
                        mEncoder.Save(ImageArray);

                        PCLResultFileStorageDetail aPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                        aPCLResultFileStorageDetail.IsImage = true;
                        aPCLResultFileStorageDetail.IsUseForPrinting = mRepCheckBox.IsChecked.GetValueOrDefault();
                        aPCLResultFileStorageDetail.File = ImageArray.ToArray();
                        aPCLResultFileStorageDetail.V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                        mPCLResultFileStorageDetail.Add(aPCLResultFileStorageDetail);
                    }
                }
                //▼====: #003
                if (!mPCLResultFileStorageDetail.Any(x => x.IsUseForPrinting) && CountImagePrintSelect > 0)
                {
                    foreach (var item in mPCLResultFileStorageDetail)
                    {
                        if (mPCLResultFileStorageDetail.IndexOf(item) < CountImagePrintSelect)
                        {
                            item.IsUseForPrinting = true;
                        }
                    }
                }
                //▲====: #003
            }
            return mPCLResultFileStorageDetail;
        }
        public void ClearAllCapturedImage()
        {
            gImages.Clear();
        }

        public void ViewCapturedImage(object sender)
        {
            ListBox theListBox = sender as ListBox;
            if (theListBox == null)
                return;
            if (theListBox.SelectedItem == null)
                return;

            WriteableBitmap ObjBitmapImage = theListBox.SelectedItem as WriteableBitmap;

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

        private string CreateNameAndPathForImageFile(string strPatientCode, out string strFolderPath, string PCLStorePool, string SubFolderName = "Images")
        {
            string FileExtension = ".jpg";
            strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PCLStorePool, SubFolderName, DateTime.Now.ToString("yyMMdd")));
            if (!System.IO.Directory.Exists(strFolderPath))
                System.IO.Directory.CreateDirectory(strFolderPath);
            string strFileName = String.Join("-", new string[] { DateTime.Now.ToString("yyMMddHHmmssfff"), strPatientCode, Guid.NewGuid().ToString(), FileExtension });
            return strFileName;
        }

        public bool SaveCapturedImageToLocalDrive(string strPatientCode, WriteableBitmap theCapturedImage)
        {
            //string strPCLFileLocalStorePath = Globals.ServerConfigSection.Pcls.ImageCaptureFileLocalStorePath;
            string strFolderPath = "";
            try
            {
                strFolderPath = "";
                string strFileName = CreateNameAndPathForImageFile(strPatientCode, out strFolderPath, strLocalStorageFolderPath);
                string strFolderPathAndFileName = Path.Combine(strFolderPath, strFileName);
                if (!File.Exists(strFolderPathAndFileName) && theCapturedImage != null)
                {
                    using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                    {
                        var ImageArray = new MemoryStream();
                        BmpBitmapEncoder mEncoder = new BmpBitmapEncoder();
                        mEncoder.Frames.Add(BitmapFrame.Create(theCapturedImage));
                        mEncoder.Save(ImageArray);
                        PCLResultFileStorageDetail aPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                        aPCLResultFileStorageDetail.IsImage = true;
                        aPCLResultFileStorageDetail.File = ImageArray.ToArray();
                        byte[] ImageBuffer = ImageArray.ToArray();
                        fs.Write(ImageBuffer, 0, ImageBuffer.Length);
                        fs.Close();
                        return true;
                    }
                }
                MessageBox.Show(eHCMSResources.Z2724_G1_KhongTheLuuAnhTaiMayTram);
                return false;
            }
            catch (Exception ex)
            {
                if (Globals.ServerConfigSection.Pcls.SaveImgWhenCapturing_Local)
                {
                    this.HideBusyIndicator();
                }
                //DeleteStoredImageFile(ListFilePathForDeletion);
                //DeleteFile(ResultFile);
                throw (ex);
            }
        }
        #endregion

        #region Properties
        public string _PatientCode;
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
        #endregion
    }
}
