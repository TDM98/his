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

using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using MediaCaptureWPF;
using Windows.Media.Capture;
using aEMR.Controls;
using System.IO;
using DirectShowLib;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using aEMR.Infrastructure;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using aEMR.Common;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IImageCapture_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageCapture_V2ViewModel : Conductor<object>, IImageCapture_V2
    {
        public WriteableBitmap GetCapturedImage() { return gSnapshots == null || gSnapshots.SelectedItem == null ? null : gSnapshots.SelectedItem as WriteableBitmap; }
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [ImportingConstructor]
        public ImageCapture_V2ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }

        #region Properties
        //private ComboBox gAudioSources { get; set; }        
        private ComboBox gVideoDevicesCbo { get; set; }
        private Button gStartStopWebcam { get; set; }
        private Button btnPause { get; set; }
        private Button btnRecord { get; set; }
        private Button btnStop { get; set; }
        public Image gMap { get; set; }
        private Button gCaptureWebcam { get; set; }
        private ListBox gSnapshots { get; set; }

        private WPFMediaKit.DirectShow.Controls.VideoCaptureElement gVideoCaptureElement { get; set; }
        private ObservableCollection<WriteableBitmap> _gImages = new ObservableCollection<WriteableBitmap>();
        public ObservableCollection<WriteableBitmap> gImages
        {
            get => _gImages; set
            {
                _gImages = value;
                NotifyOfPropertyChange(() => gImages);
            }
        }

        private ObservableCollection<DeviceInformation> _gInputDeviceCollection;
        public ObservableCollection<DeviceInformation> gInputDeviceCollection
        {
            get => _gInputDeviceCollection;
            set
            {
                _gInputDeviceCollection = value;
                NotifyOfPropertyChange(() => gInputDeviceCollection);
            }
        }
        private DeviceInformation _gInputDevice;
        public DeviceInformation gInputDevice
        {
            get => _gInputDevice; set
            {
                _gInputDevice = value;
                NotifyOfPropertyChange(() => gInputDevice);
            }
        }
        #endregion

        private bool m_bCaptureManInitialized = false;
        private CapturePreview m_TheCapturePreviewer = null;
        private bool m_bMediaCapturePlaying = false;

        private MediaCapture theCaptureManager = null;

        #region Events
        protected override async void OnActivate()
        {
            base.OnActivate();

            try
            {
                Task<string> getVideoDevTask = GetVideoProfileSupportedDeviceIdAsync(Windows.Devices.Enumeration.Panel.Back);
                await getVideoDevTask;
                string videoDevId = getVideoDevTask.Result;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (m_bMediaCapturePlaying)
            {
                StopMediaCapture();
            }
            if (m_bCaptureManInitialized)
            {
                theCaptureManager.Dispose();
            }
        }

        //protected async void ActivateCaptureDev()
        //{
        //    if (m_initialized)
        //    {
        //        return; // Already initialized
        //    }
        //    m_initialized = true;

        //    MediaCaptureInitializationSettings mci8 = new MediaCaptureInitializationSettings
        //    {
        //        StreamingCaptureMode = StreamingCaptureMode.Video
        //    };

        //    var capture = new MediaCapture();
        //    //await capture.InitializeAsync(new MediaCaptureInitializationSettings
        //    //    {
        //    //        StreamingCaptureMode = StreamingCaptureMode.Video // No audio
        //    //    });

        //    await capture.InitializeAsync(mci8);

        //    m_TheCapturePreviewer = new CapturePreview(capture);

        //}


        public void ImageCapture_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void ImageCapture_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private Image CapturePreviewCtrl = null;

        public void ScanImage_Loaded(object sender, RoutedEventArgs e)
        {
            CapturePreviewCtrl = sender as Image;
        }

        private async Task<bool> StartMediaCapture()
        {
            try
            {
                DeviceInformation selectedVideoDevice = gInputDevice;
                if (selectedVideoDevice == null)
                {
                    MessageBox.Show("Video device either not exists or Not being selected.");
                    return false; ;
                }

                if (!m_bCaptureManInitialized)
                {
                    theCaptureManager = new MediaCapture();
                    await theCaptureManager.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = selectedVideoDevice.Id,
                        StreamingCaptureMode = StreamingCaptureMode.Video // No audio
                    });
                    m_bCaptureManInitialized = true;
                }

                m_TheCapturePreviewer = new CapturePreview(theCaptureManager);

                CapturePreviewCtrl.Source = m_TheCapturePreviewer;
                await m_TheCapturePreviewer.StartAsync();

                m_bMediaCapturePlaying = true;

                return true;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
            }
            return false;
        }

        private async void StopMediaCapture()
        {
            m_bMediaCapturePlaying = false;
            await m_TheCapturePreviewer.StopAsync();
        }


        public void VideoSourcesCbo_Loaded(object sender, RoutedEventArgs e)
        {
            gVideoDevicesCbo = sender as ComboBox;
        }

        public void StartStopWebcam_Loaded(object sender, RoutedEventArgs e)
        {
            gStartStopWebcam = sender as Button;
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
        public void CaptureWebcam_Loaded(object sender, RoutedEventArgs e)
        {
            gCaptureWebcam = sender as Button;
            gCaptureWebcam.Content = eHCMSResources.K1688_G1_CaptureImg;
        }
        public void Snapshots_Loaded(object sender, RoutedEventArgs e)
        {
            gSnapshots = sender as ListBox;
            gSnapshots.ItemsSource = gImages;
        }

        public async void StartStopWebcam_Click(object sender, RoutedEventArgs e)
        {
            if (videoDevices == null)
            {
                this.ShowBusyIndicator();
                Task<string> getVideoDevTask = GetVideoProfileSupportedDeviceIdAsync(Windows.Devices.Enumeration.Panel.Back);
                await getVideoDevTask;
                string videoDevId = getVideoDevTask.Result;
                this.HideBusyIndicator();

                if (videoDevices == null || videoDevId.Length < 1)
                {
                    return;
                }
            }
            if (!m_bMediaCapturePlaying)
            {
                this.ShowBusyIndicator();
                Task<bool> startCaptureTask = StartMediaCapture();
                await startCaptureTask;
                bool bRes = startCaptureTask.Result;
                this.HideBusyIndicator();
                if (bRes)
                {
                    gStartStopWebcam.Content = eHCMSResources.S0729_G1_StopCamera;
                    gCaptureWebcam.IsEnabled = true;
                }
            }
            else
            {
                StopMediaCapture();
                gStartStopWebcam.Content = eHCMSResources.S0724_G1_StartCamera;
                gCaptureWebcam.IsEnabled = false;
            }
        }

        public void CaptureWebcam_Click(object sender, RoutedEventArgs e)
        {
            CaptureVideoImage();
        }

        public async void CaptureVideoImage()
        {
            if (!m_bMediaCapturePlaying) return;
            try
            {
                ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
                var imgStream = new InMemoryRandomAccessStream();
                await theCaptureManager.CapturePhotoToStreamAsync(imgFormat, imgStream);
                WriteableBitmap theReadyImg = Globals.GetWriteableBitmapFromStream(imgStream.AsStream());
                gImages.Add(theReadyImg);
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
            }
            else
            {
                gMap.Source = null;
            }
        }
        #endregion
        #region Methods
        private List<WriteableBitmap> GetSelectedImage(ListBox aSnapshots)
        {
            List<WriteableBitmap> SelectedImage = new List<WriteableBitmap>();
            if (aSnapshots != null)
            {
                for (int i = 0; i < aSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(aSnapshots.ItemContainerGenerator.ContainerFromItem(aSnapshots.Items[i]));
                    List<Control> mChildren = aSnapshots.GetChildrenByType<Control>();
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
            get { return GetSelectedImageForStore(gSnapshots); }
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
            List<WriteableBitmap> mSelectedImage = GetSelectedImage(gSnapshots);
            DeleteSelectedImage(mSelectedImage);
        }
        private List<PCLResultFileStorageDetail> GetSelectedImageForStore(ListBox aSnapshots)
        {
            List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = new List<PCLResultFileStorageDetail>();
            if (aSnapshots != null)
            {
                for (int i = 0; i < aSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(aSnapshots.ItemContainerGenerator.ContainerFromItem(aSnapshots.Items[i]));
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
        public DeviceInformationCollection videoDevices { get; set; }
        public async Task<string> GetVideoProfileSupportedDeviceIdAsync(Windows.Devices.Enumeration.Panel panel)
        {
            string deviceId = string.Empty;

            // Finds all video capture devices
            //videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.All);

            ObservableCollection<DeviceInformation> videoDevObsCol = new ObservableCollection<DeviceInformation>();
            foreach (DeviceInformation devInfo in videoDevices)
            {
                videoDevObsCol.Add(devInfo);
            }
            gInputDeviceCollection = videoDevObsCol;

            if (gVideoDevicesCbo != null)
            {
                if (gVideoDevicesCbo.ItemsSource != null)
                {
                    gVideoDevicesCbo.SelectedIndex = 0;
                    if (gInputDevice != null)
                    {
                        deviceId = gInputDevice.Id;
                    }
                }
            }

            return deviceId;
        }
        #endregion

        private bool _SnapshotVisible = true;
        public bool SnapshotVisible
        {
            get => _SnapshotVisible; set
            {
                _SnapshotVisible = value;
                NotifyOfPropertyChange(() => SnapshotVisible);
            }
        }
    }
}