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
    class Program
    {
        const string PATH_TEST_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test_Small_Grayscale";
        const string PATH_TRAIN_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale";
        const string PATH_TRAIN_SMALL_GRAY_FILTER1 = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale_Filter1";

        public const int FILTER1_AMOUNT = 32;
        public const int FILTER1_SIZE = 2;
        public const int FILTER1_STRIDE = 2;
        public static double[][,] Filters1;
        public static double[][,] Filters1Result;

        static Random random = new Random();

        static void Main(string[] args)
        {
            //Load image data
            string csvPath = Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train.csv");
            int[][,] imageData = ReadCsv(csvPath);

            //Load filters
            Filters1 = new double[FILTER1_AMOUNT][,];
            Filters1Result = new double[FILTER1_AMOUNT][,];
            for (int i = 0; i < FILTER1_AMOUNT; i++)
            {
                Filters1[i] = GenerateWeightMatrix(FILTER1_SIZE, -0.5, 0.5);
            }

            for (int i = 0; i < imageData.Count(); i++)
            {
                var image = imageData[i];

                //Use filters 1
                for (int f = 0; f < FILTER1_AMOUNT; f++)
                {
                    var filter = Filters1[f];
                    Filters1Result[f] = new double[FILTER1_AMOUNT, FILTER1_AMOUNT];
                    for (int x = 0; x < image.GetLength(0); x += FILTER1_STRIDE)
                    {
                        for (int y = 0; y < image.GetLength(0); y += FILTER1_STRIDE)
                        {
                            Filters1Result[f][x / 2, y / 2] = image[x, y] * filter[0, 0]
                                + image[x + 1, y] * filter[1, 0]
                                + image[x, y + 1] * filter[0, 1]
                                + image[x + 1, y + 1] * filter[1, 1];
                        }
                    }
                    PrintResult(Filters1Result[f], f);
                }
            }
        }

        private static void PrintResult(double[,] image, int index)
        {
            Bitmap grayscale = new Bitmap(image.GetLength(0), image.GetLength(0));
            int x, y;


            // Loop through the images pixels to reset color.
            for (x = 0; x < image.GetLength(0); x++)
            {
                for (y = 0; y < image.GetLength(0); y++)
                {
                    int grayScale = Clamp((int)(image[x, y] * 255),0,255);
                    Color nc = Color.FromArgb(0, grayScale, grayScale, grayScale);
                    grayscale.SetPixel(x, y, nc);
                }
            }
            grayscale.Save(Path.Combine(PATH_TRAIN_SMALL_GRAY_FILTER1, $"{index}.jpg"), ImageFormat.Jpeg);
        }

        private static int[][,] ReadCsv(string csvPath)
        {
            var lines = File.ReadAllLines(csvPath);
            var takeLines = lines.OrderBy(l => l.Substring(2048, 3)).Take(100).ToArray();
            int[][,] data = new int[takeLines.Count()][,];
            for (int i = 0; i < data.Count(); i++)
            {
                var line = takeLines[i];
                var lineData = line.Split(';').Take(4096);
                var lineVector = lineData.Select(d => int.Parse(d)).ToArray();
                data[i] = ConvertMatrix(lineVector, 64, 64);
            }
            return data;
        }
        private static int[,] ConvertMatrix(int[] flat, int m, int n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            int[,] ret = new int[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(int));
            return ret;
        }
        private static double[,] GenerateWeightMatrix(int size, double min, double max)
        {
            double[,] weights = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                var tempWeights = GenerateWeightArray(size, min, max);
                for (int j = 0; j < tempWeights.Count(); j++)
                {
                    weights[i, j] = tempWeights[j];
                }
            }
            return weights;
        }
        private static double[] GenerateWeightArray(int size, double min, double max)
        {
            return GenerateWeights(size, min, max).ToArray();
        }
        private static IEnumerable<double> GenerateWeights(int amount, double min, double max)
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
        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
