using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using DataEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IIPCLResultGraph)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLResultGraphViewModel : ViewModelBase, IIPCLResultGraph
    {
        public IList<PCLExamTestItems> PCLExamTestItemCollection { get; set; }
        public void CanvasGraph_Loaded(object sender, object Avgs)
        {
            Canvas mCanvas = sender as Canvas;
            mCanvas.SizeChanged += Canvas_SizeChanged;
            DrawGraph(mCanvas);
        }
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas mCanvas = sender as Canvas;
            DrawGraph(mCanvas);
        }
        private void DrawGraph(Canvas aCanvas)
        {
            aCanvas.Children.Clear();
            int MinYPoint = 50;
            int MaxYPoint = 50;
            int MinXPoint = 80;
            int FontSizePoint = 10;
            if (PCLExamTestItemCollection == null || PCLExamTestItemCollection.Count < 2)
            {
                return;
            }
            double GraphHeight = aCanvas.ActualHeight - MaxYPoint - MinYPoint;
            double GraphWidth = aCanvas.ActualWidth - MinXPoint - 50;
            TextBlock txtMaxValue = new TextBlock();
            long[] ExamTypeIDCollection = PCLExamTestItemCollection.Select(x => x.PCLExamTypeID).Distinct().ToArray();
            Color[] ColorCollection = new Color[] { Colors.DarkOrange, Colors.Brown };

            PCLExamTestItemCollection = PCLExamTestItemCollection.Where(x => x.SamplingDate.HasValue && x.SamplingDate != null).ToList();
            var FirstDate = PCLExamTestItemCollection.Min(x => x.SamplingDate.Value);
            var TotalDate = (PCLExamTestItemCollection.Max(x => x.SamplingDate.Value) - FirstDate).TotalDays;
            for (int i = 0; i < ExamTypeIDCollection.Length; i++)
            {
                var aItemID = ExamTypeIDCollection[i];
                var aTestItemID = PCLExamTestItemCollection.Where(x => x.SamplingDate.HasValue && x.SamplingDate != null && x.PCLExamTypeID == aItemID).Min(x => x.PCLExamTestItemID);
                var DrawColor = Colors.DarkOrange;
                if (ColorCollection.Length > i)
                {
                    DrawColor = ColorCollection[i];
                }
                var ItemCollection = PCLExamTestItemCollection.Where(x => x.SamplingDate.HasValue && x.SamplingDate != null && x.PCLExamTestItemID == aTestItemID).OrderBy(x => x.SamplingDate).ToList();
                if (ItemCollection.Count < 2)
                {
                    continue;
                }
                double OutValue;
                Double MaxValue = ItemCollection.Where(x => x != null && double.TryParse(x.Value, out OutValue)).Max(x => Convert.ToDouble(x.Value));
                Double MinValue = ItemCollection.Where(x => x != null && double.TryParse(x.Value, out OutValue)).Min(x => Convert.ToDouble(x.Value));

                if (string.IsNullOrEmpty(txtMaxValue.Text))
                {
                    txtMaxValue.Text = MaxValue.ToString();
                }
                else
                {
                    txtMaxValue.Text = string.Format("{0}/{1}", txtMaxValue.Text, MaxValue.ToString());
                }
                if (!aCanvas.Children.Contains(txtMaxValue))
                {
                    txtMaxValue.Foreground = new SolidColorBrush(DrawColor);
                    Canvas.SetLeft(txtMaxValue, 10);
                    Canvas.SetTop(txtMaxValue, 10);
                    aCanvas.Children.Add(txtMaxValue);
                }
                TextBlock txtMinValue = new TextBlock();
                txtMinValue.Text = MinValue.ToString();
                txtMinValue.Foreground = new SolidColorBrush(DrawColor);
                Canvas.SetLeft(txtMinValue, 10);
                Canvas.SetTop(txtMinValue, MinYPoint + GraphHeight - (GraphHeight / MaxValue * MinValue) - FontSizePoint);
                aCanvas.Children.Add(txtMinValue);

                Debug.WriteLine(String.Format("MaxValue:{2}, Graph:{0}-{1}", GraphWidth, GraphHeight, MaxValue));
                Point PreviousPoint = new Point(0, 0);
                foreach (var aItem in ItemCollection)
                {
                    Double PCLValue = 0;
                    if (Double.TryParse(aItem.Value, out PCLValue))
                    {
                        double XPointValue = (aItem.SamplingDate.Value - FirstDate).TotalDays == 0 ? MinXPoint : MinXPoint + GraphWidth * ((aItem.SamplingDate.Value - FirstDate).TotalDays / TotalDate);
                        double YPointValue = MinYPoint + GraphHeight - (GraphHeight / MaxValue * PCLValue);
                        if (PreviousPoint.X * PreviousPoint.Y != 0)
                        {
                            var VaLueLine = new Line();
                            VaLueLine.Stroke = new SolidColorBrush(DrawColor);
                            VaLueLine.X1 = PreviousPoint.X;
                            VaLueLine.Y1 = PreviousPoint.Y;
                            Debug.WriteLine(string.Format("TotalDays: {0}/{1}", (aItem.SamplingDate.Value - FirstDate).TotalDays, TotalDate));
                            VaLueLine.X2 = XPointValue;
                            VaLueLine.Y2 = YPointValue;
                            VaLueLine.StrokeThickness = 1;
                            aCanvas.Children.Add(VaLueLine);

                            if ((aItem.SamplingDate.Value - FirstDate).TotalDays != 0)
                            {
                                TextBlock txtValue = new TextBlock();
                                txtValue.Text = PCLValue.ToString();
                                txtValue.Foreground = new SolidColorBrush(ColorCollection[i % 2]);
                                Canvas.SetLeft(txtValue, XPointValue - 10);
                                if (PreviousPoint.Y > YPointValue)
                                {
                                    Canvas.SetTop(txtValue, YPointValue - FontSizePoint * 2);
                                }
                                else
                                {
                                    Canvas.SetTop(txtValue, YPointValue + FontSizePoint - 5);
                                }
                                aCanvas.Children.Add(txtValue);
                            }
                        }
                        if ((aItem.SamplingDate.Value - FirstDate).TotalDays != 0)
                        {
                            TextBlock txtItemValue = new TextBlock();
                            txtItemValue.Text = aItem.SamplingDate.Value.ToString("dd/MM");
                            txtItemValue.Foreground = new SolidColorBrush(ColorCollection[i % 2]);
                            Canvas.SetLeft(txtItemValue, XPointValue);
                            Canvas.SetTop(txtItemValue, GraphHeight + MinYPoint);
                            aCanvas.Children.Add(txtItemValue);
                        }
                        else
                        {
                            TextBlock txtMinLValue = new TextBlock();
                            txtMinLValue.Text = PCLValue.ToString();
                            txtMinLValue.Foreground = new SolidColorBrush(DrawColor);
                            Canvas.SetLeft(txtMinLValue, 10);
                            Canvas.SetTop(txtMinLValue, YPointValue - FontSizePoint);
                            aCanvas.Children.Add(txtMinLValue);
                        }
                        PreviousPoint = new Point(XPointValue, YPointValue);

                        Ellipse ePoint = new Ellipse();
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        if (aItem.IsAbnormal)
                        {
                            mySolidColorBrush.Color = Colors.Red;
                        }
                        else
                        {
                            mySolidColorBrush.Color = Colors.Blue;
                        }
                        ePoint.Fill = mySolidColorBrush;
                        ePoint.StrokeThickness = 2;
                        ePoint.Stroke = Brushes.White;
                        ePoint.Width = 11;
                        ePoint.Height = 11;
                        Canvas.SetTop(ePoint, PreviousPoint.Y - 5);
                        Canvas.SetLeft(ePoint, PreviousPoint.X - 5);
                        ePoint.ToolTip = PCLValue;
                        aCanvas.Children.Add(ePoint);

                        Debug.WriteLine(string.Format("Point:{0}-{1}, Value={2}", PreviousPoint.X, PreviousPoint.Y, PCLValue));
                    }
                }
            }
            var HLine = new Line();
            HLine.Stroke = Brushes.DarkGray;
            HLine.X1 = MinXPoint;
            HLine.Y1 = MinYPoint;
            HLine.X2 = MinXPoint;
            HLine.Y2 = aCanvas.ActualHeight - MaxYPoint;
            HLine.StrokeThickness = 1;
            aCanvas.Children.Add(HLine);

            var VLine = new Line();
            VLine.Stroke = Brushes.DarkGray;
            VLine.X1 = MinXPoint;
            VLine.Y1 = aCanvas.ActualHeight - MaxYPoint;
            VLine.X2 = GraphWidth + MinXPoint;
            VLine.Y2 = aCanvas.ActualHeight - MaxYPoint;
            VLine.StrokeThickness = 1;
            aCanvas.Children.Add(VLine);
        }
    }
}