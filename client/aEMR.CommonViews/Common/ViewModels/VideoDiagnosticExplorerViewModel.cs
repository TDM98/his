using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Collections.Generic;
using DataEntities;
using System.Linq;
using eHCMSLanguage;
using ImageTools.IO.Bmp;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IVideoDiagnosticExplorer)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class VideoDiagnosticExplorerViewModel : Conductor<object>, IVideoDiagnosticExplorer
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public VideoDiagnosticExplorerViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        string ImagePool;
        string ImageStore;
        string ImageThumbTemp;

        private void GetAppConfigValue()
        {
            ImagePool = Globals.ServerConfigSection.Hospitals.PCLResourcePool;
            ImageStore = Globals.ServerConfigSection.Hospitals.PCLStorePool;
            ImageThumbTemp = Globals.ServerConfigSection.Hospitals.PCLThumbTemp;

            //==== 20161013 CMN Begin: Add PCL Image Method
            if (ObjPatientPCLImagingResult == null)
                ControlVideoVisibility = false;
            else
                ControlVideoVisibility = true;
            //==== 20161013 CMN End.
        }
        //==== 20161013 CMN Begin: Add PCL Image Method
        private bool _ControlVideoVisibility;
        public bool ControlVideoVisibility
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
        //==== 20161013 CMN End.
        //private IList<string> _ObjGetAllConfigItemValues;
        //public IList<string> ObjGetAllConfigItemValues
        //{
        //    get
        //    {
        //        return _ObjGetAllConfigItemValues;
        //    }
        //    set
        //    {
        //        if (_ObjGetAllConfigItemValues != value)
        //        {
        //            _ObjGetAllConfigItemValues = value;
        //            NotifyOfPropertyChange(() => ObjGetAllConfigItemValues);
        //        }
        //    }
        //}
       
        protected override void OnActivate()
        {
 	        base.OnActivate();
            //GetAllConfigItemValues();
            GetAppConfigValue();
        }

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

        private Stream _VideoStream;
        public Stream VideoStream
        {
            get { return _VideoStream; }
            set
            {
                _VideoStream = value;
                NotifyOfPropertyChange(()=>VideoStream);
            }
        }


        private ScaleTransform Scale;

        private MediaElement mediaPreview;
        public void mediaPreview_Loaded(object sender, RoutedEventArgs e)
        {
            mediaPreview = (sender as MediaElement);
            //==== 20161123 CMN Begin: Add resize soom video
            mediaPreview.MediaOpened += new RoutedEventHandler(mediaPreview_MediaOpened);
            //==== 20161123 CMN End.
            if(VideoStream!=null)
            {
                //mediaPreview.SetSource(VideoStream);
            }
            else
            {
                mediaPreview.Source = null;
            }

            Scale = mediaPreview.RenderTransform as ScaleTransform;

            
        }
        //==== 20161123 CMN Begin: Add resize soom video
        void mediaPreview_MediaOpened(object sender, RoutedEventArgs e)
        {
            MoveToCenter();
        }
        //==== 20161123 CMN End.

        #region Control media
        public void btPlay()
        {
            mediaPreview.Play();
        }
        public void btPause()
        {
            mediaPreview.Pause();
        }

        public void btStop()
        {
            mediaPreview.Stop();
        }

        public void btMute()
        {
            mediaPreview.IsMuted = !mediaPreview.IsMuted;
        }

        public void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPreview.Volume = (sender as Slider).Value;
        }


        /// <summary>
        /// Zoom
        /// </summary>

        private Storyboard StoryBoardScale;
        private DoubleAnimation animationScaleX;
        private DoubleAnimation animationScaleY;

        private Canvas mainCanvas;
        public void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            mainCanvas = (sender as Canvas);

            mainCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(mainCanvas_MouseLeftButtonUp);
            mainCanvas.MouseMove += new MouseEventHandler(mainCanvas_MouseMove);

            //Tinh lai chieu rong, chieu cao thuc te cua canvas.
            RectangleGeometry rectGeo = new RectangleGeometry();
            rectGeo.Rect = new Rect(0, 0, mainCanvas.ActualWidth, mainCanvas.ActualHeight);
            mainCanvas.Clip = rectGeo;
            
            StoryBoardScale = mainCanvas.Resources["StoryBoardScale"] as Storyboard;
            animationScaleX = StoryBoardScale.Children[0] as DoubleAnimation;
            animationScaleY = StoryBoardScale.Children[1] as DoubleAnimation;
        }

        private bool isMoving;
        private Point _FirstPoint;

        public void mainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            _FirstPoint = default(Point);
        }

        public void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMoving)
            {
                return;
            }
            if (isMoving == true)
            {
                Point nextPoint = e.GetPosition(mainCanvas);
                double x = nextPoint.X - _FirstPoint.X;
                double y = nextPoint.Y - _FirstPoint.Y;

                Canvas.SetLeft(mediaPreview, Canvas.GetLeft(mediaPreview) + x);
                Canvas.SetTop(mediaPreview, Canvas.GetTop(mediaPreview) + y);
                _FirstPoint = nextPoint;
            }
        }
        
        public void mediaPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;
            _FirstPoint = e.GetPosition(mainCanvas);
        }

        private void MoveToCenter()
        {
            //Lay toa do goc tren ben trai cua imgPreview trong he toa do cua Canvas
            Point CanvasCenter = new Point(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2);
            Point ImageCenter = GetCenterPoint(mediaPreview);
            Point imgCoor = new Point(CanvasCenter.X - ImageCenter.X, CanvasCenter.Y - ImageCenter.Y);

            Canvas.SetLeft(mediaPreview, imgCoor.X);
            Canvas.SetTop(mediaPreview, imgCoor.Y);
        }
        
        public void btZommIn()
        {
            animationScaleX.To = null;
            animationScaleY.To = null;

            _CenterPoint = GetCenterPoint(mediaPreview);

            Scale.CenterX = _CenterPoint.X;
            Scale.CenterY = _CenterPoint.Y;

            animationScaleX.By = 0.25;
            animationScaleY.By = 0.25;

            StoryBoardScale.Begin();

            //Scale.ScaleX += 0.25;
            //Scale.ScaleY += 0.25;
        }

        public void btZommOut()
        {
            //Scale.ScaleX -= 0.25;
            //Scale.ScaleY -= 0.25;

            if (Scale.ScaleX > 0.25)
            {
                animationScaleX.To = null;
                animationScaleY.To = null;

                animationScaleX.By = -0.25;
                animationScaleY.By = -0.25;

                StoryBoardScale.Begin();
            }
        }

        private Point _CenterPoint;
        private Point GetCenterPoint(MediaElement pMediaElement)
        {
            Point p = new Point(pMediaElement.NaturalVideoWidth / 2, pMediaElement.NaturalVideoHeight / 2);
            return p;
        }
        //==== 20161010 CMN Begin: Create control for edit thumb images
        private StackPanel GetControlForThumbnail(string ImageName, System.Windows.Controls.Image ImageFile)
        {
            CheckBox ThumbCheckbox = new CheckBox();
            ThumbCheckbox.Name = "chk_" + ImageName;
            ThumbCheckbox.Width = 15;
            ThumbCheckbox.Height = 15;
            ThumbCheckbox.HorizontalAlignment = HorizontalAlignment.Center;
            Button ThumbButton = new Button();
            ThumbButton.Name = "btn_" + ImageName;
            ThumbButton.Width = 15;
            ThumbButton.Height = 15;
            ThumbButton.HorizontalAlignment = HorizontalAlignment.Center;
            System.Windows.Resources.StreamResourceInfo ResourceStream = Application.GetResourceStream(new Uri("/aEMR.CommonViews;component/Assets/Images/Delete.png", UriKind.Relative));
            BitmapImage ButtonIcon = new BitmapImage();
            //ButtonIcon.SetSource(ResourceStream.Stream);
            ImageBrush IconBrush = new ImageBrush();
            IconBrush.ImageSource = ButtonIcon;
            ThumbButton.Background = IconBrush;
            ThumbButton.Margin = new Thickness(0, 5, 0, 0);
            StackPanel ThumbStack = new StackPanel();
            ThumbStack.Name = "stk_" + ImageName;
            ThumbStack.Orientation = Orientation.Vertical;
            ThumbStack.Width = 16;
            ThumbStack.Children.Add(ThumbCheckbox);
            ThumbStack.Children.Add(ThumbButton);
            ThumbStack.HorizontalAlignment = HorizontalAlignment.Center;
            ThumbButton.Click += (sender, e) =>
            {
                thumbs.Children.Remove(ThumbStack);
                thumbs.Children.Remove(ImageFile);
                scroller.UpdateLayout();
                double scrollPos = thumbs.ActualWidth;
                scroller.ScrollToHorizontalOffset(scrollPos);
            };
            return ThumbStack;
        }
        //==== 20161010 CMN End.
        public void btCaptureImage()
        {
            //WriteableBitmap snapShot = new WriteableBitmap(mediaPreview, null);
            string name = System.Guid.NewGuid().ToString();
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Height = 89;
            image.Margin = new Thickness(10);
            //image.Source = snapShot;
            image.Name = "img_" + name;
            thumbs.Children.Add(image);
            //==== 20161010 CMN Begin: Add Delete button for thumbs image
            //CheckBox chk = new CheckBox();
            //chk.Name = "chk_" + name;
            //thumbs.Children.Add(chk);
            thumbs.Children.Add(GetControlForThumbnail(name, image));
            //==== 20161010 CMN End.
            scroller.UpdateLayout();
            double scrollPos = thumbs.ActualWidth;
            scroller.ScrollToHorizontalOffset(scrollPos);
        }

        #endregion


        private StackPanel thumbs;
        public void thumbs_Loaded(object sender, RoutedEventArgs e)
        {
            thumbs = (sender as StackPanel);
        }

        private ScrollViewer scroller;
        public void scroller_Loaded(object sender, RoutedEventArgs e)
        {
            scroller=(sender as ScrollViewer);
        }

        //==== 20161010 CMN Begin: Add Delete button for thumbs image
        private List<string> GetSelectedImageName()
        {
            List<string> CheckedImageName = new List<string>();
            for (int i = 0; i < thumbs.Children.Count - 1; i += 2)
            {
                System.Windows.Controls.Image SelectedImage = thumbs.Children[i] as System.Windows.Controls.Image;
                StackPanel SelectedStack = thumbs.Children[i + 1] as StackPanel;
                string ChildName = SelectedStack.Name;
                var stkCheckbox = SelectedStack.Children[0] as CheckBox;
                if (stkCheckbox.IsChecked == true)
                {
                    CheckedImageName.Add(ChildName.Replace("stk_", "img_"));
                }
            }
            return CheckedImageName;
        }
        private List<byte[]> GetImageToSave(List<string> CheckedImageName)
        {
            List<byte[]> CheckedImageArray = new List<byte[]>();
            foreach (UIElement Child in thumbs.Children)
            {
                if (Child.GetType().Equals(typeof(System.Windows.Controls.Image)) && CheckedImageName.Contains((Child as FrameworkElement).Name))
                {
                    System.Windows.Controls.Image ImageArray = (Child as System.Windows.Controls.Image);
                    WriteableBitmap Bitmap = new WriteableBitmap((BitmapSource)ImageArray.Source);
                    var bmpCode = new BmpEncoder();
                    var destination = new MemoryStream();
                    //bmpCode.Encode(ImageExtensions.ToImage(Bitmap), destination);
                    CheckedImageArray.Add(destination.ToArray());
                }
            }
            return CheckedImageArray;
        }
        private void ClearImageSaved(List<string> CheckedImageName)
        {
            CheckedImageName = CheckedImageName.Select(x => x.Replace("img_", "")).ToList();
            List<UIElement> SelectedChild = new List<UIElement>();
            foreach (string ImageName in CheckedImageName)
            {
                foreach (UIElement Child in thumbs.Children)
                {
                    if ((Child as FrameworkElement).Name.EndsWith(ImageName))
                        SelectedChild.Add(Child);
                }
            }
            while (SelectedChild.Count > 0)
            {
                thumbs.Children.Remove(SelectedChild[0]);
                SelectedChild.RemoveAt(0);
            }
        }
        private void SaveImageToDatabase()
        {
            List<string> CheckedImageName = GetSelectedImageName();
            if (CheckedImageName.Count > 0)
            {
                mediaPreview.Pause();
                List<PCLResultFileStorageDetail> ListImageFile = new List<PCLResultFileStorageDetail>();
                List<byte[]> SelectedImageFile = GetImageToSave(CheckedImageName);
                foreach (byte[] ImageArray in SelectedImageFile)
                {
                    PCLResultFileStorageDetail mPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                    mPCLResultFileStorageDetail.IsImage = true;
                    mPCLResultFileStorageDetail.File = ImageArray;
                    mPCLResultFileStorageDetail.V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                    ListImageFile.Add(mPCLResultFileStorageDetail);
                }
                this.ShowBusyIndicator();
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PCLsImportClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginUploadImageToDatabase(ObjPatientPCLImagingResult, ListImageFile, new List<PCLResultFileStorageDetail>(), Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var items = contract.EndUploadImageToDatabase(asyncResult);
                                    if (items)
                                    {
                                        ClearImageSaved(CheckedImageName);
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    }
                    finally
                    {
                        this.HideBusyIndicator();
                    }

                });
                t.Start();
            }
            else
                MessageBox.Show(eHCMSResources.A0502_G1_Msg_InfoChonHinhMuonLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }
        //==== 20161010 CMN End.
        public void btSaveThumbs()
        {
            if(thumbs.Children.Count>0)
            {
                //==== 20161010 CMN Begin: Add Delete button for thumbs image
                //if (CheckChooseThumb())
                //{
                //    SaveImageClipBoard();
                //}
                //else
                //{
                //    MessageBox.Show(eHCMSResources.A0502_G1_Msg_InfoChonHinhMuonLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //}
                SaveImageToDatabase();
                //==== 20161010 CMN End.
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0407_G1_Msg_InfoChuaCoHinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private bool CheckChooseThumb()
        {
            for (int idx = 0; idx < thumbs.Children.Count; idx++)
            {
                object obj = thumbs.Children[idx];
                if (obj.GetType().Equals(typeof(CheckBox)))
                {
                    if (((CheckBox)obj).IsChecked != null && ((CheckBox)obj).IsChecked == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SaveImageClipBoard()
        {
            ObservableCollection<byte[]> buffer=SaveImageSource.SaveArrayImage(thumbs);

            List<PCLResultFileStorageDetail> listImageFile = new List<PCLResultFileStorageDetail>();

            for (int nIdx = 0; nIdx < buffer.Count ; ++nIdx)
            {
                PCLResultFileStorageDetail mPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                mPCLResultFileStorageDetail.IsImage = true;
                mPCLResultFileStorageDetail.File = buffer[nIdx];
                mPCLResultFileStorageDetail.V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                listImageFile.Add(mPCLResultFileStorageDetail);
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUploadImageToDatabase(ObjPatientPCLImagingResult, listImageFile, new List<PCLResultFileStorageDetail>(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndUploadImageToDatabase(asyncResult);
                                if (items)
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
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
                }
                finally
                {
                    this.HideBusyIndicator();
                }

            });
            t.Start();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new PCLsImportClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        List<byte[]> Listbyte = new List<byte[]>(buffer);

            //        contract.BeginSaveImageClipBoard(Listbyte, ImageThumbTemp, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                if(contract.EndSaveImageClipBoard(asyncResult))
            //                {
            //                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //                }
            //                else
            //                {
            //                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //                }
                          
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
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

    }
}


//private void GetAllConfigItemValues()
//{
//    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "ConfigItem..." });

//    var t = new Thread(() =>
//    {
//        using (var serviceFactory = new CommonServiceClient())
//        {
//            var contract = serviceFactory.ServiceInstance;

//            contract.BeginGetAllConfigItemValues(Globals.DispatchCallback((asyncResult) =>
//            {
//                try
//                {
//                    var items = contract.EndGetAllConfigItemValues(asyncResult);
//                    if (items != null)
//                    {
//                        ObjGetAllConfigItemValues = items;
//                        GetAppConfigValue();
//                    }
//                    else
//                    {
//                        ObjGetAllConfigItemValues = null;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    Globals.IsBusy = false;
//                }
//            }), null);
//        }


//    });
//    t.Start();
//}