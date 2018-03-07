using System;

namespace MNISTConvolutional
{
    public class FullyConnectedLayer : Layer
    {
        public float[] Input { get; set; }
        public Tensor Weights { get; set; }
        public float[] Gradients { get; set; }
        public float[] OldGradients { get; set; }
        public FullyConnectedLayer(int outSize, TdSize inSize)
        {
            Type = LayerType.FullyConnected;
            In = new Tensor(inSize.X, inSize.Y, inSize.Z);
            Out = new Tensor(outSize, 1, 1);
            GradsIn = new Tensor(inSize.X, inSize.Y, inSize.Z);
            Weights = new Tensor(inSize.X * inSize.Y * inSize.Z, outSize, 1);

            Input = new float[outSize];
            Gradients = new float[outSize];
            OldGradients = new float[outSize];

            int maxValue = inSize.X * inSize.Y * inSize.Z;
            for (int i = 0; i < outSize; i++)
            {
                for (int h = 0; h < maxValue; h++)
                {
                    Weights.Set(h, i, 0, 2.19722f / maxValue * Utilities.GetRandomFloat(0, 1));
                }
            }
        }

        public override void Activate(Tensor input)
        {
            In = input;
            Activate();
        }

        public override void Activate()
        {
            for (int n = 0; n < Out.Size.X; n++)
            {
                float inputV = 0;

                for (int i = 0; i < In.Size.X; i++)
                {
                    for (int j = 0; j < In.Size.Y; j++)
                    {
                        for (int z = 0; z < In.Size.Z; z++)
                        {
                            int m = Map(i, j, z);
                            inputV += In.Get(i, j, z) * Weights.Get(m, n, 0);
                        }
                    }
                }
                Input[n] = inputV;
                Out.Set(n, 0, 0, (float)Utilities.Sigmoid(inputV));
            }
        }

        public int Map(int x, int y, int z)
        {
            return z * (In.Size.X * In.Size.Y)
                + y * (In.Size.X)
                + x;
        }

        public override void FixWeights()
        {
            for (int n = 0; n < Out.Size.X; n++)
            {
                float gradient = Gradients[n];
                float oldGradient = OldGradients[n];
                for (int i = 0; i < In.Size.X; i++)
                {
                    for (int j = 0; j < In.Size.Y; j++)
                    {
                        for (int z = 0; z < In.Size.Z; z++)
                        {
                            int m = Map(i, j, z);
                            float w = Weights.Get(m, n, 0);
                            Weights.Set(m, n, 0, Utilities.UpdateWeight(w, gradient, oldGradient, In.Get(i, j, z)));
                        }
                    }
                }
                oldGradient = Utilities.UpdateGradient(gradient, oldGradient);
                OldGradients[n] = oldGradient;
            }
        }

        public override void CalculateGradients(Tensor nextLayerGradients)
        {
            for (int x = 0; x < GradsIn.Size.X; x++)
            {
                for (int y = 0; y < GradsIn.Size.Y; y++)
                {
                    for (int z = 0; z < GradsIn.Size.Z; z++)
                    {
                        GradsIn.Set(x, y, z, 0f);
                    }
                }
            }

            for (int n = 0; n < Out.Size.X; n++)
            {
                Gradients[n] = nextLayerGradients.Get(n, 0, 0) * (float)Utilities.SigmoidDerivative(Input[n]);
                for (int i = 0; i < In.Size.X; i++)
                {
                    for (int j = 0; j < In.Size.Y; j++)
                    {
                        for (int z = 0; z < In.Size.Z; z++)
                        {
                            int m = Map(i, j, z);
                            var oldGradIn = GradsIn.Get(i, j, z);
                            GradsIn.Set(i, j, z, oldGradIn + Gradients[n] * Weights.Get(m, n, 0));
                        }
                    }
                }
            }
        }
    }
}