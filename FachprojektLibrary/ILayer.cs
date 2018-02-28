namespace FachprojektLibrary
{
    public interface ILayer
    {
        int NeuronAmount { get; }
        int NeuronSize { get; set; }
        double[] GetOutputs(double[] inputs);
        void AdjustWeights(double[] topError);
        void AdjustFilterWeights(double[][][] topDeltas);
        double GetWeightErrorSum(int e);
    }
}