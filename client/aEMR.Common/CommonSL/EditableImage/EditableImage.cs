using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace aEMR.Common
{
    public class EditableImage
    {
        private int _width;
        private int _height;
        private bool _init;
        private byte[] _buffer;
        private int _rowLength;

        protected event EventHandler<EditableImageErrorEventArgs> ImageError;

        public EditableImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Width after the EditableImage has been initialized");
                }
                else if ((value <= 0) || (value > 2047))
                {
                    OnImageError("Error: Width must be between 0 and 2047");
                }
                else
                {
                    _width = value;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Height after the EditableImage has been initialized");
                }
                else if ((value <= 0) || (value > 2047))
                {
                    OnImageError("Error: Height must be between 0 and 2047");
                }
                else
                {
                    _height = value;
                }
            }
        }

        public int Length
        {
            get
            {
                return _buffer.Length;
            }
        }

        public void SetPixel(int col, int row, Color color)
        {
            SetPixel(col, row, color.R, color.G, color.B, color.A);
        }

        private void init()
        {
            if (!_init)
            {
                _rowLength = _width * 4 + 1;
                //_colLength = _height * 4 + 1;
                _buffer = new byte[_rowLength *_height];

                // Initialize
                for (int idx = 0; idx < _height; idx++)
                {
                    _buffer[idx * _rowLength] = 0;      // Filter bit
                }

                _init = true;
            }
        }

        public void SetPixel(int col, int row, byte red, byte green, byte blue, byte alpha)
        {
            init();

            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            // Set the pixel
            int start = _rowLength * row + col * 4 + 1;
            _buffer[start] = red;
            _buffer[start + 1] = green;
            _buffer[start + 2] = blue;
            _buffer[start + 3] = alpha;
        }

        public void BlendAlphaPixel(int col, int row, Color color)
        {
            BlendAlphaPixel(col, row, color.R, color.G, color.B, color.A);
        }

        public void BlendAlphaPixel(int col, int row, byte red, byte green, byte blue, byte alpha)
        {
            init();

            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            // Set the pixel
            int start = _rowLength * row + col * 4 + 1;
            BlendAlpha(start, red, green, blue, alpha);
            //_buffer[start++] = red;
            //_buffer[start++] = green;
            //_buffer[start++] = blue;
            //_buffer[start] = alpha;
        }

        private void BlendAlpha(int start, byte r, byte g, byte b, byte a)
        {
            double a1 = (Convert.ToDouble(a) / Convert.ToDouble(255));
            _buffer[start] = Convert.ToByte((1 - a1) * _buffer[start] + a1 * r);
            _buffer[start + 1] = Convert.ToByte((1 - a1) * _buffer[start + 1] + a1 * g);
            _buffer[start + 2] = Convert.ToByte((1 - a1) * _buffer[start + 2] + a1 * b);
        }

        public void SetPixels(EditableImage Image)
        {
            init();
            Array.Copy(Image._buffer, _buffer, Image._buffer.Length);
        }

        public void SetPixels(EditableImage Image, int col, int row)
        {
            init();
            int start = Convert.ToInt32(_rowLength * row) + Convert.ToInt32(col * 4) + 1;
            Array.Copy(Image._buffer, 0, _buffer, start, Image._buffer.Length);
        }

        public Color GetPixel(int col, int row)
        {
            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            Color color = new Color();
            int _base = _rowLength * row + col * 4 + 1;

            color.R = _buffer[_base];
            color.G = _buffer[_base + 1];
            color.B = _buffer[_base + 2];
            color.A = _buffer[_base + 3];

            return color;
        }

        public Stream GetStream
        {
            get
            {
                Stream stream;

                if (!_init)
                {
                    OnImageError("Error: Image has not been initialized");
                    stream = null;
                }
                else
                {
                    stream = AxPngEncoder.Encode(_buffer, _width, _height);
                }

                return stream;
            }
        }

        private void OnImageError(string msg)
        {
            if (null != ImageError)
            {
                EditableImageErrorEventArgs args = new EditableImageErrorEventArgs();
                args.ErrorMessage = msg;
                ImageError(this, args);
            }
        }

        protected class EditableImageErrorEventArgs : System.EventArgs
        {
            private string _errorMessage = string.Empty;

            public string ErrorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
        }

    }
}