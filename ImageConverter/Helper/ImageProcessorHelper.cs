using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImageConverter.Helper
{
   internal class ImageProcessorHelper
    {

       internal ImageProcessorHelper()
       { }

       internal void ReduceImageByPercent(string sourceImageFilePath, string newImageFilePath, int percent)
       {
           if (!File.Exists(sourceImageFilePath))
               return;

           if (File.Exists(newImageFilePath))
           {
               //var prompt = System.Windows.MessageBox.Show("This will override the existing file.\n Do you want to proceed?", "Warning!", System.Windows.MessageBoxButton.YesNo);
               //if (prompt == System.Windows.MessageBoxResult.No)
               return;
           }

           try
           {
               using (var originalImage = Image.FromFile(sourceImageFilePath))
               {
                   var percentage = (float)percent / 100;
                   var srcHeight = originalImage.Height;
                   var srcWidth = originalImage.Width;
                   var destHeight = (int)(srcHeight * percentage);
                   var destWidth = (int)(srcWidth * percentage);
                   var bmImage = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
                   bmImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                   using (var grImage = Graphics.FromImage(bmImage))
                   {
                       grImage.CompositingQuality = CompositingQuality.HighQuality;
                       grImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
                       grImage.DrawImage(originalImage,
                           new Rectangle(0, 0, destWidth, destHeight),
                           new Rectangle(0, 0, srcWidth, srcHeight),
                           GraphicsUnit.Pixel);
                   }

                   //var imageCodecinfo = GetEncoderInfo(originalImage.RawFormat);
                   //var encoder = Encoder.Quality;
                   //var encoderParameters = new EncoderParameters(1);
                   //var encoderParameter = new EncoderParameter(encoder, )
                   TrySaveImage(bmImage, originalImage.RawFormat, newImageFilePath);
               }
           }
           catch (Exception) { }
       }

       internal void ReduceImageByScale(string sourceImageFilePath, string newImageFilePath, int sizeWidth, int sizeHeight)
       {
           if (!File.Exists(sourceImageFilePath))
               return;

           if (File.Exists(newImageFilePath))
           {
               //var prompt = System.Windows.MessageBox.Show("This will override the existing file.\n Do you want to proceed?", "Warning!", System.Windows.MessageBoxButton.YesNo);
               //if (prompt == System.Windows.MessageBoxResult.No)
               return;
           }
           try
           {
               using (var originalImage = Image.FromFile(sourceImageFilePath))
               {
                   var srcWidth = originalImage.Width;
                   var srcHeight = originalImage.Height;

                   var ratioX = (float)sizeWidth / (float)srcWidth;
                   var ratioY = (float)sizeHeight / (float)srcHeight;
                   var ratio = Math.Min(ratioX, ratioY);
                   var destWidth = (int)(srcWidth * ratio);
                   var destHeight = (int)(srcHeight * ratio);

                   var bmImage = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
                   bmImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                   using (var grImage = Graphics.FromImage(bmImage))
                   {
                       grImage.CompositingQuality = CompositingQuality.HighQuality;
                       grImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
                       grImage.DrawImage(originalImage,
                           new Rectangle(0, 0, destWidth, destHeight),
                           new Rectangle(0, 0, srcWidth, srcHeight),
                           GraphicsUnit.Pixel);
                   }

                   //var imageCodecinfo = GetEncoderInfo(originalImage.RawFormat);
                   //var encoder = Encoder.Quality;
                   //var encoderParameters = new EncoderParameters(1);
                   //var encoderParameter = new EncoderParameter(encoder, )
                   TrySaveImage(bmImage, originalImage.RawFormat, newImageFilePath);
                   bmImage.Dispose();
               }
           }
           catch (Exception) { }
       }

       internal void ConvertImageFormat(string imageFilePath, string format)
       {
           if (File.Exists(imageFilePath))
           {
               var fileInfo = new FileInfo(imageFilePath);
               using (var image = Image.FromFile(imageFilePath))
               {
                   switch (format)
                   {
                       case ".ico":
                             TrySaveImage(image,ImageFormat.Icon, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".jpg":
                             TrySaveImage(image, ImageFormat.Jpeg, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".png":
                             TrySaveImage(image, ImageFormat.Png, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".gif":
                             TrySaveImage(image, ImageFormat.Gif, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".bmp":
                             TrySaveImage(image, ImageFormat.Bmp, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".tif":
                             TrySaveImage(image, ImageFormat.Tiff, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       case ".wmf":
                             TrySaveImage(image, ImageFormat.Wmf, fileInfo.FullName.Replace(fileInfo.Extension, format));
                             break;
                       default:
                           break;
                   }
               }
           }

       }

       private void TrySaveImage(Image image, ImageFormat format, string filePath)
       {
           try
           {
               if (File.Exists(filePath))
               {
                   //var prompt = System.Windows.MessageBox.Show("This will override the existing file.\n Do you want to proceed?", "Warning!", System.Windows.MessageBoxButton.YesNo);
                   //if (prompt == System.Windows.MessageBoxResult.No)
                       return;
               }
               image.Save(filePath, format);

           }
           catch (Exception)
           { }
       }

       private ImageCodecInfo GetEncoderInfo(ImageFormat format)
       {
           return ImageCodecInfo.GetImageDecoders().Where(f => f.FormatID == format.Guid).SingleOrDefault();
       }
    }
}
