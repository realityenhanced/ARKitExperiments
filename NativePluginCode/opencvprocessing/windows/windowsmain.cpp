#include <opencv2/opencv.hpp>
#include <iostream>

extern "C" int PerformOperation(uchar* imageBuffer, int numColumns, int numRows);

using namespace cv;
using namespace std;

int main(int argc, char** argv)
{
    int retVal = 0;
    if (argc == 2)
    {
        char* imageName = argv[1];
        Mat image = imread(imageName, IMREAD_COLOR);
        if (!image.data)
        {
            printf("ERROR: No image data.");
            retVal = -1;
        }
        else
        {
            namedWindow("Display window", WINDOW_KEEPRATIO);
            imshow("Display window", image);
            waitKey(0);

            if (image.type() != CV_8UC3)
            {
                printf("Invalid mat");
                retVal = -1;
            }
            else
            {
                cout << PerformOperation(image.ptr(), image.cols, image.rows) << endl;
            }
        }
    }
    else
    {
        printf("ERROR: Image path wasnt passed in.");
        retVal = -1;
    }

    return retVal;
}

