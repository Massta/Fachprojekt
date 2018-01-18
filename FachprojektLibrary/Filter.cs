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
        public double[,] WeightMatrix;
        public double Error;
        public int Size;
        public int Stride;
        public Filter(int size, int stride,int inputSize, double min, double max)
        {
            Size = size;
            Stride = stride;
            Weights = Utilities.GenerateFilter(size, min, max);
            WeightMatrix = GetWeightMatrix(inputSize);
        }

        public double[,] GetWeightMatrix(int inputSize)
        {
            int matrixWidth = inputSize * inputSize;
            int matrixHeight = (inputSize * inputSize) / (Size * Size);
            double[,] outputMatrix = new double[matrixWidth, matrixHeight];
            int filterSkipCounter = 0;
            int filterSkipWidthCounter = 0;
            for (int i = 0; i < matrixHeight; i++)
            {
                if (i != 0 && i % (inputSize / Size) == 0)
                {
                    filterSkipCounter++;
                    filterSkipWidthCounter = 0;
                }
                int skipZeroes = filterSkipWidthCounter * Stride + /*+ Size * inputSize * (i * Stride / inputSize);*/filterSkipCounter * (Size * inputSize);
                for (int j = 0; j < Size; j++)
                {
                    for (int k = 0; k < Size; k++)
                    {
                        outputMatrix[skipZeroes + k + j * inputSize, i] = Weights[j, k];
                    }
                }
                filterSkipWidthCounter++;
            }
            return outputMatrix;
        }

        public double GetWeightError(int e)
        {
            return Error * Weights[e / Size, e % Size]; //Oh Gott
        }
    }
}
