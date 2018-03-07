using System;

namespace MNISTConvolutional
{
    public class PoolLayer : Layer
    {
        int Stride;
        int ExtendFilter;

        public PoolLayer(int stride, int extendFilter, TdSize inSize)
        {
            Stride = stride;
            ExtendFilter = extendFilter;

            GradsIn = new Tensor(inSize.X, inSize.Y, inSize.Z);
            In = new Tensor(inSize.X, inSize.Y, inSize.Z);
            Out = new Tensor((inSize.X - ExtendFilter) / Stride + 1,
                (inSize.Y - ExtendFilter) / Stride + 1,
                inSize.Z);
        }

        public TdSize MapToInput(TdSize output, int z)
        {
            return new TdSize
            {
                X = output.X * Stride,
                Y = output.Y * Stride,
                Z = output.Z
            };
        }

        public int NormalizeRange(float f, int max, bool lim_min)
        {
            if (f <= 0)
            {
                return 0;
            }
            max -= 1;
            if (f >= max)
            {
                return max;
            }
            if (lim_min) // left side of inequality
            {
                return (int)Math.Ceiling(f);
            }
            else
            {
                return (int)Math.Floor(f);
            }
        }

        public Range MapToOutput(int x, int y)
        {
            Range range = new Range()
            {
                MinX = NormalizeRange((x - ExtendFilter + 1) / Stride, Out.Size.X, true),
                MinY = NormalizeRange((y - ExtendFilter + 1) / Stride, Out.Size.Y, true),
                MinZ = 0,
                MaxX = NormalizeRange(x / Stride, Out.Size.X, false),
                MaxY = NormalizeRange(y / Stride, Out.Size.Y, false),
                MaxZ = (int)Out.Size.Z - 1,
            };
            return range;
        }

        public override void Activate(Tensor input)
        {
            In = input;
            Activate();
        }

        public override void Activate()
        {
            for (int x = 0; x < Out.Size.X; x++)
            {
                for (int y = 0; y < Out.Size.Y; y++)
                {
                    for (int z = 0; z < Out.Size.Z; z++)
                    {
                        TdSize val = new TdSize { X = x, Y = y, Z = 0 };
                        TdSize mapped = MapToInput(val, 0);
                        float mval = float.MinValue;
                        for (int i = 0; i < ExtendFilter; i++)
                        {
                            for (int j = 0; j < ExtendFilter; j++)
                            {
                                float v = In.Get(mapped.X + i, mapped.Y + j, z);
                                if (v > mval)
                                {
                                    mval = v;
                                }
                            }
                        }
                        Out.Set(x, y, z, mval);
                    }
                }
            }
        }

        public override void FixWeights()
        {

        }

        public override void CalculateGradients(Tensor nextLayerGradients)
        {
            for (int x = 0; x < In.Size.X; x++)
            {
                for (int y = 0; y < In.Size.Y; y++)
                {
                    Range range = MapToOutput(x, y);
                    for (int z = 0; z < In.Size.Z; z++)
                    {
                        float sum_error = 0;
                        for (int i = range.MinX; i <= range.MaxX; i++)
                        {
                            int minx = i * Stride;
                            for (int j = range.MinY; j <= range.MaxY; j++)
                            {
                                int miny = j * Stride;

                                int is_max = (In.Get(x, y, z) == Out.Get(i, j, z)) ? 1 : 0;
                                sum_error += is_max * nextLayerGradients.Get(i, j, z);
                            }
                        }
                        GradsIn.Set(x, y, z, sum_error);
                    }
                }
            }
        }
    }
}