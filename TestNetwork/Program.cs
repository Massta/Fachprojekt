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
            var inputs = GenerateWeights(10000, 0, 1);
            var data = inputs.Select(i => new { Data = i, Label = i > 0.5 });
            weight = GenerateWeights(1, -1, 1)[0];
            double errorRate = 1;
            while(errorRate > 0.001)
            {
                double errorCount = 0;
                for(int i = 0; i < data.Count(); i++)
                {
                    var currentData = data.ElementAt(i);
                    var neuronResult = Sigmoid((weight*currentData.Data) + bias);
                    
                    if(neuronResult > 0.5 == currentData.Label)
                    {
                        //Guessed correctly
                        Console.WriteLine("yay");
                    }
                    else
                    {
                        //Guessed incorrectly
                        Console.WriteLine("noo");
                        errorCount++;
                    }
                    Console.WriteLine(errorRate); 
                    if(i != 0)
                    {
                        errorRate = errorCount / i;
                    }
                    var error = Math.Pow(neuronResult - (currentData.Label ? 1 : 0), 2);
                    var az = SigmoidDerivative(/*(weight * currentData.Data) + bias*/neuronResult);
                    var ca = 2 * (neuronResult - (currentData.Label ? 1 : 0));
                    var zw = currentData.Data;
                    var anpassung = zw * az * error;
                    weight += 0.1 * anpassung;
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
