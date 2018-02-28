using System;
using System.Threading;

namespace HardcodedNetwork
{
    public class Program
    {
        public const double LEARN_RATE = 0.0001;
        public const double ERROR_MIN = 0.01;
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
            new double[]{Utilities.GetRandomDouble(-0.1,1),Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };

        public static double[][] Filter2Level1 = new double[][] {
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };

        public static double[][] Filter1Level1Result;
        public static double[][] Filter2Level1Result;

        public static double[][] Pooling1Level1Result;
        public static double[][] Pooling2Level1Result;

        public static double[][] Filter1Level2 = new double[][] {
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };

        public static double[][] Filter2Level2 = new double[][] {
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };
        public static double[][] Filter3Level2 = new double[][] {
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };

        public static double[][] Filter4Level2 = new double[][] {
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
            new double[]{ Utilities.GetRandomDouble(-0.1, 1), Utilities.GetRandomDouble(-0.1,1)},
        };

        public static double Filter1Level2Result;
        public static double Filter2Level2Result;
        public static double Filter3Level2Result;
        public static double Filter4Level2Result;

        public static double[] Neuron1Level3 = new double[] { Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), 0 };
        public static double[] Neuron2Level3 = new double[] { Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), 0 };

        public static double Neuron1Level3Output;
        public static double Neuron2Level3Output;

        public static double[] Neuron1Level4 = new double[] { Utilities.GetRandomDouble(-1, 1), Utilities.GetRandomDouble(-1, 1), 0 };

        public static double Neuron1Level4Output;


        static void Main(string[] args)
        {
            var images = Utilities.GetImages(1000000);
            double errorRate = 1;
            double wronglyGuessed = 0;
            int epoch = 0;
            while (errorRate > ERROR_MIN)
            {
                Console.WriteLine("=== Epoche " + epoch + " ===");
                for (int currentImageIndex = 0; currentImageIndex < images.Length; currentImageIndex++)
                {
                    var inputData = images[currentImageIndex];
                    var label = Utilities.HasSquare(inputData);
                    #region Forwardpass

                    //Filter Level 1
                    Filter1Level1Result = new double[4][];
                    Filter2Level1Result = new double[4][];
                    for (int i = 0; i < inputData.Length; i += 2)
                    {
                        Filter1Level1Result[i / 2] = new double[4];
                        Filter2Level1Result[i / 2] = new double[4];
                        for (int j = 0; j < inputData.Length; j += 2)
                        {
                            Filter1Level1Result[i / 2][j / 2] = inputData[i][j] * Filter1Level1[0][0]
                                + inputData[i + 1][j] * Filter1Level1[1][0]
                                + inputData[i][j + 1] * Filter1Level1[0][1]
                                + inputData[i + 1][j + 1] * Filter1Level1[1][1];
                            Filter2Level1Result[i / 2][j / 2] = inputData[i][j] * Filter2Level1[0][0]
                                + inputData[i + 1][j] * Filter2Level1[1][0]
                                + inputData[i][j + 1] * Filter2Level1[0][1]
                                + inputData[i + 1][j + 1] * Filter2Level1[1][1];
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
                        Filter1Level2Result/255,
                        Filter2Level2Result/255,
                        Filter3Level2Result/255,
                        Filter4Level2Result/255
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

                    if (label != (Neuron1Level4Output > 0.5))
                    {
                        wronglyGuessed++;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    errorRate = wronglyGuessed / (double)(currentImageIndex + 1);
                    Console.WriteLine($"[{currentImageIndex}/{images.Length}] Wanted: {label} Output: {Neuron1Level4Output > 0.5} ({Neuron1Level4Output.ToString("n2")} Errorrate: {errorRate}");
                    Console.ForegroundColor = ConsoleColor.White;

                    double wantedResult = Convert.ToDouble(label);

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
                            delta1Level3 * fullyConnectInput[0],
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
                        }
                    };
                    for (int i = 0; i < Neuron1Level3.Length; i++)
                    {
                        Neuron1Level3[i] -= LEARN_RATE * gradientsLevel3[0][i];
                        Neuron2Level3[i] -= LEARN_RATE * gradientsLevel3[1][i];
                    }

                    //Deltas für Davids kleine Lieblinge
                    double deltaConvolutionalResult1 = delta1Level3 * Neuron1Level3[0]
                        + delta2Level3 * Neuron2Level3[0];
                    double deltaConvolutionalResult2 = delta1Level3 * Neuron1Level3[1]
                        + delta2Level3 * Neuron2Level3[1];
                    double deltaConvolutionalResult3 = delta1Level3 * Neuron1Level3[2]
                        + delta2Level3 * Neuron2Level3[2];
                    double deltaConvolutionalResult4 = delta1Level3 * Neuron1Level3[3]
                        + delta2Level3 * Neuron2Level3[3];

                    //Level 2 Convolutional
                    double[][] deltaConvolutional1Level2 = new double[2][];
                    double[][] deltaConvolutional2Level2 = new double[2][];
                    double[][] deltaConvolutional3Level2 = new double[2][];
                    double[][] deltaConvolutional4Level2 = new double[2][];
                    for (int i = 0; i < 2; i++)
                    {
                        deltaConvolutional1Level2[i] = new double[2];
                        deltaConvolutional2Level2[i] = new double[2];
                        deltaConvolutional3Level2[i] = new double[2];
                        deltaConvolutional4Level2[i] = new double[2];
                        for (int j = 0; j < 2; j++)
                        {
                            deltaConvolutional1Level2[i][j] = Filter1Level2[i][j] * deltaConvolutionalResult1;
                            deltaConvolutional2Level2[i][j] = Filter2Level2[i][j] * deltaConvolutionalResult2;
                            deltaConvolutional3Level2[i][j] = Filter3Level2[i][j] * deltaConvolutionalResult3;
                            deltaConvolutional4Level2[i][j] = Filter4Level2[i][j] * deltaConvolutionalResult4;
                        }
                    }

                    double[][] deltaFilter1Level2 = new double[2][];
                    double[][] deltaFilter2Level2 = new double[2][];
                    double[][] deltaFilter3Level2 = new double[2][];
                    double[][] deltaFilter4Level2 = new double[2][];
                    for (int i = 0; i < 2; i++)
                    {
                        deltaFilter1Level2[i] = new double[2];
                        deltaFilter2Level2[i] = new double[2];
                        deltaFilter3Level2[i] = new double[2];
                        deltaFilter4Level2[i] = new double[2];
                        for (int j = 0; j < 2; j++)
                        {
                            deltaFilter1Level2[i][j] = Pooling1Level1Result[0][0] * deltaConvolutionalResult1;
                            deltaFilter2Level2[i][j] = Pooling1Level1Result[0][0] * deltaConvolutionalResult2;
                            deltaFilter3Level2[i][j] = Pooling2Level1Result[0][0] * deltaConvolutionalResult3;
                            deltaFilter4Level2[i][j] = Pooling2Level1Result[0][0] * deltaConvolutionalResult4;
                            Filter1Level2[i][j] -= LEARN_RATE * deltaFilter1Level2[i][j];
                            Filter2Level2[i][j] -= LEARN_RATE * deltaFilter2Level2[i][j];
                            Filter3Level2[i][j] -= LEARN_RATE * deltaFilter3Level2[i][j];
                            Filter4Level2[i][j] -= LEARN_RATE * deltaFilter4Level2[i][j];
                        }
                    }

                    //Level 1 Convolutional - Pooling
                    double[][] deltaPooling1Level1 = new double[4][];
                    double[][] deltaPooling2Level1 = new double[4][];
                    for (int i = 0; i < 4; i++)
                    {
                        deltaPooling1Level1[i] = new double[4];
                        deltaPooling2Level1[i] = new double[4];
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                for (int l = 0; l < 2; l++)
                                {
                                    deltaPooling1Level1[i * 2 + k][j * 2 + l] =
                                      deltaConvolutional1Level2[i][j] + deltaConvolutional2Level2[i][j];
                                    deltaPooling2Level1[i * 2 + k][j * 2 + l] =
                                      deltaConvolutional3Level2[i][j] + deltaConvolutional4Level2[i][j];
                                }
                            }
                        }
                    }

                    //Level 1 Convolutional

                    double[][] deltaFilter1Level1 = new double[2][];
                    double[][] deltaFilter2Level1 = new double[2][];
                    for (int i = 0; i < 2; i++)
                    {
                        deltaFilter1Level1[i] = new double[2];
                        deltaFilter2Level1[i] = new double[2];
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                for (int l = 0; l < 2; l++)
                                {
                                    deltaFilter1Level1[k][l] += deltaPooling1Level1[i][j] * inputData[i * 2 + k][j * 2 + l];
                                    deltaFilter2Level1[k][l] += deltaPooling2Level1[i][j] * inputData[i * 2 + k][j * 2 + l];
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            Filter1Level1[i][j] -= LEARN_RATE * deltaFilter1Level1[i][j];
                            Filter2Level1[i][j] -= LEARN_RATE * deltaFilter2Level1[i][j];
                        }
                    }

                    #endregion
                }
                wronglyGuessed = 0;
                epoch++;
                //Thread.Sleep(2000);
            }
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
