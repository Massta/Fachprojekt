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
        const string PATH_TEST = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test";
        const string PATH_TRAIN = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train";
        const string PATH_OUTPUT = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Edited";
        const string PATH_STRETCHED = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\1.0 Stretched";
        const string PATH_TEST_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test_Small_Grayscale";
        const string PATH_TRAIN_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale";
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train_num.csv"));
            var cats = lines.Take(1000).ToArray();
            var dogs = lines.Skip(12500).Take(1000).ToArray();
            StringBuilder newCsv = new StringBuilder();
            int catC = 0;
            int dogC = 0;
            for (int j = 0; j < 2000; j++)
            {
                if (j % 2 == 0)
                {
                    newCsv.AppendLine(cats[catC]);
                    catC++;
                }
                else
                {
                    newCsv.AppendLine(dogs[dogC]);
                    dogC++;
                }
            }
            File.WriteAllText(Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train_num_ordered_small.csv"), newCsv.ToString());
            //Console.WriteLine("Reading files...");
            //var files = Directory.GetFiles(PATH_TRAIN);
            //Console.WriteLine("Loading images...");

            //var csvPath = Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train.csv");

            //var images = files.Select(f => System.Drawing.Image.FromFile(f));

            //int i = 0;
            //StringBuilder csvText = new StringBuilder();
            //foreach (var image in images)
            //{
            //    var resized = ResizeImage(image, 64, 64);
            //    string fileName = Path.GetFileName(files.ElementAt(i));
            //    string label = fileName.Contains("cat") ? "cat" : "dog";
            //    //resized.Save(Path.Combine(STRETCHED_PATH, fileName), ImageFormat.Jpeg);
            //    Bitmap original = new Bitmap(resized);
            //    Bitmap grayscale = new Bitmap(original.Width, original.Height);
            //    int x, y;


            //    // Loop through the images pixels to reset color.
            //    for (x = 0; x < original.Width; x++)
            //    {
            //        for (y = 0; y < original.Height; y++)
            //        {
            //            Color oc = original.GetPixel(x, y);
            //            int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
            //            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
            //            grayscale.SetPixel(x, y, nc);
            //            csvText.Append($"{grayScale};");
            //        }
            //    }
            //    csvText.Append(label + "\n");
            //    grayscale.Save(Path.Combine(PATH_TRAIN_SMALL_GRAY, fileName), ImageFormat.Jpeg);
            //    i++;
            //}
            //File.WriteAllText(csvPath, csvText.ToString());
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
