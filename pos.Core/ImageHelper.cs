using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace pos_system.pos.Core
{
    public static class ImageHelper
    {
        private static readonly string BasePath = ConfigurationManager.AppSettings["ImageBasePath"];
        private static readonly string DefaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default_product.jpg");

        public static Image LoadProductImage(string fileName)
        {
            try
            {
                // 1. Try to load specified product image
                if (!string.IsNullOrEmpty(fileName))
                {
                    string fullPath = Path.Combine(BasePath, fileName);
                    if (File.Exists(fullPath))
                    {
                        return LoadImageFromFile(fullPath);
                    }
                }

                // 2. Try to load default image from file
                if (File.Exists(DefaultImagePath))
                {
                    return LoadImageFromFile(DefaultImagePath);
                }

                // 3. Generate a default image programmatically
                return GenerateDefaultImage();
            }
            catch
            {
                // 4. Final fallback to generated image
                return GenerateDefaultImage();
            }
        }

        private static Image LoadImageFromFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Image.FromStream(fs);
            }
        }

        public static Image GenerateDefaultImage()
        {
            // Create a simple placeholder image
            const int size = 100;
            var image = new Bitmap(size, size);

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DarkGray, 0, 0, size - 1, size - 1);

                using (var font = new Font("Arial", 8))
                using (var format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString("No Image", font, Brushes.Black,
                        new RectangleF(0, 0, size, size), format);
                }
            }
            return image;
        }

        public static string SaveProductImage(Image image, int productId)
        {
            if (image == null) return null;

            string fileName = $"{productId}.jpg";
            string fullPath = Path.Combine(BasePath, fileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // Create a copy to avoid GDI+ errors
                using (Bitmap newImage = new Bitmap(image))
                {
                    // Save with high quality compression
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                    newImage.Save(fullPath, jpegEncoder, encoderParams);
                }

                return fileName;
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteProductImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string fullPath = Path.Combine(BasePath, fileName);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Ignore deletion errors
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static void CreateDefaultImageFile()
        {
            try
            {
                if (!File.Exists(DefaultImagePath))
                {
                    using (var image = GenerateDefaultImage())
                    {
                        image.Save(DefaultImagePath, ImageFormat.Jpeg);
                    }
                }
            }
            catch
            {
                // Ignore creation errors
            }
        }
    }
}