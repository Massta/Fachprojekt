using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektLibrary
{
    public class ConvolutionalLayer : ILayer
    {
        public int NeuronAmount => throw new NotImplementedException();

        public int NeuronSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int FilterAmount { get; set; }
        public int FilterSize { get; set; }
        public int FilterStride { get; set; }
        public Filter[] Filters;
        public ConvolutionalLayer(int filterAmount, double learnRate = 0.01)
        {
            FilterAmount = filterAmount;
            FilterSize = 2;
            FilterStride = 2;
            Filters = new Filter[filterAmount];
            for (int i = 0; i < filterAmount; i++)
            {
                Filters[i] = new Filter(FilterSize, FilterStride, -1,1);
            }
        }

        public void AdjustWeights(double[] topError)
        {
            throw new NotImplementedException();
        }

        public double[] GetOutputs(double[] inputs)
        {
            int width = (int)Math.Sqrt(inputs.Length);
            int lengthNewPic = inputs.Length / 4;
            double[] result = new double[lengthNewPic*FilterAmount];
            
            //Für alle Filter
            for (int f = 0; f < FilterAmount; f++)
            {
                //Filter über Bild
                for (int i = 0; i < width; i += FilterStride)
                {
                    for (int j = 0; j < width; j += FilterStride)
                    {
                        result[((i / 2) + width * (j / 2)) + lengthNewPic * f] = inputs[i + width * j] * Filters[f].Weights[0, 0];
                        result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + 1 + width * j] * Filters[f].Weights[1, 0];
                        result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + width * (j + 1)] * Filters[f].Weights[0, 1];
                        result[((i / 2) + width * (j / 2)) + lengthNewPic * f] += inputs[i + 1 + width * (j + 1)] * Filters[f].Weights[1, 1];
                    }
                }
            }

            //als double[] ausgeben
            return result;
        }

        public double GetWeightErrorSum(int e)
        {
            return Filters.Sum(n => n.GetWeightError(e));
        }
    }
}
