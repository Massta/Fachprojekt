using System;
using System.Collections.Generic;
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

        static Random random = new Random();

        static void Main(string[] args)
        {
            //Load image data
            string csvPath = Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train.csv");
            int[][,] imageData = ReadCsv(csvPath);

            //Load filters
            Filters1 = new double[FILTER1_AMOUNT][,];
            for (int i = 0; i < FILTER1_AMOUNT; i++)
            {
                Filters1[i] = GenerateWeightMatrix(FILTER1_SIZE, -0.5, 0.5);
            }

            for (int i = 0; i < imageData.Count(); i++)
            {
                var image = imageData[i];
            }
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
            double[,] weights = new double[size,size];
            for (int i = 0; i < size; i++)
            {
                var tempWeights = GenerateWeightArray(size, min, max);
                for(int j = 0; j < tempWeights.Count(); j++)
                {
                    weights[i,j] = tempWeights[j];
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
    }
}
