#include <opencv2/opencv.hpp>
using namespace cv;

#ifdef _CONSOLE 
void DebugDisplayImage(Mat& mat)
{
    namedWindow("Display window", WINDOW_KEEPRATIO);
    imshow("Display window", mat);
    waitKey(0);
}
#endif