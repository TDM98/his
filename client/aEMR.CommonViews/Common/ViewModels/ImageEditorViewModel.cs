using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IImageEditor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImageEditorViewModel : ViewModelBase, IImageEditor
    {
        #region Properties
        enum ToolCase
        {
            Pencil,
            Brush,
            Line,
            Rectangle,
            Circle,
            Text
        }
        private Brush _gForceGroundColor = Brushes.Black;
        private Brush _gBackGroundColor = Brushes.Transparent;
        private ToolCase _CurrentToolCase = ToolCase.Pencil;
        private int CurrentLineSize = 1;
        private ObservableCollection<FontFamily> _SystemFontCollection = Fonts.SystemFontFamilies.ToObservableCollection();
        private FontFamily _SelectedFont;
        private ObservableCollection<int> _FontSizeCollection = new ObservableCollection<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
        private int _SelectedFontSize = 12;
        private string _gAddingText;
        private Canvas CurrentCanvas { get; set; }
        public string ImageSourcePath { get; set; }
        public Brush gForceGroundColor
        {
            get
            {
                return _gForceGroundColor;
            }
            set
            {
                if (_gForceGroundColor == value)
                {
                    return;
                }
                _gForceGroundColor = value;
                NotifyOfPropertyChange(() => gForceGroundColor);
            }
        }
        public Brush gBackGroundColor
        {
            get
            {
                return _gBackGroundColor;
            }
            set
            {
                if (_gBackGroundColor == value)
                {
                    return;
                }
                _gBackGroundColor = value;
                NotifyOfPropertyChange(() => gBackGroundColor);
            }
        }
        private ToolCase CurrentToolCase
        {
            get
            {
                return _CurrentToolCase;
            }
            set
            {
                if (_CurrentToolCase == value)
                {
                    return;
                }
                _CurrentToolCase = value;
                NotifyOfPropertyChange(() => IsVisibleSizePanel);
                NotifyOfPropertyChange(() => IsVisibleTextPanel);
            }
        }
        public bool IsVisibleSizePanel
        {
            get
            {
                return CurrentToolCase == ToolCase.Brush
                    || CurrentToolCase == ToolCase.Circle
                    || CurrentToolCase == ToolCase.Line
                    || CurrentToolCase == ToolCase.Rectangle;
            }
        }
        public bool IsVisibleTextPanel
        {
            get
            {
                return CurrentToolCase == ToolCase.Text;
            }
        }
        private Point CurrentPoint { get; set; }
        private Rectangle CurrentRectangle { get; set; }
        private Line CurrentLine { get; set; }
        private Ellipse CurrentEllipse { get; set; }
        private TextBlock CurrentTextBlock { get; set; }
        public ObservableCollection<FontFamily> SystemFontCollection
        {
            get
            {
                return _SystemFontCollection;
            }
            set
            {
                if (_SystemFontCollection == value)
                {
                    return;
                }
                _SystemFontCollection = value;
                NotifyOfPropertyChange(() => SystemFontCollection);
            }
        }
        public FontFamily SelectedFont
        {
            get
            {
                return _SelectedFont;
            }
            set
            {
                if (_SelectedFont == value)
                {
                    return;
                }
                _SelectedFont = value;
                NotifyOfPropertyChange(() => SelectedFont);
            }
        }
        public ObservableCollection<int> FontSizeCollection
        {
            get
            {
                return _FontSizeCollection;
            }
            set
            {
                if (_FontSizeCollection == value)
                {
                    return;
                }
                _FontSizeCollection = value;
                NotifyOfPropertyChange(() => FontSizeCollection);
            }
        }
        public int SelectedFontSize
        {
            get
            {
                return _SelectedFontSize;
            }
            set
            {
                if (_SelectedFontSize == value)
                {
                    return;
                }
                _SelectedFontSize = value;
                NotifyOfPropertyChange(() => SelectedFontSize);
            }
        }
        public string gAddingText
        {
            get
            {
                return _gAddingText;
            }
            set
            {
                if (_gAddingText == value)
                {
                    return;
                }
                _gAddingText = value;
                NotifyOfPropertyChange(() => gAddingText);
            }
        }
        #endregion
        #region Events
        public void CanvasGraph_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ImageSourcePath))
            {
                return;
            }
            CurrentCanvas = sender as Canvas;
            CurrentCanvas.Width = 800;
            CurrentCanvas.Height = 600;
            ImageBrush BackgroundImageBrush = new ImageBrush();
            BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(ImageSourcePath));
            CurrentCanvas.Background = BackgroundImageBrush;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            gForceGroundColor = Brushes.Black;
            SelectedFont = SystemFontCollection.Where(x => x.Source == "Arial").FirstOrDefault();
        }
        public void btnChooseForcegroundColor()
        {
            Color PickedColor;
            if (CommonGlobals.TryPickColor(out PickedColor))
            {
                gForceGroundColor = new SolidColorBrush(PickedColor);
            }
        }
        public void btnChooseBackgroundColor()
        {
            Color PickedColor;
            if (CommonGlobals.TryPickColor(out PickedColor))
            {
                gBackGroundColor = new SolidColorBrush(PickedColor);
            }
            else
            {
                gBackGroundColor = new SolidColorBrush(Colors.Transparent);
            }
        }
        public void CanvasGraph_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            if (CurrentToolCase == ToolCase.Pencil || CurrentToolCase == ToolCase.Brush)
            {
                var VaLueLine = new Line();
                VaLueLine.Stroke = gForceGroundColor;
                VaLueLine.X1 = CurrentPoint.X;
                VaLueLine.Y1 = CurrentPoint.Y;
                VaLueLine.X2 = e.GetPosition(CurrentCanvas).X;
                VaLueLine.Y2 = e.GetPosition(CurrentCanvas).Y;
                VaLueLine.StrokeThickness = CurrentLineSize;
                CurrentCanvas.Children.Add(VaLueLine);
                CurrentPoint = e.GetPosition(CurrentCanvas);
            }
            else if (CurrentToolCase == ToolCase.Rectangle)
            {
                double XPoint = CurrentPoint.X - e.GetPosition(CurrentCanvas).X >= 0 ? e.GetPosition(CurrentCanvas).X : CurrentPoint.X;
                double YPoint = CurrentPoint.Y - e.GetPosition(CurrentCanvas).Y >= 0 ? e.GetPosition(CurrentCanvas).Y : CurrentPoint.Y;
                Canvas.SetLeft(CurrentRectangle, XPoint);
                Canvas.SetTop(CurrentRectangle, YPoint);
                CurrentRectangle.Width = Math.Abs(CurrentPoint.X - e.GetPosition(CurrentCanvas).X);
                CurrentRectangle.Height = Math.Abs(CurrentPoint.Y - e.GetPosition(CurrentCanvas).Y);
            }
            else if (CurrentToolCase == ToolCase.Line)
            {
                CurrentLine.X2 = e.GetPosition(CurrentCanvas).X;
                CurrentLine.Y2 = e.GetPosition(CurrentCanvas).Y;
            }
            else if (CurrentToolCase == ToolCase.Circle)
            {
                double XPoint = CurrentPoint.X - e.GetPosition(CurrentCanvas).X >= 0 ? e.GetPosition(CurrentCanvas).X : CurrentPoint.X;
                double YPoint = CurrentPoint.Y - e.GetPosition(CurrentCanvas).Y >= 0 ? e.GetPosition(CurrentCanvas).Y : CurrentPoint.Y;
                Canvas.SetLeft(CurrentEllipse, XPoint);
                Canvas.SetTop(CurrentEllipse, YPoint);
                CurrentEllipse.Width = Math.Abs(CurrentPoint.X - e.GetPosition(CurrentCanvas).X);
                CurrentEllipse.Height = Math.Abs(CurrentPoint.Y - e.GetPosition(CurrentCanvas).Y);
            }
            else if (CurrentToolCase == ToolCase.Text)
            {
                Canvas.SetLeft(CurrentTextBlock, e.GetPosition(CurrentCanvas).X);
                Canvas.SetTop(CurrentTextBlock, e.GetPosition(CurrentCanvas).Y);
            }
        }
        public void CanvasGraph_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentPoint = e.GetPosition(CurrentCanvas);
            if (CurrentToolCase == ToolCase.Rectangle)
            {
                CurrentRectangle = new Rectangle();
                CurrentRectangle.Stroke = gForceGroundColor;
                CurrentRectangle.StrokeThickness = CurrentLineSize;
                CurrentRectangle.Fill = gBackGroundColor;
                Canvas.SetLeft(CurrentRectangle, CurrentPoint.X);
                Canvas.SetTop(CurrentRectangle, CurrentPoint.Y);
                CurrentCanvas.Children.Add(CurrentRectangle);
            }
            else if (CurrentToolCase == ToolCase.Line)
            {
                CurrentLine = new Line();
                CurrentLine.Stroke = gForceGroundColor;
                CurrentLine.StrokeThickness = CurrentLineSize;
                CurrentLine.X1 = CurrentPoint.X;
                CurrentLine.Y1 = CurrentPoint.Y;
                CurrentCanvas.Children.Add(CurrentLine);
            }
            else if (CurrentToolCase == ToolCase.Circle)
            {
                CurrentEllipse = new Ellipse();
                CurrentEllipse.Stroke = gForceGroundColor;
                CurrentEllipse.StrokeThickness = CurrentLineSize;
                CurrentEllipse.Fill = gBackGroundColor;
                Canvas.SetLeft(CurrentEllipse, CurrentPoint.X);
                Canvas.SetTop(CurrentEllipse, CurrentPoint.Y);
                CurrentCanvas.Children.Add(CurrentEllipse);
            }
            else if (CurrentToolCase == ToolCase.Text)
            {
                CurrentTextBlock = new TextBlock();
                CurrentTextBlock.Text = gAddingText;
                CurrentTextBlock.Foreground = gForceGroundColor;
                CurrentTextBlock.FontSize = SelectedFontSize;
                CurrentTextBlock.FontFamily = SelectedFont;
                Canvas.SetLeft(CurrentTextBlock, CurrentPoint.X);
                Canvas.SetTop(CurrentTextBlock, CurrentPoint.Y);
                CurrentCanvas.Children.Add(CurrentTextBlock);
            }
        }
        public void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                if (CurrentCanvas.Children == null || CurrentCanvas.Children.Count == 0)
                {
                    return;
                }
                CurrentCanvas.Children.RemoveAt(CurrentCanvas.Children.Count - 1);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                CallSaveCurrentEdited();
            }
        }
        public void btnPencilCase()
        {
            CurrentLineSize = 1;
            CurrentToolCase = ToolCase.Pencil;
        }
        public void btnBrushCase()
        {
            CurrentToolCase = ToolCase.Brush;
        }
        public void btnCircleCase()
        {
            CurrentToolCase = ToolCase.Circle;
        }
        public void btnLineCase()
        {
            CurrentToolCase = ToolCase.Line;
        }
        public void btnRectangleCase()
        {
            CurrentToolCase = ToolCase.Rectangle;
        }
        public void btnTextCase()
        {
            CurrentToolCase = ToolCase.Text;
        }
        public void btnLineSize1()
        {
            CurrentLineSize = 1;
        }
        public void btnLineSize3()
        {
            CurrentLineSize = 3;
        }
        public void btnLineSize5()
        {
            CurrentLineSize = 5;
        }
        public void btnLineSize7()
        {
            CurrentLineSize = 7;
        }
        public void btnLineSize9()
        {
            CurrentLineSize = 9;
        }
        public void btnSave()
        {
            CallSaveCurrentEdited();
        }
        #endregion
        #region Methods
        private byte[] CreateSaveBitmap(Canvas aCanvas)
        {
            try
            {
                RenderTargetBitmap mRender = new RenderTargetBitmap((int)aCanvas.Width, (int)aCanvas.Height, 96d, 96d, PixelFormats.Pbgra32);
                // needed otherwise the image output is black
                aCanvas.Measure(new Size((int)aCanvas.Width, (int)aCanvas.Height));
                aCanvas.Arrange(new Rect(new Size((int)aCanvas.Width, (int)aCanvas.Height)));
                mRender.Render(aCanvas);
                //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                PngBitmapEncoder mEncoder = new PngBitmapEncoder();
                mEncoder.Frames.Add(BitmapFrame.Create(mRender));
                MemoryStream aStream = new MemoryStream();
                mEncoder.Save(aStream);
                return aStream.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CallSaveCurrentEdited()
        {
            try
            {
                this.DlgShowBusyIndicator();
                string TempDirectoryPath = System.IO.Path.Combine(Globals.ServerConfigSection.Servers.ImageCaptureFilePublicStorePath, "SurgeryImages");
                string FileName = string.Format("{0}-{1}.png", Globals.GetCurServerDateTime().ToString("yyyyMMddHHmmssfff"), Guid.NewGuid());
                var BitmapArray = CreateSaveBitmap(CurrentCanvas);
                if (BitmapArray != null && BitmapArray.Length > 0)
                {
                    var UploadThread = new Thread(() =>
                    {
                        using (var serviceFactory = new CommonService_V2Client())
                        {
                            try
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginDoUpload(FileName, BitmapArray, false, TempDirectoryPath, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        if (contract.EndDoUpload(asyncResult))
                                        {
                                            ImageSourcePath = System.IO.Path.Combine(Globals.ServerConfigSection.CommonItems.ServerPublicAddress, string.Format("Pictures/SurgeryImages/{0}", FileName));
                                            TryClose();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
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
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }
                    });
                    UploadThread.Start();
                }
            }
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            }
        }
        #endregion
    }
}