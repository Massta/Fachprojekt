using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektLibrary
{
    public class Filter
    {
        public double[,] Weights;
        public double Error;
        public int Size;
        public int Stride;
        public Filter(int size, int stride, double min, double max)
        {
            Size = size;
            Stride = stride;
            Weights = Utilities.GenerateFilter(size, min, max);
        }

        public double GetWeightError(int e)
        {
            return Error * Weights[e/Size,e%Size]; //Oh Gott
        }
    }
}
