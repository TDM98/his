using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IImageDisgnosticExplorer)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageDisgnosticExplorerViewModel : Conductor<object>, IImageDisgnosticExplorer
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ImageDisgnosticExplorerViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private BitmapImage _ObjBitmapImage;
        public BitmapImage ObjBitmapImage
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

        //==== 20161008 CMN Begin: Change Image View to WriteableBitmap
        private WriteableBitmap _ObjWBitmapImage;
        public WriteableBitmap ObjWBitmapImage
        {
            get
            {
                if (TypeOfBitmapImage == 1)
                    return _ObjWBitmapImage;
                return new WriteableBitmap(ObjBitmapImage);
            }
            set
            {
                if (_ObjWBitmapImage != value)
                {
                    _ObjWBitmapImage = value;
                    NotifyOfPropertyChange(() => ObjWBitmapImage);
                }
            }
        }
        //Check type of Input Bitmap (0 = Bitmap; 1 = WriteableBitmap)
        private int _TypeOfBitmapImage;
        public int TypeOfBitmapImage
        {
            get { return _TypeOfBitmapImage; }
            set
            {
                if (_TypeOfBitmapImage != value)
                {
                    _TypeOfBitmapImage = value;
                    NotifyOfPropertyChange(() => TypeOfBitmapImage);
                }
            }
        }
        //==== 20161008 CMN End.

        private Storyboard StoryBoardScale;
        private DoubleAnimation animationScaleX;
        private DoubleAnimation animationScaleY;

        private Storyboard StoryBoardRotator;
        private DoubleAnimation animationRotator;

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

            MoveToCenter();

            StoryBoardScale = mainCanvas.Resources["StoryBoardScale"] as Storyboard;
            animationScaleX = StoryBoardScale.Children[0] as DoubleAnimation;
            animationScaleY = StoryBoardScale.Children[1] as DoubleAnimation;


            StoryBoardRotator = mainCanvas.Resources["StoryBoardRotator"] as Storyboard;
            animationRotator = StoryBoardRotator.Children[0] as DoubleAnimation;
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

                Canvas.SetLeft(imgPreview, Canvas.GetLeft(imgPreview) + x);
                Canvas.SetTop(imgPreview, Canvas.GetTop(imgPreview) + y);
                _FirstPoint = nextPoint;
            }
        }
        

        private Image imgPreview;
        private ScaleTransform Scale;
        private RotateTransform Rotator;
        
        public  void imgPreview_Loaded(object sender, RoutedEventArgs e)
        {
            imgPreview = (sender as Image);
            Scale =((System.Windows.Media.TransformGroup)(imgPreview.RenderTransform)).Children[1] as ScaleTransform;
            Rotator = ((System.Windows.Media.TransformGroup)(imgPreview.RenderTransform)).Children[0] as RotateTransform;
        }

        public void imgPreview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;
            _FirstPoint = e.GetPosition(mainCanvas);
        }

        private void MoveToCenter()
        {
            if (imgPreview == null) return;
            //Lay toa do goc tren ben trai cua imgPreview trong he toa do cua Canvas
            Point CanvasCenter = new Point(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2);
            Point ImageCenter = GetCenterPoint(imgPreview);
            Point imgCoor = new Point(CanvasCenter.X - ImageCenter.X, CanvasCenter.Y - ImageCenter.Y);

            Canvas.SetLeft(imgPreview, imgCoor.X);
            Canvas.SetTop(imgPreview, imgCoor.Y);
        }
        
        private Point GetCenterPoint(Image img)
        {
            if (img == null) return new Point();
            Point p = new Point(img.ActualWidth / 2, img.ActualHeight / 2);
            return p;
        }


        #region btAjust
        
        private Point _CenterPoint;

        public void btnOriginal()
        {
            //Scale lai trang thai ban dau.
            _CenterPoint = GetCenterPoint(imgPreview);

            Scale.CenterX = _CenterPoint.X;
            Scale.CenterY = _CenterPoint.Y;

            animationScaleX.To = 1;
            animationScaleY.To = 1;

            StoryBoardScale.Begin();

            //Quay lai trang thai ban dau:
            animationRotator.To = 0;
            //////////////////////////RotateButton.IsEnabled = false;
            StoryBoardRotator.Begin();
            //Chuyen lai goc trai tren cung
            //MoveToTopLeft();
            MoveToCenter();
        }

        public void btZoomIn()
        {
            animationScaleX.To = null;
            animationScaleY.To = null;

            _CenterPoint = GetCenterPoint(imgPreview);

            Scale.CenterX = _CenterPoint.X;
            Scale.CenterY = _CenterPoint.Y;

            animationScaleX.By = 0.25;
            animationScaleY.By = 0.25;

            StoryBoardScale.Begin();
        }

        public void btZoomOut()
        {
            if (Scale.ScaleX > 0.25)
            {
                animationScaleX.To = null;
                animationScaleY.To = null;

                animationScaleX.By = -0.25;
                animationScaleY.By = -0.25;

                StoryBoardScale.Begin();
            }
        }

        public void btRotate()
        {
            _CenterPoint = GetCenterPoint(imgPreview);

            Rotator.CenterX = _CenterPoint.X;
            Rotator.CenterY = _CenterPoint.Y;

            animationRotator.To = null;
            /////////////////RotateButton.IsEnabled = false;
            StoryBoardRotator.Begin();
        }

        public void btOK()
        {
            TryClose();
        }

        public void btCancel()
        {
            TryClose();
        }
        #endregion
 
    }
}
