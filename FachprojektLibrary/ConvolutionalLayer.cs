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
        //Anzah der Filter
        public int NeuronAmount => Filters == null ? 0 : Filters.Length;

        //Größe des Filters nxn
        public int NeuronSize { get; set; }
        public int FilterStride { get; set; }
        public int InputSize { get; set; }
        public int ImageAmount { get; set; }
        public Filter[] Filters;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterAmount"></param>
        /// <param name="inputSize">Breite/Höhe des Bildes</param>
        /// <param name="learnRate"></param>
        public ConvolutionalLayer(int filterAmount, int inputSize, int imageAmount, double learnRate = 0.01)
        {
            NeuronSize = 4;
            FilterStride = 4;
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
            throw new NotImplementedException();
        }

        public double[] GetOutputs(double[] inputs)
        {
            //int width = (int)Math.Sqrt(inputs.Length);
            //int lengthNewPic = inputs.Length / (NeuronSize * NeuronSize);
            //double[] result = new double[lengthNewPic * FilterAmount];

            ////Für alle Filter
            //for (int f = 0; f < FilterAmount; f++)
            //{
            //    //Filter über Bild
            //    for (int i = 0; i < width; i += FilterStride)
            //    {
            //        for (int j = 0; j < width; j += FilterStride)
            //        {
            //            result[((i / 2) + width * (j / 2)) + lengthNewPic * f] = inputs[i + width * j] * Filters[f].Weights[0, 0];
            //            result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + 1 + width * j] * Filters[f].Weights[1, 0];
            //            result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + width * (j + 1)] * Filters[f].Weights[0, 1];
            //            result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + 1 + width * (j + 1)] * Filters[f].Weights[1, 1];
            //        }
            //    }
            //}
            int imageIndexSize = inputs.Length / ImageAmount;
            int outputImageSize = (InputSize * InputSize) / (NeuronSize * NeuronSize);
            double[] filterOutputs = new double[ImageAmount * NeuronAmount * outputImageSize];
            for (int a = 0; a < ImageAmount; a++)
            {
                var currentInputImage = inputs
                    .Skip(a * imageIndexSize)
                    .Take(imageIndexSize)
                    .ToArray();
                for (int i = 0; i < NeuronAmount; i++)
                {
                    var currentVector = Utilities.MatrixVectorMultiplication(Filters[i].WeightMatrix, currentInputImage);
                    for (int j = 0; j < currentVector.Length; j++)
                    {
                        filterOutputs[a * outputImageSize + i * currentVector.Length + j] = currentVector[j];
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
