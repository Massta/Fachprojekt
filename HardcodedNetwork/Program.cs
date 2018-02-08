using System;

namespace HardcodedNetwork
{
    public class Program
    {
        public static double[][] InputData = new double[][]{
            new double[]{ 10,10,0,20,30,20,30,10},
            new double[]{ 10,30,200, 200, 200, 200, 10,20},
            new double[]{ 10,20, 200, 10,30,200,0,0},
            new double[]{ 0,30,200,20,0,200,0,30},
            new double[]{ 20,40,200,200,200,200,10,0},
            new double[]{ 20,20,0,0,10,10,10,0},
            new double[]{ 10,0,10,30,20,20,0,0},
            new double[]{ 20,30,0,20,0,0,20,0},
        };
        public static double[][] Filter1Level1 = new double[][] {
            new double[]{-0.2,0.1},
            new double[]{0.3,0},
        };

        public static double[][] Filter2Level1 = new double[][] {
            new double[]{0.2,-0.3},
            new double[]{0,0.4},
        };

        public static double[][] Filter1Level1Result;
        public static double[][] Filter2Level1Result;

        public static double[][] Pooling1Level1Result;
        public static double[][] Pooling2Level1Result;

        public static double[][] Filter1Level2 = new double[][] {
            new double[]{0,0.1},
            new double[]{-0.2,0},
        };

        public static double[][] Filter2Level2 = new double[][] {
            new double[]{0.1,0.2},
            new double[]{0.3,0.4},
        };
        public static double[][] Filter3Level2 = new double[][] {
            new double[]{0,0.2},
            new double[]{-0.3,0.1},
        };

        public static double[][] Filter4Level2 = new double[][] {
            new double[]{-0.2,0.2},
            new double[]{0,0},
        };

        public static double Filter1Level2Result;
        public static double Filter2Level2Result;
        public static double Filter3Level2Result;
        public static double Filter4Level2Result;

        public static double[] Neuron1Level1 = new double[] { 0.1, -0.2, -0.1, 0, 0 };
        public static double[] Neuron2Level1 = new double[] { 0, 0.3, 0.4, -0.5, 0 };

        public static double Neuron1Level1Output;
        public static double Neuron2Level1Output;

        public static double[] Neuron1Level2 = new double[] { 0.1,-0.1,0 };

        public static double Neuron1Level2Output;


