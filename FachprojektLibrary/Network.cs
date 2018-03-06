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

        public void AdjustWeights(double[] topError)
        {
            var topLayer = _layers[_layers.Length - 1];

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
                    var imageOutAmount = ((ConvolutionalLayer)currentLayer).ImageAmountOutput;
                    double[] currentError = new double[imageOutAmount];
                    for (int e = 0; e < imageOutAmount; e++)
                    {
                        currentError[e] = currentTopLayer.GetWeightErrorSum(e);
                    }
                    currentLayer.AdjustWeights(currentError);
                }
                else if (typeof(ConvolutionalLayer) == currentLayer.GetType()
                   && typeof(ConvolutionalLayer) == currentTopLayer.GetType())
                {
                    //AdjustFilterWeights
                    currentLayer.AdjustFilterWeights(((ConvolutionalLayer)currentTopLayer).ImageDeltas);
                }
                else
                {
                    throw new System.Exception("Network has wrong order");
                }

            }
        }
    }
}