using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePreprocessing
{
    public class Program
    {
        const string TEST_PATH = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test";
        const string OUTPUT_PATH = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Edited";
        const string STRETCHED_PATH = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\1.0 Stretched";
        static void Main(string[] args)
        {
            Console.WriteLine("Reading files...");
            var files = Directory.GetFiles(TEST_PATH);
            decimal maxAspectRatio = 0;
            decimal averageAspectRatioQuer = 0;
            decimal averageAspectRatioHoch = 0;
            Console.WriteLine("Loading images...");

            var images = files.Select(f => System.Drawing.Image.FromFile(f));

            //var querkant = images.Where(i => i.Width > i.Height);
            //var hochkant = images.Where(i => i.Width < i.Height);
            //var gleichkant = images.Where(i => i.Width == i.Height);

            //var querkantAspectRatio = querkant.Sum(q => (double)q.Width / (double)q.Height) / (double)querkant.Count();
            //Console.WriteLine($"Querkant: {querkant.Count()} Seitenverhältnis: {querkantAspectRatio}");

            //var hochkantAspectRatio = hochkant.Sum(q => (double)q.Width / (double)q.Height) / (double)hochkant.Count();
            //Console.WriteLine($"Hochkant: {hochkant.Count()} Seitenverhältnis: {hochkantAspectRatio}");

            //Console.WriteLine($"Gleichkant: {gleichkant.Count()}");

            int i = 0;
            foreach (var image in images)
            {
                var resized = ResizeImage(image, 64, 64);
                string fileName = Path.GetFileName(files.ElementAt(i));
                resized.Save(Path.Combine(STRETCHED_PATH, fileName), ImageFormat.Jpeg);
                i++;
            }

            //foreach (var file in files)
            //{
            //    var testFile = Path.Combine(TEST_PATH, file);//files[0];
            //    var testImage = System.Drawing.Image.FromFile(testFile);
            //    decimal aspectRatio = testImage.Width / testImage.Height;
            //    if (aspectRatio > 1)
            //    {
            //        //Querkant
            //        averageAspectRatioQuer += aspectRatio / files.Count();
            //    }
            //    else
            //    {
            //        //Hochkant
            //        averageAspectRatioHoch += aspectRatio / files.Count();
            //    }
            //    if (aspectRatio > maxAspectRatio)
            //    {
            //        Console.WriteLine($"Datei {testFile} w: {testImage.Width} h: {testImage.Height}");
            //        maxAspectRatio = aspectRatio;
            //    }
            //}
            //Console.WriteLine($"Average Quer aspect: {averageAspectRatioQuer}, Average Hoch aspect: {averageAspectRatioHoch}");
            //var resized = ResizeImage(testImage, 128, 128);
            //resized.Save(Path.Combine(OUTPUT_PATH, "1.jpg"), ImageFormat.Jpeg);
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