        static void Main(string[] args)
        {
            //Filter Level 1
            Filter1Level1Result = new double[4][];
            Filter2Level1Result = new double[4][];
            for (int i = 0; i < InputData.Length; i += 2)
            {
                Filter1Level1Result[i / 2] = new double[4];
                Filter2Level1Result[i / 2] = new double[4];
                for (int j = 0; j < InputData.Length; j += 2)
                {
                    Filter1Level1Result[i / 2][j / 2] = InputData[i][j] * Filter1Level1[0][0]
                        + InputData[i + 1][j] * Filter1Level1[1][0]
                        + InputData[i][j + 1] * Filter1Level1[0][1]
                        + InputData[i + 1][j + 1] * Filter1Level1[1][1];
                    Filter2Level1Result[i / 2][j / 2] = InputData[i][j] * Filter2Level1[0][0]
                        + InputData[i + 1][j] * Filter2Level1[1][0]
                        + InputData[i][j + 1] * Filter2Level1[0][1]
                        + InputData[i + 1][j + 1] * Filter2Level1[1][1];
                }
            }

            //ReLU Level 1
            for (int i = 0; i < Filter1Level1Result.Length; i++)
            {
                for (int j = 0; j < Filter1Level1Result.Length; j++)
                {
                    Filter1Level1Result[i][j] = Math.Max(Filter1Level1Result[i][j], 0);
                    Filter2Level1Result[i][j] = Math.Max(Filter2Level1Result[i][j], 0);
                }
            }

            //Pooling Level 1 (max)
            Pooling1Level1Result = new double[2][];
            Pooling2Level1Result = new double[2][];
            for (int i = 0; i < Filter1Level1Result.Length; i+=2)
            {
                Pooling1Level1Result[i / 2] = new double[2];
                Pooling2Level1Result[i / 2] = new double[2];
                for (int j = 0; j < Filter1Level1Result.Length; j += 2)
                {
                    Pooling1Level1Result[i / 2][j / 2] = Math.Max(
                        Math.Max(Filter1Level1Result[i][j],
                        Filter1Level1Result[i + 1][j]),
                        Math.Max(Filter1Level1Result[i][j + 1],
                        Filter1Level1Result[i + 1][j + 1])
                        );
                    Pooling2Level1Result[i / 2][j / 2] = Math.Max(
                        Math.Max(Filter2Level1Result[i][j],
                        Filter2Level1Result[i + 1][j]),
                        Math.Max(Filter2Level1Result[i][j + 1],
                        Filter2Level1Result[i + 1][j + 1])
                        );
                }
            }

            //Filter Level 2
            Filter1Level2Result = Pooling1Level1Result[0][0] * Filter1Level2[0][0]
                + Pooling1Level1Result[1][0] * Filter1Level2[1][0]
                + Pooling1Level1Result[0][1] * Filter1Level2[0][1]
                + Pooling1Level1Result[1][1] * Filter1Level2[1][1];
            Filter2Level2Result = Pooling1Level1Result[0][0] * Filter2Level2[0][0]
                + Pooling1Level1Result[1][0] * Filter2Level2[1][0]
                + Pooling1Level1Result[0][1] * Filter2Level2[0][1]
                + Pooling1Level1Result[1][1] * Filter2Level2[1][1];

            Filter3Level2Result = Pooling2Level1Result[0][0] * Filter3Level2[0][0]
                + Pooling2Level1Result[1][0] * Filter3Level2[1][0]
                + Pooling2Level1Result[0][1] * Filter3Level2[0][1]
                + Pooling2Level1Result[1][1] * Filter3Level2[1][1];
            Filter4Level2Result = Pooling2Level1Result[0][0] * Filter4Level2[0][0]
                + Pooling2Level1Result[1][0] * Filter4Level2[1][0]
                + Pooling2Level1Result[0][1] * Filter4Level2[0][1]
                + Pooling2Level1Result[1][1] * Filter4Level2[1][1];

            //ReLU Level 2
            Filter1Level2Result = Math.Max(Filter1Level2Result, 0);
            Filter2Level2Result = Math.Max(Filter2Level2Result, 0);
            Filter3Level2Result = Math.Max(Filter3Level2Result, 0);
            Filter4Level2Result = Math.Max(Filter4Level2Result, 0);

            double[] fullyConnectInput = new double[]
            {
                Filter1Level2Result,
                Filter2Level2Result,
                Filter3Level2Result,
                Filter4Level2Result
            };

            //Fully Connected Level 1
            double outputSum1Level1 = 0;
            double outputSum2Level1 = 0;
            for (int i = 0; i < Neuron1Level1.Length - 1; i++)
            {
                outputSum1Level1 += fullyConnectInput[i] * Neuron1Level1[i];
                outputSum2Level1 += fullyConnectInput[i] * Neuron2Level1[i];
            }
            outputSum1Level1 += Neuron1Level1[Neuron1Level1.Length - 1];
            outputSum2Level1 += Neuron2Level1[Neuron2Level1.Length - 1];
            Neuron1Level1Output = Sigmoid(outputSum1Level1);
            Neuron2Level1Output = Sigmoid(outputSum2Level1);

            double[] fullyConnectInput2 = new double[]
            {
                Neuron1Level1Output,
                Neuron2Level1Output
            };

            //Fully Connected Level 2
            double outputSum1Level2 = 0;
            for (int i = 0; i < Neuron1Level2.Length - 1; i++)
            {
                outputSum1Level2 += fullyConnectInput2[i] * Neuron1Level2[i];
            }
            outputSum1Level2 += Neuron1Level2[Neuron1Level2.Length - 1];
            Neuron1Level2Output = Sigmoid(outputSum1Level2);
        }

        public static double Sigmoid(double x, double delta = 1)
        {
            return 1.0 / (1.0 + Math.Exp(-delta * x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }
        public static double ReLu(double x)
        {
            return Math.Max(0, x);
        }
    }

}
