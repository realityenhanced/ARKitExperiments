#include "..\common.h"
#include <iostream>

using namespace cv;
using namespace std;

#define FUNCTION_TO_EXECUTE(a,b,c) MatchDescriptors(a,b,c);

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
            if (image.type() != CV_8UC3)
            {
                printf("Invalid mat");
                retVal = -1;
            }
            else
            {
                int result = FUNCTION_TO_EXECUTE(image.ptr(), image.cols, image.rows);
                cout << result << endl;
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

