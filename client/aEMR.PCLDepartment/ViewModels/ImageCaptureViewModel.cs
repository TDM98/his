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
using System.Windows.Shapes;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Controls;
using System.IO;
using DirectShowLib;
using aEMR.Common.Collections;
using System;

namespace aEMR.PCLDepartment.ViewModels
{
    [Obsolete("ImageCaptureViewModel is deprecated, please use ImageCapture_V2ViewModel instead")]
    [Export(typeof(IImageCapture)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageCaptureViewModel : Conductor<object>, IImageCapture
    {
        public WriteableBitmap GetCapturedImage() { return gSnapshots == null || gSnapshots.SelectedItem == null ? null : gSnapshots.SelectedItem as WriteableBitmap; }
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [ImportingConstructor]
        public ImageCaptureViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }
        #region Properties
        private ComboBox gAudioSources { get; set; }
        private Rectangle gWebcam { get; set; }
        private Button gStartStopWebcam { get; set; }
        private Button btnPause { get; set; }
        private Button btnRecord { get; set; }
        private Button btnStop { get; set; }
        private Image gMap { get; set; }
        private Button gCaptureWebcam { get; set; }
        private ListBox gSnapshots { get; set; }
        private Button gSaveImage { get; set; }
        private WPFMediaKit.DirectShow.Controls.VideoCaptureElement gVideoCaptureElement { get; set; }
        private ObservableCollection<WriteableBitmap> gImages = new ObservableCollection<WriteableBitmap>();
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
        private DsDevice _gInputDevice;
        public DsDevice gInputDevice
        {
            get => _gInputDevice; set
            {
                _gInputDevice = value;
                NotifyOfPropertyChange(() => gInputDevice);
            }
        }
        #endregion
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            gInputDeviceCollection = WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.ToObservableCollection();
            gInputDevice = gInputDeviceCollection.FirstOrDefault();
        }
        public void ImageCapture_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public void VideoCaptureElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (gVideoCaptureElement != null && gVideoCaptureElement.IsPlaying)
            {
                gStartStopWebcam.Content = eHCMSResources.S0729_G1_StopCamera;
                gCaptureWebcam.IsEnabled = true;
            }
            this.HideBusyIndicator();
        }
        public void ImageCapture_Unloaded(object sender, RoutedEventArgs e)
        {
            if (gVideoCaptureElement != null && gVideoCaptureElement.IsPlaying)
                StartStopWebcam_Click(null, null);
        }
        public void VideoCaptureElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (gVideoCaptureElement != null && gVideoCaptureElement.IsPlaying)
                StartStopWebcam_Click(null, null);
            gVideoCaptureElement = sender as WPFMediaKit.DirectShow.Controls.VideoCaptureElement;
            gVideoCaptureElement.UpdateLayout();
        }
        public void AudioSources_Loaded(object sender, RoutedEventArgs e)
        {
            gAudioSources = sender as ComboBox;
            var mItemsSource = WPFMediaKit.DirectShow.Controls.MultimediaUtil.AudioRendererNames.ToList();
            gAudioSources.ItemsSource = mItemsSource;
            gAudioSources.SelectedItem = mItemsSource.FirstOrDefault();
        }
        public void Webcam_Loaded(object sender, RoutedEventArgs e)
        {
            gWebcam = sender as Rectangle;
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
        public void SaveImage_Loaded(object sender, RoutedEventArgs e)
        {
            gSaveImage = sender as Button;
            gSaveImage.Content = eHCMSResources.S0544_G1_Save;
        }
        public void StartStopWebcam_Click(object sender, RoutedEventArgs e)
        {
           if (!gVideoCaptureElement.IsPlaying)
            {
                gVideoCaptureElement.VideoCaptureDevice = gInputDevice;
                gVideoCaptureElement.Play();
                this.ShowBusyIndicator();
            }
            else
            {
                gVideoCaptureElement.Stop();
                gVideoCaptureElement.VideoCaptureDevice = null;
                gStartStopWebcam.Content = eHCMSResources.S0724_G1_StartCamera;
                gCaptureWebcam.IsEnabled = false;
                gSaveImage.IsEnabled = false;
            }
        }
        public void CaptureWebcam_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)gVideoCaptureElement.ActualWidth, (int)gVideoCaptureElement.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(gVideoCaptureElement);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            WriteableBitmap mWriteableBitmap = new WriteableBitmap(bmp);
            gImages.Add(mWriteableBitmap);
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
        #endregion
    }
}