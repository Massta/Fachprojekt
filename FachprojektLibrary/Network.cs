namespace FachprojektLibrary
{
    public class Network
    {
        public ILayer[] _layers { get; set; }
        public Network(params ILayer[] layers)
        {
            _layers = layers;
        }
        public double[] GetOutputs(double[] inputs)
        {
            double[] oldOutputs = _layers[0].GetOutputs(inputs);
            for (int i = 1; i < _layers.Length; i++)
            {
                oldOutputs = _layers[i].GetOutputs(oldOutputs);
            }
            return oldOutputs;
        }

        public void AdjustWeights(double label, double guess, double[] outputs)
        {
            var topLayer = _layers[_layers.Length - 1];

            double[] topError = new double[topLayer.NeuronAmount];
            for (int i = 0; i < topLayer.NeuronAmount; i++)
            {
                double y = i == label ? 1 : 0;
                double x = outputs[i] > 0.5 ? 1 : 0;//i == guess ? 1 : 0;
                topError[i] = y - x;
            }
            topLayer.AdjustWeights(topError);
            for (int i = _layers.Length - 2; i >= 0; i--)
            {
                var currentLayer = _layers[i];
                var currentTopLayer = _layers[i + 1];

                if (typeof(FullyConnectedLayer) == currentLayer.GetType()
                    && typeof(FullyConnectedLayer) == currentTopLayer.GetType())
                {
                    double[] currentError = new double[currentLayer.NeuronAmount];
                    for (int e = 0; e < currentLayer.NeuronAmount; e++)
                    {
                        currentError[e] = currentTopLayer.GetWeightErrorSum(e);
                    }
                    currentLayer.AdjustWeights(currentError);
                }
                else if (typeof(ConvolutionalLayer) == currentLayer.GetType()
                   && typeof(FullyConnectedLayer) == currentTopLayer.GetType())
                {
                    double[] currentError = new double[currentLayer.NeuronAmount];
                    for (int e = 0; e < currentLayer.NeuronAmount; e++)
                    {
                        currentError[e] = currentTopLayer.GetWeightErrorSum(e);
                    }
                    currentLayer.AdjustWeights(currentError);
                }
                else if (typeof(ConvolutionalLayer) == currentLayer.GetType()
                   && typeof(ConvolutionalLayer) == currentTopLayer.GetType())
                {
                    //AdjustFilterWeights
                }
                else
                {
                    throw new System.Exception("Network has wrong order");
                }

            }
        }
    }
}