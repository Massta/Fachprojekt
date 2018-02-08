using System;

namespace HardcodedNetwork
{
    public class Program
    {
        public const double LEARN_RATE = 0.01;
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

        public static double[] Neuron1Level3 = new double[] { 0.1, -0.2, -0.1, 0, 0 };
        public static double[] Neuron2Level3 = new double[] { 0, 0.3, 0.4, -0.5, 0 };

        public static double Neuron1Level3Output;
        public static double Neuron2Level3Output;

        public static double[] Neuron1Level4 = new double[] { 0.1, -0.1, 0 };

        public static double Neuron1Level4Output;


        static void Main(string[] args)
        {
            #region Forwardpass

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
            for (int i = 0; i < Filter1Level1Result.Length; i += 2)
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

            //Fully Connected Level 3
            double outputSum1Level3 = 0;
            double outputSum2Level3 = 0;
            for (int i = 0; i < Neuron1Level3.Length - 1; i++)
            {
                outputSum1Level3 += fullyConnectInput[i] * Neuron1Level3[i];
                outputSum2Level3 += fullyConnectInput[i] * Neuron2Level3[i];
            }
            outputSum1Level3 += Neuron1Level3[Neuron1Level3.Length - 1];
            outputSum2Level3 += Neuron2Level3[Neuron2Level3.Length - 1];
            Neuron1Level3Output = Sigmoid(outputSum1Level3);
            Neuron2Level3Output = Sigmoid(outputSum2Level3);

            double[] fullyConnectInput2 = new double[]
            {
                Neuron1Level3Output,
                Neuron2Level3Output
            };

            //Fully Connected Level 4
            double outputSum1Level4 = 0;
            for (int i = 0; i < Neuron1Level4.Length - 1; i++)
            {
                outputSum1Level4 += fullyConnectInput2[i] * Neuron1Level4[i];
            }
            outputSum1Level4 += Neuron1Level4[Neuron1Level4.Length - 1];
            Neuron1Level4Output = Sigmoid(outputSum1Level4);

            #endregion

            Console.WriteLine("Output: " + Neuron1Level4Output);
            double wantedResult = 1;

            #region Backwardpass

            //Level 4 Fullyconnected
            double delta1Level4 = -(wantedResult - Neuron1Level4Output) * SigmoidDerivative(Neuron1Level4Output);
            double[] gradientsLevel4 = new double[]
            {
                delta1Level4 * fullyConnectInput2[0],
                delta1Level4 * fullyConnectInput2[1],
                delta1Level4 * 1,
            };
            for (int i = 0; i < Neuron1Level4.Length; i++)
            {
                Neuron1Level4[i] -= LEARN_RATE * gradientsLevel4[i];
            }

            //Level 3 Fullyconnected
            double delta1Level3 = (delta1Level4 * Neuron1Level4[0]) * SigmoidDerivative(Neuron1Level3Output);
            double delta2Level3 = (delta1Level4 * Neuron1Level4[1]) * SigmoidDerivative(Neuron2Level3Output);
            double[][] gradientsLevel3 = new double[][]
            {
                new double[]
                {
                    delta1Level3 * Pooling1Level1Result[0][0],
                    delta1Level3 * fullyConnectInput[1],
                    delta1Level3 * fullyConnectInput[2],
                    delta1Level3 * fullyConnectInput[3],
                    delta1Level3 * 1
                },
                new double[]
                {
                    delta2Level3 * fullyConnectInput[0],
                    delta2Level3 * fullyConnectInput[1],
                    delta2Level3 * fullyConnectInput[2],
                    delta2Level3 * fullyConnectInput[3],
                    delta2Level3 * 1
                },
            };
            for (int i = 0; i < Neuron1Level3.Length; i++)
            {
                Neuron1Level3[i] -= LEARN_RATE * gradientsLevel3[0][i];
                Neuron2Level3[i] -= LEARN_RATE * gradientsLevel3[1][i];
            }

            //Level 2 Convolutional
            double delta1Level2 = (delta1Level3 * Neuron1Level3[0] + delta2Level3 * Neuron2Level3[0])
                * ReLUDerivative(Filter1Level2Result);
            double delta2Level2 = (delta1Level3 * Neuron1Level3[1] + delta2Level3 * Neuron2Level3[1])
                * ReLUDerivative(Filter2Level2Result);
            double delta3Level2 = (delta1Level3 * Neuron1Level3[2] + delta2Level3 * Neuron2Level3[2])
                * ReLUDerivative(Filter3Level2Result);
            double delta4Level2 = (delta1Level3 * Neuron1Level3[3] + delta2Level3 * Neuron2Level3[3])
                * ReLUDerivative(Filter4Level2Result);

            double[][] gradientsLevel2 = new double[][]
            {
                new double[]
                {
                    delta1Level2 * Pooling1Level1Result[0][0],
                    delta1Level2 * Pooling1Level1Result[1][0],
                    delta1Level2 * Pooling1Level1Result[0][1],
                    delta1Level2 * Pooling1Level1Result[1][1],
                },
                new double[]
                {
                    delta2Level2 * Pooling1Level1Result[0][0],
                    delta2Level2 * Pooling1Level1Result[1][0],
                    delta2Level2 * Pooling1Level1Result[0][1],
                    delta2Level2 * Pooling1Level1Result[1][1],
                },
                new double[]
                {
                    delta3Level2 * Pooling2Level1Result[0][0],
                    delta3Level2 * Pooling2Level1Result[1][0],
                    delta3Level2 * Pooling2Level1Result[0][1],
                    delta3Level2 * Pooling2Level1Result[1][1],
                },
                new double[]
                {
                    delta4Level2 * Pooling2Level1Result[0][0],
                    delta4Level2 * Pooling2Level1Result[1][0],
                    delta4Level2 * Pooling2Level1Result[0][1],
                    delta4Level2 * Pooling2Level1Result[1][1],
                }
            };

            Filter1Level2[0][0] -= LEARN_RATE * gradientsLevel2[0][0];
            Filter1Level2[1][0] -= LEARN_RATE * gradientsLevel2[0][1];
            Filter1Level2[0][1] -= LEARN_RATE * gradientsLevel2[0][2];
            Filter1Level2[1][1] -= LEARN_RATE * gradientsLevel2[0][3];

            Filter2Level2[0][0] -= LEARN_RATE * gradientsLevel2[1][0];
            Filter2Level2[1][0] -= LEARN_RATE * gradientsLevel2[1][1];
            Filter2Level2[0][1] -= LEARN_RATE * gradientsLevel2[1][2];
            Filter2Level2[1][1] -= LEARN_RATE * gradientsLevel2[1][3];

            Filter3Level2[0][0] -= LEARN_RATE * gradientsLevel2[2][0];
            Filter3Level2[1][0] -= LEARN_RATE * gradientsLevel2[2][1];
            Filter3Level2[0][1] -= LEARN_RATE * gradientsLevel2[2][2];
            Filter3Level2[1][1] -= LEARN_RATE * gradientsLevel2[2][3];

            Filter4Level2[0][0] -= LEARN_RATE * gradientsLevel2[3][0];
            Filter4Level2[1][0] -= LEARN_RATE * gradientsLevel2[3][1];
            Filter4Level2[0][1] -= LEARN_RATE * gradientsLevel2[3][2];
            Filter4Level2[1][1] -= LEARN_RATE * gradientsLevel2[3][3];

            //Level 1 Convolutional
            double delta1Level1 = (delta1Level2 * Filter1Level2[0][0]
                + delta2Level2 * Filter2Level2[0][0]
                + delta3Level2 * Filter1Level2[0][0]
                + delta4Level2 * Filter2Level2[0][0])
                * ReLUDerivative(Filter1Level1Result);
            double delta2Level1 = (delta1Level3 * Neuron1Level3[1] + delta2Level3 * Neuron2Level3[1])
                * ReLUDerivative(Filter2Level2Result);

            double[][] gradientsLevel2 = new double[][]
            {
                new double[]
                {
                    delta1Level2 * Pooling1Level1Result[0][0],
                    delta1Level2 * Pooling1Level1Result[1][0],
                    delta1Level2 * Pooling1Level1Result[0][1],
                    delta1Level2 * Pooling1Level1Result[1][1],
                },
                new double[]
                {
                    delta2Level2 * Pooling1Level1Result[0][0],
                    delta2Level2 * Pooling1Level1Result[1][0],
                    delta2Level2 * Pooling1Level1Result[0][1],
                    delta2Level2 * Pooling1Level1Result[1][1],
                },
                new double[]
                {
                    delta3Level2 * Pooling2Level1Result[0][0],
                    delta3Level2 * Pooling2Level1Result[1][0],
                    delta3Level2 * Pooling2Level1Result[0][1],
                    delta3Level2 * Pooling2Level1Result[1][1],
                },
                new double[]
                {
                    delta4Level2 * Pooling2Level1Result[0][0],
                    delta4Level2 * Pooling2Level1Result[1][0],
                    delta4Level2 * Pooling2Level1Result[0][1],
                    delta4Level2 * Pooling2Level1Result[1][1],
                }
            };

            Filter1Level2[0][0] -= LEARN_RATE * gradientsLevel2[0][0];
            Filter1Level2[1][0] -= LEARN_RATE * gradientsLevel2[0][1];
            Filter1Level2[0][1] -= LEARN_RATE * gradientsLevel2[0][2];
            Filter1Level2[1][1] -= LEARN_RATE * gradientsLevel2[0][3];

            Filter2Level2[0][0] -= LEARN_RATE * gradientsLevel2[1][0];
            Filter2Level2[1][0] -= LEARN_RATE * gradientsLevel2[1][1];
            Filter2Level2[0][1] -= LEARN_RATE * gradientsLevel2[1][2];
            Filter2Level2[1][1] -= LEARN_RATE * gradientsLevel2[1][3];

            Filter3Level2[0][0] -= LEARN_RATE * gradientsLevel2[2][0];
            Filter3Level2[1][0] -= LEARN_RATE * gradientsLevel2[2][1];
            Filter3Level2[0][1] -= LEARN_RATE * gradientsLevel2[2][2];
            Filter3Level2[1][1] -= LEARN_RATE * gradientsLevel2[2][3];

            Filter4Level2[0][0] -= LEARN_RATE * gradientsLevel2[3][0];
            Filter4Level2[1][0] -= LEARN_RATE * gradientsLevel2[3][1];
            Filter4Level2[0][1] -= LEARN_RATE * gradientsLevel2[3][2];
            Filter4Level2[1][1] -= LEARN_RATE * gradientsLevel2[3][3];

            #endregion
        }

        public static double Sigmoid(double x, double delta = 1)
        {
            return 1.0 / (1.0 + Math.Exp(-delta * x));
        }

        public static double SigmoidDerivative(double x)
        {
            return x * (1 - x);
        }
        public static double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        public static double ReLUDerivative(double x)
        {
            if (x > 0)
            {
                return 1;
            }
            return 0;
        }
    }

}
