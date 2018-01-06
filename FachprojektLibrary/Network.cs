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

        public void AdjustWeights(double label, double guess)
        {
            var topLayer = _layers[_layers.Length - 1];

            double[] topError = new double[topLayer.NeuronAmount];
            for (int i = 0; i < topLayer.NeuronAmount; i++)
            {
                double y = i == label ? 1 : 0;
                double x = i == guess ? 1 : 0;
                topError[i] = y - x;
            }
            topLayer.AdjustWeights(topError);
            for (int i = _layers.Length - 2; i >= 0; i--)
            {
                var currentLayer = _layers[i];
                var currentTopLayer = _layers[i + 1];

                double[] currentError = new double[currentLayer.NeuronAmount];
                for (int e = 0; e < currentLayer.NeuronAmount; e++)
                {
                    currentError[e] = currentTopLayer.GetWeightErrorSum(e);
                }
                currentLayer.AdjustWeights(currentError);
            }
        }
    }
}