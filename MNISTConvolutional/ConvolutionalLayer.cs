using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNISTConvolutional
{
    public class ConvolutionalLayer : Layer
    {
        public int Stride { get; set; }
        public int ExtendFilter { get; set; }
        public Tensor[] Filters { get; set; }
        public Tensor[] Gradients { get; set; }
        public Tensor[] OldGradients { get; set; }
        public ConvolutionalLayer(int stride, int extendFilter, int numberFilters, TdSize inSize)
        {
            Type = LayerType.Convolutional;
            Filters = new Tensor[numberFilters];
            Gradients = new Tensor[numberFilters];
            OldGradients = new Tensor[numberFilters];
            GradsIn = new Tensor(inSize.X, inSize.Y, inSize.Z);
            In = new Tensor(inSize.X, inSize.Y, inSize.Z);
            Out = new Tensor((inSize.X - extendFilter) / stride + 1,
                (inSize.Y - extendFilter) / stride + 1,
                numberFilters);
            Stride = stride;
            ExtendFilter = extendFilter;

            for (int a = 0; a < numberFilters; a++)
            {
                Tensor t = new Tensor(ExtendFilter, ExtendFilter, inSize.Z);

                int maxval = ExtendFilter * ExtendFilter * inSize.Z;

                for (int i = 0; i < ExtendFilter; i++)
                {
                    for (int j = 0; j < ExtendFilter; j++)
                    {
                        for (int z = 0; z < inSize.Z; z++)
                        {
                            t.Set(i, j, z, 1.0f / maxval * Utilities.GetRandomFloat(0, 1));
                        }
                    }
                }
                Filters[a] = t;
            }
            for (int i = 0; i < numberFilters; i++)
            {
                Tensor gradient = new Tensor(ExtendFilter, ExtendFilter, inSize.Z);
                Tensor oldGradient = new Tensor(ExtendFilter, ExtendFilter, inSize.Z);
                Gradients[i] = gradient;
                OldGradients[i] = oldGradient;
            }
        }

        public TdSize MapToInput(TdSize outV, int z)
        {
            outV.X *= Stride;
            outV.Y *= Stride;
            outV.Z *= z;
            return outV;
        }

        public int NormalizeRange(float f, int max, bool limMin)
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
            if (limMin)
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
            return new Range
            {
                MinX = NormalizeRange((x - ExtendFilter + 1) / Stride, Out.Size.X, true), //TODO: Falsche Berechnung?
                MinY = NormalizeRange((y - ExtendFilter + 1) / Stride, Out.Size.Y, true),
                MinZ = 0,
                MaxX = NormalizeRange(x / Stride, Out.Size.X, false),
                MaxY = NormalizeRange(y / Stride, Out.Size.Y, false),
                MaxZ = Filters.Length - 1
            };
        }

        public override void Activate(Tensor tIn)
        {
            this.In = tIn;
            Activate();
        }

        public override void Activate()
        {
            for(int filter = 0; filter < Filters.Length; filter++)
            {
                Tensor currentFilter = Filters[filter];
                for (int x = 0; x < Out.Size.X; x++)
                {
                    for (int y = 0; y < Out.Size.Y; y++)
                    {
                        TdSize mapped = MapToInput(new TdSize {X = x, Y = y, Z = 0 }, 0);
                        float sum = 0;
                        for (int i = 0; i < ExtendFilter; i++)
                        {
                            for (int j = 0; j < ExtendFilter; j++)
                            {
                                for(int z = 0; z < In.Size.Z; z++)
                                {
                                    float f = currentFilter.Get(i, j, z);
                                    float v = In.Get(mapped.X + i, mapped.Y + j, z);
                                    sum += f * v;
                                }
                            }
                        }
                        Out.Set(x, y, filter, sum);
                    }
                }
            }
        }

        public override void FixWeights()
        {
            for(int a = 0; a < Filters.Length; a++)
            {
                for(int i = 0; i < ExtendFilter; i++)
                {
                    for (int j = 0; j < ExtendFilter; j++)
                    {
                        for(int z = 0; z < In.Size.Z; z++)
                        {
                            float w = Filters[a].Get(i, j, z);
                            float gradient = Gradients[a].Get(i, j, z);
                            float oldGradient = OldGradients[a].Get(i, j, z);
                            Filters[a].Set(i, j, z, Utilities.UpdateWeight(w, gradient, oldGradient));
                            oldGradient = Utilities.UpdateGradient(gradient, oldGradient);
                            OldGradients[a].Set(i, j, z, oldGradient);
                        }
                    }
                }
            }
        }

        public override void CalculateGradients(Tensor nextLayerGradients)
        {
            for(int k = 0; k < Gradients.Length; k++)
            {
                for(int i = 0; i < ExtendFilter; i++)
                {
                    for (int j = 0; j < ExtendFilter; j++)
                    {
                        for(int z = 0; z < In.Size.Z; z++)
                        {
                            Gradients[k].Set(i, j, z, 0);
                        }
                    }
                }
            }

            for(int x = 0; x < In.Size.X; x++)
            {
                for (int y = 0; y < In.Size.Y; y++)
                {
                    Range range = MapToOutput(x, y);
                    for (int z = 0; z < In.Size.Z; z++)
                    {
                        float sumError = 0;
                        for(int i = range.MinX; i <= range.MaxX; i++)
                        {
                            int minX = i * Stride;
                            for (int j = range.MinY; j <= range.MaxY; j++)
                            {
                                int minY = j * Stride;
                                for (int k = range.MinZ; k <= range.MaxZ; k++)
                                {
                                    int wApplied = (int)Filters[k].Get(x - minX, y - minY, z);
                                    sumError += wApplied * nextLayerGradients.Get(i, j, k);
                                    var oldValue = Gradients[k].Get(x - minX, y - minY, z);
                                    Gradients[k].Set(x - minX, y - minY, z, oldValue+In.Get(x, y, z));
                                }
                            }
                        }
                        GradsIn.Set(x, y, z, sumError);
                    }
                }
            }
        }
    }

    public class Range
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MinZ { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MaxZ { get; set; }
    }
}
