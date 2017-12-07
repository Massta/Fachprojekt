int data[64*64+1];
int amountData = 5000;

int dataDimension = 64 * 64;

double randfrom(double min, double max);
void train(const char *  filename);
void test(const char *  filename);