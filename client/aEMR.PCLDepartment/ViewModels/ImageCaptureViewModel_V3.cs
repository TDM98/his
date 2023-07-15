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

//[Obsolete("ImageCaptureViewModel is deprecated, please use ImageCapture_V2ViewModel instead")]

namespace aEMR.PCLDepartment.ViewModels
{    
    [Export(typeof(IImageCapture_V3)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageCapture_V3ViewModel : Conductor<object>, IImageCapture_V3
    {
        public WriteableBitmap GetCapturedImage() { return gSnapshots == null || gSnapshots.SelectedItem == null ? null : gSnapshots.SelectedItem as WriteableBitmap; }
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [ImportingConstructor]
        public ImageCapture_V3ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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
        }        

        protected override void OnDeactivate(bool close)
        {
            DeActivateCaptureDevice();
            base.OnDeactivate(close);
        }

        private void DeActivateCaptureDevice()
        {
            if (m_bCaptureManInitialized && VideoCaptureElemCtrl != null)
            {
                if (m_bMediaCapturePlaying && VideoCaptureElemCtrl.IsPlaying)
                {
                    VideoCaptureElemCtrl.Stop();
                }
                VideoCaptureElemCtrl.VideoCaptureDevice = null;
                m_bCaptureManInitialized = false;
            }
        }

        //public void VideoCaptureElement_MediaOpened(object sender, RoutedEventArgs e)
        //{
        //    if (m_bCaptureManInitialized && VideoCaptureElemCtrl != null && VideoCaptureElemCtrl.IsPlaying)
        //    {
        //        StartStopCaptureDevBtn.Content = eHCMSResources.S0729_G1_StopCamera;
        //        btnCaptureImage.IsEnabled = true;
        //        m_bMediaCapturePlaying = true;
        //    }            
        //}

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

        //public void AudioSources_Loaded(object sender, RoutedEventArgs e)
        //{
        //    gAudioSources = sender as ComboBox;
        //    var mItemsSource = WPFMediaKit.DirectShow.Controls.MultimediaUtil.AudioRendererNames.ToList();
        //    gAudioSources.ItemsSource = mItemsSource;
        //    gAudioSources.SelectedItem = mItemsSource.FirstOrDefault();
        //}

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
        public void Map_Loaded(object sender, RoutedEventArgs e)
        {
            gMap = sender as Image;
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
                    VideoCaptureElemCtrl.VideoCaptureDevice = null;
                    StartStopCaptureDevBtn.Content = eHCMSResources.S0724_G1_StartCamera;
                    btnCaptureImage.IsEnabled = false;
                    gSaveImage.IsEnabled = false;
                    m_bMediaCapturePlaying = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("CANNOT Start/Stop Media Device.");
                ClientLoggerHelper.LogError(ex.ToString());                
            }
        }

        private void CaptureImageFromDevice()
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)VideoCaptureElemCtrl.ActualWidth, (int)VideoCaptureElemCtrl.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(VideoCaptureElemCtrl);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            WriteableBitmap mWriteableBitmap = new WriteableBitmap(bmp);
            gImages.Add(mWriteableBitmap);
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
                gMap.Source = gSnapshots.SelectedItem as WriteableBitmap;
                gSaveImage.IsEnabled = true;
            }
            else
            {
                gMap.Source = null;
                gSaveImage.IsEnabled = false;
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
        private List<PCLResultFileStorageDetail> _FileForStore = new List<PCLResultFileStorageDetail>();
        public List<PCLResultFileStorageDetail> FileForStore
        {
            get { return GetSelectedImageForStore(); }
            set
            {
                if (_FileForStore != value)
                {
                    _FileForStore = value;

                    NotifyOfPropertyChange(() => FileForStore);
                }
            }
        }
        public void ClearSelectedImage()
        {
            List<WriteableBitmap> mSelectedImage = GetSelectedImage();
            DeleteSelectedImage(mSelectedImage);
        }
        private List<PCLResultFileStorageDetail> GetSelectedImageForStore()
        {
            List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = new List<PCLResultFileStorageDetail>();
            if (gSnapshots != null)
            {
                for (int i = 0; i < gSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(gSnapshots.ItemContainerGenerator.ContainerFromItem(gSnapshots.Items[i]));
                    List<CheckBox> mChildren = mListBoxItem.GetChildrenByType<CheckBox>();
                    CheckBox mCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbImage"));
                    if (mCheckBox.IsChecked == true)
                    {
                        var ImageArray = new MemoryStream();
                        BmpBitmapEncoder mEncoder = new BmpBitmapEncoder();
                        mEncoder.Frames.Add(BitmapFrame.Create(gImages[i]));
                        mEncoder.Save(ImageArray);
                        PCLResultFileStorageDetail aPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                        aPCLResultFileStorageDetail.IsImage = true;
                        aPCLResultFileStorageDetail.File = ImageArray.ToArray();
                        aPCLResultFileStorageDetail.V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                        mPCLResultFileStorageDetail.Add(aPCLResultFileStorageDetail);
                    }
                }
            }
            return mPCLResultFileStorageDetail;
        }
        public void ClearAllCapturedImage()
        {
            gImages.Clear();
        }

        private bool _SnapshotVisible = true;
        public bool SnapshotVisible
        {
            get => _SnapshotVisible; set
            {
                _SnapshotVisible = value;
                NotifyOfPropertyChange(() => SnapshotVisible);
            }
        }


        #endregion
    }
}