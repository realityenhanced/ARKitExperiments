#include <opencv2/opencv.hpp>
using namespace cv;

extern "C"
{
    int PerformOperation()
    {
        // TEST
        Mat myMat = Mat::eye(101, 21, CV_32F);
        return myMat.cols;
    }
}
