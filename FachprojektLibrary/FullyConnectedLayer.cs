using System.Linq;

namespace FachprojektLibrary
{
    public class FullyConnectedLayer : ILayer
    {
        public Neuron[] _neurons;
        public int NeuronAmount => _neurons == null ? 0 : _neurons.Length;
        public int NeuronSize { get; set; }
        public FullyConnectedLayer(int neuronAmount, int neuronSize, bool useRelu = false, double learnRate = 0.01)
        {
            NeuronSize = neuronSize;
            _neurons = new Neuron[neuronAmount];
            for (int i = 0; i < neuronAmount; i++)
            {
                _neurons[i] = new Neuron(neuronSize, useRelu, learnRate);
            }
        }

        public double[] GetOutputs(double[] inputs)
        {
            double[] outputs = new double[NeuronAmount];
            for (int i = 0; i < NeuronAmount; i++)
            {
                outputs[i] = _neurons[i].GetOutput(inputs);
            }
            return outputs;
        }

        public void AdjustWeights(double[] topError)
        {
            for (int i = 0; i < NeuronAmount; i++)
            {
                //if(topError[i] != 0)
                //{
                _neurons[i].AdjustWeights(topError[i]);
                //}
            }
        }

        public double GetWeightErrorSum(int i)
        {
            return _neurons.Sum(n => n.GetWeightError(i));
        }

        public void AdjustFilterWeights(double[][][] topDeltas)
        {
            throw new System.NotImplementedException("Don't use this in a fc layer");
        }
    }
}