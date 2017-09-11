#include "..\common.h"
#include <iostream>
#include <memory>

using namespace cv;
using namespace std;

int main(int argc, char** argv)
{
    int retVal = 0;
    if (argc == 3)
    {
        Mat inputImage = imread(argv[1], IMREAD_COLOR);
        if (!inputImage.data)
        {
            printf("ERROR: No input image data.");
            retVal = -1;
        }
        else
        {
            if (inputImage.type() != CV_8UC3)
            {
                printf("Invalid input image mat");
                retVal = -1;
            }
            else
            {
                cout << "Saving descriptors.." << endl;
                int result = SaveDescriptors(inputImage.ptr(), inputImage.cols, inputImage.rows);
                cout << result << endl;

                Mat testImage = imread(argv[2], IMREAD_COLOR);
                if (!testImage.data)
                {
                    printf("ERROR: No test image data.");
                    retVal = -1;
                }
                else
                {
                    if (inputImage.type() != CV_8UC3)
                    {
                        printf("Invalid test image mat");
                        retVal = -1;
                    }
                    else
                    {
                        cout << "Matching descriptors.." << endl;
                        shared_ptr<int> boundingRect(new int[4], default_delete<int[]>());
                        int result = MatchDescriptors(testImage.ptr(), testImage.cols, testImage.rows, boundingRect.get());
                        cout << result << endl;
                        if (result == 1)
                        {
                            int* boundingRectPtr = boundingRect.get();
                            cout << "Boundary = " << boundingRectPtr[0] << " " << boundingRectPtr[1] << " " << boundingRectPtr[2] << " " << boundingRectPtr[3] << endl;
                        }
                    }
                }
            }
        }
    }
    else
    {
        printf("ERROR: Input & Test Image paths were not passed in.");
        retVal = -1;
    }

    return retVal;
}