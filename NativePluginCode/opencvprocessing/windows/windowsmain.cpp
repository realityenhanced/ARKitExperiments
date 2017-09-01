#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>

extern "C" int PerformOperation();

using namespace cv;
using namespace std;

int main(int argc, char** argv)
{
    cout << PerformOperation() << endl;
    return 0;
}

