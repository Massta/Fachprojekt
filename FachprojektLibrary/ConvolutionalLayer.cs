using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FachprojektLibrary
{
    public class ConvolutionalLayer : ILayer
    {
        public const int FILTER_SIZE = 2;
        //Anzah der Filter
        public int NeuronAmount => Filters == null ? 0 : Filters.Length;
        public int ImageAmountOutput => ImageAmount * NeuronAmount;
        public double LearnRate { get; set; }

        //Größe des Filters nxn
        public int NeuronSize { get; set; }
        public int FilterStride { get; set; }
        public int InputSize { get; set; }
        public int ImageAmount { get; set; }
        public double[][][] ImageDeltas { get; set; }
        public double[][][] FilterDeltas { get; set; }
        public double[][][] LastInputs { get; set; }
        public Filter[] Filters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterAmount"></param>
        /// <param name="inputSize">Breite/Höhe des Bildes</param>
        /// <param name="learnRate"></param>
        public ConvolutionalLayer(int filterAmount, int inputSize, int imageAmount, double learnRate = 0.01)
        {
            LearnRate = learnRate;
            NeuronSize = FILTER_SIZE;
            FilterStride = FILTER_SIZE;
            InputSize = inputSize;
            ImageAmount = imageAmount;
            Filters = new Filter[filterAmount];
            for (int i = 0; i < filterAmount; i++)
            {
                Filters[i] = new Filter(NeuronSize, FilterStride, inputSize, -1, 1);
            }
        }

        public void AdjustWeights(double[] topError)
        {
            //ImageDeltas schreiben
            ImageDeltas = new double[ImageAmount][][];
            for (int image = 0; image < ImageAmount; image++)
            {
                ImageDeltas[image] = new double[InputSize][];
                for (int i = 0; i < InputSize; i++)
                {
                    ImageDeltas[image][i] = new double[InputSize];
                    for (int j = 0; j < InputSize; j++)
                    {
                        for (int f = 0; f < NeuronAmount; f++)
                        {
                            ImageDeltas[image][i][j] += Filters[f].Weights[i, j] * topError[image * NeuronAmount + f];
                        }
                    }
                }
            }

            //FilterDeltas schreiben
            FilterDeltas = new double[NeuronAmount][][];
            for (int filter = 0; filter < NeuronAmount; filter++)
            {
                FilterDeltas[filter] = new double[FILTER_SIZE][];
                for (int i = 0; i < FILTER_SIZE; i++)
                {
                    FilterDeltas[filter][i] = new double[FILTER_SIZE];
                    for (int j = 0; j < FILTER_SIZE; j++)
                    {
                        for (int image = 0; image < InputSize; image++)
                        {
                            FilterDeltas[filter][i][j] += LastInputs[image][i][j] * topError[filter * NeuronAmount + image];
                        }
                    }
                }
            }

            for (int filter = 0; filter < NeuronAmount; filter++)
            {
                for (int i = 0; i < FILTER_SIZE; i++)
                {
                    for (int j = 0; j < FILTER_SIZE; j++)
                    {
                        Filters[filter].Weights[i, j] += LearnRate * FilterDeltas[filter][i][j];
                    }
                }
            }
        }

        public void AdjustFilterWeights(double[][][] topDeltas)
        {
            double[][][] dx = new double[ImageAmount][][];
            for (int i = 0; i < ImageAmount; i++)
            {
                dx[i] = new double[InputSize][];
                for (int j = 0; j < InputSize; j++)
                {
                    dx[i][j] = new double[InputSize];
                }
            }
            double[][][] dw = new double[NeuronAmount][][];
            for (int i = 0; i < NeuronAmount; i++)
            {
                dw[i] = new double[FILTER_SIZE][];
                for (int j = 0; j < FILTER_SIZE; j++)
                {
                    dw[i][j] = new double[FILTER_SIZE];
                }
            }

            for (int image = 0; image < ImageAmount; image++)
            {
                for (int filter = 0; filter < NeuronAmount; filter++)
                {
                    for (int i = 0; i < topDeltas[0].Length; i++)
                    {
                        for (int j = 0; j < topDeltas[0][0].Length; j++)
                        {
                            for (int k = 0; k < FILTER_SIZE; k++)
                            {
                                for (int l = 0; l < FILTER_SIZE; l++)
                                {
                                    dx[image][i * FILTER_SIZE + k][j * FILTER_SIZE + l] += topDeltas[image * NeuronAmount + filter][i][j] * Filters[filter].Weights[k, l];
                                    double currentTopDelta = topDeltas[image * NeuronAmount + filter][i][j];
                                    double currentLastInput = LastInputs[image][i * FILTER_SIZE/*topDeltas[0].Length*/ + k][j * FILTER_SIZE/*topdeltas.length*/ + l];
                                    dw[filter][k][l] +=
                                        currentTopDelta
                                        * currentLastInput;
                                }
                            }
                        }
                    }
                }
            }

            ImageDeltas = dx;

            for (int filter = 0; filter < NeuronAmount; filter++)
            {
                for (int i = 0; i < FILTER_SIZE; i++)
                {
                    for (int j = 0; j < FILTER_SIZE; j++)
                    {
                        Filters[filter].Weights[i, j] += LearnRate * dw[filter][i][j];
                    }
                }
            }
        }

        public double[] GetOutputs(double[] inputs)
        {
            LastInputs = Utilities.To3DArray(inputs, ImageAmount, InputSize, InputSize);
            int imageIndexSize = inputs.Length / ImageAmount;
            int outputImageSize = (InputSize * InputSize) / (NeuronSize * NeuronSize);
            int outputImageWidth = InputSize / NeuronSize;
            double[] filterOutputs = new double[ImageAmount * NeuronAmount * outputImageSize];
            for (int image = 0; image < ImageAmount; image++)
            {
                var currentInputImage = inputs
                    .Skip(image * imageIndexSize)
                    .Take(imageIndexSize)
                    .ToArray();
                double[][] currentInputImageMatrix = Utilities.To3DArray(currentInputImage, 1, InputSize, InputSize)[0];
                for (int filter = 0; filter < NeuronAmount; filter++)
                {
                    //var currentVector = Utilities.MatrixVectorMultiplication(Filters[filter].WeightMatrix, currentInputImage);
                    double[] currentVector = new double[outputImageSize];
                    double[][] currentMatrix = new double[outputImageWidth][];
                    for (int i = 0; i < outputImageWidth; i++)
                    {
                        currentMatrix[i] = new double[outputImageWidth];
                    }
                    var currentFilter = Filters[filter];
                    for (int i = 0; i < currentInputImageMatrix.Length; i += FilterStride)
                    {
                        for (int j = 0; j < currentInputImageMatrix.Length; j += FilterStride)
                        {
                            for (int fi = 0; fi < FILTER_SIZE; fi++)
                            {
                                for (int fj = 0; fj < FILTER_SIZE; fj++)
                                {
                                    currentMatrix[i / FILTER_SIZE][j / FILTER_SIZE] += currentInputImageMatrix[i + fi][j + fj] * currentFilter.Weights[fi, fj];
                                }
                            }
                        }
                    }
                    for (int i = 0; i < outputImageWidth; i++)
                    {
                        for (int j = 0; j < outputImageWidth; j++)
                        {
                            currentVector[i * outputImageWidth + j] = currentMatrix[i][j];
                        }
                    }
                    for (int j = 0; j < currentVector.Length; j++)
                    {
                        filterOutputs[image * outputImageSize * NeuronAmount + filter * currentVector.Length + j] = currentVector[j];
                    }
                }
            }


            //als double[] ausgeben
            return filterOutputs;
        }

        public double GetWeightErrorSum(int e)
        {
            return Filters.Sum(n => n.GetWeightError(e));
        }
    }
}
