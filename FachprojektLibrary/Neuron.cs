namespace FachprojektLibrary
{
    public class Neuron
    {
        public int _size;
        public double[] _weights;
        public double _bias;

        private double _error;
        private double _lastOutput;
        private double[] _lastInputs;

        public double _learnRate;

        public bool UseReLu;

        public Neuron(int size, bool useReLu = false, double learnRate = 0.01)
        {
            _learnRate = learnRate;
            UseReLu = useReLu;
            _size = size;
            _weights = Utilities.GenerateWeights(size, -0.05, 0.05);
        }

        public double GetOutput(double[] inputs)
        {
            _lastInputs = inputs;
            double sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += _weights[i] * inputs[i];
            }
            sum += _bias;
            if (UseReLu)
            {
                _lastOutput = Utilities.ReLu(sum);
            }
            else
            {
                _lastOutput = Utilities.Sigmoid(sum);
            }
            return _lastOutput;
        }
        public void AdjustWeights(double topError)
        {
            if (UseReLu)
            {
                _error = Utilities.ReLuDerivative(_lastOutput) * topError;
            }
            else
            {
                _error = Utilities.SigmoidDerivative(_lastOutput) * topError;
            }
            for (int i = 0; i < _size; i++)
            {
                _weights[i] += _learnRate * _error * _lastInputs[i];
            }
            _bias += _learnRate * _error;
        }

        public double GetWeightError(int i)
        {
            return _error * _weights[i];
        }
    }
}