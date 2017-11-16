using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektAufgabe2
{
    class Program
    {
        const double LEARN_RATE = 0.01;
        const int AMOUNT_DATA = 784;
        const int AMOUNT_HIDDEN = 300;
        const int AMOUNT_OUTPUT = 10;
        static Random random = new Random();
        static void Main(string[] args)
        {
            train();
        }
        class sigmoid
        {
            public static double output(double x)
            {
                return 1.0 / (1.0 + Math.Exp(-7 * x));
            }

            public static double derivative(double x)
            {
                return x * (1 - x);
            }
        }
        class relu
        {
            public static double output(double x)
            {
                return Math.Max(0, x);
            }

            public static double derivative(double x)
            {
                return x < 0 ? 0 : 1;
            }
        }
        class Neuron
        {
            public double[] inputs;
            public double[] weights;
            public double error;

            private double biasWeight;

            public Neuron(int size)
            {
                inputs = new double[size];
                weights = new double[size];
            }

            public double output
            {
                get
                {
                    double sum = 0;
                    for (int i = 0; i < inputs.Count(); i++)
                    {
                        sum += weights[i] * inputs[i];
                    }
                    //Console.WriteLine("Sum: " + sum + " /255=" + sum / 255 + " sig=" + sigmoid.output(sum / 255));
                    sum /= 255;
                    sum += biasWeight;
                    return sigmoid.output(sum);
                    //(weights[0] * inputs[0] + weights[1] * inputs[1] + biasWeight);
                }
            }

            public void randomizeWeights()
            {
                //for (int i = 0; i < inputs.Length; i++)
                //{
                //    weights[i] = r.NextDouble() * 0.05;//r.NextDouble(); //TODO
                //}
                weights = GenerateWeights(inputs.Length).ToArray();
                biasWeight = 0; // r.NextDouble(); //TODO
            }

            public void adjustWeights()
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    weights[i] += LEARN_RATE * error * inputs[i];
                }
                biasWeight += LEARN_RATE * error;
            }
        }
        private static IEnumerable<double> GenerateWeights(int size)
        {
            object syncLock = new object();
            for (int i = 0; i < size; i++)
            {
                lock (syncLock)
                { // synchronize
                  //yield return random.NextDouble() * (random.NextDouble() > 0.5 ? -1 : 1);
                    yield return (random.NextDouble() * 0.1) - 0.05;
                }
            }
        }
        private static void train()
        {
            var trainNumbers = ReadNumbers(@"C:\Users\Julius Jacobsohn\Dropbox\Informatik TU Dortmund\Fachprojekte\Data Mining\Aufgabe 1\mnist_train_small.csv");

            List<Neuron> hiddenNeurons = new List<Neuron>();
            for (int i = 0; i < AMOUNT_HIDDEN; i++)
            {
                // creating the neurons
                Neuron newNeuron = new Neuron(AMOUNT_DATA);
                newNeuron.randomizeWeights();
                // random weights
                hiddenNeurons.Add(newNeuron);
            }

            List<Neuron> outputNeurons = new List<Neuron>();
            for (int i = 0; i < AMOUNT_OUTPUT; i++)
            {
                // creating the neurons
                Neuron newNeuron = new Neuron(AMOUNT_HIDDEN);
                newNeuron.randomizeWeights();
                // random weights
                outputNeurons.Add(newNeuron);
            }

            int epoch = 0;

            int[] oldNumbers = new int[trainNumbers.Count()];
            int trainNumberCount = trainNumbers.Count();
            Retry:
            epoch++;
            for (int numberIndex = 0; numberIndex < trainNumbers.Count(); numberIndex++)  // very important, do NOT train for only one example
            {
                var currentNumber = trainNumbers.ElementAt(numberIndex);
                // 1) forward propagation (calculates output)
                for (int i = 0; i < hiddenNeurons.Count; i++)
                {
                    var currentNeuron = hiddenNeurons.ElementAt(i);
                    double[] newInputs = currentNumber.Data;
                    currentNeuron.inputs = newInputs;
                }

                int biggestOutputIndex = 0;
                double biggestOutputValue = 0;
                for (int i = 0; i < outputNeurons.Count; i++)
                {
                    var currentNeuron = outputNeurons.ElementAt(i);
                    var hiddenOutput = hiddenNeurons.Select(n => n.output).ToArray();
                    currentNeuron.inputs = hiddenOutput;
                    if (currentNeuron.output > biggestOutputValue)
                    {
                        biggestOutputValue = currentNeuron.output;
                        biggestOutputIndex = i;
                    }
                }
                if (oldNumbers[numberIndex] != biggestOutputIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if(currentNumber.Label == biggestOutputIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine($"({numberIndex}/{trainNumberCount}) Gesuchte Zahl: {currentNumber.Label}, Geratene Zahl: {biggestOutputIndex}");
                Console.ForegroundColor = ConsoleColor.White;
                oldNumbers[numberIndex] = biggestOutputIndex;

                // 2) back propagation (adjusts weights)

                // adjusts the weight of the output neuron, based on its error
                for (int i = 0; i < outputNeurons.Count; i++)
                {
                    var currentNeuron = outputNeurons.ElementAt(i);
                    int y = currentNumber.Label == i ? 1 : 0;
                    currentNeuron.error = sigmoid.derivative(currentNeuron.output) * (y - (currentNeuron.output > 0.5 ? 1 : 0));
                    currentNeuron.adjustWeights();
                }

                // then adjusts the hidden neurons' weights, based on their errors
                for (int i = 0; i < hiddenNeurons.Count; i++)
                {
                    var currentNeuron = hiddenNeurons.ElementAt(i);
                    currentNeuron.error = sigmoid.derivative(currentNeuron.output) * outputNeurons.Sum(n => n.error * n.weights[i]);
                    currentNeuron.adjustWeights();
                }
            }
            Console.WriteLine("Epoche " + epoch);
            if (epoch < 10000)
                goto Retry;

            Console.ReadLine();
        }
        public static IEnumerable<Number> ReadNumbers(string path)
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
                    Guess = -1,
                });
            }
            return numberList;
        }
    }
}
