using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNISTConvolutional
{
    class Program
    {
        static void Main(string[] args)
        {
            Case[] cases = Utilities.ReadCSV(Path.Combine(@"C:\Users\Julius Jacobsohn\OneDrive\Dokumente\MNIST", "mnist_train_small.csv"));

            Layer[] layers = new Layer[4];

            ConvolutionalLayer layer1 = new ConvolutionalLayer(1, 5, 8, cases[0].Data.Size);
            ReluLayer layer2 = new ReluLayer(layer1.Out.Size);
            PoolLayer layer3 = new PoolLayer(2, 2, layer2.Out.Size);
            FullyConnectedLayer layer4 = new FullyConnectedLayer(10, layer3.Out.Size);

            layers[0] = layer1;
            layers[1] = layer2;
            layers[2] = layer3;
            layers[3] = layer4;

            float amse = 0;
            int ic = 0;

            for (int ep = 0; ep < 100000;)
            {
                foreach (Case c in cases)
                {
                    float xError = Train(layers, c.Data, c.Out);
                    amse += xError;

                    ep++;
                    ic++;
                    //Console.WriteLine("Case " + ep + " err=" + amse / ic);
                }
            }
        }

        private static float Train(Layer[] layers, Tensor data, Tensor expected)
        {
            layers[0].Activate(data);
            for (int i = 1; i < layers.Count(); i++)
            {
                layers[i].Activate(layers[i - 1].Out);
            }

            int label = -1;
            float guess = -1;
            int biggestIndex = -1;
            for(int i = 0; i < 10; i++)
            {
                if(expected.Data[i] > 0)
                {
                    label = i;
                }
                if(layers.Last().Out.Data[i] > guess)
                {
                    guess = layers.Last().Out.Data[i];
                    biggestIndex = i;
                }
            }
            if(label == biggestIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine($"Gegeben: {label}, Geraten: {biggestIndex} ({guess.ToString("n2")})");

            Tensor topGradients = layers.Last().Out.Substract(expected);

            layers.Last().CalculateGradients(topGradients);
            for (int i = layers.Length - 2; i >= 0; i--)
            {
                layers[i].CalculateGradients(layers[i + 1].GradsIn);
            }
            for (int i = 0; i < layers.Count(); i++)
            {
                layers[i].FixWeights();
            }
            float error = 0;
            for (int i = 0; i < topGradients.FullSize; i++)
            {
                float f = expected.Data[i];
                if (f > 0.5)
                {
                    error += Math.Abs(topGradients.Data[i]);
                }
            }
            return error * 100;
        }
    }
}
