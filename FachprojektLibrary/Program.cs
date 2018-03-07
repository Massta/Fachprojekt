using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FachprojektLibrary
{
    class Program
    {
        public const int LAYER_SIZE_HIDDEN = 243;
        public const int LAYER_SIZE_OUTPUT = 10;
        public const double LEARN_RATE = 0.01;
        public const double MAXIMUM_ERROR_PERCENTAGE = 0.05;
        public const double BATCH_SIZE = 1;

        public const int INPUT_DATA_WIDTH = 243;
        static void Main(string[] args)
        {
            //Number[] trainingNumbers = Utilities.ReadKaggleCsv(@"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale\Train.csv");
            ConvolutionalLayer c1 = new ConvolutionalLayer(3, 32, 1, LEARN_RATE);
            ConvolutionalLayer c2 = new ConvolutionalLayer(3, 16, 3, LEARN_RATE);
            ConvolutionalLayer c3 = new ConvolutionalLayer(3, 8, 9, LEARN_RATE);
            ConvolutionalLayer c4 = new ConvolutionalLayer(3, 4, 27, LEARN_RATE);
            ConvolutionalLayer c5 = new ConvolutionalLayer(3, 2, 81, LEARN_RATE);
            FullyConnectedLayer hiddenLayer = new FullyConnectedLayer(LAYER_SIZE_HIDDEN, INPUT_DATA_WIDTH, false, LEARN_RATE);
            FullyConnectedLayer outputLayer = new FullyConnectedLayer(LAYER_SIZE_OUTPUT, LAYER_SIZE_HIDDEN, false, LEARN_RATE);
            Network network = new Network(c1, c2, c3, c4, c5, hiddenLayer, outputLayer);

            Number[] trainingNumbers = Utilities.ReadCsv(@"C:\Users\Julius Jacobsohn\OneDrive\Dokumente\MNIST\mnist_train_small_appended.csv")
                .Take(500).ToArray();
            //var batches = GetBatches(trainingNumbers, (int)BATCH_SIZE);
            //int bCounter = 0;
            //foreach(var batch in batches)
            //{
            //    Train(network, batch);
            //    Console.WriteLine("Finished training batch "+bCounter);
            //    bCounter++;
            //    Thread.Sleep(500);
            //}
            Train(network, trainingNumbers);

            Thread.Sleep(1000);

            Number[] testNumbers = Utilities.ReadCsv(@"C:\Users\Julius Jacobsohn\OneDrive\Dokumente\MNIST\mnist_train_small_appended.csv");
            Test(network, testNumbers);

            //Small Rectangles
            //ConvolutionalLayer c1 = new ConvolutionalLayer(4, 8, 1, LEARN_RATE);
            //ConvolutionalLayer c2 = new ConvolutionalLayer(4, 4, 4, LEARN_RATE);
            //ConvolutionalLayer c3 = new ConvolutionalLayer(4, 2, 16, LEARN_RATE);
            //FullyConnectedLayer hiddenLayer = new FullyConnectedLayer(LAYER_SIZE_HIDDEN, INPUT_DATA_WIDTH, false, LEARN_RATE);
            //FullyConnectedLayer outputLayer = new FullyConnectedLayer(LAYER_SIZE_OUTPUT, LAYER_SIZE_HIDDEN, false, LEARN_RATE);
            //Network network = new Network(c1, c2, c3, hiddenLayer, outputLayer);
            //Number[] trainingNumbers = Utilities.GetImages(10000);
            //Console.WriteLine(trainingNumbers.Count(n => n.Label == 1.0));
            //Train(network, trainingNumbers);
            //Thread.Sleep(1000);
            //Number[] testNumbers = Utilities.GetImages(100000);
            //Test(network, testNumbers);

            //Small Rectangles - 8 Filters
            //ConvolutionalLayer c1 = new ConvolutionalLayer(8, 8, 1, LEARN_RATE);
            //ConvolutionalLayer c2 = new ConvolutionalLayer(8, 4, 8, LEARN_RATE);
            //ConvolutionalLayer c3 = new ConvolutionalLayer(8, 2, 64, LEARN_RATE);
            //FullyConnectedLayer hiddenLayer = new FullyConnectedLayer(LAYER_SIZE_HIDDEN, INPUT_DATA_WIDTH, false, LEARN_RATE);
            //FullyConnectedLayer outputLayer = new FullyConnectedLayer(LAYER_SIZE_OUTPUT, LAYER_SIZE_HIDDEN, false, LEARN_RATE);
            //Network network = new Network(c1, c2, c3, hiddenLayer, outputLayer);
            //Number[] trainingNumbers = Utilities.GetImages(10000);
            //Console.WriteLine(trainingNumbers.Count(n => n.Label == 1.0));
            //Train(network, trainingNumbers);
            //Thread.Sleep(1000);
            //Number[] testNumbers = Utilities.GetImages(10000);
            //Test(network, testNumbers);
        }

        private static Number[][] GetBatches(Number[] trainingNumbers, int batchSize)
        {
            Number[][] batches = new Number[trainingNumbers.Count() / batchSize][];
            for (int i = 0; i < batches.Length; i++)
            {
                batches[i] = trainingNumbers.Skip(i * batchSize).Take(batchSize).ToArray();
            }
            return batches;
        }

        public static void Train(Network network, Number[] numbers)
        {
            double errorCount = 0;
            double errorPercentage = 1;
            int epoch = 0;
            while (errorPercentage > MAXIMUM_ERROR_PERCENTAGE)
            {
                double[] batchError = new double[LAYER_SIZE_OUTPUT];
                int currentNumber = 1;
                double totalNumbers = numbers.Length;
                foreach (var number in numbers)
                {
                    var outputs = network.GetOutputs(number.Data);
                    int biggestIndex = -1;
                    double biggestNumber = -1;
                    for (int i = 0; i < outputs.Length; i++)
                    {
                        if (outputs[i] > biggestNumber)
                        {
                            biggestNumber = outputs[i];
                            biggestIndex = i;
                        }
                    }
                    number.Guess = outputs[biggestIndex];
                    number.LabelGuess = biggestIndex;
                    double[] labelArray = new double[outputs.Length];
                    for (int i = 0; i < outputs.Length; i++)
                    {
                        if (i == number.Label)
                        {
                            labelArray[i] = 1;
                        }
                        else
                        {
                            labelArray[i] = 0;
                        }
                    }
                    if (number.Label != number.LabelGuess)
                    {
                        errorCount++;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    errorPercentage = errorCount / (currentNumber);
                    if (currentNumber % 100 == 0)
                    {
                        double errorPercentageLast100 = 0;
                        if (currentNumber > 100 && currentNumber < numbers.Length - 100)
                        {
                            errorPercentageLast100 = numbers.Skip(currentNumber - 100).Take(100).Count(n => n.Label != n.LabelGuess);
                        }
                        Console.WriteLine($"[{epoch}:{currentNumber}/{totalNumbers}] Gegeben: {number.Label} Geraten: {number.LabelGuess} ({number.Guess.ToString("n2")}) Fehlerprozentsatz: {Math.Round(errorPercentage * 100)}% Fehlerprozentsatz (letzte 100): {Math.Round(errorPercentageLast100)}%");
                    }
                    //Console.WriteLine($"[{epoch}:{currentNumber}/{totalNumbers}] Gegeben: {number.Label} Geraten: {number.LabelGuess} ({number.Guess.ToString("n2")}) Fehlerprozentsatz: {Math.Round(errorPercentage * 100)}%");
                    Console.ForegroundColor = ConsoleColor.White;
                    double[] topError = new double[LAYER_SIZE_OUTPUT];
                    for (int i = 0; i < LAYER_SIZE_OUTPUT; i++)
                    {
                        topError[i] = labelArray[i] - outputs[i];
                        batchError[i] += topError[i];
                    }
                    if(currentNumber%BATCH_SIZE == 0)
                    {
                        network.AdjustWeights(batchError);
                    }
                    batchError = new double[LAYER_SIZE_OUTPUT];
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
                var outputs = network.GetOutputs(number.Data);
                int biggestIndex = -1;
                double biggestNumber = -1;
                for (int i = 0; i < outputs.Length; i++)
                {
                    if (outputs[i] > biggestNumber)
                    {
                        biggestNumber = outputs[i];
                        biggestIndex = i;
                    }
                }
                number.Guess = outputs[biggestIndex];
                number.LabelGuess = biggestIndex;
                if (number.Label != number.LabelGuess)
                {
                    errorCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                errorPercentage = errorCount / (currentNumber + 1);
                double errorPercentageLast100 = 0;
                if (currentNumber > 100 && currentNumber < numbers.Length - 100)
                {
                    errorPercentageLast100 = numbers.Skip(currentNumber - 100).Take(100).Count(n => n.Label != n.Guess);
                }
                if (currentNumber % 100 == 0)
                {
                    if (currentNumber > 100 && currentNumber < numbers.Length - 100)
                    {
                        errorPercentageLast100 = numbers.Skip(currentNumber - 100).Take(100).Count(n => n.Label != n.LabelGuess);
                    }
                    Console.WriteLine($"[Test:{currentNumber}/{totalNumbers}] Gegeben: {number.Label} Geraten: {number.LabelGuess} ({number.Guess.ToString("n2")}) Fehlerprozentsatz: {Math.Round(errorPercentage * 100)}% Fehlerprozentsatz (letzte 100): {Math.Round(errorPercentageLast100)}%");
                }
                Console.ForegroundColor = ConsoleColor.White;
                currentNumber++;
            }
        }
    }
}
