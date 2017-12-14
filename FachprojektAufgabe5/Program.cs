using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FachprojektAufgabe5
{
    class Program
    {
        public const string PATH_TEST_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Test_Small_Grayscale";
        public const string PATH_TRAIN_SMALL_GRAY = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale";
        public const string PATH_TRAIN_SMALL_GRAY_FILTER1 = @"C:\Users\Julius Jacobsohn\Documents\Kaggle\Train_Small_Grayscale_Filter1";

        public static int[][,] ImageData;
        public static int[] Labels;
        public const double LEARN_RATE = 0.005;

        public const int FILTER1_AMOUNT = 32;
        public const int FILTER1_SIZE = 2;
        public const int FILTER1_STRIDE = 2;
        public static double[][,] Filters1;
        public static double[][,] Filters1Result;

        public const int FILTER2_AMOUNT = 16;
        public const int FILTER2_SIZE = 2;
        public const int FILTER2_STRIDE = 2;
        public static double[][][,] Filters2;
        public static double[][,] Filters2Result;
        public static double[][] Filter2Weights;

        public const int FILTER3_AMOUNT = 8;
        public const int FILTER3_SIZE = 2;
        public const int FILTER3_STRIDE = 2;
        public static double[][][,] Filters3;
        public static double[][,] Filters3Result;
        public static double[][] Filter3Weights;

        public const int LAYER_HIDDEN_SIZE = 300;
        public const int LAYER_HIDDEN_INPUT = 512;

        public const int LAYER_OUTPUT_SIZE = 2;
        public const int LAYER_OUTPUT_INPUT = 300;

        public static double[,] HiddenWeights; //300, 512
        public static double[] HiddenBias; //300
        public static double[] HiddenOutputs; //300
        public static double[] HiddenError; //300

        public static double[,] OutputWeights; //2, 300
        public static double[] OutputBias; //2
        public static double[] OutputOutputs; //2
        public static double[] OutputError; //2

        static void Main(string[] args)
        {
            //Load image data
            #region Convolution Init
            string csvPath = Path.Combine(PATH_TRAIN_SMALL_GRAY, "Train.csv");
            ImageData = Utilities.ReadCsv(csvPath, out Labels);

            //Load filters
            Filters1 = new double[FILTER1_AMOUNT][,];
            Filters1Result = new double[FILTER1_AMOUNT][,];
            for (int i = 0; i < FILTER1_AMOUNT; i++)
            {
                Filters1[i] = Utilities.GenerateWeightMatrix(FILTER1_SIZE, -0.5, 1);
            }

            Filters2 = new double[FILTER2_AMOUNT][][,];
            Filters2Result = new double[FILTER2_AMOUNT][,];
            Filter2Weights = new double[FILTER2_AMOUNT][];
            for (int i = 0; i < FILTER2_AMOUNT; i++)
            {
                Filters2[i] = new double[FILTER1_AMOUNT][,];
                Filter2Weights[i] = new double[FILTER1_AMOUNT];
                for (int j = 0; j < FILTER1_AMOUNT; j++)
                {
                    Filters2[i][j] = Utilities.GenerateWeightMatrix(FILTER2_SIZE, -0.5, 1);
                    Filter2Weights[i][j] = Utilities.GetRandomNumber(0, 2);
                }
            }

            Filters3 = new double[FILTER3_AMOUNT][][,];
            Filters3Result = new double[FILTER3_AMOUNT][,];
            Filter3Weights = new double[FILTER3_AMOUNT][];
            for (int i = 0; i < FILTER3_AMOUNT; i++)
            {
                Filters3[i] = new double[FILTER2_AMOUNT][,];
                Filter3Weights[i] = new double[FILTER2_AMOUNT];
                for (int j = 0; j < FILTER2_AMOUNT; j++)
                {
                    Filters3[i][j] = Utilities.GenerateWeightMatrix(FILTER3_SIZE, -0.5, 1);
                    Filter3Weights[i][j] = Utilities.GetRandomNumber(0, 2);
                }
            }

            #endregion

            #region FC Init
            HiddenWeights = Utilities.GenerateWeightMatrix(LAYER_HIDDEN_SIZE, LAYER_HIDDEN_INPUT, -0.5, 0.5);
            HiddenBias = new double[300];
            HiddenOutputs = new double[300];
            HiddenError = new double[300];

            OutputWeights = Utilities.GenerateWeightMatrix(LAYER_OUTPUT_SIZE, LAYER_OUTPUT_INPUT, -0.5, 0.5);
            OutputBias = new double[2];
            OutputOutputs = new double[2];
            OutputError = new double[2];
            #endregion

            for (int imageIndex = 0; imageIndex < ImageData.Count(); imageIndex++)
            {
                var image = ImageData[imageIndex];
                double[,] dst = new double[image.GetLength(0), image.GetLength(1)];
                Array.Copy(image, dst, image.Length);
                //PrintResult(dst, imageIndex, 0, -1);
                #region Convolution
                //Use filters 1
                for (int f = 0; f < FILTER1_AMOUNT; f++)
                {
                    var filter = Filters1[f];
                    Filters1Result[f] = new double[FILTER1_AMOUNT, FILTER1_AMOUNT];
                    for (int x = 0; x < image.GetLength(0); x += FILTER1_STRIDE)
                    {
                        for (int y = 0; y < image.GetLength(0); y += FILTER1_STRIDE)
                        {
                            Filters1Result[f][x / 2, y / 2] = image[x, y] * filter[0, 0]
                                + image[x + 1, y] * filter[1, 0]
                                + image[x, y + 1] * filter[0, 1]
                                + image[x + 1, y + 1] * filter[1, 1];
                        }
                    }
                    Utilities.PrintResult(Filters1Result[f], imageIndex, 1, f);
                }
                //Use filters 2
                for (int fti = 0; fti < FILTER2_AMOUNT; fti++) //Schleife um alle 16 Filter Tensoren
                {
                    var ft = Filters2[fti]; //Filtertensor = Filterwürfel
                    Filters2Result[fti] = new double[FILTER2_AMOUNT, FILTER2_AMOUNT]; //Jede der 16 Ergebnisscheiben ist 16x16 groß
                    for (int x = 0; x < Filters1Result.GetLength(0); x += FILTER2_STRIDE)
                    {
                        for (int y = 0; y < Filters1Result.GetLength(0); y += FILTER2_STRIDE)
                        {
                            for (int d = 0; d < 32; d++) //Schleife um alle 32 schichten im aktuellen Tensor
                            {
                                var filter = ft[d]; //Filter = 2x2 Filter an Stelle f im Filtertensor
                                var f1resultImage = Filters1Result[d]; //Bild an Stelle f
                                Filters2Result[fti][x / 2, y / 2] += (f1resultImage[x, y] * filter[0, 0]
                                    + f1resultImage[x + 1, y] * filter[1, 0]
                                    + f1resultImage[x, y + 1] * filter[0, 1]
                                    + f1resultImage[x + 1, y + 1] * filter[1, 1]) * (1d / 32d) * Filter2Weights[fti][d];
                            }
                        }
                    }
                    Utilities.PrintResult(Filters2Result[fti], imageIndex, 2, fti);
                }
                //Use filters 3
                for (int fti = 0; fti < FILTER3_AMOUNT; fti++) //Schleife um alle 8 Filter Tensoren
                {
                    var ft = Filters3[fti]; //Filtertensor = Filterwürfel
                    Filters3Result[fti] = new double[FILTER3_AMOUNT, FILTER3_AMOUNT]; //Jede der 8 Ergebnisscheiben ist 8x8 groß
                    for (int x = 0; x < Filters2Result.GetLength(0); x += FILTER3_STRIDE)
                    {
                        for (int y = 0; y < Filters2Result.GetLength(0); y += FILTER3_STRIDE)
                        {
                            for (int d = 0; d < 16; d++) //Schleife um alle 16 schichten im aktuellen Tensor
                            {
                                var filter = ft[d]; //Filter = 2x2 Filter an Stelle f im Filtertensor
                                var f2resultImage = Filters2Result[d]; //Bild an Stelle f
                                Filters3Result[fti][x / 2, y / 2] += (f2resultImage[x, y] * filter[0, 0]
                                    + f2resultImage[x + 1, y] * filter[1, 0]
                                    + f2resultImage[x, y + 1] * filter[0, 1]
                                    + f2resultImage[x + 1, y + 1] * filter[1, 1]) * (1d / 32d) * Filter3Weights[fti][d];
                            }
                        }
                    }
                    Utilities.PrintResult(Filters3Result[fti], imageIndex, 3, fti);
                }

                #endregion

                #region FC

                //<Forwardprop>
                //Berechne outputs des hidden layers
                for (int i = 0; i < LAYER_HIDDEN_SIZE; i++)
                {
                    double outputSum = 0;
                    int counter = 0;
                    for (int d = 0; d < FILTER3_AMOUNT; d++)
                    {
                        for (int x = 0; x < Filters3Result[d].GetLength(0); x++)
                        {
                            for (int y = 0; y < Filters3Result[d].GetLength(0); y++)
                            {
                                outputSum += HiddenWeights[i, counter] * ImageData[d][x, y];
                                counter++;
                            }
                        }
                    }
                    outputSum += HiddenBias[i];
                    HiddenOutputs[i] = Utilities.Sigmoid(outputSum);
                }

                //Berechne outputs des output layers
                for (int i = 0; i < LAYER_OUTPUT_SIZE; i++)
                {
                    double outputSum = 0;
                    for (int j = 0; j < LAYER_OUTPUT_INPUT; j++)
                    {
                        outputSum += OutputWeights[i, j] * HiddenOutputs[j];
                    }
                    outputSum += OutputBias[i];
                    OutputOutputs[i] = Utilities.Sigmoid(outputSum);
                }
                //</Forwardprop>
                int biggestIndex = -1;
                double biggestOutput = -1;
                for (int i = 0; i < LAYER_OUTPUT_SIZE; i++)
                {
                    if (OutputOutputs[i] > biggestOutput)
                    {
                        biggestIndex = i;
                        biggestOutput = OutputOutputs[i];
                    }
                }

                Console.WriteLine($"Geraten: {biggestIndex} ({biggestOutput}) Ergebnis: {Labels[imageIndex]}");
                #endregion

            }
        }
    }
}
