using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektLibrary
{
    class Program
    {
        public const int LAYER_SIZE_HIDDEN = 50;
        public const int LAYER_SIZE_OUTPUT = 2;
        public const double LEARN_RATE = 0.1;
        public const double MAXIMUM_ERROR_PERCENTAGE = 0.004;

        public const int INPUT_DATA_WIDTH = 64;
        static void Main(string[] args)
        {
            //ConvolutionalLayer c1 = new ConvolutionalLayer(4, 64, 1);
            //ConvolutionalLayer c2 = new ConvolutionalLayer(4, 16, 4);
            //ConvolutionalLayer c3 = new ConvolutionalLayer(4, 4, 16);
            FullyConnectedLayer hiddenLayer = new FullyConnectedLayer(LAYER_SIZE_HIDDEN, INPUT_DATA_WIDTH, false, LEARN_RATE);
            FullyConnectedLayer outputLayer = new FullyConnectedLayer(LAYER_SIZE_OUTPUT, LAYER_SIZE_HIDDEN, false, LEARN_RATE);
            Network network = new Network(hiddenLayer, outputLayer);
            //Number[] trainingNumbers = Utilities.ReadCsv(@"D:\Dropbox\Informatik TU Dortmund\Fachprojekte\Data Mining\MNIST\mnist_train.csv");
            //Number[] trainingNumbers = Utilities.ReadKaggleCsv(@"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale\Train.csv");
            Number[] trainingNumbers = Utilities.GetImages(10000);
            Train(network, trainingNumbers);

            string netName = Utilities.GenerateNetworkName(network);

            //Utilities.StoreNetwork(network, $@"C:\Users\Julius Jacobsohn\Documents\Visual Studio 2017\Projects\MNIST_Convolutional\MNIST_Convolutional\resource\{netName}.json");
            //TODO

            //Number[] testNumbers = Utilities.ReadCsv(@"D:\Dropbox\Informatik TU Dortmund\Fachprojekte\Data Mining\MNIST\mnist_test.csv");
            //Test(network, testNumbers);
        }

        public static void Train(Network network, Number[] numbers)
        {
            double errorCount = 0;
            double errorPercentage = 1;
            int epoch = 0;
            while (errorPercentage > MAXIMUM_ERROR_PERCENTAGE)
            {
                int currentNumber = 0;
                double totalNumbers = numbers.Length;
                foreach (var number in numbers)
                {
                    var outputs = network.GetOutputs(number.NormalizedData);
                    double biggestIndex = -1;
                    double biggestNumber = 0;
                    for (int i = 0; i < outputs.Length; i++)
                    {
                        if (outputs[i] > biggestNumber)
                        {
                            biggestNumber = outputs[i];
                            biggestIndex = i;
                        }
                    }
                    number.Guess = biggestIndex;
                    if (number.Label != number.Guess)
                    {
                        errorCount++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (currentNumber != 0)
                        {
                            errorPercentage = errorCount / currentNumber;
                        }
                    }
                    if (currentNumber % 100 == 0)
                    {
                        double errorPercentageLast100 = 0;
                        if (currentNumber > 100 && currentNumber < numbers.Length - 100)
                        {
                            errorPercentageLast100 = numbers.Skip(currentNumber - 100).Take(100).Count(n => n.Label != n.Guess);
                        }
                        Console.WriteLine($"[{epoch}:{currentNumber}/{totalNumbers}] Gegeben: {number.Label} Geraten: {biggestIndex} ({biggestNumber}) Fehlerprozentsatz: {Math.Round(errorPercentage * 100)}% Fehlerprozentsatz (letzte 100): {Math.Round(errorPercentageLast100)}%");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    network.AdjustWeights(number.Label, biggestIndex, outputs);
                    currentNumber++;
                }
                errorCount = 0;
                epoch++;
            }
        }

        public static void Test(Network network, Number[] numbers)
        {
            double errorCount = 0;
            double errorPercentage = 1;
            int currentNumber = 0;
            double totalNumbers = numbers.Length;
            foreach (var number in numbers)
            {
                var outputs = network.GetOutputs(number.NormalizedData);
                double biggestIndex = -1;
                double biggestNumber = 0;
                for (int i = 0; i < outputs.Length; i++)
                {
                    if (outputs[i] > biggestNumber)
                    {
                        biggestNumber = outputs[i];
                        biggestIndex = i;
                    }
                }
                number.Guess = biggestIndex;
                if (number.Label != number.Guess)
                {
                    errorCount++;
                    if (currentNumber != 0)
                    {
                        errorPercentage = errorCount / currentNumber;
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                double errorPercentageLast100 = 0;
                if (currentNumber > 100 && currentNumber < numbers.Length - 100)
                {
                    errorPercentageLast100 = numbers.Skip(currentNumber - 100).Take(100).Count(n => n.Label != n.Guess);
                }
                if (currentNumber % 100 == 0)
                {
                    Console.WriteLine($"[Test:{currentNumber}/{totalNumbers}] Gegeben: {number.Label} Geraten: {biggestIndex} ({biggestNumber}) Fehlerprozentsatz: {Math.Round(errorPercentage * 100)}% Fehlerprozentsatz (letzte 100): {Math.Round(errorPercentageLast100)}%");
                }
                Console.ForegroundColor = ConsoleColor.White;
                currentNumber++;
            }
        }
    }
}
