using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FachprojektLibrary
{
    public class Utilities
    {
        static Random random = new Random();
        public static double Sigmoid(double x, double delta = 1)
        {
            return 1.0 / (1.0 + Math.Exp(-delta * x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }
        public static double ReLu(double x)
        {
            return Math.Max(0, x);
        }

        public static double ReLuDerivative(double x)
        {
            return x < 0 ? 0 : 1;
        }
        public static double[] GenerateWeights(int size, double min, double max)
        {
            object syncLock = new object();
            double[] weights = new double[size];
            for (int i = 0; i < size; i++)
            {
                lock (syncLock)
                {
                    weights[i] = (random.NextDouble() * (max - min)) + min;
                }
            }
            return weights;
        }

        public static double[,] GenerateFilter(int filterSize, double min, double max)
        {
            object syncLock = new object();
            double[,] filter = new double[filterSize, filterSize];
            for (int i = 0; i < filterSize; i++)
            {
                for (int j = 0; j < filterSize; j++)
                {
                    lock (syncLock)
                    {
                        filter[i, j] = (random.NextDouble() * (max - min)) + min;
                    }
                }
            }
            return filter;
        }

        public static string GenerateNetworkName(Network network)
        {
            string networkName = DateTime.Now.ToString("yyyyMMdd_mm_");
            networkName += $"{network._layers.Length} Layers (";
            foreach (var layer in network._layers)
            {
                networkName += $"{layer.NeuronAmount}x{layer.NeuronSize} Neurons, ";
            }
            networkName = networkName.TrimEnd(',', ' ');
            networkName += ")";
            return networkName;
        }

        public static void StoreNetwork(Network network, string path)
        {
            //var json = new JavaScriptSerializer().Serialize(network);
            //var test = JsonConvert.SerializeObject(network);
            //File.WriteAllText(path, test);
        }

        public static Number[] ReadCsv(string path)
        {
            Console.WriteLine(path);
            string[] fileInputArray = File.ReadAllLines(path);
            List<Number> numberList = new List<Number>();
            foreach (var line in fileInputArray)
            {
                string[] lineData = line.Split(',');
                numberList.Add(new Number
                {
                    Label = int.Parse(lineData[0]),
                    Data = lineData.Skip(1).Select(v => double.Parse(v)).ToArray(),
                });
            }
            return numberList.ToArray();
        }

        public static Number[] ReadKaggleCsv(string path)
        {
            Console.WriteLine(path);
            string[] fileInputArray = File.ReadAllLines(path);
            List<Number> numberList = new List<Number>();
            foreach (var line in fileInputArray)
            {
                string[] lineData = line.Split(';');
                numberList.Add(new Number
                {
                    Label = lineData[64 * 64] == "cat" ? 0 : 1,
                    Data = lineData.Take(64 * 64).Select(v => double.Parse(v)).ToArray(),
                });
            }
            Number[] randomNumbers = numberList.ToArray().OrderBy(x => random.Next()).ToArray();
            return randomNumbers;
        }
    }
}