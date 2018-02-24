using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNetwork
{
    class Program
    {
        static Random random = new Random();
        static double weight;
        static double bias;
        static void Main(string[] args)
        {
            var inputs = GenerateWeights(10000, -1, 1);
            var data = inputs.Select(i => new { Data = i, Label = i > 0 });
            weight = GenerateWeights(1, -1, 1)[0];
            double errorRate = 1;
            while (errorRate > 0.001)
            {
                double errorCount = 0;
                for (int i = 0; i < data.Count(); i++)
                {
                    var currentData = data.ElementAt(i);
                    var neuronResult = Sigmoid((weight * currentData.Data) + bias);
                    var wantedResult = (currentData.Label ? 1 : 0);


                    if (neuronResult > 0.5 == currentData.Label)
                    {
                        //Guessed correctly
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        //Guessed incorrectly
                        Console.ForegroundColor = ConsoleColor.Red;
                        errorCount++;
                    }
                    errorRate = errorCount / (i+1);
                    Console.WriteLine($@"[{i}/{data.Count()}] Guess: {neuronResult} Wanted: {wantedResult} Errorrate: {errorRate}");
                    var error = Math.Pow(neuronResult - wantedResult, 2);
                    var az = SigmoidDerivative(neuronResult);
                    var ca = (wantedResult-neuronResult);
                    var zw = neuronResult;
                    var anpassung = zw * az * ca;
                    weight -= 0.01 * anpassung;
                    //bias -= 0.01 * az * zw;
                }
            }
        }
        public static double Sigmoid(double x, double delta = 1)
        {
            return 1.0 / (1.0 + Math.Exp(-delta * x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
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
    }
}
