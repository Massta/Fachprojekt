namespace FachprojektLibrary
{
    public interface ILayer
    {
        int NeuronAmount { get; }
        int NeuronSize { get; set; }
        double[] GetOutputs(double[] inputs);

        void AdjustWeights(double[] topError);
        double GetWeightErrorSum(int e);
    }
}