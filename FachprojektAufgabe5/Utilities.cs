using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektAufgabe5
{
    public class Utilities
    {

        static Random random = new Random();

        public static void PrintResult(double[,] image, int imageIndex, int filterLevel, int filterIndex)
        {
            Bitmap grayscale = new Bitmap(image.GetLength(0), image.GetLength(0));
            int x, y;


            // Loop through the images pixels to reset color.
            for (x = 0; x < image.GetLength(0); x++)
            {
                for (y = 0; y < image.GetLength(0); y++)
                {
                    int grayScale = Clamp((int)(image[x, y]), 0, 255);
                    Color nc = Color.FromArgb(0, grayScale, grayScale, grayScale);
                    grayscale.SetPixel(x, y, nc);
                }
            }
            grayscale.Save(Path.Combine(Program.PATH_TRAIN_SMALL_GRAY_FILTER1, $"{imageIndex}.{filterLevel}.{filterIndex}.jpg"), ImageFormat.Jpeg);
        }

        public static int[][,] ReadCsv(string csvPath, out int[] labels)
        {
            var lines = File.ReadAllLines(csvPath);
            var takeLines = lines.OrderBy(l => l.Substring(2100, 3)).Take(100).ToArray();
            int[][,] data = new int[takeLines.Count()][,];
            labels = new int[takeLines.Count()];
            for (int i = 0; i < data.Count(); i++)
            {
                var line = takeLines[i];
                var lineData = line.Split(';');
                labels[i] = lineData[4096] == "cat" ? 0 : 1;
                var lineVector = lineData.Take(4096).Select(d => int.Parse(d)).ToArray();
                data[i] = ConvertMatrix(lineVector, 64, 64);
            }
            return data;
        }
        public static int[,] ConvertMatrix(int[] flat, int m, int n)
        {
            int[,] ret = new int[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    ret[i, j] = flat[i * m + j];
                }
            }
            return ret;

            //if (flat.Length != m * n)
            //{
            //    throw new ArgumentException("Invalid length");
            //}
            //int[,] ret = new int[m, n];
            //// BlockCopy uses byte lengths: a double is 8 bytes
            //Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(int));
            //return ret;
        }
        public static double Sigmoid(double x)
        {
            double exp_val = Math.Exp(-7 * x);
            return 1.0 / (1.0 + exp_val);
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }
        public static double[,] GenerateWeightMatrix(int size, double min, double max)
        {
            return GenerateWeightMatrix(size, size, min, max);
        }
        public static double[,] GenerateWeightMatrix(int m, int n, double min, double max)
        {
            double[,] weights = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                var tempWeights = GenerateWeightArray(n, min, max);
                for (int j = 0; j < tempWeights.Count(); j++)
                {
                    weights[i, j] = tempWeights[j];
                }
            }
            return weights;
        }
        public static double[] GenerateWeightArray(int size, double min, double max)
        {
            return GenerateWeights(size, min, max).ToArray();
        }
        public static IEnumerable<double> GenerateWeights(int amount, double min, double max)
        {
            object syncLock = new object();
            for (int i = 0; i < amount; i++)
            {
                lock (syncLock)
                {
                    yield return (random.NextDouble() * (max - min)) + min;
                }
            }
        }

        public static double GetRandomNumber(double min, double max)
        {
            return (random.NextDouble() * (max - min)) + min;
        }
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static double Normalize(double colour)
        {
            return colour / 255;
        }
    }
}
