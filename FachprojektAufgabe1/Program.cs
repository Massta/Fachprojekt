using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektAufgabe1
{
    class Program
    {
        const double LEARN_RATE = 1;
        const double THRESHHOLD = 0;
        const double ERROR_PERCENTAGE_WANTED = 0.02;

        static double[] Weights;
        static double ErrorPercentage = 1;
        static void Main(string[] args)
        {
            var trainNumbers = ReadNumbers(@"C:\Users\Julius Jacobsohn\Dropbox\Informatik TU Dortmund\Fachprojekte\Data Mining\Aufgabe 1\mnist_train_small.csv");
            Console.WriteLine("Parsed data");
            Console.WriteLine("Please enter number 1:");
            int n1 = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter number 2:");
            int n2 = int.Parse(Console.ReadLine());

            Train(trainNumbers.Where(v => v.Label == n1 || v.Label == n2), n1, n2);

            Console.WriteLine("Testing...");
            var testNumbers = ReadNumbers(@"C:\Users\Julius Jacobsohn\Dropbox\Informatik TU Dortmund\Fachprojekte\Data Mining\Aufgabe 1\mnist_test.csv")
                .Where(v => v.Label == n1 || v.Label == n2);
            foreach(var number in testNumbers)
            {
                int guess = CalculateOutput(number);
                number.Guess = guess == 1 ? n1 : n2;
            }
            var correctlyGuessed = testNumbers.Where(v => v.Label == v.Guess);
            var wronglyGuessed = testNumbers.Where(v => v.Label != v.Guess);
            Console.WriteLine($"Test finished. Correctly guessed: {correctlyGuessed.Count()}/{testNumbers.Count()}, Wrongly guessed: {wronglyGuessed.Count()}/{testNumbers.Count()}");
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
                    Data = lineData.Skip(1).Select(v => int.Parse(v)).ToArray(),
                    Guess = -1,
                });
            }
            return numberList;
        }

        private static IEnumerable<double> GenerateWeights(int size)
        {
            Random random = new Random();
            object syncLock = new object();
            for (int i = 0; i < size; i++)
            {
                lock (syncLock)
                { // synchronize
                    //yield return random.NextDouble() * (random.NextDouble() > 0.5 ? -1 : 1);
                    yield return random.NextDouble();
                }
            }
        }

        private static void Train(IEnumerable<Number> numbers, int n1, int n2)
        {
            Console.WriteLine("Starting training...");
            Weights = GenerateWeights(numbers.FirstOrDefault().Data.Count()).ToArray();
            int iterations = 0;
            while (ErrorPercentage > ERROR_PERCENTAGE_WANTED)
            {
                for (int i = 0; i < numbers.Count(); i++)
                {
                    Number number = numbers.ElementAt(i);
                    int guess = CalculateOutput(number);
                    number.Guess = guess == 1 ? n1 : n2;
                    var wOld = Weights.Clone() as double[];
                    for(int j = 0; j < number.Data.Count(); j++)
                    {
                        //w_new    = w_old      + LEARN_RATE * x_i            * (y_i                          - f(x_i)
                        int x = number.Data[j];
                        Weights[j] = Weights[j] + LEARN_RATE * number.Data[j] * ((number.Label == n1 ? 1 : 0) - guess);
                        //if(wOld[j] != Weights[j])
                        //{
                        //    Console.WriteLine($"New weight diff. New: {Weights[j]}, Old: {wOld[j]}");
                        //}
                    }
                }
                int numberCount = numbers.Count();
                double wronglyGuessedNumbers = numbers.Count(v => v.Label != v.Guess);
                ErrorPercentage = wronglyGuessedNumbers / (double)numbers.Count();
                iterations++;
            }

            Console.WriteLine("Finished. Numbers:");
            Console.WriteLine("Iterations: "+iterations);
            Console.WriteLine($"Errorpercentage: {ErrorPercentage}");
        }

        private static int CalculateOutput(Number number)
        {
            double sum = 0;
            for (int i = 0; i < number.Data.Count(); i++)
            {
                sum += number.Data[i] * Weights[i];
            }
            return sum >= THRESHHOLD ? 1 : 0;
        }
    }
}

//public void train(String file)
//{
//    try
//    {
//        boolean error = true;
//        int iterationCount = 0;
//        int totalLineNumber = 0;
//        while (error)
//        {
//            iterationCount++;
//            error = false;
//            java.io.BufferedReader FileReader =                      //ein Reader um die Datei Zeilenweise auszulesen
//                    new java.io.BufferedReader(
//                        new java.io.FileReader(
//                            new java.io.File(file)
//                        )
//                    );

//            int truePositives = 0;
//            int trueNegatives = 0;
//            int falsePositives = 0;
//            int falseNegatives = 0;

//            String zeile = "";
//            while (null != (zeile = FileReader.readLine()))
//            {         //lesen jeder Zeile  
//                totalLineNumber++;
//                String[] line = zeile.split(",");

//                int[] numbers = new int[line.length];
//                for (int i = 0; i < line.length; i++) numbers[i] = Integer.parseInt(line[i]);

//                String lastTested;

//                boolean test = testLine(numbers);
//                //If false negative, increment weights
//                if (!test && testedValue == numbers[0])
//                {
//                    incrementW(numbers);
//                    error = true;
//                    falseNegatives++;
//                    lastTested = "FN";
//                }
//                //If false positive, decrement weights
//                else if (test && testedValue != numbers[0])
//                {
//                    decrementW(numbers);
//                    error = true;
//                    falsePositives++;
//                    lastTested = "FP";
//                }
//                else if (test && testedValue == numbers[0])
//                {
//                    truePositives++;
//                    lastTested = "TP";
//                }
//                else
//                {
//                    trueNegatives++;
//                    lastTested = "TN";
//                }

//                if (totalLineNumber % 1015 == 0 && DEBUG)
//                {
//                    System.out.println("Total Line number " + totalLineNumber);
//                    System.out.println("First Value in Line is " + numbers[0] + ", the test was " + test + ", and it was recognised as " + lastTested);
//                }

//            }

//            if (DEBUG)
//            {
//                System.out.println("True positives: " + truePositives);
//                System.out.println("True negatives: " + trueNegatives);
//                System.out.println("False positives: " + falsePositives);
//                System.out.println("False negatives: " + falseNegatives);
//            }
//            double falseResults = 100 * (falseNegatives + falsePositives);
//            falseResults /= (truePositives + trueNegatives + falsePositives + falseNegatives);

//            System.out.println("Iteration number " + iterationCount + " for tested Value " + testedValue + ", false results: " + falseResults + "%");

//            if (!error)
//            {
//                System.out.println("All results have been assigned correctly. Program terminates loop now.");
//            }
//            else if (iterationCount == maxIterationCount)
//            {
//                System.out.println(maxIterationCount + " iterations reached. Program terminates loop now.");
//                error = false;
//            }
//        }

//    }
//    catch (Exception e)
//    {
//        e.printStackTrace();
//    }
//}