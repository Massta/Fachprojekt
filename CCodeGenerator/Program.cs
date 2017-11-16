using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var trainNumbers = ReadNumbers(@"mnist_train.csv");
            for (int i = 0; i < 60000; i++)
            {
                string toAppend = "";
                var number = trainNumbers.ElementAt(i);
                for (int j = 0; j < number.Data.Count() + 1; j++)
                {
                    int outputValue = j == 0 ? number.Label : number.Data.ElementAt(j - 1);
                    if(outputValue != 0)
                    {
                        toAppend += $"data[{i}][{j}] = {outputValue};\n";
                    }
                }
                File.AppendAllText("coutput.txt", toAppend);
                Console.WriteLine(i);
            }
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
    }
}
