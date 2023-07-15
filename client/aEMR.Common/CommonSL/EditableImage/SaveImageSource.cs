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
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;

namespace aEMR.Common
{
    public class SaveImageSource
    {
        // TxD 24/05/2018: The following method is BRAND NEW and thus requires TESTING 
        public static byte[] SaveImageToByteArray(Image bmp)
        {
            WriteableBitmap writeableBitmap = (WriteableBitmap)bmp.Source;
            if (writeableBitmap != null)
            {
                int height = writeableBitmap.PixelHeight;
                int width = writeableBitmap.PixelWidth;
                var stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
                var bitmapData = new byte[height * stride];
                writeableBitmap.CopyPixels(bitmapData, stride, 0);
                return bitmapData;
            }
            return null;
        }

        // TxD 24/05/2018: Commented out the following coming from SILVERLIGHT and replaced with 
        //                  a corresponding version above that will work for WPF because 
        //                  WriteableBitmap in WPF does not expose Pixels 
        //                  the code that had error was:
        //                  int pixel = writeableBitmap.Pixels[(i * width) + j];
        //
        //public static byte[] SaveImageToByteArray(Image bmp)
        //{
        //    WriteableBitmap writeableBitmap = (WriteableBitmap)bmp.Source;
        //    if (writeableBitmap != null)
        //    {
        //        int height = writeableBitmap.PixelHeight;
        //        int width = writeableBitmap.PixelWidth;
        //        // Create an EditableImage for encoding
        //        EditableImage ei = new EditableImage(width, height);
        //        // Transfer pixels from the WriteableBitmap to the EditableImage
        //        for (int i = 0; i < height; i++)
        //        {
        //            for (int j = 0; j < width; j++)
        //            {
        //                int pixel = writeableBitmap.Pixels[(i * width) + j];
        //                ei.SetPixel(j, i,
        //                                    (byte)((pixel >> 16) & 0xFF),   // R
        //                                    (byte)((pixel >> 8) & 0xFF),    // G
        //                                    (byte)(pixel & 0xFF),           // B
        //                                    (byte)((pixel >> 24) & 0xFF)    // A
        //                  );
        //            }
        //        }
        //        // Generate a PNG stream from the EditableImage and convert to byte[]
        //        Stream png = ei.GetStream;
        //        int len = (int)png.Length;
        //        byte[] bytes = new byte[len];
        //        png.Read(bytes, 0, len);
        //        return bytes;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        // TxD 24/05/2018: Commented out the following coming from SILVERLIGHT because of the same reason above
        //
        //public static void SaveImageAsPng(WriteableBitmap writeableBitmap)
        //{
        //    if (writeableBitmap != null)
        //    {
        //        SaveFileDialog sfd = new SaveFileDialog();
        //        sfd.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
        //        sfd.DefaultExt = ".png";
        //        sfd.FilterIndex = 1;
        //        if ((bool)sfd.ShowDialog())
        //        {
        //            using (Stream fs = sfd.OpenFile())
        //            {
        //                int height = writeableBitmap.PixelHeight;
        //                int width = writeableBitmap.PixelWidth;
        //                // Create an EditableImage for encoding
        //                EditableImage ei = new EditableImage(width, height);
        //                // Transfer pixels from the WriteableBitmap to the EditableImage
        //                for (int i = 0; i < height; i++)
        //                {
        //                    for (int j = 0; j < width; j++)
        //                    {
        //                        int pixel = writeableBitmap.Pixels[(i * width) + j];

        //                        ei.SetPixel(j, i,
        //                                            (byte)((pixel >> 16) & 0xFF),   // R
        //                                            (byte)((pixel >> 8) & 0xFF),    // G
        //                                            (byte)(pixel & 0xFF),           // B
        //                                            (byte)((pixel >> 24) & 0xFF)    // A
        //                          );
        //                    }
        //                }
        //                // Generate a PNG stream from the EditableImage and convert to byte[]
        //                Stream png = ei.GetStream;
        //                int len = (int)png.Length;
        //                byte[] bytes = new byte[len];
        //                png.Read(bytes, 0, len);
        //                // Write the PNG to disk
        //                fs.Write(bytes, 0, len);
        //            }

        //        }

        //    }
        //}
        public static ObservableCollection<byte[]> SaveArrayImage(StackPanel thumbs)
        {
            ObservableCollection<byte[]> MainArray = new ObservableCollection<byte[]>();
            string name = "";
            int i = 0;
            for (int idx = 0; idx < thumbs.Children.Count; idx++)
            {
                object obj = thumbs.Children[idx];
                if (obj.GetType().Equals(typeof(CheckBox)))
                {
                    if (((CheckBox)obj).IsChecked != null && ((CheckBox)obj).IsChecked == true)
                    {
                        if (name == "")
                        {
                            name += ((CheckBox)obj).Name;
                        }
                        else
                        {
                            name += ";" + ((CheckBox)obj).Name;
                        }
                        i++;
                    }

                }
            }
            string[] valueArray = name.Split(';');

            Image[] img = new Image[valueArray.Length];
            for (int n = 0; n < valueArray.Length; n++)
            {
                for (int idx = 0; idx < thumbs.Children.Count; idx++)
                {
                    object obj = thumbs.Children[idx];
                    if (obj.GetType().Equals(typeof(Image)))
                    {
                        if (((Image)obj).Name == valueArray[n].ToString().Replace("chk", "img"))
                        {
                            MainArray.Add(SaveImageToByteArray((Image)obj));

                        }
                    }
                }
            }
            return MainArray;
        }
    }
}
