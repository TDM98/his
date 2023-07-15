using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Text;

namespace aEMR.Common.XamlWriters
{
    public class XamlWriter : IDisposable
    {
        private bool disposed = false;
        ~XamlWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.writer.Close();
                this.writer = null;

                disposed = true;
            }
        }
        
        public const string NamespaceClient = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        /// <summary>
        /// The XAML namespace
        /// </summary>
        public const string NamespaceXaml = "http://schemas.microsoft.com/winfx/2006/xaml";

        public const string PrefixXaml = "x";
        private static readonly Color DefaultColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        private XmlWriter writer;
        private XamlWriterSettings settings;
        private static readonly Point DefaultPoint = new Point();
        private int elementCount = 0;
        private static readonly Rect DefaultRect = new Rect();
        private static readonly Size DefaultSize = new Size();
        private static readonly Thickness DefaultThickness = new Thickness();
        private static readonly CornerRadius DefaultCornerRadius = new CornerRadius();

        protected void WriteAttribute(string name, string value)
        {
            if (this.settings.WriteDefaultValues || !string.IsNullOrEmpty(value))
            {
                this.writer.WriteAttributeString(name, value);
            }
        }
        protected void WriteAttribute(string name, Color value, Color defaultValue)
        {
            if (this.settings.WriteDefaultValues || value != defaultValue)
            {
                WriteAttribute(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, bool value, bool defaultValue)
        {
            if (this.settings.WriteDefaultValues || value != defaultValue)
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, GridLength value, GridLength defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, double value, double defaultValue)
        {
            if (!double.IsInfinity(value) && !double.IsNaN(value) && (this.settings.WriteDefaultValues || value != defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Enum value, Enum defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string prefix, string name, string ns, string value)
        {
            if (this.settings.WriteDefaultValues || !string.IsNullOrEmpty(value))
            {
                this.writer.WriteAttributeString(prefix, name, ns, value);
            }
        }
        protected void WriteAttribute(string name, Point value, Point defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Uri value)
        {
            if (value != null)
            {
                WriteAttribute(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Thickness value, Thickness defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Cursor value, Cursor defaultValue)
        {
            if (value != null && (this.settings.WriteDefaultValues || value != defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, CornerRadius value, CornerRadius defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, DoubleCollection value)
        {
            if (this.settings.WriteDefaultValues || value != null && value.Count > 0)
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Size value, Size defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, Rect value, Rect defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, PointCollection value)
        {
            if (this.settings.WriteDefaultValues || value != null && value.Count > 0)
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, FontFamily value, string defaultValue)
        {
            if (value != null && (this.settings.WriteDefaultValues || value.Format() != defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, FontStretch value, FontStretch defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, FontStyle value, FontStyle defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, FontWeight value, FontWeight defaultValue)
        {
            if (this.settings.WriteDefaultValues || !value.Equals(defaultValue))
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }
        protected void WriteAttribute(string name, TextDecorationCollection value)
        {
            if (this.settings.WriteDefaultValues || value != null)
            {
                this.writer.WriteAttributeString(name, value.Format());
            }
        }

        protected virtual bool WriteContent(FrameworkElement element)
        {
            // write element content
            int count = VisualTreeHelper.GetChildrenCount(element);
            bool result = true;

            for (int i = 0; i < count; i++)
            {
                FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(element, i);
                if (!WriteElement(child))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        private void WriteComment(string comment, params object[] o)
        {
            this.writer.WriteComment(" " + string.Format(comment, o) + " ");
        }

        private void WriteElement(Geometry geometry)
        {
            string typeName = WriteStartElement(geometry);

            EllipseGeometry ellipse = geometry as EllipseGeometry;
            if (ellipse != null)
            {
                WriteAttribute("Center", ellipse.Center, DefaultPoint);
                WriteAttribute("RadiusX", ellipse.RadiusX, 0);
                WriteAttribute("RadiusY", ellipse.RadiusY, 0);
                goto End;
            }

            GeometryGroup group = geometry as GeometryGroup;
            if (group != null)
            {
                WriteAttribute("FillRule", group.FillRule, FillRule.EvenOdd);
                foreach (Geometry child in group.Children)
                {
                    WriteElement(child);
                }
                goto End;
            }

            LineGeometry line = geometry as LineGeometry;
            if (line != null)
            {
                WriteAttribute("StartPoint", line.StartPoint, DefaultPoint);
                WriteAttribute("EndPoint", line.EndPoint, DefaultPoint);
                goto End;
            }

            PathGeometry path = geometry as PathGeometry;
            if (path != null)
            {
                WriteAttribute("FillRule", path.FillRule, FillRule.EvenOdd);

                //WriteComment(Resources.MessageXamlPathDataNotSupported);
                goto End;
            }

            RectangleGeometry rectangle = geometry as RectangleGeometry;
            if (rectangle != null)
            {
                WriteAttribute("Rect", rectangle.Rect, DefaultRect);
                WriteAttribute("RadiusX", rectangle.RadiusX, 0);
                WriteAttribute("RadiusY", rectangle.RadiusY, 0);
                goto End;
            }

        End:
            if (geometry.Transform != null)
            {
                WriteStartElement(string.Format("{0}.Transform", typeName));
                WriteElement(geometry.Transform);
                WriteEndElement();
            }
            WriteEndElement();
        }
        private void WriteElement(PathFigure figure)
        {
            string typeName = WriteStartElement(figure);
            WriteAttribute("StartPoint", figure.StartPoint, DefaultPoint);
            WriteAttribute("IsClosed", figure.IsClosed, false);
            WriteAttribute("IsFilled", figure.IsFilled, true);

            foreach (PathSegment segment in figure.Segments)
            {
                WriteElement(segment);
            }

            WriteEndElement();
        }
        private void WriteElement(PathSegment segment)
        {
            string typeName = WriteStartElement(segment);

            ArcSegment arc = segment as ArcSegment;
            if (arc != null)
            {
                WriteAttribute("IsLargeArc", arc.IsLargeArc, false);
                WriteAttribute("Point", arc.Point, DefaultPoint);
                WriteAttribute("RotationAngle", arc.RotationAngle, 0);
                WriteAttribute("Size", arc.Size, DefaultSize);
                WriteAttribute("SweepDirection", arc.SweepDirection, SweepDirection.Counterclockwise);
                goto End;
            }
            BezierSegment bezier = segment as BezierSegment;
            if (bezier != null)
            {
                WriteAttribute("Point1", bezier.Point1, DefaultPoint);
                WriteAttribute("Point2", bezier.Point2, DefaultPoint);
                WriteAttribute("Point3", bezier.Point3, DefaultPoint);
                goto End;
            }
            LineSegment line = segment as LineSegment;
            if (line != null)
            {
                WriteAttribute("Point", line.Point, DefaultPoint);
                goto End;
            }
            PolyBezierSegment polyBezier = segment as PolyBezierSegment;
            if (polyBezier != null)
            {
                WriteAttribute("Points", polyBezier.Points);
                goto End;
            }
            PolyLineSegment polyLine = segment as PolyLineSegment;
            if (polyLine != null)
            {
                WriteAttribute("Points", polyLine.Points);
                goto End;
            }
            PolyQuadraticBezierSegment polyQuadraticBezier = segment as PolyQuadraticBezierSegment;
            if (polyQuadraticBezier != null)
            {
                WriteAttribute("Points", polyQuadraticBezier.Points);
                goto End;
            }
            QuadraticBezierSegment quadraticBezier = segment as QuadraticBezierSegment;
            if (quadraticBezier != null)
            {
                WriteAttribute("Point1", quadraticBezier.Point1, DefaultPoint);
                WriteAttribute("Point2", quadraticBezier.Point2, DefaultPoint);
                goto End;
            }
        End:
            WriteEndElement();
        }
        public void WriteElement(Transform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }

            string typeName = WriteStartElement(transform);

            MatrixTransform matrix = transform as MatrixTransform;
            if (matrix != null)
            {
                WriteStartElement(string.Format("{0}.Matrix", typeName));
                WriteStartElement("Matrix");
                WriteAttribute("M11", matrix.Matrix.M11, 1);
                WriteAttribute("M12", matrix.Matrix.M12, 0);
                WriteAttribute("M21", matrix.Matrix.M21, 0);
                WriteAttribute("M22", matrix.Matrix.M22, 1);
                WriteAttribute("OffsetX", matrix.Matrix.OffsetX, 0);
                WriteAttribute("OffsetY", matrix.Matrix.OffsetY, 0);
                WriteEndElement();
                WriteEndElement();
                goto End;
            }
            RotateTransform rotate = transform as RotateTransform;
            if (rotate != null)
            {
                WriteAttribute("Angle", rotate.Angle, 0);
                WriteAttribute("CenterX", rotate.CenterX, 0);
                WriteAttribute("CenterY", rotate.CenterY, 0);
                goto End;
            }
            ScaleTransform scale = transform as ScaleTransform;
            if (scale != null)
            {
                WriteAttribute("ScaleX", scale.ScaleX, 1);
                WriteAttribute("ScaleY", scale.ScaleY, 1);
                WriteAttribute("CenterX", scale.CenterX, 0);
                WriteAttribute("CenterY", scale.CenterY, 0);
                goto End;
            }
            SkewTransform skew = transform as SkewTransform;
            if (skew != null)
            {
                WriteAttribute("AngleX", skew.AngleX, 0);
                WriteAttribute("AngleY", skew.AngleY, 0);
                WriteAttribute("CenterX", skew.CenterX, 0);
                WriteAttribute("CenterY", skew.CenterY, 0);
                goto End;
            }
            TransformGroup group = transform as TransformGroup;
            if (group != null)
            {
                foreach (Transform child in group.Children)
                {
                    WriteElement(child);
                }
                goto End;
            }
            TranslateTransform translate = transform as TranslateTransform;
            if (translate != null)
            {
                WriteAttribute("X", translate.X, 0);
                WriteAttribute("Y", translate.Y, 0);
                goto End;
            }

        End:
            WriteEndElement();
        }
        private void WriteElement(System.Windows.Ink.Stroke stroke)
        {
            string typeName = WriteStartElement(stroke);
            // drawing attributes
            WriteStartElement(string.Format("{0}.DrawingAttributes", typeName));
            WriteStartElement("DrawingAttributes");
            WriteAttribute("Color", stroke.DrawingAttributes.Color, Colors.Black);
            WriteAttribute("OutlineColor", stroke.DrawingAttributes.OutlineColor, DefaultColor);
            WriteAttribute("Width", stroke.DrawingAttributes.Width, 3.0);
            WriteAttribute("Height", stroke.DrawingAttributes.Height, 3.0);
            WriteEndElement();
            WriteEndElement();
            WriteEndElement();
        }
        private void WriteElement(Brush brush)
        {
            string typeName = WriteStartElement(brush);

            WriteAttribute("Opacity", brush.Opacity, 1);

            GradientBrush gradient = brush as GradientBrush;
            if (gradient != null)
            {
                LinearGradientBrush linearGradient = gradient as LinearGradientBrush;
                if (linearGradient != null)
                {
                    WriteAttribute("StartPoint", linearGradient.StartPoint, DefaultPoint);
                    WriteAttribute("EndPoint", linearGradient.EndPoint, new Point(1, 1));
                }
                RadialGradientBrush radialGradient = gradient as RadialGradientBrush;
                if (radialGradient != null)
                {
                    WriteAttribute("Center", radialGradient.Center, new Point(.5, .5));
                    WriteAttribute("GradientOrigin", radialGradient.GradientOrigin, new Point(.5, .5));
                    WriteAttribute("RadiusX", radialGradient.RadiusX, .5);
                    WriteAttribute("RadiusY", radialGradient.RadiusY, .5);
                }
                WriteAttribute("ColorInterpolationMode", gradient.ColorInterpolationMode, ColorInterpolationMode.SRgbLinearInterpolation);
                WriteAttribute("MappingMode", gradient.MappingMode, BrushMappingMode.RelativeToBoundingBox);
                WriteAttribute("SpreadMethod", gradient.SpreadMethod, GradientSpreadMethod.Pad);

                foreach (GradientStop stop in gradient.GradientStops)
                {
                    WriteStartElement(stop);
                    WriteAttribute("Color", stop.Color, DefaultColor);
                    WriteAttribute("Offset", stop.Offset, 0);
                    WriteEndElement();
                }
                goto End;
            }

            SolidColorBrush solidColor = brush as SolidColorBrush;
            if (solidColor != null)
            {
                WriteAttribute("Color", solidColor.Color, DefaultColor);
                goto End;
            }

            TileBrush tile = brush as TileBrush;
            if (tile != null)
            {
                ImageBrush image = tile as ImageBrush;
                if (image != null)
                {
                    BitmapImage bitmap = image.ImageSource as BitmapImage;
                    if (bitmap != null)
                    {
                        WriteAttribute("ImageSource", bitmap.UriSource);
                    }
                }
                VideoBrush video = tile as VideoBrush;
                if (video != null)
                {
                    WriteAttribute("SourceName", video.SourceName);
                }

                WriteAttribute("AlignmentX", tile.AlignmentX, AlignmentX.Center);
                WriteAttribute("AlignmentY", tile.AlignmentY, AlignmentY.Center);
                WriteAttribute("Stretch", tile.Stretch, Stretch.Fill);
                goto End;
            }

        End:
            if (brush.Transform != null)
            {
                WriteStartElement(string.Format("{0}.Transform", typeName));
                WriteElement(brush.Transform);
                WriteEndElement();
            }
            if (brush.RelativeTransform != null)
            {
                WriteStartElement(string.Format("{0}.RelativeTransform", typeName));
                WriteElement(brush.RelativeTransform);
                WriteEndElement();
            }

            WriteEndElement();
        }
        private void WriteElement(TriggerCollection triggers)
        {
            foreach (EventTrigger trigger in triggers)
            {
                WriteStartElement(trigger);
                //WriteAttribute("RoutedEvent", trigger.RoutedEvent.ToString());
                foreach (BeginStoryboard beginStoryboard in trigger.Actions)
                {
                    WriteStartElement(beginStoryboard);
                    WriteElement(beginStoryboard.Storyboard);
                    this.writer.WriteEndElement();
                }
                this.writer.WriteEndElement();
            }
        }
        private void WriteElement(Timeline timeline)
        {
            //TODO: implement
        }
        private void WriteElement(Inline inline)
        {
            string typeName = WriteStartElement(inline);
            string text = null;

            Run run = inline as Run;
            if (run != null)
            {
                text = run.Text;
            }

            WriteAttribute("FontFamily", inline.FontFamily, "Portable User Interface");
            WriteAttribute("FontSize", inline.FontSize, 14.666);
            WriteAttribute("FontStretch", inline.FontStretch, FontStretches.Normal);
            WriteAttribute("FontStyle", inline.FontStyle, FontStyles.Normal);
            WriteAttribute("FontWeight", inline.FontWeight, FontWeights.Normal);
            WriteAttribute("TextDecorations", inline.TextDecorations);

            Color? foreground;
            if (TryParseColorValue(inline.Foreground, out foreground))
            {
                WriteAttribute("Foreground", foreground.Value, Colors.Black);

                if (text != null)
                {
                    this.writer.WriteString(text);
                }
            }
            else if (inline.Foreground != null)
            {
                if (text != null)
                {
                    WriteAttribute("Text", text);
                }

                WriteStartElement(string.Format("{0}.Foreground", typeName));
                WriteElement(inline.Foreground);
                WriteEndElement();
            }

            WriteEndElement();
        }
        public bool WriteElement(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (this.elementCount++ == this.settings.MaxUIElements)
            {
                return false;
            }

            string typeName = WriteStartElement(element);

            if (element.Parent is Canvas)
            {
                WriteAttribute("Canvas.Left", Canvas.GetLeft(element), 0);
                WriteAttribute("Canvas.Top", Canvas.GetTop(element), 0);
                WriteAttribute("Canvas.ZIndex", Canvas.GetZIndex(element), 0);
            }
            else if (element.Parent is Grid)
            {
                WriteAttribute("Grid.Row", Grid.GetRow(element), 0);
                WriteAttribute("Grid.Column", Grid.GetColumn(element), 0);
                WriteAttribute("Grid.RowSpan", Grid.GetRowSpan(element), 1);
                WriteAttribute("Grid.ColumnSpan", Grid.GetColumnSpan(element), 1);
            }

            WriteAttribute("Width", element.Width, 0);
            WriteAttribute("Height", element.Height, 0);
            WriteAttribute("MinWidth", element.MinWidth, 0);
            WriteAttribute("MaxWidth", element.MaxWidth, double.PositiveInfinity);
            WriteAttribute("MinHeight", element.MinHeight, 0);
            WriteAttribute("MaxHeight", element.MaxHeight, double.PositiveInfinity);
            WriteAttribute("Margin", element.Margin, DefaultThickness);
            WriteAttribute("HorizontalAlignment", element.HorizontalAlignment, HorizontalAlignment.Stretch);
            WriteAttribute("VerticalAlignment", element.VerticalAlignment, VerticalAlignment.Stretch);
            WriteAttribute("Opacity", element.Opacity, 1);
            WriteAttribute("Visibility", element.Visibility, Visibility.Visible);
            WriteAttribute("IsHitTestVisible", element.IsHitTestVisible, true);
            WriteAttribute("RenderTransformOrigin", element.RenderTransformOrigin, DefaultPoint);
            WriteAttribute("Cursor", element.Cursor, Cursors.Arrow);

            //note: order is on most used type first. TextBox must appear before Control, since TextBox is a control

            // TODO: what about ItemsPresenter and Popup??
            Panel panel = null;
            Control control = null;

            Border border = element as Border;
            if (border != null)
            {
                WriteInnerElement(border, typeName);
                goto End;
            }
            panel = element as Panel;
            if (panel != null)
            {
                WriteInnerElement(panel, typeName);
                goto End;
            }
            Shape shape = element as Shape;
            if (shape != null)
            {
                WriteInnerElement(shape, typeName);
                goto End;
            }
            TextBox textBox = element as TextBox;
            if (textBox != null)
            {
                WriteInnerElement(textBox, typeName);
                goto End;
            }
            control = element as Control;
            if (control != null)
            {
                WriteInnerElement(control, typeName);
                goto End;
            }
            TextBlock textblock = element as TextBlock;
            if (textblock != null)
            {
                WriteInnerElement(textblock, typeName);
                goto End;
            }

            Image image = element as Image;
            if (image != null)
            {
                WriteInnerElement(image, typeName);
                goto End;
            }
            MediaElement media = element as MediaElement;
            if (media != null)
            {
                WriteInnerElement(media, typeName);
                goto End;
            }
            MultiScaleImage multiScaleImage = element as MultiScaleImage;
            if (multiScaleImage != null)
            {
                WriteInnerElement(multiScaleImage, typeName);
                goto End;
            }
            Glyphs glyphs = element as Glyphs;
            if (glyphs != null)
            {
                WriteInnerElement(glyphs, typeName);
                goto End;
            }

        End:

            if (element.Triggers != null && element.Triggers.Count > 0)
            {
                WriteStartElement(string.Format("{0}.Triggers", typeName));
                WriteElement(element.Triggers);
                WriteEndElement();
            }
            if (element.Clip != null)
            {
                WriteStartElement(string.Format("{0}.Clip", typeName));
                WriteElement(element.Clip);
                WriteEndElement();
            }
            if (element.OpacityMask != null)
            {
                WriteStartElement(string.Format("{0}.OpacityMask", typeName));
                WriteElement(element.OpacityMask);
                WriteEndElement();
            }
            if (element.RenderTransform != null)
            {
                // skip writing identity matrices
                MatrixTransform matrix = element.RenderTransform as MatrixTransform;
                if (matrix == null || (matrix != null && !matrix.Matrix.IsIdentity))
                {
                    WriteStartElement(string.Format("{0}.RenderTransform", typeName));
                    WriteElement(element.RenderTransform);
                    WriteEndElement();
                }
            }
            bool result = true;
            if (element.GetType().Equals(typeof(TextBox)) || element.GetType().Equals(typeof(RadioButton)) || element.GetType().Equals(typeof(CheckBox)) || element.GetType().Equals(typeof(Button)))
            {

            }
            else
            {
                result = WriteContent(element);
            }

            WriteEndElement();

            return result;
        }

        private void WriteInnerElement(Shape shape, string typeName)
        {
            Color? fill;
            if (TryParseColorValue(shape.Fill, out fill))
            {
                WriteAttribute("Fill", fill.Value, DefaultColor);
            }

            Color? stroke;
            if (TryParseColorValue(shape.Stroke, out stroke))
            {
                WriteAttribute("Stroke", stroke.Value, DefaultColor);
            }

            WriteAttribute("StrokeDashArray", shape.StrokeDashArray);
            WriteAttribute("StrokeDashCap", shape.StrokeDashCap, PenLineCap.Flat);
            WriteAttribute("StrokeDashOffset", shape.StrokeDashOffset, 0);
            WriteAttribute("StrokeEndLineCap", shape.StrokeEndLineCap, PenLineCap.Flat);
            WriteAttribute("StrokeLineJoin", shape.StrokeLineJoin, PenLineJoin.Miter);
            WriteAttribute("StrokeMiterLimit", shape.StrokeMiterLimit, 10);
            WriteAttribute("StrokeStartLineCap", shape.StrokeStartLineCap, PenLineCap.Flat);
            WriteAttribute("StrokeThickness", shape.StrokeThickness, 1);
            WriteAttribute("Stretch", shape.Stretch, Stretch.None);

            if (fill == null && shape.Fill != null)
            {
                WriteStartElement(string.Format("{0}.Fill", typeName));
                WriteElement(shape.Fill);
                WriteEndElement();
            }
            if (stroke == null && shape.Stroke != null)
            {
                WriteStartElement(string.Format("{0}.Stroke", typeName));
                WriteElement(shape.Stroke);
                WriteEndElement();
            }
        }
        private void WriteInnerElement(Glyphs glyphs, string typeName)
        {
            //TODO: implement
        }
        private void WriteInnerElement(Border border, string typeName)
        {
            Color? background;
            if (TryParseColorValue(border.Background, out background))
            {
                WriteAttribute("Background", background.Value, DefaultColor);
            }
            Color? borderBrush;
            if (TryParseColorValue(border.BorderBrush, out borderBrush))
            {
                WriteAttribute("BorderBrush", borderBrush.Value, DefaultColor);
            }
            WriteAttribute("BorderThickness", border.BorderThickness, DefaultThickness);
            WriteAttribute("CornerRadius", border.CornerRadius, DefaultCornerRadius);
            WriteAttribute("Padding", border.Padding, DefaultThickness);

            if (background == null && border.Background != null)
            {
                WriteStartElement(string.Format("{0}.Background", typeName));
                WriteElement(border.Background);
                WriteEndElement();
            }
            if (borderBrush == null && border.BorderBrush != null)
            {
                WriteStartElement(string.Format("{0}.BorderBrush", typeName));
                WriteElement(border.BorderBrush);
                WriteEndElement();
            }
        }
        private void WriteInnerElement(TextBox textBox, string typeName)
        {
            //TODO: implement
            string text;
            if (TryParseTextValue(textBox, out text))
            {
                WriteAttribute("Text", text);
            }

            Color? background;
            if (TryParseColorValue(textBox.Background, out background))
            {
                WriteAttribute("Background", background.Value, Colors.Black);
            }

            Color? foreground;
            if (TryParseColorValue(textBox.Foreground, out foreground))
            {
                WriteAttribute("Foreground", foreground.Value, Colors.Black);
            }
           

            Color? borderBrush;
            if (TryParseColorValue(textBox.BorderBrush, out borderBrush))
            {
                WriteAttribute("BorderBrush", borderBrush.Value, DefaultColor);
            }
            WriteAttribute("BorderThickness", textBox.BorderThickness, DefaultThickness);
            WriteAttribute("FontFamily", textBox.FontFamily, "Portable User Interface");
            WriteAttribute("FontSize", textBox.FontSize, 11);
            WriteAttribute("FontStretch", textBox.FontStretch, FontStretches.Normal);
            WriteAttribute("FontStyle", textBox.FontStyle, FontStyles.Normal);
            WriteAttribute("FontWeight", textBox.FontWeight, FontWeights.Normal);

            WriteAttribute("HorizontalContentAlignment", textBox.HorizontalContentAlignment, HorizontalAlignment.Left);
            WriteAttribute("VerticalContentAlignment", textBox.VerticalContentAlignment, VerticalAlignment.Top);
            WriteAttribute("Padding", textBox.Padding, DefaultThickness);

            WriteAttribute("MaxLength", textBox.MaxLength,5000);
            WriteAttribute("IsTabStop", textBox.IsTabStop, true);
            WriteAttribute("TabIndex", textBox.TabIndex, -1);
            WriteAttribute("TabNavigation", textBox.TabNavigation, KeyboardNavigationMode.Local);

            if (background == null && textBox.Background != null)
            {
                WriteStartElement(string.Format("{0}.Background", typeName));
                WriteElement(textBox.Background);
                WriteEndElement();
            }
            if (foreground ==null && textBox.Foreground != null)
            {
                WriteStartElement(string.Format("{0}.Foreground", typeName));
                WriteElement(textBox.Foreground);
                WriteEndElement();
            }
            if (borderBrush == null && textBox.BorderBrush != null)
            {
                WriteStartElement(string.Format("{0}.BorderBrush", typeName));
                WriteElement(textBox.BorderBrush);
                WriteEndElement();
            }

           
        }
        private void WriteInnerElement(Image image, string typeName)
        {
            BitmapImage bitmap = image.Source as BitmapImage;
            if (bitmap != null)
            {
                WriteAttribute("Source", bitmap.UriSource);
            }
            WriteAttribute("Stretch", image.Stretch, Stretch.Uniform);
        }
        private void WriteInnerElement(MediaElement mediaElement, string typeName)
        {
            //TODO: implement
            if (mediaElement.Source != null)
            {
                WriteAttribute("Source", mediaElement.Source);
            }
            WriteAttribute("Stretch", mediaElement.Stretch, Stretch.Uniform);
        }
        //private void WriteInnerElement(MultiScaleImage image, string typeName)
        //{
        //    //TODO: implement
        //}
        private void WriteInnerElement(TextBlock textBlock, string typeName)
        {
            string text;
            if (TryParseTextValue(textBlock, out text))
            {
                WriteAttribute("Text", text);
            }
            WriteAttribute("FontFamily", textBlock.FontFamily, "Portable User Interface");
            WriteAttribute("FontSize", textBlock.FontSize, 14.666);
            WriteAttribute("FontStretch", textBlock.FontStretch, FontStretches.Normal);
            WriteAttribute("FontStyle", textBlock.FontStyle, FontStyles.Normal);
            WriteAttribute("FontWeight", textBlock.FontWeight, FontWeights.Normal);
            WriteAttribute("TextDecorations", textBlock.TextDecorations);
            WriteAttribute("TextWrapping", textBlock.TextWrapping, TextWrapping.NoWrap);
            WriteAttribute("TextAlignment", textBlock.TextAlignment, TextAlignment.Left);

            Color? foreground;
            if (TryParseColorValue(textBlock.Foreground, out foreground))
            {
                WriteAttribute("Foreground", foreground.Value, Colors.Black);
            }
            else if (textBlock.Foreground != null)
            {
                WriteStartElement(string.Format("{0}.Foreground", typeName));
                WriteElement(textBlock.Foreground);
                WriteEndElement();
            }
            if (text == null && textBlock.Inlines != null)
            {
                if (textBlock.Inlines.Count == 1 && textBlock.Inlines[0] is Run)
                {
                    this.writer.WriteString(((Run)textBlock.Inlines[0]).Text);
                }
                else
                {
                    foreach (Inline inline in textBlock.Inlines)
                    {
                        WriteElement(inline);
                    }
                }
            }
        }
        protected virtual void WriteInnerElement(Control control, string typeName)
        {
            string text = "";
            if (TryParseTextValue(control, out text))
            {
                WriteAttribute("Content", text);
            }

            string ischecked = "";
            if (TryParseTextValueIsCheck(control, out ischecked))
            {
                WriteAttribute("IsChecked", ischecked);
            }

            Color? background;
            if (TryParseColorValue(control.Background, out background))
            {
                WriteAttribute("Background", background.Value, Colors.Black);
            }
            Color? foreground;
            if (TryParseColorValue(control.Foreground, out foreground))
            {
                WriteAttribute("Foreground", foreground.Value, Colors.Black);
            }
            Color? borderBrush;
            if (TryParseColorValue(control.BorderBrush, out borderBrush))
            {
                WriteAttribute("BorderBrush", borderBrush.Value, DefaultColor);
            }
            WriteAttribute("BorderThickness", control.BorderThickness, DefaultThickness);

            WriteAttribute("FontFamily", control.FontFamily, "Portable User Interface");
            WriteAttribute("FontSize", control.FontSize, 11);
            WriteAttribute("FontStretch", control.FontStretch, FontStretches.Normal);
            WriteAttribute("FontStyle", control.FontStyle, FontStyles.Normal);
            WriteAttribute("FontWeight", control.FontWeight, FontWeights.Normal);

            WriteAttribute("HorizontalContentAlignment", control.HorizontalContentAlignment, HorizontalAlignment.Left);
            WriteAttribute("VerticalContentAlignment", control.VerticalContentAlignment, VerticalAlignment.Top);
            WriteAttribute("Padding", control.Padding, DefaultThickness);

            WriteAttribute("IsEnabled", control.IsEnabled, true);
            WriteAttribute("IsTabStop", control.IsTabStop, true);
            WriteAttribute("TabIndex", control.TabIndex, -1);
            WriteAttribute("TabNavigation", control.TabNavigation, KeyboardNavigationMode.Local);

            if (background == null && control.Background != null)
            {
                WriteStartElement(string.Format("{0}.Background", typeName));
                WriteElement(control.Background);
                WriteEndElement();
            }
            if (foreground == null && control.Foreground != null)
            {
                WriteStartElement(string.Format("{0}.Foreground", typeName));
                WriteElement(control.Foreground);
                WriteEndElement();
            }
            if (borderBrush == null && control.BorderBrush != null)
            {
                WriteStartElement(string.Format("{0}.BorderBrush", typeName));
                WriteElement(control.BorderBrush);
                WriteEndElement();
            }

            // a custom class derived from Control
            WriteCustomProperties(control, typeName, typeof(Control));
        }

        private static bool TryParseTextValue(TextBlock textBlock, out string value)
        {
            value = null;
            bool result = false;

            if (textBlock.Inlines != null && textBlock.Inlines.Count == 1)
            {
                Run run = textBlock.Inlines[0] as Run;
                if (run != null && run.Text == textBlock.Text && run.Text.IndexOf('\n') == -1)
                {
                    value = run.Text;
                    result = true;
                }
            }

            return result;
        }
        #region Ny create
        private static bool TryParseTextValue(TextBox textBox, out string value)
        {
            value = null;
            bool result = false;

            if (textBox.Text != null)
            {
                value = textBox.Text;
                result = true;
            }

            return result;
        }
        private static bool TryParseTextValue(Control control, out string value)
        {
            value = null;
            bool result = false;

            if (control.GetType().Equals(typeof(RadioButton)))
            {
                RadioButton radio = control as RadioButton;
                value = radio.Content.ToString();
                result = true;
            }

            else if (control.GetType().Equals(typeof(CheckBox)))
            {
                CheckBox checkbox = control as CheckBox;
                value = checkbox.Content.ToString();
                result = true;
            }

            else if (control.GetType().Equals(typeof(Button)))
            {
                Button button = control as Button;
                value = button.Content.ToString();
                result = true;
            }
            return result;
        }
        private static bool TryParseTextValueIsCheck(Control control, out string value)
        {
            value = null;
            bool result = false;

            if (control.GetType().Equals(typeof(RadioButton)))
            {
                RadioButton radio = control as RadioButton;
                value = radio.IsChecked.GetValueOrDefault().ToString();
                result = true;
            }
            else if (control.GetType().Equals(typeof(CheckBox)))
            {
                CheckBox checkbox = control as CheckBox;
                value = checkbox.IsChecked.GetValueOrDefault().ToString();
                result = true;
            }

            return result;
        }
      
        #endregion
        private static bool TryParseColorValue(Brush brush, out Color? value)
        {
            value = null;
            bool result = false;

            SolidColorBrush color = brush as SolidColorBrush;
            if (color != null )
            {
                //&& color.Opacity == 1 && color.RelativeTransform == null && color.Transform == null
                value = color.Color;
                result = true;
            }

            return result;
        }

        private static bool IsDerivableCoreType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type == typeof(Control) || type == typeof(UserControl) || type == typeof(ItemsControl) ||
                type == typeof(TextBox) || type == typeof(Panel) || type == typeof(Canvas) ||
                type == typeof(Grid) || type == typeof(StackPanel);
        }

        private void WriteCustomProperties(FrameworkElement element, string typeName, Type baseType)
        {
            if (!IsDerivableCoreType(element.GetType()))
            {
                //TODO: write custom XAML properties
            }
        }

        protected virtual string WriteStartElement(DependencyObject o)
        {
            Type type = o.GetType();
            string typeName = type.Name;

            WriteStartElement(typeName);
            string name = (string)o.GetValue(FrameworkElement.NameProperty);
            if (!string.IsNullOrEmpty(name))
            {
                WriteAttribute(PrefixXaml, "Name", NamespaceXaml, name);
            }

            return typeName;
        }
        protected void WriteStartElement(string name)
        {
            this.writer.WriteStartElement(name, NamespaceClient);
        }
        protected void WriteEndElement()
        {
            this.writer.WriteEndElement();
        }

        private void WriteInnerElement(Panel panel, string typeName)
        {
            Color? background;
            if (TryParseColorValue(panel.Background, out background))
            {
                WriteAttribute("Background", background.Value, DefaultColor);
            }

            Canvas canvas = panel as Canvas;
            if (canvas != null)
            {
                WriteCustomProperties(canvas, typeName, typeof(Canvas));
                goto End;
            }
            Grid grid = panel as Grid;
            if (grid != null)
            {
                WriteAttribute("ShowGridLines", grid.ShowGridLines, false);

                WriteCustomProperties(grid, typeName, typeof(Grid));

                if (grid.ColumnDefinitions.Count > 0)
                {
                    WriteStartElement("Grid.ColumnDefinitions");
                    foreach (ColumnDefinition column in grid.ColumnDefinitions)
                    {
                        WriteStartElement("ColumnDefinition");
                        WriteAttribute("Width", column.Width, new GridLength(1));
                        WriteAttribute("MinWidth", column.MinWidth, 0);
                        WriteAttribute("MaxWidth", column.MaxWidth, double.PositiveInfinity);
                        WriteEndElement();
                    }
                    WriteEndElement();
                }
                if (grid.RowDefinitions.Count > 0)
                {
                    WriteStartElement("Grid.RowDefinitions");
                    foreach (RowDefinition row in grid.RowDefinitions)
                    {
                        WriteStartElement("RowDefinition");
                        WriteAttribute("Height", row.Height, new GridLength(1));
                        WriteAttribute("MinHeight", row.MinHeight, 0);
                        WriteAttribute("MaxHeight", row.MaxHeight, double.PositiveInfinity);
                        WriteEndElement();
                    }
                    WriteEndElement();
                }
                goto End;
            }
            StackPanel stackPanel = panel as StackPanel;
            if (stackPanel != null)
            {
                WriteAttribute("Orientation", stackPanel.Orientation, Orientation.Vertical);

                WriteCustomProperties(stackPanel, typeName, typeof(StackPanel));
                goto End;
            }

            InkPresenter ink = panel as InkPresenter;
            if (ink != null)
            {
                if (ink.Strokes != null)
                {
                    WriteStartElement(string.Format("{0}.Strokes", typeName));
                    foreach (Stroke stroke in ink.Strokes)
                    {
                        WriteElement(stroke);
                    }
                    WriteEndElement();
                }
                goto End;
            }

            // a custom class derived from Panel
            WriteCustomProperties(panel, typeName, typeof(Panel));

        End:
            if (background == null && panel.Background != null)
            {
                WriteStartElement(string.Format("{0}.Background", typeName));
                WriteElement(panel.Background);
                WriteEndElement();
            }
        }
        public XamlWriter(XmlWriter writer, XamlWriterSettings settings)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            this.writer = writer;
            this.settings = settings;
        }
        public static XamlWriter CreateWriter(StringBuilder output, bool newLineOnAttributes, XamlWriterSettings settings)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.NewLineOnAttributes = newLineOnAttributes;
            xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;

            XmlWriter writer = XmlWriter.Create(output, xmlSettings);

            return new XamlWriter(writer, settings);
        }
    }

}
