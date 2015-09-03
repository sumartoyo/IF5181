using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Warna
{
    public static class Processor
    {
        //public static Bitmap ResizeImageTo500(Image image)
        //{
        //    var maxLength = 500;

        //    var destWidth = image.Width;
        //    var destHeight = image.Height;

        //    if (image.Width > image.Height)
        //    {
        //        destWidth = maxLength;
        //        destHeight = (int)Math.Round((float)maxLength * (float)image.Height / (float)image.Width);
        //    }
        //    else
        //    {
        //        destHeight = maxLength;
        //        destWidth = (int)Math.Round((float)maxLength * (float)image.Width / (float)image.Height);
        //    }

        //    var destRect = new Rectangle(0, 0, destWidth, destHeight);
        //    var destImage = new Bitmap(destWidth, destHeight);
        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}

        public static unsafe ColorData Count(string filePath)
        {
            var result = new ColorData();

            using (var bitmap = new Bitmap(filePath))
            {
                var imageData = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                var pointer = (byte*)imageData.Scan0.ToPointer();
                var colors = new int[256, 256, 256];
                var bpp = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                var imageLength = bitmap.Width * bitmap.Height * bpp;
                for (var i = 0; i < imageLength; i += bpp)
                {
                    byte b = bpp > 0 ? pointer[i] : (byte)0;
                    byte g = bpp > 0 ? pointer[i + 1] : (byte)0;
                    byte r = bpp > 0 ? pointer[i + 2] : (byte)0;

                    // count unique
                    if (colors[r, g, b] == 0)
                    {
                        result.CountUnique += 1;
                    }
                    colors[r, g, b] += 1;

                    // count most color
                    if (colors[r, g, b] > result.CountMostColor)
                    {
                        result.CountMostColor = colors[r, g, b];
                        result.MostColor = new Color
                        {
                            R = r,
                            G = g,
                            B = b
                        };
                    }

                    // count histogram
                    result.CountR[r] += 1;
                    result.CountG[g] += 1;
                    result.CountB[b] += 1;
                    result.CountGrayScale[(int)Math.Floor((r + g + b + 0.0) / 3)] += 1;
                }

                bitmap.UnlockBits(imageData);
            }

            return result;
        }
    }

    public class ColorData
    {
        public int CountUnique;
        public Color MostColor;
        public int CountMostColor;
        public int[] CountR;
        public int[] CountG;
        public int[] CountB;
        public int[] CountGrayScale;

        public ColorData()
        {
            CountUnique = 0;
            MostColor = new Color();
            CountMostColor = 0;
            CountR = new int[256];
            CountG = new int[256];
            CountB = new int[256];
            CountGrayScale = new int[256];
        }
    }

    public class Color
    {
        public byte R;
        public byte G;
        public byte B;

        public Color()
        {
            R = 0;
            G = 0;
            B = 0;
        }
    }
}
