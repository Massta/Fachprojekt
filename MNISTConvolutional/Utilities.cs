﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNISTConvolutional
{
    public class Utilities
    {
        const float LEARNING_RATE = 0.01f;

        public static Case[] ReadCSV(string path)
        {
            var lines = File.ReadAllLines(path);
            Case[] cases = new Case[lines.Count()];
            int c = 0;
            foreach (var line in lines)
            {
                var lineData = line.Split(',').Select(l => float.Parse(l)).ToArray();
                var label = lineData[0];
                lineData = lineData.Skip(1).ToArray();
                Tensor tData = new Tensor(28, 28, 1);
                Tensor tOut = new Tensor(10, 1, 1);
                for (int x = 0; x < 28; x++)
                {
                    for (int y = 0; y < 28; y++)
                    {
                        tData.Set(x, y, 0, lineData[x * 28 + y]/255f);
                    }
                }
                tOut.Set((int)label, 0, 0, 1f);
                cases[c] = new Case
                {
                    Data = tData,
                    Out = tOut
                };
                c++;
            }
            return cases;
        }

        const float MOMENTUM = 0.6f;
        const float WEIGHT_DECAY = 0.001f;
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

        public static string PrintVector(double[] vector)
        {
            string m = "";
            for (int j = 0; j < vector.Length; j++)
            {
                m += (vector[j] == 0 ? 0 : 1) + ", ";
            }
            return m;
        }

        public static string PrintMatrix(double[,] matrix)
        {
            string m = "";
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    m += (matrix[j, i] == 0 ? 0 : 1) + ", ";
                }
                m += "\n";
            }
            return m;
        }
        //public static double[] MatrixVectorMultiplication(double[,] matrix, double[] vector)
        //{
        //    int rows = matrix.GetLength(1);
        //    int columns = matrix.GetLength(0);

        //    double[] result = new double[columns];

        //    for (int row = 0; row < rows; row++)
        //    {
        //        double sum = 0;
        //        for (int column = 0; column < columns; column++)
        //        {
        //            sum += matrix[row, column] * vector[column];
        //        }
        //        result[row] = sum;
        //    }
        //    return result;
        //}
        public static double[] MatrixVectorMultiplication(double[,] matrix, double[] vector)
        {
            int columns = matrix.GetLength(0); //4096
            int rows = matrix.GetLength(1); //256

            double[] result = new double[rows];

            for (int row = 0; row < rows; row++)
            {
                double sum = 0;
                for (int column = 0; column < columns; column++)
                {
                    sum += matrix[column, row] * vector[column];
                }
                result[row] = sum;
            }
            return result;
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

        //public static string GenerateNetworkName(Network network)
        //{
        //    string networkName = DateTime.Now.ToString("yyyyMMdd_mm_");
        //    networkName += $"{network._layers.Length} Layers (";
        //    foreach (var layer in network._layers)
        //    {
        //        networkName += $"{layer.NeuronAmount}x{layer.NeuronSize} Neurons, ";
        //    }
        //    networkName = networkName.TrimEnd(',', ' ');
        //    networkName += ")";
        //    return networkName;
        //}

        public static double[][][] To3DArray(double[] inputs, int x, int y, int z)
        {
            var newArray = new double[x][][];
            for (int i = 0; i < x; i++)
            {
                newArray[i] = new double[y][];
                for (int j = 0; j < y; j++)
                {
                    newArray[i][j] = new double[z];
                    for (int k = 0; k < z; k++)
                    {
                        newArray[i][j][k] = inputs[(i * y * z) + (j * z) + k];
                    }
                }
            }

            return newArray;
        }

        //public static Number[] ReadCsv(string path)
        //{
        //    Console.WriteLine(path);
        //    string[] fileInputArray = File.ReadAllLines(path);
        //    List<Number> numberList = new List<Number>();
        //    foreach (var line in fileInputArray)
        //    {
        //        string[] lineData = line.Split(',');
        //        numberList.Add(new Number
        //        {
        //            Label = int.Parse(lineData[0]),
        //            Data = lineData.Skip(1).Select(v => double.Parse(v)).ToArray(),
        //        });
        //    }
        //    return numberList.ToArray();
        //}

        //public static Number[] ReadKaggleCsv(string path)
        //{
        //    Console.WriteLine(path);
        //    string[] fileInputArray = File.ReadAllLines(path);
        //    List<Number> numberList = new List<Number>();
        //    foreach (var line in fileInputArray)
        //    {
        //        string[] lineData = line.Split(';');
        //        numberList.Add(new Number
        //        {
        //            Label = lineData[64 * 64] == "cat" ? 0 : 1,
        //            Data = lineData.Take(64 * 64).Select(v => double.Parse(v)).ToArray(),
        //        });
        //    }
        //    Number[] randomNumbers = numberList.ToArray().OrderBy(x => random.Next()).ToArray();
        //    return randomNumbers;
        //}

        public static Random rnd = new Random();
        public static void StoreImages(string path, double[][][] images)
        {
            for (int i = 0; i < images.Length; i++)
            {
                string name = images[i].Any(v => v.Any(x => x == 200)) ? "1" : "0";
                StoreImage(Path.Combine(path, i + "_" + name + ".bmp"), images[i]);
            }
        }
        public static void StoreImage(string path, double[][] image)
        {
            Bitmap bitmap = new Bitmap(image.Length, image.Length);
            for (int i = 0; i < image.Length; i++)
            {
                for (int j = 0; j < image[i].Length; j++)
                {
                    Color c = Color.FromArgb((int)image[i][j], (int)image[i][j], (int)image[i][j]);
                    bitmap.SetPixel(i, j, c);
                }
            }
            bitmap.Save(path);
        }
        //public static Number[] GetImages(int amount)
        //{
        //    double[][][] images = new double[amount][][];
        //    double[] labels = new double[amount];
        //    for (int i = 0; i < amount; i++)
        //    {
        //        images[i] = GetDistortedImage(0, 50);
        //        if (GetRandomNumber(0, 2) > 0)
        //        {
        //            labels[i] = 1.0;
        //            //Create rectangle
        //            int wI = GetRandomNumber(0, 5);
        //            int hI = GetRandomNumber(0, 5);
        //            //top row
        //            images[i][wI][hI] = 255;
        //            images[i][wI + 1][hI] = 255;
        //            images[i][wI + 2][hI] = 255;
        //            images[i][wI + 3][hI] = 255;
        //            //left bar
        //            images[i][wI][hI + 1] = 255;
        //            images[i][wI][hI + 2] = 255;
        //            //right bar
        //            images[i][wI + 3][hI + 1] = 255;
        //            images[i][wI + 3][hI + 2] = 255;
        //            //bottom row
        //            images[i][wI][hI + 3] = 255;
        //            images[i][wI + 1][hI + 3] = 255;
        //            images[i][wI + 2][hI + 3] = 255;
        //            images[i][wI + 3][hI + 3] = 255;
        //        }
        //        else
        //        {
        //            labels[i] = 0.0;
        //        }
        //    }
        //    int c = -1;
        //    return images.Select(i => new Number
        //    {
        //        Data = i.SelectMany(x => x).ToArray(),
        //        Label = labels[++c]
        //    }).ToArray();
        //}

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

        public static float GetRandomFloat(float min, float max)
        {
            return min + ((float)random.NextDouble() * (max - min));
        }

        public static float UpdateWeight(float w, float gradient, float oldGradient, float multp = 1)
        {
            float m = (gradient + oldGradient * MOMENTUM);
            w -= LEARNING_RATE * m * multp +
                 LEARNING_RATE * WEIGHT_DECAY * w;
            return w;
        }

        public static float UpdateGradient(float gradient, float oldGradient)
        {
            return (gradient + oldGradient * MOMENTUM);
        }
    }
}
