using System;
using System.Linq;

namespace MNISTConvolutional
{
    public class Tensor
    {
        public float[] Data { get; set; }
        public bool[] BoolData => Data.Select(d => d == 1.0 ? true : false).ToArray();
        public TdSize Size { get; set; }
        public int FullSize => Size.X * Size.Y * Size.Z;
        public Tensor(int x, int y, int z)
        {
            Size = new TdSize
            {
                X = x,
                Y = y,
                Z = z
            };
            Data = new float[FullSize];
        }

        public Tensor(Tensor t)
        {
            Data = t.Data;
            Size = t.Size;
        }

        public Tensor(float[][][] data)
        {
            Size = new TdSize
            {
                Z = data.Length,
                Y = data[0].Length,
                X = data[0][0].Length
            };
            int counter = 0;
            for (int i = 0; i < Size.X; i++)
            {
                for (int j = 0; j < Size.Y; j++)
                {
                    for (int k = 0; k < Size.Z; k++)
                    {
                        Data[counter] = data[k][j][i];
                        counter++;
                    }
                }
            }
        }

        public Tensor Add(Tensor t)
        {
            if (t.FullSize != FullSize)
            {
                throw new System.Exception("No compatible sizes: " + t.FullSize + ", " + FullSize);
            }
            Tensor output = new Tensor(Size.X, Size.Y, Size.Z);
            for (int i = 0; i < t.FullSize; i++)
            {
                output.Data[i] = Data[i] + t.Data[i];
            }
            return output;
        }

        public Tensor Substract(Tensor t)
        {
            if (t.FullSize != FullSize)
            {
                throw new System.Exception("No compatible sizes: " + t.FullSize + ", " + FullSize);
            }
            Tensor output = new Tensor(Size.X, Size.Y, Size.Z);
            for (int i = 0; i < t.FullSize; i++)
            {
                output.Data[i] = Data[i] - t.Data[i];
            }
            return output;
        }

        public float Get(int x, int y, int z)
        {
            return Data[z * (Size.X * Size.Y) + y * (Size.X) + x];
        }
        public void Set(int x, int y, int z, float value)
        {
            Data[z * (Size.X * Size.Y) + y * (Size.X) + x] = value;
        }
    }

    public class TdSize
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}