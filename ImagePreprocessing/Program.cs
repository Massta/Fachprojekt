﻿using System;
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
        const string PATH_MNIST = @"C:\Users\Julius Jacobsohn\OneDrive\Dokumente\MNIST";
        const string PATH_TEST = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test";
        const string PATH_TRAIN = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train";
        const string PATH_OUTPUT = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Edited";
        const string PATH_STRETCHED = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\1.0 Stretched";
        const string PATH_TEST_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test_Small_Grayscale";
        const string PATH_TRAIN_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale";
        static void Main(string[] args)
        {
            var linesTraining = File.ReadAllLines(Path.Combine(PATH_MNIST, "mnist_train_small_appended.csv")).Take(50);
            var linesTesting = File.ReadAllLines(Path.Combine(PATH_MNIST, "mnist_test_appended - Copy.csv")).Take(50);
            var lineList = new List<string>();
            lineList.AddRange(linesTraining);
            lineList.AddRange(linesTesting);
            lineList = lineList.OrderBy(v => Guid.NewGuid()).ToList();
            File.WriteAllLines(Path.Combine(PATH_MNIST, "mnist_test_appended.csv"), lineList.Take(100).ToArray());
            //var lines = File.ReadAllLines(Path.Combine(PATH_MNIST, "mnist_train_small_appended.csv"));
            //Console.WriteLine("Read lines");
            //StringBuilder allLines = new StringBuilder();
            //int counter = 0;
            //foreach (var line in lines)
            //{
            //    var numbers = line.Split(',');
            //    var label = numbers[0];
            //    numbers = numbers.Skip(1).ToArray();
            //    allLines.Append(label);
            //    for (int i = 0; i < numbers.Count(); i++)
            //    {
            //        allLines.Append("," + numbers[i]);
            //        if (i % 28 == 0)
            //        {
            //            allLines.Append(",0,0,0,0");
            //        }
            //    }
            //    for (int j = 0; j < 128; j++)
            //    {
            //        allLines.Append(",0");
            //    }
            //    allLines.Append("\n");
            //    //var newLine = line;

            //    ////for(int i = 0; i < 240; i++)
            //    ////{
            //    ////    newLine += ",0";
            //    ////}
            //    //allLines.AppendLine(newLine);
            //    //Console.WriteLine(counter);
            //    counter++;
            //}
            //Console.WriteLine("Appended");
            //var result = allLines.ToString().TrimEnd();
            //for (int i = 0; i < 0; i++)
            //{
            //    var line = result.Split('\n');
            //    var lineData = line[i].Split(',');
            //    PrintImage(lineData.Skip(1).Select(l => int.Parse(l)).ToArray(), 32, Path.Combine(PATH_MNIST, "Images", "image_" + i + ".bmp"));
            //}
            //Console.WriteLine("Trimmed");
            //File.AppendAllText(Path.Combine(PATH_MNIST, "mnist_train_appended.csv"), result);
            //Console.WriteLine("Saved");
            ////var lines = File.ReadAllLines(Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train_num.csv"));
            ////var cats = lines.Take(1000).ToArray();
            ////var dogs = lines.Skip(12500).Take(1000).ToArray();
            ////StringBuilder newCsv = new StringBuilder();
            ////int catC = 0;
            ////int dogC = 0;
            ////for (int j = 0; j < 2000; j++)
            ////{
            ////    if (j % 2 == 0)
            ////    {
            ////        newCsv.AppendLine(cats[catC]);
            ////        catC++;
            ////    }
            ////    else
            ////    {
            ////        newCsv.AppendLine(dogs[dogC]);
            ////        dogC++;
            ////    }
            ////}
            ////File.WriteAllText(Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train_num_ordered_small.csv"), newCsv.ToString());
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

        public static void PrintImage(int[] data, int width, string path)
        {
            Bitmap b = new Bitmap(width, width);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var grayScale = data[j * width + i];
                    Color nc = Color.FromArgb(255, grayScale, grayScale, grayScale);
                    b.SetPixel(i, j, nc);
                }
            }
            b.Save(path);
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
