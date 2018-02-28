using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HardcodedNetwork
{
    public class Utilities
    {
        public static Random rnd = new Random();
        //public static void StoreImages(string path, double[][][] images)
        //{
        //    for (int i = 0; i < images.Length; i++)
        //    {
        //        string name = images[i].Any(v => v.Any(x => x == 200)) ? "1" : "0";
        //        StoreImage(Path.Combine(path, i + "_" + name + ".bmp"), images[i]);
        //    }
        //}
        //public static void StoreImage(string path, double[][] image)
        //{
        //    Bitmap bitmap = new Bitmap(image.Length, image.Length);
        //    for (int i = 0; i < image.Length; i++)
        //    {
        //        for (int j = 0; j < image[i].Length; j++)
        //        {
        //            Color c = Color.FromArgb((int)image[i][j], (int)image[i][j], (int)image[i][j]);
        //            bitmap.SetPixel(i, j, c);
        //        }
        //    }
        //    bitmap.Save(path);
        //}
        public static double[][][] GetImages(int amount)
        {
            double[][][] images = new double[amount][][];
            for (int i = 0; i < amount; i++)
            {
                images[i] = GetDistortedImage(0, 100);
                if (GetRandomNumber(0, 2) > 0)
                {
                    //Create rectangle
                    int wI = GetRandomNumber(0, 5);
                    int hI = GetRandomNumber(0, 5);
                    //top row
                    images[i][wI][hI] = 200;
                    images[i][wI + 1][hI] = 200;
                    images[i][wI + 2][hI] = 200;
                    images[i][wI + 3][hI] = 200;
                    //left bar
                    images[i][wI][hI + 1] = 200;
                    images[i][wI][hI + 2] = 200;
                    //right bar
                    images[i][wI + 3][hI + 1] = 200;
                    images[i][wI + 3][hI + 2] = 200;
                    //bottom row
                    images[i][wI][hI + 3] = 200;
                    images[i][wI + 1][hI + 3] = 200;
                    images[i][wI + 2][hI + 3] = 200;
                    images[i][wI + 3][hI + 3] = 200;
                }
            }
            return images;
        }

        public static double[][] GetDistortedImage(int min, int max)
        {
            min /= 10;
            max /= 10;
            double[][] image = new double[8][];
            for (int i = 0; i < image.Length; i++)
            {
                image[i] = new double[8];
                for (int j = 0; j < image[i].Length; j++)
                {
                    image[i][j] = GetRandomNumber(min, max) * 10;
                }
            }
            return image;
        }

        public static bool HasSquare(double[][] inputData)
        {
            return inputData.Any(d => d.Any(x => x >= 200));
        }

        public static int GetRandomNumber(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static double GetRandomDouble(double min, double max)
        {
            return min + (max - min) * rnd.NextDouble();
        }
    }
}
