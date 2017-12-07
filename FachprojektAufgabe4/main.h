int data[785];
int amountData = 150;

int dataDimension = 64 * 64;

double randfrom(double min, double max);
void read_CSV(const char * filename);
void train(const char *  filename);
void test(const char *  filename);