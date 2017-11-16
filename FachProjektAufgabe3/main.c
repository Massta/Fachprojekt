#define _GNU_SOURCE

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <math.h>
#include "main.h"

#define ARRAYSIZE(x)  (sizeof(x)/sizeof(*(x)))

double hiddenWeights[300][784];
double hiddenBias[300];
double hiddenOutputs[300];
double hiddenError[300];

double outputWeights[10][300];
double outputBias[10];
double outputOutputs[10];
double outputError[10];

double LEARN_RATE = 0.01;

double sigmoid(double x)
{
	double exp_val = exp(-7 * x);

	return 1.0 / (1.0 + exp_val);
}

double derivative(double x)
{
	return x * (1 - x);
}

int main() {

	printf("Ich bin gaaaaanz am Anfang");


	srand((long)time(NULL));
	//fillData();
	//randomize weight
	for (int i = 0; i < 300; i++) {
		for (int j = 0; j < 784; j++) {
			hiddenWeights[i][j] = randfrom(-0.05, 0.05);
		}
	}
	for (int i = 0; i < 10; i++) {
		for (int j = 0; j < 300; j++) {
			outputWeights[i][j] = randfrom(-0.05, 0.05);
		}
	}

	printf("Ich bin gerade vor dem Aufruf von read_CSV");
	read_CSV("mnist_train_small.csv");
	printf("Ich bin gerade hinter dem Aufruf von read_CSV");
}

double randfrom(double min, double max)
{
	double range = (max - min);
	double div = RAND_MAX / range;
	return min + (rand() / div);
}


void read_CSV(const char *  filename)
{
	char buffer[2048];
	char *record, *line;

	printf("Ich bin gerade vor der ersten Schleife");

	int iteration = 0, zeileNummer = 0;
	while (iteration < 10)
	{
		printf("Iteration %i\n", iteration);

		FILE *fstream = fopen(filename, "r");
		if (fstream == NULL)
		{
			printf("\n file opening failed ");
		}
		else
		{
			//Datei wurde geoeffnet

			zeileNummer = 0;
			//Gehe durch alle Zeilen durch
			while ((line = fgets(buffer, sizeof(buffer), fstream)) != NULL)
			{
				//Lies Zeile ein
				record = strtok(line, ",");
				for (int j = 0; j < 785; j++)
				{
					data[j] = atoi(record);
					record = strtok(NULL, ",");
				}

				//Zeile wurde eingelesen

				//Zeile verarbeiten

				int label = data[0];

				//<Forwardprop>
				//Berechne outputs des hidden layers
				for (int i = 0; i < 300; i++) {
					double outputSum = 0;
					for (int j = 1; j < 785; j++)
					{
						outputSum += hiddenWeights[i][j - 1] * data[j];
					}
					outputSum /= 255;
					outputSum += hiddenBias[i];
					hiddenOutputs[i] = sigmoid(outputSum);
				}

				//Berechne outputs des output layers
				for (int i = 0; i < 10; i++) {
					double outputSum = 0;
					for (int j = 0; j < 300; j++)
					{
						outputSum += outputWeights[i][j] * hiddenOutputs[j];
					}
					outputSum;
					outputSum += outputBias[i];
					outputOutputs[i] = sigmoid(outputSum);
				}
				//</Forwardprop>

				//Finde gr??ten Wert
				int biggestIndex = -1;
				double biggestOutput = -1;
				for (int i = 0; i < 10; i++) {
					if (outputOutputs[i] > biggestOutput) {
						biggestIndex = i;
						biggestOutput = outputOutputs[i];
					}
				}

				printf("Zeile Nummer: %i, Gesuchte Zahl: %i, Geratene Zahl: %i\n", zeileNummer, label, biggestIndex);

				//<Backprop>

				//Output layer anpassen
				for (int i = 0; i < 10; i++)
				{
					int y = 0;
					if (label == i) {
						y = 1;
					}
					int x = 0;
					if (outputOutputs[i] > 0.5) {
						x = 1;
					}
					//Error berechnen
					outputError[i] = derivative(outputOutputs[i]) * (y - x);
					//Adjust weights
					for (int j = 0; j < 300; j++) {
						outputWeights[i][j] += LEARN_RATE * outputError[i] * hiddenOutputs[j];
					}
				}

				//Hidden layer anpassen
				for (int i = 0; i < 300; i++)
				{
					//Error berechnen
					double outputErrorSum = 0;
					for (int j = 0; j < 10; j++) {
						outputErrorSum += outputError[j] * outputWeights[j][i];
					}
					hiddenError[i] = derivative(hiddenOutputs[i]) * outputErrorSum;

					//Adjust weights
					for (int j = 1; j < 785; j++) {
						hiddenWeights[i][j - 1] += LEARN_RATE * hiddenError[i] * data[j];
					}
				}

				//</Backprop>

				zeileNummer++;
			}

			//Datei schlieﬂen
			fclose(fstream);
		}


		iteration++;
	}


}