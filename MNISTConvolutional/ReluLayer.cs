namespace MNISTConvolutional
{
    internal class ReluLayer : Layer
    {

        public ReluLayer(TdSize inSize)
        {
            Type = LayerType.ReLU;
            In = new Tensor(inSize.X, inSize.Y, inSize.Z);
            Out = new Tensor(inSize.X, inSize.Y, inSize.Z);
            GradsIn = new Tensor(inSize.X, inSize.Y, inSize.Z);
        }


        public override void Activate(Tensor input)
        {
            In = input;
            Activate();
        }

        public override void Activate()
        {
            for (int i = 0; i < In.Size.X; i++)
            {
                for (int j = 0; j < In.Size.Y; j++)
                {
                    for (int z = 0; z < In.Size.Z; z++)
                    {
                        float v = In.Get(i, j, z);
                        if (v < 0)
                        {
                            v = 0;
                        }
                        Out.Set(i, j, z, v);
                    }
                }
            }
        }

        public override void FixWeights()
        {

        }

        public override void CalculateGradients(Tensor nextLayerGradients)
        {
            for (int i = 0; i < In.Size.X; i++)
            {
                for (int j = 0; j < In.Size.Y; j++)
                {
                    for (int z = 0; z < In.Size.Z; z++)
                    {
                        float value = (In.Get(i, j, z) < 0) ? 0 : nextLayerGradients.Get(i, j, z);
                        GradsIn.Set(i, j, z, value);
                    }
                }
            }
        }
    }
}